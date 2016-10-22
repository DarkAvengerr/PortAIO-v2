using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;  

using LeagueSharp.Common.Data;
using LeagueSharp.Common;
using LeagueSharp;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SethLeona
{
    class Program
    {
        public const string ChampionName = "Leona";

        public static Menu _root;
        public static Spell _q, _w, _e, _r;
        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> _spells = new List<Spell>();

        public static SpellSlot _exhaustSlot;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;
            Console.WriteLine("Loaded after champion check!");

            _q = new Spell(SpellSlot.Q, 120f);
            _w = new Spell(SpellSlot.W, 450f);
            _e = new Spell(SpellSlot.E, 875f);
            _e.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.SkillshotLine);

            _r = new Spell(SpellSlot.R, 1200f);
            _r.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _exhaustSlot = ObjectManager.Player.GetSpellSlot("SummonerExhaust");

            _spells.Add(_q);
            _spells.Add(_w);
            _spells.Add(_e);
            _spells.Add(_r);

            _root = new Menu("SethLeona", "Leona", true);
            _root.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            Orbwalker = new Orbwalking.Orbwalker(_root.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _root.AddSubMenu(targetSelectorMenu);

            _root.AddSubMenu(new Menu("Spell Handler", "SpellsM"));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Combo", "Combo"));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseExhaustCombo", "Use Exhaust").SetValue(true));
            _root.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "Combo!").SetValue(
                        new KeyBind(_root.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));            

            _root.AddSubMenu(new Menu("Drawing Manager", "SharpDrawer"));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));

            _root.AddSubMenu(new Menu("Misc Manager", "Misc"));
            _root.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "Interrupt Spells").SetValue(true));
            _root.SubMenu("InterruptSpells").AddItem(new MenuItem("EQInterrupt", "Use EQ on Target").SetValue(true));
            _root.SubMenu("InterruptSpells").AddItem(new MenuItem("RInterrupt", "Use R on Target").SetValue(true));
            _root.SubMenu("Misc").AddItem(new MenuItem("QUsage", "Auto-Q on GapClosers").SetValue(true));

            _root.AddToMainMenu();
            Chat.Print("<font color='#0066FF'>Seth </font>: Leona [ Loaded ]");

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser -= OnEnemyGapcloser;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_root.Item("DrawQ").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _q.Range, _root.Item("DrawQ").GetValue<Circle>().Color); }
            if (_root.Item("DrawW").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _w.Range, _root.Item("DrawW").GetValue<Circle>().Color); }
            if (_root.Item("DrawE").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _e.Range, _root.Item("DrawE").GetValue<Circle>().Color); }
            if (_root.Item("DrawR").GetValue<Circle>().Active)
            { Render.Circle.DrawCircle(ObjectManager.Player.Position, _r.Range, _root.Item("DrawR").GetValue<Circle>().Color); }
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (_root.Item("QUsage").GetValue<bool>())
            {
                if (ObjectManager.Player.Distance(gapcloser.Sender) < ObjectManager.Player.AttackRange)
                {
                    _q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, gapcloser.Sender);
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var MenuInstance = _root.Item("InterruptSpells").GetValue<bool>();
            var EQInstance = _root.Item("EQInterrupt").GetValue<bool>();
            var RInstance = _root.Item("RInterrupt").GetValue<bool>();

            if (MenuInstance)
            {
                if (ObjectManager.Player.Distance(sender) < _q.Range && _q.IsReady())
                {
                    _q.Cast();
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                }

                if (ObjectManager.Player.Distance(sender) < _e.Range && _e.IsReady() && _q.IsReady())
                {
                    if (EQInstance)
                    {
                        _q.Cast();
                        _e.CastIfHitchanceEquals(sender, HitChance.High, true);
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                    }
                }
                else if (ObjectManager.Player.Distance(sender) > _e.Range)
                {
                    if (RInstance)
                    {
                        if (ObjectManager.Player.Distance(sender) < _r.Range && _r.IsReady())
                            _r.CastIfHitchanceEquals(sender, HitChance.High, true);
                    }
                }
            }
        }
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_root.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                var qHandler = _root.Item("UseQCombo").GetValue<bool>();
                var wHandler = _root.Item("UseWCombo").GetValue<bool>();
                var eHandler = _root.Item("UseECombo").GetValue<bool>();
                var rHandler = _root.Item("UseRCombo").GetValue<bool>();

                var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical);
                var randuin = ItemData.Randuins_Omen.GetItem();

                if (target != null)
                {
                    if (qHandler && _q.IsReady())
                    {
                        if (ObjectManager.Player.Distance(target) < _q.Range)
                        {
                            _q.Cast();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                    if (eHandler && _e.IsReady())
                    {
                        if (ObjectManager.Player.Distance(target) < _e.Range)
                        {
                            _e.CastIfHitchanceEquals(target, HitChance.High, true);
                            if (randuin.IsOwned() && randuin.IsReady() && randuin.IsInRange(target))
                                randuin.Cast();
                        }
                    }

                    if (wHandler && _w.IsReady())
                    {
                        if (ObjectManager.Player.Distance(target) < _w.Range)
                            _w.Cast();
                    }

                    if (_root.Item("UseExhaustCombo").GetValue<bool>())
                    {
                        if (_exhaustSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(_exhaustSlot) == SpellState.Ready)
                        {
                            ObjectManager.Player.Spellbook.CastSpell(_exhaustSlot, target);
                        }
                    }

                    if (wHandler && _w.IsReady())
                    {
                        if (ObjectManager.Player.Distance(target) < _w.Range)
                            _w.Cast();
                    }

                    if (rHandler && _r.IsReady())
                    {
                        if (ObjectManager.Player.Distance(target) > _e.Range)
                        {
                            if (ObjectManager.Player.Distance(target) < _r.Range)
                            {
                                _r.CastIfHitchanceEquals(target, HitChance.Immobile);
                            }
                        }
                    }
                }
            }
        }
    }
}
