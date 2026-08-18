using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using VanillaPsycastsExpanded;

namespace ResurrectAnimalsPatch
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("hellold6.resurrectanimals");
            harmony.Patch(
                AccessTools.Method(typeof(Ability_TargetCorpse), "ValidateTarget"),
                prefix: new HarmonyMethod(typeof(Patch_ValidateTarget), nameof(Patch_ValidateTarget.Prefix))
            );

            Log.Message("[ResurrectAnimals] Harmony patch applied to Ability_TargetCorpse.ValidateTarget");
        }
    }

    public static class Patch_ValidateTarget
    {
        /// <summary>
        /// Runs before Ability_TargetCorpse.ValidateTarget. Only Ability_Resurrect
        /// gets the widened animal-corpse allowance; every other corpse-targeting
        /// ability (Corpsetalk, Enthrall, etc) falls through to the original method.
        ///
        /// IMPORTANT: we do NOT call the base Ability.ValidateTarget via reflection
        /// here. ValidateTarget is virtual, and reflection Invoke() on a virtual
        /// method always performs virtual dispatch to the most-derived override —
        /// which is this very method (now patched), causing infinite recursion /
        /// a stack overflow. So for Ability_Resurrect we just decide pass/fail
        /// ourselves and never call back into any ValidateTarget at all.
        /// </summary>
        public static bool Prefix(Ability_TargetCorpse __instance, LocalTargetInfo target, bool showMessages, ref bool __result)
        {
            if (!(__instance is Ability_Resurrect))
                return true; // run the original method unmodified

            Thing thing = target.Thing;
            Corpse corpse = thing as Corpse;

            if (corpse == null)
            {
                if (showMessages)
                    Messages.Message("VPE.MustBeCorpse".Translate(), new LookTargets(thing), MessageTypeDefOf.CautionInput, true);
                __result = false;
                return false; // skip original
            }

            if (!corpse.InnerPawn.RaceProps.Humanlike && !corpse.InnerPawn.RaceProps.Animal)
            {
                if (showMessages)
                    Messages.Message("VPE.MustBeCorpseHumanlike".Translate(), new LookTargets(thing), MessageTypeDefOf.CautionInput, true);
                __result = false;
                return false; // skip original
            }

            // Passed our checks — accept the target. (Base Ability.ValidateTarget
            // in VEF just does a couple of generic sanity checks that don't apply
            // meaningfully to a corpse/item target here, so it's safe to accept.)
            __result = true;
            return false; // skip original entirely
        }
    }
}