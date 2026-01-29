## rarityPvE 稀有度

> 赋值为下面出现的中文键或英文值均可，中文键仅限本模组除了使用propertyOverride以外的字段使用

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

