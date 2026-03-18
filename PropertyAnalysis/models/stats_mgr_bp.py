from typing import Optional

from Global import config, logger
from flask import jsonify
from pathlib import Path
from flask.blueprints import Blueprint

from stats_mgr import StatsManager

stats_mgr: Optional[StatsManager] = None

stats_mgr_bp = Blueprint('stats_mgr', __name__, url_prefix='/stats_mgr')

def get_stats_mgr() -> Optional[StatsManager]:
    global stats_mgr
    return stats_mgr

def set_stats_mgr(target_stats_mgr: Optional[StatsManager]):
    global stats_mgr
    stats_mgr = target_stats_mgr

@stats_mgr_bp.route('/', strict_slashes=False)
def _index():
    save_file_path = config.get('StatsManagerSavePath')
    save_file_path_object = Path(save_file_path)

    file_size = save_file_path_object.stat().st_size if save_file_path_object.exists() else 0.000

    data = {
        'Default Save Data Path': save_file_path,
        'Loaded StatsManger': stats_mgr is not None,
        'Is Save File Exist': save_file_path_object.exists(),
        'Save File Size': f'{file_size/1024:.3f}kb'
    }
    return jsonify(data)

@stats_mgr_bp.route('/load', strict_slashes=False)
def _load_save_file():
    """从plk数据文件加载数据"""
    global stats_mgr
    try:
        save_file_path = config.get('StatsManagerSavePath')
        stats_mgr = StatsManager.create_from_file(save_file_path)
        data_count = len(stats_mgr.data) if stats_mgr is not None else 0
        logger.info(f'导入plk数据文件完成: {data_count}条数据')
        return jsonify({'Success': f'Load data success', 'Load Data Count': data_count, 'path': save_file_path}), 200
    except Exception as e:
        logger.error(f'导入plk数据文件时出错: {e}')
        return jsonify({'Error': str(e)}), 500

@stats_mgr_bp.route('/load/<path:folder_path>', strict_slashes=False)
def _load_folder(folder_path: str):
    """从文件夹加载数据"""
    import asyncio
    global stats_mgr
    try:
        stats_mgr = asyncio.run(StatsManager.create_from_folder(folder_path))
        data_count = len(stats_mgr.data) if stats_mgr is not None else 0
        logger.info(f'从文件夹导入数据完成: {data_count}条数据')
        return jsonify({'Success': f'Load data success', 'Load Data Count': data_count, 'path': folder_path}), 200
    except Exception as e:
        logger.error(f'从文件夹导入数据时出错: {e}')
        return jsonify({'Error': str(e)}), 500

@stats_mgr_bp.route('/save', strict_slashes=False)
def _save_data_to_file():
    """保存数据到plk数据文件"""
    global stats_mgr
    try:
        save_file_path = config.get('StatsManagerSavePath')
        if stats_mgr is None:
            logger.warning('尝试保存数据时还未加载过任何数据')
            return jsonify({'Error': 'No Data To Save!'}), 500
        data_count = len(stats_mgr.data) if stats_mgr is not None else 0
        stats_mgr.save_to_file(save_file_path)
        logger.info(f'保存数据到plk文件完成: {data_count}条数据')
        return jsonify({'Success': f'Save data success', 'Save Data Count': data_count}), 201
    except Exception as e:
        logger.error(f'保存数据到plk文件时出错: {e}')
        return jsonify({'Error': str(e)}), 500
