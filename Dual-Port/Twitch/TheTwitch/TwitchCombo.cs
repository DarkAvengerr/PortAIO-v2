using System;
using System.Collections.Generic;
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
    class TwitchCombo : ComboProvider
    {
        public int BlueTrinketLevel;
        public bool AutoBuyBlueTrinket;

        public TwitchCombo(float targetSelectorRange, IEnumerable<Skill> skills, Orbwalking.Orbwalker orbwalker)
            : base(targetSelectorRange, skills, orbwalker)
        {
        }

        public TwitchCombo(float targetSelectorRange, Orbwalking.Orbwalker orbwalker, params Skill[] skills)
            : base(targetSelectorRange, orbwalker, skills)
        {
        }

        protected override void OnUpdate(Orbwalking.OrbwalkingMode mode)
        {
            base.OnUpdate(mode);

            if (AutoBuyBlueTrinket && ObjectManager.Player.Level >= BlueTrinketLevel && ObjectManager.Player.InFountain() && ObjectManager.Player.InventoryItems.Any(item => item.Id == ItemId.Warding_Totem_Trinket))
            {
                Shop.BuyItem(ItemId.Farsight_Alteration);
            }
        }

        public override bool ShouldBeDead(EloBuddy.Obj_AI_Base target, float additionalSpellDamage = 0f)
        {
            return base.ShouldBeDead(target, TwitchE.GetRemainingPoisonDamageMinusRegeneration(target));
        }

        protected override AIHeroClient SelectTarget()
        {
            var target = base.SelectTarget();
            if (target.IsValidTarget() && TargetSelector.IsInvulnerable(target, TargetSelector.DamageType.Physical))
                return HeroManager.Enemies.Where(enemy => enemy.IsValidTarget(TargetRange) && !enemy.IsBehindWindWall() && !TargetSelector.IsInvulnerable(enemy, TargetSelector.DamageType.Physical)).MaxOrDefault(TargetSelector.GetPriority);
            return target;
        }
    }
}
