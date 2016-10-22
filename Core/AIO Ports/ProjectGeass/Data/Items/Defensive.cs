using LeagueSharp.Common.Data;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Data.Items
{

    public class Defensive
    {
        #region Public Properties

        public EloBuddy.SDK.Item Merc{get;} = new EloBuddy.SDK.Item(ItemId.Mercurial_Scimitar);
        public EloBuddy.SDK.Item Qss{get;} = new EloBuddy.SDK.Item(ItemId.Quicksilver_Sash);

        #endregion Public Properties
    }

}