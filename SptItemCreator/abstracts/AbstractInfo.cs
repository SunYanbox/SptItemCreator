using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;

namespace SptItemCreator.abstracts;

/// <summary>
/// 抽象信息基类
/// </summary>
public abstract record AbstractInfo
{
    [JsonIgnore] public static LocalLog? LocalLog;
    [JsonIgnore] public static ItemHelper? ItemHelper;
    
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
            UpdateDatabaseService(databaseService);
    }
}