## Wiki介绍

本Wiki旨在系统整理SPT的各项数据资源，提供模组接口文档与物品创建指南等信息，助力开发者与玩家更高效地使用本模组和扩展SPT物品。

## 创建流程

服务器启动后, Ctrl+左键点开`https://127.0.0.1:6969/SIC`或者直接在浏览器中输出这个地址

然后你就会看到一个目前比较简陋的UI界面

点击上方的导航栏以进入物品创建界面(暂时不支持物品编辑, 暂时没有提供弹药编辑页面, 但仍然可以通过通用物品创建页面借助PropertyOverride创建)

**所有物品默认是未启用的**, 如果保存后游戏里没有看到一般是这个选项的问题

> 后续修改参考: 
>
> - [手动创建物品方式.md](./创建物品教程/手动创建物品方式.md)
> - [模组数据结构](./数据结构/)

## 参考文献

本Wiki中提到的部分代码，数据与译文节选自:

>  [server-csharp/Libraries/SPTarkov.Server.Assets/SPT_Data/database](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Assets/SPT_Data/database)

本Wiki中提到的部分常量节选自:

> [server-csharp/Libraries/SPTarkov.Server.Core](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Core)

剩余代码与常量节选自本模组