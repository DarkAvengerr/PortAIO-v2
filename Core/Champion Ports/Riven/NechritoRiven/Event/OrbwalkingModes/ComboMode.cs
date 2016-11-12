using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using Core;

    using LeagueSharp.Common;

    #endregion

    internal class ComboMode : Core
    {
        #region Public Methods and Operators

        public static void Combo()
        {
            var targetAquireRange = Spells.R.IsReady() ? Player.AttackRange + 390 : Player.AttackRange + 370;

            var target = TargetSelector.GetTarget(targetAquireRange, TargetSelector.DamageType.Physical);

            if (target == null || !target.IsValidTarget()) return;

            if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR)
            {
                var pred = Spells.R.GetPrediction(target, true, collisionable: new[] { CollisionableObjects.YasuoWall });

                if (pred.Hitchance < HitChance.High || target.HasBuff(BackgroundData.InvulnerableList.ToString()) || Player.Spellbook.IsAutoAttacking)
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

            #region Q3 Wall

            if (Qstack == 3
                    && target.Distance(Player) >= Player.AttackRange
                    && target.Distance(Player) <= 650
                    && MenuConfig.Q3Wall
                    && Spells.E.IsReady())
            {
                var wallPoint = FleeLogic.GetFirstWallPoint(Player.Position, Player.Position.Extend(target.Position, 650));

                Player.GetPath(wallPoint);

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
            #endregion

            if (Spells.E.IsReady())
            {
                Spells.E.Cast(target.Position);

                if (MenuConfig.AlwaysR && Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR)
                {
                    Spells.R.Cast();
                }
                else
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(10, Usables.CastHydra);
                }
            }

            if (!Spells.W.IsReady() || !BackgroundData.InRange(target))
            {
                return;
            }

            if (MenuConfig.Doublecast && Spells.Q.IsReady() && Qstack != 2)
            {
                BackgroundData.CastW(target);
                BackgroundData.DoubleCastQ(target);
            }
            else
            {
                BackgroundData.CastW(target);
            }
        }

        #endregion
    }
}
