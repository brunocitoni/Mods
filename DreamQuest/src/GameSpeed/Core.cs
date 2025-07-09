using MelonLoader;
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
}