using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Enums;
using Path = System.IO.Path;

namespace SptItemCreator.Services;

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

[UsedImplicitly]
public sealed class BuffCache
{
    [JsonPropertyName("buffType")] public HashSet<string> BuffType { get; } = [];
    [JsonPropertyName("skillName")] public HashSet<string> SkillName { get; } = [];
}

[UsedImplicitly]
public sealed class AmmoCache
{
    /// <summary> 子弹类型 </summary>
    [JsonPropertyName("ammoType")]
    public HashSet<string> AmmoType { get; set; } = [];
    
    /// <summary> 口径类型 </summary>
    [JsonPropertyName("caliber")]
    public HashSet<string> Caliber { get; set; } = [];
    
    /// <summary> 抛壳声音 </summary>
    [JsonPropertyName("CasingSounds")]
    public HashSet<string> CasingSounds { get; set; } = [];
    
    /// <summary> 子弹射击时的音效类型 </summary>
    [JsonPropertyName("ammoSfx")]
    public HashSet<string> AmmoSfx { get; set; } = [];
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
    public string? ModPath { get; private set; }
    private string? ModCachePath { get; set; }
    private string? ParentIdCachePath { get; set; }
    private string? BackgroundColorCachePath { get; set; }
    private string? BuffCachePath { get; set; }
    private string? AmmoCachePath { get; set; }
    public readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    /// <summary>
    /// ParentId的缓存
    /// </summary>
    public ParentIdCache? ParentIdCache { get; private set; }
    /// <summary>
    /// BackgroundColor的缓存
    /// </summary>
    public HashSet<string>? BackgroundColorCache { get; private set; }
    /// <summary>
    /// Buff相关缓存
    /// </summary>
    public BuffCache? BuffCache { get; private set; }
    /// <summary>
    /// 子弹相关缓存
    /// </summary>
    public AmmoCache? AmmoCache { get; private set; }
    
    public Task OnLoad()
    {
        ModPath ??= modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        ModCachePath ??= Path.Combine(ModPath, "cache");
        ParentIdCachePath ??= Path.Combine(ModCachePath, "ParentIdCache.json");
        BackgroundColorCachePath ??= Path.Combine(ModCachePath, "BackgroundColorCache.json");
        BuffCachePath ??= Path.Combine(ModCachePath, "BuffCache.json");
        AmmoCachePath ??= Path.Combine(ModCachePath, "AmmoCache.json");
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

        await OrganizeAmmo();
        await OrganizeBuff();
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
    /// 整理物品背景色数据(只会在首次加载模组时加载一次)
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
    /// 整理效果名称与类型数据(只会在首次加载模组时加载一次)
    /// </summary>
    private async Task OrganizeBuff()
    {
        if (BuffCache is not null) return;
        if (Path.Exists(BuffCachePath))
        {
            try
            {
                BuffCache = await jsonUtil.DeserializeFromFileAsync<BuffCache>(BuffCachePath!);
                (int? countBuffType, int? countSkillName) = (BuffCache?.BuffType.Count, BuffCache?.SkillName.Count);
                if (countBuffType > 0 && countSkillName > 0)
                {
                    localLog.LocalLogMsg(LocalLogType.Info, $"加载BuffType(效果类型)缓存共{countBuffType}条, 加载SkillName(技能名称)缓存共{countSkillName}条");
                    return;
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Warn, $"加载BuffCache(效果缓存)时出现问题: {e.Message}");
            }
        }

        BuffCache = new BuffCache();
        
        foreach ((string _, IEnumerable<Buff> buffs) in databaseService.GetGlobals().Configuration.Health.Effects.Stimulator.Buffs)
        {
            foreach (Buff buff in buffs)
            {
                if (!string.IsNullOrEmpty(buff.SkillName)) BuffCache.SkillName.Add(buff.SkillName);
                if  (!string.IsNullOrEmpty(buff.BuffType)) BuffCache.BuffType.Add(buff.BuffType);
            }
        }
        
        try
        {
            await File.WriteAllTextAsync(BuffCachePath!, jsonUtil.Serialize(BuffCache, true));
        }
        catch (Exception e)
        {
            localLog.LocalLogMsg(LocalLogType.Warn, $"保存BuffCache(效果缓存)时出现问题: {e.Message}");
        }
        
        localLog.LocalLogMsg(LocalLogType.Info, "已整理BuffCache(效果缓存)");
    }

    
    /// <summary>
    /// 整理子弹数据(只会在首次加载模组时加载一次)
    /// </summary>
    private async Task OrganizeAmmo()
    {
        if (AmmoCache is not null) return;
        if (Path.Exists(AmmoCachePath))
        {
            try
            {
                AmmoCache = await jsonUtil.DeserializeFromFileAsync<AmmoCache>(AmmoCachePath!);
                (int ? countAmmoSfx, 
                    int? countCaliber, 
                    int? countCasingSounds,
                    int? countAmmoType) = (AmmoCache?.AmmoSfx.Count, 
                    AmmoCache?.Caliber.Count, 
                    AmmoCache?.CasingSounds.Count,
                    AmmoCache?.AmmoType.Count);
                if (countAmmoSfx > 0 && countCaliber > 0 && countCasingSounds > 0 && countAmmoType > 0)
                {
                    localLog.LocalLogMsg(LocalLogType.Info,
                        $"子弹缓存加载成功 - 射击音效: {countAmmoSfx.ToString() ?? "0"} 个, " +
                        $"口径: {countCaliber.ToString() ?? "0"} 个, " +
                        $"弹壳音效: {countCasingSounds.Value.ToString()} 个" +
                        $"子弹类型: {countAmmoType.ToString() ?? "0"} 个");
                    return;
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Warn, $"加载AmmoCache(子弹缓存)时出现问题: {e.Message}");
            }
        }

        AmmoCache = new AmmoCache();
        foreach (TemplateItem templateItem in databaseService.GetTemplates().Items.Values)
        {
            if (templateItem is { Properties.AmmoSfx: not null })
            {
                AmmoCache.AmmoSfx.Add(templateItem.Properties.AmmoSfx);
            }
            if (templateItem is { Properties.Caliber: not null })
            {
                AmmoCache.Caliber.Add(templateItem.Properties.Caliber);
            }
            if (templateItem is { Properties.CasingSounds: not null })
            {
                AmmoCache.CasingSounds.Add(templateItem.Properties.CasingSounds);
            }
            if (templateItem is { Properties.AmmoType: not null })
            {
                AmmoCache.AmmoType.Add(templateItem.Properties.AmmoType);
            }
        }
        
        try
        {
            await File.WriteAllTextAsync(AmmoCachePath!, jsonUtil.Serialize(AmmoCache, true));
        }
        catch (Exception e)
        {
            localLog.LocalLogMsg(LocalLogType.Warn, $"保存AmmoCache(子弹缓存)时出现问题: {e.Message}");
        }
        
        localLog.LocalLogMsg(LocalLogType.Info, "已整理AmmoCache(子弹缓存)");
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