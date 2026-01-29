using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Utils.Json.Converters;
using SptItemCreator.Abstracts;

namespace SptItemCreator.InfoClasses;

public sealed record MedicalInfo : AbstractInfo
{
    [JsonPropertyName("maxHpResource")]
    public int? MaxHpResource { get; set; }
    [JsonPropertyName("hpResourceRate")]
    public double? HpResourceRate { get; set; }
    [JsonPropertyName("medUseTime")]
    public double? MedUseTime { get; set; }
    [JsonPropertyName("medEffectType")]
    public string? MedEffectType { get; set; }
    [JsonPropertyName("effects_health")]
    [JsonConverter(typeof (ArrayToObjectFactoryConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Dictionary<HealthFactor, EffectsHealthProperties>? EffectsHealth { get; set; }

    [JsonPropertyName("effects_damage")]
    [JsonConverter(typeof (ArrayToObjectFactoryConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Dictionary<DamageEffectType, EffectsDamageProperties>? EffectsDamage { get; set; }
    /// <summary>E.g. "Stomach" or "RightLeg"</summary>
    [JsonPropertyName("bodyPartPriority")]
    public List<string>? BodyPartPriority { get; set; }
    
    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (MaxHpResource != null) properties.MaxHpResource = MaxHpResource;
        if (MedUseTime != null) properties.MedUseTime = MedUseTime;
        if (MedEffectType != null) properties.MedEffectType = MedEffectType;
        if (HpResourceRate != null) properties.HpResourceRate = HpResourceRate;
        if (EffectsHealth != null) properties.EffectsHealth = EffectsHealth;
        if (EffectsDamage != null) properties.EffectsDamage = EffectsDamage;
        if (BodyPartPriority != null) properties.BodyPartPriority = BodyPartPriority;
    }
}
