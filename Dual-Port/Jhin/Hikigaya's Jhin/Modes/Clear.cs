using Jhin___The_Virtuoso.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Modes
{
    static class Clear
    {
        /// <summary>
        /// Execute Q Clear
        /// </summary>
        private static void ExecuteQ()
        {
            var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.Q.Range).MinOrDefault(x => x.Health);
            Spells.Q.CastOnUnit(min);
        }

        /// <summary>
        /// Execute W Clear
        /// </summary>
        private static void ExecuteW()
        {
            var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.W.Range);
            if (Spells.W.GetLineFarmLocation(min).MinionsHit >= Menus.Config.Item("w.hit.x.minion").GetValue<Slider>().Value)
            {
                Spells.W.Cast(Spells.W.GetLineFarmLocation(min).Position);
            }
        }

        /// <summary>
        /// Execute Clear
        /// </summary>
        public static void ExecuteClear()
        {
            if (ObjectManager.Player.ManaPercent < Menus.Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Spells.Q.LSIsReady() && Menus.Config.Item("q.clear").GetValue<bool>()) // done working
            {
                ExecuteQ();
            }

            if (Spells.W.LSIsReady() && Menus.Config.Item("w.clear").GetValue<bool>()) // done working
            {
                ExecuteW();
            }
        }
    }
}
