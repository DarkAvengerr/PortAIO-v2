using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ItemData = LeagueSharp.Common.Data.ItemData;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sharpy_AIO.Plugins
{
    public class Hecarim
    {
        private Menu Menu;
        private AIHeroClient Player = ObjectManager.Player;
        private Orbwalking.Orbwalker Orbwalker;
        private Spell Q, W, E, R;

        public Hecarim()
        {
            Chat.Print("Sharpy AIO :: Hecarim Loaded :)");

            Q = new Spell(SpellSlot.Q, 350f, TargetSelector.DamageType.Physical);
            W = new Spell(SpellSlot.W, 525f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E, 325f, TargetSelector.DamageType.Physical);
            R = new Spell(SpellSlot.R, 1000f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };

            R.SetSkillshot(.25f, 300f, 1570f, false, SkillshotType.SkillshotCircle);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Hecarim", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CC", "Use R Min Hit").SetValue(new Slider(1, 1, 5)));
            Menu.AddSubMenu(combo);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(harass);

            // 이동 메뉴
            var flee = new Menu("Flee", "Flee");
            flee.AddItem(new MenuItem("FE", "Use E").SetValue(true));
            flee.AddItem(new MenuItem("FM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(flee);

            // 막타 메뉴
            var lasthit = new Menu("LastHit", "LastHit");
            lasthit.AddItem(new MenuItem("LHQ", "Use Q").SetValue(true));
            lasthit.AddItem(new MenuItem("LHM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(lasthit);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LCW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleCelar");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCW", "Use W").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MA", "AntiGapcloser Mode").SetValue(new StringList(new[] { "E + AA", "R", "Never" }, 1)));
            misc.AddItem(new MenuItem("MI", "Interrupter Mode").SetValue(new StringList(new[] { "E + AA", "R", "Never" }, 0)));
            misc.AddItem(new MenuItem("MK", "Use R for Killsteal").SetValue(true));
            Menu.AddSubMenu(misc);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
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

            //new DamageIndicator();
            //DamageIndicator.DamageToUnit = getcombodamage;
            //DamageIndicator.Enabled = Menu.Item("DIA").GetValue<bool>();
            //DamageIndicator.Fill = Menu.Item("DIF").GetValue<Circle>().Active;
            //DamageIndicator.FillColor = Menu.Item("DIF").GetValue<Circle>().Color;

            Menu.Item("DIA").ValueChanged += Hecarim_ValueChanged;
            Menu.Item("DIF").ValueChanged += Hecarim_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Hecarim_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            //DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            //DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Hecarim_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            //DamageIndicator.Enabled = e.GetNewValue<bool>();
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
                    if (W.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, DW.Color, 3);
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

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            switch (Menu.Item("MI").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        if (E.IsReadyPerfectly())
                        {
                            if (sender.IsValidTarget(E.Range))
                            {
                                E.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                            }
                        }
                    }
                    break;

                case 1:
                    {
                        if (R.IsReadyPerfectly())
                        {
                            if (sender.IsValidTarget(R.Range))
                            {
                                R.CastOnUnit(sender);
                            }
                        }
                    }
                    break;
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            switch (Menu.Item("MA").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    {
                        if (E.IsReadyPerfectly())
                        {
                            if (gapcloser.Sender.IsValidTarget(E.Range))
                            {
                                E.Cast();
                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        if (R.IsReadyPerfectly())
                        {
                            if (gapcloser.Sender.IsValidTarget(R.Range))
                            {
                                R.CastOnUnit(gapcloser.Sender);
                            }
                        }
                    }
                    break;
                case 2:
                    break;
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
                    if (Menu.Item("CQ").GetValue<bool>() || Menu.Item("HQ").GetValue<bool>() && Player.IsManaPercentOkay(Menu.Item("HM").GetValue<Slider>().Value))
                    {
                        if (Q.IsReadyPerfectly())
                        {
                            var starget = TargetSelector.GetSelectedTarget();
                            if (starget != null && Player.Position.Distance(starget.Position) <= Q.Range)
                            {
                                Q.Cast();
                            }
                        }
                    }
                }
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                {
                    if (Player.IsManaPercentOkay(Menu.Item("LHM").GetValue<Slider>().Value))
                    {
                        if (Menu.Item("LHQ").GetValue<bool>())
                        {
                            if (Q.IsReadyPerfectly())
                            {
                                args.Process = false;
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
                                if (E.IsReadyPerfectly())
                                {
                                    if (Menu.Item("FE").GetValue<bool>())
                                    {
                                        E.Cast();
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("LHM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("LHQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                                        if (target != null)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }
                            }
                        }
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
                                                Q.Cast();
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                            if (target != null)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                    }
                                }

                                if (Menu.Item("HW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (starget != null&& Player.Position.Distance(starget.Position) <= W.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
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
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                        if (target != null)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }

                                if (Menu.Item("LCW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(W.Range).FirstOrDefault(x => x.IsValidTarget(W.Range));
                                        if (target != null)
                                        {
                                            W.Cast();
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
                                            Q.Cast();
                                        }
                                    }
                                }

                                if (Menu.Item("JCW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(W.Range));
                                        if (target != null)
                                        {
                                            W.Cast();
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
                                if (R.IsReadyPerfectly())
                                {
                                    var rc = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)).ToList();
                                    if (rc.Count >= Menu.Item("CC").GetValue<Slider>().Value)
                                    {
                                        if (starget != null && Player.Position.Distance(starget.Position) <= R.Range)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                R.Cast(starget, false, true);
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(R.Range, R.DamageType);
                                            if (target != null)
                                            {
                                                R.Cast(target, false, true);
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
                                            Q.Cast();
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                        if (target != null)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("CW").GetValue<bool>())
                            {
                                if (W.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
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

                            if (Menu.Item("CE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= E.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            E.Cast();
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                        if (target != null)
                                        {
                                            E.Cast();
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
                if (R.IsReadyPerfectly())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(R.GetDamage(x), R.DamageType, R.Range));
                    if (target != null)
                    {
                        R.CastOnUnit(target);
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
