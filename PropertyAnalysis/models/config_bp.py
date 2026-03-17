from flask import jsonify, Flask
from flask.blueprints import Blueprint

from Global import config as _config

config_bp = Blueprint('cfg', __name__, url_prefix='/cfg')

@config_bp.route('/', strict_slashes=False)
def config():
    return jsonify(_config)

@config_bp.route('/keys', strict_slashes=False)
def config_keys():
    return jsonify(list(_config.keys()))

@config_bp.route('/keys/<string:key>', strict_slashes=False)
def config_get_key(key):
    return jsonify(_config.get(key))

def register_blueprint(app: Flask):
    app.register_blueprint(config_bp)