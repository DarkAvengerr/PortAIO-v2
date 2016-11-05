using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KoreanOlaf
{
    using LeagueSharp;

    using ItemData = LeagueSharp.Common.Data.ItemData;

    class OlafOffensiveItems
    {
        private readonly OlafMenu olafMenu;

        public OlafOffensiveItems(OlafMenu menu)
        {
            olafMenu = menu;
        }

        public void UseItems(AIHeroClient target)
        {
            UseYomuusBlade();
            UseBilgewaterCutlass(target);
            UseBOTRK(target);
            UseHydra();
            UseTiamat();
        }

        public void UseItemsLaneClear()
        {
            UseHydraLaneClear();
            UseTiamatLaneClear();
        }

        public void UseHarasItems()
        {
            UseHydraHaras();
            UseTiamatHaras();
        }

        private void UseBilgewaterCutlass(AIHeroClient target)
        {
            if (olafMenu.GetParamBool("koreanolaf.combomenu.items.bilgewater")
                && ItemData.Bilgewater_Cutlass.GetItem().IsInRange(target)
                && ItemData.Bilgewater_Cutlass.GetItem().IsReady())
            {
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
            }
        }

        private void UseBOTRK(AIHeroClient target)
        {
            if (olafMenu.GetParamBool("koreanolaf.combomenu.items.botrk")
                && ItemData.Blade_of_the_Ruined_King.GetItem().IsInRange(target)
                && ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
            {
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            }
        }

        private void UseYomuusBlade()
        {
            if (olafMenu.GetParamBool("koreanolaf.combomenu.items.yomuus")
                && ItemData.Youmuus_Ghostblade.GetItem().IsReady())
            {
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
            }
        }

        private void UseHydra()
        {
            if (olafMenu.GetParamBool("koreanolaf.combomenu.items.hydra")
                && ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            }
        }

        private void UseTiamat()
        {
            if (olafMenu.GetParamBool("koreanolaf.combomenu.items.tiamat")
                && ItemData.Tiamat_Melee_Only.GetItem().IsReady())
            {
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            }
        }

        private void UseHydraLaneClear()
        {
            if (olafMenu.GetParamBool("koreanolaf.laneclearmenu.items.hydra")
                && ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            }
        }

        private void UseTiamatLaneClear()
        {
            if (olafMenu.GetParamBool("koreanolaf.laneclearmenu.items.tiamat")
                && ItemData.Tiamat_Melee_Only.GetItem().IsReady())
            {
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            }
        }

        private void UseHydraHaras()
        {
            if (olafMenu.GetParamBool("koreanolaf.harasmenu.items.hydra")
                && ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            }
        }

        private void UseTiamatHaras()
        {
            if (olafMenu.GetParamBool("koreanolaf.harasmenu.items.tiamat")
                && ItemData.Tiamat_Melee_Only.GetItem().IsReady())
            {
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            }
        }
    }
}