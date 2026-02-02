## Prefab

[SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#Prefab](https://github.com/sp-tarkov/server-csharp/blob/895d5326/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Tables/TemplateItem.cs#L1648-L1655)

Model Path

```c#
public record Prefab
{
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("rcid")]
    public string? Rcid { get; set; }
}
```

For the Prefab property of SPT items, the Path is the relative path of the model with respect to **assets**, for example: 

`"assets/content/items/ammo/patrons/patron_46x30_ap_sx.bundle"`

The RcID attribute is all `""` (9355 / 9355)

