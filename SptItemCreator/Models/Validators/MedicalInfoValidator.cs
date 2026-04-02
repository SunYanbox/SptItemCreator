using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Validators;

public class MedicalInfoValidator : BaseValidator
{
    public override bool CanHandle(INewItem newItem) => newItem.MedicalInfo is not null;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.MedicalInfo is null) return true;
        ValidateFieldFormats(newItem, errorCollector);
        return true;
    }

    private static void ValidateFieldFormats(INewItem newItem, IErrorCollector errorCollector)
    {
        MedicalInfo? med = newItem.MedicalInfo;

        if (med?.MedUseTime is <= 0)
            errorCollector.AddError("MedicalInfo", $"[MedUseTime] 使用时间必须为正数: {med.MedUseTime}");

        if (med?.HpResourceRate < 0)
            errorCollector.AddError("MedicalInfo", $"[HpResourceRate] 治疗速率不能为负: {med.HpResourceRate}");

        if (med?.EffectsHealth is not null)
        {
            foreach (KeyValuePair<HealthFactor, EffectsHealthProperties> kvp in med.EffectsHealth)
            {
                if (kvp.Value.Value is null or 0)
                    errorCollector.AddError("MedicalInfo",
                        $"[EffectsHealth] {kvp.Key} 的治疗值可能无效: {kvp.Value.Value}");
            }
        }

        if (med?.EffectsDamage is not null)
        {
            foreach (KeyValuePair<DamageEffectType, EffectsDamageProperties> kvp in med.EffectsDamage)
            {
                if (kvp.Value.Value == null)
                    errorCollector.AddError("MedicalInfo",
                        $"[EffectsHealth] {kvp.Key} 的值缺失");
                else if (kvp.Value.Value == 0)
                    errorCollector.AddError("MedicalInfo",
                        $"[EffectsHealth] {kvp.Key} 的治疗值为0可能不是预期行为: {kvp.Value.Value}");

            }
        }
    }
}
