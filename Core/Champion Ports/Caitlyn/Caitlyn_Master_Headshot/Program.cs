using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX; 
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;

using EloBuddy; 
using LeagueSharp.Common; 
namespace Caitlyn_Master_Headshot
{
    class Program
    {
        // Variable
        public static Menu myMenu;
        public static AIHeroClient myHero;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static float lastTrap;
        private static Dictionary<int, GameObject> trapDict = new Dictionary<int, GameObject>();

        // Loader
        public static void Main()
        {
            OnLoad(new EventArgs()); 
        }

        private static void OnLoad(EventArgs args)
        {       
            initVariable();

            if (myHero.ChampionName != "Caitlyn")
                return;

            Chat.Print("<font color=\"#FF001E\">Caitlyn Master Headshot- </font><font color=\"#FF980F\"> Loaded</font>");
            initMenu();

            GameObject.OnCreate += Obj_AI_Base_OnCreate;
            GameObject.OnDelete += Obj_AI_Base_OnDelete;
            Game.OnUpdate += Update;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnBuffGain += OnBuffAdd;
            Drawing.OnDraw += OnDraw;
        }

        private static void initVariable()
        {
            Q = new Spell(SpellSlot.Q, true);
            Q.Range = Q.Range - 50;
            W = new Spell(SpellSlot.W, true);
            E = new Spell(SpellSlot.E, true);
            E.Range = E.Range - 50;
            R = new Spell(SpellSlot.R, true);

            lastTrap = Game.Time;
            myHero = ObjectManager.Player;
        }

        private static void initMenu()
        {
            myMenu = new Menu("Caitlyn - Master Headshot", "CaitlynMasterHeadshot", true);

            myMenu.AddSubMenu(new Menu("Combo Settings", "Combo"));;
                myMenu.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use (Q)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("qHitChance", "(Q) Hit Chance").SetValue(new Slider(3, 3, 6)));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("infoW", ""));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("useW", "Use (W)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("wDelay", "Delay Between Each Trap (ms)").SetValue(new Slider(1500, 0, 3000)));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("wHitChance", "(W) Hit Chance").SetValue(new Slider(5, 3, 6)));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("infoE", ""));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("useE", "Use (E)").SetValue(true));
                myMenu.SubMenu("Combo").AddItem(new MenuItem("eHitChance", "(E) Hit Chance").SetValue(new Slider(5, 3, 6)));

            myMenu.AddSubMenu(new Menu("Harass Settings", "Harass")); ;
                myMenu.SubMenu("Harass").AddItem(new MenuItem("useQ.Harass", "Use (Q)").SetValue(true));
                myMenu.SubMenu("Harass").AddItem(new MenuItem("qHitChance.Harass", "(Q) Hit Chance").SetValue(new Slider(5, 3, 6)));
                myMenu.SubMenu("Harass").AddItem(new MenuItem("qMana", "Mana Manger").SetValue(new Slider(80, 0, 100)));

            myMenu.AddSubMenu(new Menu("Ultimate Settings", "R"));
                myMenu.SubMenu("R").AddItem(new MenuItem("useR", "Auto Use (R)").SetValue(true));
                myMenu.SubMenu("R").AddItem(new MenuItem("rCombo", "Use while Combo").SetValue(true));

            myMenu.AddSubMenu(new Menu("Trap Settings", "Trap"));
                myMenu.SubMenu("Trap").AddItem(new MenuItem("autoW", "Auto (W)").SetValue(true));

            myMenu.AddSubMenu(new Menu("Anti GapCloser Settings", "AntiGapCloser"));
                myMenu.SubMenu("AntiGapCloser").AddItem(new MenuItem("AntiGapCloser", "Auto (E) AntiGapCloser").SetValue(true));

            myMenu.AddSubMenu(new Menu("KillSteal Settings", "KillSteal"));
                myMenu.SubMenu("KillSteal").AddItem(new MenuItem("qKillSteal", "Use (Q)").SetValue(true));
                myMenu.SubMenu("KillSteal").AddItem(new MenuItem("qKillSteal.Hitchance", "(Q) Hit Chance").SetValue(new Slider(3, 3, 6)));

            myMenu.AddSubMenu(new Menu("Drawing Settings", "Draw"));
                myMenu.SubMenu("Draw").AddItem(new MenuItem("qRange", "Draw (Q)").SetValue(new Circle(true, Color.AliceBlue)));
                myMenu.SubMenu("Draw").AddItem(new MenuItem("wRange", "Draw (W)").SetValue(new Circle(true, Color.Aqua)));
                myMenu.SubMenu("Draw").AddItem(new MenuItem("eRange", "Draw (E)").SetValue(new Circle(true, Color.Aquamarine)));

            myMenu.AddSubMenu(new Menu("Orbwalk Settings", "InitOrbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(myMenu.SubMenu("InitOrbwalker"));

            myMenu.AddItem(new MenuItem("void", ""));
            myMenu.AddItem(new MenuItem("author", "Author: Rambe"));

            myMenu.AddToMainMenu();
        }

        // Callback
        private static void OnDraw(EventArgs args)
        {
            if (myHero == null)
                return;
            if (myHero.Position.IsOnScreen() && Q.IsReady() && myMenu.Item("qRange").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, myMenu.Item("qRange").GetValue<Circle>().Color);

            if (myHero.Position.IsOnScreen() && W.IsReady() && myMenu.Item("wRange").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, myMenu.Item("wRange").GetValue<Circle>().Color);

            if (myHero.Position.IsOnScreen() && E.IsReady() && myMenu.Item("eRange").GetValue<Circle>().Active)
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, myMenu.Item("eRange").GetValue<Circle>().Color);

        }

        private static void Update(EventArgs args)
        {
            if (myHero == null || myHero.IsDead)
                return;

            if (myMenu.Item("useR").GetValue<Boolean>())
                autoCastR();

            if (myMenu.Item("qKillSteal").GetValue<Boolean>())
                qKillSteal();

                switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }            
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Q.IsReady() && sender.IsMe && (int)Args.Slot == 49)
            {
                var Target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
                if (ValidTarget(Target) && Target.NetworkId == Args.Target.NetworkId)
                {
                    PredictionOutput qPred = Q.GetPrediction(Target);
                    if ((int)qPred.Hitchance > myMenu.Item("qHitChance").GetValue<Slider>().Value)
                        Q.Cast(qPred.CastPosition);
                }
            }
        }

        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!myMenu.Item("AntiGapCloser").GetValue<bool>())
                return;

            if (E.IsReady())
            {
                PredictionOutput ePred = E.GetPrediction(gapcloser.Sender);
                E.Cast(ePred.CastPosition);
            }
        }

        private static void Obj_AI_Base_OnCreate(GameObject obj, EventArgs args)
        {
            if (obj.IsAlly && obj.Name == "Cupcake Trap")
            {
                trapDict.Add(obj.NetworkId, obj);
            }
               
        }

        private static void Obj_AI_Base_OnDelete(GameObject obj, EventArgs args)
        {
            if (obj.IsAlly && obj.Name == "Cupcake Trap")
            {
                if (trapDict.ContainsKey(obj.NetworkId))
                    trapDict.Remove(obj.NetworkId);
            }
        }

        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs Args)
        {
            if (!myMenu.Item("autoW").GetValue<Boolean>())
                return;

            if (W.IsReady() && sender.IsEnemy && sender.IsChampion() && sender.Distance(myHero) < W.Range)
            {
                if (Args.Buff.Type == BuffType.Stun || Args.Buff.Type == BuffType.Taunt || Args.Buff.Type == BuffType.Knockup || Args.Buff.Type == BuffType.Snare)
                    W.Cast(sender.Position);
            }
        }

        // Utility
        private static bool ValidTarget(AIHeroClient unit)
        {
            return !(unit == null) && unit.IsValid && unit.IsTargetable && !unit.IsInvulnerable;
        }

        private static int IsTrapNear(Vector3 Position, int Range)
        {
            int trapNear = 0;
            foreach (var trap in trapDict)
            {
                if (Position.Distance(trap.Value.Position) < Range)
                    trapNear++;
            }           

            return trapNear;
        }

        private static int CountEnemyNear(Vector3 From, int Range)
        {
            int enemyNear = 0;
            foreach(var unit in HeroManager.Enemies)
            {
                if (From.Distance(unit.Position) < 500)
                    enemyNear++;
            }
            return enemyNear;
        }

        private static AIHeroClient GetTarget()
        {
            AIHeroClient Target = TargetSelector.GetTarget(myHero.AttackRange, TargetSelector.DamageType.Physical);
            if(!ValidTarget(Target))
                Target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);

            return Target;
        }

        // Combo
        private static void Combo()
        {
            var Target = GetTarget();

            if (!ValidTarget(Target))
                return;

            wCastCombo(Target);
            eCastCombo(Target);      
        }

        private static void wCastCombo(Obj_AI_Base Target)
        {
            if (myMenu.Item("useW").GetValue<Boolean>() && W.IsReady() && Game.Time - lastTrap > (myMenu.Item("wDelay").GetValue<Slider>().Value)/1000)
            {
                PredictionOutput wPred = W.GetPrediction(Target);
                if (IsTrapNear(wPred.CastPosition, 100) == 0 && (int)wPred.Hitchance >= myMenu.Item("wHitChance").GetValue<Slider>().Value)
                {
                    W.Cast(wPred.CastPosition);
                    lastTrap = Game.Time;
                }
            }

        }

        private static void eCastCombo(Obj_AI_Base Target)
        {
            if (myMenu.Item("useE").GetValue<Boolean>() && E.IsReady())
            {
                PredictionOutput ePred = E.GetPrediction(Target);
                               
                if ((int)ePred.Hitchance >= myMenu.Item("eHitChance").GetValue<Slider>().Value)
                    E.Cast(ePred.CastPosition);
            }
        }

        // Harass
        private static void Harass()
        {
            var Target = GetTarget();

            if (!ValidTarget(Target))
                return;

            qHarass(Target);
        }

        private static void qHarass(AIHeroClient unit)
        {
            if (Q.IsReady() && myMenu.Item("useQ.Harass").GetValue<Boolean>() && myHero.ManaPercent > myMenu.Item("qMana").GetValue<Slider>().Value)
            {
                PredictionOutput qPred = Q.GetPrediction(unit);
                if ((int)qPred.Hitchance >= myMenu.Item("qHitChance.Harass").GetValue<Slider>().Value)
                    Q.Cast(qPred.CastPosition);
            }
        }

        // Misc
        private static void autoCastR()
        {
            if (!R.IsReady() || (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && !myMenu.Item("rCombo").GetValue<Boolean>()))
                return;

            foreach (var unit in HeroManager.Enemies)
            {
                if (ValidTarget(unit) && myHero.Distance(unit) > (myHero.AttackRange+500) && R.GetDamage(unit, 0) > unit.Health && CountEnemyNear(myHero.Position, 1500) == 0)
                {
                    PredictionInput predInput = new PredictionInput { From = myHero.Position, Radius = 1500, Range = 3000 };
                    predInput.CollisionObjects[0] = CollisionableObjects.YasuoWall;
                    predInput.CollisionObjects[1] = CollisionableObjects.Heroes;
               
                    IEnumerable<Obj_AI_Base> rCol = Collision.GetCollision(new List<Vector3> { unit.Position }, predInput).ToArray();
                    IEnumerable<Obj_AI_Base> rObjCol = rCol as Obj_AI_Base[] ?? rCol.ToArray();

                    if (rObjCol.Count() == 0 && CountEnemyNear(unit.Position, 1000) == 1)
                        R.Cast(unit);
                }
            }
        }

        private static void qKillSteal()
        {
            foreach (var unit in HeroManager.Enemies)
            {
                if (ValidTarget(unit) && Q.GetDamage(unit, 0) > unit.Health && CountEnemyNear(myHero.Position, (int)myHero.AttackRange) == 0)
                {
                    PredictionOutput qPred = Q.GetPrediction(unit);
                    if ((int)qPred.Hitchance >= myMenu.Item("qKillSteal.Hitchance").GetValue<Slider>().Value && myHero.Distance(qPred.CastPosition) > (myHero.AttackRange + 100) && unit.MoveSpeed >= myHero.MoveSpeed)
                        Q.Cast(qPred.CastPosition);
                }
            }
        }
    }
}
