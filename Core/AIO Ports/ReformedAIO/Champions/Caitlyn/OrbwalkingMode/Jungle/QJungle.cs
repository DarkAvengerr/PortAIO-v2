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

        private readonly QSpell spell;

        public QJungle(QSpell spell)
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

            Menu.AddItem(new MenuItem("Caitlyn.Jungle.Q.Overkill", "Overkill Check").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            if (Mobs == null || !Mobs.IsValid || !CheckGuardians()) return;

            var qPrediction = spell.Spell.GetPrediction(Mobs);

            if (Menu.Item("Caitlyn.Jungle.Q.Overkill").GetValue<bool>() && Mobs.Health < ObjectManager.Player.GetAutoAttackDamage(Mobs) * 4)
            {
                return;
            }

            LeagueSharp.Common.Utility.DelayAction.Add(5, ()=> spell.Spell.Cast(qPrediction.CastPosition));
        }
    }
}
