using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using EloBuddy;

namespace Mordekaiser.Logics
{
    public class LogicW
    {
        public static void Initiate()
        {
            Game.OnUpdate += GameOnUpdate;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
        }

        private static void GameOnUpdate(EventArgs args)
        {
            if (Utils.Player.Self.Spellbook.GetSpell(SpellSlot.W).Name != "mordekaisercreepingdeath2")
                return;

            var countEnemy = Utils.Player.Self.LSCountAlliesInRange(Spells.WDamageRadius);

            if (countEnemy == 0)
                return;

            var t = TargetSelector.GetTarget(Spells.WDamageRadius, TargetSelector.DamageType.Magical);
            if (!t.LSIsValidTarget()) 
                return;

            var targetMovementSpeed = t.MoveSpeed;
            var myMovementSpeed = Utils.Player.Self.MoveSpeed;

            if (myMovementSpeed <= targetMovementSpeed)
            {
                if (!t.LSIsFacing(Utils.Player.Self) && t.Path.Count() >= 1 &&
                    t.LSDistance(Utils.Player.Self) > Spells.WDamageRadius - 20)
                {
                    Spells.W.Cast();
                    return;
                }
            }
        }

        public static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (args.SData.Name == ((AIHeroClient) sender).GetSpell(SpellSlot.W).Name)
            {
                if (((AIHeroClient) sender).GetSpell(SpellSlot.W).Name == "mordekaisercreepingdeath2")
                    Spells.WCastedTime = Environment.TickCount;
                else
                    Spells.WCastedTime = 0;
            }
        }
    }
}
