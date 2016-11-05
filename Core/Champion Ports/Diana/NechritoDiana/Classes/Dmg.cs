using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Diana
{
    class Dmg
    {
        public static float SmiteDamage(AIHeroClient target)
        {
            if (Logic.Smite == SpellSlot.Unknown || Program.Player.Spellbook.CanUseSpell(Logic.Smite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Program.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
        }
        public static float IgniteDamage(AIHeroClient target)
        {
            if (Spells.Ignite == SpellSlot.Unknown || Program.Player.Spellbook.CanUseSpell(Spells.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Program.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                
                if (Program.Player.Masteries.Equals("thunderlordsdecree")) damage += (float)Program.Player.GetAutoAttackDamage(enemy) * (1.05f);
                // dianapassivebuff or dianamoonlight, cba to actually check yet. Also showing too much dmg on dmg indicator, like 30% too much

                if (Program.Player.HasBuff("dianapassivebuff"))
                {
                    if (Spells.R.IsReady() && Spells.Q.IsReady())
                        damage += Spells.Q.GetDamage(enemy) + Spells.R.GetDamage(enemy) +
                            Spells.R.GetDamage(enemy) + (float)Program.Player.GetAutoAttackDamage(enemy);
                }
                damage = damage + (float)Program.Player.GetAutoAttackDamage(enemy);

                if (Spells.Q.IsReady()) damage += Spells.Q.GetDamage(enemy);
                if (Spells.W.IsReady()) damage += Spells.W.GetDamage(enemy);
                if (Spells.R.IsReady()) damage += Spells.R.GetDamage(enemy);
                return damage;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
    }
}
