## PropertyAnalysis

一个Python项目，用于分析模组生成的缓存数据

安装依赖:

```shell
conda activate <yourEnvName>
conda install --yes --file requirements.txt
```

运行脚本:
```shell
python app.py
```

## 脚本功能

启动一个默认6666端口的本地服务, 提供一些获取分析好或原始数据的API接口

e.g. 获取当前配置
```shell
curl.exe -s http://127.0.0.1:6666/cfg/
```

e.g. 获取所有配置的键
```shell
curl.exe -s http://127.0.0.1:6666/cfg/keys/
```

e.g. 获取指定配置的值
```shell
curl.exe -s http://127.0.0.1:6666/cfg/keys/StatsManagerSavePath
```

## 必须的配置

需要在当前目录创建一个名为`config.yaml`的文件用于设置配置

```yaml
# 需要手动设置
SptItemCreatorStatsCacheFolderPath: "SptItemCreator/StatsCache的绝对路径"

# 默认值
StatsManagerSavePath: "data/StatsManager.plk"
```