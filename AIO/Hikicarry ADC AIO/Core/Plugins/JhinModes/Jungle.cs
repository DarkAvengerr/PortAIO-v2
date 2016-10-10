using HikiCarry.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Plugins.JhinModes
{
    static class Jungle
    {
        /// <summary>
        /// Execute Jungle
        /// </summary>
        public static void ExecuteJungle()
        {
            if (ObjectManager.Player.ManaPercent < Initializer.Config.Item("jungle.mana",true).GetValue<Slider>().Value)
            {
                return;
            }

            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Jhin.Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            
            if (mobs == null || (mobs.Count == 0))
            {
                return;
            }

            if (Jhin.Q.IsReady() && Initializer.Config.Item("q.clear",true).GetValue<bool>())
            {
                Jhin.Q.Cast(mobs[0]);
            }

            if (Jhin.W.IsReady() && Initializer.Config.Item("w.clear",true).GetValue<bool>())
            {
                Jhin.W.Cast(mobs[0]);
            }
        }
    }
}
