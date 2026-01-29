using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Enums;
using SptItemCreator.InfoClasses;

namespace SptItemCreator.NewItemClasses;

/// <summary>
/// 食物/饮品
/// </summary>
public class NewItemDrinkOrFood: NewItemCommon
{
    [JsonPropertyName("drinkFoodInfo")]
    public DrinkFoodInfo? DrinkFoodInfo { get; set; }
    
    protected override void DoPropertyApplication(TemplateItemProperties props, DatabaseService? databaseService = null)
    {
        base.DoPropertyApplication(props, databaseService);
        DrinkFoodInfo?.Update(props, databaseService);
    }

    protected override void DoCustomParameterValidation(Dictionary<string, string> oldResults)
    {
        base.DoCustomParameterValidation(oldResults);
        if (DrinkFoodInfo == null) oldResults["DrinkFoodInfo"] = "DrinkFoodInfo属性不存在, 无法正确生成食物与饮品数据";
    }

    protected override bool DoCustomValidation()
    {
        Enable ??= Default.NewItemEnable;
        return base.DoCustomValidation() && DrinkFoodInfo != null;
    }
}