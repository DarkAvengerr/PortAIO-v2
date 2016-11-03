using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Utils;
using SharpDX;
using Bootstrap = ChallengerYi.Backbone.Bootstrap;
using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.SDK;
namespace ChallengerYi
{
    internal static class Program
    {
        public static void Main()
        {
            if (ObjectManager.Player.ChampionName.ToLower().Contains("yi"))
            {
                DelayAction.Add(1250, Bootstrap.Start);
            }
        }
    }
}
