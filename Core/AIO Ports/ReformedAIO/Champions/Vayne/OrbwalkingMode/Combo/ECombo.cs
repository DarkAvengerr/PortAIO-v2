using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Condemn_Logic;
    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        private readonly CondemnTypes condemnTypes;

        public ECombo(ESpell spell, CondemnTypes condemnTypes)
        {
            this.spell = spell;
            this.condemnTypes = condemnTypes;
        }

        private HeroInfo info;
        
        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || Menu.Item("Vayne.Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || info.HasSpellShield(Target)
                || !CheckGuardians())
            {
                return;
            }

            if (Menu.Item("Vayne.Combo.E.Stack").GetValue<bool>() && spell.WStack(Target))
            {
                spell.Spell.CastOnUnit(Target);
            }

            switch (Menu.Item("Vayne.Combo.E.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (condemnTypes.Reformed(Target, Menu.Item("Vayne.Combo.E.Push").GetValue<Slider>().Value, spell.Spell))
                    {
                        spell.Spell.CastOnUnit(Target);
                    }
                    break;

                case 1:
                    if (condemnTypes.Marksman(Target, spell.Spell))
                    {
                        spell.Spell.CastOnUnit(Target);
                    }
                    break;

                case 2:
                    if (condemnTypes.SharpShooter(
                        Target,
                        spell.Spell,
                        Menu.Item("Vayne.Combo.E.Push").GetValue<Slider>().Value))
                    {
                        spell.Spell.CastOnUnit(Target);
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

            Menu.AddItem(new MenuItem("Vayne.Combo.E.Mode", "Mode: ").SetValue(new StringList(new[] { "Reformed", "Marksman", "Sharpshooter"})));

            Menu.AddItem(new MenuItem("Vayne.Combo.E.Stack", "Proc W With E").SetValue(false));

            Menu.AddItem(new MenuItem("Vayne.Combo.E.Push", "Push Distance").SetValue(new Slider(425, 0, 460)));

            Menu.AddItem(new MenuItem("Vayne.Combo.E.Mana", "Min Mana %").SetValue(new Slider(10, 0, 100)));

            info = new HeroInfo();
        }
    }
}
