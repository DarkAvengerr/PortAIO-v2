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
    public class Cassiopeia
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private SpellSlot Flash = ObjectManager.Player.GetSpellSlot("summonerFlash");
        private Spell Q, W, E, R;

        private bool IsPoisoned(Obj_AI_Base unit)
        {
            return
                unit.Buffs.Where(x => x.IsActive && x.Type == BuffType.Poison).Any(x => x.EndTime >= (Game.Time + .35 + 700 / 1900));
        }

        public Cassiopeia()
        {
            Chat.Print("Sharpy AIO :: Cassiopeia Loaded :)");

            Q = new Spell(SpellSlot.Q, 850f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };
            W = new Spell(SpellSlot.W, 850f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.VeryHigh };
            E = new Spell(SpellSlot.E, 700f, TargetSelector.DamageType.Magical);
            R = new Spell(SpellSlot.R, 825f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };

            Q.SetSkillshot(.75f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            W.SetSkillshot(.25f, 100f, 2450f, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0f, float.MaxValue);
            R.SetSkillshot(.5f, (float)(80f + Math.PI / 180), float.MaxValue, false, SkillshotType.SkillshotCone);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Cassiopeia", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo","Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CC", "Use R Min Hit").SetValue(new Slider(1, 1, 5)));
            combo.AddItem(new MenuItem("CF", "Use R Min Facing").SetValue(new Slider(1, 1, 5)));
            combo.AddItem(new MenuItem("CA", "Disable AutoAttack").SetValue(true));
            combo.AddItem(new MenuItem("CK", "Use R Semi-automatic").SetValue(new KeyBind('R', KeyBindType.Press)));
            Menu.AddSubMenu(combo);
            
            // 궁 + 플래쉬 메뉴
            var rfmenu = new Menu("R + Flash", "RFMenu");
            rfmenu.AddItem(new MenuItem("RF", "R + Flash").SetValue(new KeyBind('T', KeyBindType.Press)));
            rfmenu.AddItem(new MenuItem("R0", "How To R + Flash?"));
            rfmenu.AddItem(new MenuItem("R1", "1. Turn On TargetSelector -> Focus Selected Target"));
            rfmenu.AddItem(new MenuItem("R2", "2. Left Click Target"));
            rfmenu.AddItem(new MenuItem("R3", "3. Wait Target In the R + Flash Range"));
            rfmenu.AddItem(new MenuItem("R4", "4. Press R + Flash Key (Default Key 'T')"));
            combo.AddSubMenu(rfmenu);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HA", "Use Q Auto Harass Toggle").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("HM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(harass);

            // 막타 메뉴
            var lasthit = new Menu("LastHit", "LastHit");
            lasthit.AddItem(new MenuItem("LHE", "Use E").SetValue(true));
            lasthit.AddItem(new MenuItem("LHN", "Non Poison").SetValue(true));
            lasthit.AddItem(new MenuItem("LHM", "If Mana").SetValue(new Slider(0, 0, 100)));
            Menu.AddSubMenu(lasthit);

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
            misc.AddItem(new MenuItem("MA", "Use AntiGapcloser").SetValue(true));
            misc.AddItem(new MenuItem("MI", "Use Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("MQ", "Use Q Killsteal").SetValue(false));
            misc.AddItem(new MenuItem("ME", "Use E Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("MN", "Use E Killsteal non Poisoned").SetValue(true));
            misc.AddItem(new MenuItem("MR", "Use R Killsteal").SetValue(true));
            Menu.AddSubMenu(misc);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DF", "Draw R + Flash Range").SetValue(new Circle(true, Color.Green)));
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

            Menu.Item("DIA").ValueChanged += Cassiopeia_ValueChanged;
            Menu.Item("DIF").ValueChanged += Cassiopeia_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Cassiopeia_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            //DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            //DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Cassiopeia_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            //DamageIndicator.Enabled = e.GetNewValue<bool>();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("DO").GetValue<bool>())
            {
                var drawQ = Menu.Item("DQ").GetValue<Circle>();
                if (drawQ.Active)
                {
                    if (Q.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range, drawQ.Color, 3);
                    }
                }

                var drawW = Menu.Item("DW").GetValue<Circle>();
                if (drawW.Active)
                {
                    if (W.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, drawW.Color, 3);
                    }
                }

                var drawE = Menu.Item("DE").GetValue<Circle>();
                if (drawE.Active)
                {
                    if (E.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, drawE.Color, 3);
                    }
                }

                var drawR = Menu.Item("DR").GetValue<Circle>();
                if (drawR.Active)
                {
                    if (R.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, drawR.Color, 3);
                    }
                }

                var drawF = Menu.Item("DF").GetValue<Circle>();
                if (drawF.Active)
                {
                    if (R.IsReadyPerfectly() && Flash.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range + 425f, drawF.Color, 3);
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("MI").GetValue<bool>())
            {
                if (R.IsReadyPerfectly())
                {
                    if (sender.IsValidTarget(R.Range))
                    {
                        if (sender.IsFacing(Player))
                        {
                            R.Cast(sender);
                        }
                    }
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("MA").GetValue<bool>())
            {
                if (R.IsReadyPerfectly())
                {
                    if (gapcloser.Sender.IsValidTarget(R.Range))
                    {
                        if (gapcloser.Sender.IsFacing(Player))
                        {
                            R.Cast(gapcloser.Sender);
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
                        if (Menu.Item("LHE").GetValue<bool>())
                        {
                            if (E.IsReadyPerfectly())
                            {
                                args.Process = false;
                            }
                        }
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Menu.Item("CA").GetValue<bool>())
                {
                    args.Process = false;
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(20))
            {
                if (Menu.Item("CK").GetValue<KeyBind>().Active)
                {
                    var starget = TargetSelector.GetSelectedTarget();
                    if (R.IsReadyPerfectly())
                    {
                        if (starget != null && starget.Position.Distance(Player.Position) <= R.Range)
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
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("LHM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("LHE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        if (!Menu.Item("LHN").GetValue<bool>())
                                        {
                                            var target = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x, 1), E.DamageType, E.Range) && IsPoisoned(x));
                                            if (target != null)
                                            {
                                                E.CastOnUnit(target);
                                            }
                                        }
                                        else
                                        {
                                            var target = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x, 1), E.DamageType, E.Range));
                                            if (target != null)
                                            {
                                                E.CastOnUnit(target);
                                            }
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
                                if (Menu.Item("HE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                        if (target != null && IsPoisoned(target))
                                        {
                                            E.CastOnUnit(target);
                                        }
                                    }
                                }

                                if (Menu.Item("HQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                        if (target != null)
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                }

                                if (Menu.Item("HW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null)
                                        {
                                            W.Cast(target);
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
                                        var target = Q.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range));
                                        if (target.MinionsHit >= 1)
                                        {
                                            Q.Cast(target.Position);
                                        }
                                    }
                                }

                                if (Menu.Item("LCW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = W.GetCircularFarmLocation(MinionManager.GetMinions(W.Range));
                                        if (target.MinionsHit >= 1)
                                        {
                                            W.Cast(target.Position);
                                        }
                                    }
                                }

                                if (Menu.Item("LCE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x),E.DamageType,E.Range) && IsPoisoned(x));
                                        if (target != null)
                                        {
                                            E.CastOnUnit(target);
                                        }
                                        else
                                        {
                                            var otarget = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.IsValidTarget(E.Range) && IsPoisoned(x));
                                            if (otarget != null)
                                            {
                                                E.CastOnUnit(otarget);
                                            }
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
                                        var target = MinionManager.GetMinions(700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.IsValidTarget(700) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
                                        if (target != null)
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                }

                                if (Menu.Item("JCW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.IsValidTarget(700) && W.GetPrediction(x).Hitchance >= W.MinHitChance);
                                        if (target != null)
                                        {
                                            W.Cast(target);
                                        }
                                    }
                                }

                                if (Menu.Item("JCE").GetValue<bool>())
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.IsValidTarget(E.Range) && IsPoisoned(x));
                                        if (target != null)
                                        {
                                            E.CastOnUnit(target);
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
                                    var target = TargetSelector.GetTarget(R.Range, R.DamageType);
                                    if (target != null)
                                    {
                                        var ctarget = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)).ToList();
                                        var ftarget = ctarget.Where(x => x.IsFacing(Player)).ToList();

                                        if (ctarget.Count >= Menu.Item("CC").GetValue<Slider>().Value || ftarget.Count >= Menu.Item("CF").GetValue<Slider>().Value)
                                        {
                                            R.Cast(target, false, true);
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("CE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                    if (target != null && IsPoisoned(target))
                                    {
                                        E.CastOnUnit(target);
                                    }
                                }
                            }

                            if (Menu.Item("CQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                    if (target != null)
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }

                            if (Menu.Item("CW").GetValue<bool>())
                            {
                                if (W.IsReadyPerfectly())
                                {
                                    var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                    if (target != null)
                                    {
                                        W.Cast(target);
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.None:
                        break;
                }

                if (Menu.Item("RF").GetValue<KeyBind>().Active)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var target = TargetSelector.GetSelectedTarget();
                    if (target != null && Player.Position.Distance(target.Position) < R.Range + 425f)
                    {
                        if (!target.IsZombie)
                        {
                            if (Flash != SpellSlot.Unknown)
                            {
                                if (R.IsReadyPerfectly() && Flash.IsReady())
                                {
                                    R.Cast();
                                    LeagueSharp.Common.Utility.DelayAction.Add(5, () => Player.Spellbook.CastSpell(Flash, target.Position));
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("HA").GetValue<KeyBind>().Active)
                {
                    if (Player.IsManaPercentOkay(Menu.Item("HM").GetValue<Slider>().Value))
                    {
                        if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                        {
                            if (Q.IsReadyPerfectly())
                            {
                                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                if (target != null)
                                {
                                    Q.Cast(target);
                                }
                            }
                        }
                    }
                }
            }
            KillSteal();
        }

        private void KillSteal()
        {
            if (Menu.Item("ME").GetValue<bool>())
            {
                if (E.IsReadyPerfectly())
                {
                    if (!Menu.Item("MN").GetValue<bool>())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x), E.DamageType, E.Range) && IsPoisoned(x));
                        if (target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }
                    }
                    else
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x), E.DamageType, E.Range));
                        if (target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }
                    }
                }
            }

            if (Menu.Item("MR").GetValue<bool>())
            {
                if (R.IsReadyPerfectly())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(R.GetDamage(x), R.DamageType, R.Range));
                    if (target.IsValidTarget(R.Range))
                    {
                        R.Cast(target);
                    }
                }
            }

            if (Menu.Item("MQ").GetValue<bool>())
            {
                if (Q.IsReadyPerfectly())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(target);
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
