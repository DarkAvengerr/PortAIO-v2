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
 namespace Sharpy_AIO.Plugins
{
    public class DrMundo
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private Spell Q, W, E, R;

        public DrMundo()
        {
            Chat.Print("Sharpy AIO :: Dr.Mundo Loaded :)");

            Q = new Spell(SpellSlot.Q, 1000f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.VeryHigh };
            W = new Spell(SpellSlot.W, 325f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(.25f, 60f, 1850f, true, SkillshotType.SkillshotLine);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Dr.Mundo", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CH", "Use R If HP").SetValue(new Slider(30, 0, 100)));
            Menu.AddSubMenu(combo);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HE", "Use E").SetValue(true));
            Menu.AddSubMenu(harass);

            // 막타 메뉴
            var lasthit = new Menu("LastHit", "LastHit");
            lasthit.AddItem(new MenuItem("LHQ", "Use Q(Long)").SetValue(true));
            Menu.AddSubMenu(lasthit);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LCE", "Use E").SetValue(true));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleClear");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCE", "Use E").SetValue(true));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MK", "Use Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("MB", "Block W If HP").SetValue(new Slider(30, 0, 100)));
            misc.AddItem(new MenuItem("MA", "Auto W Off").SetValue(true));
            Menu.AddSubMenu(misc);

            // 킬스틸 메뉴
            var killsteal = new Menu("Killsteal Setting", "Killsteal Setting");
            killsteal.AddItem(new MenuItem("KQ", "Use Q").SetValue(true));
            killsteal.AddItem(new MenuItem("KEQ", "Use EQ").SetValue(true));
            misc.AddSubMenu(killsteal);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DH", "Draw HP Percent").SetValue(true));
            drawing.AddItem(new MenuItem("DO", "Disable All Drawings").SetValue(false));
            Menu.AddSubMenu(drawing);

            // 데미지 인디케이터 메뉴
            var da = new Menu("Damage Indicator", "Damage Indicator");
            da.AddItem(new MenuItem("DIA", "Use Damage Indicator").SetValue(true));
            da.AddItem(new MenuItem("DIF", "Damage Indicator Fill Color").SetValue(new Circle(true, Color.Goldenrod)));
            Menu.AddSubMenu(da);

            Menu.AddToMainMenu();

            new DamageIndicator();
            DamageIndicator.DamageToUnit = getcombodamage;
            DamageIndicator.Enabled = Menu.Item("DIA").GetValue<bool>();
            DamageIndicator.Fill = Menu.Item("DIF").GetValue<Circle>().Active;
            DamageIndicator.FillColor = Menu.Item("DIF").GetValue<Circle>().Color;

            Menu.Item("DIA").ValueChanged += DrMundo_ValueChanged;
            Menu.Item("DIF").ValueChanged += DrMundo_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void DrMundo_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void DrMundo_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            DamageIndicator.Enabled = e.GetNewValue<bool>();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("DO").GetValue<bool>())
            {
                var DQ = Menu.Item("DQ").GetValue<Circle>();
                if (DQ.Active)
                {
                    if (Q.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range, DQ.Color, 3);
                    }
                }

                var DW = Menu.Item("DW").GetValue<Circle>();
                if (DW.Active)
                {
                    Render.Circle.DrawCircle(Player.Position, W.Range, DW.Color, 3);
                }

                if (Menu.Item("DH").GetValue<bool>())
                {
                    var position = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    Drawing.DrawText(position.X, position.Y + 40, Color.White, "HP Percent :" + Player.HealthPercent);
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (target is AIHeroClient)
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (Menu.Item("CE").GetValue<bool>())
                        {
                            if (E.IsReadyPerfectly())
                            {
                                E.Cast();
                            }
                        }
                    }

                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (Menu.Item("HE").GetValue<bool>())
                        {
                            if (E.IsReadyPerfectly())
                            {
                                E.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (target is Obj_AI_Minion)
                    {
                        if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                        {
                            if (Menu.Item("LCE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    var t = MinionManager.GetMinions(125f).FirstOrDefault(x => x.IsValidTarget(125f));
                                    if (t != null)
                                    {
                                        E.Cast();
                                    }
                                }
                            }

                            if (Menu.Item("JCE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    var t = MinionManager.GetMinions(125f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                        .FirstOrDefault(x => x.IsValidTarget(125f));
                                    if (t != null)
                                    {
                                        E.Cast();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(20))
            {
                var starget = TargetSelector.GetSelectedTarget();
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            if (Menu.Item("LHQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
                                    if (target != null)
                                    {
                                        if (Player.Position.Distance(target.Position) > 125f)
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        {
                            if (Menu.Item("HQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= Q.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            Q.Cast(starget);
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTargetNoCollision(Q);
                                        if (target.IsValidTarget(Q.Range))
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                }
                            }

                            if (Player.HealthPercent >= Menu.Item("MB").GetValue<Slider>().Value)
                            {
                                if (Menu.Item("HW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (starget != null)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (Player.Position.Distance(starget.Position) <= W.Range + 20f && W.Instance.ToggleState == 1)
                                                {
                                                    W.Cast();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(W.Range + 20f, W.DamageType);
                                            if (target != null)
                                            {
                                                if (W.Instance.ToggleState == 1)
                                                {
                                                    W.Cast();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Menu.Item("LCQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
                                    if (target != null)
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }

                            if (Menu.Item("JCQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                        .FirstOrDefault(x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
                                    if (target != null)
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (Menu.Item("CQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= Q.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            Q.Cast(starget);
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTargetNoCollision(Q);
                                        if (target.IsValidTarget(Q.Range))
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                }
                            }

                            if (Player.HealthPercent >= Menu.Item("MB").GetValue<Slider>().Value)
                            {
                                if (Menu.Item("CW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (starget != null)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (Player.Position.Distance(starget.Position) <= W.Range + 20f && W.Instance.ToggleState == 1)
                                                {
                                                    W.Cast();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(W.Range + 20f, W.DamageType);
                                            if (target != null)
                                            {
                                                if (W.Instance.ToggleState == 1)
                                                {
                                                    W.Cast();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.None:
                        break;
                }

                if (Menu.Item("CR").GetValue<bool>())
                {
                    if (Player.HealthPercent <= Menu.Item("CH").GetValue<Slider>().Value)
                    {
                        if (R.IsReadyPerfectly())
                        {
                            R.Cast();
                        }
                    }
                }

                if (Menu.Item("MA").GetValue<bool>())
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    {
                        var target = MinionManager.GetMinions(W.Range).FirstOrDefault(x => x.IsValidTarget(W.Range));
                        var t = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(W.Range));

                        if (target == null && t == null)
                        {
                            if (W.Instance.ToggleState != 1)
                            {
                                W.Cast();
                            }
                        }
                    }
                    else
                    {
                        if (starget != null)
                        {
                            if (starget.Position.Distance(Player.Position) >= 599.5f)
                            {
                                if (W.Instance.ToggleState != 1)
                                {
                                    W.Cast();
                                }
                            }
                        }
                        else
                        {
                            var target = TargetSelector.GetTarget(float.MaxValue, W.DamageType);
                            if (target != null)
                            {
                                if (target.Position.Distance(Player.Position) >= 599.5f)
                                {
                                    if (W.Instance.ToggleState != 1)
                                    {
                                        W.Cast();
                                    }
                                }
                            }
                            else
                            {
                                if (W.Instance.ToggleState != 1)
                                {
                                    W.Cast();
                                }
                            }
                        }
                    }
                }
            }

            if (Player.HealthPercent < Menu.Item("MB").GetValue<Slider>().Value)
            {
                if (W.Instance.ToggleState != 1)
                {
                    W.Cast();
                }
            }

            KillSteal();
        }

        private void KillSteal()
        {
            if (Menu.Item("MK").GetValue<bool>())
            {
                if (Menu.Item("KEQ").GetValue<bool>())
                {
                    if (Q.IsReadyPerfectly() && E.IsReadyPerfectly())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x) + E.GetDamage(x), Q.DamageType, Q.Range));
                        if (target != null)
                        {
                            E.Cast();
                            Q.Cast(target);
                        }
                    }
                }

                if (Menu.Item("KQ").GetValue<bool>())
                {
                    if (Q.IsReadyPerfectly())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                        if (target != null)
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
        }

        private float getcombodamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (Q.IsReadyPerfectly())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReadyPerfectly())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.IsReadyPerfectly())
            {
                damage += E.GetDamage(enemy);
            }

            return damage;
        }
    }
}
