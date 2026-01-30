using System.Text.Json.Serialization;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SptItemCreator.Abstracts;

namespace SptItemCreator.InfoClasses;

public sealed record AmmoInfo : AbstractInfo
{
    [JsonPropertyName("ammoType")]
    public string? AmmoType { get; set; }
    
    [JsonPropertyName("initialSpeed")]
    public double? InitialSpeed { get; set; }
    
    [JsonPropertyName("bulletMassGram")]
    public double? BulletMassGram { get; set; }
    
    [JsonPropertyName("damage")]
    public double? Damage { get; set; }
    
    [JsonPropertyName("penetrationPower")]
    public int? PenetrationPower { get; set; }
    
    [JsonPropertyName("armorDamage")]
    public double? ArmorDamage { get; set; }
    
    [JsonPropertyName("fragmentationChance")]
    public double? FragmentationChance { get; set; }
    
    [JsonPropertyName("ricochetChance")]
    public double? RicochetChance { get; set; }
    
    [JsonPropertyName("heavyBleedingDelta")]
    public double? HeavyBleedingDelta { get; set; }
    
    [JsonPropertyName("lightBleedingDelta")]
    public double? LightBleedingDelta { get; set; }
    
    [JsonPropertyName("tracer")]
    public bool? Tracer { get; set; }
    
    [JsonPropertyName("tracerColor")]
    public string? TracerColor { get; set; }
    
    [JsonPropertyName("caliber")]
    public string? Caliber { get; set; }
    
    [JsonPropertyName("buckshotBullets")]
    public int? BuckshotBullets { get; set; }
    
    [JsonPropertyName("ammoAccr")]
    public double? AmmoAccr { get; set; }
    
    [JsonPropertyName("ammoRec")]
    public double? AmmoRec { get; set; }
    
    [JsonPropertyName("ammoDist")]
    public double? AmmoDist { get; set; }
    
    [JsonPropertyName("staminaBurnPerDamage")]
    public double? StaminaBurnPerDamage { get; set; }

    public override void UpdateProperties(TemplateItemProperties properties)
    {
        if (AmmoType != null) properties.AmmoType = AmmoType;
        if (InitialSpeed != null) properties.InitialSpeed = InitialSpeed;
        if (BulletMassGram != null) properties.BulletMassGram = BulletMassGram;
        if (Damage != null) properties.Damage = Damage;
        if (PenetrationPower != null) properties.PenetrationPower = PenetrationPower;
        if (ArmorDamage != null) properties.ArmorDamage = ArmorDamage;
        if (FragmentationChance != null) properties.FragmentationChance = FragmentationChance;
        if (RicochetChance != null) properties.RicochetChance = RicochetChance;
        if (HeavyBleedingDelta != null) properties.HeavyBleedingDelta = HeavyBleedingDelta;
        if (LightBleedingDelta != null) properties.LightBleedingDelta = LightBleedingDelta;
        if (Tracer != null) properties.Tracer = Tracer;
        if (TracerColor != null) properties.TracerColor = TracerColor;
        if (Caliber != null) properties.Caliber = Caliber;
        if (BuckshotBullets != null) properties.BuckshotBullets = BuckshotBullets;
        if (AmmoAccr != null) properties.AmmoAccr = AmmoAccr;
        if (AmmoRec != null) properties.AmmoRec = AmmoRec;
        if (AmmoDist != null) properties.AmmoDist = AmmoDist;
        if (StaminaBurnPerDamage != null) properties.StaminaBurnPerDamage = StaminaBurnPerDamage;
    }
}