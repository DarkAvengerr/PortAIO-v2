using System;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using SharpDX;
using SharpDX.Direct3D9;
using UnderratedAIO.Helpers;
using Environment = UnderratedAIO.Helpers.Environment;


using EloBuddy; 
using LeagueSharp.Common; 
 namespace UnderratedAIO.Champions
{
    internal class TahmKench
    {
        public static Menu config;
        public static Orbwalking.Orbwalker orbwalker;
        public static AutoLeveler autoLeveler;
        public static Spell Q, W, WSkillShot, E, R;
        public static readonly AIHeroClient player = ObjectManager.Player;
        public Team lastWtarget = Team.Null;
        public static bool justWOut, justQ, blockW;
        public float lastE;

        public TahmKench()
        {
            InitTahmKench();
            InitMenu();
            //Chat.Print("<font color='#9933FF'>Soresu </font><font color='#FFFFFF'>- Tahm Kench</font>");
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Helpers.Jungle.setSmiteSlot();
            HpBarDamageIndicator.DamageToUnit = ComboDamage;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
        }

        private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (config.Item("useqgc", true).GetValue<bool>() && Q.IsReady() &&
                gapcloser.End.Distance(player.Position) < 200 && !gapcloser.Sender.ChampionName.ToLower().Contains("yi"))
            {
                Q.Cast(gapcloser.End);
            }
        }

        private void InitTahmKench()
        {
            Q = new Spell(SpellSlot.Q, 800);

            Q.SetSkillshot(0.5f, 80, 2000, true, SkillshotType.SkillshotLine);

            WSkillShot = new Spell(SpellSlot.W, 900);
            WSkillShot.SetSkillshot(0.5f, 80, 900, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 235);
            W.SetTargetted(0.5f, float.MaxValue);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 1700);
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (FpsBalancer.CheckCounter())
            {
                return;
            }
            orbwalker.SetMovement(true);
            blockW = false;
            if (config.Item("useDevour", true).GetValue<bool>() && W.IsReady() && !justQ)
            {
                EatAlly();
            }
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                default:
                    break;
            }
            if (config.Item("useShield", true).GetValue<bool>() && E.IsReady())
            {
                UseShield();
            }
        }

        private void UseShield()
        {
            var playerData = Program.IncDamages.GetAllyData(player.NetworkId);
            if (playerData == null)
            {
                return;
            }

            if (config.Item("ShieldUnderHealthP", true).GetValue<Slider>().Value > player.HealthPercent &&
                (playerData.DamageTaken > 50 && player.AttackShield >= playerData.DamageTaken) ||
                playerData.DamageTaken > player.Health)
            {
                E.Cast();
            }

            if (playerData.DamageTaken >
                player.Health * config.Item("ShieldDamage", true).GetValue<Slider>().Value / 100)
            {
                E.Cast();
            }
        }

        private void EatAlly()
        {
            var allies =
                HeroManager.Allies.Where(a => a.Distance(player) < W.Range && !a.IsMe)
                    .OrderByDescending(a => config.Item("Priority" + a.ChampionName, true).GetValue<Slider>().Value)
                    .ToArray();
            if (allies.Any())
            {
                for (int i = 0; i <= allies.Count() - 1; i++)
                {
                    var allyData = Program.IncDamages.GetAllyData(allies[i].NetworkId);
                    if (allyData == null || !config.Item("useEat" + allies[i].ChampionName, true).GetValue<bool>())
                    {
                        continue;
                    }
                    if (CheckCasting(allies[i]) && !allies[i].IsInvulnerable)
                    {
                        if (((config.Item("EatUnderHealthP" + allies[i].ChampionName, true).GetValue<Slider>().Value >
                              allies[i].HealthPercent && allyData.ProjectileDamageTaken > 50) ||
                             allyData.ProjectileDamageTaken > allies[i].Health) && !allies[i].Spellbook.IsCastingSpell)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }

                        if (allyData.ProjectileDamageTaken >
                            allies[i].Health *
                            config.Item("EatDamage" + allies[i].ChampionName, true).GetValue<Slider>().Value / 100)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                        if (config.Item("targetedCC" + allies[i].ChampionName, true).GetValue<bool>() && allyData.AnyCC)
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                        if (config.Item("ontargetedCC" + allies[i].ChampionName, true).GetValue<bool>() &&
                            allyData.ProjectileDamageTaken > 50 &&
                            (allies[i].HasBuffOfType(BuffType.Knockup) || allies[i].HasBuffOfType(BuffType.Fear) ||
                             allies[i].HasBuffOfType(BuffType.Flee) || allies[i].HasBuffOfType(BuffType.Stun) ||
                             allies[i].HasBuffOfType(BuffType.Snare)))
                        {
                            lastWtarget = Team.Ally;
                            W.CastOnUnit(allies[i], true);
                        }
                    }
                    if (i + 1 <= allies.Count() - 1 &&
                        config.Item("Priority" + allies[i + 1].ChampionName, true).GetValue<Slider>().Value <
                        config.Item("Priority" + allies[i].ChampionName, true).GetValue<Slider>().Value)
                    {
                        if (config.Item("allyPrior", true).GetValue<bool>())
                        {
                            blockW = true;
                        }
                        return;
                    }
                }
            }
        }

        private bool CheckCasting(AIHeroClient hero)
        {
            if (!config.Item("dontuseDevourcasting", true).GetValue<bool>())
            {
                return true;
            }
            if (hero.Spellbook.IsCastingSpell || hero.Spellbook.IsChanneling || hero.Spellbook.IsCharging)
            {
                return false;
            }
            return true;
        }

        private void Harass()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(900, TargetSelector.DamageType.Magical, true);
            float perc = config.Item("minmanaH", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc || target == null)
            {
                return;
            }
            if (config.Item("useqH", true).GetValue<bool>() && Q.CanCast(target) && !justWOut)
            {
                handeQ(target, HitChance.VeryHigh);
            } //usewminiH
            if (config.Item("usewminiH", true).GetValue<bool>())
            {
                HandleWHarass(target);
            }
            if (config.Item("usewH", true).GetValue<bool>())
            {
                handleWEnemyHero(target);
            }
        }

        private void handleWEnemyHero(AIHeroClient target)
        {
            if (target.GetBuffCount("TahmKenchPDebuffCounter") == 3 && !CombatHelper.CheckCriticalBuffs(target) &&
                !target.HasBuffOfType(BuffType.Stun) && !target.HasBuffOfType(BuffType.Snare) && !Q.CanCast(target) &&
                !justQ && !Program.IncDamages.GetEnemyData(target.NetworkId).IncSkillShot)
            {
                orbwalker.SetMovement(false);
                if (Orbwalking.CanMove(100) && Game.CursorPos.Distance(target.Position) < 300)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, target.Position.Extend(player.Position, 100));
                }

                lastWtarget = Team.Enemy;
                W.CastOnUnit(target);
            }
        }

        private void HandleWHarass(AIHeroClient target)
        {
            if (W.IsReady() && MinionInYou && WSkillShot.CanCast(target))
            {
                WSkillShot.CastIfHitchanceEquals(target, HitChance.High);
            }
            if (W.IsReady() && !SomebodyInYou && WSkillShot.CanCast(target) &&
                player.Distance(target) > config.Item("usewminiRange", true).GetValue<Slider>().Value)
            {
                var mini =
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .OrderBy(e => e.Health)
                        .FirstOrDefault();
                if (mini != null)
                {
                    lastWtarget = Team.Minion;
                    W.CastOnUnit(mini, true);
                }
            }
        }


        private static bool SomebodyInYou
        {
            get { return player.HasBuff("tahmkenchwhasdevouredtarget"); }
        }

        private bool MinionInYou
        {
            get { return SomebodyInYou && lastWtarget == Team.Minion; }
        }

        private bool EnemyInYou
        {
            get { return SomebodyInYou && lastWtarget == Team.Enemy; }
        }

        private bool AllyInYou
        {
            get { return SomebodyInYou && lastWtarget == Team.Ally; }
        }

        private void Clear()
        {
            float perc = config.Item("minmana", true).GetValue<Slider>().Value / 100f;
            if (player.Mana < player.MaxMana * perc)
            {
                return;
            }
            MinionManager.FarmLocation bestPosition =
                WSkillShot.GetLineFarmLocation(
                    MinionManager.GetMinions(
                        ObjectManager.Player.ServerPosition, WSkillShot.Range, MinionTypes.All, MinionTeam.NotAlly));

            if (W.IsReady() && !SomebodyInYou && config.Item("usewLC", true).GetValue<bool>() &&
                bestPosition.MinionsHit >= config.Item("wMinHit", true).GetValue<Slider>().Value && !justQ)
            {
                var mini =
                    MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .OrderBy(e => e.Health)
                        .FirstOrDefault();
                if (mini != null)
                {
                    lastWtarget = Team.Minion;
                    W.CastOnUnit(mini, true);
                }
            }
            if (W.IsReady() && config.Item("usewLC", true).GetValue<bool>() && MinionInYou)
            {
                WSkillShot.Cast(bestPosition.Position);
            }
        }

        private void Combo()
        {
            AIHeroClient target = DrawHelper.GetBetterTarget(900, TargetSelector.DamageType.Magical, true);
            if (target == null || target.IsInvulnerable || target.MagicImmune)
            {
                return;
            }
            if (config.Item("useItems").GetValue<bool>())
            {
                ItemHandler.UseItems(target, config, ComboDamage(target));
            }
            var ignitedmg = (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            bool hasIgnite = player.Spellbook.CanUseSpell(player.GetSpellSlot("SummonerDot")) == SpellState.Ready;
            if (config.Item("useIgnite", true).GetValue<bool>() &&
                ignitedmg > HealthPrediction.GetHealthPrediction(target, 700) && hasIgnite &&
                !CombatHelper.CheckCriticalBuffs(target))
            {
                player.Spellbook.CastSpell(player.GetSpellSlot("SummonerDot"), target);
            }
            if (Q.CanCast(target) && config.Item("useq", true).GetValue<bool>() && !justWOut)
            {
                handeQ(target, HitChance.High);
            }
            if (W.IsReady() && !SomebodyInYou && config.Item("usew", true).GetValue<bool>() && !blockW)
            {
                if (!config.Item("dontusewks", true).GetValue<bool>() ||
                    ((getWDamage(target) < HealthPrediction.GetHealthPrediction(target, 600) &&
                      player.CountAlliesInRange(1200) > 0) || player.CountAlliesInRange(1200) == 0))
                {
                    handleWEnemyHero(target);
                }
            }
            if (config.Item("usewmini", true).GetValue<bool>())
            {
                HandleWHarass(target);
            }
        }

        private double getWDamage(AIHeroClient target)
        {
            var r = R.Level - 1;
            var Rpercent = new float[] { 0.04f, 0.06f, 0.08f }[r >= 1 ? r : 1];
            var bonusDmg = 20 + ((player.MaxHealth - (515f + (95f * (player.Level - 1f)))) * Rpercent);
            var dmg = (new double[] { 0.20, 0.23, 0.26, 0.29, 0.32 }[W.Level - 1] +
                       0.02 * player.TotalMagicalDamage / 100) * target.MaxHealth;
            return Damage.CalcDamage(player, target, Damage.DamageType.Magical, dmg + bonusDmg);
        }

        private void handeQ(AIHeroClient target, HitChance hitChance)
        {
            if (player.Distance(target) <= Orbwalking.GetRealAutoAttackRange(target) && !Orbwalking.CanAttack() &&
                Orbwalking.CanMove(100) && target.GetBuffCount("TahmKenchPDebuffCounter") != 2)
            {
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, hitChance);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, hitChance);
                }
            }
            else if (player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
            {
                if (Program.IsSPrediction)
                {
                    Q.SPredictionCast(target, hitChance);
                }
                else
                {
                    Q.CastIfHitchanceEquals(target, hitChance);
                }
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            DrawHelper.DrawCircle(config.Item("drawqq", true).GetValue<Circle>(), Q.Range);
            DrawHelper.DrawCircle(config.Item("drawww", true).GetValue<Circle>(), W.Range);
            var r = R.Level - 1;
            DrawHelper.DrawCircle(
                config.Item("drawrr", true).GetValue<Circle>(), new int[] { 4000, 5000, 6000 }[r >= 1 ? r : 1]);
            HpBarDamageIndicator.Enabled = config.Item("drawcombo", true).GetValue<bool>();
        }

        private static float ComboDamage(AIHeroClient hero)
        {
            double damage = 0;
            if (Q.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.Q);
            }
            if (W.IsReady())
            {
                damage += Damage.GetSpellDamage(player, hero, SpellSlot.E);
            }
            //damage += ItemHandler.GetItemsDamage(target);
            var ignitedmg = player.GetSummonerSpellDamage(hero, Damage.SummonerSpell.Ignite);
            if (player.Spellbook.CanUseSpell(player.GetSpellSlot("summonerdot")) == SpellState.Ready &&
                hero.Health < damage + ignitedmg)
            {
                damage += ignitedmg;
            }
            return (float) damage;
        }


        private void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //tahmkenchw
            //tahmkenchwcasttimeandanimation
            if (sender.IsMe)
            {
                if (args.SData.Name == "tahmkenchwcasttimeandanimation")
                {
                    justWOut = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => { justWOut = false; });
                }
                if (args.SData.Name == "TahmKenchQ")
                {
                    var dist = player.Distance(args.End);
                    justQ = true;
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => justQ = false);
                }
                if (args.Slot == SpellSlot.W)
                {
                    if (args.Target != null)
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("Tahm ate :" + args.Target.Name);
                        Console.WriteLine("----");
                    }
                    else
                    {
                        Console.WriteLine("----");
                        Console.WriteLine("Tahm ate : null");
                        Console.WriteLine("----");
                    }
                }
            }
        }

        private void InitMenu()
        {
            config = new Menu("TahmKench ", "TahmKench", true);
            // Target Selector
            Menu menuTS = new Menu("Selector", "tselect");
            TargetSelector.AddToMenu(menuTS);
            config.AddSubMenu(menuTS);
            // Orbwalker
            Menu menuOrb = new Menu("Orbwalker", "orbwalker");
            orbwalker = new Orbwalking.Orbwalker(menuOrb);
            config.AddSubMenu(menuOrb);
            // Draw settings
            Menu menuD = new Menu("Drawings ", "dsettings");
            menuD.AddItem(new MenuItem("drawqq", "Draw Q range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawww", "Draw W range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawrr", "Draw R range", true))
                .SetValue(new Circle(false, Color.FromArgb(180, 100, 146, 166)));
            menuD.AddItem(new MenuItem("drawcombo", "Draw combo damage", true)).SetValue(true);
            config.AddSubMenu(menuD);
            // Combo Settings 
            Menu menuC = new Menu("Combo ", "csettings");
            menuC.AddItem(new MenuItem("useq", "Use Q", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usew", "Use W on target", true)).SetValue(true);
            menuC.AddItem(new MenuItem("dontusewks", "   Block KS", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewmini", "Use W on minion", true)).SetValue(true);
            menuC.AddItem(new MenuItem("usewminiRange", "   Min range", true))
                .SetValue(new Slider(300, 0, (int) WSkillShot.Range));
            menuC.AddItem(new MenuItem("useIgnite", "Use Ignite", true)).SetValue(true);
            menuC = ItemHandler.addItemOptons(menuC);
            config.AddSubMenu(menuC);
            // Harass Settings
            Menu menuH = new Menu("Harass ", "Hsettings");
            menuH.AddItem(new MenuItem("useqH", "Use Q", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewH", "Use W on target", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewminiH", "Use W on minion", true)).SetValue(true);
            menuH.AddItem(new MenuItem("usewminiHInf", "   Min range(Combo)"));
            menuH.AddItem(new MenuItem("minmanaH", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuH);
            // LaneClear Settings
            Menu menuLC = new Menu("LaneClear ", "Lcsettings");
            menuLC.AddItem(new MenuItem("useqLC", "Use Q", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("usewLC", "Use w", true)).SetValue(true);
            menuLC.AddItem(new MenuItem("wMinHit", "   Min hit", true)).SetValue(new Slider(3, 1, 6));
            menuLC.AddItem(new MenuItem("minmana", "Keep X% mana", true)).SetValue(new Slider(1, 1, 100));
            config.AddSubMenu(menuLC);

            Menu menuM = new Menu("Misc ", "Msettings");
            menuM.AddItem(new MenuItem("useqgc", "Use Q gapclosers", true)).SetValue(false);

            Menu Shield = new Menu("Shield(E)", "Shieldsettings");
            Shield.AddItem(new MenuItem("ShieldUnderHealthP", "Shield Under X% health", true))
                .SetValue(new Slider(20, 0, 100));
            Shield.AddItem(new MenuItem("ShieldDamage", "Damage in %health", true)).SetValue(new Slider(40, 0, 100));
            Shield.AddItem(new MenuItem("useShield", "Enabled", true)).SetValue(true);
            menuM.AddSubMenu(Shield);
            Menu AllyDef = new Menu("Devour(W) on ally", "Devoursettings");
            foreach (var ally in HeroManager.Allies.Where(a => !a.IsMe))
            {
                Menu allyMenu = new Menu(ally.ChampionName, ally.ChampionName + "settings");
                allyMenu.AddItem(new MenuItem("EatUnderHealthP" + ally.ChampionName, "Eat X% health", true))
                    .SetValue(new Slider(20, 0, 100));
                allyMenu.AddItem(new MenuItem("EatDamage" + ally.ChampionName, "Eat at Damage in %health", true))
                    .SetValue(new Slider(40, 0, 100));
                allyMenu.AddItem(new MenuItem("targetedCC" + ally.ChampionName, "Eat brefore Targeted CC", true))
                    .SetValue(true);
                allyMenu.AddItem(new MenuItem("ontargetedCC" + ally.ChampionName, "Eat on CC", true)).SetValue(true);
                allyMenu.AddItem(new MenuItem("Priority" + ally.ChampionName, "Priority", true))
                    .SetValue(new Slider(Environment.Hero.GetPriority(ally.ChampionName), 1, 5));
                allyMenu.AddItem(new MenuItem("useEat" + ally.ChampionName, "Enabled", true)).SetValue(true);
                AllyDef.AddSubMenu(allyMenu);
            }
            AllyDef.AddItem(new MenuItem("dontuseDevourcasting", "Don't interrupt ally", true)).SetValue(true);
            AllyDef.AddItem(new MenuItem("allyPrior", "Prioritize ally over damage enemy", true)).SetValue(true);
            AllyDef.AddItem(new MenuItem("useDevour", "Enabled", true)).SetValue(true);
            menuM.AddSubMenu(AllyDef);
            menuM = DrawHelper.AddMisc(menuM);
            config.AddSubMenu(menuM);

            
            config.AddItem(new MenuItem("UnderratedAIO", "by Soresu v" + Program.version.ToString().Replace(",", ".")));
            config.AddToMainMenu();
        }
    }

    public enum Team
    {
        Null,
        Ally,
        Enemy,
        Minion
    }
}