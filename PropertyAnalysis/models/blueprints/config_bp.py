from flask import jsonify, request
from flask.blueprints import Blueprint

from Global import config as _config, save_config

config_bp = Blueprint('cfg', __name__, url_prefix='/cfg')

@config_bp.route('/', strict_slashes=False)
def config():
    return jsonify(_config)

@config_bp.route('/keys', strict_slashes=False)
def config_keys():
    return jsonify(list(_config.keys()))

@config_bp.route('/keys/<string:key>', methods=['GET', 'POST'], strict_slashes=False)
def config_get_or_set_key(key):
    if request.method == 'GET':
        # GET 请求：返回配置值
        value = _config.get(key)
        return jsonify({key: value})

    elif request.method == 'POST':
        # POST 请求：设置配置值
        data = request.get_json()

        if not data or key not in data:
            return jsonify({"error": f"Missing value for key '{key}'"}), 400

        # 设置新值
        new_value = data[key]
        _config[key] = new_value

        save_config()

        return jsonify({
            "message": f"Config '{key}' updated successfully",
            key: new_value
        }), 201
    else:
        return jsonify({"error": f"Unsupported method '{request.method}'"}), 400