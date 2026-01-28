using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Abstracts;

namespace SptItemCreator.InfoClasses;


public record BuffsInfo : AbstractInfo
{
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
        Dictionary<string, IEnumerable<Buff>> buffs = databaseService.GetTables().Globals.Configuration.Health.Effects.Stimulator.Buffs;
        bool stimulatorBuffsIsNull = string.IsNullOrEmpty(StimulatorBuffs);
        if (stimulatorBuffsIsNull && Buffs is null)
        {
            return;
        }
        if (!stimulatorBuffsIsNull && Buffs != null)
        {
            if (!buffs.TryAdd(StimulatorBuffs!, Buffs))
            {
                LocalLog?.LocalLogMsg(LocalLogType.Info, $"已成功创建新效果{StimulatorBuffs}(共{Buffs.Count}条)\n\tPath={ItemPath}");
                return;
            }
        }
        if (!stimulatorBuffsIsNull && Buffs == null && !buffs.ContainsKey(StimulatorBuffs!))
        {
            string warn =
                $"检测到效果字段赋值了`stimulatorBuffs`, 但没有提供`buffs`, 并且没有已被注册的`stimulatorBuffs`({StimulatorBuffs}), 请检查你的新物品文件";
            throw new Exception(warn);
        }
    }
}
