using Dark_Star_Thresh.Core;
using Dark_Star_Thresh.Update;
using Dark_Star_Thresh.Drawings;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    class Load
    {
        public static void LoadAssembly()
        {
            Spells.Load();
            MenuConfig.LoadMenu();

            Game.OnUpdate += Misc.Skinchanger;
            Game.OnUpdate += Mode.GetActiveMode;

            Drawing.OnDraw += DrawRange.OnDraw;
            Drawing.OnEndScene += DrawDmg.OnEndScene;

            Interrupter2.OnInterruptableTarget += Misc.OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += Misc.OnEnemyGapcloser;

            Chat.Print("<b><font color=\"#FFFFFF\">[</font></b><b><font color=\"#F52887\">Dark Star Thresh</font></b><b><font color=\"#FFFFFF\">]</font></b><b><font color=\"#FFFFFF\"> Loaded</font></b>");
        }
    }
}
