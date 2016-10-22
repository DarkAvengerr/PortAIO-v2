using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Dark_Star_Thresh
{
    using Dark_Star_Thresh.Core;
    using Dark_Star_Thresh.Drawings;
    using Dark_Star_Thresh.Update;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Load
    {
        public static void LoadAssembly()
        {
            MenuConfig.LoadMenu();
            Spells.Load();

            TacticalMap.OnPing += Mode.OnPing;
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
