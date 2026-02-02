## Available values for itemSound

> This property most likely represents the sound effect triggered when an item is picked up, dragged, or placed into the backpack/quick slot.
>
> You can assign either the Chinese keys or English values listed below.
>
> You can also assign `通用声音` or `generic`. The Chinese keys are only for use within this mod's fields other than propertyOverride.

```c#
public static class ItemSoundData
{
    // Drink ItemSound dictionary
    public static readonly Dictionary<string, string> DrinkSounds = new()
    {
        { "瓶子声音", "food_bottle" },
        { "果汁纸盒声音", "food_juice_carton" },
        { "苏打罐声音", "food_soda_can" }
    };

    // Food ItemSound dictionary
    public static readonly Dictionary<string, string> FoodSounds = new()
    {
        { "零食声音", "food_snack" },
        { "罐头声音", "food_tin_can" },
        { "果汁纸盒声音", "food_juice_carton" }
    };
    
    // Medical item sounds
    public static readonly Dictionary<string, string> MedsSounds = new()
    {
        { "医疗注射器", "med_stimulator" },
        { "医疗绷带", "med_bandage" },
        { "医疗药片", "med_pills" },
        { "医疗急救包", "med_medkit" },
    };
    
    /// <summary>
    /// Equipment sounds
    /// Barrel sounds
    /// Item sounds
    /// </summary>
    public static readonly Dictionary<string, string> OtherSounds = new()
    {
        { "装备护甲", "gear_armor" },
        { "改装零件", "mod" },
        { "纸张翻动声", "item_paper" },
        { "金属容器", "container_metal" },
        { "塑料容器", "container_plastic" },
        { "眼镜类装备", "gear_goggles" },
        { "刀具通用", "knife_generic" },
        { "地图声音", "item_map" },
        { "货币声音", "item_money" },
        { "珠宝首饰", "jewelry" },
        { "突击步枪", "weap_ar" },
        { "精密工具", "spec_multitool" },
        { "通用装备", "gear_generic" },
    };
    
    public static readonly Dictionary<string, string> AmmoSounds = new()
    {
        {"单发子弹音效", "ammo_singleround"},
        {"霰弹枪音效", "ammo_shotgun"},
        {"榴弹发射器音效", "ammo_launcher"}
    };
}
```