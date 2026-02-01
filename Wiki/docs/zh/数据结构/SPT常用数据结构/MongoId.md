## MongoId

[SPTarkov.Server.Core/Models/Common/MongoId.cs](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Common/MongoId.cs)

MongoId由24位(12字节)组成, 分别为：

- 时间戳（4字节）：记录创建时刻（自1970-01-01的秒数）
- 机器标识（3字节）：区分不同机器
- 进程ID（2字节）：同一机器上的不同进程
- 计数器（3字节）：同一进程内的自增序列

示例：`5f9d9b8e6f8b4a1e3c7d5a30`

**如何写自己的MongoId**

- 方法一：线上MongoId生成器
- 方法二：在已有的MongoId中改几个字符, 随后在items.json中检索确保唯一(不推荐)
- 方法三：随便写24位的字符串，一般情况很难很难撞到SPT中存在的Id
- 方法四：自己写代码编一个/让AI编一个简单的脚本生成MongoId