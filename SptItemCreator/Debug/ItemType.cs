using SPTarkov.Server.Core.Models.Common;

namespace SptItemCreator.Debug;

/// <summary>
/// 创建新物品类型的字符串枚举
///
/// - 目前对应SPT4.0.8~SPT4.0.11的类型数据, 共119类型 | 2026 / 1 / 29 / 12:00
/// </summary>
internal static class ItemType
{
    public const string Ammo = "ammo"; // 弹药
    public const string AmmoBox = "ammo_box"; // 弹药箱
    public const string ArmBand = "arm_band"; // 臂章
    public const string Armor = "armor"; // 护甲
    public const string ArmorPlate = "armor_plate"; // 装甲板
    public const string ArmoredEquipment = "armored_equipment"; // 装甲装备
    public const string AssaultCarbine = "assault_carbine"; // 突击卡宾枪
    public const string AssaultRifle = "assault_rifle"; // 突击步枪
    public const string AssaultScope = "assault_scope"; // 突击瞄准镜
    public const string AuxiliaryMod = "auxiliary_mod"; // 辅助配件
    public const string Backpack = "backpack"; // 背包
    public const string Barrel = "barrel"; // 枪管
    public const string BarterItem = "barter_item"; // 交易物品
    public const string Battery = "battery"; // 电池
    public const string Bipod = "bipod"; // 两脚架
    public const string BuildingMaterial = "building_material"; // 建筑材料
    public const string BuiltInInserts = "built_in_inserts"; // 内置插板
    public const string Charge = "charge"; // 充电器
    public const string Collimator = "collimator"; // 准直镜
    public const string CompactCollimator = "compact_collimator"; // 紧凑型准直镜
    public const string Compass = "compass"; // 指南针
    public const string Compensator = "compensator"; // 补偿器
    public const string CompoundItem = "compound_item"; // 复合物品
    public const string CultistAmulet = "cultist_amulet"; // 邪教徒护身符
    public const string CylinderMagazine = "cylinder_magazine"; // 弹鼓
    public const string Drink = "drink"; // 饮料
    public const string Drugs = "drugs"; // 毒品
    public const string Electronics = "electronics"; // 电子设备
    public const string Equipment = "equipment"; // 装备
    public const string FaceCover = "face_cover"; // 面部遮挡
    public const string FlashHider = "flash_hider"; // 消焰器
    public const string Flashlight = "flashlight"; // 手电筒
    public const string Flyer = "flyer"; // 传单
    public const string Food = "food"; // 食物
    public const string FoodDrink = "food_drink"; // 食品饮料
    public const string Foregrip = "foregrip"; // 前握把
    public const string Fuel = "fuel"; // 燃料
    public const string FunctionalMod = "functional_mod"; // 功能配件
    public const string Gasblock = "gasblock"; // 导气块
    public const string GearMod = "gear_mod"; // 装备配件
    public const string GrenadeLauncher = "grenade_launcher"; // 榴弹发射器
    public const string Handguard = "handguard"; // 护木
    public const string Headphones = "headphones"; // 耳机
    public const string Headwear = "headwear"; // 头戴装备
    public const string HideoutAreaContainer = "hideout_area_container"; // 藏身处区域容器
    public const string HouseholdGoods = "household_goods"; // 家居用品
    public const string Info = "info"; // 信息物品
    public const string Inventory = "inventory"; // 库存
    public const string IronSight = "iron_sight"; // 机械瞄具
    public const string Item = "item"; // 物品
    public const string Jewelry = "jewelry"; // 珠宝
    public const string Key = "key"; // 钥匙
    public const string KeyMechanical = "key_mechanical"; // 机械钥匙
    public const string Keycard = "keycard"; // 钥匙卡
    public const string Knife = "knife"; // 刀
    public const string Launcher = "launcher"; // 发射器
    public const string LightLaser = "light_laser"; // 激光指示器
    public const string LockableContainer = "lockable_container"; // 可上锁容器
    public const string LootContainer = "loot_container"; // 战利品容器
    public const string Lubricant = "lubricant"; // 润滑剂
    public const string MachineGun = "machine_gun"; // 机枪
    public const string Magazine = "magazine"; // 弹匣
    public const string Map = "map"; // 地图
    public const string MarkOfUnknown = "mark_of_unknown"; // 未知标记
    public const string MarksmanRifle = "marksman_rifle"; // 精确射手步枪
    public const string MasterMod = "master_mod"; // 主配件
    public const string MedKit = "med_kit"; // 医疗包
    public const string Medical = "medical"; // 医疗物品
    public const string MedicalSupplies = "medical_supplies"; // 医疗用品
    public const string Meds = "meds"; // 药品
    public const string MobContainer = "mob_container"; // 移动容器
    public const string Mod = "mod"; // 配件
    public const string Money = "money"; // 货币
    public const string Mount = "mount"; // 支架
    public const string Multitools = "multitools"; // 多功能工具
    public const string Muzzle = "muzzle"; // 枪口装置
    public const string MuzzleCombo = "muzzle_combo"; // 组合枪口装置
    public const string NightVision = "night_vision"; // 夜视仪
    public const string OpticScope = "optic_scope"; // 光学瞄准镜
    public const string Other = "other"; // 其他
    public const string Pistol = "pistol"; // 手枪
    public const string PistolGrip = "pistol_grip"; // 手枪握把
    public const string PlantingKits = "planting_kits"; // 种植工具包
    public const string Pms = "pms"; // PMS
    public const string Pockets = "pockets"; // 口袋
    public const string PortableRangeFinder = "portable_range_finder"; // 便携测距仪
    public const string RadioTransmitter = "radio_transmitter"; // 无线电发射器
    public const string RailCovers = "rail_covers"; // 导轨盖
    public const string RandomLootContainer = "random_loot_container"; // 随机战利品容器
    public const string Receiver = "receiver"; // 机匣
    public const string RepairKits = "repair_kits"; // 维修工具包
    public const string Revolver = "revolver"; // 左轮手枪
    public const string Rocket = "rocket"; // 火箭弹
    public const string RocketLauncher = "rocket_launcher"; // 火箭发射器
    public const string SearchableItem = "searchable_item"; // 可搜索物品
    public const string Shaft = "shaft"; // 轴
    public const string Shotgun = "shotgun"; // 霰弹枪
    public const string Sights = "sights"; // 瞄具
    public const string Silencer = "silencer"; // 消音器
    public const string SimpleContainer = "simple_container"; // 简单容器
    public const string Smg = "smg"; // 冲锋枪
    public const string SniperRifle = "sniper_rifle"; // 狙击步枪
    public const string SortingTable = "sorting_table"; // 分类桌
    public const string SpecItem = "spec_item"; // 特殊物品
    public const string SpecialScope = "special_scope"; // 特殊瞄准镜
    public const string SpecialWeapon = "special_weapon"; // 特殊武器
    public const string SpringDrivenCylinder = "spring_driven_cylinder"; // 弹簧驱动气缸
    public const string StackableItem = "stackable_item"; // 可堆叠物品
    public const string Stash = "stash"; // 藏匿处
    public const string StationaryContainer = "stationary_container"; // 固定容器
    public const string Stimulator = "stimulator"; // 兴奋剂
    public const string Stock = "stock"; // 枪托
    public const string TacticalCombo = "tactical_combo"; // 战术组合
    public const string ThermalVision = "thermal_vision"; // 热成像
    public const string ThrowWeap = "throw_weap"; // 投掷武器
    public const string Tool = "tool"; // 工具
    public const string Vest = "vest"; // 胸挂
    public const string Visors = "visors"; // 面罩
    public const string Weapon = "weapon"; // 武器

    /// <summary>
    /// 类型与MongoId的映射字典
    /// </summary>
    public static readonly Dictionary<string, MongoId> TypesDict = new()
    {
        { Ammo, BaseClasses.AMMO },
        { AmmoBox, BaseClasses.AMMO_BOX },
        { ArmBand, BaseClasses.ARM_BAND },
        { Armor, BaseClasses.ARMOR },
        { ArmorPlate, BaseClasses.ARMOR_PLATE },
        { ArmoredEquipment, BaseClasses.ARMORED_EQUIPMENT },
        { AssaultCarbine, BaseClasses.ASSAULT_CARBINE },
        { AssaultRifle, BaseClasses.ASSAULT_RIFLE },
        { AssaultScope, BaseClasses.ASSAULT_SCOPE },
        { AuxiliaryMod, BaseClasses.AUXILIARY_MOD },
        { Backpack, BaseClasses.BACKPACK },
        { Barrel, BaseClasses.BARREL },
        { BarterItem, BaseClasses.BARTER_ITEM },
        { Battery, BaseClasses.BATTERY },
        { Bipod, BaseClasses.BIPOD },
        { BuildingMaterial, BaseClasses.BUILDING_MATERIAL },
        { BuiltInInserts, BaseClasses.BUILT_IN_INSERTS },
        { Charge, BaseClasses.CHARGE },
        { Collimator, BaseClasses.COLLIMATOR },
        { CompactCollimator, BaseClasses.COMPACT_COLLIMATOR },
        { Compass, BaseClasses.COMPASS },
        { Compensator, BaseClasses.COMPENSATOR },
        { CompoundItem, BaseClasses.COMPOUND_ITEM },
        { CultistAmulet, BaseClasses.CULTIST_AMULET },
        { CylinderMagazine, BaseClasses.CYLINDER_MAGAZINE },
        { Drink, BaseClasses.DRINK },
        { Drugs, BaseClasses.DRUGS },
        { Electronics, BaseClasses.ELECTRONICS },
        { Equipment, BaseClasses.EQUIPMENT },
        { FaceCover, BaseClasses.FACE_COVER },
        { FlashHider, BaseClasses.FLASH_HIDER },
        { Flashlight, BaseClasses.FLASHLIGHT },
        { Flyer, BaseClasses.FLYER },
        { Food, BaseClasses.FOOD },
        { FoodDrink, BaseClasses.FOOD_DRINK },
        { Foregrip, BaseClasses.FOREGRIP },
        { Fuel, BaseClasses.FUEL },
        { FunctionalMod, BaseClasses.FUNCTIONAL_MOD },
        { Gasblock, BaseClasses.GASBLOCK },
        { GearMod, BaseClasses.GEAR_MOD },
        { GrenadeLauncher, BaseClasses.GRENADE_LAUNCHER },
        { Handguard, BaseClasses.HANDGUARD },
        { Headphones, BaseClasses.HEADPHONES },
        { Headwear, BaseClasses.HEADWEAR },
        { HideoutAreaContainer, BaseClasses.HIDEOUT_AREA_CONTAINER },
        { HouseholdGoods, BaseClasses.HOUSEHOLD_GOODS },
        { Info, BaseClasses.INFO },
        { Inventory, BaseClasses.INVENTORY },
        { IronSight, BaseClasses.IRON_SIGHT },
        { Item, BaseClasses.ITEM },
        { Jewelry, BaseClasses.JEWELRY },
        { Key, BaseClasses.KEY },
        { KeyMechanical, BaseClasses.KEY_MECHANICAL },
        { Keycard, BaseClasses.KEYCARD },
        { Knife, BaseClasses.KNIFE },
        { Launcher, BaseClasses.LAUNCHER },
        { LightLaser, BaseClasses.LIGHT_LASER },
        { LockableContainer, BaseClasses.LOCKABLE_CONTAINER },
        { LootContainer, BaseClasses.LOOT_CONTAINER },
        { Lubricant, BaseClasses.LUBRICANT },
        { MachineGun, BaseClasses.MACHINE_GUN },
        { Magazine, BaseClasses.MAGAZINE },
        { Map, BaseClasses.MAP },
        { MarkOfUnknown, BaseClasses.MARK_OF_UNKNOWN },
        { MarksmanRifle, BaseClasses.MARKSMAN_RIFLE },
        { MasterMod, BaseClasses.MASTER_MOD },
        { MedKit, BaseClasses.MED_KIT },
        { Medical, BaseClasses.MEDICAL },
        { MedicalSupplies, BaseClasses.MEDICAL_SUPPLIES },
        { Meds, BaseClasses.MEDS },
        { MobContainer, BaseClasses.MOB_CONTAINER },
        { Mod, BaseClasses.MOD },
        { Money, BaseClasses.MONEY },
        { Mount, BaseClasses.MOUNT },
        { Multitools, BaseClasses.MULTITOOLS },
        { Muzzle, BaseClasses.MUZZLE },
        { MuzzleCombo, BaseClasses.MUZZLE_COMBO },
        { NightVision, BaseClasses.NIGHT_VISION },
        { OpticScope, BaseClasses.OPTIC_SCOPE },
        { Other, BaseClasses.OTHER },
        { Pistol, BaseClasses.PISTOL },
        { PistolGrip, BaseClasses.PISTOL_GRIP },
        { PlantingKits, BaseClasses.PLANTING_KITS },
        { Pms, BaseClasses.PMS },
        { Pockets, BaseClasses.POCKETS },
        { PortableRangeFinder, BaseClasses.PORTABLE_RANGE_FINDER },
        { RadioTransmitter, BaseClasses.RADIO_TRANSMITTER },
        { RailCovers, BaseClasses.RAIL_COVERS },
        { RandomLootContainer, BaseClasses.RANDOM_LOOT_CONTAINER },
        { Receiver, BaseClasses.RECEIVER },
        { RepairKits, BaseClasses.REPAIR_KITS },
        { Revolver, BaseClasses.REVOLVER },
        { Rocket, BaseClasses.ROCKET },
        { RocketLauncher, BaseClasses.ROCKET_LAUNCHER },
        { SearchableItem, BaseClasses.SEARCHABLE_ITEM },
        { Shaft, BaseClasses.SHAFT },
        { Shotgun, BaseClasses.SHOTGUN },
        { Sights, BaseClasses.SIGHTS },
        { Silencer, BaseClasses.SILENCER },
        { SimpleContainer, BaseClasses.SIMPLE_CONTAINER },
        { Smg, BaseClasses.SMG },
        { SniperRifle, BaseClasses.SNIPER_RIFLE },
        { SortingTable, BaseClasses.SORTING_TABLE },
        { SpecItem, BaseClasses.SPEC_ITEM },
        { SpecialScope, BaseClasses.SPECIAL_SCOPE },
        { SpecialWeapon, BaseClasses.SPECIAL_WEAPON },
        { SpringDrivenCylinder, BaseClasses.SPRING_DRIVEN_CYLINDER },
        { StackableItem, BaseClasses.STACKABLE_ITEM },
        { Stash, BaseClasses.STASH },
        { StationaryContainer, BaseClasses.STATIONARY_CONTAINER },
        { Stimulator, BaseClasses.STIMULATOR },
        { Stock, BaseClasses.STOCK },
        { TacticalCombo, BaseClasses.TACTICAL_COMBO },
        { ThermalVision, BaseClasses.THERMAL_VISION },
        { ThrowWeap, BaseClasses.THROW_WEAP },
        { Tool, BaseClasses.TOOL },
        { Vest, BaseClasses.VEST },
        { Visors, BaseClasses.VISORS },
        { Weapon, BaseClasses.WEAPON }
    };

    /// <summary>
    /// 根据类型字符串获取对应的MongoId
    /// </summary>
    /// <param name="type">类型字符串</param>
    /// <returns>对应的MongoId，如果未找到则返回基础物品类型</returns>
    public static MongoId GetMongoId(string type)
    {
        return TypesDict.TryGetValue(type, out MongoId mongoId) ? mongoId : BaseClasses.ITEM;
    }

    /// <summary>
    /// 检查类型字符串是否有效
    /// </summary>
    /// <param name="type">类型字符串</param>
    /// <returns>是否有效</returns>
    public static bool IsValidType(string type)
    {
        return TypesDict.ContainsKey(type);
    }

    /// <summary>
    /// 获取所有支持的类型列表
    /// </summary>
    /// <returns>类型字符串列表</returns>
    public static List<string> GetAllTypes()
    {
        return TypesDict.Keys.ToList();
    }
}