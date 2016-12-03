using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    public static class ManaManager
    {
        private static Menu FarmMenu;
        private static Menu DrawMenu;
        public static bool SpellFarm;

        public static bool HasEnoughMana(int manaPercent)
        {
            return ObjectManager.Player.ManaPercent >= manaPercent;
        }

        public static void AddSpellFarm(Menu mainMenu)
        {
            FarmMenu = mainMenu;

            mainMenu.AddItem(new MenuItem("SpellFarm", "Use Spell Farm(Mouse Scroll)", true).SetValue(true))
                .Permashow(true, "Use Spell Farm");

            SpellFarm = mainMenu.Item("SpellFarm", true).GetValue<bool>();

            Game.OnWndProc += OnWndProc;
        }

        public static void AddDrawFarm(Menu mainMenu)
        {
            DrawMenu = mainMenu;

            mainMenu.AddItem(new MenuItem("DrawFarm", "Draw Spell Farm Status", true).SetValue(true));

            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead && !Shop.IsOpen && !MenuGUI.IsChatOpen  )
            {
                if (DrawMenu.Item("DrawFarm", true).GetValue<bool>())
                {
                    var MePos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                    Drawing.DrawText(MePos[0] - 57, MePos[1] + 48, Color.FromArgb(242, 120, 34),
                        "Spell Farm:" + (SpellFarm ? "On" : "Off"));
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
