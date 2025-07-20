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
            Time.timeScale = 1.0f;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Time.timeScale = 1.0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Time.timeScale = 1.2f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 1.4f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Time.timeScale = 1.8f;
            } else if (Input.GetKeyDown(KeyCode.Alpha5))
            {                
                Time.timeScale = 2.0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Time.timeScale = 2.5f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                Time.timeScale = 3.0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Time.timeScale = 4.0f;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                Time.timeScale = 5.0f;
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