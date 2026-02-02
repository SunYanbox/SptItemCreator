## Buff

[SPTarkov.Server.Core/Models/Eft/Common/Globals.cs#Buff](https://github.com/sp-tarkov/server-csharp/blob/895d5326/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Globals.cs#L1978-L2002)

用于定义食物, 饮品, 注射器的效果

```c#
public record Buff
{
    [JsonPropertyName("BuffType")]
    public string BuffType { get; set; }

    [JsonPropertyName("Chance")]
    public double Chance { get; set; }

    [JsonPropertyName("Delay")]
    public double Delay { get; set; }

    [JsonPropertyName("Duration")]
    public double Duration { get; set; }

    [JsonPropertyName("Value")]
    public double Value { get; set; }

    [JsonPropertyName("AbsoluteValue")]
    public bool AbsoluteValue { get; set; }

    [JsonPropertyName("SkillName")]
    public string SkillName { get; set; }

    public IEnumerable<string> AppliesTo { get; set; }
}
```

Json示例:

```json
{
    "AbsoluteValue": true,
    "BuffType": "EnergyRate",
    "Chance": 1,
    "Delay": 1,
    "Duration": 900,
    "SkillName": "",
    "Value": 1
}
```

