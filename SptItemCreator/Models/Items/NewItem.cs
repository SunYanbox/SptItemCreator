using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Core.Enums;
using SptItemCreator.Core.Services;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.Abstracts.Extensions;
using SptItemCreator.Models.InfoData;
using SptItemCreator.Models.Validators;

namespace SptItemCreator.Models.Items;

/// <summary>
/// 统一新物品类 - 所有物品类型共用此类
/// 通过 ActualType 属性区分不同物品类型的验证和属性应用逻辑
/// </summary>
[UsedImplicitly]
public sealed class NewItem: INewItem
{
    private static readonly IValidator Validators = IValidator.Build(
            new BaseInfoValidator(),
            new AttributeInfoValidator(),
            new BuffsInfoValidator(),
            new DrinkFoodInfoValidator(),
            new MedicalInfoValidator(),
            new AmmoInfoValidator()
        );
    
    #region 属性

    [JsonPropertyName("enable")]
    public bool? Enable { get; set; }
    [JsonPropertyName("baseInfo")]
    public BaseInfo? BaseInfo { get; set; }
    [JsonPropertyName("propertyOverride")]
    public TemplateItemProperties? PropertyOverride { get; set; }
    [JsonPropertyName("attributeInfo")]
    public AttributeInfo? AttributeInfo { get; set; }
    [JsonPropertyName("buffsInfo")]
    public BuffsInfo? BuffsInfo { get; set; }
    [JsonPropertyName("drinkFoodInfo")]
    public DrinkFoodInfo? DrinkFoodInfo { get; set; }
    [JsonPropertyName("medicalInfo")]
    public MedicalInfo? MedicalInfo { get; set; }
    [JsonPropertyName("ammoInfo")]
    public AmmoInfo? AmmoInfo { get; set; }
    [JsonIgnore] public string ItemPath { get; set; } = string.Empty;
    [JsonIgnore] public static DatabaseService? DatabaseService;

    [JsonIgnore]
    public AbstractInfo[] NeedValidator => new AbstractInfo?[]
        {
            BaseInfo,
            AttributeInfo, 
            BuffsInfo, 
            MedicalInfo, 
            DrinkFoodInfo, 
            AmmoInfo
        }
        .Where(info => info is not null).Cast<AbstractInfo>().ToArray();
    
    #endregion

    #region 创建物品方法

    public (bool verify, IErrorCollector errors) Verify()
    {
        Enable ??= Default.NewItemEnable;
        IErrorCollector errorCollector = new ErrorCollector(this);
        bool result = IValidator.ValidateAll(Validators, this, errorCollector);
        return (result, errorCollector);
    }

    private void WarnIfHandbookParentIdIsNull(IErrorCollector errorCollector)
    {
        if (BaseInfo?.HandbookParentId != null) return;
        var errorMessage = $"没有为物品{this.ToIdNameString()}设置HandbookParentId, 会影响在跳蚤市场与手册中的分类";
        errorCollector.AddError("CreateNewItem", errorMessage);
        var sptErrorMessage = $"[SptItemCreator] {errorMessage}";
        if (LocalLog.SptLogger is not null)
        {
            LocalLog.SptLogger.Warning(sptErrorMessage);
        }
        else
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{sptErrorMessage}");
            Console.ForegroundColor = originalColor;
        }
    }

    /// <summary>
    /// 根据物品数据返回创建新物品的NewItemDetails
    /// 返回null表示数据无效等意外情况
    /// </summary>
    /// <returns></returns>
    public NewItemDetails? CreateNewItem()
    {
        (bool verify, IErrorCollector errors) = Verify();
        if (!verify || BaseInfo?.Id == null || BaseInfo?.ParentId == null || BaseInfo?.CloneId != null)
        {
            if (BaseInfo?.CloneId != null) errors.AddError("CreateNewItem", "在不通过克隆物品创建物品时 baseInfo.cloneId 意外被赋值");
            if (!errors.IsEmpty())
            {
                LocalLog.Logger.Error(errors.ErrorsToString());
            }
            return null;
        }
        WarnIfHandbookParentIdIsNull(errors);
        if (BaseInfo == null) return null;
        PropertyApplyAll();
        if (!errors.IsEmpty())
        {
            LocalLog.Logger.Warn($"创建物品{this.ToIdNameString()}时出现问题: {errors.ErrorsToString()}");
        }
        return new NewItemDetails
        {
            NewItem = new TemplateItem
            {
                Id = BaseInfo.Id,
                Name = BaseInfo.Name,
                Parent = BaseInfo.ParentId,
                Properties = PropertyOverride,
                Prototype = BaseInfo.CloneId,
                Type = "Item"
            },
            FleaPriceRoubles = BaseInfo.FleaPrice,
            HandbookPriceRoubles = BaseInfo.HandbookPrice,
            HandbookParentId = BaseInfo.HandbookParentId,
            Locales = BaseInfo.Locales ?? new Dictionary<string, LocaleDetails>
            {
                {
                    "ch",
                    new LocaleDetails
                    {
                        Name = BaseInfo.Name,
                        ShortName = BaseInfo.Name,
                        Description = BaseInfo.Description
                    }
                },
                {
                    "en",
                    new LocaleDetails
                    {
                        Name = BaseInfo.Name,
                        ShortName = BaseInfo.Name,
                        Description = BaseInfo.Description
                    }
                }
            }
        };
    }

    /// <summary>
    /// 根据物品数据返回创建新物品的NewItemFromCloneDetails
    /// 返回null表示数据无效等意外情况
    /// </summary>
    /// <returns></returns>
    public NewItemFromCloneDetails? CreateItemFromClone()
    {
        (bool verify, IErrorCollector errors) = Verify();
        if (!verify || BaseInfo?.Id == null || BaseInfo?.ParentId == null || BaseInfo?.CloneId == null)
        {
            if (BaseInfo?.CloneId == null) errors.AddError("CreateItemFromClone", "在通过克隆物品创建物品时 baseInfo.cloneId 没有正确赋值");
            if (!errors.IsEmpty())
            {
                LocalLog.Logger.Error(errors.ErrorsToString());
            }
            return null;
        }
        WarnIfHandbookParentIdIsNull(errors);
        if (BaseInfo == null) return null;
        PropertyApplyAll();
        if (!errors.IsEmpty())
        {
            LocalLog.Logger.Warn($"克隆模式创建物品{this.ToIdNameString()}时出现问题: {errors.ErrorsToString()}");
        }
        return new NewItemFromCloneDetails
        {
            ItemTplToClone = BaseInfo.CloneId!,
            // ParentId refers to the Node item the gun will be under, you can check it in https://db.sp-tarkov.com/search
            ParentId = BaseInfo.ParentId,
            NewId = BaseInfo.Id,
            FleaPriceRoubles = BaseInfo.FleaPrice,
            HandbookPriceRoubles = BaseInfo.HandbookPrice,
            HandbookParentId = BaseInfo.HandbookParentId,
            Locales = BaseInfo.Locales ?? new Dictionary<string, LocaleDetails>
            {
                {
                    "ch",
                    new LocaleDetails
                    {
                        Name = BaseInfo.Name,
                        ShortName = BaseInfo.Name,
                        Description = BaseInfo.Description
                    }
                },
                {
                    "en",
                    new LocaleDetails
                    {
                        Name = BaseInfo.Name,
                        ShortName = BaseInfo.Name,
                        Description = BaseInfo.Description
                    }
                }
            },
            OverrideProperties = PropertyOverride,
        };
    }

    /// <summary>
    /// 根据已有信息覆盖所有PropertyApply属性并更新服务器数据
    /// </summary>
    private void PropertyApplyAll()
    {
        if (PropertyOverride == null) PropertyOverride = new TemplateItemProperties();
        if (BaseInfo == null) return;
        foreach (AbstractInfo info in NeedValidator)
        {
            info.Update(PropertyOverride, DatabaseService);
        }
        LocalLog.Logger.Debug($"已使用[{string.Join(",", GetType()
            .GetProperties()
            .Where(p => p.PropertyType.IsSubclassOf(typeof(AbstractInfo)))
            .Where(p => p.GetValue(this) != null)
            .Select(p => p.Name)
            .ToList())}]生成物品{BaseInfo.Name}({BaseInfo.Id})的属性Property");
    }

    #endregion

    public override string ToString()
    {
        return $"NewItem {{ baseInfo: {LocalLog.ToStringExcludeNulls(BaseInfo)}, overrideProperties: {LocalLog.ToStringExcludeNulls(PropertyOverride)} }}";
    }
    
    #region 兼容性

    /// <summary>
    /// 旧格式类型标识符（仅用于向后兼容读取，不参与序列化输出）
    /// </summary>
    [JsonPropertyName("$type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LegacyType
    {
        get => null; // 永远返回null，不写入JSON
        set => _legacyTypeValue = value;
    }

    [JsonIgnore]
    private string? _legacyTypeValue;

    #endregion
}
