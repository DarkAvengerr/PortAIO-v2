using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace DarkMage
{
    class Modes
    {
        public virtual void Update(SyndraCore core)
        {

            switch (core.GetMenu.Orb.ActiveMode)
            {
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Combo:
                    Combo(core);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.Mixed:
                    Harash(core);
                    break;
                case LeagueSharp.Common.Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear(core);
                    Jungleclear(core);
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit(core);
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
                default:
                    break;
            }
            Keys(core);
        }
        public virtual void Keys(SyndraCore core)
        {

        }
        public virtual void Jungleclear(SyndraCore core)
        {
        }

        public virtual void Laneclear(SyndraCore core)
        {
        }
        public virtual void LastHit(SyndraCore core)
        {

        }
        public virtual void Harash(SyndraCore core)
        {
        }

        public virtual void Combo(SyndraCore core)
        {
        }
    }
}

