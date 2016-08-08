using System.Linq;
using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Geometry;
using iSeriesReborn.Utility.MenuUtility;
using iSeriesReborn.Utility.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Vayne.Skills
{
    class VayneQ
    {
        public static void HandleQLogic(Obj_AI_Base target)
        {
            if (Variables.spells[SpellSlot.Q].IsEnabledAndReady() && target.LSIsValidTarget())
            {
                var qEndPosition = ObjectManager.Player.ServerPosition.LSExtend(Game.CursorPos, 325f);

                if (!IsSafe(qEndPosition))
                {
                    return;
                }

                var qBurstModePosition = GetQBurstModePosition();
                if (qBurstModePosition != null && IsSafe((Vector3) qBurstModePosition))
                {
                    CastQ((Vector3) qBurstModePosition);
                }
                else
                {
                    CastQ(qEndPosition);
                }
            }
        }

        private static void CastQ(Vector3 position)
        {
            if (Variables.spells[SpellSlot.R].IsEnabledAndReady() &&
                PositioningVariables.EnemiesClose.Count() >=
                MenuExtensions.GetItemValue<Slider>("iseriesr.vayne.combo.r.minen").Value)
            {
                Variables.spells[SpellSlot.R].Cast();
            }

            LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
            Variables.spells[SpellSlot.Q].Cast(position);
        }

        private static Vector3? GetQBurstModePosition()
        {
            var positions =
                GetWallQPositions(70).ToList().OrderBy(pos => pos.LSDistance(ObjectManager.Player.ServerPosition, true));

            foreach (var position in positions)
            {
                if (position.LSIsWall() && IsSafe(position))
                {
                    return position;
                }
            }
            
            return null;
        }

        private static Vector3[] GetWallQPositions(float Range)
        {
            Vector3[] vList =
            {
                (ObjectManager.Player.ServerPosition.LSTo2D() + Range * ObjectManager.Player.Direction.LSTo2D()).To3D(),
                (ObjectManager.Player.ServerPosition.LSTo2D() - Range * ObjectManager.Player.Direction.LSTo2D()).To3D()

            };
            
            return vList;
        }

        public static bool IsSafe(Vector3 Position)
        {
            if((Position.LSUnderTurret(true) && !ObjectManager.Player.LSUnderTurret(true)) || (PositioningVariables.EnemiesClose.Count() > 1 && iSRGeometry.GetEnemyPoints().Contains(Position.LSTo2D())))
            {
                return false;
            }

            return true;
        }
    }
}
