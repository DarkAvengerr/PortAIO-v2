using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Brand
{
    using Common;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Linq;
    using Color = System.Drawing.Color;

    internal class Program
    {
        public static Menu Menu;
        public static int SkinID;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite = SpellSlot.Unknown;
        public static AIHeroClient Me;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        public static void Main()
        {
            Load();
        }

        public static void Load()
        {
            if (ObjectManager.Player.ChampionName != "Brand")
            {
                return;
            }

            Me = ObjectManager.Player;

            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 750);

            Q.SetSkillshot(0.25f, 50f, 1600f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0.25f, 2000f);

            Ignite = Me.GetSpellSlot("SummonerDot");

            SkinID = Me.SkinId;

            Menu = new Menu("Flowers' Brand", "NightMoon", true);

            var OrbMenu = Menu.AddSubMenu(new Menu("Orbwalking", "orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbMenu);
            }

            var ComboMenu = Menu.AddSubMenu(new Menu("Combo", "nightmoon.combo.Menu"));
            {
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.q", "Use Q", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.qonlypas", "Use Q (Only Enemy Have Passive)", true)
                    .SetValue(new KeyBind('T', KeyBindType.Toggle, true))).Permashow();
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.w", "Use W", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.e", "Use E", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.r", "Use R", true).SetValue(true));
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.rcount", "Min Counts In R Ranges", true)
                    .SetValue(new Slider(3, 0, 6)));
                ComboMenu.AddItem(new MenuItem("nightmoon.combo.dot", "Use Ignite", true).SetValue(true));
            }

            var HarassMenu = Menu.AddSubMenu(new Menu("Harass", "nightmoon.harass.Menu"));
            {
                HarassMenu.AddItem(new MenuItem("nightmoon.harass.q", "Use Q", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("nightmoon.harass.w", "Use W", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("nightmoon.harass.e", "Use E", true).SetValue(true));
                HarassMenu.AddItem(new MenuItem("nightmoon.harass.mana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var LaneMenu =  Menu.AddSubMenu(new Menu("LaneClear", "nightmoon.lc.Menu"));
            {
                LaneMenu.AddItem(new MenuItem("nightmoon.lc.w", "Use W", true).SetValue(true));
                LaneMenu.AddItem(new MenuItem("nightmoon.lc.e", "Use E", true).SetValue(true));
                LaneMenu.AddItem(new MenuItem("nightmoon.lc.count", "Min Minions Counts >=", true)
                    .SetValue(new Slider(3, 0, 8)));
                LaneMenu.AddItem(new MenuItem("nightmoon.lc.mana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var JungleMenu = Menu.AddSubMenu(new Menu("JungleClear", "nightmoon.jc.Menu"));
            {
                JungleMenu.AddItem(new MenuItem("nightmoon.jc.q", "Use Q", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("nightmoon.jc.w", "Use W", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("nightmoon.jc.e", "Use E", true).SetValue(true));
                JungleMenu.AddItem(new MenuItem("nightmoon.jc.mana", "When Player ManaPercent >= x%", true)
                    .SetValue(new Slider(50)));
            }

            var KillMenu = Menu.AddSubMenu(new Menu("KillSteal", "nightmoon.ks.Menu"));
            {
                KillMenu.AddItem(new MenuItem("nightmoon.ks.q", "Use Q", true).SetValue(true));
                KillMenu.AddItem(new MenuItem("nightmoon.ks.w", "Use W", true).SetValue(true));
                KillMenu.AddItem(new MenuItem("nightmoon.ks.e", "Use E", true).SetValue(true));
            }

            var MiscMenu = Menu.AddSubMenu(new Menu("Misc", "nightmoon.misc.Menu"));
            {
                MiscMenu.AddItem(new MenuItem("nightmoon.misc.eqgap", "E+Q AntiGapcloser", true).SetValue(true));
                MiscMenu.AddItem(new MenuItem("nightmoon.misc.eqint", "E+Q Interrupter", true).SetValue(true));
            }

            var PredMenu = Menu.AddSubMenu(new Menu("Prediction", "Prediction"));
            {
                PredMenu.AddItem(new MenuItem("SelectPred", "Select Prediction: ", true).SetValue(new StringList(new[]
                {
                    "Common Prediction", "OKTW Prediction", "SDK Prediction", "SPrediction(Need F5 Reload)",
                    "xcsoft AIO Prediction"
                }, 1)));
                PredMenu.AddItem(
                    new MenuItem("SetHitchance", "HitChance: ", true).SetValue(
                        new StringList(new[] { "VeryHigh", "High", "Medium", "Low" })));
                PredMenu.AddItem(new MenuItem("AboutCommonPred", "Common Prediction -> LeagueSharp.Commmon Prediction"));
                PredMenu.AddItem(new MenuItem("AboutOKTWPred", "OKTW Prediction -> Sebby' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSDKPred", "SDK Prediction -> LeagueSharp.SDKEx Prediction"));
                PredMenu.AddItem(new MenuItem("AboutSPred", "SPrediction -> Shine' Prediction"));
                PredMenu.AddItem(new MenuItem("AboutxcsoftAIOPred", "xcsoft AIO Prediction -> xcsoft ALL In One Prediction"));
            }

            var SkinMenu = Menu.AddSubMenu(new Menu("SkinChance", "SkinChance"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(new[]
                        {
                            "Classic", "Apocalyptic Brand", "Vandal Brand", "Cryocore Brand", "Zombie Brand",
                            "Spirit Fire Brand"
                        })));
            }

            var DrawMenu = Menu.AddSubMenu(new Menu("Draw", "nightmoon.draw.Menu"));
            {
                DrawMenu.AddItem(new MenuItem("nightmoon.draw.q", "Q Range", true).SetValue(new Circle(true, Color.PowderBlue)));
                DrawMenu.AddItem(new MenuItem("nightmoon.draw.w", "W Range", true).SetValue(new Circle(true, Color.NavajoWhite)));
                DrawMenu.AddItem(new MenuItem("nightmoon.draw.e", "E Range", true).SetValue(new Circle(true, Color.Violet)));
                DrawMenu.AddItem(new MenuItem("nightmoon.draw.r", "R Range", true).SetValue(new Circle(true, Color.DarkOliveGreen)));
                DrawMenu.AddItem(new MenuItem("nightmoon.draw.damage", "Draw Combo Damage", true).SetValue(true));
            }

            if (Menu.Item("SelectPred", true).GetValue<StringList>().SelectedIndex == 3)
            {
                SPrediction.Prediction.Initialize(PredMenu);
            }

            Menu.AddToMainMenu();

            Game.OnUpdate += OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += OnInterruptableTarget;
            Drawing.OnDraw += OnDraw;
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                //ObjectManager.//Player.SetSkin(ObjectManager.Player.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            KillStealLogic();

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
            }

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
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range - 30, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget() || !CheckTargetSureCanKill(target))
                return;

            if (Menu.Item("nightmoon.combo.dot", true).GetValue<bool>() && Ignite != SpellSlot.Unknown && Ignite.IsReady())
            {
                if (GetComboDamage(target) > target.Health - 150 || target.HealthPercent < 20)
                {
                    Me.Spellbook.CastSpell(Ignite, target);
                }
            }

            if (Menu.Item("nightmoon.combo.w", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.CastTo(target, true);
            }

            if (Menu.Item("nightmoon.combo.e", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.CastOnUnit(target);
            }

            if (Menu.Item("nightmoon.combo.q", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                if (Menu.Item("nightmoon.combo.qonlypas", true).GetValue<KeyBind>().Active)
                {
                    if (HavePassive(target))
                    {
                        Q.CastTo(target);
                    }
                }
                else
                {
                    Q.CastTo(target);
                }
            }

            if (Menu.Item("nightmoon.combo.r", true).GetValue<bool>() && R.IsReady())
            {
                if (target.IsValidTarget(R.Range) && target.HealthPercent < R.GetDamage(target))
                {
                    R.CastOnUnit(target, true);
                }

                foreach (
                    var rtarget in
                    HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range))
                        .OrderByDescending(x => x.CountEnemiesInRange(R.Range)))
                {
                    if (rtarget.CountEnemiesInRange(R.Range) >=
                        Menu.Item("nightmoon.combo.rcount", true).GetValue<Slider>().Value)
                    {
                        R.CastOnUnit(rtarget, true);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Me.ManaPercent >= Menu.Item("nightmoon.harass.mana", true).GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(1000, TargetSelector.DamageType.Magical);

                if (target.IsValidTarget())
                {
                    if (Menu.Item("nightmoon.harass.q", true).GetValue<bool>() && Q.IsReady() && 
                        target.IsValidTarget(Q.Range) && HavePassive(target))
                    {
                        Q.CastTo(target);
                    }

                    if (Menu.Item("nightmoon.harass.w", true).GetValue<bool>() && W.IsReady() &&
                        target.IsValidTarget(W.Range))
                    {
                        W.CastTo(target);
                    }

                    if (Menu.Item("nightmoon.harass.e", true).GetValue<bool>() && E.IsReady() &&
                        target.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(target, true);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (Me.ManaPercent >= Menu.Item("nightmoon.lc.mana", true).GetValue<Slider>().Value)
            {
                var minions = MinionManager.GetMinions(Me.ServerPosition, W.Range);
                var FarmWLoaction = W.GetCircularFarmLocation(minions, W.Width);

                if (Menu.Item("nightmoon.lc.w", true).GetValue<bool>() && W.IsReady())
                {
                    if (FarmWLoaction.MinionsHit >= Menu.Item("nightmoon.lc.count", true).GetValue<Slider>().Value)
                    {
                        W.Cast(FarmWLoaction.Position, true);
                    }
                }

                if (Menu.Item("nightmoon.lc.e", true).GetValue<bool>() && E.IsReady())
                {
                    var eminions = MinionManager.GetMinions(Me.ServerPosition, E.Range);

                    if (eminions.Count >= 2)
                    {
                        foreach (
                            var m in eminions
                                  .Where(e => e.HasBuff("Brandablaze")))
                        {
                            E.CastOnUnit(m, true);
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (Me.ManaPercent >= Menu.Item("nightmoon.jc.mana", true).GetValue<Slider>().Value)
            {
                var mobs = MinionManager.GetMinions(Me.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (mobs.Count > 0)
                {
                    var mob = mobs.FirstOrDefault();

                    if (Menu.Item("nightmoon.jc.w", true).GetValue<bool>() && W.IsReady())
                    {
                        W.Cast(mob.Position, true);
                    }

                    if (Menu.Item("nightmoon.jc.q", true).GetValue<bool>() && Q.IsReady())
                    {
                        Q.Cast(mob, true);
                    }

                    if (Menu.Item("nightmoon.jc.e", true).GetValue<bool>() && E.IsReady())
                    {
                        E.CastOnUnit(mob, true);
                    }
                }
            }
        }

        private static void KillStealLogic()
        {
            foreach (
                var target in
                HeroManager.Enemies.Where(
                    e => e.IsValidTarget() && !e.IsDead && !e.IsZombie && CheckTargetSureCanKill(e)))
            {
                if (Menu.Item("nightmoon.ks.q", true).GetValue<bool>() && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.Q) > target.Health + target.MagicShield)
                {
                    Q.CastTo(target);
                }

                if (Menu.Item("nightmoon.ks.w", true).GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.W) > target.Health + target.MagicShield)
                {
                    W.CastTo(target);
                }

                if (Menu.Item("nightmoon.ks.e", true).GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range) &&
                    Me.GetSpellDamage(target, SpellSlot.E) > target.Health + target.MagicShield)
                {
                    E.CastOnUnit(target, true);
                }
            }
        }

        private static bool CheckTargetSureCanKill(Obj_AI_Base target)
        {
            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return false;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return false;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return false;
            }

            return !target.HasBuff("FioraW");
        }

        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var e = gapcloser.Sender;

            if (Menu.Item("nightmoon.misc.eqgap", true).GetValue<bool>() && e.IsValidTarget(E.Range))
            {
                if (HavePassive(e))
                {
                    if (Q.IsReady())
                    {
                        Q.CastTo(e);
                    }
                }
                else if (!HavePassive(e))
                {
                    if (E.IsReady())
                    {
                        E.CastOnUnit(e, true);
                    }
                }
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Menu.Item("nightmoon.misc.eqint", true).GetValue<bool>() && sender.IsValidTarget(E.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                if (HavePassive(sender))
                {
                    if (Q.IsReady())
                    {
                        Q.CastTo(sender);
                    }
                }
                else if (!HavePassive(sender))
                {
                    if (E.IsReady())
                    {
                        E.CastOnUnit(sender, true);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("nightmoon.draw.q", true).GetValue<Circle>().Active && Q.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, Q.Range, Menu.Item("nightmoon.draw.q", true).GetValue<Circle>().Color);
            }

            if (Menu.Item("nightmoon.draw.w", true).GetValue<Circle>().Active && W.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, W.Range, Menu.Item("nightmoon.draw.w", true).GetValue<Circle>().Color);
            }

            if (Menu.Item("nightmoon.draw.e", true).GetValue<Circle>().Active && E.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, E.Range, Menu.Item("nightmoon.draw.e", true).GetValue<Circle>().Color);
            }

            if (Menu.Item("nightmoon.draw.r", true).GetValue<Circle>().Active && R.IsReady())
            {
                Render.Circle.DrawCircle(Me.Position, R.Range, Menu.Item("nightmoon.draw.r", true).GetValue<Circle>().Color);
            }

            if (Menu.Item("nightmoon.draw.damage", true).GetValue<bool>())
            {
                foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                {
                    HpBarDraw.Unit = e;
                    HpBarDraw.DrawDmg(GetComboDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }
            }
        }

        private static float GetComboDamage(AIHeroClient target)
        {
            double Damage = 0;

            if (Q.IsReady())
                Damage += Me.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                Damage += Me.GetSpellDamage(target, SpellSlot.W);

            if (E.IsReady())
                Damage += Me.GetSpellDamage(target, SpellSlot.E);

            if (R.IsReady())
                Damage += Me.GetSpellDamage(target, SpellSlot.R);

            if (Ignite.IsReady())
                Damage += Me.GetSummonerSpellDamage(target, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return 0;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3)
            {
                return 0;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return 0;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3)
            {
                return 0;
            }

            if (target.HasBuff("FioraW"))
            {
                return 0;
            }

            return (float)Damage;
        }

        private static bool HavePassive(AIHeroClient e)
        {
            return e.HasBuff("Brandablaze");
        } 
    }
}
