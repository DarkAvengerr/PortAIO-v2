using EloBuddy; 
using LeagueSharp.Common; 
namespace myCommon
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;

    public static class LevelsManager
    {
        private static Menu levelMenu;

        public static void AddToMenu(Menu mainMenu)
        {
            levelMenu = mainMenu;

            mainMenu.AddItem(new MenuItem("LevelsEnable", "Enabled", true).SetValue(false));
            mainMenu.AddItem(new MenuItem("LevelsAutoR", "Auto Level R", true).SetValue(true));
            mainMenu.AddItem(
                new MenuItem("LevelsDelay", "Auto Level Delays", true).SetValue(new Slider(700, 0, 2000)));
            mainMenu.AddItem(
                new MenuItem("LevelsLevels", "When Player Level >= Enable!", true).SetValue(new Slider(3, 1, 6)));
            mainMenu.AddItem(
                new MenuItem("LevelsMode", "Mode: ", true).SetValue(
                    new StringList(new[]
                        {"Q -> W -> E", "Q -> E -> W", "W -> Q -> E", "W -> E -> Q", "E -> Q -> W", "E -> W -> Q"})));

            Obj_AI_Base.OnLevelUp += OnLevelUp;
        }

        private static void OnLevelUp(Obj_AI_Base sender, EventArgs Args)
        {
            if (!sender.IsMe || !levelMenu.Item("LevelsEnable", true).GetValue<bool>())
            {
                return;
            }

            if (levelMenu.Item("LevelsAutoR", true).GetValue<bool>() && (ObjectManager.Player.Level == 6 || ObjectManager.Player.Level == 11 || ObjectManager.Player.Level == 16))
            {
                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }

            if (ObjectManager.Player.Level >= levelMenu.Item("LevelsLevels", true).GetValue<Slider>().Value)
            {
                int Delay = levelMenu.Item("LevelsDelay", true).GetValue<Slider>().Value;

                if (ObjectManager.Player.Level < 3)
                {
                    switch (levelMenu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 1:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 2:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 3:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                        case 4:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 5:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                    }
                }
                else if (ObjectManager.Player.Level > 3)
                {
                    switch (levelMenu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);

                            //Q -> W -> E
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.W);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 1:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);

                            //Q -> E -> W
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 2:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);

                            //W -> Q -> E
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 3:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);

                            //W -> E -> Q
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.Q);
                            break;
                        case 4:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);

                            //E -> Q -> W
                            DelayLevels(Delay, SpellSlot.E);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 5:
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);

                            //E -> W -> Q
                            DelayLevels(Delay, SpellSlot.E);
                            DelayLevels(Delay + 50, SpellSlot.W);
                            DelayLevels(Delay + 100, SpellSlot.Q);
                            break;
                    }
                }
            }
        }

        private static void DelayLevels(int time, SpellSlot slot)
        {
            LeagueSharp.Common.Utility.DelayAction.Add(time, () => ObjectManager.Player.Spellbook.LevelSpell(slot));
        }
    }
}
