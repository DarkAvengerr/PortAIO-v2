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
            var QPred = Champion.Javelin.GetPrediction(Target);
            var SwipePred = Champion.Swipe.GetPrediction(Target);
            var PouncePred = Champion.Pounce.GetPrediction(Target);
            var bushW = Champion.Bushwack.GetPrediction(Target).UnitPosition;
            var Hunted = Player.LSHasBuff("NidaleePassiveHunted") || Player.LSHasBuff("exposeweaknessdebuff") || Target.LSHasBuff("NidaleePassiveHunted") || Target.LSHasBuff("exposeweaknessdebuff");

            // The full 1v1 rotation
            if ((Player.LSDistance(Target.Position) <= 1500) && Target != null && Target.LSIsValidTarget())
            {
                if (CatForm() && Champion.Aspect.LSIsReady() && !Hunted)
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Champion.Pounce.LSIsReady() && !Target.LSUnderTurret() && Target.LSDistance(Player) <= 750 && Hunted)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);
                    Champion.Pounce.Cast(Target);
                }
                if (!CatForm() && Champion.Bushwack.LSIsReady() && Player.ManaPercent >= 30 && (Player.LSDistance(Target.Position) <= Champion.Bushwack.Range))
                {
                    Champion.Bushwack.Cast(Target.ServerPosition - 75);
                }
               if (!CatForm() && Champion.Javelin.LSIsReady() && QPred.Hitchance >= HitChance.VeryHigh && QPred.Hitchance != HitChance.Collision)
                {
                    Champion.Javelin.Cast(QPred.CastPosition);
                }
                if(!CatForm() && Champion.Primalsurge.LSIsReady() && Player.HealthPercent <= 85)
                {
                    Champion.Primalsurge.Cast(Player);
                }
                if (!CatForm() && Champion.Aspect.LSIsReady() && !Champion.Javelin.LSIsReady() && (Player.LSDistance(Target.Position) <= 325) || Hunted)
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Target.LSDistance(Player) <= 300 && Champion.Swipe.LSIsReady())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Target);
                    Champion.Swipe.Cast(SwipePred.CastPosition);
                }
                if (CatForm() && Champion.Pounce.LSIsReady() && !Target.LSUnderTurret() && Target.LSDistance(Player) <= 370 && !Hunted)
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
            var QPred = Champion.Javelin.GetPrediction(Target);
            if (Target != null && Target.LSIsValidTarget() && !Target.IsZombie)
            {
                if (!CatForm() && Champion.Javelin.LSIsReady() && QPred.Hitchance >= HitChance.VeryHigh && QPred.Hitchance != HitChance.Collision)
                {
                    Champion.Javelin.Cast(QPred.CastPosition);
                }
            }
        }
        public static void Lane()
        {
            var minions = MinionManager.GetMinions(600f).FirstOrDefault();
            if (minions == null) return;

            if (!CatForm() && minions.LSDistance(Player) <= 325)
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
                if(CatForm() && m.Health < (float)Program.Player.LSGetAutoAttackDamage(m) && m.LSDistance(Player) > 300)
                {
                    Champion.Aspect.Cast();
                }
                if (!CatForm() && Player.ManaPercent <= MenuConfig.jnglQ.Value)
                    Champion.Javelin.Cast(m);
                if (!CatForm() && Player.HealthPercent <= MenuConfig.jnglHeal.Value)
                    Champion.Primalsurge.Cast(Player);

                if (!CatForm() && Champion.Bushwack.LSIsReady() && (float)Player.LSGetAutoAttackDamage(m) > m.Health)
                {
                    Champion.Bushwack.Cast(m.ServerPosition);
                }
                if (!CatForm() && Champion.Aspect.LSIsReady())
                {
                    Champion.Aspect.Cast();
                }
                if (CatForm() && Champion.Swipe.LSIsReady() && m.LSDistance(Player) < 200)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, m);
                    Champion.Swipe.Cast(m.ServerPosition);
                }
                if (CatForm() && Champion.Pounce.LSIsReady())
                {
                    Champion.Pounce.Cast(m);
                }
                if (CatForm() && Champion.Takedown.LSIsReady())
                {
                    Champion.Takedown.Cast(m);
                }
                if (CatForm() && Champion.Aspect.LSIsReady())
                {
                    Champion.Aspect.Cast();
                }
                if (!CatForm() && Champion.Aspect.LSIsReady())
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
            var end = Player.ServerPosition.LSExtend(Game.CursorPos, Champion.Pounce.Range);
            var WallPoint = FleeLogic.GetFirstWallPoint(Player.ServerPosition, end);


            if (IsWallDash && Champion.Pounce.LSIsReady())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);
            }
            if (IsWallDash && Champion.Pounce.LSIsReady() && WallPoint.LSDistance(Player.ServerPosition) <= 800)
            {
                if (!(WallPoint.LSDistance(Player.ServerPosition) <= 600)) return;

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, WallPoint);

                if (!(WallPoint.LSDistance(Player.ServerPosition) < 50) || !Champion.Pounce.LSIsReady()) return;

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
