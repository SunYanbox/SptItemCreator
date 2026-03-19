import tkinter as tk
from tkinter import ttk
from typing import Optional, Dict, List, Any

import matplotlib

matplotlib.use('TkAgg')  # 使用 Tkinter 后端
import matplotlib.pyplot as plt
from gui.stats_display.data_processor import DataType, DataProcessor, convert_to_numeric

# 配置 matplotlib 中文字体支持
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun', 'KaiTi', 'FangSong']
plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题

from Global import logger
from servers import (
    StatisticsServer,
    calculate_basic_statistics,
    calculate_extended_statistics
)


class StatsInfoTab(tk.Frame):
    """
    统计信息显示标签页

    用于显示数值属性的基本统计信息（均值、标准差、最大最小值等）
    """

    def __init__(self, parent: tk.Misc):
        super().__init__(parent)
        self._create_widgets()

    def _create_widgets(self):
        """创建界面组件"""
        # 主容器带滚动条
        self.container = ttk.Frame(self)
        self.container.pack(fill='both', expand=True)

        # 基本统计信息区域
        basic_frame = ttk.LabelFrame(self.container, text="基本统计")
        basic_frame.pack(fill='x', padx=5, pady=5)

        self.basic_text = tk.Text(basic_frame, height=10, wrap='word')
        self.basic_text.pack(fill='x', padx=5, pady=5)
        self.basic_text.configure(state='disabled')

        # 扩展统计信息区域
        extended_frame = ttk.LabelFrame(self.container, text="扩展统计")
        extended_frame.pack(fill='x', padx=5, pady=5)

        self.extended_text = tk.Text(extended_frame, height=6, wrap='word')
        self.extended_text.pack(fill='x', padx=5, pady=5)
        self.extended_text.configure(state='disabled')

        # 数据类型提示
        self.type_label = ttk.Label(self.container, text="")
        self.type_label.pack(fill='x', padx=5, pady=2)

        # 初始化服务器
        self.stats_server = StatisticsServer()

    def update_display(self, prop_data: Optional[Dict[str, List[Any]]], prop_name: str):
        """
        更新统计信息显示

        Args:
            prop_data: 属性数据 { 类型名称 -> 值列表 }
            prop_name: 属性名称
        """
        # 清空显示
        self._clear_display()

        if not prop_data:
            self._set_text(self.basic_text, f"属性 '{prop_name}' 无数据")
            return

        try:
            # 合并所有类型的值
            all_values: List[Any] = []
            for type_name, values in prop_data.items():
                if isinstance(values, list):
                    all_values.extend(values)

            if not all_values:
                self._set_text(self.basic_text, f"属性 '{prop_name}' 数据为空")
                return

            # 使用 DataProcessor 过滤数据
            supported_values, unsupported_counts = DataProcessor.filter_supported(all_values)

            # 构建类型标签
            total_count = len(all_values)
            supported_count = len(supported_values)

            if unsupported_counts:
                unsupported_info = ", ".join([f"{k}: {v}" for k, v in unsupported_counts.items()])
                self.type_label.configure(
                    text=f"可统计数据: {supported_count}/{total_count} (已排除: {unsupported_info})"
                )
                logger.debug(f"属性 '{prop_name}' 包含不可统计类型: {unsupported_info}")
            else:
                self.type_label.configure(text=f"数据总数: {total_count}")

            if not supported_values:
                self._set_text(self.basic_text,
                               f"属性 '{prop_name}' 无可统计数据\n\n"
                               f"仅支持以下类型的统计:\n"
                               f"  - 字符串 (str)\n"
                               f"  - 布尔值 (bool)\n"
                               f"  - 整数 (int)\n"
                               f"  - 浮点数 (float)\n\n"
                               f"检测到的类型:\n"
                               f"  {', '.join([f'{k}: {v}' for k, v in unsupported_counts.items()])}"
                               )
                return

            # 检测数据类型
            data_type = DataProcessor.detect_type(supported_values)

            if data_type == DataType.NUMERIC:
                self._display_numeric_stats(supported_values, prop_name)
            elif data_type == DataType.STRING:
                self._display_string_stats(supported_values, prop_name)
            elif data_type == DataType.BOOLEAN:
                self._display_boolean_stats(supported_values, prop_name)
            else:
                self._display_mixed_stats(supported_values, prop_name)

        except Exception as e:
            error_msg = f"统计计算失败: {e}"
            logger.error(error_msg, exc_info=True)
            self._set_text(self.basic_text, error_msg)

    def _display_numeric_stats(self, values: List[Any], prop_name: str):
        """显示数值型统计信息"""
        try:
            numeric_values = convert_to_numeric(values)

            if len(numeric_values) < 1:
                self._set_text(self.basic_text, "无法转换为数值数据")
                return

            # 计算基本统计
            basic_stats = calculate_basic_statistics(numeric_values)

            if not basic_stats:
                self._set_text(self.basic_text, "基本统计计算失败")
                return

            # 格式化基本统计显示
            basic_lines = [
                f"属性: {prop_name}",
                f"数据数量: {basic_stats.get('count', 0)}",
                f"均值: {basic_stats.get('mean', 0):.4f}",
                f"标准差: {basic_stats.get('std_dev', 0):.4f}",
                f"方差: {basic_stats.get('variance', 0):.4f}",
                f"最小值: {basic_stats.get('min', 0):.4f}",
                f"最大值: {basic_stats.get('max', 0):.4f}",
                f"中位数: {basic_stats.get('median', 0):.4f}",
                f"总和: {basic_stats.get('sum', 0):.4f}",
                f"四分位距 (IQR): {basic_stats.get('iqr', 0):.4f}",
            ]
            self._set_text(self.basic_text, '\n'.join(basic_lines))

            # 计算扩展统计
            if len(numeric_values) >= 3:
                extended_stats = calculate_extended_statistics(numeric_values)
                if extended_stats:
                    extended_lines = [
                        f"偏度: {extended_stats.get('skewness', 0):.4f}",
                        f"峰度: {extended_stats.get('kurtosis', 0):.4f}",
                        f"变异系数: {extended_stats.get('coefficient_of_variation', 0):.4f}",
                    ]
                    self._set_text(self.extended_text, '\n'.join(extended_lines))
                else:
                    self._set_text(self.extended_text, "扩展统计计算失败")
            else:
                self._set_text(self.extended_text, "数据不足，无法计算扩展统计（需要至少3个数据点）")

        except Exception as e:
            error_msg = f"数值统计计算异常: {e}"
            logger.error(error_msg, exc_info=True)
            self._set_text(self.basic_text, error_msg)

    def _display_string_stats(self, values: List[Any], prop_name: str):
        """显示字符串型统计信息"""
        try:
            str_values = [str(v) for v in values]
            unique_count = len(set(str_values))
            total_count = len(str_values)

            # 计算字符串长度统计
            lengths = [len(s) for s in str_values]
            avg_length = sum(lengths) / len(lengths) if lengths else 0
            max_length = max(lengths) if lengths else 0
            min_length = min(lengths) if lengths else 0

            lines = [
                f"属性: {prop_name}",
                f"数据数量: {total_count}",
                f"唯一值数量: {unique_count}",
                f"平均长度: {avg_length:.2f}",
                f"最大长度: {max_length}",
                f"最小长度: {min_length}",
                f"重复率: {(1 - unique_count / total_count) * 100:.2f}%" if total_count > 0 else "重复率: N/A",
            ]
            self._set_text(self.basic_text, '\n'.join(lines))
            self._set_text(self.extended_text, "字符串类型无扩展统计")

        except Exception as e:
            error_msg = f"字符串统计计算异常: {e}"
            logger.error(error_msg, exc_info=True)
            self._set_text(self.basic_text, error_msg)

    def _display_boolean_stats(self, values: List[Any], prop_name: str):
        """显示布尔型统计信息"""
        try:
            true_count = sum(1 for v in values if v is True)
            false_count = sum(1 for v in values if v is False)
            total_count = len(values)

            lines = [
                f"属性: {prop_name}",
                f"数据数量: {total_count}",
                f"True 数量: {true_count} ({true_count / total_count * 100:.2f}%)" if total_count > 0 else "True 数量: 0",
                f"False 数量: {false_count} ({false_count / total_count * 100:.2f}%)" if total_count > 0 else "False 数量: 0",
            ]
            self._set_text(self.basic_text, '\n'.join(lines))
            self._set_text(self.extended_text, "布尔类型无扩展统计")

        except Exception as e:
            error_msg = f"布尔统计计算异常: {e}"
            logger.error(error_msg, exc_info=True)
            self._set_text(self.basic_text, error_msg)

    def _display_mixed_stats(self, values: List[Any], prop_name: str):
        """显示混合型统计信息"""
        try:
            type_counts: Dict[str, int] = {}
            for v in values:
                type_name = type(v).__name__
                type_counts[type_name] = type_counts.get(type_name, 0) + 1

            lines = [
                f"属性: {prop_name}",
                f"数据数量: {len(values)}",
                f"类型分布:",
            ]
            for type_name, count in sorted(type_counts.items(), key=lambda x: x[1], reverse=True):
                lines.append(f"  - {type_name}: {count} ({count / len(values) * 100:.2f}%)")

            self._set_text(self.basic_text, '\n'.join(lines))
            self._set_text(self.extended_text, "混合类型无法计算扩展统计")

        except Exception as e:
            error_msg = f"混合统计计算异常: {e}"
            logger.error(error_msg, exc_info=True)
            self._set_text(self.basic_text, error_msg)

    def _set_text(self, text_widget: tk.Text, content: str):
        """设置文本框内容"""
        text_widget.configure(state='normal')
        text_widget.delete('1.0', tk.END)
        text_widget.insert('1.0', content)
        text_widget.configure(state='disabled')

    def _clear_display(self):
        """清空显示"""
        self._set_text(self.basic_text, "")
        self._set_text(self.extended_text, "")
        self.type_label.configure(text="")
