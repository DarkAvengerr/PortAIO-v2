using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.Antigapcloser
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class VayneAntiGapcloser : OrbwalkingChild
    {
        public override string Name { get; set; } = "AntiGapcloser";

        private readonly QSpell spell;

        private readonly ESpell eSpell;

        public VayneAntiGapcloser(QSpell qspell, ESpell espell)
        {
            this.spell = qspell;
            this.eSpell = espell;
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!CheckGuardians() || gapcloser.Sender == null)
            {
                return;
            }

            var target = gapcloser.Sender;

            var dashPos = ObjectManager.Player.ServerPosition + (ObjectManager.Player.ServerPosition - target.ServerPosition).Normalized() * 300;

            switch (Menu.Item("Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (spell.Spell.IsReady())
                    {
                        spell.Spell.Cast(dashPos);
                    }
                    break;
                case 1:
                    if (eSpell.Spell.IsReady())
                    {
                        eSpell.Spell.Cast(target);
                    }
                    break;
                case 2:
                    if (!spell.Spell.IsReady() && eSpell.Spell.IsReady())
                    {
                        eSpell.Spell.Cast(target);
                    }
                    else
                    {
                        spell.Spell.Cast(dashPos);
                    }
                    break;
            }

            if (target.IsValidTarget(spell.Spell.Range))
            {
                spell.Spell.Cast(dashPos);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Mode", "Mode: ").SetValue(new StringList(new[] { "Q", "E", "Automatic"}, 2)));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
        }
    }
}
