using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        private readonly ESpell eSpell;

        private readonly QSpell qSpell;

        public WHarass(WSpell spell, ESpell eSpell, QSpell qSpell)
        {
            this.spell = spell;
            this.eSpell = eSpell;
            this.qSpell = qSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Ziggs.Harass.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Ziggs.Harass.W.Insec").GetValue<bool>() && eSpell.GameobjectLists != null)
            {
                Insec();
            }
            else if (Menu.Item("Ziggs.Harass.W.Jump").GetValue<bool>()
                     && qSpell.Spell.IsReady()
                     && Target.Distance(ObjectManager.Player) > 850
                     && Target.HealthPercent <= 10
                     && ObjectManager.Player.HealthPercent >= 30
                     && ObjectManager.Player.ManaPercent >= 15)
            {
                var position = ObjectManager.Player.ServerPosition + (ObjectManager.Player.ServerPosition - Target.Position).Normalized() * 200;
                spell.Spell.Cast(position);
            }
            else
            {
                Harass();
            }
        }

        private void Insec()
        {
            foreach (var obj in eSpell.GameobjectLists)
            {
                if (obj.Position.Distance(Target.Position) > 350 || ObjectManager.Player.Distance(Target) > 350) return;

                var position = obj.Position.Extend(Target.Position, Target.Distance(obj.Position) + 50);

                if (position.Distance(ObjectManager.Player.Position) < spell.Spell.Width)
                {
                    return;
                }

                spell.Spell.Cast(position);
            }
        }

        private void Harass()
        {
            var prediction = ObjectManager.Player.ServerPosition.Extend(spell.Spell.GetPrediction(Target).CastPosition, spell.Spell.Range);

            spell.Spell.Cast(prediction);
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

            Menu.AddItem(new MenuItem("Ziggs.Harass.W.Jump", "W Self").SetValue(false).SetTooltip("Jump to extend Q range"));

            //   Menu.AddItem(new MenuItem("Ziggs.Harass.W.Ult", "W Into Ult (BETA)").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Harass.W.Insec", "W Into Minefield").SetValue(true));

            //   Menu.AddItem(new MenuItem("Ziggs.Harass.W.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Harass.W.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
