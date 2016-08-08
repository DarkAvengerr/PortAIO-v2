using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
using EloBuddy;

namespace KurisuRiven
{
    internal class KurisuRiven
    {
        #region Riven: Main

        private static int lastq;
        private static int lastw;
        private static int laste;
        private static int lastaa;
        private static int lasthd;
        private static int lastwd;

        private static bool canq;
        private static bool canw;
        private static bool cane;
        private static bool canmv;
        private static bool canaa;
        private static bool canws;
        private static bool canhd;
        private static bool hashd;

        private static bool didq;
        private static bool didw;
        private static bool dide;
        private static bool didws;
        private static bool didaa;
        private static bool didhd;
        private static bool didhs;
        private static bool ssfl;

        private static Menu menu;
        private static Spell q, w, e, r;
        private static Orbwalking.Orbwalker orbwalker;
        private static AIHeroClient player = ObjectManager.Player;
        private static HpBarIndicator hpi = new HpBarIndicator();
        private static Obj_AI_Base qtarg; // semi q target

        private static int qq;
        private static int cc;
        private static int pc;  
        private static bool uo;
        private static SpellSlot flash;

        private static float truerange;
        private static Vector3 movepos;
        #endregion

        # region Riven: Utils

        private static bool menubool(string item)
        {
            return menu.Item(item).GetValue<bool>();
        }

        private static int menuslide(string item)
        {
            return menu.Item(item).GetValue<Slider>().Value;
        }

        private static int menulist(string item)
        {
            return menu.Item(item).GetValue<StringList>().SelectedIndex;
        }

        private static float xtra(float dmg)
        {
           return r.LSIsReady() ? (float) (dmg + (dmg*0.2)) : dmg;
        }

        private static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDamage(unit) / 1.65 >= unit.Health;
        }

        private static Obj_AI_Base GetCenterMinion()
        {
            var minionposition = MinionManager.GetMinions(300 + q.Range).Select(x => x.Position.LSTo2D()).ToList();
            var center = MinionManager.GetBestCircularFarmLocation(minionposition, 250, 300 + q.Range);

            return center.MinionsHit >= 3
                ? MinionManager.GetMinions(1000).OrderBy(x => x.LSDistance(center.Position)).FirstOrDefault()
                : null;
        }

        private static void TryIgnote(Obj_AI_Base target)
        {
            var ignote = player.LSGetSpellSlot("summonerdot");
            if (player.Spellbook.CanUseSpell(ignote) == SpellState.Ready)
            {
                if (target.LSDistance(player.ServerPosition) <= 600)
                {
                    if (cc <= menuslide("userq") && q.LSIsReady() && menubool("useignote"))
                    {
                        if (ComboDamage(target) >= target.Health &&
                            target.Health / target.MaxHealth * 100 > menuslide("overk") || 
                            menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (r.LSIsReady() && uo)
                            {
                                player.Spellbook.CastSpell(ignote, target);
                            }
                        }
                    }
                }
            }
        }

        private static void useinventoryitems(Obj_AI_Base target)
        {
            if (Items.HasItem(3142) && Items.CanUseItem(3142))
                Items.UseItem(3142);

            if (target.LSDistance(player.ServerPosition, true) <= 450 * 450)
            {
                if (Items.HasItem(3144) && Items.CanUseItem(3144))
                    Items.UseItem(3144, target);
                if (Items.HasItem(3153) && Items.CanUseItem(3153))
                    Items.UseItem(3153, target);
            }
        }

        private static readonly string[] minionlist =
        {
            // summoners rift
            "SRU_Razorbeak", "SRU_Krug", "Sru_Crab", "SRU_Baron", "SRU_Dragon",
            "SRU_Blue", "SRU_Red", "SRU_Murkwolf", "SRU_Gromp", 
            
            // twisted treeline
            "TT_NGolem5", "TT_NGolem2", "TT_NWolf6", "TT_NWolf3",
            "TT_NWraith1", "TT_Spider"
        };

        #endregion

        public KurisuRiven()
        {
            if (player.ChampionName != "Riven")
            {
                return;
            }

            w = new Spell(SpellSlot.W, 250f);
            e = new Spell(SpellSlot.E, 270f);

            q = new Spell(SpellSlot.Q, 260f);
            q.SetSkillshot(0.25f, 100f, 2200f, false, SkillshotType.SkillshotCircle);

            r = new Spell(SpellSlot.R, 900f);  
            r.SetSkillshot(0.25f, 90f, 1600f, false, SkillshotType.SkillshotCircle);

            flash = player.LSGetSpellSlot("summonerflash");
            OnDoCast();

            OnPlayAnimation();
            Interrupter();
            OnGapcloser();
            OnCast();
            Drawings();
            OnMenuLoad();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Chat.Print("<b><font color=\"#66FF33\">Kurisu's Riven</font></b> - Loaded!");
            TargetSelector.CustomTS = true;
        }

        private static AIHeroClient _sh;
        static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (ulong) WindowsMessages.WM_LBUTTONDOWN)
            {
                _sh = HeroManager.Enemies
                     .FindAll(hero => hero.LSIsValidTarget() && hero.LSDistance(Game.CursorPos, true) < 40000) // 200 * 200
                     .OrderBy(h => h.LSDistance(Game.CursorPos, true)).FirstOrDefault();
            }
        }

        private static AIHeroClient riventarget()
        {
            var cursortarg = HeroManager.Enemies
                .Where(x => x.LSDistance(Game.CursorPos) <= 1400 && x.LSDistance(player.ServerPosition) <= 1400)
                .OrderBy(x => x.LSDistance(Game.CursorPos)).FirstOrDefault(x => x.LSIsValidTarget());

            var closetarg = HeroManager.Enemies
                .Where(x => x.LSDistance(player.ServerPosition) <= e.Range + 100)
                .OrderBy(x => x.LSDistance(player.ServerPosition)).FirstOrDefault(x => x.LSIsValidTarget());

            return _sh ?? cursortarg ?? closetarg;
        }

        private static bool wrektAny()
        {
            return menu.SubMenu("combo").SubMenu("rivenw").SubMenu("req").Items.Any(i => i.GetValue<bool>()) &&
                 player.LSGetEnemiesInRange(1250).Any(ez => menu.Item("w" + ez.ChampionName).GetValue<bool>());
        }

        private static bool rrektAny()
        {
            return menu.SubMenu("combo").SubMenu("rivenr2").SubMenu("req2").Items.Any(i => i.GetValue<bool>()) &&
                 player.LSGetEnemiesInRange(1250).Any(ez => menu.Item("r" + ez.ChampionName).GetValue<bool>());
        }

        #region Riven: OnDoCast
        private static void OnDoCast()
        {
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.LSIsAutoAttack())
                {
                    if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (riventarget().LSIsValidTarget() && !riventarget().IsZombie && 
                           !riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (shy() && uo && !canhd)
                            {
                                if (riventarget().HasBuffOfType(BuffType.Stun))
                                    r.Cast(riventarget().ServerPosition);

                                if (!riventarget().HasBuffOfType(BuffType.Stun))
                                    r.CastIfHitchanceEquals(riventarget(), HitChance.Medium);
                            }
                        }
                    }

                    if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (riventarget().LSIsValidTarget() && !riventarget().IsZombie &&
                           !riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }
                    }

                    if (menu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (riventarget().LSIsValidTarget(e.Range + 200))
                        {
                            if (player.Health / player.MaxHealth * 100 <= menuslide("vhealth"))
                            {
                                if (menubool("usecomboe") && cane)
                                {
                                    if (!riventarget().IsMelee)
                                    {
                                        e.Cast(riventarget().ServerPosition);
                                    }
                                    else
                                    {
                                        e.Cast(Game.CursorPos);
                                    }
                                }
                            }
                        }
                    }

                    if (menu.Item("combokey").GetValue<KeyBind>().Active)
                    {
                        if (Utils.GameTimeTickCount - lasthd < 1600)
                        {
                            if (w.LSIsReady() && riventarget().LSDistance(player.ServerPosition) <= w.Range + 25)
                            {
                                w.Cast();
                            }
                        }

                        if (qtarg != null && riventarget() != null)
                        {
                            if (qtarg.NetworkId == riventarget().NetworkId)
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

                    else if (menu.Item("clearkey").GetValue<KeyBind>().Active)
                    {
                        if (!player.LSUnderTurret(true) || !HeroManager.Enemies.Any(x => x.LSIsValidTarget(1400)))
                        {
                            if (Utils.GameTimeTickCount - lasthd < 1600 && args.Target is Obj_AI_Minion)
                            {
                                if (w.LSIsReady() && args.Target.Position.LSDistance(player.ServerPosition) <= w.Range + 25)
                                {
                                    w.Cast();
                                }
                            }

                            if (qtarg.IsValid<Obj_AI_Minion>() && !qtarg.Name.StartsWith("Minion"))
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

                if (sender.IsMe && args.SData.LSIsAutoAttack())
                {
                    didaa = false;
                    canmv = true;
                    canaa = true;
                    canq = true;
                    cane = true;
                    canw = true;
                }
            };
        }

        #endregion

        #region Riven: OnUpdate

        private static bool isteamfightkappa;
        private static void Game_OnUpdate(EventArgs args)
        {
            // harass active
            didhs = menu.Item("harasskey").GetValue<KeyBind>().Active;

            // ulti check
            uo = player.GetSpell(SpellSlot.R).Name != "RivenFengShuiEngine";

            // hydra check
            hashd = Items.HasItem(3077) || Items.HasItem(3074) || Items.HasItem(3748);
            canhd = Items.CanUseItem(3077) || Items.CanUseItem(3074) || Items.CanUseItem(3748);

            // my radius
            truerange = player.AttackRange + player.LSDistance(player.BBox.Minimum) + 1;

            // if no valid target cancel to cursor pos
            if (!qtarg.LSIsValidTarget(truerange + 100))
                 qtarg = player;

            if (!riventarget().LSIsValidTarget())
                _sh = null;

            if (!canmv && didq)
            {
                if (orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None ||
                    menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (Player.IssueOrder(GameObjectOrder.MoveTo, movepos))
                    {
                        didq = false;
                        LeagueSharp.Common.Utility.DelayAction.Add(40, () =>
                        {
                            canmv = true;
                            canaa = true;
                        });
                    }
                }

                else if (qtarg.LSIsValidTarget(q.Range) && menubool("semiq"))
                {
                    if (Player.IssueOrder(GameObjectOrder.MoveTo, movepos))
                    {
                        didq = false;
                        LeagueSharp.Common.Utility.DelayAction.Add(40, () =>
                        {
                            canmv = true;
                            canaa = true;
                        });
                    }
                }
            }

            // move target position
            if (qtarg != player && qtarg.LSDistance(player.ServerPosition) < r.Range)
                movepos = player.Position.LSExtend(Game.CursorPos, player.LSDistance(Game.CursorPos) + 500);

            // move to game cursor pos
            if (qtarg == player)
                movepos = player.ServerPosition + (Game.CursorPos - player.ServerPosition).LSNormalized() * 125;
          
            SemiQ();
            AuraUpdate();
            CombatCore();

            orbwalker.SetAttack(canmv);
            orbwalker.SetMovement(canmv);

            if (riventarget().LSIsValidTarget())
            {
                if (menu.Item("combokey").GetValue<KeyBind>().Active)
                {
                    ComboTarget(riventarget());
                    TryIgnote(riventarget());
                }
            }

            if (menu.Item("shycombo").GetValue<KeyBind>().Active)
            {
                OrbTo(riventarget(), 350);

                if (riventarget().LSIsValidTarget())
                {
                    SomeDash(riventarget());

                    if (w.LSIsReady() && riventarget().LSDistance(player.ServerPosition) <= w.Range + 50)
                    {
                        checkr();
                        w.Cast();
                    }

                    else if (q.LSIsReady() && riventarget().LSDistance(player.ServerPosition) <= truerange + 100)
                    {
                        //if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                        //{
                        //    DoOneQ(riventarget().ServerPosition);
                        //}
                        
                        checkr();
                        TryIgnote(riventarget());

                        if (canq && !canhd && Utils.GameTimeTickCount - lasthd >= 300)
                        {
                            if (Utils.GameTimeTickCount - lastw >= 300 + Game.Ping)
                            {
                                useinventoryitems(riventarget());
                                q.Cast(riventarget().ServerPosition);
                            }
                        }
                    }
                }
            }

            if (didhs && riventarget().LSIsValidTarget())
            {
                HarassTarget(riventarget());
            }

            if (player.IsValid && menu.Item("clearkey").GetValue<KeyBind>().Active)
            {
                Clear();
                Wave();
            }

            if (player.IsValid && menu.Item("fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
            }

            WindSlashExecute();
            Windslash();

            isteamfightkappa = player.LSCountAlliesInRange(1500) > 1 && player.LSCountEnemiesInRange(1350) > 2 ||
                               player.LSCountEnemiesInRange(1200) > 2;
        }

        #endregion

        #region Riven: Menu
        private static void OnMenuLoad()
        {
            menu = new Menu("Kurisu's Riven", "kurisuriven", true);

            var orbwalkah = new Menu("Orbwalk", "rorb");
            orbwalker = new Orbwalking.Orbwalker(orbwalkah);
            menu.AddSubMenu(orbwalkah);

            var keybinds = new Menu("Keybinds", "keybinds");
            keybinds.AddItem(new MenuItem("combokey", "Combo")).SetValue(new KeyBind(32, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("harasskey", "Harass")).SetValue(new KeyBind(67, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("clearkey", "Jungle/Laneclear")).SetValue(new KeyBind(86, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("fleekey", "Flee")).SetValue(new KeyBind(65, KeyBindType.Press));
            keybinds.AddItem(new MenuItem("shycombo", "Burst Combo")).SetValue(new KeyBind('T', KeyBindType.Press));
            keybinds.AddItem(new MenuItem("semiq", "Auto Q Harass/Jungle")).SetValue(true);
            menu.AddSubMenu(keybinds);

            var drMenu = new Menu("Drawings", "drawings");
            drMenu.AddItem(new MenuItem("linewidth", "Line Width")).SetValue(new Slider(1, 1, 6));
            drMenu.AddItem(new MenuItem("drawengage", "Draw Engage Range")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            drMenu.AddItem(new MenuItem("drawr2", "Draw R2 Range")).SetValue(new Circle(true, Color.FromArgb(150, Color.White)));
            drMenu.AddItem(new MenuItem("drawburst", "Draw Burst Range")).SetValue(new Circle(true, Color.FromArgb(150, Color.LawnGreen)));
            drMenu.AddItem(new MenuItem("drawf", "Draw Target")).SetValue(new Circle(true, Color.FromArgb(255, Color.GreenYellow)));
            drMenu.AddItem(new MenuItem("drawdmg", "Draw Combo Damage Fill")).SetValue(true);
            menu.AddSubMenu(drMenu);

            var combo = new Menu("Combo", "combo");

            var qmenu = new Menu("Q  Settings", "rivenq");
            qmenu.AddItem(new MenuItem("wq3", "Ward + Q3 (Flee)")).SetValue(true);
            qmenu.AddItem(new MenuItem("qint", "Interrupt with 3rd Q")).SetValue(true);
            qmenu.AddItem(new MenuItem("keepq", "Use Q Before Expiry")).SetValue(true);
            qmenu.AddItem(new MenuItem("usegap", "Gapclose with Q")).SetValue(true);
            qmenu.AddItem(new MenuItem("gaptimez", "Gapclose Q Delay (ms)")).SetValue(new Slider(115, 0, 200));
            qmenu.AddItem(new MenuItem("safeq", "Block Q into multiple Enemies")).SetValue(false);
            combo.AddSubMenu(qmenu);

            var wmenu = new Menu("W Settings", "rivenw");
            var newmenu = new Menu("Required Targets", "req").SetFontStyle(FontStyle.Regular, SharpDX.Color.LawnGreen);
            foreach (var hero in HeroManager.Enemies)
                newmenu.AddItem(new MenuItem("w" + hero.ChampionName, hero.ChampionName))
                    .SetValue(false).SetTooltip("Only W if it will hit " + hero.ChampionName).DontSave();
            wmenu.AddSubMenu(newmenu);

            wmenu.AddItem(new MenuItem("usecombow", "Use W in Combo")).SetValue(true);
            wmenu.AddItem(new MenuItem("wint", "Use on Interrupt")).SetValue(true);
            wmenu.AddItem(new MenuItem("wgap", "Use on Gapcloser")).SetValue(true);
            combo.AddSubMenu(wmenu);

            var emenu = new Menu("E  Settings", "rivene");

            emenu.AddItem(new MenuItem("usecomboe", "Use E in Combo")).SetValue(true);
            emenu.AddItem(new MenuItem("vhealth", "Use E if HP% <=")).SetValue(new Slider(60));
            emenu.AddItem(new MenuItem("safee", "Block E into multiple Enemies")).SetValue(true);
            combo.AddSubMenu(emenu);

            var rmenu = new Menu("R1 Settings", "rivenr");
            rmenu.AddItem(new MenuItem("useignote", "Combo with Ignite")).SetValue(true);
            rmenu.AddItem(new MenuItem("user", "Use R1 in Combo")).SetValue(new KeyBind('H', KeyBindType.Toggle, true)).Permashow();
            rmenu.AddItem(new MenuItem("ultwhen", "Use R1 when")).SetValue(new StringList(new[] { "Normal Kill", "Hard Kill", "Always" }, 2));
            rmenu.AddItem(new MenuItem("overk", "Dont R1 if target HP % <=")).SetValue(new Slider(25, 1, 99));
            rmenu.AddItem(new MenuItem("userq", "Use only if Q Count <=")).SetValue(new Slider(2, 1, 3));
            rmenu.AddItem(new MenuItem("multib", "Burst when")).SetValue(new StringList(new[] { "Damage Check", "Always" }, 1));
            rmenu.AddItem(new MenuItem("flashb", "-> Flash in Burst")).SetValue(true).SetFontStyle(FontStyle.Regular, SharpDX.Color.LawnGreen);
            combo.AddSubMenu(rmenu);

            var r2menu = new Menu("R2 Settings", "rivenr2");
            var newmenu2 = new Menu("Required Targets", "req2").SetFontStyle(FontStyle.Regular, SharpDX.Color.LawnGreen);
            foreach (var hero in HeroManager.Enemies)
                newmenu2.AddItem(new MenuItem("r" + hero.ChampionName, hero.ChampionName))
                    .SetValue(false).SetTooltip("Only R2 if it will hit " + hero.ChampionName).DontSave();
            r2menu.AddSubMenu(newmenu2);

            r2menu.AddItem(new MenuItem("usews", "Use R2 in Combo")).SetValue(true);
            r2menu.AddItem(new MenuItem("rhitc", "-> Hitchance"))
                .SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 2));
            r2menu.AddItem(new MenuItem("saver", "Save R2 (When in AA Range)")).SetValue(false);
            r2menu.AddItem(new MenuItem("overaa", "Save R2 if target will die in AA")).SetValue(new Slider(2, 1, 6));
            r2menu.AddItem(new MenuItem("wsmode", "Use R2 when")).SetValue(new StringList(new[] { "Kill Only", "Max Damage" }, 1));
            r2menu.AddItem(new MenuItem("keepr", "Use R2 Before Expiry")).SetValue(true);
            combo.AddSubMenu(r2menu);

            menu.AddSubMenu(combo);

            var harass = new Menu("Harass", "harass");
            harass.AddItem(new MenuItem("useharassw", "Use W in Harass")).SetValue(true);
            harass.AddItem(new MenuItem("usegaph", "Use E in Harass")).SetValue(true);
            harass.AddItem(new MenuItem("qtoo", "Use Escape/Flee: "))
                .SetValue(new StringList(new[] {"Away from Target", "To Ally Turret", "To Cursor"}, 1));
            harass.AddItem(new MenuItem("useitemh", "Use Tiamat/Hydra")).SetValue(true);
            menu.AddSubMenu(harass);

            var farming = new Menu("Farming", "farming");

            var wc = new Menu("Jungle", "waveclear");
            wc.AddItem(new MenuItem("usejungleq", "Use Q in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglew", "Use W in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglee", "Use E in Jungle")).SetValue(true);
            farming.AddSubMenu(wc);

            var jg = new Menu("WaveClear", "jungle");
            jg.AddItem(new MenuItem("uselaneq", "Use Q in WaveClear")).SetValue(true);
            jg.AddItem(new MenuItem("useaoeq", "Try Q AoE WaveClear")).SetValue(false);
            jg.AddItem(new MenuItem("uselanew", "Use W in WaveClear")).SetValue(true);
            jg.AddItem(new MenuItem("wminion", "Use W Minions >=")).SetValue(new Slider(3, 1, 6));
            jg.AddItem(new MenuItem("uselanee", "Use E in WaveClear")).SetValue(true);
            farming.AddSubMenu(jg);

            menu.AddSubMenu(farming);
            menu.AddToMainMenu();
        }

        #endregion

        #region Riven : Some Dash
        private static bool canburst()
        {
            if (riventarget() == null || !r.LSIsReady())
            {
                return false;
            }

            if (IsLethal(riventarget()) && menulist("multib") == 0)
            {
                return true;
            }

            if (menu.Item("shycombo").GetValue<KeyBind>().Active)
            {
                if (shy())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool shy()
        {
            if (r.LSIsReady() && riventarget() != null && menulist("multib") != 0)
            {
                return true;
            }

            return false;
        }

        private static void doFlash()
        {
            if (riventarget() != null && (canburst() || shy()))
            {
                if (!flash.LSIsReady() || !menubool("flashb"))
                    return;

                if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (riventarget().LSDistance(player.ServerPosition) > e.Range + 50 &&
                        riventarget().LSDistance(player.ServerPosition) <= e.Range + w.Range + 275)
                    {
                        var second =
                            HeroManager.Enemies.Where(
                                x => x.NetworkId != riventarget().NetworkId &&
                                     x.LSDistance(riventarget().ServerPosition) <= r.Range)
                                .OrderByDescending(xe => xe.LSDistance(riventarget().ServerPosition))
                                .FirstOrDefault();

                        if (second != null)
                        {
                            var pos = riventarget().ServerPosition +
                                      (riventarget().ServerPosition - second.ServerPosition).LSNormalized() * 75;

                            player.Spellbook.CastSpell(flash, pos);
                        }

                        else
                        {
                            player.Spellbook.CastSpell(flash,
                                riventarget().ServerPosition.LSExtend(player.ServerPosition, 115));
                        }
                    }
                }
            }
        }

        private static void SomeDash(AIHeroClient target)
        {
            if (!menu.Item("shycombo").GetValue<KeyBind>().Active ||
                !target.IsValid<AIHeroClient>() || uo)
                return;

            if (riventarget() == null || !r.LSIsReady())
                return;

            if (flash.LSIsReady() &&  w.LSIsReady() && (canburst() || shy()) && menulist("multib") != 2)
            {
                if (e.LSIsReady() && target.LSDistance(player.ServerPosition) <= e.Range + w.Range + 275)
                {
                    if (target.LSDistance(player.ServerPosition) > e.Range + truerange + 50)
                    {
                        e.Cast(target.ServerPosition);

                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.LSIsReady() && target.LSDistance(player.ServerPosition) <= w.Range + 275)
                {
                    if (target.LSDistance(player.ServerPosition) > truerange + 50)
                    {
                        if (!uo)
                            r.Cast();
                    }
                }
            }

            else
            {
                if (e.LSIsReady() && target.LSDistance(player.ServerPosition) <= e.Range + w.Range - 25)
                {
                    if (target.LSDistance(player.ServerPosition) > truerange + 50)
                    {
                        e.Cast(target.ServerPosition);

                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.LSIsReady() && target.LSDistance(player.ServerPosition) <= w.Range - 10)
                {
                    if (!uo)
                        r.Cast();
                }
            }
        }

        #endregion

        #region Riven: Combo

        private static void ComboTarget(AIHeroClient target)
        {
            OrbTo(target);
            TryIgnote(target);

            var endq = player.Position.LSExtend(target.Position, q.Range + 35);
            var ende = player.Position.LSExtend(target.Position, e.Range + 35);

            if (target.LSDistance(player.ServerPosition) <= q.Range + 90 && q.LSIsReady())
            {
                if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (target.LSDistance(player.ServerPosition) <= q.Range + 90 && q.LSIsReady())
                    {
                        DoOneQ(target.ServerPosition);
                    }
                }
            }

            if (e.LSIsReady() && 

               (target.LSDistance(player.ServerPosition) <= e.Range + w.Range || 
                uo && target.LSDistance(player.ServerPosition) > truerange + 200) &&     
                 target.LSDistance(player.ServerPosition) > truerange + 100)
            {
                if (menubool("usecomboe") && cane)
                {
                    if (menubool("safee"))
                    {
                        if (ende.LSCountEnemiesInRange(200) <=2)
                        {
                            e.Cast(target.IsMelee ? Game.CursorPos : target.ServerPosition);
                        }
                    }

                    else
                    {
                        e.Cast(target.IsMelee ? Game.CursorPos : target.ServerPosition);
                    }
                }

                if (target.LSDistance(player.ServerPosition) <= e.Range + w.Range)
                {
                    checkr();

                    if (!canburst() && canhd && uo)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }

                if (!canburst() && canhd)
                {
                    if (Items.CanUseItem(3077))
                        Items.UseItem(3077);
                    if (Items.CanUseItem(3074))
                        Items.UseItem(3074);
                }
            }

            if (w.LSIsReady() && menubool("usecombow") && target.LSDistance(player.ServerPosition) <= w.Range)
            {
                if (target.LSDistance(player.ServerPosition) <= w.Range && Utils.GameTimeTickCount - lasthd > 1600)
                {
                    useinventoryitems(target);
                    checkr();

                    if (menubool("usecombow") && canw)
                    {
                        if (!isteamfightkappa || 
                             isteamfightkappa && !wrektAny() || 
                             menubool("w" + target.ChampionName))
                        {
                            w.Cast();
                        }
                    }
                }
            }

            var catchRange = e.LSIsReady() ? e.Range + truerange + 200 : truerange + 200;
            if (q.LSIsReady() && target.LSDistance(player.ServerPosition) <= q.Range + 100)
            {
                useinventoryitems(target);
                checkr();

                if (IsLethal(target))
                {
                    if (canhd) return;
                }

                if (menulist("wsmode") == 1 && IsLethal(target))
                {
                    if (cc == 2 && e.LSIsReady() && cane)
                    {
                        e.Cast(target.ServerPosition);
                    }
                }

                if (canq)
                {
                    if (menubool("safeq"))
                    {
                        if (endq.LSCountEnemiesInRange(200) <= 2)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }

                    else
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            else if (q.LSIsReady() && target.LSDistance(player.ServerPosition) > catchRange)
            {
                if (menubool("usegap"))
                {
                    if (Utils.GameTimeTickCount - lastq >= menuslide("gaptimez") * 10)
                    {
                        if (q.LSIsReady() && Utils.GameTimeTickCount - laste >= 600)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }
                }
            }

            else if (target.Health <= q.GetDamage(target) * 2 + player.LSGetAutoAttackDamage(target) * 2)
            {
                if (target.LSDistance(player.ServerPosition) > truerange + q.Range + 10)
                {
                    if (target.LSDistance(player.ServerPosition) <= q.Range * 2)
                    {
                        if (Utils.GameTimeTickCount - lastq >= 250)
                        {
                            q.Cast(target.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Harass

        private static void HarassTarget(Obj_AI_Base target)
        {
            if (target == null)
            {
                return;
            }

            Vector3 qpos;
            switch (menulist("qtoo"))
            {
                case 0:
                    qpos = player.ServerPosition + 
                        (player.ServerPosition - target.ServerPosition).LSNormalized()*500;
                    break;
                case 1:
                    var tt = ObjectManager.Get<Obj_AI_Turret>()
                        .Where(t => (t.IsAlly)).OrderBy(t => t.LSDistance(player.Position)).First();
                    if (tt != null)
                        qpos = tt.Position;
                    else
                        qpos = player.ServerPosition +
                                (player.ServerPosition - target.ServerPosition).LSNormalized() * 500;
                    break;
                default:
                    qpos = Game.CursorPos;
                    break;
            }

            if (q.LSIsReady())
                OrbTo(target);

            if (cc == 2 && canq && q.LSIsReady())
            {
                if (!e.LSIsReady())
                {
                    orbwalker.SetAttack(false);
                    orbwalker.SetMovement(false);

                    canaa = false;
                    canmv = false;

                    if (Player.IssueOrder(GameObjectOrder.MoveTo, qpos))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(150 - Game.Ping/2, () =>
                        {
                            q.Cast(qpos);

                            orbwalker.SetAttack(true);
                            orbwalker.SetMovement(true);

                            canaa = true;
                            canmv = true;
                        });
                    }
                }
            }

            if (e.LSIsReady() && (cc == 3 || !q.LSIsReady() && cc == 0))
            {
                if (player.LSDistance(target.ServerPosition) <= 300)
                {
                    if (menubool("usegaph") && cane)
                        e.Cast(qpos);
                }
            }

            if (!target.ServerPosition.LSUnderTurret(true))
            {
                if (q.LSIsReady() && canq && (cc < 2 || e.LSIsReady()))
                {
                    if (target.LSDistance(player.ServerPosition) <= truerange + q.Range)
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            if (e.LSIsReady() && cane && q.LSIsReady() && cc < 1 &&
                target.LSDistance(player.ServerPosition) > truerange + 100 &&
                target.LSDistance(player.ServerPosition) <= e.Range + truerange + 50)
            {
                if (!target.ServerPosition.LSUnderTurret(true))
                {
                    if (menubool("usegaph") && cane)
                    {
                        e.Cast(target.ServerPosition);
                    }
                }
            }

            else if (w.LSIsReady() && canw && target.LSDistance(player.ServerPosition) <= w.Range + 10)
            {
                if (!player.ServerPosition.LSUnderTurret(true))
                {
                    if (menubool("useharassw") && canw)
                    {
                        w.Cast();
                    }
                }
            }
        }

        #endregion
         
        #region Riven: Windslash

        private static void Windslash()
        {
            if (uo && menubool("usews") && r.LSIsReady())
            {
                if (menu.Item("shycombo").GetValue<KeyBind>().Active && canburst())
                {
                    if (riventarget().LSDistance(player.ServerPosition) <= player.AttackRange + 100)
                    {
                        if (canhd) return;
                    }
                }

                #region MaxDmage

                if (menulist("wsmode") == 1)
                {
                    if (riventarget().LSIsValidTarget(r.Range) && !riventarget().IsZombie)
                    {
                        if (Rdmg(riventarget()) / riventarget().MaxHealth * 100 >= 50)
                        {
                            var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                            if (p.Hitchance >= HitChance.Medium && canws && !riventarget().HasBuff("kindredrnodeathbuff"))
                            {
                                if (!isteamfightkappa || menubool("r" + riventarget().ChampionName) || isteamfightkappa && !rrektAny())
                                {
                                    r.Cast(p.CastPosition);
                                }
                            }
                        }

                        if (q.LSIsReady() && cc <= 2)
                        {
                            var aadmg = player.LSGetAutoAttackDamage(riventarget(), true) * 2;
                            var currentrdmg = Rdmg(riventarget());
                            var qdmg = Qdmg(riventarget()) * 2;

                            var damage = aadmg + currentrdmg + qdmg;

                            if (riventarget().Health <= xtra((float) damage))
                            {
                                if (riventarget().LSDistance(player.ServerPosition) <= truerange + q.Range)
                                {
                                    var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                                    if (!riventarget().HasBuff("kindredrnodeathbuff"))
                                    {
                                        if (p.Hitchance == HitChance.High && canws || r.CastIfWillHit(riventarget(), 3) && canws)
                                        {
                                            if (!isteamfightkappa || menubool("r" + riventarget().ChampionName) || isteamfightkappa && !rrektAny())
                                            {
                                                r.Cast(p.CastPosition);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }             
                }

                #endregion           
            }        
        }

        private static void WindSlashExecute()
        {
            if (uo && menubool("usews") && r.LSIsReady())
            {
                #region Killsteal
                foreach (var t in ObjectManager.Get<AIHeroClient>().Where(h => h.LSIsValidTarget(r.Range)))
                {
                    if (menubool("saver"))
                    {
                        if (player.LSGetAutoAttackDamage(t, true) *  2 + menuslide("overaa") >= t.Health)
                        {
                            if (player.HealthPercent > 70 && player.LSCountEnemiesInRange(q.Range) <= 2)
                            {
                                continue;
                            }
                        }
                    }

                    if (Rdmg(t) >= t.Health)
                    {
                        var p = r.GetPrediction(t, true, -1f, new[] { CollisionableObjects.YasuoWall });
                        if (p.Hitchance == (HitChance) menulist("rhitc") + 4 && canws && !t.HasBuff("kindredrnodeathbuff"))
                        {
                            r.Cast(p.CastPosition);
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        #region Riven: Lane/Jungle

        private static void Clear()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f,
                MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            foreach (var unit in minions.Where(m => !m.Name.Contains("Mini")))
            {
                OrbTo(unit);

                if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (unit.LSDistance(player.ServerPosition) <= q.Range + 90 && q.LSIsReady())
                    {
                        DoOneQ(unit.ServerPosition);
                    }
                }

                if (Utils.GameTimeTickCount - laste < 600)
                {
                    if (unit.LSDistance(player.ServerPosition) <= w.Range + 45)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }

                if (e.LSIsReady() && cane && menubool("usejunglee"))
                {
                    if (player.Health / player.MaxHealth * 100 <= 70 ||
                        unit.LSDistance(player.ServerPosition) > truerange + 30)
                    {
                        e.Cast(unit.ServerPosition);
                    }
                }

                if (w.LSIsReady() && canw && menubool("usejunglew") && Utils.GameTimeTickCount - lasthd > 1600)
                {
                    if (unit.LSDistance(player.ServerPosition) <= w.Range + 25)
                    {
                        w.Cast();
                    }
                }

                if (q.LSIsReady() && canq && menubool("usejungleq"))
                {
                    if (unit.LSDistance(player.ServerPosition) <= q.Range + 90)
                    {
                        if (canhd) return;
          
                        if (qtarg != null && qtarg.NetworkId == unit.NetworkId)
                            q.Cast(unit.ServerPosition);
                    }
                }
            }
        }

        private static void Wave()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f);

            foreach (var unit in minions.Where(x => x.IsMinion))
            {
                OrbTo(menubool("useaoeq") && GetCenterMinion().LSIsValidTarget() 
                    ? GetCenterMinion() 
                    : unit);

                if (q.LSIsReady() && unit.LSDistance(player.ServerPosition) <= truerange + 100)
                {
                    if (canq && menubool("uselaneq") && minions.Count >= 2 &&
                        (!player.ServerPosition.LSExtend(unit.ServerPosition, q.Range).LSUnderTurret(true) ||
                        !HeroManager.Enemies.Any(x => x.LSIsValidTarget(1400))))
                    {
                        if (GetCenterMinion().LSIsValidTarget() && menubool("useaoeq"))
                            q.Cast(GetCenterMinion());
                        else
                            q.Cast(unit.ServerPosition);
                    }
                }

                if (w.LSIsReady())
                {
                    if (minions.Count(m => m.LSDistance(player.ServerPosition) <= w.Range + 10) >= menuslide("wminion"))
                    {
                        if (canw && menubool("uselanew"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);

                            w.Cast();
                        }
                    }
                }

                if (e.LSIsReady() && !player.ServerPosition.LSExtend(unit.ServerPosition, e.Range).LSUnderTurret(true))
                {
                    if (unit.LSDistance(player.ServerPosition) > truerange + 30)
                    {
                        if (cane && menubool("uselanee"))
                        {
                            if (GetCenterMinion().LSIsValidTarget() && menubool("useaoeq"))
                                e.Cast(GetCenterMinion());
                            else
                                e.Cast(unit.ServerPosition);
                        }
                    }

                    else if (player.Health / player.MaxHealth * 100 <= 70)
                    {
                        if (cane && menubool("uselanee"))
                        {
                            if (GetCenterMinion().LSIsValidTarget() && menubool("useaoeq"))
                                q.Cast(GetCenterMinion());
                            else
                                q.Cast(unit.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Flee

        private static void Flee()
        {
            if (canmv)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (cc > 2 && Items.GetWardSlot() != null && menubool("wq3"))
            {
                var attacker = HeroManager.Enemies.FirstOrDefault(x => x.LSDistance(player.ServerPosition) <= q.Range + 50);
                if (attacker.LSIsValidTarget(q.Range))
                {
                    if (Utils.GameTimeTickCount - lastwd >= 1000 && didq)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(100,
                            () => Items.UseItem((int) Items.GetWardSlot().Id, attacker.ServerPosition));
                    }
                }
            }

            if (player.LSCountEnemiesInRange(w.Range) > 0)
            {
                if (w.LSIsReady())
                    w.Cast();
            }

            if (ssfl)
            {
                if (Utils.GameTimeTickCount - lastq >= 600)
                {
                    q.Cast(Game.CursorPos);
                }

                if (cane && e.LSIsReady())
                {
                    if (cc >= 2 || !q.LSIsReady() && !player.LSHasBuff("RivenTriCleave", true))
                    {
                        if (!player.ServerPosition.LSExtend(Game.CursorPos, e.Range + 10).LSIsWall())
                            e.Cast(Game.CursorPos);
                    }
                }
            }

            else
            {
                if (q.LSIsReady())
                {
                    q.Cast(Game.CursorPos);
                }

                if (e.LSIsReady() && Utils.GameTimeTickCount - lastq >= 250)
                {
                    if (!player.ServerPosition.LSExtend(Game.CursorPos, e.Range).LSIsWall())
                        e.Cast(Game.CursorPos);
                }
            }
        }

        #endregion

        #region Riven: Semi Q 

        private static void SemiQ()
        {
            if (canq && Utils.GameTimeTickCount - lastaa >= 150)
            {
                if (menubool("semiq"))
                {
                    if (q.LSIsReady() && Utils.GameTimeTickCount - lastaa < 1200 && qtarg != null)
                    {
                        if (qtarg.LSIsValidTarget(q.Range + 100) &&
                            !menu.Item("clearkey").GetValue<KeyBind>().Active &&
                            !menu.Item("harasskey").GetValue<KeyBind>().Active &&
                            !menu.Item("combokey").GetValue<KeyBind>().Active &&
                            !menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (qtarg.IsValid<AIHeroClient>() && !qtarg.LSUnderTurret(true))
                                q.Cast(qtarg.ServerPosition);
                        }

                        if (!menu.Item("harasskey").GetValue<KeyBind>().Active &&
                            !menu.Item("clearkey").GetValue<KeyBind>().Active &&
                            !menu.Item("combokey").GetValue<KeyBind>().Active &&
                            !menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (qtarg.LSIsValidTarget(q.Range + 100) && !qtarg.Name.Contains("Mini"))
                            {
                                if (!qtarg.Name.StartsWith("Minion") && minionlist.Any(name => qtarg.Name.StartsWith(name)))
                                {
                                    q.Cast(qtarg.ServerPosition);
                                }
                            }

                            if (qtarg.LSIsValidTarget(q.Range + 100))
                            {
                                if (qtarg.IsValid<AIHeroClient>() || qtarg.IsValid<Obj_AI_Turret>())
                                {
                                    if (uo)
                                        q.Cast(qtarg.ServerPosition);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Check R
        private static void checkr()
        {
            if (!r.LSIsReady() || uo || !menu.Item("user").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (menu.Item("shycombo").GetValue<KeyBind>().Active)
            {
                r.Cast();
                return;
            }

            var targets = HeroManager.Enemies.Where(ene => ene.LSIsValidTarget(r.Range));
            var heroes = targets as IList<AIHeroClient> ?? targets.ToList();

            foreach (var target in heroes)
            {
                if (cc > menuslide("userq"))
                {
                    return;
                }

                if (target.Health / target.MaxHealth * 100 <= menuslide("overk") && IsLethal(target))
                {
                    if (heroes.Count() < 2)
                    {
                        continue;
                    }
                }

                if (menulist("ultwhen") == 2)
                    r.Cast();

                if (q.LSIsReady() || Utils.GameTimeTickCount - lastq < 1000 && cc < 3)
                {
                    if (heroes.Count() < 2)
                    {
                        if (target.Health / target.MaxHealth * 100 <= menuslide("overk") && IsLethal(target))
                            return;
                    }

                    if (heroes.Count(ene => ene.LSDistance(player.ServerPosition) <= 750) > 1)
                        r.Cast();

                    if (menulist("ultwhen") == 0)
                    {
                        if ((ComboDamage(target)/1.3) >= target.Health && target.Health >= (ComboDamage(target)/1.8))
                        {
                            r.Cast();
                        }
                    }

                    if (menulist("ultwhen") == 1)
                    {
                        if (ComboDamage(target) >= target.Health && target.Health >= ComboDamage(target)/1.8)
                        {
                            r.Cast();
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: On Cast
        private static void OnCast()
        {
            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (!sender.IsMe)
                {
                    return;
                }

                if (args.SData.LSIsAutoAttack())
                {
                    qtarg = (Obj_AI_Base) args.Target;
                    lastaa = Utils.GameTimeTickCount;
                }

                if (!didq && args.SData.LSIsAutoAttack())
                {
                    var targ = (AttackableUnit) args.Target;
                    if (targ != null && player.LSDistance(targ.Position) <= q.Range + 120)
                    {
                        didaa = true;
                        canaa = false;
                        canq = false;
                        canw = false;
                        cane = false;
                        canws = false;
                        // canmv = false;
                    }
                }

                if (args.SData.Name.ToLower().Contains("ward"))
                    lastwd = Utils.GameTimeTickCount;

                switch (args.SData.Name)
                {
                    case "ItemTiamatCleave":
                        lasthd = Utils.GameTimeTickCount;
                        didhd = true;
                        canhd = false;

                        if (menulist("wsmode") == 1 || menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (menu.Item("combokey").GetValue<KeyBind>().Active || 
                                menu.Item("shycombo").GetValue<KeyBind>().Active)
                            {
                                if (canburst() && uo)
                                {
                                    if (riventarget().LSIsValidTarget() && !riventarget().IsZombie && !riventarget().HasBuff("kindredrnodeathbuff"))
                                    {
                                        if (!isteamfightkappa || menubool("r" + riventarget().ChampionName) ||
                                             isteamfightkappa && !rrektAny() || menu.Item("shycombo").GetValue<KeyBind>().Active)
                                        {
                                            LeagueSharp.Common.Utility.DelayAction.Add(140 - Game.Ping/2,
                                                () =>
                                                {
                                                    if (riventarget().HasBuffOfType(BuffType.Stun))
                                                        r.Cast(riventarget().ServerPosition);
                                                    else
                                                        r.Cast(r.CastIfHitchanceEquals(riventarget(), HitChance.Medium));
                                                });
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "RivenTriCleave":
                        canq = false;
                        cc += 1;
                        didq = true;
                        didaa = false;
                        lastq = Utils.GameTimeTickCount;
                        canmv = false;  
               
                        var dd = new[] {280 - Game.Ping, 290 - Game.Ping, 380 - Game.Ping};

                        LeagueSharp.Common.Utility.DelayAction.Add(dd[Math.Max(cc, 1) - 1], () =>
                        {
                            if (orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None ||
                                menu.Item("shycombo").GetValue<KeyBind>().Active)
                                Chat.Say("/d");

                            else if (qtarg.LSIsValidTarget(450) && menubool("semiq"))
                                Chat.Say("/d");
                        });

                        if (!uo) ssfl = false;
                        break;
                    case "RivenMartyr":
                        canq = false;
                        canmv = false;
                        didw = true;
                        lastw = Utils.GameTimeTickCount;
                        canw = false;

                        break;
                    case "RivenFeint":
                        canmv = false;
                        dide = true;
                        didaa = false;
                        laste = Utils.GameTimeTickCount;
                        cane = false;

                        if (menu.Item("fleekey").GetValue<KeyBind>().Active)
                        {
                            if (uo && r.LSIsReady() && cc == 2 && q.LSIsReady())
                            {
                                var btarg = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Physical);
                                if (btarg.LSIsValidTarget())
                                    r.CastIfHitchanceEquals(btarg, HitChance.Medium);
                                else
                                    r.Cast(Game.CursorPos);
                            }
                        }

                        if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (cc == 2 && !uo && r.LSIsReady() && riventarget() != null)
                            {
                                checkr();
                                LeagueSharp.Common.Utility.DelayAction.Add(240 - Game.Ping, () => q.Cast(riventarget().ServerPosition));
                            }
                        }

                        if (menu.Item("combokey").GetValue<KeyBind>().Active)
                        {
                            if (cc == 2 && !uo && r.LSIsReady() && riventarget() != null)
                            {
                                checkr();
                                LeagueSharp.Common.Utility.DelayAction.Add(240 - Game.Ping, () => q.Cast(riventarget().ServerPosition));
                            }

                            if (menulist("wsmode") == 1 && cc == 2 && uo)
                            {
                                if (riventarget().LSIsValidTarget(r.Range + 100) && IsLethal(riventarget()))
                                {
                                    LeagueSharp.Common.Utility.DelayAction.Add(100 - Game.Ping,
                                    () => r.Cast(r.CastIfHitchanceEquals(riventarget(), HitChance.Medium)));
                                }
                            }
                        }

                        break;
                    case "RivenFengShuiEngine":
                        ssfl = true;
                        doFlash();
                        break;
                    case "RivenIzunaBlade":
                        ssfl = false;
                        didws = true;
                        canws = false;

                        if (w.LSIsReady() && riventarget().LSIsValidTarget(w.Range + 55))
                            w.Cast();

                        else if (q.LSIsReady() && riventarget().LSIsValidTarget())
                            q.Cast(riventarget().ServerPosition);

                        break;
                }
            };
        }

        #endregion

        #region Riven: Misc 

        private static void DoOneQ(Vector3 pos)
        {
            canq = false;

            if (q.LSIsReady() && Utils.GameTimeTickCount - lastq > 5000)
            {
                if (q.Cast(pos))
                {
                    lastq = Utils.GameTimeTickCount;
                    didq = true;
                    canq = false;
                }
            }
        }

        private static void Interrupter()
        {
            Interrupter2.OnInterruptableTarget += (sender, args) =>
            {
                if (menubool("wint") && w.LSIsReady())
                {
                    if (!sender.Position.LSUnderTurret(true))
                    {
                        if (sender.LSIsValidTarget(w.Range))
                            w.Cast();

                        if (sender.LSIsValidTarget(w.Range + e.Range) && e.LSIsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }

                if (menubool("qint") && q.LSIsReady() && cc >= 2)
                {
                    if (!sender.Position.LSUnderTurret(true))
                    {
                        if (sender.LSIsValidTarget(q.Range))
                            q.Cast(sender.ServerPosition);

                        if (sender.LSIsValidTarget(q.Range + e.Range) && e.LSIsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }
            };
        }

        private static void OnGapcloser()
        {
            AntiGapcloser.OnEnemyGapcloser += gapcloser =>
            {
                if (menubool("wgap") && w.LSIsReady())
                {
                    if (gapcloser.Sender.LSIsValidTarget(w.Range))
                    {
                        if (!gapcloser.Sender.ServerPosition.LSUnderTurret(true))
                        {
                            if (!isteamfightkappa || menubool("w" + gapcloser.Sender.ChampionName) || isteamfightkappa && !wrektAny())
                            {
                                w.Cast();
                            }
                        }
                    }
                }           
            };
        }

        private void OnPlayAnimation()
        {
          
        }

        #endregion

        #region Riven: Aura

        private static void AuraUpdate()
        {
            if (!player.IsDead)
            {
                foreach (var buff in player.Buffs)
                {
                    //if (buff.Name == "RivenTriCleave")
                    //    cc = buff.Count;

                    if (buff.Name == "rivenpassiveaaboost")
                        pc = buff.Count;
                }

                if (player.LSHasBuff("RivenTriCleave", true) && !menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (player.GetBuff("RivenTriCleave").EndTime - Game.Time <= 0.25f)
                    {
                        if (!player.LSIsRecalling() && !player.Spellbook.IsChanneling)
                        {
                            var qext = player.ServerPosition.LSTo2D() + 
                                       player.Direction.LSTo2D().LSPerpendicular() * q.Range + 100;

                            if (menubool("keepq"))
                            {
                                if (qext.To3D().LSCountEnemiesInRange(200) <= 1 && !qext.To3D().LSUnderTurret(true))
                                {
                                    q.Cast(Game.CursorPos);
                                }
                            }
                        }
                    }
                }

                if (r.LSIsReady() && uo && menubool("keepr"))
                {
                    if (player.GetBuff("RivenFengShuiEngine").EndTime - Game.Time <= 0.25f)
                    {
                        if (!riventarget().LSIsValidTarget(r.Range) || riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (e.LSIsReady() && uo)
                                e.Cast(Game.CursorPos);

                            r.Cast(Game.CursorPos);
                        }

                        if (riventarget().LSIsValidTarget(r.Range) && !riventarget().HasBuff("kindredrnodeathbuff"))
                            r.CastIfHitchanceEquals(riventarget(), HitChance.High);
                    }
                }

                if (!player.LSHasBuff("rivenpassiveaaboost", true))
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => pc = 1);

                if (cc > 2)
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => cc = 0);
            }
        }

        #endregion

        #region Riven : Combat/Orbwalk

        private static void OrbTo(Obj_AI_Base target, float rangeoverride = 0f)
        {
            if (canmv)
            {
                if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (target.LSIsValidTarget(truerange + 100))
                        Orbwalking.Orbwalk(target, Game.CursorPos, 80f, 0f, false, false);

                    else
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (canmv && canaa)
            {
                if (q.LSIsReady() || Utils.GameTimeTickCount - lastq <= 400 - Game.Ping && cc < 3)
                {
                    if (target.LSIsValidTarget(truerange + 200 + rangeoverride))
                    {
                        Orbwalking.LastAATick = 0;
                    }
                }
            }
        }

        private static void CombatCore()
        {
            if (didaa && Utils.GameTimeTickCount - lastaa >= 
                100 - Game.Ping / 2 + 55 + player.AttackCastDelay * 1000)
                didaa = false;

            if (didhd && canhd && Utils.GameTimeTickCount - lasthd >= 250)
                didhd = false;

            if (didq && Utils.GameTimeTickCount - lastq >= 500)
                didq = false;

            if (didw && Utils.GameTimeTickCount - lastw >= 266)
            {
                didw = false;
                canmv = true;
                canaa = true;
            }

            if (dide && Utils.GameTimeTickCount - laste >= 350)
            {
                dide = false;
                canmv = true;
                canaa = true;
            }

            if (didws && Utils.GameTimeTickCount - laste >= 366)
            {
                didws = false;
                canmv = true;
                canaa = true;
            }

            if (!canw && w.LSIsReady() && !(didaa || didq || dide))
                 canw = true;

            if (!cane && e.LSIsReady() && !(didaa || didq || didw))
                 cane = true;

            if (!canws && r.LSIsReady() && !didaa && uo)
                 canws = true;

            if (!canaa && !(didq || didw || dide || didws || didhd || didhs) && 
                Utils.GameTimeTickCount - lastaa >= 1000)
                canaa = true;

            if (!canmv && !(didq || didw || dide || didws || didhd || didhs) &&
                Utils.GameTimeTickCount - lastaa >= 1100)
                canmv = true;
        }

        #endregion

        #region Riven: Math/Damage

        private static float ComboDamage(Obj_AI_Base target, bool checkq = false)
        {
            if (target == null)
                return 0f;

            var ignote = player.LSGetSpellSlot("summonerdot");
            var ad = (float)player.LSGetAutoAttackDamage(target);
            var runicpassive = new[] { 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };

            var ra = ad +
                        (float)
                            ((+player.FlatPhysicalDamageMod + player.BaseAttackDamage) *
                            runicpassive[Math.Min(player.Level, 18) / 3]);

            var rw = Wdmg(target);
            var rq = Qdmg(target);
            var rr = r.LSIsReady() ? Rdmg(target) : 0;

            var ii = (ignote != SpellSlot.Unknown && player.GetSpell(ignote).State == SpellState.Ready && r.LSIsReady()
                ? player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite)
                : 0);

            var tmt = Items.HasItem(3077) && Items.CanUseItem(3077)
                ? player.GetItemDamage(target, Damage.DamageItems.Tiamat)
                : 0;

            var hyd = Items.HasItem(3074) && Items.CanUseItem(3074)
                ? player.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var tdh = Items.HasItem(3748) && Items.CanUseItem(3748)
                ? player.GetItemDamage(target, Damage.DamageItems.Hydra)
                : 0;

            var bwc = Items.HasItem(3144) && Items.CanUseItem(3144)
                ? player.GetItemDamage(target, Damage.DamageItems.Bilgewater)
                : 0;

            var brk = Items.HasItem(3153) && Items.CanUseItem(3153)
                ? player.GetItemDamage(target, Damage.DamageItems.Botrk)
                : 0;

            var items = tmt + hyd + tdh + bwc + brk;

            var damage = (rq * 3 + ra * 3 + rw + rr + ii + items);

            return xtra((float) damage);
        }


        private static double Wdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (w.LSIsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] {50, 80, 110, 150, 170}[w.Level - 1] + 1*player.FlatPhysicalDamageMod + player.BaseAttackDamage);
            }

            return dmg;
        }

        private static double Qdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (q.LSIsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    -10 + (q.Level * 20) + (0.35 + (q.Level * 0.05)) * (player.FlatPhysicalDamageMod + player.BaseAttackDamage));
            }

            return dmg;
        }

        private static double Rdmg(Obj_AI_Base target)
        {
            double dmg = 0;

            if (r.LSIsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    (new double[] {80, 120, 160}[Math.Max(r.Level, 1) - 1] + 0.6 * player.FlatPhysicalDamageMod) *
                    (((target.MaxHealth - target.Health) / target.MaxHealth) * 2.67 + 1));
            }

            return dmg;
        }

        #endregion

        #region Riven: Drawings

        private static void Drawings()
        {
            Drawing.OnDraw += args =>
            {
                if (!player.IsDead)
                {
                    if (riventarget().LSIsValidTarget())
                    {
                        var tpos = Drawing.WorldToScreen(riventarget().Position);

                        if (menu.Item("drawf").GetValue<Circle>().Active)
                        {
                            Render.Circle.DrawCircle(riventarget().Position, 120,
                                menu.Item("drawf").GetValue<Circle>().Color, 1);
                        }

                        if (riventarget().HasBuff("Stun"))
                        {
                            var b = riventarget().GetBuff("Stun");
                            if (b.Caster.IsMe && b.EndTime - Game.Time > 0)
                            {
                                Drawing.DrawText(tpos[0], tpos[1], Color.Lime, "STUNNED " + (b.EndTime - Game.Time).ToString("F"));
                            }
                        }
                    }

                    if (_sh.LSIsValidTarget())
                    {
                        if (menu.Item("drawf").GetValue<Circle>().Active)
                        {
                            Render.Circle.DrawCircle(_sh.Position, 90, menu.Item("drawf").GetValue<Circle>().Color, 6);
                        }
                    }

                    if (menu.Item("drawengage").GetValue<Circle>().Active)
                    {
                        Render.Circle.DrawCircle(player.Position,
                                player.AttackRange + e.Range + 35, menu.Item("drawengage").GetValue<Circle>().Color,
                                menu.Item("linewidth").GetValue<Slider>().Value);
                        }

                    if (menu.Item("drawr2").GetValue<Circle>().Active)
                    {
                        Render.Circle.DrawCircle(player.Position, r.Range, menu.Item("drawr2").GetValue<Circle>().Color,
                            menu.Item("linewidth").GetValue<Slider>().Value);
                    }

                    if (menu.Item("drawburst").GetValue<Circle>().Active && (canburst() || shy()) && riventarget().LSIsValidTarget())
                    {
                        var xrange = menubool("flashb") && flash.LSIsReady() ? 255 : 0;
                        Render.Circle.DrawCircle(riventarget().Position, e.Range + w.Range - 25 + xrange,
                            menu.Item("drawburst").GetValue<Circle>().Color, menu.Item("linewidth").GetValue<Slider>().Value);
                    }
                }
            };

            Drawing.OnEndScene += args =>
            {
                if (!menubool("drawdmg"))
                    return;

                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
                {
                    var color = r.LSIsReady() && IsLethal(enemy)
                        ? new ColorBGRA(0, 255, 0, 90)
                        : new ColorBGRA(255, 255, 0, 90);

                    hpi.unit = enemy;
                    hpi.drawDmg(ComboDamage(enemy), color);
                }

            };
        }

        #endregion
    }
}
