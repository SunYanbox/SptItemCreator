namespace SptItemCreator.Enums;

public static class ItemRarityData
{
    // 稀有度中文->键字典
    public static readonly Dictionary<string, string> RarityMapping = new()
    {
        { "普通", "Common" },
        { "稀有", "Rare" },
        { "超级稀有", "Superrare" },
        { "不存在", "Not_exist" }
    };

    // 所有允许的稀有度键
    public static readonly HashSet<string> AllowKeys = ["Common", "Rare", "Superrare", "Not_exist"];

    /// <summary>
    /// 根据中文键获取对应的稀有度键, 兼容正确的英文键, 如果都不正确, 返回null
    /// </summary>
    /// <param name="key">中文或英文稀有度键</param>
    /// <returns>对应的英文稀有度键，如果不存在返回null</returns>
    public static string? GetRarityKey(string key)
    {
        if (AllowKeys.Contains(key)) return key;
        return RarityMapping.GetValueOrDefault(key);
    }
}