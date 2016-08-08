using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using VayneHunter_Reborn.Utility;
using VayneHunter_Reborn.Utility.Helpers;
using VayneHunter_Reborn.Utility.MenuUtility;
using EloBuddy;

namespace VayneHunter_Reborn.Skills.Condemn.Methods
{
    class Marksman
    {
        public static AIHeroClient GetTarget(Vector3 fromPosition)
        {
            foreach (var target in HeroManager.Enemies.Where(h => h.LSIsValidTarget(Variables.spells[SpellSlot.E].Range)))
            {
                var pushDistance = MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.pushdistance").Value;

                var targetPosition = Vector3.Zero;

                var pred = Variables.spells[SpellSlot.E].GetPrediction(target);

                if (pred.Hitchance > HitChance.Impossible)
                {
                    targetPosition = pred.UnitPosition;
                }

                if (targetPosition == Vector3.Zero)
                {
                    return null;
                }

                var finalPosition = targetPosition.LSExtend(fromPosition, -pushDistance);
                var finalPosition2 = targetPosition.LSExtend(fromPosition, -(pushDistance / 2f));
                var underTurret = MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.condemnturret") && (finalPosition.LSUnderTurret(true));
                var j4Flag = MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.condemnflag") && (finalPosition.IsJ4Flag(target) || finalPosition2.IsJ4Flag(target));
                if (finalPosition.LSIsWall() || finalPosition2.LSIsWall() || underTurret || j4Flag)
                {
                    if (MenuExtensions.GetItemValue<bool>("dz191.vhr.misc.condemn.onlystuncurrent") && Variables.Orbwalker.GetTarget() != null &&
                            !target.NetworkId.Equals(Variables.Orbwalker.GetTarget().NetworkId))
                    {
                        return null;
                    }

                    if (target.Health + 10 <=
                            ObjectManager.Player.LSGetAutoAttackDamage(target) *
                            MenuExtensions.GetItemValue<Slider>("dz191.vhr.misc.condemn.noeaa").Value)
                    {
                        return null;
                    }

                    return target;
                }
            }
            return null;
        }
    }
}
