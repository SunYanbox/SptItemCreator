using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Abstracts;
using SptItemCreator.Enums;
using SptItemCreator.Services;

namespace SptItemCreator.InfoClasses;

/// <summary>
/// 新物品基础信息记录类
/// 包含物品的基本属性和元数据
/// </summary>
/// <remarks>
/// 属性列表:
/// - Id: 物品ID (必需) 
/// - Type: 模板类型，适合SptItemCreator模组的数据类型 (可选) 
/// - Name: 物品名称 (可选) 
/// - Description: 物品描述 (可选)
/// - Locales: 本地化数据库 (可选)
/// - Author: 作者名称 (可选) 
/// - License: 创建物品的协议 (可选) 
/// - Order: 影响新物品的创建顺序，数值越大加载越慢 (可选) 
/// - ParentId: 物品创建的ParentId (必需) 
/// - CloneId: 复制物品创建的原型Id (可选) 
/// - HandbookParentId: 复制物品创建的HandbookParentId (可选) 
/// - TraderId: 默认售卖该物品的商人Id (可选) 
/// - FleaPrice: 价格 (默认值: 1)
/// - HandbookPrice: 价格 (默认值: 1)
/// - Prefab: 物品模型 (可选)
/// - UsePrefab: 使用时的物品模型 (可选)
/// - CanSellOnRagfair: 是否允许在跳蚤市场售卖 (默认值: true)
/// - AllowAll: 一键允许所有容器放置本物品
/// - CanFilter: 指定哪些容器可放置本物品(优先级大于allowAll)
/// - CantFilter: 指定哪些容器不可放置本物品(优先级大于allowAll)
/// - IsHadInit: 是否已进行过初始化与参数验证 (内部使用)
/// </remarks>
///
public sealed record BaseInfo: AbstractInfo
{
    [JsonIgnore] private static Dictionary<MongoId, TemplateItem>? _itemTemplate;
    /// <summary>
    /// 物品ID
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    /// <summary>
    /// 模板类型(此处为适合SptItemCreator模组的数据类型)  [可缺省]
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 物品名称  [可缺省]
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 物品描述  [可缺省]
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 本地化数据库 [可缺省]
    /// </summary>
    [JsonPropertyName("locales")]
    public Dictionary<string, LocaleDetails>? Locales { get; set; }
    
    /// <summary>
    /// 作者名称  [可缺省]
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }
    
    /// <summary>
    /// 创建的这个物品的协议  [可缺省]
    /// </summary>
    [JsonPropertyName("license")]
    public string? License { get; set; }
    
    /// <summary>
    /// 影响新物品的创建顺序, 数值越大加载越慢  [可缺省]
    /// </summary>
    [JsonPropertyName("order")]
    public int? Order { get; set; }
    
    /// <summary>
    /// 物品创建的ParentId
    /// </summary>
    [JsonPropertyName("parentId")]
    public string? ParentId { get; set; }
    
    /// <summary>
    /// 复制物品创建的原型Id  [可缺省]
    /// </summary>
    [JsonPropertyName("cloneId")]
    public string? CloneId { get; set; }
    
    /// <summary>
    /// 复制物品创建的HandbookParentId  [可缺省]
    /// </summary>
    [JsonPropertyName("handbookParentId")]
    public string? HandbookParentId { get; set; }
    
    /// <summary>
    /// 默认售卖该物品的商人Id  [可缺省]
    /// </summary>
    [JsonPropertyName("traderId")]
    public string? TraderId { get; set; }

    /// <summary>
    /// 跳蚤市场价格
    /// </summary>
    [JsonPropertyName("fleaPrice")]
    public double FleaPrice { get; set; }
    
    /// <summary>
    /// 手册价格
    /// </summary>
    [JsonPropertyName("handbookPrice")]
    public double HandbookPrice { get; set; }
    
    /// <summary>
    /// 物品模型
    /// </summary>
    [JsonPropertyName("prefab")]
    public Prefab? Prefab { get; set; }
    
    /// <summary>
    /// 使用物品模型
    /// </summary>
    [JsonPropertyName("usePrefab")]
    public Prefab? UsePrefab { get; set; }
    
    /// <summary>
    /// 是否允许在跳蚤市场售卖 默认为true
    /// </summary>
    [JsonPropertyName("canSellOnRagfair")]
    public bool CanSellOnRagfair { get; set; } = Default.BaseInfoCanSellOnRagfair;
    
    /// <summary>
    /// 一键允许所有容器放置本物品
    /// </summary>
    [JsonPropertyName("allowAll")]
    public bool? AllowAll { get; set; }
    
    /// <summary>
    /// 指定哪些容器可放置本物品(优先级大于allowAll)
    /// </summary>
    [JsonPropertyName("canFilter")]
    public HashSet<MongoId>? CanFilter { get; set; }

    /// <summary>
    /// 指定哪些容器不可放置本物品(优先级大于allowAll)
    /// </summary>
    [JsonPropertyName("cantFilter")]
    public HashSet<MongoId>? CantFilter { get; set; }

    /// <summary>
    /// 是否已进行过初始化与参数验证
    /// </summary>
    [JsonIgnore] public bool IsHadInit { get; set; }

    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (!string.IsNullOrEmpty(Name)) properties.Name = Name;
        if (!string.IsNullOrEmpty(Name)) properties.ShortName = Name;
        if (!string.IsNullOrEmpty(Description)) properties.Description = Description;
        properties.CanSellOnRagfair = CanSellOnRagfair;
        try
        {
            if (Prefab != null)
            {
                if (string.IsNullOrEmpty(Prefab.Path))
                {
                    LocalLog?.LocalLogMsg(LocalLogType.Warn, $"物品{Name}的Prefab.Path为空, 请检查{ItemPath}");
                }
                properties.Prefab = Prefab;
            }
            if (UsePrefab != null)
            {
                if (string.IsNullOrEmpty(UsePrefab.Path))
                {
                    LocalLog?.LocalLogMsg(LocalLogType.Warn, $"物品{Name}的UsePrefab.Path为空, 请检查{ItemPath}");
                }
                properties.UsePrefab = UsePrefab;
            }
        }
        catch (Exception e)
        {
            LocalLog?.LocalLogMsg(LocalLogType.Error, $"物品{Name}的Prefab或UsePrefab语法错误, 无法解析 - {ItemPath} - {e.Message}");
        }
    }
    
    public override void UpdateDatabaseService(DatabaseService databaseService)
    {
        if (Id is null) return;
        _itemTemplate ??= databaseService.GetTables().Templates.Items;
        HashSet<MongoId> containerOutput = [];
        // 一键允许所有容器放置本物品
        if (AllowAll ?? false)
        {
            LocalLog?.LocalLogMsg(LocalLogType.Info, $"新物品{Name}已启用`AllowAll`字段");
            foreach ((MongoId containerTpl, TemplateItem container) in _itemTemplate.Where(x =>
                         x.Value.Properties?.Grids != null
                         && ItemHelper!.IsOfBaseclasses(x.Value.Id,
                             [BaseClasses.SIMPLE_CONTAINER, BaseClasses.MOB_CONTAINER])))
            {
                string name = ItemHelper!.GetItemName(containerTpl);
                if (container.Properties is not { Grids: not null })
                {
                    if (containerOutput.Add(containerTpl))
                    {
                        LocalLog?.LocalLogMsg(LocalLogType.Warn, $"容器{name}({containerTpl})没有非空的Grids属性");
                    }
                    continue;
                }
                foreach (Grid grid in container.Properties.Grids)
                {
                    if (grid.Properties is not { Filters: not null })
                    {
                        if (containerOutput.Add(containerTpl))
                        {
                            LocalLog?.LocalLogMsg(LocalLogType.Warn, $"容器{name}({containerTpl})的网格{grid.Name}({grid.Id})没有非空Filters属性");
                        }
                        continue;
                    }
                    foreach (GridFilter gridFilter in grid.Properties.Filters)
                    {
                        // 允许容器放这个物品
                        gridFilter.Filter?.Add(Id);
                        // 去掉限制
                        gridFilter.ExcludedFilter?.Remove(Id);
                        if (ParentId != null) gridFilter.ExcludedFilter?.Remove(ParentId);
                    }
                }
            }
        }
        // 指定哪些容器可放置本物品
        foreach (TemplateItem container in from containerId in CanFilter ?? [] 
                 where !(CantFilter?.Contains(containerId) ?? false) 
                 where !ItemHelper!.IsOfBaseclasses(containerId, [BaseClasses.SIMPLE_CONTAINER, BaseClasses.MOB_CONTAINER]) 
                 select _itemTemplate[containerId])
        {
            if (container.Properties is not { Grids: not null }) continue;
            foreach (Grid grid in container.Properties.Grids)
            {
                if (grid.Properties == null) continue;
                if (grid.Properties?.Filters == null) continue;
                foreach (GridFilter gridFilter in grid.Properties.Filters)
                {
                    gridFilter.Filter?.Add(Id);
                    gridFilter.ExcludedFilter?.Remove(Id);
                    if (ParentId is not null)
                        gridFilter.ExcludedFilter?.Remove(ParentId);
                }
            }
        }
        // 指定哪些容器不可放置本物品
        foreach (TemplateItem container in from containerId in CantFilter ?? [] 
                 where !(CanFilter?.Contains(containerId) ?? false) 
                 where !ItemHelper!.IsOfBaseclasses(containerId, [BaseClasses.SIMPLE_CONTAINER, BaseClasses.MOB_CONTAINER]) 
                 select _itemTemplate[containerId])
        {
            if (container.Properties is not { Grids: not null }) continue;
            foreach (Grid grid in container.Properties.Grids)
            {
                if (grid.Properties == null) continue;
                if (grid.Properties?.Filters == null) continue;
                foreach (GridFilter gridFilter in grid.Properties.Filters)
                {
                    gridFilter.Filter?.Remove(Id);
                    gridFilter.ExcludedFilter?.Add(Id);
                }
            }
        }
    }
}


/*
  "5b47574386f77428ca22b2ed": "能源物品",
  "5b47574386f77428ca22b2ee": "建筑材料",
  "5b47574386f77428ca22b2ef": "电子产品",
  "5b47574386f77428ca22b2f0": "日常用品",
  "5b47574386f77428ca22b2f1": "贵重物品",
  "5b47574386f77428ca22b2f2": "易燃物品",
  "5b47574386f77428ca22b2f3": "医疗用品",
  "5b47574386f77428ca22b2f4": "其他",
  "5b47574386f77428ca22b2f6": "工具",
  "5b47574386f77428ca22b32f": "面部装备",
  "5b47574386f77428ca22b330": "头部装备",
  "5b47574386f77428ca22b331": "眼部装备",
  "5b47574386f77428ca22b335": "饮品",
  "5b47574386f77428ca22b336": "食物",
  "5b47574386f77428ca22b337": "药品",
  "5b47574386f77428ca22b338": "急救包",
  "5b47574386f77428ca22b339": "创伤处理",
  "5b47574386f77428ca22b33a": "注射器",
  "5b47574386f77428ca22b33b": "子弹",
  "5b47574386f77428ca22b33c": "弹药包",
  "5b47574386f77428ca22b33e": "交换用物品",
  "5b47574386f77428ca22b33f": "装备",
  "5b47574386f77428ca22b340": "给养",
  "5b47574386f77428ca22b341": "情报物品",
  "5b47574386f77428ca22b342": "钥匙",
  "5b47574386f77428ca22b343": "地图",
  "5b47574386f77428ca22b344": "医疗物品",
  "5b47574386f77428ca22b345": "特殊装备",
  "5b47574386f77428ca22b346": "弹药",
 */
