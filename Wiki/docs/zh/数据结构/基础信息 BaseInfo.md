## BaseInfo

> 继承自 `AbstractInfo` 的基础信息记录类，用于定义新物品的核心元数据和配置

| 属性             | 数据类型                                                     | 必需性   | 默认值                                                  | 描述                                                   |
| ---------------- | ------------------------------------------------------------ | -------- | ------------------------------------------------------- | ------------------------------------------------------ |
| Id               | [MongoId](./SPT常用数据结构/MongoId.md)                      | **必需** |                                                         | 物品唯一标识符                                         |
| Type             | string                                                       | 可选     | "common"                                                | 模板类型                                               |
| Name             | string                                                       | 可选     | 未命名物品                                              | 物品名称, 简称                                         |
| Description      | string                                                       | 可选     | 名称+作者+协议<br/>（提供Locales后优先用Locales的描述） | 物品描述                                               |
| Locales          | Dictionary<string, [LocaleDetails](./SPT常用数据结构/LocaleDetails.md)> | 可选     | null                                                    | 物品的本地化信息                                       |
| Author           | string                                                       | 可选     | 佚名                                                    | 作者名称                                               |
| License          | string                                                       | 可选     | MIT                                                     | 使用协议                                               |
| Order            | int                                                          | 可选     | 0                                                       | 加载顺序                                               |
| ParentId         | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 父级ID, 当赋值了CloneId时可选, 当没有赋值CloneId时必需 |
| CloneId          | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 原型ID                                                 |
| HandbookParentId | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 手册父级ID                                             |
| TraderId         | [MongoId](./SPT常用数据结构/MongoId.md)                      | 可选     | null                                                    | 默认商人ID                                             |
| FleaPrice        | double                                                       | 可选     | 1                                                       | 跳蚤市场价格                                           |
| HandbookPrice    | double                                                       | 可选     | 1                                                       | 手册价格(商人售卖价格)                                 |
| Prefab           | [Prefab](.\SPT常用数据结构\Prefab.md)                        | 可选     | null                                                    | 物品模型                                               |
| UsePrefab        | [Prefab](.\SPT常用数据结构\Prefab.md)                        | 可选     | null                                                    | 使用模型                                               |
| CanSellOnRagfair | bool                                                         | 必需     | true                                                    | 跳蚤市场销售权限                                       |
| AllowAll         | bool                                                         | 可选     | false                                                   | 一键允许所有容器放置本物品                             |
| CanFilter        | HashSet<[MongoId](./SPT常用数据结构/MongoId.md)>             | 可选     | []                                                      | 指定哪些容器可放置本物品(优先级大于allowAll)           |
| CantFilter       | HashSet<[MongoId](./SPT常用数据结构/MongoId.md)>             | 可选     | []                                                      | 指定哪些容器不可放置本物品(优先级大于allowAll)         |

> Locales可选键：, ch, cz, en, es-mx, es, fr, ge, hu, it, jp, kr, pl, po, ro, ru, sk, tu

示例:
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
