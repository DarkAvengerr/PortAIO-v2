using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;
    using System;

    internal class AutoLevel
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        internal static void Inject()
        {
            var AutoLevelMenu = Menu.Add(new Menu("AutoLevels", "Auto Levels"));
            {
                AutoLevelMenu.Add(new MenuBool("Enable", "Enabled", false));
                AutoLevelMenu.Add(new MenuBool("AutoR", "Auto Level R", true));
                AutoLevelMenu.Add(new MenuSlider("Delay", "Auto Level Delays", 700, 0, 2000));
                AutoLevelMenu.Add(new MenuSlider("Levels", "When Player Level >= Enable Auto Levels", 3, 1, 6));
                AutoLevelMenu.Add(new MenuList<string>("Mode", "Mode: ", new[] { "Q -> W -> E", "Q -> E -> W", "W -> Q -> E", "W -> E -> Q", "E -> Q -> W", "E -> W -> Q" }));
            }

            Obj_AI_Base.OnLevelUp += OnLevelUp;
        }

        private static void OnLevelUp(Obj_AI_Base sender, EventArgs Args)
        {
            if (!sender.IsMe || !Menu["AutoLevels"]["Enable"])
            {
                return;
            }

            if (Menu["AutoLevels"]["AutoR"] && (Me.Level == 6 || Me.Level == 11 || Me.Level == 16))
            {
                Me.Spellbook.LevelSpell(SpellSlot.R);
            }

            if (Me.Level >= Menu["AutoLevels"]["Levels"].GetValue<MenuSlider>().Value)
            {
                int Delay = Menu["AutoLevels"]["Delay"].GetValue<MenuSlider>().Value;

                if (Me.Level >= 4)
                {
                    switch (Menu["AutoLevels"]["Mode"].GetValue<MenuList>().Index)
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
                        default:
                            //break
                            break;
                    }
                }
            }
        }

        private static void DelayLevels(int time, SpellSlot slot)
        {
            DelayAction.Add(time, () => Me.Spellbook.LevelSpell(slot));
        }
    }
}