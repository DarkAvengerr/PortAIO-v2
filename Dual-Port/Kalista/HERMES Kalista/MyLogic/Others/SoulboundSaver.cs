using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HERMES_Kalista.MyLogic.Others
{
    public static class SoulboundSaver
    {
        private static AIHeroClient _connectedAlly;
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
                if (_connectedAlly != null && Program.R.LSIsReady())
                {
                    if ((!(sender is AIHeroClient) || args.SData.LSIsAutoAttack()) && args.Target != null &&
                        args.Target.NetworkId == _connectedAlly.NetworkId)
                    {
                        _incomingDamage.Add(
                            _connectedAlly.ServerPosition.LSDistance(sender.ServerPosition)/args.SData.MissileSpeed +
                            Game.Time, (float) sender.LSGetAutoAttackDamage(_connectedAlly));
                    }
                    else if (sender is AIHeroClient)
                    {
                        var attacker = (AIHeroClient) sender;
                        var slot = attacker.LSGetSpellSlot(args.SData.Name);

                        if (slot != SpellSlot.Unknown)
                        {
                            if (slot == attacker.LSGetSpellSlot("SummonerDot") && args.Target != null &&
                                args.Target.NetworkId == _connectedAlly.NetworkId)
                            {
                                _instantDamage.Add(Game.Time + 2,
                                    (float) attacker.GetSummonerSpellDamage(_connectedAlly, Damage.SummonerSpell.Ignite));
                            }
                            else if (slot.HasFlag(SpellSlot.Q | SpellSlot.W | SpellSlot.E | SpellSlot.R) &&
                                     ((args.Target != null && args.Target.NetworkId == _connectedAlly.NetworkId) ||
                                      args.End.LSDistance(_connectedAlly.ServerPosition) <
                                      Math.Pow(args.SData.LineWidth, 2)))
                            {
                                _instantDamage.Add(Game.Time + 2, (float) attacker.LSGetSpellDamage(_connectedAlly, slot));
                            }
                        }
                    }
                }
            }

            if (sender.IsMe)
            {
                if (args.SData.Name == "KalistaExpungeWrapper")
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(250, Orbwalking.ResetAutoAttackTimer);
                }
            }
        }

        public static void OnUpdate(EventArgs args)
        {
            if (!Program.ComboMenu.Item("RComboSupport").GetValue<bool>() || ObjectManager.Player.LSIsRecalling() || ObjectManager.Player.LSInFountain())
                return;

            if (_connectedAlly == null)
            {
                _connectedAlly = HeroManager.Allies.FirstOrDefault(a => a.LSHasBuff("kalistacoopstrikeally"));
                return;
            }
            else
            {
                if (IncomingDamage > _connectedAlly.Health && _connectedAlly.LSCountEnemiesInRange(500) > 0)
                {
                    Program.R.Cast();
                }
                else
                {
                    if ((_connectedAlly.ChampionName == "Blitzcrank" || _connectedAlly.ChampionName == "Skarner" ||
                         _connectedAlly.ChampionName == "TahmKench"))
                    {
                        foreach (
                            var unit in
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(
                                        h => h.IsEnemy && h.IsHPBarRendered && _connectedAlly.LSDistance(h.Position) > 800)
                            )
                        {
                            // Get buffs
                            for (int i = 0; i < unit.Buffs.Count(); i++)
                            {
                                // Check if the Soulbound is in a good range
                                var enemy = HeroManager.Enemies.Where(x => _connectedAlly.LSDistance(unit.Position) > 800);
                                // Check if the Soulbound is a Blitzcrank
                                // Check if the enemy is hooked
                                // Check if target was far enough for ult
                                if ((unit.Buffs[i].Name.ToLower() == "rocketgrab2" ||
                                     unit.Buffs[i].Name == "skarnerimpale".ToLower() ||
                                     unit.Buffs[i].Name.ToLower() == "tahmkenchwdevoured") &&
                                    unit.Buffs[i].IsActive && enemy.Count() > 0)
                                {
                                    Program.R.Cast();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
