using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lane
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELane : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ELane(ESpell spell)
        {
            this.spell = spell;
        }

        private DashPosition dashPos;

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null
                || !CheckGuardians()
                || (Menu.Item("Lane.E.Turret").GetValue<bool>() && (dashPos.DashEndPosition(Minion, spell.Spell.Range).UnderTurret(true) || Minion.UnderTurret(true)))
                || (Menu.Item("Lane.E.Enemies").GetValue<Slider>().Value < dashPos.DashEndPosition(Minion, spell.Spell.Range).CountEnemiesInRange(500))
                || (Menu.Item("Lane.E.Killable").GetValue<bool>() && Minion.Health > spell.Spell.GetDamage(Minion)))
            {
                return;
            }

            spell.Spell.CastOnUnit(Minion);
        }

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

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            dashPos = new DashPosition();

            Menu.AddItem(new MenuItem("Lane.E.Killable", "Only Killable Minions").SetValue(true));

            Menu.AddItem(new MenuItem("Lane.E.Enemies", "Don't E Into X Enemies").SetValue(new Slider(1, 0, 5)));

            Menu.AddItem(new MenuItem("Lane.E.Turret", "Turret Check").SetValue(true));
        }
    }
}