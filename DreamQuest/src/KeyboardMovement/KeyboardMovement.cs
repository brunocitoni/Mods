using HarmonyLib;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(KeyboardMovement.KeyboardMovement), "KeyboardMovement", "1.0.0", "BrunoCitoni", null)]
[assembly: MelonGame("Peter Whalen", "Dream Quest")]

namespace KeyboardMovement
{
    public class KeyboardMovement : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized KeyboardMovement.");
            MovementListener.Initialize();
        }
    }

    public class MovementListener : MonoBehaviour
    {
        private static MovementListener _instance;
        public static DungeonBoardPhysical boardPhysical;
        public static DungeonPlayerPhysical playerPhysical;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising MovementListener");
                GameObject obj = new GameObject("MovementListener");
                _instance = obj.AddComponent<MovementListener>();
                GameObject.DontDestroyOnLoad(obj);

            }
            else
            {
                MelonLogger.Msg("Tried initialising a second MovementListener");
            }
        }

        public static void AssignBoard(DungeonBoardPhysical _board)
        {
            MelonLogger.Msg("Assigning board");
            boardPhysical = _board;
            if (boardPhysical != null)
                MelonLogger.Msg("Board assigned as " + boardPhysical);
            else
                MelonLogger.Msg("Could not assign the board");
        }

        public static void AssignDungeonPlayerPhysical(DungeonPlayerPhysical _player)
        {
            playerPhysical = _player;
            if (playerPhysical != null)
                MelonLogger.Msg("Player assigned as " + playerPhysical);
            else
                MelonLogger.Msg("Could not assign the player");
        }

        public void Update()
        {
            if (boardPhysical == null) return;

            Tile toMoveTo = null;

            if (!boardPhysical.board.game.activeShop) // if a shop window is not active we can move around the dungeon
            {
                if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
                {
                    toMoveTo = boardPhysical.board.PlayerTile().Right();
                }
                if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    toMoveTo = boardPhysical.board.PlayerTile().Left();
                }
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    toMoveTo = boardPhysical.board.PlayerTile().Up();
                }
                if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
                {
                    toMoveTo = boardPhysical.board.PlayerTile().Down();
                }

                if (toMoveTo != null)
                {
                    boardPhysical.board.TryMoveTo(toMoveTo);
                    return;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                MelonLogger.Msg("Pressed space");

                if (playerPhysical.miniDisplay == null)
                {
                    MelonLogger.Msg("No miniDisplay found");
                    return;
                }

                // Try to find any ShopDialogueButton inside miniDisplay
                ShopDialogueButton[] buttons = playerPhysical.miniDisplay.GetComponentsInChildren<ShopDialogueButton>();

                buttons[0].button.OnMouseDown();
                buttons[0].button.OnMouseUp();
            }
        }
    }

    [HarmonyPatch(typeof(DungeonBoardPhysical), "Awake")]
    public static class PatchMovementListener
    {
        public static void Postfix(DungeonBoardPhysical __instance)
        {
            MovementListener.Initialize();
            MovementListener.AssignBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(DungeonPlayerPhysical), "Initialize")]
    public static class PatchDungeonPlayerPhysicalInit
    {
        public static void Postfix(DungeonPlayerPhysical __instance)
        {
            MovementListener.Initialize();
            MovementListener.AssignDungeonPlayerPhysical(__instance);
        }
    }
}
