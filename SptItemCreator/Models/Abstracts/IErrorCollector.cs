namespace SptItemCreator.Models.Abstracts;

public interface IErrorCollector
{
    /// <summary>
    /// 添加错误
    /// </summary>
    /// <param name="category">类别</param>
    /// <param name="errorMessage">错误信息</param>
    public void AddError(string category, string errorMessage);

    /// <summary>
    /// 是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty();

    /// <summary>
    /// 把错误转换为字符串
    /// </summary>
    /// <returns></returns>
    public string ErrorsToString();
}