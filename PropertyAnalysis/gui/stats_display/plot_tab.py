import tkinter as tk
from tkinter import ttk
from typing import Optional, Dict, List, Any, Tuple

import gettext

import matplotlib
import numpy as np

matplotlib.use('TkAgg')  # 使用 Tkinter 后端
import matplotlib.pyplot as plt
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg, NavigationToolbar2Tk

from gui.stats_display.data_processor import DataType, DataProcessor

# 配置 matplotlib 中文字体支持
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun', 'KaiTi', 'FangSong']
plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题

from Global import logger
from servers import (
    get_top_frequencies, PlotServer
)


class PlotTab(tk.Frame):
    """
    统计图表显示标签页

    用于显示统计图表（柱状图、直方图等）
    """
    _current_prop_data: Optional[Dict[str, List[Any]]]
    _current_prop_name: str
    _filtered_values: List[Any]

    def __init__(self, parent: tk.Misc):
        super().__init__(parent)
        self.plot_server = PlotServer()
        self._current_figure = None
        self._current_canvas = None
        self._create_widgets()

    def _create_widgets(self):
        """创建界面组件"""
        # 顶部控制区
        control_frame = ttk.Frame(self)
        control_frame.pack(fill='x', padx=5, pady=5)

        ttk.Label(control_frame, text=gettext.gettext("图表类型:")).pack(side='left')

        self.plot_type_var = tk.StringVar(value='bar')
        self.plot_type_combo = ttk.Combobox(
            control_frame,
            textvariable=self.plot_type_var,
            values=['bar', 'histogram', 'pie'],
            state='readonly',
            width=12
        )
        self.plot_type_combo.pack(side='left', padx=5)

        # 柱状图/饼图显示数量
        ttk.Label(control_frame, text=gettext.gettext("显示项数:")).pack(side='left', padx=(10, 0))

        self.top_n_var = tk.IntVar(value=15)
        self.top_n_spinbox = ttk.Spinbox(
            control_frame, from_=5, to=50,
            textvariable=self.top_n_var, width=5
        )
        self.top_n_spinbox.pack(side='left', padx=5)

        self.draw_btn = ttk.Button(control_frame, text=gettext.gettext("绘制"), command=self._on_draw)
        self.draw_btn.pack(side='left', padx=10)

        # 图表显示区域
        self.plot_frame = ttk.Frame(self)
        self.plot_frame.pack(fill='both', expand=True, padx=5, pady=5)

        # 状态标签
        self.status_label = ttk.Label(self, text=gettext.gettext("请选择属性后点击绘制"))
        self.status_label.pack(fill='x', padx=5, pady=2)

        # 存储当前数据
        self._current_prop_data = None
        self._current_prop_name = ""

    def _on_draw(self):
        """绘制按钮回调"""
        if self._current_prop_data:
            self.update_display(self._current_prop_data, self._current_prop_name)

    def update_display(self, prop_data: Optional[Dict[str, List[Any]]], prop_name: str):
        """
        更新图表显示

        Args:
            prop_data: 属性数据 { 类型名称 -> 值列表 }
            prop_name: 属性名称
        """
        # 存储当前数据
        self._current_prop_data = prop_data
        self._current_prop_name = prop_name

        # 清除旧图表
        self._clear_plot()

        if not prop_data:
            self.status_label.configure(text=gettext.gettext("无数据可绘制"))
            return

        try:
            # 合并所有类型的值
            all_values: List[Any] = []
            for type_name, values in prop_data.items():
                if isinstance(values, list):
                    all_values.extend(values)

            if not all_values:
                self.status_label.configure(text=gettext.gettext("数据为空"))
                return

            # 使用 DataProcessor 过滤数据
            supported_values, unsupported_counts = DataProcessor.filter_supported(all_values)
            total_count = len(all_values)
            supported_count = len(supported_values)

            if not supported_values:
                unsupported_info = ", ".join([f"{k}: {v}" for k, v in unsupported_counts.items()])
                self.status_label.configure(
                    text=gettext.gettext("无可绘制数据 (已排除: {})").format(unsupported_info)
                )
                return

            # 检测数据类型
            data_type = DataProcessor.detect_type(supported_values)
            plot_type = self.plot_type_var.get()

            # 存储过滤后的数据用于绘图
            self._filtered_values = supported_values

            # 根据数据类型和图表类型绘制
            if data_type == DataType.NUMERIC:
                if plot_type == 'histogram':
                    self._draw_histogram(supported_values, prop_name)
                else:
                    self._draw_bar_chart(supported_values, prop_name)
            else:
                # 非数值型数据使用柱状图或饼图
                if plot_type == 'pie':
                    self._draw_pie_chart(supported_values, prop_name)
                else:
                    self._draw_bar_chart(supported_values, prop_name)

            # 更新状态标签
            status_parts = [gettext.gettext("图表已绘制 ({} 类型)").format(data_type)]
            if unsupported_counts:
                unsupported_info = ", ".join([f"{k}: {v}" for k, v in unsupported_counts.items()])
                status_parts.append(gettext.gettext("已排除: {}").format(unsupported_info))
            status_parts.append(gettext.gettext("数据: {}/{}").format(supported_count, total_count))
            self.status_label.configure(text=" | ".join(status_parts))

        except Exception as e:
            error_msg = gettext.gettext("图表绘制失败: {}").format(e)
            logger.error(error_msg, exc_info=True)
            self.status_label.configure(text=error_msg)

    def _draw_bar_chart(self, values: List[Any], prop_name: str):
        """绘制柱状图"""
        try:
            # 获取频率数据
            result = self._get_top_frequencies_data(values, prop_name)
            if result is None:
                self.status_label.configure(text=gettext.gettext("无法计算频率"))
                return
            categories, counts = result

            # 创建图表
            fig, ax = plt.subplots(figsize=(8, 5), dpi=100)

            bars = ax.bar(range(len(categories)), counts, color='steelblue', alpha=0.8)
            ax.set_title(gettext.gettext('{} - 频率分布').format(prop_name), fontsize=12, fontweight='bold')
            ax.set_xlabel(gettext.gettext('值'), fontsize=10)
            ax.set_ylabel(gettext.gettext('频数'), fontsize=10)
            ax.set_xticks(range(len(categories)))
            ax.set_xticklabels(categories, rotation=45, ha='right', fontsize=8)

            # 在柱子上方显示数值
            for bar, count in zip(bars, counts):
                ax.text(bar.get_x() + bar.get_width() / 2., bar.get_height(),
                        f'{count}', ha='center', va='bottom', fontsize=8)

            ax.grid(True, alpha=0.3, axis='y')
            plt.tight_layout()

            self._display_figure(fig)

        except Exception as e:
            error_msg = gettext.gettext("柱状图绘制异常: {}").format(e)
            logger.error(error_msg, exc_info=True)
            self.status_label.configure(text=error_msg)

    def _draw_histogram(self, values: List[Any], prop_name: str):
        """绘制直方图"""
        try:
            numeric_values = DataProcessor.convert_to_numeric(values)

            if len(numeric_values) < 2:
                self.status_label.configure(text=gettext.gettext("数值数据不足（需要至少2个数值）"))
                return

            # 创建图表
            fig, ax = plt.subplots(figsize=(8, 5), dpi=100)

            n, bins, _ = ax.hist(numeric_values, bins='auto', color='skyblue',
                                 edgecolor='black', alpha=0.7)
            ax.set_title(f'{prop_name} - {gettext.gettext("数值分布")}', fontsize=12, fontweight='bold')
            ax.set_xlabel(gettext.gettext('数值'), fontsize=10)
            ax.set_ylabel(gettext.gettext('频数'), fontsize=10)
            ax.grid(True, alpha=0.3)

            # 添加统计信息
            stats_text = gettext.gettext('数量: {}\n均值: {:.2f}\n标准差: {:.2f}').format(
                len(numeric_values), np.mean(numeric_values), np.std(numeric_values))
            ax.text(0.95, 0.95, stats_text, transform=ax.transAxes,
                    fontsize=9, verticalalignment='top', horizontalalignment='right',
                    bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.5))

            plt.tight_layout()
            self._display_figure(fig)

        except Exception as e:
            error_msg = gettext.gettext("直方图绘制异常: {}").format(e)
            logger.error(error_msg, exc_info=True)
            self.status_label.configure(text=error_msg)

    def _draw_pie_chart(self, values: List[Any], prop_name: str):
        """绘制饼图"""
        try:
            # 获取频率数据
            result = self._get_top_frequencies_data(values, prop_name)
            if result is None:
                self.status_label.configure(text=gettext.gettext("无法计算频率"))
                return
            labels, counts = result

            # 如果值太多，合并为"其他"
            if len(labels) > 10:
                other_count = sum(counts[10:])
                labels = labels[:10] + [gettext.gettext('其他')]
                counts = counts[:10] + [other_count]

            # 创建图表
            fig, ax = plt.subplots(figsize=(8, 6), dpi=100)

            _, _, _ = ax.pie(
                counts, labels=labels, autopct='%1.1f%%',
                startangle=90, textprops={'fontsize': 8}
            )
            ax.set_title(gettext.gettext('{} - 占比分布').format(prop_name), fontsize=12, fontweight='bold')

            plt.tight_layout()
            self._display_figure(fig)

        except Exception as e:
            error_msg = gettext.gettext("饼图绘制异常: {}").format(e)
            logger.error(error_msg, exc_info=True)
            self.status_label.configure(text=error_msg)

    def _get_top_frequencies_data(self, values: List[Any], prop_name: str) -> Optional[Tuple[List[str], List[int]]]:
        """
        获取频率统计数据（提取公共逻辑）

        Args:
            values: 数据值列表
            prop_name: 属性名称

        Returns:
            成功时返回 (labels, counts)，失败时返回 None
        """
        if not values:
            return None

        # 计算频率
        data_dict = {prop_name: values}
        top_n = self.top_n_var.get()
        top_freq = get_top_frequencies(data_dict, top_n)

        if prop_name not in top_freq or not top_freq[prop_name]:
            return None

        # 准备数据
        labels = [self._format_label(str(item[0])) for item in top_freq[prop_name]]
        counts = [item[1] for item in top_freq[prop_name]]

        return labels, counts

    def _format_label(self, label: str) -> str:
        """格式化标签"""
        if len(label) > 15:
            return label[:12] + "..."
        return label

    def _display_figure(self, fig):
        """在界面中显示图表"""
        # 清除旧图表
        self._clear_plot()

        # 创建 Canvas
        self._current_canvas = FigureCanvasTkAgg(fig, master=self.plot_frame)
        self._current_canvas.draw()
        self._current_canvas.get_tk_widget().pack(fill='both', expand=True)

        # 添加工具栏
        toolbar_frame = ttk.Frame(self.plot_frame)
        toolbar_frame.pack(fill='x')
        toolbar = NavigationToolbar2Tk(self._current_canvas, toolbar_frame)
        toolbar.update()

        self._current_figure = fig

    def _clear_plot(self):
        """清除当前图表"""
        if self._current_canvas is not None:
            self._current_canvas.get_tk_widget().destroy()
            self._current_canvas = None

        if self._current_figure is not None:
            plt.close(self._current_figure)
            self._current_figure = None

        # 清除 plot_frame 中的所有内容
        for widget in self.plot_frame.winfo_children():
            widget.destroy()
