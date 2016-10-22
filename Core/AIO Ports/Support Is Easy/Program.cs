using EloBuddy;
namespace Support
{
    using System;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Support.Util;

    using Version = System.Version;

    internal class Program
    {
        public static Version Version;

        public static void Main()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version;

            try
            {
                var type = Type.GetType("Support.Plugins." + ObjectManager.Player.ChampionName);

                if (type != null)
                {
                    Helpers.UpdateCheck();
                    Protector.Init();
                    DynamicInitializer.NewInstance(type);
                    return;
                }

                Helpers.PrintMessage(ObjectManager.Player.ChampionName + " not supported");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
