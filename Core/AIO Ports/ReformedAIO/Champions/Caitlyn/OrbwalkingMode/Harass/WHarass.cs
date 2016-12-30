using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Caitlyn.OrbwalkingMode.Harass
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using ReformedAIO.Champions.Caitlyn.Spells;

    using RethoughtLib.FeatureSystem.Implementations;

    internal sealed class WHarass : OrbwalkingChild
    {
        public override string Name { get; set; } = "W";

        private readonly WSpell spell;

        public WHarass(WSpell spell)
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
                || Menu.Item("Caitlyn.Harass.W.Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent)
            {
                return;
            }

            var wPrediction = spell.Spell.GetPrediction(Target);

            if (!Menu.Item("Caitlyn.Harass.W.Target").GetValue<bool>() || wPrediction.Hitchance < HitChance.VeryHigh)
            {
                return;
            }

            spell.Spell.Cast(wPrediction.CastPosition);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Caitlyn.Harass.W.Mana", "Mana %").SetValue(new Slider(5, 0, 100)));

            Menu.AddItem(new MenuItem("Caitlyn.Harass.W.AntiGapcloser", "Anti-Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Harass.W.Target", "W Behind Target").SetValue(true));

         // Menu.AddItem(new MenuItem("Harass.W.Immobile", "W On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("Caitlyn.Harass.W.Bush", "Auto W On Bush").SetValue(false));
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
            if (!Menu.Item("Caitlyn.Harass.W.AntiGapcloser").GetValue<bool>() || !CheckGuardians() || !gapcloser.Sender.IsEnemy) return;

            spell.Spell.Cast(gapcloser.End);
        }


        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians()
                || !Menu.Item("Caitlyn.Harass.W.Bush").GetValue<bool>() 
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
