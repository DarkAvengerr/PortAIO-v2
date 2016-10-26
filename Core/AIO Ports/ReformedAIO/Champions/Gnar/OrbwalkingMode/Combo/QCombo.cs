namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Combo
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.Menu;
    using RethoughtLib.Menu.Presets;

    #endregion

    internal sealed class QCombo : ChildBase
    {
        private GnarState gnarState;
      
        public override string Name { get; set; } = "Q";

        private readonly Orbwalking.Orbwalker orbwalker;

        public QCombo(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            if ((Menu.Item(menu.Name + "BlockIfTransforming").GetValue<bool>()
                && Vars.Player.ManaPercent >= 90 && Vars.Player.ManaPercent < 100)
                || Vars.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            if (this.gnarState.Mini)
            {
                Mini();
            }

            if (this.gnarState.Mega)
            {
                Mega();
            }
        }

        private void Mini()
        {
            if (!Spells.Q.IsReady())
            {
                return;
            }

            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Menu.Item(menu.Name + "Q1Range").GetValue<Slider>().Value)))
            {
                var prediction = Spells.Q.GetPrediction(target, true);

                if (Menu.Item(menu.Name + "QHighHitChance").GetValue<bool>() && prediction.Hitchance >= HitChance.High)
                {
                    Spells.Q.Cast(prediction.CastPosition);
                }

                if (Menu.Item(menu.Name + "BetaQ").GetValue<bool>() && prediction.CollisionObjects.Count > 0 && prediction.CollisionObjects[0].CountEnemiesInRange(100) > 0)
                {
                    Spells.Q.Cast(prediction.CollisionObjects[0].Position);
                }
            }
        }

        private void Mega()
        {
            if (!Spells.Q2.IsReady())
            {
                return;
            }

            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Menu.Item(menu.Name + "Q2Range").GetValue<Slider>().Value)))
            {
                var prediction = Spells.Q2.GetPrediction(target, true);

                if (prediction.Hitchance >= HitChance.Medium)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => Spells.Q2.Cast(prediction.CastPosition));
                }

                if (Menu.Item(menu.Name + "BetaQ2").GetValue<bool>() && prediction.CollisionObjects.Count > 0
                    && prediction.CollisionObjects[0].CountEnemiesInRange(100) > 0)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => Spells.Q2.Cast(prediction.CollisionObjects[0].Position));
                }
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.gnarState = new GnarState();

            var selecter = new MenuItem("Form", "Form").SetValue(new StringList(new[] { "Mini", "Mega" }));

            var mini = new List<MenuItem>
            {
                 new MenuItem("Q1Range", "Range").SetValue(new Slider(1100, 0, 1100)),
                 new MenuItem("QHighHitChance", "Q Frequently").SetValue(true),
                 new MenuItem("BetaQ", "Allow Collision").SetValue(true).SetTooltip("Will Q On Minions Near Target"),
                 new MenuItem("BlockIfTransforming", "Block If Transforming").SetValue(true)
             };

            var mega = new List<MenuItem>
            {
                 new MenuItem("Q2Range", "Range").SetValue(new Slider(1100, 0, 1100)),
                 new MenuItem("BetaQ2", "Allow Collision Q").SetValue(true).SetTooltip("Will Q On Minions Near Target")
             };

            var menuGenerator = new MenuGenerator(this.Menu, new DynamicMenu("Dynamic Menu", selecter, new[] { mini, mega }));

            menuGenerator.Generate();
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
