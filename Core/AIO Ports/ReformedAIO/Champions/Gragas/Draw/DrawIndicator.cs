using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.Draw
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal sealed class DrawIndicator : ChildBase
    {
        #region Fields

        private HeroHealthBarIndicator heroHealthBarIndicator;

        private LogicAll logic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "Draw Damage";

        #endregion

        #region Public Methods and Operators

        public void OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                this.heroHealthBarIndicator.Unit = enemy;
                this.heroHealthBarIndicator.DrawDmg(this.logic.ComboDmg(enemy), enemy.Health <= this.logic.ComboDmg(enemy) * 1.25 ? Color.LawnGreen : Color.Yellow);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnDisable(sender, featureBaseEventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnEnable(sender, featureBaseEventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            logic = new LogicAll();
            heroHealthBarIndicator = new HeroHealthBarIndicator();
        }

        #endregion
    }
}