import tkinter as tk
from tkinter import messagebox
from typing import Optional

from Global import logger, config
from gui.detail_viewer import DetailViewer
from managers.stats_mgr import StatsManager


def _create_listbox_with_scrollbar(parent_frame, height=15, select_mode=tk.SINGLE):
    """创建带有滚动条的Listbox

    Args:
        parent_frame: 父容器
        height: Listbox高度
        select_mode: 选择模式

    Returns:
        tuple: (listbox, scrollbar)
    """
    listbox = tk.Listbox(parent_frame, height=height, selectmode=select_mode)
    listbox.pack(fill='both', expand=True, pady=5)

    scrollbar = tk.Scrollbar(parent_frame, orient='vertical', command=listbox.yview)
    scrollbar.pack(side='right', fill='y')
    listbox.config(yscrollcommand=scrollbar.set)

    return listbox, scrollbar


class StatsViewerGUI:
    """数据查看GUI界面"""
    stats_mgr: Optional[StatsManager] = None
    
    def __init__(self, parent: tk.Misc, app=None):
        self.pk_listbox = None
        self.status_label = None
        self.bc_listbox = None
        self.parent = parent
        if not hasattr(app, 'add_tab') or not callable(app.add_tab):
            raise TypeError("app.add_tab must be callable")
        self.app = app  # 主应用程序引用
        self.auto_load_plk()
        self.create_widgets()
        self.refresh_base_classes()

    def auto_load_plk(self):
        """重新根据配置文件加载数据"""
        try:
            save_file_path = config.get('StatsManagerSavePath')
            self.stats_mgr = StatsManager.create_from_file(save_file_path)
        except Exception as e:
            logger.error(f'在StatsViewerGUI中, 根据配置自动初始化StatsManager失败: {e}', exc_info=True)

    def create_widgets(self):
        """创建界面组件"""
        # 顶部标签
        title_label = tk.Label(self.parent, text="数据查看", font=("Arial", 14, "bold"))
        title_label.pack(pady=10)
        
        # 状态信息
        status_frame = tk.Frame(self.parent)
        status_frame.pack(fill='x', padx=10, pady=5)
        
        data_count = len(self.stats_mgr.data) if self.stats_mgr else 0
        self.status_label = tk.Label(status_frame, text=f"已加载数据条数: {data_count}")
        self.status_label.pack(side='left')
        
        # 双列布局容器
        main_container = tk.Frame(self.parent)
        main_container.pack(fill='both', expand=True, padx=10, pady=5)
        
        # 左列: BaseClasses 区域
        bc_frame = tk.LabelFrame(main_container, text="BaseClasses 列表", padx=10, pady=10)
        bc_frame.pack(side='left', fill='both', expand=True, padx=(0, 5))
        
        # BaseClasses 列表框架
        bc_list_frame = tk.Frame(bc_frame)
        bc_list_frame.pack(fill='both', expand=True)
        
        tk.Label(bc_list_frame, text="所有BaseClasses:").pack(anchor='w')
        
        self.bc_listbox, _ = _create_listbox_with_scrollbar(
            bc_list_frame, height=15, select_mode=tk.SINGLE
        )
        
        # BaseClasses 按钮
        bc_button_frame = tk.Frame(bc_frame)
        bc_button_frame.pack(pady=5)
        
        tk.Button(bc_button_frame, text="刷新列表", command=self.refresh_base_classes).pack(side='left', padx=5)
        tk.Button(bc_button_frame, text="查看详情", command=self.show_props_for_selected).pack(side='left', padx=5)
        
        # 右列: PropKeys 区域
        pk_frame = tk.LabelFrame(main_container, text="PropKeys 列表", padx=10, pady=10)
        pk_frame.pack(side='right', fill='both', expand=True, padx=(5, 0))
        
        pk_list_frame = tk.Frame(pk_frame)
        pk_list_frame.pack(fill='both', expand=True)
        
        tk.Label(pk_list_frame, text="所有PropKeys:").pack(anchor='w')
        
        self.pk_listbox, _ = _create_listbox_with_scrollbar(
            pk_list_frame, height=15, select_mode=tk.SINGLE
        )

        # PropKeys 按钮
        pk_button_frame = tk.Frame(pk_frame)
        pk_button_frame.pack(pady=5)

        tk.Button(pk_button_frame, text="刷新列表", command=self.refresh_prop_keys).pack(side='left', padx=5)
        
        # 绑定事件
        self.bc_listbox.bind('<<ListboxSelect>>', self.on_bc_selected)
        
        # 初始化数据
        self.refresh_base_classes()
        self.refresh_prop_keys()
    
    def refresh_base_classes(self):
        """刷新BaseClasses列表"""
        self.bc_listbox.delete(0, tk.END)
        if self.stats_mgr:
            base_classes = self.stats_mgr.base_classes
            for bc in sorted(base_classes):
                self.bc_listbox.insert(tk.END, bc)
        else:
            self.bc_listbox.insert(tk.END, "未加载数据")
            logger.warning("StatsManager未加载")
    
    def refresh_prop_keys(self):
        """刷新PropKeys列表"""
        self.pk_listbox.delete(0, tk.END)
        if self.stats_mgr:
            prop_keys = self.stats_mgr.prop_keys
            for pk in sorted(prop_keys):
                self.pk_listbox.insert(tk.END, pk)
        else:
            self.pk_listbox.insert(tk.END, "未加载数据")
    
    def on_bc_selected(self, _):
        """BaseClass选中事件"""
        selection = self.bc_listbox.curselection()
        if selection:
            bc_name = self.bc_listbox.get(selection[0])
            self.update_status_for_selected(bc_name)
    
    def update_status_for_selected(self, bc_name: str) -> None:
        """更新状态显示选中的BaseClass信息"""
        if not self.stats_mgr:
            return
        
        all_props = self.stats_mgr.get_prop_keys(bc_name)
        unique_props = self.stats_mgr.get_unique_prop_keys(bc_name)
        
        prop_count = len(all_props) if all_props else 0
        unique_count = len(unique_props) if unique_props else 0
        
        self.status_label.config(
            text=f"已加载数据条数: {len(self.stats_mgr.data) if self.stats_mgr else 0} | "
                 f"选中: {bc_name} | 属性: {prop_count} | 唯一属性: {unique_count}"
        )
    
    def show_props_for_selected(self) -> Optional[tk.Toplevel]:
        """显示选中BaseClass的属性"""
        selection = self.bc_listbox.curselection()
        if not selection:
            messagebox.showwarning("警告", "请先选择一个BaseClass")
            return None
        
        bc_name = self.bc_listbox.get(selection[0])
        if not self.stats_mgr:
            messagebox.showerror("错误", "StatsManager未加载")
            return None

        title = f"BaseClass 详情: {bc_name}"

        detail_win = tk.Toplevel(self.app.root)
        detail_win.title(title)
        detail_win.geometry("700x500")

        # 创建详情查看器
        viewer = DetailViewer(detail_win, bc_name, self.stats_mgr)
        viewer.pack(fill='both', expand=True, padx=10, pady=10)

        def add_to_tab(inner_bc_name: str, stats_mgr):
            self.app.add_tab(lambda parent : DetailViewer.build(parent, inner_bc_name, stats_mgr), inner_bc_name, title, enable_close=True)
            detail_win.destroy()

        # 控制按钮框架
        button_frame = tk.Frame(detail_win)
        button_frame.pack(side='bottom', fill='x', padx=10, pady=5)

        tk.Button(button_frame, text="添加到标签页",
                  command=lambda: add_to_tab(bc_name, self.stats_mgr)).pack(side='left', padx=5)

        tk.Button(button_frame, text="关闭窗口",
                  command=detail_win.destroy).pack(side='right', padx=5)

        return detail_win
        
