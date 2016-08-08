using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace DZBard
{
    class Program
    {
        public static void Game_OnGameLoad()
        {
            BardBootstrap.OnLoad();
        }
    }
}
