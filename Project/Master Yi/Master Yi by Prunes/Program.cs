using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace MasterYiByPrunes
{
    class Program
    {
        public const string ChampName = "MasterYi";
        public static Orbwalking.Orbwalker Orbwalker;  //change to Orbwalker and delete the LXorbwalker line
        //public static LxOrbwalker Orbwalker = new LxOrbwalker(); // to switch back to default orb

        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        private static Items.Item tiamatItem, hydraItem, botrkItem, bilgeItem, randuinsItem, GhostbladeItem;
        public static readonly int[] RedMachete = { 3715, 3718, 3717, 3716, 3714 };
        public static readonly int[] BlueMachete = { 3706, 3710, 3709, 3708, 3707 };
        public static readonly string[] SmiteNames = {"s5_summonersmiteplayerganker", "s5_summonersmiteduel"};
        public static readonly string[] DodgeSpells = { "infiniteduresschannel", "InfiniteDuress", "Dazzle", "Terrify", "PantheonQ", "dariusexecute", "runeprison", "goldcardpreattack", "zedult", "ViR", "VayneCondemn", "KatarinaQ", "SyndraR", "blindingdart", "ireliaequilibriumstrike", "maokaiunstablegrowth", "Disintegrate", 
"VeigarPrimordialBurst", "FioraDance", "NasusQAttack"};
        public static Menu Config;
        public static SpellSlot smiteSlot = SpellSlot.Unknown;
        public static Spell smite;
        public static List<Spell> SpellList = new List<Spell>();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            Console.Write("loaded");
        }

        static void Game_OnGameLoad(EventArgs args)
        {

            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 600);
            Q.SetTargetted(0.5f, 2000);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            bilgeItem= new Items.Item(3144, 475f);
            botrkItem = new Items.Item(3153, 425f);
            hydraItem = new Items.Item(3074, 250f);
            tiamatItem = new Items.Item(3077, 250f);
            randuinsItem = new Items.Item(3143, 490f);
            GhostbladeItem = new Items.Item(3142, 590f);


            Config = new Menu("Prunes" + ChampName, ChampName, true);
            var ts = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(ts);
          //  SimpleTs.AddToMenu(ts);
            Config.AddSubMenu(ts);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));   
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));  
           // var lxMenu = new Menu("LX_Orbwalker", "LXOrb");  //lx
            //LxOrbwalker.AddToMenu(lxMenu);
            //Config.AddSubMenu(lxMenu); //lx
            Config.AddSubMenu(new Menu("Combo", "Combo"));

            Config.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useSmite", "Use smite in combo?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("smartQ", "Save Q for dodging/gapclose?").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Qks", "KS with Q?").SetValue(true));


           // Config.SubMenu("Combo").AddItem(new MenuItem("MSdiff", "Movespeed difference for Q").SetValue(new Slider(50, 0, 200)));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            //Config.SubMenu("Combo").AddItem( new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
            Config.SubMenu("Combo").AddItem(new MenuItem("Combo2", "Combo Without Magnet").SetValue(new KeyBind(67, KeyBindType.Press)));

            Config.AddToMainMenu();

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q Range").SetValue(new Circle(true, Color.FromArgb(255, 255, 255, 255))));
            SmiteSlot();


            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;

            Chat.Print("<font color='#00FFFF'>Master Yi</font><font color='#FFFFFF'> By Prunes</font>");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Config.Item("Combo2").GetValue<KeyBind>().Active)
            {
                Combo2();
            }


        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var menuItem = Config.Item(Q.Slot + "Range").GetValue<Circle>();
            if (menuItem.Active)
            {
                if (Q.IsReady())       
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Green);
                else           
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.Red);
            }
        }


        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var target2 = TargetSelector.GetTarget(Q.Range/2, TargetSelector.DamageType.Physical);

            if (target == null) return;
            if (R.IsReady() && Config.Item("useR").GetValue<bool>())
            {
                
                R.Cast();
            }
            
            if (target.IsValidTarget(Q.Range) &&
                ObjectManager.Player.Spellbook.CanUseSpell((smiteSlot)) == SpellState.Ready && (smitetype() == "s5_summonersmiteplayerganker" || smitetype() == "s5_summonersmiteduel") && Config.Item("useSmite").GetValue<bool>())
            {
                SmiteSlot();
                ObjectManager.Player.Spellbook.CastSpell(smiteSlot, target);
            }
             

            if (Q.IsReady() && Config.Item("useQ").GetValue<bool>())
            {
                Qlogic();
            }
            if (E.IsReady() && Config.Item("useE").GetValue<bool>() && Orbwalking.InAutoAttackRange(target))
            {
                E.Cast();
            }    
            else if (W.IsReady() && Orbwalking.InAutoAttackRange(target) && Config.Item("useW").GetValue<bool>())
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                LeagueSharp.Common.Utility.DelayAction.Add(400, () => W.Cast());
                W.Cast();
                EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                Orbwalking.ResetAutoAttackTimer();
            }

            if (tiamatItem.IsReady() && target.IsValidTarget(tiamatItem.Range))
            {
                tiamatItem.Cast();
            }
            if (hydraItem.IsReady() && target.IsValidTarget(tiamatItem.Range))
            {
                hydraItem.Cast();
            }
            if (botrkItem.IsReady() && target.IsValidTarget(botrkItem.Range))
            {
                botrkItem.Cast(target);
            }
            if (bilgeItem.IsReady() && target.IsValidTarget(bilgeItem.Range))
            {
                bilgeItem.Cast(target);
            }
            if (GhostbladeItem.IsReady() && target.IsValidTarget(Q.Range))
            {
                GhostbladeItem.Cast();
            }
            if (randuinsItem.IsReady() && target.IsValidTarget(randuinsItem.Range))
            {
                randuinsItem.Cast();
            }

              //this was magnet, no need with new obwalker 
                
            else if (target2.IsEnemy && target2.IsValidTarget() && !target2.IsMinion && !Player.Spellbook.IsAutoAttacking && !Orbwalking.InAutoAttackRange(target2))
             {
                   EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target2);

             LeagueSharp.Common.Utility.DelayAction.Add(400, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target2));
             EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target2);
             }
                 
        }

        public static void Combo2()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
           // var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;
            var dragon = ObjectManager.Get<Obj_AI_Minion>().First(it => it.IsValidTarget(Q.Range));

            if (dragon.Spellbook.IsCastingSpell)
            {
                Q.CastOnUnit(dragon);
            }




            if (bilgeItem.IsReady() && target.IsValidTarget(bilgeItem.Range))
            {
                bilgeItem.Cast(target);
            }
            if (GhostbladeItem.IsReady() && target.IsValidTarget(Q.Range))
            {
                GhostbladeItem.Cast();
            }
            if (target.IsValidTarget(Q.Range) && R.IsReady() && Config.Item("useR").GetValue<bool>())
            {
                R.Cast();
            }
            if (target.IsValidTarget(Q.Range) && Q.IsReady() && Config.Item("useQ").GetValue<bool>())
            {
               Qlogic();
            }
            if (target.IsValidTarget(Q.Range) && E.IsReady() && Config.Item("useE").GetValue<bool>())
            {
                E.Cast();
            }
            else if (target.IsValidTarget(Q.Range) && W.IsReady() && Orbwalking.InAutoAttackRange(target) && Config.Item("useW").GetValue<bool>())
            {
                 EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                LeagueSharp.Common.Utility.DelayAction.Add(350, () => W.Cast());
                 EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                Orbwalking.ResetAutoAttackTimer();
            }
            if (tiamatItem.IsReady() && target.IsValidTarget(tiamatItem.Range))
            {
                tiamatItem.Cast();
            }
            if (hydraItem.IsReady() && target.IsValidTarget(tiamatItem.Range))
            {
                hydraItem.Cast();
            }
            if (botrkItem.IsReady() && target.IsValidTarget(botrkItem.Range))
            {
                botrkItem.Cast(target);
            }

            if (randuinsItem.IsReady() && target.IsValidTarget(randuinsItem.Range))
            {
                randuinsItem.Cast();
            }
        }


        public static void Qlogic()
        {
          //  var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Q.GetDamage(target) >= target.Health && Config.Item("Qks").GetValue<bool>())
            {
                Q.CastOnUnit(target);
            }
            
            if ((Player.MoveSpeed - target.MoveSpeed) < 50 && target.IsMoving && Config.Item("smartQ").GetValue<bool>())
            {
                
                Q.CastOnUnit(target);
            }
            if ((target.IsDashing() || target.LastCastedSpellName() == "SummonerFlash") && Config.Item("smartQ").GetValue<bool>())
            {
                Q.CastOnUnit(target);
            }
            if (Player.Health < Player.MaxHealth / 4 && Config.Item("smartQ").GetValue<bool>())
            {
                Q.CastOnUnit(target);
            }
            if (!Config.Item("smartQ").GetValue<bool>())
            {
                Q.CastOnUnit(target);
                
            }
        }

        public static void Qtarget(string SpellCasted, string champname)
        {
        var minion = ObjectManager.Get<Obj_AI_Minion>().First(it => it.IsValidTarget(Q.Range));
       // var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Physical);
        var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
        var anychamp = ObjectManager.Get<AIHeroClient>().LastOrDefault(it => it.IsValidTarget(Q.Range));
            if (target.IsValidTarget(Q.Range))
            {
                Console.WriteLine(SpellCasted);
                Q.Cast(target);
            }
            if (minion.IsValidTarget(Q.Range) && SpellCasted == "zedult")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(400, () => Q.CastOnUnit(minion));
            }
            if (minion.IsValidTarget(Q.Range) && SpellCasted == "DragonFireBall")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(3340, () => Q.CastOnUnit(minion));
            }
            else if (anychamp.IsValidTarget(Q.Range) && SpellCasted == "zedult")
            {
                LeagueSharp.Common.Utility.DelayAction.Add(400, () => Q.CastOnUnit(anychamp));
            }

        }

        public static void WWsmited()
        {
          //  var longtarg = SimpleTs.GetTarget(1000, SimpleTs.DamageType.Physical);
            var longtarg = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Physical);
            var miniontarg = ObjectManager.Get<Obj_AI_Minion>().First(it => it.IsValidTarget(Q.Range));
            Console.WriteLine("WW SMITED");
            Console.WriteLine("Smiter: " + longtarg.BaseSkinName);
            if (longtarg.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(longtarg);
            }
            else
            {
                Q.CastOnUnit(miniontarg, true);
                LeagueSharp.Common.Utility.DelayAction.Add(200, () => Q.CastOnUnit(miniontarg));
            }
        }
        
        
        static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender is Obj_SpellMissile)
            {
                var missile = sender as Obj_SpellMissile;
                if (missile.SpellCaster is Obj_AI_Minion && missile.Target.IsMe)
                {
                   //Console.WriteLine( "Spellcaster: " + missile.SpellCaster); 
                   Console.WriteLine("minion attacking");
                }
            }
        }
        
        public static void OnProcessSpellCast(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (arg.Target.IsMe && obj is AIHeroClient && DodgeSpells.Any(arg.SData.Name.Equals))
            {
                Qtarget(arg.SData.Name, obj.BaseSkinName);
                Console.WriteLine(arg.SData.Name);
            }
            else if (arg.Target.IsMe && obj.BaseSkinName == "Warwick" && SmiteNames.Any(arg.SData.Name.Equals))
            {

                WWsmited();
            }
            else if (arg.Target.IsMe && arg.SData.Name == "DragonFireBall")
            {
                Console.WriteLine(arg.SData.Name);
               // Qtarget(arg.SData.Name, obj.BaseSkinName);
            }

        }



        public static string smitetype()
        {

            if (BlueMachete.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteplayerganker";
            }

            if (RedMachete.Any(id => Items.HasItem(id)))
            {
                return "s5_summonersmiteduel";
            }
            return "summonersmite";
        }


        public static void SmiteSlot()
        {
            foreach (var spell in ObjectManager.Player.Spellbook.Spells.Where(spell => String.Equals(spell.Name, smitetype(), StringComparison.CurrentCultureIgnoreCase)))
            {
                smiteSlot = spell.Slot;
                smite = new Spell(smiteSlot, 700);
                
                return;
            }
        }
    }



}
