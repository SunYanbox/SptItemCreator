import json
import tkinter as tk
from tkinter import ttk
from typing import Optional

from Global import gettext

from managers.stats_mgr import StatsManager
from gui.stats_display import StatsInfoTab, FrequencyTab, PlotTab


class DetailViewer(tk.Frame):
    """BaseClass详情查看器（非窗口组件）"""
    json_text: tk.Text
    unique_listbox: tk.Listbox
    unique_frame: tk.Frame
    all_listbox: tk.Listbox
    all_frame: tk.Frame
    notebook: ttk.Notebook
    right_notebook: ttk.Notebook
    parent: tk.Misc
    bc_name: str
    stats_mgr: StatsManager
    json_container: tk.Frame
    stats_info_tab: StatsInfoTab
    frequency_tab: FrequencyTab
    plot_tab: PlotTab

    def __init__(self, parent: tk.Misc, bc_name: str, stats_mgr):
        """初始化详情查看器

        Args:
            parent: 父容器
            bc_name: BaseClass名称
            stats_mgr: StatsManager实例
        """
        super().__init__(parent)
        self.parent = parent
        self.bc_name = bc_name
        self.stats_mgr = stats_mgr

        # 获取属性
        self.all_props = stats_mgr.get_prop_keys(bc_name) if stats_mgr else None
        self.unique_props = stats_mgr.get_unique_prop_keys(bc_name) if stats_mgr else None

        # JSON缩进设置
        self.json_indent = tk.IntVar(value=2)

        # 创建主框架
        self.create_widgets()

    def create_widgets(self):
        """创建界面组件"""
        # 信息标签
        info_text = gettext("BaseClass: {}\n").format(self.bc_name)
        info_text += gettext("总属性数: {}\n").format(len(self.all_props) if self.all_props else 0)
        info_text += gettext("唯一属性数: {}").format(len(self.unique_props) if self.unique_props else 0)

        # 信息在顶部显示
        info_frame = ttk.Frame(self)
        info_frame.pack(fill='x', side='top', padx=5, pady=5)
        tk.Label(info_frame, text=info_text, height=3, font=("Arial", 10)).pack(fill='x')

        # 创建左右分隔的主容器
        main_paned = ttk.PanedWindow(self, orient='horizontal')
        main_paned.pack(fill='both', expand=True)

        # 左侧面板
        left_frame = tk.Frame(main_paned)
        main_paned.add(left_frame, weight=1)

        # 创建选项卡
        self.notebook = ttk.Notebook(left_frame)
        self.notebook.pack(fill='both', expand=True, padx=10, pady=10)

        # 所有属性标签页
        self.all_frame = tk.Frame(self.notebook)
        self.notebook.add(self.all_frame, text=gettext("所有属性"))

        self.all_listbox = tk.Listbox(self.all_frame, exportselection=0)
        self.all_listbox.pack(fill='both', expand=True, padx=5, pady=5)

        if self.all_props:
            for prop in sorted(self.all_props):
                self.all_listbox.insert(tk.END, prop)
        else:
            self.all_listbox.insert(tk.END, gettext("无属性"))

        # 绑定选择事件
        self.all_listbox.bind('<<ListboxSelect>>', self.on_prop_select)

        # 唯一属性标签页
        self.unique_frame = tk.Frame(self.notebook)
        self.notebook.add(self.unique_frame, text=gettext("唯一属性"))

        self.unique_listbox = tk.Listbox(self.unique_frame, exportselection=0)
        self.unique_listbox.pack(fill='both', expand=True, padx=5, pady=5)

        if self.unique_props:
            for prop in sorted(self.unique_props):
                self.unique_listbox.insert(tk.END, prop)
        else:
            self.unique_listbox.insert(tk.END, gettext("无唯一属性"))

        # 绑定选择事件
        self.unique_listbox.bind('<<ListboxSelect>>', self.on_prop_select)

        # 右侧面板 - JSON显示区域
        right_frame = tk.Frame(main_paned)
        main_paned.add(right_frame, weight=1)

        # 创建右侧选项卡
        self.right_notebook = ttk.Notebook(right_frame)
        self.right_notebook.pack(fill='both', expand=True, padx=5, pady=5)

        # JSON显示标签页
        json_tab = tk.Frame(self.right_notebook)
        self.right_notebook.add(json_tab, text=gettext("JSON数据"))

        # 缩进选择控件
        indent_frame = tk.Frame(json_tab)
        indent_frame.pack(fill='x', padx=5, pady=5)

        tk.Label(indent_frame, text=gettext("JSON缩进:")).pack(side='left')

        tk.Radiobutton(
            indent_frame, text="2", variable=self.json_indent, value=2,
            command=self.update_json_display
        ).pack(side='left', padx=5)

        tk.Radiobutton(
            indent_frame, text="4", variable=self.json_indent, value=4,
            command=self.update_json_display
        ).pack(side='left', padx=5)

        # JSON显示的Frame容器
        self.json_container = tk.Frame(json_tab)
        self.json_container.pack(fill='both', expand=True, padx=5, pady=5)

        # JSON显示文本框
        self.json_text = tk.Text(self.json_container)
        self.json_text.pack(fill='both', expand=True)

        # 滚动条
        json_scrollbar = ttk.Scrollbar(self.json_text, orient='vertical', command=self.json_text.yview)
        json_scrollbar.pack(side='right', fill='y')
        self.json_text.configure(yscrollcommand=json_scrollbar.set)

        # 统计信息标签页
        stats_frame = tk.Frame(self.right_notebook)
        self.right_notebook.add(stats_frame, text=gettext("统计信息"))
        self.stats_info_tab = StatsInfoTab(stats_frame)
        self.stats_info_tab.pack(fill='both', expand=True)

        # 频率统计标签页
        freq_frame = tk.Frame(self.right_notebook)
        self.right_notebook.add(freq_frame, text=gettext("频率统计"))
        self.frequency_tab = FrequencyTab(freq_frame)
        self.frequency_tab.pack(fill='both', expand=True)

        # 统计图表标签页
        plot_frame = tk.Frame(self.right_notebook)
        self.right_notebook.add(plot_frame, text=gettext("统计图表"))
        self.plot_tab = PlotTab(plot_frame)
        self.plot_tab.pack(fill='both', expand=True)

        # 初始化显示
        self.update_json_display()


    def on_prop_select(self, _):
        """处理属性选择事件"""
        self.update_json_display()
        self.update_stats_display()


    def update_json_display(self):
        """更新JSON显示内容"""
        # 获取当前选中的属性
        selected_prop = self.get_selected_prop()

        # 清空文本框
        self.json_text.configure(state='normal')
        self.json_text.delete('1.0', tk.END)

        if selected_prop and self.stats_mgr:
            # 获取属性数据
            stats_struct = self.stats_mgr.data.get(self.bc_name)
            if stats_struct:
                prop_data = stats_struct.get_prop_data(selected_prop)
                if prop_data:
                    try:
                        # 格式化JSON
                        json_str = json.dumps(prop_data, indent=self.json_indent.get(), ensure_ascii=False)
                        self.json_text.insert('1.0', json_str)
                    except Exception as e:
                        self.json_text.insert('1.0', gettext("JSON格式化错误: {}").format(e))
                else:
                    self.json_text.insert('1.0', gettext("属性 '{}' 无数据").format(selected_prop))
            else:
                self.json_text.insert('1.0', gettext("BaseClass '{}' 不存在").format(self.bc_name))
        else:
            self.json_text.insert('1.0', gettext("请从左侧选择一个属性"))

        # 设置为只读
        self.json_text.configure(state='disabled')

    def update_stats_display(self):
        """更新统计相关标签页的显示"""
        selected_prop = self.get_selected_prop()

        if selected_prop and self.stats_mgr:
            stats_struct = self.stats_mgr.data.get(self.bc_name)
            if stats_struct:
                prop_data = stats_struct.get_prop_data(selected_prop)
                if prop_data:
                    # 更新统计信息标签页
                    self.stats_info_tab.update_display(prop_data, selected_prop)
                    # 更新频率统计标签页
                    self.frequency_tab.update_display(prop_data, selected_prop)
                    # 更新统计图表标签页
                    self.plot_tab.update_display(prop_data, selected_prop)
                else:
                    self._clear_stats_tabs(selected_prop)
            else:
                self._clear_stats_tabs(selected_prop)
        else:
            self._clear_stats_tabs("")

    def _clear_stats_tabs(self, prop_name: str):
        """清空统计标签页显示"""
        self.stats_info_tab.update_display(None, prop_name)
        self.frequency_tab.update_display(None, prop_name)
        self.plot_tab.update_display(None, prop_name)


    def get_selected_prop(self) -> Optional[str]:
        """获取当前选中的属性名称"""
        # 检查当前选中的标签页
        current_tab = self.notebook.select()
        if not current_tab:
            return None

        # 根据标签页获取对应的Listbox
        if self.notebook.index(current_tab) == 0:
            listbox = self.all_listbox
        else:
            listbox = self.unique_listbox

        # 获取选中项
        selection = listbox.curselection()
        if selection:
            return listbox.get(selection[0])
        return None

    @staticmethod
    def build(parent: tk.Misc, bc_name: str, stats_mgr):
        return DetailViewer(parent, bc_name, stats_mgr)