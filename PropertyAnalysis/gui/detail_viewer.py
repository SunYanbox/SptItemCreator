import tkinter as tk
from tkinter import ttk


def show_detail_window(parent: tk.Misc, bc_name: str, stats_mgr) -> None:
    """显示BaseClass详情窗口
    
    Args:
        parent: 父窗口
        bc_name: BaseClass名称
        stats_mgr: StatsManager实例
    """
    # 获取属性
    all_props = stats_mgr.get_prop_keys(bc_name) if stats_mgr else None
    unique_props = stats_mgr.get_unique_prop_keys(bc_name) if stats_mgr else None
    
    # 创建详情窗口
    detail_win = tk.Toplevel(parent)
    detail_win.title(f"BaseClass 详情: {bc_name}")
    detail_win.geometry("500x400")
    
    # 信息标签
    info_text = f"BaseClass: {bc_name}\n"
    info_text += f"总属性数: {len(all_props) if all_props else 0}\n"
    info_text += f"唯一属性数: {len(unique_props) if unique_props else 0}"
    
    tk.Label(detail_win, text=info_text, font=("Arial", 10)).pack(pady=10)
    
    # 创建选项卡
    notebook = ttk.Notebook(detail_win)
    notebook.pack(fill='both', expand=True, padx=10, pady=10)
    
    # 所有属性标签页
    all_frame = tk.Frame(notebook)
    notebook.add(all_frame, text="所有属性")
    
    all_listbox = tk.Listbox(all_frame)
    all_listbox.pack(fill='both', expand=True, padx=5, pady=5)
    
    if all_props:
        for prop in sorted(all_props):
            all_listbox.insert(tk.END, prop)
    else:
        all_listbox.insert(tk.END, "无属性")
    
    # 唯一属性标签页
    unique_frame = tk.Frame(notebook)
    notebook.add(unique_frame, text="唯一属性")
    
    unique_listbox = tk.Listbox(unique_frame)
    unique_listbox.pack(fill='both', expand=True, padx=5, pady=5)
    
    if unique_props:
        for prop in sorted(unique_props):
            unique_listbox.insert(tk.END, prop)
    else:
        unique_listbox.insert(tk.END, "无唯一属性")