## LocaleDetails

[SPTarkov.Server.Core/Models/Spt/Mod/NewItemDetails.cs#LocaleDetails](https://github.com/sp-tarkov/server-csharp/blob/main/Libraries/SPTarkov.Server.Core/Models/Spt/Mod/NewItemDetails.cs#L56C1-L66C2)

LocaleDetails contains three attributes: name, abbreviation, and description: 

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

Write in JSON format as: 

```json
{
    "name": "name",
    "shortName": "short name",
    "description": "description"
}
```

