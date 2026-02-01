using System.Text.Json;
using JetBrains.Annotations;
using SptItemCreator.NewItemClasses;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Abstracts;
using SptItemCreator.Enums;

namespace SptItemCreator.Services;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
[UsedImplicitly]
public sealed class DataLoader(
        LocalLog localLog,
        JsonUtil jsonUtil,
        ConfigService configService,
        ItemHelper itemHelper,
        DatabaseService databaseService
    ): IOnLoad
{
    private static string? _modName;
    private static LocalLog? _localLog;
    private static JsonUtil? _jsonUtil;
    private static ConfigService? _configService;
    /// <summary>
    /// 通用/默认创建物品接口
    /// </summary>
    public readonly Dictionary<string, NewItemCommon> NewItemCommon = new();
    /// <summary>
    /// 食物饮品创建物品接口
    /// </summary>
    public readonly Dictionary<string, NewItemDrinkOrFood> NewItemDrinkOrDrugs = new();
    /// <summary>
    /// 药品创建
    /// </summary>
    public readonly Dictionary<string, NewItemMedical> NewItemMedical = new();
    /// <summary>
    /// 弹药
    /// </summary>
    public readonly Dictionary<string, NewItemAmmo> NewItemAmmo = new();

    public Task OnLoad()
    {
        AbstractInfo.LocalLog ??= localLog;
        AbstractInfo.ItemHelper ??= itemHelper;
        AbstractNewItem.LocalLog ??= localLog;
        AbstractNewItem.DatabaseService ??= databaseService;
        _modName ??= configService.ModMetadata?.Name ?? "SptItemCreator";
        _jsonUtil  ??= jsonUtil;
        _configService ??= configService;
        _localLog = localLog;
        if (localLog.DataFolderPath == null)
        {
            localLog.LocalLogMsg(LocalLogType.Error, $"数据文件路径为空\n\t堆栈: {LocalLog.GetCurrentStackTrace()}");
            return Task.CompletedTask;
        }
        
        
        List<string> foundFiles = [];
        List<string> jumpFolderPath = [];
        List<string> jumpFilePath = [];
        
        TraverseForSicFiles(localLog.DataFolderPath, foundFiles, jumpFolderPath, jumpFilePath);
        
        if (jumpFilePath.Count + jumpFolderPath.Count >= 1)
        {
            var message = string.Empty;
            if (jumpFolderPath.Count > 0)
            {
                message += $"已跳过模板文件夹:\n\t - {string.Join("\n\t - ", jumpFolderPath)}";
                if (jumpFilePath.Count > 0)
                {
                    message += "\n\n";
                }
            }
            if (jumpFilePath.Count > 0)
            {
                message +=
                    $"已跳过模板文件:\n\t - {string.Join("\n\t - ", jumpFilePath)}";
            }

            _localLog.LocalLogMsg(LocalLogType.Debug, message);
        }
        
        foreach (string file in foundFiles)
        {
            try
            {
                var newItemBase = DeserializeBasedOnType<NewItemCommon>(File.ReadAllText(file));
                if (newItemBase == null) throw new Exception("反序列化的结果为null");
                if (newItemBase.BaseInfo == null) throw new Exception("反序列化后获取不到baseInfo字段");
                newItemBase.BaseInfo.ItemPath = file;
                if (newItemBase.BuffsInfo is not null) newItemBase.BuffsInfo.ItemPath = file;
                if (newItemBase.AttributeInfo is not null) newItemBase.AttributeInfo.ItemPath = file;
                newItemBase.ItemPath = file;
                newItemBase.Verify();
                localLog.LocalLogMsg(LocalLogType.Debug, $"已加载新物品 Id{newItemBase.BaseInfo.Id}({newItemBase.BaseInfo.Name}, @{newItemBase.BaseInfo.Author}) \t License = {newItemBase.BaseInfo.License} \t Path = {file}");
                // 类型转换
                switch (newItemBase.BaseInfo.Type)
                {
                    case SicType.Common: NewItemCommon.Add(file, newItemBase); break;
                    case SicType.DrinkOrFood:
                    {
                        var newItemDrinkOrFood = (newItemBase as NewItemDrinkOrFood)!;
                        if (newItemDrinkOrFood.DrinkFoodInfo is not null) newItemDrinkOrFood.DrinkFoodInfo.ItemPath = file;
                        NewItemDrinkOrDrugs.Add(file, newItemDrinkOrFood);
                        break;
                    }
                    case SicType.Medical:
                    {
                        var newItemMedical = (newItemBase as NewItemMedical)!;
                        if (newItemMedical.MedicalInfo is not null) newItemMedical.MedicalInfo.ItemPath = file;
                        NewItemMedical.Add(file, newItemMedical);
                        break;
                    }
                    case SicType.Ammo:
                    {
                        var newItemAmmo = (newItemBase as NewItemAmmo)!;
                        if (newItemAmmo.AmmoInfo is not null) newItemAmmo.AmmoInfo.ItemPath = file;
                        NewItemAmmo.Add(file, newItemAmmo);
                        break;
                    }
                    default: 
                        localLog.LocalLogMsg(LocalLogType.Error, $"在分类新物品数据\"{file}\"类型时出现问题: `baseInfo.type` (当前为: {newItemBase.BaseInfo.Type}) 不存在或不合法 \n\t > Path = {file}");
                        break;
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Error, $"在反序列化\"{file}\"时出现问题: {e.Message}");
            }
        }
        
        localLog.LocalLogMsg(LocalLogType.Info, $"已处理{foundFiles.Count}条sic文件");
        
        return Task.CompletedTask;
    }
    
    // 根据 TypeIdentifier 在反序列化时直接创建正确的类型
    public static T? DeserializeBasedOnType<T>(string json) where T: NewItemCommon
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        string typeIdentifier = doc.RootElement.GetProperty("$type").GetString()!;
        if (_jsonUtil != null)
        {
            if (typeIdentifier == SicType.Common)
                return (T?)_jsonUtil.Deserialize<NewItemCommon>(json);
            if (typeIdentifier == SicType.DrinkOrFood)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemDrinkOrFood>(json);
            if (typeIdentifier == SicType.Medical)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemMedical>(json);
            if (typeIdentifier == SicType.Ammo)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemAmmo>(json);
            return _jsonUtil.Deserialize<T>(json);
        }
        _localLog?.LocalLogMsg(LocalLogType.Warn, $"解析数据时出现问题: _jsonUtil未初始化");
        return null;
    }

    private static bool JumpFolderOrFile(string? name)
    {
        return name != null
               && (_configService?.Config?.IgnoreTemplateFiles ?? false)
               && (Path.GetFileName(name).Contains("Template") || Path.GetFileName(name).Contains("模板"));
    }

    /// <summary>
    /// 递归遍历, 获取新物品数据
    /// </summary>
    /// <param name="path">目录路径</param>
    /// <param name="results">结果列表</param>
    /// <param name="jumpFolderPath">跳过的文件夹路径列表</param>
    /// <param name="jumpFilePath">跳过的文件路径列表</param>
    /// <param name="currentDepth">当前递归深度（内部使用）</param>
    public static void TraverseForSicFiles(string path, List<string> results, List<string> jumpFolderPath, List<string> jumpFilePath, int currentDepth = 10)
    {
        if (JumpFolderOrFile(Path.GetDirectoryName(path)))
        {
            jumpFolderPath.Add(path);
            return;
        }
        try
        {
            // 检查递归深度
            if (currentDepth < 0)
            {
                if (_localLog != null) _localLog.LocalLogMsg(LocalLogType.Warn, $"达到最大递归深度，停止遍历: {path}");
                else Console.WriteLine($"[{_modName}] 达到最大递归深度，停止遍历: {path}");
                return;
            }
            
            // 递归遍历所有子目录
            foreach (string subDirectory in Directory.GetDirectories(path))
            {
                if (JumpFolderOrFile(Path.GetDirectoryName(subDirectory)))
                {
                    jumpFolderPath.Add(subDirectory);
                    continue;
                }
                TraverseForSicFiles(subDirectory, results, jumpFolderPath, jumpFilePath, currentDepth - 1);
            }

            // 遍历当前目录的所有文件
            foreach (string file in Directory.GetFiles(path))
            {
                if (JumpFolderOrFile(Path.GetFileName(file)))
                {
                    jumpFilePath.Add(file);
                    continue;
                }
                if (file.EndsWith(".sic") || file.EndsWith(".sic.json") || file.EndsWith(".sic.jsonc"))
                {
                    results.Add(file);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            if (_localLog != null) _localLog.LocalLogMsg(LocalLogType.Error, $"无权访问目录: {path}");
            else Console.WriteLine($"[{_modName}] 无权访问目录: {path}");
        }
        catch (DirectoryNotFoundException)
        {
            if (_localLog != null) _localLog.LocalLogMsg(LocalLogType.Error, $"目录不存在: {path}");
            else Console.WriteLine($"[{_modName}] 目录不存在: {path}");
        }
        catch (Exception ex)
        {
            if (_localLog != null) _localLog.LocalLogMsg(LocalLogType.Error, $"处理目录 {path} 时出错: {ex.Message}");
            else Console.WriteLine($"[{_modName}] 处理目录 {path} 时出错: {ex.Message}");
        }
    }
    
}