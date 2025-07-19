using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using static MelonLoader.MelonLogger;

[assembly: MelonInfo(typeof(KeyboardControls.KeyboardControlsInit), "KeyboardControls", "1.0.0", "BrunoCitoni", null)]
[assembly: MelonGame("Peter Whalen", "Dream Quest")]

namespace KeyboardControls
{
    public class KeyboardControlsInit : MelonMod
    {
        public override void OnInitializeMelon()
        {
            KeyboardControlsMaster.Initialize();
            LoggerInstance.Msg("KeyboardControls initialized");
        }

        public override void OnLateUpdate()
        {
            if (KeyboardControlsMaster._instance == null)
            {
                LoggerInstance.Msg("KeyboardControlsMaster was null, reinitializing...");
                KeyboardControlsMaster.Initialize();
            }
        }
    }

    public class KeyboardControlsMaster : MonoBehaviour
    {
        public static KeyboardControlsMaster _instance;
        public static GameObject GO;
        public static GamePhysical game;

        public enum state
        {
            DUNGEON,
            COMBAT,
            MENU,
            SHOP
        }

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlMaster");
                GO = new GameObject("KeyboardControlMaster");
                _instance = GO.AddComponent<KeyboardControlsMaster>();
                GameObject.DontDestroyOnLoad(GO);
            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlMaster");
            }

            game = GamePhysical.instance;
            MelonLogger.Msg("Game physical assigned");

            EnableSelf(true);

            // iùnitialize all the other components
            KeyboardControlCombat.Initialize();
            KeyboardControlDungeon.Initialize();
            KeyboardControlShop.Initialize();
            KeyboardControlMenu.Initialize();

            // force update to menu as default for start of game
            UpdateState(_currentState, true);
        }
        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlMaster enabled: " + _instance.enabled);
        }

        private static state _currentState = state.MENU; // or whatever default fits

        public static void UpdateState(state newState, bool forceUpdate = false)
        {
            if (_currentState == newState && !forceUpdate) return; // no change, do nothing

            MelonLogger.Msg("Updating state to: " + newState);
            _currentState = newState;

            switch (newState)
            {
                case state.DUNGEON:
                    KeyboardControlDungeon.EnableSelf(true);
                    KeyboardControlCombat.EnableSelf(false);
                    KeyboardControlMenu.EnableSelf(false);
                    KeyboardControlShop.EnableSelf(false);
                    break;
                case state.COMBAT:
                    KeyboardControlDungeon.EnableSelf(false);
                    KeyboardControlCombat.EnableSelf(true);
                    KeyboardControlMenu.EnableSelf(false);
                    KeyboardControlShop.EnableSelf(false);
                    break;
                case state.MENU:
                    KeyboardControlDungeon.EnableSelf(false);
                    KeyboardControlCombat.EnableSelf(false);
                    KeyboardControlMenu.EnableSelf(true);
                    KeyboardControlShop.EnableSelf(false);
                    break;
                case state.SHOP:
                    KeyboardControlDungeon.EnableSelf(false);
                    KeyboardControlCombat.EnableSelf(false);
                    KeyboardControlMenu.EnableSelf(false);
                    KeyboardControlShop.EnableSelf(true);
                    break;
            }
        }

        public void Update()
        {
            if (game == null)
            {
                MelonLogger.Msg("GamePhysical is null, trying to assign it");
                game = GamePhysical.instance;
                if (game == null)
                {
                    MelonLogger.Error("GamePhysical is still null, cannot proceed");
                    return;
                }
            }

            if (game.game != null)
            {
                if (game.game.activeShop != null)
                {
                    UpdateState(state.SHOP);
                    return;
                }
            }

            switch (GameManager.gameType)
            {
                case GameType.AIGame:
                    UpdateState(state.COMBAT);
                    break;
                case GameType.Dungeon:
                    UpdateState(state.DUNGEON);
                    break;
                case GameType.MainMenu:
                    UpdateState(state.MENU);
                    break;
                default:
                    UpdateState(state.MENU);
                    break;
            }

        }


    }

    #region Dungeon
    public class KeyboardControlDungeon : MonoBehaviour
    {
        public static KeyboardControlDungeon _instance;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlDungeon");
                _instance = _instance = KeyboardControlsMaster.GO.AddComponent<KeyboardControlDungeon>();

            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlDungeon");
            }

            EnableSelf(false);
        }

        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlDungeon enabled: " + _instance.enabled);
        }

        public void Update()
        {
            if (KeyboardControlsMaster.game.game.dungeon.board == null) return;

            Tile toMoveTo = null;

            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                toMoveTo = KeyboardControlsMaster.game.game.dungeon.board.PlayerTile().Right();
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                toMoveTo = KeyboardControlsMaster.game.game.dungeon.board.PlayerTile().Left();
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                toMoveTo = KeyboardControlsMaster.game.game.dungeon.board.PlayerTile().Up();
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                toMoveTo = KeyboardControlsMaster.game.game.dungeon.board.PlayerTile().Down();
            }

            if (toMoveTo != null)
            {
                KeyboardControlsMaster.game.game.dungeon.board.TryMoveTo(toMoveTo);
                return;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                MelonLogger.Msg("Pressed space");

                if (KeyboardControlsMaster.game.game.dungeon.player.physical.miniDisplay == null)
                {
                    MelonLogger.Msg("No miniDisplay found");
                    return;
                }

                // Try to find any ShopDialogueButton inside miniDisplay
                ShopDialogueButton[] buttons = KeyboardControlsMaster.game.game.dungeon.player.physical.miniDisplay.GetComponentsInChildren<ShopDialogueButton>();

                buttons[0].button.OnMouseDown();
                buttons[0].button.OnMouseUp();
            }

        }
    }
    #endregion

    #region Shop
    public class KeyboardControlShop : MonoBehaviour
    {
        public static KeyboardControlShop _instance;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlShop");
                _instance = KeyboardControlsMaster.GO.AddComponent<KeyboardControlShop>();
            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlShop");
            }

            EnableSelf(false);
        }
        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlShop enabled: " + _instance.enabled);
        }

        private ShopDialogueButton selectedClickable;
        private List<ShopDialogueButton> allClickables;

        public void Update()
        {
            if (KeyboardControlsMaster.game.game.activeShop == null) return;

            allClickables = KeyboardControlsMaster.game.game.activeShop.GetComponentsInChildren<ShopDialogueButton>().ToList();
            if (allClickables.Count == 0) return;

            // WASD directional input
            if (Input.GetKeyDown(KeyCode.W)) {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(3);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(2);
            }

            if (Input.GetKeyUp(KeyCode.Space) && selectedClickable != null)
            {
                MelonLogger.Msg("Pressed space on selected clickable: " + selectedClickable);
                if (selectedClickable is ShopDialogueButton)
                {
                    ShopDialogueButton btn = selectedClickable as ShopDialogueButton;
                    btn.button.OnMouseDown();
                    btn.button.OnMouseUp();
                }
            }
        }

        private ShopDialogueButton ClosestInDirection(int direction)
        {
            // select the closest to selectedClickable from the allClickable list in the specific direction 0 = Up, 1 = Down, 2 = Left, 3 = Right
            if (selectedClickable == null)
            {
                // If nothing is selected, default to the first clickable
                return allClickables.FirstOrDefault();
            }
            Vector2 moveDir = Vector2.zero;
            if (direction == 0)
            {
                moveDir = Vector2.up;
            }
            else if (direction == 1)
            {
                moveDir = new Vector2(0,-1);
            }
            else if (direction == 2)
            {
                moveDir = new Vector2(-1,0);
            }
            else if (direction == 3)
            {
                moveDir = Vector2.right;

            }
            else
            {
                MelonLogger.Error("Invalid direction: " + direction);
                return null;
            }
            Vector2 currentPos = selectedClickable.transform.position;
            ShopDialogueButton best = null;
            float bestDist = float.MaxValue;
            foreach (var btn in allClickables)
            {
                if (btn == selectedClickable) continue;
                Vector2 toTarget = (Vector2)btn.transform.position - currentPos;
                // Project button relative position onto direction
                float dot = Vector2.Dot(toTarget.normalized, moveDir.normalized);
                // Must be in *exact* direction (not perpendicular or behind)
                if (dot <= 0.01f) continue;
                // Reject any buttons not in the same direction
                float axisMatch = Vector2.Dot(toTarget, moveDir);
                if (axisMatch <= 0f) continue;
                // Prefer closest in that direction
                float dist = toTarget.magnitude;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = btn;
                }
            }
            if (best != null)
            {
                // Highlight the new button
                selectedClickable = best;
                MelonLogger.Msg("Selected button in direction " + direction + ": " + best);
                HighlightSelected();
                return best;
            }
            else
            {
                // If no button found in that direction, return the current one
                MelonLogger.Msg("No button found in direction " + direction + ", returning current: " + selectedClickable);
                HighlightSelected();
                return selectedClickable;
            }
        }
        
        private void UnhighlightAll()
        {
            foreach (var btn in allClickables)
            {
                btn.UnHighlight(); // Assuming you have a UnHighlight() method
            }
        }

        private void HighlightSelected()
        {
            foreach (var btn in allClickables)
            {
                if (btn == selectedClickable)
                    btn.Highlight(); // Assuming you have a Highlight(bool) method
                else
                    btn.UnHighlight();
            }
        }

    }
    #endregion

    #region Menu
    public class KeyboardControlMenu : MonoBehaviour
    {
        public static KeyboardControlMenu _instance;
        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlMenu");
                _instance = KeyboardControlsMaster.GO.AddComponent<KeyboardControlMenu>();
            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlMenu");
            }

            EnableSelf(false);
        }
        public static void EnableSelf(bool enable)
        {
            _instance.enabled = enable;
            MelonLogger.Msg("KeyboardControlMenu enabled: " + _instance.enabled);
        }


        private ShopDialogueButton selectedClickable;
        private List<ShopDialogueButton> allClickables;

        public void Update()
        {
            allClickables = FindObjectOfType<ShopDialogueButton>()?.GetComponentsInChildren<ShopDialogueButton>().ToList();
            if (allClickables.Count == 0) return;

            // WASD directional input
            if (Input.GetKeyDown(KeyCode.W))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(3);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                UnhighlightAll();
                selectedClickable = ClosestInDirection(2);
            }

            if (Input.GetKeyUp(KeyCode.Space) && selectedClickable != null)
            {
                MelonLogger.Msg("Pressed space on selected clickable: " + selectedClickable);
                if (selectedClickable is ShopDialogueButton)
                {
                    ShopDialogueButton btn = selectedClickable as ShopDialogueButton;
                    btn.button.OnMouseDown();
                    btn.button.OnMouseUp();
                }
            }
        }

        private ShopDialogueButton ClosestInDirection(int direction)
        {
            // select the closest to selectedClickable from the allClickable list in the specific direction 0 = Up, 1 = Down, 2 = Left, 3 = Right
            if (selectedClickable == null)
            {
                // If nothing is selected, default to the first clickable
                return allClickables.FirstOrDefault();
            }
            Vector2 moveDir = Vector2.zero;
            if (direction == 0)
            {
                moveDir = Vector2.up;
            }
            else if (direction == 1)
            {
                moveDir = new Vector2(0, -1);
            }
            else if (direction == 2)
            {
                moveDir = new Vector2(-1, 0);
            }
            else if (direction == 3)
            {
                moveDir = Vector2.right;

            }
            else
            {
                MelonLogger.Error("Invalid direction: " + direction);
                return null;
            }
            Vector2 currentPos = selectedClickable.transform.position;
            ShopDialogueButton best = null;
            float bestDist = float.MaxValue;
            foreach (var btn in allClickables)
            {
                if (btn == selectedClickable) continue;
                Vector2 toTarget = (Vector2)btn.transform.position - currentPos;
                // Project button relative position onto direction
                float dot = Vector2.Dot(toTarget.normalized, moveDir.normalized);
                // Must be in *exact* direction (not perpendicular or behind)
                if (dot <= 0.01f) continue;
                // Reject any buttons not in the same direction
                float axisMatch = Vector2.Dot(toTarget, moveDir);
                if (axisMatch <= 0f) continue;
                // Prefer closest in that direction
                float dist = toTarget.magnitude;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = btn;
                }
            }
            if (best != null)
            {
                // Highlight the new button
                selectedClickable = best;
                MelonLogger.Msg("Selected button in direction " + direction + ": " + best);
                HighlightSelected();
                return best;
            }
            else
            {
                // If no button found in that direction, return the current one
                MelonLogger.Msg("No button found in direction " + direction + ", returning current: " + selectedClickable);
                HighlightSelected();
                return selectedClickable;
            }
        }

        private void UnhighlightAll()
        {
            foreach (var btn in allClickables)
            {
                btn.UnHighlight(); // Assuming you have a UnHighlight() method
            }
        }

        private void HighlightSelected()
        {
            foreach (var btn in allClickables)
            {
                if (btn == selectedClickable)
                    btn.Highlight(); // Assuming you have a Highlight(bool) method
                else
                    btn.UnHighlight();
            }
        }

    }
    #endregion

    #region Combat
    public class KeyboardControlCombat : MonoBehaviour
    {
        public static KeyboardControlCombat _instance;
        public static Hand playerHand;
        static int highlightedIndex = 0;
        static Card currentSelectedCard;
        static Player player;

        public static void Initialize()
        {
            if (_instance == null)
            {
                MelonLogger.Msg("Initialising KeyboardControlCombat");
                _instance = KeyboardControlsMaster.GO.AddComponent<KeyboardControlCombat>();

            }
            else
            {
                MelonLogger.Msg("Tried initialising a second KeyboardControlCombat");
            }

            EnableSelf(false);
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
            KeyboardControlCombat.AssignHand(__instance.me);
        }
    }

    [HarmonyPatch(typeof(Player), "StartTurnEffects")]
    public static class PatchPlayerStartTurn
    {
        public static void Postfix(Player __instance)
        {
            MelonLogger.Msg("Starting turn");
            if (KeyboardControlCombat.playerHand != null && __instance == __instance.game.me)
                KeyboardControlCombat.OnTurnStart();
        }
    }

    #endregion
}

