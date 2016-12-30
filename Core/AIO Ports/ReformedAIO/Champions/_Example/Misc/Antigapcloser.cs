using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions._Example.Misc
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions._Example.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ExampleAntiGapcloser : OrbwalkingChild
    {
        public override string Name { get; set; } = "Anti-Gapcloser";

        private readonly QSpell spell;

        public ExampleAntiGapcloser(QSpell spell)
        {
            this.spell = spell;
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!CheckGuardians() || gapcloser.Sender == null)
            {
                return;
            }

            if (gapcloser.End.Distance(ObjectManager.Player.Position) <= spell.Spell.Range)
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
