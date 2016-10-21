using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    internal class AsheMenu
    {
        #region Attributes
        private Ashe _parentAssembly;
        #endregion

        #region Properties
        public HitChance ComboHitChance
        {
            get { return Menu.GetItemEndKey("HitChance", "Combo").GetHitchance(); }
        }


        private Dictionary<Orbwalking.OrbwalkingMode, string> _menuKeys = new Dictionary<Orbwalking.OrbwalkingMode, string>()
        {
            {Orbwalking.OrbwalkingMode.Combo,"Combo"},
            {Orbwalking.OrbwalkingMode.LastHit,""},
            {Orbwalking.OrbwalkingMode.Mixed,"Mixed"},
            {Orbwalking.OrbwalkingMode.LaneClear,"Clear"},
            {Orbwalking.OrbwalkingMode.None,""},
        };

        public MenuItem GetKeyForMode(string key,Orbwalking.OrbwalkingMode mode)
        {
            Console.WriteLine("key: " + key + " mode: " + mode + " res: " + _menuKeys[mode]);
            return Menu.GetItemEndKey(key, _menuKeys[mode]);
        }

        #endregion
        public Menu Menu { get { return Program.RootMenu; } }


        public AsheMenu(Ashe parentAssembly)
        {
            this._parentAssembly = parentAssembly;
        }
        public void Initialize()
        {

            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);

            Menu.AddSubMenu(targetSelector);

            var drawingMenu = new Menu("Drawings", _parentAssembly.GetNamespace() + ".Drawings");
            drawingMenu.AddItem("Off", "Activate Drawings", true);
            drawingMenu.AddItem("W", "Draw W Range", new Circle());
            drawingMenu.AddItem("AutoR", "Draw Auto R Range", new Circle());
            drawingMenu.AddItem("FillColor", "Fill color",new Circle(true, Color.FromArgb(204, 204, 0, 0)));
            Menu.AddSubMenu(drawingMenu);

            var comboMenu = new Menu("Combo", _parentAssembly.GetNamespace() + ".Combo");
            comboMenu.AddItem("HitChance", "Hitchance", new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3));
            comboMenu.AddItem("UseQ", "Use Q", true);
            comboMenu.AddItem("UseW", "Use W", true);
            comboMenu.AddItem("UseWMana", "Minimum Mana to Use W", new Slider(10,0,100));
            comboMenu.AddItem("UseR", "Use R", true);
            comboMenu.AddItem("SaveR", "Save Mana for R", true);

            Menu.AddSubMenu(comboMenu);

            var harassMenu = new Menu("Harrass", _parentAssembly.GetNamespace() + ".Mixed");
            harassMenu.AddItem("UseQ", "Use Q", true);
            harassMenu.AddItem("UseW", "Use W", true);
            harassMenu.AddItem("ManaThreshold", "Minimum mana for harass", new Slider(55));

            Menu.AddSubMenu(harassMenu);

            var clearMenu = new Menu("Lane & Jungler Clear", _parentAssembly.GetNamespace() + ".Clear");
            clearMenu.AddItem("UseQ", "Use Q", true);
            clearMenu.AddItem("UseW", "Use W", true);

            Menu.AddSubMenu(clearMenu);


            var miscMenu = new Menu("Misc", _parentAssembly.GetNamespace() + ".Misc");
            var miscAutoRMenu = new Menu("Auto R", _parentAssembly.GetNamespace() + ".Misc.AutoR");
            miscAutoRMenu.AddItem("Toggle", "Auto R when in range", new KeyBind(84, KeyBindType.Press)); // T Key
            miscAutoRMenu.AddItem("Range", "Auto R Range", new Slider(2000, Convert.ToInt32(_parentAssembly.Player.AttackRange), 4000));
            miscAutoRMenu.AddItem("Hitchance", "Auto R Hitchance", new StringList(new[] { "Low", "Medium", "High", "Very High" }, 3));
            miscMenu.AddSubMenu(miscAutoRMenu);

            Menu.AddSubMenu(miscMenu);

            var credits = new Menu("Credits", "Romesti");
            credits.AddItem(new MenuItem(_parentAssembly.GetNamespace() + ".Paypal", "You can make a donation via paypal :)"));
            Menu.AddSubMenu(credits);

            Menu.AddItem(new MenuItem("422442fsaafs4242f", ""));
            Menu.AddItem(new MenuItem("422442fsaafsf", "Version " + _parentAssembly.ScriptVersion));
            Menu.AddItem(new MenuItem("fsasfafsfsafsa", "Made By Romesti"));

            Menu.AddToMainMenu();


            Console.WriteLine("Menu Loaded lol");
        }



    }
}
