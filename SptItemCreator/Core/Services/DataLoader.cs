using System.Diagnostics;
using System.Text.Json;
using JetBrains.Annotations;
using SptItemCreator.Models.Items;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Core.Enums;

namespace SptItemCreator.Core.Services;

[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PostDBModLoader + 1)]
[UsedImplicitly]
public sealed class DataLoader(
        JsonUtil jsonUtil,
        ItemHelper itemHelper,
        ConfigService configService,
        ISptLogger<DataLoader> sptLogger,
        DatabaseService databaseService
    ): IOnLoad
{
    private static string? _modName;
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
        AbstractInfo.ItemHelper ??= itemHelper;
        AbstractNewItem.DatabaseService ??= databaseService;
        _modName ??= configService.ModMetadata?.Name ?? "SptItemCreator";
        _jsonUtil  ??= jsonUtil;
        _configService ??= configService;
        if (LocalLog.DataFolderPath == null)
        {
            LocalLog.Logger.Error($"数据文件路径为空\n\t堆栈: {LocalLog.GetCurrentStackTrace()}");
            return Task.CompletedTask;
        }
        
        
        List<string> foundFiles = [];
        List<string> jumpFolderPath = [];
        List<string> jumpFilePath = [];
        
        TraverseForSicFiles(LocalLog.DataFolderPath, foundFiles, jumpFolderPath, jumpFilePath);
        
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

            LocalLog.Logger.Debug(message);
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
                LocalLog.Logger.Debug($"已加载新物品({(newItemBase.Enable ?? false ? "已" : "未")}启用) Id{newItemBase.BaseInfo.Id}({newItemBase.BaseInfo.Name}, @{newItemBase.BaseInfo.Author}) \t License = {newItemBase.BaseInfo.License} \t Path = {file}");
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
                        LocalLog.Logger.Error($"在分类新物品数据\"{file}\"类型时出现问题: `baseInfo.type` (当前为: {newItemBase.BaseInfo.Type}) 不存在或不合法 \n\t > Path = {file}");
                        break;
                }
            }
            catch (Exception e)
            {
                LocalLog.Logger.Error($"在反序列化\"{file}\"时出现问题: {e.Message}", e);
            }
        }
        
        LocalLog.Logger.Info($"已处理{foundFiles.Count}条sic文件");
        
        // 验证必需物品ID
        ValidateRequiredItemIds();
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 移除 JSONC 文本中的注释（单行 // 和多行 /* */）
    /// 使用状态机正确处理字符串内的注释标记和转义字符
    /// </summary>
    /// <param name="jsonc">JSONC 格式的文本</param>
    /// <returns>移除注释后的 JSON 文本</returns>
    public static string StripJsoncComments(string jsonc)
    {
        var result = new System.Text.StringBuilder(jsonc.Length);
        var state = ParseState.Normal;
        
        for (var i = 0; i < jsonc.Length; i++)
        {
            char current = jsonc[i];
            char next = i + 1 < jsonc.Length ? jsonc[i + 1] : '\0';
            
            switch (state)
            {
                case ParseState.Normal:
                    switch (current)
                    {
                        case '"':
                            state = ParseState.InString;
                            result.Append(current);
                            break;
                        case '/' when next == '/':
                            state = ParseState.InSingleLineComment;
                            i++; // 跳过第二个 '/'
                            break;
                        case '/' when next == '*':
                            state = ParseState.InMultiLineComment;
                            i++; // 跳过 '*'
                            break;
                        default:
                            result.Append(current);
                            break;
                    }
                    break;
                    
                case ParseState.InString:
                    state = current switch
                    {
                        '\\' => ParseState.InEscape,
                        '"' => ParseState.Normal,
                        _ => state
                    };

                    result.Append(current);
                    break;
                    
                case ParseState.InEscape:
                    // 任何转义字符后都回到字符串状态
                    state = ParseState.InString;
                    result.Append(current);
                    break;
                    
                case ParseState.InSingleLineComment:
                    if (current == '\n')
                    {
                        state = ParseState.Normal;
                        result.Append(current); // 保留换行符
                    }
                    // 其他注释内容跳过
                    break;
                    
                case ParseState.InMultiLineComment:
                    if (current == '*' && next == '/')
                    {
                        state = ParseState.Normal;
                        i++; // 跳过 '/'
                    }
                    // 其他注释内容跳过
                    break;
            }
        }
        
        return result.ToString();
    }
    
    private enum ParseState
    {
        Normal,
        InString,
        InEscape,
        InSingleLineComment,
        InMultiLineComment
    }
    
    // 根据 TypeIdentifier 在反序列化时直接创建正确的类型
    public static T? DeserializeBasedOnType<T>(string json) where T: NewItemCommon
    {
        // 预处理：移除 JSONC 注释
        string cleanJson = StripJsoncComments(json);
        
        using JsonDocument doc = JsonDocument.Parse(cleanJson);
        string typeIdentifier = doc.RootElement.GetProperty("$type").GetString()!;
        
        if (_jsonUtil != null)
        {
            if (typeIdentifier == SicType.Common)
                return (T?)_jsonUtil.Deserialize<NewItemCommon>(cleanJson);
            if (typeIdentifier == SicType.DrinkOrFood)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemDrinkOrFood>(cleanJson);
            if (typeIdentifier == SicType.Medical)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemMedical>(cleanJson);
            if (typeIdentifier == SicType.Ammo)
                return (T?)(NewItemCommon?)_jsonUtil.Deserialize<NewItemAmmo>(cleanJson);
            return _jsonUtil.Deserialize<T>(cleanJson);
        }
        LocalLog.Logger.Warn("解析数据时出现问题: _jsonUtil未初始化");
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
                LocalLog.Logger.Warn($"达到最大递归深度，停止遍历: {path}");
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
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            LocalLog.Logger.Error($"无权访问目录: {path}", unauthorizedAccessException);
        }
        catch (DirectoryNotFoundException directoryNotFoundException)
        {
            LocalLog.Logger.Error($"目录不存在: {path}", directoryNotFoundException);
        }
        catch (Exception ex)
        {
            LocalLog.Logger.Error($"处理目录 {path} 时出错: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 验证必需物品ID是否存在且已启用
    /// 收集所有问题后汇总输出错误日志
    /// </summary>
    private void ValidateRequiredItemIds()
    {
        var stopwatch = Stopwatch.StartNew();
        
        List<string>? requiredIds = _configService?.Config?.RequiredItemIds;
        if (requiredIds == null || requiredIds.Count == 0)
        {
            return;
        }

        // 构建所有已加载物品的ID映射: ID -> (是否启用, 文件路径)
        var itemMap = new Dictionary<string, (bool Enabled, string Path)>();
        
        // 遍历所有物品字典
        foreach (NewItemCommon item in NewItemCommon.Values)
        {
            if (item.BaseInfo?.Id != null)
            {
                itemMap[item.BaseInfo.Id] = (item.Enable ?? false, item.ItemPath ?? "未知路径");
            }
        }
        
        foreach (NewItemDrinkOrFood item in NewItemDrinkOrDrugs.Values)
        {
            if (item.BaseInfo?.Id != null)
            {
                itemMap[item.BaseInfo.Id] = (item.Enable ?? false, item.ItemPath ?? "未知路径");
            }
        }
        
        foreach (NewItemMedical item in NewItemMedical.Values)
        {
            if (item.BaseInfo?.Id != null)
            {
                itemMap[item.BaseInfo.Id] = (item.Enable ?? false, item.ItemPath ?? "未知路径");
            }
        }
        
        foreach (NewItemAmmo item in NewItemAmmo.Values)
        {
            if (item.BaseInfo?.Id != null)
            {
                itemMap[item.BaseInfo.Id] = (item.Enable ?? false, item.ItemPath ?? "未知路径");
            }
        }

        // 收集问题
        var missingIds = new List<string>();
        var disabledItems = new List<(string Id, string Path)>();

        foreach (string requiredId in requiredIds)
        {
            if (!itemMap.TryGetValue(requiredId, out (bool Enabled, string Path) itemInfo))
            {
                missingIds.Add(requiredId);
            }
            else if (!itemInfo.Enabled)
            {
                disabledItems.Add((requiredId, itemInfo.Path));
            }
        }

        // 汇总输出
        int totalIssues = missingIds.Count + disabledItems.Count;
        if (totalIssues > 0)
        {
            var message = $"要求必需物品验证失败，共 {totalIssues} 个问题:";
            
            if (missingIds.Count > 0)
            {
                message += $"\n  缺失的物品ID ({missingIds.Count}):";
                foreach (string id in missingIds)
                {
                    message += $"\n    - {id}";
                }
            }
            
            if (disabledItems.Count > 0)
            {
                message += $"\n  未启用的物品ID ({disabledItems.Count}):";
                foreach ((string id, string path) in disabledItems)
                {
                    message += $"\n    - {id} (文件: {path})";
                }
            }
            
            stopwatch.Stop();

            message += "\n\n    你可以在修复问题后使用**游戏根目录/SPT/user/profiles/backups**的存档备份文件恢复存档";
            message += $"\n    验证耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms";
            
            sptLogger.Error(message);
            LocalLog.Logger.Error(message);
        }
        else
        {
            stopwatch.Stop();
            
            LocalLog.Logger.Debug($"必需物品验证通过，共验证 {requiredIds.Count} 个ID, 验证耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        }
    }
    
}