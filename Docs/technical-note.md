# Technical Note

## Scene structure

- Objects that are not destroyed on scene load:

  - PlayerStatHUD
  - Inventory
  - LevelSystem
  - LevelEvents

- Objects that are must present on every scenes:
  - PauseMenu
  - InventoryCanvas
  - SmoothCamera
  - EventSystem (Unity)
  - PlayerDino
  - DroppedItemTooltip

`DontDestroyOnSceneLoad` objects must be placed in the first room to follow the player. If testing individual room, they are also must be placed in the scene.

EventSystem is required for UI interaction.

## Weird behaviours FAQ

- If the new scene you just loaded using sceneManager appear to be lack of environmental lighting or darker shadow than normal, it is intended behaviour by Unity. It's only happened when you load scene in Editor. When you build the game, it won't happened. Link: <https://answers.unity.com/questions/1264278/unity-5-directional-light-problem-please-help.html>
