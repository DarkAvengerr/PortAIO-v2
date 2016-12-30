using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Yasuo.OrbwalkingMode.Combo
{
    using System;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Yasuo.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class ECombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "E";

        private readonly ESpell spell;

        public ECombo(ESpell spell)
        {
            this.spell = spell;
        }

        private DashPosition dashPos;

        private static AIHeroClient Target => TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);

        private Obj_AI_Base Minion => MinionManager.GetMinions(ObjectManager.Player.Position, 1000).LastOrDefault(
            m => dashPos.DashEndPosition(m, spell.Spell.Range).Distance(Target.Position) <= ObjectManager.Player.Distance(Target)
                 && dashPos.DashEndPosition(m, spell.Spell.Range).Distance(Target.Position) > ObjectManager.Player.AttackRange);

        private void OnUpdate(EventArgs args)
        {
            if (Target == null
                || !CheckGuardians()
                || (Menu.Item("Yasuo.Combo.E.Mouse").GetValue<bool>() &&  Minion != null && Minion.Distance(Game.CursorPos) > Menu.Item("Yasuo.Combo.E.Radius").GetValue<Slider>().Value)
                || (Menu.Item("Yasuo.Combo.E.Turret").GetValue<bool>() && dashPos.DashEndPosition(Target, spell.Spell.Range).UnderTurret(true))
                || (Menu.Item("Yasuo.Combo.E.Enemies").GetValue<Slider>().Value < ObjectManager.Player.CountEnemiesInRange(1000)))
            {
                return;
            }

            if (Minion != null && Target.Distance(ObjectManager.Player) > spell.Spell.Range)
            {
                spell.Spell.CastOnUnit(Minion);
            }
            else
            {
                spell.Spell.CastOnUnit(Target);
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Drawing.OnEndScene -= OnEndScene;
            Game.OnUpdate -= OnUpdate;
        }

        private void OnEndScene(EventArgs args)
        {
            if (!Menu.Item("Yasuo.Combo.E.Mouse").GetValue<bool>())
            {
                return;
            }

            Render.Circle.DrawCircle(Game.CursorPos, Menu.Item("Yasuo.Combo.E.Radius").GetValue<Slider>().Value, Color.AliceBlue);
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
            Drawing.OnEndScene += OnEndScene;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            dashPos = new DashPosition();

            Menu.AddItem(new MenuItem("Yasuo.Combo.E.Mouse", "Mouse Based").SetValue(true));

            Menu.AddItem(new MenuItem("Yasuo.Combo.E.Radius", "Mouse Radius").SetValue(new Slider(450, 100, 900)));

            Menu.AddItem(new MenuItem("Yasuo.Combo.E.Enemies", "Don't E Into X Enemies").SetValue(new Slider(3, 0, 5)));

            Menu.AddItem(new MenuItem("Yasuo.Combo.E.Turret", "Turret Check").SetValue(true));
        }
    }
}