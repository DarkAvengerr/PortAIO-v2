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
 namespace Mundo_Sharpy.Plugins
{
    public class DrMundo
    {
        private AIHeroClient Player = ObjectManager.Player;
        private Spell Q, W, E, R;
        private Orbwalking.Orbwalker Orbwalker {  get { return MenuProvider.Orbwalker; } }

        public DrMundo()
        {
            Chat.Print("<font Color = \"#ffffff\">Mundo Sharpy Loaded");

            Q = new Spell(SpellSlot.Q, 1000f, TargetSelector.DamageType.Magical) { MinHitChance = HitChance.High };
            W = new Spell(SpellSlot.W, 162.5f, TargetSelector.DamageType.Magical);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(.5f, 60f, 1850, true, SkillshotType.SkillshotLine);

            MenuProvider.Champion.Combo.addUseQ();
            MenuProvider.Champion.Combo.addUseW();
            MenuProvider.Champion.Combo.addItem("Use W If HP", new Slider(10, 0, 100));
            MenuProvider.Champion.Combo.addUseE();
            MenuProvider.Champion.Combo.addUseR();
            MenuProvider.Champion.Combo.addItem("Use R HP Percent", new Slider(60, 0, 100));

            MenuProvider.Champion.Harass.addUseQ();
            MenuProvider.Champion.Harass.addUseW();
            MenuProvider.Champion.Harass.addItem("Use W If HP", new Slider(10, 0, 100));
            MenuProvider.Champion.Harass.addUseE();

            MenuProvider.Champion.Lasthit.addUseQ();

            MenuProvider.Champion.Laneclear.addUseQ();
            MenuProvider.Champion.Laneclear.addItem("Use E (Jungle too)", true);

            MenuProvider.Champion.Jungleclear.addUseQ();

            MenuProvider.Champion.Misc.addUseKillsteal();

            MenuProvider.Champion.Drawings.addDrawQrange(Color.Green, true);
            MenuProvider.Champion.Drawings.addDrawWrange(Color.White, true);

            Game.OnUpdate += Game_OnUpdate;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        private void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (unit.IsMe)
            {
                if (E.isReadyPerfectly())
                {
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        if (MenuProvider.Champion.Combo.UseE)
                        {
                            if (target is AIHeroClient)
                            {
                                E.Cast();
                            }
                        }
                    }
                    else
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                    {
                        if (MenuProvider.Champion.Harass.UseE)
                        {
                            if (target is AIHeroClient)
                            {
                                E.Cast();
                            }
                        }
                    }
                    else
                    if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                    {
                        if (MenuProvider.Champion.Laneclear.getBoolValue("Use E (Jungle too)"))
                        {
                            if (target is Obj_AI_Minion)
                            {
                                E.Cast();
                            }
                        }
                    }
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
            var drawW = MenuProvider.Champion.Drawings.DrawWrange;
            if (drawW.Active)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, drawW.Color, 3);
            }
        }

        private void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                Orbwalking.ResetAutoAttackTimer();
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
                            var LQ = MenuProvider.Champion.Lasthit.UseQ;
                            if (LQ)
                            {
                                if (Q.isReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(Q.Range).FirstOrDefault(x => x.isKillableAndValidTarget(Q.GetDamage(x, 1), Q.DamageType, Q.Range));
                                    if (target != null)
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                        }
                        break;

                    case Orbwalking.OrbwalkingMode.Mixed:
                        {
                            var HQ = MenuProvider.Champion.Harass.UseQ;
                            if (HQ)
                            {
                                if (Q.isReadyPerfectly())
                                {
                                    var target = TargetSelector.GetTargetNoCollision(Q);
                                    if (target.IsValidTarget(Q.Range))
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                            var HP = MenuProvider.Champion.Harass.getSliderValue("Use W If HP").Value;
                            if (Player.HealthPercent >= HP)
                            {
                                var HW = MenuProvider.Champion.Harass.UseW;
                                if (HW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null && W.Instance.ToggleState == 1)
                                        {
                                            W.Cast();
                                        }
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
                        break;

                    case Orbwalking.OrbwalkingMode.LaneClear:
                        {
                            var LQ = MenuProvider.Champion.Laneclear.UseQ;
                            if (LQ)
                            {
                                if (Q.isReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(Q.Range).OrderBy(x => x.Health).FirstOrDefault
                                        (x => x.isKillableAndValidTarget(Q.GetDamage(x), TargetSelector.DamageType.Magical, Q.Range) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
                                    if (target != null)
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                            var JQ = MenuProvider.Champion.Jungleclear.UseQ;
                            if (JQ)
                            {
                                if (Q.isReadyPerfectly())
                                {
                                    var target = MinionManager.GetMinions(600, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                                        .FirstOrDefault(x => x.IsValidTarget(600) && Q.GetPrediction(x).Hitchance >= Q.MinHitChance);
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
                            var CQ = MenuProvider.Champion.Combo.UseQ;
                            if (CQ)
                            {
                                if (Q.isReadyPerfectly())
                                {
                                    var target = TargetSelector.GetTargetNoCollision(Q);
                                    if (target.IsValidTarget(Q.Range))
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                            var HP = MenuProvider.Champion.Combo.getSliderValue("Use W If HP").Value;
                            if (Player.HealthPercent >= HP)
                            {
                                var CW = MenuProvider.Champion.Combo.UseW;
                                if (CW)
                                {
                                    if (W.isReadyPerfectly())
                                    {
                                        var target = TargetSelector.GetTarget(W.Range, W.DamageType);
                                        if (target != null && W.Instance.ToggleState == 1)
                                        {
                                            W.Cast();
                                        }
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
                        break;

                    case Orbwalking.OrbwalkingMode.None:
                        break;
                }
            }
            var CR = MenuProvider.Champion.Combo.UseR;
            if (CR)
            {
                var RHP = MenuProvider.Champion.Combo.getSliderValue("Use R HP Percent").Value;
                if (Player.HealthPercent <= RHP)
                {
                    R.Cast();
                }
            }
            KillSteal();
        }

        private void KillSteal()
        {
            var KSON = MenuProvider.Champion.Misc.UseKillsteal;
            if (KSON)
            {
                if (Q.isReadyPerfectly() && E.isReadyPerfectly())
                {
                    var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsValidTarget(Q.Range)
                        && x.Health < Q.GetDamage(x) + E.GetDamage(x));
                    if (target.IsValidTarget(Q.Range))
                    {
                        E.Cast();
                        Q.Cast(target);
                    }
                }
                else
                {
                    if (Q.isReadyPerfectly())
                    {
                        var target = ObjectManager.Get<AIHeroClient>().FirstOrDefault(x => x.IsValidTarget(Q.Range)
                            && x.Health < Q.GetDamage(x));
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
        }
    }
}
