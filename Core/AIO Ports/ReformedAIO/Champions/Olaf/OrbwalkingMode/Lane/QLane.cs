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

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QLane(QSpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range).FirstOrDefault(m => m.Health < spell.Spell.GetDamage(m) && m.Distance(ObjectManager.Player) > ObjectManager.Player.AttackRange + 200);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (Menu.Item("Enemy").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(2000) > 0))
            {
                return;
            }


            var prediction = spell.Spell.GetPrediction(Mob, true);

            spell.Spell.Cast(prediction.CastPosition.Extend(ObjectManager.Player.Position, -Menu.Item("Distance").GetValue<Slider>().Value));
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

            Menu.AddItem(new MenuItem("Distance", "Shortened Throw Distance").SetValue(new Slider(60, 0, 100)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(70, 0, 100)));
        }
    }
}
