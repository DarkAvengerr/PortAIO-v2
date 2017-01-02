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
 namespace Herrari_488_GTB.Plugins
{
    public class Hecarim
    {
        private AIHeroClient Player = ObjectManager.Player;
        private Orbwalking.Orbwalker Orbwalker
        {
            get
            {
                return
                    MenuProvider.Orbwalker;
            }
        }
        private Spell Q, W, E, R;

        public Hecarim()
        {
            Chat.Print("<font color = \"#cc0000\">Herrari 488 GTB Loaded :)");

            Q = new Spell(SpellSlot.Q, 350f, TargetSelector.DamageType.Physical) { MinHitChance = HitChance.High };
            W = new Spell(SpellSlot.W, 525f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.Medium };
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 1000f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };

            E.SetTargetted(0f, float.MaxValue);
            R.SetSkillshot(0f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            MenuProvider.Champion.Combo.addUseQ();
            MenuProvider.Champion.Combo.addUseW();
            MenuProvider.Champion.Combo.addItem("Use E is Manually");
            MenuProvider.Champion.Combo.addItem("Use R is Manually :)");
            MenuProvider.Champion.Combo.addItem("Only Attack Selected Target", true);

            MenuProvider.Champion.Harass.addUseQ();
            MenuProvider.Champion.Harass.addUseW();
            MenuProvider.Champion.Harass.addIfMana(60);

            MenuProvider.Champion.Flee.addUseE();
            MenuProvider.Champion.Flee.addIfMana(60);

            MenuProvider.Champion.Lasthit.addUseQ();
            MenuProvider.Champion.Lasthit.addIfMana(60);

            MenuProvider.Champion.Laneclear.addUseQ();
            MenuProvider.Champion.Laneclear.addUseW();
            MenuProvider.Champion.Laneclear.addIfMana(60);

            MenuProvider.Champion.Jungleclear.addUseQ();
            MenuProvider.Champion.Jungleclear.addUseW();
            MenuProvider.Champion.Jungleclear.addIfMana(60);

            MenuProvider.Champion.Misc.addItem("AntiGapCloser Mode", new StringList(new[] { "E + AA", "R", "Never" }));
            MenuProvider.Champion.Misc.addItem("Interrupter Mode", new StringList(new[] { "E + AA", "R", "Never" }));

            MenuProvider.Champion.Drawings.addDrawQrange(Color.White, true);
            MenuProvider.Champion.Drawings.addDrawRrange(Color.IndianRed, true);
            


            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalking.CanMove(20))
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Flee:
                        {
                            var FE = MenuProvider.Champion.Flee.UseE;
                            if (FE)
                            {
                                if (Player.isManaPercentOkay(MenuProvider.Champion.Flee.IfMana))
                                {
                                    if (E.isReadyPerfectly())
                                    {
                                        E.Cast();
                                    }
                                }
                            }
                        }
                        break;

                    case Orbwalking.OrbwalkingMode.LastHit:
                        {
                            var LQ = MenuProvider.Champion.Lasthit.UseQ;
                            if (LQ)
                            {
                                if (Player.isManaPercentOkay(MenuProvider.Champion.Lasthit.IfMana))
                                {
                                    if (Q.isReadyPerfectly())
                                    {
                                        var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.isKillableAndValidTarget(Q.GetDamage(x, 1), Q.DamageType, Q.Range));
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
                            if (Player.isManaPercentOkay(MenuProvider.Champion.Harass.IfMana))
                            {
                                var HW = MenuProvider.Champion.Harass.UseW;
                                if (HW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null)
                                        {
                                            W.Cast();
                                        }
                                    }
                                }
                                var HQ = MenuProvider.Champion.Harass.UseQ;
                                if (HQ)
                                {
                                    if (Q.isReadyPerfectly())
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
                        break;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            if (Player.isManaPercentOkay(MenuProvider.Champion.Laneclear.IfMana))
                            {
                                var LQ = MenuProvider.Champion.Laneclear.UseQ;
                                if (LQ)
                                {
                                    if (Q.isReadyPerfectly())
                                    {
                                        var qLoc = Q.GetCircularFarmLocation(MinionManager.GetMinions(Q.Range));
                                        if (qLoc.MinionsHit >= 1)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }
                                var LW = MenuProvider.Champion.Laneclear.UseW;
                                if (LW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var wLoc = W.GetCircularFarmLocation(MinionManager.GetMinions(W.Range));
                                        if (wLoc.MinionsHit >= 1)
                                        {
                                            W.Cast();
                                        }
                                    }
                                }
                            }

                            if (Player.isManaPercentOkay(MenuProvider.Champion.Jungleclear.IfMana))
                            {
                                var JQ = MenuProvider.Champion.Jungleclear.UseQ;
                                if (JQ)
                                {
                                    if (Q.isReadyPerfectly())
                                    {
                                        var mob = MinionManager.GetMinions
                                            (Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(Q.Range));
                                        if (mob != null)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }

                                var JW = MenuProvider.Champion.Jungleclear.UseW;
                                if (JW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var mob = MinionManager.GetMinions
                                            (W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.IsValidTarget(W.Range));
                                        if (mob != null)
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
                            var CO = MenuProvider.Champion.Combo.getBoolValue("Only Attack Selected Target");
                            if (!CO)
                            {
                                var CQ = MenuProvider.Champion.Combo.UseQ;
                                if (CQ)
                                {
                                    if (Q.isReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                        if (target != null)
                                        {
                                            Q.Cast();
                                        }
                                    }
                                }

                                var CW = MenuProvider.Champion.Combo.UseW;
                                if (CW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null)
                                        {
                                            W.Cast();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var target = TargetSelector.GetSelectedTarget();
                                if (target != null)
                                {
                                    var CQ = MenuProvider.Champion.Combo.UseQ;
                                    if (CQ)
                                    {
                                        if (Q.isReadyPerfectly())
                                        {
                                            var qTarget = TargetSelector.GetTarget(Q.Range, Q.DamageType);
                                            if (qTarget != null)
                                            {
                                                Q.Cast();
                                            }
                                        }
                                    }

                                    var CW = MenuProvider.Champion.Combo.UseW;
                                    if (CW)
                                    {
                                        if (W.isReadyPerfectly())
                                        {
                                            var wTarget = TargetSelector.GetTarget(W.Range, W.DamageType);
                                            if (wTarget != null)
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
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = MenuProvider.Champion.Drawings.DrawQrange;
            if (Q.isReadyPerfectly() && drawQ.Active)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, drawQ.Color, 3);
            }

            var drawR = MenuProvider.Champion.Drawings.DrawRrange;
            if (R.isReadyPerfectly() && drawR.Active)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, drawR.Color, 3);
            }
        }

        private void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var anti = MenuProvider.Champion.Misc.getStringListValue("AntiGapCloser Mode").SelectedIndex;
            if (anti == 0)
            {
                if (E.isReadyPerfectly())
                {
                    if (gapcloser.Sender.IsValidTarget(325))
                    {
                        E.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, gapcloser.Sender);
                    }
                }
            }
            else
            {
                if (anti == 2)
                {
                    if (R.isReadyPerfectly())
                    {
                        if (gapcloser.Sender.IsValidTarget(R.Range))
                        {
                            R.CastOnUnit(gapcloser.Sender);
                        }
                    }
                }
                else return;
            }
        }

        private void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var inter = MenuProvider.Champion.Misc.getStringListValue("Interrupter Mode").SelectedIndex;
            if (inter == 0)
            {
                if (E.isReadyPerfectly())
                {
                    if (sender.IsValidTarget(325))
                    {
                        E.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, sender);
                    }
                }
            }
            else
            {
                if (inter == 1)
                {
                    if (R.isReadyPerfectly())
                    {
                        if (sender.IsValidTarget(R.Range))
                        {
                            R.CastOnUnit(sender);
                        }
                    }
                }
                else return;
            }
        }

        private void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Unit.IsMe)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                {
                    var LQ = MenuProvider.Champion.Lasthit.UseQ;
                    if (LQ)
                    {
                        if (Player.isManaPercentOkay(MenuProvider.Champion.Lasthit.IfMana))
                        {
                            if (Q.isReadyPerfectly())
                            {
                                args.Process = false;
                            }
                            else return;
                        }
                    }
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

        private float getcombodamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (Q.isReadyPerfectly())
            {
                damage += Q.GetDamage(enemy);
            }

            if (W.isReadyPerfectly())
            {
                damage += W.GetDamage(enemy);
            }

            if (E.isReadyPerfectly())
            {
                damage += E.GetDamage(enemy);
            }

            if (R.isReadyPerfectly())
            {
                damage += R.GetDamage(enemy);
            }

            return damage;
        }
    }
}
