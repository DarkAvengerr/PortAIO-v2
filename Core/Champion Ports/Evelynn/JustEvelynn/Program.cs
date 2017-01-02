using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using JustEvelynn;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JustEvelynn
{
    internal class Program
    {
        public const string ChampName = "Evelynn";
        public const string Menuname = "JustEvelynn";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("JustEvelynn Loaded - [V.1.0.0.0]", 8000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 500f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 275f);
            R = new Spell(SpellSlot.R, 650f);
            R.SetSkillshot(0.25f, 350f, 1300f, false, SkillshotType.SkillshotCircle);


            abilitySequence = new int[] {1, 3, 1, 2, 1, 4, 1, 2, 1, 2, 4, 2, 3, 2, 3, 4, 3, 3};

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Rene", "Min Enemies for R").SetValue(new Slider(2, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("AutoR", "Auto R").SetValue(false));
            Config.SubMenu("Combo")
                .AddItem(new MenuItem("Renem", "Min Enemies for Auto R").SetValue(new Slider(3, 1, 5)));

            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("hE", "Use E").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("harassmana", "Harass Mana Percentage").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Harass")
                .AddItem(
                    new MenuItem("AutoHarass", "Auto Harass", true).SetValue(new KeyBind("J".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Config.SubMenu("Harass").AddItem(new MenuItem("aQ", "Use Q for Auto Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("aW", "Use W for Auto Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("aharassmana", "Auto Harass Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Item
            Config.AddSubMenu(new Menu("Item", "Item"));
            Config.SubMenu("Item").AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            Config.SubMenu("Item")
                .AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));

            //Farm
            Config.AddSubMenu(new Menu("Farm", "Farm"));
            Config.SubMenu("Farm")
                .AddItem(
                    new MenuItem("AutoFarm", "Auto Farm", true).SetValue(new KeyBind("X".ToCharArray()[0],
                        KeyBindType.Toggle)));
            Config.SubMenu("Farm").AddItem(new MenuItem("fQ", "Use Q for Auto Farm").SetValue(true));
            Config.SubMenu("Farm").AddItem(new MenuItem("fE", "Use E for Auto Farm").SetValue(true));
            Config.SubMenu("Farm")
                .AddItem(new MenuItem("farmmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));


            //Laneclear
            Config.AddSubMenu(new Menu("Clear", "Clear"));
            Config.SubMenu("Clear").AddItem(new MenuItem("lQ", "Use Q").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("lE", "Use E").SetValue(true));
            Config.SubMenu("Clear")
                .AddItem(new MenuItem("lanemana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Draw
            Config.AddSubMenu(new Menu("Draw", "Draw"));
            Config.SubMenu("Draw").AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));
            
            //Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksQ", "Killsteal with Q").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksE", "Killsteal with E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksR", "Killsteal with R").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("antigap", "AntiGapCloser with W").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("autolevel", "Auto Level Spells").SetValue(false));
            var dmg = new MenuItem("combodamage", "Damage Indicator").SetValue(true);
            var drawFill = new MenuItem("color", "Fill colour", true).SetValue(new Circle(true, Color.DarkRed));
            Config.SubMenu("Draw").AddItem(drawFill);
            Config.SubMenu("Draw").AddItem(dmg);

            //DrawDamage.DamageToUnit = GetComboDamage;
            //DrawDamage.Enabled = dmg.GetValue<bool>();
            //DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            //DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;

            dmg.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                //DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                //DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddToMainMenu();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Chat.Print(
                "<font color=\"#ba00b3\">JustEvelynn - <font color=\"#FFFFFF\"> Latest Version Successfully Loaded.</font>");
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && gapcloser.Sender.IsValidTarget(player.AttackRange) &&
                Config.Item("antigap").GetValue<bool>())
                W.Cast();
        }

        private static void AutoR()
        {
            if (!R.IsReady() || !Config.Item("AutoR").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);


            var enemyss = target.CountEnemiesInRange(R.Range);
            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && target.IsValidTarget(R.Range))
                if (Config.Item("Rene").GetValue<Slider>().Value <= enemyss)
                {
                    var pred = R.GetPrediction(target).Hitchance;
                    if (pred >= HitChance.High)
                        R.CastIfWillHit(target, enemyss);
                }

        }

        private static void Lasthit2()
        {
            var lastmana = Config.Item("farmmana").GetValue<Slider>().Value;
            var useq = Config.Item("fQ").GetValue<bool>();
            var usee = Config.Item("fE").GetValue<bool>();
            var minionCount = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            {
                foreach (var minion in minionCount)
                {
                    if (useq && Q.IsReady()
                        && minion.IsValidTarget(Q.Range)
                        && player.ManaPercent >= lastmana
                        && minion.Health < Q.GetDamage(minion))
                    {
                        Q.Cast();
                    }

                    if (usee && E.IsReady()
                        && minion.IsValidTarget(E.Range)
                        && player.ManaPercent >= lastmana
                        && minion.Health < E.GetDamage(minion))
                    {
                        E.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void combo()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            var enemys = target.CountEnemiesInRange(650);
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("UseE").GetValue<bool>())
                E.CastOnUnit(target);

            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && target.IsValidTarget(R.Range))
                if (Config.Item("Rene").GetValue<Slider>().Value <= enemys)
                {
                    R.CastIfHitchanceEquals(target, HitChance.VeryHigh);
                }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();
        }

        private static float GetComboDamage(AIHeroClient enemy)
        {
            double damage = 0d;

            if (Q.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);

            if (E.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += player.GetSpellDamage(enemy, SpellSlot.R);

            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            return (float) damage;
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float) player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Killsteal()
        {
            if (Config.Item("ksQ").GetValue<bool>() && Q.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(Q.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.Q));
                if (target.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }

            if (Config.Item("ksE").GetValue<bool>() && E.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(E.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.E));
                if (target.IsValidTarget(Q.Range))
                {
                    E.CastOnUnit(target);
                }
            }

            if (Config.Item("ksR").GetValue<bool>() && R.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(R.Range) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.R));
                if (target.IsValidTarget(R.Range))
                {
                    R.CastIfHitchanceEquals(target, HitChance.High);
                }
            }
        }

        // Thanks to jackisback & xQx for
        private static void items()
        {
            Ignite = player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;

            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("eL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(player) && target.IsValidTarget(Q.Range)
                && Config.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

            if (player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            if (Config.Item("autolevel").GetValue<bool>()) LevelUpSpells();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            Killsteal();
            AutoR();
            var autoHarass = Config.Item("AutoHarass", true).GetValue<KeyBind>().Active;
            if (autoHarass)
                AutoHarass();
            var farm = Config.Item("AutoFarm", true).GetValue<KeyBind>().Active;
            if (farm)
                Lasthit2();
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var automana = Config.Item("aharassmana").GetValue<Slider>().Value;
            if (target == null || !target.IsValidTarget())
                return;

            if (Q.IsReady() && Config.Item("aQ").GetValue<bool>() && target.IsValidTarget(Q.Range) && player.ManaPercent <= automana)
                Q.Cast();
        }


        //Thanks to LuNi
        private static void LevelUpSpells()
        {
            int qL = player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = new int[] {0, 0, 0, 0};
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }

        private static void harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            if (target == null || !target.IsValidTarget())
                return;

            if (E.IsReady() && player.ManaPercent >= harassmana &&
                Config.Item("hE").GetValue<bool>())
                E.CastOnUnit(target);

            if (Config.Item("hQ").GetValue<bool>() && target.IsValidTarget(Q.Range) &&
                player.ManaPercent >= harassmana)
                Q.Cast();

        }

        private static void Clear()
        {
            var farmmana = Config.Item("lanemana").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if (Config.Item("lQ").GetValue<bool>()
                        && Q.IsReady()
                        && player.ManaPercent >= farmmana
                        && minion.IsValidTarget(Q.Range))
                    {
                        Q.Cast();
                    }

                    if (Config.Item("lE").GetValue<bool>()
                        && E.IsReady()
                        && player.ManaPercent >= farmmana
                        && minion.IsValidTarget(E.Range)
                        )
                    {
                        E.CastOnUnit(minion);
                    }

                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, Q.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Edraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, E.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Rdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, R.Range, System.Drawing.Color.White, 3);


        }
    }
}
