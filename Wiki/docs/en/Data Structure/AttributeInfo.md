## AttributeInfo

> A property information record class inherited from `AbstractInfo`, used to define the item's physical properties, interaction properties, and gameplay mechanics properties.

| Property              | In Data File       | Type   | Default  | Description                         | Options                                                       | Optionality |
| --------------------- | ------------------ | ------ | -------- | ----------------------------------- | ------------------------------------------------------------ | ----------- |
| **Weight**            | "weight"           | double | null     | Item weight (kg)                    | A real number                                                 | Optional    |
| **Width**             | "width"            | int    | null     | Item width (in slots)               | Positive integer                                              | Optional    |
| **Height**            | "height"           | int    | null     | Item height (in slots)              | Positive integer                                              | Optional    |
| **RarityPvE**         | "rarityPvE"        | string | null     | Item rarity in PvE mode             | [LootRarity](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Enums/LootRarity.cs) | Optional    |
| **DiscardLimit**      | "discardLimit"     | double | null     | Discard limit value                 | Common values: -1, 0, 1, 2                                   | Optional    |
| **ItemSound**         | "itemSound"        | string | "generic"| Item interaction sound type         | [Values assignable to itemSound](../Common Constants/String Constants/Available values for itemSound.md) | Optional    |
| **StackMaxSize**      | "stackMaxSize"     | int    | 1        | Maximum stack size                  | Common: 1, 20, 40, 50, 60,<br/>Currency between 100~100e4    | Optional    |
| **ExaminedByDefault** | "examinedByDefault"| bool   | true     | Whether examined by default         | Usually true                                                  | Optional    |
| **ExamineTime**       | "examineTime"      | double | null     | Time required to examine (seconds)  | Usually 1                                                     | Optional    |
| **LootExperience**    | "lootExperience"   | int    | null     | Experience gained from looting      | Mostly between 5~100                                         | Optional    |
| **ExamineExperience** | "examineExperience"| int    | null     | Experience gained from examining    | Mostly between 5~100                                         | Optional    |
| **BackgroundColor**   | "backgroundColor"  | string | null     | Item background color               | default, blue, black, etc.<br />Details can be viewed in the cache folder after the mod runs | Optional    |

## attributeInfo Field

```json
// Item property configuration
"attributeInfo": {
    // Item weight (kg)
    "weight": 0.3,
    // Item width in stash (in slots)
    "width": 3,
    // Item height in stash (in slots)
    "height": 3,
    // Rarity level in PvE mode, refer to constants at the end, key/value in dictionary both work
    "rarityPvE": "Common",
    // Discard limit, null means inheriting the limit from the cloned item
    "discardLimit": null,
    // Item interaction sound type, refer to constants below for assignment
    "itemSound": "food_snack",
    // Maximum stack size, null means inheriting the max stack size from the cloned item
    "stackMaxSize": null,
    // Whether examined by default (no need to examine again)
    "examinedByDefault": true,
    // Time required to examine (seconds)
    "examineTime": 2,
    // Experience gained when looting
    "lootExperience": 50,
    // Experience gained when examining
    "examineExperience": 500,
    // Item background color
    "backgroundColor": "default"
}
```

For rarityPvE, refer to [rarityPvE Rarity](../Common Constants/String Constants/rarityPvE.md)

For itemSound, refer to [Values assignable to itemSound](../Common Constants/String Constants/Available values for itemSound.md)

## Examples

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

Fields that are null can also be omitted:

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
