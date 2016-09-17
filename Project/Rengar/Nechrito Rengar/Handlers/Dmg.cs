using LeagueSharp;
using LeagueSharp.Common;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Nechrito_Rengar.Handlers
{
    class Dmg : Core
    {
        public static float ComboDmg(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (Player.CanAttack) damage = damage + (float)Player.GetAutoAttackDamage(enemy);
           
                if (Champion.W.IsReady()) damage = damage + Champion.W.GetDamage(enemy);

                if (Champion.Q.IsReady()) damage = damage + Champion.Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);

                if (Champion.Q.IsReady() && Player.Mana == 5) damage = damage + Champion.Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy);

                if (Player.Mana == 5) damage = damage + (float)Player.GetAutoAttackDamage(enemy) * 2;

                return damage;
            }
            return 0;
        }
        public static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDmg(unit) / 1.65 >= unit.Health;
        }
        public static float IgniteDamage(AIHeroClient target)
        {
            if (Champion.Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Champion.Ignite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }
        public static float SmiteDamage(AIHeroClient target)
        {
            if (Champion.Smite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Champion.Smite) != SpellState.Ready)
            {
                return 0f;
            }
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
        }
    }
}