using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SPrediction;
using vSupport_Series.Core.Database;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{
    public class Janna : Helper
    {
        public static Menu Config;
        public static Spell Q, W, E, R;
        public static Orbwalking.Orbwalker Orbwalker;

        public Janna()
        {
            JannaOnLoad();
        }

        private static void JannaOnLoad()
        {
            if (ObjectManager.Player.ChampionName != "Janna")
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

                    esettings.AddItem(new MenuItem("use.e.turret", "Use (E) On Turret").SetValue(true))
                    .ValueChanged += (s, ar) =>
                    {
                        Config.Item("turret.hp.percent").Show(ar.GetNewValue<bool>());
                    };
                    esettings.AddItem(new MenuItem("turret.hp.percent", "Turret HP Percent").SetValue(new Slider(10, 1, 99))).Show(esettings.Item("use.e.turret").GetValue<bool>());

                    esettings.AddItem(new MenuItem("evade.protector.E", "If enemy spell damage bigger than carry health cast (E) for protect").SetValue(true));
                    esettings.AddItem(new MenuItem("protect.carry.from.turret", "Protect Carry From Turrets").SetValue(true));
                    esettings.AddItem(new MenuItem("min.mana.for.e", "Min. Mana").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(esettings);
                }

                var rsettings = new Menu(":: R Settings", ":: R Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold); ;
                {
                    var evademenu = new Menu(":: Protectable Skillshots", ":: Protectable Skillshots");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.EvadeableSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsSkillshot)))
                        {
                            evademenu.AddItem(new MenuItem(string.Format("r.protect.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        rsettings.AddSubMenu(evademenu);
                    }
                    var targettedmenu = new Menu(":: Protectable Targetted Spells", ":: Protectable Targetted Spells");
                    {
                        foreach (var spell in HeroManager.Enemies.SelectMany(enemy => SpellDatabase.TargetedSpells.Where(p => p.ChampionName == enemy.ChampionName && p.IsTargeted)))
                        {
                            targettedmenu.AddItem(new MenuItem(string.Format("r.protect.targetted.{0}", spell.SpellName), string.Format("{0} ({1})", spell.ChampionName, spell.Slot)).SetValue(true));
                        }
                        rsettings.AddSubMenu(targettedmenu);
                    }

                    var rwhitelist = new Menu(":: Whitelist", ":: Whitelist");
                    {
                        foreach (var ally in HeroManager.Allies.Where(x => x.IsValid))
                        {
                            rwhitelist.AddItem(new MenuItem("r." + ally.ChampionName, "(R): " + ally.ChampionName).SetValue(HighChamps.Contains(ally.ChampionName)));
                        }
                        rsettings.AddSubMenu(rwhitelist);
                    }

                    rsettings.AddItem(
                        new MenuItem("protector.settings", "Protector Mode: ").SetValue(new StringList(new string[] { "Smart (Other logics soon™)" })));
                    rsettings.AddItem(new MenuItem("spell.damage.percent", "Min. Spell Damage Percentage").SetValue(new Slider(10, 1, 99)));
                    /*rsettings.AddItem(new MenuItem("total.carry.health.percent", "Total Carries HP Percentage").SetValue(new Slider(10, 1, 99)));*/
                    rsettings.AddItem(new MenuItem("evade.protector", "If enemy spell damage bigger than carry health cast (R) for protect").SetValue(true));
                    Config.AddSubMenu(rsettings);
                }

                var drawing = new Menu(":: Draw Settings", ":: Draw Settings");
                {
                    drawing.AddItem(new MenuItem("janna.q.draw", "Q Range").SetValue(new Circle(true, Color.Chartreuse)));
                    drawing.AddItem(new MenuItem("janna.w.draw", "W Range").SetValue(new Circle(true, Color.Yellow)));
                    drawing.AddItem(new MenuItem("janna.e.draw", "E Range").SetValue(new Circle(true, Color.White)));
                    drawing.AddItem(new MenuItem("janna.r.draw", "R Range").SetValue(new Circle(true, Color.SandyBrown)));
                    Config.AddSubMenu(drawing);
                }

                Config.AddToMainMenu();
            }

            SPrediction.Prediction.Initialize(Config);

            Obj_AI_Base.OnSpellCast += JannaOnProcess;
            AntiGapcloser.OnEnemyGapcloser += JannaOnGapcloser;
            Game.OnUpdate += JannaOnUpdate;
            Drawing.OnDraw += JannaOnDraw;
        }

        private static void JannaOnDraw(EventArgs args)
        {
            if (Q.IsReady() && ActiveCheck("janna.q.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, GetColor("janna.q.draw", Config));
            }
            if (W.IsReady() && ActiveCheck("janna.w.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, GetColor("janna.w.draw", Config));
            }
            if (E.IsReady() && ActiveCheck("janna.e.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, GetColor("janna.e.draw", Config));
            }
            if (R.IsReady() && ActiveCheck("janna.r.draw", Config))
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, GetColor("janna.r.draw", Config));
            }
        }

        private static void JannaOnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
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

                if (sender is Obj_AI_Turret && args.Target.IsAlly && ObjectManager.Player.ManaPercent >= Config.Item("min.mana.for.e").GetValue<Slider>().Value
                    && Config.Item("e." + ((AIHeroClient)args.Target).ChampionName).GetValue<bool>() && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < E.Range
                    && Config.Item("protect.carry.from.turret").GetValue<bool>())
                {
                    E.Cast((AIHeroClient)args.Target);
                }

                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                    && !args.SData.IsAutoAttack() && (Config.Item("e.protect." + args.SData.Name).GetValue<bool>() || Config.Item("e.protect.targetted." + args.SData.Name).GetValue<bool>())
                    && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health)
                {
                    E.Cast((AIHeroClient)args.Target);
                }

                if (sender is AIHeroClient && sender.IsEnemy && args.Target.IsAlly && args.Target.Type == GameObjectType.obj_AI_Turret
                    && args.SData.IsAutoAttack() && ObjectManager.Player.ManaPercent >= Config.Item("min.mana.for.e").GetValue<Slider>().Value
                    && ((AIHeroClient)args.Target).Distance(ObjectManager.Player.Position) < E.Range
                    && ((AIHeroClient)args.Target).HealthPercent < Config.Item("turret.hp.percent").GetValue<Slider>().Value)
                {
                    E.Cast((AIHeroClient)args.Target);
                }


            }

            if (R.IsReady())
            {
                if (sender is AIHeroClient && args.Target.IsAlly && args.Target.Type == GameObjectType.AIHeroClient
                && !args.SData.IsAutoAttack() && (Config.Item("r.protect." + args.SData.Name).GetValue<bool>() || Config.Item("r.protect.targetted." + args.SData.Name).GetValue<bool>())
                && sender.IsEnemy && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) > ((AIHeroClient)args.Target).Health
                && sender.GetSpellDamage(((AIHeroClient)args.Target), args.SData.Name) * 100 / ((AIHeroClient)args.Target).Health < Config.Item("spell.damage.percent").GetValue<Slider>().Value)
                {
                    R.Cast((AIHeroClient)args.Target);
                }
            }

        }

        private static void JannaOnGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsEnemy && gapcloser.End.Distance(ObjectManager.Player.Position) < 200 &&
               gapcloser.Sender.IsValidTarget(Q.Range) && Config.Item("q.antigapcloser").GetValue<bool>())
            {
                Q.Cast(gapcloser.Sender);
            }
        }

        private static int CarriesCount()
        {
            return
                HeroManager.Allies.Count(
                    x =>
                        HighChamps.Contains(x.ChampionName) &&
                        x.Distance(ObjectManager.Player.Position) < R.Range && !x.IsDead && x.IsVisible && !x.IsZombie);
        }

        private static void JannaOnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
            } 
        }

        private static void OnCombo()
        {
            if (Config.Item("q.combo").GetValue<bool>() && Q.IsReady())
            {
                switch (Config.Item("q.settings").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                        {
                            Q.SPredictionCast(enemy, SpellHitChance(Config, "q.normal.hit.chance"));
                        }
                        break;
                    case 1:
                        if (ObjectManager.Player.CountEnemiesInRange(Q.Range) >= Config.Item("q.hit.count").GetValue<Slider>().Value)
                        {
                            foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Q.GetAoeSPrediction().HitCount >= Config.Item("q.hit.count").GetValue<Slider>().Value))
                            {
                                Q.Cast(Q.GetAoeSPrediction().CastPosition);
                            }
                        }
                        break;
                }
            }
            if (Config.Item("w.combo").GetValue<bool>() && Q.IsReady())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    W.CastOnUnit(enemy);
                }
            }
        }
    }
}
