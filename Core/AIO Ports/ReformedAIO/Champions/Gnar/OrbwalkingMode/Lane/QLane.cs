namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Lane
{
    using System;
    using System.Collections.Generic;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.Menu;
    using RethoughtLib.Menu.Presets;

    internal sealed class QLane : ChildBase
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "Q";

        private readonly Orbwalking.Orbwalker orbwalker;

        public QLane(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        private void GameOnUpdate(EventArgs args)
        {
            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LaneClear || Vars.Player.Spellbook.IsAutoAttacking)
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
            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            if (!Spells.Q.IsReady() || (Menu.Item(menu.Name + "Q1Enemy").GetValue<bool>() && Vars.Player.CountEnemiesInRange(1100) > 0))
            {
                return;
            }

            var minions = MinionManager.GetMinions(Menu.Item(menu.Name + "Q1Range").GetValue<Slider>().Value);

            var prediction = Spells.Q.GetLineFarmLocation(minions);

            if (prediction.MinionsHit >= Menu.Item(menu.Name + "Q1HitCount").GetValue<Slider>().Value)
            {
                Spells.Q.Cast(prediction.Position);
            }
        }

        private void Mega()
        {
            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            if (!Spells.Q2.IsReady() || (Menu.Item(menu.Name + "Q2Enemy").GetValue<bool>() && Vars.Player.CountEnemiesInRange(1100) > 0))
            {
                return;
            }

            var minions = MinionManager.GetMinions(Menu.Item(menu.Name + "Q2Range").GetValue<Slider>().Value);

            var prediction = Spells.Q2.GetLineFarmLocation(minions);

            if (prediction.MinionsHit >= Menu.Item(menu.Name + "Q2HitCount").GetValue<Slider>().Value)
            {
                Spells.Q2.Cast(prediction.Position);
            }
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            this.gnarState = new GnarState();

            var selecter = new MenuItem("GnarForm", "Form").SetValue(new StringList(new[] { "Mini", "Mega" }));

            var mini = new List<MenuItem>
            {
                 new MenuItem("Q1Range", "Range").SetValue(new Slider(600, 0, 600)),
                 new MenuItem("Q1HitCount", "Min Hit Count").SetValue(new Slider(2, 0, 6)),
                 new MenuItem("Q1Enemy", "Block If Nearby Enemies").SetValue(true)
             };

            var mega = new List<MenuItem>
            {
                 new MenuItem("Q2Range", "Range").SetValue(new Slider(600, 0, 700)),
                 new MenuItem("Q2HitCount", "Min Hit Count").SetValue(new Slider(3, 0, 6)),
                 new MenuItem("Q2Enemy", "Block If Nearby Enemies").SetValue(true)
             };

            var menuGenerator = new MenuGenerator(Menu, new DynamicMenu("Dynamic Menu", selecter, new[] { mini, mega }));

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
