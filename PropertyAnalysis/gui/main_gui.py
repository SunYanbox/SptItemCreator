import tkinter as tk
from tkinter import ttk, messagebox
import sys
import os
from typing import Dict, Union, Any, Optional, Callable

from models.object_context import ObjectContext

# 导入本地模块
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from Global import logger
from gui.config_gui import ConfigGUI
from gui.stats_viewer import StatsViewerGUI
from gui.detail_viewer import DetailViewer


class MainApplication:
    """主应用程序窗口"""
    
    def __init__(self, root: tk.Tk):
        self.config_gui = None
        self.stats_gui = None
        self.config_tab = None
        self.stats_tab = None
        self.main_frame = None
        self.status_bar = None
        self.tab_control: Optional[ttk.Notebook] = None
        self._tabs: Dict[str, tk.Widget] = {}  # 存储动态创建的详情标签页 ID -> 实例
        self.root = root
        self.root.title("PropertyAnalysis GUI")
        self.root.geometry("1000x700")

        # 初始化GUI组件
        self.init_gui()
        
        # 绑定窗口关闭事件
        self.root.protocol("WM_DELETE_WINDOW", self.on_closing)
        
        logger.info("PropertyAnalysis GUI 应用程序已启动")
    
    def init_gui(self):
        """初始化GUI界面"""
        # 创建菜单栏
        self.create_menu()
        
        # 创建主容器
        self.create_main_container()
        
        # 初始化选项卡
        self.init_tabs()
    
    def create_menu(self):
        """创建菜单栏"""
        menubar = tk.Menu(self.root)
        self.root.config(menu=menubar)
        
        # 文件菜单
        file_menu = tk.Menu(menubar, tearoff=0)
        menubar.add_cascade(label="文件", menu=file_menu)
        file_menu.add_command(label="退出", command=self.on_closing)
        
        # 工具菜单
        tools_menu = tk.Menu(menubar, tearoff=0)
        menubar.add_cascade(label="工具", menu=tools_menu)
        tools_menu.add_command(label="重新加载数据", command=self.reload_data)
        tools_menu.add_separator()
        tools_menu.add_command(label="打开日志", command=self.open_log)
        
        # 帮助菜单
        help_menu = tk.Menu(menubar, tearoff=0)
        menubar.add_cascade(label="帮助", menu=help_menu)
        help_menu.add_command(label="关于", command=self.show_about)
    
    def create_main_container(self):
        """创建主容器"""
        # 创建主框架
        self.main_frame = ttk.Frame(self.root, padding="10")
        self.main_frame.pack(fill='both', expand=True)
        
        # 状态栏
        self.status_bar = ttk.Label(self.root, text="就绪", relief='sunken', anchor='w')
        self.status_bar.pack(side='bottom', fill='x')
        
        # 创建选项卡控件
        self.tab_control = ttk.Notebook(self.main_frame)
        self.tab_control.pack(fill='both', expand=True)
    
    def init_tabs(self):
        """初始化选项卡"""
        # 数据查看选项卡
        self.stats_tab = ttk.Frame(self.tab_control)
        self.tab_control.add(self.stats_tab, text="数据查看")
        
        # 配置管理选项卡
        self.config_tab = ttk.Frame(self.tab_control)
        self.tab_control.add(self.config_tab, text="配置管理")
        
        # 初始化各选项卡内容
        self.init_stats_tab()
        self.init_config_tab()
        
        # 绑定选项卡切换事件
        self.tab_control.bind("<<NotebookTabChanged>>", self.on_tab_changed)

    def close_tab(self, name: str):
        if name in self._tabs:
            try:
                sure_tab_id = self._tabs[name]
                self.tab_control.forget(sure_tab_id)
                self._tabs[name].destroy()
            except Exception as e:
                logger.error(f'清理标签页{name}时出现错误: {e}', exc_info=True)
            self._tabs.pop(name)
            logger.debug(f'已执行关闭标签页: {name}, 剩余标签页: {",".join(self._tabs.keys())}')
    
    def add_tab(self, widget:  Callable[[tk.Widget], tk.Widget], name: str, tab_title: str, enable_close: bool = False):
        if name in self._tabs:
            self.tab_control.select(self._tabs[name])
            self.update_status(f"已切换到已有标签页: {tab_title}")
            return
        # 创建新标签页框架
        tab_frame = ttk.Frame(self.tab_control)
        self.tab_control.add(tab_frame, text=tab_title)
        
        title_frame = ttk.Frame(tab_frame)
        title_frame.pack(fill='x', side='top', padx=10)

        tk.ttk.Label(title_frame, text=tab_title).pack(side='left')

        if enable_close:
            tk.ttk.Button(title_frame, text='x', command=lambda x = name: self.close_tab(x)).pack(side='right')

        viewer = widget(tab_frame)
        
        viewer.pack(fill='both', expand=True, padx=10, pady=10)
        
        # 存储标签页信息
        self._tabs[name] = tab_frame
        
        # 切换到新标签页
        self.update_status(f"已创建详情标签页: {tab_title}")

    def init_stats_tab(self):
        """初始化数据查看选项卡"""
        self.stats_gui = StatsViewerGUI(self.stats_tab, self)
    
    def init_config_tab(self):
        """初始化配置管理选项卡"""
        self.config_gui = ConfigGUI(self.config_tab)
    
    def on_tab_changed(self, _):
        """选项卡切换事件"""
        selected_tab = self.tab_control.select()
        tab_name = self.tab_control.tab(selected_tab, 'text')
        
        if tab_name == "数据查看":
            self.update_status("数据查看模式")
            # 可以在这里刷新数据
            if hasattr(self, 'stats_gui'):
                self.stats_gui.refresh_base_classes()
                self.stats_gui.refresh_prop_keys()
        
        elif tab_name == "配置管理":
            self.update_status("配置管理模式")
    
    def reload_data(self):
        """重新加载数据"""
        try:
            # 重新赋值oc即可重新加载数据
            self.stats_gui.oc = ObjectContext()
            if hasattr(self, 'stats_gui'):
                self.stats_gui.refresh_base_classes()
                self.stats_gui.refresh_prop_keys()
            
            self.update_status("数据已重新加载")
            messagebox.showinfo("成功", "数据已重新加载")
            logger.info("数据重新加载完成")
            
        except Exception as e:
            logger.error(f"重新加载数据时出错: {e}", exc_info=True)
            messagebox.showerror("错误", f"重新加载数据时出错:\n{str(e)}")
    
    def open_log(self):
        """打开日志文件"""
        log_path = os.path.join(os.path.dirname(__file__), "..", "data", "PropertyAnalysis.log")
        if os.path.exists(log_path):
            try:
                os.startfile(log_path)
                self.update_status(f"已打开日志文件: {log_path}")
            except Exception as e:
                messagebox.showerror("错误", f"无法打开日志文件:\n{str(e)}")
                logger.error(f"打开日志文件失败: {e}")
        else:
            messagebox.showwarning("警告", f"日志文件不存在:\n{log_path}")
    
    def show_about(self):
        """显示关于对话框"""
        about_text = """PropertyAnalysis GUI
        
版本: 1.0.0
作者: Suntion
描述: SPTarkov 模组属性分析工具
        
基于 Flask API 蓝图迁移到 Tkinter GUI
        
功能:
- 配置管理
- 数据查看 (BaseClasses 和 PropKeys)
- 属性统计分析
        
© 2026 Suntion"""
        
        messagebox.showinfo("关于", about_text)
    
    def update_status(self, message: str):
        """更新状态栏"""
        self.status_bar.config(text=message)
        self.root.update_idletasks()
    
    def on_closing(self):
        """窗口关闭事件"""
        if messagebox.askokcancel("退出", "确定要退出应用程序吗？"):
            logger.info("PropertyAnalysis GUI 应用程序已退出")
            self.root.destroy()


def main():
    """主函数"""
    root = tk.Tk()
    
    # 设置窗口样式
    style = ttk.Style()
    style.theme_use('clam')  # 使用 'clam' 主题
    
    _ = MainApplication(root)
    root.mainloop()


if __name__ == "__main__":
    main()