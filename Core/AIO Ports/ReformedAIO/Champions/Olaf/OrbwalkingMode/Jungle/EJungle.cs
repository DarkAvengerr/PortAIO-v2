using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Olaf.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public EJungle(ESpell spell)
        {
            this.spell = spell;
        }

        private IOrderedEnumerable<Obj_AI_Base> Mob =>
            MinionManager.GetMinions(ObjectManager.Player.Position,
                spell.Spell.Range,
                MinionTypes.All,
                MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null
                || !CheckGuardians()
                || (Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent))
            {
                return;
            }

            foreach (var m in Mob)
            {
                if ((m.Health <= spell.Spell.GetDamage(m) && Menu.Item("Killable").GetValue<bool>()) || !Menu.Item("Killable").GetValue<bool>())
                {
                    spell.Spell.Cast(m);
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


            Menu.AddItem(new MenuItem("Killable", "Use Only When Killable").SetValue(true));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
