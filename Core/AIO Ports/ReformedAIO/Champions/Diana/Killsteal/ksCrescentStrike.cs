using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Diana.Killsteal
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Diana.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class KsCrescentStrike : ChildBase
    {
        #region Fields

        private CrescentStrikeLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Crescent Strike";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            Game.OnUpdate += OnUpdate;
        }

        //protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        //{
        //    qLogic = new CrescentStrikeLogic();
        //    base.OnLoad(sender, eventArgs);
        //}

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs) // TODO Add Dmg Multiplier(?)
        {
            Menu.AddItem(new MenuItem(Menu.Name + "QRange", "Q Range ").SetValue(new Slider(820, 0, 825)));

            Menu.AddItem(new MenuItem(Menu.Name + "QMana", "Mana %").SetValue(new Slider(45, 0, 100)));

            qLogic = new CrescentStrikeLogic();
        }

        private void OnUpdate(EventArgs args)
        {
            var target =
                HeroManager.Enemies.FirstOrDefault(
                    x =>
                    !x.IsDead && x.IsValidTarget(Menu.Item(Menu.Name + "QRange").GetValue<Slider>().Value));

            if (Menu.Item(Menu.Name + "QMana").GetValue<Slider>().Value > Variables.Player.ManaPercent) return;

            if (target != null && target.Health < qLogic.GetDmg(target))
            {
                Variables.Spells[SpellSlot.Q].Cast(qLogic.QPred(target));
            }
        }

        #endregion
    }
}