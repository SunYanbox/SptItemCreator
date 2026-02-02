## DrinkFoodInfo 饮食物品信息类

定义饮品和食物类物品的消耗属性及恢复效果。

| 属性            | 在数据文件中的写法 | 类型   | 说明常见数值            | 常见数值                                                     | 可选性 |
| --------------- | ------------------ | ------ | ----------------------- | ------------------------------------------------------------ | ------ |
| **FoodUseTime** | "foodUseTime"      | double | 食用/饮用所需时间（秒） | 2，3，4，5，6，15                                            | 可选   |
| **Hydration**   | "hydration"        | double | 水分恢复值              | 常见数值处于-99~100之间                                      | 可选   |
| **Energy**      | "energy"           | double | 能量恢复值              | 常见数值处于-19~100之间                                      | 可选   |
| **MaxResource** | "maxResource"      | int    | 最大使用次数/资源量     | 饮品常见40，50，60，70，100；<br />食物一般为1；其他物品一般为0 | 可选   |

## drinkDrugInfo字段

```json
// 食物/饮品配置
"drinkFoodInfo": {
  "foodUseTime": 5, // 使用时间
  "hydration": 33, // 水分变化量
  "energy": 33, // 能力变化量
  "maxResource": 20 // 最大资源(适合饮品)
}
```

## 示例

```json
"drinkFoodInfo": {
    "foodUseTime": 5,
    "hydration": 33,
    "energy": 33,
    "maxResource": 20
}
```

