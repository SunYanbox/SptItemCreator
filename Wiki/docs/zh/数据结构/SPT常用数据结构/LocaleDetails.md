## LocaleDetails

[SPTarkov.Server.Core/Models/Spt/Mod/NewItemDetails.cs#LocaleDetails](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Spt/Mod/NewItemDetails.cs#L56C1-L66C2)

LocaleDetails包含名称，简称，描述三个属性：

```c#
public record LocaleDetails
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
```

写成Json格式为：

```json
{
    "name": "名称",
    "shortName": "简称",
    "description": "描述"
}
```

