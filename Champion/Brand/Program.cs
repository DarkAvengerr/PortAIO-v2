using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LeagueSharp.Common;

namespace TheBrand
{
    class Program
    {
        public static void Main()
        {
            CustomEvents.Game.OnGameLoad += new Brand().Load;
        }
    }
}
