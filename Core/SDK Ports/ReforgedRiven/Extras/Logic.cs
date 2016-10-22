#region

using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Reforged_Riven.Extras
{
    internal class Logic : Core
    {
        internal static bool ForceQ;
        internal static bool _forceW;
        internal static bool _forceR;
        internal static bool _forceR2;
        internal static bool _forceItem;

        public static AttackableUnit Qtarget;

        public static int WRange => Player.HasBuff("RivenFengShuiEngine")
            ? 330
            : 265;

        public static bool InWRange(AttackableUnit t) => t.IsValidTarget(WRange);

        public static bool InQRange(GameObject target)
        {
            return target != null && (Player.HasBuff("RivenFengShuiEngine")
                ? 330 >= Player.Distance(target.Position)
                : 265 >= Player.Distance(target.Position));
        }

        public static void ForceItem()
        {
            if (Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) _forceItem = true;
            DelayAction.Add(500, () => _forceItem = false);
        }

        public static void ForceSkill()
        {
            if (ForceQ && Spells.Q.IsReady() && Qtarget != null)
            {
                Spells.Q.Cast(Player.Position.Extend(Qtarget.Position, - 250));
            }

            if (_forceR && Spells.R.Instance.Name == IsFirstR)
            {
                Spells.R.Cast();
            }

            if (_forceItem && Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0)
            {
                Items.UseItem(Item);
            }

            if (!_forceR2 || Spells.R.Instance.Name != IsSecondR) return;

            var target = Variables.TargetSelector.GetSelectedTarget();
            if (target == null) return;

            var pred = Spells.R.GetPrediction(target);
            if (pred.Hitchance > HitChance.High)
            {
                Spells.R.Cast(pred.CastPosition);
            }
        }

        public static void ForceR()
        {
            _forceR = Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR;
            DelayAction.Add(500, () => _forceR = false);
        }

        public static void ForceR2()
        {
            _forceR2 = Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR;
            DelayAction.Add(500, () => _forceR2 = false);
        }

        public static void ForceW()
        {
            _forceW = Spells.W.IsReady();
            DelayAction.Add(500, () => _forceW = false);
        }

        public static void ForceCastQ(AttackableUnit target)
        {
            ForceQ = true;
            Qtarget = target;
        }

        public static void CastYomu()
        {
            if (!Items.CanUseItem(3142)) return;

            Items.UseItem(3142);
        }

        public static List<string> TargetedAntiSpell = new List<string>()
        {
            "MonkeyKingQAttack", "YasuoDash",
            "FizzPiercingStrike", "RengarQ",
            "GarenQAttack", "GarenRPreCast",
            "PoppyPassiveAttack", "viktorqbuff" ,
            "FioraEAttack",
        };

        public static List<string> EAntiSpell = new List<string>()
        {
           "MonkeyKingSpinToWin",  "KatarinaRTrigger",
            "HungeringStrike", "TwitchEParticle",
            "RengarPassiveBuffDashAADummy", "RengarPassiveBuffDash",
            "IreliaEquilibriumStrike", "BraumBasicAttackPassiveOverride",
            "gnarwproc", "hecarimrampattack",
            "illaoiwattack", "JaxEmpowerTwo",
            "JayceThunderingBlow", "RenektonSuperExecute",
            "vaynesilvereddebuff",
           
        };

        public static List<string> WAntiSpell = new List<string>()
        {
            "RenektonPreExecute",
            "TalonCutthroat",
            "IreliaEquilibriumStrike",
            "XenZhaoThrust3",
            "KatarinaRTrigger",
            "KatarinaE",
            "MonkeyKingSpinToWin"
        };
            

        public static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;


            if (args.SData.Name.Contains("ItemTiamatCleave")) _forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) ForceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) _forceW = false;
            if (args.SData.Name == IsFirstR) _forceR = false;
            if (args.SData.Name == IsSecondR) _forceR2 = false;
        }

        private static int Item
           =>
               Items.CanUseItem(3077) && Items.HasItem(3077)
                   ? 3077
                   : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;
    }
}