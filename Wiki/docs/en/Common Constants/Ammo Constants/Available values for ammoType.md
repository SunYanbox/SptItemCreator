## Available values for ammoType

> You need to use one of the three strings below, just like in the example.

- `bullet`
- `buckshot`
- `grenade`

This attribute is used for ammunition, with the specific meaning as follows: 

- `bullet`: **Bullet**
- `buckshot`: **Buckshot** (shotgun pellets)
- `grenade`: **Grenade**

**Example**

```jsonc
{
    "_id": "54527ac44bdc2d36668b4567",
    "_name": "patron_556x45_M855A1",
    "_parent": "5485a8684bdc2da71d8b4567",
    "_type": "Item",
    "_props": {
        "ammoType": "bullet"
        // Other properties omitted here
    }
}
```
