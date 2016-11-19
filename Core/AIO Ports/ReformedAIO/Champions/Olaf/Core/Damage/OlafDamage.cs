using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Olaf.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Olaf.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class OlafDamage : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        public OlafDamage(ESpell eSpell, WSpell wSpell, QSpell qSpell)
        {
            this.eSpell = eSpell;
            this.wSpell = wSpell;
            this.qSpell = qSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            comboDmg += qSpell.GetDamage(target);

            if (wSpell.Spell.IsReady())
            {
                comboDmg += (float)ObjectManager.Player.GetAutoAttackDamage(target, true) * 3;
            }


            comboDmg += eSpell.Spell.GetDamage(target);

            return comboDmg;
        }
    }
}
