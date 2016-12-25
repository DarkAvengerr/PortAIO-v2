using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
namespace e.Motion_Gangplank
{
    internal class Barrel
    {
        
        public int BarrelAttackTick;
        public int BarrelObjectNetworkID;
        public int GetNetworkID()
        {
            return BarrelObjectNetworkID;
        }
        public Obj_AI_Minion GetBarrel()
        {
            return ObjectManager.GetUnitByNetworkId<Obj_AI_Minion>((uint)BarrelObjectNetworkID);
        }
        public bool CanAANow()
        {
            //Console.WriteLine();
            return Utils.TickCount >= BarrelAttackTick - Program.Player.AttackCastDelay * 1000;
        }

        public bool CanQNow(int delay = 0)
        {
            if (Program.Player.Distance(GetBarrel().Position)<=625 && Helper.GetQTime(GetBarrel().Position) + delay + Utils.TickCount >= BarrelAttackTick + Config.Menu.Item("misc.additionalServerTick").GetValue<Slider>().Value)
            {
                return true;
            }
            return false;
        }

        public Barrel(Obj_AI_Minion barrel)
        {
            BarrelObjectNetworkID = barrel.NetworkId;
            BarrelAttackTick = GetBarrelAttackTick();
        }

        public void ReduceBarrelAttackTick()
        {
            if (Program.Player.Level < 7) BarrelAttackTick -= 2000;
            else if (Program.Player.Level < 13) BarrelAttackTick -= 1000;
            else BarrelAttackTick -= 500;
        }
        private static int GetBarrelAttackTick()
        {
            if (Program.Player.Level < 7) return Utils.TickCount + 4000;
            if (Program.Player.Level < 13) return Utils.TickCount + 2000;
            return Utils.TickCount + 1000;
        }

        
    }
}
