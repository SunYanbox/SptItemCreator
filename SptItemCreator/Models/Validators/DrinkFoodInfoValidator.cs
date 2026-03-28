using SptItemCreator.Models.Abstracts;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Validators;

public class DrinkFoodInfoValidator : BaseValidator
{
    public override bool CanHandle(INewItem newItem) => newItem.DrinkFoodInfo is not null;

    public override bool Validate(INewItem newItem, IErrorCollector errorCollector)
    {
        if (newItem.DrinkFoodInfo is null) return true;
        ValidateFieldFormats(newItem, errorCollector);
        return true;
    }

    private static void ValidateFieldFormats(INewItem newItem, IErrorCollector errorCollector)
    {
        DrinkFoodInfo? food = newItem.DrinkFoodInfo;

        if (food?.FoodUseTime is < 0)
            errorCollector.AddError("DrinkFoodInfo", $"[FoodUseTime] 使用时间不应为负: {food.FoodUseTime}");

        if (food?.Hydration is 0)
            errorCollector.AddError("DrinkFoodInfo", "[Hydration] 值为0可能不是预期行为");
        if (food?.Energy is 0)
            errorCollector.AddError("DrinkFoodInfo", "[Energy] 值为0可能不是预期行为");
    }
}
