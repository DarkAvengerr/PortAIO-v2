using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ezreal.OrbwalkingMode.JungleClear
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QJungle(QSpell qSpell)
        {
            this.qSpell = qSpell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.Position, qSpell.Spell.Range, MinionTypes.All, MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

            foreach (var m in mob)
            {
                var prediction = qSpell.Spell.GetPrediction(m);

                switch (Menu.Item("Hitchance").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (prediction.Hitchance >= HitChance.Medium)
                        {
                            qSpell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                    case 1:
                        if (prediction.Hitchance >= HitChance.High)
                        {
                            qSpell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
                    case 2:
                        if (prediction.Hitchance >= HitChance.VeryHigh)
                        {
                            qSpell.Spell.Cast(prediction.CastPosition);
                        }
                        break;
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

            Menu.AddItem(new MenuItem("Hitchance", "Hitchance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 1)));

            Menu.AddItem(new MenuItem("Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
