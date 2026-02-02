## BuffsInfo

> An effect record class inherited from `AbstractInfo`, used to define the status effects provided by food, drinks, or injectors.

| Property             | In Data File      | Type                                                | Description                         | Optionality |
| -------------------- | ----------------- | --------------------------------------------------- | ----------------------------------- | ----------- |
| **StimulatorBuffs**  | "stimulatorBuffs" | string                                              | Unique identifier for the effect    | Optional    |
| **Buffs**            | "buffs"           | List<[Buff](./Common Data Structures in SPT/Related To The Effect/Buff.md)> | List of beneficial effects          | Optional    |

You can assign only `StimulatorBuffs` to an existing status effect, or assign both together; other cases will result in an error.

## buffsInfo Field

This field is for food/drinks/injectors, making it convenient to register new buffs.

```json
// Beneficial effect configuration
"buffsInfo": {
    // Stimulator effect configuration, empty string indicates no stimulator effect
    "stimulatorBuffs": "",
    // List of beneficial effects provided by the item, null indicates no additional effects
    "buffs": null
}
```

Example with buffs:

**Note: If stimulatorBuffs is assigned a unique value and buffs is set to null, it may cause registration failure for buffs and lead to client-side issues.**

You can use buff names already registered in **[globals.json](./Common Data Structures in SPT/SPTData/globals.json.md)**.

```json
// Buff configuration
"buffsInfo": {
    // Registered buff name
    "stimulatorBuffs": "BuffsStimulatorTpl",
    // Registered buffs; if null, registration will be skipped; if stimulatorBuffs is already registered, it will be used.
    "buffs": [
        {
            "AbsoluteValue": true,
            "BuffType": "HealthRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 300,
            "SkillName": "",
            "Value": 1
        },
        {
            "AbsoluteValue": true,
            "BuffType": "SkillRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 300,
            "SkillName": "Metabolism",
            "Value": 20
        }
    ]
}
```

## Example

```json
"buffsInfo": {
    "stimulatorBuffs": "BuffsSuntionElixirOfStrength",
    "buffs": [
        {
            "AbsoluteValue": true,
            "BuffType": "StaminaRate",
            "Chance": 1,
            "Delay": 1,
            "Duration": 10,
            "SkillName": "",
            "Value": 12
        },
        {
            "AbsoluteValue": true,
            "BuffType": "HandsTremor",
            "Chance": 1,
            "Delay": 0,
            "Duration": 5,
            "SkillName": "",
            "Value": 0
        }
    ]
}

