using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ziggs.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ziggs.Core.Damage;
    using ReformedAIO.Champions.Ziggs.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        private readonly ESpell eSpell;

        private readonly QSpell qSpell;

        private readonly ZiggsDamage damage;

        public WCombo(WSpell spell, ESpell eSpell, QSpell qSpell, ZiggsDamage damage)
        {
            this.spell = spell;
            this.eSpell = eSpell;
            this.qSpell = qSpell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(qSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Ziggs.Combo.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Ziggs.Combo.W.Insec").GetValue<bool>() && eSpell.GameobjectLists != null)
            {
                Insec();
            }
            else if (Menu.Item("Ziggs.Combo.W.Jump").GetValue<bool>() 
                     && qSpell.Spell.IsReady()
                     && Target.Distance(ObjectManager.Player) > 850 
                     && Target.Health <= damage.GetComboDamage(Target)
                     && ObjectManager.Player.HealthPercent >= Target.HealthPercent
                     && ObjectManager.Player.ManaPercent >= 20)
            {
                var pred = spell.Prediction(Target).CastPosition;

                spell.Spell.Cast(ObjectManager.Player.Position.Extend(pred, -140));
            }
            else
            {
                Combo();
            }
        }

        private void Insec()
        {
            foreach (var obj in eSpell.GameobjectLists)
            {
                if (obj.Position.Distance(Target.Position) < 350 && ObjectManager.Player.Distance(Target) < 350)
                {
                    var position = obj.Position.Extend(Target.Position, Target.Distance(obj.Position) + 50);

                    if (position.Distance(ObjectManager.Player.Position) < spell.Spell.Width)
                    {
                        return;
                    }

                    spell.Spell.Cast(position);
                }
                else
                {
                   Combo();
                }
            }
        }

        private void Combo()
        {
            var pred = spell.Prediction(Target).CastPosition;

            spell.Spell.Cast(Target.Position.Extend(pred, 140));
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

            Menu.AddItem(new MenuItem("Ziggs.Combo.W.Jump", "W Self").SetValue(false).SetTooltip("Jump to extend Q range"));

         //   Menu.AddItem(new MenuItem("Ziggs.Combo.W.Ult", "W Into Ult (BETA)").SetValue(true));

            Menu.AddItem(new MenuItem("Ziggs.Combo.W.Insec", "W Into Minefield").SetValue(true));

         //   Menu.AddItem(new MenuItem("Ziggs.Combo.W.Hitchance", "Hitchance: ").SetValue(new StringList(new[] { "Medium", "High", "Very High" })));

            Menu.AddItem(new MenuItem("Ziggs.Combo.W.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
