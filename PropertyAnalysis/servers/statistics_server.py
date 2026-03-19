import math
import statistics
from typing import List, Union, Tuple, Dict
from Global import logger


def calculate_basic_statistics(data: Union[List[Union[int, float]], Tuple[Union[int, float], ...]]) -> Dict[str, float]:
    """
    计算数值数据的基本统计量
    
    Args:
        data: 数值列表或元组
        
    Returns:
        包含统计量的字典: 均值, 标准差, 方差, 最大值, 最小值, 中位数, 总和, 数量
    """
    if not data:
        logger.warning('传入空数据列表')
        return {}
    
    # 转换为浮点数列表以便计算
    values = [float(x) for x in data]
    
    try:
        mean_value = statistics.mean(values)
        std_ev_value = statistics.stdev(values) if len(values) > 1 else 0.0
        variance_value = statistics.variance(values) if len(values) > 1 else 0.0
        max_value = max(values)
        min_value = min(values)
        median_value = statistics.median(values)
        sum_value = sum(values)
        count = len(values)
        
        # 可选: 四分位数
        if len(values) >= 4:
            q1 = statistics.quantiles(values, n=4)[0]
            q3 = statistics.quantiles(values, n=4)[2]
            iqr = q3 - q1
        else:
            q1 = q3 = iqr = 0.0
        
        result = {
            'mean': mean_value,
            'std_dev': std_ev_value,
            'variance': variance_value,
            'max': max_value,
            'min': min_value,
            'median': median_value,
            'sum': sum_value,
            'count': count,
            'q1': q1,
            'q3': q3,
            'iqr': iqr
        }
        
        logger.debug(f'统计计算完成: 数据长度={count}, 均值={mean_value:.4f}, 标准差={std_ev_value:.4f}')
        return result
        
    except Exception as e:
        logger.error(f'统计计算失败: {e}', exc_info=True)
        return {}


def calculate_extended_statistics(data: Union[List[Union[int, float]], Tuple[Union[int, float], ...]]) -> Dict[str, float]:
    """
    计算扩展统计量，包括偏度、峰度等
    
    Args:
        data: 数值列表或元组
        
    Returns:
        包含扩展统计量的字典
    """
    if len(data) < 3:
        logger.warning('数据不足，无法计算扩展统计量')
        return {}
    
    values = [float(x) for x in data]
    n = len(values)
    mean_value = statistics.mean(values)
    
    # 计算中心矩
    m2 = sum((x - mean_value) ** 2 for x in values) / n
    m3 = sum((x - mean_value) ** 3 for x in values) / n
    m4 = sum((x - mean_value) ** 4 for x in values) / n
    
    # 偏度 (Fisher-Pearson 系数)
    if m2 == 0:
        skewness = 0.0
    else:
        skewness = m3 / (m2 ** 1.5)
    
    # 峰度 (Fisher 定义，正态分布为0)
    if m2 == 0:
        kurtosis = -3.0  # 定义值
    else:
        kurtosis = m4 / (m2 ** 2) - 3
    
    # 变异系数
    cv = math.sqrt(m2) / mean_value if mean_value != 0 else float('inf')
    
    result = {
        'skewness': skewness,
        'kurtosis': kurtosis,
        'coefficient_of_variation': cv,
        'second_moment': m2,
        'third_moment': m3,
        'fourth_moment': m4
    }
    
    logger.debug(f'扩展统计计算完成: 偏度={skewness:.4f}, 峰度={kurtosis:.4f}')
    return result


class StatisticsServer:
    """统计服务类，提供统计计算方法"""
    
    def __init__(self):
        self.logger = logger
        
    def get_statistics(self, data: Union[List[Union[int, float]], Tuple[Union[int, float], ...]]) -> Dict[str, Dict[str, float]]:
        """
        获取完整统计信息
        
        Args:
            data: 数值列表或元组
            
        Returns:
            包含基本统计量和扩展统计量的字典
        """
        basic = calculate_basic_statistics(data)
        extended = calculate_extended_statistics(data)
        
        return {
            'basic_statistics': basic,
            'extended_statistics': extended
        }
    
    def describe(self, data: Union[List[Union[int, float]], Tuple[Union[int, float], ...]]) -> str:
        """
        生成统计描述字符串
        
        Args:
            data: 数值列表或元组
            
        Returns:
            格式化的统计描述
        """
        stats = self.get_statistics(data)
        basic = stats.get('basic_statistics', {})
        
        if not basic:
            return "无有效统计数据"
        
        lines = [
            "=== 统计描述 ===",
            f"数量: {basic.get('count', 0)}",
            f"均值: {basic.get('mean', 0):.4f}",
            f"标准差: {basic.get('std_dev', 0):.4f}",
            f"方差: {basic.get('variance', 0):.4f}",
            f"最小值: {basic.get('min', 0):.4f}",
            f"最大值: {basic.get('max', 0):.4f}",
            f"中位数: {basic.get('median', 0):.4f}",
            f"Q1: {basic.get('q1', 0):.4f}",
            f"Q3: {basic.get('q3', 0):.4f}",
            f"IQR: {basic.get('iqr', 0):.4f}",
        ]
        
        return '\n'.join(lines)