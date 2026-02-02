## rarityPvE

> Can be assigned either the Chinese keys or English values listed below. The Chinese keys are only for use within this mod's fields other than propertyOverride.

```jsonc
{
    "普通": "Common",
    "稀有": "Rare",
    "超级稀有": "Superrare",
    "不存在": "Not_exist"
}
```

Original enumeration:

```C#
namespace SPTarkov.Server.Core.Models.Enums;

public enum LootRarity
{
  Not_exist = -1, // 0xFFFFFFFF
  Common = 0,
  Rare = 1,
  Superrare = 2,
}
```

