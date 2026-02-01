using System.Text.Json.Serialization;
using SptItemCreator.Enums;

namespace SptItemCreator.Models;

public class ModConfig
{
    /// <summary>
    /// 同步日志
    /// 在记录本地日志时, 同步将Info, Warn, Error级别的日志输出到SPT服务端控制台
    /// </summary>
    [JsonPropertyName("synchronousLogging")]
    public bool? SynchronousLogging { get; set; } = false;
    
    /// <summary>
    /// 本地日志文件最大字节数
    /// 1MB = 1024KB = 1024*1024B
    /// </summary>
    [JsonPropertyName("localFilesMaximumBytes")]
    public long? LocalFilesMaximumBytes { get; set; } = Default.LocalFilesMaximumBytes;

    /// <summary>
    /// 忽略模板文件
    /// 忽略名称中带有"模板", "Template"的所有文件与文件夹下的文件
    /// </summary>
    [JsonPropertyName("ignoreTemplateFiles")]
    public bool? IgnoreTemplateFiles { get; set; } = true;
}