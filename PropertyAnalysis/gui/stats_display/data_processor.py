from typing import Dict, List, Any, Tuple

import matplotlib

matplotlib.use('TkAgg')  # 使用 Tkinter 后端
import matplotlib.pyplot as plt

# 配置 matplotlib 中文字体支持
plt.rcParams['font.sans-serif'] = ['SimHei', 'Microsoft YaHei', 'SimSun', 'KaiTi', 'FangSong']
plt.rcParams['axes.unicode_minus'] = False  # 解决负号显示问题


class DataType:
    """数据类型枚举"""
    NUMERIC = 'numeric'  # 数值型
    STRING = 'string'  # 字符串型
    BOOLEAN = 'boolean'  # 布尔型
    MIXED = 'mixed'  # 混合型
    EMPTY = 'empty'  # 空数据
    UNSUPPORTED = 'unsupported'  # 不支持的类型（dict, list 等）


class DataProcessor:
    """
    数据处理器

    负责过滤和处理可统计的数据类型（字符串、布尔、整型、浮点数）
    排除不支持统计的复杂类型（dict, list, set 等）
    """

    # 支持统计的基本类型
    SUPPORTED_TYPES = (str, bool, int, float)

    @classmethod
    def is_supported(cls, value: Any) -> bool:
        """
        检查值是否为支持的统计类型

        Args:
            value: 待检查的值

        Returns:
            是否支持统计
        """
        # 注意：bool 是 int 的子类，需要先检查 bool
        if isinstance(value, bool):
            return True
        if isinstance(value, (int, float)) and not isinstance(value, bool):
            return True
        if isinstance(value, str):
            return True
        return False

    @classmethod
    def filter_supported(cls, values: List[Any]) -> Tuple[List[Any], Dict[str, int]]:
        """
        过滤出可统计的值，并统计不支持类型的数量

        Args:
            values: 原始数据列表

        Returns:
            Tuple[List[Any], Dict[str, int]]:
                - 可统计的值列表
                - 不支持类型的统计 {类型名: 数量}
        """
        supported_values: List[Any] = []
        unsupported_counts: Dict[str, int] = {}

        for v in values:
            if cls.is_supported(v):
                supported_values.append(v)
            else:
                type_name = type(v).__name__
                unsupported_counts[type_name] = unsupported_counts.get(type_name, 0) + 1

        return supported_values, unsupported_counts

    @classmethod
    def detect_type(cls, values: List[Any]) -> str:
        """
        检测数据列表的主要类型

        Args:
            values: 数据值列表（应为已过滤的可统计值）

        Returns:
            数据类型字符串
        """
        if not values:
            return DataType.EMPTY

        numeric_count = 0
        string_count = 0
        bool_count = 0

        for v in values:
            if isinstance(v, bool):
                bool_count += 1
            elif isinstance(v, (int, float)) and not isinstance(v, bool):
                numeric_count += 1
            elif isinstance(v, str):
                # 尝试判断是否为数值字符串
                try:
                    float(v)
                    numeric_count += 1
                except (ValueError, TypeError):
                    string_count += 1

        total = len(values)

        # 判断主要类型
        if numeric_count == total:
            return DataType.NUMERIC
        elif string_count == total:
            return DataType.STRING
        elif bool_count == total:
            return DataType.BOOLEAN
        else:
            return DataType.MIXED

    @classmethod
    def convert_to_numeric(cls, values: List[Any]) -> List[float]:
        """
        将数据列表转换为数值类型

        Args:
            values: 原始数据列表（应为已过滤的可统计值）

        Returns:
            数值列表
        """
        result = []
        for v in values:
            try:
                if isinstance(v, bool):
                    result.append(float(v))
                elif isinstance(v, (int, float)) and not isinstance(v, bool):
                    result.append(float(v))
                elif isinstance(v, str):
                    result.append(float(v))
            except (ValueError, TypeError):
                continue
        return result

    @classmethod
    def to_hashable(cls, value: Any) -> Any:
        """
        将值转换为可哈希的形式用于频率统计

        Args:
            value: 原始值（应为已过滤的可统计值）

        Returns:
            可哈希的值
        """
        # 已过滤的值都是可哈希的，直接返回
        return value


def convert_to_numeric(values: List[Any]) -> List[float]:
    """将数据列表转换为数值类型（兼容函数）"""
    return DataProcessor.convert_to_numeric(values)