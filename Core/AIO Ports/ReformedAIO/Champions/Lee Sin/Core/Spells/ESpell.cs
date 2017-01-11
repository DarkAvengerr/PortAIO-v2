using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.Core.Spells
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.FeatureSystem.Switches;

    internal sealed class ESpell : SpellChild
    {
        public override string Name { get; set; } = "Tempest";

        public override Spell Spell { get; set; }

        public bool E1 => Spell.Instance.SData.Name.ToLower().Contains("one");

        public int PassiveStacks => ObjectManager.Player.HasBuff("blindmonkpassive_cosmetic")
                                      ? ObjectManager.Player.GetBuff("blindmonkpassive_cosmetic").Count
                                      : 0;

        private int Item => Items.CanUseItem(3077) && Items.HasItem(3077)
                            ? 3077 
                            : Items.CanUseItem(3074) && Items.HasItem(3074)
                            ? 3074
                            : Items.CanUseItem(3748) && Items.HasItem(3748)
                            ? 3748
                            : 0;


        public bool ShouldE2(Obj_AI_Base target)
        {
            return target.GetBuff("BlindMonkTempest").EndTime - Game.Time <= .1f;
        }

        public void CastItem()
        {
            if (Items.CanUseItem(Item) && Item != 0)
            {
                Items.UseItem(Item);
            }
        }

        public float GetDamage(Obj_AI_Base target)
        {
            return Spell.GetDamage(target);
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
