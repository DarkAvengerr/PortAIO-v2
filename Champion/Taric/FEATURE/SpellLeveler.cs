namespace SkyLv_Taric
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class SpellLeveler
    {
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Taric.Player;
            }
        }

        static SpellLeveler()
        {
            SkyLv_Taric.Menu.SubMenu("Misc").AddSubMenu(new Menu("Auto Level Spell", "Auto Level Spell"));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").AddItem(new MenuItem("Taric.AutoLevelSpell", "Auto Level Spell").SetValue(false));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").AddSubMenu(new Menu("Advanced Settings", "Advanced Settings"));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV1", "Spell choice Lv. 1").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV2", "Spell choice Lv. 2").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV3", "Spell choice Lv. 3").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV4", "Spell choice Lv. 4").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV5", "Spell choice Lv. 5").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV6", "Spell choice Lv. 6").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV7", "Spell choice Lv. 7").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV8", "Spell choice Lv. 8").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV9", "Spell choice Lv. 9").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 2)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV10", "Spell choice Lv. 10").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV11", "Spell choice Lv. 11").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV12", "Spell choice Lv. 12").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV13", "Spell choice Lv. 13").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV14", "Spell choice Lv. 14").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 3)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV15", "Spell choice Lv. 15").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV16", "Spell choice Lv. 16").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 4)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV17", "Spell choice Lv. 17").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));
            SkyLv_Taric.Menu.SubMenu("Misc").SubMenu("Auto Level Spell").SubMenu("Advanced Settings").AddItem(new MenuItem("Taric.SpellLV18", "Spell choice Lv. 18").SetValue(new StringList(new[] { "No Spell", "Q", "W", "E", "R" }, 1)));


            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (SkyLv_Taric.Menu.Item("Taric.AutoLevelSpell").GetValue<bool>())
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV1").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV2").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV3").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV4").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV5").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV6").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV7").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV8").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV9").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV10").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV11").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV12").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV13").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV14").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV15").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV16").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV17").GetValue<StringList>().SelectedIndex)
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
                    switch (SkyLv_Taric.Menu.Item("Taric.SpellLV18").GetValue<StringList>().SelectedIndex)
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
