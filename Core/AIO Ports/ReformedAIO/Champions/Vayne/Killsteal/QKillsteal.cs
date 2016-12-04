using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Killsteal
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QKillsteal : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly DashSmart dashSmart;

        public QKillsteal(QSpell spell, DashSmart dashSmart)
        {
            this.spell = spell;
            this.dashSmart = dashSmart;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range + ObjectManager.Player.AttackRange, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null 
                || Target.Health > spell.GetDamage(Target)
                || Target.Distance(ObjectManager.Player) < ObjectManager.Player.AttackRange 
                || Menu.Item("Vayne.Killsteal.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            switch (Menu.Item("Vayne.Killsteal.Q.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                    break;
                case 1:
                    spell.Spell.Cast(dashSmart.Kite(Target.Position.To2D(), spell.Spell.Range).To3D());
                    break;
                case 2:
                    spell.Spell.Cast(dashSmart.ToSafePosition(Target, Target.Position, spell.Spell.Range));
                    break;
            }
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

            Menu.AddItem(new MenuItem("Vayne.Killsteal.Q.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" })));

            Menu.AddItem(new MenuItem("Vayne.Killsteal.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
