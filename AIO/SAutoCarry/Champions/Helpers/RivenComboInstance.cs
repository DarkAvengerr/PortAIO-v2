using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SAutoCarry.Champions;
using SharpDX;
using EloBuddy;

namespace SAutoCarry.Champions.Helpers
{
    internal class ComboInstance
    {
        private static Riven s_Champion;
        private static int s_lastGapCloseTick;

        public static Action<AIHeroClient>[] MethodsOnUpdate = new Action<AIHeroClient>[3];
        public static Action<AIHeroClient, string>[] MethodsOnAnimation = new Action<AIHeroClient, string>[3];
        public static Action<AIHeroClient>[] GapCloseMethods = new Action<AIHeroClient>[3];
        
        private const int Q = 0, W = 1, E = 2, R = 3, Q2 = 4, W2 = 5, E2 = 6, R2 = 7;

        public static void Initialize(Riven Me)
        {
            s_Champion = Me;
            #region Gapclosers
            GapCloseMethods[0] = new Action<AIHeroClient>((t) =>
            {
                if (t.LSDistance(ObjectManager.Player.ServerPosition) > Me.ConfigMenu.Item("MMINDIST").GetValue<Slider>().Value)
                {
                    if (Utils.TickCount - s_lastGapCloseTick < 150)
                        return;

                    if (Me.Spells[E].LSIsReady())
                    {
                        int eMode = 3;
                        if (Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
                            eMode = Me.ConfigMenu.Item("CEMODE").GetValue<StringList>().SelectedIndex;
                        else if (Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
                            eMode = Me.ConfigMenu.Item("HEMODE").GetValue<StringList>().SelectedIndex;
                        int comboMode = Me.ConfigMenu.Item(String.Format("CMETHOD{0}", t.ChampionName)).GetValue<StringList>().SelectedIndex;
                        if (eMode == 0)
                        {
                            Me.Spells[E].Cast(t.ServerPosition);
                            if(comboMode == 0)
                            {
                                if (Me.CheckR1(t))
                                {
                                    Me.Spells[R].Cast();
                                    return;
                                }
                            }
                            else if (comboMode == 1)
                            {
                                if (!ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                    Me.Spells[R].Cast();
                            }
                            s_lastGapCloseTick = Utils.TickCount;
                        }
                        else if (eMode == 1)
                        {
                            Me.Spells[E].Cast(Game.CursorPos);
                            if (comboMode == 0)
                            {
                                if (Me.CheckR1(t))
                                {
                                    Me.Spells[R].Cast();
                                    return;
                                }
                            }
                            else if (comboMode == 1)
                            {
                                if (!ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                    Me.Spells[R].Cast();
                            }
                            s_lastGapCloseTick = Utils.TickCount;
                        }
                    }
                }
            });

            GapCloseMethods[1] = new Action<AIHeroClient>((t) =>
            {
                if (t.LSDistance(ObjectManager.Player.ServerPosition) > Me.ConfigMenu.Item("MMINDIST").GetValue<Slider>().Value)
                {
                    if (Utils.TickCount - s_lastGapCloseTick < 150)
                        return;

                    if (!Me.Spells[E].LSIsReady())
                    {
                        if (Me.Spells[Q].LSIsReady())
                        {
                            Me.Spells[Q].Cast(t.ServerPosition, true);
                            s_lastGapCloseTick = Utils.TickCount;
                            return;
                        }
                    }
                }

                if (t.LSDistance(ObjectManager.Player.ServerPosition) > ObjectManager.Player.AttackRange + 50)
                {
                    if (!Me.ConfigMenu.Item("MKEEPQ").GetValue<bool>() && Animation.QStacks != 0 && Utils.TickCount - Animation.LastQTick >= 3500)
                        Me.Spells[Q].Cast(t.ServerPosition, true);
                }
            });

            GapCloseMethods[2] = new Action<AIHeroClient>((t) =>
            {
                if (Utils.TickCount - s_lastGapCloseTick < 150)
                    return;

                s_lastGapCloseTick = Utils.TickCount;
                if (Target.IsTargetFlashed() && Me.ConfigMenu.Item("CUSEF").GetValue<KeyBind>().Active)
                {
                    if (t.LSDistance(ObjectManager.Player.ServerPosition) > 300 && t.LSDistance(ObjectManager.Player.ServerPosition) < 500 && Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
                    {
                        int steps = (int)(t.LSDistance(ObjectManager.Player.ServerPosition) / 10);
                        Vector3 direction = (t.ServerPosition - ObjectManager.Player.ServerPosition).LSNormalized();
                        for (int i = 0; i < steps - 1; i++)
                        {
                            if (NavMesh.GetCollisionFlags(ObjectManager.Player.ServerPosition + direction * 10 * i).HasFlag(CollisionFlags.Wall))
                                return;
                        }
                        ObjectManager.Player.Spellbook.CastSpell(Me.SummonerFlash, t.ServerPosition);
                    }
                    Target.SetFlashed(false);
                }
            });
            #endregion

            #region Normal Combo
            MethodsOnUpdate[0] = (t) =>
            {
                if (t != null)
                {
                    //gapclose
                    for (int i = 0; i < GapCloseMethods.Length; i++)
                        GapCloseMethods[i](t);

                    if (Me.CheckR1(t))
                    {
                        if (Me.Spells[E].LSIsReady())
                            Me.Spells[E].Cast(t.ServerPosition);
                        Me.Spells[R].Cast();
                        if (t.LSIsValidTarget(Me.Spells[W].Range))
                            Me.Spells[W].Cast();
                    }

                    if (Me.CheckR2(t))
                    {
                        Me.Spells[R].Cast(t.ServerPosition);
                        if (Me.Spells[Q].LSIsReady())
                        {
                            Me.Spells[Q].Cast(t.ServerPosition, true);
                            if(!Me.IsDoingFastQ)
                                Me.FastQCombo();
                        }
                    }

                    if (Me.Spells[W].LSIsReady() && t.LSDistance(ObjectManager.Player.ServerPosition) < Me.Spells[W].Range + t.BoundingRadius + (Animation.UltActive ? 10 : 0) && ! Me.IsDoingFastQ)
                    {
                        if (ObjectManager.Player.LSCountEnemiesInRange(1000) == 1 && ObjectManager.Player.HealthPercent > 50)
                        {
                            if (Me.Spells[E].LSIsReady() && Me.Spells[Q].LSIsReady())
                                return;

                            if (!Me.Spells[E].LSIsReady() && Me.Spells[Q].LSIsReady() && Utils.TickCount - Animation.LastETick < 1000)
                                return;
                        }
                        if (Me.Spells[E].LSIsReady() && ObjectManager.Player.LSDistance(t.ServerPosition) > 125 && Me.ConfigMenu.Item("CALWAYSE").GetValue<bool>())
                            Me.Spells[E].Cast(t.ServerPosition);
                        Me.CastCrescent();
                        Me.Spells[W].Cast(true);
                    }
                }

                if (!Animation.CanAttack() && Animation.CanCastAnimation && !Me.Spells[W].LSIsReady() && !Me.CheckR1(t))
                {
                    if (Animation.QStacks != 0 && Me.CalculateAADamage(t, 2) + (Me.Spells[E].LSIsReady() && Me.Spells[W].LSIsReady(2000) ? Me.Spells[W].GetDamage(t) : 0) + (Me.Spells[E].LSIsReady() && ObjectManager.Player.HasBuff("RivenFengShuiEngine") && Me.Spells[R].LSIsReady() ? Me.CalculateDamageR2(t) : 0) > t.Health && ObjectManager.Player.HealthPercent > 20 && ObjectManager.Player.LSCountEnemiesInRange(1000) > 1)
                        return;
                    Me.FastQCombo();
                }
            };

            MethodsOnAnimation[0] = (t, animname) =>
            {
                if (Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo || Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Mixed)
                {
                    t = Target.Get(600, true);
                    if (t != null)
                    {
                        if(animname == "Spell1c")
                        {
                            if (Me.Spells[W].LSIsReady() && t.LSIsValidTarget(Me.Spells[W].Range))
                                Me.Spells[W].Cast(true);
                        }
                        if (animname == "Spell3") //e w & e q etc
                        {
                            if (Me.CheckR1(t))
                            {
                                Me.Spells[R].Cast();
                                return;
                            }

                            if (Me.Spells[W].LSIsReady() && t.LSDistance(ObjectManager.Player.ServerPosition) < Me.Spells[W].Range + t.BoundingRadius && !Me.IsDoingFastQ && Me.Spells[Q].LSIsReady())
                            {
                                Me.Spells[W].Cast(true);
                                return;
                            }

                            if (Me.Spells[Q].LSIsReady() && !Me.IsDoingFastQ && !Me.CheckR1(t) && t.LSDistance(ObjectManager.Player.ServerPosition) < Me.Spells[Q].Range)
                            {
                                if (ObjectManager.Player.LSIsDashing())
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(Utils.TickCount - ObjectManager.Player.GetDashInfo().EndTick, () =>
                                        {
                                            Me.Spells[Q].Cast(t.ServerPosition, true);
                                            Me.FastQCombo();
                                        });
                                    return;
                                }
                            }

                            Me.CastCrescent();
                        }
                        else if (animname == "Spell4a")
                        {
                            if (Me.Spells[W].LSIsReady() && t.LSIsValidTarget(Me.Spells[W].Range - 10))
                            {
                                Me.Spells[W].Cast();
                                return;
                            }
                        }
                        else if (animname == "Spell4b")
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () =>
                            {
                                if (Me.IsCrestcentReady)
                                    Me.CastCrescent();
                                if (Me.Spells[Q].LSIsReady())
                                {
                                    Me.Spells[Q].Cast(t.ServerPosition, true);
                                    if (!Me.IsDoingFastQ)
                                        Me.FastQCombo();
                                }
                            });
                        }
                        else if (animname == "Spell2")
                        {
                            if (Me.Spells[Q].LSIsReady() && !Me.IsDoingFastQ)
                            {
                                Me.Spells[Q].Cast(t.ServerPosition, true);
                                Me.FastQCombo();
                            }
                        }
                    }
                }
            };
            #endregion

            #region Shy Burst (E-R-Flash-W-AA-R2-Hydra-Q)
            MethodsOnUpdate[1] = (t) =>
            {
                t = Target.Get(1000);
                if (t != null)
                {
                    if (t.Health - Me.CalculateDamageR2(t) < 0 && ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady() && t.LSDistance(ObjectManager.Player.ServerPosition) < 650)
                        Me.Spells[R].Cast(t.ServerPosition);

                    if (Me.Spells[E].LSIsReady() && ObjectManager.Player.ServerPosition.LSDistance(t.ServerPosition) <= 700 && !ObjectManager.Player.HasBuff("RivenFengShuiEngine"))
                    {
                        Me.Spells[E].Cast(t.ServerPosition);
                        if (!Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                            Me.Spells[R].Cast();
                        return;
                    }

                    if (Me.Spells[W].LSIsReady() && t.LSIsValidTarget(Me.Spells[W].Range + t.BoundingRadius + 10))
                    {
                        Me.CastCrescent();
                        Me.Spells[W].Cast();
                        return;
                    }
                }
            };

            MethodsOnAnimation[1] = (t, animname) =>
            {
                switch (animname)
                {
                    case "Spell3": //e r1
                        {
                            if (!ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                Me.Spells[R].Cast();
                        }
                        break;
                    case "Spell4a": //r flash
                        {
                            if (t.LSDistance(ObjectManager.Player.ServerPosition) > 300)
                            {   
                                ObjectManager.Player.Spellbook.CastSpell(Me.SummonerFlash, t.ServerPosition);
                                Me.CastCrescent();
                            }
                        }
                        break;
                    case "Spell4b":
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(100, () =>
                                {
                                    if (Me.IsCrestcentReady)
                                        Me.CastCrescent();
                                    if (Me.Spells[Q].LSIsReady())
                                    {
                                        Me.Spells[Q].Cast(t.ServerPosition, true);
                                        if (!Me.IsDoingFastQ)
                                            Me.FastQCombo();
                                    }
                                });
                        }
                        break;
                    case "Spell2":
                        {
                            if (Me.Spells[Q].LSIsReady())
                            {
                                Me.Spells[Q].Cast(t.ServerPosition, true);
                            }
                        }
                        break;
                }
            };
            #endregion

            #region Flash Combo (Q1-Q2-E-R1-Flash-Q3-Hydra-W-R2)
            MethodsOnUpdate[2] = (t) =>
            {
                if (!ObjectManager.Player.Spellbook.GetSpell(Me.SummonerFlash).LSIsReady() && !ObjectManager.Player.HasBuff("RivenFengShuiEngine"))
                {
                    MethodsOnUpdate[0](t);
                    return;
                }

                t = Target.Get(1000);
                if (Animation.QStacks == 2)
                {
                    if (!Me.Spells[E].LSIsReady() && !ObjectManager.Player.HasBuff("RivenFengShuiEngine"))
                        return;

                    if (t != null)
                    {
                        if (Me.Spells[E].LSIsReady())
                        {
                            Me.Spells[E].Cast(t.ServerPosition);
                            return;
                        }

                        if (t.LSIsValidTarget(600))
                        {
                            Me.CastCrescent();
                            if (Me.Spells[W].LSIsReady())
                            {
                                if (t.LSIsValidTarget(Me.Spells[W].Range + t.BoundingRadius))
                                    Me.Spells[W].Cast();
                            }
                            else
                                if (ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                    Me.Spells[R].Cast(t.ServerPosition);
                        }
                    }
                }
                else
                {
                    if (Me.Spells[Q].LSIsReady())
                    {
                        if (Utils.TickCount - Animation.LastQTick >= 1000)
                            Me.Spells[Q].Cast(Game.CursorPos, true);
                    }
                }
            };

            MethodsOnAnimation[2] = (t, animname) =>
            {
                {
                    switch (animname)
                    {
                        case "Spell3": //e r1
                            {
                                if (!ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                    Me.Spells[R].Cast();
                            }
                            break;
                        case "Spell4a": //r1 flash
                            {
                                if (t.LSDistance(ObjectManager.Player.ServerPosition) > 300 && Me.Orbwalker.ActiveMode == SCommon.Orbwalking.Orbwalker.Mode.Combo)
                                {
                                    ObjectManager.Player.Spellbook.CastSpell(Me.SummonerFlash, t.ServerPosition);
                                    Me.Spells[Q].Cast(t.ServerPosition, true);
                                }
                            }
                            break;
                        case "Spell2": //w r2
                            {
                                if (ObjectManager.Player.HasBuff("RivenFengShuiEngine") && !Me.ConfigMenu.Item("CDISABLER").GetValue<KeyBind>().Active && Me.Spells[R].LSIsReady())
                                    Me.Spells[R].Cast(t.ServerPosition);
                            }
                            break;
                    }
                }
            };
            #endregion
        }
    }
}
