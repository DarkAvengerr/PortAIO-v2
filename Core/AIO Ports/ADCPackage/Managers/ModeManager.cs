using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCPackage.Plugins;
using LeagueSharp.Common;

using EloBuddy; namespace ADCPackage
{
    class OrbwalkerSwitch
    {
        public static void Update()
        {
            switch (Menu.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    switch (PluginLoader.Champname)
                    {
                        case "Jinx":
                            Jinx.Combo();
                            break;
                        case "Tristana":
                            Tristana.Combo();
                            break;
                        case "Corki":
                            Corki.Combo();
                            break;
                    }
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    switch (PluginLoader.Champname)
                    {
                        case "Jinx":
                            Jinx.Harass();
                            break;
                        case "Tristana":
                            Tristana.Harass();
                            break;
                        case "Corki":
                            Corki.Harass();
                            break;
                    }
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    switch (PluginLoader.Champname)
                    {
                        case "Jinx":
                            Jinx.LaneClear();
                            break;
                        case "Tristana":
                            Tristana.LaneClear();
                            break;
                        case "Corki":
                            Corki.LaneClear();
                            break;
                    }
                    break;
            }
        }
    }
}
