## rarityPvE 稀有度

```jsonc
{
    "普通": "Common",
    "稀有": "Rare",
    "超级稀有": "Superrare",
    "不存在": "Not_exist"
}
```

原枚举：

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

