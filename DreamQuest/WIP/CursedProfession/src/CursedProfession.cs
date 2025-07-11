using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[assembly: MelonInfo(typeof(CursedProfession.CursedProfessionInit), "CursedProfession", "1.0.0", "Bruno", null)]
[assembly: MelonGame(null, null)]

namespace CursedProfession
{
    public class CursedProfessionInit : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized CursedProfession.");
        }
    }

    public class CursedProfession : ProfessionBase
    {
        int cursesInDeck;
        public bool cursePlayable = false;
        int level = 1;

        public override void Initialize()
        {
            this.health = 5;
            this.mana = 2;
            this.cards = 2;
            this.actions = 1;
            this.gold = 0;
            this.deck = new string[]
            {
            "Curse",
            "Curse",
            "Curse",
            typeof(BlindPassion).AssemblyQualifiedName,
            typeof(BlindRage).AssemblyQualifiedName,
            "FlameCharge",
            "StaticCharge"
            };

            this.internalName = typeof(CursedProfession).AssemblyQualifiedName;
            base.Initialize();

            //RemoveAttack2();
            CountDeckCurses();
        }

        
        // TODO
        private void RemoveAttack2()
        {
            foreach (var card in this.deck)
            {
                this.game.currentDungeon.player.RemoveCardFromDeck("Attack2");
                this.game.currentDungeon.player.RemoveCardFromDeck("Attack2");
            }
        }
        

        private void CountDeckCurses()
        {
            cursesInDeck = 0;
            foreach (var card in this.deck)
            {
                if (card == "Curse")
                    cursesInDeck++;
            }
        }

        public override void AddDungeonActions(DungeonPlayer d)
        {

        }

        // used for the menu only
        public override Texture GetBigTexture()
        {
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "CursedProfession");
            string path = Path.Combine(folder, "cursedBig.png");

            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage
            if (tex.LoadImage(imageData))
            {
                MelonLogger.Msg("texture loaded successfully.");
                return tex;
            }
            else
            {
                MelonLogger.Msg("failed to load texture from image data.");
                return null;
            }
        }

        // needed by DungeonPlayer
        public override Texture GetLittleTexture()
        {
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "CursedProfession");
            string path = Path.Combine(folder, "cursedPortrait.png");

            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage

            if (tex.LoadImage(imageData))
            {
                MelonLogger.Msg("texture loaded successfully.");
                return tex;
            }
            else
            {
                MelonLogger.Msg("failed to load texture from image data.");
                return null;
            }
        }


        public override string ClassName()
        {
            return "Cursed";
        }

        public override string Description()
        {
            return "A cursed child of destiny. Blind, mauled and stunted physically, you drew strenght from the same darkness you were cast out in.";
        }

        public override string AbilityDescription()
        {
            return "Cursed Blood: You have health regeneration equal to half the amount of curses in your base deck, rounded down. Enemies have 75% innate dodge.";
        }

        public override string[] GetPossibleNames()
        {
            return new string[]
            {
"Caleb, the Corrupted",

"Silverius, the Scorned",

"Moran, the Marked"
            };
        }

        // Token: 0x06002092 RID: 8338 RVA: 0x00016A0C File Offset: 0x00014C0C
        public override ClassBias[] ClassBiases()
        {
            return new ClassBias[]
            {
                ClassBias.WIZARD,
                ClassBias.PRIEST
            };
        }

        // TODO
        public override List<LevelUpReward> SpecificLevelUpBonuses(int targetLevel)
        {
            List<LevelUpReward> list = new List<LevelUpReward>();
            list.Add(new LevelUpReward("Action", 1, LevelUpRewardType.SPECIAL, this.dungeon));
            list.Add(new LevelUpReward("Card1", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Card1", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Card2", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Card2", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Card3", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Card3", 1, LevelUpRewardType.RANDOM_CARD, this.dungeon));
            list.Add(new LevelUpReward("Delete", 1, LevelUpRewardType.SPECIAL, this.dungeon));
            int num = 3;
            if (targetLevel >= 8)
            {
                num = 8;
            }
            else if (targetLevel >= 5)
            {
                num = 5;
            }
            num += 2;
            list.Add(new LevelUpReward("Health", num, LevelUpRewardType.SPECIAL, this.dungeon));
            float num2 = Game.RandomFloat();
            if (num2 < 0.5f)
            {
                list.Add(new LevelUpReward("Mana", 3, LevelUpRewardType.SPECIAL, this.dungeon));
            }
            return list;
        }

        // TODO
        public override LevelUpReward SpecificLevelUpPrimaryReward(int targetLevel)
        {
            int x = 0;
            LevelUpRewardType t = LevelUpRewardType.CARD;
            string s = string.Empty;
            if (targetLevel == 7)
            {
                s = "Card";
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }
            else if (targetLevel == 5)
            {
                s = "Card";
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }
            else if (targetLevel == 10)
            {
                s = "Talent";
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }
            else if (targetLevel == 3)
            {
                s = typeof(CombatAbilityCursedStrenght).AssemblyQualifiedName;
                t = LevelUpRewardType.ACTION;
                x = 1;
            }
            ;
            level = targetLevel;
            return new LevelUpReward(s, x, t, this.dungeon);
        }

        // TODO
        public override List<string> RandomCards(int targetLevel)
        {
            return new List<string>(new string[]
            {
            "Solidify",
            "CurseOfWeakness",
            "FrostShape",
            "Study",
            "LastChance",
            "Slice",
            "KineticCharge",
            "Channel",
            "Fly",
            "ElementalSurge"
            });
        }

        // TODO
        public virtual void TieredCards(List<string> l, int cardTier)
        {
            this.AttackCards(l, cardTier);
        }

        // TODO
        public override List<string> HighCards()
        {
            int cardTier = 3;
            List<string> list = new List<string>();
            this.TieredCards(list, cardTier);
            return list;
        }

        // TODO
        public override List<string> MidCards()
        {
            int cardTier = 2;
            List<string> list = new List<string>();
            this.TieredCards(list, cardTier);
            return list;
        }

        // TODO
        public override List<string> LowCards()
        {
            int cardTier = 1;
            List<string> list = new List<string>();
            this.TieredCards(list, cardTier);
            return list;
        }

        // TODO
        public override int LevelUpHealth(int targetLevel)
        {
            return 2;
        }

        // TODO
        public override int RewardWeight(CardData c)
        {
            return (int)Math.Max(c.thief, c.warrior);
        }

        public override void CombatApplyToPlayer(Player p)
        {
            CountDeckCurses();
            MelonLogger.Msg("Curses in deck " + cursesInDeck);
            MelonLogger.Msg("Adding helth regen for " + Mathf.FloorToInt((float)cursesInDeck / 2));
            p.AddToAttribute(PlayerAttributes.HEALTH_REGEN, Mathf.FloorToInt((float)cursesInDeck /2));
            p.Enemy().AddToAttribute(PlayerAttributes.DODGE, 75);
        }

        public void SetCursePlayable(bool playable)
        {
            cursePlayable = playable;
        }
    }

    #region CombatAbilities
    public class CombatAbilityCursedStrenght : CombatAbility
    {
        // Token: 0x06000EA7 RID: 3751 RVA: 0x00044800 File Offset: 0x00042A00
        public override void Initialize()
        {
            base.Initialize();
            this.cooldown = 1;
            this.currentCooldown = 0;
        }

        // Token: 0x06000EA8 RID: 3752 RVA: 0x00044818 File Offset: 0x00042A18
        public override Texture GetTexture()
        {
            return (Texture)Resources.Load("Textures/CombatAbilityAvoidance", typeof(Texture));
        }

        // Token: 0x06000EA9 RID: 3753 RVA: 0x00044834 File Offset: 0x00042A34
        public override string Description()
        {
            return "This turn you can play curse cards to deal Level damage each";
        }

        // Token: 0x06000EAA RID: 3754 RVA: 0x0004483C File Offset: 0x00042A3C
        public override string Name()
        {
            return "Cursed Strenght";
        }

        // Token: 0x06000EAB RID: 3755 RVA: 0x00044844 File Offset: 0x00042A44
        public override void DoMe(Player p)
        {
            CursedProfession profession = p.game.currentDungeon.player.profession as CursedProfession;
            profession?.SetCursePlayable(true);
        }
    }

    [HarmonyPatch(typeof(Player), "StartTurnEffects")]
    public static class PatchStartTurnEffects
    {
        public static void Postfix(Player __instance)
        {
            CursedProfession profession = __instance.game.currentDungeon.player.profession as CursedProfession;
            profession?.SetCursePlayable(false);
        }
    }

    #endregion
    #region CARDS
    public class BlindPassion : SpellCard
    {
        Texture cardTexture;

        // Token: 0x060004FA RID: 1274 RVA: 0x0001D7DC File Offset: 0x0001B9DC
        public override void Initialize()
        {
            this.PythonInitialize();
        }

        // Token: 0x060004FB RID: 1275 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
        public override void PlayEffect()
        {
            this.PythonPlayEffect();
        }

        // Token: 0x060004FC RID: 1276 RVA: 0x0001D7EC File Offset: 0x0001B9EC
        public virtual void PythonInitialize()
        {
            this.internalName = typeof(BlindPassion).AssemblyQualifiedName;
            this.cardName = "Blind Passion";
            this.text = "Draw 2 cards, then discard a card. If you discard a curse this way, heal 5 damage. Overhealing becomes a shield.";
            this.flavorText = "The secret does seem to be hard work, yes, but it's also a kind of blind passion, an inspiration. - TES: Morrowind";
            this.cost = 0;
            this.goldCost = 35;
            this.manaCost = 4;
            this.level = 1;
            this.maxLevel = 1;
            this.tier = 1;
            this.decayTo = string.Empty;
            base.Initialize();
            this.AIKeepValue = 5;
            this.AIPlaySequence = 50;
            this.cardTexture = GetTextureFromPath(ImageName());
            if (this.IsPhysical() && this.physical)
            {
                this.physical.ChangeTextureNow(GetTextureFromPath(ImageName()));
            }
        }

        // Token: 0x060004FD RID: 1277 RVA: 0x0001D864 File Offset: 0x0001BA64
        public void PythonPlayEffect()
        {
            this.Draw(2);

            // Temporary handler to listen for discard
            void HandleDiscard(Card card)
            {
                MelonLogger.Msg("Inside handle discard");
                if (card is Curse)
                {
                    int num = this.Heal(5);
                    num = 5 - num;
                    if (num > 0)
                    {
                        this.PlayerShield(num);
                    }
                }
                else
                {
                    MelonLogger.Msg("Discarded a non-curse card");
                }

                // Unsubscribe after discard
                DiscardEventSystem.CardDiscarded -= HandleDiscard;
                MelonLogger.Msg("Unsubscribed to handle discard");
            }

            // Subscribe before discarding
            DiscardEventSystem.CardDiscarded += HandleDiscard;

            // Trigger the discard
            this.player.Discard(1);
        }
        

        public override string ImageName()
        {
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "CursedProfession");
            string path = Path.Combine(folder, "BlindPassion.png");
            return path;
        }

        private Texture GetTextureFromPath(string path)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage
            if (tex.LoadImage(imageData))
            {
                return tex;
            }
            else
            {
                MelonLogger.Msg("Card texture failed to load from image data.");
                return null;
            }
        }
    }

    public class BlindRage : SpellCard
    {
        Texture cardTexture;

        // Token: 0x060004FA RID: 1274 RVA: 0x0001D7DC File Offset: 0x0001B9DC
        public override void Initialize()
        {
            this.PythonInitialize();
        }

        // Token: 0x060004FB RID: 1275 RVA: 0x0001D7E4 File Offset: 0x0001B9E4
        public override void PlayEffect()
        {
            this.PythonPlayEffect();
        }

        // Token: 0x060004FC RID: 1276 RVA: 0x0001D7EC File Offset: 0x0001B9EC
        public virtual void PythonInitialize()
        {
            this.internalName = typeof(BlindRage).AssemblyQualifiedName;
            this.cardName = "Blind Rage";
            this.text = "Draw 2 cards, then discard a card. If you discard a curse this way, deal 5 piercing damage.";
            this.flavorText = "ira initium insaniae est - Seneca";
            this.cost = 0;
            this.goldCost = 35;
            this.manaCost = 4;
            this.level = 1;
            this.maxLevel = 1;
            this.tier = 1;
            this.decayTo = string.Empty;
            base.Initialize();
            this.AIKeepValue = 5;
            this.AIPlaySequence = 50;
            this.cardTexture = GetTextureFromPath(ImageName());
            if (this.IsPhysical() && this.physical)
            {
                this.physical.ChangeTextureNow(GetTextureFromPath(ImageName()));
            }
        }

        // Token: 0x060004FD RID: 1277 RVA: 0x0001D864 File Offset: 0x0001BA64
        public void PythonPlayEffect()
        {
            this.Draw(2);

            // Temporary handler to listen for discard
            void HandleDiscard(Card card)
            {
                MelonLogger.Msg("Inside handle discard");
                if (card is Curse)
                {
                    int num = this.DealDamage(5,DamageTypes.RAW);
                }
                else
                {
                    MelonLogger.Msg("Discarded a non-curse card");
                }

                // Unsubscribe after discard
                DiscardEventSystem.CardDiscarded -= HandleDiscard;
                MelonLogger.Msg("Unsubscribed to handle discard");
            }

            // Subscribe before discarding
            DiscardEventSystem.CardDiscarded += HandleDiscard;

            // Trigger the discard
            this.player.Discard(1);
        }


        public override string ImageName()
        {
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "CursedProfession");
            string path = Path.Combine(folder, "BlindRage.png");
            return path;
        }

        private Texture GetTextureFromPath(string path)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage
            if (tex.LoadImage(imageData))
            {
                return tex;
            }
            else
            {
                MelonLogger.Msg("Card texture failed to load from image data.");
                return null;
            }
        }
    }

    #endregion

    // discard effects
    [HarmonyPatch(typeof(Card), "Discard")]
    public static class Card_Discard_Patch
    {
        static void Postfix(Card __instance)
        {
            DiscardEventSystem.OnCardDiscarded(__instance);
        }
    }

    public static class DiscardEventSystem
    {
        public static event Action<Card> CardDiscarded;

        public static void OnCardDiscarded(Card card)
        {
            CardDiscarded?.Invoke(card);
        }
    }

    [HarmonyPatch(typeof(CardList), "GetCardList")]
    public static class PatchGetCardList
    {
        public static void Postfix(CardList __instance)
        {
            CardList.cardList.Add(new CardData(typeof(BlindPassion).AssemblyQualifiedName, new PreferenceWrapper[]
                        {
                new PreferenceWrapper(ClassBias.WIZARD, BiasType.FREQUENCY, 1)
                        }, 30, new UserAttribute[0], false, null, (float)3, (float)1, (float)6, (float)1, 4, 2, "SpellCard", DamageTypes.NONE, "Blind Passion"));

            CardList.cardList.Add(new CardData(typeof(BlindRage).AssemblyQualifiedName, new PreferenceWrapper[]
            {
                new PreferenceWrapper(ClassBias.WIZARD, BiasType.FREQUENCY, 1)
            }, 30, new UserAttribute[0], false, null, (float)3, (float)1, (float)6, (float)1, 4, 2, "SpellCard", DamageTypes.RAW, "Blind Rage"));
        }
    }

    [HarmonyPatch(typeof(ProfessionList), "AllProfessionList")]
    public static class PatchProfessionList
    {
        public static void Postfix(ref string[] __result)
        {
            // Create a new array with one extra slot
            var newList = new string[__result.Length + 1];
            __result.CopyTo(newList, 0);

            // Add your custom profession at the end
            newList[__result.Length] = typeof(CursedProfession).AssemblyQualifiedName;

            // Set the modified result
            __result = newList;
        }
    }

    [HarmonyPatch(typeof(Curse), "IsPlayValid")]
    public static class PatchCursePlaying
    {
        public static bool Prefix(Curse __instance, ref bool __result)
        {
            MelonLogger.Msg("Inside IsPlayValid Prefix");

            if ((__instance.player.game.currentDungeon.player.profession.internalName == typeof(CursedProfession).AssemblyQualifiedName))
            {
                CursedProfession profession = __instance.player.game.currentDungeon.player.profession as CursedProfession;
                bool cursePlayable = profession.cursePlayable;

                if (cursePlayable)
                {
                    MelonLogger.Msg("Returning true");
                    __result = true; // set the return value
                    return false;    // skip original method
                }
            }

            MelonLogger.Msg("Returning false");

            __result = false; // set the return value
            return false;     // skip original method
        }
    }


    [HarmonyPatch(typeof(Curse), "PlayEffect")]
    public static class PatchCurseEffect
    {
        public static bool Prefix(Curse __instance)
        {
            MelonLogger.Msg("Inside PlayEffect Prefix");

            __instance.DealDamage(__instance.player.game.currentDungeon.player.level, DamageTypes.RAW);
            return false;
        }
    }
}
