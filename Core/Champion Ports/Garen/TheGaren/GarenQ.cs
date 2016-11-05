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
    class GarenQ : Skill
    {
        public bool OnlyAfterAuto;
        private bool _recentAutoattack;
        public bool UseWhenOutOfRange;

        public GarenQ(SpellSlot spell)
            : base(spell)
        {
            Orbwalking.AfterAttack += OnAfterAttack;
            HarassEnabled = false;
            OnlyUpdateIfTargetValid = false;
        }

        private void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            _recentAutoattack = true;
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            base.Update(mode, combo, target);
            _recentAutoattack = false;
        }


        public override void Execute(AIHeroClient target)
        {
            var buff = ObjectManager.Player.GetBuff("GarenE");
            if (buff != null && buff.EndTime - Game.Time > 0.75f * (Level + 1) + 0.5f) return;
            var nearEnemyCount = ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange * 2);
            if (nearEnemyCount > 0 && (!OnlyAfterAuto || _recentAutoattack) || nearEnemyCount == 0 && UseWhenOutOfRange)
            {
                Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        public override void LaneClear()
        {
            if (_recentAutoattack)
            {
               Cast();
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        public override void Interruptable(ComboProvider combo, AIHeroClient sender, ComboProvider.InterruptableSpell interruptableSpell, float endTime)
        {
            if (endTime - Game.Time > Math.Max(sender.Distance(ObjectManager.Player) - Orbwalking.GetRealAutoAttackRange(sender), 0) / ObjectManager.Player.MoveSpeed + 0.5f)
            {
                Cast();
                Orbwalking.Orbwalk(sender, sender.Position);
            }
        }

        public override int GetPriority()
        {
            return 2;
        }
    }
}
