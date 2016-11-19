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

        private readonly WSpell wSpell;

        public WCombo(WSpell wSpell)
        {
            this.wSpell = wSpell;
        }

        private AIHeroClient Target => TargetSelector.GetTarget(wSpell.Spell.Range, TargetSelector.DamageType.Physical);

        private bool ewq;

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);

            Obj_AI_Base.OnProcessSpellCast -= OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser -= Gapcloser;
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);

            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += Gapcloser;
            Game.OnUpdate += OnUpdate;
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Mana", "Mana %").SetValue(new Slider(5, 0, 100)));

            Menu.AddItem(new MenuItem("AntiGapcloser", "Anti-Gapcloser").SetValue(true));

            Menu.AddItem(new MenuItem("Target", "W Behind Target").SetValue(true));

            Menu.AddItem(new MenuItem("Immobile", "W On Immobile").SetValue(true));

            Menu.AddItem(new MenuItem("Bush", "Auto W On Bush").SetValue(false));
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !wSpell.Spell.IsReady())
            {
                return;
            }
          
            this.ewq = args.SData.Name == "CaitlynPiltoverPeacemaker";
        }

        private void Gapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("AntiGapcloser").GetValue<bool>() || !CheckGuardians() || !gapcloser.Sender.IsEnemy) return;

            wSpell.Spell.Cast(gapcloser.End);
        }

      
        private void OnUpdate(EventArgs args)
        {
            if (!CheckGuardians())
            {
                return;
            }

            if (Menu.Item("Bush").GetValue<bool>()
                && Utils.TickCount - wSpell.Spell.LastCastAttemptT < 2500
                && !ObjectManager.Player.IsRecalling())
            {
                // Beta
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Ammo < 2) return;

                var path = ObjectManager.Player.GetWaypoints().LastOrDefault().To3D();

                if (!NavMesh.IsWallOfGrass(path, 0)) return;

                LeagueSharp.Common.Utility.DelayAction.Add(100, ()=> wSpell.Spell.Cast(path));
            }

            if (Target == null 
                || Menu.Item("Mana").GetValue<Slider>().Value > ObjectManager.Player.ManaPercent
                || Utils.TickCount - wSpell.Spell.LastCastAttemptT < 1100)
            {
                return;
            }

            var wPrediction = wSpell.Spell.GetPrediction(Target);

            if (Menu.Item("Target").GetValue<bool>()) 
            {
                if (wPrediction.Hitchance < HitChance.VeryHigh)
                {
                    return;
                }

                wSpell.Spell.Cast(wPrediction.CastPosition);
            }

            if (wPrediction.Hitchance < HitChance.Immobile || !Menu.Item("Immobile").GetValue<bool>()) return;

            wSpell.Spell.Cast(wPrediction.CastPosition);
        }
    }
}
