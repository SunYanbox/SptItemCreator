本模组开发目的是通过封装物品创建接口，简化物品创建流程，同时提供详细的文档，说明不同属性与字段的作用

## 支持

1. 自定义模型
    将你的自定义模型放置于bundles文件夹下, 参考模型相对于bundles文件夹的相对路径写prefab和usePrefab，随后在bundles.json文件中添加对应模型路径信息
2. 要求
- 新物品文件命名以`.sic`, `.sic.json`或`.sic.jsonc`结尾, 内容符合jsonc格式即可

- 新物品文件必须有以下字段, 推荐基于根据data文件夹下的模板进行修改:
  ```jsonc
  {
  "$type": "common", // 必须有
  "enable": false, // 修改后, 确定要添加后再改为true
  "baseInfo": {
    "id": "6900c8e93ea877662a000012", // 必须有, 且唯一
    "type": "common" // 必须有
  }
  ```
  
- 目前支持的类型(`$type`字段) 
  
  >  **所有类型都需要写`baseInfo`字段, 可选`propertyOverride`, `attributeInfo`和`buffsInfo`字段**
  
  - "common": 通过`propertyOverride`(和`_props`相同)控制所有属性
  
  后面的字段是唯一且与`$type`对应的
  
  - "drinkOrFood": 通过`drinkFoodInfo`字段快速创建食物与饮品
  - "medical": 通过`medicalInfo`字段快速创建药品
  - "ammo": 通过`ammoInfo`字段快速创建弹药

## 详细字段解释

赋值为null或者直接缺失键, 在用了`cloneId`的情况下表示继承`cloneId`物品的属性, 没有时表示缺失这个属性

### baseInfo

**注意: id, type必须提供; traderId不提供或提供有误时不会添加给商人; 其他属性都有默认值**(只有baseInfo有默认值)

`cloneId`和`handbookParentId`都没提供时, 是完全通过`propertyOverride`字段创建

```jsonc
"baseInfo": {
    // 物品的唯一标识ID，使用24位十六进制字符串表示
    "id": "6900c8e93ea877662a000000",
    // 物品类型，与$type保持一致
    "type": "drinkOrDrugs",
    // 物品在游戏中显示的名称
    "name": "食物模板",
    // 物品描述，null表示使用默认描述或无描述
    "description": null,
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
    // 物品基础价格(默认1)
    "price": 5000,
    // 物品模型配置，null表示使用默认模型(null表示赋值cloneId后, 默认用cloneId的模型)
    "prefab": null,
    // 使用时的模型配置，null表示使用默认模型(null表示赋值cloneId后, 默认用cloneId的模型)
    "usePrefab": null,
    // 是否允许在跳蚤市场售卖(默认true)
    "CanSellOnRagfair": true
}
```

#### 添加模型方式

例如在bundles文件夹下放了一个名为`ammo_5x45_superSSA.bundle`的模型文件

那么，你需要在模组文件夹的bundles.json的`manifest`字段里添加以下内容：
```json
{
  "key": "ammo_5x45_superSSA.bundle",
  "dependencyKeys": []
}
```
> 然后，在你物品的sic.jsonc文件内，baseInfo的prefab字段需要填写以下内容：
```json
"prefab": {
  "path": "ammo_5x45_superSSA.bundle",
  "rcid": ""
}
```

如果在bundles中使用文件夹进行分类，路径与key需要是完整的相对于bundles文件夹的相对路径(省略./的)

例如：相对于bundles文件夹的相对路径为`AmmoClasses/ammo_5x45_superSSA.bundle`，
那么不论是baseInfo、bundles.json亦或是overrideProperties属性中，都需要使用**AmmoClasses/ammo_5x45_superSSA.bundle**作为路径或者key

### attributeInfo

```jsonc
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
    "examineExperience": 500
}
```

### buffsInfo

这个字段是给食物/饮品/注射器的, 方便注册新buff

```jsonc
// 增益效果配置
"buffsInfo": {
    // 兴奋剂效果配置，空字符串表示无兴奋剂效果
    "stimulatorBuffs": "",
    // 物品提供的增益效果列表，null表示无额外增益
    "buffs": null
}
```

有buff的示例:

**注意: 如果stimulatorBuffs赋值了唯一值, buffs赋值为null, 会导致无法注册buffs导致客户端出现问题**

```jsonc
// Buff配置
"buffsInfo": {
    // 注册的buff名称
    "stimulatorBuffs": "BuffsStimulatorTpl",
    // 注册的buff, 如果为null, 会跳过注册; stimulatorBuffs已经被注册, 则使用
    "buffs": [
        {
            "AbsoluteValue": true,
            "BuffType": "HealthRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 300,
            "SkillName": "",
            "Value": 1
        },
        {
            "AbsoluteValue": true,
            "BuffType": "SkillRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 300,
            "SkillName": "Metabolism",
            "Value": 20
        }
    ]
}
```

### propertyOverride

完全照抄`游戏根目录\SPT\SPT_Data\database\templates\items.json`的物品的`_props`属性即可

### drinkDrugInfo

```jsonc
// 食物/饮品配置
"drinkFoodInfo": {
  "foodUseTime": 5, // 使用时间
  "hydration": 33, // 水分变化量
  "energy": 33, // 能力变化量
  "maxResource": 20 // 最大资源(适合饮品)
}
```

### medicalInfo

不同药品写法不太一样, 参考我的模板赋值

```jsonc
"medicalInfo": {
  "maxHpResource": null, // 最大治疗耐久
  "hpResourceRate": null, // 每次治疗消耗的耐久
  "medUseTime": null, // 药品使用时间
  "medEffectType": null, // 效果结算方式: duringUse 或 afterUse
  "effects_health": [], // 没有时为[], 有时为{}
  "effects_damage": [], // 没有时为[], 有时为{}
  "bodyPartPriority": [] // 手术包的优先治疗部位
}
```

### ammoInfo

```jsonc
// 弹药属性配置
"ammoInfo": {
    "ammoType": "bullet",

    // 弹道初速（米/秒）
    "initialSpeed": 880.0,
    // 弹头质量（克）
    "bulletMassGram": 16.0,
    // 基础伤害值
    "damage": 80.0,
    // 穿透力（护甲等级）
    "penetrationPower": 35,
    // 对护甲的伤害系数
    "armorDamage": 0.6,

    // 碎裂几率（0.0-1.0）
    "fragmentationChance": 0.15,
    // 跳弹几率（0.0-1.0）
    "ricochetChance": 0.1,
    // 重流血几率增量
    "heavyBleedingDelta": 0.3,
    // 轻流血几率增量
    "lightBleedingDelta": 0.5,

    // 是否曳光弹
    "tracer": true,
    "tracerColor": "tracerRed",
    "caliber": "Caliber556x45NATO",
    // 霰弹子弹数量（仅霰弹有效）
    "buckshotBullets": 8,

    // 精度影响系数
    "ammoAccr": 0.0,
    // 后坐力影响系数
    "ammoRec": 0.0,
    // 射程影响系数
    "ammoDist": 0.0,
    // 每点伤害消耗的体力值
    "staminaBurnPerDamage": 0.5
}
```

## WiKi

- [Wiki-主页](Wiki/主页.md)
- [WiKi-常用常量](Wiki/常用常量)
- [WiKi-子弹相关常量](Wiki/常用常量/子弹相关常量)
