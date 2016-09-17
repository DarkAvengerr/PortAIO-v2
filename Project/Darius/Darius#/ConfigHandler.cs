using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DariusSharp
{
    /** 100% Credits to Hellsing for this great stuff!
     * Thanks that I'm allowed to take some parts of it**/

    internal class ConfigHandler
    {
        //Config Menu
        private const string MenuName = "mehDarius";
        private static MenuWrapper Config;

        //Private Wrapper Links
        private static Dictionary<string, MenuWrapper.BoolLink> _boolLinks = new Dictionary<string, MenuWrapper.BoolLink>();
        private static Dictionary<string, MenuWrapper.CircleLink> _circleLinks = new Dictionary<string, MenuWrapper.CircleLink>();
        private static Dictionary<string, MenuWrapper.KeyBindLink> _keyLinks = new Dictionary<string, MenuWrapper.KeyBindLink>();
        private static Dictionary<string, MenuWrapper.SliderLink> _sliderLinks = new Dictionary<string, MenuWrapper.SliderLink>();

        //Public Wrapper Links
        public static Dictionary<string, MenuWrapper.BoolLink> BoolLinks { get { return _boolLinks; } }
        public static Dictionary<string, MenuWrapper.CircleLink> CircleLinks { get { return _circleLinks; } }
        public static Dictionary<string, MenuWrapper.KeyBindLink> KeyLinks { get { return _keyLinks; } }
        public static Dictionary<string, MenuWrapper.SliderLink> SliderLinks { get { return _sliderLinks; } }

        //Process Links
        private static void ProcessLink(string key, object value)
        {
            if (value is MenuWrapper.BoolLink)
                _boolLinks.Add(key, value as MenuWrapper.BoolLink);
            else if (value is MenuWrapper.CircleLink)
                _circleLinks.Add(key, value as MenuWrapper.CircleLink);
            else if (value is MenuWrapper.KeyBindLink)
                _keyLinks.Add(key, value as MenuWrapper.KeyBindLink);
            else if (value is MenuWrapper.SliderLink)
                _sliderLinks.Add(key, value as MenuWrapper.SliderLink);
        }

        //Create the Config Menu
        public static void Initialize()
        {
            //Create Config
            Config = new MenuWrapper(MenuName);
 
            //Combo
            var subMenu = Config.MainMenu.AddSubMenu("Combo");
            ProcessLink("comboUseQ", subMenu.AddLinkedBool("Use Q"));
            ProcessLink("comboUseW", subMenu.AddLinkedBool("Use W"));
            ProcessLink("comboUseE", subMenu.AddLinkedBool("Use E"));
            ProcessLink("comboUseR", subMenu.AddLinkedBool("Use R"));
            ProcessLink("comboActive", subMenu.AddLinkedKeyBind("Combo active", 32, KeyBindType.Press));

            //Harass
            subMenu = Config.MainMenu.AddSubMenu("Harass");
            ProcessLink("harassUseQ", subMenu.AddLinkedBool("Use Q"));
            ProcessLink("harassMana", subMenu.AddLinkedSlider("Mana usage in percent (%)", 30));
            ProcessLink("harassActive", subMenu.AddLinkedKeyBind("Harass active", 'X', KeyBindType.Press));
//TOMORROW  ProcessLink("harassToggle", subMenu.AddLinkedKeyBind("Harass active (Toggle)", 'C', KeyBindType.Toggle));

            //Misc
            subMenu = Config.MainMenu.AddSubMenu("Misc");
            ProcessLink("KillstealR", subMenu.AddLinkedBool("Killsteal R"));
            ProcessLink("adjustDmg", subMenu.AddLinkedSlider("Adjust ultimate dmg", 0, -150, 150));
//          ProcessLink("packetCast", subMenu.AddLinkedBool("Packet casting"));

            //Drawings
            subMenu = Config.MainMenu.AddSubMenu("Drawings");
            ProcessLink("drawRangeQ", subMenu.AddLinkedCircle("Q range", true, Color.FromArgb(150, Color.IndianRed), SpellHandler.Q.Range));
            ProcessLink("drawRangeW", subMenu.AddLinkedCircle("W range", true, Color.FromArgb(150, Color.MediumPurple), SpellHandler.W.Range));
            ProcessLink("drawRangeE", subMenu.AddLinkedCircle("E range", true, Color.FromArgb(150, Color.DarkRed), SpellHandler.E.Range));
            ProcessLink("drawRangeR", subMenu.AddLinkedCircle("R range", false, Color.FromArgb(150, Color.Red), SpellHandler.R.Range));
        }
    }
}
