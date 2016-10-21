using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace DarkEzreal.Main
{
    using System;
    using System.Windows.Forms;

    using DarkEzreal.Common;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;

    using Menu = LeagueSharp.SDK.UI.Menu;

    internal class Config
    {
        public static Menu MenuIni, QMenu, WMenu, EMenu, RMenu, MiscMenu, DrawMenu;

        public static AIHeroClient Player = ObjectManager.Player;

        public static string[] JungleMobs;

        public static bool Init()
        {
            try
            {
                if (!Player.ChampionName.Equals("Ezreal"))
                {
                    return false;
                }

                SpellsManager.Init();

                switch (Game.MapId)
                {
                    case GameMapId.SummonersRift:
                        JungleMobs = new[]
                                         {
                                             "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire", "SRU_Dragon_Water", "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", 
                                             "SRU_RiftHerald", "Sru_Crab", "SRU_Murkwolf", "SRU_Blue", "SRU_Red"
                                         };
                        break;
                    case GameMapId.TwistedTreeline:
                        JungleMobs = new[] { "TT_NWraith", "TT_NWolf", "TT_NGolem", "TT_Spiderboss" };
                        break;
                    case GameMapId.CrystalScar:
                        JungleMobs = new[] { "AscXerath" };
                        break;
                    default:
                        JungleMobs = new[] { "Nothing" };
                        break;
                }

                MenuIni = new Menu("DarkEzreal", "DarkEzreal", true).Attach();
                QMenu = MenuIni.CreateMenu("Q", "Q Settings");
                WMenu = MenuIni.CreateMenu("W", "W Settings");
                EMenu = MenuIni.CreateMenu("E", "E Settings");
                RMenu = MenuIni.CreateMenu("R", "R Settings");
                MiscMenu = MenuIni.CreateMenu("MiscMenu", "Misc Settings");
                DrawMenu = MenuIni.CreateMenu("DrawMenu", "Drawings Settings");

                var Qhit = new Menu("Qhit", "HitChance Settings");
                Qhit.Add(new MenuList<string>("hit", "Q HitChance", new[] { "Low", "Medium", "High", "Very High" }));
                QMenu.Add(Qhit);

                var Qc = new Menu("Qc", "Combo Settings");
                Qc.CreateBool("Q", "Use Q");
                Qc.CreateBool("AQ", "AA > Q", false);
                Qc.CreateSliderButton("mana", "Q ManaManager");
                QMenu.Add(Qc);

                var Qh = new Menu("Qh", "Harass Settings");
                Qh.CreateKeyBind("Q", "Use Q", Keys.L, KeyBindType.Toggle);
                Qh.CreateBool("autoQ", "Auto Q", false);
                Qh.CreateSliderButton("mana", "Q ManaManager", 60);
                QMenu.Add(Qh);

                var Qlh = new Menu("Qlh", "LastHit Settings");
                Qlh.CreateKeyBind("Q", "Use Q", Keys.L, KeyBindType.Toggle);
                Qlh.CreateBool("autoQ", "Auto Q", false);
                Qlh.CreateBool("Qunk", "Q LastHit Out of AA Range");
                Qlh.CreateSliderButton("mana", "Q ManaManager", 60);
                QMenu.Add(Qlh);

                var Qlc = new Menu("Qlc", "LaneClear Settings");
                Qlc.CreateKeyBind("Q", "Use Q", Keys.L, KeyBindType.Toggle);
                Qlc.CreateBool("autoQ", "Auto Q", false);
                Qlc.CreateBool("lhQ", "Q LastHit Only", false);
                Qlc.CreateBool("Qunk", "Q LastHit Out of AA Range Only");
                Qlc.CreateSliderButton("mana", "Q ManaManager", 60);
                QMenu.Add(Qlc);

                var Qjc = new Menu("Qjc", "JungleClear Settings");
                Qjc.CreateBool("Q", "Use Q");
                Qjc.CreateBool("Qprio", "Focus Large Mobs First");
                Qjc.CreateSliderButton("mana", "Q ManaManager", 60);
                QMenu.Add(Qjc);

                var Qks = new Menu("Qks", "Stealer Settings");
                Qks.CreateBool("Q", "Use Q");
                Qks.CreateBool("Qjs", "Steal Jungle Mobs");
                Qks.CreateBool("Qks", "Steal Champions");
                QMenu.Add(Qks);

                var Whit = new Menu("Whit", "HitChance Settings");
                Whit.Add(new MenuList<string>("hit", "W HitChance", new[] { "Low", "Medium", "High", "Very High" }));
                WMenu.Add(Whit);

                var Wc = new Menu("Wc", "Combo Settings");
                Wc.CreateBool("W", "Use W");
                Wc.CreateSliderButton("mana", "W ManaManager", 60);
                WMenu.Add(Wc);

                var Wh = new Menu("Wh", "Harass Settings");
                Wh.CreateKeyBind("W", "Use W", Keys.H, KeyBindType.Toggle);
                Wh.CreateBool("autoW", "Auto W", false);
                Wh.CreateSliderButton("mana", "W ManaManager", 60);
                WMenu.Add(Wh);

                var Wks = new Menu("Wks", "Stealer Settings");
                Wks.CreateBool("W", "Use W");
                Wks.CreateBool("Wks", "Steal Champions");
                WMenu.Add(Wks);

                var Ec = new Menu("Ec", "Combo Settings");
                Ec.CreateBool("kiteE", "E Kite Melee", false);
                Ec.CreateBool("autoE", "Auto E Into W", false);
                Ec.CreateBool("gapE", "E Gap Close To Target", false);
                Ec.CreateSliderButton("danger", "Dont E Into X Enemies", 3, 1, 6);
                Ec.CreateSliderButton("mana", "E ManaManager", 60);
                EMenu.Add(Ec);

                var Eh = new Menu("Eh", "Harass Settings");
                Eh.CreateBool("autoE", "Auto E Into W", false);
                Eh.CreateSliderButton("danger", "Dont E Into X Enemies", 3, 1, 6);
                Eh.CreateSliderButton("mana", "E ManaManager", 60);
                EMenu.Add(Eh);

                var Eks = new Menu("Eks", "Stealer Settings");
                Eks.CreateBool("E", "Use E");
                Eks.CreateBool("Eks", "Steal Champions");
                EMenu.Add(Eks);

                var Rhit = new Menu("Rhit", "HitChance Settings");
                Rhit.Add(new MenuList<string>("hit", "R HitChance", new string[] { "Low", "Medium", "High", "Very High" }));
                RMenu.Add(Rhit);

                var Rc = new Menu("Rc", "Combo Settings");
                Rc.CreateBool("R", "Use R");
                Rc.CreateBool("Rcc", "Auto R CC'ed target");
                Rc.CreateBool("Rfinisher", "Use R as Finisher");
                var Raoe = new Menu("Raoe", "R AoE Settings");
                Raoe.CreateBool("target", "Check AoE From Main Target");
                Raoe.CreateSliderButton("Raoe", "Use R on X Enemies", 3, 1, 6);
                Rc.Add(Raoe);
                Rc.CreateSliderButton("mana", "R ManaManager");
                RMenu.Add(Rc);

                var Rks = new Menu("Rks", "Stealer Settings");
                Rks.CreateBool("R", "Use R");
                Rks.CreateSlider("range", "[R] Steal Range (0 = Global)", 4000, 0, 10000);
                Rks.CreateBool("Rjs", "Steal Jungle Mobs", false);
                Rks.CreateBool("Rks", "Steal Champions");
                RMenu.Add(Rks);
                RMenu.CreateKeyBind("Rkey", "R HotKey", Keys.S, KeyBindType.Press);

                var Rmin = MiscMenu.CreateSlider("Rmin", "Adjust MIN R Range", 500, 50, 3000);

                var Rmax = MiscMenu.CreateSlider("Rmax", "Adjust MAX R Range", 2000, Rmin.Value, 15000);

                Rmin.ValueChanged += delegate { Rmax.MinValue = Rmin.Value + 150; };

                Rmax.ValueChanged += delegate { SpellsManager.R.Range = Rmax.Value; };

                var stealer = new Menu("steal", "Select JungleMobs");
                foreach (var mob in JungleMobs)
                {
                    stealer.CreateBool(mob, "Steal " + mob);
                }

                MiscMenu.Add(stealer);
                MiscMenu.CreateBool("hooks", "Anti Hooks");
                MiscMenu.CreateKeyBind("EW", "E > W KeyBind", Keys.A, KeyBindType.Press);
                MiscMenu.CreateBool("Egap", "Anti GapClosers");

                foreach (var spell in SpellsManager.Spells)
                {
                    DrawMenu.CreateBool(spell.Slot.ToString(), "Draw " + spell.Slot);
                }

                Game.OnUpdate += Modes.ModesManager.GameOnOnUpdate;
                Drawing.OnDraw += EventsHandler.Drawing_OnDraw;
                Obj_AI_Base.OnProcessSpellCast += EventsHandler.ObjAiBaseOnOnProcessSpellCast;
                Obj_AI_Base.OnBuffGain += EventsHandler.Obj_AI_Base_OnBuffAdd;
                Events.OnGapCloser += EventsHandler.Events_OnGapCloser;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now.ToString("[H:mm:ss") + " - DarkEzreal] ERROR: " + e);
                return false;
            }
        }
    }
}
