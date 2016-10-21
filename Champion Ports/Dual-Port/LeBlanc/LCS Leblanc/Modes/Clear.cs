using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LCS_LeBlanc.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace LCS_LeBlanc.Modes
{
    internal static class Clear
    {
        public static void JungleInit()
        {
            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (ObjectManager.Player.ManaPercent < Utilities.Slider("jungle.mana") && (mob == null || mob.Count == 0))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.jungle") && mob[0].IsValidTarget(Spells.Q.Range))
            {
                Spells.Q.CastOnUnit(mob[0]);
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.jungle") && mob[0].IsValidTarget(Spells.W.Range))
            {
                Spells.W.Cast(mob[0].Position);
            }

            if (Spells.E.IsReady() && Utilities.Enabled("e.jungle") && mob[0].IsValidTarget(Spells.E.Range))
            {
                Spells.E.Cast(mob[0].Position);
            }
        }

        public static string LeBlancTeam()
        {
            return ObjectManager.Player.Team == GameObjectTeam.Chaos ? "chaos" : "order";
        }

        public static string GetSiegeMinionName()
        {
            return LeBlancTeam() == "chaos" ? "SRU_ChaosMinionSiege" : "SRU_OrderMinionSiege";
        }

        public static void WaveInit()
        {
            var minion = MinionManager.GetMinions(ObjectManager.Player.Position, Spells.Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (ObjectManager.Player.ManaPercent < Utilities.Slider("clear.mana") && (minion == null || minion.Count == 0))
            {
                return;
            }

            if (Spells.Q.IsReady() && Utilities.Enabled("q.lasthit"))
            {
                foreach (var min in minion.Where(o => o.IsValidTarget(Spells.Q.Range) && o.CharData.BaseSkinName.EndsWith("MinionSiege") &&
                    Spells.Q.GetDamage(o) > o.Health))
                {
                    Spells.Q.Cast(min);
                }
            }

            if (Spells.W.IsReady() && Utilities.Enabled("w.clear"))
            {
                var aeominpos = Spells.W.GetCircularFarmLocation(minion);
                if (aeominpos.MinionsHit >= Utilities.Slider("w.hit.x.minion"))
                {
                    Spells.W.Cast(aeominpos.Position);
                }
            }
        }
    }
}
