using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DZAIO_Reborn.Core.ChampionHelper;
using DZAIO_Reborn.Core.MenuHelper;
using DZLib.Core;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.SDK;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Core
{
    class Bootstrap
    {
        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            DZLib.Menu.ModesMenuExtensions.SetBaseName($"dzaio.champion.{ObjectManager.Player.ChampionName.ToLowerInvariant()}");
            DZLib.Logging.LogHelper.OnLoad();
            MenuGenerator.GenerateMenu();
            SPrediction.ConfigMenu.Initialize(Variables.AssemblyMenu,
                $"{ObjectManager.Player.ChampionName}: SPrediction");

            ChampionLoader.LoadChampion();
            DZAntigapcloser.BuildMenu(Variables.AssemblyMenu, $"{ObjectManager.Player.ChampionName}: Antigapcloser","dzaio.antigp");
            
            DZAIO.Init();
        }
    }
}
