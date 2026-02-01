namespace SptItemCreator.Enums;

public static class Default
{
    /// <summary> 默认新物品加载行为 </summary>
    public const bool NewItemEnable = false;
    /// <summary> 默认新物品名称行为 </summary>
    public const string BaseInfoName = "未命名物品";
    /// <summary> 默认新物品SIC类型 </summary>
    public const string BaseInfoType = "common";
    /// <summary> 默认新物品描述 </summary>
    public const string BaseInfoDescription = "";
    /// <summary> 默认新物品作者 </summary>
    public const string BaseInfoAuthor = "佚名";
    /// <summary> 默认新物品协议 </summary>
    public const string BaseInfoLicense = "MIT";
    /// <summary> 默认新物品加载优先级(影响新物品的创建顺序, 数值越大加载越慢) </summary>
    public const int BaseInfoOrder = 0;
    /// <summary> 默认新物品是否允许在跳蚤市场售卖 </summary>
    public const bool BaseInfoCanSellOnRagfair = true;
    /// <summary> 默认新物品跳蚤市场最低价格(1) </summary>
    public const double BaseInfoFleaPriceMinimum = 1.000000;
    /// <summary> 默认新物品手册最低价格(1) </summary>
    public const double BaseInfoHandbookPriceMinimum = 1.000000;
    /// <summary> 默认ParentId缓存更新时间 </summary>
    public const double ParentIdCacheUpdateTime = 90 * 60 * 1000;
    /// <summary> 默认缓存系统更新/加载时间 </summary>
    public const long CacheSystemUpdateTime = 5 * 60 * 1000;
    /// <summary> 本地日志文件最大字节数(512KB) </summary>
    public const long LocalFilesMaximumBytes = 512 * 1024;
}