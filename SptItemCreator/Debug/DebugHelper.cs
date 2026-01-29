using System.Reflection;
using System.Text;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;
using Path = System.IO.Path;

namespace SptItemCreator.Debug;

[Injectable]
public class DebugHelper(
    LocalLog localLog,
    ModHelper modHelper,
    ItemHelper itemHelper,
    JsonUtil jsonUtil,
    DatabaseService databaseService,
    ISptLogger<DebugHelper> logger): IOnLoad
{
    /// <summary>
    /// 整理所有类型的物品数据
    /// </summary>
    public void OrganizeItemTypeData()
    {
        // 调试用, 整理所有类型的物品数据
        localLog.LocalLogMsg(LocalLogType.Warn, "<OrganizeItemTypeData>如果你发现服务端卡死或者看到这条消息, 这是由于调试用任务未被正确关闭, 请将包含该提示的日志告知作者");
        string pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        string classificationPath = Path.Combine(pathToMod, "Classification");
        Directory.CreateDirectory(classificationPath);
        foreach ((string name, MongoId mongoId) in ItemType.TypesDict)
        {
            try
            {
                List<MongoId> tpls = itemHelper.GetItemTplsOfBaseType(mongoId.ToString()).ToList();
                List<TemplateItem> items = [];
                foreach (MongoId tpl in tpls)
                {
                    (bool tag, TemplateItem? item) = itemHelper.GetItem(tpl);
                    if (tag)
                    {
                        if (item != null) items.Add(item);
                    }
                    
                    string? content = jsonUtil.Serialize(items, true);
                    if (!string.IsNullOrEmpty(content))
                    {
                        File.WriteAllText(Path.Combine(classificationPath, $"{name}.json"), content);
                    }
                }
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Error, $"整理{name}类型的物品时出现错误: {e.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// 整理所有类型的物品ParentId数据
    /// </summary>
    public void OrganizeParentId()
    {
        long start = DateTime.UtcNow.Ticks;
        localLog.LocalLogMsg(LocalLogType.Warn, "<OrganizeParentId>如果你发现服务端卡死或者看到这条消息, 这是由于调试用任务未被正确关闭, 请将包含该提示的日志告知作者");
        Dictionary<MongoId, TemplateItem> itemTemplate = databaseService.GetTemplates().Items;
        Dictionary<string, string> localeCh = databaseService.GetLocales().Global["ch"].Value!;
        // 使用更合适的容量初始化，减少扩容
        Dictionary<MongoId, string> parentIdLocal = new(50);
        Dictionary<MongoId, int> parentIdCount = new(50);
        var nullCount = 0;
        var errorLength = 0;
        
        foreach (TemplateItem templateValue in itemTemplate.Values)
        {
            var parentStr = templateValue.Parent.ToString();
            int parentLength = parentStr.Length;
            
            if (parentLength == 0)
            {
                nullCount++;
                continue;
            }

            if (parentLength != 24)
            {
                errorLength++;
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
                bool tag = true;
                // 不同parentId有不同的local
                foreach (string variable in new List<string>()
                         {
                             $"APCFilter/{parentEnName}",
                             $"EWishlistGroup/{parentEnName}",
                             $"hideout/{parentEnName}",
                             parentEnName
                         })
                {
                    if (localeCh.TryGetValue(variable, out string? localParentIdName))
                    {
                        if (tag) localName += " | ";
                        localName += $" {localParentIdName}";
                        tag = false;
                    }
                }
                if (tag) localName += "     |";
            }
    
            // 使用Add方法，避免TryAdd的开销（因为我们已经检查过不存在）
            parentIdLocal.Add(parentId, localName);
            parentIdCount.Add(parentId, 1);
        }
        
        // markdown格式化
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("| ParentId | ParentId对应物品的名称 | ParentId在assets/database/locales/中的名称 | 使用的频数 |\n|:--------:|:--------:|:--------:|:--------:|");
        foreach (MongoId parentId in parentIdLocal.Keys)
        {
            string msg = $"| {parentId} | {parentIdLocal[parentId]} | {parentIdCount[parentId]} |";
            stringBuilder.AppendLine(msg);
        }
        long end = DateTime.UtcNow.Ticks;
        stringBuilder.AppendLine($"- 有{nullCount}条空ParentId, 有{errorLength}条错误长度的ParentId, 总计耗时: {(end - start) / 10_000_000.0:F3}s");
        stringBuilder.AppendLine();
        logger.Warning(stringBuilder.ToString());
    }

    public Task OnLoad()
    {
        // OrganizeParentId();
        
        return Task.CompletedTask;
    }
}