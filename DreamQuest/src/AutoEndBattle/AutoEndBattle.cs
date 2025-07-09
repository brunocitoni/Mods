using MelonLoader;
using HarmonyLib;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(AutoEndBattle.AutoEndBattle), "AutoEndBattle", "1.0.0", "Bruno", null)]
[assembly: MelonGame("Peter Whalen", "Dream Quest")]

namespace AutoEndBattle
{
    public class AutoEndBattle : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized AutoEndBattles.");
        }
    }

    [HarmonyPatch(typeof(Player), "Die")]
    public static class AutoEndBattleFix
    {
        public static bool Prefix(Player __instance)
        {
            bool actuallyDie = !__instance.game.DeathPrevention(__instance);
            if (actuallyDie)
            {
                __instance.game.Lose(__instance);

                if (__instance == __instance.game.them) // it's a monster who just died, not the player
                {
                    Infoblock[] allScripts = GameObject.FindObjectsOfType<Infoblock>();

                    MelonLogger.Msg("Found " + allScripts.Length + " Infoblock scripts. Trying to act of the first one");

                    allScripts[0].EndGame();
                }
            }

            return false;
        }
    }
}