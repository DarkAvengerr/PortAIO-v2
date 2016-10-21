using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeShyvana
{
    using static Program;
    using static extension;
    class jungleclear
    {
        public static void UpdateJungleClear()
        {
            if (EJungleClear && E.IsReady())
            {
                var minion = MinionManager.GetMinions(E.Range,MinionTypes.All,MinionTeam.Neutral,MinionOrderTypes.MaxHealth).FirstOrDefault();
                if (minion != null)
                {
                    E.Cast(minion);
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
        public static void OnAttackJungleClear(AttackableUnit unit, AttackableUnit target)
        {
            if (target.IsValidTarget() && target.Team != GameObjectTeam.Neutral)
                return;
            if (WJungleClear && W.IsReady())
            {
                W.Cast();
            }
        }
    }
}
