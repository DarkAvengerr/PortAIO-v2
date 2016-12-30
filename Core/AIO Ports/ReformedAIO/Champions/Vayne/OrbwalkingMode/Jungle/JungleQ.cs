using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Jungle
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QJungle : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly DashSmart dashSmart;

        public QJungle(QSpell spell, DashSmart dashSmart)
        {
            this.spell = spell;
            this.dashSmart = dashSmart;
        }

        private Obj_AI_Base Mob =>
             MinionManager.GetMinions(ObjectManager.Player.Position,
                 spell.Spell.Range,
                 MinionTypes.All,
                 MinionTeam.Neutral).FirstOrDefault();

        private void OnUpdate(EventArgs args)
        {
            if (Menu.Item("Vayne.Jungle.Q.Reset").GetValue<bool>())
            {
                return;
            }

            if (Mob == null
                || Menu.Item("Vayne.Jungle.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || !CheckGuardians())
            {
                return;
            }

            switch (Menu.Item("Vayne.Jungle.Q.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                    break;
                case 1:
                    spell.Spell.Cast(dashSmart.Kite(Mob.Position.To2D(), spell.Spell.Range).To3D());
                    break;
                case 2:
                    spell.Spell.Cast(dashSmart.ToSafePosition(Mob, Mob.Position, spell.Spell.Range));
                    break;
            }
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Mob == null
                 || !sender.IsMe
                 || Menu.Item("Vayne.Jungle.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                 || !CheckGuardians())
            {
                return;
            }

            switch (Menu.Item("Vayne.Jungle.Q.Mode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                    break;
                case 1:
                    spell.Spell.Cast(dashSmart.Kite(Mob.Position.To2D(), spell.Spell.Range).To3D());
                    break;
                case 2:
                    spell.Spell.Cast(dashSmart.ToSafePosition(Mob, Mob.Position, spell.Spell.Range));
                    break;
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Game.OnUpdate -= OnUpdate;
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Jungle.Q.Reset", "ONLY Q After Auto").SetValue(true));

            Menu.AddItem(new MenuItem("Vayne.Jungle.Q.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" })));

            Menu.AddItem(new MenuItem("Vayne.Jungle.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
