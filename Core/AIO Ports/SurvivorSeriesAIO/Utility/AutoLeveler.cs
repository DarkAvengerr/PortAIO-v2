using System;
using LeagueSharp;
using LeagueSharp.Common;
using SurvivorSeriesAIO.Core;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SurvivorSeriesAIO.Utility
{
    internal class AutoLeveler : AutoLevelerBase
    {
        public int lvl1, lvl2, lvl3, lvl4;

        public AutoLeveler(IRootMenu menu) : base(menu)
        {
            Config = new Configuration(menu.AutoLeveler);
            Chat.Print(
                "<font color='#0993F9'>[SurvivorSeries AIO]</font> <font color='#FF8800'>AutoLeveler Loaded.</font>");
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            if ((lvl2 == lvl3) || (lvl2 == lvl4) || (lvl3 == lvl4))
                Chat.Print(
                    "<font color='#0993F9'>[SS AutoLeveler]</font> <font color='#FF8800'>Please select abilities to level up first.</font>");
        }

        public Configuration Config { get; }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!Menu.PlugAutoLeveler.GetValue<bool>())
                return;
            lvl1 = Config.AutoLeveler1.GetValue<StringList>().SelectedIndex;
            lvl2 = Config.AutoLeveler2.GetValue<StringList>().SelectedIndex;
            lvl3 = Config.AutoLeveler3.GetValue<StringList>().SelectedIndex;
            lvl4 = Config.AutoLeveler4.GetValue<StringList>().SelectedIndex;
        }

        private void GotStronger()
        {
            // Reminder
            Chat.Print(
                "<font color='#0993F9'>[SS AIO | Reminder]</font> <font color='#FF8800'>You got strong enough, Lower the LaneClear Mana Manager Sliders!</font>");
        }

        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (sender.IsMe && Config.Reminders.GetValue<bool>() && (ObjectManager.Player.Level >= 6))
                GotStronger();

            if (!sender.IsMe || !Menu.PlugAutoLeveler.GetValue<bool>() ||
                (ObjectManager.Player.Level < Config.AutoLvlStartFrom.GetValue<Slider>().Value))
                return;
            if ((lvl2 == lvl3) || (lvl2 == lvl4) || (lvl3 == lvl4))
                return;
            var delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
        }

        private void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if ((indx == 0) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if ((indx == 1) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if ((indx == 2) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        public class Configuration
        {
            public Configuration(Menu root)
            {
                AutoLevelerMenu = MenuFactory.CreateMenu(root, "AutoLeveler");

                AutoLevelerSettings(MenuItemFactory.Create(AutoLevelerMenu));
            }

            public MenuItem AutoLeveler1 { get; set; }
            public MenuItem AutoLeveler2 { get; set; }
            public MenuItem AutoLeveler3 { get; set; }
            public MenuItem AutoLeveler4 { get; set; }
            public Menu AutoLevelerMenu { get; set; }
            public MenuItem AutoLvlStartFrom { get; set; }
            public MenuItem Reminders { get; set; }

            public void AutoLevelerSettings(MenuItemFactory factory)
            {
                AutoLeveler1 =
                    factory.WithName("First: ").WithValue(new StringList(new[] {"Q", "W", "E", "R"}, 3)).Build();

                AutoLeveler2 =
                    factory.WithName("Second: ").WithValue(new StringList(new[] {"Q", "W", "E", "R"}, 0)).Build();

                AutoLeveler3 =
                    factory.WithName("Third: ").WithValue(new StringList(new[] {"Q", "W", "E", "R"}, 0)).Build();

                AutoLeveler4 =
                    factory.WithName("Fourth: ").WithValue(new StringList(new[] {"Q", "W", "E", "R"}, 1)).Build();

                AutoLvlStartFrom =
                    factory.WithName("AutoLeveler Start from Level: ").WithValue(new Slider(2, 6, 1)).Build();

                Reminders = factory.WithName("Enable [SS AIO] Reminders?").WithValue(true).Build();
            }
        }
    }
}