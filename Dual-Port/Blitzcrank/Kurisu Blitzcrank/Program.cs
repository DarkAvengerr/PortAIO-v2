using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Blitzcrank
{
    class Program
    {
        // Keepo
        internal static Menu Root;
        internal static Random Rand;
        internal static Spell Q, W, E, R;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient Player => ObjectManager.Player;

        internal static int Limiter;
        internal static int LastFlash;
        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Blitzcrank")
            {
                return;
            }

            Rand = new Random();
            Q = new Spell(SpellSlot.Q, 950f);
            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 155f);
            R = new Spell(SpellSlot.R, 545f);

            Root = new Menu("Blitzcrank", "blitzcrank", true);


            var ormenu =  new Menu("Orbwalk", "ormenu");
            Orbwalker = new Orbwalking.Orbwalker(ormenu);
            Root.AddSubMenu(ormenu);

            var kemenu = new Menu("Keys", "kemenu");
            kemenu.AddItem(new MenuItem("usecombo", "Combo [active]")).SetValue(new KeyBind(32, KeyBindType.Press));
            kemenu.AddItem(new MenuItem("useharass", "Harass [active]")).SetValue(new KeyBind('C', KeyBindType.Press));
            kemenu.AddItem(new MenuItem("grabkey", "Grab [active]")).SetValue(new KeyBind('G', KeyBindType.Press));
            kemenu.AddItem(new MenuItem("useflee", "Flee [active]")).SetValue(new KeyBind('A', KeyBindType.Press));
            Root.AddSubMenu(kemenu);

            var comenu = new Menu("Combo", "cmenu");

            var qsmenu = new Menu("Config", "qsmenu");

            var auqmenu = new Menu("Auto Grab", "auqmenu");
            foreach (var hero in HeroManager.Enemies)
                auqmenu.AddItem(new MenuItem("auq" + hero.ChampionName, hero.ChampionName))
                    .SetValue(false).SetTooltip("Auto grab " + hero.ChampionName + " (Dashing/Immobile/Casting)");

            qsmenu.AddSubMenu(auqmenu);

            var blqmenu = new Menu("Blacklist", "blqmenu");
            foreach (var hero in HeroManager.Enemies)
                blqmenu.AddItem(new MenuItem("blq" + hero.ChampionName, hero.ChampionName))
                    .SetValue(false);

            qsmenu.AddSubMenu(blqmenu);
            qsmenu.AddItem(new MenuItem("pred", "Hitchance")).SetValue(new Slider(4, 1, 4));
            qsmenu.AddItem(new MenuItem("fpred", "Flash Hitchance")).SetValue(new Slider(4, 1, 4));
            qsmenu.AddItem(new MenuItem("maxq", "Maximum Q Range")).SetValue(new Slider((int) Q.Range, 100, (int) Q.Range));
            qsmenu.AddItem(new MenuItem("minq", "Minimum Q Range")).SetValue(new Slider(420, 100, (int) Q.Range));
            qsmenu.AddItem(new MenuItem("grabhp", "Dont grab if below HP%")).SetValue(new Slider(0,0,100));
            comenu.AddSubMenu(qsmenu);

            comenu.AddItem(new MenuItem("useqcombo", "Use Q")).SetValue(true);
            comenu.AddItem(new MenuItem("useecombo", "Use E")).SetValue(true);
            comenu.AddItem(new MenuItem("usercombo", "Use R")).SetValue(true);
            Root.AddSubMenu(comenu);

            var fmenu = new Menu("Flee", "fmenu");
            fmenu.AddItem(new MenuItem("usewflee", "Use W")).SetValue(true);
            fmenu.AddItem(new MenuItem("useeflee", "Use E")).SetValue(true);
            Root.AddSubMenu(fmenu);

            var exmenu = new Menu("Extra", "exmenu");
            exmenu.AddItem(new MenuItem("int", "Interrupt")).SetValue(false);
            exmenu.AddItem(new MenuItem("supp", "Support")).SetValue(true);
            exmenu.AddItem(new MenuItem("swag", "Use Swag")).SetValue(false).SetTooltip("No Swag Yet :(");

            Root.AddSubMenu(exmenu);

            var skmenu = new Menu("Skins", "skmenu");
            var skinitem = new MenuItem("useskin", "Enabled");
            skmenu.AddItem(skinitem).SetValue(false);

            skinitem.ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                    ObjectManager.Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
                }
            };

            skmenu.AddItem(new MenuItem("skinid", "Skin Id")).SetValue(new Slider(2, 0, 12));
            Root.AddSubMenu(skmenu);

            var drmenu = new Menu("Drawings", "drmenu");
            drmenu.AddItem(new MenuItem("drawq", "Draw Q")).SetValue(false);
            drmenu.AddItem(new MenuItem("drawr", "Draw R")).SetValue(false);
            Root.AddSubMenu(drmenu);

            Root.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            Chat.Print("<b>Blitzcrank#</b> - Loaded!");

            if (Menu.GetMenu("Activator", "activator") == null &&
                Menu.GetMenu("ElUtilitySuite", "ElUtilitySuite") == null &&
                Menu.GetMenu("adcUtility", "adcUtility") == null &&
                Menu.GetMenu("NabbActivator", "nabbactivator.menu") == null &&
                Menu.GetMenu("Slutty Utility", "Slutty Utility") == null &&
                Menu.GetMenu("MActivator", "masterActivator") == null)
            {
                Chat.Print("<font color=\"#FFF280\">Wooa</font>! you aren't using any activator. " +
                               "How about trying <b>Activator#</b> :^)");
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Root.Item("drawq").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.LawnGreen : Color.Red, 2);
            }

            if (Root.Item("drawr").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.LawnGreen : Color.Red, 2);
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Root.Item("supp").GetValue<bool>())
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed ||
                    Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                {
                    var minion = args.Target as Obj_AI_Base;
                    if (minion != null && minion.IsMinion && minion.IsValidTarget())
                    {
                        if (HeroManager.Allies.Any(x => x.IsValidTarget(1000, false) && !x.IsMe))
                        {
                            if (Player.HasBuff("talentreaperdisplay"))
                            {
                                var b = Player.GetBuff("talentreaperdisplay");
                                if (b.Count > 0)
                                {
                                    args.Process = true;
                                    return;
                                }
                            }

                            args.Process = false;
                        }
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && sender.IsValidTarget() && Root.Item("int").GetValue<bool>())
            {
                if (R.IsReady() && Player.Distance(sender.ServerPosition) <= R.Range)
                {
                    if (args.DangerLevel >= Interrupter2.DangerLevel.High)
                    {
                        R.Cast();
                    }
                }

                if (Q.IsReady() && Player.Distance(sender.ServerPosition) <= Root.Item("maxq").GetValue<Slider>().Value)
                {
                    if (Player.HealthPercent < Root.Item("grabhp").GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (!Root.Item("blq" + sender.ChampionName).GetValue<bool>() &&
                        Player.Distance(sender.ServerPosition) > Root.Item("minq").GetValue<Slider>().Value)
                    {
                        Q.Cast(sender);
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "summonerflash")
            {
                LastFlash = Utils.GameTimeTickCount;
            }

            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy && Q.IsReady() && Root.Item("useqcombo").GetValue<bool>())
            {
                if (Player.HealthPercent < Root.Item("grabhp").GetValue<Slider>().Value)
                {
                    return;
                }

                if (hero.IsValidTarget(Root.Item("maxq").GetValue<Slider>().Value) && hero.Health > Q.GetDamage(hero))
                {
                    if (!Root.Item("blq" + hero.ChampionName).GetValue<bool>() &&
                         Root.Item("auq" + hero.ChampionName).GetValue<bool>())
                    {
                        if (hero.Distance(Player.ServerPosition) > Root.Item("minq").GetValue<Slider>().Value)
                        {
                            Q.CastIfHitchanceEquals(hero, HitChance.VeryHigh);
                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || !Orbwalking.CanMove(100))
            {
                return;
            }

            Grab(Root.Item("grabkey").GetValue<KeyBind>().Active);
            Flee(Root.Item("useflee").GetValue<KeyBind>().Active);

            Combo(Root.Item("useqcombo").GetValue<bool>(), Root.Item("useecombo").GetValue<bool>(), 
                  Root.Item("usercombo").GetValue<bool>());

            Secure(!Root.Item("supp").GetValue<bool>(), !Root.Item("supp").GetValue<bool>());

            if (Root.Item("useskin").GetValue<bool>())
            {
                Player.SetSkin(Player.CharData.BaseSkinName, Root.Item("skinid").GetValue<Slider>().Value);
            }

            foreach (var ene in HeroManager.Enemies.Where(x => x.IsValidTarget(Root.Item("maxq").GetValue<Slider>().Value)))
            {
                if (Player.HealthPercent < Root.Item("grabhp").GetValue<Slider>().Value)
                {
                    return;
                }

                if (!Root.Item("blq" + ene.ChampionName).GetValue<bool>() && 
                     Root.Item("auq" + ene.ChampionName).GetValue<bool>())
                {
                    if (ene.Distance(Player.ServerPosition) > Root.Item("minq").GetValue<Slider>().Value && Q.IsReady())
                    {
                        Q.CastIfHitchanceEquals(ene, HitChance.Dashing, true);
                        Q.CastIfHitchanceEquals(ene, HitChance.Immobile, true);
                    }
                }
            }
        }

        private static void Flee(bool enable)
        {
            if (!enable)
            {
                return;
            }

            if (W.IsReady())
            {
                W.Cast();
            }

            var ene = HeroManager.Enemies.FirstOrDefault(x => x.Distance(Player.ServerPosition) <= E.Range + 200);
            if (E.IsReady() && ene.IsValidTarget())
            {
                E.Cast();
            }

            if (Player.HasBuff("powerfist") && Orbwalking.InAutoAttackRange(ene))
            {
                if (Utils.GameTimeTickCount - Limiter >= 150 + Game.Ping)
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, ene);
                    Limiter = Utils.GameTimeTickCount;
                }

                return;
            }

            Orbwalking.Orbwalk(null, Game.CursorPos);
        }

        private static void Combo(bool useq, bool usee, bool user)
        {
            if (!Root.Item("usecombo").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (useq && Q.IsReady())
            {
                var QT = TargetSelector.GetTarget(Root.Item("maxq").GetValue<Slider>().Value, TargetSelector.DamageType.Magical);
                if (QT != null && Root.Item("blq" + QT.ChampionName).GetValue<bool>())
                {
                    return;
                }

                if (!(Player.HealthPercent < Root.Item("grabhp").GetValue<Slider>().Value))
                {
                    if (QT.IsValidTarget() && QT.Distance(Player.ServerPosition) > Root.Item("minq").GetValue<Slider>().Value)
                    {
                        if (!QT.IsZombie && !TargetSelector.IsInvulnerable(QT, TargetSelector.DamageType.Magical))
                        {
                            var poutput = Q.GetPrediction(QT); // prediction output
                            if (Utils.GameTimeTickCount - LastFlash < 1500)
                            {
                                if (poutput.Hitchance == (HitChance) Root.Item("fpred").GetValue<Slider>().Value + 2)
                                {
                                    Q.Cast(poutput.CastPosition);
                                }
                            }

                            if (poutput.Hitchance == (HitChance) Root.Item("pred").GetValue<Slider>().Value + 2)
                            {
                                Q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }

            if (usee && E.IsReady())
            {
                var ET =
                    HeroManager.Enemies.FirstOrDefault(
                        x => x.HasBuff("rocketgrab2") || x.Distance(Player.ServerPosition) <= E.Range + 200);

                if (ET != null)
                {
                    if (!ET.IsZombie && !TargetSelector.IsInvulnerable(ET, TargetSelector.DamageType.Magical))
                    {
                        E.Cast();
                    }
                }
            }

            if (user && R.IsReady())
            {
                var RT = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                if (RT.IsValidTarget() && !RT.IsZombie)
                {
                    if (!TargetSelector.IsInvulnerable(RT, TargetSelector.DamageType.Magical))
                    {
                        if (RT.Health > R.GetDamage(RT) && !E.IsReady() && RT.HasBuffOfType(BuffType.Knockup))
                        {
                            R.Cast();
                        }
                    }
                }
            }
        }

        private static void Grab(bool enable)
        {
            if (Q.IsReady() && enable)
            {
                var QT = TargetSelector.GetTarget(Root.Item("maxq").GetValue<Slider>().Value, TargetSelector.DamageType.Magical);
                if (QT != null && Root.Item("blq" + QT.ChampionName).GetValue<bool>())
                {
                    return;
                }

                if (!(Player.HealthPercent < Root.Item("grabhp").GetValue<Slider>().Value))
                {
                    if (QT.IsValidTarget() && QT.Distance(Player.ServerPosition) > Root.Item("minq").GetValue<Slider>().Value)
                    {
                        if (!QT.IsZombie && !TargetSelector.IsInvulnerable(QT, TargetSelector.DamageType.Magical))
                        {
                            var poutput = Q.GetPrediction(QT); // prediction output
                            if (Utils.GameTimeTickCount - LastFlash < 1500)
                            {
                                if (poutput.Hitchance == (HitChance) Root.Item("fpred").GetValue<Slider>().Value + 2)
                                {
                                    Q.Cast(poutput.CastPosition);
                                }
                            }

                            if (poutput.Hitchance == (HitChance) Root.Item("pred").GetValue<Slider>().Value + 2)
                            {
                                Q.Cast(poutput.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        private static void Secure(bool useq, bool user)
        {
            if (useq && Q.IsReady())
            {
                var QT = HeroManager.Enemies.FirstOrDefault(x => Q.GetDamage(x) > x.Health);
                if (QT.IsValidTarget(Root.Item("maxq").GetValue<Slider>().Value))
                {
                    var poutput = Q.GetPrediction(QT); // prediction output
                    if (poutput.Hitchance >= (HitChance) Root.Item("pred").GetValue<Slider>().Value + 2)
                    {
                        if (!QT.IsZombie && !TargetSelector.IsInvulnerable(QT, TargetSelector.DamageType.Magical))
                        {
                            Q.Cast(poutput.CastPosition);
                        }
                    }
                }
            }

            if (user && R.IsReady())
            {
                var RT = HeroManager.Enemies.FirstOrDefault(x => R.GetDamage(x) > x.Health);
                if (RT.IsValidTarget(R.Range) && !RT.IsZombie)
                {
                    if (!TargetSelector.IsInvulnerable(RT, TargetSelector.DamageType.Magical))
                    {
                        R.Cast();
                    }
                }
            }
        }
    }
}
