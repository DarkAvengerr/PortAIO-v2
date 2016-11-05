using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Evelynn
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class SpellLeveler
    {
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Evelynn.Player;
            }
        }

        static SpellLeveler()
        {
            SkyLv_Evelynn.Menu.SubMenu("Misc").AddSubMenu(new Menu("Auto Level Spell", "Auto Level Spell"));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").AddItem(new MenuItem("Evelynn.AutoLevelSpell", "Auto Level Spell").SetValue(false));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV1", "Spell choice Lv. 1").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV2", "Spell choice Lv. 2").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV3", "Spell choice Lv. 3").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV4", "Spell choice Lv. 4").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV5", "Spell choice Lv. 5").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV6", "Spell choice Lv. 6").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV7", "Spell choice Lv. 7").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV8", "Spell choice Lv. 8").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV9", "Spell choice Lv. 9").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV10", "Spell choice Lv. 10").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV11", "Spell choice Lv. 11").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV12", "Spell choice Lv. 12").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV13", "Spell choice Lv. 13").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV14", "Spell choice Lv. 14").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV15", "Spell choice Lv. 15").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV16", "Spell choice Lv. 16").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV17", "Spell choice Lv. 17").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Evelynn.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Evelynn.SpellLV18", "Spell choice Lv. 18").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Evelynn.Menu.Item("Evelynn.AutoLevelSpell").GetValue<bool>())
            {
                LevelUpSpells();
            }
        }


        public static void LevelUpSpells()
        {
            int qL = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            int wL = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            int eL = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level;
            int rL = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                if (qL + wL + eL + rL == 0)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV1").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 1)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV2").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 2)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV3").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 3)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV4").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 4)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV5").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 5)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV6").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 6)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV7").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 7)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV8").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 8)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV9").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 9)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV10").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 10)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV11").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 11)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV12").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 12)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV13").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 13)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV14").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 14)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV15").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 15)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV16").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 16)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV17").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }

                if (qL + wL + eL + rL == 17)
                {
                    switch (SkyLv_Evelynn.Menu.Item("Evelynn.SpellLV18").GetValue<StringList>().SelectedIndex)
                    {
                        case 1:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                                break;
                            }

                        case 2:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                                break;
                            }

                        case 3:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                                break;
                            }
                        case 4:
                            {
                                ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
                                break;
                            }
                    }
                    return;
                }
            }



        }

    }
}
