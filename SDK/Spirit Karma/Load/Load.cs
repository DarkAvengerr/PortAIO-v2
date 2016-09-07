#region

using LeagueSharp;
using LeagueSharp.SDK;
using Spirit_Karma.Core;
using Spirit_Karma.Draw;
using Spirit_Karma.Event;
using Spirit_Karma.Menus;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Spirit_Karma.Load
{
    internal class Load
    {
        public static void LoadAssembly()
        {
            Spells.Load();
            MenuConfig.Load();

            Drawing.OnDraw += DrawMantra.SelectedMantra;
            Drawing.OnEndScene += DrawDmg.OnDrawEnemy;

            Game.OnUpdate += SkinChanger.Update;
            Game.OnUpdate += Mode.OnUpdate;
            Game.OnUpdate += Trinkets.Update;

            
            AssemblyVersion.CheckVersion();

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Spirit Karma</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded Sucessfully</font></b>");
        }
    }
}
