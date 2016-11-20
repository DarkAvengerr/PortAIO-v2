using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;

    internal static class Computed

    {
        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }
        internal static bool HasBlue { get { return ObjectManager.Player.HasBuff("bluecardpreattack"); } }
        internal static bool HasRed { get { return ObjectManager.Player.HasBuff("redcardpreattack"); } }
        internal static bool HasGold { get { return ObjectManager.Player.HasBuff("goldcardpreattack"); } }
        internal static string HasACard
        {
            get
            {
                if (ObjectManager.Player.HasBuff("bluecardpreattack"))
                    return "blue";
                if (ObjectManager.Player.HasBuff("goldcardpreattack"))
                    return "gold";
                if (ObjectManager.Player.HasBuff("redcardpreattack"))
                    return "red";
                return "empty";
            }
        }

        #endregion

        #region Public Methods and Operators

        public static void InterruptableSpell_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Config.IsChecked("goldInter"))
            {
                if (sender.IsEnemy && Orbwalking.InAutoAttackRange(sender))
                {
                    if (HasGold)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                    }
                }
            }
        }

        public static void Gapcloser_OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (Config.IsChecked("goldGap"))
            {
                if (gapcloser.Sender.IsEnemy && Orbwalking.InAutoAttackRange(gapcloser.Sender))
                {
                    if (HasGold)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                    }
                }
            }
        }

        public static void OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is AIHeroClient)
            {
                args.Process = CardSelector.Status != SelectStatus.Selecting
                               && Environment.TickCount - CardSelector.LastWSent > 300;
            }

            if (CardSelector.Status == SelectStatus.Selecting
                && ((Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    || (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)))
            {
                args.Process = false;
            }

            var targetDis = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (HasACard != "empty"
                && !HeroManager.Enemies.Contains(args.Target)
                && targetDis.IsValidTarget()
                && !targetDis.IsZombie
                && Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && (ObjectManager.Player.Distance(targetDis) < Orbwalking.GetAttackRange(ObjectManager.Player) + 250))
            {
                args.Process = false;

                var target = TargetSelector.GetTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), TargetSelector.DamageType.Magical);

                if (target.IsValidTarget() && !target.IsZombie)
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Gate")
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }

            if (!sender.IsMe)
            {
                //none
            }
        }

        public static void SafeCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            //none
        }

        public static void YellowIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var canWKill =
            HeroManager.Enemies.FirstOrDefault(
                h =>
                !h.IsDead && h.IsValidTarget(Spells.Q.Range)
                && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

            if (!sender.IsMe || args.SData.Name.ToLower() != "goldcardpreattack" || !Spells.Q.IsReady() || canWKill != null || ObjectManager.Player.ManaPercent < Config.GetSliderValue("qAMana"))
            {
                return;
            }

            var targetDis = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (targetDis == null || !targetDis.IsValidTarget(Spells.Q.Range / 2))
            {
                return;
            }

            if (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (targetDis.IsValidTarget(Spells.Q.Range))
                {
                    var qPred = Spells.Q.GetPrediction(targetDis);

                    if (qPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Spells.Q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        public static void RedIntoQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var canWKill =
                HeroManager.Enemies.FirstOrDefault(
                h =>
                !h.IsDead && h.IsValidTarget(Spells.Q.Range)
                && h.Health < ObjectManager.Player.GetSpellDamage(h, SpellSlot.W));

            if (!sender.IsMe || args.SData.Name.ToLower() != "redcardpreattack" || !Spells.Q.IsReady() || canWKill != null || ObjectManager.Player.ManaPercent < Config.GetSliderValue("qAMana"))
            {
                return;
            }

            var targetDis = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (targetDis == null || !targetDis.IsValidTarget(Spells.Q.Range / 2))
            {
                return;
            }

            if (Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Mainframe.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (targetDis.IsValidTarget(Spells.Q.Range))
                {
                    var qPred = Spells.Q.GetPrediction(targetDis);

                    if (qPred.Hitchance >= HitChance.VeryHigh)
                    {
                        Spells.Q.Cast(qPred.CastPosition);
                    }
                }
            }
        }

        #endregion
    }
}