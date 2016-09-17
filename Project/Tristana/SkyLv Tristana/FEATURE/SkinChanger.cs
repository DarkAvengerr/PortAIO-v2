using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class SkinChanger
    {

        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }
        #endregion


        static SkinChanger()
        {
            //Menu
            SkyLv_Tristana.Menu.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            SkyLv_Tristana.Menu.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Tristana.SkinChanger", "Use Skin Changer").SetValue(false));
            SkyLv_Tristana.Menu.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Tristana.SkinChangerName", "Skin choice").SetValue(new StringList(new[] 
            { "Original", "Riot Girl", "Earnest Elf", "Firefighter", "Guerilla", "Buccaneer", "Rocketeer", "Chroma 1", "Chroma 2", "Chroma 3", "Dragon Trainer", })));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        }

        
    }
}
