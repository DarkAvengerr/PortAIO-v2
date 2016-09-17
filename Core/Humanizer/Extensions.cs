using LeagueSharp.Common;

using EloBuddy;
namespace PortAIOHuman
{
    internal static class Extensions
    {
        public static int TimeSince(this int time)
        {
            return Utils.TickCount - time;
        }
    }
}