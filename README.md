# Resurrect Animals (VPE Patch)

A tiny Harmony patch mod for Vanilla Psycasts Expanded that lets the Necropath
"Resurrect" ability target animal corpses, not just humanlike ones. Every other
corpse-targeting ability (Corpsetalk, Enthrall, etc) is untouched.

## What's actually going on

The humanlike-only restriction isn't in the AbilityDef XML — it's hardcoded in
`VanillaPsycastsExpanded.Ability_TargetCorpse.ValidateTarget()`, which checks
`corpse.InnerPawn.RaceProps.Humanlike` and rejects anything else. This mod
Harmony-prefixes that method, and only for `Ability_Resurrect` specifically,
so it also allows `RaceProps.Animal`. All other subclasses of
`Ability_TargetCorpse` fall through to the original, unmodified check.

## Folder layout

```
ResurrectAnimals/
├── About/
│   └── About.xml
├── Assemblies/          <- compiled DLL goes here
│   └── ResurrectAnimalsPatch.dll  (not included — build it yourself, see below)
└── Source/
    ├── HarmonyPatch.cs
    └── ResurrectAnimalsPatch.csproj
```

## Building the DLL

You need a C# toolchain (Visual Studio, Rider, or just the `dotnet` CLI with
the .NET Framework 4.7.2 targeting pack installed).

1. Open `Source/ResurrectAnimalsPatch.csproj` and fix the `<RimWorldDir>` and
   `<WorkshopDir>` properties at the top if your install paths differ from the
   defaults (they're currently set to the Steam paths we found earlier:
   `C:\Program Files (x86)\Steam\steamapps\...`).
2. **Double-check the VEF workshop ID.** I used `2023507013` as a placeholder
   for "Vanilla Expanded Framework" — that may not be the exact folder on your
   machine. In-game, go to Mods, click VEF, and check the folder under
   `steamapps\workshop\content\294100\` that contains `VEF.dll` (or
   `VanillaExpandedFramework.dll` — the class names matched `VEF.Abilities.*`
   in what we decompiled, so the assembly is likely just named `VEF.dll`).
   Update the `<HintPath>` accordingly.
3. Build:
   ```
   cd Source
   dotnet build -c Release
   ```
   This drops `ResurrectAnimalsPatch.dll` straight into `../Assemblies/`
   (set via `OutputPath` in the csproj).

## Installing

1. Copy the whole `ResurrectAnimals` folder into your RimWorld `Mods` folder:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\ResurrectAnimals\
   ```
2. Enable it in the mod list, **after** Harmony, Vanilla Expanded Framework,
   and Vanilla Psycasts Expanded.
3. Double check the `<modDependencies>`/`<loadAfter>` packageIds in
   `About/About.xml` match what's actually installed — I used my best guess
   for VEF's (`oskarpotocki.vanillafactionsexpanded.core`) and VPE's
   (`vanillaexpanded.vpsycastse`) packageIds, but these can drift between
   versions. You can confirm the exact IDs from each mod's own `About.xml` in
   their Workshop folders, or in-game via the mod info panel.

## Notes / caveats

- Resurrecting an animal will still route through `Ability_Resurrect.Cast`,
  which spawns a `SoulFromSky` skyfaller and adds the `VPE_Sacrificed` hediff
  to a finger — none of that logic is animal-specific, so it should work
  as-is, but you may want to test on a dev-mode save first (spawn/kill a
  simple animal, corpse it, try the ability) in case any downstream effect
  (e.g. `AbilityExtension_PsychicComa`) assumes a humanlike target somewhere.
- This does **not** widen Corpsetalk or Enthrall — only Resurrect — since
  those have their own lore/balance reasons to stay humanlike-only. If you
  want those opened up too, it's a one-line change in `HarmonyPatch.cs`
  (broaden the `__instance is Ability_Resurrect` check).
