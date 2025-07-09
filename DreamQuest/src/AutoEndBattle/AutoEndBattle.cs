using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityScript.Lang;
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

 // if player has no cards left in hand immediately pass, what about powers????
    [HarmonyPatch(typeof(Infoblock), "NotifyBehave")]
    public class NotifyBehavePatch
    {
        static void Prefix(Infoblock __instance)
        {
            __instance.playAllCardsBlock = false;

            if (!__instance.player.HasLegalPlays() &&
                __instance.player.game.winner == null &&
                !__instance.shaken &&
                !__instance.IgnoreShake() && __instance.player.hand.ChildCount() == 0)
            {
                __instance.shaken = true;
                __instance.StartCoroutine_Auto(__instance.ShakeEndTurnButton());
                __instance.player.game.Tutorial(UserAttribute.TUTORIAL_COMBAT_END_TURN);

                // ✅ Inject your call here
                __instance.EndTurn();
            }
        }
    }


}