using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Entities;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Skills
{
    class VayneE
    {
        public static void HandleELogic()
        {
            if (Variables.spells[SpellSlot.E].IsEnabledAndReady())
            {
                if (MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.qe") 
                    && Variables.spells[SpellSlot.E].LSIsReady() 
                    && Variables.spells[SpellSlot.Q].LSIsReady())
                {
                    const int currentStep = 30;
                    var direction = ObjectManager.Player.Direction.LSTo2D().LSPerpendicular();
                    for (var i = 0f; i < 360f; i += currentStep)
                    {
                        var angleRad = Geometry.DegreeToRadian(i);
                        var rotatedPosition = ObjectManager.Player.Position.LSTo2D() + (300f * direction.LSRotated(angleRad));
                        ECheck(rotatedPosition.To3D(), true);
                    }
                }
                else
                {
                    ECheck(ObjectManager.Player.ServerPosition);
                }
            }
        }

        public static void ECheck(Vector3 from, bool castQFirst = false)
        {
            if (!Variables.spells[SpellSlot.E].LSIsReady())
            {
                return;
            }

            var pushDistance = MenuExtensions.GetItemValue<Slider>("iseriesr.vayne.misc.condemn.pushdist").Value;
            var accuracy = MenuExtensions.GetItemValue<Slider>("iseriesr.vayne.misc.condemn.acc").Value / 100f;

            if (pushDistance >= 410)
            {
                var PushEx = pushDistance;
                pushDistance -= (10 + (PushEx - 410) / 2);
            }
            
            foreach (var target in GameObjects.EnemyHeroes.Where(h => !h.IsInvulnerable && !TargetSelector.IsInvulnerable(h, TargetSelector.DamageType.Physical, false) && h.LSIsValidTarget()))
            {
                
                var targetPosition = Variables.spells[SpellSlot.E].GetPrediction(target).UnitPosition;

                var finalPosition = targetPosition.LSExtend(from, -pushDistance);
                var finalPosition_ex = target.ServerPosition.LSExtend(from, -pushDistance);

                var condemnRectangle = new iSRPolygon(iSRPolygon.Rectangle(targetPosition.LSTo2D(), finalPosition.LSTo2D(), target.BoundingRadius));
                var condemnRectangle_ex = new iSRPolygon(iSRPolygon.Rectangle(target.ServerPosition.LSTo2D(), finalPosition_ex.LSTo2D(), target.BoundingRadius));

                if (IsBothNearWall(target))
                {
                    return;
                }

                if (condemnRectangle.Points.Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle.Points.Count() * accuracy
                    && condemnRectangle_ex.Points.Count(point => NavMesh.GetCollisionFlags(point.X, point.Y).HasFlag(CollisionFlags.Wall)) >= condemnRectangle_ex.Points.Count() * accuracy)
                {
                    if (castQFirst && VayneQ.IsSafe(from))
                    {
                        Variables.spells[SpellSlot.Q].Cast(from);
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(250 + Game.Ping / 2f + 125), () =>
                        {
                            WardBush(from, targetPosition.LSExtend(from, -pushDistance));
                            Variables.spells[SpellSlot.E].Cast(target);
                        });
                        return;
                    }

                    WardBush(from, targetPosition.LSExtend(from, -pushDistance));
                    Variables.spells[SpellSlot.E].Cast(target);
                    return;
                }
            }
        }

        private static void WardBush(Vector3 from, Vector3 endPosition)
        {
            if (!MenuExtensions.GetItemValue<bool>("iseriesr.vayne.misc.condemn.wardbush"))
            {
                return;
            }

            var wardSlot = Items.GetWardSlot();
            if (wardSlot != null)
            {
                var wardPos = from.LSExtend(endPosition, from.LSDistance(endPosition) - 65f);
                if (NavMesh.IsWallOfGrass(wardPos, 65))
                {
                    if (Items.CanUseItem(wardSlot.Slot))
                    {
                        Items.UseItem(wardSlot.Slot, wardPos);
                    }
                }
            }
        }
        
        private static bool IsBothNearWall(Obj_AI_Base target)
        {
            var positions =
                GetWallQPositions(target, 110).ToList().OrderBy(pos => pos.LSDistance(target.ServerPosition, true));
            var positions_ex =
            GetWallQPositions(ObjectManager.Player, 70).ToList().OrderBy(pos => pos.LSDistance(ObjectManager.Player.ServerPosition, true));

            if (positions.Any(p => p.LSIsWall()))
            {
                return true;
            }
            return false;
        }

        private static Vector3[] GetWallQPositions(Obj_AI_Base player,float Range)
        {
            Vector3[] vList =
            {
                (player.ServerPosition.LSTo2D() + Range * player.Direction.LSTo2D()).To3D(),
                (player.ServerPosition.LSTo2D() - Range * player.Direction.LSTo2D()).To3D()

            };

            return vList;
        }
    }
}
