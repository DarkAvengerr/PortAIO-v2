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

        private readonly ESpell spell;

        public EJungle(ESpell spell)
        {
            this.spell = spell;
        }

        private Obj_AI_Base Mobs =>
            MinionManager.GetMinions(ObjectManager.Player.Position,
                spell.Spell.Range,
                MinionTypes.All,
                MinionTeam.Neutral).FirstOrDefault();

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
        }

        private void OnUpdate(EventArgs args)
        {
            if (Mobs == null || !CheckGuardians() || Mobs.Health < ObjectManager.Player.GetAutoAttackDamage(Mobs) * 3 || Mobs.BaseSkinName == "Baron") return;

            var qPrediction = spell.Spell.GetPrediction(Mobs);

            spell.Spell.Cast(qPrediction.CastPosition);
        }
    }
}
