#region REFS
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Color = System.Drawing.Color;
using Collision = LeagueSharp.Common.Collision;
//using Orbwalking = Kalima.refs.Orbwalking;
#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Kalima {
    internal class Kalista {

        #region GAME LOAD
        #region INIT STUFF
        static Dictionary<Vector3, Vector3> jumpPos;
        static readonly AIHeroClient Player = ObjectManager.Player;
        static int level { get { return Player.Level; } }
        static Vector3 MyPosition { get { return Player.Position; } }
        static bool ERand {
            get {
                if (randomizeEpop && Player.ChampionsKilled > randomizeEpopminkills) { return true; }
                return false;
            }
        }
        #region SOULMATE RELATED STUFF
        //store the soulbound friend..
        static AIHeroClient soulmate { get { return HeroManager.Allies.Find(a => a.HasBuff("kalistacoopstrikeally")); } }
        static bool ValidSoul { get { if (soulmate != null && !soulmate.IsDead) { return true; };return false; } }
        static float soulmateHealthPercent { get { return soulmate.HealthPercent; } }
        static Vector3 soulmateposition { get { return soulmate.Position; } }
        static bool soulbalistador {
            get {
                var s = soulmate.ChampionName;
                if (s == "Blitzcrank" || s == "Skarner" || s == "TahmKench") { return true; }
                return false;
            }
        }
        static float soulmateRangeFromMe { get { return MyPosition.Distance(soulmate.Position); } }
        static int soulmatesignal {
            get {
                if (soulmateRangeFromMe < 800) { return 2; }
                if (soulmateRangeFromMe > 800 && soulmateRangeFromMe < R.Range) { return 1; }
                return 0;
            }
        }
        #endregion
        static Orbwalking.Orbwalker Orbwalker;
        static Menu kalimenu;
        static Menu kalm { get { return Kalista.kalimenu; } }
        static float Manapercent { get { return Player.ManaPercent; } }
        static bool spellsreadyEQ { get { if (E.IsReady() && Q.IsReady()) { return true; };return false; } }
        static bool spellsEQmana { get { if (Player.Mana > (E.ManaCost + Q.ManaCost)) { return true; } return false; } }
        static bool playerisready { get { if (Player.IsRecalling() || Player.IsDead) { return false; };return true; } }
        static bool canuseheal = false;
        static long? onupdatetimers20000 = DateTime.Now.Ticks;
        static bool onupdate20000 { get { if ((DateTime.Now.Ticks - onupdatetimers20000) > 200000000) { onupdatetimers20000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers2000 = DateTime.Now.Ticks;
        static bool onupdate2000 { get { if ((DateTime.Now.Ticks - onupdatetimers2000) > 20000000) { onupdatetimers2000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers1000 = DateTime.Now.Ticks;
        static bool onupdate1000 { get { if ((DateTime.Now.Ticks - onupdatetimers1000) > 10000000) { onupdatetimers1000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers500 = DateTime.Now.Ticks;
        static bool onupdate500 { get { if ((DateTime.Now.Ticks - onupdatetimers500) > 5000000) { onupdatetimers500 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers200 = DateTime.Now.Ticks;
        static bool onupdate200 { get { if ((DateTime.Now.Ticks - onupdatetimers200) > 2000000) { onupdatetimers200 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers100 = DateTime.Now.Ticks;
        static bool onupdate100 { get { if ((DateTime.Now.Ticks - onupdatetimers100) > 1000000) { onupdatetimers100 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? onupdatetimers50 = DateTime.Now.Ticks;
        static bool onupdate50 { get { if ((DateTime.Now.Ticks - onupdatetimers50) > 500000) { onupdatetimers50 = DateTime.Now.Ticks; return true; }; return false; } }

        static long? ondrawtimers20000 = DateTime.Now.Ticks;
        static bool ondraw20000 { get { if ((DateTime.Now.Ticks - ondrawtimers20000) > 200000000) { ondrawtimers20000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers2000 = DateTime.Now.Ticks;
        static bool ondraw2000 { get { if ((DateTime.Now.Ticks - ondrawtimers2000) > 20000000) { ondrawtimers2000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers1000 = DateTime.Now.Ticks;
        static bool ondraw1000 { get { if ((DateTime.Now.Ticks - ondrawtimers1000) > 10000000) { ondrawtimers1000 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers500 = DateTime.Now.Ticks;
        static bool ondraw500 { get { if ((DateTime.Now.Ticks - ondrawtimers500) > 5000000) { ondrawtimers500 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers200 = DateTime.Now.Ticks;
        static bool ondraw200 { get { if ((DateTime.Now.Ticks - ondrawtimers200) > 2000000) { ondrawtimers200 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers100 = DateTime.Now.Ticks;
        static bool ondraw100 { get { if ((DateTime.Now.Ticks - ondrawtimers100) > 1000000) { ondrawtimers100 = DateTime.Now.Ticks; return true; }; return false; } }
        static long? ondrawtimers50 = DateTime.Now.Ticks;
        static bool ondraw50 { get { if ((DateTime.Now.Ticks - ondrawtimers50) > 500000) { ondrawtimers50 = DateTime.Now.Ticks; return true; }; return false; } }


        static Spell Q, W, E, R;
        static int MyLevel = 0;
        static Items.Item botrk = new Items.Item(3153, 550);
        static Items.Item mercurial = new Items.Item(3139, 0f);//debuff
        static Items.Item dervish = new Items.Item(3137, 0f);//debuff
        static Items.Item qss = new Items.Item(3140, 0f);//debuff
        static SpellSlot summHeal;
        #endregion //INIT STUFF

        static void Game_OnGameLoad(EventArgs args) {//"1 3 1 2 1 4 1 3 1 3 4 3 3 2 2 4 2 2";
            if (Player.ChampionName != "Kalista") { return; }
            Q = new Spell(SpellSlot.Q, 1150f);
            Q.SetSkillshot(0.25f, 40f, 1700f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1200f);
            R = new Spell(SpellSlot.R, 1400f);

            menuload();
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            //time based ondraws...
            Drawing.OnDraw += DraWing.Drawing_OnDraw;
            //AIHeroClient.OnProcessSpellCast += Event_OnPreProcessSpellCast;
            Orbwalking.OnNonKillableMinion += Event_OnNonKillableMinion;
            Orbwalking.BeforeAttack += Event_OnBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += Event_OnEnemyGapcloser;
            FillPositions();
            summHeal = Player.GetSpellSlot("summonerheal");
        }

        static void Main(string[] args) { CustomEvents.Game.OnGameLoad += Game_OnGameLoad; }
        #endregion //GAME LOAD

        #region MENU

        static void menuload() {
            kalimenu = new Menu("Kalimá", Player.ChampionName, true);
            Menu OrbwalkerMenu = kalimenu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);
            #region MENU/SUBMENU CREATION
            Menu haraM = kalimenu.AddSubMenu(new Menu("Harass", "Harass"));
            Menu LaneM = kalimenu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            Menu JungM = kalimenu.AddSubMenu(new Menu("Jungle Clear", "Jungleclear"));
            Menu MiscM = kalimenu.AddSubMenu(new Menu("Misc", "Misc"));
            Menu ItemM = kalimenu.AddSubMenu(new Menu("Items", "Items"));
            Menu DrawM = kalimenu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Menu BotrkM = ItemM.AddSubMenu(new Menu("Botrk", "Botrk"));
            Menu Debuffs = ItemM.AddSubMenu(new Menu("Debuffs", "Debuffs"));
            Menu Debuffspells = Debuffs.AddSubMenu(new Menu("Debuff Spells", "Debuff Spells"));
            Menu AutoW = MiscM.AddSubMenu(new Menu("Auto W", "Auto W"));
            #endregion
            #region HARASS SUBMENU
            haraM.AddItem(new MenuItem("harassQ", "Use Q", true).SetValue(true));
            haraM.AddItem(new MenuItem("harassQchance", "Q cast if Chance of hit is:", true).SetValue(new Slider(2, 1, 4)));
            haraM.AddItem(new MenuItem("harassmanaminQ", "Q requires % mana", true).SetValue(new Slider(75, 0, 100)));
            haraM.AddItem(new MenuItem("harassQsavemanaforE", "Use Q only if there's mana for E", true).SetValue(true));
            haraM.AddItem(new MenuItem("harassuseE", "Use E", true).SetValue(true));
            haraM.AddItem(new MenuItem("harassEoutOfRange", "Use E when out of range", true).SetValue(true));
            haraM.AddItem(new MenuItem("harassEoutOfRangeMinSpears", "Out of Range requires X spears", true).SetValue(new Slider(3, 1, 10)));
            haraM.AddItem(new MenuItem("harassEThroughMinions", "Harass Through Minions", true).SetValue(true));
            haraM.AddItem(new MenuItem("harassEThroughMinionsMinMinions", "Minimum minions to E champion", true).SetValue(new Slider(1, 1, 10)));
            haraM.AddItem(new MenuItem("harassEMinionminhealth", "E requires minion % health to prevent E cooldown", true).SetValue(new Slider(7, 1, 50)));
            haraM.AddItem(new MenuItem("harassmanaminE", "E requires % mana", true).SetValue(new Slider(60, 0, 100)));
            haraM.AddItem(new MenuItem("harassActive", "Active", true).SetValue(true));
            #endregion
            #region JUNGLE SUBMENU
            JungM.AddItem(new MenuItem("jungleclearQ", "Use Q", true).SetValue(true));
            JungM.AddItem(new MenuItem("jungleclearQMaxdistance", "Q Max distance from jungle minion", true).SetValue(new Slider(250, 0, 1150)));
            JungM.AddItem(new MenuItem("jungleclearQmana", "Q requires % mana", true).SetValue(new Slider(30, 0, 100)));
            JungM.AddItem(new MenuItem("jungleclearSaveManaForE", "Save mana for E", true).SetValue(true));
            JungM.AddItem(new MenuItem("jungleclearE", "Use E", true).SetValue(true));
            JungM.AddItem(new MenuItem("jungleclearEmana", "E requires % mana", true).SetValue(new Slider(40, 0, 100)));
            JungM.AddItem(new MenuItem("jungleclearPopdragbaron", "Pop E on Dragon/Baron?", true).SetValue(true));
            JungM.AddItem(new MenuItem("jungleclearQdragBaron", "Use Q on dragon/baron?", true).SetValue(true));
            JungM.AddItem(new MenuItem("jungleActive", "Active", true).SetValue(true));
            #endregion
            #region LANECLEAR SUBMENU
            LaneM.AddItem(new MenuItem("laneclearQ", "Use Q", true).SetValue(true));
            LaneM.AddItem(new MenuItem("laneclearQminMinions", "Q cast if minions >= X", true).SetValue(new Slider(2, 1, 10)));
            LaneM.AddItem(new MenuItem("laneclearmanaminQ", "Q requires % mana", true).SetValue(new Slider(65, 0, 100)));
            LaneM.AddItem(new MenuItem("laneclearSaveManaForE", "Save Mana for E", true).SetValue(true));
            LaneM.AddItem(new MenuItem("laneclearE", "Use E", true).SetValue(true));
            LaneM.AddItem(new MenuItem("laneclearEMinMinions", "Minimum Minions to pop E", true).SetValue(new Slider(2, 0, 10)));
            LaneM.AddItem(new MenuItem("laneclearEMinIncrease", "Increase number by Level (decimal):", true).SetValue(new Slider(2, 0, 4)));
            LaneM.AddItem(new MenuItem("laneclearEminhealth", "E req minion % health to prevent E cooldown", true).SetValue(new Slider(7, 1, 50)));
            LaneM.AddItem(new MenuItem("laneclearmanaminE", "E requires % mana", true).SetValue(new Slider(55, 0, 100)));
            LaneM.AddItem(new MenuItem("laneclearbigminionsE", "E when it can kill siege/super minions", true).SetValue(true));
            LaneM.AddItem(new MenuItem("laneclearBigMinionsMinMana", "Big minions require % mana", true).SetValue(new Slider(45, 1, 100)));
            LaneM.AddItem(new MenuItem("laneclearEnonAAkillable", "E when non-killable by AA", true).SetValue(false));
            #endregion
            #region BOTRK SUBMENU OPTIONS
            BotrkM.AddItem(new MenuItem("botrkKS", "Use when target has < x% health + Q+E(dmg)", true).SetValue(new Slider(70, 10, 100)));
            BotrkM.AddItem(new MenuItem("botrkmyheal", "Use when my health is at: < x%", true).SetValue(new Slider(40, 0, 100)));
            BotrkM.AddItem(new MenuItem("botrkactive", "Active", true).SetValue(true));
            #endregion
            #region DEBUFF SUBMENU
            Debuffs.AddItem(new MenuItem("debuffitems", "Supports QSS/Mercurial/Dervish"));
            Debuffs.AddItem(new MenuItem("debuffActive", "Active", true).SetValue(true));

            Debuffspells.AddItem(new MenuItem("debuff_blind", "Blind", true).SetValue(false));
            Debuffspells.AddItem(new MenuItem("debuff_rocketgrab2", "Blitz Grab", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_charm", "Charm", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_dehancer", "Dehancer", true).SetValue(false));
            Debuffspells.AddItem(new MenuItem("debuff_dispellExhaust", "Exhaust", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_fear", "Fear", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_flee", "Flee", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_polymorph", "Polymorph", true).SetValue(false));
            Debuffspells.AddItem(new MenuItem("debuff_snare", "Snare", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_suppression", "Suppression", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_stun", "Stun", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_silence", "Silence", true).SetValue(false));
            Debuffspells.AddItem(new MenuItem("debuff_taunt", "Taunt", true).SetValue(true));
            Debuffspells.AddItem(new MenuItem("debuff_zedultexecute", "Zed Ult", true).SetValue(true));
            #endregion
            #region AUTO W SUBMENU
            AutoW.AddItem(new MenuItem("autoW", "Auto W (Toggle)", true).SetValue(true));
            AutoW.AddItem(new MenuItem("autoWmana", "Min Mana for AutoW", true).SetValue(new Slider(60, 1, 100)));
            AutoW.AddItem(new MenuItem("autoWKey", "Auto W HotKey").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Press)));
            AutoW.AddItem(new MenuItem("autowenemyisntnear", "Dont Send W with an enemy in X Range:", true).SetValue(new Slider(2000, 0, 5000)));
            AutoW.AddItem(new MenuItem("autowsentinelisntnear", "Dont Send W with a sentinel in X Range of it:", true).SetValue(new Slider(1500, 500, 5000)));
            AutoW.AddItem(new MenuItem("autowspottooclosetome", "Dont Send W with me in X Range of spot:", true).SetValue(new Slider(1500, 500, 5000)));

            Menu AutoWSpots = AutoW.AddSubMenu(new Menu("Auto W Spots", "Auto W Spots"));
            AutoWSpots.AddItem(new MenuItem("Blue_Camp_Blue_Buff", "Blue Camp Blue Buff", true).SetValue(true));
            AutoWSpots.AddItem(new MenuItem("Blue_Camp_Red_Buff", "Blue Camp Red Buff", true).SetValue(false));
            AutoWSpots.AddItem(new MenuItem("Red_Camp_Blue_Buff", "Red Camp Blue Buff", true).SetValue(true));
            AutoWSpots.AddItem(new MenuItem("Red_Camp_Red_Buff", "Red Camp Red Buff", true).SetValue(true));
            AutoWSpots.AddItem(new MenuItem("AutoWDragon", "Dragon", true).SetValue(true));
            AutoWSpots.AddItem(new MenuItem("AutoWBaron", "Baron", true).SetValue(true));
            AutoWSpots.AddItem(new MenuItem("Mid_Bot_River", "Mid Bot River", true).SetValue(true));
            #endregion
            #region MISC SUBMENU
            MiscM.AddItem(new MenuItem("AutoLevel", "Auto Level Skills", true).SetValue(true));
            MiscM.AddItem(new MenuItem("autoresetAA", "Auto Reset AA", true).SetValue(true));
            MiscM.AddItem(new MenuItem("useheal", "Use heal to save myself", true).SetValue(true));
            MiscM.AddItem(new MenuItem("usehealat", "Use heal when health < %", true).SetValue(new Slider(20, 0, 100)));
            MiscM.AddItem(new MenuItem("killsteal", "Kill Steal", true).SetValue(true));
            MiscM.AddItem(new MenuItem("randomizeEpop", "Look more human by failing E pop on kill?", true).SetValue(true));
            MiscM.AddItem(new MenuItem("randomizeEpopminkills", "Min kills to randomize E pop", true).SetValue(new Slider(3, 1, 50)));
            MiscM.AddItem(new MenuItem("gapcloserQ", "use Q on GapCloser", true).SetValue(true));
            MiscM.AddItem(new MenuItem("gapcloserE", "use E on GapCloser", true).SetValue(true));
            MiscM.AddItem(new MenuItem("savesoulbound", "Save Soulbound (With R)", true).SetValue(true));
            MiscM.AddItem(new MenuItem("savesoulboundat", "Save when health < %", true).SetValue(new Slider(25, 0, 100)));
            MiscM.AddItem(new MenuItem("popEbeforedying", "Pop E before dying", true).SetValue(true));
            MiscM.AddItem(new MenuItem("fleeKey", "Flee/Wall Jump").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            #endregion
            #region DRAWINGS SUBMENU
            DrawM.AddItem(new MenuItem("drawAA", "Auto Attack Range").SetValue(new Circle(false, Color.FromArgb(207, 207, 23))));
            DrawM.AddItem(new MenuItem("drawjumpspots", "Jump Spots").SetValue(new Circle(false, Color.FromArgb(0, 0, 255))));
            DrawM.AddItem(new MenuItem("drawQ", "Q Range").SetValue(new Circle(false, Color.FromArgb(43, 255, 0))));
            DrawM.AddItem(new MenuItem("drawW", "W Range").SetValue(new Circle(false, Color.FromArgb(0, 0, 255))));
            DrawM.AddItem(new MenuItem("drawE", "E Range").SetValue(new Circle(false, Color.FromArgb(57, 138, 204))));
            DrawM.AddItem(new MenuItem("drawR", "R Range").SetValue(new Circle(false, Color.FromArgb(19, 154, 161))));
            //marksman...
            DrawM.AddItem(new MenuItem("drawEdmg", "Draw E dmg HPbar").SetValue(new Circle(true, Color.FromArgb(0, 138, 184))));
            DrawM.AddItem(new MenuItem("drawEspearsneeded", "Draw E# Spears Needed").SetValue(new Circle(false, Color.FromArgb(255, 140, 0))));
            DrawM.AddItem(new MenuItem("drawsoulmatelink", "Draw Link Signal", true).SetValue(true));
            DrawM.AddItem(new MenuItem("drawcoords", "Draw Map Coords", true).SetValue(false));
            #endregion
            #region BALISTA SUBMENU
            var BalistaChamp = HeroManager.Allies.Find(x => x.ChampionName == "Blitzcrank" || x.ChampionName == "Skarner" || x.ChampionName == "TahmKench");
            if (BalistaChamp != null) {
                Menu balista = kalimenu.AddSubMenu(new Menu("Balista", "Balista"));
                Menu targetselect = balista.AddSubMenu(new Menu("Target Selector", "Target Selector"));

                Menu champselect = balista.AddSubMenu(new Menu("Drawings", "Drawings"));
                champselect.AddItem(new MenuItem("balistadrawminrange", "Min Range", true).SetValue(false));
                champselect.AddItem(new MenuItem("balistadrawmaxrange", "Max Range", true).SetValue(false));
                champselect.AddItem(new MenuItem("balistadrawlineformat", "Line Range", true).SetValue(true));

                foreach (var enemy in HeroManager.Enemies.FindAll(x => x.IsEnemy)) {
                    targetselect.AddItem(new MenuItem("balistaTarget" + enemy.ChampionName, enemy.ChampionName).SetValue(true));
                    targetselect.AddItem(new MenuItem("balistaminhealth" + enemy.ChampionName, "Min Health to Ult", true).SetValue(new Slider(200, 1, 1000)));
                }
                if (BalistaChamp.ChampionName == "Blitzcrank") {
                    balista.AddItem(new MenuItem("balistaMinRangeSoulFromEnemy", "Min Range enemy from Soulmate", true).SetValue(new Slider(300, 300, 925)));
                }
                balista.AddItem(new MenuItem("balistaMinRangeMeFromSoul", "Min Range me from Soulmate", true).SetValue(new Slider(450, 450, 1400)));
                balista.AddItem(new MenuItem("balistaMaxRangeMeFromSoul", "Max Range me from soulmate", true).SetValue(new Slider(1400, 500, 1400)));
                balista.AddItem(new MenuItem("balistaMinRangeMeFromEnemy", "Min Range me from Enemy", true).SetValue(new Slider(1400, 500, 1400)));
                balista.AddItem(new MenuItem("balistaMaxRangeMeFromEnemy", "Max Range me from enemy", true).SetValue(new Slider(2300, 500, 2300)));
                balista.AddItem(new MenuItem("balistaActive", "Active", true).SetValue(true));
            
            }
            #endregion
            kalimenu.AddToMainMenu();
        }

        //menu values into vars
        #region HARASS MENU VARS
        static bool harassQ { get { return kalm.Item("harassQ", true).GetValue<Boolean>(); } }
        static float harassmanaminQ { get { return kalm.Item("harassmanaminQ", true).GetValue<Slider>().Value; } }
        static float harassQchance { get { return kalm.Item("harassQchance", true).GetValue<Slider>().Value; } }
        static bool harassQsavemanaforE { get { return kalm.Item("harassQsavemanaforE", true).GetValue<Boolean>(); } }
        static bool harassE { get { return kalm.Item("harassuseE", true).GetValue<Boolean>(); } }
        static float harassmanaminE { get { return kalm.Item("harassmanaminE", true).GetValue<Slider>().Value; } }
        static bool harassEoutOfRange { get { return kalm.Item("harassEoutOfRange", true).GetValue<Boolean>(); } }
        static float harassEoutOfRangeMinSpears { get { return kalm.Item("harassEoutOfRangeMinSpears", true).GetValue<Slider>().Value; } }
        static bool harassEThroughMinions { get { return kalm.Item("harassEThroughMinions", true).GetValue<Boolean>(); } }
        static float harassEThroughMinionsMinMinions { get { return kalm.Item("harassEThroughMinionsMinMinions", true).GetValue<Slider>().Value; } }
        static float harassEMinionminhealth { get { return kalm.Item("harassEMinionminhealth", true).GetValue<Slider>().Value; } }
        static bool harassActive { get { return kalm.Item("harassActive", true).GetValue<Boolean>(); } }
        #endregion
        #region JUNGLE MENU VARS
        static bool jungleclearQ { get { return kalm.Item("jungleclearQ", true).GetValue<Boolean>(); } }
        static float jungleclearQMaxdistance { get { return kalm.Item("jungleclearQMaxdistance", true).GetValue<Slider>().Value; } }
        static float jungleclearQmana { get { return kalm.Item("jungleclearQmana", true).GetValue<Slider>().Value; } }
        static bool jungleclearSaveManaForE { get { return kalm.Item("jungleclearSaveManaForE", true).GetValue<Boolean>(); } }
        static bool jungleclearE { get { return kalm.Item("jungleclearE", true).GetValue<Boolean>(); } }
        static float jungleclearEmana { get { return kalm.Item("jungleclearEmana", true).GetValue<Slider>().Value; } }
        static bool jungleclearPopdragbaron { get { return kalm.Item("jungleclearPopdragbaron", true).GetValue<Boolean>(); } }
        static bool jungleclearQdragBaron { get { return kalm.Item("jungleclearQdragBaron", true).GetValue<Boolean>(); } }
        static bool jungleActive { get { return kalm.Item("jungleActive", true).GetValue<Boolean>(); } }


        #endregion
        #region LANECLEAR MENU VARS
        static bool laneclearQ { get { return kalm.Item("laneclearQ", true).GetValue<Boolean>(); } }
        static bool laneclearQmana { get { if (laneclearmanaminQ > Manapercent) { return true; };return false; } }
        static float laneclearQminMinions { get { return kalm.Item("laneclearQminMinions", true).GetValue<Slider>().Value; } }
        static float laneclearmanaminQ { get { return kalm.Item("laneclearmanaminQ", true).GetValue<Slider>().Value; } }
        static bool laneclearSaveManaForE { get { return kalm.Item("laneclearSaveManaForE", true).GetValue<Boolean>(); } }
        static bool laneclearE { get { return kalm.Item("laneclearE", true).GetValue<Boolean>(); } }
        static bool laneclearEmana { get { if (Manapercent >= laneclearmanaminE) { return true; };return false; } }
        static float laneclearEMinMinions { get { return kalm.Item("laneclearEMinMinions", true).GetValue<Slider>().Value; } }
        static float laneclearEMinIncrease { get { return kalm.Item("laneclearEMinIncrease", true).GetValue<Slider>().Value; } }
        static float laneclearEminhealth { get { return kalm.Item("laneclearEminhealth", true).GetValue<Slider>().Value; } }
        static float laneclearmanaminE { get { return kalm.Item("laneclearmanaminE", true).GetValue<Slider>().Value; } }
        static bool laneclearbigminionsE { get { return kalm.Item("laneclearbigminionsE", true).GetValue<Boolean>(); } }
        static float laneclearBigMinionsMinMana { get { return kalm.Item("laneclearBigMinionsMinMana", true).GetValue<Slider>().Value; } }
        static bool laneclearEnonAAkillable { get { return kalm.Item("laneclearEnonAAkillable", true).GetValue<Boolean>(); } }

        static double laneclearEminincrminions { get { return Math.Round(laneclearEMinMinions + (Player.Level * laneclearEMinIncrease / 10), 0, MidpointRounding.ToEven); } }
        #endregion
        #region BOTRK MENU VARS
        static float botrkKS { get { return kalm.Item("botrkKS", true).GetValue<Slider>().Value; } }
        static float botrkmyheal { get { return kalm.Item("botrkmyheal", true).GetValue<Slider>().Value; } }
        static bool botrkactive { get { return kalm.Item("botrkactive", true).GetValue<Boolean>(); } }
        #endregion
        #region DEBUFF MENU VARS
        static bool debuffActive { get { return kalm.Item("debuffActive", true).GetValue<Boolean>(); } }
        static bool debuffBlind { get { return kalm.Item("debuff_blind", true).GetValue<Boolean>(); } }
        static bool debuffBlitzGrab { get { return kalm.Item("debuff_rocketgrab2", true).GetValue<Boolean>(); } }
        static bool debuffCharm { get { return kalm.Item("debuff_charm", true).GetValue<Boolean>(); } }
        static bool debuffDehancer { get { return kalm.Item("debuff_dehancer", true).GetValue<Boolean>(); } }
        static bool debuffExhaust { get { return kalm.Item("debuff_dispellExhaust", true).GetValue<Boolean>(); } }
        static bool debuffFear { get { return kalm.Item("debuff_fear", true).GetValue<Boolean>(); } }
        static bool debuffFlee { get { return kalm.Item("debuff_flee", true).GetValue<Boolean>(); } }
        static bool debuffPolymorph { get { return kalm.Item("debuff_polymorph", true).GetValue<Boolean>(); } }
        static bool debuffSnare { get { return kalm.Item("debuff_snare", true).GetValue<Boolean>(); } }
        static bool debuffSuppression { get { return kalm.Item("debuff_suppression", true).GetValue<Boolean>(); } }
        static bool debuffStun { get { return kalm.Item("debuff_stun", true).GetValue<Boolean>(); } }
        static bool debuffSilence { get { return kalm.Item("debuff_silence", true).GetValue<Boolean>(); } }
        static bool debuffTaunt { get { return kalm.Item("debuff_taunt", true).GetValue<Boolean>(); } }
        static bool debuffZed { get { return kalm.Item("debuff_zedultexecute", true).GetValue<Boolean>(); } }
        #endregion
        #region AUTO W MENU VARS
        static bool autoW { get { return kalm.Item("autoW", true).GetValue<Boolean>(); } }
        static float autoWmana { get { return kalm.Item("autoWmana", true).GetValue<Slider>().Value; } }
        static bool autoWKey { get { return kalm.Item("autoWKey").GetValue<KeyBind>().Active; } }
        static float autowenemyisntnear { get { return kalm.Item("autowenemyisntnear", true).GetValue<Slider>().Value; } }
        static float autowsentinelisntnear { get { return kalm.Item("autowsentinelisntnear", true).GetValue<Slider>().Value; } }
        static float autowSpotTooCloseToMe { get { return kalm.Item("autowspottooclosetome", true).GetValue<Slider>().Value; } }
        static bool autoWBlueBlueActive { get { return kalm.Item("Blue_Camp_Blue_Buff", true).GetValue<Boolean>(); } }
        static bool autoWBlueRedActive { get { return kalm.Item("Blue_Camp_Red_Buff", true).GetValue<Boolean>(); } }
        static bool autoWRedBlueActive { get { return kalm.Item("Red_Camp_Blue_Buff", true).GetValue<Boolean>(); } }
        static bool autoWDragonActive { get { return kalm.Item("AutoWDragon", true).GetValue<Boolean>(); } }
        static bool autoWBaronActive { get { return kalm.Item("AutoWBaron", true).GetValue<Boolean>(); } }
        static bool autoWRiverActive { get { return kalm.Item("Mid_Bot_River", true).GetValue<Boolean>(); } }
        #endregion
        #region MISC MENU VARS
        static bool AutoLevel { get { return kalm.Item("AutoLevel", true).GetValue<Boolean>(); } }
        static bool autoresetAA { get { return kalm.Item("autoresetAA", true).GetValue<Boolean>(); } }
        static bool useheal { get { return kalm.Item("useheal", true).GetValue<Boolean>(); } }
        static float usehealat { get { return kalm.Item("usehealat", true).GetValue<Slider>().Value; } }
        static bool killsteal { get { return kalm.Item("killsteal", true).GetValue<Boolean>(); } }
        static bool randomizeEpop { get { return kalm.Item("randomizeEpop", true).GetValue<Boolean>(); } }
        static float randomizeEpopminkills { get { return kalm.Item("randomizeEpopminkills", true).GetValue<Slider>().Value; } }
        static bool gapcloserQ { get { return kalm.Item("gapcloserQ", true).GetValue<Boolean>(); } }
        static bool gapcloserE { get { return kalm.Item("gapcloserE", true).GetValue<Boolean>(); } }
        static bool savesoulbound { get { return kalm.Item("savesoulbound", true).GetValue<Boolean>(); } }
        static float savesoulboundat { get { return kalm.Item("savesoulboundat", true).GetValue<Slider>().Value; } }
        static bool popEbeforedying { get { return kalm.Item("popEbeforedying", true).GetValue<Boolean>(); } }
        static bool fleeKey { get { return kalm.Item("fleeKey").GetValue<KeyBind>().Active; } }
        #endregion
        #region DRAWINGS MENU VARS
        static Circle drawAA { get { return kalm.Item("drawAA").GetValue<Circle>(); } }
        static Circle drawJumpSpots { get { return kalm.Item("drawjumpspots").GetValue<Circle>(); } }
        static Circle drawQ { get { return kalm.Item("drawQ").GetValue<Circle>(); } }
        static Circle drawW { get { return kalm.Item("drawW").GetValue<Circle>(); } }
        static Circle drawE { get { return kalm.Item("drawE").GetValue<Circle>(); } }
        static Circle drawR { get { return kalm.Item("drawR").GetValue<Circle>(); } }
        static Circle drawEdmg { get { return kalm.Item("drawEdmg").GetValue<Circle>(); } }
        static Circle drawEspearsneeded { get { return kalm.Item("drawEspearsneeded").GetValue<Circle>(); } }
        static bool drawsoulmatelink { get { return kalm.Item("drawsoulmatelink", true).GetValue<Boolean>(); } }
        static bool drawcoords { get { return kalm.Item("drawcoords", true).GetValue<Boolean>(); } }
        #endregion
        #region BALISTA MENU VARS
        static bool balistaActive { get { return kalm.Item("balistaActive", true).GetValue<Boolean>(); } }
        static bool balistaDrawMinRange { get { return kalm.Item("balistadrawminrange", true).GetValue<Boolean>(); } }
        static bool balistaDrawMaxRange { get { return kalm.Item("balistadrawmaxrange", true).GetValue<Boolean>(); } }
        static bool balistaDrawLines { get { return kalm.Item("balistadrawlineformat", true).GetValue<Boolean>(); } }
        static float balistaMinRangeMeFromSoul { get { return kalm.Item("balistaMinRangeMeFromSoul", true).GetValue<Slider>().Value; } }
        static float balistaMaxRangeMeFromSoul { get { return kalm.Item("balistaMaxRangeMeFromSoul", true).GetValue<Slider>().Value; } }
        static float balistaMinRangeMeFromEnemy { get { return kalm.Item("balistaMinRangeMeFromEnemy", true).GetValue<Slider>().Value; } }
        static float balistaMaxRangeMeFromEnemy { get { return kalm.Item("balistaMaxRangeMeFromEnemy", true).GetValue<Slider>().Value; } }
        static float balistaMinRangeSoulFromEnemy { get { return kalm.Item("balistaMinRangeSoulFromEnemy", true).GetValue<Slider>().Value; } }
        #endregion
        #endregion

        #region EVENT GAME ON UPDATE
        static void Game_OnUpdate(EventArgs args) {
            if (Player.IsRecalling() || Player.IsDead) { return; }
            if (onupdate20000) {
            }
            if (onupdate2000) {
            }
            if (onupdate1000) {
                if (Player.Level >= MyLevel) { Event_OnLevelUp(); }
                if (autoW || kalm.Item("autoWKey").GetValue<KeyBind>().Active) {
                    AutoW();
                }
            }
            if (onupdate500) {

            }
            if (onupdate200) {
                if (fleeKey) {
                    ShowjumpsandFlee();
                }
                //buffadd just sucks and adds buffs too late..not feasible for balista or debuffs....
                debuff();
                var closebyenemy = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
                if (closebyenemy != null) {
                    Event_OnItems(closebyenemy);
                }
            }
            if (onupdate100) {
                if (killsteal) {
                    Killsteal();
                }
                if (harassActive || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) {
                    harass();
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit) {
                    laneclear();
                }
            }
            if (onupdate50) {
                if (jungleActive) {
                    Jungleclear();
                }
            }
        }

        static void Event_OnLevelUp() {
            if (kalm.Item("AutoLevel", true).GetValue<Boolean>()) {
                DraWing.drawtext("levelupspells", 3, Drawing.Width * 0.45f, Drawing.Height * 0.90f, Color.PapayaWhip, "Levelling up Spells");
                if (MyLevel == 0) {
                    Player.Spellbook.LevelUpSpell(SpellSlot.W);
                    MyLevel++;
                } else {
                    if (MyLevel == 3) { Player.Spellbook.LevelUpSpell(SpellSlot.Q); }
                    Player.Spellbook.LevelUpSpell(SpellSlot.R);
                    Player.Spellbook.LevelUpSpell(SpellSlot.E);
                    Player.Spellbook.LevelUpSpell(SpellSlot.W);
                    Player.Spellbook.LevelUpSpell(SpellSlot.Q);
                    MyLevel++;
                }
            }
        }

        #endregion

        #region EVENT DRAWING ONDRAW
        static void Drawing_OnDraw(EventArgs args) {
            if (Player.IsRecalling() || Player.IsDead) { return; }
            if (ondraw20000) {
            }
            if (ondraw2000) {
            }
            if (ondraw1000) {
                if (drawsoulmatelink && ValidSoul) {
                    draw_soulmate_link();
                }
            }
            if (ondraw500) {
                if (drawJumpSpots.Active) {
                    draw_jump_spots();
                }
            }
            if (ondraw200) {
            }
            if (ondraw100) {
            }
            if (ondraw50) {
            }
            //realtime
            if (drawEdmg.Active && E.Level > 0) {
                var enemieswithspears = HeroManager.Enemies.Where(x => x.HasBuff("kalistaexpungemarker") && x.IsHPBarRendered);
                if (enemieswithspears != null) {
                    var barsize = 104f;
                    foreach (var enemy in enemieswithspears) {
                        var health = enemy.Health;
                        var maxhealth = enemy.MaxHealth;
                        var pos = enemy.HPBarPosition;
                        var percent = GetEDamage(enemy) / maxhealth * barsize;
                        var start = pos + (new Vector2(10f, 19f));
                        var end = pos + (new Vector2(10f + percent, 19f));

                        DraWing.drawline("drawEdmg" + enemy.ChampionName, 0.1, start[0], start[1], end[0], end[1], 4.0f, drawEdmg.Color);
                    }
                }
            }
        }

        static void draw_soulmate_link() {
            switch (soulmatesignal) {
                case 0:
                    DraWing.drawtext("drawlink", 1, Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.Red, "Connection Signal with " + soulmate.ChampionName + ": None");
                    break;
                case 1:
                    DraWing.drawtext("drawlink", 1, Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.Yellow, "Connection Signal with " + soulmate.ChampionName + ": Low");
                    break;
                case 2:
                    DraWing.drawtext("drawlink", 1, Drawing.Width * 0.45f, Drawing.Height * 0.82f, Color.Green, "Connection Signal with " + soulmate.ChampionName + ": Good");
                    break;
            }
        }

        #endregion

        #region HARASS
        static void harass() {
            if (!spellsreadyEQ) { return; }
            if (!(harassActive || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)) { return; }

            var dontprocessQ = true;
            if (laneclearSaveManaForE && !spellsEQmana) { dontprocessQ = false; }

            if (harassQ && dontprocessQ && Manapercent > harassmanaminQ) {
                var enemies = HeroManager.Enemies.Where(x => DistanceFromMe(x) < Q.Range && Q.CanCast(x) &&
                    ((Q.GetPrediction(x).Hitchance >= gethitchanceQ) ||
                    (Q.GetPrediction(x).Hitchance == HitChance.Collision)));
                if (enemies != null) { 
                    foreach (var enemy in enemies) {
                        switch (Q.GetPrediction(enemy).Hitchance) {
                            case HitChance.Collision:
                                var collide = Q.GetPrediction(enemy).CollisionObjects;
                                var dontbother = 0;
                                foreach (var thing in collide) {
                                    if ((thing.Health > Q.GetDamage(thing)) || thing.CharData.BaseSkinName == "gangplankbarrel") { dontbother = 1; }
                                }
                                if (dontbother == 0) {
                                    Q.Cast(enemy);
                                }
                                break;
                            default:
                                Q.Cast(enemy);
                                break;
                        }                    
                    }                
                }
            }
            if (harassE && ECanCast) {
                var enemies = HeroManager.Enemies.Where(x => x.HasBuff("kalistaexpungemarker") && E.CanCast(x));
                if (enemies == null) { return; }
                //out of range E
                foreach (var enemy in enemies) {
                    if (harassEoutOfRange && enemy.GetBuffCount("kalistaexpungemarker") >= harassEoutOfRangeMinSpears && DistanceFromMe(enemy) > (E.Range-10)) { ECast(); }
                    if (harassEThroughMinions) {
                        var minions = MinionManager.GetMinions(Player.ServerPosition, R.Range, MinionTypes.All, MinionTeam.NotAlly).FindAll(x => E.IsKillable(x));
                        if (minions != null && minions.Count() >= harassEThroughMinionsMinMinions) {
                            ECast();                        
                        }
                    }

                }
            }
        }
        #endregion

        #region JUNGLE CLEAR

        static void Jungleclear() {
            if (!playerisready || !spellsreadyEQ || !jungleActive) { return; }
            var minions = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.All, MinionOrderTypes.MaxHealth);
            var biggies = minions.Find(x => x.CharData.BaseSkinName.ToLower().Contains("dragon") || x.CharData.BaseSkinName.ToLower().Contains("baron"));
            var inside = minions.Find(x => x.Team == GameObjectTeam.Neutral && !x.CharData.BaseSkinName.ToLower().Contains("dragon") && !x.CharData.BaseSkinName.ToLower().Contains("baron"));
            
            if (inside != null) {
                //Q section..
                if (jungleclearQ && Manapercent > jungleclearQmana && DistanceFromMe(inside.Position) < jungleclearQMaxdistance && Q.CanCast(inside)) {
                    if (jungleclearSaveManaForE) {
                        if (spellsEQmana) { Q.Cast(inside); }
                    } else { Q.Cast(inside); }
                }
                //E section
                if (jungleclearE && Manapercent > jungleclearEmana && E.CanCast(inside) && inside.Health < GetEDamage(inside)) {
                    ECast();
                }            
            }

            if (biggies != null && jungleclearPopdragbaron) {
                var dmgE = GetEDamage(biggies);
                if (Player.HasBuff("barontarget")) { dmgE = dmgE * 0.5f; }
                var bighealth = (biggies.Health + (biggies.HPRegenRate / 2));
                var dmgQ = Q.GetDamage(biggies);
                var combo = Player.GetAutoAttackDamage(biggies)+dmgQ;
                if (ECanCast) {                    
                    if (dmgE > bighealth) { ECast(); }
                }
                if (Manapercent > jungleclearQmana && jungleclearQdragBaron) {
                    if (jungleclearSaveManaForE) {
                        if (spellsEQmana) { Q.Cast(biggies); }
                    } else { Q.Cast(biggies); }
                }
                if (DistanceFromMe(biggies) > jungleclearQMaxdistance) {
                    if (spellsEQmana && combo > biggies.Health && spellsreadyEQ) { Q.Cast(biggies); } else if (Q.IsReady() && dmgQ > biggies.Health) { Q.Cast(biggies); }
                }
            }
        }
        #endregion

        #region LANECLEAR

        static void laneclear() {
            if (!playerisready || !spellsreadyEQ) { return; }

            var dontprocessQ = true;
            if (laneclearSaveManaForE && !spellsEQmana) { dontprocessQ = false; }

            if (laneclearQ && laneclearQmana && Q.IsReady() && dontprocessQ) {
                var MinionsQ = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy).Find(x => x.Health < Q.GetDamage(x) &&
                    Q_GetCollisionMinions(Player, Player.ServerPosition.Extend(x.ServerPosition, Q.Range)).Count >= kalm.Item("laneclearQminMinions", true).GetValue<Slider>().Value &&
                    Q_GetCollisionMinions(Player, Player.ServerPosition.Extend(x.ServerPosition, Q.Range)).All(xx => xx.Health < Q.GetDamage(xx)));
                if (MinionsQ != null) {
                    Q.Cast(MinionsQ);
                }
            }

            if (laneclearE && laneclearEmana && ECanCast) {
                var MinionsE = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy).FindAll(x =>
                    x.HasBuff("kalistaexpungemarker") && E.IsKillable(x) && x.HealthPercent > laneclearEminhealth);
                if (MinionsE != null && MinionsE.Count() >= laneclearEminincrminions) {
                    ECast();
                }
                //check for big minions first
                if (laneclearbigminionsE && Manapercent >= laneclearBigMinionsMinMana) {
                    var bigminions = MinionsE.Find(x => x.CharData.BaseSkinName.ToLower().Contains("siege") || x.CharData.BaseSkinName.ToLower().Contains("super"));
                    if (bigminions != null) { ECast(); }
                }
            }
        }
        #endregion

        #region MISC EVENTS
        static void Event_OnNonKillableMinion(AttackableUnit minion) {
            var minionX = (Obj_AI_Minion)minion;
            var health = minionX.Health;
            if (laneclearEnonAAkillable && E.CanCast(minionX) && ECanCast && laneclearEmana && health > laneclearEminhealth && E.IsKillable(minionX)) {
                ECast();            
            }
        }

        static void Event_OnEnemyGapcloser(ActiveGapcloser target) {
            //gapcloser
            if (gapcloserQ) {
                var source = target.Sender;
                if (Player.Position.Distance(source.Position) > Q.Range) { return; }
                if (Q.CanCast(source)) { Q.Cast(source); }
            }
            if (gapcloserE && ECanCast && E.CanCast(target.Sender)) {
                ECast();
            }
        }

        static void Event_OnBeforeAttack(Orbwalking.BeforeAttackEventArgs args) {
            if (autoresetAA) {
                Orbwalking.ResetAutoAttackTimer();
            }
        }
        #endregion

        
        #region MISC FUNCTIONS
        static HitChance gethitchanceQ { get { return hitchanceQ(); } }
        static HitChance hitchanceQ() {
            switch (kalm.Item("harassQchance", true).GetValue<Slider>().Value) {
                case 1:
                    return HitChance.Low;
                case 2:
                    return HitChance.Medium;
                case 3:
                    return HitChance.High;
                case 4:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        //credits to xcsoft for this function
        static List<Obj_AI_Base> Q_GetCollisionMinions(AIHeroClient source, Vector3 targetposition) {
            var input = new PredictionInput {
                Unit = source,
                Radius = Q.Width,
                Delay = Q.Delay,
                Speed = Q.Speed,
            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return Collision.GetCollision(new List<Vector3> { targetposition }, input).OrderBy(obj => obj.Distance(source, false)).ToList();
        }

        static long? ecastlastusedon = DateTime.Now.Ticks;
        static bool ECanCast { get { if (!E.IsReady()) { return false; }; if ((DateTime.Now.Ticks - ecastlastusedon) > 5000000) { return true; }; return false; } }
        static void ECast() {
            ecastlastusedon = DateTime.Now.Ticks;
            E.Cast();
        }

        static bool hasundyingbuff(AIHeroClient target) {
            //checks for undying buffs and shields
            var hasbufforshield = TargetSelector.IsInvulnerable(target, TargetSelector.DamageType.Magical, false);
            if (hasbufforshield) { return true; }
            var hasbuff = HeroManager.Enemies.Find(a =>
                target.CharData.BaseSkinName == a.CharData.BaseSkinName && a.Buffs.Any(b =>
                    b.Name.ToLower().Contains("chrono shift") ||
                    b.Name.ToLower().Contains("poppyditarget")));
            if (hasbuff != null) { return true; }
            return false;
        }

        static void Killsteal() {
            var enemies = HeroManager.Enemies.Where(x => !hasundyingbuff(x) && (Q.CanCast(x) && DistanceFromMe(x) < Q.Range) || E.CanCast(x));
            if (enemies == null || !ECanCast) { return; }

            foreach (var enemy in enemies) {
                var edmg = GetEDamage(enemy);
                var enemyhealth = enemy.Health;
                var enemyregen = enemy.HPRegenRate / 2;
                var enemyhealthwithreg = enemyhealth + enemyregen;
                //check against erand
                if (ERand && enemyhealthwithreg + (Player.GetAutoAttackDamage(enemy)*2) <= edmg) {
                    ECast(); return;
                }
                //normal e pop...
                if (enemyhealthwithreg <= edmg) { ECast(); return; }
                if (Player.Mana < Q.ManaCost + E.ManaCost) { return; }
                if (Q.GetPrediction(enemy).Hitchance >= HitChance.Medium && Q.CanCast(enemy)) {
                    var qdamage = Q.GetDamage(enemy);
                    if (qdamage + edmg > enemyhealthwithreg) {
                        Q.Cast(enemy);
                    }
                }
            }
        }

        static float GetEDamage(Obj_AI_Base target, int spears = 0) {
            var stacks = target.GetBuffCount("kalistaexpungemarker");
            if (spears > 0) { stacks = spears; }
            if (stacks == 0) { return 1; }
            if (target is AIHeroClient) {
                return E.GetDamage(target);
            } else {
                return E.GetDamage(target) - 10;
            }
        }

        static void debuff() {
            if (!debuffActive || !qss.IsReady() || !mercurial.IsReady() || !dervish.IsReady()) { return; }
            //credits to Mactivator for "useful" spell names instead of adding a bunch of crap or having to parse each one
            var debuff = false;
            var spell = "";
            if (debuffBlind && Player.HasBuffOfType(BuffType.Blind)) { debuff = true; spell = "Blind"; }
            if (debuffBlitzGrab && Player.HasBuff("rocketgrab2")) { debuff = true; spell = "Blitz grab"; }
            if (debuffCharm && Player.HasBuffOfType(BuffType.Charm)) { debuff = true; spell = "Charm"; }
            if (debuffFear && Player.HasBuffOfType(BuffType.Fear)) { debuff = true; spell = "Fear"; }
            if (debuffFlee && Player.HasBuffOfType(BuffType.Flee)) { debuff = true; spell = "Flee"; }
            if (debuffSnare && Player.HasBuffOfType(BuffType.Snare)) { debuff = true; spell = "Snare"; }
            if (debuffTaunt && Player.HasBuffOfType(BuffType.Taunt)) { debuff = true; spell = "Taunt"; }
            if (debuffSuppression && Player.HasBuffOfType(BuffType.Suppression)) { debuff = true; spell = "Suppression"; }
            if (debuffStun && Player.HasBuffOfType(BuffType.Stun)) { debuff = true; spell = "Stun"; }
            if (debuffPolymorph && Player.HasBuffOfType(BuffType.Polymorph)) { debuff = true; spell = "Polymorph"; }
            if (debuffSilence && Player.HasBuffOfType(BuffType.Silence)) { debuff = true; spell = "Silence"; }
            if (debuffDehancer && Player.HasBuffOfType(BuffType.CombatDehancer)) { debuff = true; spell = "CombatDehancer"; }
            if (debuffZed && Player.HasBuff("zedulttargetmark")) { debuff = true; spell = "zedulttargetmark"; }
            if (debuffExhaust && Player.HasBuff("summonerexhaust")) { debuff = true; spell = "summonerexhaust"; }

            if (!debuff) { return; }

            DraWing.drawtext("debuffing", 5, Drawing.Width * 0.45f, Drawing.Height * 0.90f, Color.Red, "Debuffing from: " + spell);
            var s = false;
            var i = "";
            if (qss.IsReady()) {
                s = true; i = "qss";
                qss.Cast();
            } else if (mercurial.IsReady()) {
                s = true; i = "merc";
                mercurial.Cast();
            } else if (dervish.IsReady()) {
                s = true; i = "derv";
                dervish.Cast();
            }
            if (s) {
                DraWing.drawtext("debuffing", 5, Drawing.Width * 0.45f, Drawing.Height * 0.90f, Color.Red, "Debuffing with item: " + i);
            }
        }

        static void Event_OnItems(AIHeroClient target) {
            if (!botrk.IsOwned()) { return; }
            var targethealth = target.Health;
            var qdmg = Q.GetDamage(target);
            var edmg = GetEDamage(target);
            if (botrkactive && botrk.IsReady() && botrk.IsInRange(target)) {
                //selfish self-preservation
                if (Player.HealthPercent < kalm.Item("botrkmyheal", true).GetValue<Slider>().Value) { botrk.Cast(target); }
                //total dmg that I can do to target
                var totaldmg = qdmg + edmg;
                //get in health how much is x% of his total health
                var healthdmg = (kalm.Item("botrkKS", true).GetValue<Slider>().Value / 100) * target.MaxHealth;
                //if his health is less than x%+q+e then just botrkhim
                if (target.Health < healthdmg + totaldmg) {
                    botrk.Cast(target);
                    DraWing.drawtext("botrkwho", 3, Drawing.Width * 0.45f, Drawing.Height * 0.80f, Color.PapayaWhip, "Using botrk on: " + target.ChampionName);
                }
            }
        }

        static float DistanceFromMe(Obj_AI_Base args) {
            return Player.Position.Distance(args.Position);
        }

        static float DistanceFromMe(Vector3 args) {
            return Vector3.Distance(Player.Position,args);
        }

        #endregion misc

        #region AUTO W (Sentinel stuff)
        static readonly List<mysentinels> _mysentinels = new List<mysentinels>();
        internal class mysentinels {
            public string Name;
            public Vector3 Position;
            public mysentinels(string name, Vector3 position) {
                Name = name;
                Position = position;
            }
        }
        static int? sentinelcloserthan(Vector3 position, float distance) {
            foreach (var xxxXxxx in ObjectManager.Get<AttackableUnit>().Where(obj => obj.Name.Contains("RobotBuddy"))) {
                if (Vector3.Distance(position, xxxXxxx.Position) < distance) { return 1; }
            }
            return 0;
        }
        static void fillsentinels() {
            _mysentinels.Clear();
            foreach (var xxxXxxx in ObjectManager.Get<AttackableUnit>().Where(obj => obj.Name.Contains("RobotBuddy"))) {
                _mysentinels.Add(new mysentinels("RobotBuddy", xxxXxxx.Position));
            }
            //add the camps where to send sentinels to...
            _mysentinels.Add(new mysentinels("Blue_Camp_Blue_Buff", (Vector3)SummonersRift.Jungle.Blue_BlueBuff));
            _mysentinels.Add(new mysentinels("Blue_Camp_Red_Buff", (Vector3)SummonersRift.Jungle.Blue_RedBuff));
            _mysentinels.Add(new mysentinels("Red_Camp_Blue_Buff", (Vector3)SummonersRift.Jungle.Red_BlueBuff));
            _mysentinels.Add(new mysentinels("Red_Camp_Red_Buff", (Vector3)SummonersRift.Jungle.Red_RedBuff));
            _mysentinels.Add(new mysentinels("Dragon", (Vector3)SummonersRift.River.Dragon));
            _mysentinels.Add(new mysentinels("Baron", (Vector3)SummonersRift.River.Baron));
            _mysentinels.Add(new mysentinels("Mid_Bot_River", new Vector3(8370f, 6176f, -71.2406f)));
            //add river mid bush here...
            //_mysentinels.Add(new mysentinels("RiverTop", (Vector3)SummonersRift.Bushes.);
        }

        static void AutoW() {
            if (!W.IsReady()) { return; }
            var closestenemy = HeroManager.Enemies.Find(x => DistanceFromMe(x) < autowenemyisntnear);
            if (closestenemy != null) { return; }
            if (Player.IsDashing() || Player.Spellbook.IsAutoAttacking || Player.InFountain() || Player.IsRecalling()) { return; }
            if (autoWKey || Manapercent > autoWmana) {
                fillsentinels();
                Random rnd = new Random();
                var destinations = _mysentinels.Where(x => !x.Name.Contains("RobotBuddy") &&
                    DistanceFromMe(x.Position) < W.Range &&
                    DistanceFromMe(x.Position) > autowSpotTooCloseToMe &&
                    sentinelcloserthan(x.Position, autowsentinelisntnear) == 0).OrderBy(s => rnd.Next()).ToList();
                if (destinations == null) {
                    var dest = destinations.First();
                    W.Cast(dest.Position);
                    Notifications.AddNotification(new Notification("sending bug to:" + dest.Name, 5000).SetTextColor(Color.FromArgb(255, 0, 0)));
                    return;
                }
            }
        }

        #endregion

        #region WALLJUMP
        static void draw_jump_spots() {
            const float circleRange = 75f;
            foreach (var pos in jumpPos) {
                if (Player.Distance(pos.Key) <= 500f || Player.Distance(pos.Value) <= 500f) {
                    DraWing.drawcircle("jump" + pos, 0.5, pos.Key, circleRange, Color.Blue);
                    DraWing.drawcircle("jump" + pos, 0.5, pos.Value, circleRange, Color.Blue);
                }
            }
        }

        static void ShowjumpsandFlee() {
            if (!Q.IsReady()) { return; }
            DraWing.drawtext("jumpactive", 0.0333, Drawing.Width * 0.45f, Drawing.Height * 0.10f, Color.GreenYellow, "Wall Jump Active");
            var XXX = (Vector3)canjump();
            if (XXX != null) {
                DraWing.drawtext("couldjump", 0.0333, Drawing.Width * 0.45f, Drawing.Height * 0.50f, Color.GreenYellow, "could jump here");
                Q.Cast(XXX);
                Orbwalking.Orbwalk(null, XXX, 90f, 0f, false, false);
            } else {
                DraWing.drawtext("couldjump", 0.0333, Drawing.Width * 0.45f, Drawing.Height * 0.50f, Color.GreenYellow, "can't jump here");
            }

            foreach (var pos in jumpPos) {
                if (Player.Distance(pos.Key) <= 50f || Player.Distance(pos.Value) <= 50f) {
                    var x = (Vector3)canjump();
                    if (x != null) {
                        Q.Cast(x);
                        Orbwalking.Orbwalk(null, x, 90f, 0f, false, false);
                    }
                } else { return; }
            }
        }

        static Vector3? canjump() {
            var wallCheck = VectorHelper.GetFirstWallPoint(Player.Position, Player.Position);
            //loop angles around the player to check for a point to jump to
            //credits to hellsing wherever it has his code here somewhere... xD
            float maxAngle = 80;
            float step = maxAngle / 20;
            float currentAngle = 0;
            float currentStep = 0;
            Vector3 currentPosition = Player.Position;
            Vector2 direction = ((Player.Position.To2D() + 50) - currentPosition.To2D()).Normalized();
            while (true) {
                if (currentStep > maxAngle && currentAngle < 0) { break; }

                if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0) {
                    currentAngle = (currentStep) * (float)Math.PI / 180;
                    currentStep += step;
                } else if (currentAngle > 0) {
                    currentAngle = -currentAngle;
                }

                Vector3 checkPoint;

                // One time only check for direct line of sight without rotating
                if (currentStep == 0) {
                    currentStep = step;
                    checkPoint = currentPosition + 300 * direction.To3D();
                } else {
                    checkPoint = currentPosition + 300 * direction.Rotated(currentAngle).To3D();
                }
                if (checkPoint.IsWall()) { continue; }
                // Check if there is a wall between the checkPoint and currentPosition
                wallCheck = VectorHelper.GetFirstWallPoint(checkPoint, currentPosition);
                if (wallCheck == null) { continue; } //jump to the next loop
                //get the jump point
                Vector3 wallPositionOpposite = (Vector3)VectorHelper.GetFirstWallPoint((Vector3)wallCheck, currentPosition, 5);
                //check if the walking path is big enough to be worth a jump..if not then just skip to the next loop
                if (Player.GetPath(wallPositionOpposite).ToList().To2D().PathLength() - Player.Distance(wallPositionOpposite) < 230) {
                    DraWing.drawtext("couldjump", 0.0333, Drawing.Width * 0.45f, Drawing.Height * 0.50f, Color.GreenYellow, "not worth a jump...");
                    continue;
                }

                //check the jump distance and if its short enough then jump...
                if (Player.Distance(wallPositionOpposite, true) < Math.Pow(300 - Player.BoundingRadius / 2, 2)) {
                    return wallPositionOpposite;
                }
            }
            return null;
        }

        static void FillPositions() {
            jumpPos = new Dictionary<Vector3, Vector3>();
            var pos1001 = new Vector3(9340f, 4474f, -71.2406f);
            var pos1002 = new Vector3(9084f, 4640f, 51.95212f);
            jumpPos.Add(pos1001, pos1002);

            var pos1003 = new Vector3(7824f, 5998f, 51.4058f);
            var pos1004 = new Vector3(8010f, 6228f, -71.2406f);
            jumpPos.Add(pos1003, pos1004);

            var pos1005 = new Vector3(9830f, 3040f, 60.5358f);
            var pos1006 = new Vector3(9774f, 2760f, 49.22291f);
            jumpPos.Add(pos1005, pos1006);

            var pos1007 = new Vector3(6616f, 11674f, 53.83324f);
            var pos1008 = new Vector3(6462f, 12004f, 56.4768f);
            jumpPos.Add(pos1007, pos1008);
        }
        #endregion
    }
    #region MY DRAWING CLASS FOR TIMED CIRCLES/TEXT/LINE
    //drawing class for timed drawings (pls jodus add this feature in l# xD)
    internal class DraWing {
        private static readonly AIHeroClient Player = ObjectManager.Player;
        public static void Drawing_OnDraw(EventArgs args) {
            var timerightnow = Game.Time;
            //remove old items from lists
            drawtextlist.RemoveAll(x => timerightnow - x.Addedon > x.Timer);
            drawcirclelist.RemoveAll(x => timerightnow - x.Addedon > x.Timer);
            drawlinelist.RemoveAll(x => timerightnow - x.Addedon > x.Timer);
            //draw everything...
            if (drawtextlist.Count > 0) {
                foreach (var x in drawtextlist) {
                    Drawing.DrawText(x.X, x.Y, x.Color, x.Format);
                }
            }
            if (drawcirclelist.Count > 0) {
                foreach (var x in drawcirclelist) {
                    Render.Circle.DrawCircle(x.Position, x.Radius, x.Color);
                }
            }
            if (drawlinelist.Count > 0) {
                foreach (var x in drawlinelist) {
                    Drawing.DrawLine(x.X, x.Y, x.X2, x.Y2, x.Thickness, x.Color);
                }
            }
        }

        private static List<Drawline> drawlinelist = new List<Drawline>();
        private class Drawline {
            public string Name { get; set; }
            public double Timer { get; set; }
            public float Addedon { get; set; }
            //here goes the function stuff...
            public float X { get; set; }
            public float Y { get; set; }
            public float X2 { get; set; }
            public float Y2 { get; set; }
            public float Thickness { get; set; }
            public Color Color { get; set; }
        }
        public static void drawline(string name, double timer, float x, float y, float x2, float y2, float thickness, Color color) {
            drawlinelist.RemoveAll(xXx => xXx.Name == name);
            drawlinelist.Add(new Drawline() { Name = name, Timer = timer, Addedon = Game.Time, X = x, Y = y, X2 = x2, Y2 = y2, Thickness = thickness, Color = color });
            return;
        }

        private static List<Drawcircle> drawcirclelist = new List<Drawcircle>();
        private class Drawcircle {
            public string Name { get; set; }
            public double Timer { get; set; }
            public float Addedon { get; set; }
            public Vector3 Position { get; set; }
            public float Radius { get; set; }
            public Color Color { get; set; }
        }
        public static void drawcircle(string name, double timer, Vector3 position, float radius, Color color) {
            drawcirclelist.RemoveAll(x => x.Name == name);
            drawcirclelist.Add(new Drawcircle() { Name = name, Timer = timer, Addedon = Game.Time, Position = position, Radius = radius, Color = color });
            return;
        }

        private static List<Drawtext> drawtextlist = new List<Drawtext>();
        private class Drawtext {
            public string Name { get; set; }
            public double Timer { get; set; }
            public float Addedon { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public Color Color { get; set; }
            public string Format { get; set; }
        }
        public static void drawtext(string name, double timer, float X, float Y, Color color, string format) {
            drawtextlist.RemoveAll(x => x.Name == name);
            drawtextlist.Add(new Drawtext() { Name = name, Timer = timer, Addedon = Game.Time, X = X, Y = Y, Color = color, Format = format });
            return;
        }
    }
    #endregion

    #region VECTOR HELPER FROM STACKOVERFLOW
    internal class VectorHelper {
        private static readonly AIHeroClient player = ObjectManager.Player;

        // Credits to furikuretsu from Stackoverflow (http://stackoverflow.com/a/10772759)
        // Modified for my needs
        #region ConeCalculations

        public static bool IsLyingInCone(Vector2 position, Vector2 apexPoint, Vector2 circleCenter, double aperture) {
            // This is for our convenience
            double halfAperture = aperture / 2;

            // Vector pointing to X point from apex
            Vector2 apexToXVect = apexPoint - position;

            // Vector pointing from apex to circle-center point.
            Vector2 axisVect = apexPoint - circleCenter;

            // X is lying in cone only if it's lying in 
            // infinite version of its cone -- that is, 
            // not limited by "round basement".
            // We'll use dotProd() to 
            // determine angle between apexToXVect and axis.
            bool isInInfiniteCone = DotProd(apexToXVect, axisVect) / Magn(apexToXVect) / Magn(axisVect) >
                // We can safely compare cos() of angles 
                // between vectors instead of bare angles.
            Math.Cos(halfAperture);

            if (!isInInfiniteCone)
                return false;

            // X is contained in cone only if projection of apexToXVect to axis
            // is shorter than axis. 
            // We'll use dotProd() to figure projection length.
            bool isUnderRoundCap = DotProd(apexToXVect, axisVect) / Magn(axisVect) < Magn(axisVect);

            return isUnderRoundCap;
        }

        private static float DotProd(Vector2 a, Vector2 b) {
            return a.X * b.X + a.Y * b.Y;
        }

        private static float Magn(Vector2 a) {
            return (float)(Math.Sqrt(a.X * a.X + a.Y * a.Y));
        }

        #endregion

        public static Vector2? GetFirstWallPoint(Vector3 from, Vector3 to, float step = 25) {
            return GetFirstWallPoint(from.To2D(), to.To2D(), step);
        }

        public static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25) {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step) {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building)) {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }

        public static List<Obj_AI_Base> GetDashObjects(IEnumerable<Obj_AI_Base> predefinedObjectList = null) {
            List<Obj_AI_Base> objects;
            if (predefinedObjectList != null)
                objects = predefinedObjectList.ToList();
            else
                objects = ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsValidTarget(Orbwalking.GetRealAutoAttackRange(o))).ToList();

            var apexPoint = player.ServerPosition.To2D() + (player.ServerPosition.To2D() - Game.CursorPos.To2D()).Normalized() * Orbwalking.GetRealAutoAttackRange(player);

            return objects.Where(o => VectorHelper.IsLyingInCone(o.ServerPosition.To2D(), apexPoint, player.ServerPosition.To2D(), Math.PI)).OrderBy(o => o.Distance(apexPoint, true)).ToList();
        }
    }
    #endregion

}
