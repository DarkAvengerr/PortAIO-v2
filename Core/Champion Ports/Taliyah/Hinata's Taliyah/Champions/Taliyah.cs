using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hinata_s_Taliyah.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace Hinata_s_Taliyah.Champions
{
    class Taliyah
    {
        public Taliyah()
        {
            TaliyahOnLoad();
        }

        private static void TaliyahOnLoad()
        {
            Spells.Initialize();
            Menus.Initialize();

            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnGapclose;
            Interrupter2.OnInterruptableTarget += OnInterrupt;
        }

        private static void OnInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && args.DangerLevel >= Interrupter2.DangerLevel.Low && Spells.W.IsReady() && Utilities.Enabled("w.interrupt")
                && sender.IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(sender.Position, sender.Position.Extend(ObjectManager.Player.Position, -50));
            }
        }

        private static void OnGapclose(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.End.Distance(ObjectManager.Player.Position) < 200 && Spells.E.IsReady()
                && Utilities.Enabled("e.gapcloser") && gapcloser.Sender.IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(gapcloser.End);
            }
        }


        private static void OnUpdate(EventArgs args)
        {
            switch (Menus.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnJungle();
                    OnClear();
                    break;
            }
        }

        private static void OnCombo()
        {
            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.combo") && target.IsValidTarget(Spells.Q.Range))
                {
                    var pred = Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("hitchance"))
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }

                if (Spells.W.IsReady() && Utilities.Enabled("w.combo") && target.IsValidTarget(Spells.W.Range))
                {
                    var pred = Spells.W.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("hitchance"))
                    {
                        switch (Menus.Config.Item("w.mode").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                Spells.W.Cast(pred.CastPosition, pred.CastPosition.Extend(ObjectManager.Player.Position, 50));
                                break;
                            case 1:
                                Spells.W.Cast(pred.CastPosition, pred.CastPosition.Extend(ObjectManager.Player.Position, -50));
                                break;
                        }
                        
                    }

                }

                if (Spells.E.IsReady() && Utilities.Enabled("e.combo") && target.IsValidTarget(Spells.E.Range))
                {
                    if (Utilities.Enabled("e.combo.type2"))
                    {
                        if (target.IsImmobile())
                        {
                            Spells.E.Cast(target);
                        }
                    }
                    else
                    {
                        var pred = Spells.E.GetPrediction(target);
                        if (pred.Hitchance >= Utilities.HikiChance("hitchance"))
                        {
                            Spells.E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <= Utilities.Slider("harassmana"))
            {
                return;
            }

            var target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (Spells.Q.IsReady() && Utilities.Enabled("q.harass") && target.IsValidTarget(Spells.Q.Range))
                {
                    var pred = Spells.Q.GetPrediction(target);
                    if (pred.Hitchance >= Utilities.HikiChance("hitchance"))
                    {
                        Spells.Q.Cast(pred.CastPosition);
                    }
                }
            }
        }

        private static void OnJungle()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("junglemana"))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob == null || mob.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") && mob[0].IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.Cast(mob[0].Position);
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle") && mob[0].IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(mob[0].Position, mob[0].Position.Extend(ObjectManager.Player.Position, 50));
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.jungle") && mob[0].IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(mob[0].Position);
            }
        }

        private static void OnClear()
        {
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clearmana"))
            {
                return;
            }

            var minions = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minions == null || minions.Count == 0)
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.clear"))
            {
                var minionloc = Spells.Q.GetCircularFarmLocation(minions);
                if (minionloc.MinionsHit >= Utilities.Slider("q.min.count"))
                {
                    Spells.Q.Cast(minionloc.Position);
                }
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.clear"))
            {
                var minionloc = Spells.E.GetCircularFarmLocation(minions);
                if (minionloc.MinionsHit >= Utilities.Slider("e.min.count"))
                {
                    Spells.E.Cast(minionloc.Position);
                }
            }
        }

        
    }
}
