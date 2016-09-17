using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CheerleaderLux
{
    // Todo List: - Combo is le finished
    // - Misc (anti-gap, anti-rengar, anti-khazix etc) - low prio

    public class Lux : Extensions.Statics
    {
        public const string ChampName = "Lux";
        public static void OnLoad(EventArgs args)
        {
            if (player.ChampionName != ChampName)
                return;

            Printmsg("Sucessfully loaded - Patch: " + Game.Version);
            Printmsg("Assembly Version: 1.0.0.3 - Beta");
            Console.WriteLine("Cheerloader Lux loaded.");

            Q1 = new Spell(SpellSlot.Q, 1150);
            W1 = new Spell(SpellSlot.W, 1075);
            E1 = new Spell(SpellSlot.E, 1300);
            R1 = new Spell(SpellSlot.R, 3400);

            Q1.SetSkillshot(0.25f, 85f, 1150f, false, SkillshotType.SkillshotLine);
            W1.SetSkillshot(0.25f, 110f, 1200f, false, SkillshotType.SkillshotLine);
            E1.SetSkillshot(0.25f, 270f, 800f, false, SkillshotType.SkillshotCircle);
            R1.SetSkillshot(1.1f, 190f, float.MaxValue, false, SkillshotType.SkillshotLine);


            #region Menu

            Config = new Menu("Cheerleader Lux", "Lux", true);
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings")));
            var combo = Config.AddSubMenu(new Menu("Combat Settings", "Combat Settings"));
            var hotkey = Config.AddSubMenu(new Menu("Hotkeys", "Hotkeys"));
            var prediction = Config.AddSubMenu(new Menu("Prediction Settings", "Prediction Settings"));
            var farm = Config.AddSubMenu(new Menu("Farm Settings", "Farm Settings"));
            var lane = farm.AddSubMenu(new Menu("Laneclear Settings", "Laneclear Farm Settings"));
            var jungle = farm.AddSubMenu(new Menu("Jungleclear Settings", "Jungleclear Settings"));
            var misc = Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            var drawing = Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));

            Config.AddItem(new MenuItem("optimize", "Optimize Assembly").SetValue(false));


            Config.AddItem(new MenuItem("blank1", "                          "));


            Config.AddItem(new MenuItem("blank3", "Made by Eversong").SetFontStyle(System.Drawing.FontStyle.Bold));

            //Combo Menu
            var Rsettings = combo.AddSubMenu(new Menu("Advanced [R] Settings", "advR").SetFontStyle(System.Drawing.FontStyle.Bold));
            Rsettings.AddItem(new MenuItem("advanced.R.aoe", "Use [R] on X amount of Enemies").SetValue(true));
            Rsettings.AddItem(new MenuItem("advanced.R.aoe.count", "Enemy Count").SetValue(new Slider(3, 5, 1)));
            Rsettings.AddItem(new MenuItem("combo.r.slider", "Min. [R] Distance").SetValue(new Slider(450, 1000, 0)));
            combo.AddItem(new MenuItem("combo.Q", "Use Q").SetValue(true));
            combo.AddItem(new MenuItem("combo.W", "Use W").SetValue(true));
            combo.AddItem(new MenuItem("combo.E", "Use E").SetValue(true));
            combo.AddItem(new MenuItem("combo.R", "Use R").SetValue(true));

            //Hotkeys
            hotkey.AddItem(new MenuItem("semi-ult", "Semi-manual [R] - [Cast ult on your target with Prediction] ").SetValue(new KeyBind('H', KeyBindType.Press)));
            hotkey.AddItem(new MenuItem("semi-w", "Semi-manual [W] - [Shield closest ally to mouse cursor] ").SetValue(new KeyBind('W', KeyBindType.Press)));

            //Harras Menu
            var Harass = combo.AddSubMenu(new Menu("Harass Settings", "harras").SetFontStyle(System.Drawing.FontStyle.Bold));
            Harass.AddItem(new MenuItem("harass.support", "Disable Autoattack").SetValue(false));
            Harass.AddItem(new MenuItem("harass.mana.slider", "Min. Mana Percentage").SetValue(new Slider(65, 100, 0)));
            Harass.AddItem(new MenuItem("harass.Q", "Use [Q] in Harass").SetValue(true));
            Harass.AddItem(new MenuItem("harass.E", "Use [E] in Harass").SetValue(true));
            Harass.AddItem(new MenuItem("autoharass", "AutoHarass Toggle").SetValue(new KeyBind('L', KeyBindType.Toggle)));

            //AutoSpells
            var autospells = combo.AddSubMenu(new Menu("Auto Spell Settings", "ASS").SetFontStyle(System.Drawing.FontStyle.Bold));
            autospells.AddItem(new MenuItem("autospells.R", "Auto [R] on Killable Enemies").SetValue(true));
            autospells.AddItem(new MenuItem("autospells.ignite", "Auto Ignite on Killable Enemies").SetValue(true));
            autospells.AddItem(new MenuItem("autospells.Q.cc", "Auto [Q] on CC'd enemies").SetValue(true));
            autospells.AddItem(new MenuItem("autospells.Q.collision", "Auto [Q] if it can hit 2 enemies").SetValue(true));

            //Drawings
            var dmgdraw = drawing.AddSubMenu(new Menu("Damage Indicator Settings", "DMGI"));
            dmgdraw.AddItem(new MenuItem("disable.dmg", "Fully Disable Damage Indicator").SetValue(false));
            dmgdraw.AddItem(new MenuItem("drawing.dmg", "[Damage Indicator]:").SetValue(new StringList(new[] { "Common", "Custom" })));
            dmgdraw.AddItem(new MenuItem("drawing.dmg.color", "Color").SetValue(new Circle(true, System.Drawing.Color.Orange)));
            dmgdraw.AddItem(new MenuItem("draw.R.dmg", "Draw Current [R] Damage on Enemies").SetValue(false));

            var miscdraw = drawing.AddSubMenu(new Menu("Misc Drawings", "MiscD"));
            miscdraw.AddItem(new MenuItem("drawing.minimap", "Draw Lux [R] Minimap Range").SetValue(true));
            miscdraw.AddItem(new MenuItem("draw.LuxE.position", "Draw Lux [E] Position").SetValue(true));
            miscdraw.AddItem(new MenuItem("draw.harass.indicator", "Draw AutoHarass Indicator").SetValue(true));
            miscdraw.AddItem(new MenuItem("drawing.indicator", "Draw Enemy Indicator").SetValue(false));
            miscdraw.AddItem(new MenuItem("print.debug.chat", "Print Debug Messages").SetValue(false));

            drawing.AddItem(new MenuItem("disable.draws", "Disable all Drawings").SetValue(false));
            drawing.AddItem(new MenuItem("draw.Q", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.Gray)));
            drawing.AddItem(new MenuItem("draw.W", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.DodgerBlue)));
            drawing.AddItem(new MenuItem("draw.E", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.LightBlue)));
            drawing.AddItem(new MenuItem("draw.R", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.CornflowerBlue)));

            //laneclear
            lane.AddItem(new MenuItem("laneclear.level", "Don't use abilities till level").SetValue(new Slider(8, 18, 1)));
            lane.AddItem(new MenuItem("laneclear.mana.slider", "Player Mana Percentage").SetValue(new Slider(75, 100, 0)));
            lane.AddItem(new MenuItem("laneclear.Q", "Use Q").SetValue(true));
            lane.AddItem(new MenuItem("laneclear.Q.count", "[Q] Minions Hit").SetValue(new Slider(2, 2, 1)));
            lane.AddItem(new MenuItem("laneclear.E", "Use E").SetValue(true));
            lane.AddItem(new MenuItem("laneclear.E.count", "[E] Minions Hit").SetValue(new Slider(3, 10, 1)));

            //jungle
            jungle.AddItem(new MenuItem("jungleclear.mana.slider", "Player Mana Percentage").SetValue(new Slider(25, 100, 0)));
            jungle.AddItem(new MenuItem("jungleclear.Q", "Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclear.E", "Use E").SetValue(true));

            //Misc
            misc.AddItem(new MenuItem("antigap.Q", "Anti-Gapcloser [Q]").SetValue(true));
            misc.AddItem(new MenuItem("anti.rengar", "Anti-Rengar [Q]").SetValue(true));
            misc.AddItem(new MenuItem("anti.khazix", "Anti-Khazix [Q]").SetValue(true));

            //Prediction

            prediction.AddItem(new MenuItem("prediction.draw", "Draw Prediction Line").SetValue(false));
            prediction.AddItem(new MenuItem("prediction.Q", "[Q] Hitchance").SetValue(
                    new StringList(new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 3)));
            prediction.AddItem(new MenuItem("prediction.E", "[E] Hitchance").SetValue(
                    new StringList(new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 3)));
            prediction.AddItem(new MenuItem("prediction.R", "[R] Hitchance").SetValue(
                    new StringList(new[] { HitChance.Low.ToString(), HitChance.Medium.ToString(), HitChance.High.ToString(), HitChance.VeryHigh.ToString() }, 3)));
            prediction.AddItem(new MenuItem("ayy", "LeagueSharp.Common Prediction"));


            Config.AddToMainMenu();

            #endregion Menu

            Game.OnUpdate += OrbwalkerModes;
            Game.OnUpdate += AutoSpells;
            Obj_AI_Base.OnProcessSpellCast += Wlogic;
            AntiGapcloser.OnEnemyGapcloser += Antigapcloser;
            Orbwalking.BeforeAttack += SupportMode;
            GameObject.OnCreate += EisGone;
            GameObject.OnCreate += EisAlive;
            Obj_AI_Base.OnProcessSpellCast += Tickcount;
            Drawing.OnDraw += Drawings;
            Extensions.Drawmethods.DrawEvent();


        }

        private static void Antigapcloser(ActiveGapcloser gapcloser)
        {
            if (player.IsDead || gapcloser.Sender.IsInvulnerable)
                return;

            var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
            if (gapcloser.Sender.IsValidTarget(Q1.Range) && Config.Item("antigap.Q").GetValue<bool>())
            {
                Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius,
                    System.Drawing.Color.DeepPink);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, System.Drawing.Color.DodgerBlue, "GAPCLOSER DETECTED!");
            }

            if (Q1.IsReady() && gapcloser.Sender.IsValidTarget(Q1.Range) &&
                Config.Item("antigap.Q").GetValue<bool>())
            {
                SpellCast(gapcloser.Sender, Q1.Range, Q1, true, 1, false, PredQ("prediction.Q"));
                SpellCast(gapcloser.Sender, Q1.Range, Q1, true, 1, false, HitChance.Dashing);
            }
    }

        private static void Wlogic(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe || sender == null || !sender.IsEnemy || sender.IsMinion || W1.Level < 1 || !sender.IsValid || !Config.Item("combo.W").GetValue<bool>() || Player.Spellbook.IsAutoAttacking) return;
                var lowestally = HeroManager.Allies.Where(a => !a.IsDead && a.IsValidTarget(W1.Range) && a.IsValid && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

                //Will Prioritize W cast on Allies
                if (args.Target.IsMe && W1.IsReady() && lowestally == null && sender.IsEnemy)
                    W1.Cast(Game.CursorPos);

                if (lowestally.Distance(Player.Position) > W1.Range) return;

                if (args.Target.IsMe && lowestally != null && W1.IsReady() && sender.IsEnemy)
                    W1.Cast(lowestally);

                //Ally Receiving Damage
                if (args.Target.Position.Distance(player.Position) <= W1.Range && args.Target.IsAlly && sender.IsEnemy && lowestally != null)
                    W1.Cast(lowestally);
            }
            catch (Exception)
            {
            }
        }

        private static void SupportMode(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Config.Item("harass.support").GetValue<bool>())
            {
                if (((Obj_AI_Base)Orbwalker.GetTarget()).IsMinion) args.Process = false;
            }
            if (E1.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

        }

        private static void AutoSpells(EventArgs args)
        {
            //E
            Edetonation();
            //R
            Actives();
            if (Config.Item("semi-ult").GetValue<KeyBind>().Active)
            {
                var target = TargetSelector.GetTarget(R1.Range, TargetSelector.DamageType.Magical);
                if (target == null || !target.IsValid) return;
                if (R1.IsReady())
                {
                    SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
                }
            }
            if (Config.Item("semi-w").GetValue<KeyBind>().Active)
            {
                var ally = HeroManager.Allies.Where(a => a.IsValid && !a.IsMinion && !a.IsMe).OrderBy(a => a.Distance(Game.CursorPos)).FirstOrDefault();
                if (W1.IsReady())
                {
                    W1.Cast(ally);
                }
            }

            if (Q1.IsReady() && Config.Item("autospells.Q.cc").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Magical);
                if (target == null || !target.IsValid) return;
                var cc = target.HasBuffOfType(BuffType.Snare)
                        || target.HasBuffOfType(BuffType.Suppression)
                        || target.HasBuffOfType(BuffType.Taunt)
                        || target.HasBuffOfType(BuffType.Stun)
                        || target.HasBuffOfType(BuffType.Charm)
                        || target.HasBuffOfType(BuffType.Fear);
            }

            if (Q1.IsReady() && Config.Item("autospells.Q.collision").GetValue<bool>())
            {
                var enemies = HeroManager.Enemies.Where(e => e.IsValidTarget(Q1.Range));
                foreach (var enemy in enemies)
                {
                    if (enemy == null) return;
                    var collisionobjects = Q1.GetPrediction(enemy).CollisionObjects.Where(c => c.IsEnemy && !c.IsMinion);
                    if (collisionobjects.Count() == 1)
                    {
                        Q1.Cast(enemy);
                    }
                }
            }
        }

        private static void Tickcount(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //LuxLightStrikeKugel -- E Cast
            //LuxLightstrikeToggle -- E Toggle
            //LuxLightBinding -- Q
            //LuxPrismaticWave -- W
            //LuxMaliceCannon -- R

            var spellname = args.SData.Name;

            if (spellname.Contains("LuxLightBinding"))
            {
                Q1.LastCastAttemptT = Environment.TickCount;
                Printchat("[Q] Casted Tickcount: " + Q1.LastCastAttemptT + "Speed: " + args.SData.LineWidth);
            }

            if (spellname.Contains("LuxLightStrikeKugel"))
            {
                W1.LastCastAttemptT = Environment.TickCount;
                Printchat("[E] Casted Tickcount: " + W1.LastCastAttemptT + "Speed: " + args.SData.MissileSpeed);
            }

            if (spellname.Contains("LuxLightstrikeToggle"))
            {
                E1.LastCastAttemptT = Environment.TickCount;
                Printchat("[E-Toggle] Casted Tickcount: " + E1.LastCastAttemptT + "Speed: " + args.SData.MissileSpeed);
            }
            if (spellname.Contains("LuxMaliceCannon"))
            {
                R1.LastCastAttemptT = Environment.TickCount;
                Printchat("[R] Casted Tickcount: " + R1.LastCastAttemptT + "Speed: " + args.SData.MissileSpeed);
            }

            //if (spellname.Contains("LuxMaliceCannon"))

        }

        private static void Drawings(EventArgs args)
        {
            if (Config.Item("disable.draws").GetValue<bool>())
                return;

            if (Config.Item("optimize").GetValue<bool>()) return;

            if (Lux_E != null && Config.Item("draw.LuxE.position").GetValue<bool>())
            {
                var pos1 = Drawing.WorldToScreen(player.Position);
                var pos2 = Drawing.WorldToScreen(Lux_E.Position);


                Drawing.DrawLine(pos1, pos2, 1, System.Drawing.Color.LightBlue);
                Drawing.DrawCircle(Lux_E.Position, 100, System.Drawing.Color.Gray);

                foreach (var enemy in HeroManager.Enemies.Where(e => !e.IsDead))
                {
                    var pos3 = Drawing.WorldToScreen(enemy.Position);
                    if (enemy.Position.Distance(Lux_E.Position) < 200)
                        Drawing.DrawLine(pos2, pos3, 1, System.Drawing.Color.DarkRed);
                }
 
            }

            if (Config.Item("draw.Q").GetValue<Circle>().Active)
                if (Q1.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q1.Range,
                        Q1.IsReady() ? Config.Item("draw.Q").GetValue<Circle>().Color : System.Drawing.Color.Red);

            if (Config.Item("draw.W").GetValue<Circle>().Active)
                if (W1.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, W1.Range,
                        W1.IsReady() ? Config.Item("draw.W").GetValue<Circle>().Color : System.Drawing.Color.Red);

            if (Config.Item("draw.E").GetValue<Circle>().Active)
                if (E1.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, E1.Range - 1,
                        E1.IsReady() ? Config.Item("draw.E").GetValue<Circle>().Color : System.Drawing.Color.Red);

            if (Config.Item("draw.R").GetValue<Circle>().Active)
                if (R1.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, R1.Range - 2,
                        R1.IsReady() ? Config.Item("draw.R").GetValue<Circle>().Color : System.Drawing.Color.Red);

        }
        private static void Edetonation()
        {
            var target = TargetSelector.GetTarget(R1.Range, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (Lux_E == null) return;
                var rooted = target.HasBuff("LuxLightBindingMis");
                var debuff = target.HasBuff("luxilluminatingfraulein");

                var qcollision = Q1.GetCollision(player.Position.To2D(), new List<Vector2> { Q1.GetPrediction(target).CastPosition.To2D() });
                var minioncol = qcollision.Where(x => (x is AIHeroClient) && x.IsEnemy).Count();

                //Lux E detonation (Object Bounding Radius)
                if (Lux_E != null && rooted && target.Distance(player.Position) <=
                    Orbwalking.GetRealAutoAttackRange(player) && target.Health > E1.GetDamage(target) && debuff)
                {
                    Printchat("[E] Detonation Blocked. Reason: AA-able");
                    return;
                }

                if (Lux_E != null && rooted && target.Distance(player.Position) <=
                    Orbwalking.GetRealAutoAttackRange(player) + 300 && target.Health > E1.GetDamage(target) && debuff && target.CountEnemiesInRange(600) <= 1)
                {
                    Printchat("[E] Detonation Blocked. Reason: AA-able");
                    return;
                }

                if (Lux_E != null && Lux_E.Position.CountEnemiesInRange(E1.Width) >= 1)
                {
                    E1.Cast();
                    Printchat("[E] Toggle Cast. Reason: Enemy Detected");
                }
            }

            else if (Lux_E != null && Lux_E.Position.CountEnemiesInRange(E1.Width) >= 1)
            {
                E1.Cast();
                Printchat("[E] Toggle Cast. Reason: Enemy Detected");
            }
        }

        private static void EisAlive(GameObject sender, EventArgs args)
        {
            //Lux_Base_E_mis.troy
            //Lux_Base_E_tar_aoe_green.troy
            //Lux_Base_E_tar_nova.troy


            if (sender.Name == "Lux_Base_E_mis.troy")
            {
                Lux_E = sender;
                Printchat("Lux E object detected");
            }
        }

        private static void EisGone(GameObject sender, EventArgs args)
        {

            if (sender.Name == "Lux_Base_E_tar_nova.troy")
            {
                Lux_E = null;
                Printchat("Lux E has detonated");
            }
        }

        private static void OrbwalkerModes(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Routine();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    HarassRoutine();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    FarmMethod();
                    JungleMethod();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
            }

            if (Config.Item("autoharass").GetValue<KeyBind>().Active)
            {
                HarassRoutine();
            }
        }
        /// <summary>
        /// Printchat is being used for debug purposes.
        /// </summary>
        /// <param name="message"></param>
        private static void Printchat(string message)
        {
            if (!Config.Item("print.debug.chat").GetValue<bool>())
                return;

            Chat.Print(
                "<font color='#FFB90F'>[Console]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        private static void Printmsg(string message)
        {
            Chat.Print(
                "<font color='#00ff00'>[Cheerleader Lux]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        private static void Actives()
        {
            var enemies = HeroManager.Enemies.Where(e => e.IsValid);
            foreach (var enemy in enemies)
            {
                if (enemy == null) return;
                else if (enemy != null && enemy.IsValidTarget(R1.Range) && Config.Item("autospells.R").GetValue<bool>())
                {
                    SpellCastR(enemy);
                }
            }

        }
        public static int PassiveDMG(Obj_AI_Base target)
        {
           double PassiveDMG = player.CalcDamage(target, Damage.DamageType.Magical,
                10 + (8 * player.Level) + 0.2 * player.FlatMagicDamageMod);
            return (int)PassiveDMG;
        }
        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        /// <summary>
        /// Quick and Easy Spellcasts params for Lux
        /// </summary>
        /// <param name="target"></param> Which unit should we cast spells on
        /// <param name="range"></param> Range check for targets
        /// <param name="spellslot"></param> Which spellslot 
        /// <param name="collision"></param> Collision Check for Q usage
        /// <param name="count"></param>  Only use said ability if it can hit X amount of enemies
        /// <param name="objectcheck"></param> Objectcheck for E usage
        public static void SpellCast(AIHeroClient target, float range,  Spell spell, bool collision, byte count, bool objectcheck, HitChance Hitchance)
        {
            var CastPosition = Prediction.GetPrediction(target, 0.25f);
            if (spell.IsReady() && target.IsValidTarget(range) && CastPosition.Hitchance >= Hitchance && !collision && !objectcheck)
            {
                spell.Cast(target);
            }

            if (spell.IsReady() && target.IsValidTarget(range) && collision && CastPosition.Hitchance >= Hitchance 
                && Q1.GetPrediction(target).CollisionObjects.Count() <= 1 && !objectcheck)
            {
                spell.Cast(target);
            }

            if (Lux_E == null && spell.IsReady() && objectcheck && target.IsValidTarget(E1.Range) && CastPosition.Hitchance >= Hitchance && !collision)
            {
                spell.Cast(target);
            }
        }
        private static void JungleMethod()
        {
            var mana = Config.Item("jungleclear.mana.slider").GetValue<Slider>().Value;
            if (player.ManaPercent < mana) return;

            var minions = MinionManager.GetMinions(E1.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => m.IsValid
            && m.Distance(Player) < E1.Range && m != null).OrderBy(m => m.MaxHealth);
            var aaminions = MinionManager.GetMinions(E1.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => m.IsValid
            && m.Distance(Player) < Orbwalking.GetRealAutoAttackRange(player) && m != null).OrderBy(m => m.MaxHealth);

            var efarmpos = E1.GetCircularFarmLocation(new List<Obj_AI_Base>(minions), E1.Width);

            if (efarmpos.MinionsHit >= 1 &&
                E1.IsReady() && Config.Item("jungleclear.E").GetValue<bool>() && Environment.TickCount - Q1.LastCastAttemptT > 450)
                E1.Cast(efarmpos.Position);

            var qfarmpos = Q1.GetLineFarmLocation(new List<Obj_AI_Base>(minions), Q1.Width);

            if (qfarmpos.MinionsHit >= 1 &&
                Q1.IsReady() && Config.Item("jungleclear.Q").GetValue<bool>() && Environment.TickCount - E1.LastCastAttemptT > 450)
                Q1.Cast(qfarmpos.Position);

            foreach (var minion in aaminions.Where(m => m.IsMinion && !m.IsDead
            && m.HasBuff("luxilluminatingfraulein")))
            {
                if (minion.IsValid && minion != null)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                }
            }
        }
        private static void FarmMethod()
        {
            var mana = Config.Item("laneclear.mana.slider").GetValue<Slider>().Value;
            var level = Config.Item("laneclear.level").GetValue<Slider>().Value;
            if (player.ManaPercent < mana || player.Level < level) return;

            var eminions = Config.Item("laneclear.E.count").GetValue<Slider>().Value;
            var qminions = Config.Item("laneclear.Q.count").GetValue<Slider>().Value;

            var minions = MinionManager.GetMinions(E1.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid 
            && m.Distance(Player) < E1.Range).ToList();
            var aaminions = MinionManager.GetMinions(E1.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid 
            && m.Distance(Player) < Orbwalking.GetRealAutoAttackRange(player)).ToList();

            var efarmpos = E1.GetCircularFarmLocation(new List<Obj_AI_Base>(minions), E1.Width);

            if (efarmpos.MinionsHit >=  eminions &&
                E1.IsReady() && Config.Item("laneclear.E").GetValue<bool>() && Environment.TickCount - Q1.LastCastAttemptT > 450)
                E1.Cast(efarmpos.Position);

            var qfarmpos = Q1.GetLineFarmLocation(new List<Obj_AI_Base>(minions), Q1.Width);

            if (qfarmpos.MinionsHit >= qminions &&
                Q1.IsReady() && Config.Item("laneclear.Q").GetValue<bool>() && Environment.TickCount - E1.LastCastAttemptT > 450)
                Q1.Cast(qfarmpos.Position);

            foreach (var minion in aaminions.Where(m => m.IsMinion && !m.IsDead 
            && m.HasBuff("luxilluminatingfraulein")))
            {
                if (minion.IsValid)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                }
            }


        }
        public static void HarassRoutine() //Add only use Q on CC
        {
            var target = TargetSelector.GetTarget(R1.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid) return;

            var cc = 0; //Add Requirements

            var manaslider = Config.Item("harass.mana.slider").GetValue<Slider>().Value;

            if (player.ManaPercent < manaslider) return;

            if (Config.Item("harass.Q").GetValue<bool>() && Environment.TickCount - E1.LastCastAttemptT > 800 && Environment.TickCount - R1.LastCastAttemptT > 800)
                SpellCast(target, Q1.Range, Q1, true, 1, false, PredQ("prediction.Q"));

            if (Config.Item("harass.E").GetValue<bool>() && Environment.TickCount - Q1.LastCastAttemptT > 800 && Environment.TickCount - R1.LastCastAttemptT > 800)
                SpellCast(target, E1.Range, E1, false, 1, true, PredE("prediction.E"));

             
        }
        public static void Routine()
        {
            var targetR = TargetSelector.GetTarget(R1.Range, TargetSelector.DamageType.Magical);
            var target = TargetSelector.GetTarget(Q1.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid || targetR == null || !targetR.IsValid) return;

            Ignite = player.GetSpellSlot("summonerdot");

            //[R] Aoe Cast
            byte hitcount;
            hitcount = (byte)Config.Item("advanced.R.aoe.count").GetValue<Slider>().Value;

            if (Config.Item("advanced.R.aoe").GetValue<bool>() && R1.IsReady() && !E1.IsReady() && target.IsValidTarget(E1.Range + E1.Width))
            {
                R1.CastIfWillHit(target, hitcount);
            }

            #region -- Variables/Floats etc.
            float qdmg = Q1.GetDamage(target);
            float edmg = E1.GetDamage(target);
            float rdmg = R1.GetDamage(target);
            float aa = (float)Player.GetAutoAttackDamage(target);
            var insideE = Lux_E != null && target.Distance(Lux_E.Position) <= E1.Width;
            var thp = target.Health;
            var AArange = Orbwalking.GetRealAutoAttackRange(player);
            var debuff = target.HasBuff("luxilluminatingfraulein");
            if (debuff)
                rdmg += PassiveDMG(target);
            if (debuff)
                aa += PassiveDMG(target);
            if (insideE)
                rdmg += edmg;
            var rooted = target.HasBuff("LuxLightBindingMis");
            #endregion


            if (insideE && thp < edmg && target.IsValidTarget(R1.Range))
                return;

            if (rooted && insideE && rdmg + edmg > thp && target.IsValidTarget(R1.Range))
                SpellCastR(targetR);

            #region -- Q spellcast

            var prediction = Q1.GetPrediction(target, true);
            var collision = Q1.GetCollision(Player.Position.To2D(), new List<Vector2> { prediction.UnitPosition.To2D() });
            if (Config.Item("combo.Q").GetValue<bool>() 
                && Environment.TickCount - E1.LastCastAttemptT > 400 && Environment.TickCount - R1.LastCastAttemptT > 800 && prediction.Hitchance >= PredQ("prediction.Q"))
            {
                if (collision.Count == 2)
                {
                    if (collision[0].IsChampion() || collision[1].IsChampion())
                    {
                        Q1.Cast(prediction.CastPosition);
                    }
                }
                else if (collision.Count == 1 && collision[0].IsChampion())
                {
                    Q1.Cast(prediction.CastPosition);
                }
                else if (collision.Count <= 1)
                {
                    Q1.Cast(prediction.CastPosition);
                }
                #endregion -- Q spellcast end


                if (rooted && thp < aa && target.IsValidTarget(AArange))
                    return;

                if (Config.Item("combo.E").GetValue<bool>() && Environment.TickCount - Q1.LastCastAttemptT > 875 && Environment.TickCount - R1.LastCastAttemptT > 800)
                    SpellCast(target, E1.Range, E1, false, 1, true, PredE("prediction.E"));

                if (IgniteKillCheck() < thp && target.HasBuff("summonerdot"))
                    return;

                if (Config.Item("combo.R").GetValue<bool>() && R1.IsReady())
                    SpellCastR(targetR);

                if (!target.IsValidTarget(600)) return;

                var ignitemenu = Config.Item("autospells.ignite").GetValue<bool>();

                if (thp > IgniteDamage(target) && thp < IgniteDamage(target) + edmg + aa && rooted && E1.IsReady() && target.IsValidTarget(600) && Ignite.IsReady() && ignitemenu)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                    Printchat("Ignite casted");
                }

                if (thp < IgniteDamage(target) + rdmg + aa && rooted && Ignite.IsReady() && R1.IsReady() && ignitemenu)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                    Printchat("Ignite casted");
                }

                if (thp < IgniteDamage(target) + rdmg + aa && Ignite.IsReady() && R1.IsReady() && ignitemenu)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                    Printchat("Ignite casted");
                }

                if (thp < IgniteDamage(target) && target.IsValidTarget(600) && AllyCheck(target, 600) < 1 && Ignite.IsReady() && ignitemenu)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                    Printchat("Ignite casted");
                }
            }
        }

        private static float IgniteKillCheck() //Credits to Justy ~ Note to self: don't forget to give credits.
        {

            var igniteBuff =
                player.Buffs.Where(buff => buff.Name == "summonerdot")
                    .OrderBy(buff => buff.StartTime)
                    .FirstOrDefault();
            if (igniteBuff == null)
            {
                return 0;
            }
            else
            {
                var igniteDamage = Math.Floor(igniteBuff.EndTime - Game.Time) *
                player.GetSummonerSpellDamage(Attackers[0], Damage.SummonerSpell.Ignite) / 5;
                return (float)igniteDamage;
            }
        }
        public static float AllyCheck(AIHeroClient target, int range)
        {
            var allies = 
                ObjectManager.Get<AIHeroClient>().Where(a => a.IsAlly 
                && a.Distance(target.Position) < range && a.HealthPercent > 20 && !a.IsMe).ToList();

            return allies.Count;
            
        }
        public static void SpellCastR(AIHeroClient target)
        {
            if (target == null) return;
            if (!target.IsValid) return;
            var rslider = Config.Item("combo.r.slider").GetValue<Slider>().Value;

            #region variables/floats
            //[R] Combo Sequences
            double qdmg = Q1.GetDamage(target);
            double edmg = E1.GetDamage(target);
            double rdmg = R1.GetDamage(target);
            double aa = Player.GetAutoAttackDamage(target);
            Ignite = player.GetSpellSlot("summonerdot");
            var insideE = Lux_E != null && target.Distance(Lux_E.Position) < E1.Width;
            var thp = target.Health;
            var AArange = Orbwalking.GetRealAutoAttackRange(player);

            var debuff = target.HasBuff("luxilluminatingfraulein");

            if (debuff)
                rdmg += PassiveDMG(target);
            if (debuff)
                aa += PassiveDMG(target);
            if (insideE)
                rdmg += edmg;

            var qcollision = Q1.GetCollision(player.Position.To2D(), new List<Vector2> { Q1.GetPrediction(target).CastPosition.To2D() });
            var minioncol = qcollision.Where(x => (x is AIHeroClient) && x.IsEnemy).Count();
            var qpred = Q1.GetPrediction(target).Hitchance >= HitChance.High;
            var epred = E1.GetPrediction(target).Hitchance >= HitChance.High;

            var rooted = target.HasBuff("LuxLightBindingMis");


            if (target.Distance(player.Position) < rslider && !rooted) return;


            #endregion
            if (insideE && Lux_E != null)
                return;

                //Checks if Allies can kill the bitch
            if (AllyCheck(target, 600) >= 2 && target.Health < rdmg / 2)
                return;
            //Checks if an Ally can kill le bitch
            if (AllyCheck(target, 600) >= 1 && target.Health < rdmg / 2)
                return;

            if (rooted && insideE && rdmg + edmg > thp && target.IsValidTarget(R1.Range))
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));

            if (E1.IsReady() && thp < edmg && target.IsValidTarget(E1.Range) && epred)
                return;

            if (Q1.IsReady() && thp < qdmg && target.IsValidTarget(Q1.Range) && minioncol <= 1 && qpred)
                return;

            if (insideE && thp < edmg)
                return;

            if (insideE && thp < edmg + aa && target.IsValidTarget(AArange))
                return;

            if (thp < aa && target.IsValidTarget(AArange))
                return;

            if (thp < edmg && E1.IsReady() && rooted && target.IsValidTarget(E1.Range))
                return;

            if (Q1.IsReady() && !E1.IsReady() && thp < qdmg && target.IsValidTarget(Q1.Range) && minioncol <= 1 && qpred)
                return;

            if (E1.IsReady() && !Q1.IsReady() && thp < edmg && target.IsValidTarget(E1.Range) && epred)
                return;

            if (rooted && debuff && thp < aa && target.IsValidTarget(AArange))
                return;

            if (Environment.TickCount - E1.LastCastAttemptT < 100 || Environment.TickCount - Q1.LastCastAttemptT < 800 && !rooted)
                return;

            if (rooted && insideE && thp < rdmg + edmg && target.IsValidTarget(R1.Range))
            {
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
            }

            if (rooted && E1.IsReady() && target.IsValidTarget(E1.Range) && thp < rdmg + edmg && target.IsValidTarget(R1.Range))
            {
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
            }

            if (rooted && debuff && thp < rdmg + aa && target.IsValidTarget(AArange))
            {
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
            }

            if (rooted && debuff && thp < rdmg && target.IsValidTarget(R1.Range))
            {
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
            }

            if (thp < rdmg)
            {
                SpellCast(target, R1.Range, R1, false, 1, false, PredR("prediction.R"));
            }         
        }
        public static HitChance PredQ(string name)
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
        public static HitChance PredE(string name)
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
        public static HitChance PredR(string name)
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

        //WIP
        private static void Junglesteal()
        {
            if (!R1.IsReady())
                return;

            if (Config.Item("Blue").GetValue<bool>()) //Blue
            {
                var blueBuff =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.CharData.BaseSkinName == "SRU_Blue")
                        .Where(x => player.GetSpellDamage(x, SpellSlot.R) > x.Health)
                        .FirstOrDefault(x => x.Team != player.Team); /// ---------- Check Playerteam stuff ~

                if (blueBuff != null)
                    R1.Cast(blueBuff);
            }

            if (Config.Item("Red").GetValue<bool>()) //Red
            {
                var redBuff =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.CharData.BaseSkinName == "SRU_Red")
                        .Where(x => player.GetSpellDamage(x, SpellSlot.R) > x.Health)
                        .FirstOrDefault(x => x.Team != player.Team);

                if (redBuff != null)
                    R1.Cast(redBuff);
            }

            if (Config.Item("Baron").GetValue<bool>()) //Baron
            {
                var Baron =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.CharData.BaseSkinName == "SRU_Baron")
                        .Where(x => player.GetSpellDamage(x, SpellSlot.R) > x.Health)
                        .FirstOrDefault(x => x.Team != player.Team);

                if (Baron != null)
                    R1.Cast(Baron);
            }

            if (Config.Item("Dragon").GetValue<bool>()) //Dragon
            {
                var Dragon =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .Where(x => x.CharData.BaseSkinName == "SRU_Dragon")
                        .Where(x => player.GetSpellDamage(x, SpellSlot.R) > x.Health)
                        .FirstOrDefault(x => x.Team != player.Team);

                if (Dragon != null)
                    R1.Cast(Dragon);
            }
        }
    }
}

