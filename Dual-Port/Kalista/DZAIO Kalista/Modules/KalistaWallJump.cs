using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Helpers.Positioning;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Kalista.Modules
{
    class KalistaWallJump : IModule
    {
        private float LastMovementTick = 0f;

        public void OnLoad()
        {
        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<KeyBind>("dzaio.champion.kalista.kalista.wallJump").Active;
        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            if ((PositioningHelper.DoPositionsCrossWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                && PositioningHelper.GetWallLength(ObjectManager.Player.ServerPosition, Game.CursorPos) >= (35f)))
            {
                MoveToLimited(PositioningHelper.GetFirstWallPoint(ObjectManager.Player.ServerPosition, Game.CursorPos));
            }
            else
            {
                MoveToLimited(Game.CursorPos);
            }

            var directionExtension = ObjectManager.Player.ServerPosition.To2D() + ObjectManager.Player.Direction.To2D().Perpendicular() * (ObjectManager.Player.BoundingRadius * 1.10f);
            var oppositeDirectionExtension = ObjectManager.Player.ServerPosition.To2D() + ObjectManager.Player.Direction.To2D().Perpendicular() * -(ObjectManager.Player.BoundingRadius * 2f);
            var ExtendedPosition = ObjectManager.Player.ServerPosition.To2D() + ObjectManager.Player.Direction.To2D().Perpendicular() * (300 + 65f);

            if (directionExtension.IsWall() && PositioningHelper.DoPositionsCrossWall(ObjectManager.Player.ServerPosition, ExtendedPosition.To3D())
                    && Variables.Spells[SpellSlot.Q].IsReady()
                    && PositioningHelper.DoPositionsCrossWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                    && PositioningHelper.GetWallLength(ObjectManager.Player.ServerPosition, ExtendedPosition.To3D()) <= (280f - 65f / 2f))
            {
                Variables.Spells[SpellSlot.Q].Cast(oppositeDirectionExtension);
            }
        }

        public void MoveToLimited(Vector3 where)
        {
            if (Game.Time - LastMovementTick < 90f)
            {
                return;
            }
            LastMovementTick = Game.Time;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
    }
}
