using Boo.Lang.Runtime;
using GamblerProfession;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[assembly: MelonInfo(typeof(GamblerProfession.GamblerProfessionInit), "GamblerProfession", "1.0.0", "Bruno", null)]
[assembly: MelonGame(null, null)]

namespace GamblerProfession
{
    public class GamblerProfessionInit : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized GamblerProfession.");
        }
    }

    public class GamblerProfession : ProfessionBase
    {
        int level = 1;

        public override void Initialize()
        {
            this.health = 12;
            this.mana = 0;
            this.cards = 2;
            this.actions = 1;
            this.gold = 5;
            this.deck = new string[]
            {
            typeof(DoubleRoll).AssemblyQualifiedName,
            "ChaosStrike",
            "ChaosStrike",
            "ChaosStrike",
            "ChaosStrike",
            "Pickpocket",
            "LastChance",
            "ChaosPrayer",
            "ElementalSurge"

            };
            this.internalName = typeof(GamblerProfession).AssemblyQualifiedName;
            base.Initialize();
        }

        public override void AddDungeonActions(DungeonPlayer d)
        {

        }

        // used for the menu only
        public override Texture GetBigTexture()
        {
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "GamblerProfession");
            string path = Path.Combine(folder, "gamblerBig.png");

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
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "GamblerProfession");
            string path = Path.Combine(folder, "gamblerPortrait.png");

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
            return "Gambler";
        }

        public override string Description()
        {
            return "A reformed Chaos Disciple, cast out of the sect. Trying to find their way in the world, they fell into the dangers of high-stakes gambling.";
        }

        public override string AbilityDescription()
        {
            return "Gamble: You and the enemy start every combat with a random buff.";
        }

        public override string[] GetPossibleNames()
        {
            return new string[]
            {
            "Pablo, the Pourous",
            "Artur, the Adverse",
            "Carl, the Chaste"
            };
        }

        // Token: 0x06002092 RID: 8338 RVA: 0x00016A0C File Offset: 0x00014C0C
        public override ClassBias[] ClassBiases()
        {
            return new ClassBias[]
            {
                ClassBias.ASSASSIN,
                ClassBias.WARRIOR
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
            if (targetLevel == 4)
            {
                s = "Card";
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }
            else if (targetLevel == 7)
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
            else if (targetLevel == 2)
            {
                s = typeof(CombatAbilityCoinToss).AssemblyQualifiedName;
                t = LevelUpRewardType.ACTION;
                x = 1;
            }
            else if (targetLevel == 6)
            {
                s = "Talent";
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }

            level = targetLevel;
            return new LevelUpReward(s, x, t, this.dungeon);
        }

        // TODO
        public override LevelUpReward FixedBonus(int targetLevel)
        {
            return (targetLevel != 3) ? null : new LevelUpReward("Action", 1, LevelUpRewardType.SPECIAL, this.dungeon);
        }

        // TODO
        public override List<string> RandomCards(int targetLevel)
        {
            return new List<string>(new string[]
            {
            "Jab",
            "Circle",
            "Meditation",
            "InnerStrength",
            "Study",
            "Focus",
            "LastChance",
            "Slice",
            "Dice",
            "Backstab",
            "Fly"
            });
        }

        // TODO
        public virtual void TieredCards(List<string> l, int cardTier)
        {
            this.WizardCards(l, cardTier);
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
            List<PlayerAttributes> possiblePlayerAttributes = new();
            List<PlayerAttributes> possibleEnemyAttributes = new();

            // general ones go here
            possiblePlayerAttributes.Add(PlayerAttributes.SHIELD);

            possiblePlayerAttributes.Add(PlayerAttributes.HEALTH_REGEN);

            // level dependant ones go here
            int playerLevel = level;

            for (int currentLevel = 2; currentLevel <= playerLevel && currentLevel <= 10; currentLevel++)
            {
                switch (currentLevel)
                {
                    case 2:
                        possiblePlayerAttributes.Add(PlayerAttributes.AIR_VULN);
                        possiblePlayerAttributes.Add(PlayerAttributes.EARTH_VULN);
                        possiblePlayerAttributes.Add(PlayerAttributes.FIRE_VULN);
                        possiblePlayerAttributes.Add(PlayerAttributes.WATER_VULN);
                        break;
                    case 3:
                        possiblePlayerAttributes.Add(PlayerAttributes.PHYS_RESIST);
                        possiblePlayerAttributes.Add(PlayerAttributes.ELEMENTAL_RESIST);

                        break;
                    case 4:
                        break;
                    case 5:
                        possiblePlayerAttributes.Add(PlayerAttributes.CRUEL);
                        possiblePlayerAttributes.Add(PlayerAttributes.POISON);
                        possiblePlayerAttributes.Add(PlayerAttributes.MIND_IMMUNE);
                        break;
                    case 6:
                        possiblePlayerAttributes.Add(PlayerAttributes.DODGE);
                        possiblePlayerAttributes.Add(PlayerAttributes.PERM_NEGATE_FIRST);
                        possiblePlayerAttributes.Add(PlayerAttributes.REFLECT);
                        break;
                    case 7:
                        possiblePlayerAttributes.Add(PlayerAttributes.DOUBLE_ATTACK);
                        possiblePlayerAttributes.Add(PlayerAttributes.INVISIBLE);
                        break;
                    case 8:
                        possiblePlayerAttributes.Add(PlayerAttributes.RANDOM_IMMUNITY);
                        possiblePlayerAttributes.Add(PlayerAttributes.EARTH_IMMUNE);
                        possiblePlayerAttributes.Add(PlayerAttributes.FIRE_IMMUNE);
                        possiblePlayerAttributes.Add(PlayerAttributes.WATER_IMMUNE);
                        possiblePlayerAttributes.Add(PlayerAttributes.AIR_IMMUNE);
                        break;
                    case 9:
                        possiblePlayerAttributes.Add(PlayerAttributes.FLUID);

                        break;
                    case 10:
                        possiblePlayerAttributes.Add(PlayerAttributes.DOUBLE_DAMAGE);
                        break;
                }
            }

            possibleEnemyAttributes = new(possiblePlayerAttributes);
            possibleEnemyAttributes.Add(PlayerAttributes.NEGATEFIRST);

            int counter = 1;
            if (level >= 4)
            {
                counter = 2;
            }

            for (int i = 0; i < counter; i++)
            {

                PlayerAttributes playerBuff = possiblePlayerAttributes[p.game.InGameRandomRange(0, possiblePlayerAttributes.Count - 1)];

                MelonLogger.Msg("Adding " + playerBuff + " for value " + level + " to player");
                p.AddToAttribute(playerBuff, level);
                possiblePlayerAttributes.Remove(playerBuff);


                PlayerAttributes enemyBuff = possibleEnemyAttributes[p.game.InGameRandomRange(0, possibleEnemyAttributes.Count - 1)];

                MelonLogger.Msg("Adding " + enemyBuff + " for value " + level + " to enemy");
                p.Enemy().AddToAttribute(enemyBuff, level);
                possibleEnemyAttributes.Remove(enemyBuff);
            }

        }

    }

    public class CombatAbilityCoinToss : CombatAbility
    {
        ~CombatAbilityCoinToss()
        {
            MelonLogger.Msg("CombatAbilityCoinToss destroyed.");
        }

        public override void Initialize()
        {
            base.Initialize();
            this.cooldown = 2;
            this.currentCooldown = 0;
        }

        // Token: 0x06000F13 RID: 3859 RVA: 0x00009D44 File Offset: 0x00007F44
        public override Texture GetTexture()
        {
            return (Texture)Resources.Load("Textures/CombatAbilityDesperatePrayer", typeof(Texture));
        }

        // Token: 0x06000F14 RID: 3860 RVA: 0x00009D5F File Offset: 0x00007F5F
        public override string Description()
        {
            return "Use all your gold to deal between 1 and total amount in piercing damage";
        }

        // Token: 0x06000F15 RID: 3861 RVA: 0x00009D66 File Offset: 0x00007F66
        public override string Name()
        {
            return "Coin Toss";
        }

        // Token: 0x06000F16 RID: 3862 RVA: 0x00009D6D File Offset: 0x00007F6D
        public override void DoMe(Player p)
        {
            if (this.dungeon.currentCombat.currentDungeon != null)
            {
                if (this.dungeon.currentCombat.currentDungeon.player.gold > 0)
                {
                    int num = this.dungeon.currentCombat.currentDungeon.player.gold;
                    this.dungeon.currentCombat.currentDungeon.player.gold = 0;
                    p.DealDamage(UnityEngine.Random.Range(1, num), DamageTypes.RAW);
                }
                else
                {
                    MelonLogger.Msg("could not spend the money");
                }
            }
            else
            {
                MelonLogger.Msg("this.dungeon.currentCombat.currentDungeon was null");
            }
        }

        // Token: 0x06000F17 RID: 3863 RVA: 0x000533D4 File Offset: 0x000515D4
        public override bool IsValid()
        {
            return (this.dungeon == null || this.dungeon.currentCombat == null || this.dungeon.currentCombat.me == null || this.dungeon.currentCombat.currentDungeon.player.gold > 0) && this.currentCooldown == 0;
        }

        // Token: 0x06000F18 RID: 3864 RVA: 0x00009D7D File Offset: 0x00007F7D
        public override string FailText()
        {
            return "Not enough coins";
        }
    }

    #region CARDS

    public class DoubleRoll : AttackCard
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
            this.internalName = typeof(DoubleRoll).AssemblyQualifiedName;
            this.cardName = "Double Roll";
            this.text = "Deal 1-6 @atk damage to you. Deal 1-6 damage to the opponent";
            this.flavorText = "Could go both ways";
            this.cost = 0;
            this.goldCost = 150;
            this.manaCost = 0;
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
                MelonLogger.Msg("is was physical!");
                this.physical.ChangeTextureNow(GetTextureFromPath(ImageName()));
            }
            else
            {
                MelonLogger.Msg("is was NOT physical!");
            }

        }

        // Token: 0x060004FD RID: 1277 RVA: 0x0001D864 File Offset: 0x0001BA64
        public virtual void PythonPlayEffect()
        {
            int num1 = UnityEngine.Random.Range(1, 6);
            int num2 = UnityEngine.Random.Range(1, 6);
            this.DamageSelf(num1, DamageTypes.PHYSICAL);
            this.DealDamage(num2, DamageTypes.PHYSICAL);
        }

        public override string ImageName()
        {
            MelonLogger.Msg("Image name called");
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "GamblerProfession");
            string path = Path.Combine(folder, "DoubleRoll.png");
            MelonLogger.Msg(path);
            return path;
        }

        private Texture GetTextureFromPath(string path)
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage
            if (tex.LoadImage(imageData))
            {
                MelonLogger.Msg("Card texture loaded successfully.");
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



    [HarmonyPatch(typeof(CardList), "GetCardList")]
    public static class PatchGetCardList
    {
        public static void Postfix(CardList __instance)
        {
            CardList.cardList.Add(new CardData(typeof(DoubleRoll).AssemblyQualifiedName, new PreferenceWrapper[]
                        {
                new PreferenceWrapper(ClassBias.WARRIOR, BiasType.FREQUENCY, 1)
                        }, 30, new UserAttribute[0], false, null, (float)3, (float)1, (float)6, (float)1, 4, 2, "AttackCard", DamageTypes.PHYSICAL, "Double Roll"));
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
            newList[__result.Length] = typeof(GamblerProfession).AssemblyQualifiedName;

            // Set the modified result
            __result = newList;
        }
    }
}
