using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;


using EloBuddy; 
using LeagueSharp.Common; 
namespace Camille
{
    class Program
    {
        internal static Menu RootMenu;
        internal static Spell Q, W, E, R;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static AIHeroClient Player => ObjectManager.Player;
        internal static HpBarIndicator BarIndicator = new HpBarIndicator();

        internal static bool HasQ2 => Player.HasBuff(Q2BuffName);
        internal static bool HasQ => Player.HasBuff(QBuffName);
        internal static bool OnWall => Player.HasBuff(WallBuffName) || E.Instance.Name != "CamilleE";
        internal static bool IsDashing => Player.HasBuff(EDashBuffName + "1") || Player.HasBuff(EDashBuffName + "2");
        internal static bool ChargingW => Player.HasBuff(WBuffName);

        internal static string WBuffName => "camillewconeslashcharge";
        internal static string EDashBuffName => "camilleedash";
        internal static string WallBuffName => "camilleedashtoggle"; 
        internal static string QBuffName => "camilleqprimingstart";
        internal static string Q2BuffName => "camilleqprimingcomplete";
        internal static string RBuffName => "camillertether";
        internal static string REmitterName => "Camille_Base_R_Indicator_Edge.troy";

        internal static int TickLimiter;
        internal static Dictionary<float, Obj_GeneralParticleEmitter> rPoint =
            new Dictionary<float, Obj_GeneralParticleEmitter>();

        // Camille_Base_R_tar_buf.troy, Camille_Base_R_cas_sound.troy
        // Camille_Base_R_BA_tar.troy 
        // camillewconeslashcharge
        // camilleeonwall

        public static void Main()
        {
            // Camille by Kurisu :^)
            OnGameLoad(new EventArgs());
        }

        internal static void OnGameLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "Camille")
                {
                    return;
                }

                Q = new Spell(SpellSlot.Q, 135f);
                W = new Spell(SpellSlot.W, 625f);
                E = new Spell(SpellSlot.E, 975f);
                R = new Spell(SpellSlot.R, 375f);

                RootMenu = new Menu("Camille#", "camille", true);

                var omenu = new Menu("-] Orbwalk", "orbwalk");
                Orbwalker = new Orbwalking.Orbwalker(omenu);
                RootMenu.AddSubMenu(omenu);

                var kemenu = new Menu("-] Keys", "kemenu");
                kemenu.AddItem(new MenuItem("usecombo", "Combo [active]")).SetValue(new KeyBind(32, KeyBindType.Press));
                kemenu.AddItem(new MenuItem("useharass", "Harass [active]")).SetValue(new KeyBind('G', KeyBindType.Press));
                kemenu.AddItem(new MenuItem("useclear", "Clear [active]")).SetValue(new KeyBind('V', KeyBindType.Press));
                kemenu.AddItem(new MenuItem("useflee", "Flee [active]")).SetValue(new KeyBind('A', KeyBindType.Press));
                RootMenu.AddSubMenu(kemenu);

                var comenu = new Menu("-] Combo", "cmenu");

                var tcmenu = new Menu("-] Extra", "tcmenu");

                //tcmenu.AddItem(new MenuItem("autoq", "Q Killsteal")).SetValue(true);
                //tcmenu.AddItem(new MenuItem("autoe1", "E Killsteal")).SetValue(true);
                //tcmenu.AddItem(new MenuItem("autoeq", "E -> Q Killsteal")).SetValue(true);

                var abmenu = new Menu("-] Skills", "abmenu");

                var whemenu = new Menu("R Focus Targets", "whemenu").SetFontStyle(FontStyle.Regular, SharpDX.Color.Cyan);
                foreach (var hero in HeroManager.Enemies)
                    whemenu.AddItem(new MenuItem("whR" + hero.ChampionName, hero.ChampionName))
                        .SetValue(false).SetTooltip("R Only on " + hero.ChampionName).DontSave();
                abmenu.AddSubMenu(whemenu);

                abmenu.AddItem(new MenuItem("useqcombo", "Use Q")).SetValue(true);
                abmenu.AddItem(new MenuItem("usewcombo", "Use W")).SetValue(true);
                abmenu.AddItem(new MenuItem("useecombo", "Use E")).SetValue(true);
                abmenu.AddItem(new MenuItem("usercombo", "Use R")).SetValue(true);
                comenu.AddSubMenu(abmenu);

                tcmenu.AddItem(new MenuItem("useehelper", "Use E Assists")).SetValue(true);
                tcmenu.AddItem(new MenuItem("wdash", "Always W While Dashing")).SetValue(false);
                tcmenu.AddItem(new MenuItem("r33", "Focus R Target")).SetValue(false);
                tcmenu.AddItem(new MenuItem("eturret", "Dont E Under Turret")).SetValue(new KeyBind('L', KeyBindType.Toggle, true)).Permashow();
                tcmenu.AddItem(new MenuItem("blocke", "Dont E Leave Ultimatum")).SetValue(true);
                tcmenu.AddItem(new MenuItem("minerange", "Minimum E Range")).SetValue(new Slider(165, 0, (int) E.Range));
                comenu.AddSubMenu(tcmenu);

                RootMenu.AddSubMenu(comenu);


                var hamenu = new Menu("-] Harass", "hamenu");
                hamenu.AddItem(new MenuItem("useqharass", "Use Q")).SetValue(true);
                hamenu.AddItem(new MenuItem("usewharass", "Use W")).SetValue(true);
                hamenu.AddItem(new MenuItem("harassmana", "Harass Mana %")).SetValue(new Slider(65));
                RootMenu.AddSubMenu(hamenu);

                var clmenu = new Menu("-] Clear", "clmenu");
                clmenu.AddItem(new MenuItem("clearnearenemy", "Dont Clear Near Enemy")).SetValue(true);
                clmenu.AddItem(new MenuItem("useqclear", "Use Q")).SetValue(true);
                clmenu.AddItem(new MenuItem("usewclear", "Use W")).SetValue(true);
                clmenu.AddItem(new MenuItem("usewlane", "-> Use In Lane")).SetValue(false);
                clmenu.AddItem(new MenuItem("usewlanehit", "-> Minimum Hit in Lane")).SetValue(new Slider(3, 1, 6));
                clmenu.AddItem(new MenuItem("useeclear", "Use E")).SetValue(true);
                clmenu.AddItem(new MenuItem("clearmana", "Clear Mana %")).SetValue(new Slider(55));
                RootMenu.AddSubMenu(clmenu);

                var fmenu = new Menu("-] Flee", "fmenu");
                fmenu.AddItem(new MenuItem("useeflee", "Use E")).SetValue(true);
                RootMenu.AddSubMenu(fmenu);

                var exmenu = new Menu("-] Events", "exmenu");
                exmenu.AddItem(new MenuItem("interruptx", "Interrupt")).SetValue(false).ValueChanged +=
                    (sender, eventArgs) => eventArgs.Process = false;
                exmenu.AddItem(new MenuItem("antigapcloserx", "Anti-Gapcloser")).SetValue(false).ValueChanged +=
                    (sender, eventArgs) => eventArgs.Process = false;
                RootMenu.AddSubMenu(exmenu);

                var skmenu = new Menu("-] Skins", "skmenu");
                var skinitem = new MenuItem("useskin", "Enabled");
                skmenu.AddItem(skinitem).SetValue(false);

                skinitem.ValueChanged += (sender, eventArgs) =>
                {
                    if (!eventArgs.GetNewValue<bool>())
                    {
                        //ObjectManager.//Player.SetSkin(ObjectManager.Player.BaseSkinName, ObjectManager.Player.SkinId);
                    }
                };

                skmenu.AddItem(new MenuItem("skinid", "Skin Id")).SetValue(new Slider(1, 0, 4));
                RootMenu.AddSubMenu(skmenu);

                var drmenu = new Menu("-] Draw", "drmenu");
                drmenu.AddItem(new MenuItem("drawhpbarfill", "Draw HPBarFill")).SetValue(true);
                drmenu.AddItem(new MenuItem("drawmyehehe", "Draw E")).SetValue(new Circle(true, System.Drawing.Color.FromArgb(165, 0, 220, 144)));
                drmenu.AddItem(new MenuItem("drawmywhehe", "Draw W")).SetValue(new Circle(true, System.Drawing.Color.FromArgb(165, 37, 230, 255)));
                drmenu.AddItem(new MenuItem("drawmyrhehe", "Draw R")).SetValue(new Circle(true, System.Drawing.Color.FromArgb(165, 0, 220, 144)));
                RootMenu.AddSubMenu(drmenu);

                RootMenu.AddToMainMenu();

                Game.OnUpdate += Game_OnUpdate;
                Drawing.OnDraw += Drawing_OnDraw;
                Drawing.OnEndScene += Drawing_OnEndScene;
                Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnSpellCast;
                EloBuddy.Player.OnIssueOrder += CamilleOnIssueOrder;
                GameObject.OnCreate += Obj_GeneralParticleEmitter_OnCreate;

                // test
                var color = System.Drawing.Color.FromArgb(200, 0, 220, 144);
                var hexargb = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

                Chat.Print("<b><font color=\"" + hexargb + "\">Camille#</font></b> - Loaded!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (RootMenu.Item("drawhpbarfill").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => ene.IsValidTarget() && !ene.IsZombie))
                {
                    var color = R.IsReady() && IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 90)
                        : new ColorBGRA(255, 255, 0, 90);

                    BarIndicator.unit = enemy;
                    BarIndicator.drawDmg((float) ComboDamage(enemy), color);
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var eCircle = RootMenu.Item("drawmyehehe").GetValue<Circle>();
            if (eCircle.Active)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, eCircle.Color);
            }

            var wCircle = RootMenu.Item("drawmywhehe").GetValue<Circle>();
            if (wCircle.Active)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, wCircle.Color);
            }

            var rCircle = RootMenu.Item("drawmyrhehe").GetValue<Circle>();
            if (rCircle.Active)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, rCircle.Color);
            }
        }

        private static void Obj_GeneralParticleEmitter_OnCreate(GameObject sender, EventArgs args)
        {
            var emitter = sender as Obj_GeneralParticleEmitter;
            if (emitter != null && emitter.Name.ToLower() == REmitterName.ToLower())
            {
                rPoint[Game.Time] = emitter;
            }
        }

        private static bool RKappa()
        {
            return RootMenu.SubMenu("cmenu").SubMenu("abmenu").SubMenu("whemenu").Items.Any(i => i.GetValue<bool>()) &&
                 Player.GetEnemiesInRange(E.Range*2).Any(ez => RootMenu.Item("whR" + ez.ChampionName).GetValue<bool>());
        }

        private static void CamilleOnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (!RootMenu.Item("useehelper").GetValue<bool>())
            {
                return;
            }

            if (OnWall && E.IsReady() && RootMenu.Item("usecombo").GetValue<KeyBind>().Active)
            {
                var issueOrderPos = args.TargetPosition;
                if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
                {
                    var issueOrderDirection = (issueOrderPos - Player.Position).To2D().Normalized();

                    var aiHero = TargetSelector.GetTarget(E.Range + 750, TargetSelector.DamageType.Physical);
                    if (aiHero != null)
                    {
                        var heroDirection = (aiHero.Position - Player.Position).To2D().Normalized();
                        if (heroDirection.AngleBetween(issueOrderDirection) > 20)
                        {
                            args.Process = false;
                            Orbwalking.Orbwalk(aiHero, aiHero.Position);
                            //Console.WriteLine("Redirected @ Hero");
                        }
                    }
                }
            }

            if (OnWall && E.IsReady() && RootMenu.Item("useclear").GetValue<KeyBind>().Active)
            {
                var issueOrderPos = args.TargetPosition;
                if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
                {
                    var issueOrderDirection = (issueOrderPos - Player.Position).To2D().Normalized();

                    var aiMob = MinionManager.GetMinions(Player.Position, E.Range,
                            MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

                    if (aiMob != null)
                    {
                        var heroDirection = (aiMob.Position - Player.Position).To2D().Normalized();
                        if (heroDirection.AngleBetween(issueOrderDirection) > 20)
                        {
                            args.Process = false;
                            Orbwalking.Orbwalk(null, aiMob.Position);
                            //Console.WriteLine("Redirected @ Mob");
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.IsAutoAttack())
            {
                var aiHero = args.Target as AIHeroClient;
                if (aiHero.IsValidTarget())
                {
                    if (!Player.UnderTurret(true) || RootMenu.Item("usecombo").GetValue<KeyBind>().Active)
                    {
                        if (!Q.IsReady() || HasQ && !HasQ2)
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }
                    }
                }

                if (RootMenu.Item("usecombo").GetValue<KeyBind>().Active)
                {
                    if (aiHero.IsValidTarget() && RootMenu.Item("useqcombo").GetValue<bool>())
                    {
                        UseQ(aiHero);
                    }
                }

                if (RootMenu.Item("useharass").GetValue<KeyBind>().Active)
                {
                    if (aiHero.IsValidTarget())
                    {
                        if (Player.Mana / Player.MaxMana * 100 < RootMenu.Item("harassmana").GetValue<Slider>().Value)
                        {
                            return;
                        }

                        UseQ(aiHero);
                    }
                }

                if (RootMenu.Item("useclear").GetValue<KeyBind>().Active)
                {
                    var aiMob = args.Target as Obj_AI_Minion;
                    if (aiMob != null && aiMob.IsValidTarget())
                    {

                        if (Player.UnderTurret(true) && Player.CountEnemiesInRange(1000) > 0)
                        {
                            return;
                        }

                        if (!Q.IsReady() || HasQ && !HasQ2)
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }
                    }

                    var aiBase = args.Target as Obj_AI_Base;
                    if (aiBase != null && aiBase.IsValidTarget() && aiBase.Name.StartsWith("Minion"))
                    {
                        #region LaneClear Q

                        if (Player.CountEnemiesInRange(1000) < 1 || !RootMenu.Item("clearnearenemy").GetValue<bool>() || Player.UnderAllyTurret())
                        {
                            if (aiBase.UnderTurret(true) && Player.CountEnemiesInRange(1000) > 0 && !Player.UnderAllyTurret())
                            {
                                return;
                            }

                            if (Player.Mana / Player.MaxMana * 100 < RootMenu.Item("clearmana").GetValue<Slider>().Value)
                            {
                                if (Player.CountEnemiesInRange(1000) > 0 && !Player.UnderAllyTurret())
                                {
                                    return;
                                }
                            }

                            #region AA -> Q 

                            if (Q.IsReady() && RootMenu.Item("useqclear").GetValue<bool>())
                            {
                                if (aiBase.Distance(Player.ServerPosition) <= Q.Range + 90)
                                {
                                    UseQ(aiBase);
                                }
                            }

                            #endregion
                        }

                        #endregion
                    }

                    #region AA-> Q any attackable
                    var unit = args.Target as AttackableUnit;
                    if (unit != null)
                    {
                        // if jungle minion
                        var m = unit as Obj_AI_Minion;
                        if (m != null && !m.BaseSkinName.StartsWith("sru_plant"))
                        {
                            #region AA -> Q
                            if (Q.IsReady() && RootMenu.Item("useqclear").GetValue<bool>())
                            {
                                if (m.Position.Distance(Player.ServerPosition) <= Q.Range + 90)
                                {
                                    UseQ(m);
                                }
                            }

                            #endregion
                        }

                        if (Q.IsReady() && !HeroManager.Enemies.Any(x => x.IsValidTarget(1200)))
                        {
                            UseQ(unit);
                        }
                    }

                    #endregion
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            foreach (var entry in rPoint)
            {
                var timestamp = entry.Key;
                if (Game.Time - timestamp > 4f)
                {
                    rPoint.Remove(timestamp);
                    break;
                }
            }

            if (RootMenu.Item("useflee").GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(null, Game.CursorPos);
                UseE(Game.CursorPos, false);
            }

            if (RootMenu.Item("useskin").GetValue<bool>())
            {
                //Player.SetSkin(Player.BaseSkinName, RootMenu.Item("skinid").GetValue<Slider>().Value);
            }

            if (RootMenu.Item("r33").GetValue<bool>())
            {
                var rtarget = HeroManager.Enemies.FirstOrDefault(x => x.HasBuff(RBuffName));
                if (rtarget != null && rtarget.IsValidTarget() && !rtarget.IsZombie)
                {
                    if (Orbwalking.InAutoAttackRange(rtarget))
                    {
                        TargetSelector.SetTarget(rtarget);
                        Orbwalker.ForceTarget(rtarget);
                    }
                }
            }

            if (IsDashing || OnWall)
            {
                return;
            }

            if (RootMenu.Item("usecombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }

            if (RootMenu.Item("useclear").GetValue<KeyBind>().Active)
            {
                if (Player.Mana / Player.MaxMana * 100 > RootMenu.Item("clearmana").GetValue<Slider>().Value)
                {
                    Clear();
                }
            }

            if (RootMenu.Item("useharass").GetValue<KeyBind>().Active)
            {
                if (Player.Mana / Player.MaxMana * 100 > RootMenu.Item("harassmana").GetValue<Slider>().Value)
                {
                    Harass();
                }
            }

            tt = Player.CountAlliesInRange(1500) > 1 && Player.CountEnemiesInRange(1350) > 2 ||
                 Player.CountEnemiesInRange(1200) > 2;
        }

        static void Combo()
        {
            var target = TargetSelector.GetTarget(E.IsReady() ? E.Range * 2 : W.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && !target.IsZombie)
            {
                if (RootMenu.Item("usewcombo").GetValue<bool>())
                    UseW(target);

                if (RootMenu.Item("useecombo").GetValue<bool>())
                    UseE(target.ServerPosition);

                if (RootMenu.Item("usercombo").GetValue<bool>())
                    UseR(target);
            }
        }

        static void Harass()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget() && !target.IsZombie)
            {
                if (RootMenu.Item("usewharass").GetValue<bool>())
                    UseW(target);
            }
        }

        private static void Clear()
        {
            var minions = MinionManager.GetMinions(Player.Position, W.Range,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);


            if (RootMenu.Item("usewlane").GetValue<bool>() && W.IsReady())
            {
                var minz = MinionManager.GetMinions(Player.Position, W.Range);

                var farmradius =
                    MinionManager.GetBestCircularFarmLocation(minz.Select(x => x.Position.To2D()).ToList(), 165f,
                        W.Range);

                if (farmradius.MinionsHit >= RootMenu.Item("usewlanehit").GetValue<Slider>().Value)
                {
                    W.Cast(farmradius.Position);
                }
            }

            foreach (var unit in minions.Where(m => !m.Name.Contains("Mini")))
            {
                if (!W.IsReady() && !ChargingW && RootMenu.Item("useeclear").GetValue<bool>())
                    UseE(unit.ServerPosition, false);

                if (RootMenu.Item("usewclear").GetValue<bool>())
                    UseW(unit);
            }
        }

        static void UseQ(AttackableUnit t)
        {
            if (Q.IsReady())
            {
                if (!HasQ || HasQ2)
                {
                    if (Q.Cast())
                    {
                        Orbwalking.ResetAutoAttackTimer(); // TEMP
                    }
                }
            }
        }

        static void UseW(Obj_AI_Base target)
        {
            if (OnWall || CanW(target) == false || ChargingW)
            {
                return;
            }

            if (W.IsReady() && target.Distance(Player.ServerPosition) <= W.Range)
            {
                W.Cast(target.ServerPosition);
            }
        }

        static void UseE(Vector3 p, bool combo = true)
        {
            try
            {
                if (IsDashing || OnWall || !E.IsReady())
                {
                    return;
                }

                if (combo)
                {
                    if (!RootMenu.Item("useecombo").GetValue<bool>()) 
                    {
                        return;
                    }

                    if (Player.Distance(p) < RootMenu.Item("minerange").GetValue<Slider>().Value)
                    {
                        return;
                    }

                    if (RootMenu.Item("blocke").GetValue<bool>())
                    {
                        if (rPoint.Any(entry => p.Distance(entry.Value.Position) > 450))
                        {
                            return;
                        }
                    }

                    if (p.UnderTurret(true) && RootMenu.Item("eturret").GetValue<KeyBind>().Active)
                    {
                        return;
                    }
                }

                var posChecked = 0;
                var maxPosChecked = 80;
                var posRadius = 145;
                var radiusIndex = 0;

                var candidatePos = new List<Vector2>();

                while (posChecked < maxPosChecked)
                {
                    radiusIndex++;

                    var curRadius = radiusIndex * (0x2 * posRadius);
                    var curCurcleChecks = (int) Math.Ceiling((2 * Math.PI * curRadius) / (2 * (double) posRadius));

                    for (var i = 1; i < curCurcleChecks; i++)
                    {
                        posChecked++;

                        var cRadians = (0x2 * Math.PI / (curCurcleChecks - 1)) * i;
                        var xPos = (float) Math.Floor(p.X + curRadius * Math.Cos(cRadians));
                        var yPos = (float) Math.Floor(p.Y + curRadius * Math.Sin(cRadians));

                        var desiredPos = new Vector2(xPos, yPos);

                        if (RootMenu.Item("blocke").GetValue<bool>())
                        {
                            if (rPoint.Any(entry => p.Distance(entry.Value.Position) > 450))
                            {
                                continue;
                            }
                        }

                        if (desiredPos.IsWall())
                        {
                            candidatePos.Add(desiredPos);
                        }
                    }
                }

                var bestWallPoint =
                    candidatePos.Where(x => Player.Distance(x) <= E.Range && x.Distance(p) <= E.Range)
                        .OrderBy(x => x.Distance(p))
                        .FirstOrDefault();

                if (E.IsReady() && bestWallPoint.IsValid())
                {
                    if (RootMenu.Item("wdash").GetValue<bool>() && combo)
                    {
                        var eTravelTime = 0f; // todo
                        if (eTravelTime <= 2000)
                        {
                            W.Cast(p);
                        }
                    }

                    if (E.Cast(bestWallPoint))
                    {
                        candidatePos.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static bool tt;
        static void UseR(AIHeroClient target)
        {
            if (target.Distance(Player.ServerPosition) <= R.Range)
            {
                if (R.IsReady() && ComboDamage(target) >= target.Health)
                {
                    if (!tt || tt && !RKappa() || RootMenu.Item("whR" + target.ChampionName).GetValue<bool>())
                    { 
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        static void Secure(bool i, bool o)
        {
            return;

            if (i && Q.IsReady())
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range)))
                {
                    // todo: qdmg;
                    UseQ(hero);
                }
            }

            if (o)
            {
                foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range * 2)))
                {
                    // todo: qdmg;
                    UseE(hero.ServerPosition);
                    UseQ(hero);
                }
            }
        }

        static bool CanW(Obj_AI_Base target)
        {
            const float wCastTime = 2000f;

            if (OnWall || HasQ2)
            {
                return false;
            }

            if (Player.GetAutoAttackDamage(target, true) * 3 >= target.Health)
            {
                return false;
            }

            var b = Player.GetBuff(QBuffName);
            if (b != null && (b.EndTime - Game.Time) * 1000 <= wCastTime)
            {
                return false;
            }

            var c = Player.GetBuff(Q2BuffName);
            if (c != null && (c.EndTime - Game.Time) * 1000 <= wCastTime)
            {
                return false;
            }

            return true;
        }

        private static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDamage(unit) / 1.65 >= unit.Health;
        }

        private static double ComboDamage(Obj_AI_Base unit)
        {
            if (unit == null)
                return 0d;

            var qq = new[] { 2, 3, 4, 4 } [(Math.Min(Player.Level, 18) / 6)];

            return Math.Min(qq, Player.Mana / Q.ManaCost) * Qdmg(unit, false) + Wdmg(unit) +
                   RBonus(Player.GetAutoAttackDamage(unit, true), unit) * qq + Edmg(unit);
        }


        private static double Qdmg(Obj_AI_Base target, bool aareset = true)
        {
            double dmg = 0;

            if (Q.IsReady() && target != null)
            {

                dmg += Player.CalcDamage(target, Damage.DamageType.Physical, Player.GetAutoAttackDamage(target, true) +
                    (new[]  { 0.2, 0.25, 0.30, 0.35, 0.40 } [Q.Level - 1] * (Player.BaseAttackDamage + Player.FlatPhysicalDamageMod)));

                var dmgreg = Player.CalcDamage(target, Damage.DamageType.Physical, Player.GetAutoAttackDamage(target, true) +
                    (new[] { 0.4, 0.5, 0.6, 0.7, 0.8 }[Q.Level - 1] * (Player.BaseAttackDamage + Player.FlatPhysicalDamageMod)));

                var pct = 52 + (3 * Math.Min(16, Player.Level));

                var dmgtrue = Player.CalcDamage(target, Damage.DamageType.True, dmgreg * pct / 100);

                dmg += dmgtrue;

                if (aareset)
                    dmg += (RBonus(Player.GetAutoAttackDamage(target), target)); // aa reset
            }

            return dmg;
        }

        private static double Wdmg(Obj_AI_Base target, bool bonus = false)
        {
            double dmg = 0;

            if (W.IsReady() && target != null)
            {
                dmg += Player.CalcDamage(target, Damage.DamageType.Physical,
                    (new[] { 65, 95, 125, 155, 185 } [W.Level - 1] + (0.6 * Player.FlatPhysicalDamageMod)));

                var wpc = new[] { 6, 6.5, 7, 7.5, 8 };
                var pct = wpc[W.Level - 1];

                if (Player.FlatPhysicalDamageMod >= 100)
                    pct += Math.Min(300, Player.FlatPhysicalDamageMod) * 3 / 100;

                if (bonus && target.Distance(Player.ServerPosition) > 400)
                    dmg += Player.CalcDamage(target, Damage.DamageType.Physical, pct * (target.MaxHealth / 100));
            }

            return dmg;
        }

        private static double Edmg(Obj_AI_Base target)
        {
            double dmg = 0;

            if (E.IsReady() && target != null)
            {
                dmg += Player.CalcDamage(target, Damage.DamageType.Physical,
                    (new[] { 70, 115, 160, 205, 250  }[E.Level - 1] + (0.75 * Player.FlatPhysicalDamageMod)));
            }

            return dmg;
        }

        private static double RBonus(double dmg, Obj_AI_Base target)
        {
            if (R.IsReady() || target.HasBuff(RBuffName))
            {
                var xtra = new[] { 5, 10, 15, 15 } [R.Level - 1] + (new[] { 4, 6, 8, 8 } [R.Level - 1] * (target.Health / 100));
                return dmg + xtra;
            }

            return dmg;
        }
    }
}
