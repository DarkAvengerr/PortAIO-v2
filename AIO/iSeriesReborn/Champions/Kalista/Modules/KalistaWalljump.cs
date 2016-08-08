using DZLib.Logging;
using iSeriesReborn.Utility;
using iSeriesReborn.Utility.Geometry;
using iSeriesReborn.Utility.ModuleHelper;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.Champions.Kalista.Modules
{
    class KalistaWalljump : IModule
    {
        private float LastMovementTick = 0f;
        public string GetName()
        {
            return "Kalista_Walljump";
        }

        public ModuleType GetModuleType()
        {
            return ModuleType.OnUpdate;
        }

        public bool ShouldRun()
        {
            return MenuExtensions.GetItemValue<KeyBind>("iseriesr.kalista.misc.walljump").Active;
        }

        public void Run()
        {
            if ((iSRGeometry.IsOverWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                && iSRGeometry.GetWallLength(ObjectManager.Player.ServerPosition, Game.CursorPos) >= (35f)))
            {
                MoveToLimited(iSRGeometry.GetFirstWallPoint(ObjectManager.Player.ServerPosition, Game.CursorPos));
            }
            else
            {
                MoveToLimited(Game.CursorPos);
            }

            var dir = ObjectManager.Player.ServerPosition.LSTo2D() + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * (ObjectManager.Player.BoundingRadius * 1.10f);
            var oppositeDir = ObjectManager.Player.ServerPosition.LSTo2D() + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * -(ObjectManager.Player.BoundingRadius * 2f);
            var Extended = ObjectManager.Player.ServerPosition.LSTo2D() + ObjectManager.Player.Direction.LSTo2D().LSPerpendicular() * (300 + 65f);
            if (dir.LSIsWall() && iSRGeometry.IsOverWall(ObjectManager.Player.ServerPosition, Extended.To3D())
                    && Variables.spells[SpellSlot.Q].LSIsReady()
                    && iSRGeometry.IsOverWall(ObjectManager.Player.ServerPosition, Game.CursorPos)
                    && iSRGeometry.GetWallLength(ObjectManager.Player.ServerPosition, Extended.To3D()) <= (280f - 65f / 2f))
            {
                Variables.spells[SpellSlot.Q].Cast(oppositeDir);
            }
        }

        public void MoveToLimited(Vector3 where)
        {
            if (Game.Time - LastMovementTick < 800f)
            {
                return;
            }
            LastMovementTick = Game.Time;

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, where);
        }
    }
}
