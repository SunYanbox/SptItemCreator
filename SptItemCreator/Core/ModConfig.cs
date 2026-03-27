using System.Text.Json.Serialization;

namespace SptItemCreator.Core;

public class ModConfig
{
    /// <summary>
    /// 忽略模板文件
    /// 忽略名称中带有"模板", "Template"的所有文件与文件夹下的文件
    /// </summary>
    [JsonPropertyName("ignoreTemplateFiles")]
    public bool? IgnoreTemplateFiles { get; set; } = true;

    /// <summary>
    /// 缓存是否已初始化
    /// 首次运行时为 false，统计完成后自动设为 true
    /// 设为 true 时，后续启动将跳过统计计算
    /// </summary>
    [JsonPropertyName("cacheInitialized")]
    public bool? CacheInitialized { get; set; } = false;

    /// <summary>
    /// 是否始终更新缓存
    /// 设为 true 时，无论 cacheInitialized 状态如何，都会执行哈希检查并更新变化的缓存
    /// 适用于物品数据大量改变后需要重新构建缓存的场景
    /// </summary>
    [JsonPropertyName("alwaysUpdateCache")]
    public bool? AlwaysUpdateCache { get; set; } = false;
}