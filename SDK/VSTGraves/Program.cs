using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp.SDK;
    using System;

    static class Program
    {
        public static void Main()
        {
            Bootstrap.Init();
            OnLoad();
        }

        private static void OnLoad()
        {
            if (GameObjects.Player.ChampionName != "Graves")
            {
                return;
            }

            new Graves();
        }
    }
}
