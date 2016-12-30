using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Flay";

        public override Spell Spell { get; set; }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public void Push(Obj_AI_Base target)
        {
            Spell.Cast(target.Position);
        }

        public void Pull(Obj_AI_Base target)
        {
            Spell.Cast(target.Position.Extend(ObjectManager.Player.Position, ObjectManager.Player.Distance(target) + 400));
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.E, 400);
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
