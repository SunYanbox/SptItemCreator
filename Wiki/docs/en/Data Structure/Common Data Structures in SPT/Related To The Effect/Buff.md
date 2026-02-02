## Buff

[SPTarkov.Server.Core/Models/Eft/Common/Globals.cs#Buff](https://github.com/sp-tarkov/server-csharp/blob/895d5326/Libraries/SPTarkov.Server.Core/Models/Eft/Common/Globals.cs#L1978-L2002)

Used to define the effects of food, drinks, and syringes

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

JSON Example:

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

