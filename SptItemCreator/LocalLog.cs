using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace SptItemCreator;

public enum LocalLogType
{
    Info,
    Warn,
    Debug,
    Error
}

public static class Constants
{
    public const string LogFolder = "logs";
    public const string DataFolder = "data";
}

/// <summary>
/// 封装本地化日志, 获取模组配置信息
/// </summary>
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class LocalLog(ModHelper modHelper, ISptLogger<LocalLog> logger): IOnLoad
{
    public string? DataFolderPath { get; set; }
    public string? LogFolderPath { get; set; }
    protected readonly Dictionary<LocalLogType, StreamWriter> LogWriters = new();
    protected readonly Lock LogLock = new();

    public bool TryCatch(string task, Func<bool> func)
    {
        try
        {
            bool result = func();
            logger.Debug($"[{GetModName()}]<{task}> 任务完成");
            return result;
        }
        catch (Exception e)
        {
            logger.Error($"[{GetModName()}]<{task}>: {e.Message}\n\t{e.StackTrace}");
            LocalLogMsg(LocalLogType.Error, $"{e.Message}\n\t{e.StackTrace}");
            return false;
        }
    }
    
    public Task OnLoad()
    {
        string pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

        TryCatch("初始化本地日志核心", () => InitLogCore(pathToMod) );
        
        LocalLogMsg(LocalLogType.Info, "----------------------------------------本地化日志加载完成----------------------------------------");
        LocalLogMsg(LocalLogType.Warn, "----------------------------------------本地化日志加载完成----------------------------------------");
        LocalLogMsg(LocalLogType.Error, "----------------------------------------本地化日志加载完成----------------------------------------");
        LocalLogMsg(LocalLogType.Debug, "----------------------------------------本地化日志加载完成----------------------------------------");
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// 初始化日志记录核心
    /// </summary>
    /// <param name="pathToMod">模组文件夹</param>
    /// <returns></returns>
    protected bool InitLogCore(string pathToMod)
    {
        const int maxLogFileSize = 10 * 1024 * 1024; // 10 MB
        LogFolderPath = Path.Combine(pathToMod, Constants.LogFolder);
        DataFolderPath = Path.Combine(pathToMod, Constants.DataFolder);

        TryCatch("创建日志文件夹", () =>
        {
            Directory.CreateDirectory(LogFolderPath);
            return true;
        });
        TryCatch("创建数据文件夹", () =>
        {
            Directory.CreateDirectory(DataFolderPath);
            return true;
        });
        
        string infoPath = Path.Combine(LogFolderPath, "info.log");
        string warnPath = Path.Combine(LogFolderPath, "warn.log");
        string debugPath = Path.Combine(LogFolderPath, "debug.log");
        string errorPath = Path.Combine(LogFolderPath, "error.log");

        TryCatch("日志过大检测", () =>
        {
            foreach (string filePath in new[] { infoPath, warnPath, debugPath, errorPath })
            {
                if (!File.Exists(filePath)) continue;
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > maxLogFileSize)
                {
                    TryCatch($"日志文件过大，删除并重建: {filePath}", () =>
                    {
                        File.Delete(filePath);
                        return true;
                    });
                }
            }
            return true;
        });
        
        TryCatch("注册Info日志写入流", () =>
        {
            LogWriters[LocalLogType.Info] =
                new StreamWriter(new FileStream(infoPath, FileMode.Append, FileAccess.Write));
            return true;
        });
        TryCatch("注册Warn日志写入流", () =>
        {
            LogWriters[LocalLogType.Warn] =
                new StreamWriter(new FileStream(warnPath, FileMode.Append, FileAccess.Write));
            return true;
        });
        TryCatch("注册Debug日志写入流", () =>
        {
            LogWriters[LocalLogType.Debug] =
                new StreamWriter(new FileStream(debugPath, FileMode.Append, FileAccess.Write));
            return true;
        });
        TryCatch("注册Error日志写入流", () =>
        {
            LogWriters[LocalLogType.Error] =
                new StreamWriter(new FileStream(errorPath, FileMode.Append, FileAccess.Write));
            return true;
        });
        return true;
    }

    /// <summary>
    /// 记录日志到文件中
    /// </summary>
    /// <param name="type">日志类型</param>
    /// <param name="message">日志消息</param>
    public void LocalLogMsg(LocalLogType type, string message)
    {
        if (LogWriters.TryGetValue(type, out StreamWriter? writer))
        {
            TryCatch("记录本地日志", () =>
            {
                lock (LogLock)
                {
                    using var sw = new StreamWriter(writer.BaseStream, writer.Encoding, 1024, true);
                    sw.AutoFlush = true;
                    message += type == LocalLogType.Debug ? "\n" : string.Empty;
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {type.ToString()} - {message}");
                    sw.Flush();
                }

                return true;
            });
        }
    }

    /// <summary>
    /// Hook一个任务函数, 跟踪函数的运行结果
    /// </summary>
    /// <param name="task">任务描述</param>
    /// <param name="func">任务闭包函数</param>
    /// <returns>任务执行情况</returns>
    public void LocalLogHook(string task, Func<Task> func)
    {
        Task? taskResult = null;
        LocalLogMsg(LocalLogType.Debug, $"<任务: {task}> 开始");
        try
        {
            Task result = taskResult = func();
            LocalLogMsg(LocalLogType.Debug, $"<任务: {task}> 执行结果: {result}({result.Id}, {result.CreationOptions}, 状态: {result.Status})");
            // return result;
        }
        catch (Exception e)
        {
            LocalLogMsg(LocalLogType.Error, $"<任务: {task}> {e.Message}\n\t{e.StackTrace}\n\t{taskResult}({taskResult?.Id}, {taskResult?.CreationOptions}, 状态: {taskResult?.Status})");
            // return Task.FromException<Exception>(e);
        }
    }
    
    public static string GetCurrentStackTrace()
    {
        return new StackTrace(true).ToString();
    }

    public static string GetModName() => "SptItemCreator";
    
    public static string ToStringExcludeNulls(object? obj)
    {
        if (obj == null) return "null";
        
        var builder = new StringBuilder();
        builder.Append($"{obj.GetType().Name} {{ ");
        
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);
        
        var nonNullProperties = properties.Select(p => new { 
            Property = p, 
            Value = p.GetValue(obj) 
        }).Where(x => x.Value != null).ToList();
        
        for (int i = 0; i < nonNullProperties.Count; i++)
        {
            if (i > 0) builder.Append(", ");
            
            var item = nonNullProperties[i];
            builder.Append(item.Property.Name);
            builder.Append(" = ");
            
            AppendEscapedValue(builder, item.Value);
        }
        
        builder.Append(" }");
        return builder.ToString();
    }

    private static void AppendEscapedValue(StringBuilder builder, object value)
    {
        if (value == null)
        {
            builder.Append("null");
            return;
        }

        Type valueType = value.GetType();
        
        // 优先处理集合类型
        if (value is IEnumerable enumerable && !(value is string))
        {
            AppendEscapedEnumerable(builder, enumerable);
            return;
        }

        switch (value)
        {
            case string str:
                AppendEscapedString(builder, str);
                break;
            case char c:
                AppendEscapedChar(builder, c);
                break;
            default:
                builder.Append(value);
                break;
        }
    }

    private static void AppendEscapedEnumerable(StringBuilder builder, IEnumerable enumerable)
    {
        if (enumerable == null)
        {
            builder.Append("null");
            return;
        }
        
        builder.Append('[');
        
        bool first = true;
        foreach (object? item in enumerable)
        {
            if (!first) builder.Append(", ");
            AppendEscapedValue(builder, item);
            first = false;
        }
        
        builder.Append(']');
    }

    // AppendEscapedString 和 AppendEscapedChar 方法保持不变
    private static void AppendEscapedString(StringBuilder builder, string str)
    {
        builder.Append('"');
        
        foreach (char c in str)
        {
            switch (c)
            {
                case '\\': builder.Append("\\\\"); break;
                case '\"': builder.Append("\\\""); break;
                case '\'': builder.Append("\\'"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                case '\b': builder.Append("\\b"); break;
                case '\f': builder.Append("\\f"); break;
                case '\0': builder.Append("\\0"); break;
                default:
                    if (char.IsControl(c))
                    {
                        builder.Append($"\\u{(int)c:x4}");
                    }
                    else
                    {
                        builder.Append(c);
                    }
                    break;
            }
        }
        
        builder.Append('"');
    }

    private static void AppendEscapedChar(StringBuilder builder, char c)
    {
        builder.Append('\'');
        
        switch (c)
        {
            case '\\': builder.Append("\\\\"); break;
            case '\'': builder.Append("\\'"); break;
            case '\n': builder.Append("\\n"); break;
            case '\r': builder.Append("\\r"); break;
            case '\t': builder.Append("\\t"); break;
            case '\b': builder.Append("\\b"); break;
            case '\f': builder.Append("\\f"); break;
            case '\0': builder.Append("\\0"); break;
            default:
                if (char.IsControl(c))
                {
                    builder.Append($"\\u{(int)c:x4}");
                }
                else
                {
                    builder.Append(c);
                }
                break;
        }
        
        builder.Append('\'');
    }
}