using EloBuddy; 
using LeagueSharp.Common; 
namespace ADCCOMMON
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    public static class ManaManager
    {
        private static Menu FarmMenu;
        private static Menu drawMenu;
        public static bool SpellFarm;
        public static bool SpellHarass;

        public static bool HasEnoughMana(int manaPercent)
        {
            return ObjectManager.Player.ManaPercent >= manaPercent && !ObjectManager.Player.UnderTurret(true);
        }

        public static void AddSpellFarm(Menu mainMenu)
        {
            FarmMenu = mainMenu;

            mainMenu.AddItem(new MenuItem("SpellFarm", "Use Spell Farm(Mouse Scroll)", true).SetValue(true));
            mainMenu.AddItem(
                    new MenuItem("SpellHarass", "Use Spell Harass(In LaneClear Mode)", true).SetValue(new KeyBind('H',
                        KeyBindType.Toggle, true)))
                .ValueChanged += ManaManager_ValueChanged;

            mainMenu.Item("SpellFarm", true).Permashow(true, "Use Spell Farm");
            mainMenu.Item("SpellHarass", true).Permashow(true, "Use Spell Harass(In LaneClear Mode)");

            SpellFarm = mainMenu.Item("SpellFarm", true).GetValue<bool>();
            SpellHarass = mainMenu.Item("SpellHarass", true).GetValue<KeyBind>().Active;

            Game.OnWndProc += OnWndProc;
        }

        private static void ManaManager_ValueChanged(object obj, OnValueChangeEventArgs Args)
        {
            SpellHarass = Args.GetNewValue<KeyBind>().Active;
        }

        public static void AddDrawFarm(Menu mainMenu)
        {
            drawMenu = mainMenu;

            mainMenu.AddItem(new MenuItem("DrawFarm", "Draw Spell Farm Status", true).SetValue(true));
            mainMenu.AddItem(new MenuItem("DrawHarass", "Draw Spell Harass Status", true).SetValue(true));
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (drawMenu.Item("DrawFarm", true).GetValue<bool>())
                {
                    var MePos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                    Drawing.DrawText(MePos[0] - 57, MePos[1] + 48, Color.FromArgb(242, 120, 34),
                        "Spell Farm:" + (SpellFarm ? "On" : "Off"));
                }

                if (drawMenu.Item("DrawHarass", true).GetValue<bool>())
                {
                    var MePos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                    Drawing.DrawText(MePos[0] - 57, MePos[1] + 68, Color.FromArgb(242, 120, 34),
                        "Spell Hars:" + (SpellHarass ? "On" : "Off"));
                }
            }
        }

        private static void OnWndProc(WndEventArgs Args)
        {
            if (Args.Msg == 0x20a)
            {
                FarmMenu.Item("SpellFarm", true).SetValue(!FarmMenu.Item("SpellFarm", true).GetValue<bool>());
                SpellFarm = FarmMenu.Item("SpellFarm", true).GetValue<bool>();
            }
        }
    }
}
