# Resurrect Animals (VPE Patch)

A tiny Harmony patch mod for Vanilla Psycasts Expanded that lets the Necropath
"Resurrect" ability target animal corpses, not just humanlike ones. Every other
corpse-targeting ability (Corpsetalk, Enthrall, etc) is untouched.

## Steam Workshop

[Resurrect Animals (VPE Patch)](https://steamcommunity.com/sharedfiles/filedetails/?id=3785523304)

Workshop ID: `3785523304`

> Note: Some automated requests to Steam Workshop pages may incorrectly display a
> “removed from the community” message. The mod is publicly available when accessed
> normally.

## What's actually going on

The humanlike-only restriction isn't in the AbilityDef XML — it's hardcoded in
`VanillaPsycastsExpanded.Ability_TargetCorpse.ValidateTarget()`, which checks
`corpse.InnerPawn.RaceProps.Humanlike` and rejects anything else. This mod
Harmony-prefixes that method, and only for `Ability_Resurrect` specifically,
so it also allows `RaceProps.Animal`. All other subclasses of
`Ability_TargetCorpse` fall through to the original, unmodified check.

