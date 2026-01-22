## effects_health的键(只能用后面的整数)

这个属性(`effects_health`)用于饮品或者食物

```C#
public enum HealthFactor
{
  None = 0,
  Health = 1,
  Hydration = 2,
  Energy = 3,
  Radiation = 4,
  Temperature = 5,
  Poisoning = 6,
  Effect = 100, // 0x00000064
}
```

#### 例子

**饮品**

```jsonc
// 物品juice_army的effects_health，恢复8能量与30水分
"effects_health": {
    "3": {
        "value": 8
    },
    "2": {
        "value": 30
    }
}
```

**食物**

```jsonc
// 物品Crackers的effects_health，扣除5水分，恢复10能量
"effects_health": {
    "3": {
        "value": 10
    },
    "2": {
        "value": -5
    }
}
```

