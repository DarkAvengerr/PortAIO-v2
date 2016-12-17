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

        internal static bool tt;
        internal static bool HasQ2 => Player.HasBuff(Q2BuffName);
        internal static bool HasQ => Player.HasBuff(QBuffName);
        internal static bool OnWall => Player.HasBuff(WallBuffName) || E.Instance.Name != "CamilleE";
        internal static bool IsDashing => Player.HasBuff(EDashBuffName + "1") || Player.HasBuff(EDashBuffName + "2");
        internal static bool ChargingW => Player.HasBuff(WBuffName);
        internal static bool KnockedBack(Obj_AI_Base target) => target != null && target.HasBuff(KnockBackBuffName);

        internal static string WBuffName => "camillewconeslashcharge";
        internal static string EDashBuffName => "camilleedash";
        internal static string WallBuffName => "camilleedashtoggle"; 
        internal static string QBuffName => "camilleqprimingstart";
        internal static string Q2BuffName => "camilleqprimingcomplete";
        internal static string RBuffName => "camillertether";
        internal static string REmitterName => "Camille_Base_R_Indicator_Edge.troy";
        internal static string KnockBackBuffName => "camilleeknockback2";

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
                    return;

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

                var revade = new Menu("-] Evade", "revade");
                revade.AddItem(new MenuItem("revade", "Use R to Evade")).SetValue(true);

                foreach (var spell in from entry in Evadeable.DangerList
                    select entry.Value into spell from hero in
                        HeroManager.Enemies
                            .Where(x => x.ChampionName.ToLower() == spell.ChampionName.ToLower())
                    select spell)
                {
                    revade.AddItem(new MenuItem("revade" + spell.SDataName.ToLower(), "-> " + spell.ChampionName + " R"))
                        .SetValue(true);
                }
                
                tcmenu.AddItem(new MenuItem("r33", "Focus R Target")).SetValue(true);
                tcmenu.AddItem(new MenuItem("r55", "Only R Selected Target")).SetValue(false);
                tcmenu.AddItem(new MenuItem("eturret", "Dont E Under Turret")).SetValue(new KeyBind('L', KeyBindType.Toggle, true)).Permashow();
                tcmenu.AddItem(new MenuItem("blocke", "Dont E Leave Ultimatum")).SetValue(true);
                tcmenu.AddItem(new MenuItem("minerange", "Minimum E Range")).SetValue(new Slider(165, 0, (int) E.Range));
                tcmenu.AddItem(new MenuItem("enhancede", "Enhanced E Precision")).SetValue(false);
                tcmenu.AddItem(new MenuItem("www", "Expirimental Combo")).SetValue(false).SetTooltip("W -> E");
                comenu.AddSubMenu(tcmenu);

                comenu.AddSubMenu(revade);
                comenu.AddSubMenu(abmenu);


                RootMenu.AddSubMenu(comenu);


                var hamenu = new Menu("-] Harass", "hamenu");
                hamenu.AddItem(new MenuItem("useqharass", "Use Q")).SetValue(true);
                hamenu.AddItem(new MenuItem("usewharass", "Use W")).SetValue(true);
                hamenu.AddItem(new MenuItem("harassmana", "Harass Mana %")).SetValue(new Slider(65));
                RootMenu.AddSubMenu(hamenu);

                var clmenu = new Menu("-] Clear", "clmenu");
                clmenu.AddItem(new MenuItem("clearnearenemy", "Dont Clear Near Enemy")).SetValue(true);
                clmenu.AddItem(new MenuItem("t11", "Use Hydra")).SetValue(true);
                clmenu.AddItem(new MenuItem("useqclear", "Use Q")).SetValue(true);
                clmenu.AddItem(new MenuItem("usewclear", "Use W")).SetValue(true);
                clmenu.AddItem(new MenuItem("usewlane", "-> Use In Lane")).SetValue(false);
                clmenu.AddItem(new MenuItem("usewlanehit", "-> Minimum Hit in Lane")).SetValue(new Slider(3, 1, 6));
                clmenu.AddItem(new MenuItem("useeclear", "Use E")).SetValue(true);
                clmenu.AddItem(new MenuItem("clearmana", "Clear Mana %")).SetValue(new Slider(35));
                RootMenu.AddSubMenu(clmenu);

                var fmenu = new Menu("-] Flee", "fmenu");
                fmenu.AddItem(new MenuItem("useeflee", "Use E")).SetValue(true);
                RootMenu.AddSubMenu(fmenu);

                var exmenu = new Menu("-] Events", "exmenu");
                exmenu.AddItem(new MenuItem("interrupt", "Interrupt")).SetValue(true);
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
                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
                GameObject.OnCreate += Obj_GeneralParticleEmitter_OnCreate;
                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

                var color = System.Drawing.Color.FromArgb(200, 0, 220, 144);
                var hexargb = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

                Chat.Print("<b><font color=\"" + hexargb + "\">Camille#</font></b> - Loaded!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var attacker = sender as AIHeroClient;
            if (attacker != null && attacker.IsEnemy)
            {
                var aiTarget = args.Target as AIHeroClient;

                var bestTarget = TargetSelector.GetTarget(R.Range + 100, TargetSelector.DamageType.Physical);
                if (bestTarget == null)
                {
                    return;
                }

                if (R.IsReady() && RootMenu.Item("revade").GetValue<bool>())
                {
                    if (attacker.Distance(Player) <= R.Range + 35)
                    {
                        foreach (var spell in Evadeable.DangerList.Select(entry => entry.Value)
                            .Where(spell => spell.SDataName.ToLower() == args.SData.Name.ToLower())
                            .Where(spell => RootMenu.Item("revade" + spell.SDataName.ToLower()).GetValue<bool>()))
                        {
                            switch (spell.EvadeType)
                            {
                                case EvadeType.Target:
                                    if (aiTarget != null && aiTarget.IsMe)
                                    {
                                        UseR(bestTarget, true);
                                    }
                                    break;

                                case EvadeType.SelfCast:
                                    if (attacker.Distance(Player) <= R.Range)
                                    {
                                        UseR(bestTarget, true);
                                    }
                                    break;
                                case EvadeType.SkillshotLine:
                                    var lineStart = args.Start.To2D();
                                    var lineEnd = args.End.To2D();

                                    if (lineStart.Distance(lineEnd) < R.Range)
                                        lineEnd = lineStart + (lineEnd - lineStart).Normalized() * R.Range + 25;

                                    if (lineStart.Distance(lineEnd) > R.Range)
                                        lineEnd = lineStart + (lineEnd - lineStart).Normalized() * R.Range * 2;

                                    var spellProj = Player.ServerPosition.To2D().ProjectOn(lineStart, lineEnd);
                                    if (spellProj.IsOnSegment)
                                    {
                                        UseR(bestTarget, true);
                                    }
                                    break;

                                case EvadeType.SkillshotCirce:
                                    var curStart = args.Start.To2D();
                                    var curEnd = args.End.To2D();

                                    if (curStart.Distance(curEnd) > R.Range)
                                        curEnd = curStart + (curEnd - curStart).Normalized() * R.Range;

                                    if (curEnd.Distance(Player) <= R.Range)
                                    {
                                        UseR(bestTarget, true);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (RootMenu.Item("interrupt").GetValue<bool>())
            {
                if (sender.IsValidTarget(E.Range) && E.IsReady())
                {
                    UseE(sender.ServerPosition);
                }
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
            if (OnWall && E.IsReady() && RootMenu.Item("usecombo").GetValue<KeyBind>().Active)
            {
                var issueOrderPos = args.TargetPosition;
                if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
                {
                    var issueOrderDirection = (issueOrderPos - Player.Position).To2D().Normalized();

                    var aiHero = TargetSelector.GetTarget(E.Range + 100, TargetSelector.DamageType.Physical);
                    if (aiHero != null)
                    {
                        var heroDirection = (aiHero.Position - Player.Position).To2D().Normalized();
                        if (heroDirection.AngleBetween(issueOrderDirection) > 10)
                        {
                            var isDangerPos = aiHero.ServerPosition.UnderTurret(true) && RootMenu.Item("eturret").GetValue<KeyBind>().Active;
                            if (isDangerPos == false)
                            {
                                args.Process = false;
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, aiHero.ServerPosition, false);
                            }
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

                    var aiMob = MinionManager.GetMinions(Player.Position, W.Range + 100,
                        MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

                    if (aiMob != null)
                    {
                        var heroDirection = (aiMob.Position - Player.Position).To2D().Normalized();
                        if (heroDirection.AngleBetween(issueOrderDirection) > 10)
                        {
                            args.Process = false;
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, aiMob.ServerPosition, false);
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
                            if (RootMenu.Item("t11").GetValue<bool>())
                            {
                                if (!aiMob.IsMinion || (Player.CountEnemiesInRange(900) < 1 
                                    || !RootMenu.Item("clearnearenemy").GetValue<bool>() || Player.UnderAllyTurret()))
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
                    }

                    var aiBase = args.Target as Obj_AI_Base;
                    if (aiBase != null && aiBase.IsValidTarget() && aiBase.Name.StartsWith("Minion"))
                    {
                        #region LaneClear Q

                        if (Player.CountEnemiesInRange(1000) < 1 || Player.UnderAllyTurret() 
                            || !RootMenu.Item("clearnearenemy").GetValue<bool>())
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
                        if (Player.CountEnemiesInRange(1000) < 1 || Player.UnderAllyTurret()
                            || !RootMenu.Item("clearnearenemy").GetValue<bool>())
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

                            if (Q.IsReady())
                            {
                                if (RootMenu.Item("useqclear").GetValue<bool>())
                                {
                                    UseQ(unit);
                                }
                            }
                        }
                    }

                    #endregion
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(!ChargingW);

            foreach (var entry in rPoint)
            {
                var timestamp = entry.Key;
                if (Game.Time - timestamp > 4f)
                {
                    rPoint.Remove(timestamp);
                    break;
                }

                var ultimatum = entry.Value;
                if (!ultimatum.IsValid || !ultimatum.IsVisible)
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
                MinionTypes.All, MinionTeam.NotAlly, MinionOrderTypes.MaxHealth);

            foreach (var unit in minions)
            {
                if (!unit.Name.Contains("Mini"))
                {
                    if (RootMenu.Item("usewclear").GetValue<bool>())
                    {
                        UseW(unit);
                    }

                    if (!W.IsReady() || !RootMenu.Item("usewclear").GetValue<bool>())
                    {
                        if (!ChargingW && RootMenu.Item("useeclear").GetValue<bool>())
                            UseE(unit.ServerPosition, false);
                    }
                }
                else
                {
                    if (Player.CountEnemiesInRange(1000) < 1 ||
                        !RootMenu.Item("clearnearenemy").GetValue<bool>())
                    {
                        if (RootMenu.Item("usewlane").GetValue<bool>() && W.IsReady())
                        {
                            var farmradius =
                                MinionManager.GetBestCircularFarmLocation(
                                    minions.Where(x => x.IsMinion).Select(x => x.Position.To2D()).ToList(), 165f, W.Range);

                            if (farmradius.MinionsHit >= RootMenu.Item("usewlanehit").GetValue<Slider>().Value)
                            {
                                W.Cast(farmradius.Position);
                            }
                        }
                    }
                }
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
                        //Orbwalking.ResetAutoAttackTimer(); // TEMP
                    }
                }
                else
                {
                    var aiHero = t as AIHeroClient;
                    if (aiHero != null && Qdmg(aiHero, false) + Player.GetAutoAttackDamage(aiHero, true) * 1 >= aiHero.Health)
                    {
                        if (Q.Cast())
                        {
                            //Orbwalking.ResetAutoAttackTimer(); // TEMP
                        }
                    }
                }
            }
        }

        static void UseW(Obj_AI_Base target)
        {
            if (OnWall || !CanW(target))
            {
                return;
            }

            if (ChargingW || IsDashing)
            {
                return;
            }

            if (KnockedBack(target))
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
            if (IsDashing || OnWall || ChargingW || !E.IsReady())
            {
                return;
            }

            if (combo)
            {
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
            var maxPosChecked = 40;
            var posRadius = 145;
            var radiusIndex = 0;

            if (RootMenu.Item("enhancede").GetValue<bool>())
            {
                maxPosChecked = 80;
                posRadius = 65;
            }

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
                        if (rPoint.Any(entry => desiredPos.Distance(entry.Value.Position) > 450))
                        {
                            continue;
                        }
                    }

                    if (ChargingW)
                    {
                        var wtarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
                        if (wtarget != null)
                        {
                            if (desiredPos.Distance(wtarget.ServerPosition) > W.Range)
                            {
                                return;
                            }
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
                if (W.IsReady() && RootMenu.Item("usewcombo").GetValue<bool>())
                {
                    if (combo && RootMenu.Item("www").GetValue<bool>()) // dumb lol
                    {
                        var mouseDir = Player.ServerPosition + (Game.CursorPos - Player.ServerPosition).Normalized() * 265;
                        var wallPointReversed = bestWallPoint.Extend(mouseDir.To2D(), 1000);

                        // expiremental
                        // xd

                        int dashSpeedEst = 1450;
                        int hookSpeedEst = 1250;

                        float e1Time = 1000 * (Player.Distance(bestWallPoint) / hookSpeedEst);
                        float meToWall = e1Time + (1000 * (Player.Distance(bestWallPoint) / dashSpeedEst));
                        float wallToHero = (1000 * (bestWallPoint.Distance(p) / dashSpeedEst));

                        var travelTime = 250 + meToWall + wallToHero;
                        if (travelTime >= 1250 && travelTime <= 1750)
                        {
                            W.Cast(wallPointReversed);
                        }

                        if (travelTime > 1750)
                        {
                            var delay = 100 + (travelTime - 1750);
                            LeagueSharp.Common.Utility.DelayAction.Add((int) delay, () => W.Cast(wallPointReversed));
                        }
                    }
                }

                E.Cast(bestWallPoint);
            }
        }

        static void UseR(AIHeroClient target, bool force = false)
        {
            if (R.IsReady() && force)
            {
                R.CastOnUnit(target);
            }

            if (target.Distance(Player) <= R.Range)
            {
                if (RootMenu.Item("r55").GetValue<bool>())
                {
                    var unit = TargetSelector.GetSelectedTarget();
                    if (unit == null || unit.NetworkId != target.NetworkId)
                    {
                        return;
                    }
                }

                if (Qdmg(target) + Player.GetAutoAttackDamage(target) * 2 >= target.Health)
                {
                    if (Orbwalking.InAutoAttackRange(target))
                    {
                        return;
                    }
                }

                if (R.IsReady() && ComboDamage(target) >= target.Health)
                {
                    if (!tt || tt && !RKappa() || RootMenu.Item("whR" + target.ChampionName).GetValue<bool>())
                    { 
                        R.CastOnUnit(target);
                    }
                }
            }
        }

        static bool CanW(Obj_AI_Base target)
        {
            const float wCastTime = 2000f;

            if (OnWall || HasQ2 || IsDashing)
            {
                return false;
            }

            if (Orbwalking.InAutoAttackRange(target))
            {
                if (Player.GetAutoAttackDamage(target, true) * 2 + Qdmg(target) >= target.Health)
                {
                    return false;
                }
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


        private static double Qdmg(Obj_AI_Base target, bool includeq2 = true)
        {
            double dmg = 0;

            if (Q.IsReady() && target != null)
            {
                dmg += Player.CalcDamage(target, Damage.DamageType.Physical, Player.GetAutoAttackDamage(target, true) +
                    (new[]  { 0.2, 0.25, 0.30, 0.35, 0.40 } [Q.Level - 1] * (Player.BaseAttackDamage + Player.FlatPhysicalDamageMod)));

                var dmgreg = Player.CalcDamage(target, Damage.DamageType.Physical, Player.GetAutoAttackDamage(target, true) +
                    (new[] { 0.4, 0.5, 0.6, 0.7, 0.8 } [Q.Level - 1] * (Player.BaseAttackDamage + Player.FlatPhysicalDamageMod)));

                var pct = 52 + (3 * Math.Min(16, Player.Level));

                var dmgtrue = Player.CalcDamage(target, Damage.DamageType.True, dmgreg * pct / 100);

                if (includeq2)
                {
                    dmg += dmgtrue;
                }
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
                    (new[] { 70, 115, 160, 205, 250 } [E.Level - 1] + (0.75 * Player.FlatPhysicalDamageMod)));
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
