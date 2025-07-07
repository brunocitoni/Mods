using Boo.Lang.Runtime;
using GamblerProfession;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        List<Dictionary<PlayerAttributes, int>> possibleAttributes = new();
        float[] weights;
        Dictionary<PlayerAttributes, int> appliedToPlayer;
        Dictionary<PlayerAttributes, int> appliedToEnemy;

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
            "Pickpocket",
            "LastChance",
            "ChaosPrayer"
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
            if (targetLevel == 7)
            {
                s = typeof(CoinToss).AssemblyQualifiedName;
                t = LevelUpRewardType.SPECIAL;
                x = 1;
            }
            else if (targetLevel == 4)
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
                s = typeof(CombatAbilityGamble).AssemblyQualifiedName;
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
        public override List<string> RandomCards(int targetLevel)
        {
            return new List<string>(new string[]
            {
            "Jab",
            "Meditation",
            "InnerStrength",
            "Study",
            typeof(DoubleRoll).AssemblyQualifiedName,
            "LastChance",
            "Slice",
            "Dice",
            "Backstab",
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

        private void InitDictionaryAndWeights()
        {
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.PHYS_IMMUNE, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.REFLECT, level * 2 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.PENANCE, level } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.WEAKNESS, level } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.EARTH_IMMUNE, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.WATER_IMMUNE, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.HEALTH_REGEN, level } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.FIRE_VULN, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.AIR_VULN, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.SHIELD, level * 2 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.EARTH_VULN, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.WATER_VULN, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.PHYS_RESIST, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.FIRE_IMMUNE, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.AIR_IMMUNE, 1 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.POISON, level } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.DODGE, level * 5 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.SUPER_FIRE_SHIELD, level } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.BASE_CHILLED, level % 3 } });
            possibleAttributes.Add(new Dictionary<PlayerAttributes, int> { { PlayerAttributes.BERSERK, level % 5 } });

            weights = new float[19]
            {
    0.020000f,
    0.029169f,
    0.041741f,
    0.058170f,
    0.077998f,
    0.099818f,
    0.121969f,
    0.142290f,
    0.158842f,
    0.170122f,
    0.158842f,
    0.142290f,
    0.121969f,
    0.099818f,
    0.077998f,
    0.058170f,
    0.041741f,
    0.029169f,
    0.020000f
            };

        }

        // TODO
        public override void CombatApplyToPlayer(Player p)
        {
            InitDictionaryAndWeights();
            ApplyNewBuff(p);
        }

        void RemoveAppliedBuff(Player p)
        {
            p.AddToAttribute(
                appliedToPlayer.First().Key,
                -appliedToPlayer.First().Value
            );

            p.Enemy().AddToAttribute(
                appliedToEnemy.First().Key,
                -appliedToEnemy.First().Value
            );
        }

        void ApplyNewBuff(Player p)
        {
            int playerBuffIndex = GetRandomWeightedIndex(weights);
            int enemyBuffIndex = GetRandomWeightedIndex(weights);

            MelonLogger.Msg("index player buff : " + playerBuffIndex);
            if (playerBuffIndex > possibleAttributes.Count)
                MelonLogger.Msg("SOMETHING WENT WRONG WITH playerBuffIndex: " + playerBuffIndex);
            p.AddToAttribute(
                possibleAttributes[playerBuffIndex].First().Key,
                possibleAttributes[playerBuffIndex].First().Value
            );

            MelonLogger.Msg("assigned player buff : " + possibleAttributes[playerBuffIndex].First().Key);
            appliedToPlayer = new(possibleAttributes[playerBuffIndex]);

            if (enemyBuffIndex > possibleAttributes.Count)
                MelonLogger.Msg("SOMETHING WENT WRONG WITH enemyBuffIndex: " + enemyBuffIndex);

            MelonLogger.Msg("index enemy buff : " + enemyBuffIndex);
            p.Enemy().AddToAttribute(
                possibleAttributes[enemyBuffIndex].First().Key,
                possibleAttributes[enemyBuffIndex].First().Value
            );
            MelonLogger.Msg("assigned enemy buff : " + possibleAttributes[enemyBuffIndex].First().Key);
            appliedToEnemy = new(possibleAttributes[enemyBuffIndex]);
        }

        public void RerollBuffs(Player p)
        {
            RemoveAppliedBuff(p);
            ApplyNewBuff(p);
        }

        int GetRandomWeightedIndex(float[] weights)
        {
            if (weights == null || weights.Length == 0) return -1;

            float totalWeight = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                float w = weights[i];
                if (float.IsPositiveInfinity(w)) return i;
                if (w >= 0f && !float.IsNaN(w)) totalWeight += w;
            }

            if (totalWeight <= 0f) return -1;

            float r = UnityEngine.Random.value;
            float cumulative = 0f;

            for (int i = 0; i < weights.Length; i++)
            {
                float w = weights[i];
                if (w <= 0f || float.IsNaN(w)) continue;

                cumulative += w / totalWeight;
                if (r < cumulative)
                {
                    return i;
                }
            }

            // Fallback in case of floating point rounding errors
            return weights.Length - 1;
        }

    }

    #region CombatAbilities
    public class CombatAbilityGamble : CombatAbility
    {
        ~CombatAbilityGamble()
        {
            MelonLogger.Msg("CombatAbilityGamble destroyed.");
        }

        public override void Initialize()
        {
            base.Initialize();
            this.cooldown = 0;
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
            return "Spend 5 gold to reroll buffs on you and the enemy";
        }

        // Token: 0x06000F15 RID: 3861 RVA: 0x00009D66 File Offset: 0x00007F66
        public override string Name()
        {
            return "Gamble!";
        }

        // Token: 0x06000F16 RID: 3862 RVA: 0x00009D6D File Offset: 0x00007F6D
        public override void DoMe(Player p)
        {
            if (this.dungeon.currentCombat.currentDungeon != null)
            {
                if (this.dungeon.currentCombat.currentDungeon.player.gold >= 5)
                {
                    this.dungeon.currentCombat.currentDungeon.player.gold -= 5;
                    // TODO invoke reroll buffs
                    GamblerProfession gambler = this.dungeon.player.profession as GamblerProfession;
                    gambler.RerollBuffs(p);
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
            return (this.dungeon == null || this.dungeon.currentCombat == null || this.dungeon.currentCombat.me == null || this.dungeon.currentCombat.currentDungeon.player.gold >= 5) && this.currentCooldown == 0;
        }

        // Token: 0x06000F18 RID: 3864 RVA: 0x00009D7D File Offset: 0x00007F7D
        public override string FailText()
        {
            return "Not enough coins";
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

    #endregion
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
            this.flavorText = "COULD GO YOUR WAY, COULD GO MINE. - Alan Partridge";
            this.cost = 0;
            this.goldCost = 20;
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
                this.physical.ChangeTextureNow(GetTextureFromPath(ImageName()));
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
            string folder = Path.Combine(MelonEnvironment.UserDataDirectory, "GamblerProfession");
            string path = Path.Combine(folder, "DoubleRoll.png");
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