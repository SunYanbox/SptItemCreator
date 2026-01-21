using System.Text.Json.Serialization;
using SptItemCreator.abstracts;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace SptItemCreator.infoClasses;

public record AttributeInfo: AbstractInfo
{
    [JsonIgnore] public new static bool ShouldUpdateDatabaseService => false;
    [JsonPropertyName("weight")]
    public double? Weight { get; set; }
    [JsonPropertyName("width")]
    public int? Width { get; set; }
    [JsonPropertyName("height")]
    public int? Height { get; set; }
    [JsonPropertyName("rarityPvE")]
    public string? RarityPvE { get; set; }
    [JsonPropertyName("discardLimit")]
    public double? DiscardLimit { get; set; }
    /// <summary>
    /// 默认为"generic"
    /// </summary>
    [JsonPropertyName("itemSound")]
    public string? ItemSound { get; set; }
    // 检视与经验相关
    [JsonPropertyName("stackMaxSize")]
    public int? StackMaxSize { get; set; } = 1;
    
    [JsonPropertyName("examinedByDefault")]
    public bool? ExaminedByDefault { get; set; } = true;
    
    [JsonPropertyName("examineTime")]
    public double? ExamineTime { get; set; }
    
    [JsonPropertyName("lootExperience")]
    public int? LootExperience { get; set; }
    
    [JsonPropertyName("examineExperience")]
    public int? ExamineExperience { get; set; }

    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (Weight != null && Weight >= 0) properties.Weight = Weight;
        if (Width != null && Width >= 0) properties.Width = Width;
        if (Height != null && Height >= 0) properties.Height = Height;
        if (DiscardLimit != null) properties.DiscardLimit = DiscardLimit;
        if (ExamineTime != null) properties.ExamineTime = ExamineTime;
        if (LootExperience != null) properties.LootExperience = LootExperience;
        if (ExamineExperience != null) properties.ExamineExperience = ExamineExperience;
        if (StackMaxSize != null) properties.StackMaxSize = StackMaxSize;
        if (ExaminedByDefault != null) properties.ExaminedByDefault = ExaminedByDefault;
        if (RarityPvE != null)
        {
            string? rarity = ItemRarityData.GetRarityKey(RarityPvE);
            if (!string.IsNullOrEmpty(rarity)) properties.RarityPvE = rarity;
        }
        if (ItemSound != null)
        {
            string? itemSound = ItemSoundData.GetItemSoundKey(ItemSound);
            if (!string.IsNullOrEmpty(itemSound)) properties.ItemSound = itemSound;
        }
    }
}