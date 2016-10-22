using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Lissandra_the_Ice_Goddess.Utility.Damage
{
    class DamagePrediction
    {
        public delegate void OnKillableDelegate(AIHeroClient sender, AIHeroClient target, SpellData SData);

        public static event OnKillableDelegate OnTargettedSpellWillKill;

        private static int HealthBuffer = 15; //Safety check

        static DamagePrediction()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient) || !(args.Target is AIHeroClient))
            {
                return;
            }

            var senderHero = (AIHeroClient) sender;
            var targetHero = (AIHeroClient) args.Target;

            var predictedDamage = Orbwalking.IsAutoAttack(args.SData.Name)
                ? senderHero.GetAutoAttackDamage(targetHero, true)
                : senderHero.GetSpellDamage(targetHero, args.SData.Name);

            if (predictedDamage > targetHero.Health + HealthBuffer)
            {
                if (OnTargettedSpellWillKill != null)
                {
                    OnTargettedSpellWillKill(senderHero, targetHero, args.SData);
                }
            }
        }
    }
}
