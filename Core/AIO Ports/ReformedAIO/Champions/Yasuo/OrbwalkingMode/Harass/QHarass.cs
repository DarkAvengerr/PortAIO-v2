using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Harass
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private HeroInfo heroInfo;

        private readonly Q1Spell qSpell;

        private readonly Q3Spell q3Spell;

        public QHarass(Q1Spell qSpell, Q3Spell q3Spell)
        {
            this.qSpell = qSpell;
            this.q3Spell = q3Spell;
        }

        private float Range => ObjectManager.Player.HasBuff("YasuoQ3W") ? q3Spell.Spell.Range : qSpell.Spell.Range;

        private AIHeroClient Target => TargetSelector.GetTarget(Range, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null || !CheckGuardians())
            {
                return;
            }

            if (q3Spell.Active && heroInfo.GetStunDuration(Target) < q3Spell.Spell.Delay)
            {
                switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        q3Spell.Spell.CastIfHitchanceEquals(Target, HitChance.Medium);
                        break;
                    case 1:
                        q3Spell.Spell.CastIfHitchanceEquals(Target, HitChance.High);
                        break;
                    case 2:
                        q3Spell.Spell.CastIfHitchanceEquals(Target, HitChance.VeryHigh);
                        break;
                }
            }
            else
            {
                switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        qSpell.Spell.CastIfHitchanceEquals(Target, HitChance.Medium);
                        break;
                    case 1:
                        qSpell.Spell.CastIfHitchanceEquals(Target, HitChance.High);
                        break;
                    case 2:
                        qSpell.Spell.CastIfHitchanceEquals(Target, HitChance.VeryHigh);
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

            heroInfo = new HeroInfo();

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}