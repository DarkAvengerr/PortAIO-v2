using System;
using System.Linq;
using LeagueSharp;
using System.Drawing;
using LeagueSharp.Common;
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FuckingLucianReborn
{
    public class Program
    {
        private static Menu _config;
        private static int _lastTick;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Spell _q = new Spell(SpellSlot.Q);
        private static Spell _q2 = new Spell(SpellSlot.Q);
        private static Spell _w = new Spell(SpellSlot.W);
        private static Spell _w2 = new Spell(SpellSlot.W);
        private static Spell _e = new Spell(SpellSlot.E);
        private static string[] select = {"Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jinx", "Kalista", "KogMaw", "Lucian", "MissFortune","Quinn","Sivir","Teemo","Tristana","TwistedFate","Twitch","Urgot","Varus","Vayne"};
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != "Lucian")
            {
                return;
            }
            _q2.SetSkillshot(0.25f, 70, 3000, false, SkillshotType.SkillshotLine);
            _w.SetSkillshot(0.25f, 70, 1500, false, SkillshotType.SkillshotLine);
            _w.MinHitChance = HitChance.Medium;
            _w2.SetSkillshot(0.25f, 70, 1500, true, SkillshotType.SkillshotLine);
            _config = new Menu("FuckingLucianReborn", "FuckingLucianReborn", true);
            _orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _config.AddSubMenu(targetSelectorMenu);
            _config.SubMenu("Combo").SubMenu("Q Extended settings").AddItem(new MenuItem("qexcomhero", "Q Only Certain Champions").SetValue(false));
            foreach (var hero in HeroManager.Enemies) { _config.SubMenu("Combo").SubMenu("Q Extended settings").SubMenu("Certain Champions").AddItem(new MenuItem("autocom" + hero.ChampionName, hero.ChampionName).SetValue(select.Contains(hero.ChampionName))); }
            _config.SubMenu("Combo").SubMenu("Q Extended settings").AddItem(new MenuItem("manac", "Mana").SetValue(new Slider(33, 100, 0)));
            _config.SubMenu("Combo").SubMenu("E settings").AddItem(new MenuItem("emodswitch", "Switch Key").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            _config.SubMenu("Combo").SubMenu("E settings").AddItem(new MenuItem("emod", "E Mode").SetValue(new StringList(new[]{"Safe", "To mouse", "None"})));
            _config.SubMenu("Combo").AddItem(new MenuItem("qcom", "Q").SetValue(true));
            _config.SubMenu("Combo").AddItem(new MenuItem("qexcom", "Q Extended").SetValue(true));
            _config.SubMenu("Combo").AddItem(new MenuItem("wcom", "W").SetValue(true));
            _config.SubMenu("Combo").SubMenu("Items").SubMenu("Botrk").AddItem(new MenuItem("Botrk", "Botrk").SetValue(true));
            _config.SubMenu("Combo").SubMenu("Items").SubMenu("Cutlass").AddItem(new MenuItem("Cutlass", "Cutlass").SetValue(true));
            _config.SubMenu("Combo").SubMenu("Items").SubMenu("Youmuus").AddItem(new MenuItem("Youmuus", "Youmuus").SetValue(true));
            _config.SubMenu("Killsteal").AddItem(new MenuItem("qkil", "Q").SetValue(true));
            _config.SubMenu("Killsteal").AddItem(new MenuItem("qexkil", "Q Extended").SetValue(true));
            _config.SubMenu("Killsteal").AddItem(new MenuItem("wkil", "W").SetValue(true));
            _config.SubMenu("Harass").AddItem(new MenuItem("qexharhero", "Q Only Certain Champions").SetValue(true));
            foreach (var hero in HeroManager.Enemies) { _config.SubMenu("Harass").SubMenu("Certain Champions").AddItem(new MenuItem("autohar" + hero.ChampionName, hero.ChampionName).SetValue(select.Contains(hero.ChampionName))); }
            _config.SubMenu("Harass").AddItem(new MenuItem("manah", "Mana").SetValue(new Slider(33, 100, 0)));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("QExRange", "Q Extended range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("EaaRange", "E + AA range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").SubMenu("Spells").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(true, Color.FromArgb(150, Color.DodgerBlue))));
            _config.SubMenu("Drawings").AddItem(new MenuItem("emodraw", "E Mode Text").SetValue(true));
            _config.SubMenu("Drawings").AddItem(new MenuItem("tdraw", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
            _config.AddToMainMenu();
            Obj_AI_Base.OnProcessSpellCast += oncast;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            
            Chat.Print("<b><font color=\"#04EECA\">Fucking</font> <font color=\"#DC0DA1\">Lucian</font> <font color=\"#FF0000\">Reborn</font> <font color=\"#FFFFFF\">by</font> <font color=\"#FFEB00\">folxu</font> <font color=\"#00FF2F\">Loaded!</font></b>");
            Chat.Print("<b><font color=\"#FFA600\">Working on 5.18</font></b>");
            Chat.Print("<b><font color=\"#FF00F3\">GL HF !</font></b>");
        }
        //Reset Auto Attack After Spells
        private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
            {
                return;
            }
            if (spell.Name.ToLower().Contains("lucianq") || spell.Name.ToLower().Contains("lucianw") || spell.Name.ToLower().Contains("luciane"))
            {
                LeagueSharp.Common.Utility.DelayAction.Add(450, Orbwalking.ResetAutoAttackTimer);
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            Emode();
            var cerha = _config.Item("qexharhero").GetValue<bool>();
            if (cerha)
            {
                var manh = _config.Item("manah").GetValue<Slider>().Value;
                if (_q.IsReady() && (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100 > manh && (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit))
                {
                    var thc = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault(hero => _config.Item("autohar" + hero.ChampionName).GetValue<bool>());
                    qexcast(thc);
                }
            }
            else
            {
                var manh = _config.Item("manah").GetValue<Slider>().Value;
                if (_q.IsReady() && (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100 > manh && (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed || _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit))
                {
                    var ch = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault();
                    qexcast(ch);
                }
            }
            var qc = _config.Item("qexcom").GetValue<bool>();
            if (qc)
            {
                var cerco = _config.Item("qexcomhero").GetValue<bool>();
                if (cerco)
                {
                    var manc = _config.Item("manac").GetValue<Slider>().Value;
                    if (_q.IsReady() && (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100 > manc && _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        var tcc = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault(hero => _config.Item("autocom" + hero.ChampionName).GetValue<bool>());
                        qexcast(tcc);
                    }
                }
                else
                {
                    var manc = _config.Item("manac").GetValue<Slider>().Value;
                    if (_q.IsReady() && (ObjectManager.Player.Mana/ObjectManager.Player.MaxMana)*100 > manc && _orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    {
                        var ch = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault();
                        qexcast(ch);
                    }
                }
            }
            var qk = _config.Item("qkil").GetValue<bool>();
            if (qk && _q.IsReady())
            {
                var kq = HeroManager.Enemies.Where(hero => hero.IsValidTarget(675)).FirstOrDefault(hero => ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) >= hero.Health);
                _q.CastOnUnit(kq);
            }
            var qke = _config.Item("qexkil").GetValue<bool>();
            if (qke && _q.IsReady())
            {
                var kqe = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1150)).Where(hero => hero.Distance(ObjectManager.Player) > 675).FirstOrDefault(hero => ObjectManager.Player.GetSpellDamage(hero, SpellSlot.Q) >= hero.Health);
                qexcast(kqe);
            }
            var wk = _config.Item("wkil").GetValue<bool>();
            if (wk && _w.IsReady())
            {
                var kw = HeroManager.Enemies.Where(hero => hero.IsValidTarget(1000)).FirstOrDefault(hero => ObjectManager.Player.GetSpellDamage(hero, SpellSlot.W) >= hero.Health);
                var WPred = _w2.GetPrediction(kw);
                if (WPred.Hitchance >= HitChance.High)
                {
                    _w2.Cast(WPred.CastPosition);
                }
            }
        }
        private static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var enemy = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
            if (enemy.IsValidTarget())
            {
                var quse = _config.Item("qcom").GetValue<bool>();
                var wuse = _config.Item("wcom").GetValue<bool>();
                var botuse = _config.Item("Botrk").GetValue<bool>();
                var cutuse = _config.Item("Cutlass").GetValue<bool>();
                var youuse = _config.Item("Youmuus").GetValue<bool>();
                if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (quse)
                    {
                        if (_q.IsReady())
                        {
                            _q.CastOnUnit(enemy);
                        }
                        else
                        {
                            if (_w.IsReady() && wuse)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(200, Wuse);
                            }
                        }
                    }
                }
                if (_orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    var EMode =_config.Item("emod").GetValue<StringList>().SelectedIndex;
                    var obj = (Obj_AI_Base) target;
                    var pos = Geometry.CircleCircleIntersection(ObjectManager.Player.ServerPosition.To2D(), Prediction.GetPrediction(obj, 0.25f).UnitPosition.To2D(), 425, Orbwalking.GetRealAutoAttackRange(obj));
                    switch (EMode)
                    {
                        case 0:
                            if (_e.IsReady())
                            {
                                if (pos.Count() > 0)
                                {
                                    _e.Cast(pos.MinOrDefault(i => i.Distance(Game.CursorPos)));
                                }
                                else
                                {
                                    _e.Cast(ObjectManager.Player.ServerPosition.Extend(obj.ServerPosition, -425));
                                }
                            }
                            else
                            {
                                if (_q.IsReady() && quse)
                                {
                                    _q.CastOnUnit(enemy);
                                }
                                else
                                {
                                    if (_w.IsReady() && wuse)
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(200, Wuse);
                                    }
                                }
                            }
                        break;
                        case 1:
                            if (_e.IsReady())
                            {
                                _e.Cast(ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 425));
                            }
                            else
                            {
                                if (_q.IsReady() && quse)
                                {
                                    _q.CastOnUnit(enemy);
                                }
                                else
                                {
                                    if (_w.IsReady() && wuse)
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(200, Wuse);
                                    }
                                }
                            }
                        break;
                        case 2:
                            if (_q.IsReady() && quse)
                            {
                                _q.CastOnUnit(enemy);
                            }
                            else
                            {
                                if (_w.IsReady() && wuse)
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(200, Wuse);
                                }
                            }
                        break;
                    }
                    if (enemy.Distance(ObjectManager.Player) < 550)
                    {
                        if (botuse && Items.HasItem(3153) && Items.CanUseItem(3153))
                        {
                            Items.UseItem(3153, enemy);
                        }
                        if (cutuse && Items.HasItem(3144) && Items.CanUseItem(3144))
                        {
                            Items.UseItem(3144, enemy);
                        }
                        if (youuse && Items.HasItem(3142) && Items.CanUseItem(3142))
                        {
                            Items.UseItem(3142);
                        }
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            var wts = Drawing.WorldToScreen(ObjectManager.Player.Position);
            var drt = _config.Item("tdraw").GetValue<Circle>();
            var emdraw = _config.Item("emodraw").GetValue<bool>();
            var EMode =_config.Item("emod").GetValue<StringList>().SelectedIndex;
            var Qran = _config.Item("QRange").GetValue<Circle>();
            var Qexran = _config.Item("QExRange").GetValue<Circle>();
            var Wran = _config.Item("WRange").GetValue<Circle>();
            var Eran = _config.Item("ERange").GetValue<Circle>();
            var Eaaran = _config.Item("EaaRange").GetValue<Circle>();
            var Rran = _config.Item("RRange").GetValue<Circle>();
            if (drt.Active)
            {
                var td = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
                if (td != null && td.IsValidTarget())
                {
                    Render.Circle.DrawCircle(td.Position, 115f, drt.Color, 1);
                }
            }
            if (emdraw)
            {
                switch (EMode)
                {
                    case 0:
                        Drawing.DrawText(wts[0], wts[1], Color.White, "Safe");
                    break;
                    case 1:
                        Drawing.DrawText(wts[0], wts[1], Color.White, "To mouse");
                    break;
                    case 2:
                        Drawing.DrawText(wts[0], wts[1], Color.White, "None");
                    break;
                }
            }
            if (Qran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 675, Qran.Color);
            }
            if (Qexran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1150, Qexran.Color);
            }
            if (Wran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1000, Wran.Color);
            }
            if (Eran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 425, Eran.Color);
            }
            if (Eaaran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 425 + Orbwalking.GetRealAutoAttackRange(ObjectManager.Player), Eaaran.Color);
            }
            if (Rran.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, 1400, Rran.Color);
            }
        }
        private static void Emode()
        {
            var lasttime = Environment.TickCount - _lastTick;
            var EMode = _config.Item("emod").GetValue<StringList>().SelectedIndex;
            if (!_config.Item("emodswitch").GetValue<KeyBind>().Active || lasttime <= Game.Ping)
            {
                return;
            }
            switch (EMode)
            {
                case 0:
                    _config.Item("emod").SetValue(new StringList(new[]{"Safe", "To mouse", "None"}, 1));
                    _lastTick = Environment.TickCount + 300;
                break;
                case 1:
                    _config.Item("emod").SetValue(new StringList(new[]{"Safe", "To mouse", "None"}, 2));
                    _lastTick = Environment.TickCount + 300;
                break;
                case 2:
                    _config.Item("emod").SetValue(new StringList(new[]{"Safe", "To mouse", "None"}));
                    _lastTick = Environment.TickCount + 300;
                break;
            }
        }
        private static void qexcast(AIHeroClient unit)
        {
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, 675, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in minions)
            {
                if (_q2.WillHit(unit, ObjectManager.Player.ServerPosition.Extend(minion.ServerPosition, 1150), 0, HitChance.VeryHigh))
                {
                    _q2.CastOnUnit(minion);
                }
            }
        }
        private static void Wuse()
        {
            var t = TargetSelector.GetTarget(700, TargetSelector.DamageType.Physical);
            _w.Cast(t);
        }
    }
}
