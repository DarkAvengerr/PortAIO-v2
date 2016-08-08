using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_AurelionSol
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;


    internal class AAManager
    {

        static AAManager()
        {
            //Menu
            SkyLv_AurelionSol.Menu.SubMenu("Combo").AddItem(new MenuItem("AurelionSol.DisableAAWhenW", "Disable AA When Using Outer StarRing W In Combo").SetValue(true));

            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            AAManagers();
        }


        public static void AAManagers()
        {
            if (SkyLv_AurelionSol.Menu.Item("AurelionSol.DisableAAWhenW").GetValue<bool>() && SkyLv_AurelionSol.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (CustomLib.isWInLongRangeMode())
                {
                    SkyLv_AurelionSol.Orbwalker.SetAttack(false);
                }
                else
                    SkyLv_AurelionSol.Orbwalker.SetAttack(true);
            }
            else
                SkyLv_AurelionSol.Orbwalker.SetAttack(true);
        }

    }
}

