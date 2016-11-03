namespace Flowers_ADC_Series.Utility
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class AutoLevels : Program
    {
        private new static readonly Menu Menu = Utilitymenu;

        public static void Init()
        {
            var AutoLevelMenu = Menu.AddSubMenu(new Menu("Auto Levels", "Auto Levels"));
            {
                AutoLevelMenu.AddItem(new MenuItem("LevelsEnable", "Enabled", true).SetValue(false));
                AutoLevelMenu.AddItem(new MenuItem("LevelsAutoR", "Auto Level R", true).SetValue(true));
                AutoLevelMenu.AddItem(
                    new MenuItem("LevelsDelay", "Auto Level Delays", true).SetValue(new Slider(700, 0, 2000)));
                AutoLevelMenu.AddItem(
                    new MenuItem("LevelsLevels", "When Player Level >= Enable!", true).SetValue(new Slider(3, 1, 6)));
                AutoLevelMenu.AddItem(
                    new MenuItem("LevelsMode", "Mode: ", true).SetValue(
                        new StringList(new[]
                            {"Q -> W -> E", "Q -> E -> W", "W -> Q -> E", "W -> E -> Q", "E -> Q -> W", "E -> W -> Q"})));
            }

            Obj_AI_Base.OnLevelUp += OnLevelUp;
        }

        private static void OnLevelUp(Obj_AI_Base sender, EventArgs Args)
        {
            if (!sender.IsMe || !Menu.Item("LevelsEnable", true).GetValue<bool>())
            {
                return;
            }

            if (Menu.Item("LevelsAutoR", true).GetValue<bool>() && (Me.Level == 6 || Me.Level == 11 || Me.Level == 16))
            {
                Me.Spellbook.LevelSpell(SpellSlot.R);
            }

            if (Me.Level >= Menu.Item("LevelsLevels", true).GetValue<Slider>().Value)
            {
                int Delay = Menu.Item("LevelsDelay", true).GetValue<Slider>().Value;

                if (Me.Level < 3)
                {
                    switch (Menu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 1:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 2:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            break;
                        case 3:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                        case 4:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            break;
                        case 5:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            break;
                    }
                }
                else if (Me.Level > 3)
                {
                    switch (Menu.Item("LevelsMode", true).GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);

                            //Q -> W -> E
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.W);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 1:
                            if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);

                            //Q -> E -> W
                            DelayLevels(Delay, SpellSlot.Q);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 2:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);

                            //W -> Q -> E
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.E);
                            break;
                        case 3:
                            if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);

                            //W -> E -> Q
                            DelayLevels(Delay, SpellSlot.W);
                            DelayLevels(Delay + 50, SpellSlot.E);
                            DelayLevels(Delay + 100, SpellSlot.Q);
                            break;
                        case 4:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);

                            //E -> Q -> W
                            DelayLevels(Delay, SpellSlot.E);
                            DelayLevels(Delay + 50, SpellSlot.Q);
                            DelayLevels(Delay + 100, SpellSlot.W);
                            break;
                        case 5:
                            if (Me.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.E);
                            else if (Me.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.W);
                            else if (Me.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                                Me.Spellbook.LevelSpell(SpellSlot.Q);

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
