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

    internal sealed class WJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WJungle(WSpell spell)
        {
            this.spell = spell;
        }

        private static IOrderedEnumerable<Obj_AI_Base> Mob =>
            MinionManager.GetMinions(ObjectManager.Player.Position,
                ObjectManager.Player.AttackRange,
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
                if (m.Health >= ObjectManager.Player.GetAutoAttackDamage(m) * 3)
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

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
