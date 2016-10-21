using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoJhin
{
    public static class BadaoJhinAuto
    {
        public static int RAutoTick = 0;
        public static AIHeroClient RAutoTarget = null;
        public static void BadaoActiavate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (BadaoJhinHelper.UseRAuto() && BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
            {
                if (RAutoTarget.BadaoIsValidTarget())
                {
                    var x = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    var y = Drawing.WorldToScreen(RAutoTarget.Position);
                    Drawing.DrawLine(x, y, 2, Color.Red);
                }
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (BadaoJhinHelper.AutoRModeOnTap() && BadaoJhinHelper.UseRAuto() && BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
            {
                if (args.Msg == (uint)WindowsMessages.WM_KEYDOWN && args.WParam == BadaoJhinHelper.AutoRTapKey() 
                    && BadaoMainVariables.R.IsReady())
                {
                    if (RAutoTarget.BadaoIsValidTarget())
                    {
                        BadaoMainVariables.R.Cast(RAutoTarget);
                    }
                }

            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Environment.TickCount - RAutoTick >= 200 && BadaoJhinHelper.UseRAuto() 
                && BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
            {
                RAutoTick = Environment.TickCount;
                if (BadaoJhinHelper.AutoRTargetAuto())
                {
                    var target = TargetSelector.GetTarget(BadaoMainVariables.R.Range, TargetSelector.DamageType.Physical,
                        true, HeroManager.Enemies.Where(x => x.IsValid && !BadaoChecker.BadaoInTheCone(
                             x.Position.To2D(), ObjectManager.Player.Position.To2D(),
                             ObjectManager.Player.Position.To2D()
                             + ObjectManager.Player.Direction.To2D().Normalized().Perpendicular() * BadaoMainVariables.R.Range, 60)));
                    if (target.BadaoIsValidTarget())
                    {
                        RAutoTarget = target;
                    }
                }
                else if (BadaoJhinHelper.AutoRTargetNearMouse())
                {
                    var target = HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.R.Range)
                             && BadaoChecker.BadaoInTheCone(
                             x.Position.To2D(), ObjectManager.Player.Position.To2D(),
                             ObjectManager.Player.Position.To2D()
                             + ObjectManager.Player.Direction.To2D().Normalized().Perpendicular() * BadaoMainVariables.R.Range, 60))
                             .OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault();
                    if (target.BadaoIsValidTarget())
                    {
                        RAutoTarget = target;
                    }
                }
                else if (BadaoJhinHelper.AutoRTargetSelected())
                {
                    var target = TargetSelector.GetSelectedTarget();
                    if (target.BadaoIsValidTarget(BadaoMainVariables.R.Range) &&
                        BadaoChecker.BadaoInTheCone(
                             target.Position.To2D(), ObjectManager.Player.Position.To2D(),
                             ObjectManager.Player.Position.To2D()
                             + ObjectManager.Player.Direction.To2D().Normalized().Perpendicular() * BadaoMainVariables.R.Range, 60))
                    {
                        RAutoTarget = target;
                    }
                }
            }

            // auto ping

            if (BadaoJhinHelper.UseAutoPingKillable())
            {
                if (BadaoMainVariables.R.IsReady() && BadaoMainVariables.R.Instance.SData.Name != "JhinRShot")
                {
                    foreach (var hero in HeroManager.Enemies
                        .Where(x => x.BadaoIsValidTarget(BadaoMainVariables.R.Range) &&  BadaoJhinHelper.GetRdamage(x) >= x.Health))
                    {
                        BadaoJhinPing.Ping(hero.Position.To2D());
                        break;
                    }
                }
            }

            //JhinR
            if (BadaoJhinHelper.UseRAuto() && BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
            {

                if (BadaoJhinHelper.AutoRModeAuto())
                {
                    if (RAutoTarget.BadaoIsValidTarget())
                    {
                        BadaoMainVariables.R.Cast(RAutoTarget);
                    }
                }
            }
            if (BadaoMainVariables.R.Instance.SData.Name == "JhinRShot")
                return;
            if (BadaoJhinHelper.UseAutoKS())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.W.Range)))
                {
                    if (BadaoMainVariables.W.IsReady() && BadaoJhinHelper.GetWDamage(hero) >= hero.Health)
                    {
                        var x = BadaoMainVariables.W.GetPrediction(hero).CastPosition;
                        var y = BadaoMainVariables.W.GetPrediction(hero).CollisionObjects;
                        if (!y.Any(z => z.IsChampion()) && ObjectManager.Player.Distance(x) <= BadaoMainVariables.W.Range)
                        {
                            if (BadaoMainVariables.W.Cast(x))
                                break;
                        }
                    }
                    if (BadaoMainVariables.Q.IsReady() && BadaoJhinHelper.GetQDamage(hero) >= hero.Health
                        && hero.BadaoIsValidTarget(BadaoMainVariables.Q.Range))
                    {
                        BadaoMainVariables.Q.Cast(hero);
                    }
                }
            }
            if (!BadaoJhinHelper.CanAutoMana())
                return;
            if (BadaoJhinHelper.UseWAutoTrap())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.W.Range)))
                {
                    var x = BadaoMainVariables.W.GetPrediction(hero).CastPosition;
                    var y = BadaoMainVariables.W.GetPrediction(hero).CollisionObjects;
                    if (!y.Any(z => z.IsChampion()) && ObjectManager.Player.Distance(x) <= BadaoMainVariables.W.Range
                        && BadaoJhinPassive.JhinTrap.Any(i => i.Distance(x) <= 100))
                    {
                        if (BadaoMainVariables.W.Cast(x))
                            break;
                    }
                }
            }
            if (BadaoJhinHelper.UseWAuto())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.W.Range)))
                {
                    if ((hero.HasBuffOfType(BuffType.Slow) || hero.HasBuffOfType(BuffType.Charm) || hero.HasBuffOfType(BuffType.Snare)) 
                        && BadaoJhinHelper.HasJhinPassive(hero))
                    {
                        var x = BadaoMainVariables.W.GetPrediction(hero).CastPosition;
                        var y = BadaoMainVariables.W.GetPrediction(hero).CollisionObjects;
                        if (!y.Any(z => z.IsChampion()) && ObjectManager.Player.Distance(x) <= BadaoMainVariables.W.Range)
                        {
                            if (BadaoMainVariables.W.Cast(x))
                                break;
                        }
                    }
                }
            }
        }
    }
}
