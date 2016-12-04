using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly DashSmart dashSmart;

        public QHarass(QSpell spell, DashSmart dashSmart)
        {
            this.spell = spell;
            this.dashSmart = dashSmart;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range + ObjectManager.Player.AttackRange, TargetSelector.DamageType.Physical);

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Target == null
                 || !sender.IsMe
                 || Menu.Item("Vayne.Harass.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                 || (Menu.Item("Vayne.Harass.Q.Stack").GetValue<bool>() && !spell.WStack(Target))
                 || !CheckGuardians())
            {
                return;
            }

            switch (Menu.Item("Vayne.Harass.Q.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                    break;
                case 1:
                    spell.Spell.Cast(dashSmart.Kite(Target.Position.Extend(Target.Direction, spell.Spell.Range).To2D(), spell.Spell.Range).To3D());
                    break;
                case 2:
                    spell.Spell.Cast(dashSmart.ToSafePosition(Target, Target.Position, spell.Spell.Range));
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Harass.Q.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" })));

            Menu.AddItem(new MenuItem("Vayne.Harass.Q.Stack", "Only Q To Proc W").SetValue(false));

            Menu.AddItem(new MenuItem("Vayne.Harass.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
