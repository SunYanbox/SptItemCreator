using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using SptItemCreator.InfoClasses;
using SptItemCreator.NewItemClasses;

namespace SptItemCreator.Abstracts;

/// <summary>
/// 抽象新物品基类 
/// 提供新物品创建和验证的基础功能
/// </summary>
/// 
/// 公开属性: 
/// - BaseInfo: 基础新物品信息 
/// - PropertyOverride: 需要覆盖的属性/新物品相对于克隆物品修改了的属性 
/// - ItemPath: 记录新物品文件的路径 (内部使用) 
/// - LocalLog: 储存本地日志实例，方便输出错误信息 (静态，内部使用) 
/// 
/// 公开方法: 
/// - Verify(): 验证数据有效性，并根据基础信息更新BaseInfo 
/// - CreateNewItem(): 根据物品数据返回创建新物品的NewItemDetails (详情创建模式) 
/// - CreateItemFromClone(): 根据物品数据返回创建新物品的NewItemFromCloneDetails (克隆创建模式) 
/// - CheckParameter(): 检查参数是否为空或满足要求，随后初始化 
/// - ToString(): 返回对象的字符串表示形式 
/// 
/// 受保护虚方法 (需要子类实现): 
/// - DoCustomValidation(): 执行自定义验证 (可选重载) 
/// - DoCustomParameterValidation(): 执行自定义参数验证并记录错误信息 (可选重载) 
/// - DoPropertyApplication(): 应用自定义属性覆盖 (必须重载) 
///
[JsonDerivedType(typeof(AbstractNewItem), typeDiscriminator: "abstract")]
[JsonDerivedType(typeof(NewItemCommon), typeDiscriminator: "common")]
[JsonDerivedType(typeof(NewItemDrinkOrFood), typeDiscriminator: "drinkOrFood")]
[JsonDerivedType(typeof(NewItemMedical), typeDiscriminator: "medical")]
[JsonDerivedType(typeof(NewItemAmmo), typeDiscriminator: "ammo")]
public abstract class AbstractNewItem
{
    /// <summary>
    /// 控制是否加载该物品(默认为false, 且不应频繁修改, 避免对存档造成损坏)
    /// </summary>
    [JsonPropertyName("enable")]
    public bool? Enable { get; set; } = false;
    
    /// <summary>
    /// 基础新物品信息
    /// </summary>
    [JsonPropertyName("baseInfo")]
    public BaseInfo? BaseInfo { get; set; }
    
    /// <summary>
    /// 需要覆盖的属性/新物品相对于克隆物品修改了的属性
    /// </summary>
    [JsonPropertyName("propertyOverride")]
    public TemplateItemProperties? PropertyOverride { get; set; }
    
    /// <summary>
    /// 记录新物品文件的路径
    /// </summary>
    [JsonIgnore] public string ItemPath { get; set; } = string.Empty;
    
    /// <summary>
    /// 储存本地日志实例, 方便输出错误信息
    /// </summary>
    [JsonIgnore] public static LocalLog? LocalLog;
    
    /// <summary>
    /// 储存数据库信息
    /// </summary>
    [JsonIgnore] public static DatabaseService? DatabaseService;
    
    /// <summary>
    /// 验证数据有效性, 并根据基础信息更新BaseInfo
    /// </summary>
    /// <returns></returns>
    public bool Verify()
    {
        if (BaseInfo != null)
        {
            CheckParameter();
            AutoPropertyApply();
        }
        
        if (BaseInfo?.Id == null || BaseInfo.ParentId == null || !DoCustomValidation())
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 根据物品数据返回创建新物品的NewItemDetails
    /// 返回null表示数据无效等意外情况
    /// </summary>
    /// <returns></returns>
    public NewItemDetails? CreateNewItem()
    {
        if (!Verify() || BaseInfo?.Id == null || BaseInfo?.ParentId == null || BaseInfo?.CloneId != null)
        {
            if (LocalLog == null) return null;
            var msg = "";
            if (!Verify()) msg += $"{this}.Verify 详情创建模式 验证失败:";
            if (BaseInfo?.Id == null) msg += "\n\tbaseInfo.id 意外为null";
            if (BaseInfo?.ParentId == null) msg += "\n\tbaseInfo.parent 意外为null";
            if (BaseInfo?.CloneId != null) msg += "\n\tbaseInfo.cloneId 意外被赋值";
            if (BaseInfo?.HandbookParentId != null) msg += "\n\tbaseInfo.handbookParentId 意外被赋值";
            if (!string.IsNullOrEmpty(msg)) LocalLog.LocalLogMsg(LocalLogType.Error, msg);
            return null;
        }
        if (BaseInfo == null) return null;
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
        if (Verify() && BaseInfo is { Id: not null, ParentId: not null, CloneId: not null, HandbookParentId: not null })
        {
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
        if (LocalLog == null) return null;
        var msg = "";
        if (!Verify()) msg += $"{this}.Verify 克隆创建模式 验证失败:";
        if (BaseInfo?.Id == null) msg += "\n\tbaseInfo.id 意外为null";
        if (BaseInfo?.ParentId == null) msg += "\n\tbaseInfo.parent 意外为null";
        if (BaseInfo?.CloneId == null && BaseInfo?.HandbookParentId == null) 
            msg += "\n\tbaseInfo.cloneId为null时baseInfo.handbookParentId意外为null";
        if (!string.IsNullOrEmpty(msg)) LocalLog.LocalLogMsg(LocalLogType.Error, msg);
        return null;
    }

    /// <summary>
    /// 根据已有信息覆盖PropertyApply属性, 子类重载DoPropertyApplication以扩展覆盖的内容
    ///
    /// 只有验证通过的BaseInfo才会被用来覆盖PropertyApply的信息
    /// 基类默认覆盖的内容: 名称, 描述
    /// </summary>
    private void AutoPropertyApply()
    {
        if (PropertyOverride == null) PropertyOverride = new TemplateItemProperties();
        if (BaseInfo == null) return;
        BaseInfo.Update(PropertyOverride, DatabaseService);
        DoPropertyApplication(PropertyOverride, DatabaseService);
    }
    
    /// <summary>
    /// 执行自定义验证 **必须重载**
    /// </summary>
    protected abstract bool DoCustomValidation();

    /// <summary>
    /// 执行自定义参数验证并记录错误信息 **必须重载**
    ///
    /// 可以在此处为一些属性赋默认值
    /// 
    /// 通过参数字典传递数据
    /// </summary>
    protected abstract void DoCustomParameterValidation(Dictionary<string, string> oldResults);
    
    /// <summary>
    /// 应用自定义属性覆盖 **必须重载**
    /// 应当只修改PropertyApply的内容, 对其他属性应当是只读的
    /// </summary>
    protected abstract void DoPropertyApplication(TemplateItemProperties props, DatabaseService? databaseService = null);
    
    
    /// <summary>
    /// 检查参数是否为空或满足要求, 随后初始化
    ///
    /// - 如果满足, 返回true
    /// - 如果不满足, 返回false并同步记录本地日志
    /// </summary>
    public bool CheckParameter()
    {
        Dictionary<string, string> results = new Dictionary<string, string>();

        // 检查 BaseInfo 是否为 null
        if (BaseInfo == null)
        {
            results["BaseInfo"] = $"BaseInfo意外为null @{ItemPath}";
            // 如果 BaseInfo 为 null，直接记录错误并返回
            LogValidationErrors(results);
            return false;
        }

        // 初始化 BaseInfo 并验证字段
        if (!BaseInfo.IsHadInit)
        {
            InitializeBaseInfo();
            
            // 执行自定义参数验证与数据初始化
            DoCustomParameterValidation(results);
            
            ValidateBaseInfoFields(results);
            
            BaseInfo.IsHadInit = true;
        }
        
        // 根据验证结果处理
        if (results.Count == 0) 
        {
            return true;
        }

        LogValidationErrors(results);
        return false;
    }

    /// <summary>
    /// 初始化 BaseInfo 的默认值
    /// </summary>
    private void InitializeBaseInfo()
    {
        BaseInfo ??= new BaseInfo();
        BaseInfo.Name ??= "未命名物品";
        BaseInfo.Type ??= "common";
        BaseInfo.Author ??= "佚名";
        BaseInfo.License ??= "MIT";
        BaseInfo.Description ??= "";
        BaseInfo.Order ??= 0;
        
        // 只在 Description 不包含基本信息时才追加; 提供Locales后，实际客户端显示的描述中不会有这些额外信息
        if (!BaseInfo.Description.Contains(BaseInfo.Name) || 
            !BaseInfo.Description.Contains("作者:") || 
            !BaseInfo.Description.Contains("协议:"))
        {
            BaseInfo.Description += $"\n\n{BaseInfo.Name}\n作者: @{BaseInfo.Author}\n协议: {BaseInfo.License}";
        }
        
        BaseInfo.FleaPrice = Math.Max(BaseInfo.FleaPrice, 1); // 避免价格为0导致物品无效
        BaseInfo.HandbookPrice = Math.Max(BaseInfo.HandbookPrice, 1); // 避免价格为0导致物品无效
    }

    /// <summary>
    /// 验证 BaseInfo 字段
    /// </summary>
    private void ValidateBaseInfoFields(Dictionary<string, string> results)
    {
        bool isCloneItem = BaseInfo!.CloneId != null;
        string prefix = isCloneItem ? 
            $"基于克隆创建新物品\"{ItemPath}\"时验证失败:" : 
            $"基于详情创建新物品\"{ItemPath}\"时验证失败:";
        
        List<string> errorMessages = new List<string>();

        // 验证公共字段
        ValidateCommonFields(results, errorMessages);
        
        // 根据物品类型验证特定字段
        if (isCloneItem)
        {
            ValidateCloneItemFields(results, errorMessages);
        }
        else
        {
            ValidateNewItemFields(results, errorMessages);
        }
        
        // 如果有错误消息，格式化并记录
        if (errorMessages.Count > 0)
        {
            string formattedMessage = FormatErrorMessages(prefix, errorMessages);
            results["BaseInfoValidation"] = formattedMessage;
        }
    }

    /// <summary>
    /// 验证公共字段
    /// </summary>
    private void ValidateCommonFields(Dictionary<string, string> results, List<string> errorMessages)
    {
        // 验证 Id 字段
        if (BaseInfo!.Id == null)
        {
            errorMessages.Add("缺少id字段");
            results["Id"] = "Id字段不能为null";
        }
        else if (!MongoId.IsValidMongoId(BaseInfo.Id))
        {
            errorMessages.Add("id字段不是有效的MongoId");
            results["Id"] = $"无效的MongoId: {BaseInfo.Id}";
        }

        // 验证 ParentId 字段
        if (BaseInfo.ParentId == null)
        {
            errorMessages.Add("缺少parent字段");
            results["ParentId"] = "ParentId字段不能为null";
        }
        else if (!MongoId.IsValidMongoId(BaseInfo.ParentId))
        {
            errorMessages.Add("parent字段不是有效的MongoId");
            results["ParentId"] = $"无效的MongoId: {BaseInfo.ParentId}";
        }
    }

    /// <summary>
    /// 验证克隆物品字段
    /// </summary>
    private void ValidateCloneItemFields(Dictionary<string, string> results, List<string> errorMessages)
    {
        if (BaseInfo!.CloneId != null) return;
        errorMessages.Add("缺少cloneId字段");
        results["CloneId"] = "克隆物品必须包含CloneId字段";
    }

    /// <summary>
    /// 验证新物品字段
    /// </summary>
    private void ValidateNewItemFields(Dictionary<string, string> results, List<string> errorMessages)
    {
        if (BaseInfo!.CloneId != null)
        {
            errorMessages.Add("创建新物品时意外添加了cloneId字段");
            results["CloneId"] = "新物品不应包含CloneId字段";
        }

        if (BaseInfo.HandbookParentId != null) return;
        errorMessages.Add("创建新物品时未添加handbookParentId字段, 可能导致物品无法正确分类");
        results["HandbookParentId"] = "新物品没有正确包含HandbookParentId字段, 可能导致物品无法正确分类";
    }

    /// <summary>
    /// 格式化错误消息
    /// </summary>
    private string FormatErrorMessages(string prefix, List<string> errorMessages)
    {
        var indentedMessages = errorMessages.Select(msg => $"\n\t - {msg}");
        return prefix + string.Join("", indentedMessages);
    }

    /// <summary>
    /// 记录验证错误
    /// </summary>
    private void LogValidationErrors(Dictionary<string, string> results)
    {
        if (results.Count == 0) return;

        string errorSummary = FormatValidationErrorSummary(results);
        LocalLog?.LocalLogMsg(LocalLogType.Error, errorSummary);
    }

    /// <summary>
    /// 格式化验证错误摘要
    /// </summary>
    private string FormatValidationErrorSummary(Dictionary<string, string> results)
    {
        var errorLines = results.Select(kvp => $"{kvp.Key}: {kvp.Value}");
        string detailedErrors = string.Join("\n", errorLines);
        
        return $"@{ItemPath} 物品属性验证出错，共发现 {results.Count} 个问题:\n{detailedErrors}";
    }
        
    public override string ToString()
    {
        return $"{GetType().Name} {{ baseInfo: {LocalLog.ToStringExcludeNulls(BaseInfo)}, overrideProperties: {LocalLog.ToStringExcludeNulls(PropertyOverride)}}}";
    }
}