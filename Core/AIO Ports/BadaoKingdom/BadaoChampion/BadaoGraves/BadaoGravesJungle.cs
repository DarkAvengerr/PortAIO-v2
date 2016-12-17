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
    public static class BadaoGravesJungle
    {
        public static AIHeroClient Player { get{ return ObjectManager.Player; } }
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            //Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            //EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        public static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (Player.ManaPercent < BadaoGravesVariables.ManaJungle.GetValue<Slider>().Value)
                return;
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.AttackUnit)
                return;
            if (args.Target == null)
                return;
            if (args.Target.Team != GameObjectTeam.Neutral)
                return;
            if (!(args.Target is Obj_AI_Base))
                return;
            if (Player.Distance(args.Target.Position) > Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius - 20)
                return;

            var target = args.Target;

            LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping - Game.Ping, () =>
            {
                if (BadaoMainVariables.E.IsReady() && BadaoGravesVariables.JungleE.GetValue<bool>())
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 250; i <= 425; i += 5)
                    {
                        positions.Add(Player.Position.To2D().Extend(Game.CursorPos.To2D(),250));
                    }
                    Vector2 position = positions.OrderBy(x => x.Distance(target.Position)).FirstOrDefault();
                    if (position.IsValid() && target.Position.To2D().Distance(position) <= Player.AttackRange + Player.BoundingRadius)
                    {
                        BadaoMainVariables.E.Cast(position);
                        LeagueSharp.Common.Utility.DelayAction.Add(0, () => Orbwalking.ResetAutoAttackTimer());
                        //for (int i = 0; i < delay2; i = i + 5)
                        //{
                        //    LeagueSharp.Common.Utility.DelayAction.Add(i, () =>
                        //    {
                        //        EloBuddy.Player.DoEmote(Emote.Dance);
                        //        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        //        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        //    });
                        //}
                    }
                }
            }
            );
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (Player.ManaPercent < BadaoGravesVariables.ManaJungle.GetValue<Slider>().Value)
                return;
            if (!sender.IsMe)
                return;
            if (args.SData.IsAutoAttack() && args.Target != null && args.Target.Team == GameObjectTeam.Neutral)
            {
                int delay2 = 200;
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping - Game.Ping, () =>
                {
                    if (BadaoMainVariables.E.IsReady() && BadaoGravesVariables.JungleE.GetValue<bool>())
                    {
                        var position = Player.Position.To2D().Extend(Game.CursorPos.To2D(), 250/*BadaoMainVariables.E.Range*/);
                        if (args.Target.Position.To2D().Distance(position) <= -250 + Player.AttackRange + Player.BoundingRadius
                            /*&& !Utility.UnderTurret(position.To3D(), true)*/)
                        {
                            BadaoMainVariables.E.Cast(position);
                            //for (int i = 0; i < delay2; i = i + 5)
                            //{
                            //    EloBuddy.Player.DoEmote(Emote.Dance); EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            //    LeagueSharp.Common.Utility.DelayAction.Add(i, () => {
                            //        EloBuddy.Player.DoEmote(Emote.Dance);
                            //        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            //        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                            //    });
                            //}

                        }
                        else
                        {
                            var points = Geometry.CircleCircleIntersection(Player.Position.To2D(), args.Target.Position.To2D(), 425,
                                -250 + Player.AttackRange + Player.BoundingRadius);
                            var pos = points.Where(x => !NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Wall)
                                && !NavMesh.GetCollisionFlags(x.X, x.Y).HasFlag(CollisionFlags.Building)/* && !Utility.UnderTurret(x.To3D(), true)*/)
                                .OrderBy(x => x.Distance(Game.CursorPos)/*x.To3D().CountEnemiesInRange(1000)*/).FirstOrDefault();
                            if (pos != null)
                            {
                                BadaoMainVariables.E.Cast(pos);
                                for (int i = 0; i < delay2; i = i + 5)
                                {
                                    EloBuddy.Player.DoEmote(Emote.Dance); EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                                    LeagueSharp.Common.Utility.DelayAction.Add(i, () => {
                                        EloBuddy.Player.DoEmote(Emote.Dance);
                                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, args.Target);
                                    });
                                }
                            }
                        }
                    }
                }
                );
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
                return;
            if (Player.ManaPercent < BadaoGravesVariables.ManaJungle.GetValue<Slider>().Value)
                return;
            if (BadaoMainVariables.Q.IsReady() && BadaoGravesVariables.JungleQ.GetValue<bool>())
            {
                var target  = MinionManager.GetMinions(BadaoMainVariables.Q.Range, MinionTypes.All, MinionTeam.Neutral
                                        , MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (target.BadaoIsValidTarget() && BadaoMath.GetFirstWallPoint(Player.Position.To2D(), target.Position.To2D()) == null)
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
        }
    }
}
