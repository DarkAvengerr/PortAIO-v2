using System.Linq;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SorakaSharp.Source.Handler
{
    internal static class CConfig
    {
        //Config
        internal static Menu ConfigMenu;
        private const string ConfigName = Program.ChampionName;

        //Orbwalker
        private static Orbwalking.Orbwalker Orbwalker;

        internal static void Initialize()
        {
            ConfigMenu = new Menu(ConfigName, ConfigName, true);

            //Orbwalker
            Orbwalker = new Orbwalking.Orbwalker(ConfigMenu.SubMenu("Orbwalking"));

            //Combo
            ConfigMenu.SubMenu("Combo")
                .AddItem(new MenuItem("comboUseQ", "Use Q").SetValue(true));
            ConfigMenu.SubMenu("Combo")
                .AddItem(new MenuItem("comboUseE", "Use E").SetValue(true));
            ConfigMenu.SubMenu("Combo")
                .AddItem(
                    new MenuItem("comboActive", "Combo!").SetValue(
                        new KeyBind(ConfigMenu.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Heal
            ConfigMenu.SubMenu("Heal")
                .AddItem(new MenuItem("useHeal", "Use Heal").SetValue(true));

            //Heal Priority
            ConfigMenu.SubMenu("Heal")
                .AddItem(
                    new MenuItem("priority", "Heal Priority").SetValue(
                        new StringList(new[] { "Most AD", "Most AP", "Lowest HP" })));

            //Minimum Percent for Heal
            ConfigMenu.SubMenu("Heal")
                .AddItem(new MenuItem("percentage", "Ally HP in % for Heal").SetValue(new Slider(60)));

            //Ignore Heal on...
            foreach (var ally in HeroManager.Allies.Where(ally => !ally.IsMe))
            {
                ConfigMenu.SubMenu("Heal")
                    .SubMenu("DontHeal")
                    .AddItem(new MenuItem("DontHeal" + ally.BaseSkinName, ally.BaseSkinName).SetValue(false));
            }

            //Ultimate
            ConfigMenu.SubMenu("Ultimate")
                .AddItem(new MenuItem("useUltimate", "Smart Ultimate to Save").SetValue(true));

            //Ignore Ultimate on...
            foreach (var ally in HeroManager.Allies)
            {
                ConfigMenu.SubMenu("Ultimate")
                    .SubMenu("DontUlt")
                    .AddItem(new MenuItem("DontUlt" + ally.BaseSkinName, ally.BaseSkinName).SetValue(false));
            }

            //Teamfight Ultimate (?)
            ConfigMenu.SubMenu("Ultimate")
                .AddItem(new MenuItem("useTeamfightUltimate", "Smart Teamfight Ultimate").SetValue(false));
            ConfigMenu.SubMenu("Ultimate")
                .AddItem(new MenuItem("percentage2", "Team HP in % for Heal").SetValue(new Slider(60)));

            //Misc
            ConfigMenu.SubMenu("Misc")
                .AddItem(new MenuItem("InterruptSpells", "Interrupt Spells with E").SetValue(true));
            ConfigMenu.SubMenu("Misc")
                .AddItem(new MenuItem("AntiGapCloser", "Cast E on Gapcloser").SetValue(false));

            ConfigMenu.AddToMainMenu();
        }
    }
}