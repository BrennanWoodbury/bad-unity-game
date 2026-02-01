# Pharma Clicker MVP

## Quick scene setup (automatic)
1. Add the `PharmaClickerBootstrap` component to any GameObject in the scene.
2. Press Play. The bootstrapper spawns the core systems and UI at runtime.

## Optional customization
- Create `UpgradeDefinition` assets via Assets -> Create -> Pharma Clicker -> Upgrade Definition.
- Assign them to the `PharmaClickerBootstrap` upgrades list to override the defaults.

## Notes
- The UI scripts use UnityEngine.UI.Text (legacy). If your project uses TextMeshPro, swap the fields and text assignments accordingly.
- Idle income is applied in small ticks to stay consistent during frame rate dips.
