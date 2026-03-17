import json
import asyncio
import aiofiles
from Global import logger, config
from pathlib import Path
from typing import Any, Dict, List, Optional

async def LoadFromFile(file_path: str) -> Optional[Dict[str, Any]]:
    '''异步读取文件并解析为Dict对象'''
    if not Path.exists(Path(file_path)):
        raise FileNotFoundError(file_path)
    async with aiofiles.open(file_path, mode='r', encoding='utf-8') as f:
        content = await f.read()
        load_dict: Dict[str, Any] = json.loads(content)
        top_keys = set(load_dict.keys())
        if not top_keys.issuperset(['savePath', 'statisticalData', 'handledItems', 'handleBaseClasses', 'cacheName']):
            logger.warning(f'文件{file_path}不是SptItemCreator创建的缓存文件, 无法加载')
            return None
        return load_dict

async def LoadsFormFolder(folder_path=".") -> List[Dict[str, Any]]:
    '''从文件夹路径加载数据(仅限SptItemCreator创建的缓存文件)'''
    path = Path(folder_path)
    json_files = list(p for p in path.glob("*.json") if not p.name.endswith('hash.json'))
    tasks = [LoadFromFile(p) for p in json_files]
    results = await asyncio.gather(*tasks)
    objs = [r for r in results if r is not None]
    logger.debug(f'在路径{folder_path}成功加载{len(objs)}条缓存文件')
    return objs

_data: Optional[List[Dict[str, Any]]] = None

async def GetAllData():
    '''根据config.yaml设置的路径获取所有数据'''
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
    