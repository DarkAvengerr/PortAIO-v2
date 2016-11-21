using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
using LeagueSharp.Common; 
 namespace SergixBrand
{
    public class Spells
    {
        public Spell GetQ { get; set; }

        public Spell GetW { get; set; }

        public Spell GetE { get; set; }

        public Spell GetR { get; set; }

        private SpellSlot ignite;

        public Spells()
        {
            LoadSpells();
        }
        public virtual void LoadSpells()
        {
            ignite = ObjectManager.Player.GetSpellSlot("SummonerDot");
        }
        public bool IgniteCast(Obj_AI_Base target)
        {
            if (target == null) return false;
            if (ignite.IsReady() && target.Health - ObjectManager.Player.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite) <= 0)
            {
                ObjectManager.Player.Spellbook.CastSpell(ignite, target);
                return true;
            }
            return false;
        }



        public virtual bool castQ(Core core)
        {
            return false;
        }

        public virtual bool castW(Core core)
        {
            return false;
        }

        public virtual bool castE(Core core)
        {
            return false;
        }

        public virtual bool castR(Core core)
        {
            return false;
        }
    }
}