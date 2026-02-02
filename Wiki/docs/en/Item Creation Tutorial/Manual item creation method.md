## Step 1: Copy the Template

Copy the template file (.sic.jsonc) from the `Mod Folder/data/模板-Template` folder in the mod package.

Copy and rename it to any location within `Mod Folder/data` or its subfolders.

For example:

- `Mod Folder/data/YourNewItem.sic.jsonc`
- `Mod Folder/data/Ammo/YourNewAmmo.sic.jsonc`
- `Mod Folder/data/SomeAuthorName/HisMedicine/HisMedicine.sic.jsonc`

### Step 2: Configure Key Fields

```json
{
  "$type": "common",
  "enable": true,
  "baseInfo": {
    "id": "6900c8e93ea877662a000001",
    "name": "My Custom Item",
    "parentId": "BaseItemID"
  },
  "propertyOverride":{
      
  }
}
```

1. Refer to `Game Folder\SPT\SPT_Data\database\templates\items.json` to search for data

   Look up the properties you need from your `Game Folder\SPT\SPT_Data\database\templates\items.json` and copy those properties to `propertyOverride`.

2. Refer to the attribute explanations provided in the data structure folder to modify the values of the given fields.

### Step 3: Add Models (Optional)

1. Place model files in the `bundles` folder.
2. [Register the model in `bundles.json`](Add Custom Prefab Path.md).
3. Reference the path in the item configuration.