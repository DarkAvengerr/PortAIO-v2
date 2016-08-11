using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MLGSORAKA
{
    internal class Program
    {
        public const string ChampName = "Soraka";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R, Q2;
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static GameObject ThreshGameObject;

        public static void OnLoad()
        {
            if (Player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("420 SORAKA Loaded!", 1000);

            Q = new Spell(SpellSlot.Q, 900);
            Q2 = new Spell(SpellSlot.Q, 970);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);


            Q.SetSkillshot(0.5f, 280, Qdelay(), false, SkillshotType.SkillshotCircle);

            E.SetSkillshot(0.25f, 70f, 1750, false, SkillshotType.SkillshotCircle);


            Config = new Menu("MLGSoraka", "Soraka", true);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            var healing = Config.AddSubMenu(new Menu("Healing Manager", "Healing Manager"));
            var combo = Config.AddSubMenu(new Menu("Combo Settings", "Combo Settings"));
            var harass = Config.AddSubMenu(new Menu("Harass Settings", "Harass Settings"));
            var misc = Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            var drawing = Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));


                var laneclear = Config.AddSubMenu(new Menu("Laneclear Settings", "Laneclear Settings"));

                laneclear.AddItem(new MenuItem("laneq", "Use Q").SetValue(false));
                laneclear.AddItem(new MenuItem("killq", "Use Q on >= Amount of Minions").SetValue(new Slider(2, 10, 0)));
                laneclear.AddItem(new MenuItem("lanemana", "Mana Percentage").SetValue(new Slider(75, 100, 0)));
            

            //Advanced Settings //[E] Settings

            //[W] Settings


            foreach (var hero in HeroManager.Allies)
            {

                healing.SubMenu("[W Settings]")
                    .SubMenu("Whitelist")
                    .AddItem(new MenuItem("allywhitelist." + hero.ChampionName, hero.ChampionName).SetValue(true));

            }
            healing.SubMenu("[W Settings]").AddItem(new MenuItem("wonhp", "Use [W] on <= % HP ").SetValue(true));

            foreach (var hero in HeroManager.Allies)
            {
                healing.SubMenu("[W Settings]")
                    .AddItem(
                        new MenuItem("allyhp." + hero.ChampionName, hero.ChampionName + " Health %").SetValue(
                            new Slider(65, 100, 0)));

            }
            healing.SubMenu("[W Settings]").AddItem(new MenuItem("priority", "Heal Priority").SetValue(
            new StringList(new[] { "Most AD", "Most AP", "Lowest HP", "Closest" }, 3)));

            healing.SubMenu("[W Settings]")
                .AddItem(new MenuItem("playerhp", "Don't Use W if player HP % <= ").SetValue(new Slider(35, 100, 0)));

            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("interrupt", "Auto [E] on interruptable spells").SetValue(true));
            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("Einterrupt", "Auto [E] on immobile targets").SetValue(true));
            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("AutoE", "Auto [E] on CC'd targets").SetValue(true));
            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("antigap", "Auto [E] on gapclosers").SetValue(true));
            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("AutoEx", "Auto [E] if it hits X amount of enemies").SetValue(true));
            misc.SubMenu("[E Settings]")
                .AddItem(new MenuItem("Eslider", "Enemy Count").SetValue(new Slider(2, 5, 0)));


            harass.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            harass.AddItem(new MenuItem("HarassE", "Use E").SetValue(true));
            harass.AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 100, 0)));

            foreach (var hero in HeroManager.Allies)
            {
                healing.SubMenu("[R Settings]")
                    .SubMenu("Whitelist")
                    .AddItem(new MenuItem("allybr." + hero.ChampionName, hero.ChampionName).SetValue(true));
            }
            healing.SubMenu("[R Settings]").AddItem(new MenuItem("ronhp", "Use [R] on <= % HP ").SetValue(true));
            foreach (var hero in HeroManager.Allies)
            {
                healing.SubMenu("[R Settings]")
                    .AddItem(
                        new MenuItem("allyr." + hero.ChampionName, hero.ChampionName + " Health %").SetValue(
                            new Slider(25, 100, 0)));
            }

                misc.AddItem(new MenuItem("debugq", "Debug Q prediction").SetValue(false));
            


            //ITEMS
            //Use dat blue support item thing
            //uhm something something zhonyas

            //Combo Settings
            combo.AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            combo.AddItem(new MenuItem("combo.disable.aa", "Disable AutoAttacks in Combo").SetValue(false));

            var mikael = healing.AddSubMenu(new Menu("Mikael's Crucible", "Mikael's Crucible"));
            foreach (var hero in HeroManager.Allies)
                mikael.SubMenu("Whitelist")
                 .AddItem(new MenuItem("mikael." + hero.ChampionName, hero.ChampionName).SetValue(true));
            var mikaelz = mikael.AddSubMenu(new Menu("CC List", "CC List"));

            var mikaelz1 = mikael.AddSubMenu(new Menu("Special Debuffs", "Special Debuffs"));
            mikaelz1.AddItem(new MenuItem("exh", "Exhaust").SetValue(true));
            mikael.AddItem(new MenuItem("UseMik", "[Use Mikael's Crucible on CC/Debuffs]").SetValue(false));

            mikaelz.AddItem(new MenuItem("stuns", "Stuns").SetValue(true));
            mikaelz.AddItem(new MenuItem("charms", "Charms").SetValue(true));
            mikaelz.AddItem(new MenuItem("taunts", "Taunts").SetValue(true));
            mikaelz.AddItem(new MenuItem("fears", "Fears").SetValue(true));
            mikaelz.AddItem(new MenuItem("snares", "Snares").SetValue(true));
            mikaelz.AddItem(new MenuItem("slows", "Slows").SetValue(false));


            //DRAWING
            drawing.AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            drawing.SubMenu("Spell Drawings")
                .AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings")
                .AddItem(new MenuItem("Wdraw", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings")
                .AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.IndianRed)));
            drawing.SubMenu("Spell Drawings")
                .AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(7, 30, 0)));
            drawing.SubMenu("Misc Drawings")
               .AddItem(new MenuItem("drawhp", "Draw HP % above allies").SetValue(true));

            Config.AddItem(new MenuItem("PewPew", "            Prediction Settings"));

            Config.AddItem(new MenuItem("hitchanceQ", "[Q] Hitchance").SetValue(new StringList
                (new[]
                {
                    HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                    HitChance.VeryHigh.ToString()
                }, 3)));
            Config.AddItem(new MenuItem("hitchanceE", "[E] Hitchance").SetValue(new StringList
                (new[]
                {
                    HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(),
                    HitChance.VeryHigh.ToString()
                }, 3)));

            misc.AddItem(new MenuItem("skinhax", "Skin Manager").SetValue(true));
            misc.AddItem(
                new MenuItem("sorakaskin", "Skin Name").SetValue(
                    new StringList(new[]
                    {
                        "Classic Soraka", "Dryad Soraka", "Divine Soraka", "Celestine Soraka", "Reaper Soraka",
                        "Order of the Banana Soraka"
                    })));

            Config.AddToMainMenu();

            Game.OnUpdate += Mikaels;
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnUpdate += Mode_Switch;
            Drawing.OnDraw += OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            GameObject.OnCreate += AntiObject;
            Obj_AI_Base.OnSpellCast += InterrupterSc;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
        }

        private static float Qdelay()
        {
            var target = TargetSelector.GetTarget(Q.Range + 300, TargetSelector.DamageType.Magical);
            double qdelay = 1800;

            if (target.IsValidTarget(Q.Range) && Player.Distance(target.Position) >= 150 && Player.Distance(target) <= 500)
            {
                qdelay = 1000;
                qdelay += - 0.5*Player.Distance(target.Position);
                return (float) qdelay;
            }
            if (target.IsValidTarget(Q.Range) && Player.Distance(target) >= 500)
            {
                qdelay = 1000;
                qdelay += -0.7 * Player.Distance(target.Position);
                return (float)qdelay;
            }

            else return 1800;


        }
        private static void Mikaels(EventArgs args)
        {
            var mikael = LeagueSharp.Common.Data.ItemData.Mikaels_Crucible.GetItem();
            if (Config.Item("UseMik").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Allies)
                {
                    if (Config.Item("mikael." + hero.ChampionName).GetValue<bool>() &&
                        Player.Distance(hero) <= 750)
                    {
                        if (hero.HasBuffOfType(BuffType.Stun) && Config.Item("stuns").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.Charm) && Config.Item("charms").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.Fear) && Config.Item("fears").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.Snare) && Config.Item("snares").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.Taunt) && Config.Item("taunts").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.Slow) && Config.Item("slows").GetValue<bool>() ||
                            hero.HasBuffOfType(BuffType.CombatDehancer) && Config.Item("exh").GetValue<bool>())
                        {
                            mikael.Cast(hero);
                        }
                    }
                }
            }
        }

        private static void Mode_Switch(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    break;
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (((Obj_AI_Base)Orbwalker.GetTarget()).IsMinion) args.Process = false;
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Config.Item("combo.disable.aa").GetValue<bool>())
            {
                if (((AIHeroClient)Orbwalker.GetTarget()).IsEnemy) args.Process = false;
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady() && sender.IsValidTarget(E.Range) && Config.Item("interrupt").GetValue<bool>())
                E.Cast(sender);
        }


        private static void InterrupterSc(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Team != Player.Team)
            {
                if (args.SData.Name == "KatarinaR" || args.SData.Name == "AlZaharNetherGrasp" ||
                    args.SData.Name == "LucianR" ||
                    args.SData.Name == "CaitlynPiltoverPeacemaker" || args.SData.Name == "RivenMatyr" ||
                    args.SData.Name == "VarusQ" ||
                    args.SData.Name == "AbsoluteZero" || args.SData.Name == "Drain" ||
                    args.SData.Name == "InfiniteDuress" ||
                    args.SData.Name == "MissFortuneBulletTime" || args.SData.Name == "ThreshQ")
                {
                    if (E.IsReady() && Config.Item("Einterrupt").GetValue<bool>() &&
                        sender.Distance(Player.Position) <= E.Range)
                        E.Cast(sender);
                }
                if (ThreshGameObject.Position.CountEnemiesInRange(250) >= 1)
                    E.Cast(ThreshGameObject.Position);
            }

            //teleport arrival fuck up //Credits to Sebby. I 
            foreach (
                var Object in
                    ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.Distance(Player.ServerPosition) < E.Range
                                                                  && Obj.Team != Player.Team &&
                                                                  (Obj.HasBuff("teleport_target", true) ||
                                                                   Obj.HasBuff("Pantheon_GrandSkyfall_Jump", true))))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(2500, () => { E.Cast(Object.Position); });
            }
        }

        private static void AutoE()
        {
            foreach (var hero in HeroManager.Enemies.Where(e => e.IsEnemy && e.IsValidTarget(E.Range) && !e.IsDead))
            {
                var cc = hero.HasBuffOfType(BuffType.Snare) ||
                         hero.HasBuffOfType(BuffType.Suppression) || hero.HasBuffOfType(BuffType.Taunt) ||
                         hero.HasBuffOfType(BuffType.Stun) || hero.HasBuffOfType(BuffType.Charm) ||
                         hero.HasBuffOfType(BuffType.Fear);
                if (hero.IsValidTarget(E.Range) && E.IsReady() && cc && Config.Item("AutoE").GetValue<bool>())
                    E.Cast(hero);
            }
            foreach (var hero in HeroManager.Enemies.Where(e => e.IsEnemy && e.IsValidTarget(E.Range) && !e.IsDead))
            {
                if (Config.Item("AutoEx").GetValue<bool>() && E.IsReady())
                    E.CastIfWillHit(hero, Config.Item("Eslider").GetValue<Slider>().Value);
            }

        }

        private static void AntiObject(GameObject sender, EventArgs args)
        {

                if (sender.Name.Contains("Thresh_Base_Lantern") && Player.Distance(sender.Position) < E.Range &&
                    sender.IsEnemy)
                {
                    ThreshGameObject = sender;
                }

        }

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (E.IsReady() && gapcloser.Sender.IsValidTarget(E.Range) && Config.Item("antigap").GetValue<bool>())
                LeagueSharp.Common.Utility.DelayAction.Add(50, () => { E.Cast(gapcloser.Sender); });
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<Circle>().Active)
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range - 75,
                        Q.IsReady() ? Config.Item("Qdraw").GetValue<Circle>().Color : Color.Red,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            if (Config.Item("Wdraw").GetValue<Circle>().Active)
                if (W.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range - 50,
                        W.IsReady() ? Config.Item("Wdraw").GetValue<Circle>().Color : Color.Red,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);

            if (Config.Item("Edraw").GetValue<Circle>().Active)
                if (E.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range - 75,
                        E.IsReady() ? Config.Item("Edraw").GetValue<Circle>().Color : Color.Red,
                        Config.Item("CircleThickness").GetValue<Slider>().Value);

        
            foreach (var hero in HeroManager.Allies)
            {
                var pos = hero.HPBarPosition;
                if (!hero.IsDead && !hero.IsMe && Config.Item("drawhp").GetValue<bool>() && hero.HealthPercent >
                    Config.Item("allyhp." + hero.ChampionName).GetValue<Slider>().Value)
                    Drawing.DrawText(pos.X + 40, pos.Y - 25, Color.LawnGreen, hero.HealthPercent.ToString("#.#") + "% HP");
                if (!hero.IsDead && !hero.IsMe && Config.Item("drawhp").GetValue<bool>() && hero.HealthPercent <=
                    Config.Item("allyhp." + hero.ChampionName).GetValue<Slider>().Value)
                    Drawing.DrawText(pos.X + 40, pos.Y - 25, Color.Tomato, hero.HealthPercent.ToString("#.#") + "% HP");
              }




            if (Config.Item("debugq").GetValue<bool>())
            {
                var posz = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(posz.X + 80, posz.Y - 25, Color.MediumPurple, Qdelay().ToString() + " QDelay Value");
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                Render.Circle.DrawCircle(Q.GetPrediction(target).CastPosition, 280, Color.Aqua, 0);
                Render.Circle.DrawCircle(Q.GetPrediction(target).CastPosition, 100, Color.CadetBlue, 0);
            }


        }
        private static void Game_OnGameUpdate(EventArgs args)
        {

            if (E.IsReady())
                AutoE();

            //Healing
            foreach (var hero in HeroManager.Allies)
            {
                if (hero.Position.CountEnemiesInRange(1000) >= 1 &&
                    Config.Item("allyr." + hero.ChampionName).GetValue<Slider>().Value >= hero.HealthPercent
                    && Config.Item("allybr." + hero.ChampionName).GetValue<bool>() &&
                    Config.Item("ronhp").GetValue<bool>() && !hero.IsDead && R.IsReady() && !hero.IsRecalling())
                {
                    R.Cast();
                }
            }


            if (!GetHealTarget().IsDead && GetHealTarget().Distance(Player.Position) < W.Range &&
                GetHealTarget().HealthPercent <=
                Config.Item("allyhp." + GetHealTarget().ChampionName).GetValue<Slider>().Value &&
                Config.Item("wonhp").GetValue<bool>() &&
                Config.Item("allywhitelist." + GetHealTarget().ChampionName).GetValue<bool>() &&
                Player.HealthPercent >= Config.Item("playerhp").GetValue<Slider>().Value && !GetHealTarget().IsRecalling() &&
                !GetHealTarget().InFountain())
            {
                W.Cast(GetHealTarget());
            }


        }

        public static void HealingManager()
        {
            foreach (var hero in HeroManager.Allies)
            {
                if (hero.Position.CountEnemiesInRange(800) >= 1 &&
                    Config.Item("allyr." + hero.ChampionName).GetValue<Slider>().Value >= hero.HealthPercent
                    && Config.Item("allybr." + hero.ChampionName).GetValue<bool>() &&
                    Config.Item("ronhp").GetValue<bool>() && !hero.IsDead && R.IsReady())
                {
                    R.Cast(hero);
                }
            }


            if (!GetHealTarget().IsDead && GetHealTarget().Distance(Player.Position) < W.Range &&
                GetHealTarget().HealthPercent <=
                Config.Item("allyhp." + GetHealTarget().ChampionName).GetValue<Slider>().Value &&
                Config.Item("wonhp").GetValue<bool>() &&
                Config.Item("allywhitelist." + GetHealTarget().ChampionName).GetValue<bool>() &&
                Player.HealthPercent >= Config.Item("playerhp").GetValue<Slider>().Value &&
                !GetHealTarget().InFountain())
            {
                W.Cast(GetHealTarget());
            }
        }
        private static AIHeroClient GetHealTarget()
        {
            switch (Config.Item("priority").GetValue<StringList>().SelectedIndex)
            {
                case 0: // MostAD
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range, false) && !ally.IsDead && ally.HealthPercent <=
                    Config.Item("allyhp." + ally.ChampionName).GetValue<Slider>().Value && !ally.IsMe && Config.Item("allywhitelist." + ally.ChampionName).GetValue<bool>())
                            .OrderByDescending(dmg => dmg.TotalAttackDamage())
                            .First();
                case 1: // MostAP
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range, false) && !ally.IsDead && ally.HealthPercent <=
                    Config.Item("allyhp." + ally.ChampionName).GetValue<Slider>().Value && !ally.IsMe && Config.Item("allywhitelist." + ally.ChampionName).GetValue<bool>())
                            .OrderByDescending(ap => ap.TotalMagicalDamage())
                            .First();

                case 2: //LowestHP
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range, false) && !ally.IsDead && ally.HealthPercent <=
                    Config.Item("allyhp." + ally.ChampionName).GetValue<Slider>().Value && !ally.IsMe && Config.Item("allywhitelist." + ally.ChampionName).GetValue<bool>())
                            .OrderBy(health => health.HealthPercent)
                            .First();
                case 3: //Closest - ScienceARK please add
                    return
                        HeroManager.Allies.Where(ally => ally.IsValidTarget(W.Range, false) && !ally.IsDead && ally.HealthPercent <=
                    Config.Item("allyhp." + ally.ChampionName).GetValue<Slider>().Value && !ally.IsMe && Config.Item("allywhitelist." + ally.ChampionName).GetValue<bool>())
                            .OrderBy(a => a.Distance(Player.Position)).FirstOrDefault();

            }
            return null;
        }


        private static void Laneclear()
        {
            //Why the fuck would you want to laneclear with soraka?????? idk toplane/midlane kek
            var allMinionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width);
            var lanemana = Config.Item("lanemana").GetValue<Slider>().Value;
            var Qfarmpos = E.GetCircularFarmLocation(allMinionsQ, E.Width);

            if (Player.ManaPercent < lanemana || !Config.Item("laneq").GetValue<bool>())
                return;

            if (Qfarmpos.MinionsHit >= Config.Item("killq").GetValue<Slider>().Value && Config.Item("laneq").GetValue<bool>() &&
               Player.ManaPercent >= lanemana)
            {
                Q.Cast(Qfarmpos.Position, true);
            }



        }

        private static void Harass()
        {

            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady() && Config.Item("HarassQ").GetValue<bool>() &&
                Q.GetPrediction(target).Hitchance >= PredictionQ("hitchanceQ") && Player.ManaPercent >= harassmana)
                Q.Cast(Q.GetPrediction(target).CastPosition);

            if (E.IsReady() && Config.Item("HarassE").GetValue<bool>() &&
                E.GetPrediction(target).Hitchance >= PredictionE("hitchanceE") && Player.ManaPercent >= harassmana)
                E.Cast(target);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValid && target.IsInvulnerable)
                return;

            if (Q.IsReady() && Config.Item("UseQ").GetValue<bool>() &&
                Q.GetPrediction(target).Hitchance >= PredictionQ("hitchanceQ"))
                Q.Cast(Q.GetPrediction(target).CastPosition);

            if (E.IsReady() && Config.Item("UseE").GetValue<bool>() &&
                E.GetPrediction(target).Hitchance >= PredictionE("hitchanceE"))
                E.Cast(target);

        }

        private static HitChance PredictionQ(string name)
        {
            var qpred = Config.Item(name).GetValue<StringList>();
            switch (qpred.SList[qpred.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }

        private static HitChance PredictionE(string name)
        {
            var qpred = Config.Item(name).GetValue<StringList>();
            switch (qpred.SList[qpred.SelectedIndex])
            {
                case "Low":
                    return HitChance.Low;
                case "Medium":
                    return HitChance.Medium;
                case "High":
                    return HitChance.High;
                case "Very High":
                    return HitChance.VeryHigh;
            }
            return HitChance.VeryHigh;
        }
    }
}



