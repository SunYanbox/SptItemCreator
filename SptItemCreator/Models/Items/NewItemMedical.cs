using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using SptItemCreator.Core.Enums;
using SptItemCreator.Models.InfoData;

namespace SptItemCreator.Models.Items;

public class NewItemMedical: NewItemCommon
{
    [JsonPropertyName("medicalInfo")]
    public MedicalInfo? MedicalInfo { get; set; }
    
    protected override bool DoCustomValidation()
    {
        Enable ??= Default.NewItemEnable;
        return base.DoCustomValidation();
    }

    protected override void DoCustomParameterValidation(Dictionary<string, string> oldResults)
    {
        base.DoCustomParameterValidation(oldResults);
        MedicalInfo ??= new MedicalInfo();
    }

    protected override void DoPropertyApplication(TemplateItemProperties props, DatabaseService? databaseService = null)
    {
        base.DoPropertyApplication(props, databaseService);
        MedicalInfo?.Update(props, databaseService);
    }
}