#region

using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.Utils;
using Spirit_Karma.Core;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Event
{
    internal class Mode : Core.Core
    {

        public static void OnUpdate(EventArgs args)
        {
            ChangeMantra();
            OrbHandler();
        }

        private static void ChangeMantra()
        {
            if (MenuConfig.Mantra.Active)
            {
                switch (MenuConfig.MantraMode.SelectedValue)
                {
                    case "Q":
                        DelayAction.Add(200, () => MenuConfig.MantraMode.SelectedValue = "W");
                        break;
                    case "W":
                        DelayAction.Add(200, () => MenuConfig.MantraMode.SelectedValue = "E");
                        break;
                    case "E":
                        DelayAction.Add(200, () => MenuConfig.MantraMode.SelectedValue = "Auto");
                        break;
                    case "Auto":
                        DelayAction.Add(200, () => MenuConfig.MantraMode.SelectedValue = "Q");
                        break;
                }
            }
        }
        private static void OrbHandler()
        {
            if (MenuConfig.FleeKey.Active)
            {
                Flee();
            }
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;

                case OrbwalkingMode.Hybrid:
                    Mixed();
                    break;

                case OrbwalkingMode.LaneClear:
                    Lane();
                    Jungle();
                    break;

                case OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }
        private static void Combo()
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(x => !x.IsDead && !x.IsZombie).OrderBy(hp => hp.Health))
            {
                if (!enemy.IsValidTarget(Spells.Q.Range) || enemy.IsDead || enemy.IsInvulnerable) return;

                Usables.Locket();
                //    Usables.Seraph();
                switch (MenuConfig.MantraMode.Index)
                {
                    case 0:
                        if (Spells.R.IsReady() && Spells.Q.IsReady())
                        {
                            Spells.R.Cast();
                            Spells.Q.Cast(enemy);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        break;
                    case 1:
                        if (Spells.R.IsReady() && Spells.W.IsReady())
                        {
                            Spells.R.Cast();
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        break;
                    case 2:
                        if (Spells.R.IsReady() && Spells.E.IsReady())
                        {
                            Spells.R.Cast();

                        }
                        else if (Spells.E.IsReady())
                        {
                            Spells.E.Cast(Player);
                        }
                        else if (Spells.W.IsReady())
                        {
                            Spells.W.Cast(enemy);
                        }
                        else if (Spells.Q.IsReady())
                        {
                            Usables.FrostQueen();
                            Usables.ProtoBelt();
                            Spells.Q.Cast(enemy);
                        }
                        break;
                    // Auto
                    case 3:
                        if (Player.HealthPercent <= 30 && enemy.HealthPercent >= 50)
                        {
                            goto case 2;
                        }
                        if (!enemy.IsFacing(Player))
                        {
                            goto case 1;
                        }
                        goto case 0;
                }
            }
        }

        private static void Mixed()
        {
            if (MenuConfig.HarassR && Spells.R.IsReady())
            {
                if (Spells.Q.IsReady() || Spells.E.IsReady())
                {
                    Spells.R.Cast();
                }
            }
            if (MenuConfig.HarassQ.BValue && Spells.Q.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.HarassQ.Value)) return;
                {
                    Spells.Q.Cast(Target);
                }
            }
            if (MenuConfig.HarassW.BValue && Spells.W.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.HarassW.Value)) return;
                {
                    Spells.W.Cast(Target);
                }
            }
            if (MenuConfig.HarassE.BValue && Spells.E.IsReady())
            {
                if (!(Player.ManaPercent >= MenuConfig.HarassE.Value)) return;
                {
                    Spells.E.Cast(Player);
                }
            }
        }

        private static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsMinion && m.IsEnemy && m.Team != GameObjectTeam.Neutral && m.IsValidTarget(1050));

            foreach (var m in minions)
            {
                if (MenuConfig.LaneR && Spells.R.IsReady())
                {
                    if (Spells.Q.IsReady() && Player.ManaPercent >= MenuConfig.LaneQ.Value)
                    {
                        Spells.R.Cast();
                    }
                }
                if (MenuConfig.LaneQ.BValue && Spells.Q.IsReady() && m.Health > Player.GetAutoAttackDamage(m))
                {
                    if (!(Player.ManaPercent >= MenuConfig.LaneQ.Value)) return;
                    {
                        Spells.Q.Cast(m.ServerPosition);
                    }
                }
                if (MenuConfig.LaneE.BValue && Player.ManaPercent >= MenuConfig.LaneE.Value && Spells.E.IsReady())
                {
                    if (!(Player.ManaPercent >= MenuConfig.LaneE.Value)) return;
                    {
                        Spells.E.Cast(Player);
                    }
                }
            }
        }

        private static void Jungle()
        {
            var mob = ObjectManager.Get<Obj_AI_Minion>().Where(m => !m.IsDead && !m.IsZombie && m.Team == GameObjectTeam.Neutral && m.IsValidTarget(Spells.Q.Range));
            foreach (var m in mob)
            {
                if (Spells.R.IsReady())
                {
                    if (Spells.Q.IsReady())
                    {
                        Spells.R.Cast();
                        Spells.Q.Cast(m.ServerPosition);
                    }
                    else if (Spells.E.IsReady() && Player.HealthPercent <= 80)
                    {
                        Spells.R.Cast();
                        Spells.E.Cast(Player);
                    }
                }
               else if (Spells.Q.IsReady())
                {
                    Spells.Q.Cast(m.ServerPosition);
                }
               else if (Spells.E.IsReady())
               {
                   Spells.E.Cast(Player);
               }
                else if (Spells.W.IsReady() && Player.ManaPercent >= 35)
                {
                    Spells.W.Cast(m);
                }
            }
        }

        private static void LastHit()
        {
            
        }

        private static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (!MenuConfig.FleeKey.Active)
            {
                return;
            }

            if (Spells.R.IsReady() && Spells.E.IsReady())
            {
                Spells.R.Cast();
                Spells.E.Cast(Player);
            }
           else if (Spells.E.IsReady())
            {
                Spells.E.Cast(Player);
            }
        }
    }
}
