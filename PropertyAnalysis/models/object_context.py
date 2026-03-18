from Global import config
from managers.stats_mgr import StatsManager


class ObjectContext:
    def __init__(self):
        save_file_path = config.get('StatsManagerSavePath')
        self.stats_mgr = StatsManager.create_from_file(save_file_path)
        self.config = config


