Place your custom models in the `bundles` folder, reference the relative path of the model relative to the `bundles` folder when writing `prefab` and `usePrefab`, and then add the corresponding model path information in the `bundles.json` file.

## Adding Models

For example, if you place a model file named `ammo_5x45_superSSA.bundle` in the `bundles` folder,

then you need to add the following content to the `manifest` field in the mod folder's `bundles.json`:
```json
{
  "key": "ammo_5x45_superSSA.bundle",
  "dependencyKeys": []
}
```
> Then, in your item's `sic.jsonc` file, the `prefab` field in `baseInfo` should be filled as follows:
```json
"prefab": {
  "path": "ammo_5x45_superSSA.bundle",
  "rcid": ""
}
```

Example:

![Example Image for Adding Models](../img/添加模型的示例图.png)

## Adding Models Organized by Folders

If you use folders for organization within `bundles`, the path and key need to be the complete relative path relative to the `bundles` folder (omitting `./`).

For example: if the relative path to the `bundles` folder is `AmmoClasses/ammo_5x45_superSSA.bundle`,
then whether in `baseInfo`, `bundles.json`, or the `overrideProperties` attribute, you must use **AmmoClasses/ammo_5x45_superSSA.bundle** as the path or key.