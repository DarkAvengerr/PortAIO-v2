using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions._Example.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions._Example.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    class ExampleDamage : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public ExampleDamage(QSpell qSpell, WSpell wSpell, ESpell eSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
            this.eSpell = eSpell;
            this.rSpell = rSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            if (qSpell.Spell.IsReady())
            {
                comboDmg += qSpell.GetDamage(target);
            }

            if (wSpell.Spell.IsReady())
            {
                comboDmg += wSpell.GetDamage(target);
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += eSpell.Spell.GetDamage(target);
            }

            if (rSpell.Spell.IsReady())
            {
                comboDmg += rSpell.Spell.GetDamage(target);
            }

            return comboDmg;
        }
    }
}
