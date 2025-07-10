using HarmonyLib;
using MelonLoader;
using System.Collections;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(CoroutineRunnerInit), "Coroutine Runner", "1.0.0", "BrunoCitoni")]
[assembly: MelonGame(null, null)]

public class CoroutineRunnerInit : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("CoroutineRunnerMod was loaded");
    }
}

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
                MelonLogger.Warning("CoroutineRunner.Instance was null — was it not created?");
            return _instance;
        }
    }

    private void OnDestroy()
    {
        MelonLogger.Msg("CoroutineRunner was destroyed");
    }

    public static void Create()
    {
        if (_instance != null)
        {
            MelonLogger.Warning("CoroutineRunner already exists.");
            return;
        }

        GameObject obj = new GameObject("CoroutineRunner");
        GameObject.DontDestroyOnLoad(obj);
        _instance = obj.AddComponent<CoroutineRunner>();

        MelonLogger.Msg($"[Runner] Created GameObject: {obj.name}");
        MelonLogger.Msg($"[Runner] Attached MonoBehaviour: {_instance != null}");
    }

    public void RunAfterDelay(float delaySeconds, System.Action action, bool fixedTime = false)
    {
        MelonLogger.Msg("Inside Run After Delay");
        if (fixedTime)
            StartCoroutine(RunFixedTime(action, delaySeconds));
        else
            StartCoroutine(Run(action, delaySeconds));
    }

    private IEnumerator Run(System.Action action, float delay)
    {
        MelonLogger.Msg("Coroutine started");
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private IEnumerator RunFixedTime(System.Action action, float delay)
    {
        MelonLogger.Msg("Coroutine started");
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    // need to patch it here too because it gets destroyed somehow
    [HarmonyPatch(typeof(Game), "StartGame")]
    public static class PatchCoroutineRunnerCreation
    {
        public static void Postfix(Game __instance)
        {
            CoroutineRunner.Create();
        }
    }
}
