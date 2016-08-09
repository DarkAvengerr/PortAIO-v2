using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Vi.Common
{
    internal class CommonMath
    {

        public static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;

            //if (ObjectManager.Player.Health >= 20 && ObjectManager.Player.Health <= 50)
            //{
            //    fComboDamage += ObjectManager.Player.TotalAttackDamage*3;
            //}

            //if (ObjectManager.Player.Health > 50)
            //{
            //    fComboDamage += ObjectManager.Player.TotalAttackDamage * 7;
            //}

            var comboMode = Modes.ModeCombo.MenuLocal.Item("Combo.Mode").GetValue<StringList>().SelectedIndex;
            if (Champion.PlayerSpells.Q.IsReady())
            {
                fComboDamage += Champion.PlayerSpells.Q.GetDamage(t) * Champion.PlayerSpells.E.Instance.Ammo;
                fComboDamage += comboMode == 1 ? ObjectManager.Player.TotalAttackDamage : 0;
            }

            if (Champion.PlayerSpells.E.IsReady())
            {
                fComboDamage += Champion.PlayerSpells.E.GetDamage(t) * Champion.PlayerSpells.E.Instance.Ammo;
                fComboDamage += comboMode == 1 ? ObjectManager.Player.TotalAttackDamage : 0;
            }

            if (Champion.PlayerSpells.R.IsReady())
            {
                fComboDamage += Champion.PlayerSpells.R.GetDamage(t);
                fComboDamage += comboMode == 1 ? ObjectManager.Player.TotalAttackDamage : 0;
            }

            if (CommonItems.Youmuu.IsReady())
            {
                fComboDamage += ObjectManager.Player.TotalAttackDamage * 3;
            }

            if (Common.CommonSummoner.IgniteSlot != SpellSlot.Unknown
                && ObjectManager.Player.Spellbook.CanUseSpell(Common.CommonSummoner.IgniteSlot) == SpellState.Ready)
            {
                fComboDamage += ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);
            }

            if (LeagueSharp.Common.Items.CanUseItem(3128))
            {
                fComboDamage += ObjectManager.Player.GetItemDamage(t, Damage.DamageItems.Botrk);
            }

            return (float)fComboDamage;
        }

    }
}
