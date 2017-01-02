using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lux.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lux.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class WCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WCombo(WSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Lux.Combo.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var allies = ObjectManager.Player.GetAlliesInRange(spell.Spell.Range).Where(x => !x.IsMe).OrderBy(x => x.Health);

            var ally = allies.FirstOrDefault() as Obj_AI_Base;

            if (Menu.Item("Lux.Combo.W.Hit").GetValue<Slider>().Value >= allies.Count() && spell.Prediction(ally).Hitchance >= HitChance.High)
            {
                spell.Spell.Cast(spell.Prediction(ally).CastPosition);
            }

            else if (Menu.Item("Lux.Combo.W.Health").GetValue<Slider>().Value >= ObjectManager.Player.HealthPercent)
            {
                spell.Spell.Cast(Game.CursorPos);
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

            Menu.AddItem(new MenuItem("Lux.Combo.W.Hit", "Min Allies Hit:").SetValue(new Slider(2, 1, 5)));

            Menu.AddItem(new MenuItem("Lux.Combo.W.Health", "Self HP %").SetValue(new Slider(45, 0, 100)));

            Menu.AddItem(new MenuItem("Lux.Combo.W.Mana", "Min Mana %").SetValue(new Slider(15, 0, 100)));
        }
    }
}
