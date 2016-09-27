using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeRyze
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Orbwalking.Orbwalker _orbwalker;

        public static Spell _q, _q2, _w, _e, _r;

        public static Menu _menu;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Ryze")
                return;

            //Spells
            _q = new Spell(SpellSlot.Q, 900);
            _q2 = new Spell(SpellSlot.Q, 900); // xxx bounce range
            _w = new Spell(SpellSlot.W, 600); // 600
            _e = new Spell(SpellSlot.E, 600); // 200 bounce 
            _r = new Spell(SpellSlot.R); // xx ramge

            _q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            _q2.SetSkillshot(0.26f, 50f, 1700f, false, SkillshotType.SkillshotLine);
            _q.MinHitChance = HitChance.Medium;
            _q2.MinHitChance = HitChance.Medium;

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
            //Combo
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("Block", "Smart Block AutoAttack").SetValue(true));
            Combo.AddItem(new MenuItem("ComboSwitch", "ComboModeSwitch").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Combo.AddItem(new MenuItem("ComboMode", "ComboMode").SetValue(new StringList(new[] { "Burst","AoE/Shield" }, 0)));


            //auto
            Menu Auto = spellMenu.AddSubMenu(new Menu("Auto", "Auto"));
            Auto.AddItem(new MenuItem("Wantigap", "W anti gap").SetValue(true));
            Auto.AddItem(new MenuItem("Winterrupt", "W interrupt").SetValue(true));
            Auto.AddItem(new MenuItem("AutoTear", "Auto Stack Tear").SetValue(true));
            Auto.AddItem(new MenuItem("AutoTearM", "Min Mana Stack Tear").SetValue(new Slider(40, 0, 100)));
            //Clear
            Menu JungClear = spellMenu.AddSubMenu(new Menu("JC", "Jungle Clear"));
            JungClear.AddItem(new MenuItem("QJC", "use Q Jungle Clear").SetValue(true));
            JungClear.AddItem(new MenuItem("WJC", "use W Jungle Clear").SetValue(true));
            JungClear.AddItem(new MenuItem("EJC", "use E Jungle Clear").SetValue(true));
            JungClear.AddItem(new MenuItem("ManaJC", "Min Mana Jung Clear").SetValue(new Slider(40, 0, 100)));
            Menu LaneClear = spellMenu.AddSubMenu(new Menu("LC", "Lane Clear"));
            LaneClear.AddItem(new MenuItem("QLC", "use Q Lane Clear").SetValue(true));
            LaneClear.AddItem(new MenuItem("WLC", "use w Lane Clear").SetValue(true));
            LaneClear.AddItem(new MenuItem("ELC", "use e Lane Clear").SetValue(true));
            LaneClear.AddItem(new MenuItem("ManaLC", "Min Mana Lane Clear").SetValue(new Slider(40, 0, 100)));
            Menu LastHit = spellMenu.AddSubMenu(new Menu("LH", "Last Hit"));
            LastHit.AddItem(new MenuItem("QLH", "use Q Last Hit").SetValue(true));
            LastHit.AddItem(new MenuItem("ManaLH", "Min Mana Last Hit").SetValue(new Slider(40, 0, 100)));


            //Drawing
            Menu Draw = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Draw.AddItem(new MenuItem("DQ", "Draw Q").SetValue(true));
            Draw.AddItem(new MenuItem("DW", "Draw W").SetValue(true));
            Draw.AddItem(new MenuItem("DE", "Draw E").SetValue(true));
            Draw.AddItem(new MenuItem("DrawMode", "Draw Combo Mode").SetValue(true));
            Draw.AddItem(new MenuItem("DRmini", "Draw R MiniMap").SetValue(true));
            Draw.AddItem(new MenuItem("DR", "Draw R").SetValue(true));
            //Attach to root
            _menu.AddToMainMenu();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += oncast;

            //modes
            HeavenStrikeRyze.Combo.BadaoActivate();
            HeavenStrikeRyze.Jungle.BadaoActivate();
            HeavenStrikeRyze.Lane.BadaoActivate();
            HeavenStrikeRyze.LastHit.BadaoActivate();
        }
        //combo
        public static bool BlockAA {get { return _menu.Item("Block").GetValue<bool>(); } }
        public static string mode { get { return _menu.Item("ComboMode").GetValue<StringList>().SelectedValue; } }
        // auto
        public static bool WAntiGap { get { return _menu.Item("Wantigap").GetValue<bool>(); } }
        public static bool WInterrupt { get { return _menu.Item("Winterrupt").GetValue<bool>(); } }
        public static bool AutoTear { get{return _menu.Item("AutoTear").GetValue<bool>(); } }
        public static int AutoTearM { get { return _menu.Item("AutoTearM").GetValue<Slider>().Value; } }
        // jungleclear
        public static bool QjungClear { get { return _menu.Item("QJC").GetValue<bool>(); } }
        public static bool WjungClear { get { return _menu.Item("WJC").GetValue<bool>(); } }
        public static bool EjungClear { get { return _menu.Item("EJC").GetValue<bool>(); } }
        public static int ManaJungClear { get { return _menu.Item("ManaJC").GetValue<Slider>().Value; } }
        // lane clear
        public static bool QlaneClear { get { return _menu.Item("QLC").GetValue<bool>(); } }
        public static bool WlaneClear { get { return _menu.Item("WLC").GetValue<bool>(); } }
        public static bool ElaneClear { get { return _menu.Item("ELC").GetValue<bool>(); } }
        public static int ManaLaneClear { get { return _menu.Item("ManaLC").GetValue<Slider>().Value; } }
        // last hit
        public static bool QlastHit { get { return _menu.Item("QLH").GetValue<bool>(); } }
        public static int ManaLastHit { get { return _menu.Item("ManaLH").GetValue<Slider>().Value; } }
        // draw
        public static bool DrawQ { get { return _menu.Item("DQ").GetValue<bool>(); } }
        public static bool DrawW { get { return _menu.Item("DW").GetValue<bool>(); } }
        public static bool DrawE { get { return _menu.Item("DE").GetValue<bool>(); } }
        public static bool DrawR { get { return _menu.Item("DR").GetValue<bool>(); } }
        public static bool DrawRMini { get { return _menu.Item("DRmini").GetValue<bool>(); } }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            //foreach (var item in ObjectManager.Get<Obj_AI_Base>().Where(x => x.HasBuff("RyzeE")))
            //{
            //    Render.Circle.DrawCircle(item.Position, 75, Color.Aqua);
            //}
            //var tar = ObjectManager.Get<Obj_AI_Base>().Where(x => Helper.HasEBuff(x)).MaxOrDefault(x => x.Distance(Player.Position));
            //if (tar != null)
            //{
            //    foreach (var item in Helper.GetchainedTarget(tar))
            //    {
            //        Render.Circle.DrawCircle(item.Position, 75, Color.Red);
            //    }

            //}
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, _q.Range, Color.Aqua);
            if (DrawW)
                Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Purple);
            if (DrawE)
                Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Yellow);
            if (DrawR)
                Render.Circle.DrawCircle(Player.Position, Helper.RRAnge(), Color.Aqua);
            if (DrawRMini)
                LeagueSharp.Common.Utility.DrawCircle(Player.Position, Helper.RRAnge(), Color.Aqua, 1, 23, true);
            if (_menu.Item("DrawMode").GetValue<bool>())
            {
                var x = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(x[0], x[1], Color.White, mode);
            }
        }

        public static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }
            //Chat.Print(spell.Name);
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            // use W against gap closer
            var target = gapcloser.Sender;
            if (_w.IsReady() && target.IsValidTarget(_w.Range) && WAntiGap)
            {
                _w.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            // interrupt with W
            if (_w.IsReady() && sender.IsValidTarget(_w.Range) && !sender.IsZombie && WInterrupt)
            {
                _w.Cast(sender);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            ComboModeSwitch();
            //if (Helper.CanShield())
            //Chat.Print(Helper.CanShield().ToString());
            //Chat.Print(Helper.BonusMana.ToString());
            //Chat.Print(Helper.Qstack().ToString());
            //Chat.Print(Player.ManaPercent.ToString());
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && AutoTear && Player.ManaPercent >= AutoTearM
                && Player.CountEnemiesInRange(1500) == 0)
            {
                if (ItemData.Tear_of_the_Goddess.GetItem().IsOwned() || ItemData.Archangels_Staff.GetItem().IsOwned()
                    || ItemData.Manamune.GetItem().IsOwned())
                {   
                    if (_q.IsReady())
                    {
                        _q.Cast(Player.Position);
                    }
                }
            }
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                var target = _orbwalker.GetTarget();
                if ((_w.IsReady() || (Player.Mana >= _q.ManaCost + _e.ManaCost)) && BlockAA
                    && target.IsValidTarget() && (!target.IsValidTarget(350) || Player.CountEnemiesInRange(800) >= 2)
                    || !target.IsValidTarget())
                {
                    _orbwalker.SetAttack(false);
                }
                else
                {
                    _orbwalker.SetAttack(true);
                }
            }
            else
            {
                _orbwalker.SetAttack(true);
            }
            foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(_q.Range) && !x.IsZombie))
            {
                if (_q.IsReady() && Helper.Qdamage(hero) >= hero.Health)
                    Helper.CastQTarget(hero);
                if (_w.IsReady() && Helper.Wdamge(hero) >= hero.Health)
                    _w.Cast(hero);
                if (_e.IsReady() && Helper.Edamge(hero) >= hero.Health)
                    _e.Cast(hero);
            }
        }
        private static int _lastTick;
        private static void ComboModeSwitch()
        {
            var comboMode = mode;
            var lasttime = Utils.GameTimeTickCount - _lastTick;
            if (!_menu.Item("ComboSwitch").GetValue<KeyBind>().Active ||
                lasttime <= Game.Ping)
            {
                return;
            }

            switch (comboMode)
            {
                case "Burst":
                    _menu.Item("ComboMode").SetValue(new StringList(new[] { "Burst", "AoE/Shield" }, 1));
                    _lastTick = Utils.GameTimeTickCount + 300;
                    break;
                case "AoE/Shield":
                    _menu.Item("ComboMode").SetValue(new StringList(new[] { "Burst", "AoE/Shield" }, 0));
                    _lastTick = Utils.GameTimeTickCount + 300;
                    break;
            }
        }
    }
}
