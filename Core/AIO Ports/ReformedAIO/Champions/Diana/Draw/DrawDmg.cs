using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Diana.Draw
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;
    using ReformedAIO.Library.Drawings;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal class DrawDmg : ChildBase
    {
        #region Fields

        private HeroHealthBarIndicator drawDamage;

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

                //DrawDamage.Unit = enemy;
                //DrawDamage.DrawDmg(logic.ComboDmg(enemy), easyKill);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnEndScene -= OnEndScene;

        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Drawing.OnEndScene += OnEndScene;
            
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        //{
        //    logic = new LogicAll();
        //    drawDamage = new HeroHealthBarIndicator();
        //    base.OnLoad(sender, eventArgs);
        //}

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            Menu = new Menu(Name, Name);

            Menu.AddItem(new MenuItem(Name + "Enabled", "Enabled").SetValue(true));

            logic = new LogicAll();
            drawDamage = new HeroHealthBarIndicator();
            base.OnLoad(sender, eventArgs);
        }

        #endregion
    }
}