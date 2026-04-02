> This document focuses on the update content of the mod
>
> Provides links specifying the minimum required Wiki version for the corresponding mod
>
> Missing links indicate that it inherits the Wiki version adapted for the previous mod version

## v0.1.1

Minimum Wiki Version: [Wiki_26_03_29_23_59](Wiki%20Update%20Log.md#wiki_26_03_29_23_59)

**Data Format Changes**
* Removed the `$type` field from item templates (backward compatible reading preserved)
* **Type mechanism clarification**: Item properties are determined by actually assigned Info fields, independent of `baseInfo.type`
* Data files now support JSONC comments (single-line `//` and multi-line `/* */`)

**New Features**
* Supports Chinese rarity keys (普通, 稀有, 超级稀有, 不存在)
* Supports Chinese item sound keys
* Supports ammunition type item validation

**Configuration Updates**
* Added `cacheInitialized` configuration item to control cache initialization skip logic
* Added `alwaysUpdateCache` configuration item to force cache updates
* Added `requiredItemIds` configuration item to validate required item existence and enabled status
* See [Mod Configuration](../About%20This%20Mod/Mod%20Configuration.md) for details

**Validation System**
* Added BaseInfoValidator, AttributeInfoValidator, BuffsInfoValidator
* Added MedicalInfoValidator, DrinkFoodInfoValidator, AmmoInfoValidator
* Removed invalid negative weight validation logic

**Architecture Optimization**
* Changed UpdateProperties and UpdateDatabaseService methods to protected to restrict external calls
* Enhanced null value validation logic for BaseInfo and BuffsInfo

## v0.1.0

Minimum Wiki Version: [Wiki_26_03_16_03_40](Wiki%20Update%20Log.md#wiki_26_03_16_03_40)

* Completely removed all WebUI Razor files, including layouts, pages, and various information components; the original item creation and management interface will be inaccessible
* Removed the built-in WebUI service, deleting references to HttpServer, WebApplicationBuilder, and MudBlazor.Services
* Removed obsolete WebUI access guides and related operational instructions from documentation, pending the completion of the refactored new UI or checking for alternatives
* Optimized the item validation process, unified parameter naming, and ensured attribute generation is executed only after validation passes, avoiding duplicate calculations
* Enhanced the NewItemCommon validation logic to silently clear data and log the reason when BuffsInfo is provided but StimulatorBuffs is not
* Optimized the item loading log format by adding dynamic display of item enable status, improving debug information readability
* Upgraded the project version number from 0.0.4 to 0.1.0, reflecting this major refactoring
* Updated config.json, adding the default configuration ignoreTemplateFiles: true
* Removed the original JSON cache file structure (such as ParentIdCache.json, AmmoCache.json, etc.); required statistical data will be regenerated upon startup

**New Cache System**

* Output Directory: `Game Root Directory\SPT\user\mods\SptItemCreator\StatsCache`
* Content: Based on all current SPT databases, the mod organizes a list of attribute names and all occurring values for each item type
* Purpose: Reference for creating new items; foundation for scripting frequency counts, numerical distribution analysis, mean value calculations, etc.

## v0.0.4

Minimum Wiki Version: [Wiki_26_02_02_18_20](Wiki Update Log.md#wiki_26_02_02_18_20)

- Implemented the Settings Page (SettingPage), which resides permanently in the navigation bar and provides a WebUI configuration interface
- Optimized DataLoader to support dynamically ignoring template files based on configuration
- Moved all template files to the `mod folder/data/模板-Template` directory
- Changed template filenames to bilingual (Chinese-English) to avoid mismatches between some translator results and the actual content
- Adjusted the default maximum log file size from 10MB to 512KB (logs exceeding this size will be recreated when the mod starts)

## v0.0.3

Minimum Wiki Version: [Wiki_26_01_30_14_45](Wiki Update Log.md#wiki_26_01_30_14_45)

- Added WebUI for creating items
    - Added BaseInfoWidget control for visually modifying BaseInfo data
    - Added AttributeInfoWidget control for visually modifying AttributeInfo data
    - Added DrinkFoodInfoWidget control for visually modifying DrinkFoodInfo data
    - Added MedicalInfoWidget control for visually modifying MedicalInfo data
    - Added BuffsInfoWidget control for visually modifying BuffsInfo data
    - Added PropertyOverrideWidget control for simple viewing and modification of PropertyOverride data
    - Added pages for creating **General Items**, **Medical Items**, and **Food & Drinks** (Ammo creation is temporarily on hold due to many related attributes being discovered)
- ParentId is now optional when assigning a valid CloneId. If CloneId is invalid, an error will be reported (the table in the Wiki tutorial has also been updated accordingly)
- Added a cache system to cache ParentId and BackgroundColor data from the SPT server (Cache timestamp: OnLoadOrder.TraderCallbacks - 1)
- Added the `BackgroundColor` field to `AttributeInfo`

## v0.0.2

Minimum Wiki Version: [Wiki_26_01_23_19_00](Wiki Update Log.md#wiki_26_01_23_19_00)

- Added `allowAll`, `canFilter`, and `cantFilter` fields to `baseInfo` to control which containers allow placement of this item
- Added mod metadata URL information
- Removed the static property `ShouldUpdateDatabaseService` from `AbstractInfo`, which was not functioning correctly
- All `AbstractInfo` types now include an `ItemPath` property for easier error output
- Optimized code style during file deserialization
- Fixed logical errors in null checks and error message output within `BuffsInfo`
- Fixed the issue in `测试-食物模板.sic.jsonc` and `测试-饮品模板.sic.jsonc` where `buffsInfo.stimulatorBuffs` was assigned a meaningless empty string

Known Issues

- New container-type items created via cloning, even with `allowAll=true`, may inherit the same container placement restrictions as the cloned item (cannot be placed in other incompatible containers). This issue can be resolved by clearing the local cache, though multiple cache clears may occasionally be necessary.

## v0.0.1

Minimum Wiki Version: [Wiki_26_01_23_12_33](Wiki Update Log.md#wiki_26_01_23_12_33)

- Provided a method for creating common-type items
- Provided encapsulation for creating first-aid kits, injectors, ammunition, and **effects for injectors, food, or drinks**