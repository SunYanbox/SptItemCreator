import math
from typing import Dict, List, Union, Any, Counter
from collections import Counter
from Global import logger

ValueType = Union[int, float, bool, str]


def count_label_frequencies(data: Dict[str, List[ValueType]]) -> Dict[str, Dict[Union[str, int, float, bool], int]]:
    """
    统计字典中每个标签下值的频率分布
    
    Args:
        data: 字典，键为标签名，值为值列表（可包含int, float, bool, str）
        
    Returns:
        嵌套字典：外层键为标签名，内层字典键为值，值为出现次数
        
    Note:
        会自动过滤不可哈希类型（dict, list, set等），仅统计基本类型
    """
    if not data:
        logger.warning('传入空字典')
        return {}
    
    result = {}
    
    for label, values in data.items():
        if not isinstance(values, list):
            logger.warning(f'标签"{label}"的值不是列表类型: {type(values)}')
            continue
        
        if not values:
            logger.debug(f'标签"{label}"的值为空列表')
            result[label] = {}
            continue
        
        # 防御性过滤：只保留可哈希的基本类型
        hashable_values = []
        unhashable_count = 0
        for v in values:
            # 注意：bool 是 int 的子类，需要先检查 bool
            if isinstance(v, bool):
                hashable_values.append(v)
            elif isinstance(v, (int, float, str)) and not isinstance(v, bool):
                hashable_values.append(v)
            else:
                unhashable_count += 1
        
        if unhashable_count > 0:
            logger.debug(f'标签"{label}"过滤了{unhashable_count}个不可哈希值')
        
        if not hashable_values:
            logger.debug(f'标签"{label}"无可统计值')
            result[label] = {}
            continue
        
        # 统计频率
        counter = Counter(hashable_values)
        # 将Counter转换为普通字典
        freq_dict = dict(counter)
        
        result[label] = freq_dict
        
        logger.debug(f'标签"{label}"频率统计完成: 唯一值数量={len(freq_dict)}')
    
    return result


def get_top_frequencies(data: Dict[str, List[ValueType]], top_n: int = 10) -> Dict[str, List[tuple]]:
    """
    获取每个标签出现频率最高的前N个值
    
    Args:
        data: 字典，键为标签名，值为值列表
        top_n: 要返回的前N个高频值
        
    Returns:
        字典：键为标签名，值为(值, 频数)的列表，按频数降序排列
    """
    frequencies = count_label_frequencies(data)
    result = {}
    
    for label, freq_dict in frequencies.items():
        # 按频数降序排序
        sorted_items = sorted(freq_dict.items(), key=lambda x: x[1], reverse=True)
        # 取前top_n个
        top_items = sorted_items[:top_n]
        result[label] = top_items
    
    return result


def calculate_label_statistics(data: Dict[str, List[ValueType]]) -> Dict[str, Dict[str, Any]]:
    """
    计算每个标签的统计摘要
    
    Args:
        data: 字典，键为标签名，值为值列表
        
    Returns:
        字典：键为标签名，值为包含统计信息的字典
    """
    frequencies = count_label_frequencies(data)
    result = {}
    
    for label, freq_dict in frequencies.items():
        if not freq_dict:
            result[label] = {
                'unique_count': 0,
                'total_count': 0,
                'most_common': None,
                'most_common_count': 0,
                'entropy': 0.0
            }
            continue
        
        total_count = sum(freq_dict.values())
        unique_count = len(freq_dict)
        
        # 找出最常见的值
        most_common_value = max(freq_dict.items(), key=lambda x: x[1])
        most_common, most_common_count = most_common_value
        
        # 计算熵（信息熵）
        entropy = 0.0
        for count in freq_dict.values():
            probability = count / total_count
            entropy -= probability * math.log2(probability) if probability > 0 else 0
        
        result[label] = {
            'unique_count': unique_count,
            'total_count': total_count,
            'most_common': most_common,
            'most_common_count': most_common_count,
            'most_common_percentage': most_common_count / total_count * 100,
            'entropy': entropy
        }
    
    return result


def merge_frequencies(freq_dicts: List[Dict[str, Dict[Union[str, int, float, bool], int]]]) -> Dict[str, Dict[Union[str, int, float, bool], int]]:
    """
    合并多个频率字典
    
    Args:
        freq_dicts: 频率字典列表
        
    Returns:
        合并后的频率字典
    """
    if not freq_dicts:
        return {}
    
    merged = {}
    
    for freq_dict in freq_dicts:
        for label, label_freq in freq_dict.items():
            if label not in merged:
                merged[label] = {}
            
            for value, count in label_freq.items():
                merged[label][value] = merged[label].get(value, 0) + count
    
    return merged


class FrequencyServer:
    """频率统计服务类"""
    
    def __init__(self):
        self.logger = logger
    
    def analyze(self, data: Dict[str, List[ValueType]]) -> Dict[str, Any]:
        """
        完整分析字典数据
        
        Args:
            data: 字典，键为标签名，值为值列表
            
        Returns:
            包含所有分析结果的字典
        """
        frequencies = count_label_frequencies(data)
        top_frequencies = get_top_frequencies(data)
        statistics = calculate_label_statistics(data)
        
        return {
            'frequencies': frequencies,
            'top_frequencies': top_frequencies,
            'statistics': statistics
        }
    
    def export_to_csv_format(self, data: Dict[str, List[ValueType]]) -> str:
        """
        将频率数据导出为CSV格式字符串
        
        Args:
            data: 字典，键为标签名，值为值列表
            
        Returns:
            CSV格式字符串
        """
        frequencies = count_label_frequencies(data)
        
        lines = ['label,value,count,percentage']
        
        for label, freq_dict in frequencies.items():
            total = sum(freq_dict.values())
            for value, count in freq_dict.items():
                percentage = count / total * 100 if total > 0 else 0
                # 转义特殊字符
                value_str = str(value).replace('"', '""')
                lines.append(f'{label},"{value_str}",{count},{percentage:.2f}')
        
        return '\n'.join(lines)
