> This document focuses on the update content of the Wiki documentation
>
> Provides links specifying the minimum required mod version for the corresponding Wiki
>
> Missing links indicate that it inherits the mod version adapted for the previous Wiki version

## Wiki_26_02_02_18_20

Minimum Mod Version: [v0.0.4](Mod Update Log.md#v004)

- Added [Technical Update Log](./Technical Update Log.md); underlying-related update information has been moved here.

- Added light/dark theme toggle functionality to the Wiki interface

- Added a new **Data Structures** module, organizing commonly used types and assets in SPTData
  - Regarding `0. Understanding Mod Data Structures (Optional).md`
    - Extracted information about X and moved it to the new module, adding data type details and their representation in data files for X
    - X: BaseInfo, DrinkFoodInfo, MedicalInfo, BuffsInfo, AttributeInfo

  - Added comprehensive documentation for frequently used SPT data structures, covering core types such as Buff, LocaleDetails, Prefab, MongoId, and more

  - Created explanations for SPT data files, including detailed breakdowns of globals.json and items.json

- Fixed the example description error for the `effects_damage` key
- Removed duplicate Wiki interfaces after restructuring the data structures
- When navigating to external links in the Wiki, a new tab will open.
- The Wiki's search functionality supports Chinese keyword search.
- Updated the English Wiki

## Wiki_26_01_30_14_45

Minimum Mod Version: [v0.0.3](Mod Update Log.md#v003)

- Added relevant MongoId constant data for common ParentIds.
- Added relevant color string constant data for BackgroundColor.
- Added new field data adapted for versions after v0.0.3.
- Fixed potentially ambiguous text and clarified previously unclear information.
- Reorganized Wiki documentation.

## Wiki_26_01_28_14_00

- Refactored the organization of update logs in the Wiki documentation, separating mod updates from Wiki updates.
- Corrected potentially ambiguous text in `Wiki\docs\README.md`.
- Revised mod field descriptions, clarifying the priority rules for the `propertyOverride` attribute.
- Supplemented the known issue descriptions and handling solutions in the update logs.
- Added usage instructions and example requirements for all constant documentation.
- Improved bullet-related constant documentation, adding type usage guidance.
- Unified the format of currency and trader ID documentation, emphasizing the MongoId string format.
- Updated rarity constant descriptions, clearly indicating support for bilingual (Chinese-English) key-value pairs.

## Wiki_26_01_23_19_00

Minimum Mod Version: [v0.0.2](Mod Update Log.md#v002)

- Added explanations for the new `allowAll`, `canFilter`, and `cantFilter` fields in **1. Fields Used by the Mod**.
- Fixed incorrect descriptions of usage in beverage, food, and medical-related constants. Both `effects_health` and `effects_damage` support using enumeration key names as effect keys.
- Updated handbook-type IDs, supplementing detailed IDs for weapon attachments, containers, secure containers, and other types.

## Wiki_26_01_23_12_33

Minimum Mod Version: [v0.0.1](Mod Update Log.md#v001)

- Established a new constant documentation system, splitting into detailed constant files categorized by bullets, MongoIds, medical items, etc.
- Added a complete item creation tutorial, containing four chapters: field analysis, data structure, file creation, and model addition.
- Optimized README structure, removed redundant constant lists, and added Wiki navigation and custom model configuration instructions.
- Fixed grammatical errors in trader ID documentation, unified the format of all documents, and provided complete JSON and C# enum references.
- Enhanced usage instructions for constant files, added specific examples and usage scenario descriptions to improve documentation practicality.