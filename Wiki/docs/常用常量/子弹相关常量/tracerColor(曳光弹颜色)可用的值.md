## tracerColor(曳光弹颜色)可用的值

> 需要像示例一样使用下方的字符串之一

- `red`
- `tracerRed`
- `yellow`
- `tracerYellow`
- `green`
- `tracerGreen`

**示例**

非曳光弹：

```json
{
    "_id": "54527a984bdc2d4e668b4567",
    "_name": "patron_556x45_M855",
    "_parent": "5485a8684bdc2da71d8b4567",
    "_type": "Item",
    "_props": {
      "Tracer": false,
      "TracerColor": "red",
      "TracerDistance": 0,
      // 省略其他属性
    }
}
```

曳光弹：

```json
{
    "_id": "573603c924597764442bd9cb",
    "_name": "patron_762x25tt_T_Gzh",
    "_parent": "5485a8684bdc2da71d8b4567",
    "_type": "Item",
    "_props": {
      "Tracer": true,
      "TracerColor": "tracerRed",
      "TracerDistance": 0.5,
      // 省略其他属性
    }
}
```

```json
{
    "_id": "5a608bf24f39f98ffc77720e",
    "_name": "patron_762x51_M62",
    "_parent": "5485a8684bdc2da71d8b4567",
    "_type": "Item",
    "_props": {
      "Tracer": true,
      "TracerColor": "green",
      "TracerDistance": 0,
      // 省略其他属性
    }
}
```

