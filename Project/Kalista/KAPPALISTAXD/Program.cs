using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using globals = KAPPALISTAXD.Core.KappalistaGlobals;
using KappalistaDamageIndicator = KAPPALISTAXD.Core.KappalistaDamageIndicator;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KAPPALISTAXD
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if(globals.inGameChampion.ChampionName == "Kalista")
            {
                //--- SPELL INIT ---//
                globals.Q = new Spell(SpellSlot.Q, 1200);
                globals.W = new Spell(SpellSlot.W, 5000);
                globals.E = new Spell(SpellSlot.E, 1000);
                globals.R = new Spell(SpellSlot.R, 1500);

                globals.Q.SetSkillshot(0.35f, 40, 2400, true, SkillshotType.SkillshotLine);
                globals.R.SetSkillshot(0.50f, 1500, float.MaxValue, false, SkillshotType.SkillshotCircle);

                //--- MENU INIT ---//
                new Core.KappalistaMenu();

                //--- DRAWING INIT ---//
                new Core.KappalistaDrawing();

                Chat.Print("<font color=\"#608dbd\">K</font><font color=\"#eafeef\">APPALISTA</font> XD : loaded.");

                Game.OnUpdate += OnUpdate;
                Orbwalking.OnNonKillableMinion += OnNonKillableMinion;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;

                KappalistaDamageIndicator.DamageToUnit = GetRealDamage;
                KappalistaDamageIndicator.Enabled = true;
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            //Find SoulMate
            if (globals.soulBound == null)
            {
                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(hero => hero.IsAlly))
                {
                    foreach(BuffInstance buffs in ally.Buffs)
                    {
                        if(buffs.Name.Contains("kalistacoopstrikeally"))
                        {
                            globals.soulBound = ally;
                        }
                    }
                }
            }

            //Routine
            Routine();

            switch (globals.orbwalkerInstance.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    funcSpellCast(globals.mainMenu.Item("combo-q").GetValue<bool>());
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    funcSpellCast(globals.mainMenu.Item("combo-e").GetValue<bool>());
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    funcSpellCastLane(globals.mainMenu.Item("jgclear-e").GetValue<bool>());
                    break;
            }
        }

        private static void OnNonKillableMinion(AttackableUnit minion)
        {
            if (globals.mainMenu.Item("misc-lasthit-e").GetValue<bool>())
            {
                Obj_AI_Base selectedMinion = (Obj_AI_Base)minion;
                if((globals.E.IsReady()) & (minion.IsValidTarget()) & (minion.Health <= GetRealDamage(selectedMinion)) & (globals.inGameChampion.ManaPercent > 15))
                {
                    globals.E.Cast();
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(100, Orbwalking.ResetAutoAttackTimer);
            }

            if (globals.mainMenu.Item("misc-use-r").GetValue<bool>())
            {
                if (!sender.IsAlly | !sender.IsMe)
                {
                    var sb = globals.soulBound;
                    if (sb != null)
                    {
                        if (sb.HealthPercent <= globals.mainMenu.Item("misc-use-r-pro").GetValue<Slider>().Value)
                        {
                            if(sb.Distance(sender.Position) < args.SData.CastRange + 30)
                            {
                                globals.R.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static float GetRealDamage(Obj_AI_Base target)
        {
            if (target.HasBuff("FerociousHowl"))
            {
                return (float)(globals.E.GetDamage(target) * 0.7);
            }

            if (globals.inGameChampion.HasBuff("summonerexhaust"))
            {
                return (float)(globals.E.GetDamage(target) * 0.4);
            }

            return globals.E.GetDamage(target);
        }

        private static void funcSpellCast(bool Q)
        {
            AIHeroClient target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);

            if (Q)
            {
                if(globals.Q.IsReady() && target.IsValidTarget())
                {
                    PredictionOutput pred = globals.Q.GetPrediction(target);
                    if(pred.Hitchance >= HitChance.High)
                    {
                        globals.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void funcSpellCastLane(bool E)
        {
            if (E)
            {
                if (globals.E.IsReady())
                {
                    int minions = 0;
                    List<Obj_AI_Base> minionsInERange = MinionManager.GetMinions(globals.inGameChampion.Position, globals.E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                    foreach (Obj_AI_Base minion in minionsInERange)
                    {
                        if (minion.HasBuff("kalistaexpungemarker"))
                        {
                            minions += 1;
                        }
                        if (!minion.IsDead)
                        {
                            if ((minion.IsValidTarget()) & (minion.Health < GetRealDamage(minion)) & (minions >= 3))
                            {
                                globals.E.Cast();
                                minions = 0;
                            }
                        }
                    }
                    minions = 0;
                }
            }
        }

        private static void Routine()
        {            
            //KS E
            if ((globals.mainMenu.Item("combo-e").GetValue<bool>()) | (globals.mainMenu.Item("ks-e").GetValue<bool>()))
            {
                if (globals.E.IsReady())
                {
                    List<AIHeroClient> ennemiesInERange = globals.inGameChampion.GetEnemiesInRange(globals.E.Range);
                    foreach (AIHeroClient enemy in ennemiesInERange)
                    {
                        if (!enemy.IsDead)
                        {
                            if ((enemy.IsValidTarget()) & (enemy.Health + 5 < GetRealDamage(enemy)))
                            {
                                if (globals.mainMenu.Item("misc-prevent-e").GetValue<bool>())
                                {
                                    if (!enemy.HasBuffOfType(BuffType.SpellShield) | !enemy.HasBuffOfType(BuffType.Invulnerability))
                                    {
                                        globals.E.Cast();
                                    }
                                }
                                else
                                {
                                    globals.E.Cast();
                                }
                            }
                        }
                    }
                }
            }

            //KS DB
            if (globals.mainMenu.Item("ks-db").GetValue<bool>())
            {
                if (globals.E.IsReady())
                {
                    List<Obj_AI_Base> mobsInERange = MinionManager.GetMinions(globals.inGameChampion.Position, globals.E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    foreach (Obj_AI_Base mob in mobsInERange)
                    {
                        if (mob.Name.Contains("Baron"))
                        {
                            if (globals.inGameChampion.HasBuff("barontarget"))
                            {
                                if(mob.Health < GetRealDamage(mob) * 0.5f)
                                {
                                    globals.E.Cast();
                                }
                            }
                            else
                            {
                                if (mob.Health < GetRealDamage(mob))
                                {
                                    globals.E.Cast();
                                }
                            }
                        }
                        else if (mob.Name.Contains("Dragon"))
                        {
                            if (globals.inGameChampion.HasBuff("barontarget"))
                            {
                                if (mob.Health < GetRealDamage(mob) * (1 - (.07f * globals.inGameChampion.GetBuffCount("s5test_dragonslayerbuff"))))
                                {
                                    globals.E.Cast();
                                }
                            }
                            else
                            {
                                if (mob.Health < GetRealDamage(mob))
                                {
                                    globals.E.Cast();
                                }
                            }
                        }
                        else if(!mob.Name.Contains("Mini") & !mob.Name.Contains("Dragon") & !mob.Name.Contains("Baron"))
                        {
                            if (mob.Health < GetRealDamage(mob))
                            {
                                globals.E.Cast();
                            }
                        }
                    }
                }
            }

            //E Range leaving
            if (globals.mainMenu.Item("misc-leaving-e").GetValue<bool>())
            {
                if (globals.E.IsReady())
                {
                    List<AIHeroClient> ennemiesInERange = globals.inGameChampion.GetEnemiesInRange(globals.E.Range);
                    foreach (AIHeroClient enemy in ennemiesInERange)
                    {
                        if (!enemy.IsDead)
                        {
                            if(enemy.GetBuffCount("kalistaexpungemarker") >= globals.mainMenu.Item("misc-leaving-e-pro").GetValue<Slider>().Value)
                            if ((enemy.IsValidTarget()) & (enemy.Distance(globals.inGameChampion.Position) > globals.E.Range - 50))
                            {
                                if (globals.mainMenu.Item("misc-prevent-e").GetValue<bool>())
                                {
                                    if (!enemy.HasBuffOfType(BuffType.SpellShield) | !enemy.HasBuffOfType(BuffType.Invulnerability))
                                    {
                                        globals.E.Cast();
                                    }
                                }
                                else
                                {
                                    globals.E.Cast();
                                }
                            }
                        }
                    }
                }
            }

                //E Before dying
            if (globals.mainMenu.Item("misc-dying-e").GetValue<bool>())
            {
                if(globals.inGameChampion.HealthPercent <= globals.mainMenu.Item("misc-dying-e-pro").GetValue<Slider>().Value)
                {
                    globals.E.Cast();
                }
            }

            if (globals.mainMenu.Item("misc-ward-trick").GetValue<KeyBind>().Active)
            {
                if (globals.W.IsReady())
                {
                    if(globals.W.Range >= globals.inGameChampion.Distance(SummonersRift.River.Baron))
                    {
                        globals.W.Cast(SummonersRift.River.Baron);
                    }
                    else if (globals.W.Range >= globals.inGameChampion.Distance(SummonersRift.River.Dragon))
                    {
                        globals.W.Cast(SummonersRift.River.Dragon);
                    }
                }
            }
        }
    }
}
