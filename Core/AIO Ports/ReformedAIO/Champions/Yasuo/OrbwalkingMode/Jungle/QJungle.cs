using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        public QJungle(QSpell spell)
        {
            this.spell = spell;
        }

        private float Range => spell.Spell.Range;

        private IOrderedEnumerable<Obj_AI_Base> Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 Range,
                 MinionTypes.All,
                 MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Mob)
            {
                if (ObjectManager.Player.IsDashing() && !spell.CanEQ(m))
                {
                    return;
                }

                switch (spell.Spellstate)
                {
                        case QSpell.SpellState.Whirlwind:
                        Whirldwind(m);
                        break;
                        case QSpell.SpellState.Standard:
                        spell.Spell.Cast(m);
                        break;
                }
            }
        }

        private void Whirldwind(Obj_AI_Base m)
        {
            var pred = spell.Spell.GetPrediction(m, true);

            switch (Menu.Item("Jungle.Q.Hitchance").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
                    break;
                case 1:
                    if (pred.Hitchance >= HitChance.High)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
                    break;
                case 2:
                    if (pred.Hitchance >= HitChance.VeryHigh)
                    {
                        spell.Spell.Cast(pred.CastPosition);
                    }
                    break;
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

            Menu.AddItem(new MenuItem("Jungle.Q.Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));
        }
    }
}