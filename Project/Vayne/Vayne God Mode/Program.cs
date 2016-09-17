using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GodModeOn_Vayne
{
    //To-DO
    // E-For gapcloser - select what spell to it.
    // E for Interrupt . select spells.
    // Q+E with nice logic
    class Program
    {
        public static AIHeroClient Player;
        public static Spell Q, W, E, R;
        public static SpellSlot Flash;
        public static Menu menu;
        private static int InterruptNum;
        private static int GapcloserNum;
        private static List<String> InterruptSpell; 
        private static Orbwalking.Orbwalker orb;
        static readonly string[] Gapcloser = new[]
            {
                "AkaliShadowDance", "Headbutt", "DianaTeleport", "IreliaGatotsu", "JaxLeapStrike", "JayceToTheSkies",
                "MaokaiUnstableGrowth", "MonkeyKingNimbus", "Pantheon_LeapBash", "PoppyHeroicCharge", "QuinnE",
                "XenZhaoSweep", "blindmonkqtwo", "FizzPiercingStrike", "RengarLeap"
            };
         static readonly string[] Interrupt = new[]
            {
                "KatarinaR", "GalioIdolOfDurand", "Crowstorm", "Drain", "AbsoluteZero", "ShenStandUnited", "UrgotSwap2",
                "AlZaharNetherGrasp", "FallenOne", "Pantheon_GrandSkyfall_Jump", "VarusQ", "CaitlynAceintheHole",
                "MissFortuneBulletTime", "InfiniteDuress", "LucianR"
            };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }
        private static void OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName.ToLower() != "vayne") return;
            Q = new Spell(SpellSlot.Q, 0f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 545f);
            R = new Spell(SpellSlot.R);
            Flash = Player.GetSpellSlot("summonerflash");
            Menu();
            Game.OnUpdate += Game_OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        //    Gapcloser.OnGapCloser += GapcloserOnOnGapCloser;
        }


        public static AIHeroClient targetE()
        {
            return TargetSelector.GetTarget(550,TargetSelector.DamageType.Physical,false);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
                            var DrawE = Program.menu.Item("DE").GetValue<bool>();
                            var DrawR = Program.menu.Item("DR").GetValue<bool>();
                            if (DrawR)
                            {
                                Render.Circle.DrawCircle(Player.Position, 800f, System.Drawing.Color.YellowGreen, 2);
                            }
                            if (DrawE && E.IsReady())
                            {
                        //        var wtst = Drawing.WorldToScreen(Efinishpos(targetE()));
                       //         var wtsp = Drawing.WorldToScreen(targetE().Position);
                       //         Drawing.DrawLine(wtsp.X, wtsp.Y, wtst.X, wtst.Y, 5f, System.Drawing.Color.Red);
                             //   Drawing.DrawCircle(Player.Position, 100, System.Drawing.Color.Yellow);
                                var d = targetE().Position.Distance(Program.Efinishpos(targetE()));
                                for (var i = 0; i < d; i += 10)
                                {
                                    var dist = i > d ? d : i;
                                    var point = targetE().Position.Extend(Program.Efinishpos(targetE()), dist);
                                    Render.Circle.DrawCircle(point, 1, System.Drawing.Color.YellowGreen);
                                }
                            }
        }

       public static Vector3 Efinishpos(Obj_AI_Base ts)
        {
            return Player.Position.Extend(ts.Position, ObjectManager.Player.Distance(ts.Position) + 490);
        
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (menu.Item("combokey").GetValue<KeyBind>().Active)
            {
                Combo.Combo.Do();
            }
            if (menu.Item("lanekey").GetValue<KeyBind>().Active)
            {
                Combo.LaneClear.Do();
            }
            if (menu.Item("junglekey").GetValue<KeyBind>().Active)
            {
                Combo.JungleClear.Do();
            }
        }

        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
                if (menu.Item("UseEInterrupt").GetValue<bool>())
            for (int i = 0; i < InterruptNum; i++)
            {
        if( menu.Item("S" + i).GetValue<bool>())
        {

            if (args.SData.Name == InterruptSpell[i])
            {
                if (Player.Distance(hero) <= 550)
                {
                    Program.E.Cast(hero);
                }
            }
        }
            }

                if (menu.Item("UseEGapclose").GetValue<bool>())
                    for (int i = 0; i < InterruptNum; i++)
                    {
                        if (menu.Item("G" + i).GetValue<bool>())
                        {

                            if (args.SData.Name == Gapcloser[i])
                            {
                                if (Player.Distance(hero) <= 550)
                                {
                                    Program.E.Cast(hero);
                                }
                            }
                        }
                    }
        }

        public static SpellSlot Trans(int i)
        {
            switch (i)
            {
                case 0:
                    return SpellSlot.Q;
                case 1:
                    return SpellSlot.W;
                case 2:
                    return SpellSlot.E;
                case 3:
                    return SpellSlot.R;
            }
            return SpellSlot.Q;
        }
        private static void Menu()
        {
            menu = new Menu("GodModeOn Vayne", "GodModeOn Vayne", true);
            var orbWalkerMenu = new Menu("Orbwalker", "Orbwalker");
            orb = new Orbwalking.Orbwalker(orbWalkerMenu);
            var targetSelectorMenu = new Menu("TargetSelector", "TargetSelector");


            var comboMenu = new Menu("Combo", "Combo");
            {
                comboMenu.AddItem(new MenuItem("QC", "Use Q in combo").SetValue(true));
                comboMenu.AddItem(new MenuItem("EC", "AutoEWall").SetValue(true));
                comboMenu.AddItem(new MenuItem("ECT", "OnlyUse E to sel. target").SetValue(true));
                comboMenu.AddItem(new MenuItem("combokey", "Combo key").SetValue(new KeyBind(32, KeyBindType.Press)));
                comboMenu.AddItem(new MenuItem("RC", "Use R in combo").SetValue(false));
                comboMenu.AddItem(new MenuItem("CNr", "Num enemys for R")).SetValue(new Slider(2, 1, 5));
                //        ComboMenu.AddItem(new MenuItem());
                //        ComboMenu.AddItem(new MenuItem("EC", "Q+E").SetValue(true));
            }
            #region  hola
            List<String> spellsin=new List<string>();
            foreach (AIHeroClient hero in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                  //  hero.GetSpell(Trans(i)).Name;
                    foreach (String s in Interrupt)
                    {
                        if (s == hero.GetSpell(Trans(i)).Name)
                        {
                            spellsin.Add("["+hero.ChampionName+"]"+s);
                        }
                    }
                } 
            }
            InterruptSpell = spellsin;
            int num = 0;
                     var interruptMenu = new Menu("SpellInterrupt", "E Interrupt spells");
            {
                interruptMenu.AddItem(new MenuItem("UseEInterrupt", "Use E Interrupt").SetValue(true));
         foreach (String s in spellsin)
                {
                interruptMenu.AddItem(new MenuItem("S"+num, s).SetValue(true));
                    num++;
                }
            }
            InterruptNum = num;
            List<String> spellgap = new List<string>();
            foreach (AIHeroClient hero in HeroManager.Enemies)
            {
                for (int i = 0; i < 4; i++)
                {
                    foreach (String s in Gapcloser)
                    {
                        if (s == hero.GetSpell(Trans(i)).Name)
                        {
                           spellgap.Add("[" + hero.ChampionName + "]" + s);
                        }
                    }
                }
            }
            int numg = 0;
            InterruptSpell = spellgap;
            var GapCloserMenu = new Menu("SpellGapcloser", "E to Gapcloser");
            {
                GapCloserMenu.AddItem(new MenuItem("UseEGapcloser", "Use E Gapcloser").SetValue(true));
                foreach (String s in spellgap)
                {
                GapCloserMenu.AddItem(new MenuItem("G" + numg, s).SetValue(true));
                    numg++;
                }
            }
            numg = GapcloserNum;
            #endregion
            var harrashMenu = new Menu("Harrash", "Harrash");
            {
                harrashMenu.AddItem(new MenuItem("QC", "Use Q in Harrash").SetValue(true));
                harrashMenu.AddItem(new MenuItem("harrashkey", "Harrash key").SetValue(new KeyBind('C', KeyBindType.Press)));
                
            }
            var laneClearMenu = new Menu("LaneClear", "LaneClear");
            {
                laneClearMenu.AddItem(new MenuItem("QL", "Use Q in laneclear").SetValue(true));
               laneClearMenu.AddItem(new MenuItem("lanekey", "LaneClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            var jungleClearMenu = new Menu("JungleClear", "JungleClear");
            {
                jungleClearMenu.AddItem(new MenuItem("QJ", "Use Q in Jungleclear").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("EJ", "Use E in Jungleclear").SetValue(true));
                jungleClearMenu.AddItem(new MenuItem("junglekey", "JungleClear key").SetValue(new KeyBind('V', KeyBindType.Press)));
            }
            var drawMenu = new Menu("Draw", "Draw");
            {
                drawMenu.AddItem(new MenuItem("DE", "Draw e line").SetValue(true));
                drawMenu.AddItem(new MenuItem("DR", "Draw R range").SetValue(true));
            }
            TargetSelector.AddToMenu(targetSelectorMenu);
            menu.AddSubMenu(orbWalkerMenu);
            menu.AddSubMenu(targetSelectorMenu);
            menu.AddSubMenu(comboMenu);
            menu.AddSubMenu(harrashMenu);
            menu.AddSubMenu(laneClearMenu);
            menu.AddSubMenu(jungleClearMenu);
            menu.AddSubMenu(interruptMenu);
            menu.AddSubMenu(GapCloserMenu);
            menu.AddSubMenu(drawMenu);
            menu.AddToMainMenu();
        }
    }
}
