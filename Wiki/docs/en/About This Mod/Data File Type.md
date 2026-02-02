**File extension requirement: `.sic`, `.sic.json`, or `.sic.jsonc`**

Template file naming convention: Data file names containing `模板` (Template) or `Template`

## common-type data file

```json
{
    "$type": "common", // Required
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

**Note: id and $type are required. If traderId is not provided or is incorrect, the item will not be added to a trader. Other attributes have default values.**

The data type of `propertyOverride` is [TemplateItemProperties](../Data Structure/Common Data Structures in SPT/Related To New Items/TemplateItemProperties.md), and its priority is lower than other encapsulated fields (e.g., if the `Weight` attribute is assigned in both `attributeInfo` and `propertyOverride`, the mod will use the `Weight` attribute from `attributeInfo` to override the one in `propertyOverride`).

## drinkOrFood-type data file

Based on the fields already present in common, it additionally has the `drinkFoodInfo` field.

This field can set food usage time, food durability (specifically for drinks), and energy/water restoration or deduction values.

## medical-type data file

Based on the fields already present in common, it additionally has the `medicalInfo` field.

It encapsulates data such as medical item durability/usage count, health restoration rate, medical item usage time, medical effect activation type, and surgical priority treatment area.

## ammo-type data file

Based on the fields already present in common, it additionally has the `ammoInfo` field.

It encapsulates data such as ammunition type, initial speed, projectile mass, base damage value, armor penetration capability, damage to armor, fragmentation probability, ricochet probability, bleeding-related damage values, tracer-related data, and recoil dispersion.

## Supported Fields and Examples by Different Types

Common fields: **BaseInfo** (required), AttributeInfo, BuffsInfo, propertyOverride

> Types inheriting from `NewItemCommon`

| $type       | Supported Special Fields  |
| ----------- | ------------------------- |
| common      | None                      |
| drinkOrFood | **drinkFoodInfo**         |
| medical     | medicalInfo               |
| ammo        | ammoInfo                  |