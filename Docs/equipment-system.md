# Equipment system

## Related scripts

- BackWeaponChangeScript
- WeaponChangeScript
- LevelEvent
- LevelSystem
- ItemScript, ArmorScript, WeaponScript, ProjectileScript, ConsumableScript
- InventoryController

## Explanation

The `BackWeaponChangeScript` and `WeaponChangeScript` are used to switching and update object in the onHand slot and offHand slot. Both for damage calculation purpose and visual indicator on player model.

The `LevelEvent` and `LevelSystem` use observer pattern to detect when enemies dies and drop loot.

The 5 `****Script` are used to store information about the item.

The `InventoryController` are used to store what equipment the player current have, with 5 slots(for now): Onhand weapon, Offhand weapon, Projectiles, Consumable and Armor.

## How it work

Each time the enemy die, the `ItemEventManager`, which house `LevelEvent` and `LevelSystem` calculate some stuff and drop loot at the enemy's position.

When player got near the item or interact with them, it will add to player inventory (`InventoryController`), and if there is already another item exist in that slot, it will pop out.

Everytime new stuff added to inventory, it will update the player stat accordingly.

The default katana the player has is never really removed, it just turned off.

## Stat calculation

The equipment stat is calculated the moment it dropped from enemies. There are 3 factors that affect the final stat:

- The `MINIMUM_BONUS` and `MAXIMUM_BONUS`, which is use to roll for a bonus that apply on the equipment.

- The `progressDifficulty`, which increased for every stage player cleared. This improved the equipment stat by a calculated amount.

- The `baseStat`, which is exactly like it name.

The final stat will be the bonus stat + progress stat + base stat.

### Example

A sword with: `MINIMUM_BONUS` = 10 and `MAXMIMUM_BONUS` = 100, `damagePerProgress` = 20, current `progressDifficulty` = 0.4, `baseStat` = 12.

First, it will roll the bonus damage, which is a number between 10 and 100. Assumed it get 20.

Then it multiple `damagePerProgress` with `progressDifficulty` and got 8.

Add both of them into `baseDamage` and we got final damage = 20 + 8 + 12 = 40.
