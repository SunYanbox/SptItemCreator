## itemSound 可以赋值的值

> 赋值为下面出现的中文键或英文值均可
>
> 也可以赋值为`通用声音`或`generic`，中文键仅限本模组除了使用propertyOverride以外的字段使用

```c#
public static class ItemSoundData
{
    // 饮品ItemSound字典
    public static readonly Dictionary<string, string> DrinkSounds = new()
    {
        { "瓶子声音", "food_bottle" },
        { "果汁纸盒声音", "food_juice_carton" },
        { "苏打罐声音", "food_soda_can" }
    };

    // 食物ItemSound字典
    public static readonly Dictionary<string, string> FoodSounds = new()
    {
        { "零食声音", "food_snack" },
        { "罐头声音", "food_tin_can" },
        { "果汁纸盒声音", "food_juice_carton" }
    };
    
    // 药品声音
    public static readonly Dictionary<string, string> MedsSounds = new()
    {
        { "医疗注射器", "med_stimulator" },
        { "医疗绷带", "med_bandage" },
        { "医疗药片", "med_pills" },
        { "医疗急救包", "med_medkit" },
    };
    
    /// <summary>
    /// 装备声音
    /// 枪管声音
    /// 物品声音
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