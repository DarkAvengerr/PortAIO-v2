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
    public class Renekton
    {
        private Menu Menu;
        private Orbwalking.Orbwalker Orbwalker;
        private AIHeroClient Player = ObjectManager.Player;
        private Spell Q, W, E, R;
        private Vector3 blue = new Vector3(400f,427f,182f);
        private Vector3 red = new Vector3(14290f, 14409f, 171f);

        public Renekton()
        {

            Chat.Print("Sharpy AIO :: Renekton Loaded :)");

            Q = new Spell(SpellSlot.Q, 315f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };
            W = new Spell(SpellSlot.W, 275f, TargetSelector.DamageType.Physical);
            E = new Spell(SpellSlot.E, 550f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };
            R = new Spell(SpellSlot.R);

            E.SetSkillshot(.3f, 80f, 1850f, false, SkillshotType.SkillshotLine);

            // 메인 메뉴
            Menu = new Menu("Sharpy AIO :: Renekton", "mainmenu", true);

            // 오브워커 메뉴
            Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalker"));

            // 콤보 메뉴
            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("CH", "Use R HP").SetValue(new Slider(40, 0, 100)));
            Menu.AddSubMenu(combo);

            // 견제 메뉴
            var harass = new Menu("Harass", "Harass");
            harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            harass.AddItem(new MenuItem("HE1", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("HE3", "Use E2").SetValue(true));
            harass.AddItem(new MenuItem("HE2", "E2 Mode (On = Back, Off = Enemy)").SetValue(new KeyBind('Y',KeyBindType.Toggle)));
            Menu.AddSubMenu(harass);

            // 이동 메뉴
            var flee = new Menu("Flee", "Flee");
            flee.AddItem(new MenuItem("FE", "Use E").SetValue(true));
            Menu.AddSubMenu(flee);

            // 라인클리어 메뉴
            var laneclear = new Menu("LaneClear", "LaneClear");
            laneclear.AddItem(new MenuItem("LCQ", "Use Q").SetValue(true));
            Menu.AddSubMenu(laneclear);

            // 정글클리어 메뉴
            var jungleclear = new Menu("JungleClear", "JungleClear");
            jungleclear.AddItem(new MenuItem("JCQ", "Use Q").SetValue(true));
            Menu.AddSubMenu(jungleclear);

            // 기타 메뉴
            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("MK", "Use Q for Killsteal").SetValue(true));
            misc.AddItem(new MenuItem("MI", "Use W for Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("MA", "Use W for Antigapcloser").SetValue(true));
            misc.AddItem(new MenuItem("MH", "Auto Harass Q").SetValue(new KeyBind('G',KeyBindType.Toggle)));
            Menu.AddSubMenu(misc);

            // 드로잉 메뉴
            var drawing = new Menu("Drawing", "Drawing");
            drawing.AddItem(new MenuItem("DQ", "Draw Q Range").SetValue(new Circle(true, Color.Green)));
            drawing.AddItem(new MenuItem("DE", "Draw E Rage").SetValue(new Circle(true, Color.Green)));
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

            Menu.Item("DIA").ValueChanged += Renekton_ValueChanged;
            Menu.Item("DIF").ValueChanged += Renekton_ValueChanged1;

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Renekton_ValueChanged1(object sender, OnValueChangeEventArgs e)
        {
            //DamageIndicator.Fill = e.GetNewValue<Circle>().Active;
            //DamageIndicator.FillColor = e.GetNewValue<Circle>().Color;
        }

        private void Renekton_ValueChanged(object sender, OnValueChangeEventArgs e)
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

                var DE = Menu.Item("DE").GetValue<Circle>();
                if (DE.Active)
                {
                    if (E.IsReadyPerfectly())
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, DE.Color, 3);
                    }
                }
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("MI").GetValue<bool>())
            {
                if (W.IsReadyPerfectly())
                {
                    if (sender.IsValidTarget(W.Range))
                    {
                        W.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                    }
                }
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("MA").GetValue<bool>())
            {
                if (W.IsReadyPerfectly())
                {
                    if (gapcloser.Sender.IsValidTarget(W.Range))
                    {
                        W.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
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

        private void Game_OnUpdate(EventArgs args)
        {
            var starget = TargetSelector.GetSelectedTarget();

            if (Menu.Item("MH").GetValue<KeyBind>().Active)
            {
                if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Mixed)
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
            }

            if (Orbwalking.CanMove(20))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        {
                            if (Menu.Item("FE").GetValue<bool>())
                            {
                                if (E.IsReadyPerfectly())
                                {
                                    E.Cast(Game.CursorPos);
                                }
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LastHit:
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        {
                             if (Menu.Item("HE1").GetValue<bool>())
                             {
                                 if (E.IsReadyPerfectly() && !Player.HasBuff("renektonsliceanddicedelay"))
                                 {
                                     if (starget != null && Player.Position.Distance(starget.Position) <= E.Range + W.Range)
                                     {
                                         if (!starget.IsZombie)
                                         {
                                             var minions = MinionManager.GetMinions(starget.Position, Player.Position.Distance(starget.Position)).FirstOrDefault(x => x.IsValidTarget(E.Range));
                                             if (minions != null)
                                             {
                                                 if (minions.Position.Distance(starget.Position) <= W.Range)
                                                 {
                                                     E.Cast(minions);
                                                 }
                                                 else
                                                 {
                                                     if (starget.Position.Distance(Player.Position) <= E.Range)
                                                     {
                                                         E.Cast(starget);
                                                     }
                                                 }
                                             }
                                             else
                                             {
                                                 if (starget.Position.Distance(Player.Position) <= E.Range)
                                                 {
                                                     E.Cast(starget);
                                                 }
                                             }
                                         }
                                     }
                                     else
                                     {
                                         var target = TargetSelector.GetTarget(E.Range + W.Range, TargetSelector.DamageType.Physical);
                                         if (target != null)
                                         {
                                             var minions = MinionManager.GetMinions(target.Position, Player.Position.Distance(target.Position)).FirstOrDefault(x => x.IsValidTarget(E.Range));
                                             if (minions != null)
                                             {
                                                 if (minions.Position.Distance(target.Position) <= W.Range)
                                                 {
                                                     E.Cast(minions);
                                                 }
                                                 else
                                                 {
                                                     if (target.Position.Distance(Player.Position) <= E.Range)
                                                     {
                                                         E.Cast(target);
                                                     }
                                                 }
                                             }
                                             else
                                             {
                                                 if (target.Position.Distance(Player.Position) <= E.Range)
                                                 {
                                                     E.Cast(target);
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }

                            if (Menu.Item("HW").GetValue<bool>())
                            {
                                if (W.IsReadyPerfectly())
                                {
                                    if (starget != null && Player.Position.Distance(starget.Position) <= W.Range)
                                    {
                                        if (!starget.IsZombie)
                                        {
                                            W.Cast();
                                            castTitan();
                                            LeagueSharp.Common.Utility.DelayAction.Add(10, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, starget));
                                            LeagueSharp.Common.Utility.DelayAction.Add(20, () => castTiamat());
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null)
                                        {
                                            W.Cast();
                                            castTitan();
                                            LeagueSharp.Common.Utility.DelayAction.Add(10, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, starget));
                                            LeagueSharp.Common.Utility.DelayAction.Add(20, () => castTiamat());
                                        }
                                    }
                                }
                            }

                            if (Menu.Item("HQ").GetValue<bool>())
                            {
                                if (Q.IsReadyPerfectly())
                                {
                                    if (W.Instance.State == SpellState.Cooldown || W.Instance.State == SpellState.NotLearned)
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
                            }

                            if (Menu.Item("HE3").GetValue<bool>())
                            {
                                if (Menu.Item("HE2").GetValue<KeyBind>().Active)
                                {
                                    if (E.IsReadyPerfectly() && Player.HasBuff("renektonsliceanddicedelay"))
                                    {
                                        if (!Q.IsReadyPerfectly() && !W.IsReadyPerfectly())
                                        {
                                            var tower = ObjectManager.Get<Obj_Turret>().FirstOrDefault(x => x.IsAlly);
                                            if (tower.Position.Distance(blue) < tower.Position.Distance(red))
                                            {
                                                E.Cast(blue);
                                            }
                                            else
                                            {
                                                E.Cast(red);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (E.IsReadyPerfectly() && Player.HasBuff("renektonsliceanddicedelay"))
                                    {
                                        if (!Q.IsReadyPerfectly() && !W.IsReadyPerfectly())
                                        {
                                            if (starget != null && Player.Position.Distance(starget.Position) <= E.Range)
                                            {
                                                if (!starget.IsZombie)
                                                {
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
                            }
                        }
                        break;
                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Q.IsReadyPerfectly())
                            {
                                if (Menu.Item("LCQ").GetValue<bool>())
                                {
                                    var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                    if (target != null)
                                    {
                                        Q.Cast();
                                    }
                                }

                                if (Menu.Item("JCQ").GetValue<bool>())
                                {
                                    var target = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                    if (target != null)
                                    {
                                        Q.Cast();
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
                                            Q.Cast();
                                            castTiamat();
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                        if (target != null)
                                        {
                                            Q.Cast();
                                            castTiamat();
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
                                            castTitan();
                                            LeagueSharp.Common.Utility.DelayAction.Add(10, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, starget));
                                            LeagueSharp.Common.Utility.DelayAction.Add(20, () => castTiamat());
                                        }
                                    }
                                    else
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null)
                                        {
                                            W.Cast();
                                            castTitan();
                                            LeagueSharp.Common.Utility.DelayAction.Add(10, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target));
                                            LeagueSharp.Common.Utility.DelayAction.Add(20, () => castTiamat());
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

            if (Menu.Item("CR").GetValue<bool>())
            {
                if (Player.HealthPercent <= Menu.Item("CH").GetValue<Slider>().Value)
                {
                    if (!Player.HasBuff("Recall"))
                    {
                        if (R.IsReadyPerfectly())
                        {
                            R.Cast();
                        }
                    }
                }
            }

            Killsteal();
        }

        private void castTiamat()
        {
            var tiamat = ItemData.Tiamat_Melee_Only.GetItem();
            var hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();

            if (tiamat.IsReady() || hydra.IsReady())
            {
                tiamat.Cast();
                hydra.Cast();
            }
        }

        private void castTitan()
        {
            var titan = ItemData.Titanic_Hydra_Melee_Only.GetItem();

            if (titan.IsReady())
            {
                titan.Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private void Killsteal()
        {
            if (Menu.Item("MK").GetValue<bool>())
            {
                if (Q.IsReadyPerfectly())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsKillableAndValidTarget(Q.GetDamage(x), Q.DamageType, Q.Range));
                    if (target != null)
                    {
                        if (!target.IsDead)
                        {
                            Q.Cast();
                        }
                    }
                }
            }
        }

        private float getcombodamage(Obj_AI_Base enemy)
        {
            float damage = 0f;

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Tiamat);
            }

            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                damage += (float)Player.GetItemDamage(enemy, Damage.DamageItems.Hydra);
            }

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
