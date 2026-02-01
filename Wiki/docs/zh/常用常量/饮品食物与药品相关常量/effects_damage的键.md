## effects_damage的键

> 既可以用前面的名称，也可以用对应的整数

这个属性(`effects_damage`)常用于饮品和药品

```c#
public enum DamageEffectType
{
  HeavyBleeding, // 0
  LightBleeding, // 1
  Fracture, // 2
  Contusion, // 3
  Intoxication, // 4
  LethalIntoxication, // 5
  RadExposure, // 6
  Pain, // 7
  DestroyedPart, // 8
}
```

#### 例子

**饮品**

```jsonc
// 物品item_food_purewater的effects_damage，可以移除辐射
"effects_damage": {
    "6": {
        "delay": 0,
        "duration": 0
    }
}
```

**药品**

```jsonc
// 物品morphine的effects_health，提供5min的止痛并移除挫伤
"effects_damage": {
    "3": {
        "delay": 0,
        "duration": 0,
        "fadeOut": 0
    },
    "7": {
        "delay": 0,
        "duration": 300,
        "fadeOut": 5
    }
}
// 物品morphine的effects_health，提供5min的止痛并移除挫伤
"effects_damage": {
    "Fracture": {
        "delay": 0,
        "duration": 0,
        "fadeOut": 0
    },
    "Pain": {
        "delay": 0,
        "duration": 300,
        "fadeOut": 5
    }
}
```

