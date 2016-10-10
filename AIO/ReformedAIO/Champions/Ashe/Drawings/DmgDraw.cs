using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.Drawings
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

        public override sealed string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        public DmgDraw(string name)
        {
            Name = name;
        }

        public void OnDraw(EventArgs args)
        {
            if (Variable.Player.IsDead) return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1500)))
            {
                
                drawDamage.Unit = enemy;
                drawDamage.DrawDmg(logic.ComboDamage(enemy), Color.LawnGreen);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            logic = new RLogic();
            drawDamage = new HpBarIndicator();
            base.OnLoad(sender, featureBaseEventArgs);
        }
        #endregion
    }
}