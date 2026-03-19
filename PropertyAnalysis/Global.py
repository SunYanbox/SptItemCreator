import os
from gettext import GNUTranslations
from logging import Logger

import gettext as _gettext
import yaml
from pathlib import Path
import logging as _logging
from typing import Any, Dict, Optional

# 数据路径初始化

data_path = 'data'

if not os.path.exists(data_path):
    os.makedirs(data_path)

# 日志初始化

logger: Logger = _logging.getLogger("PropertyAnalysis")
logger.setLevel(level=_logging.DEBUG)

_file_handler = _logging.FileHandler(
    Path(data_path, "PropertyAnalysis.log"),
    encoding='utf-8'
)
_file_handler.setLevel(_logging.DEBUG)
_formatter = _logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
_file_handler.setFormatter(_formatter)
logger.addHandler(_file_handler)

# 配置初始化

with open('config.yaml', 'r', encoding='utf-8') as file:
    config: Dict[str, Any] = yaml.load(file, Loader=yaml.FullLoader)
    logger.debug(f'已加载配置: [{', '.join(config.keys())}]')

# 设置默认语言配置（如果不存在）
if 'Language' not in config:
    config['Language'] = 'zh'
    logger.debug('已添加默认语言配置: zh')

language = config.get('Language', 'zh')

_translation: Optional[GNUTranslations] = None

def init_translation():
    global _translation
    try:
        # 加载翻译文件
        lang_en = _gettext.translation('PropertyAnalysis', localedir='locales', languages=['en'], fallback=True)
        lang_zh = _gettext.translation('PropertyAnalysis', localedir='locales', languages=['zh'], fallback=True)

        _gettext.bindtextdomain('PropertyAnalysis', 'locales')
        _gettext.textdomain('PropertyAnalysis')

        match language:
            case 'zh':
                lang_zh.install()
                _translation = lang_zh
            case 'en':
                lang_en.install()
                _translation = lang_en
            case _:
                lang_zh.install()
                _translation = lang_zh
    except Exception as e:
        logger.error(f'设置语言时出错: {e}')

def gettext(string: str) -> str:
    global _translation
    if _translation is None:
        return string
    else:
        return _translation.gettext(string)

def save_config():
    with open('config.yaml', 'w', encoding='utf-8') as file:
        yaml.dump(config, file)