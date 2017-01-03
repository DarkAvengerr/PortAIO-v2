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
            if (Menu.Item("Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var target = TargetSelector.GetTarget(ObjectManager.Player.AttackRange + Menu.Item("Combo.E.Distance").GetValue<Slider>().Value, TargetSelector.DamageType.Physical);

            if (!Menu.Item("Combo.E.Execute").GetValue<bool>() || target == null || target.Health > damage.GetComboDamage(target) * 1.1)
            {
                return;
            }

            if (Menu.Item("Combo.E.Mode").GetValue<StringList>().SelectedIndex == 0)
            {
                eSpell.Spell.Cast(dashSmart.ToSafePosition(target, Menu.Item("Combo.E.Distance").GetValue<Slider>().Value));
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                || !CheckGuardians()
                || Menu.Item("Combo.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + Menu.Item("Combo.E.Distance").GetValue<Slider>().Value));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                switch (Menu.Item("Combo.E.Mode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        eSpell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, Menu.Item("Combo.E.Distance").GetValue<Slider>().Value));
                        break;
                    case 1:
                        eSpell.Spell.Cast(dashSmart.Kite(target.Position.To2D(), Menu.Item("Combo.E.Distance").GetValue<Slider>().Value).To3D());
                        break;
                    case 2:
                        eSpell.Spell.Cast(dashSmart.ToSafePosition(target, Menu.Item("Combo.E.Distance").GetValue<Slider>().Value));
                        break;
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Combo.E.Mode", "Mode").SetValue(new StringList(new [] {"Cursor", "Kite", "Automatic"}, 2)));
            Menu.AddItem(new MenuItem("Combo.E.Execute", "Dive E If Killable").SetValue(true));
            Menu.AddItem(new MenuItem("Combo.E.Distance", "E Distance").SetValue(new Slider(65, 1, 425)).SetTooltip("Less = Faster"));
            Menu.AddItem(new MenuItem("Combo.E.Mana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
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
    }
}
