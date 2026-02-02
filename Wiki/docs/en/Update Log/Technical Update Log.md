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
