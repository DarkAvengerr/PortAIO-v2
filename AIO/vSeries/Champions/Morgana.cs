using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using vSupport_Series.Core.Database;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Morgana : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public Morgana()
        {
            MorganaOnLoad();
        }

        private static void MorganaOnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Morgana")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 850);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 550);

            Q.SetSkillshot(0.25f, 120f, 900f, false, SkillshotType.SkillshotLine);

            SpellDatabase.InitalizeSpellDatabase();

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker Settings"));
                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    comboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                    comboMenu.AddItem(new MenuItem("min.enemies.r", "Min. Enemies in Range to (R)").SetValue(new Slider(2, 1, 5)));

                    Config.AddSubMenu(comboMenu);
                }
                var qsettings = new Menu(":: Q Settings", ":: Q Settings");
                {

                    qsettings.AddItem(new MenuItem("q.settings", "(Q) Mode :").SetValue(new StringList(new string[] { "Normal", "Q Hit x Target" })))
                    .ValueChanged += (s, ar) =>
                    {
                        Config.Item("normal.q.info.1").Show(ar.GetNewValue<StringList>().SelectedIndex == 0);
                        Config.Item("normal.q.info.2").Show(ar.GetNewValue<StringList>().SelectedIndex == 0);
                        Config.Item("q.normal.hit.chance").Show(ar.GetNewValue<StringList>().SelectedIndex == 0);
                        Config.Item("q.hit.x.chance").Show(ar.GetNewValue<StringList>().SelectedIndex == 1);
                        Config.Item("q.hit.count").Show(ar.GetNewValue<StringList>().SelectedIndex == 1);
                        Config.Item("q.hit.x.chance.info.1").Show(ar.GetNewValue<StringList>().SelectedIndex == 1);
                        Config.Item("q.hit.x.chance.info.2").Show(ar.GetNewValue<StringList>().SelectedIndex == 1);
                    };

                    qsettings.AddItem(new MenuItem("q.normal.hit.chance", "(Q) Hit Chance").SetValue(new StringList(HitchanceNameArray, 2))).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 0);
                    qsettings.AddItem(new MenuItem("q.hit.x.chance", "(Q) Hit Chance").SetValue(new StringList(HitchanceNameArray, 1))).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 1);
                    qsettings.AddItem(new MenuItem("q.hit.count", "(Q) Hit Count").SetValue(new Slider(2, 1, 5))).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 1);
                    qsettings.AddItem(new MenuItem("normal.q.info.1", "                        :: Information ::").SetFontStyle(System.Drawing.FontStyle.Bold)).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 0);
                    qsettings.AddItem(new MenuItem("normal.q.info.2", "Thats casts q for 1 enemy")).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 0);
                    qsettings.AddItem(new MenuItem("q.hit.x.chance.info.1", "                        :: Information ::").SetFontStyle(System.Drawing.FontStyle.Bold)).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 1);
                    qsettings.AddItem(new MenuItem("q.hit.x.chance.info.2", "Thats cast q for x enemies. Set on menu")).Show(qsettings.Item("q.settings").GetValue<StringList>().SelectedIndex == 1);
                    qsettings.AddItem(new MenuItem("q.antigapcloser", "(Q) Anti-Gapcloser").SetValue(true));
                    Config.AddSubMenu(qsettings);
                }

                var esettings = new Menu(":: E Settings", ":: E Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.HotPink);
                {
                    var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                        {
                            evademenu.AddItem(new MenuItem(string.Format("e.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(evademenu);
                    }

                    var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                        {
                            targettedmenu.AddItem(new MenuItem(string.Format("e.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(targettedmenu);
                    }

                    var engagemenu = new Menu(":: Engage Spells", ":: Engage Spells");
                    {
                        foreach (var spell in HeroManager.Allies.SelectMany(ally => SpellDatabase.EscapeSpells.Where(p => p.ChampionName == ally.ChampionName)))
                        {
                            engagemenu.AddItem(new MenuItem(string.Format("e.engage.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        esettings.AddSubMenu(engagemenu);
                    }

                    var ewhitelist = new Menu(":: Whitelist", ":: Whitelist");
                    {
                        foreach (var ally in HeroManager.Allies.Where(x => x.IsValid))
                        {
                            ewhitelist.AddItem(new MenuItem("e." + ally.ChampionName, "(E): " + ally.ChampionName).SetValue(HighChamps.Contains(ally.ChampionName)));
                        }
                        esettings.AddSubMenu(ewhitelist);
                    }

                    esettings.AddItem(new MenuItem("evade.protector.E", "If enemy spell damage bigger than carry health cast (E) for protect").SetValue(true));
                    esettings.AddItem(new MenuItem("min.mana.for.e", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(esettings);
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("morgana.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("morgana.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("morgana.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("morgana.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }

                Config.AddToMainMenu();
            }

            SPrediction.Prediction.Initialize(Config);
            Config.AddItem(new MenuItem("morgana.q.hitchance", "Skillshot Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));

            Obj_AI_Base.OnProcessSpellCast += MorganaOnProcess;
            AntiGapcloser.OnEnemyGapcloser += MorganaOnGapcloser;
            Game.OnUpdate += MorganaOnUpdate;
            Drawing.OnDraw += MorganaOnDraw;
        }

        private static void MorganaOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("morgana.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("morgana.q.draw", Config));
            }
            if (W.IsReady() && ActiveCheck("morgana.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("morgana.w.draw", Config));
            }
            if (E.IsReady() && ActiveCheck("morgana.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("morgana.e.draw", Config));
            }
            if (R.IsReady() && ActiveCheck("morgana.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("morgana.r.draw", Config));
            }
        }

        private static void MorganaOnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (E.IsReady())
            {
                if (sender.IsAlly && sender is AIHeroClient && Config.Item("e.engage." + args.SData.Name).GetValue<bool>()
                && Config.Item("e." + sender.CharData.BaseSkinName).GetValue<bool>() && sender.Distance(ObjectManager.Player.Position) <= E.Range
                    && !sender.IsDead && !sender.IsZombie && sender.IsValid)
                {
                    E.CastOnUnit(sender);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && args.SData.IsAutoAttack() && ObjectManager.Player.ManaPercent >= Config.Item("min.mana.for.e").GetValue<Slider>().Value
                && Config.Item("e." + ((AIHeroClient)args.Target).ChampionName).GetValue<bool>() && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < E.Range)
                {
                    E.Cast((AIHeroClient)args.Target);
                }

                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                    && !args.SData.IsAutoAttack() && (Config.Item("e.protect." + args.SData.Name).GetValue<bool>() || Config.Item("e.protect.targetted." + args.SData.Name).GetValue<bool>())
                    && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health)
                {
                    E.Cast((AIHeroClient)args.Target);
                }
            }
        }

        private static void MorganaOnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.End.Distance(ObjectManager.Player.Position) < 200 &&
               gapcloser.Sender.IsValidTarget(Q.Range) && Config.Item("q.antigapcloser").GetValue<bool>())
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static void MorganaOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            } 
        }

        private static void Combo()
        {
            if (MenuCheck("q.combo", Config) && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) &&
                    MenuCheck("nautilus.q." + x.ChampionName, Config)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "morgana.q.hitchance"));
                }
            }

            if (Config.Item("w.combo").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.CastOnUnit(enemy);
                }
            }

            if (Config.Item("r.combo").GetValue<bool>() && R.IsReady())
            {
                if (MenuCheck("combo.r", Config) && R.IsReady() && ObjectManager.Player.CountEnemiesInRange(600f) >= SliderCheck("min.enemies.r", Config))
                {
                    R.Cast();
                }
            }
        }
    }
}
