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
    using static BadaoGravesVariables;
    using static BadaoMainVariables;
    public static class BadaoGravesBurst
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
            //EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
        }

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!BurstKey.GetValue<KeyBind>().Active)
                return;
            if (!(target is Obj_AI_Base))
                return;
            LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping - Game.Ping, () =>
            {
                if (E.IsReady() && R.IsReady())
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 250; i <= 425; i += 5)
                    {
                        positions.Add(Player.Position.To2D().Extend(target.Position.To2D(), 250));
                    }
                    Vector2 position = positions.OrderBy(x => x.Distance(target.Position)).FirstOrDefault();
                    if (position.IsValid() && target.Position.To2D().Distance(position) <= Player.AttackRange + Player.BoundingRadius)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150, () => R.Cast(target  as Obj_AI_Base));
                        LeagueSharp.Common.Utility.DelayAction.Add(300, () => E.Cast(position));
                        LeagueSharp.Common.Utility.DelayAction.Add(300, () => Orbwalking.ResetAutoAttackTimer());
                    }
                }
            });
        }

        public static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!BurstKey.GetValue<KeyBind>().Active)
                return;
            if (!sender.IsMe)
                return;
            if (args.Order != GameObjectOrder.AttackUnit)
                return;
            if (args.Target == null)
                return;
            if (!(args.Target is Obj_AI_Base))
                return;
            if (Player.Distance(args.Target.Position) > Player.BoundingRadius + Player.AttackRange + args.Target.BoundingRadius - 20)
                return;
            var target = args.Target;
            LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping - Game.Ping, () =>
            {
                if (E.IsReady() && !R.IsReady())
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 250; i <= 425; i += 5)
                    {
                        positions.Add(Player.Position.To2D().Extend(Game.CursorPos.To2D(), 250));
                    }
                    Vector2 position = positions.OrderBy(x => x.Distance(target.Position)).FirstOrDefault();
                    if (position.IsValid() && target.Position.To2D().Distance(position) <= Player.AttackRange + Player.BoundingRadius)
                    {
                        E.Cast(position);
                    }
                }
            }
          );
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!BurstKey.GetValue<KeyBind>().Active)
                return;
            var target1 = TargetSelector.GetSelectedTarget();
            Orbwalking.Orbwalk(target1.BadaoIsValidTarget(Player.BoundingRadius + Player.AttackRange + 50) ? target1 : null, Game.CursorPos);
            if (R.IsReady()/* || E.IsReady()*/)
                return;
            if (Q.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target.BadaoIsValidTarget() && BadaoMath.GetFirstWallPoint(Player.Position.To2D(), target.Position.To2D()) == null)
                {
                    Q.Cast(target);
                }
            }
            if (W.IsReady())
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target.BadaoIsValidTarget())
                {
                    W.Cast(target);
                }
            }
        }
    }
}
