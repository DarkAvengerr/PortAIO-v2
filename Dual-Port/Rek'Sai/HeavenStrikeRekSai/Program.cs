using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeReksaj
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q, _q2, _w, _e, _e2, _r;

        private static Menu _menu;

        private static string  drawe1 = "Draw E1", drawe2 = "Draw E2" , drawq2 = "Draw Q2", autoq2 = "Auto Q2",
            comboE2 = "E2 combo (tunnel)", comboW1 = "W1 combo (burrow)", comboE1 = "E1 Combo when fury >=";

        private static bool burrowed = false;

        public static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "RekSai")
                return;


            //Spells
            _q = new Spell(SpellSlot.Q);
            _q2 = new Spell(SpellSlot.Q, 1450);
            _q2.MinHitChance = HitChance.Medium;
            _w = new Spell(SpellSlot.W);
            _e = new Spell(SpellSlot.E, 250);
            _e2 = new Spell(SpellSlot.E, 500);
            _r = new Spell(SpellSlot.R);
            _q2.SetSkillshot(0.5f, 60, 1950, true, SkillshotType.SkillshotLine);
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
            spellMenu.AddItem(new MenuItem(comboW1, comboW1).SetValue(true));
            spellMenu.AddItem(new MenuItem(comboE2, comboE2).SetValue(false));
            spellMenu.AddItem(new MenuItem(comboE1, comboE1).SetValue(new Slider(0, 0, 100)));
            //auto menu
            Menu auto = spellMenu.AddSubMenu(new Menu("Auto", "Auto"));
            auto.AddItem(new MenuItem(autoq2, autoq2).SetValue(false));
            //Drawing
            Menu Draw = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Draw.AddItem(new MenuItem(drawe1, drawe1).SetValue(true));
            Draw.AddItem(new MenuItem(drawe2, drawe2).SetValue(true));
            Draw.AddItem(new MenuItem(drawq2, drawq2).SetValue(true));
            //Attach to root
            _menu.AddToMainMenu();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            if (args.SData.Name.ToLower().Contains("reksaiq"))
                Orbwalking.ResetAutoAttackTimer();
            if (args.SData.Name == "ItemTitanicHydraCleave")
            {
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && !_q.IsReady() && HasItem())
                CastItem();
            if (_orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && _q.IsReady())
                _q.Cast();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_menu.Item(drawe1).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Yellow);
            if (_menu.Item(drawe2).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, 750, Color.Yellow);
            if (_menu.Item(drawq2).GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _q2.Range, Color.Yellow);
        }


        private static void Game_OnGameUpdate(EventArgs args)
        {
            //get burrow state
            if (_w.Instance.Name.ToLower().Contains("burrowed"))
                burrowed = true;
            else burrowed = false;
            // set orbwalker 
            if (burrowed)
                _orbwalker.SetAttack(false);
            else _orbwalker.SetAttack(true);
            // ks
            KS();
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Clear();
        }
        //ks
        private static void KS()
        {
            // ks e
            if (!burrowed && _e.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_e.Range) && !x.IsZombie))
                {
                    if (target.Health < Player.CalcDamage(target, Player.Mana == 100 ? Damage.DamageType.True : Damage.DamageType.Physical, GetRawEDamage()))
                        _e.Cast(target);
                }
            }
            // ks q
            if (burrowed && _q2.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_q2.Range) && !x.IsZombie))
                {
                    if (target.Health < _q2.GetDamage(target))
                        _q2.Cast(target);
                }
            }
            // auto  q2 cast
            if (burrowed && _q2.IsReady() && _menu.Item(autoq2).GetValue<bool>() && !Player.IsRecalling())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_q2.Range) && !x.IsZombie))
                {
                    _q2.Cast(target);
                }
            }
        }
        // get e raw dmg
        private static double GetRawEDamage()
        {
            var damage = new double[] { 0.8f, 0.9f, 1, 1.1f, 1.2f }[_e.Level -1] * Player.TotalAttackDamage;
            return damage * (1 + (Player.Mana / Player.MaxMana));
        }
        private static void Combo()
        {
            // w2 cast
            if (burrowed && _w.IsReady())
            {
                var target = TargetSelector.GetTarget(Player.BoundingRadius + 200, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                    _w.Cast();
            }
            // e cast
            if (!burrowed && _e.IsReady() && Player.Mana >= _menu.Item(comboE1).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && !target.IsZombie)
                    _e.Cast(target);
            }
            // q2 cast
            if (burrowed && _q2.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_q2.Range) && !x.IsZombie))
                {
                        _q2.Cast(target);
                }
            }
            // W1 cast
            if (!burrowed && _w.IsReady() && _menu.Item(comboW1).GetValue<bool>() && Orbwalking.CanMove(80))
            {
                var target = _orbwalker.GetTarget();
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (!(target as Obj_AI_Base).HasBuff("reksaiknockupimmune"))
                        _w.Cast();
                }
                else
                {
                    var hero = TargetSelector.GetTarget(300, TargetSelector.DamageType.Physical);
                    if (hero.IsValidTarget() && !hero.IsZombie && !hero.HasBuff("reksaiknockupimmune"))
                        _w.Cast();
                }
            }
            //E2 cast
            if (burrowed && _e.IsReady() && _menu.Item(comboE2).GetValue<bool>())
            {
                if (Player.CountEnemiesInRange(300) == 0)
                {
                    var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Physical);
                    if (target.IsValidTarget() && !target.IsZombie && Prediction.GetPrediction(target,10).UnitPosition
                        .Distance(Player.Position) > target.Distance(Player.Position))
                    {
                        var x = Prediction.GetPrediction(target, 500).UnitPosition;
                        var y = Player.Position.Extend(x, _e2.Range);
                        _e2.Cast(x);
                    }
                }
            }
        }
        private static void Clear()
        {
            var minionE = MinionManager.GetMinions(_e.Range,MinionTypes.All,MinionTeam.NotAlly).FirstOrDefault();
            var minionQ2 = MinionManager.GetMinions(_q2.Range,MinionTypes.All,MinionTeam.NotAlly);
            var minionW = MinionManager.GetMinions(Player.BoundingRadius + 200, MinionTypes.All,MinionTeam.NotAlly).FirstOrDefault();
            if (minionE != null && !burrowed && _e.IsReady())
            {
                if (Player.CalcDamage(minionE, Player.Mana == 100 ? Damage.DamageType.True : Damage.DamageType.Physical, GetRawEDamage())
                    > minionE.Health)
                    _e.Cast(minionE);
                if (Player.Mana == 100)
                    _e.Cast(minionE);
            }
            if (burrowed && _q2.IsReady())
            {
                foreach (var target in minionQ2)
                {
                    _q2.Cast(target);
                }
            }
            if (minionW != null  && burrowed && _w.IsReady() && !_q2.IsReady())
            {
                _w.Cast();
            }
        }
        public static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady()
                || Items.CanUseItem(3748))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
            // titanic hydra
            if (Items.CanUseItem(3748))
                Items.UseItem(3748);
        }


    }
}
