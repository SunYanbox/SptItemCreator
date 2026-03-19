import tkinter as tk
from tkinter import ttk, messagebox
from tkinter.simpledialog import askinteger, askfloat
from typing import Optional

from Global import gettext

class ConfigGUI:
    """配置管理GUI界面"""
    config_listbox: Optional[tk.Listbox]
    value_text: Optional[tk.Text]
    language_var: Optional[tk.StringVar]
    
    def __init__(self, parent: tk.Misc):
        self.value_text = None
        self.config_listbox = None
        self.language_var = None
        self.parent = parent
        self.create_widgets()
        self.refresh_config_list()
        self.config_listbox.bind('<<ListboxSelect>>', self.on_config_selected)
    
    def create_widgets(self):
        """创建界面组件"""
        # 顶部标签
        title_label = tk.Label(self.parent, text=gettext("配置管理"), font=("Arial", 14, "bold"))
        title_label.pack(pady=10)
        
        # 语言设置框架
        lang_frame = tk.LabelFrame(self.parent, text=gettext("语言设置"), padx=10, pady=5)
        lang_frame.pack(fill='x', padx=10, pady=5)
        
        lang_inner_frame = tk.Frame(lang_frame)
        lang_inner_frame.pack(fill='x')
        
        tk.Label(lang_inner_frame, text=gettext("当前语言:")).pack(side='left', padx=5)
        
        self.language_var = tk.StringVar()
        from Global import config
        self.language_var.set(config.get('Language', 'zh'))
        
        lang_combo = ttk.Combobox(lang_inner_frame, textvariable=self.language_var, 
                                   values=['zh', 'en'], state='readonly', width=10)
        lang_combo.pack(side='left', padx=5)
        
        tk.Button(lang_inner_frame, text=gettext("切换语言"), 
                  command=self.change_language).pack(side='left', padx=5)
        
        # 语言切换提示
        tk.Label(lang_frame, text=gettext("提示: 切换语言后需要重启应用程序才能生效"), 
                 fg='gray').pack(anchor='w', pady=2)
        
        # 配置键列表框架
        list_frame = tk.Frame(self.parent)
        list_frame.pack(fill='both', expand=True, padx=10, pady=5)
        
        # 列表标签
        tk.Label(list_frame, text=gettext("配置键列表:")).pack(anchor='w')
        
        # 配置键列表
        self.config_listbox = tk.Listbox(list_frame, height=15, selectmode=tk.SINGLE)
        self.config_listbox.pack(fill='both', expand=True, pady=5)
        
        # 滚动条
        scrollbar = tk.Scrollbar(list_frame, orient='vertical', command=self.config_listbox.yview)
        scrollbar.pack(side='right', fill='y')
        self.config_listbox.config(yscrollcommand=scrollbar.set)
        
        # 绑定选择事件
        self.config_listbox.bind('<<ListboxSelect>>', self.on_config_selected)
        
        # 配置值显示区域
        value_frame = tk.Frame(self.parent)
        value_frame.pack(fill='both', expand=True, padx=10, pady=5)
        
        tk.Label(value_frame, text=gettext("配置值:"), font=("Arial", 10)).pack(anchor='w')
        
        self.value_text = tk.Text(value_frame, height=8, width=50, state='disabled')
        self.value_text.pack(fill='both', expand=True)
        
        # 按钮框架
        button_frame = tk.Frame(self.parent)
        button_frame.pack(pady=10)
        
        tk.Button(button_frame, text=gettext("刷新"), command=self.refresh_config_list).pack(side='left', padx=5)
        tk.Button(button_frame, text=gettext("保存配置"), command=self.save_config).pack(side='left', padx=5)
        tk.Button(button_frame, text=gettext("编辑选中项"), command=self.edit_selected).pack(side='left', padx=5)
    
    def refresh_config_list(self):
        """刷新配置键列表"""
        assert self.config_listbox is not None
        self.config_listbox.delete(0, tk.END)
        from Global import config
        for key in sorted(config.keys()):
            self.config_listbox.insert(tk.END, key)
    
    def change_language(self):
        """切换语言"""
        new_lang = self.language_var.get()
        from Global import config, save_config, logger
        
        if new_lang == config.get('Language', 'zh'):
            messagebox.showinfo(gettext("提示"), gettext("语言未更改"))
            return
        
        # 更新配置
        config['Language'] = new_lang
        save_config()
        logger.info(f"语言配置已更改为: {new_lang}")
        
        # 提示用户重启
        messagebox.showinfo(gettext("成功"), 
            gettext("语言已切换为: {}\n请重启应用程序以使更改生效").format(
                gettext("中文") if new_lang == 'zh' else gettext("英文")
            ))
    
    def on_config_selected(self, _):
        """配置项选中时显示值"""
        selection = self.config_listbox.curselection()
        if not selection:
            return
        key = self.config_listbox.get(selection[0])
        from Global import config
        value = config.get(key)
        
        # 显示值
        self.value_text.config(state='normal')
        self.value_text.delete(1.0, tk.END)
        if isinstance(value, (list, dict)):
            import yaml
            value_str = yaml.dump(value, allow_unicode=True, default_flow_style=False)
        else:
            value_str = str(value)
        self.value_text.insert(1.0, value_str)
        self.value_text.config(state='disabled')
    
    def save_config(self):
        """保存配置到文件"""
        try:
            from Global import save_config
            save_config()
            messagebox.showinfo(gettext("成功"), gettext("配置已保存"))
        except Exception as e:
            from Global import logger
            logger.error(f"保存配置时出错: {e}")
            messagebox.showerror(gettext("错误"), gettext("保存配置时出错: {}").format(e))
    
    def edit_selected(self):
        """编辑选中的配置项"""
        selection = self.config_listbox.curselection()
        if not selection:
            messagebox.showwarning(gettext("警告"), gettext("请先选择一个配置项"))
            return
        
        key = self.config_listbox.get(selection[0])
        from Global import config
        value = config.get(key)
        
        # 处理数字类型：使用原有对话框
        if isinstance(value, int):
            new_value = askinteger(gettext("编辑配置"), gettext("输入 {} 的新值:").format(key), initialvalue=value)
            if new_value is None:
                return
            config[key] = new_value
            self.refresh_config_list()
            self.on_config_selected(None)
            messagebox.showinfo(gettext("成功"), gettext("配置项 {} 已更新").format(key))
            return
        elif isinstance(value, float):
            new_value = askfloat(gettext("编辑配置"), gettext("输入 {} 的新值:").format(key), initialvalue=value)
            if new_value is None:
                return
            config[key] = new_value
            self.refresh_config_list()
            self.on_config_selected(None)
            messagebox.showinfo(gettext("成功"), gettext("配置项 {} 已更新").format(key))
            return
        
        # 处理字符串、列表、字典类型：使用大文本输入对话框
        if isinstance(value, (str, list, dict)):
            # 创建大文本编辑对话框
            import yaml
            edit_win = tk.Toplevel(self.parent)
            edit_win.title(gettext("编辑配置: {}").format(key))
            edit_win.geometry("750x500")
            
            # 标签
            tk.Label(edit_win, text=gettext("编辑 {} (支持多行输入):").format(key), font=("Arial", 10)).pack(pady=5)
            
            # 文本区域
            text_frame = tk.Frame(edit_win)
            text_frame.pack(fill='both', expand=True, padx=10, pady=5)
            
            text_widget = tk.Text(text_frame, wrap='word', font=("Courier", 10))
            text_widget.pack(side='left', fill='both', expand=True)
            
            # 滚动条
            scrollbar = tk.Scrollbar(text_frame, orient='vertical', command=text_widget.yview)
            scrollbar.pack(side='right', fill='y')
            text_widget.config(yscrollcommand=scrollbar.set)
            
            # 预填充当前值
            if isinstance(value, str):
                current_text = value
            else:  # list 或 dict
                current_text = yaml.dump(value, allow_unicode=True, default_flow_style=False)
            text_widget.insert(1.0, current_text)
            text_widget.focus_set()
            
            # 按钮框架
            button_frame = tk.Frame(edit_win)
            button_frame.pack(pady=10)
            
            def on_confirm():
                """确认按钮回调"""
                new_text = text_widget.get(1.0, tk.END).strip()
                # 处理输入
                if isinstance(value, str):
                    new_value = new_text
                else:
                    try:
                        new_value = yaml.safe_load(new_text)
                        # 验证类型匹配
                        if isinstance(value, list) and not isinstance(new_value, list):
                            messagebox.showerror(gettext("错误"), gettext("输入必须为YAML列表格式"))
                            return
                        if isinstance(value, dict) and not isinstance(new_value, dict):
                            messagebox.showerror(gettext("错误"), gettext("输入必须为YAML字典格式"))
                            return
                    except yaml.YAMLError:
                        messagebox.showerror(gettext("错误"), gettext("无效的YAML格式"))
                        return
                
                config[key] = new_value
                self.refresh_config_list()
                self.on_config_selected(None)
                edit_win.destroy()
                messagebox.showinfo(gettext("成功"), gettext("配置项 {} 已更新").format(key))
            
            def on_cancel():
                """取消按钮回调"""
                edit_win.destroy()
            
            tk.Button(button_frame, text=gettext("确定"), command=on_confirm).pack(side='left', padx=5)
            tk.Button(button_frame, text=gettext("取消"), command=on_cancel).pack(side='left', padx=5)
            
            # 绑定回车和ESC键
            # edit_win.bind('<Return>', lambda e: on_confirm())
            edit_win.bind('<Escape>', lambda e: on_cancel())
            
            # 让窗口获得焦点
            edit_win.transient(self.parent.winfo_toplevel())
            edit_win.grab_set()
            edit_win.wait_window()
        else:
            messagebox.showinfo(gettext("提示"), gettext("该类型({})的编辑功能尚未实现").format(type(value).__name__))