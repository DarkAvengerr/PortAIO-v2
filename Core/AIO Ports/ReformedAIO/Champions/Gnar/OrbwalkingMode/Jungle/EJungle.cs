using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "E";
     
        private void GameOnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            if (this.gnarState.Mini)
            {
                Mini();
            }
            else
            {
                Mega();
            }
        }

        private void Mini()
        {
            if (!Spells.E.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking || (Menu.Item("BlockIfTransforming").GetValue<bool>()
                && this.gnarState.TransForming))
            {
                return;
            }

            var m = MinionManager.GetMinions(100, MinionTypes.All, MinionTeam.Neutral).FirstOrDefault();

            var prediction = Spells.E.GetPrediction(m, true);

            if (prediction.Hitchance >= HitChance.High)
            {
                Spells.E.Cast(prediction.CastPosition);
            }
        }

        private void Mega()
        {
            if (!Spells.E2.IsReady())
            {
                return;
            }

            foreach (var m in MinionManager.GetMinions(155, MinionTypes.All, MinionTeam.Neutral))
            {
                var prediction = Spells.E2.GetPrediction(m, true);

                if (prediction.Hitchance >= HitChance.Medium)
                {
                    Spells.E2.Cast(prediction.CastPosition);
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            this.gnarState = new GnarState();

            Menu.AddItem(new MenuItem("BlockIfTransforming", "Block If Transforming").SetValue(true));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += GameOnUpdate;
        }
    }
}
