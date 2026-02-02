## BaseInfo

> A base information record class inherited from `AbstractInfo`, used to define the core metadata and configuration for a new item.

| Property           | In Data File       | Data Type                                                     | Required | Default Value                                          | Description                                                |
| ------------------ | ------------------ | ------------------------------------------------------------ | -------- | ----------------------------------------------------- | ---------------------------------------------------------- |
| Id                 | "id"               | [MongoId](./Common Data Structures in SPT/MongoId.md)           | **Required** |                                                       | Unique item identifier                                     |
| Type               | "type"             | string                                                       | Optional | "common"                                              | Template type                                              |
| Name               | "name"             | string                                                       | Optional | Unnamed Item                                          | Item name, short name                                      |
| Description        | "description"      | string                                                       | Optional | Name+Author+License<br/>(If Locales is provided, its description is prioritized) | Item description                                           |
| Locales            | "locales"          | Dictionary<string, [LocaleDetails](./Common Data Structures in SPT/LocaleDetails.md)> | Optional | null                                                  | Localization information for the item                      |
| Author             | "author"           | string                                                       | Optional | Anonymous                                             | Author name                                                |
| License            | "license"          | string                                                       | Optional | MIT                                                   | Usage license                                              |
| Order              | "order"            | int                                                          | Optional | 0                                                     | Load order                                                 |
| ParentId           | "parentId"         | [MongoId](./Common Data Structures in SPT/MongoId.md)           | Optional | null                                                  | Parent ID. Optional when CloneId is assigned; required when CloneId is not assigned. |
| CloneId            | "cloneId"          | [MongoId](./Common Data Structures in SPT/MongoId.md)           | Optional | null                                                  | Prototype ID                                               |
| HandbookParentId   | "handbookParentId" | [MongoId](./Common Data Structures in SPT/MongoId.md)           | Optional | null                                                  | Handbook parent category ID                                |
| TraderId           | "traderId"         | [MongoId](./Common Data Structures in SPT/MongoId.md)           | Optional | null                                                  | Default trader ID                                          |
| FleaPrice          | "fleaPrice"        | double                                                       | Optional | 1                                                     | Flea Market price                                          |
| HandbookPrice      | "handbookPrice"    | double                                                       | Optional | 1                                                     | Handbook price (Trader selling price)                      |
| Prefab             | "prefab"           | [Prefab](./Common Data Structures in SPT/Related To New Items/Prefab.md) | Optional | null                                                  | Item model                                                 |
| UsePrefab          | "usePrefab"        | [Prefab](./Common Data Structures in SPT/Related To New Items/Prefab.md) | Optional | null                                                  | Use model                                                  |
| CanSellOnRagfair   | "canSellOnRagfair" | bool                                                         | Required | true                                                  | Permission to sell on Flea Market                          |
| AllowAll           | "allowAll"         | bool                                                         | Optional | false                                                 | Allow placement in all containers with one click           |
| CanFilter          | "canFilter"        | HashSet<[MongoId](./Common Data Structures in SPT/MongoId.md)>  | Optional | []                                                    | Specify which containers can hold this item (priority over allowAll) |
| CantFilter         | "cantFilter"       | HashSet<[MongoId](./Common Data Structures in SPT/MongoId.md)>  | Optional | []                                                    | Specify which containers cannot hold this item (priority over allowAll) |

> Optional keys for Locales: , ch, cz, en, es-mx, es, fr, ge, hu, it, jp, kr, pl, po, ro, ru, sk, tu

**Note: id must be provided; traderId, if not provided or provided incorrectly, will not add the item to the trader; other properties have default values** (Only baseInfo has default values)

When neither `cloneId` nor `handbookParentId` is provided, creation is done entirely through the `propertyOverride` field.

```json
"baseInfo": {
    // The unique identifier ID for the item, represented as a 24-character hexadecimal string
    "id": "6900c8e93ea877662a000000",
    // Item type, consistent with $type
    "type": "drinkOrDrugs",
    // The name displayed for the item in the game
    "name": "Food Template",
    // Item description, null means using the default description or no description
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
    // Author name
    "author": "Suntion",
    // Copyright license, default is MIT
    "license": "MIT",
    // Load order when creating new items in this mod, higher numbers load later
    "order": 0,
    // Parent item ID, used for inheriting base properties (consistent with the `_parent` of the cloneId item)
    "parentId": "5448e8d04bdc2ddf718b4569",
    // Parent category ID in the handbook (must be assigned together with cloneId)
    "handbookParentId": "5b47574386f77428ca22b336",
    // Prototype item ID to copy from, based on which the new item is created (must be assigned together with handbookParentId)
    "cloneId": "5448ff904bdc2d6f028b456e",
    // Default trader ID selling this item (default null)
    "traderId": "54cb57776803fa99248b456e",
    // Flea Market price (default 1)
    "fleaPrice": 5000,
    // Handbook price (default 1)
    "handbookPrice": 5000,
    // Item model configuration, null means using the default model (null means after assigning cloneId, the cloneId's model is used by default)
    "prefab": null,
    // Model configuration when used, null means using the default model (null means after assigning cloneId, the cloneId's model is used by default)
    "usePrefab": null,
    // Whether allowed to sell on Flea Market (default true)
    "CanSellOnRagfair": true,
    // Allow placement in all containers with one click
    // Setting to false means following the same allow/disallow placement rules as the cloneId item
    // Setting to false does not equal prohibiting storage in any container
    "allowAll": false,
    // Specify which containers can hold this item (priority over allowAll)
    "canFilter": [],
    // Specify which containers cannot hold this item (priority over allowAll)
    "cantFilter": []
}
```

> MongoId present in both canFilter and cantFilter will be ignored.
>
> It is not recommended to set allowAll to true for all items. If there is a specific need, prioritize placing the container's MongoId in canFilter or cantFilter.

For handbookParentId, refer to [handbookParentId Optional Ids and Corresponding Types](../Common Constants/Common MongoId Constants/handbookParentId optional Id.md)

For traderId, refer to [Trader Ids](../Common Constants/Common MongoId Constants/Traders' Id.md)

## Example

```json
"baseInfo": {
    "id": "5f9d9b8e6f8b4a1e3c7d5a30",
    "type": "medical",
    "name": "Formula 555",
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

> Due to the significant caching requirements introduced by data validation, extensive data validation for many properties will not be implemented until the caching mechanism is optimized.

