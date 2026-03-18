from typing import Optional, Any, Dict, List, Union

from Global import config, logger
from flask import jsonify, Response
from pathlib import Path
from flask.blueprints import Blueprint

from stats_mgr import StatsManager
from models.stats_mgr_bp import get_stats_mgr
from stats_struct import StatsStruct

stats_bp = Blueprint('stats', __name__, url_prefix='/stats')

@stats_bp.route('/base_classes', methods=['GET'])
def _get_base_classes():
    stats_mgr = get_stats_mgr()
    if stats_mgr is None:
        return jsonify({'Error': 'The StatsManager not Load'}), 500
    return jsonify([s for s in stats_mgr.base_classes])

@stats_bp.route('/prop_keys', methods=['GET'])
def _get_prop_keys():
    stats_mgr = get_stats_mgr()
    if stats_mgr is None:
        return jsonify({'Error': 'The StatsManager not Load'}), 500
    return jsonify([s for s in stats_mgr.prop_keys])

@stats_bp.route('/prop_keys/<string:base_classes>', methods=['GET'])
def _get_prop_keys_base_classes(base_classes: str):
    stats_mgr = get_stats_mgr()
    if stats_mgr is None:
        return jsonify({'Error': 'The StatsManager not Load'}), 500
    return jsonify({
        'base_classes': base_classes,
        'prop_keys': [s for s in stats_mgr.get_prop_keys(base_classes)]
    })

@stats_bp.route('/prop_keys/<string:base_classes>/unique', methods=['GET'])
def _get_unique_prop_keys_base_classes(base_classes: str):
    stats_mgr = get_stats_mgr()
    if stats_mgr is None:
        return jsonify({'Error': 'The StatsManager not Load'}), 500
    return jsonify({
        'base_classes': base_classes,
        'prop_keys': [s for s in stats_mgr.get_unique_prop_keys(base_classes)]
    })

def _get_prop_data_with_error_response(base_classes: str, prop_key: str) -> Union[Dict[str, List[Any]], Response]:
    stats_mgr = get_stats_mgr()
    if stats_mgr is None:
        return jsonify({'Error': 'The StatsManager not Load'})
    prop: Optional[StatsStruct] = stats_mgr.data.get(base_classes, None)
    if prop is None:
        return jsonify({'Error': f'There are not data of BaseClasses({base_classes})'})
    prop: Optional[Dict[str, List[Any]]] = prop.get_prop_data(prop_key)
    if prop is None:
        return jsonify({'Error': f'The BaseClasses({base_classes}) don\'t have prop: {prop_key}'})
    return prop

@stats_bp.route('/<string:base_classes>/<string:prop_key>/type', methods=['GET'])
def _get_prop_type(base_classes: str, prop_key: str):
    """查看指定类型指定属性的值的属性(仅英文名枚举)"""
    data: Union[Dict[str, List[Any]], Response] = _get_prop_data_with_error_response(base_classes, prop_key)
    if isinstance(data, Response):
        return data, 500
    return jsonify({
        'base_classes': base_classes,
        'prop_key': prop_key,
        'prop_type': [s for s in data.keys()]
    })

@stats_bp.route('/<string:base_classes>/<string:prop_key>', methods=['GET'])
def _get_prop_items(base_classes: str, prop_key: str):
    """查看指定类型指定属性的所有值"""
    data: Union[Dict[str, List[Any]], Response] = _get_prop_data_with_error_response(base_classes, prop_key)
    if isinstance(data, Response):
        return data, 500
    return jsonify({
        'base_classes': base_classes,
        'prop_key': prop_key,
        'prop': { k: v for k, v in data.items()}
    })














