import pickle
from typing import Set, Tuple
from models.stats_struct import *
from dataLoader import LoadsFormFolder


class StatsManager:
    def __init__(self, data: List[Dict[str,Any]]):
        self.data: Dict[str, StatsStruct] = dict()
        # 缓存已存在所有物品类型名称
        self._base_classes_cache: Optional[Set[str]] = None
        # 储存详细类型拥有的所有物品属性名称
        self._base_classes_to_prop_keys: Dict[str, Set[str]] = dict()
        # 存储每个物品类型独有的属性
        self._unique_props_per_type: Dict[str, Set[str]]
        # 所有物品加其他有的属性名称
        self._prop_keys: Set[str] = set()
        for union_data in data:
            stats_struct = StatsStruct(union_data, union_data.get('cacheName', None))
            if stats_struct.data is not None:
                self.data[stats_struct.name] = stats_struct
                self._prop_keys.update(stats_struct.statistical_data.keys())
                self._base_classes_to_prop_keys[stats_struct.name] = set(stats_struct.statistical_data.keys())
        ## 统计唯一值
        _base_classes_to_prop_keys_values: List[Tuple[str, Set[str]]] = list(self._base_classes_to_prop_keys.items())

        self._unique_props_per_type = {
            # 获取不是the_type_name类型下其他所有集合并拆成单个元素后再合并集合, 随后利用差集的性质获取唯一的属性
            the_type_name: value_set - set().union(
                    *[s for k, s in _base_classes_to_prop_keys_values if k != the_type_name]
                )
            for the_type_name, value_set
            in self._base_classes_to_prop_keys.items()
        }

    @classmethod
    async def create_from_folder(cls, cache_folder_path: str) -> Optional['StatsManager']:
        if not Path(cache_folder_path).exists():
            return None
        else:
            data = await LoadsFormFolder(cache_folder_path)
            return StatsManager(data)

    @property
    def base_classes(self) -> Set[str]:
        """获取所有已加载的类型(BaseClasses)名称"""
        if self._base_classes_cache is None:
            self._base_classes_cache = set(self.data.keys())
        return self._base_classes_cache

    @property
    def prop_keys(self) -> Set[str]:
        """获取所有存在的属性(Props)名称"""
        return self._prop_keys

    def get_prop_keys(self, target_base_classes: str) -> Optional[Set[str]]:
        """获取指定物品类型有的所有属性名称"""
        return self._base_classes_to_prop_keys.get(target_base_classes, None)

    def get_unique_prop_keys(self, target_base_classes: str) -> Optional[Set[str]]:
        """获取指定物品类型有的唯一属性名称"""
        return self._unique_props_per_type.get(target_base_classes, None)

    def save_to_file(self, target_file_path: str) -> None:
        with open(target_file_path, mode='wb') as f:
            logger.debug(f'[StatsManager] 已保存统计数据到{target_file_path}')
            pickle.dump(self, f)

    @staticmethod
    def create_from_file(target_file_path: str) -> 'StatsManager':
        with open(target_file_path, mode='rb') as f:
            logger.debug(f'[StatsManager] 已从{target_file_path}加载统计数据')
            return pickle.load(f)



if __name__ == '__main__':
    save_file_path = config.get('StatsManagerSavePath')

    folder_path: List[str] = config.get('DataLoaderTestFolder', ["data"])

    # 保存
    # stats = asyncio.run(StatsManager.create_from_folder(folder_path[0]))
    # stats.save_to_file(save_file_path)
    # 加载
    stats = StatsManager.create_from_file(save_file_path)


    test_types = [
        'DRINK',
        'FUEL',
        'MAP'
    ]

    for type_name in test_types:
        print(f'{type_name}有的属性:')
        print(f'\t> 唯一: {", ".join(stats.get_unique_prop_keys(type_name))}')
        print(f'\t> 全部: {", ".join(stats.get_prop_keys(type_name))}')

    print(f'存在的所有baseClasses: {stats.base_classes}')

    print('所有属性中存在唯一键的情况:')
    for base_classes in stats.base_classes:
        unique_prop_keys = stats.get_unique_prop_keys(base_classes)
        if unique_prop_keys is not None and len(unique_prop_keys) > 0:
            print(f'- {base_classes}: {", ".join(unique_prop_keys)}')


