## BaseInfo 基础信息

> 继承自 `AbstractInfo` 的基础信息记录类，用于定义新物品的核心元数据和配置

| 属性             | 在数据文件中的写法 | 数据类型                                                     | 必需性   | 默认值                                                  | 描述                                                   |
| ---------------- | ------------------ | ------------------------------------------------------------ | -------- | ------------------------------------------------------- | ------------------------------------------------------ |
| Id               | "id"               | [MongoId](./SPT常用数据结构/MongoId.md)                      | **必需** |                                                         | 物品唯一标识符                                         |
| Type             | "type"             | string                                                       | 可选     | "common"                                                | 模板类型                                               |
| Name             | "name"             | string                                                       | 可选     | 未命名物品                                              | 物品名称, 简称                                         |
| Description      | "description"      | string                                                       | 可选     | 名称+作者+协议<br/>（提供Locales后优先用Locales的描述） | 物品描述                                               |
| Locales          | "locales"          | Dictionary<string, [LocaleDetails](./SPT常用数据结构/LocaleDetails.md)> | 可选     | null                                                    | 物品的本地化信息                                       |
| Author           | "author"           | string                                                       | 可选     | 佚名                                                    | 作者名称                                               |
| License          | "license"          | string                                                       | 可选     | MIT                                                     | 使用协议                                               |
| Order            | "order"            | int                                                          | 可选     | 0                                                       | 加载顺序                                               |
| ParentId         | "parentId"         | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 父级ID, 当赋值了CloneId时可选, 当没有赋值CloneId时必需 |
| CloneId          | "cloneId"          | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 原型ID                                                 |
| HandbookParentId | "handbookParentId" | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 手册父级ID                                             |
| TraderId         | "traderId"         | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 默认商人ID                                             |
| FleaPrice        | "fleaPrice"        | double                                                       | 可选     | 1                                                       | 跳蚤市场价格                                           |
| HandbookPrice    | "handbookPrice"    | double                                                       | 可选     | 1                                                       | 手册价格(商人售卖价格)                                 |
| Prefab           | "prefab"           | [Prefab](./SPT常用数据结构/新物品相关/Prefab.md)             | 可选     | null                                                    | 物品模型                                               |
| UsePrefab        | "usePrefab"        | [Prefab](./SPT常用数据结构/新物品相关/Prefab.md)             | 可选     | null                                                    | 使用模型                                               |
| CanSellOnRagfair | "canSellOnRagfair" | bool                                                         | 必需     | true                                                    | 跳蚤市场销售权限                                       |
| AllowAll         | "allowAll"         | bool                                                         | 可选     | false                                                   | 一键允许所有容器放置本物品                             |
| CanFilter        | "canFilter"        | HashSet<[MongoId](./SPT常用数据结构/MongoId.md)>             | 可选     | []                                                      | 指定哪些容器可放置本物品(优先级大于allowAll)           |
| CantFilter       | "cantFilter"       | HashSet<[MongoId](./SPT常用数据结构/MongoId.md)>             | 可选     | []                                                      | 指定哪些容器不可放置本物品(优先级大于allowAll)         |

> Locales可选键：, ch, cz, en, es-mx, es, fr, ge, hu, it, jp, kr, pl, po, ro, ru, sk, tu

**注意: id必须提供; traderId不提供或提供有误时不会添加给商人; 其他属性都有默认值**(只有baseInfo有默认值)

`cloneId`和`handbookParentId`都没提供时, 是完全通过`propertyOverride`字段创建

```json
"baseInfo": {
    // 物品的唯一标识ID，使用24位十六进制字符串表示
    "id": "6900c8e93ea877662a000000",
    // 物品类型，与$type保持一致
    "type": "drinkOrDrugs",
    // 物品在游戏中显示的名称
    "name": "食物模板",
    // 物品描述，null表示使用默认描述或无描述
    "description": null,
    "locales": {
        "ch": {
            "name": "食物模板",
            "shortName": "食物模板(短)",
            "description": "一个食物模板"
        },
        "en": {
            "name": "Food Template",
            "shortName": "Food Template(Short)",
            "description": "A food template"
        }
    },
    // 作者名称
    "author": "Suntion",
    // 版权协议, 默认为MIT
    "license": "MIT",
    // 在本模组创建新物品时的加载顺序，数值越大加载越晚
    "order": 0,
    // 父级物品ID，用于继承基础属性(和cloneId物品的`_parent`一致)
    "parentId": "5448e8d04bdc2ddf718b4569",
    // 手册中的父级分类ID(必须和cloneId同时赋值)
    "handbookParentId": "5b47574386f77428ca22b336",
    // 复制的原型物品ID，基于该物品创建新物品(必须和handbookParentId同时赋值)
    "cloneId": "5448ff904bdc2d6f028b456e",
    // 默认售卖该物品的商人ID(默认null)
    "traderId": "54cb57776803fa99248b456e",
    // 跳蚤市场价格(默认1)
    "fleaPrice": 5000,
    // 手册价格(默认1)
    "handbookPrice": 5000,
    // 物品模型配置，null表示使用默认模型(null表示赋值cloneId后, 默认用cloneId的模型)
    "prefab": null,
    // 使用时的模型配置，null表示使用默认模型(null表示赋值cloneId后, 默认用cloneId的模型)
    "usePrefab": null,
    // 是否允许在跳蚤市场售卖(默认true)
    "CanSellOnRagfair": true,
    // 一键允许所有容器放置本物品
    // 设置为false表示等同于cloneId对应物品的可放置/不可放置情况
    // 设置为false不等于禁止任何容器储存本物品
    "allowAll": false,
    // 指定哪些容器可放置本物品(优先级大于allowAll)
 	"canFilter": [],
    // 指定哪些容器不可放置本物品(优先级大于allowAll)
	"cantFilter": []
}
```

> 即在canFilter也在cantFilter中的MongoId会被忽略
>
> 不推荐所有物品都设置allowAll为true，有确切需求优先将容器的MongoId设置到canFilter或cantFilter中

handbookParentId参考[handbookParentId 可选Id与对应类型](../常用常量/常用MongoId常量/handbookParentId 可选Id与对应类型.md)

traderId参考[商人Id](../常用常量/常用MongoId常量/商人Id.md)

## 示例

```json
"baseInfo": {
    "id": "5f9d9b8e6f8b4a1e3c7d5a30",
    "type": "medical",
    "name": "五五五药剂",
    "description": null,
    "locales": {
        "ch": {
            "name": "五五五药剂",
            "shortName": "555药剂",
            "description": "一种神秘的实验性药剂，编号“555”。据称它能通过复杂的周期性剂量设计，在极长的时间跨度内持续增强使用者的多项核心生理与心理能力。效果随时间呈几何级数增长，最终达到理论上的“超凡”状态。其配方来源与长期安全性均为未知。"
        },
        "en": {
            "name": "Formula 555",
            "shortName": "555 Formula",
            "description": "A mysterious experimental chemical agent, designated /"Formula 555/". It is purported to continuously enhance multiple core physical and psychological attributes of the user over an extremely prolonged duration through a complex cyclical dosing design. The effects increase geometrically over time, culminating in a theorized /"transcendent/" state. The origin of its formula and its long-term safety profile remain unknown."
        }
    },
    "author": "Suntion",
    "license": "MIT",
    "order": 0,
    "parentId": "5448f3a64bdc2d60728b456a",
    "handbookParentId": "5b47574386f77428ca22b33a",
    "cloneId": "5c0e533786f7747fa23f4d47",
    "traderId": "54cb57776803fa99248b456e",
    "fleaPrice": 335503.36,
    "handbookPrice": 335503.36,
    "prefab": null,
    "usePrefab": null,
    "canSellOnRagfair": true,
    "allowAll": false,
    "canFilter": [],
    "cantFilter": []
}
```

> 由于做数据验证会带来巨大的缓存需求, 在缓存机制优化前不会给太多属性做数据验证
