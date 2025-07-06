using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

[assembly: MelonInfo(typeof(DreamQuestFixes.DreamQuestFixes), "DreamQuestFixes", "1.0.0", "BrunoCitoni")]
[assembly: MelonGame("Peter Whalen", "Dream Quest")]

namespace DreamQuestFixes
{
    public class DreamQuestFixes : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized Dream Quest Fixes.");
        }
    }

    [HarmonyPatch(typeof(Player), "CheckPoison")]
    public static class PatchPoison
    {
        public static bool Prefix(Player __instance)
        {
            if (__instance.poison > 0)
            {
                __instance.Pulse(PlayerAttributes.POISON);

                DamageTypes dtype = DamageTypes.EARTH;

                var enemy = __instance.Enemy();
                if (enemy.nextPierce == 1 || enemy.elementalForm == DamageTypes.RAW || enemy.elementalFormBase == DamageTypes.RAW)
                {
                    dtype = DamageTypes.RAW;
                }

                __instance.TakeDamage(__instance.poison, dtype);
                __instance.SetAttribute(PlayerAttributes.POISON, __instance.poison - 1);
            }

            return false; // Skip original method
        }
    }

    [HarmonyPatch(typeof(Discharge), "PythonInitialize")]
    public static class PatchDischargeDescription
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // Check if it's a ldstr (load string) with the specific original value
                if (instruction.opcode == OpCodes.Ldstr &&
                    instruction.operand is string str &&
                    str.Contains("Deal 1 @air damage"))
                {
                    instruction.operand = "Deal 5 @air damage for each mana you have left after casting Discharge. Lose physical immunity until your next turn.";
                }

                yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(TreasureChest), "Open")]
    public static class PatchTreasureChestDescription
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // Check if it's a ldstr (load string) with the specific original value
                if (instruction.opcode == OpCodes.Ldstr &&
                    instruction.operand is string str &&
                    str.Contains("Discard The Rest"))
                {
                    instruction.operand = "Sell The Loot";
                }

                yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(TreasureChest), "Finished")]
    public static class PatchSmashedChest
    {
        public static bool Prefix(TreasureChest __instance)
        {
            
            __instance.dungeon.player.GainGold(UnityEngine.Random.Range(1,5)*__instance.loot.Count);
            __instance.dungeon.player.physical.WipeMiniDisplayNow();
            __instance.dungeon.WindowFinished();
            __instance.Destroy();

            return false;
        }
    }    
}
