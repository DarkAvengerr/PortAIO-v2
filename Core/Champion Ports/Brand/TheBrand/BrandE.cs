using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using TheBrand.Commons;
using TheBrand.Commons.ComboSystem;
using EloBuddy;

namespace TheBrand
{
    class BrandE : Skill
    {
        private BrandQ _brandQ;
        private Obj_AI_Base _recentFarmTarget;
        public bool UseMinions;
        public bool FarmAssist;
        public bool Killsteal;
        public bool KillstealCombo;
        public bool InterruptE;


        public BrandE(SpellSlot slot)
            : base(slot) { }

        public override void Initialize(ComboProvider combo)
        {
            _brandQ = combo.GetSkill<BrandQ>();
            Orbwalking.OnNonKillableMinion += OnMinionUnkillable;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            base.Initialize(combo);
        }

        void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            _recentFarmTarget = args.Target.Type == GameObjectType.obj_AI_Base ? (Obj_AI_Base)args.Target : _recentFarmTarget;
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (Killsteal && (mode == Orbwalking.OrbwalkingMode.Combo || !KillstealCombo))
                foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(650)))
                {
                    if (!IsKillable(enemy)) continue;
                    Cast(enemy);
                }
            base.Update(mode, combo, target);
        }

        public override void Execute(AIHeroClient target)
        {
            var distance = target.Distance(ObjectManager.Player); //Todo: make him use fireminions even in range, just for showoff and potential AOE. Check if hes on fire too though
            if (distance < 950 && distance > 650 && UseMinions)
            {
                var fireMinion = MinionManager.GetMinions(650, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None).Where(minion => minion.HasBuff("brandablaze") && minion.Distance(target) < 300).MinOrDefault(minion => minion.Distance(target));
                if (fireMinion != null)
                {
                    if (Cast(fireMinion) == CastStates.SuccessfullyCasted && !target.HasSpellShield())
                        Provider.SetMarked(target);
                }
            }
            if (distance < 650)
            {
               
                if (Cast(target) == CastStates.SuccessfullyCasted && !target.HasSpellShield())
                    Provider.SetMarked(target);
            }
        }



        public override void LaneClear(ComboProvider combo, AIHeroClient target)
        {
            var minions = MinionManager.GetMinions(650, MinionTypes.All, MinionTeam.NotAlly).Where(minion => minion.HasBuff("brandablaze")).ToArray();
            if (!minions.Any()) return;
            Obj_AI_Base bestMinion = null;
            var neighbours = -1;
            foreach (var minion in minions)
            {
                var currentNeighbours = minions.Count(neighbour => neighbour.Distance(minion) < 300);
                if (currentNeighbours <= neighbours) continue;
                bestMinion = minion;
                neighbours = currentNeighbours;
            }
            Cast(bestMinion);
        }

        private void OnMinionUnkillable(AttackableUnit minion)
        {
            if (!FarmAssist) return;
            if (Provider.Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && minion.Position.Distance(ObjectManager.Player.Position) < 650 && ManaManager.CanUseMana(Orbwalking.OrbwalkingMode.LastHit) && (_recentFarmTarget == null || minion.NetworkId != _recentFarmTarget.NetworkId))
            {
                Cast(minion as Obj_AI_Base);
            }
        }

        public override void Gapcloser(ComboProvider combo, ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsValidTarget()) return;
            if (_brandQ.CanBeCast() && CanBeCast() && !gapcloser.Sender.HasBuff("brandablaze"))
                Execute(gapcloser.Sender);
        }

        public override void Interruptable(ComboProvider combo, AIHeroClient sender, ComboProvider.InterruptableSpell interruptableSpell, float endTime)
        {
            if (InterruptE && _brandQ.CouldHit(sender) && sender.Distance(ObjectManager.Player) < 650)
                Execute(sender);
        }

        public override int GetPriority()
        {
            return Provider.Target != null ? (Provider.Target.HasBuff("brandablaze") ? 0 : 4) : 0;
        }
    }
}
