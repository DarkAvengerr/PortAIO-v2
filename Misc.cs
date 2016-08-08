using EloBuddy;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortAIO.Dual_Port
{
    class Misc
    {
        public static Menu menu;

        public static void Load()
        {
            (menu = new Menu("PortAIO Misc", "PAIOMisc", true)).AddToMainMenu();

            var dualPort = new Menu("Dual-Port", "DualPAIOPort");
            menu.AddSubMenu(dualPort);
            switch(ObjectManager.Player.Hero)
            {
                case Champion.TwistedFate:
                    string[] twistedfate = { "TwistedFate by Kortatu", "SharpShooter" };
                    dualPort.AddItem(new MenuItem(Champion.TwistedFate.ToString(), "Dual-Port : ").SetValue(new StringList(twistedfate, 0)));
                    break;
                case Champion.Riven:
                    string[] riven = { "KurisuRiven", "HoolaRiven" };
                    dualPort.AddItem(new MenuItem(Champion.Riven.ToString(), "Dual-Port : ").SetValue(new StringList(riven, 0)));
                    break;
                default:
                    dualPort.AddItem(new MenuItem("info1", "There are no dual-port for this champion."));
                    dualPort.AddItem(new MenuItem("info2", "Feel free to request one."));
                    break;
            }

            var autoPlay = new Menu("Auto Play", "PortAIOAUTOPLAY");
            autoPlay.AddItem(new MenuItem("AutoPlay", "Enable AutoPlay?").SetValue(false));
            autoPlay.AddItem(new MenuItem("selectAutoPlay", "Which AutoPlay?").SetValue(new StringList(new[] { "AramDETFull", "AutoJungle" })));
            menu.AddSubMenu(autoPlay);

            var utility = new Menu("Utilities", "Utilitiesports");
            utility.AddItem(new MenuItem("enableActivator", "Enable Activator?").SetValue(false));
            utility.AddItem(new MenuItem("Activator", "Which Activator?").SetValue(new StringList(new[] { "ElUtilitySuite", "Activator#" })));

            utility.AddItem(new MenuItem("enableTracker", "Enable Tracker?").SetValue(false));
            utility.AddItem(new MenuItem("Tracker", "Which Tracker?").SetValue(new StringList(new[] { "SFXUtility", "ShadowTracker" })));

            utility.AddItem(new MenuItem("enableEvade", "Enable Evade?").SetValue(false));
            utility.AddItem(new MenuItem("Evade", "Which Evade?").SetValue(new StringList(new[] { "EzEvade", "Evade" })));

            utility.AddItem(new MenuItem("enableHuman", "Enable Humanizer?").SetValue(false));
            utility.AddItem(new MenuItem("Humanizer", "Which Humanizer?").SetValue(new StringList(new[] { "Humanizer#", "Sebby Ban Wars" })));
            menu.AddSubMenu(utility);

            menu.AddItem(new MenuItem("UtilityOnly", "Utility Only?").SetValue(false));
            menu.AddItem(new MenuItem("ChampsOnly", "Champs Only?").SetValue(false));
        }
    }
}
