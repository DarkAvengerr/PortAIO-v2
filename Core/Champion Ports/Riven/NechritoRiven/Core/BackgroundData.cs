using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    #endregion

    /// <summary>
    /// The core.
    /// </summary>
    internal class BackgroundData : Core
    {
        #region Static Fields

        private static AttackableUnit Unit { get; set; }

        private static bool doublecastQ;

        private static bool canQ;

        private static bool canW;

        /// <summary>
        ///     The e anti spell.
        /// </summary>
        public static List<string> AntigapclosingSpells = new List<string>
                                                    {
                                                        "MonkeyKingSpinToWin", "KatarinaRTrigger", "HungeringStrike",
                                                        "TwitchEParticle", "RengarPassiveBuffDashAADummy",
                                                        "RengarPassiveBuffDash", "IreliaEquilibriumStrike",
                                                        "BraumBasicAttackPassiveOverride", "gnarwproc",
                                                        "hecarimrampattack", "illaoiwattack", "JaxEmpowerTwo",
                                                        "JayceThunderingBlow", "RenektonSuperExecute",
                                                        "vaynesilvereddebuff"
                                                    };

        /// <summary>
        ///     The targeted anti spell.
        /// </summary>
        public static List<string> TargetedSpells = new List<string>
                                                           {
                                                               "MonkeyKingQAttack", "YasuoDash", "FizzPiercingStrike",
                                                               "RengarQ", "GarenQAttack", "GarenRPreCast",
                                                               "PoppyPassiveAttack", "viktorqbuff", "FioraEAttack",
                                                               "TeemoQ"
                                                           };

        /// <summary>
        ///     The w anti spell.
        /// </summary>
        public static List<string> InterrupterSpell = new List<string>
                                                    {
                                                        "RenektonPreExecute", "TalonCutthroat", "IreliaEquilibriumStrike",
                                                        "XenZhaoThrust3", "KatarinaRTrigger", "KatarinaE",
                                                    };

        public static List<string> InvulnerableList = new List<string>
                                                           {
                                                               "FioraW", "kindrednodeathbuff", "Undying Rage", "JudicatorIntervention"
                                                           };

        #endregion

        #region Public Properties

        private static int Item =>
             Items.CanUseItem(3077) && Items.HasItem(3077)
            ? 3077
             : Items.CanUseItem(3074) && Items.HasItem(3074)
              ? 3074
               : Items.CanUseItem(3748) && Items.HasItem(3748)
                ? 3748 
                 : 0;

        public static bool R1 { get; set; }


        public static bool InRange(AttackableUnit x)
        {
            return ObjectManager.Player.HasBuff("RivenFengShuiEngine")
            ? Player.Distance(x) <= 200 + x.BoundingRadius
            : Player.Distance(x) <= 125 + x.BoundingRadius;
        }
        #endregion

        #region Public Methods and Operators

        public static void ForceSkill()
        {
            if (Unit == null)
            {
                return;
            }

            if (canQ && Spells.Q.IsReady())
            {
                if (Items.CanUseItem(Item) && Item != 0 && Qstack == 3)
                {
                    Items.UseItem(Item);
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => Spells.Q.Cast(Unit.Position));
                }
                else
                {
                    Spells.Q.Cast(Unit.Position);
                }
            }

            if (canW)
            {
                if (Items.CanUseItem(Item) && Item != 0)
                {
                    Items.UseItem(Item);
                    LeagueSharp.Common.Utility.DelayAction.Add(5, () => Spells.W.Cast());
                }
                else
                {
                    Spells.W.Cast();
                }

                if (doublecastQ && Spells.Q.IsReady() && Qstack == 1)
                {
                     var delay = Spells.R.IsReady() ? 190 : 90;

                     LeagueSharp.Common.Utility.DelayAction.Add(delay, () => Spells.Q.Cast(Unit.Position));
                    //Spells.Q.Cast(Unit.Position);
                }
            }

            if (!R1 || Spells.R.Instance.Name != IsFirstR)
            {
                return;
            }

            Spells.R.Cast();
        }

        public static void DoubleCastQ(AttackableUnit x)
        {
            Unit = x;
            doublecastQ = true;
            LeagueSharp.Common.Utility.DelayAction.Add(300, () => doublecastQ = false);
        }

        public static void CastQ(AttackableUnit x)
        {
            Unit = x;
            canQ = true;
        }

        public static void CastE(AttackableUnit x)
        {
            Unit = x;
        }

        public static void FlashW()
        {
            var target = TargetSelector.GetSelectedTarget();

            Spells.W.Cast();
            LeagueSharp.Common.Utility.DelayAction.Add(10, () => Player.Spellbook.CastSpell(Spells.Flash, target.Position));
            LeagueSharp.Common.Utility.DelayAction.Add(30, ()=> DoubleCastQ(target));
        }

        public static void CastW(Obj_AI_Base x)
        {
            canW = Spells.W.IsReady() && InRange(x) && !x.HasBuff("FioraW");
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => canW = false);
        }

        public static void ForceR()
        {
            R1 = Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => R1 = false); 
        }

        public static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var argsName = args.SData.Name;

            if (argsName.Contains("RivenTriCleave"))
            {
                canQ = false;
                doublecastQ = false;
            }

            if (argsName.Contains("RivenMartyr"))
            {
                canW = false;
                doublecastQ = true;
                LeagueSharp.Common.Utility.DelayAction.Add(300, () => doublecastQ = false);
            }

            if (argsName == IsFirstR)
            {
                R1 = false;
            }
        }
        #endregion
    }
}