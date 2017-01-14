using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;

using EloBuddy; 
using LeagueSharp.Common; 
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
           return r.IsReady() ? (float) (dmg + (dmg*0.2)) : dmg;
        }

        private static bool IsLethal(Obj_AI_Base unit)
        {
            return ComboDamage(unit) / 1.65 >= unit.Health;
        }

        private static Vector2 ExtendDir(Vector2 pos, Vector2 dir, float distance)
        {
            return pos + dir * distance;
        }

        private static Vector3 ExtendDir(Vector3 pos, Vector3 dir, float distance)
        {
            return pos + dir * distance;
        }

        private static void TryIgnote(Obj_AI_Base target)
        {
            var ignote = player.GetSpellSlot("summonerdot");
            if (player.Spellbook.CanUseSpell(ignote) == SpellState.Ready)
            {
                if (target.Distance(player.ServerPosition) <= 600)
                {
                    if (cc <= menuslide("userq") && q.IsReady() && menubool("useignote"))
                    {
                        if (ComboDamage(target) >= target.Health &&
                            target.Health / target.MaxHealth * 100 > menuslide("overk") || 
                            menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (r.IsReady() && uo)
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

            if (target.Distance(player.ServerPosition, true) <= 450 * 450)
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
            r.SetSkillshot(0.25f, 110f, 1600f, false, SkillshotType.SkillshotCircle);

            flash = player.GetSpellSlot("summonerflash");
            OnSpellCast();

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
                     .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                     .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }
        }

        private static AIHeroClient riventarget()
        {
            var cursortarg = HeroManager.Enemies
                .Where(x => x.Distance(Game.CursorPos) <= 1400 && x.Distance(player.ServerPosition) <= 1400)
                .OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault(x => x.IsValidTarget());

            var closetarg = HeroManager.Enemies
                .Where(x => x.Distance(player.ServerPosition) <= e.Range + 100)
                .OrderBy(x => x.Distance(player.ServerPosition)).FirstOrDefault(x => x.IsValidTarget());

            return _sh ?? cursortarg ?? closetarg;
        }

        private static bool wrektAny()
        {
            return menu.SubMenu("combo").SubMenu("rivenw").SubMenu("req").Items.Any(i => i.GetValue<bool>()) &&
                 player.GetEnemiesInRange(1250).Any(ez => menu.Item("w" + ez.ChampionName).GetValue<bool>());
        }

        private static bool rrektAny()
        {
            return menu.SubMenu("combo").SubMenu("rivenr2").SubMenu("req2").Items.Any(i => i.GetValue<bool>()) &&
                 player.GetEnemiesInRange(1250).Any(ez => menu.Item("r" + ez.ChampionName).GetValue<bool>());
        }

        #region Riven: OnSpellCast
        private static void OnSpellCast()
        {
            Obj_AI_Base.OnSpellCast += (sender, args) =>
            {
                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    qtarg = args.Target as Obj_AI_Base;
                    didaa = false;
                }

                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    var aiHero = args.Target as AIHeroClient;
                    if (aiHero.IsValidTarget() && !player.UnderTurret(true))
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                        if (Items.CanUseItem(3748))
                            Items.UseItem(3748);
                    }

                    if (menu.Item("clearkey").GetValue<KeyBind>().Active)
                    {
                        var aiMob = args.Target as Obj_AI_Minion;
                        if (aiMob != null && aiMob.IsValidTarget() && !aiMob.IsMinion)
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }

                        var unit = args.Target as AttackableUnit;
                        if (unit != null && !HeroManager.Enemies.Any(x => x.IsValidTarget(1200)))
                        {
                            if (q.IsReady() && !didaa)
                            {
                                q.Cast(unit.Position);
                            }
                        }
                    }

                    if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (riventarget().IsValidTarget() && !riventarget().IsZombie && 
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
                        if (aiHero.IsValidTarget() && !riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                            if (Items.CanUseItem(3748))
                                Items.UseItem(3748);
                        }
                    }

                    if (menu.Item("combokey").GetValue<KeyBind>().Active ||
                        menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (e.IsReady() && riventarget().IsValidTarget(e.Range + 200))
                        {
                            if (player.Health / player.MaxHealth * 100 <= menuslide("vhealth"))
                            {
                                if (menubool("usecomboe") && !didaa)
                                {
                                    if (aiHero != null && aiHero.IsValidTarget())
                                    {
                                        e.Cast(aiHero.ServerPosition);
                                    }
                                }
                            }
                        }
                    }

                    if (menu.Item("combokey").GetValue<KeyBind>().Active && aiHero.IsValidTarget())
                    {
                        if (w.IsReady() && riventarget().Distance(player.ServerPosition) <= w.Range)
                        {
                            if (menubool("usecombow") && !didaa)
                            {
                                if (!isteamfightkappa ||
                                    isteamfightkappa && !wrektAny() ||
                                    menubool("w" + aiHero.ChampionName))
                                {
                                    if (Utils.GameTimeTickCount - lasthd < 1000)
                                    {
                                        w.Cast();
                                    }

                                    if (aiHero.HealthPercent < player.HealthPercent
                                        || (int) aiHero.HealthPercent == (int)player.HealthPercent)
                                    {
                                        if (cc >= 2 || !q.IsReady() || player.Distance(aiHero) > truerange)
                                        {
                                            w.Cast();
                                        }
                                    }
                                    else
                                    {
                                        w.Cast();
                                    }
                                }
                            }
                        }

                        if (q.IsReady() && riventarget().Distance(player.ServerPosition) <= q.Range + 100)
                        {
                            useinventoryitems(riventarget());

                            if (menulist("wsmode") == 1 && IsLethal(riventarget()))
                            {
                                if (cc == 2 && e.IsReady() && !didaa)
                                {
                                    e.Cast(riventarget().ServerPosition);
                                }
                            }

                            if (menubool("safeq"))
                            {
                                var endq = player.Position.Extend(riventarget().Position, q.Range + 35);
                                if (endq.CountEnemiesInRange(200) <= 2)
                                {
                                    q.Cast(riventarget().ServerPosition);
                                }
                            }

                            else
                            {
                                q.Cast(riventarget().ServerPosition);
                            }
                        }
                    }

                    if (menu.Item("clearkey").GetValue<KeyBind>().Active)
                    {
                        var aiBase = args.Target as Obj_AI_Base;
                        if (aiBase != null && aiBase.IsValidTarget())
                        {
                            if (w.IsReady() && !aiBase.Name.Contains("Mini") && menubool("usejunglew"))
                            {
                                if (aiBase is Obj_AI_Minion && aiBase.Distance(player.ServerPosition) <= w.Range + 25)
                                {
                                    w.Cast();
                                }
                            }

                            if (q.IsReady() && !didaa && menubool("usejungleq") && !aiBase.IsMinion)
                            {
                                if (aiBase.Distance(player.ServerPosition) <= q.Range + 90)
                                {
                                    if (canhd && !(aiBase is Obj_AI_Turret)) return;

                                    if (qtarg != null && qtarg.NetworkId == aiBase.NetworkId)
                                        q.Cast(aiBase.ServerPosition);
                                }
                            }

                            if (player.CountEnemiesInRange(900) < 1 || !menubool("clearnearenemy"))
                            {
                                if (q.IsReady() && !didaa && menubool("uselaneq"))
                                {
                                    if (aiBase.Distance(player.ServerPosition) <= q.Range + 90)
                                    {
                                        if (canhd && !(aiBase is Obj_AI_Turret)) return;

                                        if (qtarg != null && qtarg.NetworkId == aiBase.NetworkId)
                                            q.Cast(aiBase.ServerPosition);
                                    }
                                }
                            }
                        }

                        if (!player.UnderTurret(true))
                        {
                            if (!HeroManager.Enemies.Any(x => x.IsValidTarget(1400)))
                            {
                                if (Utils.GameTimeTickCount - lasthd < 1600 && args.Target is Obj_AI_Minion)
                                {
                                    if (w.IsReady() && args.Target.Position.Distance(player.ServerPosition) <= w.Range + 25)
                                    {
                                        w.Cast();
                                    }
                                }
                            }
                        }
                    }
                }

                if (sender.IsMe && args.SData.IsAutoAttack())
                {
                    SemiQ();
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
            truerange = player.AttackRange + player.Distance(player.BBox.Minimum) + 1;

            // if no valid target cancel to cursor pos
            if (!qtarg.IsValidTarget(truerange + 100))
                 qtarg = player;

            if (!riventarget().IsValidTarget())
                _sh = null;

            // move target position
            if (qtarg != player && qtarg.Distance(player.ServerPosition) < r.Range)
                movepos = player.Position.Extend(Game.CursorPos, player.Distance(Game.CursorPos) + 500);

            // move to game cursor pos
            if (qtarg == player)
                movepos = player.ServerPosition + (Game.CursorPos - player.ServerPosition).Normalized() * 125;
          
            AuraUpdate();
            Helpers();

            if (menu.Item("useskin").GetValue<bool>())
            {
                player.SetSkin(player.BaseSkinName, menu.Item("skinid").GetValue<Slider>().Value);
            }

            if (riventarget().IsValidTarget())
            {
                if (menu.Item("combokey").GetValue<KeyBind>().Active)
                {
                    ComboTarget(riventarget());
                    TryIgnote(riventarget());
                }
            }

            if (menu.Item("shycombo").GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(riventarget(), Game.CursorPos, 90f, 100f);

                if (riventarget().IsValidTarget())
                {
                    SomeDash(riventarget());

                    if (w.IsReady() && riventarget().Distance(player.ServerPosition) <= w.Range + 50)
                    {
                        checkr();
                        w.Cast();
                    }

                    else if (q.IsReady() && riventarget().Distance(player.ServerPosition) <= truerange + 100)
                    {
                        checkr();
                        TryIgnote(riventarget());

                        if (!didaa && !canhd && Utils.GameTimeTickCount - lasthd >= 300)
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

            if (didhs && riventarget().IsValidTarget())
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

            isteamfightkappa = player.CountAlliesInRange(1500) > 1 && player.CountEnemiesInRange(1350) > 2 ||
                               player.CountEnemiesInRange(1200) > 2;
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
            //rmenu.AddItem(new MenuItem("flashc", "-> Flash in Combo")).SetValue(false).SetFontStyle(FontStyle.Regular, SharpDX.Color.LawnGreen);

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
            r2menu.AddItem(new MenuItem("saver", "Smart Save R2")).SetValue(false);
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

            var farming = new Menu("Farm", "farming");

            var wc = new Menu("Jungle", "waveclear");
            wc.AddItem(new MenuItem("usejungleq", "Use Q in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglew", "Use W in Jungle")).SetValue(true);
            wc.AddItem(new MenuItem("usejunglee", "Use E in Jungle")).SetValue(true);
            farming.AddSubMenu(wc);

            var jg = new Menu("WaveClear", "jungle");
            jg.AddItem(new MenuItem("clearnearenemy", "Dont Clear Near Enemy")).SetValue(false);
            jg.AddItem(new MenuItem("uselaneq", "Use Q in WaveClear")).SetValue(true);
            jg.AddItem(new MenuItem("uselanew", "Use W in WaveClear")).SetValue(true);
            jg.AddItem(new MenuItem("wminion", "Use W in WaveClear Minions >=")).SetValue(new Slider(3, 1, 6));
            jg.AddItem(new MenuItem("usewlaneaa", "Use W on Unkillable Minion")).SetValue(true);
            jg.AddItem(new MenuItem("uselanee", "Use E in WaveClear")).SetValue(true);
            farming.AddSubMenu(jg);

            menu.AddSubMenu(farming);

            var skmenu = new Menu("El Skins", "skmenu");
            var skinitem = new MenuItem("useskin", "Enabled");
            skmenu.AddItem(skinitem).SetValue(false);

            skinitem.ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                    //ObjectManager.//Player.SetSkin(ObjectManager.Player.BaseSkinName, ObjectManager.Player.SkinId);
                }
            };

            skmenu.AddItem(new MenuItem("skinid", "Skin Id")).SetValue(new Slider(8, 0, 12));
            menu.AddSubMenu(skmenu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Riven : Some Dash
        private static bool canburst()
        {
            if (riventarget() == null || !r.IsReady())
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
            if (r.IsReady() && riventarget() != null && menulist("multib") != 0)
            {
                return true;
            }

            return false;
        }

        private static void doFlash()
        {
            if (riventarget() != null && (canburst() || shy()))
            {
                if (!flash.IsReady() || !menubool("flashb"))
                    return;

                if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (riventarget().Distance(player.ServerPosition) > e.Range + 50 &&
                        riventarget().Distance(player.ServerPosition) <= e.Range + w.Range + 275)
                    {
                        if (player.Spellbook.CastSpell(flash, riventarget().ServerPosition.Extend(player.ServerPosition, 125)))
                        {
                            Chat.Say("/d");
                            Orbwalking.ResetAutoAttackTimer();
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

            if (riventarget() == null || !r.IsReady())
                return;

            if (flash.IsReady() &&  w.IsReady() && (canburst() || shy()) && menulist("multib") != 2)
            {
                if (e.IsReady() && target.Distance(player.ServerPosition) <= e.Range + w.Range + 275)
                {
                    if (target.Distance(player.ServerPosition) > e.Range + truerange + 50)
                    {
                        e.Cast(target.ServerPosition);
                        useinventoryitems(target);
                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.IsReady() && target.Distance(player.ServerPosition) <= w.Range + 275)
                {
                    if (target.Distance(player.ServerPosition) > truerange + 50)
                    {
                        useinventoryitems(target);
                        if (!uo)
                            r.Cast();
                    }
                }
            }

            else
            {
                if (e.IsReady() && target.Distance(player.ServerPosition) <= e.Range + w.Range - 25)
                {
                    useinventoryitems(target);
                    if (target.Distance(player.ServerPosition) > truerange + 50)
                    {
                        e.Cast(target.ServerPosition);
                        useinventoryitems(target);

                        if (!uo)
                            r.Cast();
                    }
                }

                if (!e.IsReady() && target.Distance(player.ServerPosition) <= w.Range - 10)
                {
                    useinventoryitems(target);

                    if (!uo)
                        r.Cast();
                }
            }
        }

        #endregion

        #region Riven: Combo

        private static void ComboTarget(AIHeroClient target)
        {
            TryIgnote(target);

            var ende = player.Position.Extend(target.Position, e.Range + 35);
            var catchRange = e.IsReady() ? e.Range + truerange + w.Range : truerange + w.Range;

            if (target.Distance(player.ServerPosition) <= e.Range + 100 && q.IsReady())
            {
                if (Utils.GameTimeTickCount - lastw < 500 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (target.Distance(player.ServerPosition) <= e.Range + 100 && q.IsReady())
                    {
                        DoOneQ(target.ServerPosition);
                    }
                }
            }

            if (e.IsReady() && menubool("usecomboe")
                && target.Distance(player.ServerPosition) > truerange + 100
                && (target.Distance(player.ServerPosition) <= e.Range + w.Range  
                || uo && target.Distance(player.ServerPosition) > truerange + 200) 
                || target.Distance(player.ServerPosition) <= e.Range + w.Range + q.Range/2f && r.IsReady()
                && (cc == 2 && IsLethal(target) || cc == 2 && target.CountEnemiesInRange(w.Range + 35) >= 2))
            {
                if (!didaa)
                {
                    if (menubool("safee"))
                    {
                        if (ende.CountEnemiesInRange(200) <= 2)
                        {
                            e.Cast(target.ServerPosition);
                        }
                    }

                    else
                    {
                        e.Cast(target.ServerPosition);
                    }

                    if (target.Distance(player.ServerPosition) <= e.Range + w.Range)
                    {
                        checkr();

                        if (!canburst() && canhd && uo && cc != 2)
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);
                        }
                    }

                    if (!canburst() && canhd && cc != 2)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }
            }

            if (w.IsReady() && menubool("usecombow") && target.Distance(player.ServerPosition) <= w.Range)
            {
                if (Utils.GameTimeTickCount - lasthd > 1500)
                {
                    useinventoryitems(target);
                    checkr();

                    if (menubool("usecombow") && !didaa)
                    {
                        if (!isteamfightkappa || 
                            isteamfightkappa && !wrektAny() || 
                            menubool("w" + target.ChampionName))
                        {
                            if (target.HealthPercent < player.HealthPercent 
                                || (int) target.HealthPercent == (int) player.HealthPercent)
                            {
                                if (cc >= 2 || !q.IsReady() || player.Distance(target) > truerange)
                                {
                                    w.Cast();
                                }
                            }
                            else 
                            {
                                w.Cast();
                            }
                        }
                    }
                }
            }

            if (menubool("usegap") && !e.IsReady() && q.IsReady() && target.Distance(player.ServerPosition) > catchRange)
            {
                if (Utils.GameTimeTickCount - lastq >= menuslide("gaptimez") * 10)
                {
                    if (q.IsReady() && Utils.GameTimeTickCount - laste >= 1000)
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }
            else
            {
                if (target.Distance(player.ServerPosition) <= e.Range + w.Range)
                {
                    checkr();
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
                        (player.ServerPosition - target.ServerPosition).Normalized()*500;
                    break;
                case 1:
                    var tt = ObjectManager.Get<Obj_AI_Turret>()
                        .Where(t => (t.IsAlly)).OrderBy(t => t.Distance(player.Position)).First();
                    if (tt != null)
                        qpos = tt.Position;
                    else
                        qpos = player.ServerPosition +
                                (player.ServerPosition - target.ServerPosition).Normalized() * 500;
                    break;
                default:
                    qpos = Game.CursorPos;
                    break;
            }

            if (q.IsReady())
                Orbwalking.Orbwalk(target, Game.CursorPos, 90f, 100f);

            if (cc == 2 && !didaa && q.IsReady())
            {
                if (!e.IsReady())
                {
                    orbwalker.SetAttack(false);
                    orbwalker.SetMovement(false);

                    if (EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, qpos))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(175 - Game.Ping/2, () =>
                        {
                            q.Cast(qpos);

                            orbwalker.SetAttack(true);
                            orbwalker.SetMovement(true);
                        });
                    }
                }
            }

            if (e.IsReady() && (cc == 3 || !q.IsReady() && cc == 0))
            {
                if (player.Distance(target.ServerPosition) <= 300)
                {
                    if (menubool("usegaph") && !didaa)
                        e.Cast(qpos);
                }
            }

            if (!target.ServerPosition.UnderTurret(true))
            {
                if (q.IsReady() && !didaa && (cc < 2 || e.IsReady()))
                {
                    if (target.Distance(player.ServerPosition) <= truerange + q.Range)
                    {
                        q.Cast(target.ServerPosition);
                    }
                }
            }

            if (e.IsReady() && !didaa && q.IsReady() && cc < 1 &&
                target.Distance(player.ServerPosition) > truerange + 100 &&
                target.Distance(player.ServerPosition) <= e.Range + truerange + 50)
            {
                if (!target.ServerPosition.UnderTurret(true))
                {
                    if (menubool("usegaph") && !didaa)
                    {
                        e.Cast(target.ServerPosition);
                    }
                }
            }

            else if (w.IsReady() && !didaa && target.Distance(player.ServerPosition) <= w.Range + 10)
            {
                if (!player.ServerPosition.UnderTurret(true))
                {
                    if (menubool("useharassw") && !didaa)
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
            if (uo && menubool("usews") && r.IsReady())
            {
                if (menu.Item("shycombo").GetValue<KeyBind>().Active && canburst())
                {
                    if (riventarget().Distance(player.ServerPosition) <= player.AttackRange + 100)
                    {
                        if (canhd) return;
                    }
                }

                #region MaxDmage

                if (menulist("wsmode") == 1)
                {
                    if (riventarget().IsValidTarget(r.Range) && !riventarget().IsZombie)
                    {
                        if (Rdmg(riventarget()) / riventarget().MaxHealth * 100 >= 50)
                        {
                            var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                            if (p.Hitchance >= HitChance.Medium && !didaa && !riventarget().HasBuff("kindredrnodeathbuff"))
                            {
                                if (!isteamfightkappa || menubool("r" + riventarget().ChampionName) || isteamfightkappa && !rrektAny())
                                {
                                    r.Cast(p.CastPosition);
                                }
                            }
                        }

                        if (q.IsReady() && cc < 2 || q.IsReady(2) && cc < 2)
                        {
                            var aadmg = player.GetAutoAttackDamage(riventarget(), true) * 2;
                            var currentrdmg = Rdmg(riventarget());
                            var qdmg = Qdmg(riventarget()) * 2;

                            var damage = aadmg + currentrdmg + qdmg;

                            if (xtra((float) damage) >= riventarget().Health
                                || riventarget().CountEnemiesInRange(275) >= 2)
                            {
                                if (riventarget().Distance(player.ServerPosition) <= truerange + q.Range)
                                {
                                    var p = r.GetPrediction(riventarget(), true, -1f, new[] { CollisionableObjects.YasuoWall });
                                    if (!riventarget().HasBuff("kindredrnodeathbuff") && !didaa)
                                    {
                                        if (p.Hitchance >= HitChance.High && !didaa)
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
            if (uo && menubool("usews") && r.IsReady())
            {
                #region Killsteal
                foreach (var t in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(r.Range)))
                {
                    if (menubool("saver") && Orbwalking.InAutoAttackRange(t))
                    {
                        if (player.GetAutoAttackDamage(t, true) *  3 >= t.Health)
                        {
                            if (player.HealthPercent > 75)
                            {
                                if (t.CountEnemiesInRange(400) > 1 && t.CountEnemiesInRange(400) <= 2)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                    if (Rdmg(t) >= t.Health)
                    {
                        var p = r.GetPrediction(t, true, -1f, new[] { CollisionableObjects.YasuoWall });
                        if (p.Hitchance == (HitChance) menulist("rhitc") + 4 && !didaa && !t.HasBuff("kindredrnodeathbuff"))
                        {
                            r.Cast(p.CastPosition);
                        }
                    }

                    if (w.IsReady() && t.Distance(player) <= 250 && Rdmg(t) + Wdmg(t) >= t.HealthPercent)
                    {
                        var p = r.GetPrediction(t, true, -1f, new[] { CollisionableObjects.YasuoWall });
                        if (p.Hitchance == (HitChance)menulist("rhitc") + 4 && !didaa && !t.HasBuff("kindredrnodeathbuff"))
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
                if (Utils.GameTimeTickCount - lastw < 1000 && Utils.GameTimeTickCount - lasthd < 1000)
                {
                    if (unit.Distance(player.ServerPosition) <= q.Range + 90 && q.IsReady())
                    {
                        DoOneQ(unit.ServerPosition);
                    }
                }

                if (Utils.GameTimeTickCount - laste < 600)
                {
                    if (unit.Distance(player.ServerPosition) <= w.Range + 45)
                    {
                        if (Items.CanUseItem(3077))
                            Items.UseItem(3077);
                        if (Items.CanUseItem(3074))
                            Items.UseItem(3074);
                    }
                }

                if (e.IsReady() && !didaa && menubool("usejunglee"))
                {
                    if (player.Health / player.MaxHealth * 100 <= 70 ||
                        unit.Distance(player.ServerPosition) > truerange + 30)
                    {
                        e.Cast(Game.CursorPos);
                    }
                }

                if (w.IsReady() && !didaa && menubool("usejunglew"))
                {
                    if (unit.Distance(player.ServerPosition) <= w.Range + 25)
                    {
                        if (!canhd)
                        {
                            w.Cast();
                        }
                    }
                }
            }
        }

        private static void Wave()
        {
            var minions = MinionManager.GetMinions(player.Position, 600f);

            foreach (var unit in minions.Where(x => x.IsMinion))
            {
                if (player.CountEnemiesInRange(900) >= 1 && menubool("clearnearenemy"))
                {
                    return;
                }

                if (didaa && !Orbwalking.CanAttack() && Wdmg(unit) >= unit.Health && unit.IsValidTarget(w.Range))
                {
                    if (menubool("usewlaneaa") && w.IsReady())
                    {
                        w.Cast();
                    }
                }

                if (w.IsReady())
                {
                    if (minions.Count(m => m.Distance(player.ServerPosition) <= w.Range + 10) >= menuslide("wminion"))
                    {
                        if (!didaa && menubool("uselanew"))
                        {
                            if (Items.CanUseItem(3077))
                                Items.UseItem(3077);
                            if (Items.CanUseItem(3074))
                                Items.UseItem(3074);

                            w.Cast();
                        }
                    }
                }

                if (e.IsReady() && !player.ServerPosition.Extend(unit.ServerPosition, e.Range).UnderTurret(true))
                {
                    if (unit.Distance(player.ServerPosition) > truerange + 30)
                    {
                        if (!didaa && menubool("uselanee"))
                        {
                            e.Cast(unit.ServerPosition);
                        }
                    }

                    else if (player.Health / player.MaxHealth * 100 <= 70)
                    {
                        if (!didaa && menubool("uselanee"))
                        {
                            e.Cast(unit.ServerPosition);
                        }
                    }
                }
            }
        }

        #endregion

        #region Riven: Flee

        private static void Flee()
        {
            if (!didaa)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }

            if (cc > 2 && Items.GetWardSlot() != null && menubool("wq3"))
            {
                var attacker = HeroManager.Enemies.FirstOrDefault(x => x.Distance(player.ServerPosition) <= q.Range + 50);
                if (attacker.IsValidTarget(q.Range))
                {
                    if (Utils.GameTimeTickCount - lastwd >= 1000 && didq)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(100,
                            () => Items.UseItem((int) Items.GetWardSlot().Id, attacker.ServerPosition));
                    }
                }
            }

            if (player.CountEnemiesInRange(w.Range) > 0)
            {
                if (w.IsReady())
                    w.Cast();
            }

            if (ssfl)
            {
                if (Utils.GameTimeTickCount - lastq >= 600)
                {
                    q.Cast(Game.CursorPos);
                }

                if (!didaa && e.IsReady())
                {
                    if (cc >= 2 || !q.IsReady() && !player.HasBuff("RivenTriCleave", true))
                    {
                        if (!player.ServerPosition.Extend(Game.CursorPos, e.Range + 10).IsWall())
                            e.Cast(Game.CursorPos);
                    }
                }
            }

            else
            {
                if (q.IsReady())
                {
                    q.Cast(Game.CursorPos);
                }

                if (e.IsReady() && Utils.GameTimeTickCount - lastq >= 250)
                {
                    if (!player.ServerPosition.Extend(Game.CursorPos, e.Range).IsWall())
                        e.Cast(Game.CursorPos);
                }
            }
        }

        #endregion

        #region Riven: Semi Q 

        private static void SemiQ()
        {
            if (menubool("semiq"))
            {
                if (q.IsReady() && qtarg != null)
                {
                    if (qtarg.IsValidTarget(q.Range + 100) &&
                        !menu.Item("clearkey").GetValue<KeyBind>().Active &&
                        !menu.Item("harasskey").GetValue<KeyBind>().Active &&
                        !menu.Item("combokey").GetValue<KeyBind>().Active &&
                        !menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (qtarg.IsValid<AIHeroClient>() && !qtarg.UnderTurret(true))
                            q.Cast(qtarg.ServerPosition);
                    }

                    if (!menu.Item("harasskey").GetValue<KeyBind>().Active &&
                        !menu.Item("clearkey").GetValue<KeyBind>().Active &&
                        !menu.Item("combokey").GetValue<KeyBind>().Active &&
                        !menu.Item("shycombo").GetValue<KeyBind>().Active)
                    {
                        if (qtarg.IsValidTarget(q.Range + 100) && !qtarg.Name.Contains("Mini"))
                        {
                            if (!qtarg.Name.StartsWith("Minion") && minionlist.Any(name => qtarg.Name.StartsWith(name)))
                            {
                                q.Cast(qtarg.ServerPosition);
                            }
                        }

                        if (qtarg.IsValidTarget(q.Range + 100))
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

        #endregion

        #region Riven: Check R
        private static void checkr()
        {
            if (!r.IsReady() || uo || !menu.Item("user").GetValue<KeyBind>().Active)
            {
                return;
            }

            if (menu.Item("shycombo").GetValue<KeyBind>().Active)
            {
                r.Cast();
                return;
            }

            var targets = HeroManager.Enemies.Where(ene => ene.IsValidTarget(r.Range));
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

                if (q.IsReady() || Utils.GameTimeTickCount - lastq < 1000 && cc < 3)
                {
                    if (heroes.Count() < 2)
                    {
                        if (target.Health / target.MaxHealth * 100 <= menuslide("overk") && IsLethal(target))
                            return;
                    }

                    if (heroes.Count(ene => ene.Distance(player.ServerPosition) <= 750) > 1)
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

                if (args.SData.IsAutoAttack())
                {
                    qtarg = args.Target as Obj_AI_Base;
                    lastaa = Utils.GameTimeTickCount;
                }

                if (!didq && args.SData.IsAutoAttack())
                {
                    var targ = (AttackableUnit) args.Target;
                    if (targ != null)
                    {
                        didaa = true;
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
                        didaa = false;

                        if (qtarg != null && (!w.IsReady() || !menubool("usecombow")))
                        {
                            if (menu.Item("combokey").GetValue<KeyBind>().Active 
                            ||  menu.Item("clearkey").GetValue<KeyBind>().Active && !qtarg.UnderTurret(true))
                            {
                                if (!menu.Item("shycombo").GetValue<KeyBind>().Active)
                                {
                                    if (q.IsReady() && qtarg.Distance(player.ServerPosition) <= 350)
                                    {
                                        LeagueSharp.Common.Utility.DelayAction.Add(250, () => DoOneQ(qtarg.ServerPosition));
                                    }
                                }
                            }
                        }

                        if (menulist("wsmode") == 1 
                            || menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (menu.Item("combokey").GetValue<KeyBind>().Active 
                            || menu.Item("shycombo").GetValue<KeyBind>().Active)
                            {
                                if (canburst() && uo)
                                {
                                    if (riventarget().IsValidTarget() && !riventarget().IsZombie && !riventarget().HasBuff("kindredrnodeathbuff"))
                                    {
                                        if (!isteamfightkappa || menubool("r" + riventarget().ChampionName) ||
                                             isteamfightkappa && !rrektAny() || menu.Item("shycombo").GetValue<KeyBind>().Active)
                                        {
                                            LeagueSharp.Common.Utility.DelayAction.Add(240 - Game.Ping/2,
                                                () =>
                                                {
                                                    if (riventarget().HasBuffOfType(BuffType.Stun))
                                                        r.Cast(riventarget().ServerPosition);
                                                    else
                                                        r.Cast(riventarget());
                                                });
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "RivenTriCleave":
                        cc += 1;
                        didq = true;
                        didaa = false;
                        lastq = Utils.GameTimeTickCount;

                        if (!uo) ssfl = false;
                        break;
                    case "RivenMartyr":
                        didw = true;
                        lastw = Utils.GameTimeTickCount;

                        break;
                    case "RivenFeint":
                        dide = true;
                        didaa = false;
                        laste = Utils.GameTimeTickCount;

                        if (menu.Item("fleekey").GetValue<KeyBind>().Active)
                        {
                            if (uo && r.IsReady() && cc == 2 && q.IsReady())
                            {
                                var btarg = TargetSelector.GetTarget(r.Range, TargetSelector.DamageType.Physical);
                                if (btarg.IsValidTarget())
                                    r.Cast(btarg);
                                else
                                    r.Cast(Game.CursorPos);
                            }
                        }

                        if (menu.Item("shycombo").GetValue<KeyBind>().Active)
                        {
                            if (cc == 2 && !uo && r.IsReady() && riventarget() != null)
                            {
                                checkr();
                                LeagueSharp.Common.Utility.DelayAction.Add(240 - Game.Ping, () => q.Cast(riventarget().ServerPosition));
                            }
                        }

                        if (menu.Item("combokey").GetValue<KeyBind>().Active)
                        {
                            if (cc == 2 && r.IsReady() && riventarget() != null &&
                                (!uo || Utils.GameTimeTickCount - laste < 1000))
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(100, checkr);
                            }
                        }

                        break;
                    case "RivenFengShuiEngine":
                        ssfl = true;
                        doFlash();

                        if (menu.Item("combokey").GetValue<KeyBind>().Active)
                        {
                            if (cc == 2 && r.IsReady() && riventarget() != null && Utils.GameTimeTickCount - laste < 1000)
                            {
                                LeagueSharp.Common.Utility.DelayAction.Add(200 - Game.Ping / 2, () => q.Cast(riventarget().ServerPosition));
                            }
                        }

                        break;
                    case "RivenIzunaBlade":
                        ssfl = false;
                        didws = true;

                       if (cc == 2 && q.IsReady() && riventarget().IsValidTarget(r.Range))
                            q.Cast(riventarget().ServerPosition);

                        if (w.IsReady() && riventarget().IsValidTarget(w.Range + 35))
                            w.Cast();

                        else if (q.IsReady() && riventarget().IsValidTarget(e.Range + q.Range))
                            q.Cast(riventarget().ServerPosition);

                        break;
                }
            };
        }

        #endregion

        #region Riven: Misc 

        private static void DoOneQ(Vector3 pos)
        {
            if (q.IsReady() && Utils.GameTimeTickCount - lastq > 5000)
            {
                if (q.Cast(pos))
                {
                    lastq = Utils.GameTimeTickCount;
                    didq = true;
                }
            }
        }

        private static void Interrupter()
        {
            Interrupter2.OnInterruptableTarget += (sender, args) =>
            {
                if (menubool("wint") && w.IsReady())
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.IsValidTarget(w.Range))
                            w.Cast();

                        if (sender.IsValidTarget(w.Range + e.Range) && e.IsReady())
                        {
                            e.Cast(sender.ServerPosition);
                        }
                    }
                }

                if (menubool("qint") && q.IsReady() && cc >= 2)
                {
                    if (!sender.Position.UnderTurret(true))
                    {
                        if (sender.IsValidTarget(q.Range))
                            q.Cast(sender.ServerPosition);

                        if (sender.IsValidTarget(q.Range + e.Range) && e.IsReady())
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
                if (menubool("wgap") && w.IsReady())
                {
                    if (gapcloser.Sender.IsValidTarget(w.Range))
                    {
                        if (!gapcloser.Sender.ServerPosition.UnderTurret(true))
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
            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
            {
                if (sender.IsMe)
                {
                    if (orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None &&
                        menu.Item("shycombo").GetValue<KeyBind>().Active == false && !qtarg.IsValidTarget(300))
                        return;

                    // credits to whoever did this first
                    // copy pasta kappa

                    if (args.Animation.Contains("Spell1a"))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add((290 - Game.Ping/2), () =>
                        {
                            Chat.Say("/d");
                            Orbwalking.ResetAutoAttackTimer();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, + 10));
                        });
                    }
                    else if (args.Animation.Contains("Spell1b"))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add((290 - Game.Ping/2), () =>
                        {
                            Chat.Say("/d");
                            Orbwalking.ResetAutoAttackTimer();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, + 10));
                        });
                    }
                    else if (args.Animation.Contains("Spell1c"))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add((380 - Game.Ping/2), () =>
                        {
                            Chat.Say("/d");
                            Orbwalking.ResetAutoAttackTimer();
                            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, player.Position.Extend(Game.CursorPos, + 10));
                        });
                    }
                }
            };
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

                if (player.HasBuff("RivenTriCleave", true) && !menu.Item("shycombo").GetValue<KeyBind>().Active)
                {
                    if (player.GetBuff("RivenTriCleave").EndTime - Game.Time <= 0.25f)
                    {
                        if (!player.IsRecalling() && !player.Spellbook.IsChanneling)
                        {
                            var qext = player.ServerPosition.To2D() + 
                                       player.Direction.To2D().Perpendicular() * q.Range + 100;

                            if (menubool("keepq"))
                            {
                                if (qext.To3D().CountEnemiesInRange(200) <= 1 && !qext.To3D().UnderTurret(true))
                                {
                                    q.Cast(Game.CursorPos);
                                }
                            }
                        }
                    }
                }

                if (r.IsReady() && uo && menubool("keepr"))
                {
                    if (player.GetBuff("RivenFengShuiEngine").EndTime - Game.Time <= 0.25f)
                    {
                        if (!riventarget().IsValidTarget(r.Range) || riventarget().HasBuff("kindredrnodeathbuff"))
                        {
                            if (e.IsReady() && uo)
                                e.Cast(Game.CursorPos);

                            r.Cast(Game.CursorPos);
                        }

                        if (riventarget().IsValidTarget(r.Range) && !riventarget().HasBuff("kindredrnodeathbuff"))
                            r.CastIfHitchanceEquals(riventarget(), HitChance.High);
                    }
                }

                if (!player.HasBuff("rivenpassiveaaboost", true))
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => pc = 1);

                if (cc > 2)
                    LeagueSharp.Common.Utility.DelayAction.Add(1000, () => cc = 0);
            }
        }

        #endregion

        #region Riven : Combat/Orbwalk

        private static void Helpers()
        {
            if (didaa && Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= lastaa + player.AttackDelay * 1000)
                didaa = false;

            if (didhd && canhd && Utils.GameTimeTickCount - lasthd >= 250)
                didhd = false;

            if (didq && Utils.GameTimeTickCount - lastq >= 500)
                didq = false;

            if (didw && Utils.GameTimeTickCount - lastw >= 266)
                didw = false;

            if (dide && Utils.GameTimeTickCount - laste >= 350)
                dide = false;

            if (didws && Utils.GameTimeTickCount - laste >= 366)
                didws = false;
        }

        #endregion

        #region Riven: Math/Damage

        private static float ComboDamage(Obj_AI_Base target, bool checkq = false)
        {
            if (target == null)
                return 0f;

            var ignote = player.GetSpellSlot("summonerdot");
            var ad = (float)player.GetAutoAttackDamage(target);
            var runicpassive = new[] { 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5 };

            var ra = ad +
                        (float)
                            ((+player.FlatPhysicalDamageMod + player.BaseAttackDamage) *
                            runicpassive[Math.Min(player.Level, 18) / 3]);

            var rw = Wdmg(target);
            var rq = Qdmg(target);
            var rr = r.IsReady() ? Rdmg(target) : 0;

            var ii = (ignote != SpellSlot.Unknown && player.GetSpell(ignote).State == SpellState.Ready && r.IsReady()
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
            if (w.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    new[] {50, 80, 110, 150, 170}[w.Level - 1] + 1*player.FlatPhysicalDamageMod + player.BaseAttackDamage);
            }

            return dmg;
        }

        private static double Qdmg(Obj_AI_Base target)
        {
            double dmg = 0;
            if (q.IsReady() && target != null)
            {
                dmg += player.CalcDamage(target, Damage.DamageType.Physical,
                    -10 + (q.Level * 20) + (0.35 + (q.Level * 0.05)) * (player.FlatPhysicalDamageMod + player.BaseAttackDamage));
            }

            return dmg;
        }

        private static double Rdmg(Obj_AI_Base target)
        {
            double dmg = 0;

            if (r.IsReady() && target != null)
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
                    if (riventarget().IsValidTarget())
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

                    if (_sh.IsValidTarget())
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

                    if (menu.Item("drawburst").GetValue<Circle>().Active && (canburst() || shy()) && riventarget().IsValidTarget())
                    {
                        var xrange = menubool("flashb") && flash.IsReady() ? 255 : 0;
                        Render.Circle.DrawCircle(riventarget().Position, e.Range + w.Range - 25 + xrange,
                            menu.Item("drawburst").GetValue<Circle>().Color, menu.Item("linewidth").GetValue<Slider>().Value);
                    }
                }
            };
        }

        #endregion
    }
}
