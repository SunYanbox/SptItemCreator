## GET

**查看服务器状态**

```shell
curl.exe -s localhost:6666/
```

**查看已支持的所有Url**

```shell
curl.exe -s localhost:6666/url_map/
```

**查看统计数据管理器状态**

```shell
curl.exe -s localhost:6666/stats_mgr/
```

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

## POST

更新配置信息

```shell
curl -X POST http://127.0.0.1:6666/cfg/keys/StatsManagerSavePath --json "{\"StatsManagerSavePath\": \"data/StatsManager.plk\"}"
```
详情模式:
```shell
curl -v -X POST http://127.0.0.1:6666/cfg/keys/StatsManagerSavePath --json "{\"StatsManagerSavePath\": \"data/StatsManager.plk\"}"
```





