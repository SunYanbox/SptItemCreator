## BuffsInfo 增益效果信息类

继承自 `AbstractInfo` 的效果记录类，用于定义食物，饮品或针剂提供的状态效果

| 属性                | 在数据文件中的写法 | 类型                                             | 说明           | 可选性 |
| ------------------- | ------------------ | ------------------------------------------------ | -------------- | ------ |
| **StimulatorBuffs** | "stimulatorBuffs"  | string                                           | 效果唯一标识符 | 可选   |
| **Buffs**           | "buffs"            | List<[Buff](./SPT常用数据结构/效果相关/Buff.md)> | 增益效果列表   | 可选   |

可以只赋值`StimulatorBuffs`为一个已有的状态效果，或者两个一起赋值；其他情况会报错

## buffsInfo字段

这个字段是给食物/饮品/注射器的, 方便注册新buff

```json
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

可以使用**[globals.json](./SPT常用数据结构/SPTData/globals.json.md)**中已注册的buff名称

```json
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

## 示例

```json
"buffsInfo": {
    "stimulatorBuffs": "BuffsSuntionElixirOfStrength",
    "buffs": [
        {
            "AbsoluteValue": true,
            "BuffType": "StaminaRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 10,
            "SkillName": "",
            "Value": 12
        },
        {
            "AbsoluteValue": true,
            "BuffType": "HandsTremor",
            "Chance": 1,
            "Delay": 0,
            "Duration": 5,
            "SkillName": "",
            "Value": 0
        }
    ]
}
```

