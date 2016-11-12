using System;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Marksman.Common
{
    using System.Linq;
    using System.Security.AccessControl;

    public static class CommonAutoLevel
    {
        public static Menu MenuLocal { get; set; }

        public static int[] SpellLevels;

        public static void Init(Menu nParentMenu)
        {

            MenuLocal = new Menu("Auto Level", "Auto Level").SetFontStyle(FontStyle.Regular, Color.Aquamarine);
            MenuLocal.AddItem(new MenuItem("AutoLevel.Set", "at StartTime:").SetValue(new StringList(new[] { "Allways Off", "Allways On", "Remember Last Settings" }, 2)));
            MenuLocal.AddItem(new MenuItem("AutoLevel.Active", "Auto Level Active!").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle))).Permashow(true, ObjectManager.Player.ChampionName + " | " + "Auto Level Up", Color.GreenYellow);


            //MenuLocal = new Menu("Auto Level", "Auto Level").SetFontStyle(FontStyle.Regular, Color.IndianRed);
            //MenuLocal.AddItem(new MenuItem("AutoLevel.Set", "at StartTime:").SetValue(new StringList(new[] { "Allways Off", "Allways On", "Remember Last Settings" }, 2)));
            //MenuLocal.AddItem(new MenuItem("AutoLevel.Active", "Auto Level Active!").SetValue(true));
            
            var championName = ObjectManager.Player.ChampionName.ToLowerInvariant();
            
            switch (championName)
            {
                case "ashe":
                    SpellLevels = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "caitlyn":
                    SpellLevels = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
                    //SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "corki":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  > ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 }
                                      : new int[] { 1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "draven":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    break;

                case "ezreal":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  > ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 2, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 }
                                      : new int[] { 1, 3, 1, 2, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2};

                //case "ezreal":
                //    SpellLevels = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                //    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "graves":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "gnar":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "jinx":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "jhin":
                    SpellLevels = new int[] { 1, 2, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kalista":
                    SpellLevels = new int[] { 2, 3, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kindred":
                    SpellLevels = new int[] { 2, 1, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3 , 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "kogmaw":
                    SpellLevels = ObjectManager.Player.PercentMagicDamageMod
                                  < ObjectManager.Player.PercentPhysicalDamageMod
                                      ? new int[] { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 }
                                      : new int[] { 3, 2, 1, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "lucian":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "missfortune":
                    SpellLevels = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "quinn":
                    SpellLevels = new int[] { 1, 3, 2, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "sivir":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "teemo":
                    SpellLevels = new int[] { 3, 1, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;
                    
                case "tristana":
                    SpellLevels = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "twitch":
                    SpellLevels = new int[] { 3, 2, 3, 1, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "urgot":
                    SpellLevels = new int[] { 3, 1, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "vayne":
                    SpellLevels = new int[] { 1, 2, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;

                case "varus":
                    SpellLevels = new int[] { 1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };
                    //MenuLocal.AddItem(new MenuItem("AutoLevel." + championName, GetLevelList(SpellLevels)));
                    break;
            }

            switch (MenuLocal.Item("AutoLevel.Set").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    MenuLocal.Item("AutoLevel.Active")
                        .SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle));
                    break;

                case 1:
                    MenuLocal.Item("AutoLevel.Active")
                        .SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle, true));
                    break;
            }

            nParentMenu.AddSubMenu(MenuLocal);
            Game.OnUpdate += GameOnUpdate;
        }

        private static string GetLevelList(int[] spellLevels)
        {
            var a = new[] { "Q", "W", "E", "R" };
            var b = spellLevels.Aggregate("", (c, i) => c + (a[i - 1] + " - "));
            return b != "" ? b.Substring(0, b.Length - 2) : "";
        }

        private static void GameOnUpdate(EventArgs args)
        {
            if (!MenuLocal.Item("AutoLevel.Active").GetValue<KeyBind>().Active)
            {
                return;
            }

            var qLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            var wLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level;
            var eLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level;
            var rLevel = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qLevel + wLevel + eLevel + rLevel >= ObjectManager.Player.Level)
            {
                return;
            }

            var level = new int[] { 0, 0, 0, 0 };
            for (var i = 0; i < ObjectManager.Player.Level; i++)
            {
                level[SpellLevels[i] - 1] = level[SpellLevels[i] - 1] + 1;
            }

            if (qLevel < level[0])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(CommonUtils.GetRandomDelay(750, 1000), () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q));

            }

            if (wLevel < level[1])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(CommonUtils.GetRandomDelay(750, 1000), () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W));
            }

            if (eLevel < level[2])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(CommonUtils.GetRandomDelay(750, 1000), () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E));
            }

            if (rLevel < level[3])
            {
                LeagueSharp.Common.Utility.DelayAction.Add(CommonUtils.GetRandomDelay(750, 1000), () => ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R));
            }
        }
    }
}
