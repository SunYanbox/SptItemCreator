using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Enums;
using SptItemCreator.InfoClasses;

namespace SptItemCreator.NewItemClasses;

public class NewItemAmmo : NewItemCommon
{
    [JsonPropertyName("ammoInfo")]
    public AmmoInfo? AmmoInfo { get; set; }
    
    protected override bool DoCustomValidation()
    {
        Enable ??= Default.NewItemEnable;
        // 弹药特殊验证逻辑
        if (AmmoInfo is not { AmmoType: "buckshot", BuckshotBullets: <= 0 }) return true;
        LocalLog?.LocalLogMsg(LocalLogType.Error, $"弹药{ItemPath}作为霰弹类型弹药必须设置弹丸数量");
        return false;
    }

    protected override void DoCustomParameterValidation(Dictionary<string, string> oldResults)
    {
        base.DoCustomParameterValidation(oldResults);
        AmmoInfo ??= new AmmoInfo();
    }

    protected override void DoPropertyApplication(TemplateItemProperties props, DatabaseService? databaseService = null)
    {
        base.DoPropertyApplication(props, databaseService);
        AmmoInfo?.Update(props, databaseService);
    }
}