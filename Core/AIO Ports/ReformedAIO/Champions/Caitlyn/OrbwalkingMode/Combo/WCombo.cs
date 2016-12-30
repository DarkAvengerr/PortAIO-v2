using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Combo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WCombo : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WCombo(WSpell spell)
        {
            this.spell = spell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(spell.Spell.Range, TargetSelector.DamageType.Physical);

        private bool ewq;

        private Obj_AI_Turret Obj_AI_Turret;

        private const int time = 2350;

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast -= OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast -= OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        private void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe
                || Target == null
                || !CheckGuardians()
                || Utils.TickCount - spell.Spell.LastCastAttemptT < time
                || Menu.Item("Caitlyn.Combo.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var wPrediction = spell.Spell.GetPrediction(Target);

            if (!Menu.Item("Caitlyn.Combo.W.Target").GetValue<bool>() || wPrediction.Hitchance < HitChance.VeryHigh)
            {
                return;
            }

            spell.Spell.Cast(wPrediction.CastPosition);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Caitlyn.Combo.W.Mana", "Mana %").SetValue(new Slider(5, 0, 100)));

            Menu.AddItem(new MenuItem("Caitlyn.Combo.W.AntiGapcloser", "Anti-Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Combo.W.Target", "W Behind Target").SetValue(true));

            // Menu.AddItem(new MenuItem("Combo.W.Immobile", "W On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Combo.W.Bush", "Auto W On Bush").SetValue(false));
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !spell.Spell.IsReady())
            {
                return;
            }

            this.ewq = args.SData.Name == "CaitlynPiltoverPeacemaker";
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("Caitlyn.Combo.W.AntiGapcloser").GetValue<bool>() || !CheckGuardians() || !gapcloser.Sender.IsEnemy) return;

            spell.Spell.Cast(gapcloser.End);
        }


        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || !Menu.Item("Caitlyn.Combo.W.Bush").GetValue<bool>()
                || Utils.TickCount - spell.Spell.LastCastAttemptT < time)
            {
                return;
            }

            // Beta
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Ammo < 2) return;

            var path = ObjectManager.Player.Path.ToList().LastOrDefault();

            if (!NavMesh.IsWallOfGrass(path, 0)) return;

            LeagueSharp.Common.Utility.DelayAction.Add(100, () => spell.Spell.Cast(path));
        }
    }
}
