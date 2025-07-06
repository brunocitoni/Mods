using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[assembly: MelonInfo(typeof(CustomContentLoader.CustomContentLoader), "CustomContentLoader", "1.0.0", "Bruno", null)]
[assembly: MelonGame(null, null)]

namespace CustomContentLoader
{
    public class CustomContentLoader : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized Custom Content Loader.");
        }
    }

    // the two patches below are common to all custom classes, should be in a main "Enable Customn Classes" patch
    [HarmonyPatch(typeof(Game), "OutputToString")]
    public static class PatchGameOutputToString
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var getTypeMethod = typeof(object).GetMethod("GetType");
            var getAsmQualifiedName = typeof(System.Type).GetProperty("AssemblyQualifiedName").GetGetMethod();

            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Callvirt && code.operand as MethodInfo == getTypeMethod)
                {
                    // After GetType(), we need to inject a call to get_AssemblyQualifiedName
                    yield return code; // Keep GetType()
                    yield return new CodeInstruction(OpCodes.Callvirt, getAsmQualifiedName);
                }
                else
                {
                    yield return code;
                }
            }
        }
    }

    [HarmonyPatch(typeof(DungeonPlayer), "PopulateFromString")]
    public static class PatchDungeonPlayerPopulateFromString
    {
        public static void Postfix(DungeonPlayer __instance)
        {
            MelonLogger.Msg("Inside postfix");
            if (__instance.portrait == null && __instance.profession != null)
            {
                MelonLogger.Msg("trying to assign fallback texture");
                MelonLogger.Msg("profession is " + __instance.profession);
                Texture fallback = __instance.profession.GetLittleTexture();
                if (fallback != null)
                {
                    __instance.portrait = fallback;
                    MelonLogger.Msg("Assigned fallback portrait via profession.GetLittleTexture()");
                }
                else
                {
                    MelonLogger.Warning("Fallback portrait was still null!");
                }
            }
        }
    }
}