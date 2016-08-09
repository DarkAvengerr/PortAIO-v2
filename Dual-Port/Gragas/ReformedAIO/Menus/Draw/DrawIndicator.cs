using EloBuddy; namespace ReformedAIO.Champions.Gragas.Menus.Draw
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    using SharpDX;

    #endregion

    internal class DrawIndicator : ChildBase
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
                var easyKill = Variable.Spells[SpellSlot.R].IsReady()
                                   ? new ColorBGRA(0, 255, 0, 120)
                                   : new ColorBGRA(255, 255, 0, 120);

                this.drawDamage.Unit = enemy;
                this.drawDamage.DrawDmg(this.logic.ComboDmg(enemy), easyKill);
            }
        }

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnEndScene -= this.OnEndScene;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            Drawing.OnEndScene += this.OnEndScene;
        }

        protected override void OnInitialize(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.logic = new LogicAll();
            this.drawDamage = new HpBarIndicator();
            base.OnInitialize(sender, featureBaseEventArgs);
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            this.Menu = new Menu(this.Name, this.Name);

            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));
        }

        #endregion
    }
}