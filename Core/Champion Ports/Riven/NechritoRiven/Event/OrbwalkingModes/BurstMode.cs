using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using Core;

    using LeagueSharp.Common;

    #endregion

    internal class BurstMode : Core
    {
        #region Public Methods and Operators

        public static void Burst()
        {
            if (Spells.Flash.IsReady()
                && MenuConfig.AlwaysF)
            {
                var selectedTarget = TargetSelector.GetSelectedTarget();

                if (selectedTarget == null 
                    || !selectedTarget.IsValidTarget(Player.AttackRange + 625)
                    || Player.Distance(selectedTarget.Position) < Player.AttackRange
                    || (MenuConfig.Flash && selectedTarget.Health > Dmg.GetComboDamage(selectedTarget) && !Spells.R.IsReady())
                    || (!MenuConfig.Flash && (!Spells.R.IsReady() || !Spells.W.IsReady())))
                {
                    return;
                }

                Usables.CastYoumoo();
                Spells.E.Cast(selectedTarget.Position);
                Spells.R.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add(170, BackgroundData.FlashW);
            }
            else
            {
                var target = TargetSelector.GetTarget(Player.AttackRange + 360, TargetSelector.DamageType.Physical);

                if (target == null) return;

                if (Spells.R.IsReady() && Spells.R.Instance.Name == IsSecondR && Qstack > 1)
                {
                    var pred = Spells.R.GetPrediction(
                        target,
                        true,
                        collisionable: new[] { CollisionableObjects.YasuoWall });

                    if (pred.Hitchance < HitChance.High)
                    {
                        return;
                    }

                    Spells.R.Cast(pred.CastPosition);
                }

                if (Spells.E.IsReady())
                {
                    Spells.E.Cast(target.Position);
                }

                if (Spells.R.IsReady() && Spells.R.Instance.Name == IsFirstR)
                {
                    Spells.R.Cast();
                }

                if (!Spells.W.IsReady() || !BackgroundData.InRange(target))
                {
                    return;
                }

                BackgroundData.CastW(target);
                BackgroundData.DoubleCastQ(target);
            }
        }

        #endregion
    }
}
