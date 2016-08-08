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
    public class Zed
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private Spell Q, W, E, R;
        private SpellSlot Ignite = ObjectManager.Player.LSGetSpellSlot("summonerDot");
        private int LastSwitch;
        private Obj_AI_Minion shadow
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsVisible && x.IsAlly && x.Name == "Shadow" && !x.IsDead);
            }
        }

        private enum wCheck
        {
            First,
            Second,
            Cooltime
        }

        private enum rCheck
        {
            First,
            Second,
            Cooltime
        }

        private wCheck wReady
        {
            get
            {
                if (!W.IsReadyPerfectly())
                {
                    return
                        wCheck.Cooltime;
                }
                return
                    (Player.Spellbook.GetSpell(SpellSlot.W).Name == "ZedW" ? wCheck.First : wCheck.Second);
            }
        }

        private rCheck rReady
        {
            get
            {
                if (!R.IsReadyPerfectly())
                {
                    return
                        rCheck.Cooltime;
                }
                return
                    (Player.Spellbook.GetSpell(SpellSlot.R).Name == "ZedR" ? rCheck.First : rCheck.Second);
            }
        }

        public Zed()
        {
            Chat.Print("Sharpy AIO :: Zed Loaded :)");

            Q = new Spell(SpellSlot.Q, 900f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 290f,TargetSelector.DamageType.Physical);
            R = new Spell(SpellSlot.R, 625f, TargetSelector.DamageType.Physical);

            Q.SetSkillshot(.25f, 70f, 1750f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(.25f, 0f, 1750f, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0f, float.MaxValue);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Zed", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W (Line Combo Only)").SetValue(true));
            combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("CI", "Use Item").SetValue(true));
            combo.AddItem(new MenuItem("CM", "Combo Mode").SetValue(new KeyBind('L', KeyBindType.Toggle, true)));
            combo.AddItem(new MenuItem("C", "On : Normal / Off : Line"));
            Menu.AddSubMenu(combo);

            // 궁극기 메뉴
            var ult = new Menu("Ult Setting", "Ult Setting");
            ult.AddItem(new MenuItem("UR", "Cast R").SetValue(new KeyBind('T', KeyBindType.Press)));
            ult.AddItem(new MenuItem("UO", "Cast R Only Selected").SetValue(true));
            combo.AddSubMenu(ult);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW1", "Use W").SetValue(new KeyBind('Y', KeyBindType.Toggle)));
            harass.AddItem(new MenuItem("HW2", "Use W2").SetValue(false));
            harass.AddItem(new MenuItem("HE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HI", "Use Item").SetValue(true));
            harass.AddItem(new MenuItem("HA", "Auto Harass Long Poke").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            Menu.AddSubMenu(harass);

            // 이동 메뉴
            var flee = new Menu("Flee", "Flee");
            flee.AddItem(new MenuItem("FW", "Use W").SetValue(true));
            flee.AddItem(new MenuItem("FI", "Use Item").SetValue(true));
            Menu.AddSubMenu(flee);

            // 막타 메뉴
            var lasthit = new Menu("LastHit", "LastHit");
            lasthit.AddItem(new MenuItem("LHQ", "Use Q (Long)").SetValue(true));
            lasthit.AddItem(new MenuItem("LHE", "Use E (Short)").SetValue(true));
            Menu.AddSubMenu(lasthit);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            laneclear.AddItem(new MenuItem("LCE", "Use E").SetValue(true));
            laneclear.AddItem(new MenuItem("LCI", "Use Item").SetValue(true));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleClear");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCE", "Use E").SetValue(true));
            jungleclear.AddItem(new MenuItem("JCI", "Use Item").SetValue(true));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MK", "Use Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("ME", "Auto Shadow E").SetValue(true));
            Menu.AddSubMenu(misc);

            // 킬스틸 메뉴
            var killsteal = new Menu("Killsteal Setting","Killsteal Setting");
            killsteal.AddItem(new MenuItem("K0", "Use Only On Shadow"));
            killsteal.AddItem(new MenuItem("KQ", "Use Q").SetValue(true));
            killsteal.AddItem(new MenuItem("KE", "Use E").SetValue(true));
            killsteal.AddItem(new MenuItem("KI", "Use Ignite").SetValue(true));
            misc.AddSubMenu(killsteal);

            // 아이템 메뉴
            var item = new Menu("Item Setting", "Item Setting");
            item.AddItem(new MenuItem("IH", "Use Hydra").SetValue(true));
            item.AddItem(new MenuItem("IY", "Use Youmuu").SetValue(true));
            item.AddItem(new MenuItem("IB", "Use BOTRK").SetValue(true));
            misc.AddSubMenu(item);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DW", "Draw W Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DE", "Draw E Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DR", "Draw R Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DWQ", "Draw WQ Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DS", "Draw Combo Mode").SetValue(true));
            drawing.AddItem(new MenuItem("DO", "Disable All Drawings").SetValue(false));
            Menu.AddSubMenu(drawing);

            // 그림자 드로잉 메뉴
            var sd = new Menu("Shadow Drawing", "Shadow Drawing");
            sd.AddItem(new MenuItem("WQ", "Shadow Q Range").SetValue(new Circle(true, Color.WhiteSmoke)));
            sd.AddItem(new MenuItem("WE", "Shadow E Range").SetValue(new Circle(true, Color.WhiteSmoke)));
            drawing.AddSubMenu(sd);

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

            Menu.Item("DIA").ValueChanged += Zed_ValueChanged;
            Menu.Item("DIF").ValueChanged += Zed_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Zed_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Zed_ValueChanged(object sender, OnValueChangeEventArgs e)
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
                        if (R.IsReadyPerfectly())
                        {
                            Render.Circle.DrawCircle(Player.Position, R.Range, DR.Color, 3);
                        }
                    }
                }

                var DWQ = Menu.Item("DWQ").GetValue<Circle>();
                if (DWQ.Active)
                {
                    if (Q.IsReadyPerfectly() && W.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range + W.Range, DWQ.Color, 3);
                    }
                }

                var WQ = Menu.Item("WQ").GetValue<Circle>();
                if (WQ.Active)
                {
                    if (shadow != null)
                    {
                        if (Q.IsReadyPerfectly())
                        {
                            Render.Circle.DrawCircle(shadow.Position, Q.Range, WQ.Color, 3);
                        }
                    }
                }

                var WE = Menu.Item("WE").GetValue<Circle>();
                if (WE.Active)
                {
                    if (shadow != null)
                    {
                        if (E.IsReadyPerfectly())
                        {
                            Render.Circle.DrawCircle(shadow.Position, E.Range, WE.Color, 3);
                        }
                    }
                }

                if (Menu.Item("DS").GetValue<bool>())
                {
                    var position = Drawing.WorldToScreen(ObjectManager.Player.Position);
                    if (Menu.Item("CM").GetValue<KeyBind>().Active)
                    {
                        Drawing.DrawText(position.X, position.Y + 40, Color.White, "Combo Mode : Normal");
                    }
                    else
                    {
                        Drawing.DrawText(position.X, position.Y + 40, Color.White, "Combo Mode : Line");
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
                    if (Player.Mana >= E.ManaCost)
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
            }
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(20))
            {
                if (Menu.Item("UR").GetValue<KeyBind>().Active)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    if (R.IsReadyPerfectly() && rReady == rCheck.First)
                    {
                        if (Menu.Item("CI").GetValue<bool>())
                        {
                            var target = TargetSelector.GetTarget(1300f, TargetSelector.DamageType.Physical);
                            if (target != null)
                            {
                                castYoumuu();
                            }
                        }

                        if (Menu.Item("UO").GetValue<bool>())
                        {
                            var target = TargetSelector.GetSelectedTarget();
                            if (Player.Position.LSDistance(target.Position) <= R.Range && !target.IsZombie)
                            {
                                R.CastOnUnit(target);
                            }
                        }
                        else
                        {
                            var target = TargetSelector.GetTarget(R.Range, R.DamageType);
                            if (target != null)
                            {
                                R.CastOnUnit(target);
                            }
                        }
                    }
                }

                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        {
                            if (Menu.Item("FI").GetValue<bool>())
                            {
                                castYoumuu();
                            }

                            if (Menu.Item("FW").GetValue<bool>())
                            {
                                if (Player.Mana >= W.ManaCost)
                                {
                                    if (W.IsReadyPerfectly())
                                    {
                                        if (wReady == wCheck.First)
                                        {
                                            W.Cast(Game.CursorPos);
                                        }
                                    }
                                }

                                if (wReady == wCheck.Second)
                                {
                                    W.Cast();
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            if (Menu.Item("LHQ").GetValue<bool>())
                            {
                                if (Player.Mana >= Q.ManaCost)
                                {
                                    if (Q.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                                        if (target != null)
                                        {
                                            if (Player.Position.LSDistance(target.Position) > E.Range)
                                            {
                                                Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                Q.Cast(target);
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("LHE").GetValue<bool>())
                            {
                                if (Player.Mana >= E.ManaCost)
                                {
                                    if (E.IsReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x), E.DamageType, E.Range));
                                        if (target != null)
                                        {
                                            E.Cast();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        {
                            var starget = TargetSelector.GetSelectedTarget();
                            var WEQMana = W.ManaCost + E.ManaCost + Q.ManaCost;
                            if (Menu.Item("HW1").GetValue<KeyBind>().Active)
                            {
                                if (shadow == null)
                                {
                                    if (W.IsReadyPerfectly() && wReady == wCheck.First)
                                    {
                                        if (Player.Mana >= WEQMana)
                                        {
                                            if (starget != null && starget.LSIsValidTarget(W.Range) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    W.Cast(starget);
                                                }
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                                                if (target != null)
                                                {
                                                    W.Cast(target);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("HE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    if (shadow != null)
                                    {
                                        if (starget != null && starget.LSIsValidTarget(E.Range) && !starget.IsDead || starget != null && starget.LSIsValidTarget(E.Range,true,shadow.Position) && !starget.IsDead)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                E.Cast();
                                            }
                                        }
                                        else
                                        {
                                            var shadowtarget = TargetSelector.GetTarget(E.Range, E.DamageType, true, null, shadow.Position);
                                            if (shadowtarget != null)
                                            {
                                                E.Cast();
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
                                    else
                                    {
                                        if (!W.IsReadyPerfectly() || Player.Mana < WEQMana || !Menu.Item("HW1").GetValue<KeyBind>().Active)
                                        {
                                            if (starget != null && starget.LSIsValidTarget(E.Range) && !starget.IsDead)
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
                            }

                            if (Menu.Item("HQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (shadow != null)
                                    {
                                        if (starget != null && starget.LSIsValidTarget(Q.Range) && !starget.IsDead || starget != null && starget.LSIsValidTarget(Q.Range, true, shadow.Position) && !starget.IsDead)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                if (starget.LSIsValidTarget(Q.Range, true, shadow.Position)) 
                                                {
                                                    Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                                    Q.Cast(starget);
                                                }
                                                else
                                                {
                                                    if (starget.LSIsValidTarget(Q.Range))
                                                    {
                                                        Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                        Q.Cast(starget);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var shadowtarget = TargetSelector.GetTarget(Q.Range, Q.DamageType, true, null, shadow.Position);
                                            if (shadowtarget != null)
                                            {
                                                Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                                Q.Cast(shadowtarget);
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                                if (target != null)
                                                {
                                                    Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                    Q.Cast(target);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!W.IsReadyPerfectly() || Player.Mana < WEQMana || !Menu.Item("HW1").GetValue<KeyBind>().Active)
                                        {
                                            if (starget != null && starget.LSIsValidTarget(Q.Range) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                    Q.Cast(starget);
                                                }
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                                if (target != null)
                                                {
                                                    Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                    Q.Cast(target);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("HW2").GetValue<bool>())
                            {
                                if (Player.LSHasBuff("zedwhandler"))
                                {
                                    if (W.IsReadyPerfectly() && wReady == wCheck.Second)
                                    {
                                        if (starget != null && starget.LSIsValidTarget(Player.AttackRange, true, shadow.Position) && !starget.IsDead)
                                        {
                                            if (!starget.IsZombie)
                                            {
                                                W.Cast();
                                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, starget);
                                            }
                                        }
                                        else
                                        {
                                            var target = TargetSelector.GetTarget(Player.AttackRange, TargetSelector.DamageType.Physical, true, null, shadow.Position);
                                            if (target != null)
                                            {
                                                W.Cast();
                                                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                                            }
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("HI").GetValue<bool>())
                            {
                                if (starget != null && starget.LSIsValidTarget(550f) && !starget.IsDead)
                                {
                                    if (!starget.IsZombie)
                                    {
                                        castBOTRK(starget);
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(550f, TargetSelector.DamageType.Physical);
                                    if (target != null)
                                    {
                                        castBOTRK(target);
                                    }
                                }

                                if (starget != null && starget.LSIsValidTarget(350f) && !starget.IsDead)
                                {
                                    if (!starget.IsZombie)
                                    {
                                        castHydra();
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(350f, TargetSelector.DamageType.Physical);
                                    if (target != null)
                                    {
                                        castHydra();
                                    }
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Player.Mana >= Q.ManaCost)
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (Menu.Item("LCQ").GetValue<bool>())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.LSIsValidTarget(Q.Range));
                                        if (target != null)
                                        {
                                            Q.UpdateSourcePosition(Player.Position, Player.Position);
                                            Q.Cast(target);
                                        }
                                    }

                                    if (Menu.Item("JCQ").GetValue<bool>())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.LSIsValidTarget(Q.Range));
                                        if (target != null)
                                        {
                                            Q.UpdateSourcePosition(Player.Position, Player.Position);
                                            Q.Cast(target);
                                        }
                                    }
                                }
                            }

                            if (Player.Mana >= E.ManaCost)
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    if (Menu.Item("LCE").GetValue<bool>())
                                    {
                                        var target = MinionManager.GetMinions(E.Range).FirstOrDefault(x => x.LSIsValidTarget(E.Range));
                                        if (target != null)
                                        {
                                            E.Cast();
                                        }
                                    }

                                    if (Menu.Item("JCE").GetValue<bool>())
                                    {
                                        var target = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                            .FirstOrDefault(x => x.LSIsValidTarget(E.Range));
                                        if (target != null)
                                        {
                                            E.Cast();
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("LCI").GetValue<bool>())
                            {
                                var target = MinionManager.GetMinions(400f).FirstOrDefault(x => x.LSIsValidTarget(400f));
                                if (target != null)
                                {
                                    castHydra();
                                }
                            }

                            if (Menu.Item("JCI").GetValue<bool>())
                            {
                                var target = MinionManager.GetMinions(400f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                    .FirstOrDefault(x => x.LSIsValidTarget(400f));
                                if (target != null)
                                {
                                    castHydra();
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.Combo:
                        {
                            var starget = TargetSelector.GetSelectedTarget();
                            if (Menu.Item("CI").GetValue<bool>())
                            {
                                if (starget != null && starget.LSIsValidTarget(1500f) && !starget.IsDead)
                                {
                                    if (!starget.IsZombie)
                                    {
                                        castYoumuu();
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(1500f, TargetSelector.DamageType.Physical);
                                    if (target != null)
                                    {
                                        castYoumuu();
                                    }
                                }

                                if (starget != null && starget.LSIsValidTarget(550f) && !starget.IsDead)
                                {
                                    if (!starget.IsZombie)
                                    {
                                        castBOTRK(starget);
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(550f, TargetSelector.DamageType.Physical);
                                    if (target != null)
                                    {
                                        castBOTRK(target);
                                    }
                                }
                            }

                            if (Menu.Item("CM").GetValue<KeyBind>().Active)
                            {
                                if (shadow != null)
                                {
                                    if (Menu.Item("CE").GetValue<bool>())
                                    {
                                        if (E.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(E.Range) && !starget.IsDead || starget != null && starget.LSIsValidTarget(E.Range, true, shadow.Position) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    E.Cast();
                                                }
                                            }
                                            else
                                            {
                                                var shadowtarget = TargetSelector.GetTarget(E.Range, E.DamageType, true, null, shadow.Position);
                                                if (shadowtarget != null)
                                                {
                                                    E.Cast();
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

                                    if (Menu.Item("CQ").GetValue<bool>())
                                    {
                                        if (Q.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(Q.Range) && !starget.IsDead || starget != null && starget.LSIsValidTarget(Q.Range, true, shadow.Position))
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    if (starget.LSIsValidTarget(Q.Range, true, shadow.Position))
                                                    {
                                                        Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                                        Q.Cast(starget);
                                                    }
                                                    else
                                                    {
                                                        if (starget.LSIsValidTarget(Q.Range))
                                                        {
                                                            Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                            Q.Cast(starget);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var shadowtarget = TargetSelector.GetTarget(Q.Range, Q.DamageType, true, null, shadow.Position);
                                                if (shadowtarget != null)
                                                {
                                                    Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                                    Q.Cast(shadowtarget);
                                                }
                                                else
                                                {
                                                    var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                                    if (target != null)
                                                    {
                                                        Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                        Q.Cast(target);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (Menu.Item("CQ").GetValue<bool>())
                                    {
                                        if (Q.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(Q.Range) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                    Q.Cast(starget);
                                                }
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                                if (target != null)
                                                {
                                                    Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                    Q.Cast(target);
                                                }
                                            }
                                        }
                                    }

                                    if (Menu.Item("CE").GetValue<bool>())
                                    {
                                        if (E.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(E.Range) && !starget.IsDead)
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

                                if (Menu.Item("CI").GetValue<bool>())
                                {
                                    if (starget != null && starget.LSIsValidTarget(350f) && !starget.IsDead)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            castHydra();
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(350f, TargetSelector.DamageType.Physical);
                                        if (target != null)
                                        {
                                            castHydra();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (rReady == rCheck.Second)
                                {
                                    if (wReady == wCheck.First)
                                    {
                                        if (E.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(E.Range) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    E.Cast();
                                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, starget);
                                                }
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(E.Range, E.DamageType);
                                                if (target != null)
                                                {
                                                    E.Cast();
                                                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                                                }
                                            }
                                        }

                                        if (W.IsReadyPerfectly())
                                        {
                                            if (starget != null && starget.LSIsValidTarget(W.Range) && !starget.IsDead)
                                            {
                                                if (!starget.IsZombie)
                                                {
                                                    var spos = starget.Position.LSExtend(Player.Position, -650f);
                                                    W.Cast(spos);
                                                }
                                            }
                                            else
                                            {
                                                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                                                if (target != null)
                                                {
                                                    var wpos = target.Position.LSExtend(Player.Position, -650f);
                                                    W.Cast(wpos);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (wReady == wCheck.Second)
                                        {
                                            if (Q.IsReadyPerfectly())
                                            {
                                                if (starget != null && starget.LSIsValidTarget(Q.Range) && !starget.IsDead)
                                                {
                                                    if (!starget.IsZombie)
                                                    {
                                                        Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                        Q.Cast(starget);
                                                    }
                                                }
                                                else
                                                {
                                                    var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                                    if (target != null)
                                                    {
                                                        Q.UpdateSourcePosition(Player.Position, Player.Position);
                                                        Q.Cast(target);
                                                    }
                                                }
                                            }

                                            if (Menu.Item("CI").GetValue<bool>())
                                            {
                                                if (starget != null && starget.LSIsValidTarget(350f) && !starget.IsDead)
                                                {
                                                    if (!starget.IsZombie)
                                                    {
                                                        castHydra();
                                                    }
                                                }
                                                else
                                                {
                                                    var target = TargetSelector.GetTarget(350f, TargetSelector.DamageType.Physical);
                                                    if (target != null)
                                                    {
                                                        castHydra();
                                                    }
                                                }
                                            }

                                            if (!Q.IsReadyPerfectly())
                                            {
                                                if (Utils.GameTimeTickCount - LastSwitch >= 350)
                                                {
                                                    Menu.Item("CM").SetValue(new KeyBind(Menu.Item("CM").GetValue<KeyBind>().Key, KeyBindType.Toggle, true));
                                                    LastSwitch = Utils.GameTimeTickCount;
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

                if (Menu.Item("HA").GetValue<KeyBind>().Active)
                {
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                    {
                        var starget = TargetSelector.GetSelectedTarget();
                        if (W.IsReadyPerfectly() && wReady == wCheck.First)
                        {
                            if (Player.Mana >= Q.ManaCost + W.ManaCost)
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (starget != null && starget.LSIsValidTarget(Q.Range + W.Range) && !starget.IsDead)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            if (Player.Position.LSDistance(starget.Position) > W.Range)
                                            {
                                                var spos = starget.Position.LSExtend(Player.Position, -(starget.Position.LSDistance(Player.Position) + W.Range));
                                                W.Cast(spos);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range + W.Range, TargetSelector.DamageType.Physical);
                                        if (target != null)
                                        {
                                            if (Player.Position.LSDistance(target.Position) > W.Range)
                                            {
                                                var wpos = target.Position.LSExtend(Player.Position, -(target.Position.LSDistance(Player.Position) + W.Range));
                                                W.Cast(wpos);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Q.IsReadyPerfectly())
                        {
                            if (shadow != null)
                            {
                                if (starget != null && starget.LSIsValidTarget(Q.Range, true, shadow.Position) && !starget.IsDead)
                                {
                                    if (!starget.IsZombie)
                                    {
                                        Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                        Q.Cast(starget);
                                    }
                                }
                                else
                                {
                                    var target = TargetSelector.GetTarget(Q.Range, Q.DamageType, true, null, shadow.Position);
                                    if (target != null)
                                    {
                                        Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                        Q.Cast(target);
                                    }
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("ME").GetValue<bool>())
                {
                    if (E.IsReadyPerfectly())
                    {
                        if (shadow != null)
                        {
                            var starget = TargetSelector.GetSelectedTarget();
                            if (starget != null && starget.LSIsValidTarget(E.Range, true, shadow.Position))
                            {
                                if (!starget.IsZombie)
                                {
                                    E.Cast();
                                }
                            }
                            else
                            {
                                var target = TargetSelector.GetTarget(E.Range, E.DamageType, true, null, shadow.Position);
                                if (target != null)
                                {
                                    E.Cast();
                                }
                            }
                        }
                    }
                }
            }
            Killsteal();
        }

        private void Killsteal()
        {
            if (Menu.Item("MK").GetValue<bool>())
            {
                if (shadow != null)
                {
                    if (Menu.Item("KQ").GetValue<bool>())
                    {
                        if (Q.IsReadyPerfectly())
                        {
                            var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range + Player.Position.LSDistance(shadow.Position)));
                            if (target != null)
                            {
                                if (!target.IsDead)
                                {
                                    if (shadow.Position.LSDistance(target.Position) <= Q.Range)
                                    {
                                        Q.UpdateSourcePosition(shadow.Position, shadow.Position);
                                        Q.Cast(target);
                                    }
                                }
                            }
                        }
                    }

                    if (Menu.Item("KE").GetValue<bool>())
                    {
                        if (E.IsReadyPerfectly())
                        {
                            var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(E.GetDamage(x), E.DamageType, Player.Position.LSDistance(shadow.Position) + E.Range));
                            if (target != null)
                            {
                                if (!target.IsDead)
                                {
                                    if (shadow.Position.LSDistance(target.Position) <= E.Range)
                                    {
                                        E.Cast();
                                    }
                                }
                            }
                        }
                    }
                }

                if (Menu.Item("KI").GetValue<bool>())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Player.GetSummonerSpellDamage(x, Damage.SummonerSpell.Ignite), TargetSelector.DamageType.True, 600f));
                    if (target != null)
                    {
                        if (!target.IsZombie)
                        {
                            if (Ignite != SpellSlot.Unknown)
                            {
                                if (Ignite.LSIsReady())
                                {
                                    Player.Spellbook.CastSpell(Ignite, target.Position);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void castBOTRK(AIHeroClient target)
        {
            if (Menu.Item("IB").GetValue<bool>())
            {
                var bilge = ItemData.Bilgewater_Cutlass.GetItem();
                var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();

                if (bilge.IsReady() || botrk.IsReady())
                {
                    bilge.Cast(target);
                    botrk.Cast(target);
                }
            }
        }

        private void castHydra()
        {
            if (Menu.Item("IH").GetValue<bool>())
            {
                var tiamet = ItemData.Tiamat_Melee_Only.GetItem();
                var hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();

                if (tiamet.IsReady() || hydra.IsReady())
                {
                    tiamet.Cast();
                    hydra.Cast();
                }
            }
        }

        private void castYoumuu()
        {
            if (Menu.Item("IY").GetValue<bool>())
            {
                var yomu = ItemData.Youmuus_Ghostblade.GetItem();
                if (yomu.IsReady())
                {
                    yomu.Cast();
                }
            }
        }

        private float getcombodamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (ItemData.Bilgewater_Cutlass.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Bilgewater);
            }

            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Botrk);
            }

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            }

            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            }

            if (!Player.Spellbook.IsAutoAttacking)
            {
                damage += (float)Player.LSGetAutoAttackDamage(enemy);
            }

            if (Q.IsReadyPerfectly())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.IsReadyPerfectly())
            {
                damage += Q.GetDamage(enemy) / 2;
            }

            if (E.IsReadyPerfectly())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.IsReadyPerfectly())
            {
                damage += R.GetDamage(enemy);
                damage += (float)(R.Level * .15 + .05);
            }
            return damage;
        }
    }
}
