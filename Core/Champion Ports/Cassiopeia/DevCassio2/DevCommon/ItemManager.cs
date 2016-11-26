using EloBuddy; 
using LeagueSharp.Common; 
 namespace DevCommom2
{
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    public class ItemManager
    {
        private readonly List<ItemDTO> ItemDTOList;

        public ItemManager()
        {
            ItemDTOList = new List<ItemDTO>();
            InitiliazeItemList();
        }

        public bool IsItemReady(ItemName pItemName)
        {
            return ItemDTOList.First(x => x.ItemName == pItemName).Item.IsReady();
        }

        public void UseItem(ItemName pItemName, AIHeroClient target = null)
        {
            var item = ItemDTOList.First(x => x.ItemName == pItemName).Item;

            if (!item.IsReady())
            {
                return;
            }

            if (target == null)
            {
                item.Cast();
            }
            else
            {
                item.Cast(target);
            }
        }

        private void InitiliazeItemList()
        {
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3144, 450),
                ItemName = ItemName.BilgewaterCutlass
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3030, 700),
                ItemName = ItemName.HextechGLP800
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3153, 450),
                ItemName = ItemName.BladeOfTheRuineKing
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3146, 700),
                ItemName = ItemName.HextechGunblade
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3042, int.MaxValue),
                ItemName = ItemName.Muramana
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3074, 400),
                ItemName = ItemName.RavenousHydra
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3077, 400),
                ItemName = ItemName.Tiamat
            });
            ItemDTOList.Add(new ItemDTO
            {
                Item = new Items.Item(3142, (int)(ObjectManager.Player.AttackRange * 2)),
                ItemName = ItemName.YoumuusGhostblade
            });
        }
    }

    public class ItemDTO
    {
        public Items.Item Item { get; set; }
        public ItemName ItemName { get; set; }
    }

    public enum ItemName
    {
        HextechGLP800,
        BilgewaterCutlass,
        BlackfireTorch,
        BladeOfTheRuineKing,
        DeathfireGrasp,
        HextechGunblade,
        Muramana,
        RavenousHydra,
        Tiamat,
        YoumuusGhostblade,
    }
}
