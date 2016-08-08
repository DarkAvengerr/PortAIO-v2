using ItemData = LeagueSharp.Common.Data.ItemData;
using Item = LeagueSharp.Common.Items.Item;

namespace S_Plus_Class_Kalista.Structures
{
    class Items
    {
        public struct Offensive
        {
            public static Item Botrk = new Item(ItemData.Blade_of_the_Ruined_King.GetItem().Id);
            public static Item Cutless = new Item(ItemData.Bilgewater_Cutlass.GetItem().Id);
            public static Item Hydra = new Item(ItemData.Ravenous_Hydra_Melee_Only.GetItem().Id);
            public static Item Tiamat = new Item(ItemData.Tiamat_Melee_Only.GetItem().Id);
            public static Item GunBlade = new Item(ItemData.Hextech_Gunblade.GetItem().Id);
            public static Item Muraman = new Item(ItemData.Muramana.GetItem().Id);
            public static Item GhostBlade = new Item(ItemData.Youmuus_Ghostblade.GetItem().Id);
        }

        public struct Defensive
        {
            public static Item Qss = new Item(ItemData.Quicksilver_Sash.GetItem().Id);
            public static Item Merc = new Item(ItemData.Mercurial_Scimitar.GetItem().Id);
        }

        public struct Trinkets
        {
            public static Item Orb = new Item(3363);
        }

    }
}
