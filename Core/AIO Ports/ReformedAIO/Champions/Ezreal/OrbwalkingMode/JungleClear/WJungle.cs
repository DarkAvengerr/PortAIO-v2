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

    internal sealed class WJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell wSpell;

        public WJungle(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.Position, wSpell.Spell.Range, MinionTypes.All, MinionTeam.Neutral);

            if (mob == null)
            {
                return;
            }

            foreach (var m in mob)
            {
                if (!m.IsValidTarget(900))
                {
                    return;
                }

                var allies = ObjectManager.Player.GetAlliesInRange(wSpell.Spell.Range)
                     .Where(x => !x.IsMe)
                     .FirstOrDefault(x => x.Distance(ObjectManager.Player.Position) <= wSpell.Spell.Range);

                var prediction = wSpell.Spell.GetPrediction(allies);

                if (allies != null && prediction.Hitchance >= HitChance.Medium)
                {
                    wSpell.Spell.Cast(prediction.CastPosition);
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
