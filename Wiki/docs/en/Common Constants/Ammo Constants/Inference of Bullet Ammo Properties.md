## Inference of Bullet Ammo Properties

> Some properties have not yet been organized into AmmoInfo; additionally, some properties are inferred from names and may not be entirely accurate.

### **Core Performance Attributes of Ammunition**

*   **`InitialSpeed`**: The **muzzle velocity** of the bullet (in meters per second). Higher velocity results in shorter flight time, less gravity drop, and typically higher kinetic energy.
*   **`BulletMassGram`**: The **mass** of the bullet (in grams). Mass, combined with velocity, determines the bullet's kinetic energy (Damage) and momentum (which affects recoil and penetration).
*   **`Damage`**: The **base damage value** inflicted by the bullet on unarmored targets. This is a direct measure of the bullet's lethality.
*   **`PenetrationPower`**: The bullet's **penetration power**. Indicates how much damage the bullet retains after hitting an obstacle or armor. Higher values indicate greater penetration capability.
*   **`ArmorDamage`**: The **damage value inflicted on the armor itself**. This determines how quickly the bullet can degrade or penetrate an enemy's armor.
*   **`Caliber`**: The bullet's **caliber**. This is a classification identifier (e.g., `Caliber556x45`, `Caliber9x19PARA`) used to match corresponding firearms.

### **Physical and Interaction Properties of Ammunition**

*   **`ProjectileCount`**: The **number of projectiles** fired per shot. For standard bullets, this is 1; for shotgun shells (like `buckshotBullets`), it is greater than 1.
*   **`buckshotBullets`**: This is a special case or alias for `ProjectileCount`, specifically for **shotgun pellets**, indicating the number of pellets contained in one shell.
*   **`PenetrationChanceObstacle`**: The probability of the bullet **penetrating ordinary obstacles** (such as wooden planks, thin walls). 0.0 means cannot penetrate, 1.0 means guaranteed penetration.
*   **`PenetrationDamageMod`**: The **damage reduction factor** after the bullet penetrates an obstacle. For example, 0.1 means only 10% of the original damage remains after penetration.
*   **`RicochetChance`**: The probability of the bullet **ricocheting**. Ricochets may change direction and continue to cause damage.
*   **`FragmentationChance`**: The probability of the bullet **fragmenting**. Fragmentation produces multiple fragments that can cause area-of-effect damage.
*   **`MinFragmentsCount` / `MaxFragmentsCount`**: The **minimum and maximum number of fragments** produced when the bullet fragments.
*   **`Tracer`**: A boolean value indicating whether the bullet is a **tracer round**. Tracer rounds leave a visible trail during flight.
*   **`TracerColor`**: The **color of the tracer trail** (e.g., "yellow", "red").
*   **`TracerDistance`**: The **visible distance** of the tracer effect (unit may be kilometers or in-game units).

### **Impact on Weapon and Recoil**

*   **`ammoRec`** (Recoil): A modifier affecting the weapon's vertical recoil. Negative values reduce recoil, positive values increase it.
*   **`ammoAccr`** (Accuracy): A modifier affecting the weapon's shooting accuracy. Negative values improve accuracy (reduce spread), positive values decrease accuracy (increase spread).
*   **`DurabilityBurnModificator`**: A multiplier for **weapon durability consumption** by the bullet. Values greater than 1.0 accelerate weapon wear.
*   **`HeatFactor`**: The bullet's influence factor on **weapon overheating**. Higher values cause the weapon to heat up faster during sustained fire.

### **Reliability and Sound**

*   **`MisfireChance`**: The probability of the bullet **misfiring**. A misfire causes the bullet to fail to fire properly.
*   **`MalfMisfireChance` / `MalfFeedChance`**: More detailed malfunction probabilities, referring to **firing malfunctions** and **feeding malfunctions**, respectively.
*   **`ammoSfx`**: The **??? type** of the bullet's firing sound (e.g., "standard", "tracer").
*   **`casingSounds`**: The **sound type** of the casing being ejected (e.g., "rifle556", "pistol_small").

### **Other**

*   **`BallisticCoeficient`**: The **ballistic coefficient**. This is a comprehensive value reflecting the bullet's ability to overcome air resistance and maintain velocity. Higher values indicate better velocity retention over long distances and a flatter trajectory.
*   **`BulletDiameterMilimeters`**: The **actual diameter** of the bullet projectile (in millimeters). This is primarily used for physical collision detection.
*   **`StaminaBurnPerDamage`**: The **stamina drain** per point of damage inflicted on the target. This affects the player's ability to act after being hit.
*   **`HeavyBleedingDelta` / `LightBleedingDelta`**: The chance modifier for the bullet causing **heavy/light bleeding** effects.