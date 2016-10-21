using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.KogMow
{
    class KogMow
    {
        static Program Main;
        public KogMow()
        {



            Game.OnUpdate += Game_OnUpdate;
        }

        void Game_OnUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
