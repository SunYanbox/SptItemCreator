import os
import yaml
from pathlib import Path
import logging as _logging
from typing import Any, Dict

# 数据路径初始化

data_path = 'data'

if not os.path.exists(data_path):
    os.makedirs(data_path)

# 日志初始化

logger = _logging.getLogger("PropertyAnalysis")
logger.setLevel(level=_logging.DEBUG)

file_handler = _logging.FileHandler(Path(data_path, "PropertyAnalysis.log"))
file_handler.setLevel(_logging.DEBUG)
formatter = _logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
file_handler.setFormatter(formatter)
logger.addHandler(file_handler)

# 配置初始化

with open('config.yaml', 'r', encoding='utf-8') as file:
    config: Dict[str, Any] = yaml.load(file, Loader=yaml.FullLoader)
    logger.debug(f'已加载配置: [{', '.join(config.keys())}]')