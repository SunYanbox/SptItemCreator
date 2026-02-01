using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Models;
using LogLevel = SPTarkov.Server.Core.Models.Spt.Logging.LogLevel;

namespace SptItemCreator.Services;

/// <summary> 本模组的所有配置 </summary>
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public sealed class ConfigService(
        JsonUtil jsonUtil,
        ModHelper modHelper,
        ModMetadata modMetadata,
        ISptLogger<ConfigService> logger
    ): IOnLoad
{
    private static JsonUtil? _jsonUtil;
    public string? ModFolderPath { get; private set; }
    public string? ModConfigPath { get; private set; }
    public string? ModDataPath { get; private set; }
    public ModMetadata? ModMetadata { get; private set; }
    public ModConfig? Config { get; private set; }

    /// <summary>
    /// 补全模组名称的SPT日志
    /// </summary>
    /// <param name="level">LogLevel.Info | LogLevel.Warn | LogLevel.Debug | LogLevel.Error</param>
    /// <param name="data">文本</param>
    /// <param name="ex">报错</param>
    public void SptLog(LogLevel level, string data, Exception? ex = null)
    {
        data = $"[{modMetadata.Name} {modMetadata.Version}] {data}";
        switch (level)
        {
            case LogLevel.Error:
                logger.Error(data, ex);
                break;
            case LogLevel.Warn:
                logger.Warning(data, ex);
                break;
            case LogLevel.Info:
                logger.Info(data, ex);
                break;
            case LogLevel.Debug:
                logger.Debug(data, ex);
                break;
            default:
                logger.Error(new ArgumentOutOfRangeException(nameof(level), level, null).ToString());
                break;
        }
    }

    public async Task OnLoad()
    {
        _jsonUtil ??= jsonUtil;
        ModFolderPath ??= modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        ModDataPath ??= Path.Combine(ModFolderPath, "data");
        Directory.CreateDirectory(ModDataPath);
        ModConfigPath ??= Path.Combine(ModDataPath, "config.json");
        ModMetadata ??= modMetadata;
        await LoadConfig();
    }

    public async Task LoadConfig()
    {
        try
        {
            var config = await _jsonUtil!.DeserializeFromFileAsync<ModConfig>(ModConfigPath!);
            Config = config ?? throw new Exception($"加载配置文件{ModConfigPath}的结果为null");
        }
        catch (Exception e)
        {
            SptLog(LogLevel.Error, $"配置加载失败: {e.Message}", e);
        }
    }

    public async Task SaveConfig()
    {
        try
        {
            string? config = _jsonUtil!.Serialize(ModConfigPath);
            if (string.IsNullOrEmpty(config)) throw new Exception("序列化配置的结果为null");
            await File.WriteAllTextAsync(ModConfigPath!, config);
        }
        catch (Exception e)
        {
            SptLog(LogLevel.Error, $"配置保存失败: {e.Message}", e);
        }
    }
}