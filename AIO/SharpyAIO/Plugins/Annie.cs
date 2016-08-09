using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Color = System.Drawing.Color;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sharpy_AIO.Plugins
{
    public class Annie
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private SpellSlot Flash = ObjectManager.Player.GetSpellSlot("summonerFlash");
        private GameObject Tibbers;
        private Spell Q, W, E, R;
        private bool HaveTibbers
        {
            get
            {
                return ObjectManager.Player.HasBuff("infernalguardintimer");
            }
        }
        private bool HaveWave
        {
            get
            {
                return ObjectManager.Player.HasBuff("pyromania_particle");
            }
        }

        public Annie()
        {
            Chat.Print("Sharpy AIO :: Annie Loaded :)");

            Q = new Spell(SpellSlot.Q, 625f, TargetSelector.DamageType.Magical);
            W = new Spell(SpellSlot.W, 625f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };

            Q.SetTargetted(.25f, float.MaxValue);
            W.SetSkillshot(0f, (float)(50f + Math.PI / 180f), float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0f, 290f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            // 메인메뉴
            Menu = new Menu("Sharpy AIO :: Annie", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CC", "Use R Min Hit").SetValue(new Slider(1,1,5)));
            Menu.AddSubMenu(combo);

            // 플래시 + 궁 메뉴
            var fr = new Menu("Flash + R", "Flash + R");
            fr.AddItem(new MenuItem("FR", "Flash + R").SetValue(new KeyBind('T', KeyBindType.Press)));
            fr.AddItem(new MenuItem("F0", "How to Flash + R?"));
            fr.AddItem(new MenuItem("F1", "1. Turn On TargetSelector -> Focus Selected Target"));
            fr.AddItem(new MenuItem("F2", "2. Left Click Target"));
            fr.AddItem(new MenuItem("F3", "3. Wait Target In the Flash + R Range"));
            fr.AddItem(new MenuItem("F4", "4. Press Flash + R Key (Default Key 'T')"));
            combo.AddSubMenu(fr);

            // 티버 메뉴
            var tibber = new Menu("Tibbers", "Tibbers");
            tibber.AddItem(new MenuItem("TA", "Auto Move Tibbers").SetValue(true));
            tibber.AddItem(new MenuItem("TE", "Use E When Tibbers Active").SetValue(true));
            Menu.AddSubMenu(tibber);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(harass);

            // 막타 메뉴
            var lasthit = new Menu("LastHit", "LastHit");
            lasthit.AddItem(new MenuItem("LHQ", "Use Q").SetValue(true));
            lasthit.AddItem(new MenuItem("LHB", "Block Spell When Have Stun").SetValue(true));
            lasthit.AddItem(new MenuItem("LHM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(lasthit);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LCW", "Use W").SetValue(true));
            laneclear.AddItem(new MenuItem("LCB", "Block Spell When Have Stun").SetValue(true));
            laneclear.AddItem(new MenuItem("LCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleClear");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCW", "Use W").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCM", "If Mana").SetValue(new Slider(60, 0, 100)));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MA", "Use AntiGapCloser").SetValue(true));
            misc.AddItem(new MenuItem("MI", "Use Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("MK", "Use Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("MS", "Use Sharpy Logic").SetValue(true));
            misc.AddItem(new MenuItem("ME", "Auto E Toggle").SetValue(new KeyBind('G',KeyBindType.Toggle)));
            Menu.AddSubMenu(misc);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DF", "Draw Flash + R Range").SetValue(new Circle(true, Color.Green)));
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

            Menu.Item("DIA").ValueChanged += Annie_ValueChanged;
            Menu.Item("DIF").ValueChanged += Annie_ValueChanged1;

            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Annie_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Annie_ValueChanged(object sender, OnValueChangeEventArgs e)
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
                    if (W.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, DW.Color, 3);
                    }
                }

                var DR = Menu.Item("DR").GetValue<Circle>();
                if (DR.Active)
                {
                    if (R.IsReadyPerfectly() && !HaveTibbers)
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, DR.Color, 3);
                    }
                }

                var DF = Menu.Item("DF").GetValue<Circle>();
                if (DF.Active)
                {
                    if (R.IsReadyPerfectly() && !HaveTibbers && Flash.IsReady())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range + 425f, DF.Color, 3);
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("MI").GetValue<bool>())
            {
                if (HaveWave)
                {
                    if (W.IsReadyPerfectly())
                    {
                        if (sender.IsValidTarget(W.Range))
                        {
                            W.Cast(sender);
                        }
                    }
                    else
                    {
                        if (Q.IsReadyPerfectly())
                        {
                            if (sender.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(sender);
                            }
                        }
                        else
                        {
                            if (R.IsReadyPerfectly() && !HaveTibbers)
                            {
                                R.Cast(sender);
                            }
                        }
                    }
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("MA").GetValue<bool>())
            {
                if (HaveWave)
                {
                    if (W.IsReadyPerfectly())
                    {
                        if (gapcloser.Sender.IsValidTarget(W.Range))
                        {
                            W.Cast(gapcloser.Sender);
                        }
                    }
                    else
                    {
                        if (Q.IsReadyPerfectly())
                        {
                            if (gapcloser.Sender.IsValidTarget(Q.Range))
                            {
                                Q.CastOnUnit(gapcloser.Sender);
                            }
                        }
                        else
                        {
                            if (R.IsReadyPerfectly() && !HaveTibbers)
                            {
                                if (gapcloser.Sender.IsValidTarget(R.Range))
                                {
                                    R.Cast(gapcloser.Sender);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsValid && sender.IsAlly && sender.Name == "Tibbers")
            {
                Tibbers = sender;
            }

            if (Tibbers.IsDead)
            {
                Tibbers = null;
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
                            if (Menu.Item("LHB").GetValue<bool>() && !HaveWave || !Menu.Item("LHB").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    args.Process = false;
                                }
                            }
                        }
                    }
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (Menu.Item("MS").GetValue<bool>())
                    {
                        if (Q.IsReadyPerfectly() || W.IsReadyPerfectly())
                        {
                            args.Process = false;
                        }
                    }
                }
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(20))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            if (Player.IsManaPercentOkay(Menu.Item("LHM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("LHB").GetValue<bool>() && !HaveWave || !Menu.Item("LHB").GetValue<bool>())
                                {
                                    if (Menu.Item("LHQ").GetValue<bool>())
                                    {
                                        if (Q.IsReadyPerfectly())
                                        {
                                            var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x, 1), Q.DamageType, Q.Range));
                                            if (target != null)
                                            {
                                                Q.CastOnUnit(target);
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
                                if (Menu.Item("HW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (HaveWave && Menu.Item("MS").GetValue<bool>())
                                        {
                                            var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                            if (target != null)
                                            {
                                                var otarget = HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)).ToList();
                                                if (otarget.Count >= 3)
                                                {
                                                    W.Cast(target, false, true);
                                                }
                                                else
                                                {
                                                    if (Q.IsReadyPerfectly())
                                                    {
                                                        Q.CastOnUnit(target);
                                                        W.Cast(target, false, true);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(620f, W.DamageType);
                                            if (target != null)
                                            {
                                                W.Cast(target, false, true);
                                            }
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
                                            Q.CastOnUnit(target);
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
                                if (Menu.Item("LCB").GetValue<bool>() && !HaveWave || !Menu.Item("LCB").GetValue<bool>())
                                {
                                    if (Menu.Item("LCW").GetValue<bool>())
                                    {
                                        if (W.IsReadyPerfectly())
                                        {
                                            var target = W.GetLineFarmLocation(MinionManager.GetMinions(W.Range));
                                            if (target.MinionsHit >= 1)
                                            {
                                                W.Cast(target.Position);
                                            }
                                        }
                                    }

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
                                }
                            }

                            if (Player.IsManaPercentOkay(Menu.Item("JCM").GetValue<Slider>().Value))
                            {
                                if (Menu.Item("JCW").GetValue<bool>())
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.IsValidTarget(W.Range) && W.GetPrediction(x).Hitchance >= W.MinHitChance);
                                        if (target != null)
                                        {
                                            W.Cast(target);
                                        }
                                    }
                                }

                                if (Menu.Item("JCQ").GetValue<bool>())
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                        if (target != null)
                                        {
                                            Q.CastOnUnit(target);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            if (R.IsReadyPerfectly() && !HaveTibbers)
                            {
                                var target = TargetSelector.GetTarget(R.Range, R.DamageType);
                                if (target != null)
                                {
                                    var otarget = HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range)).ToList();
                                    if (otarget.Count >= Menu.Item("CC").GetValue<Slider>().Value)
                                    {
                                        R.Cast(target, false, true);
                                    }
                                }
                            }

                            if (W.IsReadyPerfectly())
                            {
                                if (HaveWave && Menu.Item("MS").GetValue<bool>())
                                {
                                    var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                    if (target != null)
                                    {
                                        var otarget = HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)).ToList();
                                        if (otarget.Count >= 3)
                                        {
                                            W.Cast(target, false, true);
                                        }
                                        else
                                        {
                                            if (Q.IsReadyPerfectly())
                                            {
                                                Q.CastOnUnit(target);
                                                W.Cast(target, false, true);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(620f, W.DamageType);
                                    if (target != null)
                                    {
                                        W.Cast(target, false, true);
                                    }
                                }
                            }

                            if (Q.IsReadyPerfectly())
                            {
                                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                if (target != null)
                                {
                                    Q.CastOnUnit(target);
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.None:
                        break;
                }

                if (Menu.Item("FR").GetValue<KeyBind>().Active)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var target = TargetSelector.GetSelectedTarget();
                    if (target != null && Player.Position.Distance(target.Position) < 425f + R.Range)
                    {
                        if (!target.IsZombie)
                        {
                            if (Flash != SpellSlot.Unknown)
                            {
                                if (R.IsReadyPerfectly() && !HaveTibbers)
                                {
                                    if (Flash.IsReady())
                                    {
                                        Player.Spellbook.CastSpell(Flash, target.Position);
                                    }                                    

                                    R.CastOnUnit(target);
                                }
                            }
                        }
                    }
                }
            }

            if (Menu.Item("ME").GetValue<KeyBind>().Active)
            {
                if (!Player.HasBuff("Recall"))
                {
                    if (E.IsReadyPerfectly())
                    {
                        if (!HaveWave)
                        {
                            E.Cast();
                        }
                    }
                }
            }

            if (Menu.Item("TE").GetValue<bool>())
            {
                if (!Player.HasBuff("Recall"))
                {
                    if (Tibbers != null)
                    {
                        if (E.IsReadyPerfectly())
                        {
                            E.Cast();
                        }
                    }
                }
            }

            if (HaveTibbers)
            {
                if (Menu.Item("TA").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(3000f, TargetSelector.DamageType.Magical);
                    if (target != null)
                    {
                        R.CastOnUnit(target);
                        return;
                    }
                }
            }
            Killsteal();
        }

        private void Killsteal()
        {
            if (Menu.Item("MK").GetValue<bool>())
            {
                if (R.IsReadyPerfectly() && !HaveTibbers)
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(R.GetDamage(x), R.DamageType, R.Range));
                    if (target != null)
                    {
                        R.CastOnUnit(target);
                    }
                }
                else
                {
                    if (Q.IsReadyPerfectly())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range) && Q.GetDamage(x) > W.GetDamage(x));
                        if (target != null)
                        {
                            Q.CastOnUnit(target);
                        }
                        else
                        {
                            if (W.IsReadyPerfectly())
                            {
                                var otarget = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range) && Q.GetDamage(x) < W.GetDamage(x));
                                if (otarget != null)
                                {
                                    W.Cast(otarget);
                                }
                            }
                        }
                    }
                    else
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(W.GetDamage(x), W.DamageType, W.Range));
                        if (target != null)
                        {
                            W.Cast(target);
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

            if (R.IsReadyPerfectly() && !HaveTibbers)
            {
                damage += R.GetDamage(enemy);
            }

            return damage;
        }
    }
}
