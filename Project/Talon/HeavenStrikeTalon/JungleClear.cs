using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeTalon
{
    using static Program;
    using static Extension;
    class JungleClear
    {
        public static void UpdateJungleClear()
        {
            if (WJungleClear && W.IsReady())
            {
                var minion = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    W.Cast(minion);
                }
            }
        }
        public static void AfterAttackJungleClear(AttackableUnit unit, AttackableUnit target)
        {
            if (target.Team != GameObjectTeam.Neutral)
                return;
            if (QJungleClear && Q.IsReady())
            {
                Q.Cast();
            }
            else if (TiamatJungleClear && HasItem())
            {
                CastItem();
            }
        }
    }
}
