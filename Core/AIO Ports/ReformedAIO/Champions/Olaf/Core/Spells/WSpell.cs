using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Olaf.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal class WSpell : SpellChild
    {
        public override string Name { get; set; } = "Vicious Strikes";

        public override Spell Spell { get; set; }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.W);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
