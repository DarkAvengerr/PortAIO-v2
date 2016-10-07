using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RivenToTheChallenger
{
    static class Activator
    {
        public static Items.Item Youmuus_Ghostblade { get; } // = ItemData.Youmuus_Ghostblade.GetItem();
        public static Items.Item Tiamat_Melee_Only { get; }//= ItemData.Tiamat_Melee_Only.GetItem();
        public static Items.Item Ravenous_Hydra_Melee_Only { get; }//= ItemData.Ravenous_Hydra_Melee_Only.GetItem();

        static Activator()
        {
            Youmuus_Ghostblade = ItemData.Youmuus_Ghostblade.GetItem();
            Tiamat_Melee_Only = ItemData.Tiamat_Melee_Only.GetItem();
            Ravenous_Hydra_Melee_Only = ItemData.Ravenous_Hydra_Melee_Only.GetItem();
        }

    }
}