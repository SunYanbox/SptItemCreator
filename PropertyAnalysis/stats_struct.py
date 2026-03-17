from pathlib import Path
from Global import logger, config
from dataLoader import DataJudge, LoadFromFile
from typing import Any, Dict, List, Optional, Union

class StatsStruct:
    """
    从SptItemCreator.Core.Cache迁移的类型
    对应模组版本: >=0.1.0
    """
    def __init__(self, data: Dict[str, Any], name: Optional[str] = None):
        self._name: Optional[str] = name
        self._dict_id: int = id(data)
        if DataJudge.is_right_dict(data):
            _data = data
            if self._name is None:
                self._name: str = _data.get("cacheName")
        else:
            logger.error(f'{str(self)}初始化时传入的字典(id={self._dict_id})不是合法的SptItemCreator.StatsCache缓存数据, 请查看data/PropertyAnalysis.log')
            _data = None
        self._data: Optional[Dict[str, Union[str, List[str], Dict[str, Dict[str, List[Any]]]]]] = _data

    @classmethod
    async def create_from_file(cls, file_path: str, name: Optional[str] = None) -> Optional['StatsStruct']:
        if not Path(file_path).exists():
            return None
        else:
            data = await LoadFromFile(file_path)
            return StatsStruct(data, name)

    @property
    def data(self) -> Optional[Dict[str, Union[str, List[str], Dict[str, Dict[str, List[Any]]]]]]:
        return self._data

    @property
    def name(self):
        return self._name

    def __str__(self):
        return f'StatsStruct(name={self.name}, id={id(self)})'

if __name__ == '__main__':
    import asyncio

    for file_path in config.get('DataLoaderTestFilePaths', []):
        try:
            a = asyncio.run(StatsStruct.create_from_file(file_path))
            print(f'{a.__class__.__name__}: {a}')
        except Exception as e:
            print(f'文件{file_path}解析时出现错误: {e}')

