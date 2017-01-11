using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WHarass(WSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private bool Dangerous()
        {
            return ObjectManager.Player.CountEnemiesInRange(2000) >= 2
                   || (ObjectManager.Player.HealthPercent <= 30 && Target.HealthPercent >= 45);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null)
            {
                return;
            }

            if ((Menu.Item("LeeSin.Harass.W.EnergyLow").GetValue<bool>() && ObjectManager.Player.Mana <= 130)
                || (!Menu.Item("LeeSin.Harass.W.Safety").GetValue<bool>() && Dangerous()))
            {
                spell.Jump(Game.CursorPos,
                     Menu.Item("LeeSin.Harass.W.Minions").GetValue<bool>(),
                     Menu.Item("LeeSin.Harass.W.Allies").GetValue<bool>(),
                     Menu.Item("LeeSin.Harass.W.Ward").GetValue<bool>());
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || !Menu.Item("LeeSin.Harass.W.Shield").GetValue<bool>()
                || !sender.IsMe
                || !args.SData.IsAutoAttack()
                || spell.W1)
            {
                return;
            }

            spell.Spell.Cast();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.EnergyLow", "Escape If Low Energy").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.Safety", "Escape If Dangerous").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.Shield", "Shield Self").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.Minions", "Jump To: Minions").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.Allies", "Jump To: Allies").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Harass.W.Ward", "Jump To: Ward").SetValue(true));
        }
    }
}
