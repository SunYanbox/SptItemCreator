using System.Text;
using SptItemCreator.Models.Abstracts;

namespace SptItemCreator.Models.Validators;

public class ErrorCollector(INewItem newItem): IErrorCollector
{
    private string ItemPath => newItem.ItemPath;
    private readonly Dictionary<string, List<string>> _errors = new();
    
    public void AddError(string category, string errorMessage)
    {
        if (!_errors.TryGetValue(category, out List<string>? value))
        {
            value = [];
            _errors[category] = value;
        }

        value.Add(errorMessage);
    }

    public bool IsEmpty()
    {
        if (_errors.Count == 0) return true;
        if (_errors.Values.Any(v => v.Count > 0)) return false;
        return true;
    }

    public string ErrorsToString()
    {
        StringBuilder stringBuilder = new();

        if (IsEmpty())
        {
            stringBuilder.Append($"ErrorCollector(Path={ItemPath}, Empty)");
        }
        else
        {
            stringBuilder.Append($"ErrorCollector(Path={ItemPath}, ");
            stringBuilder.AppendJoin(", ", _errors
                .Where(kv => kv.Value.Count > 0)
                .Select(kv => $"{kv.Key}: {{ {string.Join(", ", kv.Value)} }}"));
            stringBuilder.Append(')');
        }
        
        return stringBuilder.ToString();
    }
}