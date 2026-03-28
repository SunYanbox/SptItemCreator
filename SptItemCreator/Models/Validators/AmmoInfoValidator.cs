using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Validators;

public class AmmoInfoValidator : BaseValidator
{
    public override bool CanHandle(INewItem newItem) => newItem.AmmoInfo is not null;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.AmmoInfo is null) return true;
        ValidateFieldFormats(newItem, errorCollector);
        return true;
    }

    private static void ValidateFieldFormats(INewItem newItem, IErrorCollector errorCollector)
    {
        AmmoInfo ammo = newItem.AmmoInfo!;

        if (ammo.InitialSpeed is < 0)
            errorCollector.AddError("AmmoInfo", $"[InitialSpeed] 初速不应为负: {ammo.InitialSpeed}");

        if (ammo.BulletMassGram is < 0)
            errorCollector.AddError("AmmoInfo", $"[BulletMassGram] 弹头质量不应为负: {ammo.BulletMassGram}");

        if (ammo.ArmorDamage is < 0)
            errorCollector.AddError("AmmoInfo", $"[ArmorDamage] 护甲伤害不应为负: {ammo.ArmorDamage}");

        if (ammo.FragmentationChance is < 0 or > 1)
            errorCollector.AddError("AmmoInfo", $"[FragmentationChance] 碎片化概率应在0-1之间: {ammo.FragmentationChance}");

        if (ammo.RicochetChance is < 0 or > 1)
            errorCollector.AddError("AmmoInfo", $"[RicochetChance] 跳弹概率应在0-1之间: {ammo.RicochetChance}");

        if (ammo.Tracer == true && string.IsNullOrEmpty(ammo.TracerColor))
            errorCollector.AddError("AmmoInfo", "[TracerColor] 曳光弹必须指定曳光颜色");

        if (ammo.StaminaBurnPerDamage is < 0)
            errorCollector.AddError("AmmoInfo", $"[StaminaBurnPerDamage] 耐力消耗不应为负: {ammo.StaminaBurnPerDamage}");

        if (ammo.AmmoAccr is < 0)
            errorCollector.AddError("AmmoInfo", $"[AmmoAccr] 弹道准确率不应为负: {ammo.AmmoAccr}");

        if (ammo.AmmoRec is < 0)
            errorCollector.AddError("AmmoInfo", $"[AmmoRec] 弹道后坐力不应为负: {ammo.AmmoRec}");

        if (ammo.AmmoDist is < 0)
            errorCollector.AddError("AmmoInfo", $"[AmmoDist] 弹道距离不应为负: {ammo.AmmoDist}");
        
        if (ammo is { AmmoType: "buckshot", BuckshotBullets: <= 1 })
            errorCollector.AddError("AmmoInfo", "[AmmoType] 霰弹类型应有多于1枚的弹丸");

        if (ammo is { AmmoType: "bullet", BuckshotBullets: > 1 })
            errorCollector.AddError("AmmoInfo", "[AmmoType] 普通弹类型不应有多枚弹丸");
    }
}
