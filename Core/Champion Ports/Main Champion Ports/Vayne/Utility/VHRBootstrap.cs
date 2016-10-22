using LeagueSharp;
using VayneHunter_Reborn.External.Activator;
using VayneHunter_Reborn.Utility.MenuUtility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace VayneHunter_Reborn.Utility
{
    class VHRBootstrap
    {
        public static void OnLoad()
        {
            Variables.Menu = new LeagueSharp.Common.Menu("VayneHunter Reborn","dz191.vhr", true);

            SPrediction.Prediction.Initialize(Variables.Menu);
            MenuGenerator.OnLoad();
            External.Activator.Activator.OnLoad();
            VHR.OnLoad();
            DrawManager.OnLoad();
        }
    }
}
