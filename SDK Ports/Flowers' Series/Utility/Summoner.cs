using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Flowers_Series.Utility
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.UI;
    using System;
    using System.Linq;

    internal class Summoner
    {
        private static AIHeroClient Me => Program.Me;
        private static Menu Menu => Tools.Menu;

        private static SpellSlot Ignite = SpellSlot.Unknown;
        private static SpellSlot Exhaust = SpellSlot.Unknown;
        private static SpellSlot Heal = SpellSlot.Unknown;

        private static float IgniteRange = 600f, ExhaustRange = 650f;

        public static void Inject()
        {
            Ignite = Me.GetSpellSlot("SummonerDot");
            Exhaust = Me.GetSpellSlot("SummonerExhaust");
            Heal = Me.GetSpellSlot("SummonerHeal");


            var SummonerMenu = Menu.Add(new Menu("Summoner", "Summoner"));
            {
                if (Ignite != SpellSlot.Unknown)
                {
                    var IgniteMenu = SummonerMenu.Add(new Menu("Ignite", "Ignite"));
                    {
                        IgniteMenu.Add(new MenuBool("Enable", "Enabled", Tools.EnableActivator));
                        IgniteMenu.Add(new MenuBool("Combo", "Use In Combo", true));
                        IgniteMenu.Add(new MenuBool("KillSteal", "Use In KillSteal", true));
                    }

                    Manager.WriteConsole("Ignite Menu Load");
                }

                if (Heal != SpellSlot.Unknown)
                {
                    var HealMenu = SummonerMenu.Add(new Menu("Heal", "Heal"));
                    {
                        HealMenu.Add(new MenuBool("Enable", "Enabled", Tools.EnableActivator));
                        HealMenu.Add(new MenuBool("AllyHeal", "AllyHeal", true));
                        HealMenu.Add(new MenuSeparator("Credit", "Credit: Sebby"));
                    }

                    Manager.WriteConsole("Heal Menu Load");
                }

                if (Exhaust != SpellSlot.Unknown)
                {
                    var ExhaustMenu = SummonerMenu.Add(new Menu("Exhaust", "Exhaust"));
                    {
                        ExhaustMenu.Add(new MenuBool("Enable", "Enabled", true));

                        if (GameObjects.EnemyHeroes.Any())
                        {
                            GameObjects.EnemyHeroes.ForEach(i => ExhaustMenu.Add(new MenuBool(i.ChampionName.ToLower(), "Use On" + i.ChampionName, true)));
                        }

                        ExhaustMenu.Add(new MenuSeparator("Credit", "Credit: Kurisu"));
                    }

                    Manager.WriteConsole("Heal Menu Load");
                }
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Ignite != SpellSlot.Unknown && Ignite.IsReady() && Menu["Summoner"]["Ignite"]["Enable"])
            {
                IgniteLogic();
            }

            if (Heal != SpellSlot.Unknown && Heal.IsReady() && Menu["Summoner"]["Heal"]["Enable"])
            {
                HealLogic();
            }

            if (Exhaust != SpellSlot.Unknown && Exhaust.IsReady() && Menu["Summoner"]["Exhaust"]["Enable"])
            {
                ExhaustLogic();
            }
        }

        private static void ExhaustLogic()
        {
            var hid = GameObjects.EnemyHeroes.OrderByDescending(h => h.TotalAttackDamage).FirstOrDefault(h => h.IsValidTarget(ExhaustRange + 250));

            foreach (var hero in GameObjects.AllyHeroes)
            {
                var enemies = Variables.TargetSelector.GetTargets(ExhaustRange, DamageType.Physical);

                if (enemies.Count() == 0 || hid == null)
                {
                    continue;
                }

                foreach (var target in enemies)
                {
                    if (!Menu["Summoner"]["Exhaust"][target.ChampionName.ToLower()])
                    {
                        continue;
                    }

                    if (target.DistanceToPlayer() > 1250)
                    {
                        continue;
                    }

                    if (target.DistanceToPlayer() <= ExhaustRange)
                    {
                        if (hero.Health / hero.MaxHealth * 100 <= 60)
                        {
                            if (hero.IsFacing(target))
                            {
                                if (target.NetworkId == hid.NetworkId)
                                {
                                    Me.Spellbook.CastSpell(Exhaust, target);
                                }
                            }
                        }

                        if (target.Health / target.MaxHealth * 100 <= 30)
                        {
                            if (!target.IsFacing(hero))
                            {
                                Me.Spellbook.CastSpell(Exhaust, target);
                            }
                        }
                    }
                }
            }
        }

        private static void HealLogic()
        {
            foreach (var ally in GameObjects.AllyHeroes.Where(ally => ally.IsValid && !ally.IsDead && ally.HealthPercent < 50 && Me.Distance(ally.ServerPosition) < 700))
            {
                double dmg = OKTWCommon.GetIncomingDamage(ally, 1);

                var enemys = ally.CountEnemyHeroesInRange(700);

                if (dmg == 0 && enemys == 0)
                {
                    continue;
                }

                if (!Menu["Summoner"]["Heal"]["AllyHeal"] && !ally.IsMe)
                {
                    return;
                }

                if (ally.Health - dmg < enemys * ally.Level * 15)
                {
                    Me.Spellbook.CastSpell(Heal, ally);
                }
                else if (ally.Health - dmg < ally.Level * 10)
                {
                    Me.Spellbook.CastSpell(Heal, ally);
                }
            }
        }

        private static void IgniteLogic()
        {
            var target = Manager.GetTarget(IgniteRange, DamageType.True);

            if (Manager.CheckTarget(target))
            {
                if (Manager.InCombo && Menu["Summoner"]["Ignite"]["Combo"] && target.IsValidTarget(IgniteRange) && target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                    return;
                }

                if (GetIgniteDamage(target) > target.Health && target.IsValidTarget(IgniteRange))
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                    return;
                }
            }
        }

        private static double GetIgniteDamage(AIHeroClient target)
        {
            return 50 + 20 * Me.Level - (target.HPRegenRate / 5 * 3);
        }
    }
}
