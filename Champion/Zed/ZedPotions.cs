using EloBuddy; namespace KoreanZed
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    class ZedPotions
    {
        private readonly ZedMenu zedMenu;

        public ZedPotions(ZedMenu zedMenu)
        {
            this.zedMenu = zedMenu;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (zedMenu.GetParamBool("koreanzed.miscmenu.pot.active")
                && ObjectManager.Player.HealthPercent
                < zedMenu.GetParamSlider("koreanzed.miscmenu.pot.when")
                && !ObjectManager.Player.LSHasBuff("RegenerationPotion")
                && !ObjectManager.Player.LSInShop())
            {
                ItemData.Health_Potion.GetItem().Cast();
            }
        }
    }
}
