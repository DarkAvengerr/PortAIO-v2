using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
    #region

    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using NechritoRiven.Menus;

    #endregion

    /// <summary>
    /// The core.
    /// </summary>
    internal partial class Core
    {
        #region Static Fields

        private static AttackableUnit Unit { get; set; }

        private static bool canQ;

        private static bool canW;

        /// <summary>
        ///     The e anti spell.
        /// </summary>
        public static List<string> EAntiSpell = new List<string>
                                                    {
                                                        "MonkeyKingSpinToWin", "KatarinaRTrigger", "HungeringStrike",
                                                        "TwitchEParticle", "RengarPassiveBuffDashAADummy",
                                                        "RengarPassiveBuffDash", "IreliaEquilibriumStrike",
                                                        "BraumBasicAttackPassiveOverride", "gnarwproc",
                                                        "hecarimrampattack", "illaoiwattack", "JaxEmpowerTwo",
                                                        "JayceThunderingBlow", "RenektonSuperExecute",
                                                        "vaynesilvereddebuff"
                                                    };

        public static float LastQ;

        /// <summary>
        ///     The targeted anti spell.
        /// </summary>
        public static List<string> TargetedAntiSpell = new List<string>
                                                           {
                                                               "MonkeyKingQAttack", "YasuoDash", "FizzPiercingStrike",
                                                               "RengarQ", "GarenQAttack", "GarenRPreCast",
                                                               "PoppyPassiveAttack", "viktorqbuff", "FioraEAttack",
                                                               "TeemoQ"
                                                           };

        /// <summary>
        ///     The w anti spell.
        /// </summary>
        public static List<string> WAntiSpell = new List<string>
                                                    {
                                                        "RenektonPreExecute", "TalonCutthroat", "IreliaEquilibriumStrike",
                                                        "XenZhaoThrust3", "KatarinaRTrigger", "KatarinaE",
                                                        "MonkeyKingSpinToWin"
                                                    };

        public static List<string> NoRList = new List<string>
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

        private static bool CanQ(AttackableUnit x)
        {
            return canQ && InRange(x);  
        }

        private static bool CanW(AttackableUnit x)
        {
            return canW && InRange(x);
        }

        public static bool InRange(AttackableUnit x)
        {
            return ObjectManager.Player.HasBuff("RivenFengShuiEngine")
            ? Player.Distance(x) <= 330
            : Player.Distance(x) <= 265;
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
                if (Items.CanUseItem(Item) && Item != 0 && Qstack > 1)
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
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => Spells.W.Cast());
                }
                else
                {
                    Spells.W.Cast();
                }
            }

            if (!R1 || Spells.R.Instance.Name != IsFirstR)
            {
                return;
            }
            Spells.R.Cast();
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
        }

        public static void CastW(Obj_AI_Base x)
        {
            canW = Spells.W.IsReady() && InRange(x);
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
            }

            if (argsName.Contains("RivenMartyr"))
            {
                canW = false;
            }

            if (argsName == IsFirstR)
            {
                R1 = false;
            }
        }
        #endregion
    }
}