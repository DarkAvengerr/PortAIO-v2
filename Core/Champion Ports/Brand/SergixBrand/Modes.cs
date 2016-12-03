using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace SergixBrand
{
    public class Modes
    {
        public virtual void Update(Core core)
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
        public virtual void Keys(Core core)
        {

        }
        public virtual void Jungleclear(Core core)
        {
        }

        public virtual void Laneclear(Core core)
        {
        }
        public virtual void LastHit(Core core)
        {

        }
        public virtual void Harash(Core core)
        {
        }

        public virtual void Combo(Core core)
        {
        }
    }
}

