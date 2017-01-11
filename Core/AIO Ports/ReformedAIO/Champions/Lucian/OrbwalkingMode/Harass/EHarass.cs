using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lucian.OrbwalkingMode.Harass
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lucian.Damage;
    using ReformedAIO.Champions.Lucian.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly LucDamage damage;

        private readonly ESpell eSpell;

        private readonly DashSmart dashSmart;

        public EHarass(ESpell eSpell, LucDamage damage, DashSmart dashSmart)
        {
            this.eSpell = eSpell;
            this.damage = damage;
            this.dashSmart = dashSmart;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                || !CheckGuardians()
                || Menu.Item("Harass.E.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget(Orbwalking.GetRealAutoAttackRange(ObjectManager.Player) + Menu.Item("Harass.E.Distance").GetValue<Slider>().Value));

            foreach (var target in heroes as AIHeroClient[] ?? heroes.ToArray())
            {
                switch (Menu.Item("Harass.E.Mode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        eSpell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, Menu.Item("Harass.E.Distance").GetValue<Slider>().Value));
                        break;
                    case 1:
                        eSpell.Spell.Cast(dashSmart.Kite(target.Position.To2D(), Menu.Item("Harass.E.Distance").GetValue<Slider>().Value).To3D());
                        break;
                    case 2:
                        eSpell.Spell.Cast(dashSmart.ToSafePosition(target, Menu.Item("Harass.E.Distance").GetValue<Slider>().Value));
                        break;
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Harass.E.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" }, 2)));
            Menu.AddItem(new MenuItem("Harass.E.Execute", "Dive E If Killable").SetValue(true));
            Menu.AddItem(new MenuItem("Harass.E.Distance", "E Distance").SetValue(new Slider(65, 1, 425)).SetTooltip("Less = Faster"));
            Menu.AddItem(new MenuItem("Harass.E.Mana", "Min Mana %").SetValue(new Slider(5, 0, 100)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }
    }
}
