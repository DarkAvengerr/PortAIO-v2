using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Damage;
    using ReformedAIO.Champions.Lucian.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly LucDamage damage;

        private readonly ESpell eSpell;

        private readonly DashSmart dashSmart;

        public ECombo(ESpell eSpell, LucDamage damage, DashSmart dashSmart)
        {
            this.eSpell = eSpell;
            this.damage = damage;
            this.dashSmart = dashSmart;
        }

        private void OnUpdate(EventArgs args)
        {
            if (Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange + Menu.Item("EDistance").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);

            if (!Menu.Item("Execute").GetValue<bool>() || target == null || target.Health > (damage.GetComboDamage(target) * 1.3))
            {
                return;
            }

            if (Menu.Item("EMode").GetValue<StringList>().SelectedIndex == 0)
            {
                eSpell.Spell.Cast(dashSmart.ToSafePosition(target, target.Position, Menu.Item("EDistance").GetValue<Slider>().Value));
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                || !CheckGuardians()
                || Menu.Item("EMana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
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
                        eSpell.Spell.Cast(dashSmart.Kite(target.Position.To2D(), Menu.Item("EDistance").GetValue<Slider>().Value).To3D());
                        break;
                    case 2:
                        eSpell.Spell.Cast(dashSmart.ToSafePosition(target, target.Position, Menu.Item("EDistance").GetValue<Slider>().Value));
                        break;
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("EMode", "Mode").SetValue(new StringList(new [] {"Cursor", "Kite", "Automatic"}, 2)));
            Menu.AddItem(new MenuItem("Execute", "Dive E If Killable").SetValue(true));
            Menu.AddItem(new MenuItem("EDistance", "E Distance").SetValue(new Slider(65, 1, 425)).SetTooltip("Less = Faster"));
            Menu.AddItem(new MenuItem("EMana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }
    }
}
