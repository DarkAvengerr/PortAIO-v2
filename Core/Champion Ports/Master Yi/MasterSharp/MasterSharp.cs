using System;
using System.Collections.Generic;
using System.Linq;
//using DetuksSharp;
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

namespace MasterSharp
{
    internal class MasterSharp
    {
        public const string CharName = "MasterYi";

        public static Menu Config;

        public static Menu skillShotMenuq;
        public static Menu skillShotMenuw;
        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        public MasterSharp()
        {
            /* CallBAcks */
            onLoad();
        }

        private static void onLoad()
        {
            if (ObjectManager.Player.ChampionName != CharName)
                return;
            
            Chat.Print("MasterYi -  by DeTuKs");
            MasterYi.setSkillShots();
            try
            {
                TargetedSkills.setUpSkills();

                Config = new Menu("MasterYi - Sharp", "MasterYi", true);
                var orbwalkerMenu = new Menu("DeathWalker", "my_Orbwalker");
                Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
                Config.AddSubMenu(orbwalkerMenu);

                //TS
                Menu targetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(targetSelectorMenu);
                Config.AddSubMenu(targetSelectorMenu);
                //Combo
                Config.AddSubMenu(new Menu("Combo Sharp", "combo"));
                Config.SubMenu("combo").AddItem(new MenuItem("comboItems", "Meh everything is fine here")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("comboWreset", "AA reset W")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q to gap")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useR", "Use R")).SetValue(true);
                Config.SubMenu("combo").AddItem(new MenuItem("useSmite", "Use Smite")).SetValue(true);

                //Extra
                Config.AddSubMenu(new Menu("Extra Sharp", "extra"));
                Config.SubMenu("extra").AddItem(new MenuItem("packets", "Use Packet cast")).SetValue(false);

                Config.AddSubMenu(new Menu("Anti Skillshots", "aShots"));
                //SmartW
                Config.SubMenu("aShots").AddItem(new MenuItem("smartW", "Smart W if cantQ")).SetValue(true);
                Config.SubMenu("aShots").AddItem(new MenuItem("smartQDogue", "Q use dogue")).SetValue(true);
                Config.SubMenu("aShots").AddItem(new MenuItem("useWatHP", "use W below HP")).SetValue(new Slider(100,0,100));
                Config.SubMenu("aShots").AddItem(new MenuItem("wqOnDead", "W or Q if will kill")).SetValue(false);
                skillShotMenuq = getSkilshotMenuQ();
                Config.SubMenu("aShots").AddSubMenu(skillShotMenuq);
                skillShotMenuw = getSkilshotMenuW();
                Config.SubMenu("aShots").AddSubMenu(skillShotMenuw);

                //Debug
                Config.AddSubMenu(new Menu("Drawing", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("drawCir", "Draw circles")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("debugOn", "Debug stuff")).SetValue(new KeyBind('A', KeyBindType.Press));

                Config.AddToMainMenu();
                Drawing.OnDraw += onDraw;

                Game.OnUpdate += OnGameUpdate;

                Obj_AI_Base.OnSpellCast += OnProcessSpell;

                AttackableUnit.OnDamage += onDamage;

                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
                //Game.OnProcessPacket += OnGameProcessPacket;
                CustomEvents.Unit.OnDash += onDash;
                Orbwalking.AfterAttack += afterAttack;


            }
            catch
            {
                Chat.Print("Oops. Something went wrong with Jayce - Sharp");
            }

        }

        public static bool isYiAA(DamageType type)
        {
            //if(type == )
            return true;
        }

        private static void onDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            return;
            try
            {
                if (args.Source.NetworkId == MasterYi.player.NetworkId)
                Console.WriteLine("type: " + args.Type + " : "+ args.HitType);
                if (args.Source.NetworkId != MasterYi.player.NetworkId || !MasterYi.W.IsReady() || Orbwalking.CanAttack() || !isYiAA(args.Type))
                    return;


                GameObject targ = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Target.NetworkId);
                
                if (targ == null)
                    return;
               // Console.WriteLine("dmg: " + args.Damage + " type " + args.Type + " dmg type: " + args.HitType + " pred dmg: "+ MasterYi.player.GetAutoAttackDamage(targ));

               // if (args.Type == DamageType.Physical)
               // {
                if (targ is AIHeroClient)
                {
                    if (Config.Item("comboWreset").GetValue<bool>())
                        MasterYi.W.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
                // }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void afterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (MasterYi.W.IsReady() && Config.Item("comboWreset").GetValue<bool>() && Config.Item("useWatHP").GetValue<Slider>().Value>=MasterYi.player.HealthPercent && target is AIHeroClient && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                MasterYi.W.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add(100, delegate { Orbwalking.ResetAutoAttackTimer(); });
                
            }
        }

        private static void onDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (MasterYi.selectedTarget != null && sender.NetworkId == MasterYi.selectedTarget.NetworkId &&
                MasterYi.Q.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                && sender.Distance(MasterYi.player)<=600)
                MasterYi.Q.Cast(sender);
        }

        public static void OnGameProcessPacket(GamePacketEventArgs args)
        {
            return;

            if (Config.Item("comboWreset").GetValue<bool>() && args.PacketData[0] == 0x65 && MasterYi.W.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {

                // LogPacket(args);
                GamePacket gp = new GamePacket(args.PacketData);
                gp.Position = 1;
                Packet.S2C.Damage.Struct dmg = Packet.S2C.Damage.Decoded(args.PacketData);

                int targetID = gp.ReadInteger();
                int dType = (int) gp.ReadByte();
                int Unknown = gp.ReadShort();
                float DamageAmount = gp.ReadFloat();
                int TargetNetworkIdCopy = gp.ReadInteger();
                int SourceNetworkId = gp.ReadInteger();
                float dmga =
                    (float)
                        MasterYi.player.GetAutoAttackDamage(
                            ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)targetID));
                if (dmga - 10 > DamageAmount || dmga + 10 < DamageAmount)
                    return;
                if (MasterYi.player.NetworkId != dmg.SourceNetworkId && MasterYi.player.NetworkId == targetID)
                    return;
                Obj_AI_Base targ = ObjectManager.GetUnitByNetworkId<Obj_AI_Base>((uint)dmg.TargetNetworkId);
                if ((int) dmg.Type == 12 || (int) dmg.Type == 4 || (int) dmg.Type == 3 )
                {
                    if (MasterYi.W.IsReady() && Orbwalker.InAutoAttackRange(targ))
                    {
                        MasterYi.W.Cast(targ.Position);
                       // DeathWalker.ResetAutoAttackTimer();
                    }
                }
                   // Console.WriteLine("dtyoe: " + dType);
            }
        }

        public static Menu getSkilshotMenuQ()
        {
            //Create the skillshots submenus.
            var skillShotsQ = new Menu("Evade with Q", "aShotsSkillsQ");

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            var subMenu = new Menu(spell.MenuItemName, spell.MenuItemName);

                            subMenu.AddItem(
                                new MenuItem("qEvadeAll" + spell.MenuItemName, "Evade with Q Allways").SetValue(
                                    spell.IsDangerous));

                            subMenu.AddItem(
                                new MenuItem("qEvade" + spell.MenuItemName, "Evade with Q Combo").SetValue(
                                    spell.IsDangerous));

                            skillShotsQ.AddSubMenu(subMenu);
                        }
                    }
                }
            }
            return skillShotsQ;
        }

        public static Menu getSkilshotMenuW()
        {
            //Create the skillshots submenus.
            var skillShotsW = new Menu("Evade with W", "aShotsSkillsW");

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                if (hero.Team != ObjectManager.Player.Team)
                {
                    foreach (var spell in SpellDatabase.Spells)
                    {
                        if (spell.ChampionName == hero.ChampionName)
                        {
                            var subMenu = new Menu(spell.MenuItemName, spell.MenuItemName);

                            subMenu.AddItem(
                                new MenuItem("wEvadeAll" + spell.MenuItemName, "Evade with W allways").SetValue(
                                    spell.IsDangerous));

                            subMenu.AddItem(
                                new MenuItem("wEvade" + spell.MenuItemName, "Evade with W COmbo").SetValue(
                                    spell.IsDangerous));

                            skillShotsW.AddSubMenu(subMenu);
                        }
                    }
                }
            }
            return skillShotsW;
        }

        public static bool skillShotMustBeEvaded(string Name)
        {
            if (skillShotMenuq.Item("qEvade" + Name) != null)
            {
                return skillShotMenuq.Item("qEvade" + Name).GetValue<bool>();
            }
            return true;
        }

        public static bool skillShotMustBeEvadedAllways(string Name)
        {
            if (skillShotMenuq.Item("qEvadeAll" + Name) != null)
            {
                return skillShotMenuq.Item("qEvadeAll" + Name).GetValue<bool>();
            }
            return true;
        }

        public static bool skillShotMustBeEvadedW(string Name)
        {
            if (skillShotMenuw.Item("wEvade" + Name) != null)
            {
                return skillShotMenuw.Item("wEvade" + Name).GetValue<bool>();
            }
            return true;
        }

        public static bool skillShotMustBeEvadedWAllways(string Name)
        {
            if (skillShotMenuw.Item("wEvade" + Name) != null)
            {
                return skillShotMenuw.Item("wEvade" + Name).GetValue<bool>();
            }
            return true;
        }

        private static void OnGameUpdate(EventArgs args)
        {

            if (Config.Item("debugOn").GetValue<KeyBind>().Active) //fullDMG
            {
                foreach (var buf in MasterYi.player.Buffs)
                {
                    Console.WriteLine(buf.Name);
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AIHeroClient target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Physical);
                Orbwalker.ForceTarget(target);
                if(target != null)
                    MasterYi.selectedTarget = target;
                MasterYi.slayMaderDuker(target);
            }

            DetectedSkillshots.RemoveAll(skillshot => !skillshot.IsActive());
            foreach (var skillShot in DetectedSkillshots)
            {
                if (skillShot.IsAboutToHit(250, MasterYi.player))
                {
                    MasterYi.evadeSkillShot(skillShot);
                }
            }

            //anti buferino
            foreach (var buf in MasterYi.player.Buffs)
            {
                TargetedSkills.TargSkill skill = TargetedSkills.dagerousBuffs.FirstOrDefault(ob => ob.sName.ToLower() == buf.Name.ToLower());
                if (skill != null)
                {
                   // Console.WriteLine("Evade: " + buf.Name);
                    MasterYi.evadeBuff(buf,skill);
                }
                // if(buf.EndTime-Game.Time<0.2f)
            }
            

        }

        private static void onDraw(EventArgs args)
        {

            if (!Config.Item("drawCir").GetValue<bool>())
                return;
            LeagueSharp.Common.Utility.DrawCircle(MasterYi.player.Position, 600, Color.Green);
           
        }



        public static void OnProcessSpell(Obj_AI_Base obj, GameObjectProcessSpellCastEventArgs arg)
        {
            if (arg.Target != null && arg.Target.NetworkId == MasterYi.player.NetworkId)
            {
                //Console.WriteLine(arg.SData.Name);
                if (obj is AIHeroClient)
                {

                    var hero = (AIHeroClient) obj;
                    var spellSlot = (hero.GetSpellSlot(arg.SData.Name));
                    TargetedSkills.TargSkill skill = TargetedSkills.targetedSkillsAll.FirstOrDefault(ob => ob.sName == arg.SData.Name);
                    if (skill != null)
                    {
                        //Console.WriteLine("Evade: " + arg.SData.Name);
                        MasterYi.evadeDamage(skill.useQ,skill.useW,arg,skill.delay);
                    }

                }
            }
        }


        private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
        {
            if (skillshot.SpellData.SpellName == "VelkozQ")
            {
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                var direction = skillshot.Direction.Perpendicular();
                if (DetectedSkillshots.Count(s => s.SpellData.SpellName == "VelkozQSplit") == 0)
                {
                    for (var i = -1; i <= 1; i = i + 2)
                    {
                        var skillshotToAdd = new Skillshot(
                            DetectionType.ProcessSpell, spellData, Environment.TickCount, missile.Position.To2D(),
                            missile.Position.To2D() + i * direction * spellData.Range, skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                    }
                }
            }
        }

        private static void OnDetectSkillshot(Skillshot skillshot)
        {
            var alreadyAdded = false;

            foreach (var item in DetectedSkillshots)
            {
                if (item.SpellData.SpellName == skillshot.SpellData.SpellName &&
                    (item.Unit.NetworkId == skillshot.Unit.NetworkId &&
                     (skillshot.Direction).AngleBetween(item.Direction) < 5 &&
                     (skillshot.Start.Distance(item.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0)))
                {
                    alreadyAdded = true;
                }
            }

            //Check if the skillshot is from an ally.
            if (skillshot.Unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            //Check if the skillshot is too far away.
            if (skillshot.Start.Distance(ObjectManager.Player.ServerPosition.To2D()) >
                (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
            {
                return;
            }

            //Add the skillshot to the detected skillshot list.
            if (!alreadyAdded)
            {
                //Multiple skillshots like twisted fate Q.
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;

                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                            i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                            i++)
                        {
                            var end = skillshot.Start +
                                      skillshot.SpellData.Range *
                                      originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i);
                            var skillshotToAdd = new Skillshot(
                                skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                                skillshot.Unit);

                            DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }

                    if (skillshot.SpellData.Invert)
                    {
                        var newDirection = -(skillshot.End - skillshot.Start).Normalized();
                        var end = skillshot.Start + newDirection * skillshot.Start.Distance(skillshot.End);
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, skillshot.Start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.Centered)
                    {
                        var start = skillshot.Start - skillshot.Direction * skillshot.SpellData.Range;
                        var end = skillshot.Start + skillshot.Direction * skillshot.SpellData.Range;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        var angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.ServerPosition.To2D()).Rotated(
                                -angle / 2 * (float)Math.PI / 180);
                        var edge2 = edge1.Rotated(angle * (float)Math.PI / 180);

                        foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            var v = minion.ServerPosition.To2D() - skillshot.Unit.ServerPosition.To2D();
                            if (minion.Name == "Seed" && edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                                minion.Distance(skillshot.Unit) < 800 &&
                                (minion.Team != ObjectManager.Player.Team))
                            {
                                var start = minion.ServerPosition.To2D();
                                var end = skillshot.Unit.ServerPosition.To2D()
                                    .Extend(
                                        minion.ServerPosition.To2D(),
                                        skillshot.Unit.Distance(minion) > 200 ? 1300 : 1000);

                                var skillshotToAdd = new Skillshot(
                                    skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                                    skillshot.Unit);
                                DetectedSkillshots.Add(skillshotToAdd);
                            }
                        }
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        var start = skillshot.End - skillshot.Direction.Perpendicular() * 400;
                        var end = skillshot.End + skillshot.Direction.Perpendicular() * 400;
                        var skillshotToAdd = new Skillshot(
                            skillshot.DetectionType, skillshot.SpellData, skillshot.StartTick, start, end,
                            skillshot.Unit);
                        DetectedSkillshots.Add(skillshotToAdd);
                        return;
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;


                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");

                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;

                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);

                        var bounce1 = new Skillshot(
                            skillshot.DetectionType, bounce1SpellData, skillshot.StartTick, skillshot.End, bounce1Pos,
                            skillshot.Unit);
                        var bounce2 = new Skillshot(
                            skillshot.DetectionType, bounce2SpellData, skillshot.StartTick, bounce1Pos, bounce2Pos,
                            skillshot.Unit);

                        DetectedSkillshots.Add(bounce1);
                        DetectedSkillshots.Add(bounce2);
                    }

                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                    }

                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();

                        foreach (var s in DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                endPos = s.End;
                            }
                        }

                        foreach (var m in ObjectManager.Get<Obj_AI_Minion>())
                        {
                            if (m.BaseSkinName == "jarvanivstandard" && m.Team == skillshot.Unit.Team &&
                                skillshot.IsDanger(m.Position.To2D()))
                            {
                                endPos = m.Position.To2D();
                            }
                        }

                        if (!endPos.IsValid())
                        {
                            return;
                        }

                        skillshot.End = endPos + 200 * (endPos - skillshot.Start).Normalized();
                        skillshot.Direction = (skillshot.End - skillshot.Start).Normalized();
                    }
                }

                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    var endCSpellData = SpellDatabase.GetByName("OriannaQend");

                    var skillshotToAdd = new Skillshot(
                        skillshot.DetectionType, endCSpellData, skillshot.StartTick, skillshot.Start, skillshot.End,
                        skillshot.Unit);

                    DetectedSkillshots.Add(skillshotToAdd);
                }


                //Dont allow fow detection.
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
#if DEBUG
                Console.WriteLine(Environment.TickCount + "Adding new skillshot: " + skillshot.SpellData.SpellName);
#endif

                DetectedSkillshots.Add(skillshot);
            }
        }

    }
}
