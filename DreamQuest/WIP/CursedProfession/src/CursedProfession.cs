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
        int level = 1;

        public override void Initialize()
        {
            this.health = 10;
            this.mana = 2;
            this.cards = 2;
            this.actions = 1;
            this.gold = 0;
            this.deck = new string[]
            {
            "Curse",
            "Curse",
            "EarthShape",
            "FrostShape",
            "FlameCharge",
            "StaticCharge"
            };

            this.internalName = typeof(CursedProfession).AssemblyQualifiedName;
            base.Initialize();

            CountDeckCurses();
        }

        private void CountDeckCurses()
        {
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
            return "A cursed child of destiny. Blind, mauled and stunted physically, you found strenght in the same darkness that you were cast out in";
        }

        public override string AbilityDescription()
        {
            return "Cursed Blood: You can play curses to deal piercing damage equal to your level. You have health regeneration equal to the amount of curses in your base deck. Enemies have 50% innate dodge.";
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
                s = "Card";
                t = LevelUpRewardType.SPECIAL;
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
            "InnerStrength",
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

        // TODO
        public override void CombatApplyToPlayer(Player p)
        {
            MelonLogger.Msg("Adding helth regen for " + cursesInDeck);
            p.AddToAttribute(PlayerAttributes.HEALTH_REGEN, cursesInDeck);
            p.Enemy().AddToAttribute(PlayerAttributes.DODGE, 50);
        }
    }

    #region CombatAbilities
    #endregion

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

            if (__instance.player.game.currentDungeon.player.profession.internalName == typeof(CursedProfession).AssemblyQualifiedName)
            {
                MelonLogger.Msg("Returning true");

                __result = true; // set the return value
                return false;    // skip original method
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