using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    using SharpDX;

    internal class QSpell : SpellChild
    {
        public override string Name { get; set; } = "Tumble";

        public override Spell Spell { get; set; }

        private DashSmart dashSmart;

        public Vector3 CastTo(Obj_AI_Base target, double distance)
        {
            return dashSmart.ToSafePosition(target, distance);
        }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
        }

        public bool WStack(Obj_AI_Base target)
        {
            return target.GetBuffCount("vaynesilvereddebuff") >= 1;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Spell = new Spell(SpellSlot.Q, 300);

            dashSmart = new DashSmart();
        }

        protected override void SetSwitch()
        {
            Switch = new UnreversibleSwitch(Menu);
        }
    }
}
