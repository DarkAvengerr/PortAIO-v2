using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace BadaoKingdom.BadaoChampion.BadaoPoppy
{
    public static class BadaoPoppyHelper
    {
        // can use skills
        public static bool UseQCombo()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.ComboQ.GetValue<bool>();
        }
        public static bool UseWCombo()
        {
            return BadaoMainVariables.W.IsReady() && BadaoPoppyVariables.ComboW.GetValue<bool>();
        }
        public static bool UseEComboGap()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.ComboE.GetValue<bool>();
        }
        public static bool UseECombo(AIHeroClient target)
        {
            return BadaoMainVariables.E.IsReady()
                && BadaoPoppyConfig.config.SubMenu("Combo").Item("ComboE" + target.NetworkId).GetValue<bool>();
        }
        public static bool UseRComboKillable()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.ComboRKillable.GetValue<bool>();
        }
        public static bool UseQHarass()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.HarassQ.GetValue<bool>();
        }
        public static bool UseQJungle()
        {
            return BadaoMainVariables.Q.IsReady() && BadaoPoppyVariables.JungleQ.GetValue<bool>();
        }
        public static bool UseEJungle()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.JungleE.GetValue<bool>();
        }
        public static bool ManaJungle()
        {
            return BadaoPoppyVariables.JungleMana.GetValue<Slider>().Value
                <= ObjectManager.Player.Mana * 100 / ObjectManager.Player.MaxMana;
        }
        public static bool AssasinateActive()
        {
            return BadaoPoppyVariables.AssassinateKey.GetValue<KeyBind>().Active;
        }
        public static bool UseEAutoInterrupt()
        {
            return BadaoMainVariables.E.IsReady() && BadaoPoppyVariables.AutoEInterrupt.GetValue<bool>();
        }
        public static bool UseRAutoKS()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoRKS.GetValue<bool>();
        }
        public static bool UseRAutoInterrupt()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoRInterrupt.GetValue<bool>();
        }
        public static bool UseRAuto3Target()
        {
            return BadaoMainVariables.R.IsReady() && BadaoPoppyVariables.AutoR3Target.GetValue<bool>();
        }
        public static bool UseWAutoAntiDash(AIHeroClient target)
        {
            return BadaoMainVariables.W.IsReady()
                && BadaoPoppyConfig.config.SubMenu("Auto").Item("AutoAntiDash" + target.NetworkId).GetValue<bool>();
        }
    }
}
