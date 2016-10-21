using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.Common;
namespace Hikigaya_Syndra
{
    class Program
    {
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static Spell Q, W, E, R, Qe;
        public static int QwLastcast = 0;
        public static SpellSlot IgniteSlot;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;
        public static string[] HitchanceNameArray = { "Low", "Medium", "High", "Very High", "Only Immobile" };
        public static HitChance[] HitchanceArray = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh, HitChance.Immobile };
        public static HitChance HikiChance(string menuName)
        {
            return HitchanceArray[Config.Item(menuName).GetValue<StringList>().SelectedIndex];
        }
        // ReSharper disable once UnusedParameter.Local


        public static void OnLoad()
        {

            if (ObjectManager.Player.ChampionName != "Syndra")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 790, TargetSelector.DamageType.Magical);
            Q.SetSkillshot(0.6f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 925, TargetSelector.DamageType.Magical);
            W.SetSkillshot(0.25f, 140f, 1600f, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700, TargetSelector.DamageType.Magical);
            E.SetSkillshot(0.25f, (float)(45 * 0.5), 2500f, false, SkillshotType.SkillshotCircle);

            R = new Spell(SpellSlot.R, 675, TargetSelector.DamageType.Magical);
            R.SetTargetted(0.5f, 1100f);

            Qe = new Spell(SpellSlot.E, 1290);
            Qe.SetSkillshot(float.MaxValue, 55f, 2000f, false, SkillshotType.SkillshotCircle);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            Config = new Menu("Hikigaya Syndra", "Hikigaya Syndra", true);
            {
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu(":: Orbwalker Settings"));
                var comboMenu = new Menu(":: Combo Settings", ":: Combo Settings");
                {
                    var igniteMenu = new Menu(":: Ignite", ":: Ignite");
                    {
                        igniteMenu.AddItem(new MenuItem("use.ignite", "Use (Ignite)").SetValue(true).SetTooltip("If enemy killable with combo uses ignite"));
                        comboMenu.AddSubMenu(igniteMenu);
                    }

                    var qComboMenu = new Menu(":: Q", ":: Q");
                    {
                        qComboMenu.AddItem(new MenuItem("q.combo", "Use (Q)").SetValue(true));
                        qComboMenu.AddItem(new MenuItem("q.hit.chance", "(Q) Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
                        comboMenu.AddSubMenu(qComboMenu);
                    }

                    var qeComboMenu = new Menu(":: QE", ":: QE");
                    {
                        qeComboMenu.AddItem(new MenuItem("qe.combo", "Use (QE)").SetValue(true));
                        qeComboMenu.AddItem(new MenuItem("q.e.delay", "QE Delay").SetValue(new Slider(0, 0, 150)));
                        qeComboMenu.AddItem(new MenuItem("q.e.max.range", "QE Max Range").SetValue(new Slider(1290, 1, 1290)));
                        qeComboMenu.AddItem(new MenuItem("qe.combo.style", "» (E) Style").SetValue(new StringList(new[] { "If Enemy Stunable" })));
                        qeComboMenu.AddItem(new MenuItem("qe.hit.chance", "(QE) Hit Chance").SetValue(new StringList(HitchanceNameArray, 3)));
                        comboMenu.AddSubMenu(qeComboMenu);
                    }

                    var wComboMenu = new Menu(":: W", ":: W");
                    {
                        wComboMenu.AddItem(new MenuItem("w.combo", "Use (W)").SetValue(true));
                        wComboMenu.AddItem(new MenuItem("w.hit.chance", "(W) Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
                        comboMenu.AddSubMenu(wComboMenu);
                    }

                    var eComboMenu = new Menu(":: E", ":: E");
                    {
                        eComboMenu.AddItem(new MenuItem("e.combo", "Use (E)").SetValue(true));
                        eComboMenu.AddItem(new MenuItem("e.combo.style", "» (E) Style").SetValue(new StringList(new[] { "If Enemy Stunable" })));
                        eComboMenu.AddItem(new MenuItem("e.hit.chance", "(E) Hit Chance").SetValue(new StringList(HitchanceNameArray, 2)));
                        comboMenu.AddSubMenu(eComboMenu);
                    }

                    var rComboMenu = new Menu(":: R", ":: R");
                    {
                        var rComboWhiteMenu = new Menu(":: R - Whitelist", ":: R - Whitelist");
                        {
                            foreach (var enemy in HeroManager.Enemies.Where(enemy => enemy.IsValid))
                            {
                                rComboWhiteMenu.AddItem(new MenuItem("r.combo." + enemy.ChampionName, "(R): " + enemy.ChampionName).SetValue(true));
                            }
                            rComboMenu.AddSubMenu(rComboWhiteMenu);
                        }

                        var rUndyMenu = new Menu(":: R Undy Settings", ":: R Undy Settings");
                        {
                            rUndyMenu.AddItem(new MenuItem("kindred.r", "Kindred's Lamb's Respite(R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("vlad.w", "Vladimir (W)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("try.r", "Tryndamere's Undying Rage (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("kayle.r", "Kayle's Intervention (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("morgana.e", "Morgana's Black Shield (E)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("sivir.e", "Sivir's Spell Shield (E)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("banshee.passive", "Banshee's Veil (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("nocturne.w", "Nocturne's Shroud of Darkness (W)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("aatrox.passive", "Aatrox's (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("zac.passive", "Zac's (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("alistar.r", "Alistar's (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("sion.passive", "Sion's (PASSIVE)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("zilean.r", "Zilean's Chrono's Shift (R)").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("yorick.zombie", "Yorick's (ZOMBIE)").SetValue(true));
                            rComboMenu.AddSubMenu(rUndyMenu);
                        }

                        rComboMenu.AddItem(new MenuItem("r.combo", "Use (R)").SetValue(true));
                        rComboMenu.AddItem(new MenuItem("r.combo.style", "» (R) Style").SetValue(new StringList(new[] { "Only Enemy If Killable" })));
                        comboMenu.AddSubMenu(rComboMenu);
                    }
                    Config.AddSubMenu(comboMenu);
                }
                var harassMenu = new Menu(":: Harass Settings", ":: Harass Settings");
                {
                    var toggleMenu = new Menu(":: Toggle Settings", ":: Toggle Settings").SetFontStyle(FontStyle.Bold, SharpDX.Color.Gold);
                    {
                        toggleMenu.AddItem(new MenuItem("q.toggle", "Use (Q)").SetValue(true));
                        toggleMenu.AddItem(new MenuItem("toggle.hit.chance", "(Toggle) Hit Chance").SetValue(new StringList(HitchanceNameArray, 3)));
                        toggleMenu.AddItem(new MenuItem("toggle.active", "Toggle !").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
                        harassMenu.AddSubMenu(toggleMenu);
                    }

                    var qHarassMenu = new Menu(":: Q", ":: Q");
                    {
                        qHarassMenu.AddItem(new MenuItem("q.harass", "Use (Q)").SetValue(true));
                        harassMenu.AddSubMenu(qHarassMenu);
                    }

                    var qeHarassMenu = new Menu(":: QE", ":: QE");
                    {
                        qeHarassMenu.AddItem(new MenuItem("qe.harass", "Use (QE)").SetValue(true));
                        qeHarassMenu.AddItem(new MenuItem("qe.harass.style", "» (E) Style").SetValue(new StringList(new[] { "If Enemy Stunable" })));
                        harassMenu.AddSubMenu(qeHarassMenu);
                    }

                    var wHarassMenu = new Menu(":: W", ":: W");
                    {
                        wHarassMenu.AddItem(new MenuItem("w.harass", "Use (W)").SetValue(true));
                        harassMenu.AddSubMenu(wHarassMenu);
                    }

                    harassMenu.AddItem(new MenuItem("disable.harass.under.turret", "Disable Harass If Player Under Enemy Turret").SetValue(true));
                    harassMenu.AddItem(new MenuItem("harass.mana", "Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(harassMenu);
                }
                var clearMenu = new Menu(":: Clear Settings", ":: Clear Settings");
                {
                    var laneclearMenu = new Menu(":: Wave Clear", ":: Wave Clear");
                    {
                        laneclearMenu.AddItem(new MenuItem("keysinfo1", "                  (Q) Settings").SetTooltip("Q Settings"));
                        laneclearMenu.AddItem(new MenuItem("q.clear", "Use (Q)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("q.hit.x.minion", "Min. Minion").SetValue(new Slider(3, 1, 5)));
                        laneclearMenu.AddItem(new MenuItem("keysinfo2", "                  (W) Settings").SetTooltip("W Settings"));
                        laneclearMenu.AddItem(new MenuItem("w.clear", "Use (W)").SetValue(true));
                        laneclearMenu.AddItem(new MenuItem("w.hit.x.minion", "Min. Minion").SetValue(new Slider(4, 1, 5)));
                        clearMenu.AddSubMenu(laneclearMenu);
                    }

                    var jungleClear = new Menu(":: Jungle Clear", ":: Jungle Clear");
                    {
                        jungleClear.AddItem(new MenuItem("keysinfo1X", "                  (Q) Settings").SetTooltip("Q Settings"));
                        jungleClear.AddItem(new MenuItem("q.jungle", "Use (Q)").SetValue(true));
                        jungleClear.AddItem(new MenuItem("keysinfo2X", "                  (W) Settings").SetTooltip("W Settings"));
                        jungleClear.AddItem(new MenuItem("w.jungle", "Use (W)").SetValue(true));
                        jungleClear.AddItem(new MenuItem("keysinfo3X", "                  (E) Settings").SetTooltip("E Settings"));
                        jungleClear.AddItem(new MenuItem("e.jungle", "Use (E)").SetValue(true));
                        clearMenu.AddSubMenu(jungleClear);
                    }
                    clearMenu.AddItem(new MenuItem("clear.mana", "Min. Mana Percentage").SetValue(new Slider(50, 1, 99)));
                    Config.AddSubMenu(clearMenu);
                }
                var ksMenu = new Menu(":: Killsteal Settings", ":: Killsteal Settings");
                {
                    ksMenu.AddItem(new MenuItem("keysinfo1q1", "                  (Q) Settings").SetTooltip("Q Settings"));
                    ksMenu.AddItem(new MenuItem("q.ks", "Use (Q)").SetValue(true));
                    ksMenu.AddItem(new MenuItem("keysinfo2q", "                  (W) Settings").SetTooltip("W Settings"));
                    ksMenu.AddItem(new MenuItem("w.ks", "Use (W)").SetValue(true));
                    ksMenu.AddItem(new MenuItem("keysinfo2q2", "                  (E) Settings").SetTooltip("W Settings"));
                    ksMenu.AddItem(new MenuItem("e.ks", "Use (E)").SetValue(true));
                    Config.AddSubMenu(ksMenu);
                }
                var drawMenu = new Menu(":: Drawings", ":: Drawings");
                {
                    drawMenu.AddItem(new MenuItem("q.draw", "(Q) Range").SetValue(new Circle(false, Color.Pink)));
                    drawMenu.AddItem(new MenuItem("qe.draw", "(QE) Range").SetValue(new Circle(false, Color.CornflowerBlue)));
                    drawMenu.AddItem(new MenuItem("e.draw", "(E) Range").SetValue(new Circle(false, Color.Crimson)));
                    drawMenu.AddItem(new MenuItem("w.draw", "(W) Range").SetValue(new Circle(false, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("r.draw", "(R) Range").SetValue(new Circle(false, Color.Lime)));
                    Config.AddSubMenu(drawMenu);
                }
                var drawDamageMenu = new MenuItem("combo.damage.damageindicator", "Combo Damage").SetValue(true);
                var drawFill = new MenuItem("combo.damage.damageindicator.fill", "Combo Damage Fill").SetValue(new Circle(true, Color.Gold));

                drawMenu.SubMenu("Damage Draws").AddItem(drawDamageMenu);
                drawMenu.SubMenu("Damage Draws").AddItem(drawFill);

                DamageIndicator.DamageToUnit = Helper.TotalDamage;
                DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

                drawDamageMenu.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                };

                drawFill.ValueChanged +=
                delegate (object sender, OnValueChangeEventArgs eventArgs)
                {
                    DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                    DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                };
                Config.AddItem(new MenuItem("credits.x1", "                Developed by Hikigaya").SetFontStyle(FontStyle.Bold, SharpDX.Color.DodgerBlue));
                Config.AddToMainMenu();
            }
            Obj_AI_Base.OnSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
        }
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "SyndraQ")
                    Q.LastCastAttemptT = Environment.TickCount;
                if (args.SData.Name == "SyndraW" || args.SData.Name == "syndrawcast")
                    W.LastCastAttemptT = Environment.TickCount;
                if (args.SData.Name == "SyndraE" || args.SData.Name == "syndrae5")
                    E.LastCastAttemptT = Environment.TickCount;
            }

        }
        private static void OnUpdate(EventArgs args)
        {
            R.Range = R.Level == 3 ? 750f : 675f;
            E.Width = E.Level != 5 ? (float)(45 * 0.5) : 45f;
            Qe.Range = Config.Item("q.e.max.range").GetValue<Slider>().Value;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    KillSteal();
                    Toggle();
                    break;

            }
        }
        private static void Combo()
        {
            if (Q.IsReady() && Config.Item("q.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= HikiChance("q.hit.chance")))
                {
                    Q.Cast(enemy);
                }
            }
            if (W.IsReady() && Config.Item("w.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetPrediction(x).Hitchance >= HikiChance("w.hit.chance")))
                {
                    Helper.UseW(enemy, enemy);
                }
            }
            if (Q.IsReady() && E.IsReady() && Config.Item("qe.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Qe.Range) && Qe.GetPrediction(x).Hitchance >= HikiChance("qe.hit.chance")))
                {
                    Helper.UseQe(enemy);
                }
            }

            if (R.IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !Helper.BuffCheck(x) &&
                    Config.Item("r.combo." + x.ChampionName).GetValue<bool>()))
                {
                    if (enemy.Health < R.GetDamage(enemy))
                    {
                        R.CastOnUnit(enemy);
                        R.LastCastAttemptT = Environment.TickCount;
                    }
                }
            }

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready && Config.Item("use.ignite").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Player.Spellbook.GetSpell(IgniteSlot).SData.CastRange)
                    && x.Health < Q.GetDamage(x) + W.GetDamage(x)))
                {
                    Player.Spellbook.CastSpell(IgniteSlot, enemy);
                }
            }
        }
        private static void Harass()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
            {
                return;
            }

            if (Config.Item("disable.harass.under.turret").GetValue<bool>())
            {
                if (ObjectManager.Get<Obj_AI_Turret>().Any(x => ObjectManager.Player.Distance(x) < x.AttackRange && x.Team != ObjectManager.Player.Team))
                {
                    return;
                }
            }

            if (Q.IsReady() && Config.Item("q.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= HikiChance("q.hit.chance")))
                {
                    Q.Cast(enemy);
                }
            }
            if (W.IsReady() && Config.Item("w.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetPrediction(x).Hitchance >= HikiChance("w.hit.chance")))
                {
                    Helper.UseW(enemy, enemy);
                }
            }
            if (Q.IsReady() && E.IsReady() && Config.Item("qe.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Qe.Range) && Qe.GetPrediction(x).Hitchance >= HikiChance("qe.hit.chance")))
                {
                    Helper.UseQe(enemy);
                }
            }

        }
        private static void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("q.clear").GetValue<bool>())
            {
                var min = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range + Q.Width + 30);
                var minion = Q.GetCircularFarmLocation(min, Q.Width);
                if (minion.MinionsHit >= 3)
                {
                    Q.Cast(minion.Position);
                }
            }
            if (W.IsReady() && Config.Item("w.clear").GetValue<bool>())
            {
                var rangedMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30,
            MinionTypes.Ranged);
                var allMinionsW = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range + W.Width + 30);
                if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "SyndraW")
                {
                    //WObject
                    var gObjectPos = Helper.GetGrabableObjectPos(false);
                    if (gObjectPos.To2D().IsValid() && Environment.TickCount - W.LastCastAttemptT > Game.Ping + 150)
                    {
                        W.Cast(gObjectPos);
                    }
                }
                else if (Player.Spellbook.GetSpell(SpellSlot.W).Name == "SyndraWCast")
                {
                    var fl1 = Q.GetCircularFarmLocation(rangedMinionsW, W.Width);
                    var fl2 = Q.GetCircularFarmLocation(allMinionsW, W.Width);
                    if (fl1.MinionsHit >= 3 && W.IsInRange(fl1.Position.To3D()))
                    {
                        W.Cast(fl1.Position);
                    }
                    else if (fl2.MinionsHit >= 1 && W.IsInRange(fl2.Position.To3D()) && fl1.MinionsHit <= 2)
                    {
                        W.Cast(fl2.Position);
                    }
                }
            }
        }
        private static void JungleClear()
        {
            if (ObjectManager.Player.ManaPercent < Config.Item("clear.mana").GetValue<Slider>().Value)
            {
                return;
            }
            if (Q.IsReady() && Config.Item("q.jungle").GetValue<bool>())
            {
                var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                Q.Cast(mobs[0]);
            }
            if (W.IsReady() && Config.Item("w.jungle").GetValue<bool>() && Environment.TickCount - Q.LastCastAttemptT > 800)
            {
                var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                Helper.UseW(mobs[0], mobs[0]);
            }
            if (E.IsReady() && Config.Item("e.jungle").GetValue<bool>())
            {
                var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                E.Cast(mobs[0]);
            }
        }
        private static void KillSteal()
        {
            if (Q.IsReady() && Config.Item("q.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)
                    && Q.GetPrediction(x).Hitchance >= HikiChance("q.hit.chance")))
                {
                    Q.Cast(enemy);
                }
            }
            if (W.IsReady() && Config.Item("w.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetPrediction(x).Hitchance >= HikiChance("q.hit.chance")
                    && x.Health < W.GetDamage(x)))
                {
                    Helper.UseW(enemy, enemy);
                }
            }
            if (E.IsReady() && Config.Item("e.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && E.GetPrediction(x).Hitchance >= HikiChance("e.hit.chance")
                    && x.Health < E.GetDamage(x)))
                {
                    E.Cast(enemy);
                }
            }
        }
        private static void Toggle()
        {
            if (Config.Item("toggle.active").GetValue<KeyBind>().Active)
            {
                if (ObjectManager.Player.ManaPercent < Config.Item("harass.mana").GetValue<Slider>().Value)
                {
                    return;
                }

                if (Q.IsReady() && Config.Item("q.toggle").GetValue<bool>())
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) &&
                        Q.GetPrediction(x).Hitchance >= HikiChance("toggle.hit.chance")))
                    {
                        Q.Cast(enemy);
                    }
                }
            }
        }
        private static void OnDraw(EventArgs args)
        {
            /*
             drawMenu.AddItem(new MenuItem("q.draw", "(Q) Range").SetValue(new Circle(true,Color.Pink)));
                    drawMenu.AddItem(new MenuItem("qe.draw", "(QE) Range").SetValue(new Circle(true, Color.CornflowerBlue)));
                    drawMenu.AddItem(new MenuItem("w.draw", "(W) Range").SetValue(new Circle(true, Color.Gold)));
                    drawMenu.AddItem(new MenuItem("r.draw", "(R) Range").SetValue(new Circle(true, Color.Lime)));
             */

            if (Config.Item("q.draw").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, Color.CornflowerBlue);
            }
            if (Config.Item("qe.draw").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, Qe.Range, Color.Gold);
            }
            if (Config.Item("w.draw").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, Color.Lime);
            }
            if (Config.Item("e.draw").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, Color.Orange);
            }
            if (Config.Item("r.draw").GetValue<Circle>().Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, Color.Purple);
            }
        }
    }
}
