import tkinter as tk
from tkinter import ttk
from typing import Optional, Dict, List, Any

from Global import gettext

import matplotlib

matplotlib.use('TkAgg')  # 使用 Tkinter 后端
import matplotlib.pyplot as plt
from gui.stats_display.data_processor import DataProcessor

# 配置 matplotlib 中文字体支持
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun', 'KaiTi', 'FangSong']
plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题

from Global import logger
from servers import (
    FrequencyServer,
    get_top_frequencies
)


class FrequencyTab(tk.Frame):
    """
    频率统计显示标签页

    用于显示属性值的频率分布
    """
    _current_prop_data: Optional[Dict[str, List[Any]]]
    _current_prop_name: str

    def __init__(self, parent: tk.Misc):
        super().__init__(parent)
        self.freq_server = FrequencyServer()
        self._create_widgets()

    def _create_widgets(self):
        """创建界面组件"""
        # 顶部控制区
        control_frame = ttk.Frame(self)
        control_frame.pack(fill='x', padx=5, pady=5)

        ttk.Label(control_frame, text=gettext("显示前")).pack(side='left')

        self.top_n_var = tk.IntVar(value=20)
        self.top_n_spinbox = ttk.Spinbox(
            control_frame, from_=5, to=100,
            textvariable=self.top_n_var, width=5
        )
        self.top_n_spinbox.pack(side='left', padx=5)

        ttk.Label(control_frame, text=gettext("项")).pack(side='left')

        self.refresh_btn = ttk.Button(control_frame, text=gettext("刷新"), command=self._on_refresh)
        self.refresh_btn.pack(side='left', padx=10)

        # 频率表格
        table_frame = ttk.Frame(self)
        table_frame.pack(fill='both', expand=True, padx=5, pady=5)

        # 创建 Treeview
        columns = ('value', 'count', 'percentage')
        self.tree = ttk.Treeview(table_frame, columns=columns, show='headings', height=15)

        self.tree.heading('value', text=gettext('值'))
        self.tree.heading('count', text=gettext('频数'))
        self.tree.heading('percentage', text=gettext('百分比'))

        self.tree.column('value', width=200)
        self.tree.column('count', width=80)
        self.tree.column('percentage', width=80)

        # 滚动条
        vsb = ttk.Scrollbar(table_frame, orient='vertical', command=self.tree.yview)
        self.tree.configure(yscrollcommand=vsb.set)

        self.tree.pack(side='left', fill='both', expand=True)
        vsb.pack(side='right', fill='y')

        # 底部统计摘要
        summary_frame = ttk.LabelFrame(self, text=gettext("统计摘要"))
        summary_frame.pack(fill='x', padx=5, pady=5)

        self.summary_text = tk.Text(summary_frame, height=4, wrap='word')
        self.summary_text.pack(fill='x', padx=5, pady=5)
        self.summary_text.configure(state='disabled')

        # 存储当前数据
        self._current_prop_data = None
        self._current_prop_name = ""

    def _on_refresh(self):
        """刷新按钮回调"""
        if self._current_prop_data:
            self.update_display(self._current_prop_data, self._current_prop_name)

    def update_display(self, prop_data: Optional[Dict[str, List[Any]]], prop_name: str):
        """
        更新频率统计显示

        Args:
            prop_data: 属性数据 { 类型名称 -> 值列表 }
            prop_name: 属性名称
        """
        # 存储当前数据
        self._current_prop_data = prop_data
        self._current_prop_name = prop_name

        # 清空表格
        self._clear_table()

        if not prop_data:
            self._set_summary(gettext("无数据"))
            return

        try:
            # 合并所有类型的值
            all_values: List[Any] = []
            for type_name, values in prop_data.items():
                if isinstance(values, list):
                    all_values.extend(values)

            if not all_values:
                self._set_summary(gettext("数据为空"))
                return

            # 使用 DataProcessor 过滤数据
            supported_values, unsupported_counts = DataProcessor.filter_supported(all_values)
            total_count = len(all_values)
            supported_count = len(supported_values)

            if not supported_values:
                unsupported_info = ", ".join([f"{k}: {v}" for k, v in unsupported_counts.items()])
                self._set_summary(
                    gettext("无可统计数据\n") +
                    f"{gettext('原始数据')}: {total_count} {gettext('项')}\n"
                    f"{gettext('不支持类型')}: {unsupported_info}\n\n"
                    f"{gettext('仅支持')}: str, bool, int, float"
                )
                return

            # 计算频率
            data_dict = {prop_name: supported_values}
            top_n = self.top_n_var.get()
            top_freq = get_top_frequencies(data_dict, top_n)

            if prop_name not in top_freq:
                self._set_summary(gettext("频率计算失败"))
                return

            # 填充表格
            for value, count in top_freq[prop_name]:
                # 格式化值显示
                value_str = self._format_value(value)
                percentage = (count / supported_count * 100) if supported_count > 0 else 0
                self.tree.insert('', 'end', values=(value_str, count, f"{percentage:.2f}%"))

            # 计算统计摘要
            freq_stats = self.freq_server.analyze(data_dict)
            stats = freq_stats.get('statistics', {}).get(prop_name, {})

            if stats:
                summary_lines = [
                    f"{gettext('可统计数据')}: {supported_count}/{total_count}",
                    f"{gettext('唯一值数量')}: {stats.get('unique_count', 0)}",
                    f"{gettext('最常见值')}: {self._format_value(stats.get('most_common'))} ({stats.get('most_common_count', 0)}{gettext('次')})",
                    f"{gettext('信息熵')}: {stats.get('entropy', 0):.4f}",
                ]
                if unsupported_counts:
                    unsupported_info = ", ".join([f"{k}: {v}" for k, v in unsupported_counts.items()])
                    summary_lines.append(f"{gettext('已排除')}: {unsupported_info}")
                self._set_summary('\n'.join(summary_lines))
            else:
                self._set_summary(gettext("统计摘要计算失败"))

        except Exception as e:
            error_msg = gettext("频率统计失败: {}").format(e)
            logger.error(error_msg, exc_info=True)
            self._set_summary(error_msg)

    def _format_value(self, value: Any) -> str:
        """格式化值显示"""
        if value is None:
            return "<None>"
        elif isinstance(value, str) and len(value) > 50:
            return value[:50] + "..."
        elif isinstance(value, bool):
            return "True" if value else "False"
        elif isinstance(value, (dict, list)):
            return str(type(value).__name__)
        else:
            return str(value)

    def _clear_table(self):
        """清空表格"""
        for item in self.tree.get_children():
            self.tree.delete(item)

    def _set_summary(self, content: str):
        """设置摘要内容"""
        self.summary_text.configure(state='normal')
        self.summary_text.delete('1.0', tk.END)
        self.summary_text.insert('1.0', content)
        self.summary_text.configure(state='disabled')
