**File extension requirement: `.sic`, `.sic.json`, or `.sic.jsonc`**

Template file naming convention: Data file names containing `模板` (Template) or `Template`

## JSONC Comment Support

Data files support JSONC format with comments, using the following comment syntax:

```jsonc
{
    // Single-line comment: item unique ID
    "id": "6900c8e93ea877662a000012",
    
    /* 
     * Multi-line comment:
     * Item property description
     */
    "attributeInfo": {
        "weight": 0.3
    }
}
```

**Note**: Comment markers within strings are handled correctly and will not be mistakenly deleted.

## Type Mechanism Explanation

Item properties are determined by the **actually assigned Info fields**. The mod automatically applies all provided Info data:

| Provided Info Field | Applied Properties |
|------------------|-----------|
| `attributeInfo` | Physical properties (weight, dimensions, rarity, etc.) |
| `buffsInfo` | Buff effects (beneficial effects for food/stims) |
| `drinkFoodInfo` | Food/Drink properties (usage time, energy restoration, etc.) |
| `medicalInfo` | Medical properties (durability, recovery rate, usage time, etc.) |
| `ammoInfo` | Ammunition properties (damage, armor penetration, ballistics, etc.) |

**Note**: The `baseInfo.type` field is for identification purposes only and does not affect actual property application logic. Multiple Info fields can be provided simultaneously, and the mod will apply all assigned properties sequentially.

## common-type data file

```json
{
    "enable": false, // Change to true after modification and confirmation to add
    "baseInfo": {
        "id": "6900c8e93ea877662a000012", // Required and must be unique
        ......
    },
    "propertyOverride": { // Optional, provides comprehensive property override functionality
        ......
    },
    "attributeInfo": { // Optional, encapsulates commonly used item attributes
        ......
    },
    "buffsInfo": { // Optional, encapsulates effect registration for food/stims
        ......
    }
}
```

General item type. All unencapsulated modification data other than `baseInfo`, `attributeInfo`, and `buffsInfo` must be written into the `propertyOverride` field.

Note: When `cloneId` is not provided in `baseInfo`, all attributes of the new item come from the `propertyOverride` field.

**Note: id is required. If traderId is not provided or is incorrect, the item will not be added to a trader. Other attributes have default values.**

The data type of `propertyOverride` is [TemplateItemProperties](../Data Structure/Common Data Structures in SPT/Related To New Items/TemplateItemProperties.md), and its priority is lower than other encapsulated fields (e.g., if the `Weight` attribute is assigned in both `attributeInfo` and `propertyOverride`, the mod will use the `Weight` attribute from `attributeInfo` to override the one in `propertyOverride`).

## drinkOrFood-type data file

Provides the `drinkFoodInfo` field, which can set food usage time, food durability (specifically for drinks), and energy/water restoration or deduction values.

## medical-type data file

Provides the `medicalInfo` field, which encapsulates data such as medical item durability/usage count, health restoration rate, medical item usage time, medical effect activation type, and surgical priority treatment area.

## ammo-type data file

Provides the `ammoInfo` field, which encapsulates data such as ammunition type, initial speed, projectile mass, base damage value, armor penetration capability, damage to armor, fragmentation probability, ricochet probability, bleeding-related damage values, tracer-related data, and recoil dispersion.

## Supported Fields and Examples by Different Types

Common fields: **BaseInfo** (required), AttributeInfo, BuffsInfo, propertyOverride

| Provided Special Field | Corresponding Function Type |
| -------------- | ------------ |
| None           | General item |
| drinkFoodInfo  | Food/Drinks  |
| medicalInfo    | Medical items|
| ammoInfo       | Ammunition   |

> **Backward Compatibility**: The `$type` field in older version data files can still be read correctly but will not be output to new files. It is recommended to remove the `$type` field and use Info fields to control property application.