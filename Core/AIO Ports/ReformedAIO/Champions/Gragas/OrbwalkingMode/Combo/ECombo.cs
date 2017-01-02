using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Core.Damage;
    using ReformedAIO.Champions.Gragas.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    using HitChance = SebbyLib.Prediction.HitChance;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        private readonly GragasDamage damage;

        public ECombo(ESpell spell, GragasDamage damage)
        {
            this.spell = spell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range + 480, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Gragas.Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            if (Menu.Item("Gragas.Combo.E.Flash").GetValue<bool>()
                && damage.GetComboDamage(Target) * 1.15 > Target.Health 
                && spell.Flash.IsReady()
                && Target.Distance(ObjectManager.Player) < 800)
            {
                ObjectManager.Player.Spellbook.CastSpell(spell.Flash, Target.Position);
                spell.Spell.Cast(Target.Position);
            }
           
            else
            {
                switch (Menu.Item("Gragas.Combo.E.Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (spell.OKTW(Target).Hitchance >= HitChance.High)
                        {
                            spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                        }
                        break;
                    case 1:
                        if (spell.OKTW(Target).Hitchance >= HitChance.VeryHigh)
                        {
                            spell.Spell.Cast(spell.OKTW(Target).CastPosition);
                        }
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

            Menu.AddItem(new MenuItem("Gragas.Combo.E.Flash", "Flash Combo Killable (BETA)").SetValue(true));

            Menu.AddItem(new MenuItem("Gragas.Combo.E.Hitchance", "Hitchance:").SetValue(new StringList(new[] {"High", "Very High"})));

            Menu.AddItem(new MenuItem("Gragas.Combo.E.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
