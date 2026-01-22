using System.Text.Json.Serialization;
using JetBrains.Annotations;
using SptItemCreator.abstracts;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace SptItemCreator.infoClasses;

public record AmmoInfo : AbstractInfo
{
    [JsonIgnore] [UsedImplicitly] public new static bool ShouldUpdateDatabaseService => false;
    
    [JsonPropertyName("ammoType")]
    public virtual string? AmmoType { get; set; }
    
    [JsonPropertyName("initialSpeed")]
    public virtual double? InitialSpeed { get; set; }
    
    [JsonPropertyName("bulletMassGram")]
    public virtual double? BulletMassGram { get; set; }
    
    [JsonPropertyName("damage")]
    public virtual double? Damage { get; set; }
    
    [JsonPropertyName("penetrationPower")]
    public virtual int? PenetrationPower { get; set; }
    
    [JsonPropertyName("armorDamage")]
    public virtual double? ArmorDamage { get; set; }
    
    [JsonPropertyName("fragmentationChance")]
    public virtual double? FragmentationChance { get; set; }
    
    [JsonPropertyName("ricochetChance")]
    public virtual double? RicochetChance { get; set; }
    
    [JsonPropertyName("heavyBleedingDelta")]
    public virtual double? HeavyBleedingDelta { get; set; }
    
    [JsonPropertyName("lightBleedingDelta")]
    public virtual double? LightBleedingDelta { get; set; }
    
    [JsonPropertyName("tracer")]
    public virtual bool? Tracer { get; set; }
    
    [JsonPropertyName("tracerColor")]
    public virtual string? TracerColor { get; set; }
    
    [JsonPropertyName("caliber")]
    public virtual string? Caliber { get; set; }
    
    [JsonPropertyName("buckshotBullets")]
    public virtual int? BuckshotBullets { get; set; }
    
    [JsonPropertyName("ammoAccr")]
    public virtual double? AmmoAccr { get; set; }
    
    [JsonPropertyName("ammoRec")]
    public virtual double? AmmoRec { get; set; }
    
    [JsonPropertyName("ammoDist")]
    public virtual double? AmmoDist { get; set; }
    
    [JsonPropertyName("staminaBurnPerDamage")]
    public virtual double? StaminaBurnPerDamage { get; set; }

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