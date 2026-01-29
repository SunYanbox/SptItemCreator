using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Enums;
using Path = System.IO.Path;

namespace SptItemCreator.CacheSystem;

[UsedImplicitly]
public sealed class ParentIdNameRate
{
    [JsonPropertyName("parentId")]
    public MongoId ParentId { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("count")]
    public int Count { get; init; }
}

[UsedImplicitly]
public sealed class ParentIdCache
{
    /// <summary> 更新时间 </summary>
    [JsonPropertyName("updateTime")] public long UpdateTime { get; set; }
    /// <summary> parentId, Name, Rate </summary>
    [JsonPropertyName("parentIdNameRate")] public List<ParentIdNameRate> ParentIdNameRate { get; set; } = [];
}

/// <summary>
/// 获取SPT的服务器数据并记录到缓存中
/// </summary>
[Injectable(InjectionType.Singleton, typePriority: OnLoadOrder.TraderCallbacks - 1)]
public sealed class SPTDataCacheService(
        ModHelper modHelper,
        LocalLog localLog,
        JsonUtil jsonUtil,
        DatabaseService databaseService
    ): IOnLoad, IOnUpdate
{
    private bool _initialized;
    private bool _firstOrganize;
    private long _updateTime;
    private string? ModCachePath { get; set; }
    private string? ParentIdCachePath { get; set; }
    private string? BackgroundColorCachePath { get; set; }
    public readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    /// <summary>
    /// ParentId的缓存
    /// </summary>
    public ParentIdCache? ParentIdCache { get; private set; }
    /// <summary>
    /// BackgroundColor的缓存
    /// </summary>
    public HashSet<string>? BackgroundColorCache { get; private set; }
    
    public Task OnLoad()
    {
        ModCachePath ??= Path.Combine(modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly()), "cache");
        ParentIdCachePath ??= Path.Combine(ModCachePath, "ParentIdCache.json");
        BackgroundColorCachePath ??= Path.Combine(ModCachePath, "BackgroundColorCache.json");
        Directory.CreateDirectory(ModCachePath);
        _initialized = true;
        _firstOrganize = true;
        _updateTime = 0;
        return Task.CompletedTask;
    }
    
    public async Task<bool> OnUpdate(long secondsSinceLastRun)
    {
        if (!_initialized) return false;
        long now = Stopwatch.ElapsedMilliseconds;
        // 首次强制整理
        if (!_firstOrganize)
        {
            if (Math.Abs(now - _updateTime) <= Default.CacheSystemUpdateTime && _updateTime > 0) return false;
            _updateTime = now;
        }

        await OrganizeBackgroundColor();
        await OrganizeParentId();
        if (_firstOrganize)
        {
            _firstOrganize = false;
            localLog.LocalLogMsg(LocalLogType.Info, "已强制更新缓存");
        }
        localLog.LocalLogMsg(LocalLogType.Debug, $"更新缓存消耗时间: {(now - Stopwatch.ElapsedMilliseconds) / 1000.0:F3}s");
        return true;
    }

    /// <summary>
    /// 整理物品背景色数据(只会在首次使用模组时加载一次)
    /// </summary>
    private async Task OrganizeBackgroundColor()
    {
        if (BackgroundColorCache is not null) return;
        if (Path.Exists(BackgroundColorCachePath))
        {
            try
            {
                BackgroundColorCache =
                    await jsonUtil.DeserializeFromFileAsync<HashSet<string>>(BackgroundColorCachePath!);
                int? count = BackgroundColorCache?.Count;
                if (count >= 9)
                {
                    localLog.LocalLogMsg(LocalLogType.Info, $"加载物品BackgroundColor(背景色)缓存共{count}条");
                    return;
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Warn, $"加载物品BackgroundColor(背景色)缓存时出现问题: {e.Message}");
            }
        }

        BackgroundColorCache = [];
        foreach (TemplateItem templateItem in databaseService.GetTemplates().Items.Values)
        {
            if (templateItem is { Properties.BackgroundColor: not null })
            {
                BackgroundColorCache.Add(templateItem.Properties.BackgroundColor);
            }
        }
        
        try
        {
            await File.WriteAllTextAsync(BackgroundColorCachePath!, jsonUtil.Serialize(BackgroundColorCache, true));
        }
        catch (Exception e)
        {
            localLog.LocalLogMsg(LocalLogType.Warn, $"保存物品BackgroundColor(背景色)缓存时出现问题: {e.Message}");
        }
        
        localLog.LocalLogMsg(LocalLogType.Info, "已整理物品背景颜色缓存");
    }
    
    /// <summary>
    /// 整理所有类型的物品ParentId数据
    /// </summary>
    private async Task OrganizeParentId()
    {
        if (Path.Exists(ParentIdCachePath) && ParentIdCache is null)
        {
            try
            {
                ParentIdCache = await jsonUtil.DeserializeFromFileAsync<ParentIdCache>(ParentIdCachePath!);
                
                int? count = ParentIdCache?.ParentIdNameRate.Count;
                if (count > 0)
                {
                    localLog.LocalLogMsg(LocalLogType.Info, $"加载物品ParentId缓存共{count}条");
                    return;
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Warn, $"加载ParentId缓存时出现问题: {e.Message}");
            }
        }
        ParentIdCache ??= new ParentIdCache();
        long start = Stopwatch.ElapsedMilliseconds;
        if (!_firstOrganize)
        {
            if (Math.Abs(start - ParentIdCache.UpdateTime) <= Default.ParentIdCacheUpdateTime) return;
        }
        ParentIdCache.UpdateTime = start;
        ParentIdCache.ParentIdNameRate.Clear();
        
        Dictionary<MongoId, TemplateItem> itemTemplate = databaseService.GetTemplates().Items;
        // 使用更合适的容量初始化，减少扩容
        Dictionary<MongoId, string> parentIdLocal = new(50);
        Dictionary<MongoId, int> parentIdCount = new(50);
        
        foreach (TemplateItem templateValue in itemTemplate.Values)
        {
            var parentStr = templateValue.Parent.ToString();
            int parentLength = parentStr.Length;
            
            if (parentLength is 0 or not 24)
            {
                continue;
            }

            MongoId parentId = templateValue.Parent;
    
            // 如果parentId已经处理过，只增加计数
            if (parentIdCount.TryGetValue(parentId, out int currentCount))
            {
                parentIdCount[parentId] = currentCount + 1;
                continue;
            }
    
            // 新parentId，处理本地化名称
            string localName = parentStr;
            if (itemTemplate.TryGetValue(parentId, out TemplateItem? parentTemplateItem))
            {
                string parentEnName = parentTemplateItem.Name!;
                localName = parentEnName;
            }
    
            // 使用Add方法，避免TryAdd的开销（因为我们已经检查过不存在）
            parentIdLocal.Add(parentId, localName);
            parentIdCount.Add(parentId, 1);
        }

        foreach (MongoId parentId in parentIdLocal.Keys)
        {
            ParentIdCache.ParentIdNameRate.Add(new ParentIdNameRate
            {
                ParentId = parentId,
                Name = parentIdLocal[parentId],
                Count = parentIdCount[parentId]
            });
        }
        
        try
        {
            await File.WriteAllTextAsync(ParentIdCachePath!, jsonUtil.Serialize(ParentIdCache, true));
        }
        catch (Exception e)
        {
            localLog.LocalLogMsg(LocalLogType.Warn, $"保存ParentId缓存时出现问题: {e.Message}");
        }
        
        localLog.LocalLogMsg(LocalLogType.Info, "已整理物品ParentId缓存");
    }
}