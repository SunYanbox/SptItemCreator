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
}