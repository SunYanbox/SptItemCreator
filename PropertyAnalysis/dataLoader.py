import json
import asyncio
import aiofiles
from Global import logger, config
from pathlib import Path
from typing import Any, Dict, List, Optional
from models.prop_field import PropertyField

class DataJudge:
    __top_keys = ['savePath', 'statisticalData', 'handledItems', 'handleBaseClasses', 'cacheName']

    @staticmethod
    def is_right_top_dict(load_dict: Dict[str, Any]) -> bool:
        """判断反序列化的字典首层是否符合正确的缓存键"""
        return set(load_dict.keys()).issuperset(DataJudge.__top_keys)

    @staticmethod
    def is_valid_mongo_id(mongo_id: str) -> bool:
        """
        判断字符串是否是有效MongoId
        迁移于: https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Extensions/MongoIdExtensions.cs
        """
        # 检查长度是否为 24
        if len(mongo_id) != 24:
            return False

        # 检查每个字符是否为十六进制字符
        for c in mongo_id:
            # 判断是否为十六进制字符 (0-9, a-f, A-F)
            is_hex = (
                    ('0' <= c <= '9') or
                    ('a' <= c <= 'f') or
                    ('A' <= c <= 'F')
            )

            if not is_hex:
                return False

        return True

    @staticmethod
    def is_right_dict(load_dict: Dict[str, Any]) -> bool:
        """判断反序列化的字典是否符合正确的缓存键"""
        load_dict_id: int = id(load_dict)
        if not DataJudge.is_right_top_dict(load_dict):
            logger.warning(f'字典(id={load_dict_id})完全不满足SptItemCreator.StatsCache的数据结构: 首层键异常: [{", ".join(load_dict.keys())}]')
            return False
        # save_path
        save_path = load_dict.get("savePath")
        if not isinstance(save_path, str) or not Path.exists(Path(save_path)):
            logger.warning(
                f'字典(id={load_dict_id})[\"savePath\"]的路径不存在: {save_path}')
            return False
        # handleBaseClasses
        handle_base_classes = load_dict.get("handleBaseClasses")
        if not isinstance(handle_base_classes, str) or not DataJudge.is_valid_mongo_id(handle_base_classes):
            logger.warning(
                f'字典(id={load_dict_id})[\"handleBaseClasses\"]不是合理的MongoId: {handle_base_classes}')
            return False
        # cacheName
        cache_name = load_dict.get("cacheName")
        if not isinstance(cache_name, str):
            logger.warning(f'字典(id={load_dict_id})[\"cacheName\"]不是字符串: {type(cache_name)}({cache_name})')
            return False
        # handledItems
        handled_items = load_dict.get("handledItems")
        if not isinstance(handled_items, list):
            logger.warning(f'字典(id={load_dict_id})[\"handledItems\"]不是列表: {type(handled_items)}([{",".join(handled_items)}])')
            return False
        if any([not isinstance(handled_item, str) or not DataJudge.is_valid_mongo_id(handled_item) for handled_item in handled_items]):
            logger.warning(f'字典(id={load_dict_id})[\"handledItems\"]存在值不是字符串类型或不是有效MongoId: {type(handled_items)}([{",".join(handled_items)}])')
            return False
        # statisticalData
        statistical_data = load_dict.get("statisticalData")
        # if type(statistical_data) != dict[str, dict[str, list]]:
        if any([
            # 检查是否为字典
            not isinstance(statistical_data, dict),
            # 检查第一层所有键是否为字符串
            any(not isinstance(k, str) for k in statistical_data.keys()),
            # 检查第一层所有值是否为字典
            any(not isinstance(v, dict) for v in statistical_data.values()),
            # 检查第二层(内层字典)的结构
            any(
                # 第二层键检查(必须是字符串)
                any(not isinstance(prop_key, str) for prop_key in inner_dict.keys())
                # 第二层值检查(必须是列表)
                or any(not isinstance(value_list, list) for value_list in inner_dict.values())
                # PropertyField 验证(键必须在 PropertyField 中)
                or any(prop_key not in PropertyField for prop_key in inner_dict.keys())
                for inner_dict in statistical_data.values()
            )

        ]):
            logger.warning(
                f'字典(id={load_dict_id})[\"statisticalData\"]不是Type(dict[str, dict[str, list]])类型: {type(statistical_data)}(Count={len(statistical_data)})')
            return False
        for k, v in statistical_data.items():
            for prop_key, prop_value_list in v.items():
                if prop_key not in PropertyField:
                    logger.warning(
                        f'字典(id={load_dict_id})[\"statisticalData\"][\"{k}\"]的属性键不是PropertyField支持的值: {prop_key}')
                    return False
        return True

async def LoadFromFile(file_path: str) -> Optional[Dict[str, Any]]:
    """异步读取文件并解析为Dict对象"""
    if not Path.exists(Path(file_path)):
        raise FileNotFoundError(file_path)
    async with aiofiles.open(file_path, mode='r', encoding='utf-8') as f:
        content = await f.read()
        load_dict: Dict[str, Any] = json.loads(content)
        if not DataJudge.is_right_dict(load_dict):
            # logger.warning(f'文件{file_path}不是SptItemCreator创建的缓存文件, 无法加载')
            return None
        return load_dict

async def LoadsFormFolder(folder_path=".") -> List[Dict[str, Any]]:
    """从文件夹路径加载数据(仅限SptItemCreator创建的缓存文件)"""
    path = Path(folder_path)
    json_files = list(str(p.absolute()) for p in path.glob("*.json") if not p.name.endswith('hash.json'))
    tasks = [LoadFromFile(p) for p in json_files]
    results = await asyncio.gather(*tasks)
    objs = [r for r in results if r is not None]
    logger.debug(f'在路径{folder_path}成功加载{len(objs)}条缓存文件')
    return objs

_data: Optional[List[Dict[str, Any]]] = None

async def GetAllData():
    """根据config.yaml设置的路径获取所有数据"""
    global _data
    if _data is None:
        _data = await LoadsFormFolder(config.get("SptItemCreatorStatsCacheFolderPath"))
    return _data

if __name__ == "__main__":
    for file_path in config.get('DataLoaderTestFilePaths', []):
        try:
            a: Dict[str, Any] = asyncio.run(LoadFromFile(file_path))
            print(f'{a.__class__.__name__}: {a}')
        except Exception as e:
            print(f'文件{file_path}解析时出现错误: {e}')
    
    for folder_path in config.get('DataLoaderTestFolder', []):
        try:
            a: List[Dict[str, Any]] = asyncio.run(LoadsFormFolder(folder_path))
            for b in a:
                # 类型有的属性名称
                print(f'{b.get('cacheName')}: {len(b.get('statisticalData'))}')
        except Exception as e:
            print(f'路径{folder_path}解析时出现错误: {e}')
    