using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sAIO;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Core
{
    public class Helper : Program
    {
        public bool UseQCombo
        {
            get { return GetValueMenuBool("Combo.Q"); }
        }
        public static void CreateMenuBool(string subMenuName, string name, string displayName, bool enable)
        {               
            menu.SubMenu(subMenuName).AddItem(new MenuItem(name, displayName).SetValue(enable));
        }
        public static void CreateMenuSlider(string subMenuName, string name, string displayName, int minValue , int value, int maxValue)
        {
            menu.SubMenu(subMenuName).AddItem(new MenuItem(name, displayName).SetValue(new Slider(value, minValue, maxValue)));
        }
        public static void CreateMenuKeyBind(string subMenuName, string name, string displayName, char key, KeyBindType keyBindType)
        {
            menu.SubMenu(subMenuName).AddItem(new MenuItem(name, displayName).SetValue(new KeyBind(key, keyBindType)));
        }
        public static bool GetValueMenuBool(string itemName)
        {
            return menu.Item(itemName).GetValue<bool>();
        }
        public static int GetValueMenuSlider(string itemName)
        {
            return menu.Item(itemName).GetValue<Slider>().Value;
        }
        public static bool GetValueMenuKeyBind(string itemName)
        {
            return menu.Item(itemName).GetValue<KeyBind>().Active;
        }

        public static bool HasBuff(Obj_AI_Base target, string buffName)
        {
            foreach (BuffInstance buff in target.Buffs)
                if (buff.Name == buffName) return true;
            return false;
        }
    }
}
