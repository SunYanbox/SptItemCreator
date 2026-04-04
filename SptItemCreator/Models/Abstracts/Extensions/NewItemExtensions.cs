using System.Text;

namespace SptItemCreator.Models.Abstracts.Extensions;

public static class NewItemExtensions
{
    public static string ToStringWithStatus(this INewItem item)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append($"新物品({(item.Enable ?? false ? "已" : "未")}启用) ");
        if (item.BaseInfo is not null)
        {
            stringBuilder.Append($"Id{item.BaseInfo.Id}({item.BaseInfo.Name} @{item.BaseInfo.Author ?? "佚名"} / {item.BaseInfo.License ?? "MIT"}) ");
        }

        if (item.NeedValidator.Length > 0)
        {
            stringBuilder.Append($"非空信息({string.Join(", ", item.NeedValidator.Select(x => x.GetType().Name))}) ");
        }
        else
        {
            stringBuilder.Append("非空信息(无) ");
        }

        if (string.IsNullOrEmpty(item.ItemPath) || !File.Exists(item.ItemPath))
        {
            stringBuilder.Append("Path(无合法路径)");
        }
        else
        {
            stringBuilder.Append($"Path({item.ItemPath})");
        }
        
        return stringBuilder.ToString();
    }

    public static string ToIdNameString(this INewItem item) =>
        item.BaseInfo is null
            ? $"{item.ItemPath}(BaseInfo is null)"
            : $"{item.BaseInfo.Name}({item.BaseInfo.Id})";
}