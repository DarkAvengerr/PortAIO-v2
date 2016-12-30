using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";
            
        private readonly QSpell spell;

        public QCombo(QSpell spell)
        {
            this.spell = spell;
        }
      
        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians())
            {
                return;
            }

            switch (spell.Spellstate)
            {
                case QSpell.SpellState.Whirlwind:
                    Whirlwind();
                    break;

                case QSpell.SpellState.DashQ:
                    EQCombo();
                    break;

                case QSpell.SpellState.Standard:
                    Whirlwind();
                    break;
            }
        }

        private void EQCombo()
        {
            if (spell.CanEQ(Target))
            {
                spell.Spell.Cast(Target);
            }
        }

        private void Whirlwind()
        {
            var pred = spell.Spell.GetPrediction(Target);

            switch (Menu.Item("Combo.Q.Hitchance2").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
                    break;
                case 1:
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
                    break;
                case 2:
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
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
           
            Menu.AddItem(new MenuItem("Combo.Q.Hitchance2", "Whirlwind Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));

         //   Menu.AddItem(new MenuItem("Combo.Q.Hitchance", "Q1 Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}