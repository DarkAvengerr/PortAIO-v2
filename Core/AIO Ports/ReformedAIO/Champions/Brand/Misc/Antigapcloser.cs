using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Brand.Misc
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Brand.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class BrandAntiGapcloser : OrbwalkingChild
    {
        public override string Name { get; set; } = "Anti-Gapcloser";

        private readonly QSpell spell;

        private readonly ESpell eSpell;

        public BrandAntiGapcloser(QSpell spell, ESpell eSpell)
        {
            this.spell = spell;
            this.eSpell = eSpell;
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!CheckGuardians() 
                || gapcloser.Sender == null 
                || !spell.Spell.IsReady()
                || !gapcloser.Sender.IsValidTarget(spell.Spell.Range))
            {
                return;
            }

            var target = gapcloser.Sender;

            if (!spell.Stunnable(target) && eSpell.Spell.IsReady())
            {
                eSpell.Spell.CastOnUnit(target);
            }

            if (spell.Stunnable(target))
            {
                spell.Spell.Cast(gapcloser.End);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
        }
    }
}
