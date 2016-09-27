using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using LeagueSharp;
using LeagueSharp.Common;
/*TODO
 * Combo calc and choose best <-- kinda
 * Farming
 * Interupt
 * 
 * gap close with q < -- done
 * 
 * mash q if les hp < -- done
 * 
 * smart cancel combos < -- yup
 * 
 * gap kill <-- yup
 * 
 * overkill 
 * 
 * harass to trade good <-- done
 * 
 * 
 * fix ignite
 * 
 * R KS
 * 
 */
using Rive;
using SharpDX;
using Color = System.Drawing.Color;

using EloBuddy;
using LeagueSharp.Common;
namespace RivenSharp
{
    class RivenSharp
    {

        public const string CharName = "Riven";

        public static Menu Config;


        public static HpBarIndicator hpi = new HpBarIndicator();


        public RivenSharp()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            try
            {
                if (Riven.Player.ChampionName != "Riven") return;

                Chat.Print("RivenSharp by DeTuKs");
                Config = new Menu("Riven - Sharp", "Riven", true);
                //Orbwalkervar menu = new Menu("My Mainmenu", "my_mainmenu", true);
                var orbwalkerMenu = new Menu("LX Orbwalker", "my_Orbwalker");
                LXOrbwalker.AddToMenu(orbwalkerMenu);
                Config.AddSubMenu(orbwalkerMenu);
                //TS
                var TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(TargetSelectorMenu);
                Config.AddSubMenu(TargetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R on combo (Shuld be on)")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("forceQE", "Use Q after E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("packets", "Use packet cast")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("rush", "Rush with Q")).SetValue(true);

                //Haras
                Config.AddSubMenu(new Menu("Harass Sharp", "haras"));
                Config.SubMenu("haras").AddItem(new MenuItem("doHarasE", "Harass enemy E")).SetValue(new KeyBind('G', KeyBindType.Press, false));
                Config.SubMenu("haras").AddItem(new MenuItem("doHarasQ", "Harass enemy Q")).SetValue(new KeyBind('T', KeyBindType.Press, false));

                //Drawing
                Config.AddSubMenu(new Menu("Drawing Sharp", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("doDraw", "Dissable drawings")).SetValue(false);
                Config.SubMenu("draw").AddItem(new MenuItem("drawHp", "Draw pred hp")).SetValue(true);

                //Debug
                Config.AddSubMenu(new Menu("Debug", "debug"));
                Config.SubMenu("debug").AddItem(new MenuItem("db_targ", "Debug Target")).SetValue(new KeyBind('0', KeyBindType.Press, false));

                Config.AddToMainMenu();

                Drawing.OnDraw += onDraw;
                Drawing.OnEndScene += OnEndScene;
                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += OnCreateObject;
                GameObject.OnDelete += OnDeleteObject;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
                Obj_AI_Base.OnNewPath += OnNewPath;
                Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;

                AttackableUnit.OnDamage += onDamage;

                Game.OnSendPacket += OnGameSendPacket;
                Game.OnProcessPacket += OnGameProcessPacket;

                Riven.setSkillshots();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void onDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Source.NetworkId != Riven.Player.NetworkId || !isComboing() || LXOrbwalker.CanAttack() || !Riven.Q.IsReady())
                return;


            Obj_AI_Base targ = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)args.Target.NetworkId);
            if (targ == null)
                return;
            //Chat.Print("dmg: " + args.Damage + " type " + args.Type + " dmg type: " + args.HitType + " pred dmg: "+ Riven.Player.GetAutoAttackDamage(targ));

            // if (args.Type == DamageType.Physical && (args.HitType == DamageHitType.Normal || args.HitType == DamageHitType.Dodge))
            // {

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, targ.Position);
            //if (targ is AIHeroClient)
            Riven.Q.Cast(targ.Position);
            //  }
        }

        private static void OnPlayAnimation(GameObject sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe && args.Animation.Contains("Spell") && isComboing())
            {

                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 50, delegate { Riven.cancelAnim(); });
            }
        }

        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && Riven.Q.IsReady(500))
            {
                LXOrbwalker.ResetAutoAttackTimer();
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config.Item("drawHp").GetValue<bool>())
            {
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(Riven.rushDmgBasedOnDist(enemy), Color.Yellow);

                }
            }
        }

        /*
         * 
         */
        private static void OnGameUpdate(EventArgs args)
        {
            /*
                RivenFengShuiEngine
                rivenwindslashready
             */
            try
            {

                if (Config.Item("doHarasE").GetValue<KeyBind>().Active)
                {
                    AIHeroClient target = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);
                    LXOrbwalker.ForcedTarget = target;
                    Riven.doHarasE(target);
                }
                else if (Config.Item("doHarasQ").GetValue<KeyBind>().Active)
                {
                    AIHeroClient target = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);
                    LXOrbwalker.ForcedTarget = target;
                    Riven.doHarasQ(target);
                }


                if (LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
                {
                    AIHeroClient target = TargetSelector.GetTarget(1400, TargetSelector.DamageType.Physical);
                    LXOrbwalker.ForcedTarget = target;
                    Riven.doCombo(target);
                    //Console.WriteLine(target.NetworkId);
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex);
            }
        }


        private static void onDraw(EventArgs args)
        {
            try
            {

                if (!Config.Item("doDraw").GetValue<bool>())
                {

                    if (Config.Item("drawHp").GetValue<bool>())
                    {
                        foreach (
                            var enemy in
                                ObjectManager.Get<AIHeroClient>()
                                    .Where(ene => !ene.IsDead && ene.IsEnemy && ene.IsVisible))
                        {
                            hpi.unit = enemy;
                            hpi.drawDmg(Riven.rushDmgBasedOnDist(enemy), Color.Yellow);

                        }
                    }
                    foreach (
                        AIHeroClient enHero in
                            ObjectManager.Get<AIHeroClient>().Where(enHero => enHero.IsEnemy && enHero.Health > 0))
                    {
                        LeagueSharp.Common.Utility.DrawCircle(enHero.Position,
                            enHero.BoundingRadius + Riven.E.Range + Riven.Player.AttackRange,
                            (Riven.rushDown) ? Color.Red : Color.Blue);
                        //Drawing.DrawCircle(enHero.Position, enHero.BoundingRadius + Riven.E.Range+Riven.Player.AttackRange, Color.Blue);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            // if (sender.Name.Contains("missile") || sender.Name.Contains("Minion"))
            //     return;
            // Console.WriteLine("Object: " + sender.Name);
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {

        }

        public static bool isComboing()
        {
            if (Config.Item("doHarasE").GetValue<KeyBind>().Active ||
                Config.Item("doHarasQ").GetValue<KeyBind>().Active
                || LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
            {
                return true;
            }

            return false;
        }


        public static void OnProcessSpell(EloBuddy.Obj_AI_Base sender, EloBuddy.GameObjectProcessSpellCastEventArgs arg)
        {
            if (!sender.IsMe)
                return;

            // Chat.Print(arg.SData.Name);

            if (Config.Item("forceQE").GetValue<bool>() && sender.IsMe && arg.SData.Name.Contains("RivenFeint") && Riven.Q.IsReady() && LXOrbwalker.GetPossibleTarget() != null)
            {
                Console.WriteLine("force q");
                Riven.Q.Cast(LXOrbwalker.GetPossibleTarget().Position);
                Riven.forceQ = true;
                // Riven.timer = new System.Threading.Timer(obj => { Riven.EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Riven.difPos()); }, null, (long)100, System.Threading.Timeout.Infinite);
            }

            if (arg.SData.Name.Contains("RivenFeint") || arg.SData.Name.Contains("TriCleave") || arg.SData.Name.Contains("RivenFMartyr"))
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + LXOrbwalker.GetCurrentWindupTime() + 50, delegate { Riven.cancelAnim(); });

            if (arg.SData.Name.Contains("RivenFeint") && Riven.R.IsReady() && Config.Item("useR").GetValue<bool>())
            {
                LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 50, delegate { Riven.useRSmart(LXOrbwalker.GetPossibleTarget()); });
            }
        }



        public static int lastTargetId = 0;

        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {
            return;
            try
            {


                if (isComboing())
                {
                    if (args.PacketData[0] == 35 && Riven.Q.IsReady())
                    {
                        Console.WriteLine("Gott");
                        GamePacket gp = new GamePacket(args.PacketData);
                        gp.Position = 2;
                        int netId = gp.ReadInteger();
                        if (LXOrbwalker.GetPossibleTarget() == null || LXOrbwalker.GetPossibleTarget().NetworkId != netId)
                            return;
                        if (!LXOrbwalker.CanAttack())
                            Riven.Q.Cast(LXOrbwalker.GetPossibleTarget().Position);
                    }

                    if (args.PacketData[0] == 0x17)
                    {
                        Console.WriteLine("cancel");

                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 2;
                        int sourceId = packet.ReadInteger();
                        if (sourceId == Riven.Player.NetworkId)
                        {
                            Console.WriteLine("cancel wawf");
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
                            if (LXOrbwalker.GetPossibleTarget() != null)
                            {
                                Riven.moveTo(LXOrbwalker.GetPossibleTarget().Position);
                                //Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(LXOrbwalker.GetPossibleTarget().Position.X, LXOrbwalker.GetPossibleTarget().Position.Y)).Send();

                                // LXOrbwalker.ResetAutoAttackTimer();
                                Riven.cancelAnim(true);
                            }
                        }
                    }

                    if (args.PacketData[0] == 0xDF && false)
                    {

                        Console.WriteLine("cancel");

                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 2;
                        int sourceId = packet.ReadInteger();
                        if (sourceId == Riven.Player.NetworkId)
                        {
                            Console.WriteLine("cancel wawf");
                            Riven.moveTo(Game.CursorPos);
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
                            LXOrbwalker.ResetAutoAttackTimer();
                            Riven.cancelAnim();
                        }
                    }

                    if (args.PacketData[0] == 0x61) //move
                    {
                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 12;
                        int sourceId = packet.ReadInteger();
                        if (sourceId == Riven.Player.NetworkId)
                        {
                            if (LXOrbwalker.GetPossibleTarget() != null)
                            {
                                //    Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(LXOrbwalker.GetPossibleTarget().Position.X, LXOrbwalker.GetPossibleTarget().Position.Y)).Send();
                                LXOrbwalker.ResetAutoAttackTimer();
                            }
                        }
                    }
                    else if (args.PacketData[0] == 0x38) //animation2
                    {
                        GamePacket packet = new GamePacket(args.PacketData);
                        packet.Position = 1;
                        int sourceId = packet.ReadInteger();
                        if (packet.Size() == 9 && sourceId == Riven.Player.NetworkId)
                        {
                            Riven.moveTo(Game.CursorPos);
                            Packet.C2S.Move.Encoded(new Packet.C2S.Move.Struct(Game.CursorPos.X, Game.CursorPos.Y)).Send();
                            LXOrbwalker.ResetAutoAttackTimer();
                            Riven.cancelAnim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void OnGameSendPacket(GamePacketEventArgs args)
        {
            return;
            try
            {
                if (args.PacketData[0] == 119)
                    args.Process = false;

                //if (Riven.orbwalker.ActiveMode.ToString() == "Combo")
                //   LogPacket(args);
                if (args.PacketData[0] == 154 && LXOrbwalker.CurrentMode == LXOrbwalker.Mode.Combo)
                {
                    Packet.C2S.Cast.Struct cast = Packet.C2S.Cast.Decoded(args.PacketData);
                    if ((int)cast.Slot > -1 && (int)cast.Slot < 5)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + LXOrbwalker.GetCurrentWindupTime(), delegate { Riven.cancelAnim(true); });

                        //Chat.Say("/l");
                    }

                    if (cast.Slot == SpellSlot.E && Riven.R.IsReady() && Config.Item("useR").GetValue<bool>())
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 50, delegate { Riven.useRSmart(LXOrbwalker.GetPossibleTarget()); });
                    }
                    //Console.WriteLine(cast.Slot + " : " + Game.Ping);
                    /* if (cast.Slot == SpellSlot.Q)
                         Orbwalking.ResetAutoAttackTimer();
                     else if (cast.Slot == SpellSlot.W && Riven.Q.IsReady())
                         LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping+200, delegate { Riven.useHydra(Riven.orbwalker.GetTarget()); });
                     else if (cast.Slot == SpellSlot.E && Riven.W.IsReady())
                     {
                         Console.WriteLine("cast QQQQ");
                         LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping+200, delegate { Riven.useWSmart(Riven.orbwalker.GetTarget()); });
                     }
                     else if ((int)cast.Slot == 131 && Riven.W.IsReady())
                     {
                         Orbwalking.ResetAutoAttackTimer();
                         LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping +200, delegate { Riven.useWSmart(Riven.orbwalker.GetTarget()); });
                     }*/
                    // LogPacket(args);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }




    }
}
