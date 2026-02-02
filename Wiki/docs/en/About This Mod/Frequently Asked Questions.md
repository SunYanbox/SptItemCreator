## Q: Regarding some terms mentioned in the Wiki

Game folder: The folder containing the "SPT" folder and the "EscapeFromTarkov.exe" program file after correctly installing SPT 4.0.0 or later.

> Some programming knowledge is helpful for understanding.

Property: An attribute of a class; a key-value pair in [items.json](../Data Structure/Common Data Structures in SPT/SPTData/items.json.md) is considered a property.

Data file: By default, refers to new item data files in this module with the `.sic`, `.sic.json`, `.sic.jsonc` formats

## Q: How do I start creating a new item?

1. Copy the template file and edit it using an editor like VsCode in Jsonc format based on the template.

   > Be sure to modify the item Id in the template file to ensure it is unique.

2. Use the WebUI to create a new item based on detailed help text (editing existing files via WebUI is not currently supported).

   > If the item Id is default in the WebUI, click the **Check Data** button to immediately generate a MongoId.

## Q: Where can I find the mod's WebUI?

By default, it is `https://127.0.0.1:6969/SIC`. Specifically, it is the address opened by the SPT server + "/SIC".

## Q: The location of the template file mentioned in the Wiki.

In the **mod's main folder** under `data/Template-Template`, for mod versions v0.0.3 and earlier, it is under the `data` folder.

File names containing the strings `模板` or `Template`.

## Q: Why is the jsonc format used, but comments are not currently supported?

To support serializing/deserializing SPT-specific types, SPT's JsonUtil is used. Since this does not support the Jsonc format, files with comments cannot be loaded (when I have time, I might try removing comments before using JsonUtil, but comments cannot be re-added when saving).

## Q: Why does your documentation say some properties don't need to be assigned, but I can find assigned item templates in the current SPT version's data files?

1. The current data is organized based on **SPT 4.0.11**, and future data changes may not be updated promptly.
2. Since there are many item templates (>4000), and each category of item templates typically has dozens of entries, there may be omissions during the organization process.

> If you encounter such issues, you can submit an issue on Github: [Issues · SunYanbox/SptItemCreator](https://github.com/SunYanbox/SptItemCreator/issues)

## Q: Why do some keys in the data files need to be capitalized while others need to be lowercase?

All properties provided by this mod uniformly use keys starting with lowercase letters. Some SPT data types have properties starting with uppercase letters. When this mod's properties include these types, the keys of the properties within these types start with uppercase letters.

## Q: Why hasn't the WebUI and documentation detailed bullet-related data?

There are too many bullet-related properties, and it's overwhelming to organize them all 😭.

## Q: I have item files edited by other mods, but their data structure differs from this mod's. How can I migrate them to this mod for unified management?

Copy the item ID, locales localization data, parentId, handbookParentId, cloneId, price, selling trader, and other information from the other mods into the baseInfo of the following file. Then, assign all other properties that meet the [TemplateItemProperties](../Data Structure/Common Data Structures in SPT/Related To New Items/TemplateItemProperties.md) type under "propertyOverride".

```json
{
    "$type": "common",
    "enable": false,
    "baseInfo": {
        "id": ""  // Must be provided
        // Place other baseInfo here
    },
    "propertyOverride": {
        // Copy all other properties here
    }
}
```