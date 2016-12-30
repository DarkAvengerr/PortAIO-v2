using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Thresh.OrbwalkingMode.Harass
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Thresh.Core.Spells;

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

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || Menu.Item("Thresh.Harass.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var allies = ObjectManager.Player.GetAlliesInRange(spell.Spell.Range).Where(x => !x.IsMe).OrderBy(x => x.Health).FirstOrDefault(x => x.Distance(ObjectManager.Player) <= spell.Spell.Range + 375);

            if (allies == null || (Menu.Item("Thresh.Harass.W.Hooked").GetValue<bool>() && !Target.HasBuff("ThreshQ") && allies.Distance(Target) > ObjectManager.Player.Distance(Target)))
            {
                return;
            }

            spell.Spell.Cast(allies.Position);
        }

        private void OnPing(TacticalMapPingEventArgs args)
        {
            if (!Menu.Item("Thresh.Harass.W.Ganked").GetValue<bool>() || Target == null || !CheckGuardians())
            {
                return;
            }

            var jungler = args.Source as AIHeroClient;

            if (jungler == null
                || jungler.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("Smite")
                || jungler.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("Smite"))
            {
                return;
            }

            spell.Spell.Cast(jungler.Distance(ObjectManager.Player) <= spell.Spell.Range
                ? jungler.Position
                : args.Position.To3D());
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
            TacticalMap.OnPing -= OnPing;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
            TacticalMap.OnPing += OnPing;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Thresh.Harass.W.Safety", "Use To Rescue Ally").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Harass.W.Ganked", "Use If Ganked").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Harass.W.Hooked", "Use If Enemy Hooked").SetValue(true));

            Menu.AddItem(new MenuItem("Thresh.Harass.W.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
