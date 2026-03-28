namespace SptItemCreator.Models.Abstracts;

public interface IValidator
{
    /// <summary> 最大验证长度 </summary>
    public const int MaxValidatorLength = 64;
    
    /// <summary>
    /// 下一个验证器
    /// </summary>
    public IValidator? Validator { get; set; }
    
    /// <summary>
    /// 分析是否能验证这个物品
    /// </summary>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public bool CanHandle(INewItem newItem);
    
    /// <summary>
    /// 验证物品属性
    /// </summary>
    /// <param name="newItem"></param>
    /// <param name="errorCollector"></param>
    /// <returns>true表示验证通过</returns>
    public bool Validate(INewItem newItem, IErrorCollector errorCollector);

    /// <summary>
    /// 使用整个验证链验证指定物品的数据
    /// </summary>
    /// <returns>true表示验证通过</returns>
    public static bool ValidateAll(IValidator validator, INewItem newItem, IErrorCollector errorCollector, int maxDeep = MaxValidatorLength)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(newItem);
        ArgumentNullException.ThrowIfNull(errorCollector);
        
        IValidator? current = validator;
        var depth = 0;
    
        while (current != null && depth < maxDeep)
        {
            if (current.CanHandle(newItem) && !current.Validate(newItem, errorCollector))
                return false;
            
            current = current.Validator;
            depth++;
        }
        
        if (depth >= maxDeep)
        {
            errorCollector.AddError("验证长度", $"验证物品{newItem.BaseInfo?.Id}时超出最大验证长度限制: {MaxValidatorLength}");
            return false;
        }
    
        return true;
    }

    /// <summary>
    /// 构建验证器链
    /// </summary>
    /// <param name="validators"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IValidator Build(params IValidator[] validators)
    {
        if (validators == null || validators.Length == 0)
            throw new ArgumentException("至少需要一个验证器", nameof(validators));
        
        for (var i = 0; i < validators.Length - 1; i++)
        {
            validators[i].Validator = validators[i + 1];
        }
    
        return validators[0];
    }
}