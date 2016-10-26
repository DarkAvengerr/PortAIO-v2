namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class EJungle : ChildBase
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "E";

        private readonly Orbwalking.Orbwalker orbwalker;

        public EJungle(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear)
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
            if (!Spells.E.IsReady() || Vars.Player.Spellbook.IsAutoAttacking || (Menu.Item("BlockIfTransforming").GetValue<bool>()
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

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.gnarState = new GnarState();

            Menu.AddItem(new MenuItem("BlockIfTransforming", "Block If Transforming").SetValue(true));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += GameOnUpdate;
        }
    }
}
