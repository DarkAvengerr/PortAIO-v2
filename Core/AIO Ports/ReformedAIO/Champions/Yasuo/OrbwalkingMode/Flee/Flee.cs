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

    internal sealed class Flee : OrbwalkingChild
    {
        public override string Name { get; set; } = "Flee";

        private readonly ESpell spell;

        public Flee(ESpell spell)
        {
            this.spell = spell;
        }

        private DashPosition dashPos;

        private WallExtension wall;

        private Obj_AI_Base Minion
            =>
                MinionManager.GetMinions(ObjectManager.Player.Position, spell.Spell.Range)
                    .LastOrDefault(m => m.Distance(Game.CursorPos) <= 600);

        private IOrderedEnumerable<Obj_AI_Base> Mob
            =>
                MinionManager.GetMinions(
                    ObjectManager.Player.Position,
                    spell.Spell.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || (Menu.Item("Turret").GetValue<bool>() && Minion.UnderTurret(true))
                || (Menu.Item("Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(750))
                || !Menu.Item("Keybind").GetValue<KeyBind>().Active)
            {
                return;
            }

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Mob != null)
            {
                foreach (var m in Mob)
                {
                    var wallPoint = wall.FirstWallPoint(ObjectManager.Player.Position, dashPos.DashEndPosition(m, spell.Spell.Range));

                    if (wall.IsWallDash(wallPoint, spell.Spell.Range))
                    {
                        spell.Spell.CastOnUnit(m);
                    }
                }
            }

            if (Minion == null)
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

            Menu.AddItem(new MenuItem("Keybind", "Keybind: ").SetValue(new KeyBind('A', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("Enemies", "Don't Flee Into X Enemies").SetValue(new Slider(2, 0, 5)));

            Menu.AddItem(new MenuItem("Turret", "Turret Check").SetValue(true));
        }
    }
}