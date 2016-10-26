namespace ReformedAIO.Champions.Diana.Menus.Draw
{
    #region Using Directives

    using System;
    using System.Linq;

    using EloBuddy;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;
    using ReformedAIO.Core.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal class DrawDmg : ChildBase
    {
        #region Fields

        private HpBarIndicator drawDamage;

        private LogicAll logic;

        #endregion


        #region Public Properties

        public override string Name { get; set; } = "Draw Damage";

        #endregion

        #region Public Methods and Operators

        public void OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget(1200) && !ene.IsZombie))
            {
                var easyKill = Variables.Spells[SpellSlot.R].IsReady()
                                   ? new ColorBGRA(0, 255, 0, 120)
                                   : new ColorBGRA(255, 255, 0, 120);

                drawDamage.Unit = enemy;
                drawDamage.DrawDmg(logic.ComboDmg(enemy), easyKill);
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

        //protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        //{
        //    logic = new LogicAll();
        //    drawDamage = new HpBarIndicator();
        //    base.OnLoad(sender, featureBaseEventArgs);
        //}

        protected override sealed void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            logic = new LogicAll();
            drawDamage = new HpBarIndicator();
            base.OnLoad(sender, featureBaseEventArgs);
        }

        #endregion
    }
}