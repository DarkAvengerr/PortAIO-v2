using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Vayne.OrbwalkingMode.Laneclear
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Vayne.Core.Damage;
    using ReformedAIO.Champions.Vayne.Core.Spells;
    using ReformedAIO.Library.Dash_Handler;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class QLane : OrbwalkingChild
    {
        public override string Name { get; set; } = "Q";

        private readonly QSpell spell;

        private readonly DashSmart dashSmart;

        private readonly Damages damage;

        public QLane(QSpell spell, DashSmart dashSmart, Damages damage)
        {
            this.spell = spell;
            this.dashSmart = dashSmart;
            this.damage = damage;
        }

        private IEnumerable<Obj_AI_Base> Minion => 
             MinionManager.GetMinions(ObjectManager.Player.Position, ObjectManager.Player.AttackRange + spell.Spell.Range)
            .Where(m => m.Health < damage.GetComboDamage(m));

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Minion == null
                 || ObjectManager.Player.UnderTurret(true)
                 || !sender.IsMe
                 || Menu.Item("Vayne.Lane.Q.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                 || (Menu.Item("Vayne.Lane.Q.Enemy").GetValue<bool>() && ObjectManager.Player.CountEnemiesInRange(1250) >= 1)
                 || Minion.Count() < 2
                 || !CheckGuardians())
            {
                return;
            }

            foreach (var m in Minion)
            {
                switch (Menu.Item("Vayne.Lane.Q.Mode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        spell.Spell.Cast(ObjectManager.Player.Position.Extend(Game.CursorPos, spell.Spell.Range));
                        break;
                    case 1:
                        spell.Spell.Cast(dashSmart.Kite(m.Position.To2D(), spell.Spell.Range).To3D());
                        break;
                    case 2:
                        spell.Spell.Cast(dashSmart.ToSafePosition(m, m.Position, spell.Spell.Range));
                        break;
                }
            }
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

           
            Obj_AI_Base.OnSpellCast -= OnSpellCast;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

           
            Obj_AI_Base.OnSpellCast += OnSpellCast;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Vayne.Lane.Q.Enemy", "Enemies Check").SetValue(false));

            Menu.AddItem(new MenuItem("Vayne.Lane.Q.Mode", "Mode").SetValue(new StringList(new[] { "Cursor", "Kite", "Automatic" })));

            Menu.AddItem(new MenuItem("Vayne.Lane.Q.Mana", "Min Mana %").SetValue(new Slider(0, 0, 100)));
        }
    }
}
