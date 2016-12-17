using EloBuddy; 
using LeagueSharp.Common; 
namespace Flowers_Darius.Manager.Menu
{
    using SharpDX;
    using LeagueSharp.Common;
    using FlowersDariusCommon;
    

    internal class MenuManager : Logic
    {
        private static readonly Color menuColor = new Color(3, 253, 241);

        internal static void Init()
        {
            Menu =
                new Menu("Flowers' Darius", "Flowers' Darius", true).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);

            var targetMenu = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            {
                TargetSelector.AddToMenu(targetMenu);
            }

            var orbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            {
                comboMenu.AddItem(new MenuItem("ComboQ", "Use Q", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                comboMenu.AddItem(
                        new MenuItem("ComboR", "Use R", true).SetValue(new KeyBind('G', KeyBindType.Toggle, true)))
                    .Permashow();
                comboMenu.AddItem(new MenuItem("ComboSaveMana", "Save Mana to Cast R", true).SetValue(true));
                comboMenu.AddItem(new MenuItem("ComboIgnite", "Use Ignite", true).SetValue(true));
            }

            var harassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu.AddItem(new MenuItem("HarassQ", "Use Q", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                harassMenu.AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                harassMenu.AddItem(
                    new MenuItem("HarassMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
            }

            var clearMenu = Menu.AddSubMenu(new Menu("Clear", "Clear"));
            {
                var laneClearMenu = clearMenu.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                {
                    laneClearMenu.AddItem(new MenuItem("LaneClearQ", "Use Q", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearQCount", "Use Q| Minions Hit >= x", true).SetValue(new Slider(3, 1, 5)));
                    laneClearMenu.AddItem(new MenuItem("LaneClearW", "Use W", true).SetValue(true));
                    laneClearMenu.AddItem(
                        new MenuItem("LaneClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                var jungleClearMenu = clearMenu.AddSubMenu(new Menu("JungleClear", "JungleClear"));
                {
                    jungleClearMenu.AddItem(new MenuItem("JungleClearQ", "Use Q", true).SetValue(true));
                    jungleClearMenu.AddItem(new MenuItem("JungleClearW", "Use W", true).SetValue(true));
                    jungleClearMenu.AddItem(
                        new MenuItem("JungleClearMana", "When Player ManaPercent >= x%", true).SetValue(new Slider(60)));
                }

                clearMenu.AddItem(new MenuItem("asdqweqwe", " ", true));
                ManaManager.AddSpellFarm(clearMenu);
            }

            var killStealMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                killStealMenu.AddItem(new MenuItem("KillStealQ", "Use Q", true).SetValue(true));
                killStealMenu.AddItem(new MenuItem("KillStealR", "Use R", true).SetValue(true));
                killStealMenu.AddItem(
                    new MenuItem("KillStealRNotCombo", "Use R| Dont work in Combo Mode", true).SetValue(true));
            }

            var miscMenu = Menu.AddSubMenu(new Menu("Misc", "Misc"));
            {
                var qMenu = miscMenu.AddSubMenu(new Menu("Q Settings", "Q Settings"));
                {
                    qMenu.AddItem(new MenuItem("ComboQLock", "Lock Q Logic(Active in Combo Mode)", true).SetValue(true));
                    qMenu.AddItem(new MenuItem("HarassQLock", "Lock Q Logic(Active in Harass Mode)", true).SetValue(true));
                }

                var skinMenu = miscMenu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
                {
                    SkinManager.AddToMenu(skinMenu, 13);
                }
            }

            var drawMenu = Menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawE", "Draw E Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawR", "Draw R Range", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("DrawRStatus", "Draw R Status", true).SetValue(true));
                DamageIndicator.AddToMenu(drawMenu);
            }

            Menu.AddItem(new MenuItem("asdvre1w56", "  "));
            Menu.AddItem(new MenuItem("Credit", "Credit : NightMoon")).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);
            Menu.AddItem(new MenuItem("Version", "Version : 1.0.0.0")).SetFontStyle(
                    System.Drawing.FontStyle.Regular, menuColor);

            Menu.AddToMainMenu();
        }
    }
}
