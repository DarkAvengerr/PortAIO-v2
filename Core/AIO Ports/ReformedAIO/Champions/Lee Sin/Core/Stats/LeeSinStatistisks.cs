using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Core.Damage
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class LeeSinStatistisks : ChildBase
    {
        public override string Name { get; set; } = "Damage";

        private readonly QSpell qSpell;

        private readonly WSpell wSpell;

        private readonly ESpell eSpell;

        private readonly RSpell rSpell;

        public LeeSinStatistisks(QSpell qSpell, WSpell wSpell, ESpell eSpell, RSpell rSpell)
        {
            this.qSpell = qSpell;
            this.wSpell = wSpell;
            this.eSpell = eSpell;
            this.rSpell = rSpell;
        }

        public bool HasQ2(Obj_AI_Base target)
        {
            return target.HasBuff("BlindMonkQOne");
        }

        public float GetComboDamage(Obj_AI_Base target)
        {
            if (target == null) return 0;

            float comboDmg = 0;

            if (qSpell.Spell.IsReady())
            {
                if (qSpell.IsQ1)
                {
                    comboDmg += qSpell.GetDamage(target) + (float)qSpell.Q2Damage(target);
                }
                else
                {
                    comboDmg += qSpell.GetDamage(target);
                }
            }

            if (eSpell.Spell.IsReady())
            {
                comboDmg += eSpell.Spell.GetDamage(target);
            }

            if (rSpell.Spell.IsReady())
            {
                comboDmg += rSpell.Spell.GetDamage(target);
            }

            comboDmg += (float)ObjectManager.Player.GetAutoAttackDamage(target);

            return comboDmg;
        }

        public float EnergyCost(Obj_AI_Base target)
        {
            if (target == null) return 0;

            float energy = 0;

            if (qSpell.Spell.IsReady())
            {
                if (!qSpell.HasQ2(target))
                {
                    energy += 50;
                }
                
                if(qSpell.HasQ2(target))
                {
                    energy += 30;
                }
            }

            if (wSpell.Spell.IsReady())
            {
                if (wSpell.W1)
                {
                    energy += 50;
                }

                else
                {
                    energy += 30;
                }
            }

            if (!eSpell.Spell.IsReady())
            {
                return energy;
            }

            if (eSpell.E1)
            {
                energy += 50;
            }

            else
            {
                energy += 30;
            }

            return energy;
        }
    }
}
