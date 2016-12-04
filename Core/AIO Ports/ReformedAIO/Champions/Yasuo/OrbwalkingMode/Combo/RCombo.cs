using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Damage;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using Yasuo.Core.Spells;

    using RethoughtLib.Extensions;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly YasuoDamage damage;

        public RCombo(RSpell spell, YasuoDamage damage)
        {
            this.spell = spell;
            this.damage = damage;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null 
                || !CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && Target.UnderTurret(true))
                || (Menu.Item("Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(spell.Spell.Range))
                || (Menu.Item("Killable").GetValue<bool>() && Target.Health > damage.GetComboDamage(Target) && !Menu.Item("Always").GetValue<bool>()))
            {
                return;
            }
            
          
            if (spell.IsAirbone(Target) && spell.RemainingAirboneTime(Target) < 5)
            {
                spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("Always", "Use Always (1v1)").SetValue(true));

            Menu.AddItem(new MenuItem("Killable", "Force When Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Turret", "Don't R Into Turret").SetValue(true));

            Menu.AddItem(new MenuItem("Enemies", "Use When X Enemies").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("cDelay", "Combo Delay (Hotfix)").SetValue(new Slider(200, 50, 400)));
        }
    }
}