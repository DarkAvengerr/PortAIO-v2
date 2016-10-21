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
    class laneclear
    {
        public static void UpdateLaneClear()
        {
            if (ELaneClear && E.IsReady())
            {
                var minion = MinionManager.GetMinions(E.Range).FirstOrDefault();
                if (minion != null)
                {
                    E.Cast(minion);
                }
            }
        }
        public static void AfterAttackLaneClear(AttackableUnit unit, AttackableUnit target)
        {
            if (target.Team == GameObjectTeam.Neutral)
                return;
            if (QLaneClear && Q.IsReady())
            {
                Q.Cast();
            }
            else if (TiamatLaneClear && HasItem())
            {
                CastItem();
            }
        }
        public static void OnAttackLaneClear(AttackableUnit unit, AttackableUnit target)
        {
            if (target.IsValidTarget() && target.Team == GameObjectTeam.Neutral)
                return;
            if (WLaneClear && W.IsReady())
            {
                W.Cast();
            }
        }
    }
}
