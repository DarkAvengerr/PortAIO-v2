#region

using System;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Event
{
    internal class SkinChanger : Core.Core
    {
        public static void Update(EventArgs args)
        {
            Skins();
        }
        public static void Skins()
        {
            if (!MenuConfig.UseSkin)
            {
                return;
            }
        }
    }
}
