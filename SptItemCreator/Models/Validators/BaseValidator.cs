using SptItemCreator.Models.Abstracts;

namespace SptItemCreator.Models.Validators;

public abstract class BaseValidator(IValidator? nextValidator = null): IValidator
{
    public IValidator? Validator { get; set; } = nextValidator;
    public abstract bool CanHandle(INewItem newItem);

    public abstract bool Validate(INewItem newItem, IErrorCollector errorCollector);
}