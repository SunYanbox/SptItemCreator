## MedicalInfo

> Defines the healing effects, usage parameters, and body part priorities for medical items.

| Property                | **In Data File**        | Type                                                         | Description                                               | Common Values & Options                                      | Optionality |
| ----------------------- | ----------------------- | ------------------------------------------------------------ | --------------------------------------------------------- | ------------------------------------------------------------ | ----------- |
| **MaxHpResource**       | "maxHpResource"         | int                                                          | Maximum health resource / usage count                     | Injectors: usually 0; First Aid Kits: generally 400+; Surgery Kits: generally 3~15 | Optional    |
| **HpResourceRate**      | "hpResourceRate"        | double                                                       | Health resource recovery rate                             | Common: 50, 60, 70, 85, 175; Sanitar's value is 1            | Optional    |
| **MedUseTime**          | "medUseTime"            | double                                                       | Medical item usage time (seconds)                         | Common: 2, 3, 5, 7, 16, 20                                   | Optional    |
| **MedEffectType**       | "medEffectType"         | string                                                       | Medical effect type identifier                            | afterUse or duringUse                                         | Optional    |
| **EffectsHealth**       | "effectsHealth"         | Dictionary<[HealthFactor](./Common Data Structures in SPT/Related To The Effect/HealthFactor.md), EffectsHealthProperties> | Health recovery effect configuration                     | [HealthFactor](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/HealthFactor.cs), generally set as /[/]/(You read that right, in medical items it's an empty list/) | Optional    |
| **EffectsDamage**       | "effectsDamage"         | Dictionary<[DamageEffectType](./Common Data Structures in SPT/Related To The Effect/DamageEffectType.md), EffectsDamageProperties> | Damage healing effect configuration                     | [DamageEffectType](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/DamageEffectType.cs) is the key | Optional    |
| **BodyPartPriority**    | "bodyPartPriority"      | List<string>                                                 | Surgery kits only, body part treatment priority (e.g., "Stomach", "RightLeg") | As shown below                                                | Optional    |

> If assigning BodyPartPriority, it is generally as follows:
> ```json
> "BodyPartPriority": [
>   "Stomach",
>   "RightLeg",
>   "LeftLeg",
>   "RightArm",
>   "LeftArm"
> ]
> ```
> If used within this mod, change BodyPartPriority to bodyPartPriority.

## medicalInfo Field

Different medical items have slightly different configurations; refer to my template for assignment.

```json
"medicalInfo": {
"maxHpResource": null, // Maximum healing durability
"hpResourceRate": null, // Durability consumed per treatment
"medUseTime": null, // Medical item usage time
"medEffectType": null, // Effect resolution method: duringUse or afterUse
"effects_health": [], // When none, use []; when present, use {}
"effects_damage": [], // When none, use []; when present, use {}
"bodyPartPriority": [] // Surgery kit treatment priority for body parts
}
```

## Example

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

Currently, the "effects_health" of all drugs are set to []

