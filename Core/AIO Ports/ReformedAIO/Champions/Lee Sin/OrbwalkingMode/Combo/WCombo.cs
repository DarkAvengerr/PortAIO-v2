using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Lee_Sin.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Lee_Sin.Core.Damage;
    using ReformedAIO.Champions.Lee_Sin.Core.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        private readonly RSpell rSpell;

        private readonly LeeSinStatistisks statistisks;

        public WCombo(WSpell spell, RSpell rSpell, LeeSinStatistisks statistisks)
        {
            this.spell = spell;
            this.rSpell = rSpell;
            this.statistisks = statistisks;
        }

        private static AIHeroClient Target => TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);

        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || Target == null
                || !spell.W1
                || ObjectManager.Player.IsDashing()
                || statistisks.HasQ2(Target)
                || (Menu.Item("LeeSin.Combo.W.Mana").GetValue<bool>() && ObjectManager.Player.Mana < statistisks.EnergyCost(Target)))
            {
                return;
            }

            if ((statistisks.GetComboDamage(Target) * 1.25 > Target.Health
                || ObjectManager.Player.CountAlliesInRange(1750) >= 1) 
                && ObjectManager.Player.CountEnemiesInRange(1500) == 1
                && Target.Distance(ObjectManager.Player) > 500)
            {
                spell.Jump(Target.Position,
                    Menu.Item("LeeSin.Combo.W.Minions").GetValue<bool>(), 
                    Menu.Item("LeeSin.Combo.W.Allies").GetValue<bool>(), 
                    Menu.Item("LeeSin.Combo.W.Ward").GetValue<bool>());
            }
        }

       
        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!CheckGuardians() 
                || Target == null
                || !sender.IsMe
                || !args.SData.IsAutoAttack())
            {
                return;
            }

            spell.Spell.Cast();
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

            Menu.AddItem(new MenuItem("LeeSin.Combo.W.Mana", "Energy Check").SetValue(true).SetTooltip("Wont Go In Without Energy"));

            Menu.AddItem(new MenuItem("LeeSin.Combo.W.Star", "Gapclose").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Combo.W.Minions", "Jump To: Minions").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Combo.W.Allies", "Jump To: Allies").SetValue(true));

            Menu.AddItem(new MenuItem("LeeSin.Combo.W.Ward", "Jump To: Ward").SetValue(false));
        }
    }
}
