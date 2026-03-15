using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SuntionCore.Services.LogUtils;

namespace SptItemCreator.Core.Services;

/// <summary>
/// 封装本地化日志, 获取模组配置信息
/// </summary>
[Injectable(InjectionType = InjectionType.Singleton, TypePriority = OnLoadOrder.PreSptModLoader + 1)]
public class LocalLog(ModHelper modHelper): IOnLoad
{
    public const string DataFolder = "data";
    
    public static readonly ModLogger Logger
        = ModLogger.GetOrCreateLogger("SptItemCreator");
    
    public static string? DataFolderPath { get; set; }

    public static bool TryCatch(string task, Func<bool> func)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            bool result = func();
            Logger.Info($"<{task}>: {result} 耗时: {stopwatch.Elapsed.TotalMilliseconds:F3}ms");
            return result;
        }
        catch (Exception e)
        {
            var errorMessage = $"执行任务<{task}>时出现错误: {e.Message} 耗时: {stopwatch.Elapsed.TotalMilliseconds:F3}ms";
            Logger.Error(errorMessage, e);
            return false;
        }
    }
    
    public Task OnLoad()
    {
        string pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        DataFolderPath = Path.Combine(pathToMod, DataFolder);
        
        TryCatch("创建数据文件夹", () =>
        {
            Directory.CreateDirectory(DataFolderPath);
            return true;
        });
        return Task.CompletedTask;
    }

    public static string GetCurrentStackTrace()
    {
        return new StackTrace(true).ToString();
    }

    #region 对象转字符串

    public static string ToStringExcludeNulls(object? obj)
    {
        if (obj == null) return "null";
        
        var builder = new StringBuilder();
        builder.Append($"{obj.GetType().Name} {{ ");
        
        IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);
        
        var nonNullProperties = properties.Select(p => new { 
            Property = p, 
            Value = p.GetValue(obj) 
        }).Where(x => x.Value != null).ToList();
        
        for (var i = 0; i < nonNullProperties.Count; i++)
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

    private static void AppendEscapedValue(StringBuilder builder, object? value)
    {
        if (value == null)
        {
            builder.Append("null");
            return;
        }

        // Type valueType = value.GetType();
        
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

    private static void AppendEscapedEnumerable(StringBuilder builder, IEnumerable? enumerable)
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
            case '\\': builder.Append(@"\\"); break;
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

    #endregion
}