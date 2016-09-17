using LeagueSharp.Common;
using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KindredSpirits
{
    internal static class ItemManager
    {
        public static Items.Item Botrk
        {
            get { return LeagueSharp.Common.Data.ItemData.Blade_of_the_Ruined_King.GetItem(); }
        }

        public static Items.Item Cutlass
        {
            get { return LeagueSharp.Common.Data.ItemData.Bilgewater_Cutlass.GetItem(); }
        }

        public static Items.Item Youmuus
        {
            get { return LeagueSharp.Common.Data.ItemData.Youmuus_Ghostblade.GetItem(); }
        }

        public static bool IsValidAndReady(this Items.Item item)
        {
            return item != null && item.IsReady();
        }
    }
}