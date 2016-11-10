using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ElRengarDecentralized.Utils
{
    using System;
    using ElRengarDecentralized.Components;
    using ElRengarDecentralized.Enumerations;

    using LeagueSharp.Common;

    internal class PrioritizeManager
    {
        /// <summary>
        ///     Last switching.
        /// </summary>
        public static int LastSwitch;

        /// <summary>
        /// 
        /// </summary>
        public static void SwitchCombo()
        {
            try
            {
                var switchTime = Utils.GameTimeTickCount - LastSwitch;
                if (MyMenu.RootMenu.Item("combo.switch").GetValue<KeyBind>().Active && switchTime >= 350)
                {
                    switch (MyMenu.RootMenu.Item("combo.prio").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            MyMenu.RootMenu.Item("combo.prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 2));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                        case 1:
                            MyMenu.RootMenu.Item("combo.prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                        default:
                            MyMenu.RootMenu.Item("combo.prio").SetValue(new StringList(new[] { "E", "W", "Q" }, 0));
                            LastSwitch = Utils.GameTimeTickCount;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@PrioritizeManager.cs: Can not get spell active state for slot {0}", e);
                throw;
            }
        }
    }
}
