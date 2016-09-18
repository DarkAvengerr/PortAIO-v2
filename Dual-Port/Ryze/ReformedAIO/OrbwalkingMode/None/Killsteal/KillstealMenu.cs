using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ryze.OrbwalkingMode.None.Killsteal
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class KillstealMenu : ChildBase
    {
        #region Public Properties

        public override string Name { get; set; } = "Killsteal Menu";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu.AddItem(new MenuItem(Name + "KsE", "Use E").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "KsW", "Use W").SetValue(true));

            Menu.AddItem(new MenuItem(Name + "KsQ", "Use Q").SetValue(true));
        }

        private void OnUpdate(EventArgs args)
        {
            var target = HeroManager.Enemies.FirstOrDefault(x => !x.IsDead && x.IsValidTarget(1000));

            if (target == null) return;

            if (Variable.Spells[SpellSlot.Q].IsReady() && Menu.Item(Menu.Name + "KsQ").GetValue<bool>())
            {
                if (target.Health <= Variable.Spells[SpellSlot.Q].GetDamage(target))
                {
                    Variable.Spells[SpellSlot.Q].Cast(target);
                }
            }

            if (Variable.Spells[SpellSlot.W].IsReady() && Menu.Item(Menu.Name + "KsW").GetValue<bool>())
            {
                if (target.Health <= Variable.Spells[SpellSlot.W].GetDamage(target))
                {
                    Variable.Spells[SpellSlot.W].Cast(target);
                }
            }

            if (!Variable.Spells[SpellSlot.E].IsReady() || !Menu.Item(Menu.Name + "KsE").GetValue<bool>()) return;

            if (target.Health <= Variable.Spells[SpellSlot.E].GetDamage(target))
            {
                Variable.Spells[SpellSlot.E].Cast(target);
            }
        }

        #endregion
    }
}