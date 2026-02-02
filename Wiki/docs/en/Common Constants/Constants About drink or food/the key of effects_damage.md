## the key of effects_damage

> You can use either the previous names or the corresponding integers

This attribute (`effects_damage`) is commonly used for drinks and medicines.

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

## Example

**Drinks**

```jsonc
// The effects_damage of the item item_food_purewater, which can remove radiation
"effects_damage": {
    "6": {
        "delay": 0,
        "duration": 0
    }
}
```

**Medicines**

```jsonc
// The effects_damage of the item morphine, providing 5 minutes of pain relief and removing contusions
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
// The effects_damage of the item morphine, providing 5 minutes of pain relief and removing contusions
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

