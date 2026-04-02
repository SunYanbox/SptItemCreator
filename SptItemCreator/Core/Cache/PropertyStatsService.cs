using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Core.Services;
using HashUtil = SuntionCore.Services.HashUtils.HashUtil;

namespace SptItemCreator.Core.Cache;

[Injectable(InjectionType.Singleton)]
public class PropertyStatsService(
    JsonUtil jsonUtil, 
    ISptLogger<PropertyStatsService> sptLogger,
    ItemHelper itemHelper, 
    DatabaseService databaseService,
    ConfigService configService): IOnLoad
{
    private const string HashFileName = "hash.json";
    public const string CacheFolderName = "StatsCache";
    /// <summary> 统计器 </summary>
    public readonly Dictionary<string, StatsHandler> StatsHandlers = new();
    /// <summary> 缓存写入文件时的哈希, 避免频繁写入 </summary>
    public Dictionary<string, string> StatsFileCacheHash = new();
    public string? CacheFolderPath;
    public string? CacheHashFilePath;
    
    public Task OnLoad()
    {
        StatsHandler.ItemHelper ??= itemHelper;
        StatsHandler.DatabaseService ??= databaseService;
        
        CacheFolderPath = Path.Combine(LocalLog.ModFolder ?? "", CacheFolderName);
        CacheHashFilePath = Path.Combine(CacheFolderPath, HashFileName);
        
        Directory.CreateDirectory(CacheFolderPath);

        // 检查配置，决定是否跳过统计
        ModConfig? config = configService.Config;
        
        if (config is null)
        {
            LocalLog.Logger.Warn("[PropertyStatsService] 在本服务加载时, 无法获取到本应该早已加载的配置服务的配置信息, 缓存构建已跳过");
            sptLogger.Warning("[SptItemCreator.PropertyStatsService] When loading this service, the configuration information from the configuration service, which should have been loaded earlier, could not be obtained, and cache construction has been skipped.");
            return Task.CompletedTask;
        }

        if (config.AlwaysUpdateCache == true)
        {
            LocalLog.Logger.Debug("[PropertyStatsService] AlwaysUpdateCache=true，执行哈希检查更新");
        }
        else if (config.CacheInitialized == true)
        {
            LocalLog.Logger.Debug("[PropertyStatsService] 已跳过所有统计计算");
            return Task.CompletedTask;
        }
        else
        {
            LocalLog.Logger.Debug("[PropertyStatsService] 首次运行，执行统计计算");
        }

        Dictionary<string, MongoId> baseClassesDict = null!;

        HashSet<MongoId> baseClassesValues = null!;

        LocalLog.TryCatch("[PropertyStatsService] 反射获取所有BaseClasses类型", () =>
        {
            baseClassesDict = typeof(BaseClasses)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(f => f.FieldType == typeof(MongoId))
                .ToDictionary(
                    f => f.Name,
                    f => (MongoId)f.GetValue(null)!
                );
            baseClassesValues = baseClassesDict.Values.ToHashSet();
            LocalLog.Logger.Debug($"获取到BaseClasses(共{baseClassesValues.Count}个): [{string.Join(", ", baseClassesDict.Keys)}]");
            return true;
        });
        
        if (baseClassesDict is null || baseClassesValues is null)
            throw new ArgumentNullException(nameof(baseClassesDict) + " || " + nameof(baseClassesValues));

        LocalLog.TryCatch("[PropertyStatsService] 加载已统计缓存", () =>
        {
            List<string> successLoad = [];

            if (Path.Exists(CacheHashFilePath))
            {
                try
                {
                    StatsFileCacheHash = jsonUtil.DeserializeFromFile<Dictionary<string, string>>(CacheHashFilePath) ??
                                         new Dictionary<string, string>();
                }
                catch (Exception e)
                {
                    LocalLog.Logger.Error($"加载缓存的哈希值时出现错误({CacheHashFilePath})", e);
                }
            }
            
            List<string> files = Directory.EnumerateFiles(CacheFolderPath, "*.json", SearchOption.AllDirectories)
                .Where(file => Path.GetFileName(file) != HashFileName)
                .ToList();
            
            StatsHandler?[] results = Task.WhenAll(files.Select(async filePath =>
            {
                try
                {
                    var statsHandler = await jsonUtil.DeserializeFromFileAsync<StatsHandler>(filePath);
                    var errors = new List<string>();

                    if (statsHandler is null)
                    {
                        errors.Add("statsHandler 为空");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(statsHandler.CacheName))
                            errors.Add("CacheName 为空或null");

                        if (statsHandler.StatisticalData.Count == 0)
                            errors.Add("StatisticalData 为空");

                        if (!baseClassesValues.Contains(statsHandler.HandleBaseClasses))
                            errors.Add($"HandleBaseClasses '{statsHandler.HandleBaseClasses}' 不在允许的 baseClassesValues 中");

                        if (!string.IsNullOrEmpty(statsHandler.CacheName) && baseClassesDict.GetValueOrDefault(statsHandler.CacheName) != statsHandler.HandleBaseClasses)
                            errors.Add($"CacheName '{statsHandler.CacheName}' 对应的 BaseClasses 与 HandleBaseClasses '{statsHandler.HandleBaseClasses}' 不匹配");
                    }

                    if (errors.Count == 0)
                    {
                        return statsHandler;
                    }

                    LocalLog.Logger.Error($"加载位于缓存路径下的{filePath}时出现问题: {string.Join("; ", errors)}");
                    return null;
                }
                catch (Exception e)
                {
                    LocalLog.Logger.Error($"加载位于缓存路径下的{filePath}时出现问题", e);
                    return null;
                }
            })).Result;
            
            foreach (StatsHandler? statsHandler in results.Where(r => r is not null))
            {
                successLoad.Add(statsHandler!.CacheName!);
                StatsHandlers[statsHandler.CacheName!] = statsHandler;
            }
            
            if (successLoad.Count > 0)
            {
                LocalLog.Logger.Debug($"已加载{successLoad.Count}条统计数据: [{string.Join(", ", new HashSet<string>(successLoad))}]");
            }

            return true;
        });
        
        LocalLog.TryCatch("[PropertyStatsService] 统计数据", () =>
        {
            StringBuilder stringBuilder = new();
            foreach ((string typeName, MongoId baseClasses) in baseClassesDict)
            {
                if (!StatsHandlers.TryGetValue(typeName, out StatsHandler? handler))
                {
                    handler = new StatsHandler
                    {
                        CacheName = typeName,
                        HandleBaseClasses = baseClasses,
                        SavePath = Path.Combine(CacheFolderPath, $"{typeName}.json")
                    };
                    StatsHandlers[typeName] = handler;
                }

                stringBuilder.AppendLine("\t> " + handler.StatsItems());
            }
            LocalLog.Logger.Debug(stringBuilder.ToString());
            return true;
        });
        
        LocalLog.TryCatch("[PropertyStatsService] 保存数据", () =>
        {
            StringBuilder stringBuilder = new();
            int total = StatsHandlers.Count;
            stringBuilder.Append($"保存{total}条统计成功率: ");
            int success = 0, emptyData = 0, equalFile = 0;
            
            // 用于安全写入数据
            var keyValueChannel = Channel.CreateBounded<(string name, string hashValue)>(
                    new BoundedChannelOptions(total)
                );
            
            Task.WaitAll(StatsHandlers.Values.Select(async statsHandler =>
            {
                try
                {
                    if (statsHandler.StatisticalData.Count == 0)
                    {
                        Interlocked.Increment(ref emptyData);
                        return;
                    }
        
                    statsHandler.SavePath ??= Path.Combine(CacheFolderPath, $"{statsHandler.CacheName}.json");
                    string? json = jsonUtil.Serialize(statsHandler, true);
                    if (json is null)
                        throw new JsonException($"序列化{statsHandler.GetType().Name}({statsHandler.CacheName})的结果为空");
                    string jsonHash = HashUtil.Hash(json);
                    if (jsonHash != StatsFileCacheHash!.GetValueOrDefault(statsHandler.CacheName))
                    {
                        await File.WriteAllTextAsync(statsHandler.SavePath, json);
                        Interlocked.Increment(ref success);
                        await keyValueChannel.Writer.WriteAsync((name: statsHandler.CacheName, hashValue: jsonHash)!);
                    }
                    else
                    {
                        Interlocked.Increment(ref equalFile);
                    }
                    
                }
                catch (Exception e)
                {
                    var msg = $"保存统计数据{statsHandler.CacheName}到路径{statsHandler.SavePath}时出现错误";
                    lock (stringBuilder)
                    {
                        LocalLog.Logger.Error(msg, e);
                    }
                }
            }));
            
            keyValueChannel.Writer.Complete();

            Task.Run(async () =>
            {
                await foreach ((string cacheName, string hashValue) in keyValueChannel.Reader.ReadAllAsync())
                {
                    StatsFileCacheHash[cacheName] = hashValue;
                }
                
                await File.WriteAllTextAsync(CacheHashFilePath, jsonUtil.Serialize(StatsFileCacheHash, true));
            }).Wait();
            
            stringBuilder.AppendLine($"{(double)(success + emptyData + equalFile) / total:P4}" +
                                     $"((成功: {success}, 跳过空数据: {emptyData}, 跳过与原文件相等: {equalFile})/总共: {total})");
            LocalLog.Logger.Debug(stringBuilder.ToString());
            
            // 更新配置：标记缓存已初始化
            if (config?.CacheInitialized != true)
            {
                config ??= new ModConfig();
                config.CacheInitialized = true;
                configService.SaveConfig().Wait();
                LocalLog.Logger.Info("[PropertyStatsService] 缓存初始化完成，已更新配置 CacheInitialized=true");
            }
            
            return true;
        });
        
        return Task.CompletedTask;
    }
}