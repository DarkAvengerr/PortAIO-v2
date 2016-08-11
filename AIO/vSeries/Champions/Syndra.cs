using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;
using vSupport_Series.Core.Plugins;
using Color = System.Drawing.Color;
using Prediction = LeagueSharp.Common.Prediction;
using Orbwalking = vSupport_Series.Core.Plugins.Orbwalking;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace vSupport_Series.Champions
{

    public class Syndra : Helper
    {
        public static readonly AIHeroClient Player = ObjectManager.Player;
        public static Spell Q, W, E, R, Qe;
        public static int QwLastcast = 0;
        public static SpellSlot IgniteSlot;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Menu Config;

        public Syndra()
        {
            SyndraOnLoad();
        }

        private static void SyndraOnLoad()
        {
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

            Config = new Menu("vSupport Series: " + ObjectManager.Player.ChampionName, "vSupport Series", true);
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
                        qeComboMenu.AddItem(new MenuItem("q.e.max.range", "QE Max Range %").SetValue(new Slider(100)));
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
                            rUndyMenu.AddItem(new MenuItem("undy.tryn", "Trynda's Ult").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.kayle", "Kayle's Ult").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.zilean", "Zilean's Ult").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.alistar", "Alistar's Ult").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.zac", "Zac's Passive").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.aatrox", "Aatrox's Passive").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.sivir", "Sivir's Spell Shield").SetValue(true));
                            rUndyMenu.AddItem(new MenuItem("undy.morgana", "Morgana's Black Shield").SetValue(true));
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
                Config.AddToMainMenu();
            }
            SPrediction.Prediction.Initialize(Config);
            Obj_AI_Base.OnSpellCast += SyndraOnProcessSpellCast;
            Game.OnUpdate += SyndraOnUpdate;
            Drawing.OnDraw += SyndraOnDraw;
        }

        private static void SyndraOnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
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

        private static void SyndraOnUpdate(EventArgs args)
        {
            R.Range = R.Level == 3 ? 750f : 675f;
            E.Width = E.Level == 5 ? 45f : (float)(45 * 0.5);
            var qeRnew = Config.Item("q.e.max.range").GetValue<Slider>().Value * .01 * 1292;
            Qe.Range = (float)qeRnew;

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
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && Q.GetPrediction(x).Hitchance >= SpellHitChance(Config,"q.hit.chance")))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "q.hit.chance"));
                }
            }
            if (W.IsReady() && Config.Item("w.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetSPrediction(x).HitChance >= SpellHitChance(Config, "w.hit.chance")))
                {
                    UseW(enemy, enemy);
                }
            }
            if (Q.IsReady() && E.IsReady() && Config.Item("qe.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Qe.Range) && Qe.GetSPrediction(x).HitChance >= SpellHitChance(Config, "qe.hit.chance")))
                {
                    UseQe(enemy);
                }
            }

            if (R.IsReady() && Config.Item("r.combo").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && !BuffCheck(x) &&
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
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                {
                    Q.SPredictionCast(enemy, SpellHitChance(Config, "q.hit.chance"));
                }
            }
            if (W.IsReady() && Config.Item("w.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetSPrediction(x).HitChance >= SpellHitChance(Config, "w.hit.chance")))
                {
                    UseW(enemy, enemy);
                }
            }
            if (Q.IsReady() && E.IsReady() && Config.Item("qe.harass").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Qe.Range) && Qe.GetSPrediction(x).HitChance >= SpellHitChance(Config, "qe.hit.chance")))
                {
                    UseQe(enemy);
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
                if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
                {
                    //WObject
                    var gObjectPos = GetGrabableObjectPos(false);
                    if (gObjectPos.To2D().IsValid() && Environment.TickCount - W.LastCastAttemptT > Game.Ping + 150)
                    {
                        W.Cast(gObjectPos);
                    }
                }
                else if (Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1)
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
                W.Cast(mobs[0]);
            }
            if (W.IsReady() && Config.Item("w.jungle").GetValue<bool>() && Environment.TickCount - Q.LastCastAttemptT > 800)
            {
                var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                W.Cast(mobs[0]);
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
                    && Q.GetPrediction(x).Hitchance >= SpellHitChance(Config, "q.hit.chance")))
                {
                    Q.Cast(enemy);
                }
            }
            if (W.IsReady() && Config.Item("w.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range + W.Width) && W.GetPrediction(x).Hitchance >= SpellHitChance(Config,"q.hit.chance")
                    && x.Health < W.GetDamage(x)))
                {
                    UseW(enemy, enemy);
                }
            }
            if (E.IsReady() && Config.Item("e.ks").GetValue<bool>())
            {
                foreach (var enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && E.GetPrediction(x).Hitchance >= SpellHitChance(Config,"e.hit.chance")
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
                        Q.GetSPrediction(x).HitChance >= SpellHitChance(Config, "toggle.hit.chance")))
                    {
                        Q.SPredictionCast(enemy, SpellHitChance(Config, "toggle.hit.chance"));
                    }
                }
            }
        }
        public static void UseW(Obj_AI_Base grabObject, Obj_AI_Base enemy)
        {
            if (grabObject != null && W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var gObjectPos = GetGrabableObjectPos(false);

                if (gObjectPos.To2D().IsValid() && Environment.TickCount - Q.LastCastAttemptT > Game.Ping + 150
                    && Environment.TickCount - E.LastCastAttemptT > 750 + Game.Ping && Environment.TickCount - W.LastCastAttemptT > 750 + Game.Ping)
                {
                    var grabsomething = false;
                    if (enemy != null)
                    {
                        var pos2 = W.GetPrediction(enemy,true);
                        if (pos2.Hitchance >= HitChance.High) grabsomething = true;
                    }
                    if (grabsomething || grabObject.IsStunned)
                    {
                        W.Cast(gObjectPos);
                    }

                }
            }
            if (enemy != null && W.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 2)
            {
                var pos = W.GetPrediction(enemy, true);
                if (pos.Hitchance >= HitChance.High)
                {
                    W.Cast(pos.CastPosition);
                }
            }
        }
        public static Vector3 GetGrabableObjectPos(bool onlyOrbs)
        {
            if (onlyOrbs)
                return OrbManager.GetOrbToGrab((int)W.Range);
            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsValidTarget(W.Range)))
                return minion.ServerPosition;
            return OrbManager.GetOrbToGrab((int)W.Range);
        }
        public static void UseQe(Obj_AI_Base target)
        {
            if (!Q.IsReady() || !E.IsReady() || target == null) return;
            var sPos = Prediction.GetPrediction(target, Q.Delay + E.Delay).UnitPosition;
            if (ObjectManager.Player.Distance(sPos, true) > Math.Pow(E.Range, 2))
            {
                var orb = ObjectManager.Player.ServerPosition + Vector3.Normalize(sPos - ObjectManager.Player.ServerPosition) * E.Range;
                Qe.Delay = Q.Delay + E.Delay + ObjectManager.Player.Distance(orb) / E.Speed;
                var pos = Qe.GetPrediction(target);
                if (pos.Hitchance >= HitChance.High)
                {
                    UseQe2(target, orb);
                }
            }
            else
            {
                Q.Width = 40f;
                var pos = Q.GetPrediction(target, true);
                Q.Width = 125f;
                if (pos.Hitchance >= HitChance.VeryHigh)
                    UseQe2(target, pos.UnitPosition);
            }
        }

        public static void UseE(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }
            foreach (var orb in OrbManager.GetOrbs(true).Where(orb => orb.To2D().IsValid() && ObjectManager.Player.Distance(orb, true) < Math.Pow(E.Range, 2)))
            {
                var sp = orb.To2D() + Vector2.Normalize(ObjectManager.Player.ServerPosition.To2D() - orb.To2D()) * 100f;
                var ep = orb.To2D() + Vector2.Normalize(orb.To2D() - ObjectManager.Player.ServerPosition.To2D()) * 592;
                Qe.Delay = E.Delay + ObjectManager.Player.Distance(orb) / E.Speed;
                Qe.UpdateSourcePosition(orb);
                var pPo = Qe.GetPrediction(target).UnitPosition.To2D();
                if (pPo.Distance(sp, ep, true, true) <= Math.Pow(Qe.Width + target.BoundingRadius, 2))
                {
                    E.Cast(orb);
                }
            }
        }

        public static void UseQe2(Obj_AI_Base target, Vector3 pos)
        {
            if (target == null || !(ObjectManager.Player.Distance(pos, true) <= Math.Pow(E.Range, 2)))
            {
                return;
            }

            var sp = pos + Vector3.Normalize(ObjectManager.Player.ServerPosition - pos) * 100f;
            var ep = pos + Vector3.Normalize(pos - ObjectManager.Player.ServerPosition) * 592;
            Qe.Delay = Q.Delay + E.Delay + ObjectManager.Player.ServerPosition.Distance(pos) / E.Speed;
            Qe.UpdateSourcePosition(pos);
            var pPo = Qe.GetPrediction(target).UnitPosition.To2D().ProjectOn(sp.To2D(), ep.To2D());

            if (!pPo.IsOnSegment ||
                !(pPo.SegmentPoint.Distance(target, true) <= Math.Pow(Qe.Width + target.BoundingRadius, 2)))
            {
                return;
            }

            var delay = 280 - (int)(ObjectManager.Player.Distance(pos) / 2.5) + Config.Item("q.e.delay").GetValue<Slider>().Value;
            LeagueSharp.Common.Utility.DelayAction.Add(Math.Max(0, delay), () => E.Cast(pos));
            Qe.LastCastAttemptT = Environment.TickCount;
            Q.Cast(pos);
            UseE(target);
        }
        public static bool BuffCheck(Obj_AI_Base enemy)
        {
            if (enemy.HasBuff("UndyingRage") && Config.Item("undy.tryn").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("JudicatorIntervention") && Config.Item("undy.kayle").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("ZacRebirthReady") && Config.Item("undy.zac").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("AttroxPassiveReady") && Config.Item("undy.aatrox").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("Chrono Shift") && Config.Item("undy.aatrox").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("Ferocious Howl") && Config.Item("undy.alistar").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("Black Shield") && Config.Item("undy.morgana").GetValue<bool>())
            {
                return true;
            }
            else if (enemy.HasBuff("Spell Shield") && Config.Item("undy.sivir").GetValue<bool>())
            {
                return true;
            }
            return false;
        }
        private static void SyndraOnDraw(EventArgs args)
        {
            throw new NotImplementedException();
        }

    }
}
