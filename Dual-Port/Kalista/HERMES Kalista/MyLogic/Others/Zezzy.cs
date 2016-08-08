using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HERMES_Kalista.MyUtils;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HERMES_Kalista.MyLogic.Others
{
    public static class ZezzysPunisher
    {
        private static Dictionary<float, float> _incomingDamage = new Dictionary<float, float>();
        private static Dictionary<float, float> _instantDamage = new Dictionary<float, float>();

        public static float IncomingDamage
        {
            get { return _incomingDamage.Sum(e => e.Value) + _instantDamage.Sum(e => e.Value); }
        }

        //credits to hellsing, and jquery
        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy)
            {
                if (Program.E.LSIsReady())
                {
                    if ((!(sender is AIHeroClient) || args.SData.LSIsAutoAttack()) && args.Target != null &&
                        args.Target.NetworkId == ObjectManager.Player.NetworkId)
                    {
                        _incomingDamage.Add(
                            ObjectManager.Player.ServerPosition.LSDistance(sender.ServerPosition)/args.SData.MissileSpeed +
                            Game.Time, (float) sender.LSGetAutoAttackDamage(ObjectManager.Player));
                    }
                    else if (sender is AIHeroClient)
                    {
                        var attacker = (AIHeroClient) sender;
                        var slot = attacker.LSGetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.LSGetSpellSlot("SummonerDot") && args.Target != null &&
                                args.Target.NetworkId == ObjectManager.Player.NetworkId)
                            {
                                _instantDamage.Add(Game.Time + 2,
                                    (float)
                                        attacker.GetSummonerSpellDamage(ObjectManager.Player,
                                            Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                                     ((args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId) ||
                                      args.End.LSDistance(ObjectManager.Player.ServerPosition) <
                                      Math.Pow(args.SData.LineWidth, 2)))
                            {
                                _instantDamage.Add(Game.Time + 2,
                                    (float) attacker.LSGetSpellDamage(ObjectManager.Player, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe)
            {
                if (args.SData.Name == "KalistaExpungeWrapper")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, MyOrbwalker.ResetAutoAttackTimer);
                }
            }
        }

        public static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.LSIsRecalling() || ObjectManager.Player.LSInFountain() || !Program.E.LSIsReady())
                return;

            if ((ObjectManager.Player.HealthPercent < 10 || IncomingDamage >= ObjectManager.Player.Health) && ObjectManager.Player.LSCountEnemiesInRange(Program.E.Range) > 0)
            {
                Program.E.Cast();
            }
        }
    }
}