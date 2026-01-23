using System.Text.Json.Serialization;
using SptItemCreator.abstracts;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace SptItemCreator.infoClasses;


public record DrinkFoodInfo: AbstractInfo
{
    [JsonPropertyName("foodUseTime")]
    public double? FoodUseTime { get; set; }
    [JsonPropertyName("hydration")]
    public double? Hydration { get; set; }
    [JsonPropertyName("energy")]
    public double? Energy { get; set; }
    [JsonPropertyName("maxResource")]
    public int? MaxResource { get; set; }
    

    public Dictionary<HealthFactor, EffectsHealthProperties> GetEffectsHealth()
    {
        return new Dictionary<HealthFactor, EffectsHealthProperties>
        {
            { HealthFactor.Energy, new EffectsHealthProperties { Value = Energy } },
            { HealthFactor.Hydration, new EffectsHealthProperties { Value = Hydration } },
        };
    }

    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (MaxResource is >= 1) properties.MaxResource = MaxResource;
        if (FoodUseTime is >= 0) properties.FoodUseTime = FoodUseTime;
        if (Hydration != null || Energy != null)
            properties.EffectsHealth = GetEffectsHealth();
    }
}
