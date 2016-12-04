using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Lasthit
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;
    using ReformedAIO.Library.WallExtension;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ELasthit : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ELasthit(ESpell spell)
        {
            this.spell = spell;
        }

        private DashPosition dashPos;

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range).LastOrDefault(m => m.Distance(Game.CursorPos) <= spell.Spell.Range);

        private void OnUpdate(EventArgs args)
        {
            if (Minion == null
                || !CheckGuardians()
                || (Menu.Item("LasthitTurret").GetValue<bool>() && dashPos.DashEndPosition(Minion, spell.Spell.Range).UnderTurret(true))
                || (Menu.Item("LasthitEnemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(750))
                ||  Minion.Health > spell.Spell.GetDamage(Minion))
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

            Menu.AddItem(new MenuItem("LasthitEnemies", "Don't E Into X Enemies").SetValue(new Slider(2, 0, 5)));

            Menu.AddItem(new MenuItem("LasthitTurret", "Turret Check").SetValue(true));
        }
    }
}