using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.Core.Damage
{
    using LeagueSharp;
   
    using ReformedAIO.Champions.Annie.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class AnnieDamage : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public AnnieDamage(QSpell qSpell, WSpell wSpell, ESpell eSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
            this.eSpell = eSpell;
            this.rSpell = rSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            return qSpell.GetDamage(target)
                + wSpell.GetDamage(target)
                + eSpell.GetDamage(target)
                + rSpell.GetDamage(target);
        }
    }
}
