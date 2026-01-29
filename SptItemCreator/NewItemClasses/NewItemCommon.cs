using System.Text.Json.Serialization;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Abstracts;
using SptItemCreator.Enums;
using SptItemCreator.InfoClasses;

namespace SptItemCreator.NewItemClasses;

[Injectable]
public class NewItemCommon: AbstractNewItem
{
    [JsonPropertyName("attributeInfo")]
    public AttributeInfo? AttributeInfo { get; set; }
    [JsonPropertyName("buffsInfo")]
    public BuffsInfo? BuffsInfo { get; set; }
    
    protected override bool DoCustomValidation()
    {
        Enable ??= Default.NewItemEnable;
        return true;
    }

    protected override void DoCustomParameterValidation(Dictionary<string, string> oldResults)
    {
        // 若提供了BuffsInfo但没有提供StimulatorBuffs，则清理掉已有的Buffs数据
        if (BuffsInfo is not { StimulatorBuffs: null }) return;
        BuffsInfo.StimulatorBuffs = "";
        BuffsInfo.Buffs = null;
    }

    protected override void DoPropertyApplication(TemplateItemProperties props, DatabaseService? databaseService = null)
    {
        AttributeInfo?.Update(props, databaseService);
        BuffsInfo?.Update(props, databaseService);
    }
}