using HarmonyLib;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[assembly: MelonInfo(typeof(GameSpeed.Core), "GameSpeed", "1.0.0", "Bruno", null)]
[assembly: MelonGame(null, null)]

namespace GameSpeed
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized speed hack.");
            Time.timeScale = 2.0f;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1.0f;
                LoggerInstance.Msg("Time scale set to 1.0x");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale = 2.0f;
                LoggerInstance.Msg("Time scale set to 2.0x");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 3.0f;
                LoggerInstance.Msg("Time scale set to 3.0x");
            }
        }
    }

    [HarmonyPatch(typeof(TimerScript), nameof(TimerScript.StartTimer))]
    public static class TimerScript_StartTimer_Patch
    {
        public static bool Prefix(TimerScript __instance)
        {
            // Make sure CoroutineRunner is initialized
            if (CoroutineRunner.Instance == null)
            {
                MelonLogger.Warning("CoroutineRunner.Instance is null. Trying to create it manually.");
                CoroutineRunner.Create();
            }

            CoroutineRunner.Instance.RunAfterDelay(__instance.duration, () =>
            {
                __instance.DestroyTimer();
            }, realtime: true); // use realtime delay

            return false; // Skip original method
        }
    }

}