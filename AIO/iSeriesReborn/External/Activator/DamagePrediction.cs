using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iSeriesReborn.External.Activator
{
    class DamagePrediction
    {
        public delegate void OnKillableDelegate(AIHeroClient sender, AIHeroClient target, SpellData sData);
        public static event OnKillableDelegate OnSpellWillKill;

        static DamagePrediction()
        {
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            return;
            if (!(sender is AIHeroClient) || !(args.Target is AIHeroClient))
                return;
            var senderH = sender as AIHeroClient;
            var targetH = args.Target as AIHeroClient;
            var damage = Orbwalking.IsAutoAttack(args.SData.Name) ? sender.GetAutoAttackDamage(targetH) : GetDamage(senderH, targetH, senderH.GetSpellSlot(args.SData.Name));

            if (damage > targetH.Health + 15)
            {
                if (OnSpellWillKill != null)
                {
                    OnSpellWillKill(senderH, targetH, args.SData);
                }
            }
        }

        static float GetDamage(AIHeroClient hero, AIHeroClient target, SpellSlot slot)
        {
            return (float)hero.GetSpellDamage(target, slot);
        }
    }
}