using MudBlazor.Services;
using SptItemCreator.NewItemClasses;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Services.Mod;
using SptItemCreator.Abstracts;

namespace SptItemCreator;

/// <summary>
/// 在SPT数据库加载后第一时间加载
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class SptItemCreatorMod(
    LocalLog localLog,
    HttpServer httpServer,
    DataLoader dataLoader,
    WebApplicationBuilder builder,
    DatabaseService databaseService,
    ISptLogger<SptItemCreatorMod> logger,
    CustomItemService customItemService): IOnLoad
{
    public void CreateTask<T>(Dictionary<string, T> data, string taskName) where T: NewItemCommon
    {
        foreach ((string path, T item) in data
                     .Where(k => k.Value.BaseInfo != null)
                     .OrderBy(k => k.Value.BaseInfo?.Order ?? int.MaxValue))
        {
            localLog.LocalLogMsg(LocalLogType.Debug, $"尝试创建{taskName}: \n\t > path={path} \n\t > item={item}");
            CreateNewItemTask(item);
        }  
    }
    
    public Task OnLoad()
    {
        builder.Services.AddMudServices();
        AbstractInfo.LocalLog ??= localLog;
        AbstractNewItem.LocalLog ??= localLog;
        AbstractNewItem.DatabaseService ??= databaseService;
        
        localLog.LocalLogMsg(LocalLogType.Info, $"开始创建新物品任务...");
        CreateTask(dataLoader.NewItemCommon, "通用物品");
        CreateTask(dataLoader.NewItemDrinkOrDrugs, "食物/饮品");
        CreateTask(dataLoader.NewItemMedical, "药品");
        CreateTask(dataLoader.NewItemAmmo, "弹药");
        
        logger.Info($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} WeiUI run at {httpServer.ListeningUrl()}/SIC");
        return Task.CompletedTask;
    }

    public void CreateNewItemTask(NewItemCommon? newItemBase)
    {
        if (newItemBase != null && newItemBase.Verify() && newItemBase.BaseInfo != null)
        {
            try
            {
                if (!(newItemBase.Enable ?? false))
                {
                    localLog.LocalLogMsg(LocalLogType.Info, $"未启用目标物品: {newItemBase}");
                    return;
                }
                
                if (newItemBase.BaseInfo.CloneId != null)
                {
                    NewItemFromCloneDetails? details = newItemBase.CreateItemFromClone();
                    if (details == null)
                    {
                        localLog.LocalLogMsg(LocalLogType.Warn, $"获取物品的详情时获取的结果为null {LocalLog.GetCurrentStackTrace()}\n\t > 目标物品: {newItemBase}");
                        return;
                    }

                    CreateItemResult result = customItemService.CreateItemFromClone(details);
                    localLog.LocalLogMsg(LocalLogType.Info, $"创建新物品结果: {LocalLog.ToStringExcludeNulls(result)}\n\t> id: {newItemBase.BaseInfo.Id}\n\t> name: {newItemBase.BaseInfo.Name}");
                }
                else
                {
                    NewItemDetails? details = newItemBase.CreateNewItem();
                    if (details == null)
                    {
                        localLog.LocalLogMsg(LocalLogType.Warn, $"获取物品的详情时获取失败 {LocalLog.GetCurrentStackTrace()}\n\t > 目标物品: {newItemBase}");
                        return;
                    }

                    CreateItemResult result = customItemService.CreateItem(details);
                    localLog.LocalLogMsg(LocalLogType.Info, $"创建新物品结果: {LocalLog.ToStringExcludeNulls(result)}\n\t> id: {newItemBase.BaseInfo.Id}\n\t> name: {newItemBase.BaseInfo.Name}");
                }

                AutoAddItemToTraderAssort(newItemBase);
            }
            catch (Exception e)
            {
                localLog.LocalLogMsg(LocalLogType.Error, $"创建新物品{newItemBase.ItemPath}时出现错误: {e.Message}\n\t{e.StackTrace}\n");
            }
        }
        else
        {
            localLog.LocalLogMsg(LocalLogType.Error, $"验证新物品数据结构时验证失败 \n\t > newItemBase: {newItemBase} \n\t > 堆栈: {LocalLog.GetCurrentStackTrace()}");
        }
    }

    public void AutoAddItemToTraderAssort(NewItemCommon newItemCommon)
    {
        if (newItemCommon.BaseInfo == null || string.IsNullOrEmpty(newItemCommon.BaseInfo.Id) || string.IsNullOrEmpty(newItemCommon.BaseInfo.TraderId)) return;
        if (databaseService.GetTables().Traders.TryGetValue(newItemCommon.BaseInfo.TraderId, out var trader))
        {
            TraderAssort assort = trader.Assort;
            Item item = new Item
            {
                Id = new MongoId(),
                Template = newItemCommon.BaseInfo.Id,
                ParentId = "hideout",
                SlotId = "hideout",
                Upd = new Upd
                {
                    UnlimitedCount = true,
                    StackObjectsCount = 9999999
                }
            };
            AddItemToAssort(assort, item, newItemCommon.BaseInfo.HandbookPrice, 1);
            localLog.LocalLogMsg(LocalLogType.Info, $"添加物品给商人售卖: \n\t> trader: {trader.Base.Surname}\n\t> id: {newItemCommon.BaseInfo.Id}\n\t> name: {newItemCommon.BaseInfo.Name}");
        }
        else
        {
            localLog.LocalLogMsg(LocalLogType.Error, $"物品{newItemCommon.BaseInfo.Name}({newItemCommon.BaseInfo.Id})的默认商人{newItemCommon.BaseInfo.TraderId}不存在");
        }
    }
    
    public void AddItemToAssort(TraderAssort assort, Item item, double price = 0, int loyalLevel = 1)
    {
        assort.Items.Add(item);
        assort.LoyalLevelItems[item.Id] = loyalLevel;
        assort.BarterScheme[item.Id] =
        [
            [
                new BarterScheme()
                {
                    Count = price,
                    Template = "5449016a4bdc2d6f028b456f" // 卢布
                }
            ]
        ];
    }
}
