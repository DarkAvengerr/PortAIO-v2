using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Damage;
    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly LucDamage damage;

        private readonly ESpell eSpell;

        public ECombo(ESpell eSpell, LucDamage damage)
        {
            this.eSpell = eSpell;
            this.damage = damage;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange + eSpell.Spell.Range, TargetSelector.DamageType.Physical);

            if (!Menu.Item("Execute").GetValue<bool>() || target == null || target.Health > (damage.GetComboDamage(target) * 1.3))
            {
                return;
            }

            if (Menu.Item("EMode").GetValue<StringList>().SelectedIndex == 0)
            {
                eSpell.Spell.Cast(
                    ObjectManager.Player.Position.Extend(
                        Game.CursorPos,
                        Menu.Item("EDistance").GetValue<Slider>().Value));
            }
        }

        private void AfterAttack(AttackableUnit unit, AttackableUnit attackableunit)
        {
            if (Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent 
                || !CheckGuardians())
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + Menu.Item("EDistance").GetValue<Slider>().Value));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                switch (Menu.Item("EMode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        eSpell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, Menu.Item("EDistance").GetValue<Slider>().Value));
                        break;
                    case 1:
                        eSpell.Spell.Cast(eSpell.Deviation(ObjectManager.Player.Position.To2D(), target.Position.To2D(), Menu.Item("EDistance").GetValue<Slider>().Value).To3D());
                        break;
                }
            }
        }



        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EMode", "Mode").SetValue(new StringList(new [] {"Cursor", "Side"})));
            Menu.AddItem(new MenuItem("Execute", "Dive E If Killable").SetValue(true));
            Menu.AddItem(new MenuItem("EDistance", "E Distance").SetValue(new Slider(65, 1, 425)).SetTooltip("Less = Faster"));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
            Orbwalking.AfterAttack -= AfterAttack;
           
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            // Orbwalking.AfterAttack += AfterAttack;
        }
    }
}
