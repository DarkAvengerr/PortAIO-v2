using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Combo
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;
    using ReformedAIO.Champions.Gnar.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class RCombo : ChildBase
    {
        private WallDetection wallDetection;

        private GnarState gnarState;

        public override string Name { get; set; } = "R";

        private readonly Orbwalking.Orbwalker orbwalker;

        public RCombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(Spells.R2.Range, TargetSelector.DamageType.Physical);

        private void GameOnUpdate(EventArgs args)
        {
            if (Target == null
                || !Spells.R2.IsReady() 
                || gnarState.Mini
                || Target.IsInvulnerable)
            {
                return;
            }

            var wallPoint = this.wallDetection.GetFirstWallPoint(Target.Position, Vars.Player.Position.Extend(Target.Position, Spells.R2.Range + 55));
            Vars.Player.GetPath(wallPoint);

            if (!wallPoint.IsValid() || wallPoint.Distance(Target.Position) > Spells.R2.Range)
            {
                return;
            }

            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            Spells.R2.Cast(wallPoint);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("RRange", "Range").SetValue(new Slider(590, 0, 590)));

            // Menu.AddItem(new MenuItem("HitCount", "Auto If x Count").SetValue(new Slider(2, 0, 5)));
            gnarState = new GnarState();
            this.wallDetection = new WallDetection();
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
