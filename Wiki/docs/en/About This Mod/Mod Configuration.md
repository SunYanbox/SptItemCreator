## Mod Configuration

Configuration file location: `Game Root Directory\SPT\user\mods\SptItemCreator\config.json`

### Configuration Items Explanation

| Configuration Item | Type | Default Value | Description |
|-------------------|------|---------------|-------------|
| ignoreTemplateFiles | bool | true | Ignore files and folders with names containing "模板" or "Template" |
| cacheInitialized | bool | false | Whether the cache has been initialized. Automatically set to true after first run, subsequent startups skip statistical calculations |
| alwaysUpdateCache | bool | false | When set to true, forces hash checks and updates changed caches |
| requiredItemIds | List\<string\> | null | Required item ID list, validates existence and enabled status at startup |

### Example Configuration

```json
{
    "ignoreTemplateFiles": true,
    "cacheInitialized": false,
    "alwaysUpdateCache": false,
    "requiredItemIds": []
}
```

### Configuration Item Details

#### ignoreTemplateFiles

Controls whether to ignore template files. When set to `true`, the mod skips files and all files within folders whose names contain "模板" or "Template".

**Use Cases**:
- Using template files as references during development/debugging without loading them into the game
- Keeping template files in the data directory as examples

#### cacheInitialized

Cache initialization status flag. The mod automatically sets this to `true` after completing statistical data collection during the first run.

**Use Cases**:
- First startup: `false`, the mod performs statistical calculations
- Subsequent startups: `true`, skips statistical calculations for faster startup
- To recalculate statistics, manually change to `false`

#### alwaysUpdateCache

Forces cache updates. When set to `true`, regardless of the `cacheInitialized` status, hash checks are performed and changed caches are updated.

**Use Cases**:
- Need to rebuild cache after SPT database updates
- Large-scale changes to item data

#### requiredItemIds

Required item ID list. At startup, the mod validates whether items corresponding to these IDs exist and are enabled.

**Use Cases**:
- Your mod depends on items created by other mods
- Ensuring dependent items are correctly loaded

**Example**:

```json
{
    "requiredItemIds": [
        "5448e8d04bdc2ddf718b4569",
        "5448ff904bdc2d6f028b456e"
    ]
}
```

When items in the list are missing or not enabled, the mod outputs detailed error logs:

```
[ERROR] Required item does not exist: 5448e8d04bdc2ddf718b4569
[ERROR] Required item not enabled: 5448ff904bdc2d6f028b456e
```