using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Core.Cache.Extensions;

namespace SptItemCreator.Core.Cache;

public class StatsHandler
{
    public static ItemHelper? ItemHelper;
    public static DatabaseService? DatabaseService;
    
    [JsonIgnore]
    private static Dictionary<MongoId, TemplateItem>? _templateItems;
    /// <summary> 要保存的位置 </summary>
    [JsonPropertyName("savePath")]
    public string? SavePath { get; set; }
    /// <summary> { 属性名称 -> 属性值列表 } </summary>
    [JsonPropertyName("statisticalData")]
    public Dictionary<string, PropertyList> StatisticalData { get; set; } = new();
    [JsonPropertyName("handledItems")]
    public HashSet<MongoId> HandledItems { get; set; } = [];
    /// <summary> 处理的类型 </summary>
    [JsonPropertyName("handleBaseClasses")]
    public MongoId HandleBaseClasses { get; set; }
    /// <summary> 此缓存名称 </summary>
    [JsonPropertyName("cacheName")]
    public string? CacheName { get; set; }

    /// <summary> 统计处理类型对应的物品模板的数据 </summary>
    public string StatsItems()
    {
        _templateItems ??= DatabaseService!.GetItems();
        var stopwatch = Stopwatch.StartNew();
        
        foreach (MongoId templateId in ItemHelper!.GetItemTplsOfBaseType(HandleBaseClasses)
                     .Where(id => ItemHelper.IsValidItem(id)))
        {
            if (HandledItems.Add(templateId))
            {
                TemplateItem? templateItem = _templateItems.GetValueOrDefault(templateId);
                if (templateItem?.Properties is null)
                {
                    return $"[{CacheName}类型] 物品{templateId}无法获取到对应模板实例或模板属性实例";
                }

                foreach ((string name, object? value) in GetPublicNonNullPropertyList(templateItem.Properties))
                {
                    if (!StatisticalData.TryGetValue(name, out PropertyList? propertyList))
                    {
                        propertyList = new PropertyList();
                        StatisticalData[name] = propertyList;
                    }
                    propertyList.Add(value);
                }
            }
            else
            {
                return $"[{CacheName}类型] 重复统计物品: {templateId}";
            }
        }
        stopwatch.Stop();
        return $"统计{CacheName}类型耗时: {stopwatch.Elapsed.TotalMilliseconds:F3}ms";
    }
    
    private static List<(string Name, object? Value)> GetPublicNonNullPropertyList<T>(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .Select(p => (p.Name, Value: p.GetValue(obj)))
            .Where(x => x.Value != null)
            .ToList();
    }
}