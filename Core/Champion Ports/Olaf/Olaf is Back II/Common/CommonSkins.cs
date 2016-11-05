using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Olaf.Common
{
    internal class CommonSkins
    {
        public static Menu MenuLocal { get; private set; }

        public static void Init(Menu MenuParent)
        {
            MenuLocal = new Menu("Skins", "MenuSkin");
            {
                MenuLocal.AddItem(new MenuItem("Settings.Skin", "Skin:").SetValue(false)).ValueChanged +=
                    (sender, args) =>
                    {
                        if (!args.GetNewValue<bool>())
                        {
                        }
                    };

                string[] strSkins = new[] { "Classic", "Forsaken Olaf", "Glacial Olaf", "Brolaf", "Pentakill Olaf", "Marauder Olaf" };

                MenuLocal.AddItem(new MenuItem("Settings.SkinID", "Skin Name:").SetValue(new StringList(strSkins, 0)));
            }
            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (MenuLocal.Item("Settings.Skin").GetValue<bool>())
            {
            }
        }
    }
}
