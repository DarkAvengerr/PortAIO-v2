using EloBuddy; namespace ReformedAIO.Champions.Ryze.Drawings
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    using Damage = Logic.Damage;

    #endregion

    internal class DmgDraw : ChildBase
    {
        #region Fields

        private Damage dmg;

        private HpBarIndicator drawDamage;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "Draw Damage";

        #endregion

        #region Public Methods and Operators

        public void OnEndScene(EventArgs args)
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1200)))
            {
             
                drawDamage.Unit = enemy;
                drawDamage.DrawDmg(dmg.ComboDmg(enemy), Color.LawnGreen);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnEndScene -= OnEndScene;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnEndScene += OnEndScene;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            dmg = new Damage();
            drawDamage = new HpBarIndicator();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));
        }

        #endregion
    }
}