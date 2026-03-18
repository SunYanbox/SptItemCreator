import tkinter as tk
from tkinter import ttk
from typing import Optional, Callable

class DetailViewer(tk.Frame):
    """BaseClass详情查看器（非窗口组件）"""

    def __init__(self, parent: tk.Misc, bc_name: str, stats_mgr):
        """初始化详情查看器

        Args:
            parent: 父容器
            bc_name: BaseClass名称
            stats_mgr: StatsManager实例
        """
        super().__init__(parent)
        self.unique_listbox = None
        self.unique_frame = None
        self.all_listbox = None
        self.all_frame = None
        self.notebook = None
        self.parent = parent
        self.bc_name = bc_name
        self.stats_mgr = stats_mgr

        # 获取属性
        self.all_props = stats_mgr.get_prop_keys(bc_name) if stats_mgr else None
        self.unique_props = stats_mgr.get_unique_prop_keys(bc_name) if stats_mgr else None

        # 创建主框架
        self.create_widgets()

    def create_widgets(self):
        """创建界面组件"""
        # 信息标签
        info_text = f"BaseClass: {self.bc_name}\n"
        info_text += f"总属性数: {len(self.all_props) if self.all_props else 0}\n"
        info_text += f"唯一属性数: {len(self.unique_props) if self.unique_props else 0}"

        tk.Label(self, text=info_text, font=("Arial", 10)).pack(pady=10)

        # 创建选项卡
        self.notebook = ttk.Notebook(self)
        self.notebook.pack(fill='both', expand=True, padx=10, pady=10)

        # 所有属性标签页
        self.all_frame = tk.Frame(self.notebook)
        self.notebook.add(self.all_frame, text="所有属性")

        self.all_listbox = tk.Listbox(self.all_frame)
        self.all_listbox.pack(fill='both', expand=True, padx=5, pady=5)

        if self.all_props:
            for prop in sorted(self.all_props):
                self.all_listbox.insert(tk.END, prop)
        else:
            self.all_listbox.insert(tk.END, "无属性")

        # 唯一属性标签页
        self.unique_frame = tk.Frame(self.notebook)
        self.notebook.add(self.unique_frame, text="唯一属性")

        self.unique_listbox = tk.Listbox(self.unique_frame)
        self.unique_listbox.pack(fill='both', expand=True, padx=5, pady=5)

        if self.unique_props:
            for prop in sorted(self.unique_props):
                self.unique_listbox.insert(tk.END, prop)
        else:
            self.unique_listbox.insert(tk.END, "无唯一属性")

    @staticmethod
    def build(parent: tk.Misc, bc_name: str, stats_mgr):
        return DetailViewer(parent, bc_name, stats_mgr)