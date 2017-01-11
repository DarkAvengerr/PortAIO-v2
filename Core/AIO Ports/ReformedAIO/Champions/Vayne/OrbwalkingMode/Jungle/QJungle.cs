using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Jungle
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QJungle(QSpell spell)
        {
            this.spell = spell;
        }

        private static List<Obj_AI_Base> Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 ObjectManager.Player.AttackRange,
                 MinionTypes.All,
                 MinionTeam.Neutral,
                 MinionOrderTypes.None);
      
        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians()
                 || Mob == null
                 || !sender.IsMe
                 || !args.SData.IsAutoAttack()
                 || Menu.Item("Vayne.Jungle.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            foreach (var m in Mob)
            {
                if (m == null)
                {
                    return;
                }

                spell.Spell.Cast(spell.CastTo(m, spell.Spell.Range));
            }
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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Jungle.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
