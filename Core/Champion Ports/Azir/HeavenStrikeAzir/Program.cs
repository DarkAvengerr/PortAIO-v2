using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using EloBuddy;

namespace HeavenStrikeAzir
{
    class Program
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker _orbwalker;

        public static Spell _q, _w, _e, _r , _q2, _r2;

        public static Menu _menu;

        public static int qcount,ecount;
        public static bool Eisready { get { return Player.Mana >= _e.Instance.SData.Mana && Utils.GameTimeTickCount - ecount >= _e.Instance.Cooldown * 1000f; } }

        public static string
            drawQ = "Draw Q", drawW = "Draw W", drawQE = "Draw Q+E", drawInsec = "Draw Insec", drawSoldierAA = "Draw Soldier Attack Range" , drawFly = "Draw EQ Range";

        public static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Azir")
                return;


            //Spells
            _q = new Spell(SpellSlot.Q, 1175);
            _q2 = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W, 450);
            _e = new Spell(SpellSlot.E, 1100);
            _r = new Spell(SpellSlot.R, 250);
            _r2 = new Spell(SpellSlot.R);
            // from detuks :D
            _q.SetSkillshot(0.0f, 65, 1500, false, SkillshotType.SkillshotLine);
            _q.MinHitChance = HitChance.Medium;
            //Menu instance
            _menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            //Orbwalker
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            _orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            //Targetsleector
            _menu.AddSubMenu(orbwalkerMenu);
            Menu ts = _menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);
            //spell menu
            Menu spellMenu = _menu.AddSubMenu(new Menu("Spells", "Spells"));
            spellMenu.AddItem(new MenuItem("EQdelay", "EQ lower delay").SetValue(new Slider(100,0,300)));
            spellMenu.AddItem(new MenuItem("EQmouse", "E Q to mouse").SetValue(new KeyBind('G', KeyBindType.Press)));
            spellMenu.AddItem(new MenuItem("insec", "Insec Selected").SetValue(new KeyBind('Y', KeyBindType.Press)));
            spellMenu.AddItem(new MenuItem("insecmode", "Insec Mode").SetValue(new StringList( new [] {"nearest ally","nearest turret","mouse","last key press"},0)));
            spellMenu.AddItem(new MenuItem("insecpolar", "Insec point key").SetValue(new KeyBind('T', KeyBindType.Press)));
            //combo
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("QC", "Q").SetValue(true));
            Combo.AddItem(new MenuItem("WC", "W").SetValue(true));
            Combo.AddItem(new MenuItem("donotqC", "Save Q if target in soldier's range").SetValue(false));
            //Harass
            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));
            Harass.AddItem(new MenuItem("QH", "Q").SetValue(true));
            Harass.AddItem(new MenuItem("WH", "W").SetValue(true));
            Harass.AddItem(new MenuItem("donotqH", "Save Q if target in soldier's range").SetValue(false));

            Menu Auto = spellMenu.AddSubMenu(new Menu("Auto", "Auto"));
            Auto.AddItem(new MenuItem("RKS", "use R KS").SetValue(true));
            Auto.AddItem(new MenuItem("RTOWER", "R target to Tower").SetValue(true));
            Auto.AddItem(new MenuItem("RGAP", "R anti GAP").SetValue(false));

            //Drawing
            Menu Draw = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Draw.AddItem(new MenuItem(drawQ, drawQ).SetValue(true));
            Draw.AddItem(new MenuItem(drawW, drawW).SetValue(true));
            Draw.AddItem(new MenuItem(drawSoldierAA, drawSoldierAA).SetValue(true));
            Draw.AddItem(new MenuItem(drawFly, drawFly).SetValue(true));
            Draw.AddItem(new MenuItem(drawInsec, drawInsec).SetValue(true));
            //Attach to root
            _menu.AddToMainMenu();

            GameObjects.Initialize();
            Soldiers.AzirSoldier();
            OrbwalkCommands.Initialize();
            AzirCombo.Initialize();
            AzirHarass.Initialize();
            AzirFarm.Initialize();
            JumpToMouse.Initialize();
            Insec.Initialize();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnDoCast;
            //Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        private static void Obj_AI_Base_OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var target = gapcloser.Sender;
            if (target.IsEnemy && _r.IsReady() && target.IsValidTarget() && !target.IsZombie && RGAP)
            {
                if (target.IsValidTarget(250)) _r.Cast(target.Position);
            }
        }
        public static int EQdelay { get{ return _menu.Item("EQdelay").GetValue<Slider>().Value; } }
        public static bool drawinsecLine { get{ return _menu.Item(drawInsec).GetValue<bool>(); } }
        public static uint insecpointkey { get{return _menu.Item("insecpolar").GetValue<KeyBind>().Key; } }
        public static bool eqmouse { get { return _menu.Item("EQmouse").GetValue<KeyBind>().Active; } }
        public static bool RTOWER { get { return _menu.Item("RTOWER").GetValue<bool>(); } }
        public static bool RKS { get { return _menu.Item("RKS").GetValue<bool>(); } }
        public static bool RGAP { get { return _menu.Item("RGAP").GetValue<bool>(); } }
        public static bool qcombo { get { return _menu.Item("QC").GetValue<bool>(); } }
        public static bool wcombo { get { return _menu.Item("WC").GetValue<bool>(); } }
        public static bool donotqcombo { get { return _menu.Item("donotqC").GetValue<bool>(); } }
        public static bool qharass { get { return _menu.Item("QH").GetValue<bool>(); } }
        public static bool wharass { get { return _menu.Item("WH").GetValue<bool>(); } }
        public static bool donotqharass { get { return _menu.Item("donotqH").GetValue<bool>(); } }
        public static bool insec { get { return _menu.Item("insec").GetValue<KeyBind>().Active; } }
        public static int insecmode { get { return _menu.Item("insecmode").GetValue<StringList>().SelectedIndex; } }
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("azirq"))
            {
                Qtick = Utils.GameTimeTickCount;
                qcount = Utils.GameTimeTickCount;
 
            }
            if (args.SData.Name.ToLower().Contains("azirw"))
            {

            }
            if (args.SData.Name.ToLower().Contains("azire"))
            {
                ecount = Utils.GameTimeTickCount;

            }
            if (args.SData.Name.ToLower().Contains("azirr"))
            {

            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Auto();
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_menu.Item(drawQ).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _q.Range, Color.Yellow);
            if (_menu.Item(drawW).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Yellow);
            if (_menu.Item(drawSoldierAA).GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, 925, Color.Red);
                foreach (var sold in Soldiers.soldier.Where(x => x.Position.Distance(Player.Position) <= 925))
                {
                    Render.Circle.DrawCircle(sold.Position, 300, Color.Red);
                }
            }
            if (_menu.Item(drawFly).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, 875 + 300 - 100, Color.Pink);
            //foreach (var soldier in Soldiers.soldier)
            //{
            //    Render.Circle.DrawCircle(soldier.Position, 200, Color.Yellow);
            //}
        }

        private static void Auto()
        {
            if (RKS)
            {
                if (_r.IsReady())
                {
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(250) && !x.IsZombie && x.Health < _r.GetDamage(x)))
                    {
                        _r.Cast(hero.Position);
                    }
                }
            }
            if(RTOWER)
            {
                if (_r.IsReady())
                {
                    var turret = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.IsAlly && !x.IsDead).OrderByDescending(x => x.Distance(Player.Position)).LastOrDefault();
                    foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(250) && !x.IsZombie))
                    {
                        if (Player.ServerPosition.Distance(turret.Position)+100 >= hero.Distance(turret.Position) && hero.Distance(turret.Position) <= 775 + 250)
                        {
                            var pos = Player.Position.Extend(turret.Position, 250);
                            _r.Cast(pos);
                        }
                    }
                }
            }
        }



        public static bool  Qisready()
        {
            if (Utils.GameTimeTickCount - Qtick >= _q.Instance.Cooldown * 1000)
            {
                return true;
            }
            else
                return false;
        }
        public static int Qtick;

        public static double Wdamage(Obj_AI_Base target)
        {
            return Player.CalcDamage(target, Damage.DamageType.Magical,
                        new double[]
                        {
                            50 , 52 , 54 , 56 , 58 , 60 , 63 , 66 , 69 , 72 , 75 , 85 , 95 , 110 , 125 , 140 , 155 , 170
                        }[Player.Level - 1] + 0.6 * Player.FlatMagicDamageMod);
        }
        
    }
}
