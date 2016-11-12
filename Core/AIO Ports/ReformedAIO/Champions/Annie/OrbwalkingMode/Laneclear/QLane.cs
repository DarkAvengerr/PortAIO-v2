using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Annie.OrbwalkingMode.Laneclear
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

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range).FirstOrDefault(m => m.Health < spell.Spell.GetDamage(m));

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
                || (ObjectManager.Player.CountEnemiesInRange(1750) >= 1 && Menu.Item("Enemy").GetValue<bool>()))
            {
                return;
            }
            
            spell.Spell.Cast(Minion);
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Enemy", "Return If Nearby Enemies").SetValue(false));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(60, 0, 100)));
        }
    }
}
