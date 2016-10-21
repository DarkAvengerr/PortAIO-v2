using System.Collections.Generic;
using System.Linq;
using DZAIO_Reborn.Core;
using DZAIO_Reborn.Helpers;
using DZAIO_Reborn.Helpers.Modules;
using DZAIO_Reborn.Helpers.Positioning;
using DZAIO_Reborn.Plugins.Champions.Orianna.BallManager;
using DZAIO_Reborn.Plugins.Interface;
using DZLib.Core;
using DZLib.Menu;
using DZLib.MenuExtensions;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace DZAIO_Reborn.Plugins.Champions.Orianna
{
    class Orianna : IChampion
    {
        public PetManager BallManager;
 
        public void OnLoad(Menu menu)
        {
            var comboMenu = new Menu(ObjectManager.Player.ChampionName + ": Combo", "dzaio.champion.orianna.combo");
            {
                comboMenu.AddModeMenu(ModesMenuExtensions.Mode.Combo, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R }, new[] { true, true, true, true });
                //comboMenu.AddNoUltiMenu(false);
                menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu(ObjectManager.Player.ChampionName + ": Mixed", "dzaio.champion.orianna.harrass");
            {
                mixedMenu.AddModeMenu(ModesMenuExtensions.Mode.Harrass, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true, true });
                mixedMenu.AddSlider("dzaio.champion.orianna.mixed.mana", "Min Mana % for Harass", 30, 0, 100);
                menu.AddSubMenu(mixedMenu);
            }

            var farmMenu = new Menu(ObjectManager.Player.ChampionName + ": Farm", "dzaio.champion.orianna.farm");
            {
                farmMenu.AddModeMenu(ModesMenuExtensions.Mode.Laneclear, new[] { SpellSlot.Q, SpellSlot.W }, new[] { true, true });

                farmMenu.AddSlider("dzaio.champion.orianna.farm.w.min", "Min Minions for W", 2, 1, 6);
                farmMenu.AddSlider("dzaio.champion.orianna.farm.mana", "Min Mana % for Farm", 30, 0, 100);
                menu.AddSubMenu(farmMenu);
            }

            var extraMenu = new Menu(ObjectManager.Player.ChampionName + ": Extra", "dzaio.champion.orianna.extra");
            {
                extraMenu.AddStringList("dzaio.champion.orianna.extra.interrupter.mode", "Interrupter Mode", new []{"Q->R", "Only R"});
                extraMenu.AddBool("dzaio.champion.orianna.extra.interrupter", "Interrupter", true);
            }

            Variables.Spells[SpellSlot.Q].SetSkillshot(0f, 110f, 1425f, false, SkillshotType.SkillshotLine);
            Variables.Spells[SpellSlot.E].SetSkillshot(0.25f, 80f, 1700f, true, SkillshotType.SkillshotLine);
            CommandQueue.InitEvents();
            BallManager = new PetManager();
            BallManager.OnLoad();
        }

        public void RegisterEvents()
        {
            DZInterrupter.OnInterruptableTarget += OnInterrupter;
            DZAntigapcloser.OnEnemyGapcloser += OnGapcloser;
        }

        private void OnGapcloser(DZLib.Core.ActiveGapcloser gapcloser)
        {
            
        }

        private void OnInterrupter(AIHeroClient sender, DZInterrupter.InterruptableTargetEventArgs args)
        {
            if (Variables.Spells[SpellSlot.R].IsReady() 
                && sender.IsValidTarget(Variables.Spells[SpellSlot.Q].Range) 
                && args.DangerLevel >= DZInterrupter.DangerLevel.High
                && Variables.AssemblyMenu.GetItemValue<bool>("dzaio.champion.orianna.extra.interrupter"))
            {
                switch (
                    Variables.AssemblyMenu.GetItemValue<StringList>("dzaio.champion.orianna.extra.interrupter.mode")
                        .SelectedIndex)
                {
                    case 0:
                        this.BallManager.ProcessCommand(new Command()
                        {
                            SpellCommand = Commands.Q,
                            Where = sender.ServerPosition
                        });

                        var actionDelay =
                            (int)
                                (BallManager.BallPosition.Distance(sender.ServerPosition) /
                                 Variables.Spells[SpellSlot.Q].Speed * 1000f +
                                 Variables.Spells[SpellSlot.Q].Delay * 1000f + Game.Ping / 2f + 100f);

                        LeagueSharp.Common.Utility.DelayAction.Add(
                            actionDelay, () =>
                            {
                                var enemiesInRange =
                                    BallManager.BallPosition.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range);

                                if (enemiesInRange.Count >= 1 &&
                                    enemiesInRange.Any(n => n.NetworkId == sender.NetworkId))
                                {
                                    Variables.Spells[SpellSlot.R].Cast();
                                }
                            });
                
                    break;
                    case 1:
                        var ballPosition = BallManager.BallPosition;
                        if (sender.IsValidTarget(Variables.Spells[SpellSlot.R].Range / 2f, true, ballPosition))
                        {
                            var enemiesInRange =
                                    BallManager.BallPosition.GetEnemiesInRange(Variables.Spells[SpellSlot.R].Range);

                            if (enemiesInRange.Count >= 1 &&
                                enemiesInRange.Any(n => n.NetworkId == sender.NetworkId))
                            {
                                Variables.Spells[SpellSlot.R].Cast();
                            }
                        }
                    break;
                }
                

                
            }
        }
        public Dictionary<SpellSlot, Spell> GetSpells()
        {
            return new Dictionary<SpellSlot, Spell>
                      {
                                    { SpellSlot.Q, new Spell(SpellSlot.Q, 800f) },
                                    { SpellSlot.W, new Spell(SpellSlot.W, 1000f) },
                                    { SpellSlot.E, new Spell(SpellSlot.E, 700f) },
                                    { SpellSlot.R, new Spell(SpellSlot.R, 640f) }
                      };
        }

        public List<IModule> GetModules()
        {
            return new List<IModule>()
            {
                
            };
        }

        public void OnTick()
        {

        }

        public void OnCombo()
        {
            var qTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range / 1.5f, TargetSelector.DamageType.Magical);

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) && qTarget.IsValidTarget())
            {
                var targetPrediction = LeagueSharp.Common.Prediction.GetPrediction(qTarget, 0.75f);

                if (ObjectManager.Player.HealthPercent >= 35)
                {
                    var enemyHeroesPositions = HeroManager.Enemies.Select(hero => hero.Position.To2D()).ToList();

                    var Groups = PositioningHelper.GetCombinations(enemyHeroesPositions);

                    foreach (var group in Groups)
                    {
                        if (group.Count >= 3)
                        {
                            var Circle = MEC.GetMec(group);

                            if (Circle.Center.To3D().CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range) >= 2 &&
                                Circle.Center.Distance(ObjectManager.Player) <= Variables.Spells[SpellSlot.Q].Range &&
                                Circle.Radius <= Variables.Spells[SpellSlot.Q].Width)
                            {
                                this.BallManager.ProcessCommand(new Command()
                                {
                                    SpellCommand = Commands.Q,
                                    Where = Circle.Center.To3D()
                                });
                                return;
                            }
                        }
                    }

                }

                this.BallManager.ProcessCommand(new Command()
                {
                    SpellCommand = Commands.Q,
                    Where = targetPrediction.UnitPosition
                });


            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                qTarget.IsValidTarget(Variables.Spells[SpellSlot.W].Range))
            {
                var ballLocation = this.BallManager.BallPosition;
                var minWEnemies = 2;

                if (ObjectManager.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range + 245f) >= 2)
                {
                    if (ballLocation.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) >= minWEnemies)
                    {
                        this.BallManager.ProcessCommand(new Command() { SpellCommand = Commands.W, });
                    }
                }
                else
                {
                    if (ballLocation.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) >= 1)
                    {
                        this.BallManager.ProcessCommand(new Command() { SpellCommand = Commands.W, });
                    }
                }
            }


            if (Variables.Spells[SpellSlot.R].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo))
            {
                if (ObjectManager.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range + 250f) > 1)
                {
                    var EnemyPositions = HeroManager.Enemies.Select(hero => hero.Position.To2D()).ToList();

                    var Combinations = PositioningHelper.GetCombinations(EnemyPositions);


                    foreach (var group in Combinations)
                    {
                        if (group.Count >= 2)
                        {
                            var Circle = MEC.GetMec(group);
                            if (Variables.Spells[SpellSlot.Q].IsReady() &&
                                Circle.Center.Distance(ObjectManager.Player) <= Variables.Spells[SpellSlot.Q].Range &&
                                Circle.Radius <= Variables.Spells[SpellSlot.R].Range &&
                                Circle.Center.To3D().CountEnemiesInRange(Variables.Spells[SpellSlot.R].Range) >= 2)
                            {
                                Variables.Spells[SpellSlot.Q].Cast(Circle.Center.To3D());

                                var arrivalDelay =
                                    (int)
                                        (BallManager.BallPosition.Distance(Circle.Center.To3D()) /
                                         Variables.Spells[SpellSlot.Q].Speed * 1000f +
                                         Variables.Spells[SpellSlot.Q].Delay * 1000f + Game.Ping / 2f + 100f);

                                LeagueSharp.Common.Utility.DelayAction.Add(
                                    arrivalDelay, () =>
                                    {
                                        //Extra check just for safety
                                        if (
                                            BallManager.BallPosition.CountEnemiesInRange(
                                                Variables.Spells[SpellSlot.R].Range) >= 2)
                                        {
                                            Variables.Spells[SpellSlot.R].Cast();
                                        }
                                    });
                            }
                        }
                    }
                }
                else
                {

                    var targetForQR = TargetSelector.GetTarget(
                        Variables.Spells[SpellSlot.Q].Range / 1.2f, TargetSelector.DamageType.Magical);

                    if (targetForQR.IsValidTarget())
                    {
                        var QWDamage = ObjectManager.Player.GetComboDamage(
                            targetForQR, new[] { SpellSlot.Q, SpellSlot.W });
                        var QRDamage = ObjectManager.Player.GetComboDamage(
                            targetForQR, new[] { SpellSlot.Q, SpellSlot.W, SpellSlot.R });

                        var healthCheck = targetForQR.Health + 10f < QRDamage && !(targetForQR.Health + 10f < QWDamage);

                        if (!healthCheck)
                        {
                            return;
                        }

                        var rPosition = targetForQR.ServerPosition.Extend(
                            ObjectManager.Player.ServerPosition, Variables.Spells[SpellSlot.R].Range / 2f);

                        if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Combo) &&
                            rPosition.Distance(ObjectManager.Player.ServerPosition) <=
                            Variables.Spells[SpellSlot.Q].Range)
                        {
                            Variables.Spells[SpellSlot.Q].Cast(rPosition);

                            var actionDelay =
                                (int)
                                    (BallManager.BallPosition.Distance(rPosition) / Variables.Spells[SpellSlot.Q].Speed *
                                     1000f + Variables.Spells[SpellSlot.Q].Delay * 1000f + Game.Ping / 2f + 100f);

                            LeagueSharp.Common.Utility.DelayAction.Add(
                                actionDelay, () =>
                                {
                                    if (
                                        BallManager.BallPosition.CountEnemiesInRange(
                                            Variables.Spells[SpellSlot.R].Range) >= 1)
                                    {
                                        Variables.Spells[SpellSlot.R].Cast();
                                    }
                                });
                        }
                    }
                }

            }

        }

        public void OnMixed()
        {
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.orianna.mixed.mana").Value)
            {
                return;
            }

            var qTarget = TargetSelector.GetTarget(Variables.Spells[SpellSlot.Q].Range / 1.5f, TargetSelector.DamageType.Magical);

            if (Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) && qTarget.IsValidTarget())
            {
                var targetPrediction = LeagueSharp.Common.Prediction.GetPrediction(qTarget, 0.75f);

                if (ObjectManager.Player.HealthPercent >= 35)
                {
                    var enemyHeroesPositions = HeroManager.Enemies.Select(hero => hero.Position.To2D()).ToList();

                    var Groups = PositioningHelper.GetCombinations(enemyHeroesPositions);

                    foreach (var group in Groups)
                    {
                        if (group.Count >= 3)
                        {
                            var Circle = MEC.GetMec(group);

                            if (Circle.Center.To3D().CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range) >= 2 &&
                                Circle.Center.Distance(ObjectManager.Player) <= Variables.Spells[SpellSlot.Q].Range &&
                                Circle.Radius <= Variables.Spells[SpellSlot.Q].Width)
                            {
                                this.BallManager.ProcessCommand(new Command()
                                {
                                    SpellCommand = Commands.Q,
                                    Where = Circle.Center.To3D()
                                });
                                return;
                            }
                        }
                    }

                }

                this.BallManager.ProcessCommand(new Command()
                {
                    SpellCommand = Commands.Q,
                    Where = targetPrediction.UnitPosition
                });


            }

            if (Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Harrass) &&
                qTarget.IsValidTarget(Variables.Spells[SpellSlot.W].Range))
            {
                var ballLocation = this.BallManager.BallPosition;
                var minWEnemies = 2;

                if (ObjectManager.Player.CountEnemiesInRange(Variables.Spells[SpellSlot.Q].Range + 245f) >= 2)
                {
                    if (ballLocation.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) >= minWEnemies)
                    {
                        this.BallManager.ProcessCommand(new Command() { SpellCommand = Commands.W, });
                    }
                }
                else
                {
                    if (ballLocation.CountEnemiesInRange(Variables.Spells[SpellSlot.W].Range) >= 1)
                    {
                        this.BallManager.ProcessCommand(new Command() { SpellCommand = Commands.W, });
                    }
                }
            }

        }

        public void OnLastHit()
        { }

        public void OnLaneclear()
        {
            //
            if (ObjectManager.Player.ManaPercent <
                Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.orianna.farm.mana").Value)
            {
                return;
            }

            if (!Variables.Spells[SpellSlot.Q].IsEnabledAndReady(ModesMenuExtensions.Mode.Farm) ||
                !Variables.Spells[SpellSlot.W].IsEnabledAndReady(ModesMenuExtensions.Mode.Farm))
            {
                return;
            }

            var farmMinions = MinionManager.GetMinions(Variables.Spells[SpellSlot.Q].Range, MinionTypes.All);
            var farmLocation = Variables.Spells[SpellSlot.W].GetCircularFarmLocation(farmMinions);
            if (farmLocation.MinionsHit >= Variables.AssemblyMenu.GetItemValue<Slider>("dzaio.champion.orianna.farm.w.min").Value)
            {
                BallManager.ProcessCommandList(new List<Command>()
                {
                    new Command()
                    {
                        SpellCommand = Commands.Q,
                        Where = farmLocation.Position.To3D()
                    },
                    new Command()
                    {
                        SpellCommand = Commands.W,
                    }
                });
            }
        }

        public static List<AIHeroClient> getEHits(Vector3 endPosition)
        {
            return HeroManager.Enemies
                .Where(enemy => enemy.IsValidTarget(Variables.Spells[SpellSlot.E].Range * 1.45f) && Variables.Spells[SpellSlot.E].WillHit(enemy, endPosition))
                .ToList();
        }
    }
}
