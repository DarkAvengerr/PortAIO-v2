using EloBuddy; 
using LeagueSharp.Common; 
namespace DevCassio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Drawing;
    using LeagueSharp;
    using LeagueSharp.Common;
    using DevCommom;
    using SharpDX;
    using SebbyLib;

    /*
     * ##### DevCassio Mods #####
     * 
     * + AntiGapCloser with R when LowHealth
     * + Interrupt Danger Spell with R when LowHealth
     * + LastHit E On Posioned Minions
     * + Ignite KS
     * + Menu No-Face Exploit (PacketCast)
     * + Skin Hack
     * + Show E Damage on Enemy HPBar
     * + Assisted Ult
     * + Block Ult if will not hit
     * + Auto Ult Enemy Under Tower
     * + Auto Ult if will hit X
     * + Jungle Clear
     * + R to Save Yourself, when MinHealth and Enemy IsFacing
     * + Auto Spell Level UP
     * + Play Legit Menu :)
     * done harass e, last hit,jungle no du use e,check gap close. ,flash r,(temp sfx),e dmg fixed,assist r ok ,check mim R enemym,, flee, ks,block r with flash r.auto lvl
     * --------------------------------------------------------------------------------------------
     *  +  aa + e fix, , better w for ranged, blorck r issue?
     *  
     */

    internal class Program
    {
        public static Items.Item Zhonya = new Items.Item(3157, 0);
        private static Menu Config;
        private static Orbwalking.Orbwalker Orbwalker;
        private static readonly List<Spell> SpellList = new List<Spell>();
        private static AIHeroClient Player;
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static List<Obj_AI_Base> MinionList;
        /*private static LevelUpManager levelUpManager;*/
        private static SummonerSpellManager summonerSpellManager;
        private static int lvl1, lvl2, lvl3, lvl4;

        private static long dtBurstComboStart;
        private static long dtLastQCast;
        private static long dtLastSaveYourself;
        private static long dtLastECast;

        //public static Obj_AI_Minion LastAttackedminiMinion;
        //public static float LastAttackedminiMinionTime;
        //private static bool mustDebugPredict = false;

        public static void Main()
        {
            OnGameLoad(new EventArgs());
        }

        private static void OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;

            if (!Player.ChampionName.ToLower().Contains("cassiopeia"))
            {
                return;
            }

            InitializeSpells();
            /*InitializeLevelUpManager();*/
            InitializeMainMenu();
            InitializeAttachEvents();

            Chat.Print(
                $"<font color='#fb762d'>{Assembly.GetExecutingAssembly().GetName().Name} Loaded v{Assembly.GetExecutingAssembly().GetName().Version}</font>");
        }

        private static void InitializeSpells()
        {
            Q = new Spell(SpellSlot.Q, 850);
            Q.SetSkillshot(0.7f, 75f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            W = new Spell(SpellSlot.W, 800);
            W.SetSkillshot(0.75f, 160f, 1000, false, SkillshotType.SkillshotCircle);

            E = new Spell(SpellSlot.E, 700);
            E.SetTargetted(0.125f, 1000);

            R = new Spell(SpellSlot.R, 825);
            R.SetSkillshot(0.5f, (float)(80 * Math.PI / 180), 3200, false, SkillshotType.SkillshotCone);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            summonerSpellManager = new SummonerSpellManager();
        }

        /*private static void InitializeLevelUpManager()
        {
            var priority1 = new[] { 2, 0, 2, 1, 2, 3, 2, 0, 2, 0, 3, 0, 0, 1, 1, 3, 1, 1 };
            var priority2 = new[] { 0, 2, 2, 1, 2, 3, 2, 0, 2, 0, 3, 0, 0, 1, 2, 3, 1, 1 };
            var priority3 = new[] { 0, 2, 1, 2, 2, 3, 2, 0, 2, 0, 3, 0, 0, 1, 1, 3, 1, 1 };

            levelUpManager = new LevelUpManager();
            levelUpManager.Add("E > Q > E > W ", priority1);
            levelUpManager.Add("Q > E > E > W ", priority2);
            levelUpManager.Add("Q > E > W > E ", priority3);
        }*/

        private static void InitializeMainMenu()
        {
            Config = new Menu("DevCassio V2", "DevCassio V2", true).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Purple);

            var targetSelectorMenu = Config.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            {
                TargetSelector.AddToMenu(targetSelectorMenu);
            }

            var orbMenu = Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            {
                Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            }

            var comboMenu = Config.AddSubMenu(new Menu("Combo", "Combo").SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Green));
            {
                comboMenu.AddItem(new MenuItem("ComboMode", "Combo Mode:")
                    .SetValue(new StringList(new[] { "Dev Combo", "Rylai Combo" })));
                comboMenu.AddItem(new MenuItem("UseQCombo", "Use Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseWCombo", "Use W").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseECombo", "Use E").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseRCombo", "Use R").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));
                comboMenu.AddItem(new MenuItem("UseAACombo", "Use AA in Combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("Mode", "Combo Mode Explain").SetValue(true).SetTooltip("Just Here To Explain Rylai Mode - Will Cast E, And Only Use Q Hit Chance Is High AND Target Have Slow Buff"));
                /*comboMenu.AddItem(new MenuItem("ProbeltR", "Use Problet R").SetValue(false));*/
                comboMenu
                    .AddItem(new MenuItem("Rflash", "Use Flash R").SetValue(true))
                    .SetValue(new KeyBind("J".ToCharArray()[0], KeyBindType.Press));
                comboMenu.AddItem(new MenuItem("Rminflash", "Min Enemies F + R").SetValue(new Slider(2, 1, 5)).SetTooltip("Set This Up! It will affect your game play"));
                comboMenu.AddItem(new MenuItem("UseRSaveYourself", "Use R Save Yourself").SetValue(true));
                comboMenu
                    .AddItem(new MenuItem("UseRSaveYourselfMinHealth", "Use R Save MinHealth").SetValue(new Slider(25)).SetTooltip("Set This Up! It will affect your game play"));
            }

            var harassMenu = Config.AddSubMenu(new Menu("Harass", "Harass"));
            {
                harassMenu
                    .AddItem(
                        new MenuItem("HarassToggle", "Harras Active (toggle)").SetValue(new KeyBind(
                            "G".ToCharArray()[0],
                            KeyBindType.Toggle)));
                harassMenu.AddItem(new MenuItem("UseQHarass", "Use Q").SetValue(true));
                harassMenu.AddItem(new MenuItem("UseWHarass", "Use W").SetValue(false));
                harassMenu.AddItem(new MenuItem("UseEHarass", "Use E").SetValue(true));
                harassMenu.AddItem(new MenuItem("LastHitE", "LastHit E While Harass").SetValue(false));
            }

            var laneClearMenu = Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            {
                laneClearMenu.AddItem(new MenuItem("UseQLaneClear", "Use Q").SetValue(true));
                laneClearMenu.AddItem(new MenuItem("UseWLaneClear", "Use W").SetValue(false));
                laneClearMenu.AddItem(new MenuItem("UseELaneClear", "Use E").SetValue(true));
                laneClearMenu
                    .AddItem(new MenuItem("UseELastHitLaneClear", "ALWAYS Use E For LastHit").SetValue(true));
                laneClearMenu
                    .AddItem(
                        new MenuItem("UseELastHitLaneClearNonPoisoned", "Use E LastHit on Non Poisoned creeps").SetValue(
                            false));
                laneClearMenu.AddItem(new MenuItem("UseAaFarmLC", "Use AA In LaneClear").SetValue(true)).SetTooltip("Overwrite ALWAYS Use E For LastHit");
                /*laneClearMenu.AddItem(new MenuItem("FLC", "FAST LaneClear").SetValue(true));*/
                laneClearMenu
                    .AddItem(new MenuItem("LaneClearMinMana", "LaneClear Min Mana").SetValue(new Slider(25)));
            }

            var jungleClearMenu = Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            {
                jungleClearMenu.AddItem(new MenuItem("UseQJungleClear", "Use Q").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("UseEJungleClear", "Use E").SetValue(true));
            }

            var lastHitMenu = Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            {
                lastHitMenu.AddItem(new MenuItem("UseELastHit", "Use E").SetValue(true));
                lastHitMenu.AddItem(new MenuItem("UseAaFarm", "Use AA In LastHit").SetValue(true));
                lastHitMenu
                    .AddItem(new MenuItem("LastHitMinMana", "LastHit Min Mana").SetValue(new Slider(25)));
                /*lastHitMenu.AddItem(new MenuItem("AutoE", "Auto E On Killable Minion").SetValue(true));
                lastHitMenu
                    .AddItem(new MenuItem("AutoEmana", "Auto E Mana").SetValue(new Slider(25)));*/
            }

            var ultMenu = Config.AddSubMenu(new Menu("Ultimate", "Ultimate").SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Red));
            {
                ultMenu.AddItem(new MenuItem("UseAssistedUlt", "Use AssistedUlt").SetValue(true));
                ultMenu
                    .AddItem(
                        new MenuItem("AssistedUltKey", "Assisted Ult Key").SetValue(
                            new KeyBind("R".ToCharArray()[0], KeyBindType.Press)));
                ultMenu.AddItem(new MenuItem("BlockR", "Block Ult When 0 Hit").SetValue(true));
                ultMenu.AddItem(new MenuItem("UseUltUnderTower", "Ult Enemy Under Tower").SetValue(true));
                ultMenu.AddItem(new MenuItem("UltRange", "Ultimate Range").SetValue(new Slider(650, 0, 800)));
                ultMenu.AddItem(new MenuItem("RMinHit", "Min Enemies Hit").SetValue(new Slider(2, 1, 5))).SetTooltip("Set This Up! It will affect your game play");
                ultMenu.AddItem(new MenuItem("RMinHitFacing", "Min Enemies Facing").SetValue(new Slider(1, 1, 5)).SetTooltip("Set This Up! It will affect your game play"));
            }

            var gapcloserMenu = Config.AddSubMenu(new Menu("Gapcloser", "Gapcloser"));
            {
                gapcloserMenu.AddItem(new MenuItem("RAntiGapcloser", "R AntiGapcloser").SetValue(true));
                gapcloserMenu.AddItem(new MenuItem("RInterrupetSpell", "R InterruptSpell").SetValue(true));
                gapcloserMenu
                    .AddItem(new MenuItem("RAntiGapcloserMinHealth", "R AntiGapcloser Min Health").SetValue(new Slider(60)));
            }

            var miscMenu = Config.AddSubMenu(new Menu("Misc", "Misc"));
            {
                /*miscMenu
                    .AddItem(new MenuItem("PacketCast", "No-Face Exploit (PacketCast)").SetValue(true))
                    .SetTooltip("Packet Does Not Work, PLS IGNORE.");*/
                miscMenu
                    .AddItem(new MenuItem("FleeON", "Enable Flee").SetValue(true));
                miscMenu
                    .AddItem(new MenuItem("FleeK", "Flee Key")
                    .SetValue(new KeyBind("H".ToCharArray()[0], KeyBindType.Press)));
                miscMenu
                    .AddItem(new MenuItem("FleeQ", "Cast Q While Flee").SetValue(true))
                    .SetTooltip("Will Cast Q On Enemy To Gain Movement Speed.");
                miscMenu
                    .AddItem(new MenuItem("FleeW", "Cast W While Flee").SetValue(false));
                miscMenu
                    .AddItem(new MenuItem("FleeE", "Cast E While Flee").SetValue(false))
                    .SetTooltip("If You Have Rylai, Recommand On.");
                miscMenu
                    .AddItem(new MenuItem("StackTear", "Stack Tear").SetValue(false)).SetValue(new KeyBind(
                            "O".ToCharArray()[0],KeyBindType.Toggle));
            }

            var KSMenu = Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            {
                KSMenu
                    .AddItem(new MenuItem("KsOn", "Enable KS").SetValue(false));
                KSMenu
                    .AddItem(new MenuItem("QkS", "Q KS").SetValue(false));
                KSMenu
                    .AddItem(new MenuItem("EkS", "E KS").SetValue(false));
                KSMenu
                    .AddItem(new MenuItem("RkS", "R KS").SetValue(false));
            }

            var legitMenu = Config.AddSubMenu(new Menu("Im Legit! :)", "Legit"));
            {
                legitMenu
                    .AddItem(new MenuItem("PlayLegit", "Play Legit :)").SetValue(false))
                    .SetTooltip("Packet Does Not Work, PLS IGNORE");
                legitMenu.AddItem(new MenuItem("DisableNFE", "Disable No-Face Exploit").SetValue(true));
                legitMenu
                    .AddItem(new MenuItem("LegitCastDelay", "Cast E Delay").SetValue(new Slider(1000, 0, 2000)));
            }

            var AutoLevelerMenu = Config.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            { 
                AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
                AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 3)));
                AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 0)));
                AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
                AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 2)));
                AutoLevelerMenu.AddItem(new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));
            }

            var skinMenu = Config.AddSubMenu(new Menu("Skins Menu", "SkinMenu"));
            {
                skinMenu.AddItem(new MenuItem("UseSkin", "Enabled Skin Change").SetValue(true));
                skinMenu.AddItem(new MenuItem("SkinID", "Skin ID")).SetValue(new Slider(4, 0, 8));
            }

            var drawingMenu = Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            {
                drawingMenu
                    .AddItem(
                        new MenuItem("QRange", "Q Range").SetValue(new Circle(true,
                            System.Drawing.Color.FromArgb(255, 255, 255, 255))));
                drawingMenu
                    .AddItem(
                        new MenuItem("WRange", "W Range").SetValue(new Circle(false,
                            System.Drawing.Color.FromArgb(255, 255, 255, 255))));
                drawingMenu
                    .AddItem(
                        new MenuItem("ERange", "E Range").SetValue(new Circle(false,
                            System.Drawing.Color.FromArgb(255, 255, 255, 255))));
                drawingMenu
                    .AddItem(
                        new MenuItem("RRange", "R Range").SetValue(new Circle(false,
                            System.Drawing.Color.FromArgb(255, 255, 255, 255))));
                drawingMenu
                    .AddItem(
                        new MenuItem("ComboStatus", "Draw Combo Status").SetValue(true));
                drawingMenu.AddItem(new MenuItem("EDamage", "Show E Damage on HPBar").SetValue(true));
            }

            /*levelUpManager.AddToMenu(ref Config);*/

            var creditMenu = Config.AddSubMenu(new Menu("Credits", "Credits"));
            creditMenu.AddItem(new MenuItem("ME: LOVETAIWAN♥", "ME: LOVETAIWAN♥")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Pink);
            creditMenu.AddItem(new MenuItem("Soresu", "Soresu")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Yellow);
            creditMenu.AddItem(new MenuItem("Nightmoon", "Nightmoon")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.LightBlue);
            creditMenu.AddItem(new MenuItem("InjectionDev <3", "InjectionDev <3")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Orange);
            creditMenu.AddItem(new MenuItem("Exory", "Exory")).SetFontStyle(
                FontStyle.Bold, SharpDX.Color.Brown);

            

            Config.AddToMainMenu();

            Config.Item("UseSkin").ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                }
            };

            if (Config.Item("EDamage").GetValue<bool>())
            {
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.DamageToUnit += GetEDamage;
                //LeagueSharp.Common.Utility.HpBar//DamageIndicator.Enabled = true;
            }

            Config.Item("UltRange").ValueChanged += (sender, e) => { R.Range = e.GetNewValue<Slider>().Value; };
            R.Range = Config.Item("UltRange").GetValue<Slider>().Value;
        }

        private static void InitializeAttachEvents()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Orbwalking.BeforeAttack += BeforeAttack;
            /*Orbwalking.OnNonKillableMinion += OnNonKillableMinion;*/
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("UseSkin").GetValue<bool>())
            {
                //Player.SetSkin(Player.BaseSkinName, Config.Item("SkinID").GetValue<Slider>().Value);
            }

            if (Player.IsDead || Player.IsRecalling())
            {
                return;
            }

            if (Config.Item("Rflash").GetValue<KeyBind>().Active)
            {
                FlashCombo();
            }

            if (Config.Item("HarassToggle").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            if (Config.Item("FleeON").GetValue<bool>())
            {
                if (Config.Item("FleeK").GetValue<KeyBind>().Active)
                {
                    Flee();
                }
            }

            if (Config.Item("KsOn").GetValue<bool>())
            {
                KS();
            }

            if (Config.Item("StackTear").GetValue<KeyBind>().Active)
            {
                Stack();
            }
            UseUltUnderTower();
            /*levelUpManager.Update();*/
            if (Config.Item("AutoLevelUp").GetValue<bool>())
            {
                lvl1 = Config.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                lvl2 = Config.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                lvl3 = Config.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                lvl4 = Config.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    BurstCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    /*LastHit();*/
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    WaveClear();
                    JungleClear();
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        #region Auto LevelUp
        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Config.Item("AutoLevelUp").GetValue<bool>() ||
                (ObjectManager.Player.Level < Config.Item("AutoLvlStartFrom").GetValue<Slider>().Value))
                return;
            if ((lvl2 == lvl3) || (lvl2 == lvl4) || (lvl3 == lvl4))
                return;
            var delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
        }

        private static void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if ((indx == 0) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if ((indx == 1) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if ((indx == 2) && (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0))
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }
        #endregion

        /*private static void OnNonKillableMinion(AttackableUnit sender)
        {
            if (Player.IsDead || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (!Config.Item("AutoE", true).GetValue<bool>() || !E.IsReady())
            {
                return;
            }

            var minion = (Obj_AI_Minion)sender;

            if (minion != null && minion.IsValidTarget(E.Range) && minion.Health < Player.GetSpellDamage(minion, SpellSlot.E) &&
                Player.CountEnemiesInRange(700) == 0 && Player.ManaPercent >= Config.Item("AutoEmana").GetValue<Slider>().Value)
            {
                E.Cast(minion);
            }
        }*/

        private static void FlashCombo()
        {
            if (HeroManager.Enemies.Count(x => x.IsValidTarget(R.Range + R.Width + 425f)) > 0 && R.IsReady() &&
            ObjectManager.Player.Spellbook.CanUseSpell(Player.GetSpellSlot("SummonerFlash")) == SpellState.Ready)
            {
                foreach (
                    var target in
                    HeroManager.Enemies.Where(
                        x =>
                            !x.IsDead && !x.IsZombie && !x.IsDashing() && x.IsValidTarget(R.Range + R.Width + 425f) &&
                            x.Distance(Player) > R.Range))
                {
                    var flashPos = Player.Position.Extend(target.Position, 425f);
                    var rHit = GetRHitCount(flashPos);

                    if (rHit.Item1.Count >= Config.Item("Rminflash").GetValue<Slider>().Value)
                    {
                        var castPos = Player.Position.Extend(rHit.Item2, -(Player.Position.Distance(rHit.Item2) * 2));

                        if (R.Cast(castPos))
                        {
                            LeagueSharp.Common.Utility.DelayAction.Add(300 + Game.Ping / 2,
                                () =>
                                    ObjectManager.Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"),
                                        flashPos));
                            Chat.Print("Flash R Combo!");
                        }
                    }
                }
            }

            if (Orbwalking.CanMove(100))
            {
                Orbwalking.MoveTo(Game.CursorPos, 80f);

                Combo();
            }
        }

        private static void UseUltUnderTower()
        {
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var UseUltUnderTower = Config.Item("UseUltUnderTower").GetValue<bool>();

            if (UseUltUnderTower)
            {
                foreach (var eTarget in DevHelper.GetEnemyList())
                {
                    if (eTarget.IsValidTarget(R.Range) && eTarget.IsUnderEnemyTurret() && R.IsReady() && !eTarget.IsInvulnerable)
                    {
                        R.Cast(eTarget.Position);
                    }
                }
            }
        }

        private static void Flee()
        {
            Orbwalking.MoveTo(Game.CursorPos);
            var eTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (eTarget == null)
            {
                return;
            }
            var FleeW = Config.Item("FleeW").GetValue<bool>();
            if (FleeW && W.IsReady() && eTarget.IsValidTarget(W.Range) && !eTarget.IsValidTarget(225))
            {
                W.Cast(eTarget.Position);
            }

            var castPred = Q.GetPrediction(eTarget, true, Q.Range);
            var FleeQ = Config.Item("FleeQ").GetValue<bool>();
            var FleeE = Config.Item("FleeE").GetValue<bool>();

            if (FleeQ && Q.IsReady())
            {
                Q.Cast(castPred.CastPosition);
                return;
            }

            if (eTarget != null && FleeE && E.IsReady())
            {
                E.CastOnUnit(eTarget);
                return;
            }
        }

        private static void KS()
        {

            if (Q.IsReady() && Config.Item("QkS").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (Q.IsReady())
                    {
                        Q.Cast(target.Position);
                    }
            }
            if (E.IsReady() && Config.Item("EkS").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(E.Range) && x.Health < E.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (E.IsReady())
                    {
                        E.Cast(target);
                    }
            }
            if (R.IsReady() && Config.Item("RkS").GetValue<bool>())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(R.Range) && x.Health < R.GetDamage(x) && !x.HasBuff("guardianangle") && !x.IsZombie))
                    if (R.IsReady())
                    {
                        R.Cast(target.Position);
                    }
            }
        }

        private static void Combo()
        {
            var eTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (eTarget == null)
            {
                return;
            }

            var useQ = Config.Item("UseQCombo").GetValue<bool>();
            var useW = Config.Item("UseWCombo").GetValue<bool>();
            var useE = Config.Item("UseECombo").GetValue<bool>();
            var useR = Config.Item("UseRCombo").GetValue<bool>();
            var useIgnite = Config.Item("UseIgnite").GetValue<bool>();
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var RMinHit = Config.Item("RMinHit").GetValue<Slider>().Value;
            var RMinHitFacing = Config.Item("RMinHitFacing").GetValue<Slider>().Value;
            var UseRSaveYourself = Config.Item("UseRSaveYourself").GetValue<bool>();
            var UseRSaveYourselfMinHealth = Config.Item("UseRSaveYourselfMinHealth").GetValue<Slider>().Value;

            switch (Config.Item("ComboMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    //Dev Combo
                    if (eTarget.IsValidTarget(R.Range) && R.IsReady() && UseRSaveYourself)
                    {
                        if (Player.GetHealthPerc() < UseRSaveYourselfMinHealth && eTarget.IsFacing(Player) && !eTarget.IsInvulnerable)
                        {
                            R.Cast(eTarget, true);
                            if (dtLastSaveYourself + 3000 < Environment.TickCount)
                            {
                                Chat.Print("Save Yourself!");
                                dtLastSaveYourself = Environment.TickCount;
                            }
                        }
                    }

                    if (eTarget.IsValidTarget(R.Range) && R.IsReady() && useR)
                    {
                        var castPred = R.GetPrediction(eTarget, true, R.Range);
                        var enemiesHit = DevHelper.GetEnemyList().Where(x => R.WillHit(x, castPred.CastPosition) && !x.IsInvulnerable).ToList();
                        var enemiesFacing = enemiesHit.Where(x => x.IsFacing(Player)).ToList();

                        //if (mustDebug)
                        //    Chat.Print("Hit:{0} Facing:{1}", enemiesHit.Count(), enemiesFacing.Count());

                        if (enemiesHit.Count >= RMinHit && enemiesFacing.Count >= RMinHitFacing)
                        {
                            R.Cast(castPred.CastPosition);
                        }
                    }

                    if (E.IsReady() && useE)
                    {
                        var eTargetCastE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                        if (eTargetCastE != null && eTargetCastE.HasBuffOfType(BuffType.Poison))
                        {
                            var query =
                                DevHelper.GetEnemyList()
                                    .Where(x => x.IsValidTarget(E.Range) && x.HasBuffOfType(BuffType.Poison));

                            if (query.Any())
                            {
                                eTargetCastE = query.First();
                            }
                        }
                        else if (eTargetCastE != null && eTargetCastE.IsValidTarget(E.Range) && !eTargetCastE.IsZombie)
                        {
                            CastE(eTarget);
                        }

                        if (eTargetCastE != null)
                        {
                            var buffEndTime = GetPoisonBuffEndTime(eTargetCastE);

                            if (buffEndTime > Game.Time + E.Delay ||
                                Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health * 0.9)
                            {
                                CastE(eTarget);

                                if (Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health * 0.9)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (eTarget.IsValidTarget(Q.Range) && Q.IsReady() && useQ)
                    {
                        if (Q.Cast(eTarget, true) == Spell.CastStates.SuccessfullyCasted)
                        {
                            dtLastQCast = Environment.TickCount;
                        }
                    }

                    if (W.IsReady() && useW && !eTarget.IsValidTarget(275))
                    {
                        W.CastIfHitchanceEquals(eTarget, HitChance.High);
                    }

                    if (useW)
                    {
                        useW = !eTarget.HasBuffOfType(BuffType.Poison) ||
                               (!eTarget.IsValidTarget(Q.Range) && eTarget.IsValidTarget(W.Range + W.Width / 2));
                    }

                    if (W.IsReady() && useW && Environment.TickCount > dtLastQCast + Q.Delay * 1000)
                    {
                        W.CastIfHitchanceEquals(eTarget, eTarget.IsMoving ? HitChance.High : HitChance.Medium);
                    }

                    var igniteDamage = summonerSpellManager.GetIgniteDamage(eTarget) +
                                       Player.GetSpellDamage(eTarget, SpellSlot.E) * 2;

                    if (eTarget.Health < igniteDamage && E.Level > 0 && eTarget.IsValidTarget(600) &&
                        eTarget.HasBuffOfType(BuffType.Poison))
                    {
                        summonerSpellManager.CastIgnite(eTarget);
                    }
                    break;

                case 1:
                    if (E.IsReady() && useE)
                    {
                        var eTargetCastE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                        if (eTargetCastE != null && eTargetCastE.HasBuffOfType(BuffType.Poison))
                        {
                            var query =
                                DevHelper.GetEnemyList()
                                    .Where(x => x.IsValidTarget(E.Range) && x.HasBuffOfType(BuffType.Poison));

                            if (query.Any())
                            {
                                eTargetCastE = query.First();
                            }
                        }
                        else if (eTargetCastE != null && eTargetCastE.IsValidTarget(E.Range) && !eTargetCastE.IsZombie)
                        {
                            CastE(eTarget);
                        }

                        if (eTargetCastE != null)
                        {
                            var buffEndTime = GetPoisonBuffEndTime(eTargetCastE);

                            if (buffEndTime > Game.Time + E.Delay ||
                                Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health * 0.9)
                            {
                                CastE(eTarget);

                                if (Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health * 0.9)
                                {
                                    return;
                                }
                            }
                        }
                    }

                    if (W.IsReady() && useW && !eTarget.IsValidTarget(275))
                    {
                        W.CastIfHitchanceEquals(eTarget, HitChance.High);
                    }

                    if (useW)
                    {
                        useW = !eTarget.HasBuffOfType(BuffType.Poison) ||
                               (!eTarget.IsValidTarget(Q.Range) && eTarget.IsValidTarget(W.Range + W.Width / 2));
                    }

                    if (W.IsReady() && useW && Environment.TickCount > dtLastQCast + Q.Delay * 1000)
                    {
                        W.CastIfHitchanceEquals(eTarget, eTarget.IsMoving ? HitChance.High : HitChance.Medium);
                    }

                    if (eTarget.IsValidTarget(R.Range) && R.IsReady() && useR)
                    {
                        var castPred = R.GetPrediction(eTarget, true, R.Range);
                        var enemiesHit = DevHelper.GetEnemyList().Where(x => R.WillHit(x, castPred.CastPosition) && !x.IsInvulnerable).ToList();
                        var enemiesFacing = enemiesHit.Where(x => x.IsFacing(Player)).ToList();

                        //if (mustDebug)
                        //    Chat.Print("Hit:{0} Facing:{1}", enemiesHit.Count(), enemiesFacing.Count());

                        if (enemiesHit.Count >= RMinHit && enemiesFacing.Count >= RMinHitFacing)
                        {
                            R.Cast(castPred.CastPosition);
                        }
                    }

                    var castPredQ = Q.GetPrediction(eTarget, true, Q.Range);
                    if (eTarget.IsValidTarget(Q.Range) && Q.IsReady() && useQ)
                    {
                        if (!eTarget.HasBuffOfType(BuffType.Slow))
                            break;
                        if (Q.Cast(eTarget, true) == Spell.CastStates.SuccessfullyCasted && eTarget.MoveSpeed > Player.MoveSpeed || eTarget.IsDashing())
                            if(Q.CastIfHitchanceEquals(eTarget, HitChance.High))
                        {
                            R.Cast(castPredQ.CastPosition);
                            dtLastQCast = Environment.TickCount; //test this, fix block r, fix minion last hit and aa
                        }
                    }
                    break;

            }
        }

        private static void BurstCombo()
        {
            var eTarget = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (eTarget == null)
            {
                return;
            }

            var useQ = Config.Item("UseQCombo").GetValue<bool>();
            var useW = Config.Item("UseWCombo").GetValue<bool>();
            var useE = Config.Item("UseECombo").GetValue<bool>();
            var useR = Config.Item("UseRCombo").GetValue<bool>();
            var useIgnite = Config.Item("UseIgnite").GetValue<bool>();
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/

            double totalComboDamage = 0;

            if (R.IsReady())
            {
                totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.R);
                totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.Q);
                totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.E);
            }

            totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.Q);
            totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.E);
            totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.E);
            totalComboDamage += Player.GetSpellDamage(eTarget, SpellSlot.E);
            totalComboDamage += summonerSpellManager.GetIgniteDamage(eTarget);

            double totalManaCost = 0;

            if (R.IsReady())
            {
                totalManaCost += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
            }

            totalManaCost += ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;

            //if (mustDebug)
            //{
            //    Chat.Print("BurstCombo Damage {0}/{1} {2}", Convert.ToInt32(totalComboDamage), Convert.ToInt32(eTarget.Health), eTarget.Health < totalComboDamage ? "BustKill" : "Harras");
            //    Chat.Print("BurstCombo Mana {0}/{1} {2}", Convert.ToInt32(totalManaCost), Convert.ToInt32(eTarget.Mana), Player.Mana >= totalManaCost ? "Mana OK" : "No Mana");
            //}

            if (eTarget.Health < totalComboDamage && Player.Mana >= totalManaCost && !eTarget.IsInvulnerable)
            {
                if (R.IsReady() && useR && eTarget.IsValidTarget(R.Range) && eTarget.IsFacing(Player))
                {
                    if (totalComboDamage * 0.3 < eTarget.Health) // Anti R OverKill
                    {
                        //if (mustDebug)
                        //    Chat.Print("BurstCombo R");
                        if (R.Cast(eTarget) == Spell.CastStates.SuccessfullyCasted)
                        {
                            dtBurstComboStart = Environment.TickCount;
                        }
                    }
                    else
                    {
                        //if (mustDebug)
                        //    Chat.Print("BurstCombo OverKill");
                        dtBurstComboStart = Environment.TickCount;
                    }
                }
            }

            if (dtBurstComboStart + 5000 > Environment.TickCount && summonerSpellManager.IsReadyIgnite() &&
                eTarget.IsValidTarget(600))
            {
                //if (mustDebug)
                //    Chat.Print("Ignite");
                summonerSpellManager.CastIgnite(eTarget);
            }
        }

        private static void Harass()
        {
            var eTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (eTarget == null)
            {
                return;
            }

            var useQ = Config.Item("UseQHarass").GetValue<bool>();
            var useW = Config.Item("UseWHarass").GetValue<bool>();
            var useE = Config.Item("UseEHarass").GetValue<bool>();
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/

            //if (mustDebug)
            //    Chat.Print("Harass Target -> " + eTarget.BaseSkinName);

            if (E.IsReady() && useE)
            {
                var eTargetCastE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (eTargetCastE != null && eTargetCastE.HasBuffOfType(BuffType.Poison))
                {
                    // keep priority target 
                    // 其实就是先判断目标选择自动获取的目标是否中毒. 中毒的话 就保留去Attack这个目标
                    // 假如他没有中毒的话 else 去获取一个新的目标(中毒)
                }
                else
                {
                    var query =
                        DevHelper.GetEnemyList()
                            .Where(x => x.IsValidTarget(E.Range) && x.HasBuffOfType(BuffType.Poison));

                    if (query.Any())
                    {
                        eTargetCastE = query.First();
                    }
                }

                if (eTargetCastE != null)
                {
                    var buffEndTime = GetPoisonBuffEndTime(eTargetCastE);

                    if (buffEndTime > Game.Time + E.Delay ||
                        Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health*0.9)
                    {
                        CastE(eTarget);

                        if (Player.GetSpellDamage(eTargetCastE, SpellSlot.E) > eTargetCastE.Health*0.9)
                        {
                            return;
                        }
                    }
                }
            }

            if (E.IsReady() && useE)
            {
                var eTargetCastE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

                if (eTargetCastE != null && eTargetCastE.IsValidTarget(E.Range) && !eTargetCastE.IsZombie)
                {
                    CastE(eTarget);
                }
            }

            if (eTarget.IsValidTarget(Q.Range) && Q.IsReady() && useQ)
            {
                if (Q.Cast(eTarget, true) == Spell.CastStates.SuccessfullyCasted)
                {
                    dtLastQCast = Environment.TickCount;
                }
            }

            if (W.IsReady() && useW)
            {
                W.CastIfHitchanceEquals(eTarget, HitChance.High);
            }

            if (useW)
            {
                useW = !eTarget.HasBuffOfType(BuffType.Poison) ||
                       (!eTarget.IsValidTarget(Q.Range) && eTarget.IsValidTarget(W.Range + W.Width/2));
            }

            if (W.IsReady() && useW && Environment.TickCount > dtLastQCast + Q.Delay * 1000)
            {
                W.CastIfHitchanceEquals(eTarget, eTarget.IsMoving ? HitChance.High : HitChance.Medium);
            }
        }

        private static void WaveClear()
        {
            var useQ = Config.Item("UseQLaneClear").GetValue<bool>();
            var useW = Config.Item("UseWLaneClear").GetValue<bool>();
            var useE = Config.Item("UseELaneClear").GetValue<bool>();
            var UseELastHitLaneClear = Config.Item("UseELastHitLaneClear").GetValue<bool>();
            var UseELastHitLaneClearNonPoisoned = Config.Item("UseELastHitLaneClearNonPoisoned").GetValue<bool>();
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var LaneClearMinMana = Config.Item("LaneClearMinMana").GetValue<Slider>().Value;
            var orb = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear;

            if (Q.IsReady() && useQ && Player.GetManaPerc() >= LaneClearMinMana)
            {
                var allMinionsQ = MinionManager.GetMinions(Player.Position, Q.Range + Q.Width);
                var allMinionsQNonPoisoned = allMinionsQ.Where(x => !x.HasBuffOfType(BuffType.Poison));

                if (allMinionsQNonPoisoned.Any())
                {
                    var farmNonPoisoned = Q.GetCircularFarmLocation(allMinionsQNonPoisoned.ToList(), Q.Width * 0.8f);

                    if (farmNonPoisoned.MinionsHit >= 3)
                    {
                        Q.Cast(farmNonPoisoned.Position);
                        dtLastQCast = Environment.TickCount;
                        return;
                    }
                }

                if (allMinionsQ.Any())
                {
                    var farmAll = Q.GetCircularFarmLocation(allMinionsQ, Q.Width * 0.8f);

                    if (farmAll.MinionsHit >= 2 || allMinionsQ.Count == 1)
                    {
                        Q.Cast(farmAll.Position);
                        dtLastQCast = Environment.TickCount;
                        return;
                    }
                }
            }

            if (W.IsReady() && useW && Player.GetManaPerc() >= LaneClearMinMana &&
                Environment.TickCount > dtLastQCast + Q.Delay * 1000)
            {
                var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, W.Range + W.Width);
                var allMinionsWNonPoisoned = allMinionsW.Where(x => !x.HasBuffOfType(BuffType.Poison));

                if (allMinionsWNonPoisoned.Any())
                {
                    var farmNonPoisoned = W.GetCircularFarmLocation(allMinionsWNonPoisoned.ToList(), W.Width * 0.8f);

                    if (farmNonPoisoned.MinionsHit >= 3)
                    {
                        W.Cast(farmNonPoisoned.Position);
                        return;
                    }
                }

                if (allMinionsW.Any())
                {
                    var farmAll = W.GetCircularFarmLocation(allMinionsW, W.Width * 0.8f);

                    if (farmAll.MinionsHit >= 2 || allMinionsW.Count == 1)
                    {
                        W.Cast(farmAll.Position);
                        return;
                    }
                }
            }

            if (E.IsReady() && useE)
            {
                MinionList = MinionManager.GetMinions(Player.ServerPosition, E.Range);

                foreach (
                    var minion in
                    MinionList.Where(x => UseELastHitLaneClearNonPoisoned || x.HasBuffOfType(BuffType.Poison)))
                {
                    var buffEndTime = UseELastHitLaneClearNonPoisoned ? float.MaxValue : GetPoisonBuffEndTime(minion);
                    if (buffEndTime > Game.Time + E.Delay)
                    {
                        if (UseELastHitLaneClear)
                        {
                            if (ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E) * 0.9 >
                                SebbyLib.HealthPrediction.GetHealthPrediction(minion,
                                    (int)(E.Delay + (minion.Distance(ObjectManager.Player.Position) / E.Speed))))
                            {
                                CastE(minion);
                            }
                        }
                        else if (Player.GetManaPerc() >= LaneClearMinMana)
                        {
                            CastE(minion);
                        }
                    }
                    //else
                    //{
                    //    if (mustDebug)
                    //        Chat.Print("DONT CAST : buffEndTime " + buffEndTime);
                    //}
                }
            }
            /*
            var minions = Cache.GetMinions(Player.ServerPosition, E.Range);

            int orbTarget = 0;
            if (Orbwalker.GetTarget() != null)
                orbTarget = Orbwalker.GetTarget().NetworkId;

            if (UseELastHitLaneClear == true && orb && !Orbwalking.CanAttack() && Player.ManaPercent > Config.Item("LaneClearMinMana", true).GetValue<Slider>().Value)
            {
                var LCP = Config.Item("FLC", true).GetValue<bool>();

                foreach (var minion in minions.Where(minion => Orbwalker.InAutoAttackRange(minion) && orbTarget != minion.NetworkId))
                {
                    var hpPred = SebbyLib.HealthPrediction.GetHealthPrediction(minion, 300);
                    var dmgMinion = minion.GetAutoAttackDamage(minion);
                    var qDmg = E.GetDamage(minion);
                    if (hpPred < qDmg)
                    {
                        if (hpPred > dmgMinion)
                        {
                            if (E.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                                return;
                        }
                    }

                }
            }*/
        }

        private static void JungleClear()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (!mobs.Any())
            {
                return;
            }

            var UseQJungleClear = Config.Item("UseQJungleClear").GetValue<bool>();
            var UseEJungleClear = Config.Item("UseEJungleClear").GetValue<bool>();
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var mob = mobs.First();

            if (UseQJungleClear && Q.IsReady() && mob.IsValidTarget(Q.Range))
            {
                Q.Cast(mob.ServerPosition);
            }

            if (UseEJungleClear && E.IsReady() && mob.IsValidTarget(E.Range))
            {
                CastE(mob);
            }
        }

        /*private static void LastHit2()
        {
            if (!E.IsReady())
            {
                return;
            }
            if (Config.Item("LastHitE", true).GetValue<bool>())
            {
                var minions =
                    MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(
                            m =>
                                m.Health > 5 &&
                                m.Health <
                                (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit
                                    ? E.GetDamage(m)
                                    : E.GetDamage(m) * Config.Item("eLHDamage", true).GetValue<Slider>().Value / 100) &&
                                E.CanCast(m) &&
                                HealthPrediction.GetHealthPrediction(m, (int)(m.Distance(Player) / E.Speed * 1000)) > 0);
                if (minions != null && LastAttackedminiMinion != null)
                {
                    foreach (var minion in
                        minions.Where(
                            m =>
                                m.NetworkId != LastAttackedminiMinion.NetworkId ||
                                (m.NetworkId == LastAttackedminiMinion.NetworkId &&
                                 Utils.GameTimeTickCount - LastAttackedminiMinionTime > 700)))
                    {
                        if (minion.Team == GameObjectTeam.Neutral && minion.CountAlliesInRange(500) > 0 &&
                            minion.NetworkId != LastAttackedminiMinion.NetworkId)
                        {
                            continue;
                        }

                        if (minion.Distance(Player) <= Player.AttackRange && !Orbwalking.CanAttack() &&
                            Orbwalking.CanMove(100))
                        {
                            if (E.Cast(minion).IsCasted())
                            {
                                Orbwalking.MoveTo(Game.CursorPos, 80f);
                            }
                        }
                        else if (minion.Distance(Player) > Player.AttackRange)
                        {
                            if (E.Cast(minion).IsCasted())
                            {
                                Orbwalking.MoveTo(Game.CursorPos, 80f);
                            }
                        }
                    }
                }
            }
        }*/


        private static void LastHit()
        {
            var castE = Config.Item("LastHitE").GetValue<bool>() && E.IsReady();
            var LHE = Config.Item("UseELastHit").GetValue<bool>() && E.IsReady();
            var minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            if (LHE)
            {
                foreach (var minion in minions)
                {
                    if (
                        SebbyLib.HealthPrediction.GetHealthPrediction(minion,
                            (int)(E.Delay + minion.Distance(ObjectManager.Player.Position) / E.Speed)) <
                        ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E))
                    {
                        E.Cast(minion);
                    }
                }
            }
            if (castE)
            {
                foreach (var minion in minions)
                {
                    if (
                        SebbyLib.HealthPrediction.GetHealthPrediction(minion,
                            (int)(E.Delay + minion.Distance(ObjectManager.Player.Position) / E.Speed)) <
                        ObjectManager.Player.GetSpellDamage(minion, SpellSlot.E))
                    {
                        E.Cast(minion);
                    }
                }
            }

                            /*if (!castE)
                    return;

                var minions2 = Cache.GetMinions(Player.ServerPosition, E.Range);
                var orb = Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit;

                int orbTarget = 0;
                if (Orbwalker.GetTarget() != null)
                    orbTarget = Orbwalker.GetTarget().NetworkId;

                foreach (var minion in minions.Where(minion => orbTarget != minion.NetworkId && !Orbwalker.InAutoAttackRange(minion) && minion.Health < E.GetDamage(minion)))
                {
                    if (E.Cast(minion) == Spell.CastStates.SuccessfullyCasted)
                        return;
                }*/
        }

        private static void Stack()
        {
            if (Player.InFountain() ||(Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None))
                if (Items.HasItem(3004, Player) || Items.HasItem(3003, Player) || Items.HasItem(3070, Player) ||
                    Items.HasItem(3073, Player) || Items.HasItem(3008, Player))
                    Q.Cast(Player.ServerPosition);
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (MenuGUI.IsChatOpen)
            {
                return;
            }

            var UseAssistedUlt = Config.Item("UseAssistedUlt").GetValue<bool>();
            var AssistedUltKey = Config.Item("AssistedUltKey").GetValue<KeyBind>().Key;

            if (UseAssistedUlt && args.WParam == AssistedUltKey)
            {
                args.Process = false;
                CastAssistedUlt();
            }

        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var RAntiGapcloser = Config.Item("RAntiGapcloser").GetValue<bool>();
            var RAntiGapcloserMinHealth = Config.Item("RAntiGapcloserMinHealth").GetValue<Slider>().Value;

            if (RAntiGapcloser && Player.GetHealthPerc() <= RAntiGapcloserMinHealth &&
                gapcloser.Sender.IsValidTarget(R.Range) && R.IsReady() && !gapcloser.Sender.IsInvulnerable)
            {
                R.Cast(gapcloser.Sender.ServerPosition);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            /*var packetCast = Config.Item("PacketCast").GetValue<bool>();*/
            var RInterrupetSpell = Config.Item("RInterrupetSpell").GetValue<bool>();
            var RAntiGapcloserMinHealth = Config.Item("RAntiGapcloserMinHealth").GetValue<Slider>().Value;

            if (RInterrupetSpell && Player.GetHealthPerc() < RAntiGapcloserMinHealth && sender.IsValidTarget(R.Range) &&
                args.DangerLevel >= Interrupter2.DangerLevel.High)
            {
                R.CastIfHitchanceEquals(sender, sender.IsMoving ? HitChance.High : HitChance.Medium);
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            /*var enemy = Orbwalker.GetTarget() as AIHeroClient;
            var enemyr = HeroManager.Enemies.Where(x => R.WillHit(enemy, args.Target.Position));

            var query = DevHelper.GetEnemyList().Where(x => !R.WillHit(enemy, args.StartPosition));*/
            var menuItem = Config.Item("Rflash").GetValue<KeyBind>();

                if (Config.Item("BlockR").GetValue<bool>() && !menuItem.Active)
            {
                if (!sender.Owner.IsMe || args.Slot != SpellSlot.R)
                {
                    return;
                }

                if (
                    HeroManager.Enemies.Any(
                        x => x.IsValidTarget(R.Range + R.Width - 150) && !x.HasBuffOfType(BuffType.Invulnerability)))
                {
                    return;
                }

                args.Process = false;
                Chat.Print(string.Format("Ult Blocked"));
                /*if (HeroManager.Enemies.All(x => !x.IsValidTarget(R.Range) || !R.WillHit(x, args.StartPosition)) && args.Slot == SpellSlot.R)
                {
                    args.Process = false;
                    Chat.Print(string.Format("Ult Blocked"));
                }*/
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = Config.Item("UseAACombo").GetValue<bool>();

                var target = args.Target as Obj_AI_Base;

                if (target != null)
                {
                    if (E.IsReady() && target.HasBuffOfType(BuffType.Poison) && target.IsValidTarget(E.Range))
                    {
                        args.Process = false;
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                args.Process = Config.Item("UseAaFarm").GetValue<bool>();

                var target = args.Target as Obj_AI_Minion;

                if (target != null)
                {
                    if (E.IsReady() && target.HasBuffOfType(BuffType.Poison) && target.IsValidTarget(E.Range))
                    {
                        args.Process = false;
                    }
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                args.Process = Config.Item("UseAaFarmLC").GetValue<bool>();

                var target = args.Target as Obj_AI_Minion;

                if (target != null)
                {
                    if (E.IsReady() && target.HasBuffOfType(BuffType.Poison) && target.IsValidTarget(E.Range))
                    {
                        args.Process = false;
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();

                if (menuItem.Active)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range,
                        spell.IsReady() ? System.Drawing.Color.Green : System.Drawing.Color.Red);
                }
            }

            if (!Config.Item("ComboStatus").GetValue<bool>())
            { 
                return;
            }

            var Pos = Drawing.WorldToScreen(Player.Position);
            switch (Config.Item("ComboMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    Drawing.DrawText(
                        Pos.X - 25f,
                        Pos.Y + 25f,
                        System.Drawing.Color.LightGreen,
                        "Dev Combo");
                    break;
                case 1:
                    Drawing.DrawText(
                        Pos.X - 25f,
                        Pos.Y + 25f,
                        System.Drawing.Color.GreenYellow,
                        "Rylai Combo");
                    break;
            }

            //if (mustDebugPredict)
            //    DrawPrediction();
        }

        //private static void DrawPrediction()
        //{
        //    var eTarget = TargetSelector.GetTarget(Q.Range * 5, TargetSelector.DamageType.Magical);

        //    if (eTarget == null)
        //        return;

        //    var Qpredict = Q.GetPrediction(eTarget, true);
        //    Render.Circle.DrawCircle(Qpredict.CastPosition, Q.Width, Qpredict.Hitchance >= HitChance.High ? System.Drawing.Color.Green : System.Drawing.Color.Red);
        //}

        /*private static List<Vector3> PointsAroundTheTarget(Vector3 pos, float dist, float prec = 15, float prec2 = 6)//Soreus
        {
            if (!pos.IsValid())
            {
                return new List<Vector3>();
            }
            List<Vector3> list = new List<Vector3>();
            if (dist > 205)
            {
                prec = 30;
                prec2 = 8;
            }
            if (dist > 805)
            {
                dist = (float)(dist * 1.5);
                prec = 45;
                prec2 = 10;
            }
            var angle = 360 / prec * Math.PI / 180.0f;
            var step = dist * 2 / prec2;
            for (int i = 0; i < prec; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    list.Add(
                        new Vector3(
                            pos.X + (float)(Math.Cos(angle * i) * (j * step)),
                            pos.Y + (float)(Math.Sin(angle * i) * (j * step)) - 90, pos.Z));
                }
            }

            return list;
        }*/

        private static Tuple<List<AIHeroClient>, Vector3> GetRHitCount(Vector3 fromPos = default(Vector3))//SFX Challenger
        {
            if (fromPos == default(Vector3))
            {
                return new Tuple<List<AIHeroClient>, Vector3>(null, default(Vector3));
            }

            var predInput = new PredictionInput
            {
                Aoe = true,
                Collision = true,
                CollisionObjects = new [] { CollisionableObjects.YasuoWall},
                Delay = R.Delay,
                From = fromPos,
                Radius = R.Width,
                Range = R.Range,
                Speed = R.Speed,
                Type = R.Type,
                RangeCheckFrom = fromPos,
                UseBoundingRadius = true
            };

            var CastPosition = Vector3.Zero;
            var herosHit = new List<AIHeroClient>();
            var targetPosList = new List<RPosition>();

            foreach (var target in HeroManager.Enemies.Where(x => x.Distance(fromPos) <= R.Width + R.Range))
            {
                predInput.Unit = target;

                var pred = Prediction.GetPrediction(predInput);

                if (pred.Hitchance >= HitChance.High)
                {
                    targetPosList.Add(new RPosition(target, pred.UnitPosition));
                }
            }

            var circle = new Geometry.Polygon.Circle(fromPos, R.Range).Points;
            foreach (var point in circle)
            {
                var hits = new List<AIHeroClient>();

                foreach (var position in targetPosList)
                {
                    R.UpdateSourcePosition(fromPos, fromPos);

                    if (R.WillHit(position.position, point.To3D()))
                    {
                        hits.Add(position.target);
                    }

                    R.UpdateSourcePosition();
                }

                if (hits.Count > herosHit.Count)
                {
                    CastPosition = point.To3D();
                    herosHit = hits;
                }
            }

            return new Tuple<List<AIHeroClient>, Vector3>(herosHit, CastPosition);
        }

        private static void CastE(Obj_AI_Base unit)
        {
            var PlayLegit = Config.Item("PlayLegit").GetValue<bool>();
            var DisableNFE = Config.Item("DisableNFE").GetValue<bool>();
            var LegitCastDelay = Config.Item("LegitCastDelay").GetValue<Slider>().Value;

            if (PlayLegit)
            {
                if (Environment.TickCount > dtLastECast + LegitCastDelay)
                {
                    E.CastOnUnit(unit);
                    dtLastECast = Environment.TickCount;
                }
            }
            else
            {
                E.CastOnUnit(unit);
                dtLastECast = Environment.TickCount;
            }
        }

        private static void CastAssistedUlt()
        {
            var eTarget = Player.GetNearestEnemy();

            if (eTarget.IsValidTarget(R.Range) && R.IsReady())
            {
                R.Cast(eTarget.Position);
            }
        }

        private static float GetPoisonBuffEndTime(Obj_AI_Base target)
        {
            var buffEndTime = target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Type == BuffType.Poison)
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault();

            return buffEndTime;
        }

        private static float GetEDamage(AIHeroClient hero)
        {
            return (float)Player.GetSpellDamage(hero, SpellSlot.E);
        }

        private class RPosition
        {
            internal AIHeroClient target;
            internal Vector3 position;

            public RPosition(AIHeroClient hero, Vector3 pos)
            {
                target = hero;
                position = pos;
            }
        }
    }
}
