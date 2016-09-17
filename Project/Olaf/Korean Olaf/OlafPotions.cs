using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    class OlafPotions
    {
        private readonly OlafMenu olafMenu;

        public OlafPotions(OlafMenu olafMenu)
        {
            this.olafMenu = olafMenu;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (olafMenu.GetParamBool("koreanolaf.miscmenu.pot.healthactive")
                && ObjectManager.Player.HealthPercent
                < olafMenu.GetParamSlider("koreanolaf.miscmenu.pot.healthwhen")
                && !ObjectManager.Player.HasBuff("RegenerationPotion")
                && !ObjectManager.Player.InShop())
            {
                ItemData.Health_Potion.GetItem().Cast();
            }
        }
    }
}
