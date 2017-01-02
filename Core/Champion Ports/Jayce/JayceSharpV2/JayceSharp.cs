using System;
using System.Linq;
using System.Runtime.InteropServices;
using DetuksSharp;
using LeagueSharp;
using LeagueSharp.Common;
/*
 * ToDo:
 * Q doesnt shoot much < fixed
 * Full combo burst <-- done
 * Useles gate <-- fixed
 * Add Fulldmg combo starting from hammer <-- done
 * Auto ignite if killabe/burst <-- done
 * More advanced Q calc area on hit
 * MuraMune support <-- done
 * Auto gapclosers E <-- done
 * GhostBBlade active <-- done
 * packet cast E <-- done 
 * 
 * 
 * Auto ks with QE <-done
 * Interupt channel spells <-done
 * Omen support <- done
 * 
 * 
 * */
using SharpDX;
using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JayceSharpV2
{
    internal class JayceSharp
    {
        public const string CharName = "Jayce";

        public static Menu Config;

        

        internal static Orbwalking.Orbwalker Orbwalker;


        public JayceSharp()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (ObjectManager.Player.ChampionName != CharName)
                return;

            Console.WriteLine("Jayce - SharpV2 by DeTuKs");
            Jayce.setSkillShots();
            try
            {

                Config = new Menu("Jayce - Sharp", "Jayce", true);
                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));
                //TS
                Menu targetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(targetSelectorMenu);
                Config.AddSubMenu(targetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Use Items")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("hammerKill", "Hammer if killable")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("parlelE", "use pralel gate")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("fullDMG", "Do full damage")).SetValue(new KeyBind('A', KeyBindType.Press));
                Config.SubMenu("combo").AddItem(new MenuItem("injTarget", "Tower Injection")).SetValue(new KeyBind('G', KeyBindType.Press));
                Config.SubMenu("combo").AddItem(new MenuItem("awsPress", "Press for awsomeee!!")).SetValue(new KeyBind('Z', KeyBindType.Press));
                Config.SubMenu("combo").AddItem(new MenuItem("eAway", "Gate distance from side")).SetValue(new Slider(20,3,60));

                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("shoot", "Shoot manual Q")).SetValue(new KeyBind('T', KeyBindType.Press));
               
                Config.SubMenu("extra").AddItem(new MenuItem("gapClose", "Kick Gapclosers")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("autoInter", "Interupt spells")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("useMunions", "Q use Minion colision")).SetValue(true);
                Config.SubMenu("extra").AddItem(new MenuItem("killSteal", "Killsteal")).SetValue(false);
                Config.SubMenu("extra").AddItem(new MenuItem("packets", "Use Packet cast")).SetValue(false);

                //Debug
                Config.AddSubMenu(new Menu("Drawing", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("drawCir", "Draw circles")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawCD", "Draw CD")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawFull", "Draw full combo dmg")).SetValue(true);

                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;
                Drawing.OnEndScene += OnEndScene;

                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += onCreate;
                GameObject.OnDelete += onDelete;

                Obj_AI_Base.OnDamage += onDamage;

                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
                Interrupter2.OnInterruptableTarget += OnPosibleToInterrupt;
                //SmoothMouse.start();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Chat.Print("Oops. Something went wrong with Jayce - Sharp");
            }

        }

        private static void onDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Source.NetworkId == Jayce.Player.NetworkId)
            {
            }
        }

        private static void onDelete(GameObject sender, EventArgs args)
        {
            if (Jayce.myCastedQ != null && Jayce.myCastedQ.NetworkId == sender.NetworkId)
            {
                Jayce.myCastedQ = null;
                Jayce.castedQon = null;
            }
        }

        private static void onCreate(GameObject sender, EventArgs args)
        {
            //Console.WriteLine(sender.Name+" TYPE: "+sender.Type);

            if (sender is Obj_AI_Minion && sender.Name == "hiu" && Jayce.E1.IsReady())
            {
            }

            if (sender is MissileClient)
            {
                var mis = (MissileClient)sender;
                if (mis.SpellCaster.IsMe)
                {
                    //Console.WriteLine("My MIssle rdy");
                    Jayce.myCastedQ = mis;
                }

            }
        }

        private static void OnPosibleToInterrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {

            if (Config.Item("autoInter").GetValue<bool>() && (int)args.DangerLevel > 0)
                Jayce.knockAway(sender);
        }

        private static void OnEndScene(EventArgs args)
        {
        }


        private static void OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("gapClose").GetValue<bool>())
                Jayce.knockAway(gapcloser.Sender);
        }

        private static void OnGameUpdate(EventArgs args)
        {

            Jayce.checkForm();
            Jayce.processCDs();
            if (Config.Item("shoot").GetValue<KeyBind>().Active)
            {
                Jayce.shootQE(Game.CursorPos,true);
            }
            if (Jayce.myCastedQ != null && (Config.Item("fullDMG").GetValue<KeyBind>().Active || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo))
            {
                    Jayce.castEonSpell(Jayce.myCastedQ);
            }
            /* if (Jayce.castedQon != null && !Jayce.isHammer)
            {
                if((Jayce.getJayceEQDmg(Jayce.castedQon) > Jayce.castedQon.Health ||
                 Jayce.castedQon.Distance(Jayce.Player) > Jayce.E1.Range || !Config.Item("useExploit").GetValue<bool>()))
                {

                    if (Jayce.castQon.X != 0)
                        Jayce.shootQE(Jayce.castQon);
                }
                else
                {
                    Jayce.doExploit(Jayce.castedQon);
                }
            }*/
            //}

            

            if (Config.Item("fullDMG").GetValue<KeyBind>().Active)//fullDMG
            {
                Jayce.activateMura();
                AIHeroClient target = TargetSelector.GetTarget(Jayce.getBestRange(), TargetSelector.DamageType.Physical);
                if (Jayce.lockedTarg == null)
                    Jayce.lockedTarg = target;
                Jayce.doFullDmg(Jayce.lockedTarg);
            }
            else
            {
                Jayce.lockedTarg = null;
            }

            if (Config.Item("injTarget").GetValue<KeyBind>().Active)//fullDMG
            {
                Jayce.activateMura();
                AIHeroClient target = TargetSelector.GetTarget(Jayce.getBestRange(), TargetSelector.DamageType.Physical);
                if (Jayce.lockedTarg == null)
                    Jayce.lockedTarg = target;
                Jayce.doJayceInj(Jayce.lockedTarg);
            }
            else
            {
                Jayce.lockedTarg = null;
            }

           // if (Jayce.castEonQ != null && (Jayce.castEonQ. - 2) > Game.Time)
            //    Jayce.castEonQ = null;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Jayce.activateMura();
                AIHeroClient target = TargetSelector.GetTarget(Jayce.getBestRange(), TargetSelector.DamageType.Physical);
                Jayce.doCombo(target);
            }

            if (Config.Item("killSteal").GetValue<bool>())
                Jayce.doKillSteal();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Jayce.deActivateMura();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                Jayce.deActivateMura();
            }
        }

        private static void onDraw(EventArgs args)
        {
            //Draw CD
            if (Config.Item("drawCD").GetValue<bool>())
                Jayce.drawCD();

            if (!Config.Item("drawCir").GetValue<bool>())
                return;
            LeagueSharp.Common.Utility.DrawCircle(Jayce.Player.Position, !Jayce.isHammer ? 1100 : 600, Color.Red);

            LeagueSharp.Common.Utility.DrawCircle(Jayce.Player.Position, 1550, Color.Violet);
        }



        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {

                if (arg.SData.Name == "jayceshockblast")
                {
                    //  Jayce.castEonQ = arg;
                }
                else if (arg.SData.Name == "jayceaccelerationgate")
                {
                    Jayce.castEonQ = null;
                    // Console.WriteLine("Cast dat E on: " + arg.SData.Name);
                }
                Jayce.getCDs(arg);
            }
        }

    }
}
