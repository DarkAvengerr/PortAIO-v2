using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Ekko_master_of_time
{
    class Modes
    {
        public virtual void Update(Core ekko)
        {


            switch (ekko.Menu.Orb.ActiveMode)
            {
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo:
                    Combo(ekko);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed:
                    Harash(ekko);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear(ekko);
                    Jungleclear(ekko);
                    break;
            }


        }
        public virtual void Jungleclear(Core ekko)
        {

        }

        public virtual void Laneclear(Core ekko)
        {

        }

        public virtual void Harash(Core ekko)
        {

        }

        public virtual void Combo(Core ekko)
        {



        }
    }
}
