using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class LunarRush : ChildBase
    {
        #region Fields

        private LogicAll logic;

        #endregion

        private readonly Orbwalking.Orbwalker orbwalker;

        public LunarRush(Orbwalking.Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        #region Public Properties

        public override string Name { get; set; } = "[W] Lunar Rush";

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

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    logic = new LogicAll();
        //}

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "WKillable", "Use Only If Killable By Combo").SetValue(false));

            //   Menu.AddItem(new MenuItem(Name + "WMana", "Mana %")
            //       .SetValue(new Slider(15, 100)));

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            logic = new LogicAll();
        }

        private void Lunarrush()
        {
            var target = TargetSelector.GetTarget(
                Variables.Player.AttackRange + Variables.Player.BoundingRadius,
                TargetSelector.DamageType.Magical);

            if (target == null || !target.IsValid) return;

            if (Menu.Item(Menu.Name + "WKillable").GetValue<bool>() && logic.ComboDmg(target) < target.Health)
            {
                return;
            }

            Variables.Spells[SpellSlot.W].Cast();
        }

        private void OnUpdate(EventArgs args)
        {
            if (this.orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo
                || !Variables.Spells[SpellSlot.W].IsReady()) return;

            Lunarrush();
        }

        #endregion
    }
}