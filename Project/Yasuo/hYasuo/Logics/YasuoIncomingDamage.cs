using hYasuo.Extensions;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hYasuo.Logics
{
    internal class YasuoIncomingDamage
    {
        public static void IncomingDamage (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender is AIHeroClient && sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient
                && args.End.Distance(ObjectManager.Player.Position) < 400f && 
                args.SData.TargettingType != SpellDataTargetType.Unit
                && Spells.W.IsReady())
            {
                var calcmagicaldamage = ((AIHeroClient) args.Target).CalcDamage(ObjectManager.Player, Damage.DamageType.Magical,
                    ((AIHeroClient) args.Target).GetSpellDamage(ObjectManager.Player, args.Slot));

                var calcpsydamage = ((AIHeroClient)args.Target).CalcDamage(ObjectManager.Player, Damage.DamageType.Physical,
                    ((AIHeroClient)args.Target).GetSpellDamage(ObjectManager.Player, args.Slot));

                var calctrue = ((AIHeroClient)args.Target).CalcDamage(ObjectManager.Player, Damage.DamageType.True,
                    ((AIHeroClient)args.Target).GetSpellDamage(ObjectManager.Player, args.Slot));

                // no way to get enemy spell damage type. spagetti code 10/10

                if (calcmagicaldamage > ObjectManager.Player.Health || 
                    calcpsydamage > ObjectManager.Player.Health || calctrue > ObjectManager.Player.Health)
                {
                    Spells.W.Cast(ObjectManager.Player.Position.Extend(args.Start, Spells.W.Range));
                }

            }
        }

    }
}
