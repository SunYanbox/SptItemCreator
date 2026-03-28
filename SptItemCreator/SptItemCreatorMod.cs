using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using SptItemCreator.Models.Abstracts;
using SptItemCreator.Core.Services;
using SptItemCreator.Models.Items;

namespace SptItemCreator;

/// <summary>
/// 在SPT数据库加载后第一时间加载
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 2)]
public class SptItemCreatorMod(
    DataLoader dataLoader,
    ISptLogger<SptItemCreatorMod> sptLogger,
    DatabaseService databaseService,
    CustomItemService customItemService): IOnLoad
{
    public void CreateNewItemsTask(string taskName)
    {
        if (dataLoader.NewItems.Count == 0)
        {
            LocalLog.Logger.Warn($"任务<{taskName}>中没有加载任何数据文件");
            return;
        }
        foreach ((string _, INewItem item) in dataLoader.NewItems
                     .Where(k => k.Value.BaseInfo != null)
                     .OrderBy(k => k.Value.BaseInfo?.Order ?? int.MaxValue))
        {
            CreateNewItemTask(item);
        }  
    }
    
    public Task OnLoad()
    {
        AbstractNewItem.DatabaseService ??= databaseService;
        
        LocalLog.Logger.Info("开始创建新物品任务...");
        CreateNewItemsTask("创建新物品");
        
        return Task.CompletedTask;
    }

    public void CreateNewItemTask(INewItem? newItemBase)
    {
        if (newItemBase is null)
        {
            LocalLog.Logger.Error("创建新物品时意外传入null", new ArgumentNullException(nameof(newItemBase)));
            return;
        }
        (bool verify, IErrorCollector errors) = newItemBase.Verify();
        if (verify && newItemBase.BaseInfo != null)
        {
            try
            {
                if (!(newItemBase.Enable ?? false))
                {
                    LocalLog.Logger.Info($"未启用目标物品: {newItemBase}");
                    return;
                }
                
                NewItem.DatabaseService ??= databaseService;
                
                if (newItemBase.BaseInfo.CloneId != null)
                {
                    NewItemFromCloneDetails? details = newItemBase.CreateItemFromClone();
                    if (details == null)
                    {
                        LocalLog.Logger.Warn($"获取物品的详情时获取的结果为null {LocalLog.GetCurrentStackTrace()}\n\t > 目标物品: {newItemBase}");
                        return;
                    }

                    CreateItemResult result = customItemService.CreateItemFromClone(details);
                    LocalLog.Logger.Info($"创建新物品结果: {LocalLog.ToStringExcludeNulls(result)}\n\t> id: {newItemBase.BaseInfo.Id}\n\t> name: {newItemBase.BaseInfo.Name}");
                }
                else
                {
                    NewItemDetails? details = newItemBase.CreateNewItem();
                    if (details == null)
                    {
                        LocalLog.Logger.Warn($"获取物品的详情时获取失败 {LocalLog.GetCurrentStackTrace()}\n\t > 目标物品: {newItemBase}");
                        return;
                    }

                    CreateItemResult result = customItemService.CreateItem(details);
                    LocalLog.Logger.Info($"创建新物品结果: {LocalLog.ToStringExcludeNulls(result)}\n\t> id: {newItemBase.BaseInfo.Id}\n\t> name: {newItemBase.BaseInfo.Name}");
                }

                AutoAddItemToTraderAssort(newItemBase);
            }
            catch (Exception e)
            {
                var errorMsg = $"创建新物品{newItemBase.ItemPath}时出现错误: {e.Message}";
                LocalLog.Logger.Error(errorMsg, e);
                sptLogger.Error(errorMsg, e);
            }
        }
        else
        {
            LocalLog.Logger.Error($"验证新物品数据结构时验证失败 \n\t > 详细问题: {errors.ErrorsToString()} \n\t > newItemBase: {newItemBase} \n\t > 堆栈: {LocalLog.GetCurrentStackTrace()}");
        }
    }

    public void AutoAddItemToTraderAssort(INewItem newItem)
    {
        if (newItem.BaseInfo == null || string.IsNullOrEmpty(newItem.BaseInfo.Id) || string.IsNullOrEmpty(newItem.BaseInfo.TraderId)) return;
        if (databaseService.GetTables().Traders.TryGetValue(newItem.BaseInfo.TraderId, out Trader? trader))
        {
            TraderAssort assort = trader.Assort;
            Item item = new()
            {
                Id = new MongoId(),
                Template = newItem.BaseInfo.Id,
                ParentId = "hideout",
                SlotId = "hideout",
                Upd = new Upd
                {
                    UnlimitedCount = true,
                    StackObjectsCount = 9999999
                }
            };
            AddItemToAssort(assort, item, newItem.BaseInfo.HandbookPrice, 1);
            LocalLog.Logger.Info($"添加物品给商人售卖: \n\t> trader: {trader.Base.Surname}\n\t> id: {newItem.BaseInfo.Id}\n\t> name: {newItem.BaseInfo.Name}");
        }
        else
        {
            LocalLog.Logger.Warn($"物品{newItem.BaseInfo.Name}({newItem.BaseInfo.Id})的默认商人{newItem.BaseInfo.TraderId}不存在");
        }
    }
    
    public void AddItemToAssort(TraderAssort assort, Item item, double price = 0, int loyalLevel = 1)
    {
        assort.Items.Add(item);
        assort.LoyalLevelItems[item.Id] = loyalLevel;
        assort.BarterScheme[item.Id] =
        [
            [
                new BarterScheme
                {
                    Count = price,
                    Template = "5449016a4bdc2d6f028b456f" // 卢布
                }
            ]
        ];
    }
}
