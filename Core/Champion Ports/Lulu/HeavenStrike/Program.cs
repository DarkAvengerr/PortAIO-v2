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
 namespace HeavenStrikeLuLu
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker _orbwalker;

        private static Spell _q,_q2, _w, _e, _r;

        private static Menu _menu;

        private static GameObject pix;

        public static void Game_OnGameLoad()
        {
            //Verify Champion
            if (Player.ChampionName != "Lulu")
                return;

            //Spells
            _q = new Spell(SpellSlot.Q,925);
            _q.MinHitChance = HitChance.Medium;
            _q2 = new Spell(SpellSlot.Q,925);
            _q2.MinHitChance = HitChance.Medium;
            _w = new Spell(SpellSlot.W,650);
            _e = new Spell(SpellSlot.E,650);
            _r = new Spell(SpellSlot.R,900);
            _q.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine);
            _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine);

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
            //harass
            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));
            Harass.AddItem(new MenuItem("QH", "Q").SetValue(true));
            Harass.AddItem(new MenuItem("EH", "E").SetValue(true));
            Harass.AddItem(new MenuItem("QEH", "E+Q").SetValue(true));
            Harass.AddItem(new MenuItem("ManaH", "Min Mana Harass").SetValue(new Slider(40,0,100)));
            //combo 
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("QC", "Q").SetValue(true));
            Combo.AddItem(new MenuItem("EC", "E").SetValue(true));
            Combo.AddItem(new MenuItem("QEC", "E+Q").SetValue(true));
            Combo.AddItem(new MenuItem("RC", "R").SetValue(true));
            Combo.AddItem(new MenuItem("RHC", "R if will hit").SetValue(new Slider(2,1,5)));
            Menu Wcombo = Combo.AddSubMenu(new Menu("W", "W"));
            Wcombo.AddItem(new MenuItem("WC", "W").SetValue(true));
            foreach (var hero in HeroManager.Enemies)
            {
                Wcombo.AddItem(new MenuItem("WC" + hero.ChampionName, "W " + hero.ChampionName).SetValue(false));
            }
            //auto
            Menu Auto = spellMenu.AddSubMenu(new Menu("Auto", "Auto"));
            Auto.AddItem(new MenuItem("QA", "Ks Q").SetValue(true));
            Auto.AddItem(new MenuItem("EA", "Ks E").SetValue(true));
            Auto.AddItem(new MenuItem("QEA", "Ks E+Q").SetValue(true));
            Auto.AddItem(new MenuItem("WG", "W anti gap").SetValue(true));
            Auto.AddItem(new MenuItem("WI", "W interrupt").SetValue(true));
            Auto.AddItem(new MenuItem("RI", "R interrupt").SetValue(true));
            //AutoHarass
            Menu AutoHarass = Auto.AddSubMenu(new Menu("Auto Harass", "Auto Harass"));
            AutoHarass.AddItem(new MenuItem("AH", "Auto harass").SetValue(new KeyBind("H".ToCharArray()[0],KeyBindType.Toggle,true)));
            AutoHarass.AddItem(new MenuItem("QAH", "Q harass").SetValue(true));
            AutoHarass.AddItem(new MenuItem("EAH", "E harass").SetValue(true));
            //Auto shield
            Menu AutoShield = Auto.AddSubMenu(new Menu("Auto Shield", "Auto Shield"));
            AutoShield.AddItem(new MenuItem("EAS", "Auto Shield E").SetValue(true));
            foreach (var hero in HeroManager.Allies)
            {
                AutoShield.AddItem(new MenuItem("Shield" + hero.ChampionName, "Shield " + hero.ChampionName).SetValue(false));
                AutoShield.AddItem(new MenuItem("HPS" + hero.ChampionName, "% Hp to shield").SetValue(new Slider(20,0,100)));
            }
            //Auto R
            Menu AutoR = Auto.AddSubMenu(new Menu("Auto R", "Auto R"));
            AutoR.AddItem(new MenuItem("AR", "Auto R").SetValue(true));
            foreach (var hero in HeroManager.Allies)
            {
                AutoR.AddItem(new MenuItem("R" + hero.ChampionName, "R " + hero.ChampionName).SetValue(false));
                AutoR.AddItem(new MenuItem("HPR" + hero.ChampionName, "% Hp to R").SetValue(new Slider(20, 0, 100)));
            }
            //Drawing
            Menu Draw = _menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Draw.AddItem(new MenuItem("DQ", "Draw Q").SetValue(true));
            Draw.AddItem(new MenuItem("DW", "Draw W").SetValue(true));
            Draw.AddItem(new MenuItem("DE", "Draw E").SetValue(true));
            Draw.AddItem(new MenuItem("DR", "Draw R").SetValue(true));
            Draw.AddItem(new MenuItem("DEQ", "Draw E + Q").SetValue(true));

            //Attach to root
            _menu.AddToMainMenu();

            //Listen to events
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            //print chat as game loaded
            Chat.Print("Welcome to Heaven Strike Lulu");
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            // use W against gap closer
            var target = gapcloser.Sender;
            if (_w.IsReady() && target.IsValidTarget(_w.Range) && _menu.Item("WG").GetValue<bool>())
            {
                _w.Cast(target);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            // interrupt with W
            if (_w.IsReady() && sender.IsValidTarget(_w.Range) && !sender.IsZombie && _menu.Item("WI").GetValue<bool>())
            {
                _w.Cast(sender);
            }
            // interrupt with R
            if (_r.IsReady() && sender.IsValidTarget() && !sender.IsZombie && _menu.Item("RI").GetValue<bool>())
            {
                var target = HeroManager.Allies.Where(x => x.IsValidTarget(_r.Range, false)).OrderByDescending(x => 1 - x.Distance(sender.Position))
                    .Find(x => x.Distance(sender.Position) <= 350);
                if (target != null)
                    _r.Cast(target);
            }
        }

        public static void Drawing_OnDraw (EventArgs args)
        {
            if (Player.IsDead) return;
            if (_menu.Item("DQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _q.Range, Color.Aqua);
            if (_menu.Item("DW").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _w.Range, Color.Purple);
            if (_menu.Item("DE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _e.Range, Color.Yellow);
            if (_menu.Item("DR").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _r.Range, Color.Violet);
            if (_menu.Item("DEQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, _q.Range + _e.Range, Color.YellowGreen);
        }
        public static void Game_OnGameUpdate (EventArgs args)
        {
            Getpixed();// set value for pix
            Auto();//Auto cast spells
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Harass();
        }
        public static void Auto()
        {
            // case KS with Q
            if (_q.IsReady() && _menu.Item("QA").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget() && _q.GetDamage(x) >= x.Health 
                    && (x.Distance(Player.Position) > x.Distance(pix.Position) ? 925 >= x.Distance(pix.Position): 925 >= x.Distance(Player.Position))
                    ))
                {
                    _q.Cast(hero);
                    _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                    _q2.Cast(hero);
                }
            }
            // case KS with E
            if (_e.IsReady() && _menu.Item("EA").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(_e.Range) && _e.GetDamage(x) >= x.Health))
                {
                    _e.Cast(hero);
                }
            }
            // case KS with EQ
            if (_q.IsReady() && _e.IsReady() && Player.Mana >= _q.Instance.SData.Mana + _e.Instance.SData.Mana && _menu.Item("QEA").GetValue<bool>())
            {
                // EQ on same target
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(_e.Range) && _e.GetDamage(x) + _q.GetDamage(x) >= x.Health 
                    && _q.GetDamage(x) < x.Health))
                {
                    _e.Cast(hero);
                }
                // EQ on different target
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(_e.Range + _q.Range) && !x.IsValidTarget(_q.Range)
                    && _q.GetDamage(x) >= x.Health))
                {
                    // E target is hero
                    foreach (var target in HeroManager.AllHeroes.Where(x => x.IsValidTarget(_e.Range,false) && x.Distance(hero.Position) <= _q.Range)
                        .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                    {
                        _e.Cast(target);
                    }
                    // E target is minion
                    foreach (var target in MinionManager.GetMinions(_e.Range,MinionTypes.All,MinionTeam.All).Where(x => x.IsValidTarget(_e.Range,false) 
                        && !x.Name.ToLower().Contains("ward")  && x.Distance(hero.Position) <= _q.Range)
                            .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                    {
                        // target die with E ?
                        if (!target.IsAlly && target.Health > _e.GetDamage(target) || target.IsAlly)
                            _e.Cast(target);
                    }
                }
            }
            //auto shield
            if (_e.IsReady() && _menu.Item("EAS").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Allies.Where(x => x.IsValidTarget(_e.Range, false) && _menu.Item("Shield" + x.ChampionName).GetValue<bool>()))
                {
                    if (hero.Health * 100 / hero.MaxHealth <= _menu.Item("HPS" + hero.ChampionName).GetValue<Slider>().Value 
                        && hero.CountEnemiesInRange(900) >= 1)
                        _e.Cast(hero);
                }
            }
            //auto R save
            if (_r.IsReady() && _menu.Item("AR").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Allies.Where(x => x.IsValidTarget(_r.Range, false) && _menu.Item("R" + x.ChampionName).GetValue<bool>()))
                {
                    if (hero.Health * 100 / hero.MaxHealth <=  _menu.Item("HPR" + hero.ChampionName).GetValue<Slider>().Value
                        && hero.CountEnemiesInRange(900) >= 1)
                        _r.Cast(hero);
                }
            }
            //auto Harass
            if (!UnderTower(Player.ServerPosition) && _menu.Item("AH").GetValue<KeyBind>().Active)
            {
                //auto Q Harass
                if (_q.IsReady() && _menu.Item("QAH").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
                    var target2 = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical, true, null, pix != null ? pix.Position : Player.Position);
                    if (target != null && target.IsValidTarget())
                        _q.Cast(target);
                    if (target2 != null && target2.IsValidTarget())
                    {
                        _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                        _q2.Cast(target2);
                    }
                }
                //aut E Harass
                if (_e.IsReady() && _menu.Item("EAH").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical);
                    if (target != null && target.IsValidTarget())
                        _e.Cast(target);
                }
            }
        }
        public static void Combo()
        {
            // cast Q
            if (_q.IsReady() && _menu.Item("QC").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
                var target2 = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical, true, null, pix != null ? pix.Position : Player.Position);
                if (target != null && target.IsValidTarget())
                    _q.Cast(target);
                if (target2 != null && target2.IsValidTarget())
                {
                    _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                    _q2.Cast(target2);
                }
            }
            // cast E
            if (_e.IsReady() && _menu.Item("EC").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical);
                if (target != null && target.IsValidTarget())
                    _e.Cast(target);
            }
            // cast W
            if (_w.IsReady() && _menu.Item("WC").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(_w.Range) && _menu.Item("WC" + x.ChampionName).GetValue<bool>()))
                {
                    _w.Cast(hero);
                }
            }
            //cast R if will hit 
            if (_r.IsReady() && _menu.Item("RC").GetValue<bool>())
            {
                foreach (var hero in HeroManager.Allies.Where(x => x.IsValidTarget(_r.Range,false)))
                {
                    if (hero.CountEnemiesInRange(350) >= _menu.Item("RHC").GetValue<Slider>().Value)
                        _r.Cast(hero);
                }
            }
            // QE combo
            if (_q.IsReady() && _e.IsReady() && Player.Mana >= _q.Instance.SData.Mana + _e.Instance.SData.Mana && _menu.Item("QEC").GetValue<bool>() && _menu.Item("EC").GetValue<bool>())
            {
                var hero = TargetSelector.GetTarget(_q.Range + _e.Range, TargetSelector.DamageType.Magical);
                if (hero != null && hero.IsValidTarget() && !hero.IsValidTarget(_q.Range))
                {
                    // E target is hero
                    foreach (var target in HeroManager.AllHeroes.Where(x => x.IsValidTarget(_e.Range,false) && x.Distance(hero.Position) <= _q.Range)
                        .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                    {
                        _e.Cast(target);
                        _q.Cast(hero);
                        _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                        _q2.Cast(hero);
                    }
                    // E target is minion
                    foreach (var target in MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.All).Where(x => x.IsValidTarget(_e.Range,false)
                        && !x.Name.ToLower().Contains("ward") && x.Distance(hero.Position) <= _q.Range)
                            .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                    {
                        // target die with E ?
                        if (!target.IsAlly && target.Health > _e.GetDamage(target) || target.IsAlly)
                        {
                            _e.Cast(target);
                            _q.Cast(hero);
                            _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                            _q2.Cast(hero);
                        }
                    }
                }
            }
        }
        public static void Harass()
        {
            if (Player.Mana * 100 / Player.MaxMana >= _menu.Item("ManaH").GetValue<Slider>().Value)
            {
                // cast Q
                if (_q.IsReady() && _menu.Item("QH").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical);
                    var target2 = TargetSelector.GetTarget(_q.Range, TargetSelector.DamageType.Magical, true, null, pix != null ? pix.Position : Player.Position);
                    if (target != null && target.IsValidTarget())
                        _q.Cast(target);
                    if (target2 != null && target2.IsValidTarget())
                    {
                        _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                        _q2.Cast(target2);
                    }
                }
                // cast E
                if (_e.IsReady() && _menu.Item("EH").GetValue<bool>())
                {
                    var target = TargetSelector.GetTarget(_e.Range, TargetSelector.DamageType.Magical);
                    if (target != null && target.IsValidTarget())
                        _e.Cast(target);
                }
                // Harass EQ
                if (_q.IsReady() && _e.IsReady() && Player.Mana >= _q.Instance.SData.Mana + _e.Instance.SData.Mana && _menu.Item("QEH").GetValue<bool>() && _menu.Item("QH").GetValue<bool>())
                {
                    var hero = TargetSelector.GetTarget(_q.Range + _e.Range,TargetSelector.DamageType.Magical);
                    if (hero != null && hero.IsValidTarget() && !hero.IsValidTarget(_q.Range))
                    {
                        // E target is hero
                        foreach (var target in HeroManager.AllHeroes.Where(x => x.IsValidTarget(_e.Range,false) && x.Distance(hero.Position) <= _q.Range)
                            .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                        {
                            _e.Cast(target);
                            _q.Cast(hero);
                            _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                            _q2.Cast(hero);
                        }
                        // E target is minion
                        foreach (var target in MinionManager.GetMinions(_e.Range, MinionTypes.All, MinionTeam.All).Where(x => x.IsValidTarget(_e.Range,false)
                            && !x.Name.ToLower().Contains("ward") && x.Distance(hero.Position) <= _q.Range)
                                .OrderByDescending(y => 1 - y.Distance(hero.Position)))
                        {
                            // target die with E ?
                            if (!target.IsAlly && target.Health > _e.GetDamage(target) || target.IsAlly)
                            {
                                _e.Cast(target);
                                _q.Cast(hero);
                                _q2.SetSkillshot(0.25f, 70, 1450, false, SkillshotType.SkillshotLine, pix.Position, pix.Position);
                                _q2.Cast(hero);
                            }
                        }
                    }
                }
            }
        }
        // get Pix!
        public static void Getpixed()
        {
            if (Player.IsDead)
                pix = Player;
            if (!Player.IsDead)
                pix = ObjectManager.Get<GameObject>().Find(x => x.IsAlly && x.Name == "RobotBuddy") == null ? 
                    Player : ObjectManager.Get<GameObject>().Find(x => x.IsAlly && x.Name == "RobotBuddy");
        }
        // undertower from BrianSharp
        private static bool UnderTower(Vector3 pos)
        {
            return
                ObjectManager.Get<Obj_AI_Turret>()
                    .Any(i => i.IsEnemy && !i.IsDead && i.Distance(pos) < 850 + Player.BoundingRadius);
        }
    }
}
