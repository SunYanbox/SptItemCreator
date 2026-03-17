import traceback
from typing import Optional
from flask import Flask, jsonify, request, current_app
from werkzeug.exceptions import NotFound

from Global import logger
# 蓝图
from models.config_bp import config_bp
from stats_mgr import StatsManager

stats_mgr: Optional[StatsManager] = None

app = Flask('PropertyAnalysis')

@app.route('/', strict_slashes=False)
def index():
    return "Hello"


# 注册蓝图
app.register_blueprint(config_bp)


def register_error_handlers(app: Flask):
    @app.errorhandler(NotFound)
    def handle_not_found(_):
        return jsonify({"error": "NOT_FOUND"}), 404

    @app.errorhandler(500)
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
    app.run(port=6666)
