using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Common.Evade.Pathfinding
{
    using LeagueSharp.SDK;
    using SharpDX;
    using System;
    using System.Collections.Generic;

    public static class PathFollower
    {
        public static List<Vector2> Path = new List<Vector2>();

        public static bool IsFollowing
        {
            get
            {
                return Path.Count > 0;
            }
        }

        public static void KeepFollowingPath(EventArgs args)
        {
            if (Path.Count > 0)
            {
                while (Path.Count > 0 && Evade.PlayerPosition.Distance(Path[0]) < 80)
                {
                    Path.RemoveAt(0);
                }
            }
        }

        public static void Follow(List<Vector2> path)
        {
            Path = path;
        }

        public static void Stop()
        {
            Path = new List<Vector2>();
        }
    }
}
