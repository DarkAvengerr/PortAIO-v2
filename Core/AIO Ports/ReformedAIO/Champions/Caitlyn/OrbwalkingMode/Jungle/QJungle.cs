using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle  : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell qSpell;

        public QJungle(QSpell qSpell)
        {
            this.qSpell = qSpell;
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

            Menu.AddItem(new MenuItem("QOverkill", "Overkill Check").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            var mobs = MinionManager.GetMinions(qSpell.Spell.Range, MinionTypes.All, MinionTeam.Neutral,MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid || !CheckGuardians()) return;

            var qPrediction = qSpell.Spell.GetPrediction(mobs);

            if (Menu.Item("QOverkill").GetValue<bool>() && mobs.Health < ObjectManager.Player.GetAutoAttackDamage(mobs) * 4)
            {
                return;
            }

            LeagueSharp.Common.Utility.DelayAction.Add(5, ()=> qSpell.Spell.Cast(qPrediction.CastPosition));

        }
    }
}
