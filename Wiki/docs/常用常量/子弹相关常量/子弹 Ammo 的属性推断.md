## 子弹 Ammo 的属性推断

> 部分属性还没整理到AmmoInfo中；然后部分属性是根据名称推断的，不一定正确

### **子弹核心性能属性**

*   **`InitialSpeed`**: 子弹的**初速度**（单位：米/秒）。速度越高，飞行时间越短，受重力影响越小，通常也意味着更高的动能。
*   **`BulletMassGram`**: 子弹的**质量**（单位：克）。质量与速度共同决定了子弹的动能（Damage）和动量（影响后坐力和穿透）。
*   **`Damage`**: 子弹对无甲目标造成的**基础伤害值**。这是衡量子弹杀伤力的直接指标。
*   **`PenetrationPower`**: 子弹的**穿透力**。代表子弹在击中障碍物或护甲时，能保留多少伤害。数值越高，穿透能力越强。
*   **`ArmorDamage`**: 子弹对**护甲本身的伤害值**。这个值决定了子弹能多快地磨损或击穿敌人的护甲。
*   **`Caliber`**: 子弹的**口径**。这是一个分类标识符（如 `Caliber556x45`, `Caliber9x19PARA`），用于匹配对应的枪械。

### **子弹物理与交互属性**

*   **`ProjectileCount`**: 每发射一发子弹所射出的**弹丸数量**。普通子弹为1，霰弹枪子弹（如 `buckshotBullets`）则大于1。
*   **`buckshotBullets`**: 这是 `ProjectileCount` 的一个特例或别名，专门用于**霰弹**，表示一发弹壳里包含的弹丸数。
*   **`PenetrationChanceObstacle`**: 子弹**穿透普通障碍物**（如木板、薄墙）的概率。0.0 表示无法穿透，1.0 表示必定穿透。
*   **`PenetrationDamageMod`**: 子弹在穿透障碍物后，**伤害的衰减比例**。例如，0.1 表示穿透后只保留10%的原始伤害。
*   **`RicochetChance`**: 子弹发生**跳弹**（Ricochet）的概率。跳弹可能会改变方向并继续造成伤害。
*   **`FragmentationChance`**: 子弹**碎裂**的概率。碎裂后会产生多个碎片，可能造成范围伤害。
*   **`MinFragmentsCount` / `MaxFragmentsCount`**: 子弹碎裂后产生的**最小和最大碎片数量**。
*   **`Tracer`**: 布尔值，表示该子弹是否为**曳光弹**。曳光弹在飞行时会留下可见的轨迹。
*   **`TracerColor`**: 曳光弹的**轨迹颜色**（如 "yellow", "red"）。
*   **`TracerDistance`**: 曳光效果的**可见距离**（单位可能是千米或游戏内单位）。

### **对武器和后坐力的影响**

*   **`ammoRec` **(Recoil): 影响武器垂直后坐力的修正值。负值减少后坐力，正值增加后坐力。
*   **`ammoAccr` **(Accuracy): 影响武器射击精度的修正值。负值提高精度（减小散布），正值降低精度（增大散布）。
*   **`DurabilityBurnModificator`**: 子弹对**武器耐久度的消耗倍率**。大于1.0的值会加速武器磨损。
*   **`HeatFactor`**: 子弹对**武器过热**（Heat）的影响因子。数值越高，连续射击时武器升温越快。

### **可靠性与声音**

*   **`MisfireChance`**: 子弹**哑火**（Misfire）的概率。哑火会导致子弹无法正常发射。
*   **`MalfMisfireChance` / `MalfFeedChance`**: 更详细的故障概率，分别指**击发故障**和**供弹故障**。
*   **`ammoSfx`**: 子弹射击时的**???类型**（如 "standart", "tracer"）。
*   **`casingSounds`**: 弹壳抛出时的**声音类型**（如 "rifle556", "pistol_small"）。

### **其他**

*   **`BallisticCoeficient`**: **弹道系数**。这是一个综合反映子弹克服空气阻力、保持速度能力的数值。数值越高，远距离存速能力越好，弹道越平直。
*   **`BulletDiameterMilimeters`**: 子弹头的**实际直径**（单位：毫米）。这主要用于物理碰撞检测。
*   **`StaminaBurnPerDamage`**: 对目标造成每点伤害所附带的**体力消耗**。这会影响玩家被击中后的行动能力。
*   **`HeavyBleedingDelta` / `LightBleedingDelta`**: 子弹造成**重度/轻度出血**效果的几率加成。