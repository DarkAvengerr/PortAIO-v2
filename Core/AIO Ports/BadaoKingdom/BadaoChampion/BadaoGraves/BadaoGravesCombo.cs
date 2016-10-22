using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoGraves
{
    public static class BadaoGravesCombo
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }


        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (!sender.IsMe)
                return;
            Chat.Print(args.SData.Name);
            if (args.SData.IsAutoAttack() && args.Target != null)
            {
                LeagueSharp.Common.Utility.DelayAction.Add(50 - Game.Ping, () =>
                {
                    if (BadaoMainVariables.E.IsReady() && BadaoGravesVariables.ComboE.GetValue<bool>())
                    {
                        var position = Player.Position.To2D().Extend(Game.CursorPos.To2D(), BadaoMainVariables.E.Range);
                        if (args.Target.Position.To2D().Distance(position) <= -100 + Player.AttackRange + Player.BoundingRadius
                            && !LeagueSharp.Common.Utility.UnderTurret(position.To3D(), true))
                        {
                            BadaoMainVariables.E.Cast(position);
                        }
                        else
                        {
                            var points = Geometry.CircleCircleIntersection(Player.Position.To2D(), args.Target.Position.To2D(), 425,
                                -100 + Player.AttackRange + Player.BoundingRadius);
                            var pos = points.Where(x => !NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Wall)
                                && !NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Building) && !LeagueSharp.Common.Utility.UnderTurret(x.To3D(), true))
                                .OrderBy(x => x.Distance(Game.CursorPos)/*x.To3D().CountEnemiesInRange(1000)*/).FirstOrDefault();
                            if (pos != null)
                            {
                                BadaoMainVariables.E.Cast(pos);
                            }
                        }
                    }
                }
                );
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (BadaoMainVariables.Q.IsReady() && BadaoGravesVariables.ComboQ.GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, TargetSelector.DamageType.Physical);
                if (target.BadaoIsValidTarget() && BadaoMath.GetFirstWallPoint(Player.Position.To2D(),target.Position.To2D()) == null)
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
            if (BadaoMainVariables.W.IsReady() && BadaoGravesVariables.ComboW.GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.W.Range, TargetSelector.DamageType.Physical);
                if (target.BadaoIsValidTarget())
                {
                    BadaoMainVariables.W.Cast(target);
                }
            }
            if (BadaoMainVariables.R.IsReady() && BadaoGravesVariables.ComboR.GetValue<bool>())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.BadaoIsValidTarget(BadaoMainVariables.R.Range)))
                {
                    if (BadaoMainVariables.R.GetDamage(hero) >= hero.Health)
                    {
                        BadaoMainVariables.R.Cast(hero);
                    }
                }
            }

        }
    }
}
