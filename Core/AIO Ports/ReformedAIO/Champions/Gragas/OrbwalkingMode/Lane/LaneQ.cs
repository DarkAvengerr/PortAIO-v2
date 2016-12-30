using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Gragas.OrbwalkingMode.Lane
{
    #region Using Directives

    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Gragas.Logic;

    using RethoughtLib.FeatureSystem.Implementations;

    #endregion

    internal class LaneQ : OrbwalkingChild
    {
        #region Fields

        private QLogic qLogic;

        #endregion

        #region Public Properties

        public override string Name { get; set; } = "[Q] Barrel Roll";

        #endregion
       
        #region Methods

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
        }

        protected sealed override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("LaneQEnemy", "Only If No Enemies Visible").SetValue(true));

            Menu.AddItem(new MenuItem("LaneQDistance", "Q Distance").SetValue(new Slider(730, 0, 825)));

            Menu.AddItem(new MenuItem("LaneQHit", "Min Minions Hit").SetValue(new Slider(3, 0, 6)));

            Menu.AddItem(new MenuItem("LaneQMana", "Mana %").SetValue(new Slider(15, 0, 100)));

            qLogic = new QLogic();
        }

        private void GetMinions()
        {
            var minions =
                MinionManager.GetMinions(Menu.Item("LaneQDistance").GetValue<Slider>().Value);

            if (minions == null) return;

            if (Menu.Item("LaneQEnemy").GetValue<bool>())
            {
                if (minions.Any(m => m.CountEnemiesInRange(1500) > 0))
                {
                    return;
                }
            }

            if (minions.Count < Menu.Item("LaneQHit").GetValue<Slider>().Value) return;

            var qPred = Variable.Spells[SpellSlot.Q].GetCircularFarmLocation(minions);

            foreach (var m in minions)
            {
                if (qLogic.CanThrowQ())
                {
                    Variable.Spells[SpellSlot.Q].Cast(qPred.Position);
                }

                if (!(Variable.Spells[SpellSlot.Q].GetDamage(m) > m.Health)) continue;

                if (qLogic.CanExplodeQ(m))
                {
                    Variable.Spells[SpellSlot.Q].Cast();
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians() || Menu.Item("LaneQMana").GetValue<Slider>().Value > Variable.Player.ManaPercent) return;

            GetMinions();
        }

        #endregion
    }
}