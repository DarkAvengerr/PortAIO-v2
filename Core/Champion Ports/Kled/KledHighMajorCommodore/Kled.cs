using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using System.Collections.Generic;
using SharpDX;
using System.Drawing;

//This is an assembly for the champion Kled. I'd prefer it if you wouldn't copy this code without giving proper credits.
//B-baka.
using EloBuddy; 
using LeagueSharp.Common; 
namespace KledHighMajorCommodore
{
    internal class Kled
    {
        #region Statics
        static Menu Config;
        static Orbwalking.Orbwalker Orbwalker;
        static Spell Shotgun;
        static Spell FuckYou;
        static Spell Dash;
        static Spell Trap;
        static GameObject JoustObj;

        #endregion Statics

        #region Main Entrypoint
        public static void Main()
        {
            OnLoad(new EventArgs());
        }
        #endregion Main Entrypoint

        #region Menu/OnLoad
        static void OnLoad(EventArgs args) //It's not like I wanna initiate your assembly... b-baka
        {
            if (ObjectManager.Player.ChampionName != "Kled") return;

            Printmsg("Assembly Version: 1.2.0.0");
            Printmsg("Successfully Loaded | Have Fun!");

            Config = new Menu("HMC Kled", "Kled", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.Orange); ;
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker Settings", "Orbwalker Settings")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Target Selector")));

            //Spells
            Shotgun = new Spell(SpellSlot.Q, 750);
            Trap = new Spell(SpellSlot.Q, 750);
            Dash = new Spell(SpellSlot.E, 560);
            //3500 / 4000 / 4500 
            FuckYou = new Spell(SpellSlot.R, 3500);

            Shotgun.SetSkillshot(0.25f, 80, 3000, false, SkillshotType.SkillshotLine);
            Trap.SetSkillshot(0.25f, 40, 1800, false, SkillshotType.SkillshotLine);
            Dash.SetSkillshot(0.25f, 60, 2200, false, SkillshotType.SkillshotLine);

            //Begin Menu Variables.
            var combat = Config.AddSubMenu(new Menu("Combat Settings", "Combat Settings"));
            var killsteal = combat.AddSubMenu(new Menu("Killsteal Settings", "Killsteal Settings"));
            var farm = Config.AddSubMenu(new Menu("Farm Settings", "Farm Settings"));
            var lane = farm.AddSubMenu(new Menu("Laneclear Settings", "Laneclear Farm Settings"));
            var jungle = farm.AddSubMenu(new Menu("Jungleclear Settings", "Jungleclear Settings"));
            var drawing = Config.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));
            var dev = Config.AddSubMenu(new Menu("Developer Menu", "Developer Menu"));
            var credits = Config.AddSubMenu(new Menu("Credits", "Credits"));

            //Combat
            combat.AddItem(new MenuItem("blank3", "Kled").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            combat.AddItem(new MenuItem("Use.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            combat.AddItem(new MenuItem("Use.ShotgunAntiGapCloser", "Gapclose with Shotgun").SetValue(true));
            combat.AddItem(new MenuItem("Use.ShotgunGapCloser", "Anti-Gapclose with Shotgun").SetValue(true));
            combat.AddItem(new MenuItem("blank2", "Skaarl").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            combat.AddItem(new MenuItem("Use.Trap", "Use Trap | Use Q").SetValue(true));
            combat.AddItem(new MenuItem("Use.TrapAntiGapCloser", "Anti-Gapclose with Trap").SetValue(true));
            combat.AddItem(new MenuItem("Use.Dash", "Use Dash | Use E").SetValue(true));

            combat.AddItem(new MenuItem("blank4", "Harass/Mixed Mode").SetFontStyle(System.Drawing.FontStyle.Bold));
            combat.AddItem(new MenuItem("Use.ShotgunHarass", "Kled: Use Shotgun | Use Q").SetValue(false));
            combat.AddItem(new MenuItem("Use.TrapHarass", "Skaarl: Use Trap | Use Q").SetValue(true));

            //combat.AddItem(new MenuItem("blank4", "Offensive Items/Summoners").SetFontStyle(System.Drawing.FontStyle.Bold));
            //combat.AddItem(new MenuItem("Use.Tiamat", "Use Tiamat").SetValue(true));
            //combat.AddItem(new MenuItem("Use.Hydra", "Use Titanic/Ravenous Hydra").SetValue(true));
            //combat.AddItem(new MenuItem("Use.Ignite", "Use Ignite as Finisher").SetValue(true));

            killsteal.AddItem(new MenuItem("blank9", "Killsteal Settings").SetFontStyle(System.Drawing.FontStyle.Bold));
            killsteal.AddItem(new MenuItem("blank5", "Kled").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            killsteal.AddItem(new MenuItem("ks.shotgun", "Use Shotgun | Use Q for KS").SetValue(true));
            killsteal.AddItem(new MenuItem("blank12", "Skaarl").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            killsteal.AddItem(new MenuItem("ks.trap", "Use Trap | Use Q for KS").SetValue(true));
            killsteal.AddItem(new MenuItem("ks.dash", "Use Dash | Use E for KS").SetValue(false));
            killsteal.AddItem(new MenuItem("blank6", "Misc Settings").SetFontStyle(System.Drawing.FontStyle.Bold));
            killsteal.AddItem(new MenuItem("disable.ks", "Disable All Killsteal Methods").SetValue(false));

            //Lane
            lane.AddItem(new MenuItem("blank3", "Kled").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            lane.AddItem(new MenuItem("laneclear.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            lane.AddItem(new MenuItem("laneclear.Shotgun.count", "[Shotgun] Minions Hit").SetValue(new Slider(4, 8, 1)));
            lane.AddItem(new MenuItem("blank2", "Skaarl").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            lane.AddItem(new MenuItem("laneclear.Trap", "Use Trap | Use Q").SetValue(true));
            lane.AddItem(new MenuItem("laneclear.Trap.count", "[Trap] Minions Hit").SetValue(new Slider(4, 8, 1)));
            lane.AddItem(new MenuItem("laneclear.Dash", "Use Dash | Use E").SetValue(false));
            lane.AddItem(new MenuItem("laneclear.Dash.count", "[Dash] Minions Hit").SetValue(new Slider(4, 8, 1)));

            //Jungle
            jungle.AddItem(new MenuItem("blank3", "Kled").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            jungle.AddItem(new MenuItem("jungleclear.Shotgun", "Use Shotgun | Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("blank2", "Skaarl").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            jungle.AddItem(new MenuItem("jungleclear.Trap", "Use Trap | Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("jungleclear.Dash", "Use Dash | Use E").SetValue(true));

            //Drawing
            drawing.AddItem(new MenuItem("blank3", "Kled").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            drawing.AddItem(new MenuItem("Draw.Shotgun", "Draw Shotgun Range | Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.Gray)));
            drawing.AddItem(new MenuItem("blank2", "Skaarl").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            drawing.AddItem(new MenuItem("Draw.Trap", "Draw Trap Range | Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.DarkOrange)));
            drawing.AddItem(new MenuItem("Draw.Dash", "Draw Dash Range | Draw E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            drawing.AddItem(new MenuItem("blank420", "Misc settings").SetFontStyle(System.Drawing.FontStyle.Bold));
            drawing.AddItem(new MenuItem("draw.minimap", "Draw R range on Minimap").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            drawing.AddItem(new MenuItem("disable.draws", "Disable all Drawings").SetValue(false));
            drawing.AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(0, 30, 0)));

            //Dev
            dev.AddItem(new MenuItem("dev.on", "Enable Developer Mode").SetValue(true));
            dev.AddItem(new MenuItem("dev.chat", "Enable Debug in Chat").SetValue(false));

            //Credits
            credits.AddItem(new MenuItem("cred1", "Made By ScienceARK").SetFontStyle(System.Drawing.FontStyle.Bold));
            credits.AddItem(new MenuItem("cred2", "Playtesting/Menu: LazyMexican"));

            Config.AddToMainMenu();

            Game.OnUpdate += orbwalkerManager;
            AntiGapcloser.OnEnemyGapcloser += StopDashingAtMeBro;
            Drawing.OnDraw += GottaDrawSpellRanges;
            GameObject.OnCreate += JoustObject;
            GameObject.OnDelete += JoustObjDelete;
            Drawing.OnEndScene += EverythingInOneCsFileIsOkayImo; //potato tomato

        }
        #endregion Menu/OnLoad

        #region random shit
        private static void EverythingInOneCsFileIsOkayImo(EventArgs args)
        {
            bool drawMinimapR = Config.Item("draw.minimap").GetValue<Circle>().Active;
            if (ObjectManager.Player.Level >= 6 && drawMinimapR)
                LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, FuckYou.Range, Config.Item("draw.minimap").GetValue<Circle>().Color, 2, 30, true); //being obsolete is also okay
        }

        private static void JoustObjDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kled_Base_E_Mark.troy"))
            {
                JoustObj = null;
                Printchat("[E] Joust Mark Removed");
            }
        }

        private static void JoustObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals("Kled_Base_E_Mark.troy"))
            {
                JoustObj = sender;
                Printchat("[E] Joust Mark Detected");
            }

        }
        #endregion random shit


        #region Drawings
        //Spell Draw Ranges
        private static void GottaDrawSpellRanges(EventArgs args)
        {

            if (Config.Item("disable.draws").GetValue<bool>()) return;

            if (Config.Item("Draw.Shotgun").GetValue<Circle>().Active)
            {
                if (Shotgun.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Shotgun.Range, Shotgun.IsReady() ?
                        Config.Item("Draw.Shotgun").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
            }

            if (Config.Item("Draw.Trap").GetValue<Circle>().Active)
            {
                if (Trap.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Trap.Range, Trap.IsReady() ?
                        Config.Item("Draw.Trap").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
            }

            if (Config.Item("Draw.Dash").GetValue<Circle>().Active)
            {
                if (Dash.Level > 0)
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Dash.Range, Dash.IsReady() ?
                        Config.Item("Draw.Dash").GetValue<Circle>().Color : System.Drawing.Color.Red, Config.Item("CircleThickness").GetValue<Slider>().Value);
            }

            var EnemyTarget = TargetSelector.GetTarget(Shotgun.Range, TargetSelector.DamageType.Magical);
            if (EnemyTarget != null)
            {


                var GapClosePos = (EnemyTarget.Position.Extend(ObjectManager.Player.Position, EnemyTarget.Position.Distance(ObjectManager.Player.Position) + 250));
                var Enemypos = Drawing.WorldToScreen(EnemyTarget.Position);
                var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);

                if (Config.Item("dev.on").GetValue<bool>())
                {
                    //Remember to remove knockback dot
                    if (EnemyTarget.IsValid && GapClosePos != null)
                       Render.Circle.DrawCircle(GapClosePos, 5, System.Drawing.Color.Gold, 5);

                    if (EnemyTarget.IsValid && EnemyTrapped(EnemyTarget))
                        Drawing.DrawText(Enemypos.X - 30, Enemypos.Y + 30, System.Drawing.Color.MediumPurple, "QMark Duration: " + QMarkDuration(EnemyTarget).ToString("#.#"));

                    if (EnemyTarget.IsValid && EnemyDashable(EnemyTarget))
                        Drawing.DrawText(Enemypos.X - 30, Enemypos.Y + 20, System.Drawing.Color.Purple, "Enemy Has Joust Mark");

                    if (EnemyTarget.IsValid && EnemyDashable(EnemyTarget))
                       Drawing.DrawText(pos.X - 30, pos.Y + 20, System.Drawing.Color.Orange, "2nd Dash Duration " + GetDashDuration().ToString("#.#"));

                  if (EnemyTarget.IsValid && EnemyTrapped(EnemyTarget))
                       Render.Circle.DrawCircle(EnemyTarget.Position, 20, System.Drawing.Color.MediumPurple, 5);
               }
           }

        }
        #endregion Drawings

        #region AntiGapCloser
        private static void StopDashingAtMeBro(ActiveGapcloser gapcloser)
        {
            var UseShotgunAntiGapCloser = Config.Item("Use.ShotgunAntiGapCloser").GetValue<bool>();
            var UseTrapAntiGapCloser = Config.Item("Use.TrapAntiGapCloser").GetValue<bool>();
            if (ObjectManager.Player.IsDead || gapcloser.Sender == null)
                return;

            var targetpos = Drawing.WorldToScreen(gapcloser.Sender.Position);
            if (gapcloser.Sender.IsValidTarget(Shotgun.Range))
            {
                Render.Circle.DrawCircle(gapcloser.Sender.Position, gapcloser.Sender.BoundingRadius,
                System.Drawing.Color.DeepPink);
                Drawing.DrawText(targetpos[0] - 40, targetpos[1] + 20, System.Drawing.Color.DodgerBlue, "GAPCLOSER");
            }

            if (Shotgun.IsReady() && gapcloser.Sender.IsValidTarget(Shotgun.Range) && !Mounted() && UseShotgunAntiGapCloser && gapcloser.End.Distance(ObjectManager.Player.Position) < 350)
            {
                Shotgun.Cast(gapcloser.Sender);
            }

            if (Trap.IsReady() && gapcloser.Sender.IsValidTarget(Trap.Range) && Mounted() && UseTrapAntiGapCloser && gapcloser.End.Distance(ObjectManager.Player.Position) < 350 )
            {
                Trap.Cast(gapcloser.Sender);
            }
        }
        #endregion AntiGapCloser

        #region OrbwalkerManager
        private static void orbwalkerManager(EventArgs args)
        {

            //Switch orbwalkersmode depending on pressed button, baka.
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    FuckEmUp();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    SlapTheShitOutOfEm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    KillDemMinions();
                    WhoGoesKledJungle();
                    break;
            }

            if (!Config.Item("disable.ks").GetValue<bool>())
            {
                BitchThatWasMyKill();
            }

            //Quick and Dirty R range ~ ur waifu is shit
            if (FuckYou.Level == 1) FuckYou.Range = 3500;
            if (FuckYou.Level == 2) FuckYou.Range = 4000;
            if (FuckYou.Level == 3) FuckYou.Range = 4500;

        }
        #endregion OrbwalkerManager

        #region Buff Checks
        //IsMounted
        public static bool Mounted()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Name != "KledRiderQ")
            {
                return true;
            }
            else return false;
        }

        //EnemyHasQDebuff
        public static bool EnemyTrapped(AIHeroClient enemy)
        {
            if (enemy == null) return false;
            if (enemy.HasBuff("kledqmark"))
            {
                return true;
            }
            else return false;
        }

        //Check W Passive ON
        public static bool KledWOn()
        {
            if (ObjectManager.Player.HasBuff("kledwactive"))
            {
                return true;
            }
            else return false;
        }

        //CheckEDebuff
        public static bool EnemyDashable(AIHeroClient enemy)
        {
            if (enemy == null) return false;
            if (enemy.HasBuff("klede2target"))
            {
                return true;
            }
            else return false;
        }
        //Check Second Dash
        public static bool PlayerDashUp()
        {
            if (ObjectManager.Player.HasBuff("KledE2"))
            {
                return true;
            }
            else return false;
        }
        #endregion Buff Checks

        #region math
        //Dash Buff Duration (Expiration date)
        public static float GetDashDuration()
        {
            if (ObjectManager.Player == null || !ObjectManager.Player.HasBuff("KledE2")) return 0;
            float DashBuffDuration = ObjectManager.Player.GetBuff("KledE2").EndTime - Game.Time;
            return DashBuffDuration;

        }
        public static float QMarkDuration(AIHeroClient enemy)
        {
            if (enemy == null || !enemy.HasBuff("kledqmark")) return 0;
            float TrapBuffDuration = enemy.GetBuff("kledqmark").EndTime - Game.Time;
            return TrapBuffDuration;

        }
        #endregion math


        #region Combo
        //Combo
        private static void FuckEmUp()
        {
            var EnemyTarget = TargetSelector.GetTarget(Trap.Range, TargetSelector.DamageType.Magical);
            if (EnemyTarget == null) return;

            //DashBuff
            var DashBuff = ObjectManager.Player.GetBuff("KledE2");

            //Prediction
            var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.Medium;
            var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
            var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.Low;

            //Damages
            double DashDmg = DashDamage(EnemyTarget);
            double AADamage = ObjectManager.Player.GetAutoAttackDamage(EnemyTarget);
            double ShotgunDmg = ShotgunDamage(EnemyTarget);
            double TrapDmg = TrapDamage(EnemyTarget);

            //Menu
            var UseShotgun = Config.Item("Use.Shotgun").GetValue<bool>();
            var UseGapShotgun = Config.Item("Use.ShotgunGapCloser").GetValue<bool>();
            var UseTrap = Config.Item("Use.Trap").GetValue<bool>();
            var UseDash = Config.Item("Use.Dash").GetValue<bool>();

            if (!Mounted())
            {

                //Use Shotgun to get Skaarl
                if (ObjectManager.Player.Mana >= 80 && ObjectManager.Player.Mana < 100 && Shotgun.IsReady() && EnemyTarget.IsValidTarget(Shotgun.Range) && UseShotgun && ShotgunPred)
                {
                    Shotgun.Cast(EnemyTarget);
                }

                //Shotguncast while walking away | Sort of Fleeing?
                if (Shotgun.IsReady() && !Mounted() && EnemyTarget.IsValidTarget(Shotgun.Range) && !ObjectManager.Player.IsFacing(EnemyTarget) && EnemyTarget.IsMoving && UseShotgun && ShotgunPred)
                {
                    Shotgun.Cast(EnemyTarget);
                }

                var GapClosePos = (EnemyTarget.Position.Extend(ObjectManager.Player.Position, EnemyTarget.Position.Distance(ObjectManager.Player.Position) + 250));
                //Shotgun Gapclose | Gapclose while facing target
                if (ObjectManager.Player.Distance(EnemyTarget.Position) <= 450 + ObjectManager.Player.AttackRange && Shotgun.IsReady()
                     && ObjectManager.Player.IsFacing(EnemyTarget) && EnemyTarget.Distance(ObjectManager.Player.Position) > 200 && UseGapShotgun && ObjectManager.Player.Health > EnemyTarget.Health) //Dash towards enemy with Q.
                {
                    Shotgun.Cast(GapClosePos);
                    Printchat("[Q] Gapcloser - To Target");
                }
            }

            if (Mounted())
            {
                //Basic Trap cast
                if (Trap.IsReady() && Mounted() && EnemyTarget.IsValidTarget(Trap.Range) && UseTrap && TrapPred && !ObjectManager.Player.IsDashing())
                {
                    Trap.Cast(EnemyTarget);
                }

                //2nd Dash if duration is almost expired || Use E when duration is almost over
                if (EnemyTarget.IsValidTarget(Dash.Range * 2) && PlayerDashUp() && EnemyDashable(EnemyTarget) && GetDashDuration() < 1 && UseDash && DashPred)
                {
                    Dash.Cast(EnemyTarget);
                }

                if (Dash.IsReady())
                {

                    //Dash if enemy is in range || Use E in Range
                    if (Dash.IsReady() && EnemyTarget.IsValidTarget(Dash.Range) && !PlayerDashUp() && !EnemyTrapped(EnemyTarget) && UseDash)
                    {
                        Dash.Cast(EnemyTarget.Position);
                        Printchat("[E] Dash if enemy is in range || Use E in Range");
                    }

                    //Cast if Killable with E+AA*2 - First dash
                    if (EnemyTarget.Health < DashDmg + (AADamage * 3) && !PlayerDashUp() && UseDash && DashPred && ObjectManager.Player.Health > EnemyTarget.Health && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Cast if Killable with E+AA*2 - First dash");
                    }

                    //Cast if killable with E+AA*3 - First Dash + Passive
                    if (EnemyTarget.Health < DashDmg + (AADamage * 4) && !PlayerDashUp() && UseDash && DashPred && KledWOn() && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("Cast if killable with E+AA*3 - First Dash + Passive");
                    }

                    //Dash if Killable
                    if (EnemyTrapped(EnemyTarget) && !PlayerDashUp() && EnemyTarget.IsValidTarget(Trap.Range) && EnemyTarget.Health < TrapDmg + AADamage * 4 && EnemyTarget.Distance(ObjectManager.Player.Position) >= Dash.Range && Dash.IsReady() && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget.Position);
                        Printchat("[E] Dash if Killable");
                    }

                    //Dash if W is Active and Q hit thus ignoring the ranged requirement - After Dashable target died.
                    if (EnemyTrapped(EnemyTarget) && KledWOn() && !PlayerDashUp() && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 250)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if W is Active and Q hit thus ignoring the ranged requirement - After Dashable target died.");

                    }
                    //Dash if W is Active and Q hit thus ignoring the ranged ruquirement 
                    if (EnemyTrapped(EnemyTarget) && KledWOn() && PlayerDashUp() && EnemyDashable(EnemyTarget) && UseDash && DashPred && EnemyTarget.Distance(ObjectManager.Player.Position) > 250)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if W is Active and Q hit thus ignoring the ranged ruquirement ");
                    }

                    //Dash if target dashes. | ignores other logic.
                    if (EnemyTarget.IsValidTarget(Dash.Range) && PlayerDashUp() && UseDash && DashPred && EnemyTarget.IsDashing() && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("[E] Dash if target dashes. | ignores other logic. ");
                    }

                    //Dash if enemy is ranged (Doesn't check Q debuff) || E if enemy is melee (Ignores Q debuff)
                    if (Dash.IsReady() && EnemyTarget.IsValidTarget(Dash.Range) && !PlayerDashUp() && EnemyTarget.IsRanged && UseDash && DashPred)
                    {
                        Dash.Cast(EnemyTarget);
                        Printchat("Dash if enemy is ranged (Doesn't check Q debuff) || E if enemy is melee (Ignores Q debuff)");
                    }

                    if (JoustObj != null)
                    {

                        //2nd Dash if duration is almost expired || Use E when duration is almost over - After Enemy dies with Dash Buff
                        if (PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) <= 200 && GetDashDuration() < 1 && UseDash)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if duration is almost expired");
                        }
                        //2nd Dash if enemy is out of AA range || Out of AA E dash - After Enemy dies with Dash Buff
                        if (PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && EnemyTarget.Distance(ObjectManager.Player.Position) > 290 && UseDash)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        if (EnemyTrapped(EnemyTarget) && KledWOn() && PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && UseDash && EnemyTarget.Distance(ObjectManager.Player.Position) > 280)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        if (KledWOn() && PlayerDashUp() && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 150 && UseDash && EnemyTarget.Distance(ObjectManager.Player.Position) > 290)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash if enemy is out of AA range");
                        }

                        //Cast if Killable with E+AA*2 - 2nd dash
                        if (EnemyTarget.Health < DashDmg + (AADamage * 2) && PlayerDashUp() && UseDash && ObjectManager.Player.Health > EnemyTarget.Health && JoustObj.IsValid && EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.GetEnemiesInRange(400).Count <= 1)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("2nd Dash Cast if Killable with E+AA*2 - 2nd dash");
                        }

                        //Cast if killable with E+AA*3 - 2nd Dash + Passive
                        if (EnemyTarget.Health < DashDmg + (AADamage * 3) && PlayerDashUp() && UseDash && KledWOn() 
                            && JoustObj.IsValid & EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.Position.Distance(JoustObj.Position) < 100 && EnemyTarget.GetEnemiesInRange(400).Count <= 1)
                        {
                            Dash.Cast(EnemyTarget.Position);
                            Printchat("Cast if killable with E+AA*3 - 2nd Dash + Passive");
                        }
                    }


                }
            }
        }
        #endregion combo
        #region Killsteal
        private static void BitchThatWasMyKill()
        {
            var enemies = HeroManager.Enemies.Where(e => e.IsValidTarget(Shotgun.Range) && e != null);
            //Menu
            var UseShotgun = Config.Item("ks.shotgun").GetValue<bool>();
            var UseTrap = Config.Item("ks.trap").GetValue<bool>();
            var UseDash = Config.Item("ks.dash").GetValue<bool>();

            foreach (var EnemyTarget in enemies)
            {
                if (EnemyTarget.IsValid && EnemyTarget != null)
                {
                    //Prediction
                    var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
                    //Damages
                    double DashDmg = DashDamage(EnemyTarget);
                    double AADamage = ObjectManager.Player.GetAutoAttackDamage(EnemyTarget);
                    double ShotgunDmg = ShotgunDamage(EnemyTarget);
                    double TrapDmg = TrapDamage(EnemyTarget);

                    if (!Mounted() && EnemyTarget.Health < ShotgunDmg && UseShotgun && EnemyTarget.IsValidTarget(Shotgun.Range) && ShotgunPred) Shotgun.Cast(EnemyTarget);

                    if (Mounted())
                    {
                        if (EnemyTarget.Health < TrapDmg && UseTrap && EnemyTarget.IsValidTarget(Trap.Range) && TrapPred) Trap.Cast(EnemyTarget);
                        if (EnemyTarget.Health < TrapDmg && UseDash && EnemyTarget.IsValidTarget(Dash.Range) && DashPred) Dash.Cast(EnemyTarget);
                    }


                }


            }


        }
        #endregion Killsteal

        #region Mixed
        //Mixed
        private static void SlapTheShitOutOfEm()
        {
            var EnemyTarget = TargetSelector.GetTarget(Trap.Range, TargetSelector.DamageType.Magical);
            if (EnemyTarget == null || !EnemyTarget.IsValid) return;

            //Prediction
            var ShotgunPred = Shotgun.GetPrediction(EnemyTarget).Hitchance >= HitChance.Medium;
            var TrapPred = Trap.GetPrediction(EnemyTarget).Hitchance >= HitChance.High;
            var DashPred = Dash.GetPrediction(EnemyTarget).Hitchance >= HitChance.Low;

            //Menu
            var UseShotgun = Config.Item("Use.ShotgunHarass").GetValue<bool>();
            var UseTrap = Config.Item("Use.TrapHarass").GetValue<bool>();


            if (Mounted())
            {
                if (UseTrap && Trap.IsReady() && EnemyTarget.IsValidTarget(Trap.Range) && TrapPred)
                    Trap.Cast(EnemyTarget);
            }
            if (!Mounted())
            {
                if (UseShotgun && Shotgun.IsReady() && EnemyTarget.IsValidTarget(Shotgun.Range) && ShotgunPred)
                    Shotgun.Cast(EnemyTarget);
            }
        }
        #endregion Mixed

        #region Laneclear
        //Lane
        private static void KillDemMinions()
        {
            //Menu
            var LaneclearShotgun = Config.Item("laneclear.Shotgun").GetValue<bool>();
            var LaneclearTrap = Config.Item("laneclear.Trap").GetValue<bool>();
            var LaneclearDash = Config.Item("laneclear.Dash").GetValue<bool>();

            var Shotguncount = Config.Item("laneclear.Shotgun.count").GetValue<Slider>();
            var Trapcount = Config.Item("laneclear.Trap.count").GetValue<Slider>();
            var Dashcount = Config.Item("laneclear.Dash.count").GetValue<Slider>();

            //Shotgun
            var MinionsShotGunRange = MinionManager.GetMinions(Shotgun.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Shotgun.Range).ToList();
            var ShotgunFarmPosition = Shotgun.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsShotGunRange), Shotgun.Width);

            //Trap
            var MinionsTrapRange = MinionManager.GetMinions(Trap.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Trap.Range).ToList();
            var TrapFarmPosition = Trap.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsTrapRange), Trap.Width);

            //Dash
            var MinionsDashRange = MinionManager.GetMinions(Dash.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Dash.Range).ToList();
            var DashFarmPosition = Dash.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsDashRange), Dash.Width);


            //Checks for Kled
            if (!Mounted())
            {
                foreach (var minion in MinionsShotGunRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearShotgun) return;

                    if (Shotgun.IsReady() && ShotgunFarmPosition.MinionsHit >= Shotguncount.Value)
                    {
                        Shotgun.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }

            //Checks for Skaarl
            if (Mounted())
            {
                foreach (var minion in MinionsTrapRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearTrap) return;

                    if (Trap.IsReady() && TrapFarmPosition.MinionsHit >= Trapcount.Value && !ObjectManager.Player.IsDashing())
                    {
                        Trap.Cast(TrapFarmPosition.Position);
                    }

                }
                foreach (var minion in MinionsDashRange)
                {
                    if (minion == null || !minion.IsValid || !LaneclearDash) return;

                    if (Dash.IsReady() && DashFarmPosition.MinionsHit >= Dashcount.Value)
                    {
                        Dash.Cast(DashFarmPosition.Position);
                    }

                }
            }


        }
        #endregion laneclear

        #region Jungle
        //Jungle
        private static void WhoGoesKledJungle()
        {

            //Menu
            var JungleclearShotgun = Config.Item("jungleclear.Shotgun").GetValue<bool>();
            var JungleclearTrap = Config.Item("jungleclear.Trap").GetValue<bool>();
            var JungleclearDash = Config.Item("jungleclear.Dash").GetValue<bool>();


            //Shotgun
            var MinionsShotGunRange = MinionManager.GetMinions(Shotgun.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Shotgun.Range).ToList().OrderBy(m => m.MaxHealth);
            var ShotgunFarmPosition = Shotgun.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsShotGunRange), Shotgun.Width);

            //Trap
            var MinionsTrapRange = MinionManager.GetMinions(Trap.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Trap.Range).ToList().OrderBy(m => m.MaxHealth);
            var TrapFarmPosition = Trap.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsTrapRange), Trap.Width);

            //Dash
            var MinionsDashRange = MinionManager.GetMinions(Dash.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => m.IsValid
            && m.Distance(ObjectManager.Player) < Dash.Range).ToList().OrderBy(m => m.MaxHealth);
            var DashFarmPosition = Dash.GetLineFarmLocation(new List<Obj_AI_Base>(MinionsDashRange), Dash.Width);


            //Checks for Kled
            if (!Mounted())
            {
                foreach (var minion in MinionsShotGunRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearShotgun) return;

                    if (Shotgun.IsReady())
                    {
                        Shotgun.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }

            //Checks for Skaarl
            if (Mounted())
            {
                foreach (var minion in MinionsTrapRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearTrap) return;

                    if (Trap.IsReady() && !ObjectManager.Player.IsDashing())
                    {
                        Trap.Cast(ShotgunFarmPosition.Position);
                    }

                }
                foreach (var minion in MinionsDashRange)
                {
                    if (minion == null || !minion.IsValid || !JungleclearDash) return;

                    if (Dash.IsReady())
                    {
                        Dash.Cast(ShotgunFarmPosition.Position);
                    }

                }
            }
        }
        #endregion Jungle

        #region Chat
        static void Printchat(string message)
        {
            if (!Config.Item("dev.chat").GetValue<bool>() || !Config.Item("dev.on").GetValue<bool>())
                return;

            Chat.Print(
                "<font color='#00ff00'>[DEBUG]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        static void Printmsg(string message)
        {
            Chat.Print(
                "<font color='#FFB90F'>[High Major Commodore Kled]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        #endregion Chat

        #region Items
        private static void GottaUseItems()
        {

        }
        #endregion Items

        #region DamageCalculations..

        static double ShotgunDamage(AIHeroClient target)
        {
            return
              ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] { 0, 30, 45, 60, 75, 90 }[Shotgun.Level] + 0.8 * ObjectManager.Player.FlatPhysicalDamageMod);
            
        }
        static double TrapDamage(AIHeroClient target)
        {
            return
              ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] { 0, 25, 50, 75, 100, 125 }[Trap.Level] + 0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

        }
        static double DashDamage(AIHeroClient target)
        {
            return
              ObjectManager.Player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] { 0, 25, 45, 70, 95, 120 }[Dash.Level] + 0.6 * ObjectManager.Player.FlatPhysicalDamageMod);

        }
        #endregion DamageCalculations..

        //nobody will ever see this, teehee.
    }
}
