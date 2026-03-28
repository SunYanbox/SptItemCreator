using SPTarkov.Server.Core.Models.Common;
using SptItemCreator.Core.Enums;
using SptItemCreator.Models.Abstracts;

namespace SptItemCreator.Models.Validators;

/// <summary>
/// 用来验证并初始化BaseInfo中缺失的部分
/// </summary>
public class BaseInfoValidator: BaseValidator
{
    private static readonly HashSet<string> AllSicType = [..SicType.AllSicType];
    
    public override bool CanHandle(INewItem newItem) => true;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (!ValidateRequiredFields(newItem, errorCollector)) return false;

        ApplyDefaultValues(newItem);

        ValidateFieldFormats(newItem, errorCollector);

        return true;
    }

    /// <summary>
    /// 验证必须属性
    /// </summary>
    private static bool ValidateRequiredFields(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.BaseInfo is null)
        {
            errorCollector.AddError("BaseInfo", "物品没有BaseInfo数据");
            return false;
        }

        if (newItem.BaseInfo.ParentId is null || !MongoId.IsValidMongoId(newItem.BaseInfo.ParentId))
        {
            errorCollector.AddError("BaseInfo", $"物品({newItem.BaseInfo.Id}没有有效的BaseInfo.ParentId数据({newItem.BaseInfo.ParentId})");
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// 应用默认配置
    /// </summary>
    private static void ApplyDefaultValues(INewItem newItem)
    {
        newItem.BaseInfo!.Id ??= new MongoId();
        newItem.BaseInfo.Name ??= Default.BaseInfoName;
        newItem.BaseInfo.Type ??= Default.BaseInfoType;
        newItem.BaseInfo.Author ??= Default.BaseInfoAuthor;
        newItem.BaseInfo.License ??= Default.BaseInfoLicense;
        newItem.BaseInfo.Description ??= Default.BaseInfoDescription;
        newItem.BaseInfo.Order ??= Default.BaseInfoOrder;
        
        // 只在 Description 不包含基本信息时才追加; 提供Locales后，实际客户端显示的描述中不会有这些额外信息
        if (!newItem.BaseInfo.Description.Contains(newItem.BaseInfo.Name) || 
            !newItem.BaseInfo.Description.Contains("作者:") || 
            !newItem.BaseInfo.Description.Contains("协议:"))
        {
            newItem.BaseInfo.Description += $"\n\n{newItem.BaseInfo.Name}\n作者: @{newItem.BaseInfo.Author}\n协议: {newItem.BaseInfo.License}";
        }
        
        newItem.BaseInfo.FleaPrice = Math.Max(newItem.BaseInfo.FleaPrice, Default.BaseInfoFleaPriceMinimum); // 避免价格为0导致物品无效
        newItem.BaseInfo.HandbookPrice = Math.Max(newItem.BaseInfo.HandbookPrice, Default.BaseInfoHandbookPriceMinimum); // 避免价格为0导致物品无效
    }
    
    /// <summary>
    /// 验证属性类型等
    /// </summary>
    private static void ValidateFieldFormats(INewItem newItem, IErrorCollector errorCollector)
    {
        if (!AllSicType.Contains(newItem.BaseInfo?.Type ?? ""))
        {
            errorCollector.AddError("BaseInfo", $"[Type] 不是合法的SicType: {newItem.BaseInfo?.Type}");
        }

        if (newItem.BaseInfo?.CloneId is not null)
        {
            if (!MongoId.IsValidMongoId(newItem.BaseInfo.CloneId))
            {
                errorCollector.AddError("BaseInfo", $"[CloneId] 不是合法的MongoId: {newItem.BaseInfo.CloneId}");
            }
        }
        
        if (newItem.BaseInfo?.HandbookParentId is not null)
        {
            if (!MongoId.IsValidMongoId(newItem.BaseInfo.HandbookParentId))
            {
                errorCollector.AddError("BaseInfo", $"[HandbookParentId] 不是合法的MongoId: {newItem.BaseInfo.HandbookParentId}");
            }
        }
        
        if (newItem.BaseInfo?.TraderId is not null)
        {
            if (!MongoId.IsValidMongoId(newItem.BaseInfo.TraderId))
            {
                errorCollector.AddError("BaseInfo", $"[TraderId] 不是合法的MongoId: {newItem.BaseInfo.TraderId}");
            }
        }

        if (newItem.BaseInfo?.Prefab is not null)
        {
            if (newItem.BaseInfo.Prefab.Path is null)
            {
                errorCollector.AddError("BaseInfo", $"[Prefab] 物品{newItem.BaseInfo.Name}的Prefab非空但Prefab.Path为空");
            }
        }
        
        if (newItem.BaseInfo?.UsePrefab is not null)
        {
            if (newItem.BaseInfo.UsePrefab.Path is null)
            {
                errorCollector.AddError("BaseInfo", $"[UsePrefab] 物品{newItem.BaseInfo.Name}的UsePrefab非空但UsePrefab.Path为空");
            }
        }
    }
}