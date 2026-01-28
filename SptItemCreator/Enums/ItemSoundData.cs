namespace SptItemCreator.Enums;

/*
 * 所有基类是Item的类型
 * "_parent": "54009119af1c881c07000029"
 *
 * 物品过滤器路径:
 * `filters`, `StackSlots._props.filters`, `Slots.*._props.filters`, `Grids.*._props.filters`
 */

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
    /// 其他声音
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

    public static readonly HashSet<string> AllowKeys = new()
    {
        "food_bottle", "food_juice_carton", "food_soda_can", 
        "food_snack", "food_tin_can", 
        "med_stimulator", "med_bandage", "med_pills", "med_medkit",
        "generic",
        "gear_armor", "mod", "item_paper", "container_metal", "container_plastic", "gear_goggles", 
        "knife_generic", "item_map", "item_money", "jewelry", "weap_ar", "spec_multitool", "gear_generic",
        "ammo_singleround", "ammo_shotgun", "ammo_launcher"
    };

    /// <summary>
    /// 根据中文键获取对应的ItemSound, 兼容正确的英文键, 如果都不正确, 返回null
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string? GetItemSoundKey(string key)
    {
        if (AllowKeys.Contains(key)) return key;
        if (key == "通用声音") return "generic";
        if (FoodSounds.TryGetValue(key, out string? soundKey) 
            || DrinkSounds.TryGetValue(key, out soundKey) 
            || MedsSounds.TryGetValue(key, out soundKey) 
            || OtherSounds.TryGetValue(key, out soundKey) 
            || AmmoSounds.TryGetValue(key, out soundKey)) return soundKey;
        return null;
    }
}