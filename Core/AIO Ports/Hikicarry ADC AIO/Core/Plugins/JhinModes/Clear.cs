using HikiCarry.Champions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HikiCarry.Core.Plugins.JhinModes
{
    static class Clear
    {
        /// <summary>
        /// Execute Q Clear
        /// </summary>
        private static void ExecuteQ()
        {
            var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Jhin.Q.Range).MinOrDefault(x => x.Health);
            Jhin.Q.CastOnUnit(min);
        }

        /// <summary>
        /// Execute W Clear
        /// </summary>
        private static void ExecuteW()
        {
            var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Jhin.W.Range);
            if (Jhin.W.GetLineFarmLocation(min).MinionsHit >= Initializer.Config.Item("w.hit.x.minion",true).GetValue<Slider>().Value)
            {
                Jhin.W.Cast(Jhin.W.GetLineFarmLocation(min).Position);
            }
        }

        /// <summary>
        /// Execute Clear
        /// </summary>
        public static void ExecuteClear()
        {
            if (ObjectManager.Player.ManaPercent < Initializer.Config.Item("clear.mana",true).GetValue<Slider>().Value)
            {
                return;
            }

            if (Jhin.Q.IsReady() && Initializer.Config.Item("q.clear",true).GetValue<bool>()) // done working
            {
                ExecuteQ();
            }

            if (Jhin.W.IsReady() && Initializer.Config.Item("w.clear",true).GetValue<bool>()) // done working
            {
                ExecuteW();
            }
        }
    }
}
