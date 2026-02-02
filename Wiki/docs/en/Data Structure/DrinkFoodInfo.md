## DrinkFoodInfo

Defines the consumption properties and restorative effects for drink and food-type items.

| Property            | In Data File      | Type   | Description & Common Values            | Common Values                                               | Optionality |
| ------------------- | ----------------- | ------ | -------------------------------------- | ------------------------------------------------------------ | ----------- |
| **FoodUseTime**     | "foodUseTime"     | double | Time required to consume (seconds)    | 2, 3, 4, 5, 6, 15                                            | Optional    |
| **Hydration**       | "hydration"       | double | Hydration restoration value           | Typically between -99 and 100                                | Optional    |
| **Energy**          | "energy"          | double | Energy restoration value              | Typically between -19 and 100                                | Optional    |
| **MaxResource**     | "maxResource"     | int    | Maximum usage count / resource amount | Drinks: commonly 40, 50, 60, 70, 100; <br />Food: usually 1; other items: usually 0 | Optional    |

## drinkDrugInfo Field

```json
// Food/Drink configuration
"drinkFoodInfo": {
  "foodUseTime": 5, // Usage time
  "hydration": 33, // Hydration change
  "energy": 33, // Energy change
  "maxResource": 20 // Maximum resource (suitable for drinks)
}
```

## Example

```json
"drinkFoodInfo": {
    "foodUseTime": 5,
    "hydration": 33,
    "energy": 33,
    "maxResource": 20
}
```

