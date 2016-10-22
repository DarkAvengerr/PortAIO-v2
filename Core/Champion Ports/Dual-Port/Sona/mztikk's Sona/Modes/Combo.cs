using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkSona.Modes
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkSona.Extensions;

    using Config = mztikkSona.Config;

    internal static class Combo
    {
        #region Public Methods and Operators

        public static void OnPreAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Mainframe.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo || !Config.IsChecked("Combo.bSmartAA"))
            {
                return;
            }

            if (!ObjectManager.Player.HasBuff("sonapassiveattack") && !ObjectManager.Player.HasBuff("sonaqprocattacker"))
            {
                args.Process = false;
            }
        }

        #endregion

        #region Methods

        internal static void Execute()
        {
            if (Config.IsChecked("Combo.bR") && Spells.R.CanCast())
            {
                var rTarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
                if (rTarget != null)
                {
                    Spells.R.CastIfWillHit(rTarget, Config.GetSliderValue("Combo.minR"));

                    /*var rRectangle = new Geometry.Polygon.Rectangle(
                        ObjectManager.Player.Position, 
                        ObjectManager.Player.Position.Extend(rTarget.Position, Spells.R.Range), 
                        Spells.R.Width);
                    if (
                        HeroManager.Enemies.Count(
                            enemy =>
                            !enemy.IsDead && enemy.IsValid && !enemy.HasBuffOfType(BuffType.Invulnerability)
                            && rRectangle.IsInside(enemy.Position))
                        >= Config.GetSliderValue("Combo.minR"))
                    {
                        Spells.R.Cast(rTarget);
                    }*/
                }
            }

            if (Config.IsChecked("Combo.bFlashR") && Spells.R.CanCast() && Spells.Flash.CanCast())
            {
                var rFlashTarget = TargetSelector.GetTarget(Spells.R.Range + 375, TargetSelector.DamageType.Magical);
                if (rFlashTarget != null)
                {
                    var flashPos = ObjectManager.Player.Position.Extend(rFlashTarget.Position, 425);
                    Spells.FlashR.UpdateSourcePosition(flashPos, flashPos);
                    if (Spells.R.CountHits(
                        HeroManager.Enemies.Where(x => !x.IsDead).Select(x => x.Position).ToList(), 
                        rFlashTarget.Position) >= Config.GetSliderValue("Combo.minFlashR"))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(Spells.Flash.Slot, flashPos);
                        LeagueSharp.Common.Utility.DelayAction.Add(
                            Computed.RDelay.Next(60, 85), 
                            () => Spells.FlashR.Cast(rFlashTarget.Position));
                    }
                }
            }

            /*if (Config.IsChecked("Combo.bFlashR") && Spells.R.CanCast() && Spells.Flash.CanCast())
            {
                var rFlashTarget = TargetSelector.GetTarget(Spells.R.Range + 400, TargetSelector.DamageType.Magical);
                if (rFlashTarget != null)
                {
                    var flashPos = ObjectManager.Player.Position.Extend(rFlashTarget.Position, 425);
                    var flashUltRectangle = new Geometry.Polygon.Rectangle(
                        flashPos, 
                        flashPos.Extend(ObjectManager.Player.Position, -Spells.R.Range), 
                        Spells.R.Width);
                    if (
                        HeroManager.Enemies.Count(
                            enemy =>
                            !enemy.HasBuffOfType(BuffType.Invulnerability) && !enemy.IsDead && enemy.IsValid
                            && flashUltRectangle.IsInside(enemy.Position))
                        >= Config.GetSliderValue("Combo.minFlashR"))
                    {
                        ObjectManager.Player.Spellbook.CastSpell(Spells.Flash.Slot, flashPos);
                        LeagueSharp.Common.Utility.DelayAction.Add(Computed.RDelay.Next(50, 85), () => Spells.R.Cast(rFlashTarget));

                         For when mid animation flash is possible
                        Spells.R.Cast(flashPos);
                        Core.DelayAction(
                            () => Player.CastSpell(Spells.Flash.Slot, flashPos), 
                            Computed.RDelay.Next(150, 250));
                            
                    }
                }
            }*/
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
            {
                return;
            }

            if (Config.IsChecked("Combo.bQ") && Spells.Q.CanCast()
                && target.Distance(ObjectManager.Player) < Spells.Q.Range)
            {
                Spells.Q.Cast();
            }

            if (Config.IsChecked("Combo.bE") && Spells.E.CanCast())
            {
                if (target.Path.Length == 0 || !target.IsMoving)
                {
                    return;
                }

                var pathEndLoc = target.Path.OrderByDescending(x => x.Distance(target.Position)).FirstOrDefault();
                var dist = ObjectManager.Player.Position.Distance(target.Position);
                var distToPath = pathEndLoc.Distance(ObjectManager.Player.Position);
                if (distToPath <= dist)
                {
                    return;
                }

                var movspeedDif = ObjectManager.Player.MoveSpeed - target.MoveSpeed;
                if (movspeedDif <= 0 && !Orbwalking.InAutoAttackRange(target))
                {
                    Spells.E.Cast();
                }

                var timeToReach = dist / movspeedDif;
                if (timeToReach > 2.5f)
                {
                    Spells.E.Cast();
                }
            }
        }

        #endregion
    }
}