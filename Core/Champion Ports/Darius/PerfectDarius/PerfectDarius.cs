using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Lib = PerfectDarius.PerfectLib;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace PerfectDarius
{
    internal class PerfectDarius
    {
        internal static Menu Config;
        internal static int LastGrabTimeStamp;
        internal static int LastDunkTimeStamp;

        internal static Orbwalking.Orbwalker Orbwalker;
        static Items.Item HealthPot;
        static Items.Item ManaPot;
        static Items.Item CrystalFlask;
        static SpellSlot IgniteSlot;
        public PerfectDarius()
        {
            if (ObjectManager.Player.ChampionName == "Darius")
            {
                Chat.Print("Perfect Darius v1.0");
                HealthPot = new Items.Item(2003, 0);
                ManaPot = new Items.Item(2004, 0);
                CrystalFlask = new Items.Item(2041, 0);
                IgniteSlot = ObjectManager.Player.GetSpellSlot("summonerdot");
                Menu_OnLoad();
                Game.OnUpdate += Game_OnUpdate;

                Drawing.OnDraw += Drawing_OnDraw;
                Drawing.OnEndScene += Drawing_OnEndScene;

                Orbwalking.AfterAttack += Orbwalking_AfterAttack;

                Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

                Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            }
        }

        internal static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsValidTarget(Lib.Spellbook["E"].Range) && Lib.Spellbook["E"].IsReady())
            {
                if (Config.Item("useeint").GetValue<bool>())
                {
                    Lib.Spellbook["E"].Cast(sender.ServerPosition);
                }
            }
        }


        internal static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            switch (args.SData.Name.ToLower())
            {
                case "dariuscleave":
                    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 800, Orbwalking.ResetAutoAttackTimer);
                    break;

                case "dariusaxegrabcone":
                    LastGrabTimeStamp = Utils.GameTimeTickCount;
                    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 100, Orbwalking.ResetAutoAttackTimer);
                    break;

                case "dariusexecute":
                    LastDunkTimeStamp = Utils.GameTimeTickCount;
                    LeagueSharp.Common.Utility.DelayAction.Add(Game.Ping + 350, Orbwalking.ResetAutoAttackTimer);
                    break;
            }
        }

        internal static float RModifier
        {
            get { return Config.Item("rmodi").GetValue<Slider>().Value; }
        }

        internal static float MordeShield(AIHeroClient unit)
        {
            if (unit.ChampionName != "Mordekaiser")
            {
                return 0f;
            }

            return unit.Mana;
        }

        internal static int PassiveCount(Obj_AI_Base unit)
        {
            return unit.GetBuffCount("dariushemo") > 0 ? unit.GetBuffCount("dariushemo") : 0;
        }

        internal static void Drawing_OnEndScene(EventArgs args)
        {
        }
        internal static void Game_OnUpdate(EventArgs args)
        {

            if (Config.Item("useHPot").GetValue<bool>() && Config.Item("useHPotV").GetValue<Slider>().Value > Lib.Player.HealthPercent)
            {
                if (Items.HasItem(HealthPot.Id) && Items.CanUseItem(HealthPot.Id) && !ObjectManager.Player.HasBuff("RegenerationPotion", true) && !ObjectManager.Player.IsRecalling())
                {
                    HealthPot.Cast();
                }
            }
            if (Config.Item("useMPot").GetValue<bool>() && Config.Item("useMPotV").GetValue<Slider>().Value > Lib.Player.ManaPercent)
            {
                if (Items.HasItem(ManaPot.Id) && Items.CanUseItem(ManaPot.Id) && !ObjectManager.Player.HasBuff("ManaRegeneration", true) && !ObjectManager.Player.IsRecalling() && !ObjectManager.Player.InFountain() &&  !ObjectManager.Player.HasBuff("FlaskOfCrystalWater") && !ObjectManager.Player.HasBuff("ItemCrystalFlask"))
                {
                    ManaPot.Cast();
                }
            }
            if (Config.Item("useCFlask").GetValue<bool>() && Config.Item("useCFlaskH").GetValue<Slider>().Value > Lib.Player.HealthPercent || Config.Item("useCFlaskM").GetValue<Slider>().Value > Lib.Player.ManaPercent)
            {
                if (Items.HasItem(CrystalFlask.Id) && Items.CanUseItem(CrystalFlask.Id) && !ObjectManager.Player.HasBuff("RegenerationPotion", true) && !ObjectManager.Player.IsRecalling() && !ObjectManager.Player.InFountain() && !ObjectManager.Player.HasBuff("FlaskOfCrystalWater") && !ObjectManager.Player.HasBuff("ItemCrystalFlask"))
                {
                    CrystalFlask.Cast();
                }
            }

            if (Lib.Spellbook["E"].IsReady()  && Config.Item("tpcancel").GetValue<bool>() )
            {
                var etarget = TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical);
                if (etarget.IsValidTarget() && etarget.HasBuff("Teleport"))
                {
                    if (etarget.Distance(Lib.Player.ServerPosition) > 250)
                    {
                        Lib.Spellbook["E"].Cast(etarget.ServerPosition);
                    }
                }
            }

            if(IgniteSlot.IsReady() && Config.Item("useIgn").GetValue<bool>())
            {
                var etarget = TargetSelector.GetTarget(Lib.Spellbook["R"].Range, TargetSelector.DamageType.Physical);
                var Ignitedmg = Damage.GetSummonerSpellDamage(ObjectManager.Player, etarget, Damage.SummonerSpell.Ignite);
                if(etarget.Health < Ignitedmg)
                {
                    //Cast Ignite
                }
            }

            if (Lib.Spellbook["R"].IsReady() && Config.Item("ksr").GetValue<bool>())
            {
                foreach (var unit in HeroManager.Enemies.Where(ene => ene.IsValidTarget(Lib.Spellbook["R"].Range) && !ene.IsZombie))
                {
                    if (unit.CountEnemiesInRange(1200) <= 1 && Config.Item("ksr1").GetValue<bool>())
                    {
                        if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier + 
                            Lib.Hemorrhage(unit, PassiveCount(unit)) >= unit.Health + MordeShield(unit))
                        {
                            if (!TargetSelector.IsInvulnerable(unit, TargetSelector.DamageType.True))
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    Lib.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }

                    if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                        Lib.Hemorrhage(unit, 1) + MordeShield(unit))
                    {
                        if (!TargetSelector.IsInvulnerable(unit, TargetSelector.DamageType.True))
                        {
                            if (!unit.HasBuff("kindredrnodeathbuff"))
                                Lib.Spellbook["R"].CastOnUnit(unit);
                        }
                    }
                }
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo(Config.Item("useq").GetValue<bool>(), Config.Item("usew").GetValue<bool>(),
                          Config.Item("usee").GetValue<bool>(), Config.Item("user").GetValue<bool>());
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            if (Config.Item("caste").GetValue<KeyBind>().Active)
            {
                Orbwalking.Orbwalk(null, Game.CursorPos);
                Combo(false, false, true, false);
            }
        }
        


        internal static void Drawing_OnDraw(EventArgs args)
        {
            if (Lib.Player.IsDead)
            {
                return;
            }

            var acircle = Config.Item("drawe").GetValue<Circle>();
            if (acircle.Active)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["E"].Range, acircle.Color, 3);

            var rcircle = Config.Item("drawr").GetValue<Circle>();
            if (rcircle.Active)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["R"].Range, rcircle.Color, 3);

            var qcircle = Config.Item("drawq").GetValue<Circle>();
            if (qcircle.Active)
                Render.Circle.DrawCircle(Lib.Player.Position, Lib.Spellbook["Q"].Range, qcircle.Color, 3);

            if (!Config.Item("drawstack").GetValue<bool>())
            {
                return;
            }

            var plaz = Drawing.WorldToScreen(Lib.Player.Position);
            if (Lib.Player.GetBuffCount("dariusexecutemulticast") > 0)
            {
                var executetime = Lib.Player.GetBuff("dariusexecutemulticast").EndTime - Game.Time;
                Drawing.DrawText(plaz[0] - 15, plaz[1] + 55, System.Drawing.Color.OrangeRed, executetime.ToString("0.0"));
            }

            foreach (var enemy in HeroManager.Enemies.Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
                var enez = Drawing.WorldToScreen(enemy.Position); 
                if (enemy.GetBuffCount("dariushemo") > 0)
                {
                    var endtime = enemy.GetBuff("dariushemo").EndTime - Game.Time;
                    // Drawing.DrawText(enez[0] - 50, enez[1], System.Drawing.Color.OrangeRed,  "Stack Count: " + enemy.GetBuffCount("dariushemo"));
                   // Drawing.DrawText(enez[0] - 25, enez[1] + 20, System.Drawing.Color.OrangeRed, endtime.ToString("0.0"));
                }
            }
        }


        internal static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var hero = unit as AIHeroClient;
            if (hero == null || !hero.IsValid<AIHeroClient>() || hero.Type != GameObjectType.AIHeroClient ||
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
            {
                return;
            }

            if (Lib.Spellbook["R"].IsReady() && Lib.Player.Mana - Lib.Spellbook["W"].ManaCost > 
                Lib.Spellbook["R"].ManaCost || !Lib.Spellbook["R"].IsReady())
            {
                if (!hero.HasBuffOfType(BuffType.Slow) || Config.Item("wwww").GetValue<bool>())
                    Lib.Spellbook["W"].Cast();
            }

            if (!Lib.Spellbook["W"].IsReady() && Config.Item("iiii").GetValue<bool>())
            {
                Lib.HandleItems();
            }
        }

        internal static bool CanQ(Obj_AI_Base unit)
        {
            if (!unit.IsValidTarget() || unit.IsZombie ||
                TargetSelector.IsInvulnerable(unit, TargetSelector.DamageType.Physical))
            {
                return false;
            }

            if (Lib.Player.Distance(unit.ServerPosition) < 175 ||
                Utils.GameTimeTickCount - LastGrabTimeStamp < 350)
            {
                return false;
            }

            if (Lib.Spellbook["R"].IsReady() &&
                Lib.Player.Mana - Lib.Spellbook["Q"].ManaCost < Lib.Spellbook["R"].ManaCost)
            {
                return false;
            }

            if (Lib.Spellbook["W"].IsReady() && Lib.WDmg(unit) >= unit.Health &&
                unit.Distance(Lib.Player.ServerPosition) <= 200)
            {
                return false;
            }

            if (Lib.Spellbook["W"].IsReady() && Lib.Player.HasBuff("DariusNoxonTactictsONH") &&
                unit.Distance(Lib.Player.ServerPosition) <= 225)
            {
                return false;
            }

            if (Lib.Player.Distance(unit.ServerPosition) > Lib.Spellbook["Q"].Range)
            {
                return false;
            }

            if (Lib.Spellbook["R"].IsReady() && Lib.Spellbook["R"].IsInRange(unit) &&
                Lib.RDmg(unit, PassiveCount(unit)) - Lib.Hemorrhage(unit, 1) >= unit.Health)
            {
                return false;
            }

            if (Lib.Player.GetAutoAttackDamage(unit) * 2 + Lib.Hemorrhage(unit, PassiveCount(unit)) >= unit.Health &&
                Lib.Player.Distance(unit.ServerPosition) <= 180)
            {
                return false;
            }

            return true;
        }

        internal static void Harass()
        {
            if (Config.Item("harassq").GetValue<bool>() && Lib.Spellbook["Q"].IsReady())
            {
                if (Lib.Player.Mana / Lib.Player.MaxMana * 100 > 60)
                {
                    if (CanQ(TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical)))
                    {
                        Lib.Spellbook["Q"].Cast();
                    }
                }
            }   
        }

        internal static void Combo(bool useq, bool usew, bool usee, bool user)
        {
            if (useq && Lib.Spellbook["Q"].IsReady())
            {
                if (CanQ(TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical)))
                {
                    Lib.Spellbook["Q"].Cast();
                }
            }

            if (usew && Lib.Spellbook["W"].IsReady())
            {
                var wtarget = TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical);
                if (wtarget.IsValidTarget(Lib.Spellbook["W"].Range) && !wtarget.IsZombie)
                {
                    if (wtarget.Distance(Lib.Player.ServerPosition) <= 200 && Lib.WDmg(wtarget) >= wtarget.Health)
                    {
                        if (Utils.GameTimeTickCount - LastDunkTimeStamp >= 500)
                        {
                            Lib.Spellbook["W"].Cast();
                        }
                    }
                }
            }

            

                if (usee && Lib.Spellbook["E"].IsReady())
            {
                var etarget = TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical);
                if (etarget.IsValidTarget())
                {
                    if (etarget.Distance(Lib.Player.ServerPosition) > 250)
                    {
                        if (Lib.Player.CountAlliesInRange(1000) >= 1)
                            Lib.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (Lib.RDmg(etarget, PassiveCount(etarget)) - Lib.Hemorrhage(etarget, 1) >= etarget.Health)
                            Lib.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (Lib.Spellbook["Q"].IsReady() || Lib.Spellbook["W"].IsReady())
                            Lib.Spellbook["E"].Cast(etarget.ServerPosition);

                        if (Lib.Player.GetAutoAttackDamage(etarget) + Lib.Hemorrhage(etarget, 3) * 3 >= etarget.Health)
                            Lib.Spellbook["E"].Cast(etarget.ServerPosition);
                    }           
                }
            }

            if (user && Lib.Spellbook["R"].IsReady())
            {
                var unit = TargetSelector.GetTarget(Lib.Spellbook["E"].Range, TargetSelector.DamageType.Physical);

                if (unit.IsValidTarget(Lib.Spellbook["R"].Range) && !unit.IsZombie)
                {
                    if (!unit.HasBuffOfType(BuffType.Invulnerability) && !unit.HasBuffOfType(BuffType.SpellShield))
                    {
                        if (Lib.RDmg(unit, PassiveCount(unit)) + RModifier >= unit.Health +
                            Lib.Hemorrhage(unit, 1) + MordeShield(unit))
                        {
                            if (!TargetSelector.IsInvulnerable(unit, TargetSelector.DamageType.True))
                            {
                                if (!unit.HasBuff("kindredrnodeathbuff"))
                                    Lib.Spellbook["R"].CastOnUnit(unit);
                            }
                        }
                    }
                }
            }
        }

        internal static void Menu_OnLoad()
        {
            Config = new Menu("Perfect Darius", "darius", true);
                                            
            var perfectmenu = new Menu("Orbwalker", "perfectmenu");
            Orbwalker = new Orbwalking.Orbwalker(perfectmenu);
           
            Config.AddSubMenu(perfectmenu);

            var cmenu = new Menu("Combo Settings", "cmenu");
            cmenu.AddItem(new MenuItem("useq", "Use Q in Combo")).SetValue(true);
            cmenu.AddItem(new MenuItem("usee", "Use E in Combo")).SetValue(true);
            cmenu.AddItem(new MenuItem("caste", "Use E to Assist").SetValue(new KeyBind('E', KeyBindType.Press)));   
            cmenu.AddItem(new MenuItem("usew", "Use W to AA Reset")).SetValue(true);
            cmenu.AddItem(new MenuItem("wwww", "Use W on Slowed Targets")).SetValue(true);
            cmenu.AddItem(new MenuItem("www2w", "Please Select The Target to Use The Ult"));
            Config.AddSubMenu(cmenu);

            var hmenu = new Menu("Harass Settings", "hmenu");
            hmenu.AddItem(new MenuItem("harassq", "Use Q in harass")).SetValue(true);
            Config.AddSubMenu(hmenu);

            var lmenu = new Menu("Lane/Jungle Clear Settings", "lmenu");
            lmenu.AddItem(new MenuItem("added", "Will be added soon."));
            Config.AddSubMenu(lmenu);

            var amenu = new Menu("Activator", "amenu");
            amenu.AddItem(new MenuItem("useIgn", "Use Ignite")).SetValue(true);
            amenu.AddItem(new MenuItem("iiii", "Use Hydra")).SetValue(true);
            amenu.AddItem(new MenuItem("useHPot", "Use HP Pot")).SetValue(true);
            amenu.AddItem(new MenuItem("useHPotV", "HP < % Use").SetValue(new Slider(30, 0, 100)));
            amenu.AddItem(new MenuItem("useMPot", "Use Mana Pot")).SetValue(true);
            amenu.AddItem(new MenuItem("useMPotV", "Mana < % Use").SetValue(new Slider(30, 0, 100)));
            amenu.AddItem(new MenuItem("useCFlask", "Use Crystal Flask")).SetValue(true);
            amenu.AddItem(new MenuItem("useCFlaskH", "HP < % Use").SetValue(new Slider(45, 0, 100)));
            amenu.AddItem(new MenuItem("useCFlaskM", "Mana < % Use").SetValue(new Slider(25, 0, 100)));
            Config.AddSubMenu(amenu);

            var rmenu = new Menu("More Settings", "rmenu");
            
            rmenu.AddItem(new MenuItem("useeint", "Interrupt Spells with E")).SetValue(true);
            rmenu.AddItem(new MenuItem("tpcancel", "E to Cancel the Enemy's TP")).SetValue(true).SetTooltip("Cancels The Enemy's Teleport");
            rmenu.AddItem(new MenuItem("user", "Use R")).SetValue(true);
            rmenu.AddItem(new MenuItem("ksr", "Use R in KS")).SetValue(true).SetTooltip("Kill Steal :P");
            rmenu.AddItem(new MenuItem("ksr1", "Use R Max Dmg")).SetValue(false).SetTooltip("1v1 Fight for Highest Damage Choose to Give");
            //rmenu.AddItem(new MenuItem("userlast", "Use R before buff expiry").SetValue(true).SetTooltip("After a successful ult, will not waste R if buff will Expire"));
            rmenu.AddItem(new MenuItem("rmodi", "R Delay").SetValue(new Slider(0, 0, 500)).SetTooltip("It is not Recommended to Change"));
            rmenu.AddItem(new MenuItem("useeflee", "Auto E Fleeing Targets").SetValue(true).SetTooltip("Will Pull Fleeing Targets"));
            Config.AddSubMenu(rmenu);

            

            var drmenu = new Menu("Draw Settings", "drawings");
            drmenu.AddItem(new MenuItem("drawq", "Draw Q").SetValue(new Circle(true, System.Drawing.Color.FromArgb(150, System.Drawing.Color.AliceBlue))));
            drmenu.AddItem(new MenuItem("drawe", "Draw E").SetValue(new Circle(true, System.Drawing.Color.FromArgb(150, System.Drawing.Color.LightGreen))));
            drmenu.AddItem(new MenuItem("drawr", "Draw R").SetValue(new Circle(true, System.Drawing.Color.FromArgb(150, System.Drawing.Color.Red))));
                
            drmenu.AddItem(new MenuItem("drawfill", "Draw R Damage Fill")).SetValue(true).SetTooltip("Show the injury Level");
          //  drmenu.AddItem(new MenuItem("drawstack", "Draw Stack Count")).SetValue(true);
            Config.AddSubMenu(drmenu);

            Config.AddToMainMenu();
        }
    }
    internal class PerfectLib
    {
        internal static AIHeroClient Player = ObjectManager.Player;
        internal static System.Collections.Generic.Dictionary<string, Spell> Spellbook = new System.Collections.Generic.Dictionary<string, Spell>
        {
            { "Q", new Spell(SpellSlot.Q, 425f) },
            { "W", new Spell(SpellSlot.W, 200f) },
            { "E", new Spell(SpellSlot.E, 490f) },
            { "R", new Spell(SpellSlot.R, 460f) }
      
            
        };

        public static Items.Item HealthPot { get; private set; }
        public static Items.Item ManaPot { get; private set; }
        public static Items.Item CrystalFlask { get; private set; }
        


        internal static void HandleItems()
        {
            //HYDRA
            if (Items.HasItem(3077) && Items.CanUseItem(3077))
                Items.UseItem(3077);

            //TİAMAT
            if (Items.HasItem(3074) && Items.CanUseItem(3074))
                Items.UseItem(3074);

            //NEW ITEM
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
                Items.UseItem(3748);



           
        }

        internal static float QDmg(Obj_AI_Base unit)
        {
            return
                (float)
                    Player.CalcDamage(unit, Damage.DamageType.Physical,
                        new[] { 20, 20, 35, 50, 65, 80 }[Spellbook["Q"].Level] +
                       (new[] { 1.0, 1.0, 1.1, 1.2, 1.3, 1.4 }[Spellbook["Q"].Level] * Player.FlatPhysicalDamageMod));
        }

        internal static float WDmg(Obj_AI_Base unit)
        {
            return
                (float)
                    Player.CalcDamage(unit, Damage.DamageType.Physical,
                       Player.TotalAttackDamage + (0.4 * Player.TotalAttackDamage));
        }

        internal static float RDmg(Obj_AI_Base unit, int stackcount)
        {
            var bonus = (new[] { 20, 20, 40, 60 }[Spellbook["R"].Level] +
                            (0.25 * Player.FlatPhysicalDamageMod) * stackcount);
            return
                (float)(bonus + (Player.CalcDamage(unit, Damage.DamageType.True,
                        new[] { 100, 100, 200, 300 }[Spellbook["R"].Level] + (0.75 * Player.FlatPhysicalDamageMod))));
        }

        internal static float Hemorrhage(Obj_AI_Base unit, int stackcount)
        {
            if (stackcount < 1)
                stackcount = 1;

            return
                (float)
                    Player.CalcDamage(unit, Damage.DamageType.Physical,
                        (9 + Player.Level) + (0.3 * Player.FlatPhysicalDamageMod)) * stackcount;
        }
    }
}
