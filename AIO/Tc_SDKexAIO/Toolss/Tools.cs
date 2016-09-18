using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Toolss
{

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;

    internal static class Tools
    {
        public static Menu Menu;

        internal static void Init()
        {
            Menu = PlaySharp.Menu.Add(new Menu("Tools", "Tools"));

            PlaySharp.Write("Tools OK!");

            Potions.Init();
            Offensive.Init();
            SkinChance.Init();
            QSS.Init();

            new AutoWard(Menu);

            Variables.Orbwalker.Enabled = true;
        }
    }
}