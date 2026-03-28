using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SptItemCreator.Core.Enums;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Validators;

public class AttributeInfoValidator : BaseValidator
{
    private static readonly HashSet<string> ValidRarities 
        = [..ItemRarityData.AllowKeys, ..ItemRarityData.AllowCnKeys];
    private static readonly HashSet<string> ValidItemSounds 
        = [..ItemSoundData.AllowKeys, ..ItemSoundData.AllowCnKeys];

    public override bool CanHandle(INewItem newItem) => newItem.AttributeInfo is not null;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.AttributeInfo is null) return true;
        ValidateFieldFormats(newItem, errorCollector);
        return true;
    }

    private static void ValidateFieldFormats(INewItem newItem, IErrorCollector errorCollector)
    {
        AttributeInfo attr = newItem.AttributeInfo!;

        if (attr.StackMaxSize < 1)
            errorCollector.AddError("AttributeInfo", $"[StackMaxSize] 必须至少为1: {attr.StackMaxSize}");

        if (attr.RarityPvE is not null && !ValidRarities.Contains(attr.RarityPvE))
            errorCollector.AddError("AttributeInfo", $"[RarityPvE] 无效的稀有度: {attr.RarityPvE}");

        if (attr.ItemSound is not null && !ValidItemSounds.Contains(attr.ItemSound))
            errorCollector.AddError("AttributeInfo", $"[ItemSound] 无效的声音类型: {attr.ItemSound}");

        if (attr.LootExperience < 0)
            errorCollector.AddError("AttributeInfo", $"[LootExperience] 经验值不能为负: {attr.LootExperience}");
        if (attr.ExamineExperience < 0)
            errorCollector.AddError("AttributeInfo", $"[ExamineExperience] 经验值不能为负: {attr.ExamineExperience}");
    }
}
