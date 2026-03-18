## GET

**查看服务器状态**

```shell
curl.exe -s localhost:6666/
```

**查看已支持的所有Url**

```shell
curl.exe -s localhost:6666/url_map/
```

### cfg 配置路由

**查看所有配置信息**

```shell
curl.exe -s localhost:6666/cfg/
```

**查看所有存在的配置键**

```shell
curl.exe -s localhost:6666/cfg/keys/
```

**查看指定键的配置值**

```shell
curl.exe -s localhost:6666/cfg/keys/<string:key>/
```
示例
```shell
curl.exe -s localhost:6666/cfg/keys/StatsManagerSavePath/
```

### stats_mgr 统计路由

查看当前统计管理器状态

```shell
curl.exe -s localhost:6666/stats_mgr/
```

保存当前统计管理器数据

```shell
curl.exe -s localhost:6666/stats_mgr/save
```

从已保存的统计管理器文件加载数据

```shell
curl.exe -s localhost:6666/stats_mgr/load/
```

从SptItemCreator模组导出的StatsCache文件夹加载数据

```shell
curl.exe -s localhost:6666/stats_mgr/load/<path:folder_path>
```

## POST

更新配置信息

```shell
curl -X POST http://127.0.0.1:6666/cfg/keys/StatsManagerSavePath --json "{\"StatsManagerSavePath\": \"data/StatsManager.plk\"}"
```
详情模式:
```shell
curl -v -X POST http://127.0.0.1:6666/cfg/keys/StatsManagerSavePath --json "{\"StatsManagerSavePath\": \"data/StatsManager.plk\"}"
```





