using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class LuxDamage : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public LuxDamage(QSpell qSpell, ESpell eSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.eSpell = eSpell;
            this.rSpell = rSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            float autoDmg = target.HasBuff("luxilluminatingfraulein")
                            ? (float)ObjectManager.Player.GetAutoAttackDamage(target, true)
                            : (float)ObjectManager.Player.GetAutoAttackDamage(target);

            if (qSpell.Spell.IsReady())
            {
                comboDmg += qSpell.GetDamage(target);

                comboDmg += autoDmg;
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += eSpell.Spell.GetDamage(target);

                comboDmg += autoDmg;
            }

            if (rSpell.Spell.IsReady())
            {
                comboDmg += rSpell.Spell.GetDamage(target);
            }

            return comboDmg;
        }
    }
}
