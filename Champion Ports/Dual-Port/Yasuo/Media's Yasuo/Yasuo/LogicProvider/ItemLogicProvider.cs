using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.Yasuo.LogicProvider
{
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    class ItemLogicProvider
    {
        public Items.Item Tiamat, Hydra, Shiv, InfinityEdge, TrinityForce, Sheen;

        public ItemLogicProvider()
        {
            this.SetItems();
        }

        private void SetItems()
        {
            this.Tiamat = ItemData.Tiamat_Melee_Only.GetItem();
            this.Hydra = ItemData.Ravenous_Hydra_Melee_Only.GetItem();
            this.Sheen = ItemData.Sheen.GetItem();
            this.TrinityForce = ItemData.Trinity_Force.GetItem();
            this.Shiv = ItemData.Statikk_Shiv.GetItem();
            this.InfinityEdge = ItemData.Infinity_Edge.GetItem();
        }

        public void HasItem(ItemData itemData)
        {
            
        }
    }
}
