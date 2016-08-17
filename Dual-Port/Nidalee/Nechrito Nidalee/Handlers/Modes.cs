using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Linq;
using Nechrito_Nidalee.Extras;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Nidalee.Handlers
{
    class Modes : Core
    {
        
        public static void Combo()
        {
           
            var Target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);

            if (Target == null)
            {
                return;
            }

            var QPred = Champion.Javelin.GetPrediction(Target);
            var SwipePred = Champion.Swipe.GetPrediction(Target);
            var PouncePred = Champion.Pounce.GetPrediction(Target);
            var Hunted = Player.HasBuff("NidaleePassiveHunted") || Player.HasBuff("exposeweaknessdebuff") || Target.HasBuff("NidaleePassiveHunted") || Target.HasBuff("exposeweaknessdebuff");

            // The full 1v1 rotation
            if ((Player.Distance(Target.Position) <= 1500) && Target != null && Target.IsValidTarget())
            {
                if (CatForm() && Champion.Aspect.IsReady() && !Hunted)
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Champion.Pounce.IsReady() && !Target.UnderTurret() && Target.Distance(Player) <= 750 && Hunted)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);
                    Champion.Pounce.Cast(Target);
                }
                if (!CatForm() && Champion.Bushwack.IsReady() && Player.ManaPercent >= 30 && (Player.Distance(Target.Position) <= Champion.Bushwack.Range))
                {
                    Champion.Bushwack.Cast(Target.ServerPosition - 75);
                }
               if (!CatForm() && Champion.Javelin.IsReady() && QPred.Hitchance >= HitChance.VeryHigh && QPred.Hitchance != HitChance.Collision)
                {
                    Champion.Javelin.Cast(QPred.CastPosition);
                }
                if(!CatForm() && Champion.Primalsurge.IsReady() && Player.HealthPercent <= 85)
                {
                    Champion.Primalsurge.Cast(Player);
                }
                if (!CatForm() && Champion.Aspect.IsReady() && !Champion.Javelin.IsReady() && (Player.Distance(Target.Position) <= 325) || Hunted)
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Target.Distance(Player) <= 300 && Champion.Swipe.IsReady())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);
                    Champion.Swipe.Cast(SwipePred.CastPosition);
                }
                if (CatForm() && Champion.Pounce.IsReady() && !Target.UnderTurret() && Target.Distance(Player) <= 370 && !Hunted)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);
                    Champion.Pounce.Cast(PouncePred.CastPosition);
                }
                if (CatForm())
                {
                    Champion.Takedown.Cast(Target);
                }
            }
        }
        public static void Harass()
        {
            var Target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            if (Target == null)
            {
                return;
            }
            var QPred = Champion.Javelin.GetPrediction(Target);
            if (Target != null && Target.IsValidTarget() && !Target.IsZombie)
            {
                if (!CatForm() && Champion.Javelin.IsReady() && QPred.Hitchance >= HitChance.VeryHigh && QPred.Hitchance != HitChance.Collision)
                {
                    Champion.Javelin.Cast(QPred.CastPosition);
                }
            }
        }
        public static void Lane()
        {
            var minions = MinionManager.GetMinions(600f).FirstOrDefault();
            if (minions == null) return;

            if (!CatForm() && minions.Distance(Player) <= 325)
            { Champion.Aspect.Cast(); }

            if (!CatForm()) return;

            var m = MinionManager.GetMinions(Player.Position, 600);
            foreach (var min in m)
            {
                if (min.Health <= Champion.Takedown.GetDamage(min) && m.Count > 0)
                    Champion.Takedown.Cast(min);

                if (min.Health <= Champion.Swipe.GetDamage(min) && m.Count > 0)
                    Champion.Swipe.Cast(min.ServerPosition);

                if (min.Health <= Champion.Pounce.GetDamage(min) && m.Count > 2)
                    Champion.Pounce.Cast(min);
                   
            }
        }
        public static void Jungle()
        {
            var mobs = MinionManager.GetMinions(550 + Player.AttackRange, MinionTypes.All, MinionTeam.Neutral,
           MinionOrderTypes.MaxHealth);
            if (mobs.Count == 0)
                return;

            if(Player.HealthPercent <= MenuConfig.jnglHeal.Value && CatForm())
            { Champion.Aspect.Cast(); Champion.Primalsurge.Cast(Player); }

            foreach (var m in mobs)
            {
                if(CatForm() && m.Health < (float)Program.Player.GetAutoAttackDamage(m) && m.Distance(Player) > 300)
                {
                    Champion.Aspect.Cast();
                }
                if (!CatForm() && Player.ManaPercent <= MenuConfig.jnglQ.Value)
                    Champion.Javelin.Cast(m);
                if (!CatForm() && Player.HealthPercent <= MenuConfig.jnglHeal.Value)
                    Champion.Primalsurge.Cast(Player);

                if (!CatForm() && Champion.Bushwack.IsReady() && (float)Player.GetAutoAttackDamage(m) > m.Health)
                {
                    Champion.Bushwack.Cast(m.ServerPosition);
                }
                if (!CatForm() && Champion.Aspect.IsReady())
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Champion.Swipe.IsReady() && m.Distance(Player) < 200)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, m);
                    Champion.Swipe.Cast(m.ServerPosition);
                }
                if (CatForm() && Champion.Pounce.IsReady())
                {
                    Champion.Pounce.Cast(m);
                }
                if (CatForm() && Champion.Takedown.IsReady())
                {
                    Champion.Takedown.Cast(m);
                }
                if (CatForm() && Champion.Aspect.IsReady())
                {
                    Champion.Aspect.Cast();
                }
                if (!CatForm() && Champion.Aspect.IsReady())
                {
                    Champion.Aspect.Cast();
                }
            }
        }
        public static void Flee()
        {
            if (!MenuConfig.FleeMouse)
            {
                return;
            }
            var IsWallDash = FleeLogic.IsWallDash(Player.ServerPosition, Champion.Pounce.Range);
            var end = Player.ServerPosition.Extend(Game.CursorPos, Champion.Pounce.Range);
            var WallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);


            if (IsWallDash && Champion.Pounce.IsReady())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);
            }
            if (IsWallDash && Champion.Pounce.IsReady() && WallPoint.Distance(Player.ServerPosition) <= 800)
            {
                if (!(WallPoint.Distance(Player.ServerPosition) <= 600)) return;

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);

                if (!(WallPoint.Distance(Player.ServerPosition) < 50) || !Champion.Pounce.IsReady()) return;

                if (!CatForm())
                {
                    Champion.Aspect.Cast();
                }
                Champion.Pounce.Cast(WallPoint);
            }
            else
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

    }
}
