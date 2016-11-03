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

    internal sealed class EJungle  : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell eSpell;

        public EJungle(ESpell eSpell)
        {
            this.eSpell = eSpell;
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);
        }

        private void OnUpdate(EventArgs args)
        {
            var mobs = MinionManager.GetMinions(eSpell.Spell.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !CheckGuardians() || mobs.Health < ObjectManager.Player.GetAutoAttackDamage(mobs) * 3 || mobs.CharData.BaseSkinName == "Baron") return;

            var qPrediction = eSpell.Spell.GetPrediction(mobs);

            eSpell.Spell.Cast(qPrediction.CastPosition);
        }
    }
}
