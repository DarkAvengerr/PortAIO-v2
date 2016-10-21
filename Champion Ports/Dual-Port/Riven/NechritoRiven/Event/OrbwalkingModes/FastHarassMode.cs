using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NechritoRiven.Event.OrbwalkingModes
{
    #region

    using Core;

    using LeagueSharp.Common;

    #endregion

    internal class FastHarassMode : Core
    {
        #region Public Methods and Operators

        public static void FastHarass()
        {
            var target = TargetSelector.GetTarget(450 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);

            if (!Spells.E.IsReady())
            {
                return;
            }

            if (!InRange(target))
            {
                Spells.E.Cast(target.Position);
            }

            LeagueSharp.Common.Utility.DelayAction.Add(170, () => CastQ(target));
        }

        #endregion
    }
}
