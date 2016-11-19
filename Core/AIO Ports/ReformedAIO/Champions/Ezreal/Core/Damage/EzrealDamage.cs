using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ezreal.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ezreal.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class EzrealDamage : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public EzrealDamage(ESpell eSpell, WSpell wSpell, QSpell qSpell, RSpell rSpell)
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

            var aaDmg = (float)ObjectManager.Player.GetAutoAttackDamage(target);

            if (qSpell.Spell.IsReady())
            {
                comboDmg += qSpell.GetDamage(target) + aaDmg;
            }

            if (wSpell.Spell.IsReady())
            {
                comboDmg += wSpell.Spell.GetDamage(target);
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += eSpell.Spell.GetDamage(target);
            }

            if (rSpell.Spell.IsReady())
            {
                comboDmg += rSpell.GetDamage(target);
            }

            return comboDmg;
        }
    }
}
