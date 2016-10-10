using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using TheTwitch.Commons;
using TheTwitch.Commons.ComboSystem;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace TheTwitch
{
    class TwitchE : Skill
    {
        public bool Killsteal;
        public bool FarmAssist;
        public int HarassActivateWhenLeaving;
        public int MinFarmMinions;
        public int MinFarmDamageMinions;
        public Circle DrawRange;
        public bool CustomCalculation;
        public bool AlwaysExecuteAtFullStacks;

        private static readonly float[] BaseDamage = { 20, 35, 50, 65, 80 };
        private static readonly float[] StackDamage = { 15, 20, 25, 30, 35 };
        private static readonly float[] MaxDamage = { 110, 155, 200, 245, 290 };


        public TwitchE(SpellSlot spell)
            : base(spell)
        {
            Orbwalking.OnNonKillableMinion += OnNotKillableMinion;
        }



        private void OnNotKillableMinion(AttackableUnit minion)
        {
            if (!FarmAssist || minion.Position.Distance(ObjectManager.Player.Position) > 1100 || !ManaManager.CanUseMana(Orbwalking.OrbwalkingMode.LastHit)) return;
            var target = (Obj_AI_Base)minion;
            if (GetActivateDamage(target, target.GetBuffCountFixed("twitchdeadlyvenom")) > minion.Health)
                Cast();
        }

        public override void Update(Orbwalking.OrbwalkingMode mode, ComboProvider combo, AIHeroClient target)
        {
            if (Killsteal && HeroManager.Enemies.Any(enemy => enemy.IsValidTarget(1100) && CanKill(enemy)))
                Cast();
            base.Update(mode, combo, target);
        }

        public override void Execute(AIHeroClient target)
        {
            if (CanKill(target) || target.GetBuffCountFixed("twitchdeadlyvenom") == 6 && AlwaysExecuteAtFullStacks)
                Cast();
        }

        public override void Harass(AIHeroClient target)
        {
            base.Harass(target);
            if (HarassActivateWhenLeaving <= target.GetBuffCountFixed("twitchdeadlyvenom") && target.Distance(ObjectManager.Player) > Orbwalking.GetRealAutoAttackRange(target))
                Cast();
        }

        private float GetPassiveAndActivateDamage(Obj_AI_Base target, int targetBuffCount = 0)
        {
            if (targetBuffCount == 0) return 0;
            return (float)GetRemainingPoisonDamageMinusRegeneration(target) + GetActivateDamage(target, targetBuffCount);
        }

        private float GetActivateDamage(Obj_AI_Base target, int targetBuffCount = 0)
        {
            if (targetBuffCount == 0) return 0;
            return (float)ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical, Math.Min(MaxDamage[Level - 1] + ObjectManager.Player.TotalAttackDamage * 1.5f, targetBuffCount * (StackDamage[Level - 1] + ObjectManager.Player.TotalAttackDamage * 0.15f) + BaseDamage[Level - 1]));
        }

        public static float GetRemainingPoisonDamageMinusRegeneration(Obj_AI_Base target)
        {
            var buff = target.GetBuff("twitchdeadlyvenom");
            if (buff == null) return 0f;
            return (float)(ObjectManager.Player.CalcDamage(target, Damage.DamageType.True, ((int)(buff.EndTime - Game.Time) + 1) * GetPoisonTickDamage() * buff.Count)) - ((int)(buff.EndTime - Game.Time)) * target.HPRegenRate;
        }

        private static int GetPoisonTickDamage()
        {
            if (ObjectManager.Player.Level > 16) return 6;
            if (ObjectManager.Player.Level > 12) return 5;
            if (ObjectManager.Player.Level > 8) return 4;
            if (ObjectManager.Player.Level > 4) return 3;
            return 2;
        }

        public override void LaneClear()
        {
            var killable = 0;
            var poison = 0;
            var minions = MinionManager.GetMinions(1100, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in minions)
            {
                var buffCount = minion.GetBuffCountFixed("twitchdeadlyvenom");
                if (buffCount == 0) continue;
                poison++;
                if (CanKill(minion, false))
                    killable++;
            }
            if (MinFarmMinions <= killable || MinFarmDamageMinions <= poison)
            {
                Cast();
            }
            base.LaneClear();
        }

        public override void Draw()
        {
            if (DrawRange.Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1100, DrawRange.Color);
        }

        public override int GetPriority()
        {
            return 4;
        }

        private bool CanKill(Obj_AI_Base target, bool includePassiveDamage = true)
        {
            if (CustomCalculation)
            {
                var targetBuffCountKs = target.GetBuffCountFixed("twitchdeadlyvenom");
                if (targetBuffCountKs == 0) return false;
                return (includePassiveDamage ? GetPassiveAndActivateDamage(target, targetBuffCountKs) : GetActivateDamage(target, targetBuffCountKs)) > target.Health;
            }
            return IsKillable(target);
        }
    }
}
