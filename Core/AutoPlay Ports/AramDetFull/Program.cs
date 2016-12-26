using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; namespace ARAMDetFull
{
    class Program
    {
        public static void Main()
        {
            if (Game.MapId == GameMapId.HowlingAbyss && Game.MapId != GameMapId.SummonersRift)
            {
                EloBuddy.SDK.Core.DelayAction(() => { new ARAMDetFull(); }, 3000);
            }
            else
            {
                //
            }
        }
    }
}
