using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SebbyLib;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LordsSyndra
{
    public static class Drawings
    {
        internal static void Draw()
        {
            var menuItem = Program.Menu.Item("DrawQE").GetValue<Circle>();
            if (menuItem.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Spells.QE.Range, menuItem.Color);
            }
            menuItem = Program.Menu.Item("DrawQEC").GetValue<Circle>();

            if (Program.Menu.Item("drawing").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    if (enemy.IsVisible && !enemy.IsDead)
                    {
                        //Draw Combo Damage to Enemy HP bars

                        var hpBarPos = enemy.HPBarPosition;
                        hpBarPos.X += 45;
                        hpBarPos.Y += 18;
                        var killText = "";
                        var combodamage = GetDamage.GetComboDamage(
                            enemy, Program.Menu.Item("UseQ").GetValue<bool>(), Program.Menu.Item("UseW").GetValue<bool>(),
                            Program.Menu.Item("UseE").GetValue<bool>(), Program.Menu.Item("UseR").GetValue<bool>());
                        var PercentHPleftAfterCombo = (enemy.Health - combodamage) / enemy.MaxHealth;
                        var PercentHPleft = enemy.Health / enemy.MaxHealth;
                        if (PercentHPleftAfterCombo < 0)
                            PercentHPleftAfterCombo = 0;
                        double comboXPos = hpBarPos.X - 36 + (107 * PercentHPleftAfterCombo);
                        double currentHpxPos = hpBarPos.X - 36 + (107 * PercentHPleft);
                        var barcolor = Color.FromArgb(100, 0, 220, 0);
                        var barcolorline = Color.WhiteSmoke;
                        if (combodamage + ObjectManager.Player.GetSpellDamage(enemy, SpellSlot.Q) +
                            ObjectManager.Player.GetAutoAttackDamage(enemy) * 2 > enemy.Health)
                        {
                            killText = "Killable by: Full Combo + 1Q + 2AA";
                            if (combodamage >= enemy.Health)
                                killText = "Killable by: Full Combo";
                            barcolor = Color.FromArgb(100, 255, 255, 0);
                            barcolorline = Color.SpringGreen;
                            var linecolor = barcolor;
                            if (
                                GetDamage.GetComboDamage(
                                    enemy, Program.Menu.Item("UseQ").GetValue<bool>(), Program.Menu.Item("UseW").GetValue<bool>(),
                                    Program.Menu.Item("UseE").GetValue<bool>(), false) > enemy.Health)
                            {
                                killText = "Killable by: Q + W + E";
                                barcolor = Color.FromArgb(130, 255, 70, 0);
                                linecolor = Color.FromArgb(150, 255, 0, 0);
                            }
                            if (Program.Menu.Item("Gank").GetValue<bool>())
                            {
                                var pos = ObjectManager.Player.Position +
                                          Vector3.Normalize(enemy.Position - ObjectManager.Player.Position) * 100;
                                var myPos = Drawing.WorldToScreen(pos);
                                pos = ObjectManager.Player.Position + Vector3.Normalize(enemy.Position - ObjectManager.Player.Position) * 350;
                                var ePos = Drawing.WorldToScreen(pos);
                                Drawing.DrawLine(myPos.X, myPos.Y, ePos.X, ePos.Y, 1, linecolor);
                            }
                        }
                        var killTextPos = Drawing.WorldToScreen(enemy.Position);
                        var hPleftText = Math.Round(PercentHPleftAfterCombo * 100) + "%";
                        Drawing.DrawLine(
                            (float)comboXPos, hpBarPos.Y, (float)comboXPos, hpBarPos.Y + 5, 1, barcolorline);
                        if (Program.Menu.Item("KillText").GetValue<bool>())
                            Drawing.DrawText(killTextPos[0] - 105, killTextPos[1] + 25, barcolor, killText);
                        if (Program.Menu.Item("KillTextHP").GetValue<bool>())
                            Drawing.DrawText(hpBarPos.X + 98, hpBarPos.Y + 5, barcolor, hPleftText);
                        if (Program.Menu.Item("DrawHPFill").GetValue<bool>())
                        {
                            var diff = currentHpxPos - comboXPos;
                            for (var i = 0; i < diff; i++)
                            {
                                Drawing.DrawLine(
                                    (float)comboXPos + i, hpBarPos.Y + 2, (float)comboXPos + i,
                                    hpBarPos.Y + 10, 1, barcolor);
                            }
                        }
                    }

                    //Draw QE to cursor circle
                    if (Program.Menu.Item("UseQEC").GetValue<KeyBind>().Active && Spells.E.IsReady() && Spells.Q.IsReady() && menuItem.Active)
                    {
                        Render.Circle.DrawCircle(Game.CursorPos, 150f,
                            (enemy.Distance(Game.CursorPos, true) <= 150 * 150) ? Color.Red : menuItem.Color, 3);
                    }
                }
            }

            foreach (var spell in Spells.SpellList)
            {
                // Draw Spell Ranges
                menuItem = Program.Menu.Item("Draw" + spell.Slot).GetValue<Circle>();
                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, menuItem.Color);
                }
            }

            // Dashboard Indicators
            if (Program.Menu.Item("HUD").GetValue<bool>())
            {
                if (Program.Menu.Item("HarassActiveT").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.Yellow, "Auto Harass : On");
                else Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.68f, Color.DarkRed, "Auto Harass : Off");

                if (Program.Menu.Item("AutoKST").GetValue<KeyBind>().Active)
                    Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.Yellow, "Auto KS : On");
                else Drawing.DrawText(Drawing.Width * 0.90f, Drawing.Height * 0.66f, Color.DarkRed, "Auto KS : Off");
            }
            // Draw QE MAP
            if (Program.Menu.Item("DrawQEMAP").GetValue<bool>())
            {
                var qeTarget = TargetSelector.GetTarget(Spells.QE.Range, TargetSelector.DamageType.Magical);
                var sPos = Prediction.GetPrediction(qeTarget, Spells.Q.Delay + Spells.E.Delay).UnitPosition;
                var tPos = Spells.QE.GetPrediction(qeTarget);
                if (tPos != null && ObjectManager.Player.Distance(sPos, true) > Math.Pow(Spells.E.Range, 2) &&
                    (Spells.E.IsReady() || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).CooldownExpires - Game.Time < 2) &&
                    ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level > 0)
                {
                    var color = Color.Red;
                    var orb = ObjectManager.Player.Position + Vector3.Normalize(sPos - ObjectManager.Player.Position) * Spells.E.Range;
                    Spells.QE.Delay = Spells.Q.Delay + Spells.E.Delay + ObjectManager.Player.Distance(orb) / Spells.E.Speed;
                    if (tPos.Hitchance >= HitChance.Medium)
                        color = Color.Green;
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana +
                        ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana > ObjectManager.Player.Mana)
                        color = Color.DarkBlue;
                    var pos = ObjectManager.Player.Position + Vector3.Normalize(tPos.UnitPosition - ObjectManager.Player.Position) * 700;
                    Render.Circle.DrawCircle(pos, Spells.Q.Width, color);
                    Render.Circle.DrawCircle(tPos.UnitPosition, Spells.Q.Width / 2, color);
                    var sp1 = pos + Vector3.Normalize(ObjectManager.Player.Position - pos) * 100f;
                    var sp = Drawing.WorldToScreen(sp1);
                    var ep1 = pos + Vector3.Normalize(pos - ObjectManager.Player.Position) * 592;
                    var ep = Drawing.WorldToScreen(ep1);
                    Drawing.DrawLine(sp.X, sp.Y, ep.X, ep.Y, 2, color);
                }

            }

            if (!Program.Menu.Item("DrawWMAP").GetValue<bool>() || ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level <= 0)
            {
                return;
            }
            var color2 = Color.FromArgb(100, 255, 0, 0);
            var wTarget = TargetSelector.GetTarget(Spells.W.Range + Spells.W.Width, TargetSelector.DamageType.Magical);
            var pos2 = Spells.W.GetPrediction(wTarget, true);
            if (pos2.Hitchance >= HitChance.High)
                color2 = Color.FromArgb(100, 50, 150, 255);
            Render.Circle.DrawCircle(pos2.UnitPosition, Spells.W.Width, color2);
        }
    }
}
