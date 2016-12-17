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
            //Orbwalking.OnAttack += Orbwalking_OnAttack;
            //EloBuddy.Player.OnIssueOrder += Obj_AI_Base_OnIssueOrder;
        }

        public static void Obj_AI_Base_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
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
                if (BadaoMainVariables.E.IsReady() && BadaoGravesVariables.ComboE.GetValue<bool>())
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 250; i <= 425; i += 5)
                    {
                        positions.Add(Player.Position.To2D().Extend(Game.CursorPos.To2D(), 250));
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

        private static void Orbwalking_OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (BadaoMainVariables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;
            if (!unit.IsMe)
                return;
            int delay2 = 200;
            LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping - Game.Ping, () =>
            {
                if (BadaoMainVariables.E.IsReady() && BadaoGravesVariables.ComboE.GetValue<bool>())
                {
                    List<Vector2> positions = new List<Vector2>();
                    for (int i = 250; i <= 425; i += 5)
                    {
                        positions.Add(Player.Position.To2D().Extend(Game.CursorPos.To2D(), 250/*i*/));
                    }
                    Vector2 position = positions.OrderBy(x => x.Distance(target.Position)).FirstOrDefault();
                    if (position.IsValid() && target.Position.To2D().Distance(position) <= Player.AttackRange + Player.BoundingRadius)
                    {
                        BadaoMainVariables.E.Cast(position);
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
