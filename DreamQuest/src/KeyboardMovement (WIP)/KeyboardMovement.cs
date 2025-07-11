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
        }
    }

    // dungeon movement and interaction
    public class KeyboardControlDungeon : MonoBehaviour
    {
        private static KeyboardControlDungeon _instance;
        public static DungeonBoardPhysical boardPhysical;
        public static DungeonPlayerPhysical playerPhysical;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlDungeon");
                GameObject obj = new GameObject("KeyboardControlDungeon");
                _instance = obj.AddComponent<KeyboardControlDungeon>();
                GameObject.DontDestroyOnLoad(obj);

            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlDungeon");
            }
        }

        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlDungeon enabled: " + _instance.enabled);
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
    public static class PatchKeyboardControlDungeon
    {
        public static void Postfix(DungeonBoardPhysical __instance)
        {
            KeyboardControlDungeon.Initialize();
            KeyboardControlDungeon.AssignBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(DungeonPlayerPhysical), "Initialize")]
    public static class PatchDungeonPlayerPhysicalInit
    {
        public static void Postfix(DungeonPlayerPhysical __instance)
        {
            KeyboardControlDungeon.Initialize();
            KeyboardControlDungeon.AssignDungeonPlayerPhysical(__instance);
        }
    }


    #region Combat
    public class KeyboardControlCombat : MonoBehaviour
    {
        private static KeyboardControlCombat _instance;
        public static Hand playerHand;
        static int highlightedIndex = 0;
        static Card currentSelectedCard;
        static Player player;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlCombat");
                GameObject obj = new GameObject("KeyboardControlCombat");
                _instance = obj.AddComponent<KeyboardControlCombat>();
                GameObject.DontDestroyOnLoad(obj);

            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlCombat");
            }
        }

        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlCombat enabled: " + _instance.enabled);
        }

        public static void AssignHand(Player _player)
        {
            MelonLogger.Msg("Assigning playerHand");
            playerHand = _player.hand;
            if (playerHand != null)
                MelonLogger.Msg("Hand assigned as " + playerHand);
            else
                MelonLogger.Msg("Could not assign playerHand");

            MelonLogger.Msg("Assigning player");
            player = _player;
            if (player != null)
                MelonLogger.Msg("player assigned as " + player);
            else
                MelonLogger.Msg("Could not assign player");

            // this should happen on turn start
            OnTurnStart();
        }

        public static void HighlightNext(int index)
        {
            if (playerHand.Children().Count == 0) // no cards in hand
            {
                if (currentSelectedCard != null)
                    currentSelectedCard = null;
                return;
            }
            

            if (playerHand.Children()[index] is Card)
            {
                if (currentSelectedCard != null)
                    currentSelectedCard.UnHighlight();
                currentSelectedCard = playerHand.Children()[index] as Card;
                currentSelectedCard.Highlight();
            }
        }

        private static void ChangeHighlightIndex(int change)
        {
            highlightedIndex += change;
            if (highlightedIndex < 0)
                highlightedIndex = 0;
            if (highlightedIndex > playerHand.Children().Count - 1)
                highlightedIndex = playerHand.Children().Count - 1;
        }

        public static void OnTurnStart()
        {
            MelonLogger.Msg("Starting turn, setting highlight to 0");
            highlightedIndex = 0;
            HighlightNext(highlightedIndex);
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                ChangeHighlightIndex(1);
                HighlightNext(highlightedIndex);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ChangeHighlightIndex(-1);
                HighlightNext(highlightedIndex);
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                MelonLogger.Msg("Calling PlayMe on selected card: " + currentSelectedCard);
                if (currentSelectedCard != null)
                {
                    MelonLogger.Msg("removing highlight from selected card: " + currentSelectedCard);
                    currentSelectedCard.UnHighlight();
                    MelonLogger.Msg("Calling PlayMe on selected card: " + currentSelectedCard);
                    currentSelectedCard.PlayMe();
                    ChangeHighlightIndex(-1);
                    HighlightNext(highlightedIndex);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                player.Pass();
            }
        }
    }

        // need to patch it here too because it gets destroyed somehow
    [HarmonyPatch(typeof(Game), "StartGame")]
    public static class PatchCoroutineRunnerCreation
    {
        public static void Postfix(Game __instance)
        {
            MelonLogger.Msg("Assigning hand");
            KeyboardControlCombat.Initialize();
            KeyboardControlCombat.AssignHand(__instance.me);
        }
    }

    [HarmonyPatch(typeof(Player), "StartTurnEffects")]
    public static class PatchPlayerStartTurn
    {
        public static void Postfix( Player __instance)
        {
            MelonLogger.Msg("Starting turn");
            if (KeyboardControlCombat.playerHand != null && __instance == __instance.game.me)
                KeyboardControlCombat.OnTurnStart();
        }
    }

    #endregion
}
