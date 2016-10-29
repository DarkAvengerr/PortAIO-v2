using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class LucDamage : ChildBase
    {
        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public LucDamage(ESpell eSpell, WSpell wSpell, QSpell qSpell, RSpell rSpell)
        {
            this.eSpell = eSpell;
            this.wSpell = wSpell;
            this.qSpell = qSpell;
            this.rSpell = rSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            var aaDmg = (float)ObjectManager.Player.GetAutoAttackDamage(target, true);

            if (qSpell.Spell.IsReady())
            {
                comboDmg += qSpell.GetDamage(target) + aaDmg;
            }

            if (wSpell.Spell.IsReady())
            {
                comboDmg += wSpell.Spell.GetDamage(target) + aaDmg;
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += aaDmg;
            }

            if (target.Distance(ObjectManager.Player) >= 900 && rSpell.Spell.IsReady())
            {
                comboDmg += rSpell.GetDamage(target);
            }

            return comboDmg;
        }

        public override string Name { get; set; } = "Damage";
    }
}
