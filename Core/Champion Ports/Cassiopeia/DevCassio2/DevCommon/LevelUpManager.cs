using EloBuddy; 
using LeagueSharp.Common; 
 namespace DevCommom2
{
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;

    public class LevelUpManager
    {
        private readonly int lastLevel;
        private readonly Dictionary<string, int[]> SpellPriorityList;
        private Menu Menu;
        public int SelectedPriority;
        public AutoLevel AutoLevel;

        public LevelUpManager()
        {
            lastLevel = 0;
            SpellPriorityList = new Dictionary<string, int[]>();
        }

        public void AddToMenu(ref Menu menu)
        {
            Menu = menu;

            if (SpellPriorityList.Count > 0)
            {
                Menu.AddSubMenu(new Menu("Spell Level Up", "LevelUp"));
                Menu.SubMenu("LevelUp")
                    .AddItem(
                        new MenuItem("LevelUp_" + ObjectManager.Player.ChampionName + "_enabled", "Enable").SetValue(
                            true));
                Menu.SubMenu("LevelUp")
                    .AddItem(
                        new MenuItem("LevelUp_" + ObjectManager.Player.ChampionName + "_select", "").SetValue(
                            new StringList(SpellPriorityList.Keys.ToArray())));
                SelectedPriority =
                    Menu.Item("LevelUp_" + ObjectManager.Player.ChampionName + "_select")
                        .GetValue<StringList>()
                        .SelectedIndex;
            }
        }

        public void Add(string spellPriorityDesc, int[] spellPriority)
        {
            SpellPriorityList.Add(spellPriorityDesc, spellPriority);
            AutoLevel = new AutoLevel(spellPriority);
            AutoLevel.Enabled(true);
        }

        public void Update()
        {
            if (SpellPriorityList.Count == 0 ||
                !Menu.Item("LevelUp_" + ObjectManager.Player.ChampionName + "_enabled").GetValue<bool>() ||
                lastLevel == ObjectManager.Player.Level)
            {
                AutoLevel.Enabled(false);
            }
        }
    }
}
