using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LeeSin_EloClimber
{
    internal class MenuManager
    {
        public static Menu myMenu;

        internal static void LoadMenu()
        {
            myMenu = new Menu("Lee Sin - Elo Climber", "LeeSinEloClimber", true);

            myMenu.AddSubMenu(new Menu("Combo Settings", "Combo"));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.useQ", "Use (Q)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.qHitChance", "(Q) Hit Chance").SetValue(new Slider(3, 3, 6)));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.useW", "Use (W)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.ward", "Use Ward Jump").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.useE", "Use (E)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.useR", "Use (R)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("combo.rLogic", "Use (W)(R) Bump maximum unit").SetValue(true));

            myMenu.AddSubMenu(new Menu("Insec Settings", "Insec"));
                myMenu.SubMenu("Insec").AddItem(new MenuItem("insec.key", "Insec Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press, false)));
                myMenu.SubMenu("Insec").AddItem(new MenuItem("insec.qHitChance", "(Q) Hit Chance").SetValue(new Slider(3, 3, 6)));

            myMenu.AddSubMenu(new Menu("Ward Jump Settings", "WardJump"));
                myMenu.SubMenu("WardJump").AddItem(new MenuItem("wardjump.key", "Ward Jump Key").SetValue(new KeyBind("S".ToCharArray()[0], KeyBindType.Press, false)));

         myMenu.AddSubMenu(new Menu("Lane Clear Settings", "LaneClear"));
                myMenu.SubMenu("LaneClear").AddItem(new MenuItem("lane.useQ", "Use (Q)").SetValue(true));
                myMenu.SubMenu("LaneClear").AddItem(new MenuItem("lane.useW", "Use (W)").SetValue(true));
                myMenu.SubMenu("LaneClear").AddItem(new MenuItem("lane.useE", "Use (E)").SetValue(true));
                myMenu.SubMenu("LaneClear").AddItem(new MenuItem("lane.countE", "(E) min minion hit").SetValue(new Slider(3, 1, 6)));

            myMenu.AddSubMenu(new Menu("Jungle Clear Settings", "JungleClear"));
                myMenu.SubMenu("JungleClear").AddItem(new MenuItem("jungle.useQ", "Use (Q)").SetValue(true));
                myMenu.SubMenu("JungleClear").AddItem(new MenuItem("jungle.useW", "Use (W)").SetValue(true));
                myMenu.SubMenu("JungleClear").AddItem(new MenuItem("jungle.useE", "Use (E)").SetValue(true));

            if (LeeSin.SmiteSpell != null)
            {
                myMenu.AddSubMenu(new Menu("Smite Settings", "Smite"));
                myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.use", "Use Auto Smite").SetValue(new KeyBind("Q".ToCharArray()[0], KeyBindType.Toggle, true)));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.useQ", "Use (Q)").SetValue(true));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.Nashor", "Use on Nashor").SetValue(true));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.Herald", "Use on Herald").SetValue(true));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.Drake", "Use on Drake").SetValue(true));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.Red", "Use on Red").SetValue(true));
                    myMenu.SubMenu("Smite").AddItem(new MenuItem("smite.Blue", "Use on Blue").SetValue(true));
            }

            myMenu.AddSubMenu(new Menu("Drawing Settings", "Drawing"));
                myMenu.SubMenu("Drawing").AddItem(new MenuItem("combo.target", "Draw Target").SetValue(true));
                myMenu.SubMenu("Drawing").AddItem(new MenuItem("smite.State", "Draw Auto Smite State").SetValue(true));
                myMenu.SubMenu("Drawing").AddItem(new MenuItem("insec.WardPos", "Draw Insec Ward Position").SetValue(true));
                myMenu.SubMenu("Drawing").AddItem(new MenuItem("insec.PredictPos", "Draw Insec Unit Prediction Position").SetValue(true));


            myMenu.AddSubMenu(new Menu("Prediction Settings", "Pred"));
            myMenu.SubMenu("Pred").AddItem(new MenuItem("pred.list2", "Choose Prediction").SetValue(new StringList(new[] { "Common Pred", "Own Pred"}, 1)));

            myMenu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                LeeSin.Orbwalker = new Orbwalking.Orbwalker(myMenu.SubMenu("Orbwalker"));

            myMenu.AddToMainMenu();

        }
    }
}
