using Item = LeagueSharp.Common.Items.Item;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Data.Items
{
    public static class Defensive
    {
        public static Item Qss { get; } = new Item(ItemData.Quicksilver_Sash.GetItem().Id);
        public static Item Merc { get; } = new Item(ItemData.Mercurial_Scimitar.GetItem().Id);
    }
}