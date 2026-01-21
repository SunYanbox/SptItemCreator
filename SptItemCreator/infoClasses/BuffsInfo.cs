using System.Text.Json.Serialization;
using SptItemCreator.abstracts;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace SptItemCreator.infoClasses;


public record BuffsInfo : AbstractInfo
{
    [JsonIgnore] public new static bool ShouldUpdateDatabaseService => true;
    
    [JsonPropertyName("stimulatorBuffs")]
    public string? StimulatorBuffs { get; set; }
    [JsonPropertyName("buffs")]
    public List<Buff>? Buffs {get; set;}
    
    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (!string.IsNullOrEmpty(StimulatorBuffs))
            properties.StimulatorBuffs = StimulatorBuffs;
    }
    
    public override void UpdateDatabaseService(DatabaseService databaseService)
    {
        var buffs = databaseService.GetTables().Globals.Configuration.Health.Effects.Stimulator.Buffs;
        if (StimulatorBuffs != null && Buffs != null)
        {
            if (buffs.ContainsKey(StimulatorBuffs)) return;
            buffs[StimulatorBuffs] = Buffs;
        }
        if (StimulatorBuffs != null && Buffs == null && !buffs.ContainsKey(StimulatorBuffs)) LocalLog?.LocalLogMsg(
            LocalLogType.Warn, $"检测到效果字段赋值了`stimulatorBuffs`, 但没有提供`buffs`, 并且没有已被注册的`stimulatorBuffs`({StimulatorBuffs}), 请检查你的新物品文件");
    }
}
