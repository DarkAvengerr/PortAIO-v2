using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using SharpDX;
    using Common;

    /// <summary>
    /// (This Part From SebbyLib)
    /// </summary>
   internal static class TCommon
   {
        private static readonly List<UnitIncomingDamage> IncomingDamageList = new List<UnitIncomingDamage>();
        private static readonly List<AIHeroClient> ChampionList = new List<AIHeroClient>();

        private static AIHeroClient Player => PlaySharp.Player;

        static TCommon()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                ChampionList.Add(hero);
            }

            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Game_OnUpdate(EventArgs Args)
        {
            var time = Game.Time - 2;
            IncomingDamageList.RemoveAll(damage => time < damage.Time);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.SData == null)
            {
                return;
            }

            var targed = Args.Target as Obj_AI_Base;

            if (targed != null)
            {
                if (targed.Type == GameObjectType.AIHeroClient && targed.Team != sender.Team && sender.IsMelee)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage
                    {
                        Damage = (sender as AIHeroClient).GetSpellDamage(targed, Args.Slot),
                        TargetNetworkId = Args.Target.NetworkId,
                        Time = Game.Time,
                        Skillshot = false
                    });
                }
            }
            else
            {
                foreach (var champion in ChampionList.Where(champion => !champion.IsDead && champion.IsHPBarRendered && champion.Team != sender.Team && champion.Distance(sender) < 2000))
                {
                    if (CanHitSkillShot(champion, Args))
                    {
                        IncomingDamageList.Add(new UnitIncomingDamage
                        {
                            Damage = champion.GetSpellDamage(targed, Args.Slot),
                            TargetNetworkId = champion.NetworkId,
                            Time = Game.Time,
                            Skillshot = true
                        });
                    }
                }
            }
        }

        public static bool CanHitSkillShot(Obj_AI_Base target, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Args.Target == null && target.IsValidTarget(1000))
            {
                var pred = Movement.GetPrediction(target, 0.25f).CastPosition;

                if (Args.SData.LineWidth > 0)
                {
                    var powCalc = Math.Pow(Args.SData.LineWidth + target.BoundingRadius, 2);

                    if (pred.ToVector2().Distance(Args.End.ToVector2(), Args.Start.ToVector2(), true, true) <= powCalc ||
                        target.ServerPosition.ToVector2().Distance(Args.End.ToVector2(), Args.Start.ToVector2(), true, true) <= powCalc)
                    {
                        return true;
                    }
                }
                else if (target.Distance(Args.End) < 50 + target.BoundingRadius || pred.Distance(Args.End) < 50 + target.BoundingRadius)
                {
                    return true;
                }
            }
            return false;
        }

        public static double GetIncomingDamage(AIHeroClient target, float time = 0.5f, bool skillshots = true)
        {
            double totalDamage = 0;

            foreach (var damage in IncomingDamageList.Where(damage => damage.TargetNetworkId == target.NetworkId && Game.Time - time < damage.Time))
            {
                if (skillshots)
                {
                    totalDamage += damage.Damage;
                }
                else
                {
                    if (!damage.Skillshot)
                        totalDamage += damage.Damage;
                }
            }

            return totalDamage;
        }
    }

    public class UnitIncomingDamage
    {
        public int TargetNetworkId { get; set; }
        public float Time { get; set; }
        public double Damage { get; set; }
        public bool Skillshot { get; set; }
    }
}