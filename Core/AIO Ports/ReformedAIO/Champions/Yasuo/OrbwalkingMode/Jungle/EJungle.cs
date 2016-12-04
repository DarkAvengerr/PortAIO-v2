using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.WallExtension;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class EJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private DashPosition dashPos;

        private readonly ESpell spell;

        public EJungle(ESpell spell)
        {
            this.spell = spell;
        }

        private WallExtension wall;

        private IOrderedEnumerable<Obj_AI_Base> Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range,
                 MinionTypes.All,
                 MinionTeam.Neutral).OrderBy(m => m.MaxHealth);

        private void OnUpdate(EventArgs args)
        {
            if (Mob == null || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Mob)
            {
                var wallPoint = wall.FirstWallPoint(ObjectManager.Player.Position, dashPos.DashEndPosition(m, spell.Spell.Range));

                if (wall.IsWallDash(wallPoint, 475))
                {
                    return;
                }

                spell.Spell.CastOnUnit(m);
            }
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
        }
    }
}