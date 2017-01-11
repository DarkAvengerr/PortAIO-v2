using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Combo
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Damage;
    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Get_Information.HeroInfo;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class RCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "R";

        private readonly RSpell spell;

        private readonly QSpell qSpell;

        private readonly Damages damages;

        public RCombo(RSpell spell, QSpell qSpell, Damages damages)
        {
            this.spell = spell;
            this.qSpell = qSpell;
            this.damages = damages;
        }

        private HeroInfo info;

        private static AIHeroClient Target => TargetSelector.GetTarget(ObjectManager.Player.AttackRange + 200, TargetSelector.DamageType.Physical);

        private void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            AttackableUnit target = args.Target;

            if (target == null     // <- The
                 || Target == null // <- Fuck
                 || Menu.Item("Vayne.Combo.R.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                 || (Menu.Item("Vayne.Combo.R.Stealth").GetValue<bool>() && !qSpell.Spell.IsReady())
                 || !CheckGuardians())
            {
                return;
            }

            switch (Menu.Item("Vayne.Combo.R.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast();
                    break;
                case 1:

                    if (target.Health > damages.GetComboDamage(Target) * 4 || ObjectManager.Player.HealthPercent <= 20)
                    {
                        return;
                    }

                    spell.Spell.Cast();
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Orbwalking.BeforeAttack -= BeforeAttack;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Orbwalking.BeforeAttack += BeforeAttack;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Combo.R.Mode", "Mode: ").SetValue(new StringList(new[] { "Aggressive", "Safe" })));

            Menu.AddItem(new MenuItem("Vayne.Combo.R.Stealth", "R -> Q").SetValue(true));

            //Menu.AddItem(new MenuItem("Vayne.Combo.R.Count", "Use if X Enemies Near").SetValue(new Slider(3, 1, 5)));

            Menu.AddItem(new MenuItem("Vayne.Combo.R.Mana", "Min Mana %").SetValue(new Slider(10, 0, 100)));

            info = new HeroInfo();
        }
    }
}
