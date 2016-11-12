using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.OrbwalkingMode.Combo
{
    #region Using Directives

    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Ashe.Logic;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    #endregion

    internal class ECombo : ChildBase
    {
        #region Fields

        private ELogic eLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[E]";

        #endregion

        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnDraw -= OnDraw;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            Menu.AddItem(new MenuItem("EDistance", "Distance").SetValue(new Slider(2500, 0, 2500)).SetTooltip("Only for enemeis & not objectives"));

            Menu.AddItem(new MenuItem("ECount", "Save 1 Charge").SetValue(true));

            Menu.AddItem(new MenuItem("EToVector", "E To Objectives").SetValue(true));

            Menu.AddItem(new MenuItem("VectorDraw", "Draw Objective Position").SetValue(true));

            eLogic = new ELogic();
        }

        private void EToCamp()
        {
            if(!Menu.Item("EToVector").GetValue<bool>()) return;

            var pos =  eLogic.Camp.FirstOrDefault(x => x.Value.Distance(Variable.Player.Position) > 1500 && x.Value.Distance(Variable.Player.Position) < 7000);

            if (!pos.Value.IsValid()) return;

            LeagueSharp.Common.Utility.DelayAction.Add(290, () => Variable.Spells[SpellSlot.E].Cast(pos.Value)); // Humanized
        }

        private void Hawkshot()
        {
            var target = TargetSelector.GetTarget(Variable.Player.AttackRange, TargetSelector.DamageType.True);

            if (target == null || !eLogic.ComboE(target)) return;

            foreach (var position in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Variable.Player) < Menu.Item("EDistance").GetValue<Slider>().Value))
            {
                var path = position.GetWaypoints().FirstOrDefault().To3D();

                if (!NavMesh.IsWallOfGrass(path, 50)) return;

                Variable.Spells[SpellSlot.E].Cast(path);
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (!Menu.Item("VectorDraw").GetValue<bool>()) return;

            var pos = eLogic.Camp.FirstOrDefault(x => x.Value.Distance(Variable.Player.Position) > 2500 && x.Value.Distance(Variable.Player.Position) < 5500);

            if(!pos.Value.IsValid()) return;
 
            Render.Circle.DrawCircle(pos.Value, 100, Color.Green);
        }

        private void OnUpdate(EventArgs args)
        {
            if (!Variable.Spells[SpellSlot.E].IsReady()
                || Variable.Player.IsRecalling() 
                || Variable.Player.InShop()
                || (Menu.Item("ECount").GetValue<bool>() 
                && eLogic.GetEAmmo() == 1))
                return;

            EToCamp();

            Hawkshot();
        }

        #endregion
    }
}