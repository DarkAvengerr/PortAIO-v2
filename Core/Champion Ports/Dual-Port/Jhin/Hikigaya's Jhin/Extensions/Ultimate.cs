using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using  Jhin___The_Virtuoso.Extensions;
using LeagueSharp;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Jhin___The_Virtuoso.Extensions
{
    static class Ultimate
    {
        public static void ComboUltimate()
        {
            if (ObjectManager.Player.IsActive(Spells.R))
            {
                if (Menus.Config.Item("auto.shoot.bullets").GetValue<bool>())
                {
                    var blocked = HeroManager.Enemies.Where(x => !Menus.Config.Item("r.combo." + x.ChampionName).GetValue<bool>());
                    var tstarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical, false, blocked);
                    if (tstarget != null)
                    {
                        var pred = Spells.R.GetPrediction(tstarget);
                        if (pred.Hitchance >= Menus.Config.HikiChance("r.hit.chance"))
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (Spells.R.IsReady() && Menus.Config.Item("semi.manual.ult").GetValue<KeyBind>().Active)
                {
                    var blocked = HeroManager.Enemies.Where(x => !Menus.Config.Item("r.combo." + x.ChampionName).GetValue<bool>());
                    var tstarget = TargetSelector.GetTarget(Spells.R.Range, TargetSelector.DamageType.Physical, false, blocked);
                    if (tstarget != null)
                    {
                        var pred = Spells.R.GetPrediction(tstarget);
                        if (pred.Hitchance >= Menus.Config.HikiChance("r.hit.chance"))
                        {
                            Spells.R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
        }
    }
}
