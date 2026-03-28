using SPTarkov.Server.Core.Models.Eft.Common;
using SptItemCreator.Core.Services;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Validators;

public class BuffsInfoValidator : BaseValidator
{
    public override bool CanHandle(INewItem newItem) => newItem.BuffsInfo is not null;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.BuffsInfo is null) return true;

        BuffsInfo? buffs = newItem.BuffsInfo;
        bool hasStimulatorBuffs = !string.IsNullOrEmpty(buffs.StimulatorBuffs);
        bool hasBuffs = buffs.Buffs is { Count: > 0 };

        if (!hasStimulatorBuffs && !hasBuffs) return true;

        if (hasStimulatorBuffs && hasBuffs)
        {
            ValidateBuffsStructure(buffs.Buffs!, errorCollector);
            return true;
        }

        if (hasStimulatorBuffs && !hasBuffs)
        {
            errorCollector.AddError("BuffsInfo",
                $"[StimulatorBuffs] 指定了效果名'{buffs.StimulatorBuffs}'但未提供Buffs列表，需确保该效果已存在于数据库中");
        }

        if (!hasStimulatorBuffs && hasBuffs)
        {
            errorCollector.AddError("BuffsInfo",
                "[StimulatorBuffs] 提供了Buffs但未指定StimulatorBuffs名称, 将静默清除此Buff");
            newItem.BuffsInfo = null;
            LocalLog.Logger.Warn(
                $"物品 {newItem.ItemPath}: Buffs数据被丢弃, 原因: 缺少StimulatorBuffs名称。为了防止注册时抛出异常, 已将BuffsInfo设为 null");
        }

        return true;
    }

    private static void ValidateBuffsStructure(List<Buff> buffs, IErrorCollector errorCollector)
    {
        for (var i = 0; i < buffs.Count; i++)
        {
            Buff buff = buffs[i];
            if (string.IsNullOrEmpty(buff.BuffType))
                errorCollector.AddError("BuffsInfo", $"[Buffs[{i}]] BuffType为空");
        }
    }
}
