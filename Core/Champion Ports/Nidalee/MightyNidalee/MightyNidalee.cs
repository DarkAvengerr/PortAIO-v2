using System;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using System.Collections.Generic;

using EloBuddy; 
using LeagueSharp.Common; 
namespace MightyNidalee
{
    class Mighty
    {

//  / $$      /$$ /$$           /$$         /$$                     /$$   /$$ /$$       /$$           /$$                    
//  | $$$    /$$$|__/          | $$        | $$                    | $$$ | $$|__/      | $$          | $$                    
//  | $$$$  /$$$$ /$$  /$$$$$$ | $$$$$$$  /$$$$$$   /$$   /$$      | $$$$| $$ /$$  /$$$$$$$  /$$$$$$ | $$  /$$$$$$   /$$$$$$ 
//  | $$ $$/$$ $$| $$ /$$__  $$| $$__  $$|_  $$_/  | $$  | $$      | $$ $$ $$| $$ /$$__  $$ |____  $$| $$ /$$__  $$ /$$__  $$
//  | $$  $$$| $$| $$| $$  \ $$| $$  \ $$  | $$    | $$  | $$      | $$  $$$$| $$| $$  | $$  /$$$$$$$| $$| $$$$$$$$| $$$$$$$$
//  | $$\  $ | $$| $$| $$  | $$| $$  | $$  | $$ /$$| $$  | $$      | $$\  $$$| $$| $$  | $$ /$$__  $$| $$| $$_____/| $$_____/
//  | $$ \/  | $$| $$|  $$$$$$$| $$  | $$  |  $$$$/|  $$$$$$$      | $$ \  $$| $$|  $$$$$$$|  $$$$$$$| $$|  $$$$$$$|  $$$$$$$
//  |__/     |__/|__/ \____  $$|__/  |__/   \___/   \____  $$      |__/  \__/|__/ \_______/ \_______/|__/ \_______/ \_______/
//                    /$$  \ $$                     /$$  | $$                                                                
//                   |  $$$$$$/                    |  $$$$$$/                                                                
//                    \______/                      \______/          Copyright © ScienceARK 2016

        public static void Main()
        {
            Initialize(new EventArgs()); //Loads Menu Items & Events
        }

        //Champion Name
        private const string ChampionName = "Nidalee"; //Declare champion name

        //Spells
        public static Spell Q;
        public static Spell Q2;
        public static Spell E;
        public static Spell E2;
        public static Spell W;
        public static Spell W2;
        public static Spell W3;
        public static Spell R;

        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;


        //Main Event
        private static void Initialize(EventArgs args) //Main Event Loader
        {
            if (ObjectManager.Player.ChampionName != "Nidalee") return;

            //Declare Spells
            Q = new Spell(SpellSlot.Q, 1500);
            Q2 = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E, 600);
            E2 = new Spell(SpellSlot.E, 300);
            W = new Spell(SpellSlot.W, 900);
            W2 = new Spell(SpellSlot.W, 720);
            W3 = new Spell(SpellSlot.W, 360);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 40, 1300, true, SkillshotType.SkillshotLine);
            E.SetTargetted(0.25f, float.MaxValue);
            E2.SetSkillshot(0.25f, 300, float.MaxValue, false, SkillshotType.SkillshotCone);
            W.SetTargetted(0.25f, float.MaxValue);
            W2.SetTargetted(0.25f, 3500);
            W3.SetSkillshot(0.25f, 150, 360, false, SkillshotType.SkillshotCone);

            Config = new Menu("MightyNidalee", "MightyNidalee", true).SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange);
            Orbwalker = new Orbwalking.Orbwalker(Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            TargetSelector.AddToMenu(Config.AddSubMenu(new Menu("Target Selector", "Selector")));

            var combat = Config.AddSubMenu(new Menu("Combat Settings", "Combat Settings"));
            var farm = Config.AddSubMenu(new Menu("Farm Settings", "Farm Settings"));
            var lane = farm.AddSubMenu(new Menu("Laneclear Settings", "Laneclear Farm Settings"));
            var jungle = farm.AddSubMenu(new Menu("Jungleclear Settings", "Jungleclear Settings"));
            var misc = Config.AddSubMenu(new Menu("Misc Settings", "Misc Settings"));
            var drawing1 = misc.AddSubMenu(new Menu("Draw Settings", "Draw Settings"));

            combat.AddItem(new MenuItem("blank3", "Nidalee | Human Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.MediumPurple));
            combat.AddItem(new MenuItem("Hitchance", "Select [Q] Hitchance: ", true).SetValue(new StringList(new[]
                {"Low", "Medium", "High", "VeryHigh"}, 3)));
            combat.AddItem(new MenuItem("combat.Q", "Use Q").SetValue(true));
            combat.AddItem(new MenuItem("combat.W", "Use W").SetValue(true));
            combat.AddItem(new MenuItem("combat.E", "Use E").SetValue(true));
            combat.AddItem(new MenuItem("combat.R", "Use R Form Switch").SetValue(true));

            combat.AddItem(new MenuItem("blank4", "Nidalee | Cougar Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            combat.AddItem(new MenuItem("combat.Q2", "Use Q").SetValue(true));
            combat.AddItem(new MenuItem("combat.W2", "Use W").SetValue(true));
            combat.AddItem(new MenuItem("combat.E2", "Use E").SetValue(true));
            combat.AddItem(new MenuItem("combat.R2", "Use R Form Switch").SetValue(true));
            combat.AddItem(new MenuItem("anti.gapcloser", "Use W Anti-Gapcloser").SetValue(true));

            combat.AddItem(new MenuItem("blank5", "Healing Settings [E]").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            combat.AddItem(new MenuItem("hpslider", "Use E if Player Health % is below: ").SetValue(new Slider(70, 0, 100)));
            combat.AddItem(new MenuItem("allyhpslider", "Use E if Ally Health % is below: ").SetValue(new Slider(55, 0, 100)));
            combat.AddItem(new MenuItem("manaslider", "Min. Mana % ").SetValue(new Slider(70, 0, 100)));

            lane.AddItem(new MenuItem("blank3", "Laneclear").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gray));
            lane.AddItem(new MenuItem("blank4", "Nidalee | Cougar Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            lane.AddItem(new MenuItem("lane.Q2", "Use Q").SetValue(true));
            lane.AddItem(new MenuItem("lane.W2", "Use W").SetValue(true));
            lane.AddItem(new MenuItem("lane.E2", "Use E").SetValue(true));
            lane.AddItem(new MenuItem("blank5", "Nidalee | Human Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.MediumPurple));
            lane.AddItem(new MenuItem("lane.Q", "Use Q").SetValue(true));

            jungle.AddItem(new MenuItem("blank3", "Jungleclear").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gray));
            jungle.AddItem(new MenuItem("Jungle.R", "Use Smart R form switching").SetValue(true));
            jungle.AddItem(new MenuItem("blank5", "Nidalee | Cougar Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Orange));
            jungle.AddItem(new MenuItem("Jungle.Q2", "Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("Jungle.W2", "Use W").SetValue(true));
            jungle.AddItem(new MenuItem("Jungle.E2", "Use E").SetValue(true));
            jungle.AddItem(new MenuItem("blank4", "Nidalee | Human Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.MediumPurple));
            jungle.AddItem(new MenuItem("Jungle.Q", "Use Q").SetValue(true));
            jungle.AddItem(new MenuItem("Jungle.W", "Use W").SetValue(true));

            drawing1.AddItem(new MenuItem("blank3", "Nidalee | Human form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.MediumPurple));
            drawing1.AddItem(new MenuItem("Draw.Q", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.Gray)));
            drawing1.AddItem(new MenuItem("Draw.W", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.LightGray)));
            drawing1.AddItem(new MenuItem("Draw.E", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.MediumPurple)));
            drawing1.AddItem(new MenuItem("blank2", "Nidalee | Cougar Form").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.LawnGreen));
            drawing1.AddItem(new MenuItem("Draw.W2", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.DarkOrange)));
            drawing1.AddItem(new MenuItem("Draw.E2", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.Gold)));
            drawing1.AddItem(new MenuItem("blank420", "Misc settings").SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Gray));
            drawing1.AddItem(new MenuItem("Draw.Cooldowns", "Cooldown Drawings").SetValue(true));
            drawing1.AddItem(new MenuItem("print.debug.chat", "DebugMsg to Chat").SetValue(true));
            drawing1.AddItem(new MenuItem("disable.draws", "Disable all Drawings").SetValue(false));
            drawing1.AddItem(new MenuItem("CircleThickness", "Circle Thickness").SetValue(new Slider(0, 30, 0)));

            Config.AddToMainMenu();
            //END

            Printmsg("Mighty Nidalee sucessfully loaded - Patch: " + Game.Version);
            Printmsg("Remember to flame and have fun <3");

            Game.OnUpdate += OrbwalkerModes;
            R_Manager.EventManager();
            Orbwalking.AfterAttack += AfterAA;
            Game.OnUpdate += HealManager;
            AntiGapcloser.OnEnemyGapcloser += AntiGapCloser_OnEnemyGapcloser;
            Drawings.DrawEvent();
            Obj_AI_Base.OnProcessSpellCast += Animations;
        }

        private static void AfterAA(AttackableUnit unit, AttackableUnit target)
        {
            var combo_Q2 = Config.Item("combat.Q2").GetValue<bool>();
            if (R_Manager.CougarForm && combo_Q2) //Check Cougar
            {
                if (Q2.IsReady() && target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    Q2.Cast();
                    Printconsole("Casted Takedown! - Reason: AA Reset");
                }
            }
        }
    

        private static void Animations(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = TargetSelector.GetTarget(W2.Range / 2, TargetSelector.DamageType.Magical);
            if (target == null) return;
            //Cancel Animation
            if (!sender.IsMe) return;
            var spell = args.SData;

            if (spell.Name == "Swipe")
            {
                W2.Cast(target);
            }
        }

        private static void AntiGapCloser_OnEnemyGapcloser(ActiveGapcloser e)
        {
            var gapclose = Config.Item("anti.gapcloser").GetValue<bool>();

            var awayPosition = e.End.Extend(ObjectManager.Player.Position, ObjectManager.Player.Distance(e.End) + W2.Range / 2);
            if (R_Manager.WcougarReady && !R_Manager.CougarForm && e.End.Distance(ObjectManager.Player.Position) <= 150 && gapclose)
                R.Cast();
            if (W2.IsReady() && e.End.Distance(ObjectManager.Player.Position) <= 200)
                W.Cast(awayPosition.To2D());
        }

        private static void HealManager(EventArgs args)
        {
            var hpslider = Config.Item("hpslider").GetValue<Slider>().Value; ;
            var allyhpslider = Config.Item("allyhpslider").GetValue<Slider>().Value; ;
            var manaslider = Config.Item("manaslider").GetValue<Slider>().Value; ;

            if (ObjectManager.Player.ManaPercent < manaslider) return;

            if (!R_Manager.CougarForm)
            {
                if (ObjectManager.Player.HealthPercent < hpslider && E.IsReady())
                    E.Cast(ObjectManager.Player);

                var ally = ObjectManager.Get<Obj_AI_Base>().Where(a => a.IsAlly && !a.IsMinion && a.IsValidTarget(E.Range)).FirstOrDefault();
                if (ally == null) return;
                if (ally.Health < allyhpslider && E.IsReady() && ally != null)
                    E.Cast(ally);
            }
        }


        private static void OrbwalkerModes(EventArgs args)
        {
            //Executes Code based on Orbwalker Mode.
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (!R_Manager.CougarForm)
                        Combo_Human();
                    if (R_Manager.CougarForm)
                        Combo_Cougar();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (!R_Manager.CougarForm)
                        LaneClear_Human();
                    if (R_Manager.CougarForm)
                        LaneClear_Cougar();
                    if (!R_Manager.CougarForm)
                        JungleClear_Human();
                    if (R_Manager.CougarForm)
                        JungleClear_Cougar();
                    break;
            }


        }
        static void JungleClear_Human()
        {
            var Jungle_Q = Config.Item("Jungle.Q").GetValue<bool>();
            var Jungle_W = Config.Item("Jungle.W").GetValue<bool>();
            var Jungle_R = Config.Item("Jungle.R").GetValue<bool>();

            var QLocation = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => !m.IsDead && m.Distance(ObjectManager.Player.Position) < W2.Range).ToList();

            var Qminion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(m =>  !m.IsDead && m.Distance(ObjectManager.Player.Position) < W2.Range).OrderBy(m => m.MaxHealth).LastOrDefault();

            if (Qminion == null) return;

            if (Q.IsReady() && Jungle_Q)
                Q.Cast(Qminion);

            if (W.IsReady() && Jungle_W && Qminion.HealthPercent > 35 && !Qminion.IsMoving)
                W.Cast(Qminion);

            if (ObjectManager.Player.HealthPercent < 85 && E.IsReady())
                E.Cast(ObjectManager.Player);

            if (ObjectManager.Player.HealthPercent < 85 && E.IsReady())
                return;

            if (R.IsReady() && R_Manager.IsHunted(Qminion) && !Q.IsReady())
                R.Cast();

            if (R.IsReady() && !R_Manager.IsHunted(Qminion) && !Q.IsReady())
                R.Cast();

            var killableminions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.Health < 1);
            killableminions.FirstOrDefault();
            var closestminion = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Distance(ObjectManager.Player.Position)).FirstOrDefault();
        }

        static void JungleClear_Cougar()
        {
            var jungle_Q2 = Config.Item("Jungle.Q2").GetValue<bool>();
            var jungle_W2 = Config.Item("Jungle.W2").GetValue<bool>();
            var jungle_E2 = Config.Item("Jungle.E2").GetValue<bool>();
            var Jungle_R = Config.Item("Jungle.R").GetValue<bool>();

            var Qminion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => !m.IsDead && m.Distance(ObjectManager.Player.Position) < W2.Range).OrderBy(m => m.MaxHealth).LastOrDefault();

            var Minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => !m.IsDead);

            var GetKillableMinions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral).Where(m => !m.IsDead && m != null && m.Distance(ObjectManager.Player.Position) < ObjectManager.Player.AttackRange + m.BoundingRadius && m.Health < ObjectManager.Player.GetAutoAttackDamage(m));

            if (Q2.IsReady() && jungle_Q2)
            {
                foreach (var min in Minions.Where(m => m.Distance(ObjectManager.Player.Position) <= ObjectManager.Player.AttackRange + m.BoundingRadius && m != null))
                {
                    if (min == null) return;
                    if (min.Health < Damages.Cougar_Q_Damage(min) && min.Health > ObjectManager.Player.GetAutoAttackDamage(min))
                    {
                        Q2.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, min);
                    }
                    if (!W.IsReady() && !E.IsReady())
                    {
                        Q2.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, min);
                    }

                }
            }
            if (W2.IsReady() && jungle_W2)
            {
                if (Qminion.IsValidTarget(W2.Range / 2))
                    W2.Cast(Qminion);

                if (R_Manager.IsHunted(Qminion) && Qminion.IsValidTarget(W2.Range))
                    W2.Cast(Qminion);
            }

            if (E2.IsReady() && jungle_E2)
            {
                if (E2.IsReady() && Qminion.IsValidTarget(E2.Range))
                    E2.Cast(Qminion);
            }

            if (R.IsReady() && Jungle_R)
            {
                if (W2.IsReady() && Qminion.IsValidTarget(W2.Range / 2)
                    || E2.IsReady() && Qminion.IsValidTarget(E2.Range)
                    || Q.IsReady() && Qminion.IsValidTarget(ObjectManager.Player.AttackRange)
                    || R_Manager.IsHunted(Qminion) && Qminion.IsValidTarget(W2.Range) && W.IsReady())
                    return;

                var qpred = Q.GetPrediction(Qminion);

                if (R_Manager.QhumanReady && Qminion.IsValidTarget(Q.Range))
                    R.Cast();
            }
        }

        static void LaneClear_Human()
        {
            var lane_Q = Config.Item("lane.Q").GetValue<bool>();
            var QLocation = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).OrderBy(m => m.Health).Where(m => m.IsEnemy
            && !m.IsDead && m.IsMinion && m.Distance(ObjectManager.Player.Position) < Q.Range).ToList();

            if (Q2.IsReady() && lane_Q)
            {
                foreach (var min in QLocation)
                {
                    if (min == null) return;

                    if (Q.IsReady())
                        Q.Cast(min);
                }
            }
        }
        static void LaneClear_Cougar()
        {
            var lane_Q2 = Config.Item("lane.Q2").GetValue<bool>();
            var lane_W2 = Config.Item("lane.W2").GetValue<bool>();
            var lane_E2 = Config.Item("lane.E2").GetValue<bool>();


            var ELocation = MinionManager.GetMinions(E2.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsEnemy
            && !m.IsDead && m.IsMinion && m.Distance(ObjectManager.Player.Position) < E2.Range);
            var EfarmPred = E2.GetLineFarmLocation(new List<Obj_AI_Base>(ELocation), E2.Width);

            var WLocation = MinionManager.GetMinions(W2.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsEnemy
            && !m.IsDead && m.IsMinion && ObjectManager.Player.Distance(m.Position) > 100 && m.IsValidTarget(W3.Range));
            var WfarmPred = W2.GetLineFarmLocation(new List<Obj_AI_Base>(WLocation), W3.Width);

            var Minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).Where(m => m.IsMinion && !m.IsDead && m.IsEnemy);
            var GetKillableMinions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).Where(m =>
            m.IsMinion && m.IsEnemy && !m.IsDead && m.Distance(ObjectManager.Player.Position) < ObjectManager.Player.AttackRange + m.BoundingRadius && m.Health < ObjectManager.Player.GetAutoAttackDamage(m));


            if (Q2.IsReady() && lane_Q2)
            {
                foreach (var min in Minions.Where(m => m.Distance(ObjectManager.Player.Position) <= ObjectManager.Player.AttackRange + m.BoundingRadius))
                {
                    if (min.Health < Damages.Cougar_Q_Damage(min) && min.Health > ObjectManager.Player.GetAutoAttackDamage(min))
                    {
                        Q2.Cast();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, min);
                    }

                }
            }
            if (W3.IsReady() && lane_W2)
            {
                if (WfarmPred.MinionsHit >= 2)
                    W3.Cast(WfarmPred.Position);

            }
            if (E2.IsReady() && lane_E2)
            {
                    if (ELocation.Count() >= 1)
                    {
                        E.Cast(EfarmPred.Position);
                    }

                
            }
        }
        //Main Combo Logic
        static void Combo_Human()
        {
            //Main Target Selector, Use Spell with Highest Range for Range.
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid) return; //Important! Without checking if target is null it will spamm console errors.


            var combo_Q = Config.Item("combat.Q").GetValue<bool>();
            var combo_W = Config.Item("combat.W").GetValue<bool>();
            var combo_E = Config.Item("combat.E").GetValue<bool>();
            var combo_R = Config.Item("combat.R").GetValue<bool>();

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && combo_Q)
            {
                var qprediction = Q.GetPrediction(target);
                if (Q.IsReady() && qprediction.Hitchance >= QPrediction())
                {
                    Q.Cast(target);
                    Printconsole("Human [Q] Casted! Reason: Prediction equals Hitchance");
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && combo_E)
            {
                if (ObjectManager.Player.HealthPercent < 50)
                    E.Cast(ObjectManager.Player);
            }

            if (R.IsReady() && target.IsValidTarget(W2.Range) && combo_R)
            {
                var qpred = Q.GetPrediction(target);

                if (ObjectManager.Player.HealthPercent < 90 && E.IsReady())
                    E.Cast(ObjectManager.Player);

                if (ObjectManager.Player.HealthPercent < 90 && E.IsReady())
                    return;

                if (R_Manager.IsHunted(target) && target.IsValidTarget(W2.Range) && W.Level >= 1)
                {
                    R.Cast();
                    Printconsole("Switched to Cougar, Reason: W! - HUNTED");
                }
                if (!R_Manager.IsHunted(target) && R_Manager.WcougarReady && target.IsValidTarget(W2.Range / 2 + target.BoundingRadius) && !Q.IsReady() && W.Level >= 1)
                {
                    R.Cast();
                    Printconsole("Switched to Cougar, Reason: W! - Q AINT READY");
                }
                if (!R_Manager.IsHunted(target) && R_Manager.WcougarReady && target.IsValidTarget(W2.Range / 2 + target.BoundingRadius) && W.Level >= 1)
                {
                    R.Cast();
                    Printconsole("Switched to Cougar, Reason: W! - Q CAN'T HIT");
                }
                if (W.Level < 1 && ObjectManager.Player.Distance(target.Position) < 150 && !Q.IsReady())
                    R.Cast();
            }
        }

        static void Combo_Cougar()
        {
            //Main Target Selector, Use Spell with Highest Range for Range.
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValid) return; //Important! Without checking if target is null it will spamm console errors.


            var combo_Q = Config.Item("combat.Q2").GetValue<bool>();
            var combo_W = Config.Item("combat.W2").GetValue<bool>();
            var combo_E = Config.Item("combat.E2").GetValue<bool>();
            var combo_R = Config.Item("combat.R2").GetValue<bool>();

            if (Q2.IsReady() && target.IsValidTarget(ObjectManager.Player.AttackRange + 50) && combo_Q && !W.IsReady() && !E.IsReady())
            {
                if (W.IsReady() && target.IsValidTarget(W2.Range / 2) && ObjectManager.Player.Distance(target) > 70 && Damages.Cougar_W_Damage(target) > target.Health && combo_W) return;
                if (E.IsReady() && target.IsValidTarget(E2.Range - 25) && combo_E && target.Health < Damages.Cougar_E_Damage(target)) return;

                if (Q2.IsReady() && target.Health < Damages.Cougar_Q_Damage(target))
                {
                    Q2.Cast();
                    Printconsole("Casted Takedown! - Reason: Targets gonna die yo");
                }
                if (Q2.IsReady() && !W.IsReady() && !E.IsReady())
                {
                    Q2.Cast();
                    Printconsole("Casted Takedown! - Reason: No Spells CD's left");
                }
            }


            if (target.IsValidTarget(W2.Range) && combo_W)
            {
                if (Q2.IsReady() && target.Health < Damages.Cougar_Q_Damage(target) && target.IsValidTarget(ObjectManager.Player.AttackRange))
                    return;
                if (R_Manager.IsHunted(target) && target.IsValidTarget(W2.Range))
                {
                    W2.Cast(target);
                    Printconsole("Casted Pounce! - Reason: Target is Hunted and in Pounce [W] Range");
                }
                if (target.IsValidTarget(W2.Range / 2) && !R_Manager.IsHunted(target) && ObjectManager.Player.Distance(target) > 150)
                {
                    W2.Cast(target);
                    Printconsole("Casted Pounce! - Reason: in Pounce [W] Range");
                }
                if (target.Health > Damages.Cougar_W_Damage(target) 
                    && target.Health < Damages.Cougar_W_Damage(target) + Damages.Cougar_Q_Damage(target) 
                    && R_Manager.IsHunted(target) && Q.IsReady() && W.IsReady() && combo_Q && combo_W && ObjectManager.Player.Distance(target) > 150)
                {
                    W2.Cast(target);
                    Printconsole("Casted Pounce! - Reason: in Pounce [W+Q Combo]");
                }
            }

            if (E2.IsReady() && target.IsValidTarget(E2.Range - 25) && combo_E)
            {
                if (E2.IsReady() && !W.IsReady())
                {
                    E2.Cast(target);
                    Printconsole("Casted Swipe! - Reason: Target is Valid and in [E] Range");
                }
                if (target.Health >
                    Damages.Cougar_E_Damage(target) && target.Health < Damages.Cougar_E_Damage(target) + Damages.Cougar_Q_Damage(target)
                    && Q.IsReady() && E.IsReady() && combo_Q && combo_E)
                {
                    E2.Cast(target);
                    Printconsole("Casted Swipe! Reason: [E/Q] Combo");
                }
            }

            if (R.IsReady() && target.IsValidTarget(R.Range) && combo_R)
            {
                var qprediction = Q.GetPrediction(target);
                if (R_Manager.QhumanReady && qprediction.Hitchance >= QPrediction())
                {
                    if (W.IsReady() && R_Manager.IsHunted(target) && target.IsValidTarget(W2.Range)
                        || Q.IsReady() && ObjectManager.Player.HasBuff("Takedown")  && target.IsValidTarget(ObjectManager.Player.AttackRange + 20)
                        || E2.IsReady() && target.Health < Damages.Cougar_E_Damage(target) && target.IsValidTarget(E2.Range - 25))
                        return;
                    R.Cast();
                    Printconsole("Switched to Human Form! - Reason: Q is Ready && Prediction equals hitchance");

                    if (!W.IsReady() && !Q.IsReady() && !E.IsReady() && ObjectManager.Player.HealthPercent < 35)
                        R.Cast();
                }
            }
        }

        public static void Printmsg(string message)
        {
            Chat.Print(
                "<font color='#00ff00'>[Mighty Nidalee]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        public static HitChance QPrediction()
        {
            switch (Config.Item("Hitchance", true).GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Low;
                case 1:
                    return HitChance.Medium;
                case 2:
                    return HitChance.High;
                case 3:
                    return HitChance.VeryHigh;
            }
            return HitChance.High;
        }
        public static void Printconsole(string message)
        {
            var debugprint = Config.Item("print.debug.chat").GetValue<bool>();

            if (!debugprint)
                return;
            Chat.Print(
                "<font color='#FFB90F'>[Console]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        }
    }



