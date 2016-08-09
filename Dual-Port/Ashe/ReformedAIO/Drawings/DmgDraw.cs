using EloBuddy; namespace ReformedAIO.Champions.Ashe.Drawings
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal class DmgDraw : ChildBase
    {
        #region Fields

        private HpBarIndicator drawDamage;

        private RLogic logic;

        #endregion

        #region Public Properties

        public sealed override string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        public DmgDraw(string name)
        {
            this.Name = name;
        }

        public void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                
                this.drawDamage.Unit = enemy;
                this.drawDamage.DrawDmg(this.logic.ComboDamage(enemy), Color.LawnGreen);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= this.OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += this.OnDraw;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new RLogic();
            this.drawDamage = new HpBarIndicator();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
        }

        #endregion
    }
}