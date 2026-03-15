using System.Text.Json;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Enums;

namespace SptItemCreator.Core.Cache;

public class PropertyList
{
    public List<int>? IntProps { get; set; }
    public List<XYZ>? XYZProps { get; set; }
    public List<bool>? BoolProps { get; set; }
    public List<Color>? ColorProps { get; set; }
    public List<double>? DoubleProps { get; set; }
    public List<object>? ObjectProps { get; set; }
    public List<Prefab>? PrefabProps { get; set; }
    public List<string>? StringProps { get; set; }
    public List<MongoId>? MongoIdProps { get; set; }
    public List<LootRarity>? LootRarities { get; set; }
    public List<ReloadMode>? ReloadModeProps { get; set; }
    public List<IEnumerable<Grid>>? GridProps { get; set; }
    public List<IEnumerable<Slot>>? SlotProps { get; set; }
    public List<JsonElement>? JsonElementProps { get; set; }
    public List<ThrowWeapType>? ThrowWeapTypeProps { get; set; }
    public List<ArmorMaterial>? ArmorMaterialProps { get; set; }
    public List<IEnumerable<StackSlot>>? StackSlotProps { get; set; }
    public List<ItemDropSoundType>? ItemDropSoundTypeProps { get; set; }
    public List<IEnumerable<string>>? EnumerableStringProps { get; set; }
    public List<Dictionary<string, object>>? DictionaryProps { get; set; }
    public List<IEnumerable<MongoId>>? EnumerableMongoIdProps { get; set; }
    public List<WeaponRecoilSettings>? WeaponRecoilSettingsProps { get; set; }
    public List<IEnumerable<PlayerSideMask>>? PlayerSideMaskProps { get; set; }
    public List<IEnumerable<EquipmentSlots>>? EquipmentSlotsProps { get; set; }
    public List<IEnumerable<List<double>>>? NestedDoubleListProps { get; set; }
    public List<IEnumerable<ShotsGroupSettings>>? ShotsGroupSettingsProps { get; set; }
    public List<IEnumerable<RepairStrategyType>>? RepairStrategyTypeProps { get; set; }
    public List<Dictionary<HealthFactor, EffectsHealthProperties>>? HealthFactorDictionaryProps { get; set; }
    public List<Dictionary<DamageEffectType, EffectsDamageProperties>>? DamageEffectTypeDictionaryProps { get; set; }
}