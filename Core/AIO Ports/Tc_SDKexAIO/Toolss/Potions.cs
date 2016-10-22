using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Toolss
{
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using System;
    using System.Linq;

    using Config;
    using Common;

    using Menu = LeagueSharp.SDK.UI.Menu;

    using System.Collections.Generic;

    internal static class Potions
    {

        private static AIHeroClient Player => PlaySharp.Player;
        private static Menu Menu => Tools.Menu;

        public static void Init()
        {
            var PotionsMenu = Menu.Add(new Menu("Potions", "Potions"));
            {
                PotionsMenu.GetSeparator("Health Potion");
                PotionsMenu.GetSliderButton("HealthPotion", "Player Health = ", 50, 35, 80, false);
                PotionsMenu.GetSeparator("Corrupting Potion");
                PotionsMenu.GetSliderButton("CorruptingPotion", "Player Health = ", 50, 35, 80, false);
                PotionsMenu.GetSeparator("Refillable Potion");
                PotionsMenu.GetSliderButton("RefillablePotion", "Player Health = ", 50, 35, 80, false);
                PotionsMenu.GetSeparator("Hunter's Potion");
                PotionsMenu.GetSliderButton("HuntersPotion", "Player Health = ", 50, 35, 80, false);
                PotionsMenu.GetSeparator("Biscuit Mobe");
                PotionsMenu.GetSliderButton("Biscuit", "Player Health = ", 50, 35, 80, false);
                PotionsMenu.GetSeparator("       ");
                PotionsMenu.Add(new MenuBool("Enable", "Enable", true));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.InFountain() || Player.Buffs.Any(x => x.Name.Contains("Recall") || x.Name.Contains("Teleport")))
            {
                return;
            }

            if (Menu["Potions"]["Enable"] && Player.Buffs.Any(x => x.Name.Equals("ItemCrystalFlask", StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals("ItemCrystalFlaskJungle", StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals("ItemDarkCrystalFlask", StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals("RegenerationPotion", StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase)
                || x.Name.Equals("ItemMiniRegenPotion", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            if (Menu["Potions"]["HealthPotion"].GetValue<MenuSliderButton>().SValue >= Player.HealthPercent)
            {
                CastHpPotion();
            }

            if (Menu["Potions"]["CorruptingPotion"].GetValue<MenuSliderButton>().SValue >= Player.HealthPercent)
            {
                CastCorrupting();
            }

            if (Menu["Potions"]["RefillablePotion"].GetValue<MenuSliderButton>().SValue >= Player.HealthPercent)
            {
                CastRefillable();
            }

            if (Menu["Potions"]["HuntersPotion"].GetValue<MenuSliderButton>().SValue >= Player.HealthPercent)
            {
                CastHunter();
            }

            if (Menu["Potions"]["Biscuit"].GetValue<MenuSliderButton>().SValue >= Player.HealthPercent)
            {
                CastBiscuit();
                CastBiscuit2();
            }
        }

        #region Item

        private static void CastHpPotion()
        {
            if (Items.HasItem(2003))
                Items.UseItem(2003);
        }

        private static void CastBiscuit()
        {
            if (Items.HasItem(2009))
                Items.UseItem(2009);
        }

        private static void CastBiscuit2()
        {
            if (Items.HasItem(2010))
                Items.UseItem(2010);
        }

        private static void CastRefillable()
        {
            if (Items.HasItem(2031))
                Items.UseItem(2031);
        }

        private static void CastHunter()
        {
            if (Items.HasItem(2032))
                Items.UseItem(2032);
        }

        private static void CastCorrupting()
        {
            if (Items.HasItem(2033))
                Items.UseItem(2033);
        }

        #endregion
    }
}