using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SyndraL33T
{
    public class Graphics
    {
        private static readonly Font Font = new Font(
            Drawing.Direct3DDevice, 16, 0, FontWeight.Normal, 0, false, FontCharacterSet.Default, FontPrecision.Default,
            FontQuality.Default, FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative, "Tahoma");

        private static readonly Font FontB = new Font(
            Drawing.Direct3DDevice, 16, 0, FontWeight.Bold, 0, false, FontCharacterSet.Default, FontPrecision.Default,
            FontQuality.Default, FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative, "Tahoma");

        private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

        public static void OnDraw(EventArgs args)
        {
            if (EntryPoint.Menu.Item("l33t.stds.drawing.enabledraw").GetValue<bool>())
            {
                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawDebug").GetValue<bool>())
                {
                    DrawDebug();
                }

                var classic = EntryPoint.Menu.Item("l33t.stds.drawing.classic").GetValue<bool>();
                var playerPosition = EntryPoint.Player.Position;

                foreach (var spell in
                    Mechanics.Spells.Where(
                        spell =>
                            EntryPoint.Menu.Item("l33t.stds.drawing.draw" + spell.Key) != null &&
                            EntryPoint.Menu.Item("l33t.stds.drawing.draw" + spell.Key).GetValue<Circle>().Active))
                {
                    if (classic)
                    {
                        Drawing.DrawCircle(
                            playerPosition, spell.Value.Range,
                            EntryPoint.Menu.Item("l33t.stds.drawing.draw" + spell.Key).GetValue<Circle>().Color);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(
                            playerPosition, spell.Value.Range,
                            EntryPoint.Menu.Item("l33t.stds.drawing.draw" + spell.Key).GetValue<Circle>().Color);
                    }
                }

                foreach (var enemy in ObjectCache.GetHeroes().Where(e => e.IsHPBarRendered && !e.IsDead))
                {
                    DrawEnemyInfo(enemy);
                }

                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawQEC").GetValue<Circle>().Active &&
                    EntryPoint.Menu.Item("l33t.stds.qesettings.qetocursor").GetValue<KeyBind>().Active &&
                    Mechanics.Spells[SpellSlot.Q].IsReady())
                {
                    DrawSphereEToCursor(classic);
                }

                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawHUD").GetValue<bool>())
                {
                    DrawHud();
                }

                // Draw QE MAP
                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawQEMAP").GetValue<bool>())
                {
                    DrawSphereEMap(classic);
                }
                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawWMAP").GetValue<bool>() &&
                    Mechanics.Spells[SpellSlot.W].Level > 0)
                {
                    DrawForceOfWillMap(classic);
                }
            }
        }

        private static void DrawEnemyInfo(AIHeroClient enemy)
        {
            var hpBarPos = enemy.HPBarPosition;
            hpBarPos.X += 45;
            hpBarPos.Y += 18;
            var killText = "";
            var combodamage = enemy.GetComboDamage(
                EntryPoint.Menu.Item("l33t.stds.combo.useQ").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useW").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useE").GetValue<bool>(),
                EntryPoint.Menu.Item("l33t.stds.combo.useR").GetValue<bool>());

            var percentHPleftAfterCombo = (enemy.Health - combodamage) / enemy.MaxHealth;
            var percentHPleft = enemy.Health / enemy.MaxHealth;
            if (percentHPleftAfterCombo < 0)
            {
                percentHPleftAfterCombo = 0;
            }

            var comboXPos = hpBarPos.X - 36 + (107 * percentHPleftAfterCombo);
            double currentHpxPos = hpBarPos.X - 36 + (107 * percentHPleft);
            var barcolor = System.Drawing.Color.FromArgb(100, 0, 220, 0);
            var barcolorline = System.Drawing.Color.WhiteSmoke;
            if (combodamage + Mechanics.Spells[SpellSlot.Q].Damage(enemy) +
                EntryPoint.Player.GetAutoAttackDamage(enemy) * 2 > enemy.Health)
            {
                killText = "Killable by: Full Combo + 1Q + 2AA";
                if (combodamage >= enemy.Health)
                {
                    killText = "Killable by: Full Combo";
                }
                barcolor = System.Drawing.Color.FromArgb(100, 255, 255, 0);
                barcolorline = System.Drawing.Color.SpringGreen;
                var linecolor = barcolor;
                if (
                    enemy.GetComboDamage(
                        EntryPoint.Menu.Item("l33t.stds.combo.useQ").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.combo.useW").GetValue<bool>(),
                        EntryPoint.Menu.Item("l33t.stds.combo.useE").GetValue<bool>(), false) > enemy.Health)
                {
                    killText = "Killable by: Q + W + E";
                    barcolor = System.Drawing.Color.FromArgb(130, 255, 70, 0);
                    linecolor = System.Drawing.Color.FromArgb(150, 255, 0, 0);
                }
                if (EntryPoint.Menu.Item("l33t.stds.drawing.drawGank").GetValue<bool>())
                {
                    var pos = EntryPoint.Player.Position +
                              Vector3.Normalize(enemy.Position - EntryPoint.Player.Position) * 100;
                    var myPos = Drawing.WorldToScreen(pos);
                    pos = EntryPoint.Player.Position +
                          Vector3.Normalize(enemy.Position - EntryPoint.Player.Position) * 350;
                    var ePos = Drawing.WorldToScreen(pos);
                    Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 1, linecolor);
                }
            }
            var killTextPos = Drawing.WorldToScreen(enemy.Position);
            var hPleftText = Math.Round(percentHPleftAfterCombo * 100) + "%";
            Drawing.DrawLine((float)comboXPos, hpBarPos.Y, (float)comboXPos, hpBarPos.Y + 5, 1, barcolorline);
            if (EntryPoint.Menu.Item("l33t.stds.drawing.drawKillText").GetValue<bool>())
            {
                Drawing.DrawText(killTextPos[0] - 105, killTextPos[1] + 25, barcolor, killText);
            }
            if (EntryPoint.Menu.Item("l33t.stds.drawing.drawKillTextHP").GetValue<bool>())
            {
                Drawing.DrawText(hpBarPos.X + 98, hpBarPos.Y + 5, barcolor, hPleftText);
            }
            if (EntryPoint.Menu.Item("l33t.stds.drawing.drawHPFill").GetValue<bool>())
            {
                var diff = currentHpxPos - comboXPos;
                for (var i = 0; i < diff; i++)
                {
                    Drawing.DrawLine(
                        (float)comboXPos + i, hpBarPos.Y + 2, (float)comboXPos + i, hpBarPos.Y + 10, 1,
                        barcolor);
                }
            }
        }

        private static void DrawSphereEToCursor(bool classic)
        {
            var target = TargetSelector.GetTarget(
                        Mechanics.Spells[SpellSlot.SphereE].Range, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget())
            {
                if (classic)
                {
                    Drawing.DrawCircle(
                        Game.CursorPos, 150f,
                        (target.Distance(Game.CursorPos, true) <= 22500)
                            ? System.Drawing.Color.Red
                            : EntryPoint.Menu.Item("l33t.stds.drawing.drawQEC").GetValue<Circle>().Color);
                }
                else
                {
                    Render.Circle.DrawCircle(
                        Game.CursorPos, 150f,
                        (target.Distance(Game.CursorPos, true) <= 22500)
                            ? System.Drawing.Color.Red
                            : EntryPoint.Menu.Item("l33t.stds.drawing.drawQEC").GetValue<Circle>().Color);
                }
            }
        }

        private static void DrawHud()
        {
            if (EntryPoint.Menu.Item("l33t.stds.harass.togglekey").GetValue<KeyBind>().Active)
            {
                Drawing.DrawText(
                    Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.Yellow,
                    "Auto Harass : On");
            }
            else
            {
                Drawing.DrawText(
                    Drawing.Width * 0.90f, Drawing.Height * 0.68f, System.Drawing.Color.DarkRed,
                    "Auto Harass : Off");
            }

            if (EntryPoint.Menu.Item("l33t.stds.ks.togglekey").GetValue<KeyBind>().Active)
            {
                Drawing.DrawText(
                    Drawing.Width * 0.90f, Drawing.Height * 0.665f, System.Drawing.Color.Yellow, "Auto KS : On");
            }
            else
            {
                Drawing.DrawText(
                    Drawing.Width * 0.90f, Drawing.Height * 0.665f, System.Drawing.Color.DarkRed,
                    "Auto KS : Off");
            }
        }

        private static void DrawSphereEMap(bool classic)
        {
            var qeTarget = TargetSelector.GetTarget(
                        Mechanics.Spells[SpellSlot.SphereE].Range, TargetSelector.DamageType.Magical);
            if (qeTarget.IsValidTarget())
            {
                var sPos =
                    Prediction.GetPrediction(
                        qeTarget, Mechanics.Spells[SpellSlot.Q].Delay + Mechanics.Spells[SpellSlot.E].Delay)
                        .UnitPosition;
                var tPos = Mechanics.Spells[SpellSlot.SphereE].Instance.GetPrediction(qeTarget);
                if (tPos != null &&
                    EntryPoint.Player.Distance(sPos, true) > Math.Pow(Mechanics.Spells[SpellSlot.E].Range, 2) &&
                    (Mechanics.Spells[SpellSlot.E].IsReady() ||
                     Mechanics.Spells[SpellSlot.E].Instance.Instance.CooldownExpires - Game.Time < 2) &&
                    Mechanics.Spells[SpellSlot.E].Level > 0)
                {
                    var color = System.Drawing.Color.Red;
                    var orb = EntryPoint.Player.Position +
                              Vector3.Normalize(sPos - EntryPoint.Player.Position) *
                              Mechanics.Spells[SpellSlot.E].Range;
                    Mechanics.Spells[SpellSlot.SphereE].Instance.Delay = Mechanics.Spells[SpellSlot.Q].Delay +
                                                                         Mechanics.Spells[SpellSlot.E].Delay +
                                                                         EntryPoint.Player.Distance(orb) /
                                                                         Mechanics.Spells[SpellSlot.E].Instance
                                                                             .Speed;
                    if (tPos.Hitchance >= HitChance.Medium)
                    {
                        color = System.Drawing.Color.Green;
                    }
                    if (Mechanics.Spells[SpellSlot.Q].Instance.Instance.SData.Mana +
                        Mechanics.Spells[SpellSlot.E].Instance.Instance.SData.Mana > EntryPoint.Player.Mana)
                    {
                        color = System.Drawing.Color.DarkBlue;
                    }
                    var pos = EntryPoint.Player.Position +
                              Vector3.Normalize(tPos.UnitPosition - EntryPoint.Player.Position) * 700;
                    if (classic)
                    {
                        Drawing.DrawCircle(pos, Mechanics.Spells[SpellSlot.Q].Instance.Width, color);
                        Drawing.DrawCircle(
                            tPos.UnitPosition, Mechanics.Spells[SpellSlot.Q].Instance.Width / 2, color);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(pos, Mechanics.Spells[SpellSlot.Q].Instance.Width, color);
                        Render.Circle.DrawCircle(
                            tPos.UnitPosition, Mechanics.Spells[SpellSlot.Q].Instance.Width / 2, color);
                    }
                    var sp1 = pos + Vector3.Normalize(EntryPoint.Player.Position - pos) * 100f;
                    var sp = Drawing.WorldToScreen(sp1);
                    var ep1 = pos + Vector3.Normalize(pos - EntryPoint.Player.Position) * 592;
                    var ep = Drawing.WorldToScreen(ep1);
                    Drawing.DrawLine(sp.X, sp.Y, ep.X, ep.Y, 2, color);
                }
            }
        }

        private static void DrawForceOfWillMap(bool classic)
        {
            var color2 = System.Drawing.Color.FromArgb(100, 255, 0, 0);
            var wTarget =
                TargetSelector.GetTarget(
                    Mechanics.Spells[SpellSlot.W].Range + Mechanics.Spells[SpellSlot.W].Instance.Width,
                    TargetSelector.DamageType.Magical);
            if (wTarget.IsValidTarget())
            {
                var pos2 = Mechanics.Spells[SpellSlot.W].Instance.GetPrediction(wTarget, true);
                if (pos2.Hitchance >= HitChance.High)
                {
                    color2 = System.Drawing.Color.FromArgb(100, 50, 150, 255);
                }
                if (classic)
                {
                    Drawing.DrawCircle(pos2.UnitPosition, Mechanics.Spells[SpellSlot.W].Instance.Width, color2);
                }
                else
                {
                    Render.Circle.DrawCircle(
                        pos2.UnitPosition, Mechanics.Spells[SpellSlot.W].Instance.Width, color2);
                }
            }
        }

        private static void DrawDebug()
        {
            var minions = ObjectCache.GetMinions();
            var heros = ObjectCache.GetHeroes();

            Sprite.Begin(SpriteFlags.AlphaBlend);

            Font.DrawText(
                Sprite,
                "Close Minion Count: " + (minions != null ? minions.Count(m => m.IsValidTarget(475f)) : 0),
                (int)(Drawing.Width * 0.90f), (int)(Drawing.Height * 0.64f), Color.White);
            Font.DrawText(
                Sprite, "Minion Count: " + (minions != null ? minions.Count() : 0),
                (int)(Drawing.Width * 0.90f), (int)(Drawing.Height * 0.625f), Color.White);
            Font.DrawText(
                Sprite, "Hero Count: " + (heros != null ? heros.Count() : 0), (int)(Drawing.Width * 0.90f),
                (int)(Drawing.Height * 0.615f), Color.White);
            FontB.DrawText(Sprite, "Welcome back, " + ObjectManager.Player.Name + ".", (int)(Drawing.Width * 0.90f), (int)(Drawing.Height * 0.60f), Color.Red);

            Sprite.End();
        }
    }
}