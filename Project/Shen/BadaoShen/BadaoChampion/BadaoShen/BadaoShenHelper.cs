using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using BadaoShen;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoShen
{
    public static class BadaoShenHelper
    {
        //combo
        public static bool UseWCombo(AIHeroClient target)
        {
            return BadaoMainVariables.W.IsReady()
                && BadaoShenConfig.config.SubMenu("Combo").SubMenu("WProtect").Item("WProtect" + target.NetworkId).GetValue<bool>();
        }
        public static bool UseQCombo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoShenVariables.ComboQ.GetValue<bool>();
        }
        public static bool UseEComboToTarget(AIHeroClient target)
        {
            return BadaoMainVariables.E.IsReady()
                && BadaoShenConfig.config.SubMenu("Combo").SubMenu("EToTarget").Item("EToTarget" + target.NetworkId).GetValue<bool>();
        }
        
        //auto
        public static bool UseRAuto(AIHeroClient target)
        {
            return BadaoMainVariables.R.IsReady()
                && BadaoShenConfig.config.SubMenu("Auto").Item("AutoR" + target.NetworkId).GetValue<bool>()
                && target.Health / target.MaxHealth * 100 <= BadaoShenVariables.RHp.GetValue<Slider>().Value
                && HeroManager.Enemies.Any(x => x.BadaoIsValidTarget() && x.Position.To2D().Distance(target.Position.To2D()) <= 900);
        }

        //laneclear
        public static bool UseQLaneClear()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoShenVariables.LaneClearQ.GetValue<bool>();
        }
        //jungleclear
        public static bool UseQJungleClear()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoShenVariables.JungleClearQ.GetValue<bool>();
        }

    }
}
