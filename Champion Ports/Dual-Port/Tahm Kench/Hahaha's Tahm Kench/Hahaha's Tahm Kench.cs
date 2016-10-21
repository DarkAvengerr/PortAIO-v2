using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
//main class
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Hahaha_s_Tahm_Kench
{
    public class Tahm_Kench
    {
        public static void OnLoad()
        {
            Settings.SetSpells();
            Settings.SetMenu();

            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead &&
                Targets.Target != null &&
                Variables.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)
            {
                if (Targets.Target.IsValid<AIHeroClient>())
                {
                    var tg = Targets.Target as AIHeroClient;
                    if (Variables.Menu.Item("Hahaha_s_Tahm_Kench.settings.user").GetValue<bool>() && Variables.Q.IsReady()
                    )
                        Variables.Q.Cast(tg.Position);
                    return;
                }

                if (Targets.Target.Health < Variables.Q.GetDamage(Targets.Target))
                {
                    if (Variables.Menu.Item("hahaha_s_Tahm_Kench.settings.useq").GetValue<bool>() &&
                            Variables.Q.IsReady())
                    {
                        Variables.Q.Cast(Targets.Target);
                        return;
                    }

                }
            }
        }
    }
}