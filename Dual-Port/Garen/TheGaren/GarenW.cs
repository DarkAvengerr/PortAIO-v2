using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheKalista.Commons.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheGaren
{
    class GarenW : Skill
    {
        public bool UseAlways;
        public float MinDamagePercent; // per sec
        public bool UseOnUltimates;
        private float _healthTime;
        private float _healthValue;
        private bool _shouldUse;

        public GarenW(SpellSlot spell)
            : base(spell)
        {
            Spellbook.OnCastSpell += OnSpellcast;
            OnlyUpdateIfTargetValid = false;
        }

        private void OnSpellcast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsEnemy && args.Slot == SpellSlot.R && UseOnUltimates)
            {
                var halfLineLength = (args.EndPosition - args.StartPosition).Length() / 2f;
                if (ObjectManager.Player.Position.Distance(args.StartPosition) > halfLineLength && ObjectManager.Player.Position.Distance(args.EndPosition) > halfLineLength) return;
                if (UseAlways)
                    Cast();
                else
                    _shouldUse = true;
            }
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (Game.Time - _healthTime > 1)
            {
                _healthTime = Game.Time;
                _healthValue = ObjectManager.Player.Health;
            }

            base.Update(mode, combo, target);
            if (UseAlways && ShouldUse())
                Cast();
            _shouldUse = false;
        }

        public override void Execute(AIHeroClient target)
        {
            if (!UseAlways && ShouldUse())
                Cast();
        }

        public override void Gapcloser(ComboProvider combo, ActiveGapcloser gapcloser)
        {
            Cast();
        }

        private bool ShouldUse()
        {
            return (ObjectManager.Player.Health - HealthPrediction.GetHealthPrediction(ObjectManager.Player, 1000)) / ObjectManager.Player.MaxHealth * 100f > MinDamagePercent || (_healthValue - ObjectManager.Player.Health) / ObjectManager.Player.MaxHealth * 100f > MinDamagePercent || _shouldUse;
        }

        public override int GetPriority()
        {
            return 3;
        }
    }
}
