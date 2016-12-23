using System.Collections.Generic;
using System.Linq;
using System;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

using System.Media;
using System.Drawing;

using EloBuddy; 
using LeagueSharp.Common; 
namespace LordsSyndra
{
    public static class Program
    {
        public const string ChampionName = "Syndra";
        public static AIHeroClient Player = ObjectManager.Player;

        //Key binds
        public static MenuItem comboKey;
        public static MenuItem harassKey;
        public static MenuItem laneclearKey;
        public static MenuItem lanefreezeKey;

        //Orbwalker instance
        public static LeagueSharp.Common.Orbwalking.Orbwalker _orbwalker;
        public static Orbwalking.Orbwalker _orbwalker2;

        public static Menu Menu;
        public static Menu orbwalkerMenu;

        public static void Main()
        {
            Game_OnGameLoad(new EventArgs());
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampionName)
            {
                return;
            }


            Spells.Spellsdata();
            SyndraMenu();

            if (Menu.Item("Sound1").GetValue<bool>()) PlaySound.PlatSounds(PlaySound.welcome);
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            // Drawing.OnDraw += SpellDraws;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;

            if (Menu.Item("Orbwalker_Mode").GetValue<bool>())
            {
                Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            }

            Chat.Print("<font size='30'>Lord's Syndra</font> <font color='#b756c5'>by LordZEDith</font>");
        }

        public static void Interrupter2_OnInterruptableTarget(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            dd(unit, spell);
        }

        public static void dd(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("Interrupt").GetValue<bool>())
            {
                return;
            }

            if (spell.DangerLevel <= Interrupter2.DangerLevel.Medium || unit.IsAlly)
            {
                return;
            }

            if (Spells.E.IsReady() && Player.Distance(unit, true) <= Math.Pow(Spells.E.Range, 2) && unit.IsValidTarget(Spells.E.Range))
            {
                if (Spells.Q.IsReady())
                    Spells.UseQeSpell(unit);
                else
                    Spells.E.Cast(unit);
            }
            else if (Spells.Q.IsReady() && Spells.E.IsReady() && Player.Distance(unit, true) <= Math.Pow(Spells.QE.Range, 2))
                Spells.UseQeSpell((AIHeroClient)unit);
        }

        public static void SyndraMenu()
        {
            //Base menu
            Menu = new Menu("Lord's " + ChampionName, "Lord's " + ChampionName, true);
            orbwalkerMenu = new Menu("[Syndra] Orbwalker", "[Syndra] Orbwalker");
            //TargetSelector
            TargetSelector.AddToMenu(Menu.SubMenu("[Syndra] Target Selector"));

            //Orbwalker
            orbwalkerMenu.AddItem(new MenuItem("Orbwalker_Mode", "Sebby Orbwalker").SetValue(true));
            Menu.AddSubMenu(orbwalkerMenu);
            ChooseOrbwalker(Menu.Item("Orbwalker_Mode").GetValue<bool>()); //uncomment this line
           

            //Combo
            Menu.AddSubMenu(new Menu("Combo Settings", "Combo"));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use [Q]").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use [W]").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use [E]").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use [R]").SetValue(true));
            Menu.SubMenu("Combo").AddItem(new MenuItem("UseQE", "Use [QE] Combo").SetValue(true));

            //Harass
            Menu.AddSubMenu(new Menu("Harass Settings", "Harass"));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassAAQ", "Harass with Q if enemy AA").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseEH", "Use E").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("UseQEH", "Use QE").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassMana", "Mana %").SetValue(new Slider(0)));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassTurret", "Disable Harass if Inside Enemy Turret").SetValue(false));
            Menu.SubMenu("Harass").AddItem(new MenuItem("HarassActiveT", "Harass [Toggle]").SetValue(new KeyBind("Y".ToCharArray()[0],KeyBindType.Toggle, true)));

            //Farming menu:
            Menu.AddSubMenu(new Menu("Farm Settings", "Farm"));
            Menu.SubMenu("Farm").AddItem(new MenuItem("laneQ", "Use Q LaneClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("laneW", "Use W LaneClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("laneE", "Use E LaneClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("laneQL", "Use Q Last Hit").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("UseQJFarm", "Use Q JungleClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("UseWJFarm", "Use W JungleClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("UseEJFarm", "Use E JungleClear").SetValue(true));
            Menu.SubMenu("Farm").AddItem(new MenuItem("lanemana", "LaneClear MinMana %").SetValue(new Slider(65, 100, 0)));

            //Auto KS
            Menu.AddSubMenu(new Menu("Killsteal Settings", "AutoKS"));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseQKS", "Use Q").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseWKS", "Use W").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseEKS", "Use E").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseQEKS", "Use QE").SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseRKS", "Use R").SetValue(false));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("AutoKST", "Killsteal [TOGGLE]").SetValue(new KeyBind("U".ToCharArray()[0],KeyBindType.Toggle, true)));

            //Auto Flash Kill
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("UseFK1", "Q+E Flash Kill").SetValue(false));
            Menu.SubMenu("AutoKS").AddSubMenu(new Menu("Use Flash Kill on", "FKT"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("AutoKS").SubMenu("FKT").AddItem(new MenuItem("FKT" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(true));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("MaxE", "Max Enemies").SetValue(new Slider(2, 1, 5)));
            Menu.SubMenu("AutoKS").AddItem(new MenuItem("FKMANA", "Only Flash if mana > FC").SetValue(false));

            //Misc
            Menu.AddSubMenu(new Menu("Misc Settings", "Misc"));
            Menu.SubMenu("Misc").AddItem(new MenuItem("AntiGap", "Anti Gap Closer").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Interrupt", "Auto Interrupt Spells").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("IgniteALLCD", "Ignite if all spells are on cooldown").SetValue(false));
            if (Menu.Item("Orbwalker_Mode").GetValue<bool>())
            {
                Menu.SubMenu("Misc").AddItem(new MenuItem("OrbWAA", "AA while orbwalking").SetValue(true));
            }
            Menu.SubMenu("Misc").AddItem(new MenuItem("Sound1", "Startup Sound").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("Sound2", "In Game Sound").SetValue(true));
            Menu.SubMenu("Misc").AddItem(new MenuItem("YasuoWall", "Yasuo Windwall Check").SetValue(true));
            //QE Settings
            Menu.AddSubMenu(new Menu("[QE] Settings", "QEsettings"));
            Menu.SubMenu("QEsettings").AddItem(new MenuItem("QEDelay", "[QE] Delay").SetValue(new Slider(0, 0, 150)));
            Menu.SubMenu("QEsettings").AddItem(new MenuItem("QEMR", "[QE] Max Range %").SetValue(new Slider(100)));
            Menu.SubMenu("QEsettings").AddItem(new MenuItem("UseQEC", "[QE] to Enemy Near Cursor").SetValue(new KeyBind("T".ToCharArray()[0],KeyBindType.Press)));

            //R
            Menu.AddSubMenu(new Menu("[R] Settings", "Rsettings"));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Dont [R] if it can be killed with", "DontRw"));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwParam", "Damage From").SetValue(new StringList(new[] { "All", "Either one", "None" })));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwQ", "[Q]").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwW", "[W]").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwE", "[E]").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRw").AddItem(new MenuItem("DontRwA", "[AA]").SetValue(true));

            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Don't use [R] on", "DontR"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("Rsettings").SubMenu("DontR").AddItem(new MenuItem("DontR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(false));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("Buff Check (Don't Ult)", "DontRbuff"));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffUndying", "Trynda's Ult").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffJudicator", "Kayle's Ult").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffAlistar", "Zilean's Ult").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffZilean", "Alistar's Ult").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffZac", "Zac's Passive").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffAttrox", "Attrox's Passive").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffSivir", "Sivir's Spell Shield").SetValue(true));
            Menu.SubMenu("Rsettings").SubMenu("DontRbuff").AddItem(new MenuItem("DontRbuffMorgana", "Morgana's Black Shield").SetValue(true));
            Menu.SubMenu("Rsettings").AddSubMenu(new Menu("OverKill target by %", "okR"));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team))
                Menu.SubMenu("Rsettings").SubMenu("okR").AddItem(new MenuItem("okR" + enemy.BaseSkinName, enemy.BaseSkinName).SetValue(new Slider(0)));

            //Drawings
            Menu.AddSubMenu(new Menu("Draw Settings", "Drawing"));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "Q Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawW", "W Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawE", "E Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawR", "R Range").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQE", "QE Range").SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQEC", "QE Cursor indicator").SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawQEMAP", "QE Target Parameters").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawWMAP", "W Target Parameters").SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("Gank", "Gankable Enemy Indicator").SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("HUD", "Heads-up Display").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("DrawHPFill", "After Combo HP Fill").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("KillText", "Kill Text").SetValue(true));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("KillTextHP", "% HP After Combo Text").SetValue(false));
            Menu.SubMenu("Drawing").AddItem(new MenuItem("drawing", "Damage Indicator").SetValue(true));

            var about = new Menu("About", "About").SetFontStyle(FontStyle.Regular, fontColor: SharpDX.Color.Gray);
            Menu.AddSubMenu(about);
            about.AddItem(new MenuItem("AboutText", "About"));
            about.AddItem(new MenuItem("Author", "Author: LordZEDith").SetFontStyle(FontStyle.Italic, fontColor: SharpDX.Color.White));
            about.AddItem(new MenuItem("Credits", "Credits: Big Thanks ScienceArk for Updates and stephenjason89"));
            about.AddItem(new MenuItem("2Credits", "Credits: Sebby,NightMoon,xSLx & Esk0r"));
            about.AddItem(new MenuItem("Upvote", "Remember to upvote the assembly if you like it ! GL & HF").SetFontStyle(FontStyle.Italic, fontColor: SharpDX.Color.Goldenrod));

            //Add main menu
            Menu.AddToMainMenu();

        }
        
         public static void ChooseOrbwalker(bool mode)
         {
             if (mode)
             {
                /*   _orbwalker = new LeagueSharp.Common.Orbwalking.Orbwalker(orbwalkerMenu);
                   comboKey = Menu.Item("Orbwalk");
                   harassKey = Menu.Item("Farm");
                   laneclearKey = Menu.Item("LaneClear");
                   lanefreezeKey = Menu.Item("Freeze");
                   Chat.Print("Common Orbwalker Loaded");*/
                _orbwalker2 = new Orbwalking.Orbwalker(orbwalkerMenu);
                comboKey = Menu.Item("Orbwalk");
                harassKey = Menu.Item("Farm");
                laneclearKey = Menu.Item("LaneClear");
                lanefreezeKey = Menu.Item("Freeze");
                Chat.Print("Sebby Orbwalker Loaded");
            }
            
             else
             {
                 
             }
         }

        public static void OnCreate(GameObject obj, EventArgs args)
        {
            if (Player.Distance(obj.Position) > 1500 ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return;
            //Yasuo Wall
            if (obj.IsValid &&
                System.Text.RegularExpressions.Regex.IsMatch(
                    obj.Name, "_w_windwall.\\.troy",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                Utils._yasuoWall = obj;
            }
        }

        public static void OnDelete(GameObject obj, EventArgs args)
        {
            if (Player.Distance(obj.Position) > 1500 ||
                !ObjectManager.Get<AIHeroClient>()
                    .Any(h => h.ChampionName == "Yasuo" && h.IsEnemy && h.IsVisible && !h.IsDead)) return;
            //Yasuo Wall
            if (obj.IsValid && System.Text.RegularExpressions.Regex.IsMatch(
                obj.Name, "_w_windwall.\\.troy",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                Utils._yasuoWall = null;
            }
        }

        public static void Game_OnGameUpdate(EventArgs args)
        {
            var ComboKey = comboKey.GetValue<KeyBind>();
            var HarassKey = harassKey.GetValue<KeyBind>();
            var HarassToggle = Menu.Item("HarassActiveT").GetValue<KeyBind>();
            var LaneClearKey = laneclearKey.GetValue<KeyBind>();
            var FreezeKey = lanefreezeKey.GetValue<KeyBind>();
            var DontHarassInTurret = Menu.Item("HarassTurret").GetValue<bool>();
            var AutoKSKey = Menu.Item("AutoKST").GetValue<KeyBind>();

            if (Player.IsDead)
            {
                return;
            }

            Spells.UpdateSpellRange();

            UseQEToMouse();


            //Combo and Harass
            if (ComboKey.Active)
            {
                Combo();
            }
            else if (HarassKey.Active || HarassToggle.Active)
            {
                if (DontHarassInTurret && HarassKey.Active)
                {
                    var turret = ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(t => t.IsValidTarget(Spells.Q.Range));
                    if (turret == null)
                    {
                        Harass();
                    }
                }
                else
                {
                    Harass();
                }
            }

            //Auto KS
            if (AutoKSKey.Active)
            {
                AutoKs();
            }

            //Farm
            if (ComboKey.Active)
            {
                return;
            }

            if (LaneClearKey.Active || FreezeKey.Active)
            {
                Farm();
            }
            if (LaneClearKey.Active)
            {
                JungleFarm();
            }
        }

        public static void UseQEToMouse()
        {
            var UseQEToMouse = Menu.Item("UseQEC").GetValue<KeyBind>();

            if (UseQEToMouse.Active && Spells.E.IsReady() && Spells.Q.IsReady())
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                enemy =>
                                    enemy.Team != Player.Team && Player.Distance(enemy, true) <= Math.Pow(Spells.QE.Range, 2))
                            .Where(
                                enemy =>
                                    enemy.IsValidTarget(Spells.QE.Range) && enemy.Distance(Game.CursorPos, true) <= 150 * 150))
                {
                    Spells.UseQeSpell(enemy);
                }
            }
        }

        public static void Combo()
        {
            var UseQCombo = Menu.Item("UseQ").GetValue<bool>();
            var UseWCombo = Menu.Item("UseW").GetValue<bool>();
            var UseECombo = Menu.Item("UseE").GetValue<bool>();
            var UseRCombo = Menu.Item("UseR").GetValue<bool>();
            var UseQECombo = Menu.Item("UseQE").GetValue<bool>();

            Spells.UseSpells(UseQCombo, UseWCombo, UseECombo, UseRCombo, UseQECombo);
        }

        public static void Harass()
        {
            var HarassMinMama = Menu.Item("HarassMana").GetValue<Slider>().Value;
            var UseQHarass = Menu.Item("UseQH").GetValue<bool>();
            var UseWHarass = Menu.Item("UseWH").GetValue<bool>();
            var UseEHarass = Menu.Item("UseEH").GetValue<bool>();
            var UseQEHarass = Menu.Item("UseQEH").GetValue<bool>();

            if (Player.Mana / Player.MaxMana * 100 < HarassMinMama) 
            {
                return;
            }

            Spells.UseSpells(UseQHarass, UseWHarass, UseEHarass, false, UseQEHarass); // false is Dont Use R
        }

        public static void Farm()
        {

            //var allMinionsQ = MinionManager.GetMinions(Player.ServerPosition, , MinionTypes.All, MinionTeam.Enemy);
            //var allMinionsW = MinionManager.GetMinions(Player.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Enemy);
            var allMinions = MinionManager.GetMinions(Player.ServerPosition, 900, MinionTypes.All, MinionTeam.Enemy);
            var qfarmpos = Spells.Q.GetCircularFarmLocation(allMinions, Spells.Q.Width);
            var LaneClearMinMama = Menu.Item("lanemana").GetValue<Slider>().Value;
            var UseWLaneClear = Menu.Item("laneW").GetValue<bool>();
            var UseQLaneClear = Menu.Item("laneQ").GetValue<bool>();
            var UseQLastHit = Menu.Item("laneQL").GetValue<bool>();
            var UseELaneClear = Menu.Item("laneE").GetValue<bool>();

            if (Player.ManaPercent <= LaneClearMinMama)
            {
                return;
            }

            if (allMinions.Count == 0)
            {
                return;
            }
            //Use Q LaneClear
            if (qfarmpos.MinionsHit >= 2 && UseQLaneClear && UseQLastHit && Spells.Q.IsReady())
            {
                Spells.Q.Cast(qfarmpos.Position);
            }
            foreach (var minion in allMinions.Where(a => a.Health <= Spells.Q.GetDamage(a) && a.IsEnemy && a.IsValid))
            {
                if (UseQLaneClear && UseQLastHit && !ObjectManager.Player.Spellbook.IsAutoAttacking && Spells.Q.IsReady())
                {
                    Spells.Q.Cast(minion);
                }
            }

            //Use W LaneClear
            if (!UseWLaneClear)
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState == 1 && allMinions.Count >= 2)
                {
                    //WObject
                    var gObjectPos = Utils.GetGrabableObjectPos(false);
                    if (gObjectPos.To2D().IsValid() && Environment.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 150 &&
                        Spells.W.IsReady())
                    {
                        Spells.W.Cast(gObjectPos);
                    }
                }
                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).ToggleState != 1)
                {
                    var CanWMinions = Spells.Q.GetCircularFarmLocation(allMinions, Spells.W.Width);
                    if (CanWMinions.MinionsHit >= 2 && Spells.W.IsReady())
                    {
                        Spells.W.Cast(CanWMinions.Position);
                    }
                    else if (CanWMinions.MinionsHit >= 1 && Spells.W.IsReady())
                    {
                        Spells.W.Cast(CanWMinions.Position);
                    }
                }
            }

            if (Spells.E.IsReady() && UseELaneClear)
            {
                var EpredPosition = Spells.E.GetCircularFarmLocation(allMinions);
                if (EpredPosition.MinionsHit >= 2)
                {
                    Spells.E.Cast(EpredPosition.Position);
                }
            }
        }

        public static void JungleFarm()
        {
            var useQ = Menu.Item("UseQJFarm").GetValue<bool>();
            var useW = Menu.Item("UseWJFarm").GetValue<bool>();
            var useE = Menu.Item("UseEJFarm").GetValue<bool>();
            var mobs = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Spells.W.Range, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            var mob = mobs[0];
            if (Spells.Q.IsReady() && useQ)
            {
                Spells.Q.Cast(mob);
            }
            if (Spells.W.IsReady() && useW && Environment.TickCount - Spells.Q.LastCastAttemptT > 800)
            {
                Spells.W.Cast(mob);
            }
            if (useE && Spells.E.IsReady())
            {
                Spells.E.Cast(mob);
            }
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            {
                //Last cast time of spells
                if (sender.IsMe)
                {
                    if (args.SData.Name == "SyndraQ")
                        Spells.Q.LastCastAttemptT = Environment.TickCount;
                    if (args.SData.Name == "SyndraW" || args.SData.Name == "syndrawcast")
                        Spells.W.LastCastAttemptT = Environment.TickCount;
                    if (args.SData.Name == "SyndraE" || args.SData.Name == "syndrae5")
                        Spells.E.LastCastAttemptT = Environment.TickCount;
                }

                //Harass when enemy do attack
                if (Menu.Item("HarassAAQ").GetValue<bool>() && sender.Type == Player.Type && sender.Team != Player.Team &&
                    args.SData.Name.ToLower().Contains("attack") &&
                    Player.Distance(sender, true) <= Math.Pow(Spells.Q.Range, 2) &&
                    Player.Mana/Player.MaxMana*100 > Menu.Item("HarassMana").GetValue<Slider>().Value)
                {
                    Spells.UseQSpell((AIHeroClient)sender);
                }
                if (!sender.IsValid || sender.Team == ObjectManager.Player.Team || args.SData.Name != "YasuoWMovingWall")
                    return;
                Utils._wallCastT = Environment.TickCount;
                Utils._yasuoWallCastedPos = sender.ServerPosition.To2D();
            }
        }

        //Anti gapcloser
        public static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("AntiGap").GetValue<bool>()) return;

            if (!Spells.E.IsReady() || !(Player.Distance(gapcloser.Sender, true) <= Math.Pow(Spells.QE.Range, 2)) ||
                !gapcloser.Sender.IsValidTarget(Spells.QE.Range))
                return;
            if (Spells.E.IsReady() && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).SData.Mana <= Player.Mana)
            {
                Spells.E.Cast(gapcloser.Sender);
            }
        }

        //Interrupt dangerous spells
/*        public static void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Menu.Item("Interrupt").GetValue<bool>())
            {
                return;
            }

            if (E.IsReady() && Player.Distance(unit, true) <= Math.Pow(E.Range, 2) && unit.IsValidTarget(E.Range))
            {
                if (Q.IsReady())
                    UseQeSpell(unit);
                else
                    E.Cast(unit);
            }
            else if (Q.IsReady() && E.IsReady() && Player.Distance(unit, true) <= Math.Pow(QE.Range, 2))
                UseQeSpell((AIHeroClient) unit);
        }*/

        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var orbwalkAa = false;
            if (Menu.Item("OrbWAA").GetValue<bool>()) orbwalkAa = !Spells.Q.IsReady() && (!Spells.W.IsReady() || !Spells.E.IsReady());
            if (comboKey.GetValue<KeyBind>().Active)
                args.Process = orbwalkAa;
        }

        public static void AutoKs()
        {
            var UseQEKS = Menu.Item("UseQEKS").GetValue<bool>();
            var UseWKS = Menu.Item("UseWKS").GetValue<bool>();
            var UseQKS = Menu.Item("UseQKS").GetValue<bool>();
            var UseEKS = Menu.Item("UseEKS").GetValue<bool>();
            var UseRKS = Menu.Item("UseRKS").GetValue<bool>();

            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != Player.Team).Where(enemy =>
                        !enemy.HasBuff("UndyingRage") && !enemy.HasBuff("JudicatorIntervention") &&
                        enemy.IsValidTarget(Spells.QE.Range) && Environment.TickCount - Spells.FlashLastCast > 650 + Game.Ping)
                )
            {
                // Use QE KS
                if (GetDamage.GetComboDamage(enemy, UseQEKS, false, UseQEKS, false) >
                    enemy.Health && Player.Distance(enemy, true) <= Math.Pow(Spells.QE.Range, 2))
                {
                    Spells.UseSpells(false, false, false, false, UseQEKS);
                    PlaySound.PlatSounds();
                    //Chat.Print("QEKS " + enemy.Name);
                }
                    //Use W KS
                else if (GetDamage.GetComboDamage(enemy, false, UseWKS, false, false) > enemy.Health &&
                    Player.Distance(enemy, true) <= Math.Pow(Spells.W.Range, 2))
                {
                    Spells.UseSpells(false, UseWKS, false, false, false);
                    PlaySound.PlatSounds();
                    //Chat.Print("WKS " + enemy.Name);
                }
                    //Use Q E KS 
                else if (GetDamage.GetComboDamage(enemy, UseQKS, false, UseEKS, false) > enemy.Health &&
                    Player.Distance(enemy, true) <= Math.Pow(Spells.Q.Range + 25f, 2))
                {
                    Spells.UseSpells(UseQKS, false, UseEKS, false, false);
                    PlaySound.PlatSounds();
                    //Chat.Print("QEKSC " + enemy.Name);
                }
                    //Use QWER QE KS
                else if (GetDamage.GetComboDamage(enemy, UseQKS, UseWKS, UseEKS, UseRKS) > enemy.Health &&
                    Player.Distance(enemy, true) <= Math.Pow(Spells.R.Range, 2))
                {
                    Spells.UseSpells(UseQKS, UseWKS, UseEKS, UseRKS, UseQEKS);
                    PlaySound.PlatSounds();
                    //Chat.Print("QWERKS " + enemy.Name);
                }

                // Why ?
/*                else if (
                    (GetComboDamage(
                        enemy, false, false, Menu.Item("UseEKS").GetValue<bool>(),
                        Menu.Item("UseRKS").GetValue<bool>()) > enemy.Health ||
                     GetComboDamage(
                         enemy, false, Menu.Item("UseWKS").GetValue<bool>(),
                         Menu.Item("UseEKS").GetValue<bool>(), false) > enemy.Health) &&
                    Player.Distance(enemy, true) <= Math.Pow(QE.Range, 2))
                {
                    UseSpells(
                        false, //Q
                        false, //W
                        false, //E
                        false, //R
                        Menu.Item("UseQEKS").GetValue<bool>() //QE
                        );
                    PlaySound();
                    //Chat.Print("QEKS " + enemy.Name);
                }*/


                //Flash Kill
                var useFlash = Menu.Item("FKT" + enemy.BaseSkinName) != null &&
                               Menu.Item("FKT" + enemy.BaseSkinName).GetValue<bool>();
                var useR = Menu.Item("DontR" + enemy.BaseSkinName) != null &&
                           Menu.Item("DontR" + enemy.BaseSkinName).GetValue<bool>() == false;
                var rflash = GetDamage.GetComboDamage(enemy, UseQKS, false, UseEKS, false) < enemy.Health;
                var ePos = Spells.R.GetPrediction(enemy);


                if ((Spells.FlashSlot == SpellSlot.Unknown && ObjectManager.Player.Spellbook.CanUseSpell(Spells.FlashSlot) != SpellState.Ready) ||
                    !useFlash || !(Player.Distance(ePos.UnitPosition, true) <= Math.Pow(Spells.Q.Range + 25f + 395, 2)) ||
                    !(Player.Distance(ePos.UnitPosition, true) > Math.Pow(Spells.Q.Range + 25f + 200, 2)))
                    continue;
                if (
                    (!(GetDamage.GetComboDamage(enemy, UseQKS, false, UseEKS, false) > enemy.Health) ||
                    !Menu.Item("UseFK1").GetValue<bool>()) &&
                    (!(GetDamage.GetComboDamage(enemy, false, false, false, UseRKS) > enemy.Health) ||
                     !Menu.Item("UseFK2").GetValue<bool>() ||
                     !(Player.Distance(ePos.UnitPosition, true) <= Math.Pow(Spells.R.Range + 390, 2)) ||
                     Environment.TickCount - Spells.R.LastCastAttemptT <= Game.Ping + 750 ||
                     Environment.TickCount - Spells.QE.LastCastAttemptT <= Game.Ping + 750 ||
                     !(Player.Distance(ePos.UnitPosition, true) > Math.Pow(Spells.R.Range + 200, 2))))
                    continue;
                var totmana = 0d;
                if (Menu.Item("FKMANA").GetValue<bool>())
                {
                    totmana = Spells.SpellList.Aggregate(
                        totmana, (current, spell) => current + ObjectManager.Player.Spellbook.GetSpell(spell.Slot).SData.Mana);
                }
                if (totmana > Player.Mana && Menu.Item("FKMANA").GetValue<bool>() &&
                    Menu.Item("FKMANA").GetValue<bool>())
                    continue;
                var nearbyE = ePos.UnitPosition.CountEnemiesInRange(1000);
                if (nearbyE > Menu.Item("MaxE").GetValue<Slider>().Value)
                    continue;
                var flashPos = Player.ServerPosition -
                               Vector3.Normalize(Player.ServerPosition - ePos.UnitPosition)*400;
                if (flashPos.IsWall())
                    continue;
                if (rflash)
                {
                    if (useR)
                    {
                        //Use Ult after flash if can't be killed by QE
                        ObjectManager.Player.Spellbook.CastSpell(Spells.FlashSlot, flashPos);
                        Spells.UseSpells(false, false, false, UseRKS, false);
                        PlaySound.PlatSounds();
                    }
                }
                else
                {
                    //Q & E after flash
                    ObjectManager.Player.Spellbook.CastSpell(Spells.FlashSlot, flashPos);
                }
                Spells.FlashLastCast = Environment.TickCount;
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            Drawings.Draw();
        }

    }
}
