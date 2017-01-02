using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Yasuo.Manager.Events
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    internal class DashReset : Logic
    {
        internal static void Init()
        {
            if (Utils.TickCount - lastECast > 500)
            {
                isDashing = false;
                lastEPos = Vector3.Zero;
            }
        }
    }
}