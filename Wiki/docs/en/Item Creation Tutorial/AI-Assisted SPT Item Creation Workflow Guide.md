# AI-Assisted SPT Item Creation Workflow Guide

Based on the documentation content of the SPT Item Creation Mod, the following workflow utilizes AI technology to assist users in efficiently creating custom items.

## Workflow Overview

### Phase One: Initial Configuration
1. **Template Preparation**
    - Copy the `.sic.jsonc` template file from the mod to the `data` directory
    - Rename the file and place it in an appropriate category folder
    - *AI Assistance*: Recommend storage paths and naming conventions based on item type

2. **Basic Information Configuration**
    - Set required fields in `baseInfo`: `id` (must be unique), `name`, `type`
    - Configure `parentId` (inherit base attributes) or `cloneId` + `handbookParentId` (clone prototype)
    - *AI Assistance*:
        - Automatically generate a unique `MongoId`
        - Recommend appropriate `parentId`/`cloneId` values based on item type
        - Provide real-time explanations of field meanings

### Phase Two: Attribute Definition
3. **Core Attribute Override**
    - Find and copy `_props` attributes from `items.json` to `propertyOverride`
    - **Using the Cache System (v0.1.0+)** : The mod generates statistical data files categorized by item type in the `SPT\user\mods\SptItemCreator\StatsCache` path. You can send JSON files of specific types (copy relevant parts if dealing with large files) to AI for analysis
    - Pay attention to basic attributes such as item weight, dimensions, stack count
    - *AI Assistance*:
        - Automatically recommend attribute value ranges based on item category
        - Verify the reasonableness of attribute configurations
        - Highlight common configuration errors
        - Analyze item attribute statistics from cache files and provide configuration suggestions

4. **Specialized Attribute Configuration**
    - **General Items**: Configure `attributeInfo` (weight, dimensions, rarity, etc.)
    - **Medical Items**: Configure `medicalInfo` (treatment parameters, effect types)
    - **Food/Drink Items**: Configure `drinkFoodInfo` (usage time, recovery values)
    - **Buff Items**: Configure `buffsInfo` (effect list, duration)
    - *AI Assistance*:
        - Intelligently recommend attribute combinations based on item type
        - Generate value suggestions that maintain game balance
        - Verify dependencies between fields

### Phase Three: Model & Localization
5. **Custom Model Integration**
    - Place model files in the `bundles` folder
    - Register the model path in the `manifest` of `bundles.json`
    - Reference the model path in `baseInfo.prefab`
    - *AI Assistance*:
        - Verify the correctness of model paths
        - Detect compatibility issues with model files

6. **Localization Configuration**
    - Configure multilingual text in `baseInfo.locales`
    - Supports languages available in the game, such as Chinese, English
    - *AI Assistance*:
        - Automatically generate item description text
        - Provide name translation suggestions
        - Maintain consistency across multilingual text

### Phase Four: Validation & Activation
7. **Data Validation**
    - Check the uniqueness of `id`
    - Verify that all required fields are complete
    - Confirm values are within reasonable ranges
    - After running the server, check the mod logs in `SPT\user\mods\SuntionCore\ModLogs` to identify compatibility issues
    - *AI Assistance*:
        - Provide specific error correction suggestions
        - Analyze error information in log files

8. **Activation & Testing**
    - Set the `enable` field to `true`
    - Load the mod into the SPT server for testing
    - *AI Assistance*:
        - Generate test case suggestions
        - Highlight common runtime issues

## AI Assistance Capability Matrix

| Task Type | AI Assistance Function | Documentation Basis |
|---------|-----------|---------|
| **Field Explanation** | Real-time explanation of field meaning, type, default value | Field descriptions in data structure documentation |
| **Value Generation** | Recommend reasonable value ranges and suggested values | Common value tables in various Info classes |
| **Configuration Validation** | Check configuration completeness and reasonableness | Requirements description for data file types |
| **Issue Diagnosis** | Identify common errors and provide solutions | Q&A in FAQ documentation |
| **Workflow Guidance** | Guide users through the correct sequence of operations | Step-by-step instructions in item creation tutorials |

## Best Practice Tips

1. **Always Start from a Template**: Avoid manually creating data structures, reducing formatting errors
2. **Prioritize Encapsulated Fields**: Encapsulated fields like `attributeInfo`, `medicalInfo` take precedence over `propertyOverride

   > *Additional Note: If you are migrating a large amount of existing item data from other mods, using `propertyOverride` directly for batch migration will be more convenient and efficient, as it eliminates the need to break down existing attributes into individual encapsulated fields.*

3. **Maintain ID Uniqueness**: Use AI-generated `MongoId` to ensure no conflict with existing items (Using a MongoId generator is more appropriate)
4. **Progressive Configuration**: Configure required fields first, then gradually add optional features
5. **Test-Driven Development**: Enable and test promptly after completing key configurations to avoid error accumulation

## Important Notes

- Data file extensions must be `.sic`, `.sic.json`, or `.sic.jsonc`
- Attributes in `propertyOverride` will be overridden by encapsulated fields
- When `cloneId` is not provided, all attributes must be defined through `propertyOverride`
- Model paths must use relative paths relative to the `bundles` folder
- Version v0.1.0+ provides a cache system; view attribute statistics by item type in the `SPT\user\mods\SptItemCreator\StatsCache` path
- After running the server, check the mod logs in `SPT\user\mods\SuntionCore\ModLogs` to identify compatibility issues

This workflow breaks down the complex process of SPT item creation into manageable steps and lowers the configuration barrier through AI assistance, improving creation efficiency and accuracy.