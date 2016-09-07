using LeagueSharp;
using Preserved_Kassadin.Cores;
using Preserved_Kassadin.Update;
using Preserved_Kassadin.Update.Draw;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Preserved_Kassadin
{
    class LoadAssembly
    {
        public static void Load()
        {
            Spells.Load();
            MenuConfig.Load();

            Game.OnUpdate += Mode.Update;
            Game.OnUpdate += Trinket.Update;
            Game.OnUpdate += Skinchanger.Update;
            Game.OnUpdate += Killsteal.Update;

            Drawing.OnDraw += DrawSpells.OnDraw;
            Drawing.OnEndScene += DrawDmg.Draw;

            AssemblyVersion.CheckVersion();

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#00e5e5\">Preserved Kassadin</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded</font></b>");
        }
    }
}
