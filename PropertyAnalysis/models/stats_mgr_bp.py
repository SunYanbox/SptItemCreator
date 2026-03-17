from typing import Optional

from Global import config
from flask import jsonify
from pathlib import Path
from flask.blueprints import Blueprint

from stats_mgr import StatsManager

stats_mgr: Optional[StatsManager] = None

stats_mgr_bp = Blueprint('stats_mgr', __name__, url_prefix='/stats_mgr')

@stats_mgr_bp.route('/', strict_slashes=False)
def index():
    save_file_path = config.get('StatsManagerSavePath')
    save_file_path_object = Path(save_file_path)

    file_size = save_file_path_object.stat().st_size if save_file_path_object.exists() else 0.000

    data = {
        'Default Save Data Path': save_file_path,
        'Is Save File Exist': save_file_path_object.exists(),
        'Save File Size': f'{file_size/1024:.3f}kb'
    }
    return jsonify(data)

