using LeagueSharp.Common;

using EloBuddy; namespace HumanizerSharp
{
    internal static class Extensions
    {
        public static int TimeSince(this int time)
        {
            return Utils.TickCount - time;
        }
    }
}