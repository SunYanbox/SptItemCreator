import traceback
from flask import Flask, jsonify, request, current_app
from werkzeug.exceptions import NotFound

from Global import logger, config
from stats_mgr import StatsManager

# 蓝图
from models.config_bp import config_bp
from models.stats_bp import stats_bp
from models.stats_mgr_bp import get_stats_mgr, set_stats_mgr, stats_mgr_bp

port: int = 6666

app = Flask('PropertyAnalysis')

@app.route('/', strict_slashes=False)
def index():
    stats_mgr = get_stats_mgr()
    state = {
        'Ip:Host': f'localhost:{port}',
        'LoadedStatsManager': stats_mgr is not None,
        'len(StatsManager.data)': len(stats_mgr.data) if stats_mgr is not None else 0,
    }
    return jsonify(state)

@app.route('/url_map/', strict_slashes=False)
def get_url_map():
    routes = []
    for rule in app.url_map.iter_rules():
        routes.append(f'{list(rule.methods - {'HEAD', 'OPTIONS'})}{str(rule)}')
    return jsonify(routes)


# 注册蓝图
app.register_blueprint(config_bp)
app.register_blueprint(stats_mgr_bp)
app.register_blueprint(stats_bp)

def register_error_handlers(target_app: Flask):
    @target_app.errorhandler(NotFound)
    def handle_not_found(_):
        return jsonify({"error": "NOT_FOUND"}), 404

    @target_app.errorhandler(500)
    def handle_internal_error(_):
        request_id = request.headers.get("X-Request-ID", "N/A")
        if current_app.debug:
            tb = traceback.format_exc()
            details = {"request_id": request_id, "traceback": tb}
        else:
            logger.error(f"[{request_id}] Internal server error", exc_info=True)
            details = {"request_id": request_id}
        return jsonify({
            "error": "INTERNAL_ERROR",
            "details": details
        }), 500


register_error_handlers(app)

if __name__ == '__main__':
    print(app.url_map)
    try:
        save_file_path = config.get('StatsManagerSavePath')
        set_stats_mgr(StatsManager.create_from_file(save_file_path))
        stats_mgr = get_stats_mgr()
        data_count = len(stats_mgr.data) if stats_mgr is not None else 0
        logger.info(f'[初始化] 自动导入plk数据文件完成: {data_count}条数据')
    except Exception as e:
        logger.error(f'[初始化] 自动导入plk数据文件时出错: {e}')
    app.run(port=port)
