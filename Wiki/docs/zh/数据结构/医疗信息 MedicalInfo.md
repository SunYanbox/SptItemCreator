## MedicalInfo 医疗物品信息类

> 定义医疗物品的治疗效果、使用参数和身体部位优先级。

| 属性                 | **在数据文件中的写法** | 类型                                                         | 说明                                                      | 常见数值与可选项                                             | 可选性 |
| -------------------- | ---------------------- | ------------------------------------------------------------ | --------------------------------------------------------- | ------------------------------------------------------------ | ------ |
| **MaxHpResource**    | "maxHpResource"        | int                                                          | 最大生命值资源/使用次数                                   | 针剂一般为0；急救包一般400+；手术包一般3~15                  | 可选   |
| **HpResourceRate**   | "hpResourceRate"       | double                                                       | 生命值资源恢复速率                                        | 常见50，60，70，85，175；Sanitar的该值为1                    | 可选   |
| **MedUseTime**       | "medUseTime"           | double                                                       | 医疗物品使用时间（秒）                                    | 常见2，3，5，7，16，20                                       | 可选   |
| **MedEffectType**    | "medEffectType"        | string                                                       | 医疗效果类型标识                                          | afterUse或duringUse                                          | 可选   |
| **EffectsHealth**    | "effectsHealth"        | Dictionary<[HealthFactor](./SPT常用数据结构/效果相关/HealthFactor.md), EffectsHealthProperties> | 生命值恢复效果配置                                        | [HealthFactor](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/HealthFactor.cs), 一般设置为/[/]/(你没看错, 在药品中就是空的列表/) | 可选   |
| **EffectsDamage**    | "effectsDamage"        | Dictionary<[DamageEffectType](./SPT常用数据结构/效果相关/DamageEffectType.md), EffectsDamageProperties> | 伤害治疗效果配置                                          | [DamageEffectType](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/DamageEffectType.cs)是键 | 可选   |
| **BodyPartPriority** | "bodyPartPriority"     | List<string>                                                 | 仅限手术包，身体部位治疗优先级（如"Stomach"、"RightLeg"） | 如下所示                                                     | 可选   |

> 如果赋值BodyPartPriority，一般为以下内容
>
> ```json
> "BodyPartPriority": [
>  "Stomach",
>  "RightLeg",
>  "LeftLeg",
>  "RightArm",
>  "LeftArm"
> ]
> ```
>
> 如果是在本模组中使用，需要把BodyPartPriority改为bodyPartPriority

## medicalInfo字段

不同药品写法不太一样, 参考我的模板赋值

```json
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

## 示例

```json
"medicalInfo": {
    "maxHpResource": 10,
    "hpResourceRate": null,
    "medUseTime": null,
    "medEffectType": "duringUse",
    "effects_health": [],
    "effects_damage": null,
    "bodyPartPriority": null
}
```

目前所有药品的"effects_health"都赋值为[]

