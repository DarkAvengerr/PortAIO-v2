using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Lux.Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (spell.IsActive && spell.InRange(Target))
            {
                if (ObjectManager.Player.Distance(Target) < ObjectManager.Player.AttackRange
                    && Target.HasBuff("luxilluminatingfraulein")
                    && Menu.Item("Lux.Combo.E.Proc").GetValue<bool>())
                {
                    return;
                }

                spell.Spell.Cast();
            }
            else
            {
                switch (Menu.Item("Lux.Combo.E.Prediction").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        switch (Menu.Item("Lux.Combo.E.Hitchance").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.High)
                                {
                                    spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                                }
                                break;
                            case 1:
                                if (spell.Prediction(Target).Hitchance >= SebbyLib.Movement.HitChance.VeryHigh)
                                {
                                    spell.Spell.Cast(spell.Prediction(Target).CastPosition);
                                }
                                break;
                        }
                        break;
                    case 1:
                        spell.Spell.Cast(Target);
                        break;
                }
               
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

            Menu.AddItem(new MenuItem("Lux.Combo.E.Proc", "Try To Proc Passive").SetValue(true));

            Menu.AddItem(new MenuItem("Lux.Combo.E.Prediction", "Prediction:").SetValue(new StringList(new[] {"OKTW", "Target Position"}, 1)));

            Menu.AddItem(new MenuItem("Lux.Combo.E.Hitchance", "Hitchance:").SetValue(new StringList(new[] { "High", "Very High" })));

            Menu.AddItem(new MenuItem("Lux.Combo.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
