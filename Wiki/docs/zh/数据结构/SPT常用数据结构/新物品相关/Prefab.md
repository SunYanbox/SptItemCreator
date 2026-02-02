## Prefab

[SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#Prefab](https://github.com/sp-tarkov/server-csharp/blob/895d5326/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#L1648-L1655)

模型路径

```c#
public record Prefab
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("rcid")]
    public string? Rcid { get; set; }
}
```

对于SPT的物品的Prefab属性，Path为模型相对于**assets**的相对路径，例如：

`"assets/content/items/ammo/patrons/patron_46x30_ap_sx.bundle"`

Rcid属性均为`""` (9355 / 9355)

