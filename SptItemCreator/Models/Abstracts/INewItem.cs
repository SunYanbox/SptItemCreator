using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Abstracts;

public interface INewItem
{
    /// <summary>
    /// 是否启用该物品（默认 false，频繁修改可能导致存档损坏）
    /// </summary>
    [JsonPropertyName("enable")]
    public bool? Enable { get; set; }
    
    /// <summary>
    /// 物品基础信息（ID、名称、描述等）
    /// </summary>
    [JsonPropertyName("baseInfo")]
    public BaseInfo? BaseInfo { get; set; }
    
    /// <summary>
    /// 属性覆盖（相对于被克隆物品的修改项）
    /// </summary>
    [JsonPropertyName("propertyOverride")]
    [UsedImplicitly]
    public TemplateItemProperties? PropertyOverride { get; set; }
    
    /// <summary>
    /// 物品属性（耐久、人机工效、散热等装备属性）
    /// </summary>
    [JsonPropertyName("attributeInfo")]
    public AttributeInfo? AttributeInfo { get; set; }
    
    /// <summary>
    /// Buff 效果配置
    /// </summary>
    [JsonPropertyName("buffsInfo")]
    public BuffsInfo? BuffsInfo { get; set; }
    
    /// <summary>
    /// 饮品/食物属性（能量、水分等）
    /// </summary>
    [JsonPropertyName("drinkFoodInfo")]
    public DrinkFoodInfo? DrinkFoodInfo { get; set; }
    
    /// <summary>
    /// 医疗物品属性（治疗效果、使用时间等）
    /// </summary>
    [JsonPropertyName("medicalInfo")]
    public MedicalInfo? MedicalInfo { get; set; }
    
    /// <summary>
    /// 弹药属性（穿透、伤害、初速等）
    /// </summary>
    [JsonPropertyName("ammoInfo")]
    public AmmoInfo? AmmoInfo { get; set; }
    
    /// <summary>
    /// 记录新物品文件的路径
    /// </summary>
    [JsonIgnore] public string ItemPath { get; set; }

    /// <summary>
    /// 验证物品数据
    /// </summary>
    /// <remarks>除了非常严重的错误以外，其他情况仍然会返回true(并记录问题)</remarks>
    public (bool verify, IErrorCollector errors) Verify();

    [JsonIgnore]
    [UsedImplicitly]
    AbstractInfo[] NeedValidator { get; }

    /// <summary>
    /// 根据物品数据返回创建新物品的NewItemDetails
    /// 返回null表示数据无效等意外情况
    /// </summary>
    /// <returns></returns>
    public NewItemDetails? CreateNewItem();

    /// <summary>
    /// 根据物品数据返回创建新物品的NewItemFromCloneDetails
    /// 返回null表示数据无效等意外情况
    /// </summary>
    /// <returns></returns>
    public NewItemFromCloneDetails? CreateItemFromClone();
}