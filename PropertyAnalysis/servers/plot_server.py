import matplotlib
import base64
import io
from typing import List, Dict, Any, Union, Tuple, Optional
from enum import Enum
from Global import logger
matplotlib.use('Agg')  # 使用非交互式后端
import matplotlib.pyplot as plt
import numpy as np


class PlotType(Enum):
    """绘图类型枚举"""
    LINE = 'line'
    BAR = 'bar'
    SCATTER = 'scatter'
    HISTOGRAM = 'histogram'
    BOXPLOT = 'boxplot'
    PIE = 'pie'
    HEATMAP = 'heatmap'


class PlotServer:
    """绘图服务类，提供MATLAB风格的绘图功能"""
    
    def __init__(self, style: str = 'default'):
        """
        初始化绘图服务器
        
        Args:
            style: Matplotlib样式名称，如"default", 'ggplot', 'seaborn'
        """
        self.style = style
        self.logger = logger
    
    def _create_figure(self, fig_size: Tuple[int, int] = (10, 6), dpi: int = 100) -> Optional[Any]:
        """创建图形对象"""
        try:
            plt.style.use(self.style)
            fig, ax = plt.subplots(fig_size=fig_size, dpi=dpi)
            return fig, ax
        except Exception as e:
            self.logger.error(f'创建图形失败: {e}', exc_info=True)
            return None
    
    def plot_line(self, x_data: List[Union[int, float]], y_data: List[Union[int, float]], 
                  title: str = '折线图', x_label: str = 'X轴', y_label: str = 'Y轴',
                  fig_size: Tuple[int, int] = (10, 6), dpi: int = 100,
                  return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        绘制折线图
        
        Args:
            x_data: X轴数据
            y_data: Y轴数据
            title: 图表标题
            x_label: X轴标签
            y_label: Y轴标签
            fig_size: 图形大小 (宽度, 高度)
            dpi: 分辨率
            return_format: 返回格式，'bytes' 或 'base64'
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        if len(x_data) != len(y_data):
            self.logger.error(f'X轴和Y轴数据长度不一致: {len(x_data)} != {len(y_data)}')
            return None
        
        fig, ax = self._create_figure(fig_size, dpi)
        if fig is None:
            return None
        
        try:
            ax.plot(x_data, y_data, marker='o', linestyle='-', linewidth=2)
            ax.set_title(title, fontsize=14, fontweight='bold')
            ax.set_x_label(x_label, fontsize=12)
            ax.set_y_label(y_label, fontsize=12)
            ax.grid(True, alpha=0.3)
            ax.tick_params(axis='both', which='major', labelsize=10)
            
            return self._save_figure(fig, return_format)
        finally:
            plt.close(fig)
    
    def plot_bar(self, categories: List[str], values: List[Union[int, float]],
                 title: str = '柱状图', x_label: str = '类别', y_label: str = '数值',
                 fig_size: Tuple[int, int] = (10, 6), dpi: int = 100,
                 return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        绘制柱状图
        
        Args:
            categories: 类别标签
            values: 对应数值
            title: 图表标题
            x_label: X轴标签
            y_label: Y轴标签
            fig_size: 图形大小
            dpi: 分辨率
            return_format: 返回格式
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        if len(categories) != len(values):
            self.logger.error(f'类别和数值长度不一致: {len(categories)} != {len(values)}')
            return None
        
        fig, ax = self._create_figure(fig_size, dpi)
        if fig is None:
            return None
        
        try:
            bars = ax.bar(categories, values, color='steelblue', alpha=0.8)
            ax.set_title(title, fontsize=14, fontweight='bold')
            ax.set_x_label(x_label, fontsize=12)
            ax.set_y_label(y_label, fontsize=12)
            
            # 在柱子上方显示数值
            for bar in bars:
                height = bar.get_height()
                ax.text(bar.get_x() + bar.get_width() / 2., height,
                       f'{height:.2f}', ha='center', va='bottom', fontsize=9)
            
            ax.tick_params(axis='x', rotation=45)
            ax.grid(True, alpha=0.3, axis='y')
            
            return self._save_figure(fig, return_format)
        finally:
            plt.close(fig)
    
    def plot_histogram(self, data: List[Union[int, float]], bins: Optional[int] = None,
                       title: str = '直方图', x_label: str = '数值', y_label: str = '频数',
                       fig_size: Tuple[int, int] = (10, 6), dpi: int = 100,
                       return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        绘制直方图
        
        Args:
            data: 数值数据
            bins: 分组数量，None则自动计算
            title: 图表标题
            x_label: X轴标签
            y_label: Y轴标签
            fig_size: 图形大小
            dpi: 分辨率
            return_format: 返回格式
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        fig, ax = self._create_figure(fig_size, dpi)
        if fig is None:
            return None
        
        try:
            ax.hist(data, bins=bins, color='skyblue', edgecolor='black', alpha=0.7)
            ax.set_title(title, fontsize=14, fontweight='bold')
            ax.set_x_label(x_label, fontsize=12)
            ax.set_y_label(y_label, fontsize=12)
            ax.grid(True, alpha=0.3)
            
            # 添加统计信息
            if len(data) > 0:
                stats_text = f'数量: {len(data)}\n均值: {np.mean(data):.2f}\n标准差: {np.std(data):.2f}'
                ax.text(0.95, 0.95, stats_text, transform=ax.transAxes,
                       fontsize=10, verticalalignment='top',
                       horizontalalignment='right',
                       bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.5))
            
            return self._save_figure(fig, return_format)
        finally:
            plt.close(fig)
    
    def plot_scatter(self, x_data: List[Union[int, float]], y_data: List[Union[int, float]],
                     title: str = '散点图', x_label: str = 'X轴', y_label: str = 'Y轴',
                     fig_size: Tuple[int, int] = (10, 6), dpi: int = 100,
                     return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        绘制散点图
        
        Args:
            x_data: X轴数据
            y_data: Y轴数据
            title: 图表标题
            x_label: X轴标签
            y_label: Y轴标签
            fig_size: 图形大小
            dpi: 分辨率
            return_format: 返回格式
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        if len(x_data) != len(y_data):
            self.logger.error(f'X轴和Y轴数据长度不一致: {len(x_data)} != {len(y_data)}')
            return None
        
        fig, ax = self._create_figure(fig_size, dpi)
        if fig is None:
            return None
        
        try:
            _ = ax.scatter(x_data, y_data, c='coral', alpha=0.6, edgecolors='w', linewidth=0.5)
            ax.set_title(title, fontsize=14, fontweight='bold')
            ax.set_x_label(x_label, fontsize=12)
            ax.set_y_label(y_label, fontsize=12)
            ax.grid(True, alpha=0.3)
            
            return self._save_figure(fig, return_format)
        finally:
            plt.close(fig)
    
    def plot_boxplot(self, data: List[List[Union[int, float]]], labels: Optional[List[str]] = None,
                     title: str = '箱线图', y_label: str = '数值',
                     fig_size: Tuple[int, int] = (10, 6), dpi: int = 100,
                     return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        绘制箱线图
        
        Args:
            data: 数据列表的列表，每个子列表代表一个数据集
            labels: 每个数据集的标签
            title: 图表标题
            y_label: Y轴标签
            fig_size: 图形大小
            dpi: 分辨率
            return_format: 返回格式
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        fig, ax = self._create_figure(fig_size, dpi)
        if fig is None:
            return None
        
        try:
            bp = ax.boxplot(data, labels=labels, patch_artist=True)
            
            # 设置箱线图颜色
            colors = ['lightblue', 'lightgreen', 'lightcoral', 'lightsalmon', 'lightyellow']
            for patch, color in zip(bp['boxes'], colors):
                patch.set_facecolor(color)
            
            ax.set_title(title, fontsize=14, fontweight='bold')
            ax.set_y_label(y_label, fontsize=12)
            ax.grid(True, alpha=0.3, axis='y')
            
            return self._save_figure(fig, return_format)
        finally:
            plt.close(fig)
    
    def _save_figure(self, fig: Any, return_format: str = 'bytes') -> Optional[Union[bytes, str]]:
        """
        保存图形为指定格式
        
        Args:
            fig: matplotlib图形对象
            return_format: 返回格式，'bytes' 或 'base64'
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        buf = io.BytesIO()
        try:
            fig.savefig(buf, format='png', dpi=fig.dpi, bbox_inches='tight')
            buf.seek(0)
            
            if return_format == 'bytes':
                return buf.read()
            elif return_format == 'base64':
                image_bytes = buf.read()
                return base64.b64encode(image_bytes).decode('utf-8')
            else:
                self.logger.warning(f'不支持的返回格式: {return_format}')
                return None
        except Exception as e:
            self.logger.error(f'保存图形失败: {e}', exc_info=True)
            return None
        finally:
            buf.close()
    
    def plot_from_data(self, plot_type: PlotType, data: Dict[str, Any],
                       **kwargs) -> Optional[Union[bytes, str]]:
        """
        通用绘图方法，根据绘图类型和数据自动选择绘图函数
        
        Args:
            plot_type: 绘图类型
            data: 绘图数据字典
            **kwargs: 其他参数传递给具体绘图函数
            
        Returns:
            二进制图像数据或Base64编码字符串
        """
        if plot_type == PlotType.LINE:
            return self.plot_line(
                data.get('x', []),
                data.get('y', []),
                **kwargs
            )
        elif plot_type == PlotType.BAR:
            return self.plot_bar(
                data.get('categories', []),
                data.get('values', []),
                **kwargs
            )
        elif plot_type == PlotType.HISTOGRAM:
            return self.plot_histogram(
                data.get('data', []),
                **kwargs
            )
        elif plot_type == PlotType.SCATTER:
            return self.plot_scatter(
                data.get('x', []),
                data.get('y', []),
                **kwargs
            )
        elif plot_type == PlotType.BOXPLOT:
            return self.plot_boxplot(
                data.get('data', []),
                labels=data.get('labels'),
                **kwargs
            )
        else:
            self.logger.warning(f'尚未支持的绘图类型: {plot_type}')
            return None
    
    def create_multiple_plots(self, plots_config: List[Dict[str, Any]]) -> Dict[str, Union[bytes, str]]:
        """
        创建多个图表
        
        Args:
            plots_config: 绘图配置列表，每个配置包含plot_type和data
            
        Returns:
            字典：键为配置索引，值为图像数据
        """
        results = {}
        
        for i, config in enumerate(plots_config):
            plot_type = config.get('plot_type')
            data = config.get('data', {})
            return_format = config.get('return_format', 'bytes')
            
            if isinstance(plot_type, str):
                try:
                    plot_type = PlotType(plot_type)
                except ValueError:
                    self.logger.warning(f'无效的绘图类型字符串: {plot_type}')
                    continue
            
            if not isinstance(plot_type, PlotType):
                self.logger.warning(f'无效的绘图类型: {plot_type}')
                continue
            
            result = self.plot_from_data(plot_type, data, return_format=return_format)
            if result:
                results[str(i)] = result
        
        return results