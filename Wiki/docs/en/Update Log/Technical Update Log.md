## v0.1.1

**Architecture Refactoring**
* Refactored item loading and validation architecture, implementing a unified validator system
* **Unified NewItem class**: All item types share the `NewItem` class, with property application determined by assigned AbstractInfo instances
* Refactored error collector to support carrying item path information, facilitating issue file location
* Changed UpdateProperties and UpdateDatabaseService method access modifiers to `protected`

**DataLoader Enhancement**
* Added StripJsoncComments method to support JSONC comment stripping
* Uses a state machine to correctly handle comment markers and escape characters within strings
* Improved deserialization error logging, adding complete exception stack trace information

**Validator System**
* Implemented IValidator interface with chained validator calls
* Added BaseInfoValidator: Validates required fields, MongoId format, type legality
* Added AttributeInfoValidator: Validates positive integer dimensions, rarity, sound types
* Added BuffsInfoValidator: Validates Buff effect configurations
* Added MedicalInfoValidator: Validates medical item configurations
* Added DrinkFoodInfoValidator: Validates food and drink configurations
* Added AmmoInfoValidator: Validates ammunition configurations, projectile count logic

**Configuration System**
* Added `cacheInitialized` configuration item to achieve persistent cache initialization state
* Added `alwaysUpdateCache` configuration item to support forced cache refresh
* Added `requiredItemIds` configuration item to support dependency item validation

**Unit Testing**
* Added SptItemCreator.Tests testing project
* Added NewItem validation tests
* Added DataLoader JSONC parsing tests
* Added various ammunition test templates

## v0.1.0

* Refactored project namespaces, reorganizing core classes by functionality into SptItemCreator.Models and SptItemCreator.Core
* Refactored the logging system, completely rewriting the LocalLog class to uniformly use SuntionCore.Services.LogUtils.ModLogger as the logging interface, removing dependency injection parameters
* Transformed the TryCatch method into a static method and added Stopwatch performance monitoring mechanism
* Added a new PropertyList data structure to uniformly store various attribute lists such as IntProps, StringProps, MongoIdProps
* Added a new PropertyListExtensions extension class, enabling type-safe attribute addition and cleanup through pattern matching
* Added a new StatsHandler core component, implementing automatic extraction and aggregation statistics of non-null public attributes from item templates based on reflection
* Added a new PropertyStatsService service, implementing asynchronous batch loading, persistence, and data validation of statistical data
* Introduced a hashing mechanism to optimize cache writing logic, achieving intelligent disk writing by calculating JSON content hash values
* Introduced System.Threading.Channels.Channel to implement a thread-safe asynchronous channel for collecting write results and updating the global hash cache
* Set the version dependency for com.suntion.suntioncore in ModMetadata to >=1.2.0
* Removed the DebugHelper debugging module and all debugging methods
* Removed item type string constants and mapping dictionaries from ItemType.cs
* Fixed the logical error in the BuffsInfo.TryAdd method where a success log was incorrectly reported upon addition failure
* Corrected the defect where the hash.json file could be mistakenly processed as statistical data during cache loading

## v0.0.4

- Refactored service namespaces, unifying core services such as DataLoader, LocalLog, and SPTDataCacheService under `SptItemCreator.Services`.
- Created the ConfigService configuration management service, providing asynchronous loading/saving of configurations, encapsulating SPT's logging functionality, unifying server-side logging for the mod, and using SPT log output methods with name+version format.
- Added the ModConfig model class to define the mod configuration structure, supporting settings for synchronized logging, file size limits, template file exclusions, etc.
- Extended the LocalLog service, integrating with ConfigService to implement intelligent log output control (outputting local logs to the SPT server console).
- Implemented the Settings Page (SettingPage), providing a WebUI configuration interface.
- Optimized DataLoader to support dynamically ignoring template files based on configuration.
- Added configuration persistence support, enabling real-time saving and loading.
- Removed redundant null reference checks.
- Changed the information about skipping template files to Debug level.
- Optimized the display format of newly loaded items in local logs to be more suitable for viewing in VsCode.

## Wiki

- Migrated from the `mkdocs` theme to the `mkdocs-material` theme.
- Added multi-language support configuration, including Chinese and English versions.

## v0.0.1

- Implemented the mod's supplemental Wiki based on Python + MkDocs.

## Wiki_26_01_23_12_33

- Created a complete Wiki directory structure based on [Python MkDocs](https://www.mkdocs.org/), including the homepage, update logs, and Git commit tag guidelines.
- Integrated the MkDocs documentation system, supporting Chinese interface and theme switching, providing keyboard shortcut help (?/n/p/s), and offering comprehensive technical documentation and API links.
