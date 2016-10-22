using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp.Common;
using LeagueSharp;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Malphite
{
    class Program
    {
        public const string ChampionName = "Malphite";

        public static Menu _root;
        public static Spell _q, _w, _e, _r;
        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> _spells = new List<Spell>();

        private static SpellSlot _igniteSlot;
        public static void Main()
        {
            Game_OnGameLoaded();
        }

        private static void Game_OnGameLoaded()
        {
            if (ObjectManager.Player.BaseSkinName != ChampionName) return;

            _q = new Spell(SpellSlot.Q, 625f);
            _q.SetTargetted(0.50f, 75f);

            _w = new Spell(SpellSlot.W, 125f);
            _e = new Spell(SpellSlot.E, 375f);
            _e.SetTargetted(0.50f, 75f);

            _r = new Spell(SpellSlot.R, 1000f);
            _r.SetSkillshot(0.00f, 270, 700, false, SkillshotType.SkillshotCircle);

            _igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");

            _spells.Add(_q);
            _spells.Add(_w);
            _spells.Add(_e);
            _spells.Add(_r);

            _root = new Menu("SethMalphite", "Malphite", true);
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
            _root.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R").SetValue(false));
            _root.SubMenu("Combo").AddItem(new MenuItem("UseIgniteCombo", "Use Ignite").SetValue(true));
            _root.SubMenu("Combo")
                .AddItem(
                    new MenuItem("ComboActive", "Combo!").SetValue(
                        new KeyBind(_root.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Harass", "Harass"));
            _root.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
            _root.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
            _root.SubMenu("Harass")
                .AddItem(
                    new MenuItem("HarassActive", "Harass (toggle)!").SetValue(
                        new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Farm", "Farm"));
            _root.SubMenu("Farm").AddItem(new MenuItem("UseQFarm", "Use Q").SetValue(true));
            _root.SubMenu("Farm").AddItem(new MenuItem("UseWFarm", "Use W").SetValue(true));
            _root.SubMenu("Farm").AddItem(new MenuItem("UseEFarm", "Use E").SetValue(true));
            _root.SubMenu("Farm")
                .AddItem(
                new MenuItem("LaneClearActive", "Farm!").SetValue(
                    new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            _root.SubMenu("SpellsM").AddItem(new MenuItem("Killsteal", "Killsteal"));
            _root.SubMenu("Killsteal").AddItem(new MenuItem("StealActive", "Enabled").SetValue(true));
            _root.SubMenu("Killsteal").AddItem(new MenuItem("UseQSteal", "Use Q").SetValue(true));
            _root.SubMenu("Killsteal").AddItem(new MenuItem("UseESteal", "Use E").SetValue(true));

            _root.AddSubMenu(new Menu("Mana Manager", "ManaManager"));
            _root.SubMenu("ManaManager").AddItem(new MenuItem("Harass", "Harass"));
            if (_root.Item("UseQHarass").GetValue<bool>() == true)
                { _root.SubMenu("Harass").AddItem(new MenuItem("QMana", "Q Mana %").SetValue(new Slider(50, 100, 0))); }
            if (_root.Item("UseEHarass").GetValue<bool>() == true)
                { _root.SubMenu("Harass").AddItem(new MenuItem("EMana", "E Mana %").SetValue(new Slider(50, 100, 0))); }
            _root.SubMenu("ManaManager").AddItem(new MenuItem("Farm", "Farm"));
            if (_root.Item("UseQFarm").GetValue<bool>() == true)
                { _root.SubMenu("Farm").AddItem(new MenuItem("QManaF", "Q Mana %").SetValue(new Slider(50, 100, 0))); }
            if (_root.Item("UseWFarm").GetValue<bool>() == true)
                { _root.SubMenu("Farm").AddItem(new MenuItem("WManaF", "W Mana %").SetValue(new Slider(50, 100, 0))); }
            if (_root.Item("UseEFarm").GetValue<bool>() == true)
                { _root.SubMenu("Farm").AddItem(new MenuItem("EManaF", "E Mana %").SetValue(new Slider(50, 100, 0))); }

            _root.AddSubMenu(new Menu("Drawing Manager", "SharpDrawer"));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawW", "Draw W Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
            _root.SubMenu("SharpDrawer").AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 58, 90, 179))));
             
            _root.AddSubMenu(new Menu("Misc Manager", "Misc"));
            _root.SubMenu("Misc").AddItem(new MenuItem("InterruptSpells", "Interrupt Spells").SetValue(false));
            _root.SubMenu("InterruptSpells").AddItem(new MenuItem("RInterrupt", "Use R on Target").SetValue(false));
            _root.SubMenu("Misc").AddItem(new MenuItem("RMinHit", "Auto ult if <=").SetValue(new Slider(2, 1, 5)));
            _root.SubMenu("Misc")
                .AddItem(
                new MenuItem("FleeActive", "Flee [ WIP ]!").SetValue(
                    new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));

            _root.AddToMainMenu();
            Chat.Print("<font color='#0066FF'>Seth </font>: Malphite [ Loaded ]");

            var dmgAfterComboItem = new MenuItem("DamageAfterCombo", "Draw damage after combo").SetValue(true);
            _root.SubMenu("SharpDrawer").AddItem(dmgAfterComboItem);
            LeagueSharp.Common.Utility.HpBarDamageIndicator.DamageToUnit = GetComboDamage;
            LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = dmgAfterComboItem.GetValue<bool>();
            dmgAfterComboItem.ValueChanged += delegate (object sender, OnValueChangeEventArgs eventArgs)
            {
                LeagueSharp.Common.Utility.HpBarDamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (_root.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                var randuin = ItemData.Randuins_Omen.GetItem();
                var target = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);
                if (target != null)
                {
                    if (_root.Item("UseRCombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _r.Range && _r.IsReady())
                        { _r.Cast(target, true); }
                    }

                    if (randuin.IsOwned() && randuin.IsReady() && randuin.IsInRange(target))
                    { randuin.Cast(); }

                    if (_root.Item("UseQCombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _q.Range && _q.IsReady())
                        { _q.CastOnUnit(target, true); }
                    }
                    if (_root.Item("UseECombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _e.Range && _e.IsReady())
                        { _e.Cast(); }
                    }
                    if (_root.Item("UseIgniteCombo").GetValue<bool>())
                    {
                        if (_igniteSlot != SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                        { ObjectManager.Player.Spellbook.CastSpell(_igniteSlot, target); }
                    }
                    if (_root.Item("UseWCombo").GetValue<bool>())
                    {
                        if (ObjectManager.Player.Distance(target) < _w.Range && _w.IsReady())
                        {
                            _w.Cast();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        }
                    }
                }

                if (_root.Item("StealActive").GetValue<bool>())
                {
                    if (target != null)
                    {
                        if (_root.Item("UseQSteal").GetValue<bool>())
                        {
                            if (ObjectManager.Player.Distance(target) < _q.Range && _q.IsReady())
                            {
                                if (target.Health + 30 >= ObjectManager.Player.GetSpellDamage(target, SpellSlot.Q))
                                { _q.CastOnUnit(target); }
                            }
                        }
                        if (_root.Item("UseESteal").GetValue<bool>())
                        {
                            if (ObjectManager.Player.Distance(target) < _e.Range && _e.IsReady())
                            {
                                if (target.Health + 30 >= ObjectManager.Player.GetSpellDamage(target, SpellSlot.E))
                                { _e.Cast(); }
                            }
                        }
                    }
                }

                if (_root.Item("FleeActive").GetValue<KeyBind>().Active)
                {
                    // If you hit a minion with Q, you will get some movement speed from minion who get hited. It work it champion too.
                    var fleeMinion = MinionManager.GetMinions(_q.Range, MinionTypes.All, MinionTeam.NotAlly);
                    foreach (var minionf in fleeMinion)
                    { _q.CastOnUnit(minionf); }
                }

                if (_root.Item("HarassActive").GetValue<KeyBind>().Active)
                {
                    var hTarget = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
                    var qManah = ObjectManager.Player.MaxMana / 100 * _root.Item("QMana").GetValue<Slider>().Value;
                    var eManah = ObjectManager.Player.MaxMana / 100 * _root.Item("EMana").GetValue<Slider>().Value;
                    
                    if (hTarget != null)
                    {
                        if (_root.Item("UseQHarass").GetValue<bool>() && ObjectManager.Player.Mana >= qManah)
                        {
                            if (ObjectManager.Player.Distance(hTarget) < _q.Range && _q.IsReady())
                            { _q.CastOnUnit(hTarget); }
                        }
                        if (_root.Item("UseEHarass").GetValue<bool>() && ObjectManager.Player.Mana >= eManah)
                        {
                            if (ObjectManager.Player.Distance(hTarget) < _e.Range && _e.IsReady())
                            { _e.Cast(); }
                        }
                    } 
                }

                if (_root.Item("LaneClearActive").GetValue<KeyBind>().Active)
                {
                    var qManal = ObjectManager.Player.MaxMana / 100 * _root.Item("QManaF").GetValue<Slider>().Value;
                    var wManal = ObjectManager.Player.MaxMana / 100 * _root.Item("WManaF").GetValue<Slider>().Value;
                    var eManal = ObjectManager.Player.MaxMana / 100 * _root.Item("EManaF").GetValue<Slider>().Value;
                    var minions = MinionManager.GetMinions(ObjectManager.Player.Position, _q.Range, MinionTypes.All, MinionTeam.NotAlly);

                    foreach (var minion in minions)
                    {
                        if (ObjectManager.Player.Mana >= qManal && _root.Item("UseQFarm").GetValue<bool>() && ObjectManager.Player.Distance(minion) < _q.Range && _q.IsReady())
                        { _q.CastOnUnit(minion); }
                        if (ObjectManager.Player.Mana >= wManal && _root.Item("UseWFarm").GetValue<bool>() && ObjectManager.Player.Distance(minion) < _w.Range && _w.IsReady())
                        { _w.Cast(); }
                        if (ObjectManager.Player.Mana >= eManal && _root.Item("UseEFarm").GetValue<bool>() && ObjectManager.Player.Distance(minion) < _e.Range && _e.IsReady())
                        { _e.Cast(); }
                    }
                }

                var rTarget = TargetSelector.GetTarget(_r.Range, TargetSelector.DamageType.Magical);
                var rTargets = _root.Item("RMinHit").GetValue<Slider>().Value;
                if (_r.IsReady() && rTarget.CountEnemiesInRange(1000) >= rTargets)
                { _r.Cast(rTarget.Position, true); }
            }
        }

        private static float GetComboDamage(Obj_AI_Base t)
        {
            var fComboDamage = 0d;
            if (_q.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.Q);
            if (_r.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.R);
            if (_w.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.W);
            if (_e.IsReady())
                fComboDamage += ObjectManager.Player.GetSpellDamage(t, SpellSlot.E);
            if (_igniteSlot != SpellSlot.Unknown &&
            ObjectManager.Player.Spellbook.CanUseSpell(_igniteSlot) == SpellState.Ready)
                fComboDamage += ObjectManager.Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite);

            return (float)fComboDamage;
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (_root.Item("InterruptSpells").GetValue<bool>())
            {
                if (_root.Item("RInterrupt").GetValue<bool>())
                {
                    if (ObjectManager.Player.Distance(sender) < _r.Range && _r.IsReady())
                    { _r.Cast(sender, true); }
                } 
            }
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
    }
}