using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hahaha_s_Tahm_Kench
{

    public class Variables
    {
        public static Menu Menu, OrbwalkerMenu, SettingsMenu;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;
    }

    public class Settings
    {
        public static void SetSpells()
        {
            Variables.Q = new Spell(SpellSlot.Q, 800f);
            Variables.W = new Spell(SpellSlot.W, 250f);
            Variables.E = new Spell(SpellSlot.E, 0f);
            Variables.R = new Spell(SpellSlot.R, 4000f + 5000f + 6000f);

            Variables.Q.SetSkillshot(0.5f, Variables.Q.Instance.SData.CastRadius, float.MaxValue, false,
                SkillshotType.SkillshotLine);
        }

        public static void SetMenu()
        {
            //Main menu
            Variables.Menu = new Menu("Hahaha's Cassiopeia", "Hahaha_s_Tahm_Kench", true);

            //Orbwalker
            Variables.OrbwalkerMenu = new Menu("Orbwalker", "Hahaha_s_Tahm_Kench.orbwalker");
            {
                Variables.Orbwalker = new Orbwalking.Orbwalker(Variables.OrbwalkerMenu);
            }
            Variables.Menu.AddSubMenu(Variables.OrbwalkerMenu);

            //Settings Menu
            Variables.SettingsMenu = new Menu("Spell Menu", "Hahaha_s_Tahm_Kench.settingsmenu");
            {
                Variables.SettingsMenu.AddItem(new MenuItem("Hahaha_s_Tahm_Kench.settings.useq", "Use Q"))
                    .SetValue(true);
            }
            Variables.Menu.AddToMainMenu();
        }
    }

    //Mana Manager
    public class ManaManager
    {
        public static int NeededQMana = Variables.Menu.Item("Hahaha_s_Tahm_Kench.qmana").GetValue<Slider>().Value;
    }

    public class Targets
    {
        public static Obj_AI_Base Target
            => TargetSelector.GetTarget(Variables.Q.Range, TargetSelector.DamageType.Magical);
    }
}