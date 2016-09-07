#region

using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Core
{
   internal partial class Core
    {
       public static AttackableUnit QTarget;
       private static bool _forceQ;
       private static bool _forceW;
       private static bool _forceR;
       private static bool _forceR2;
       private static bool _forceItem;
        public static float LastQ;
        
        private static int Item
            =>
                Items.CanUseItem(3077) && Items.HasItem(3077)
                    ? 3077
                    : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;

        public static void ForceW()
        {
            _forceW = Spells.W.IsReady();
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => _forceW = false);
        }

        public static void ForceQ(AttackableUnit target)
        {
            _forceQ = true;
            QTarget = target;
        }
        public static void ForceSkill()
        {
            if (_forceQ && qTarget != null && qTarget.IsValidTarget(Spells.E.Range + Player.BoundingRadius + 70) &&
                Spells.Q.IsReady())
            {
                Spells.Q.Cast(qTarget.Position);
            }

            if (_forceW)
            {
                Spells.W.Cast();
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

            var target = TargetSelector.GetSelectedTarget();
            if (target != null) Spells.R.Cast(target.Position);
        }
        public static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.Contains("ItemTiamatCleave")) _forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) _forceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) _forceW = false;
            if (args.SData.Name == IsFirstR) _forceR = false;
            if (args.SData.Name == IsSecondR) _forceR2 = false;
        }

        public static int WRange => Player.HasBuff("RivenFengShuiEngine")
          ? 330
          : 265;

        public static bool InWRange(Obj_AI_Base t) => t != null && t.IsValidTarget(WRange);

        public static bool InQRange(GameObject target)
        {
            return target != null && (Player.HasBuff("RivenFengShuiEngine")
                ? 330 >= Player.Distance(target.Position)
                : 265 >= Player.Distance(target.Position));
        }
        public static void ForceItem()
        {
            if (Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) _forceItem = true;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => _forceItem = false);
        }

        public static void ForceR()
        {
            _forceR = Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => _forceR = false);
        }

        public static void ForceR2()
        {
            _forceR2 = Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => _forceR2 = false);
        }
        public static void ForceCastQ(AttackableUnit target)
        {
            _forceQ = true;
            qTarget = target;
        }

        public static void FlashW()
        {
            var target = TargetSelector.GetSelectedTarget();

            if (target == null || !target.IsValidTarget() || target.IsZombie) return;

            LeagueSharp.Common.Utility.DelayAction.Add(10, () => Player.Spellbook.CastSpell(Spells.Flash, target.Position));
            LeagueSharp.Common.Utility.DelayAction.Add(11, ()=> Spells.W.Cast(target));
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
    }
}
