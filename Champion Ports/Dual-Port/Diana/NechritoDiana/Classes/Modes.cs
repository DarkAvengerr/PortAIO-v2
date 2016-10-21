using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using System;
using System.Linq;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Diana
{
    class Modes
    {
        public static void ComboLogic()
        {
            var rTarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Magical);
            
            var arcPred = Spells.Q.GetArcSPrediction(rTarget).CastPosition;

            var Target = TargetSelector.GetTarget(Spells.Q.Range, TargetSelector.DamageType.Magical);

            if (Target.Health < Dmg.ComboDmg(Target) || MenuConfig.Misaya)
            {
                if (rTarget.IsValidTarget() && !rTarget.IsDead && !rTarget.IsInvulnerable && rTarget != null)
                {
                    if (Spells.Q.IsReady() && arcPred != null && Spells.R.IsReady())
                    {
                        Spells.Q.Cast(arcPred);
                        Spells.R.Cast(rTarget);
                    }
                }
            }

            if (Spells.Q.IsReady())
            {
                if (rTarget != null && arcPred != null)
                {
                    Spells.Q.Cast(arcPred);
                }
            }

            if (Spells.R.IsReady() && (ObjectManager.Player.Distance(rTarget.Position) <= Spells.R.Range))
            {
                if (rTarget != null && rTarget.HasBuff("dianamoonlight") && MenuConfig.ComboR)
                {
                    Spells.R.Cast(rTarget);
                }
                if (!MenuConfig.ComboR && rTarget != null)
                {
                    Spells.R.Cast(rTarget);
                }
            }

            if (Target != null && Target.IsValidTarget())
            {
                if (Spells.W.IsReady() && (ObjectManager.Player.Distance(Target.Position) <= ObjectManager.Player.AttackRange + 30))
                {
                    Spells.W.Cast(Target);
                }

                if (MenuConfig.ComboE && ObjectManager.Player.ManaPercent > 25)
                {
                    if (Spells.E.IsReady() && (ObjectManager.Player.Distance(Target.Position) <= Spells.E.Range - 45 || Target.CountEnemiesInRange(Spells.E.Range) > 1 || Target.IsDashing()))
                    {
                        Spells.E.Cast(Target);
                    }
                }
            }
        }
        public static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);
            var arcPred = Spells.Q.GetArcSPrediction(target).CastPosition;

            if (target != null && target.IsValidTarget() && !target.IsZombie)
            {
                if (Spells.Q.IsReady())
                {
                    Spells.Q.Cast(arcPred);
                }
                if (Spells.W.IsReady() && ObjectManager.Player.Distance(target.Position) <= ObjectManager.Player.AttackRange)
                {
                    Spells.W.Cast();
                }
            }
        }
        public static void JungleLogic()
        {
            if (MenuConfig._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var mobs = MinionManager.GetMinions(800 + ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (mobs.Count == 0) return;

                if (Spells.Q.IsReady() && Spells.R.IsReady())
                {
                    var m = MinionManager.GetMinions(800 + ObjectManager.Player.AttackRange, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    if (m != null && MenuConfig.jnglQR && (ObjectManager.Player.Distance(m[0].Position) <= 700f) && (ObjectManager.Player.Distance(m[0].Position) >= 400f) && ObjectManager.Player.ManaPercent > 20)
                    {
                        Spells.Q.Cast(m[0]);
                        Spells.R.Cast(m[0]);
                    }
                }
                if (Spells.W.IsReady() && (ObjectManager.Player.Distance(mobs[0].Position) <= 300f) && MenuConfig.jnglW)
                {
                    Spells.W.Cast();
                }

                if (Spells.E.IsReady())
                {
                    var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.E.Range);
                    foreach (var m in mobs)
                    {
                        if (m.IsAttackingPlayer && ObjectManager.Player.HealthPercent <= MenuConfig.jnglE.Value)
                        {
                            Spells.E.Cast(m);
                        }
                    }
                }
            }
        }
        public static void LaneLogic()
        {
            if (MenuConfig._orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var minions = MinionManager.GetMinions(800f).FirstOrDefault();
                if (minions == null)
                    return;

                if (Spells.W.IsReady() && MenuConfig.LaneW && (ObjectManager.Player.Distance(minions.Position) <= 250f))
                {
                    Spells.W.Cast();
                }

                if (Spells.Q.IsReady() && MenuConfig.LaneQ && ObjectManager.Player.ManaPercent >= 45 && (ObjectManager.Player.Distance(minions.Position) <= 550f))
                {
                    var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.W.Range);
                    foreach (var m in minion)
                    {
                        if (m.Health < Spells.Q.GetDamage(m) && minion.Count > 2 && !MenuConfig._orbwalker.InAutoAttackRange(m))
                            Spells.Q.Cast(m);
                    }
                }
            }
        }
        public static void Game_OnUpdate(EventArgs args)
        {
        }
        public static void Flee()
        {
            if (!MenuConfig.FleeMouse)
            {
                return;
            }
            
            var jump = Program.JumpPos.Where(x => x.Value.Distance(ObjectManager.Player.Position) < 300f && x.Value.Distance(Game.CursorPos) < 700f).FirstOrDefault();
            var monster = MinionManager.GetMinions(900f, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health).FirstOrDefault();
            var mobs = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.NotAlly);
            
            if (jump.Value.IsValid())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, jump.Value);

                foreach (var junglepos in Program.JunglePos)
                {
                    if (Game.CursorPos.Distance(junglepos) <= 350 && ObjectManager.Player.Position.Distance(junglepos) <= 850 && Spells.Q.IsReady() && Spells.R.IsReady())
                    {
                        Spells.Q.Cast(junglepos);
                        Spells.R.Cast(monster);
                    }
                }
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            foreach (var junglepos in Program.JunglePos)
            {
                if (Game.CursorPos.Distance(junglepos) <= 350 && ObjectManager.Player.Position.Distance(junglepos) <= 900 && Spells.Q.IsReady() && Spells.R.IsReady())
                {
                    Spells.Q.Cast(junglepos);
                    Spells.R.Cast(monster);
                }
                else if (Spells.R.IsReady() && !Spells.Q.IsReady() && monster.Distance(ObjectManager.Player.Position) > 600f && monster.Distance(Game.CursorPos) <= 350f)
                {
                    Spells.R.Cast(monster);
                }
            }

            if (!mobs.Any()) return;

            var mob = mobs.MaxOrDefault(x => x.MaxHealth);

            if (mob.Distance(Game.CursorPos) <= 750 && mob.Distance(ObjectManager.Player) >= 475)
            {
                if (Spells.Q.IsReady() && Spells.R.IsReady() && ObjectManager.Player.Mana > Spells.R.ManaCost + Spells.Q.ManaCost && mob.Health > Spells.Q.GetDamage(mob))
                {
                    Spells.Q.Cast(mob);
                    Spells.R.Cast(mob);
                }

                if (Spells.R.IsReady())
                {
                    Spells.R.Cast(mob);
                }
            }
        }
    }
}