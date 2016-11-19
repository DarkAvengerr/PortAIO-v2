using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Library.Dash_Handler
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal sealed class DashPosition
    {
        public Vector3 DashEndPosition(Obj_AI_Base target, float range)
        {
            return ObjectManager.Player.Position.Extend(target.Position, range).To2D().To3D();
        }
    }
}
