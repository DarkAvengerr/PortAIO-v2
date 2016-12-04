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
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal class DmgDraw : ChildBase
    {
        #region Fields

        private HeroHealthBarIndicator drawDamage;

        private RLogic logic;

        #endregion

        #region Public Properties

        public sealed override string Name { get; set; } = "Damage";

        #endregion

        #region Public Methods and Operators

    
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

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= OnDraw;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += OnDraw;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            logic = new RLogic();
            drawDamage = new HeroHealthBarIndicator();
        }
        #endregion
    }
}