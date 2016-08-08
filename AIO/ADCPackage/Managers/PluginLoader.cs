using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCPackage.Plugins;
using LeagueSharp;

using EloBuddy; namespace ADCPackage
{
    class PluginLoader
    {
        public static string Champname = ObjectManager.Player.ChampionName;

        public static void Load()
        {
            switch (Champname)
            {
                case "Jinx":
                    Jinx.Load();
                    return;
                case "Tristana":
                    Tristana.Load();
                    return;
                case "Corki":
                    Corki.Load();
                    return;
            }
            Chat.Print("[<font color='#F8F46D'>ADC Package</font>] by <font color='#79BAEC'>God</font> - <font color='#FFFFFF'>" + Champname + " not supported</font>");
        }
    }
}
