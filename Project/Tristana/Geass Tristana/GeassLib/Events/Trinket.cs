using System;
using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Events
{
    class Trinket
    {
        public const string MenuItemBase = ".Trinket.";
        public const string MenuNameBase = ".Trinket Menu";

        public Trinket()
        {
            if (!DelayHandler.Loaded) DelayHandler.Load();
            Game.OnUpdate += OnUpdate;
        }
        void OnUpdate(EventArgs args)
        {
            if (DelayHandler.CheckTrinket())
            {
                if (!Globals.Objects.GeassLibMenu.Item(Menus.Names.TrinketItemBase + "Boolean.BuyOrb").GetValue<bool>()) return;
                DelayHandler.UseTrinket();

                if (Globals.Objects.Player.Level < 9) return;
                if (!Globals.Objects.Player.InShop() || LeagueSharp.Common.Items.HasItem(Data.Items.Trinkets.Orb.Id))
                    return;

                Globals.Objects.Logger.WriteLog("Buy Orb");
                Data.Items.Trinkets.Orb.Buy();
            }
        }
        
    }
}
