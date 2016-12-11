using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Extensions
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using Color = System.Drawing.Color;
    using static Brand.Extensions.Config;
    using static Brand.Extensions.Spells;
    using static Brand.Extensions.Other;

    internal class Events
    {
        private static readonly DamageBar Indicator = new DamageBar();

        public static void Initialize()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            LeagueSharp.SDK.Events.OnInterruptableTarget += OnInterruptableTarget;
            LeagueSharp.SDK.Events.OnGapCloser += OnGapCloser;
        }

        private static void OnUpdate(EventArgs args)
        {
            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Modes.Combo.Execute();
                    break;
                case OrbwalkingMode.Hybrid:
                    Modes.Harass.Execute();
                    break;
                case OrbwalkingMode.LaneClear:
                    Modes.Clear.Execute();
                    Modes.Jungle.Execute();
                    break;
            }
            Modes.Killsteal.Execute();
            Modes.Active.Execute();
            SkinChanger();
        }

        private static void OnInterruptableTarget(object sender, LeagueSharp.SDK.Events.InterruptableTargetEventArgs args)
        {
            if (IM_E_Q && (MyHero.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana))
            {
                if (args.Sender.IsValidTarget(E.Range) && (args.Sender.HasBuff("brandablaze") || E.IsReady()))
                {
                    E.Cast(args.Sender);
                    if (Q.IsReady())
                    {
                        Q.Cast(args.Sender);
                    }
                }
            }
        }

        private static void OnGapCloser(object sender, LeagueSharp.SDK.Events.GapCloserEventArgs args)
        {
            if (GM_E_Q && (MyHero.Mana > Q.Instance.SData.Mana + E.Instance.SData.Mana))
            {
                if (args.Sender.IsValidTarget(E.Range) && (args.Sender.HasBuff("brandablaze") || E.IsReady()))
                {
                    E.Cast(args.Sender);
                    if (Q.IsReady())
                    {
                        Q.Cast(args.Sender);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            foreach (var Enemy in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
            {
                if (DM_D)
                {
                    Indicator.Unit = Enemy;
                    Indicator.DrawDmg(ComboDamage(Enemy), SharpDX.Color.Orange);
                }
            }

            if (DM_Q && Q.IsReady())
            {
                Render.Circle.DrawCircle(MyHero.Position, Q.Range, Color.Aqua, 2);
            }
            if (DM_W && W.IsReady())
            {
                Render.Circle.DrawCircle(MyHero.Position, W.Range, Color.BlueViolet, 2);
            }
            if (DM_E && E.IsReady())
            {
                Render.Circle.DrawCircle(MyHero.Position, E.Range, Color.DarkOrange, 2);
            }
            if (DM_R && R.IsReady())
            {
                Render.Circle.DrawCircle(MyHero.Position, R.Range, Color.Red, 2);
            }
        }
    }
}
