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
 namespace DZAIO_Reborn.Plugins.Champions.Sivir.Modules
{
    class SivirQKS : IModule
    {
        public void OnLoad()
        {

        }

        public bool ShouldGetExecuted()
        {
            return Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.sivir.extra.autoQKS") &&
                   ObjectManager.Player.ManaPercent > 35 && Variables.Spells[SpellSlot.Q].LSIsReady();

        }

        public DZAIOEnums.ModuleType GetModuleType()
        {
            return DZAIOEnums.ModuleType.OnUpdate;
        }

        public void OnExecute()
        {
            var target = HeroManager.Enemies.FirstOrDefault(enemy => enemy.Health + 5 < GetRealDamage(enemy)
                && enemy.LSIsValidTarget(Variables.Spells[SpellSlot.Q].Range));

            if (target != null
                && (target.NetworkId != Variables.Orbwalker.GetTarget().NetworkId))
            {
                Variables.Spells[SpellSlot.Q].CastIfHitchanceEquals(target, HitChance.High);
            }
        }

        public float GetRealDamage(AIHeroClient hero)
        {
            var baseDamage = Variables.Spells[SpellSlot.Q].GetDamage(hero);

            var Prediction = Variables.Spells[SpellSlot.Q].GetPrediction(hero);
            var collisionCount = Prediction.CollisionObjects.Count();
            var tempDamage = baseDamage;
            for (var i = 0; i < collisionCount - 1; i++)
            {
                tempDamage *= 0.85f;
            }

            return tempDamage < baseDamage * 0.40f ? baseDamage * 0.40f : tempDamage;
        }
    }
}
