## Wiki介绍

本Wiki旨在系统整理SPT的各项数据资源，提供模组接口文档与物品创建指南等信息，助力开发者与玩家更高效地使用本模组和扩展SPT物品。

## 快速上手
- **一键创建**：复制模板文件，修改关键字段即可创建新物品
- **四种类型**：支持通用、弹药、医疗、食物饮品四种物品类型

## 创建流程（3步完成）

### 步骤1：复制模板
复制`data`文件夹下的模板文件（.sic.jsonc）

### 步骤2：配置关键字段
```json
{
  "$type": "common",
  "enable": true,
  "baseInfo": {
    "id": "6900c8e93ea877662a000001",
    "name": "我的自定义物品",
    "parentId": "基础物品ID"
  }
}
```

### 步骤3：添加模型（可选）
1. 模型文件放入`bundles`文件夹
2. 在`bundles.json`中注册
3. 在物品配置中引用路径

## 常用常量速览

- rarityPvE 稀有度
- itemSound 可以赋值的值
#### MongeId类型的常量
  - 货币 Tpl Id
  - 商人Id
  - handbookParentId 可选Id与对应类型
  - ParentId常量
#### 子弹相关常量
  - tracerColor(曳光弹颜色)可用的值
  - caliber(口径)可用的值
  - ammoType可用的值
#### 饮品食物与药品相关常量
  - effects_damage的键
  - effects_health的键

## 参考文献

本Wiki中提到的部分代码，数据与译文节选自:

>  [server-csharp/Libraries/SPTarkov.Server.Assets/SPT_Data/database](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Assets/SPT_Data/database)

本Wiki中提到的部分常量节选自:

> [server-csharp/Libraries/SPTarkov.Server.Core](https://github.com/sp-tarkov/server-csharp/tree/main/Libraries/SPTarkov.Server.Core)

剩余代码与常量节选自本模组