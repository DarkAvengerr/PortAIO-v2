using LeagueSharp.Common;
using Menu = LeagueSharp.Common.Menu;

namespace DZLib.External
{
    class EvadeManager
    {
        public static void DisableEvades(int duration = -1, bool DisableEvadeSharp = true, bool DisableEzEvade = true)
        {
            if (GetEvadeSharpStatus() && DisableEvadeSharp)
            {
                var toggled = ToggleEvadeSharp();
                if (duration != -1 && toggled)
                {
                    Utility.DelayAction.Add(duration, () =>
                    { ToggleEvadeSharp(); });
                }
            }

            if (GetEzEvadeStatus() && DisableEzEvade)
            {
                var toggled = ToggleEzEvade();
                if (duration != -1 && toggled)
                {
                    Utility.DelayAction.Add(duration, () =>
                    { ToggleEzEvade(); });
                }
            }
        }

        public static void EnableEvades(bool EnableEvadeSharp = true, bool EnableEzEvade = true)
        {
            if (!GetEvadeSharpStatus() && EnableEvadeSharp)
            {
                ToggleEvadeSharp();
            }

            if (!GetEzEvadeStatus() && EnableEzEvade)
            {
                ToggleEzEvade();
            }
        }

        private static bool ToggleEzEvade()
        {
            var evadeMenu = LeagueSharp.Common.Menu.GetMenu("ezEvade", "ezEvade");
            var menuItem = evadeMenu?.Item("DodgeSkillShots");
            if (menuItem != null)
            {
                var menuValue = menuItem.GetValue<KeyBind>();
                menuValue.Active = !menuValue.Active;
                menuItem.SetValue(menuValue);
                return true;
            }
            return false;
        }

        private static bool ToggleEvadeSharp()
        {
            var evadeMenu = LeagueSharp.Common.Menu.GetMenu("Evade", "Evade");
            var menuItem = evadeMenu?.Item("Enabled");
            if (menuItem != null)
            {
                var menuValue = menuItem.GetValue<KeyBind>();
                menuValue.Active = !menuValue.Active;
                menuItem.SetValue(menuValue);
                return true;
            }
            return false;
        }

        private static bool GetEvadeSharpStatus()
        {
            var evadeMenu = LeagueSharp.Common.Menu.GetMenu("Evade", "Evade");
            var menuItem = evadeMenu?.Item("Enabled");
            return evadeMenu != null && menuItem != null && menuItem.IsActive();
        }

        private static bool GetEzEvadeStatus()
        {
            var evadeMenu = LeagueSharp.Common.Menu.GetMenu("ezEvade", "ezEvade");
            var menuItem = evadeMenu?.Item("DodgeSkillShots");
            return evadeMenu != null && menuItem != null && menuItem.IsActive();
        }
    }

    enum EvadeType
    {
        Evade, EzEvade
    }
}
