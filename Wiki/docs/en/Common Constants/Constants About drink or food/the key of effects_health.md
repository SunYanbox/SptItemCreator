## effects_health的键

> You can use either the previous names or the corresponding integers

This attribute (`effects_health`) is commonly used for drinks and foods.

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

## Example

**Drinks**

```jsonc
// The effects_health of the item juice_army, restoring 8 energy and 30 hydration
"effects_health": {
    "3": {
        "value": 8
    },
    "2": {
        "value": 30
    }
}
```

**Foods**

```jsonc
// The effects_health of the item Crackers, deducting 5 hydration and restoring 10 energy
"effects_health": {
    "3": {
        "value": 10
    },
    "2": {
        "value": -5
    }
}
// The effects_health of the item Crackers, deducting 5 hydration and restoring 10 energy
"effects_health": {
    "Energy": {
        "value": 10
    },
    "Hydration": {
        "value": -5
    }
}
```

