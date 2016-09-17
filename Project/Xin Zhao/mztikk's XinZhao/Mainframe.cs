using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikkXinZhao
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using mztikkXinZhao.Modes;

    internal static class Mainframe
    {
        #region Static Fields

        internal static Random RDelay = new Random();

        #endregion

        #region Properties

        internal static Orbwalking.Orbwalker Orbwalker { get; set; }

        #endregion

        #region Methods

        internal static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += JungleClear.JungleOnPostAttack;
            Orbwalking.AfterAttack += Combo.ComboOnPostAttack;
            Orbwalking.AfterAttack += Harass.HarassOnPostAttack;
            Orbwalking.AfterAttack += LaneClear.LaneOnPostAttack;
            Interrupter2.OnInterruptableTarget += Events.OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += Events.OnProcessSpellCast;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.IsChecked("draw.e"))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.E.Range, Color.White);
            }

            if (!Config.IsChecked("drawXinsec"))
            {
                return;
            }

            AIHeroClient xinsecTarget = null;
            switch (Config.GetStringListValue("xinsecTargetting"))
            {
                case 0:
                    xinsecTarget = TargetSelector.SelectedTarget;
                    break;
                case 1:
                    xinsecTarget = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);
                    break;
                case 2:
                    xinsecTarget =
                        HeroManager.Enemies.Where(en => en.Distance(ObjectManager.Player.Position) <= 2000)
                            .OrderBy(en => en.MaxHealth)
                            .FirstOrDefault();
                    break;
            }

            if (xinsecTarget != null && Spells.E.IsReady() && Spells.R.IsReady()
                && !xinsecTarget.HasBuff("XinZhaoIntimidate") && !xinsecTarget.IsInvulnerable)
            {
                var w2S = Drawing.WorldToScreen(xinsecTarget.Position);
                Drawing.DrawText(w2S.X, w2S.Y, Color.AntiqueWhite, "Xinsec: " + xinsecTarget.ChampionName, 10);
                if (Config.IsChecked("drawXinsecpred"))
                {
                    var extendToPos = ObjectManager.Player.Position.GetBestAllyPlace(1750);
                    var xinsecTargetExtend = xinsecTarget.Position.Extend(extendToPos, -200);
                    Drawing.DrawCircle(xinsecTargetExtend, 100, Color.AliceBlue);
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                return;
            }

            if (Config.IsKeyPressed("xinsecKey"))
            {
                Orbwalking.MoveTo(Game.CursorPos);
                Xinsec.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("jcMana"))
            {
                JungleClear.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("harassMana"))
            {
                Harass.Execute();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                && ObjectManager.Player.ManaPercent >= Config.GetSliderValue("lcMana"))
            {
                LaneClear.Execute();
            }
        }

        #endregion
    }
}