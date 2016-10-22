using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Data.Items
{

    public class Offensive
    {
        #region Public Properties

        public EloBuddy.SDK.Item Botrk{get;} = new EloBuddy.SDK.Item(ItemId.Blade_of_the_Ruined_King);
        public EloBuddy.SDK.Item Cutless{get;} = new EloBuddy.SDK.Item(ItemId.Bilgewater_Cutlass);
        public EloBuddy.SDK.Item GhostBlade{get;} = new EloBuddy.SDK.Item(ItemId.Youmuus_Ghostblade);
        public EloBuddy.SDK.Item GunBlade{get;} = new EloBuddy.SDK.Item(ItemId.Hextech_Gunblade);
        public EloBuddy.SDK.Item Hydra{get;} = new EloBuddy.SDK.Item(ItemId.Ravenous_Hydra_Melee_Only);
        public EloBuddy.SDK.Item Muraman{get;} = new EloBuddy.SDK.Item(ItemId.Muramana);
        public EloBuddy.SDK.Item Tiamat{get;} = new EloBuddy.SDK.Item(ItemId.Tiamat_Melee_Only);

        #endregion Public Properties
    }

}