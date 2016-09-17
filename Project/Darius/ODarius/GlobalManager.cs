using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ODarius
{
    internal class GlobalManager : Darius
    {
        // this for later XD
        public static int TickCount(AIHeroClient t)
        {
            var buff = t.Buffs.FirstOrDefault(x => x.Name == "dariushemo");
            return buff != null ? buff.Count : 0;
        }

        private static DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        public delegate float DamageToUnitDelegate(AIHeroClient hero);



        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static float GetComboDamage(AIHeroClient enemy)
        {
            var damage = 0d;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady() && Player.Mana >= R.Instance.SData.Mana)
                damage += Player.GetSpellDamage(enemy, SpellSlot.R); //* RCount();

            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            /*
             if (Q.Instance.SData.Mana + W.Instance.SData.Mana
                 + E.Instance.SData.Mana + R.Instance.SData.Mana <= Player.Mana)
                 damage += Q.GetDamage(enemy) + W.GetDamage(enemy) + E.GetDamage(enemy) + (R.GetDamage(enemy)*RCount());
             */

            return (float)damage;
        }

    }
}
