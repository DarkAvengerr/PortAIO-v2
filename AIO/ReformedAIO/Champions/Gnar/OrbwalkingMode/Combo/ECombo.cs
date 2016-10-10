using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class ECombo : ChildBase
    {
        private GnarState gnarState;

        private Dmg dmg;

        public override string Name { get; set; } = "E";

        private readonly Orbwalking.Orbwalker orbwalker;

        public ECombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(Menu.Item("E1Range").GetValue<Slider>().Value * 2, TargetSelector.DamageType.Physical);

            if (target == null || target.UnderTurret(true))
            {
                return;
            }

            if (Spells.E.IsReady() && !this.gnarState.Mega)
            {
                if (gnarState.TransForming
                    || target.Health < dmg.GetDamage(target) * 1.35 
                    || (Menu.Item("EOnGanked").GetValue<bool>()
                    && Vars.Player.CountAlliesInRange(900) > Vars.Player.CountEnemiesInRange(900))
                    || Spells.R2.IsReady())
                {
                    var ePred = Spells.E.GetPrediction(target);

                    if (target.Distance(ObjectManager.Player) > 500f)
                    {
                        var m = MinionManager.GetMinions(ObjectManager.Player.Position, 425).LastOrDefault();

                        if (m.UnderTurret(true))
                        {
                            return;
                        }

                        if (Vars.Player.IsFacing(m) && m.Distance(Vars.Player) >= 350)
                        {
                            Spells.E.Cast(m);
                        }
                    }
                    else
                    {
                        Spells.E.Cast(ePred.CastPosition);
                    }
                }
            }

            if (!this.gnarState.Mega || !Spells.E2.IsReady())
            {
                return;
            }

            var e2Pred = Spells.E2.GetPrediction(target);

            Spells.E2.Cast(e2Pred.CastPosition);
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;

            if (target == null || !Menu.Item("EAwayMelee").GetValue<bool>())
            {
                return;
            }

            var ePred = Spells.E.GetPrediction(target);

            if (ePred.CollisionObjects[0].Position.Distance(Vars.Player.Position) > 425)
            {
                return;
            }

            Spells.E.Cast(ePred.CollisionObjects[0].Position);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("E1Range", "Range").SetValue(new Slider(475, 0, 475)));
            Menu.AddItem(new MenuItem("EAwayMelee", "E Away From Melee's").SetValue(false));
            Menu.AddItem(new MenuItem("EonTransform", "E On Transformation").SetValue(true));
            Menu.AddItem(new MenuItem("EOnGanked", "E If Ganked Or Teamfight").SetValue(true));

            dmg = new Dmg();
            gnarState = new GnarState();
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= GameOnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += GameOnUpdate;
        }
    }
}