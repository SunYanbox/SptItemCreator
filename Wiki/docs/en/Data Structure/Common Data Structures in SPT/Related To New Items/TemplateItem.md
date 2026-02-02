## TemplateItem

[SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#TemplateItem](https://github.com/sp-tarkov/server-csharp/blob/895d53262e32e2c6a7116048c1c85bfa7770a858/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#L9C1-L69C2)

> Many item-related types have been moved into this file; in the past (for example, SPT 4.0.8), they were still separate.

The items in [items.json](../SPTData/items.json.md) all use this data structure, for example:

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

Where **`"_props"`** is of type [TemplateItemProperties](.\TemplateItemProperties.md)

