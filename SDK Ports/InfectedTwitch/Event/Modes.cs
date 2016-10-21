#region

using System;
using System.Linq;
using Infected_Twitch.Menus;
using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using static Infected_Twitch.Core.Spells;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Infected_Twitch.Event
{
    using Core;

    internal class Modes : Core
    {
        public static void Update(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.None:
                    AutoE();
                    break;
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Hybrid:
                    Harass();
                    break;
                case OrbwalkingMode.LaneClear:
                    Lane();
                    Jungle();
                    break;
            }
        }

        public static readonly string[] Monsters =
        {
            "SRU_Red", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "SRU_Murkwolf"
        };

        public static readonly string[] Dragons =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Earth", "SRU_Dragon_Elder", "SRU_Baron",
            "SRU_RiftHerald"
        };

        private static readonly Dmg dmg = new Dmg();

        private static void AutoE()
        {
            if (!E.IsReady()) return;

            if (MenuConfig.StealEpic)
            {
                foreach (var m in GameObjects.JungleLegendary.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (m.Health < E.GetDamage(m))
                    {
                        E.Cast();
                    }
                }
            }

            if (!MenuConfig.StealRed) return;

            var mob = GameObjects.Jungle.Where(x => x.IsValidTarget(E.Range));

            foreach (var m in mob)
            {
                if (!m.CharData.BaseSkinName.Contains("SRU_Red")) continue;

                if (m.Health < E.GetDamage(m))
                {
                    E.Cast();
                }
            }

            if (!SafeTarget(Target)
                || !MenuConfig.KillstealE
                || Target.Health > dmg.EDamage(Target))
            {
                return;
            }
          
            E.Cast();

            if (MenuConfig.Debug)
            {
                Chat.Print("Executing: ", Target.ChampionName);
            }
        }

        private static void Combo()
        {
            if (!SafeTarget(Target)) return;

            if (MenuConfig.ComboE)
            {
                if(!E.IsReady()) return;

                if (Target.Health <= dmg.EDamage(Target))
                {
                    E.Cast();

                    if (MenuConfig.Debug)
                    {
                        Chat.Print("Combo => Executing: ", Target.ChampionName);
                    }
                }
            }
            
            if (MenuConfig.UseYoumuu && Target.HealthPercent <= 80)
            {
                Usables.CastYomu();
            }

            if (Target.HealthPercent <= 85)
            {
                Usables.Botrk();
            }
           
            if (!W.IsReady() || !MenuConfig.ComboW
                || !Target.IsValidTarget(W.Range)
                || Target.Health <= Player.GetAutoAttackDamage(Target) * 2 && Target.Distance(Player) < Player.AttackRange
                || Player.ManaPercent < 8) return;

            
            W.Cast(Target.Position);
        }

        private static void Harass()
        {
            if (Target == null || !Target.IsValidTarget(Spells.E.Range)) return;

            if (Dmg.Stacks(Target) >= MenuConfig.HarassE && Target.Distance(Player) >= Player.AttackRange + 50)
            {
                E.Cast();
            }

            if (!MenuConfig.HarassW || !Spells.W.IsReady()) return;

            W.Cast(Target.Position);
        }

        private static void Lane()
        {
            var minions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Player.AttackRange)).ToList();
            if (!MenuConfig.LaneW || !W.IsReady()) return;
            
            var wPred = W.GetCircularFarmLocation(minions);

            if (wPred.MinionsHit < 4) return;

            W.Cast(wPred.Position);
        }

        private static void Jungle()
        {
            if (Player.Level == 1) return;

            if (MenuConfig.JungleW && Player.ManaPercent >= 20)
            {
                var wMob = GameObjects.Jungle.Where(m => m.IsValidTarget(W.Range)).ToList();

                if (wMob.Count == 0) return;

                var wPrediction = W.GetCircularFarmLocation(wMob);
                if (wPrediction.MinionsHit >= 3)
                {
                    W.Cast(wPrediction.Position);
                }
            }

            if (!MenuConfig.JungleE || !E.IsReady()) return;
           
            foreach (var m in GameObjects.JungleLarge.Where(m => m.IsValidTarget(E.Range)))
            {
                if (m.Health <= dmg.EDamage(m))
                {
                    E.Cast();
                }
            }
        }
    }
}
