using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle  : OrbwalkingChild
    {
        public override string Name { get; set; }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("QOverkill", "Overkill Check").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            var mobs = MinionManager.GetMinions(Spells.Spell[SpellSlot.E].Range, MinionTypes.All, MinionTeam.Neutral,MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (mobs == null || !mobs.IsValid || !CheckGuardians()) return;

            var qPrediction = Spells.Spell[SpellSlot.Q].GetPrediction(mobs);

            if (Menu.Item("QOverkill").GetValue<bool>() && mobs.Health < Vars.Player.GetAutoAttackDamage(mobs) * 4)
            {
                return;
            }

            LeagueSharp.Common.Utility.DelayAction.Add(5, ()=> Spells.Spell[SpellSlot.Q].Cast(qPrediction.CastPosition));

        }
    }
}
