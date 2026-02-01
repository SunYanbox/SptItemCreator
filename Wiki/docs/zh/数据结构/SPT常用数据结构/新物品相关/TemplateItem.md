## TemplateItem

[SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#TemplateItem](https://github.com/sp-tarkov/server-csharp/blob/895d53262e32e2c6a7116048c1c85bfa7770a858/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#L9C1-L69C2)

> 很多与物品相关的类型都被移动到这个文件里了，过去(比如SPT4.0.8)还是分开的

[items.json](../SPTData/items.json.md)中的物品都是这个数据结构，例如：

```json
"676a9da81888885b4e008c51": {
    "_id": "676a9da81888885b4e008c51",
    "_name": "item_container_event_twitch_winter_drops_2025_rare_day0",
    "_parent": "62f109593b54472778797866",
    "_props": {
      "AnimationVariantsNumber": 0,
      "BackgroundColor": "blue",
      ......
    },
    "_proto": "578f87a3245977356274f2cb",
    "_type": "Item"
  }
```

其中**`"_props"`**为[TemplateItemProperties](.\TemplateItemProperties.md)类型

