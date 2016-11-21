using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    internal class BrandModes : Modes
    {
  

        public override void Combo(Core core)
        {
            var useQ = core.GetMenu.GetMenu.Item("CQ").GetValue<bool>();
            var useW = core.GetMenu.GetMenu.Item("CW").GetValue<bool>();
            var useE = core.GetMenu.GetMenu.Item("CE").GetValue<bool>();
            var useR = core.GetMenu.GetMenu.Item("CR").GetValue<bool>();
            if (useQ) core.GetSpells.castQ(core);
            if (useW) core.GetSpells.castW(core);
            if (useE) core.GetSpells.castE(core);
            if (useR) core.GetSpells.castR(core);
        }
    }
}
