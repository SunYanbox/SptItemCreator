using System.Text.Json;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;
using SptItemCreator.Core.Services;

namespace SptItemCreator.Core.Cache.Extensions;

public static class PropertyListExtensions
{
    /// <summary>
    /// 根据传入对象的实际类型自动添加到合适的List中
    /// </summary>
    /// <param name="propertyList">要操作的PropertyList实例</param>
    /// <param name="value">要添加的对象（为null时忽略）</param>
    /// <returns>如果成功添加返回true，找不到匹配的列表返回false</returns>
    public static bool Add(this PropertyList propertyList, object? value)
    {
        ArgumentNullException.ThrowIfNull(propertyList);

        // 传入内容为null时忽略
        if (value == null)
            return false;

        // 根据实际类型添加到对应的列表
        return value switch
        {
            XYZ xyz => AddToXYZList(propertyList, xyz),
            Color color => AddToColorList(propertyList, color),
            int intValue => AddToIntList(propertyList, intValue),
            Prefab prefab => AddToPrefabList(propertyList, prefab),
            bool boolValue => AddToBoolList(propertyList, boolValue),
            IEnumerable<Grid> grid => AddToGridList(propertyList, grid),
            IEnumerable<Slot> slot => AddToSlotList(propertyList, slot),
            double doubleValue => AddToDoubleList(propertyList, doubleValue),
            string stringValue => AddToStringList(propertyList, stringValue),
            MongoId mongoIdValue => AddToMongoIdList(propertyList, mongoIdValue),
            LootRarity lootRarity => AddToLootRarityList(propertyList, lootRarity),
            ReloadMode reloadMode => AddToReloadModeList(propertyList, reloadMode),
            JsonElement jsonElement => AddToJsonElementList(propertyList, jsonElement),
            IEnumerable<StackSlot> stackSlot => AddToStackSlotList(propertyList, stackSlot),
            IEnumerable<MongoId> mongoId => AddToEnumerableMongoIdList(propertyList, mongoId),
            ThrowWeapType throwWeapType => AddToThrowWeapTypeList(propertyList, throwWeapType),
            ArmorMaterial armorMaterial => AddToArmorMaterialList(propertyList, armorMaterial),
            Dictionary<string, object> dictionary => AddToDictionaryList(propertyList, dictionary),
            IEnumerable<string> enumerableString => AddToEnumerableStringList(propertyList, enumerableString),
            ItemDropSoundType itemDropSoundType => AddToItemDropSoundTypeList(propertyList, itemDropSoundType),
            IEnumerable<PlayerSideMask> playerSideMask => AddToPlayerSideMaskList(propertyList, playerSideMask),
            IEnumerable<EquipmentSlots> equipmentSlots => AddToEquipmentSlotsList(propertyList, equipmentSlots),
            IEnumerable<List<double>> nestedDoubleList => AddToNestedDoubleListList(propertyList, nestedDoubleList),
            WeaponRecoilSettings weaponRecoilSettings => AddToWeaponRecoilSettingsList(propertyList, weaponRecoilSettings),
            IEnumerable<ShotsGroupSettings> shotsGroupSettings => AddToShotsGroupSettingsList(propertyList, shotsGroupSettings),
            IEnumerable<RepairStrategyType> repairStrategyType => AddToRepairStrategyTypeList(propertyList, repairStrategyType),
            Dictionary<HealthFactor, EffectsHealthProperties> healthFactorDict => AddToHealthFactorDictionaryList(propertyList, healthFactorDict),
            Dictionary<DamageEffectType, EffectsDamageProperties> damageEffectTypeDict => AddToDamageEffectTypeDictionaryList(propertyList, damageEffectTypeDict),
            // 如果没有匹配的类型，添加到ObjectProps列表
            _ => AddToObjectList(propertyList, value)
        };
    }

    /// <summary>
    /// 批量添加多个对象
    /// </summary>
    /// <param name="propertyList">要操作的PropertyList实例</param>
    /// <param name="values">要添加的对象集合</param>
    /// <returns>成功添加的数量</returns>
    public static int AddRange(this PropertyList propertyList, IEnumerable<object?> values)
    {
        ArgumentNullException.ThrowIfNull(propertyList);

        var count = 0;
        foreach (object? value in values)
        {
            if (propertyList.Add(value))
                count++;
        }
        return count;
    }

    /// <summary>
    /// 清空所有列表
    /// </summary>
    public static void ClearAll(this PropertyList propertyList)
    {
        ArgumentNullException.ThrowIfNull(propertyList);

        propertyList.MongoIdProps?.Clear();
        propertyList.StringProps?.Clear();
        propertyList.BoolProps?.Clear();
        propertyList.IntProps?.Clear();
        propertyList.LootRarities?.Clear();
        propertyList.DoubleProps?.Clear();
        propertyList.ObjectProps?.Clear();
        propertyList.EnumerableStringProps?.Clear();
        propertyList.PlayerSideMaskProps?.Clear();
        propertyList.GridProps?.Clear();
        propertyList.SlotProps?.Clear();
        propertyList.EquipmentSlotsProps?.Clear();
        propertyList.NestedDoubleListProps?.Clear();
        propertyList.ShotsGroupSettingsProps?.Clear();
        propertyList.EnumerableMongoIdProps?.Clear();
        propertyList.StackSlotProps?.Clear();
        propertyList.RepairStrategyTypeProps?.Clear();
        propertyList.PrefabProps?.Clear();
        propertyList.XYZProps?.Clear();
        propertyList.ItemDropSoundTypeProps?.Clear();
        propertyList.ThrowWeapTypeProps?.Clear();
        propertyList.ArmorMaterialProps?.Clear();
        propertyList.DictionaryProps?.Clear();
        propertyList.JsonElementProps?.Clear();
        propertyList.WeaponRecoilSettingsProps?.Clear();
        propertyList.ReloadModeProps?.Clear();
        propertyList.ColorProps?.Clear();
        propertyList.DamageEffectTypeDictionaryProps?.Clear();
        propertyList.HealthFactorDictionaryProps?.Clear();
    }

    /// <summary>
    /// 获取所有列表中元素的总数
    /// </summary>
    public static int TotalCount(this PropertyList propertyList)
    {
        ArgumentNullException.ThrowIfNull(propertyList);

        return (propertyList.MongoIdProps?.Count ?? 0) +
               (propertyList.StringProps?.Count ?? 0) +
               (propertyList.BoolProps?.Count ?? 0) +
               (propertyList.IntProps?.Count ?? 0) +
               (propertyList.LootRarities?.Count ?? 0) +
               (propertyList.DoubleProps?.Count ?? 0) +
               (propertyList.ObjectProps?.Count ?? 0) +
               (propertyList.EnumerableStringProps?.Count ?? 0) +
               (propertyList.PlayerSideMaskProps?.Count ?? 0) +
               (propertyList.GridProps?.Count ?? 0) +
               (propertyList.SlotProps?.Count ?? 0) +
               (propertyList.EquipmentSlotsProps?.Count ?? 0) +
               (propertyList.NestedDoubleListProps?.Count ?? 0) +
               (propertyList.ShotsGroupSettingsProps?.Count ?? 0) +
               (propertyList.EnumerableMongoIdProps?.Count ?? 0) +
               (propertyList.StackSlotProps?.Count ?? 0) +
               (propertyList.RepairStrategyTypeProps?.Count ?? 0) +
               (propertyList.PrefabProps?.Count ?? 0) +
               (propertyList.XYZProps?.Count ?? 0) +
               (propertyList.ItemDropSoundTypeProps?.Count ?? 0) +
               (propertyList.ThrowWeapTypeProps?.Count ?? 0) +
               (propertyList.ArmorMaterialProps?.Count ?? 0) +
               (propertyList.DictionaryProps?.Count ?? 0) +
               (propertyList.JsonElementProps?.Count ?? 0) +
               (propertyList.WeaponRecoilSettingsProps?.Count ?? 0) +
               (propertyList.ReloadModeProps?.Count ?? 0) +
               (propertyList.ColorProps?.Count ?? 0) +
               (propertyList.DamageEffectTypeDictionaryProps?.Count ?? 0) +
               (propertyList.HealthFactorDictionaryProps?.Count ?? 0);
    }

    #region 私有辅助方法
    
    private static bool AddToWeaponRecoilSettingsList(PropertyList propertyList, WeaponRecoilSettings value)
    {
        propertyList.WeaponRecoilSettingsProps ??= [];
        propertyList.WeaponRecoilSettingsProps.Add(value);
        return true;
    }

    private static bool AddToReloadModeList(PropertyList propertyList, ReloadMode value)
    {
        propertyList.ReloadModeProps ??= [];
        propertyList.ReloadModeProps.Add(value);
        return true;
    }

    private static bool AddToColorList(PropertyList propertyList, Color value)
    {
        propertyList.ColorProps ??= [];
        propertyList.ColorProps.Add(value);
        return true;
    }

    private static bool AddToDamageEffectTypeDictionaryList(PropertyList propertyList, Dictionary<DamageEffectType, EffectsDamageProperties> value)
    {
        propertyList.DamageEffectTypeDictionaryProps ??= [];
        propertyList.DamageEffectTypeDictionaryProps.Add(value);
        return true;
    }

    private static bool AddToHealthFactorDictionaryList(PropertyList propertyList, Dictionary<HealthFactor, EffectsHealthProperties> value)
    {
        propertyList.HealthFactorDictionaryProps ??= [];
        propertyList.HealthFactorDictionaryProps.Add(value);
        return true;
    }
    
    private static bool AddToItemDropSoundTypeList(PropertyList propertyList, ItemDropSoundType value)
    {
        propertyList.ItemDropSoundTypeProps ??= [];
        propertyList.ItemDropSoundTypeProps.Add(value);
        return true;
    }

    private static bool AddToThrowWeapTypeList(PropertyList propertyList, ThrowWeapType value)
    {
        propertyList.ThrowWeapTypeProps ??= [];
        propertyList.ThrowWeapTypeProps.Add(value);
        return true;
    }

    private static bool AddToArmorMaterialList(PropertyList propertyList, ArmorMaterial value)
    {
        propertyList.ArmorMaterialProps ??= [];
        propertyList.ArmorMaterialProps.Add(value);
        return true;
    }

    private static bool AddToDictionaryList(PropertyList propertyList, Dictionary<string, object> value)
    {
        propertyList.DictionaryProps ??= [];
        propertyList.DictionaryProps.Add(value);
        return true;
    }

    private static bool AddToJsonElementList(PropertyList propertyList, JsonElement value)
    {
        propertyList.JsonElementProps ??= [];
        propertyList.JsonElementProps.Add(value);
        return true;
    }

    private static bool AddToStringList(PropertyList propertyList, string value)
    {
        propertyList.StringProps ??= [];
        propertyList.StringProps.Add(value);
        return true;
    }

    private static bool AddToBoolList(PropertyList propertyList, bool value)
    {
        propertyList.BoolProps ??= [];
        propertyList.BoolProps.Add(value);
        return true;
    }

    private static bool AddToIntList(PropertyList propertyList, int value)
    {
        propertyList.IntProps ??= [];
        propertyList.IntProps.Add(value);
        return true;
    }

    private static bool AddToLootRarityList(PropertyList propertyList, LootRarity value)
    {
        propertyList.LootRarities ??= [];
        propertyList.LootRarities.Add(value);
        return true;
    }

    private static bool AddToDoubleList(PropertyList propertyList, double value)
    {
        propertyList.DoubleProps ??= [];
        propertyList.DoubleProps.Add(value);
        return true;
    }

    private static bool AddToEnumerableStringList(PropertyList propertyList, IEnumerable<string> value)
    {
        propertyList.EnumerableStringProps ??= [];
        propertyList.EnumerableStringProps.Add(value);
        return true;
    }

    private static bool AddToPlayerSideMaskList(PropertyList propertyList, IEnumerable<PlayerSideMask> value)
    {
        propertyList.PlayerSideMaskProps ??= [];
        propertyList.PlayerSideMaskProps.Add(value);
        return true;
    }

    private static bool AddToGridList(PropertyList propertyList, IEnumerable<Grid> value)
    {
        propertyList.GridProps ??= [];
        propertyList.GridProps.Add(value);
        return true;
    }

    private static bool AddToSlotList(PropertyList propertyList, IEnumerable<Slot> value)
    {
        propertyList.SlotProps ??= [];
        propertyList.SlotProps.Add(value);
        return true;
    }

    private static bool AddToEquipmentSlotsList(PropertyList propertyList, IEnumerable<EquipmentSlots> value)
    {
        propertyList.EquipmentSlotsProps ??= [];
        propertyList.EquipmentSlotsProps.Add(value);
        return true;
    }

    private static bool AddToNestedDoubleListList(PropertyList propertyList, IEnumerable<List<double>> value)
    {
        propertyList.NestedDoubleListProps ??= [];
        propertyList.NestedDoubleListProps.Add(value);
        return true;
    }

    private static bool AddToShotsGroupSettingsList(PropertyList propertyList, IEnumerable<ShotsGroupSettings> value)
    {
        propertyList.ShotsGroupSettingsProps ??= [];
        propertyList.ShotsGroupSettingsProps.Add(value);
        return true;
    }

    private static bool AddToEnumerableMongoIdList(PropertyList propertyList, IEnumerable<MongoId> value)
    {
        propertyList.EnumerableMongoIdProps ??= [];
        propertyList.EnumerableMongoIdProps.Add(value);
        return true;
    }
    
    private static bool AddToMongoIdList(PropertyList propertyList, MongoId value)
    {
        propertyList.MongoIdProps ??= [];
        propertyList.MongoIdProps.Add(value);
        return true;
    }

    private static bool AddToStackSlotList(PropertyList propertyList, IEnumerable<StackSlot> value)
    {
        propertyList.StackSlotProps ??= [];
        propertyList.StackSlotProps.Add(value);
        return true;
    }

    private static bool AddToRepairStrategyTypeList(PropertyList propertyList, IEnumerable<RepairStrategyType> value)
    {
        propertyList.RepairStrategyTypeProps ??= [];
        propertyList.RepairStrategyTypeProps.Add(value);
        return true;
    }

    private static bool AddToPrefabList(PropertyList propertyList, Prefab value)
    {
        propertyList.PrefabProps ??= [];
        propertyList.PrefabProps.Add(value);
        return true;
    }

    private static bool AddToXYZList(PropertyList propertyList, XYZ value)
    {
        propertyList.XYZProps ??= [];
        propertyList.XYZProps.Add(value);
        return true;
    }

    private static bool AddToObjectList(PropertyList propertyList, object value)
    {
        propertyList.ObjectProps ??= [];
        propertyList.ObjectProps.Add(value);
        LocalLog.Logger.Debug($"统计属性时添加了object对象: {value.GetType().Name}({value})");
        return true;
    }

    #endregion
}