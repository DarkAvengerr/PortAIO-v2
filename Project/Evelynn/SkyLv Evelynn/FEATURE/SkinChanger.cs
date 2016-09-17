using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;

    internal class SkinChanger
    {

        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Evelynn.Player;
            }
        }
        #endregion


        static SkinChanger()
        {
            //Menu
            SkyLv_Evelynn.Menu.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Evelynn.SkinChanger", "Use Skin Changer").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("Evelynn.SkinChangerName", "Skin choice").SetValue(new StringList(new[] 
            { "Original", "Shadow", "Masquerade", "Tango", "Safecracker" })));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        }

        
    }
}
