using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace DevCommom
{
    public class IgniteManager
    {
        public bool HasIgnite;

        private SpellDataInst IgniteSpell = null;

        public IgniteManager()
        {
            this.IgniteSpell = ObjectManager.Player.Spellbook.GetSpell(ObjectManager.Player.GetSpellSlot("SummonerDot"));

            if (this.IgniteSpell != null && this.IgniteSpell.Slot != SpellSlot.Unknown)
                this.HasIgnite = true;
        }

        public bool Cast(AIHeroClient enemy)
        {
            if (!enemy.IsValid || !enemy.IsHPBarRendered || !enemy.IsTargetable || enemy.IsDead)
                return false;

            if (HasIgnite && IsReady() && enemy.IsValidTarget(600))
                return ObjectManager.Player.Spellbook.CastSpell(this.IgniteSpell.Slot, enemy);

            return false;
        }

        public bool IsReady()
        {
            return HasIgnite && this.IgniteSpell.State == SpellState.Ready;
        }

        public bool CanKill(AIHeroClient enemy)
        {
            return HasIgnite && IsReady() && enemy.Health < ObjectManager.Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
        }


    }
}
