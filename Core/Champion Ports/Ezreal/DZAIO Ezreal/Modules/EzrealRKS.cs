using System;
using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Plugins.Champions.Kalista;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Ezreal.Modules
{
    class EzrealRKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.ezreal.extra.autoRKS") &&
                   ObjectManager.Player.ManaPercent >= 15 && Variables.Spells[SpellSlot.R].IsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = HeroManager.Enemies.FirstOrDefault(enemy => CanExecuteTarget(enemy)
                && enemy.IsValidTarget(Variables.Spells[SpellSlot.Q].Range));

            if (target != null
                && (target.NetworkId != Variables.Orbwalker.GetTarget().NetworkId))
            {
                Variables.Spells[SpellSlot.R].CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        private bool CanExecuteTarget(Obj_AI_Base target)
        {
            double damage = 1f;

            var prediction = Variables.Spells[SpellSlot.R].GetPrediction(target);
            var count = prediction.CollisionObjects.Count;

            damage += ObjectManager.Player.GetSpellDamage(target, SpellSlot.R);

            if (count >= 7)
            {
                damage = damage * .3f;
            }
            else if (count != 0)
            {
                damage = damage * (10 - count / 10f);
            }

            return damage >= target.Health + 10f;
        }
    }
}
