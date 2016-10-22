using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using System;

    using Core;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Menus;

    #endregion

    internal class ComboMode : Core
    {
        #region Public Methods and Operators

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Player.AttackRange + 310, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                var pred = Spells.R.GetPrediction(target, true, collisionable: new[] { CollisionableObjects.YasuoWall });

                if (pred.Hitchance < HitChance.High || target.HasBuff(NoRList.ToString()))
                {
                    return;
                }

                if ((!MenuConfig.OverKillCheck && Qstack > 1)
                    || MenuConfig.OverKillCheck 
                    && (target.HealthPercent <= 40 
                    && !Spells.Q.IsReady() && Qstack == 1
                    || target.Distance(Player) >= Player.AttackRange + 310))
                {
                    Spells.R.Cast(pred.CastPosition);
                }
            }

           if (Qstack == 3
                    && target.Distance(Player) >= Player.AttackRange
                    && target.Distance(Player) <= 650
                    && MenuConfig.Q3Wall
                    && Spells.E.IsReady())
            {
                var wallPoint = FleeLogic.GetFirstWallPoint(Player.Position, Player.Position.Extend(target.Position, 650));

                Player.GetPath(wallPoint);

                //if (wallPoint.Distance(Player.Position) > 100)
                //{
                //    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, wallPoint);
                //}

                if (!Spells.E.IsReady() || wallPoint.Distance(Player.Position) > Spells.E.Range || !wallPoint.IsValid())
                {
                    return;
                }


                Spells.E.Cast(wallPoint);

                if (Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR)
                {
                    Spells.R.Cast();
                }

                LeagueSharp.Common.Utility.DelayAction.Add(190, () => Spells.Q.Cast(wallPoint));
                
                if (wallPoint.Distance(Player.Position) <= 100)
                {
                    Spells.Q.Cast(wallPoint);
                }
            }
           else if (Spells.E.IsReady()) 
            {
                Spells.E.Cast(target.Position);

                if (Spells.R.IsReady())
                {
                    return;
                }

                LeagueSharp.Common.Utility.DelayAction.Add(10, Usables.CastHydra);
            }
           else if (MenuConfig.AlwaysR
                && Spells.R.IsReady()
                && Spells.R.Instance.Name == IsFirstR)
            {
                Spells.R.Cast();
            }
           else if (!Spells.W.IsReady() || target.HasBuff("FioraW"))
            {
                return;
            }

            if (MenuConfig.NechLogic && (Qstack > 1 || !Spells.Q.IsReady()))
            {
                CastW(target);
            }

            if (!MenuConfig.NechLogic && (Player.HasBuff("RivenFeint") || target.IsFacing(Player)))
            {
                CastW(target);
            }
        }

        #endregion
    }
}
