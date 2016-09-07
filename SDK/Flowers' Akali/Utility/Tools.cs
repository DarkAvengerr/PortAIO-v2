using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Akali.Utility
{
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    internal static class Tools
    {
        public static Menu Menu;

        internal static void Inject()
        {
            Menu = Program.Menu.Add(new Menu("Tools", "Tools"));

            Potions.Inject();
            Offensive.Inject();
            Defensive.Inject();
            Summoner.Inject();
            AutoLevel.Inject();
            SkinChance.Inject();

            Variables.Orbwalker.Enabled = true;
        }
    }
}