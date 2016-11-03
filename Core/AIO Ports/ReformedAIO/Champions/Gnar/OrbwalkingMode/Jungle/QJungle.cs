using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gnar.OrbwalkingMode.Jungle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gnar.Core;

    using RethoughtLib.FeatureSystem.Abstract_Classes;
    using RethoughtLib.FeatureSystem.Implementations;
    using RethoughtLib.Menu;
    using RethoughtLib.Menu.Presets;

    internal sealed class QJungle : OrbwalkingChild
    {
        private GnarState gnarState;

        public override string Name { get; set; } = "Q";

        private void GameOnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            if (Menu.Item(menu.Name + "BlockIfTransforming").GetValue<bool>()
                && this.gnarState.TransForming)
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
            if (!Spells.Q.IsReady() || ObjectManager.Player.Spellbook.IsAutoAttacking)
            {
                return;
            }

            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            var m = MinionManager.GetMinions(Menu.Item(menu.Name + "Q1Range").GetValue<Slider>().Value, MinionTypes.All, MinionTeam.Neutral).LastOrDefault();

            var prediction = Spells.Q.GetPrediction(m, true);

            if (prediction.Hitchance >= HitChance.High)
            {
                Spells.Q.Cast(prediction.CastPosition);
            }
        }

        private void Mega()
        {
            if (!Spells.Q2.IsReady())
            {
                return;
            }

            var menu = Menu.SubMenu(Menu.Name + "Dynamic Menu");

            foreach (var m in MinionManager.GetMinions(Menu.Item(menu.Name + "Q2Range").GetValue<Slider>().Value, MinionTypes.All, MinionTeam.Neutral))
            {
                var prediction = Spells.Q2.GetPrediction(m, true);
              
                if (prediction.Hitchance >= HitChance.Medium)
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(1, () => Spells.Q2.Cast(prediction.CastPosition));
                }
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
                               new MenuItem("BlockIfTransforming", "Block If Transforming").SetValue(true)
                           };

            var mega = new List<MenuItem> { new MenuItem("Q2Range", "Range").SetValue(new Slider(600, 0, 700)) };

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
