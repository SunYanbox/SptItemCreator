using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Web;

namespace SptItemCreator;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public record ModMetadata : AbstractModMetadata, IModWebMetadata
{
    public override string ModGuid { get; init; } = "com.suntion.sptitemcreator";
    public override string Name { get; init; } = "SptItemCreator";
    public override string Author { get; init; } = "Suntion";
    public override List<string>? Contributors { get; init; } = [];
    public override SemanticVersioning.Version Version { get; init; } = new("0.0.3");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.8");
    
    
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; } = "https://forge.sp-tarkov.com/mod/2565/spt-item-creator";
    public override bool? IsBundleMod { get; init; } = true;
    public override string License { get; init; } = "CC-BY-SA";
}