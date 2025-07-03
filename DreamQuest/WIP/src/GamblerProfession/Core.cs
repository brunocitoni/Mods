using Boo.Lang.Runtime;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[assembly: MelonInfo(typeof(GamblerProfession.GamblerProfessionInit), "GamblerProfession", "1.0.0", "Bruno", null)]
[assembly: MelonGame(null, null)]

namespace GamblerProfession
{
    public class GamblerProfessionInit : MelonMod
    {
        public static GamblerProfession Gambler;
        public override void OnInitializeMelon()
        {
            Gambler = new GamblerProfession();
            Gambler.Initialize();
        
        LoggerInstance.Msg("Initialized GamblerProfession.");
        }
    }

    public class GamblerProfession : ProfessionBase
    {
        int level = 1;

        ~GamblerProfession()
        {
            MelonLogger.Msg("GamblerProfession destroyed.");
        }

        public override void Initialize()
        {
            this.health = 12;
            this.mana = 0;
            this.cards = 2;
            this.actions = 1;
            this.gold = 5;
            this.deck = new string[]
            {
            "ChaosStrike",
            "ChaosStrike",
            "ChaosStrike",
            "Attack1",
            "Attack1",
            "Attack1",
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
            else if (targetLevel == 3)
            {
                s = typeof(CombatAbilityCoinToss).AssemblyQualifiedName;
                t = LevelUpRewardType.ACTION;
                x = 1;
            }
            else if (targetLevel == 6)
            {
                s = "RegenAbility";
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
            possiblePlayerAttributes.Add(PlayerAttributes.NEGATEFIRST);
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
            MelonLogger.Msg("GamblerProfession destroyed.");
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
            if (this.game.currentDungeon != null)
            {
                if (this.game.currentDungeon.player.gold > 0)
                {
                    this.game.currentDungeon.player.gold = 0;
                    p.DealDamage(UnityEngine.Random.Range(1, p.health), DamageTypes.RAW);
                }
                else
                {
                    MelonLogger.Msg("could not spend the money");
                }
            }
            else
            {
                MelonLogger.Msg("this.game.currentDungeon was null");
            }
        }

        // Token: 0x06000F17 RID: 3863 RVA: 0x000533D4 File Offset: 0x000515D4
        public override bool IsValid()
        {
            return (this.dungeon == null || this.dungeon.currentCombat == null || this.dungeon.currentCombat.me == null || this.dungeon.player.gold >= 6) && this.currentCooldown == 0;
        }

        // Token: 0x06000F18 RID: 3864 RVA: 0x00009D7D File Offset: 0x00007F7D
        public override string FailText()
        {
            return "Not enough coins";
        }
    }


    [HarmonyPatch(typeof(ProfessionList), "AllProfessionList")]
    public static class PatchProfessionList
    {
        public static bool Prefix(ref string[] __result)
        {
            __result = new string[]
            {
            "ProfessionAssassin", "ProfessionBard", "ProfessionDragon", "ProfessionDruid", typeof(GamblerProfession).AssemblyQualifiedName,
            "ProfessionMonk", "ProfessionNecromancer", "ProfessionPaladin", "ProfessionPriest", "ProfessionProfessor",
            "ProfessionRanger", "ProfessionSamurai", "ProfessionThief", "ProfessionWarrior", "ProfessionWizard"
            };
            return false; // skip original method
        }

    }
}
