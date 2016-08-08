using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Fiddlesticks : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static vSupport_Series.Core.Plugins.Orbwalking.Orbwalker Orbwalker;
        public static float LastW;

        public Fiddlesticks()
        {
            FiddlestickOnLoad();
        }

        public static bool Wable
        {
            get { return Game.Time - LastW > 2; }
        }
        public static bool IsWActive
        {
            get { return ObjectManager.Player.LSHasBuff("fiddlebuff"); }
        }
        private static void FiddlestickOnLoad()
        {
            Q = new Spell(SpellSlot.Q, 575);
            W = new Spell(SpellSlot.W, 575);
            E = new Spell(SpellSlot.E, 750);
            R = new Spell(SpellSlot.R, 800);

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));

                var comboMenu = new Menu("Combo Settings", "Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("fid.q.combo", "Use Q").SetValue(true));
                    comboMenu.AddItem(new MenuItem("fid.w.combo", "Use W").SetValue(true));
                    comboMenu.AddItem(new MenuItem("fid.e.combo", "Use E").SetValue(true));
                    Config.AddSubMenu(comboMenu);
                }
                var qMenu = new Menu("Q Settings", "Q Settings");
                {
                    var qWhite = new Menu("Q Whitelist", "Q Whitelist");
                    {
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            qWhite.AddItem(new MenuItem("fid.q.enemy." + enemy.ChampionName, string.Format("Q: {0}", enemy.CharData.BaseSkinName)).SetValue(HighChamps.Contains(enemy.CharData.BaseSkinName)));

                        }
                        qMenu.AddSubMenu(qWhite);
                    }
                    qMenu.AddItem(new MenuItem("fid.auto.q.immobile", "Auto (Q) If Enemy Immobile").SetValue(true));
                    qMenu.AddItem(new MenuItem("fid.auto.q.channeling", "Auto (Q) If Enemy Casting Channeling Spell").SetValue(true));
                    Config.AddSubMenu(qMenu);
                }

                var wMenu = new Menu("W Settings", "W Settings");
                {
                    var wHite = new Menu("W Whitelist", "W Whitelist");
                    {
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            wHite.AddItem(new MenuItem("fid.w.enemy." + enemy.ChampionName, string.Format("W: {0}", enemy.CharData.BaseSkinName)).SetValue(HighChamps.Contains(enemy.CharData.BaseSkinName)));

                        }
                        wMenu.AddSubMenu(wHite);
                    }
                    Config.AddSubMenu(wMenu);
                }

                var eMenu = new Menu("E Settings", "E Settings");
                {
                    var eWhite = new Menu("E Whitelist", "E Whitelist");
                    {
                        foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(o => o.IsEnemy))
                        {
                            eWhite.AddItem(new MenuItem("fid.e.enemy." + enemy.ChampionName, string.Format("E: {0}", enemy.CharData.BaseSkinName)).SetValue(HighChamps.Contains(enemy.CharData.BaseSkinName)));

                        }
                        eMenu.AddSubMenu(eWhite);

                        eMenu.AddItem(new MenuItem("fid.e.enemy.count", "(E) Min. Enemy").SetValue(new Slider(2, 1, 5)));
                        eMenu.AddItem(new MenuItem("fid.auto.e.enemy.immobile", "Auto (E) If Enemy Immobile").SetValue(true));
                        eMenu.AddItem(new MenuItem("fid.auto.e.enemy.channeling", "Auto (E) If Enemy Casting Channeling Spell").SetValue(true));
                        Config.AddSubMenu(eMenu);
                    }

                    var harassMenu = new Menu("Harass Settings", "Harass Settings");
                    {
                        harassMenu.AddItem(new MenuItem("fid.q.harass", "Use Q").SetValue(true));
                        harassMenu.AddItem(new MenuItem("fid.e.harass", "Use E").SetValue(true));
                        harassMenu.AddItem(new MenuItem("fid.harass.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                        Config.AddSubMenu(harassMenu);
                    }

                    var clearMenu = new Menu("Clear Settings", "Clear Settings");
                    {
                        clearMenu.AddItem(new MenuItem("fid.w.clear", "Use W").SetValue(true));
                        clearMenu.AddItem(new MenuItem("fid.e.clear", "Use E").SetValue(true));
                        clearMenu.AddItem(new MenuItem("fid.e.minion.hit.count", "(E) Min. Minion").SetValue(new Slider(3, 1, 5)));
                        clearMenu.AddItem(new MenuItem("fid.clear.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                        Config.AddSubMenu(clearMenu);
                    }

                    var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
                    {
                        jungleMenu.AddItem(new MenuItem("fid.q.jungle", "Use Q").SetValue(true));
                        jungleMenu.AddItem(new MenuItem("fid.w.jungle", "Use W").SetValue(true));
                        jungleMenu.AddItem(new MenuItem("fid.e.jungle", "Use E").SetValue(true));
                        jungleMenu.AddItem(new MenuItem("fid.jungle.mana", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                        Config.AddSubMenu(jungleMenu);
                    }
                    var drawMenu = new Menu("Draw Settings", "Draw Settings");
                    {
                        drawMenu.AddItem(new MenuItem("fid.q.draw", "Q Range").SetValue(new Circle(true, Color.White)));
                        drawMenu.AddItem(new MenuItem("fid.w.draw", "W Range").SetValue(new Circle(true, Color.DarkSeaGreen)));
                        drawMenu.AddItem(new MenuItem("fid.e.draw", "E Range").SetValue(new Circle(true, Color.Gold)));
                        drawMenu.AddItem(new MenuItem("fid.r.draw", "R Range").SetValue(new Circle(true, Color.DodgerBlue)));
                        Config.AddSubMenu(drawMenu);
                    }
                }
            }
            Config.AddToMainMenu();
            Obj_AI_Base.OnProcessSpellCast += FiddleOnProcessSpellCast;
            Game.OnUpdate += FiddleOnUpdate;
            Drawing.OnDraw += FiddleOnDraw;
        }


        private static void FiddleOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Wable && IsWActive && args.Slot == SpellSlot.W)
            {
                LastW = Game.Time;
            }
        }

        private static void FiddleOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Jungle();
                    WaveClear();
                    break;

            }

            if (MenuCheck("fid.auto.q.immobile", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && MenuCheck("fid.q.enemy." + x.ChampionName, Config) && IsEnemyImmobile(x)))
                {
                    Q.Cast(enemy);
                }
            }
            if (MenuCheck("fid.auto.q.channeling", Config) && Q.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(Q.Range) && MenuCheck("fid.q.enemy." + x.ChampionName, Config) && x.IsChannelingImportantSpell()))
                {
                    Q.Cast(enemy);
                }
            }
            if (MenuCheck("fid.auto.e.immobile", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range) && MenuCheck("fid.e.enemy." + x.ChampionName, Config) && IsEnemyImmobile(x)))
                {
                    E.Cast(enemy);
                }
            }
            if (MenuCheck("fid.auto.e.channeling", Config) && E.LSIsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.LSIsValidTarget(E.Range) && MenuCheck("fid.e.enemy." + x.ChampionName, Config) && x.IsChannelingImportantSpell()))
                {
                    E.Cast(enemy);
                }
            } 
            
        }

        private static void Combo()
        {
            if (Q.LSIsReady() && MenuCheck("fid.q.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (MenuCheck("fid.q.enemy." + enemy.ChampionName, Config))
                    {
                        Q.CastOnUnit(enemy);
                    }
                }
            }
            if (W.LSIsReady() && MenuCheck("fid.w.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(W.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (MenuCheck("fid.w.enemy." + enemy.ChampionName, Config))
                    {
                        W.CastOnUnit(enemy);
                    }
                }
            }
            if (E.LSIsReady() && MenuCheck("fid.e.combo", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (MenuCheck("fid.e.enemy." + enemy.ChampionName, Config) && enemy.LSCountEnemiesInRange(E.Range) >= SliderCheck("fid.e.enemy.count", Config))
                    {
                        E.CastOnUnit(enemy);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("fid.harass.mana", Config))
            {
                return;
            }

            if (Q.LSIsReady() && MenuCheck("fid.q.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(Q.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (MenuCheck("fid.q.enemy." + enemy.ChampionName, Config))
                    {
                        Q.CastOnUnit(enemy);
                    }
                }
            }
            if (E.LSIsReady() && MenuCheck("fid.e.harass", Config))
            {
                foreach (var enemy in HeroManager.Enemies.Where(o => o.LSIsValidTarget(E.Range) && !o.IsDead && !o.IsZombie))
                {
                    if (MenuCheck("fid.e.enemy." + enemy.ChampionName, Config))
                    {
                        E.CastOnUnit(enemy);
                    }
                }
            }
        }

        private static void Jungle()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("fid.jungle.mana", Config))
            {
                return;
            }

            var mob = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (mob.Count > 0)
            {
                if (Q.LSIsReady() && MenuCheck("fid.q.jungle", Config))
                {
                    Q.CastOnUnit(mob[0]);
                }
                if (W.LSIsReady() && MenuCheck("fid.w.jungle", Config))
                {
                    W.CastOnUnit(mob[0]);
                }
                if (E.LSIsReady() && MenuCheck("fid.e.jungle", Config))
                {
                    E.CastOnUnit(mob[0]);
                }
            }
        }

        private static void WaveClear()
        {
            if (ObjectManager.Player.ManaPercent < SliderCheck("fid.clear.mana", Config))
            {
                return;
            }

            var min = MinionManager.GetMinions(ObjectManager.Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (E.LSIsReady() && MenuCheck("fid.e.clear", Config))
            {
                if (min.Count > SliderCheck("fid.e.minion.hit.count", Config))
                {
                    E.CastOnUnit(min[0]);
                }
            }
            if (W.LSIsReady() && MenuCheck("fid.w.clear", Config))
            {
                W.CastOnUnit(min[0]);
            }
        }

        private static void FiddleOnDraw(EventArgs args)
        {
            if (Q.LSIsReady() && ActiveCheck("fid.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("fid.q.draw", Config));
            }
            if (W.LSIsReady() && ActiveCheck("fid.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("fid.w.draw", Config));
            }
            if (E.LSIsReady() && ActiveCheck("fid.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("fid.e.draw", Config));
            }
            if (R.LSIsReady() && ActiveCheck("fid.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("fid.r.draw", Config));
            }
        }
    }
}
