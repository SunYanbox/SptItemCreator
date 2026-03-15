using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Core.Services;

namespace SptItemCreator.Models.Abstracts;

/// <summary>
/// 抽象信息基类
/// </summary>
public abstract record AbstractInfo
{
    [JsonIgnore] public static ItemHelper? ItemHelper;
    [JsonIgnore] public string ItemPath { get; set; } = string.Empty;
    
    /// <summary>
    /// 封装更新TemplateItemProperties的逻辑
    /// </summary>
    /// <param name="properties"></param>
    public abstract void UpdateProperties(TemplateItemProperties properties);

    /// <summary>
    /// 更新DatabaseService
    /// </summary>
    /// <param name="databaseService"></param>
    public virtual void UpdateDatabaseService(DatabaseService databaseService) {}
    
    public override string ToString()
    {
        return $"{GetType().Name} {LocalLog.ToStringExcludeNulls(this).Replace("\n", "\\n")}";
    }

    /// <summary>
    /// 封装更新TemplateItemProperties和DatabaseService的逻辑
    /// </summary>
    /// <param name="properties"></param>
    /// <param name="databaseService"></param>
    public void Update(TemplateItemProperties properties, DatabaseService? databaseService = null)
    {
        UpdateProperties(properties);
        if (databaseService != null)
        {
            try
            {
                UpdateDatabaseService(databaseService);
            }
            catch (Exception e)
            {
                LocalLog.Logger.Warn($"物品更新数据库时出现问题: {e.Message}\n\tItemPath: {ItemPath}", e);
            }
        }
    }
}