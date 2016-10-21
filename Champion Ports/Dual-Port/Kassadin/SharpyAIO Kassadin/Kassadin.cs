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
    public class Kassadin
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private Spell Q, W, E, R;
        
        public Kassadin()
        {
            Chat.Print("Sharpy AIO :: Kassadin Loaded :)");

            Q = new Spell(SpellSlot.Q, 650f, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 350f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 660f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };
            R = new Spell(SpellSlot.R, 500f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };

            Q.SetTargetted(.25f, 1400f);
            E.SetSkillshot(.3f, (float)(80f + Math.PI / 180f), float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(.25f, 150f, 2300f, false, SkillshotType.SkillshotCircle);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Kassadin", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R (No Damage)").SetValue(true));
            combo.AddItem(new MenuItem("CM", "Block R If Mana").SetValue(new Slider(60,0,100)));
            combo.AddItem(new MenuItem("CRR", "Cast R Min Distance To Enemy").SetValue(new Slider(500, 0, 500)));
            Menu.AddSubMenu(combo);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(harass);

            // 이동 메뉴
            var flee = new Menu("Flee", "Flee");
            flee.AddItem(new MenuItem("FR", "Use R").SetValue(true));
            flee.AddItem(new MenuItem("FM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(flee);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LCW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LCE", "Use E").SetValue(true));
            laneclear.AddItem(new MenuItem("LCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleClear");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCW", "Use W").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCE", "Use E").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MK", "Use Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("MI", "Use Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("MA", "Use AntiGapcloser").SetValue(true));
            misc.AddItem(new MenuItem("MS", "Use Sharpy Logic").SetValue(true));
            Menu.AddSubMenu(misc);

            // 킬스틸 메뉴
            var killsteal = new Menu("Killsteal Setting", "Killsteal Setting");
            killsteal.AddItem(new MenuItem("KQ", "Use Q").SetValue(true));
            killsteal.AddItem(new MenuItem("KE", "Use E").SetValue(true));
            killsteal.AddItem(new MenuItem("KR", "Use R").SetValue(true));
            killsteal.AddItem(new MenuItem("KM", "Block R If Mana").SetValue(new Slider(20, 0, 100)));
            misc.AddSubMenu(killsteal);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(true, Color.Green)));
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

            Menu.Item("DIA").ValueChanged += Kassadin_ValueChanged;
            Menu.Item("DIF").ValueChanged += Kassadin_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;

        }

        private void Kassadin_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Kassadin_ValueChanged(object sender, OnValueChangeEventArgs e)
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

                var DE = Menu.Item("DE").GetValue<Circle>();
                if (DE.Active)
                {
                    if (E.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, DE.Color, 3);
                    }
                }

                var DR = Menu.Item("DR").GetValue<Circle>();
                if (DR.Active)
                {
                    if (R.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, DR.Color, 3);
                    }
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("MA").GetValue<bool>())
            {
                if (E.IsReadyPerfectly())
                {
                    if (gapcloser.Sender.IsValidTarget(E.Range))
                    {
                        E.Cast(gapcloser.Sender);
                    }
                }

                if (R.IsReadyPerfectly())
                {
                    if (gapcloser.Sender.IsValidTarget(R.Range))
                    {
                        var rpos = gapcloser.Sender.Position.Extend(Player.Position, -(Player.Position.Distance(gapcloser.Sender.Position) - R.Range));
                        R.Cast(rpos);
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("MI").GetValue<bool>())
            {
                if (Q.IsReadyPerfectly())
                {
                    if (sender.IsValidTarget(Q.Range))
                    {
                        Q.CastOnUnit(sender);
                    }
                }
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
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
                    if (Menu.Item("MS").GetValue<bool>())
                    {
                        var starget = TargetSelector.GetSelectedTarget();
                        if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                        {
                            if (Menu.Item("CW").GetValue<bool>())
                            {
                                if (W.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            if (!starget.IsDead)
                                                W.Cast();
                                        }
                                    }
                                    else
                                    {
                                        var t = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (t != null)
                                        {
                                            W.Cast();
                                        }
                                    }
                                }
                            }
                        }

                        if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                        {
                            if (Menu.Item("HW").GetValue<bool>())
                            {
                                if (W.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            if (!starget.IsDead)
                                                W.Cast();
                                        }
                                    }
                                    else
                                    {
                                        var t = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (t != null)
                                        {
                                            W.Cast();
                                        }
                                    }
                                }
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
                            if (W.IsReadyPerfectly())
                            {
                                if (Menu.Item("LCW").GetValue<bool>())
                                {
                                    var t = MinionManager.GetMinions(W.Range).FirstOrDefault(x => x.IsValidTarget(W.Range));
                                    if (t != null)
                                    {
                                        W.Cast();
                                    }
                                }

                                if (Menu.Item("JCW").GetValue<bool>())
                                {
                                    var t = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(W.Range));
                                    if (t != null)
                                    {
                                        W.Cast();
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
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("FM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("FR").GetValue<bool>())
                                {
                                    if (R.IsReadyPerfectly())
                                    {
                                        R.Cast(Game.CursorPos);
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("HM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("HQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= Q.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (!starget.IsDead)
                                                    Q.CastOnUnit(starget);
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                            if (target != null)
                                            {
                                                Q.CastOnUnit(target);
                                            }
                                        }
                                    }
                                }

                                if (Menu.Item("HE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= E.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (!starget.IsDead)
                                                    E.Cast(starget);
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                            if (target != null)
                                            {
                                                E.Cast(target);
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("HW").GetValue<bool>())
                            {
                                if (!Menu.Item("MS").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (!starget.IsDead)
                                                    W.Cast();
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                            if (target != null)
                                            {
                                                W.Cast();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("LCM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("LCQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                                        if (target != null)
                                        {
                                            Q.CastOnUnit(target);
                                        }
                                    }
                                }

                                if (Menu.Item("LCE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = E.GetLineFarmLocation(MinionManager.GetMinions(E.Range));
                                        if (target.MinionsHit >= 1)
                                        {
                                            E.Cast(target.Position);
                                        }
                                    }
                                }
                            }

                            if (Player.IsManaPercentOkay(Menu.Item("JCM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("JCQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                        if (target != null)
                                        {
                                            Q.CastOnUnit(target);
                                        }
                                    }
                                }

                                if (Menu.Item("JCE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(E.Range) && E.GetPrediction(x).Hitchance >= E.MinHitChance);
                                        if (target != null)
                                        {
                                            E.Cast(target);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (Menu.Item("CR").GetValue<bool>())
                            {
                                if (Player.IsManaPercentOkay(Menu.Item("CM").GetValue<Slider>().Value))
                                {
                                    if (R.IsReadyPerfectly())
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= R.Range + Q.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (!starget.IsDead)
                                                {
                                                    var rpos = starget.Position.Extend(Player.Position, -(Player.Position.Distance(starget.Position)) + (Menu.Item("CRR").GetValue<Slider>().Value));
                                                    R.Cast(rpos);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(R.Range + Q.Range, R.DamageType);
                                            if (target != null)
                                            {
                                                var rpos = target.Position.Extend(Player.Position, -(Player.Position.Distance(target.Position)) +(Menu.Item("CRR").GetValue<Slider>().Value));
                                                R.Cast(rpos);
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("CQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= Q.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            if (!starget.IsDead)
                                                Q.CastOnUnit(starget);
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                        if (target != null)
                                        {
                                            Q.CastOnUnit(target);
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("CE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= E.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            if (!starget.IsDead)
                                                E.Cast(starget);
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                        if (target != null)
                                        {
                                            E.Cast(target);
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("CW").GetValue<bool>())
                            {
                                if (!Menu.Item("MS").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (!starget.IsDead)
                                                    W.Cast();
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                            if (target != null)
                                            {
                                                W.Cast();
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
            }
            Killsteal();
        }

        private void Killsteal()
        {
            if (Menu.Item("MK").GetValue<bool>())
            {
                if (Menu.Item("KR").GetValue<bool>())
                {
                    if (Player.IsManaPercentOkay(Menu.Item("KM").GetValue<Slider>().Value))
                    {
                        if (R.IsReadyPerfectly())
                        {
                            var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(R.GetDamage(x), R.DamageType, R.Range));
                            if (target != null)
                            {
                                R.CastOnUnit(target);
                                return;
                            }
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
                            Q.CastOnUnit(target);
                            return;
                        }
                    }
                }

                if (Menu.Item("KE").GetValue<bool>())
                {
                    if (E.IsReadyPerfectly())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x), E.DamageType, E.Range));
                        if (target != null)
                        {
                            E.CastOnUnit(target);
                            return;
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

            if (R.IsReadyPerfectly())
            {
                damage += R.GetDamage(enemy);
            }

            return damage;
        }
    }
}
