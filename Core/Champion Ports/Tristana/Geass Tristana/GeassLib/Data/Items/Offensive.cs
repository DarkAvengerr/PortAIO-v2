using Item = LeagueSharp.Common.Items.Item;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Data.Items
{
    public static class Offensive
    {
        public static Item Botrk { get; } = new Item(ItemData.Blade_of_the_Ruined_King.GetItem().Id);
        public static Item Cutless { get; } = new Item(ItemData.Bilgewater_Cutlass.GetItem().Id);
        public static Item Hydra { get; } = new Item(ItemData.Ravenous_Hydra_Melee_Only.GetItem().Id);
        public static Item Tiamat { get; } = new Item(ItemData.Tiamat_Melee_Only.GetItem().Id);
        public static Item GunBlade { get; } = new Item(ItemData.Hextech_Gunblade.GetItem().Id);
        public static Item Muraman { get; } = new Item(ItemData.Muramana.GetItem().Id);
        public static Item GhostBlade { get; } = new Item(ItemData.Youmuus_Ghostblade.GetItem().Id);
    }
}