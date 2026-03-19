#!/usr/bin/env python3
"""
PropertyAnalysis 主入口点

此文件启动 Tkinter GUI 应用程序。
"""

import sys
import os
import tkinter as tk
from tkinter import ttk, messagebox

import matplotlib
matplotlib.use('TkAgg')  # 必须在导入 pyplot 之前设置后端
import matplotlib.pyplot as plt

# 添加项目根目录到 Python 路径
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from Global import logger
from gui.main_gui import MainApplication


def setup_environment():
    """设置运行环境"""
    # 确保必要的目录存在
    data_dir = os.path.join(os.path.dirname(__file__), "data")
    if not os.path.exists(data_dir):
        os.makedirs(data_dir)
        logger.info(f"创建数据目录: {data_dir}")
    
    # 检查配置文件
    config_path = os.path.join(os.path.dirname(__file__), "config.yaml")
    if not os.path.exists(config_path):
        logger.warning(f"配置文件不存在: {config_path}")


def main():
    """主函数"""
    root = None
    try:
        # 环境设置
        setup_environment()
        
        # 创建主窗口
        root = tk.Tk()
        
        # 设置窗口样式
        style = ttk.Style()
        try:
            style.theme_use('clam')  # 使用 'clam' 主题，兼容性较好
        except:
            logger.warning("无法设置 'clam' 主题，使用默认主题")
        
        # 创建应用程序
        app = MainApplication(root)
        
        logger.info("PropertyAnalysis GUI 启动成功")
        
        # 启动主循环
        root.mainloop()
        
    except Exception as e:
        logger.error(f"应用程序启动失败: {e}", exc_info=True)
        # 显示错误对话框
        error_msg = f"应用程序启动失败:\n{str(e)}"
        messagebox.showerror("启动错误", error_msg)
        sys.exit(1)
    finally:
        # 清理 matplotlib 资源，确保进程能正常退出
        plt.close('all')
        if root is not None:
            try:
                root.quit()
            except tk.TclError:
                pass


if __name__ == '__main__':
    main()
