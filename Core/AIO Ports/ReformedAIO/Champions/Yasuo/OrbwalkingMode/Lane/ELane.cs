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
    using ReformedAIO.Library.WallExtension;

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

        private WallExtension wall;

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range).LastOrDefault(m => m.Distance(Game.CursorPos) <= spell.Spell.Range);

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null
                || !CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && dashPos.DashEndPosition(Minion, spell.Spell.Range).UnderTurret(true))
                || (Menu.Item("Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(750)))
            {
                return;
            }

            var wallPoint = wall.FirstWallPoint(ObjectManager.Player.Position, Minion.Position);

            if (wall.IsWallDash(wallPoint, spell.Spell.Range))
            {
                return;
            }

            if (Menu.Item("Killable").GetValue<bool>() && Minion.Health > spell.Spell.GetDamage(Minion) + ObjectManager.Player.GetAutoAttackDamage(Minion))
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

            wall = new WallExtension();

            Menu.AddItem(new MenuItem("Killable", "Only Killable Minions").SetValue(true));

            Menu.AddItem(new MenuItem("Enemies", "Don't E Into X Enemies").SetValue(new Slider(1, 0, 5)));

            Menu.AddItem(new MenuItem("Turret", "Turret Check").SetValue(true));
        }
    }
}