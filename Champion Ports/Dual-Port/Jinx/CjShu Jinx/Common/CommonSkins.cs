using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CjShuJinx.Common
{

    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

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
                            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
                        }
                    };

                string[] strSkins = new[] { "Classic Jinx", "Mafia Jinx", "Firecracker Jinx", "Slayer Jinx" };

                MenuLocal.AddItem(new MenuItem("Settings.SkinID", "Skin Name:").SetValue(new StringList(strSkins, 0)));
            }
            MenuParent.AddSubMenu(MenuLocal);

            Game.OnUpdate += GameOnOnUpdate;
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (MenuLocal.Item("Settings.Skin").GetValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, MenuLocal.Item("Settings.SkinID").GetValue<StringList>().SelectedIndex);
            }
        }
    }
}
