using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Lane
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELane : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ELane(ESpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Mob =>
            MinionManager.GetMinions(ObjectManager.Player.Position,
                spell.Spell.Range).FirstOrDefault(m => m.Health < spell.Spell.GetDamage(m));

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (Menu.Item("Enemy").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(2000) > 0))
            {
                return;
            }


            spell.Spell.Cast(Mob);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Enemy", "Return If Nearby Enemy").SetValue(true));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
        }
    }
}
