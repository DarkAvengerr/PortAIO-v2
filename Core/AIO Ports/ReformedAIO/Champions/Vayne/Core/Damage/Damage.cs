using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class Damages : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        public Damages(QSpell qSpell, WSpell wSpell, ESpell eSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
            this.eSpell = eSpell;
        }

        public float GetComboDamage(AIHeroClient target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            if (qSpell.Spell.IsReady())
            {
                comboDmg += qSpell.GetDamage(target);
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += eSpell.Spell.GetDamage(target);
            }

            if (eSpell.WStack(target))
            {
                comboDmg += wSpell.GetDamage(target);
            }
           
            comboDmg += (float)ObjectManager.Player.GetAutoAttackDamage(target, true);
            
            return comboDmg;
        }

        public int DamageCounter(AIHeroClient target)
        {
            return (int)(target.Health / GetComboDamage(target)) + 1;
        }
    }
}
