## AttributeInfo 属性信息类

> 继承自 `AbstractInfo` 的属性信息记录类，用于定义物品的物理属性、交互属性和游戏机制属性。

| 属性                  | 在数据文件中的写法  | 类型   | 默认值    | 说明                  | 可选项                                                       | 可选性 |
| --------------------- | ------------------- | ------ | --------- | --------------------- | ------------------------------------------------------------ | ------ |
| **Weight**            | "weight"            | double | null      | 物品重量 (千克)       | 属于实数                                                     | 可选   |
| **Width**             | "width"             | int    | null      | 物品宽度 (格子数)     | 正整数                                                       | 可选   |
| **Height**            | "height"            | int    | null      | 物品高度 (格子数)     | 正整数                                                       | 可选   |
| **RarityPvE**         | "rarityPvE"         | string | null      | PvE模式下的物品稀有度 | [LootRarity](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/LootRarity.cs) | 可选   |
| **DiscardLimit**      | "discardLimit"      | double | null      | 丢弃限制数值          | 常见-1，0，1，2                                              | 可选   |
| **ItemSound**         | "itemSound"         | string | "generic" | 物品交互音效类型      | [itemSound 可以赋值的值](../常用常量/字符串常量/itemSound 可以赋值的值.md) | 可选   |
| **StackMaxSize**      | "stackMaxSize"      | int    | 1         | 最大堆叠数量          | 常见1，20，40，50，60，<br/>货币在100~100e4之间              | 可选   |
| **ExaminedByDefault** | "examinedByDefault" | bool   | true      | 是否默认已检视        | 一般为true                                                   | 可选   |
| **ExamineTime**       | "examineTime"       | double | null      | 检视所需时间 (秒)     | 一般为1                                                      | 可选   |
| **LootExperience**    | "lootExperience"    | int    | null      | 拾取获得的经验值      | 绝大多数处于5~100之间                                        | 可选   |
| **ExamineExperience** | "examineExperience" | int    | null      | 检视获得的经验值      | 绝大多数处于5~100之间                                        | 可选   |
| **BackgroundColor**   | "backgroundColor"   | string | null      | 物品背景颜色          | default, blue, black等<br />具体可以在模组运行后查看缓存文件夹的信息 | 可选   |

## attributeInfo字段

```json
// 物品属性配置
"attributeInfo": {
    // 物品重量（千克）
    "weight": 0.3,
    // 物品在仓库中的宽度（格子数）
    "width": 3,
    // 物品在仓库中的高度（格子数）
    "height": 3,
    // PvE模式下的稀有度等级, 参考文末的常量, 字典里键/值都行
    "rarityPvE": "Common",
    // 丢弃限制，null表示继承克隆的物品的限制
    "discardLimit": null,
    // 物品操作音效类型, 参考下面的常量赋值
    "itemSound": "food_snack",
    // 最大堆叠数量，null表示继承克隆的物品的最大堆叠数量
    "stackMaxSize": null,
    // 是否默认已检视（无需再次检视）
    "examinedByDefault": true,
    // 检视所需时间（秒）
    "examineTime": 2,
    // 拾取时获得的经验值
    "lootExperience": 50,
    // 检视时获得的经验值
    "examineExperience": 500,
    // 物品背景颜色
    "backgroundColor": "default"
}
```

rarityPvE参考[rarityPvE 稀有度](../常用常量/字符串常量/rarityPvE 稀有度.md)

itemSound参考[itemSound 可以赋值的值](../常用常量/字符串常量/itemSound 可以赋值的值.md)

## 示例

```json
"attributeInfo": {
    "weight": 0.4,
    "width": 1,
    "height": 1,
    "rarityPvE": "Rare",
    "discardLimit": null,
    "itemSound": null,
    "stackMaxSize": null,
    "examinedByDefault": true,
    "examineTime": 2,
    "lootExperience": 500,
    "examineExperience": 50
}
```

为null的也可以省略:

```json
"attributeInfo": {
    "weight": 0.4,
    "width": 1,
    "height": 1,
    "rarityPvE": "Rare",
    "examinedByDefault": true,
    "examineTime": 2,
    "lootExperience": 500,
    "examineExperience": 50
}
```

