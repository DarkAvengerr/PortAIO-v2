using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using TheKalista.Commons;
using TheKalista.Commons.ComboSystem;
using TheKalista.Commons.Items;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheGaren
{
    class GarenE : Skill
    {
        public bool OnlyAfterAuto;
        public int MinFarmMinions;
        public bool UseHydra;
        private bool _recentAutoattack;
        private bool _resetOrbwalker;
        private GarenQ _q;
        private GarenR _r;
        public ItemManager ItemManager;

        public GarenE(SpellSlot spell)
            : base(spell)
        {
            HarassEnabled = false;
            Orbwalking.AfterAttack += OnAfterAttack;
            OnlyUpdateIfTargetValid = false;
            OnlyUpdateIfCastable = false;
        }

        public override void Initialize(ComboProvider combo)
        {
            _q = combo.GetSkill<GarenQ>();
            _r = combo.GetSkill<GarenR>();
            base.Initialize(combo);
        }

        private void OnAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            _recentAutoattack = true;
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            base.Update(mode, combo, target);
            _recentAutoattack = false;
            if (_resetOrbwalker && !ObjectManager.Player.HasBuff("GarenE"))
            {
                _resetOrbwalker = false;
                Provider.Orbwalker.SetAttack(true);
            }
        }

        public override void Execute(AIHeroClient target)
        {
            if (!CanBeCast()) return;
            if (_r.CanBeCast() && Instance.Name != "GarenE" && target.IsValidTarget() && _r.IsKillable(target))
            {
                Cast();
                return;
            }
            if ((_q.Instance.State == SpellState.Cooldown || _q.Instance.State == SpellState.NotLearned) && !ObjectManager.Player.HasBuff("GarenQ") && (!OnlyAfterAuto || !AAHelper.WillAutoattackSoon || _recentAutoattack) && HeroManager.Enemies.Any(enemy => enemy.IsValidTarget() && Instance.Name == "GarenE" && enemy.Position.Distance(ObjectManager.Player.Position) < 325))
            {
                Provider.Orbwalker.SetAttack(false);
                _resetOrbwalker = true;
                Cast();
            }
        }

        public override void LaneClear()
        {
            if (MinionManager.GetMinions(325, MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.None).Count >= MinFarmMinions)
            {
                if (!ObjectManager.Player.HasBuff("GarenQ") && Instance.Name == "GarenE")
                    Cast();
                if (UseHydra) ItemManager.GetItem<RavenousHydra>().Use(null);
            }
        }

        public override int GetPriority()
        {
            return ObjectManager.Player.HasBuff("GarenE") ? 3 : 1;
        }
    }
}
