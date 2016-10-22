using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;
using myWorld.Library.DamageManager;
using myWorld.Library.Draw;
using myWorld.Library.STS;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Draven
{
    class Draven
    {
        float ProjectileSpeed = float.MaxValue;
        SimpleTS STS = new SimpleTS();
        AxeCather AxesCatcher = new AxeCather();
        DrawManager DM = new DrawManager();
        DamageLib DLib = new DamageLib(ObjectManager.Player);
        Spell Q, W, E, R;
        Menu Menu;
        Menu Config;

        AIHeroClient myHero = ObjectManager.Player;

        bool HasWBuff = false;
        GameObject RObject = (GameObject)null;

        public Draven()
        {
            Q = new Spell(SpellSlot.Q, 1075);
            W = new Spell(SpellSlot.W, 950);
            W.SetSkillshot(0.5f, 315f, float.MaxValue, false, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 1050);
            E.SetSkillshot(0.25f, 130f, 1600f, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R, float.MaxValue);
            R.SetSkillshot(0.5f, 155f, 2000f, false, SkillshotType.SkillshotLine);

            List<string> hitChances = new List<string>();
            foreach (HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                hitChances.Add(value.ToString());
            }

            DLib.RegistDamage("Q", DamageType.Physical, 0f, 0f, DamageType.Physical, ScalingType.AD, 0f, delegate(Obj_AI_Base target) { return AxesCatcher.GetCountAxes() > 0; }, delegate(Obj_AI_Base target) 
            { 
                return ( (float)myHero.GetAutoAttackDamage(target) * (1.45f + (0.1f * (Q.Level - 1)))) * AxesCatcher.GetCountAxes();
            });
            DLib.RegistDamage("E", DamageType.Physical, 70f, 35f, DamageType.Physical, ScalingType.BONUS_AD, 0.5f, delegate(Obj_AI_Base target) { return E.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });
            DLib.RegistDamage("R", DamageType.Physical, 175f, 100f, DamageType.Physical, ScalingType.BONUS_AD, 1.1f, delegate(Obj_AI_Base target) { return R.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });

            DM.AddCircle(ObjectManager.Player, W.Range, Color.Red, "W Draw", delegate() { return W.IsReady(); });
            DM.AddCircle(ObjectManager.Player, E.Range, Color.Red, "E Draw", delegate() { return E.IsReady(); });

            Menu = Program.MainMenu;
            Config = new Menu("Draven", "Draven");

            Menu HitChanceMenu = new Menu("HitChance", "HitChance");

            Menu ComboHitChaceMenu = new Menu("Combo", "Combo");
            ComboHitChaceMenu.AddList("HitChance.Combo.E", "E HitChance", new StringList(hitChances.ToArray(), 4));
            ComboHitChaceMenu.AddList("HitChance.Combo.R", "R HitChance", new StringList(hitChances.ToArray(), 5));
            HitChanceMenu.AddSubMenu(ComboHitChaceMenu);

            Menu HarassHitChaceMenu = new Menu("Harass", "Harass");
            HarassHitChaceMenu.AddList("HitChance.Harass.E", "E HitChance", new StringList(hitChances.ToArray(), 3));
            HarassHitChaceMenu.AddList("HitChance.Harass.R", "R HitChance", new StringList(hitChances.ToArray(), 4));
            HitChanceMenu.AddSubMenu(HarassHitChaceMenu);

            Config.AddSubMenu(HitChanceMenu);

            STS.AddToMenu(Config);
            DLib.AddToMenu(Config, new List<string>() { "Q", "E", "R", "R" });
            DM.AddToMenu(Config);
            AxesCatcher.AddToMenu(Config);

            Menu Combo = new Menu("Combo", "Combo");
            Combo.AddList("Combo.UseQ", "Use Q", new StringList(new string[] { "Zero Spins", "One Spin", "Two Spin" }, 2));
            Combo.AddList("Combo.UseW", "Use W", new StringList(new string[] { "Never", "If is not in range", "Always" }, 1));
            Combo.AddBool("Combo.UseE", "Use E");
            Combo.AddBool("Combo.UseR1", "Use R if killable");
            Combo.AddSlice("Combo.UseR2", "Use R if enemies >=", 3, 0, 5);
            //Combo.AddBool("Combo.UseItem")
            Config.AddSubMenu(Combo);

            Menu Harass = new Menu("Harass", "Harass");
            Harass.AddList("Harass.UseQ", "Use Q", new StringList(new string[] {"Zero Spins", "One Spin", "Tow Spin"}, 2));
            Harass.AddBool("Harass.UseW", "Use W");
            Harass.AddBool("Harass.UseE", "Use E");
            Harass.AddSlice("Harass.mper", 20);
            Config.AddSubMenu(Harass);

            Menu LineClear = new Menu("LineClear", "LineClear");
            LineClear.AddList("LineClear.UseQ", "Use Q", new StringList(new string[] { "Zero Spins", "One Spin", "Tow Spin" }, 2));
            LineClear.AddBool("LineClear.UseW", "Use W");
            LineClear.AddBool("LineClear.UseE", "Use E");
            LineClear.AddSlice("LineClear.mper", 20);
            Config.AddSubMenu(LineClear);

            Menu JungleClear = new Menu("JungleClear", "JungleClear");
            JungleClear.AddList("JungleClear.UseQ", "Use Q", new StringList(new string[] { "Zero Spins", "One Spin", "Tow Spin" }, 2));
            JungleClear.AddBool("JungleClear.UseW", "Use W");
            JungleClear.AddBool("JungleClear.UseE", "Use E");
            JungleClear.AddSlice("JungleClear.mper", 20);
            Config.AddSubMenu(JungleClear);

            Menu LastHit = new Menu("LastHit", "LastHit");
            LastHit.AddBool("LastHit.UseQ", "Use Q");
            LastHit.AddBool("LastHit.UseE", "Use E");
            LastHit.AddSlice("LastHit.mper", 20);
            Config.AddSubMenu(LastHit);

            Menu KillSteal = new Menu("KillSteal", "KillSteal");
            KillSteal.AddBool("KillSteal.UseQ", "Use Q");
            KillSteal.AddBool("KillSteal.UseW", "Use W");
            KillSteal.AddBool("KillSteal.UseE", "Use E");
            KillSteal.AddBool("KillSteal.useIgnite", "Use Ignite");
            Config.AddSubMenu(KillSteal);

            Menu Auto = new Menu("Auto", "Auto");
            Auto.AddBool("Auto.EI", "Use E To Interrupt");
            Auto.AddBool("Auto.EA", "Use E To Gapcloser");
            Config.AddSubMenu(Auto);

            Menu Misc = new Menu("Misc", "Misc");
            Misc.AddSlice("Misc.overkill", "Overkill % for dmg Predict..", 10, 0, 100);
            Misc.AddSlice("Misc.RRange", "R Range", 1800, 300, 6000);
            Config.AddSubMenu(Misc);

            Menu.AddSubMenu(Config);

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //throw new NotImplementedException();
            if(Config.GetBool("Auto.EA") && myHero.Position.Distance(gapcloser.End) < E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
            if(sender != null && sender.Name.ToLower().Contains("missile") && (sender.IsAlly))
            {
                //your face; what? four face. wtf? i say your face, die, i mean your face f.. help! some body help me! (already die)
            }
        }

        void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
            if(sender != null && sender.IsValid && sender.Name != null && this.RObject != null && this.RObject.Position.Distance(sender.Position) < 100 && RObject.Name == sender.Name)
            {
                //this.RObject = null;
            }
            else if(sender != null && sender.IsValid && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("_w") && sender.Name.ToLower().Contains("_buf"))
            {
                if(sender.Name.ToLower().Contains("move"))
                {
                    this.HasWBuff = false;
                }
                else if (sender.Name.ToLower().Contains("attack"))
                {
                    this.HasWBuff = false;
                }
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            //Drawing.DrawText(100, 100, Color.Aqua, AxesCatcher.GetCountAxes().ToString());
        }

        void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();

            if (myHero.IsDead) return;
            AxesCatcher.CheckCatch();
            switch(Program.MainOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }
        }

        void Combo()
        {
            Obj_AI_Base target = STS.GetTarget(1500);
            if(target.IsValidTarget(1500))
            {
                //Chat.Print("Good");
                CastQ(target, Config.GetListIndex("Combo.UseQ"));
                CastW(target, Config.GetListIndex("Combo.UseW"));
                CastE(target, CastMode.Combo);
                if(Config.GetBool("Combo.UseR1"))
                {
                    if (DLib.CalcComboDamage(target, new List<string>() { "Q", "E", "R", "R" }) * ((100 + Config.GetSlice("Misc.overkill")) / 100) > target.Health)
                    {
                        R.Cast(target);
                    }
                }
                if(Config.GetSlice("Combo.UseR2") > 0)
                {
                    PredictionOutput Best = new PredictionOutput();
                    foreach(Obj_AI_Base enemy in HeroManager.Enemies.Where(x => x.IsValidTarget(Config.GetSlice("Misc.RRange"))))
                    {
                        if(R.IsReady())
                        {
                            PredictionOutput output = R.GetPrediction(target);
                            if (Best.AoeTargetsHitCount == 0) Best = output;
                            else if (Best.AoeTargetsHitCount < output.AoeTargetsHitCount) Best = output;
                        }
                    }
                    if(Best.AoeTargetsHitCount >= Config.GetSlice("Combo.UseR2") && Best.Hitchance.IsHigerThen(HitChance.Medium))
                    {
                        R.Cast(Best.CastPosition);
                    }
                }
            }
        }

        void Harass()
        {
            if (myHero.IsManaLow(Config.GetSlice("Harass.mper"))) return;
            if(Config.GetListIndex("Harass.UseQ") > 0)
            {
                List<Obj_AI_Base> mobs = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy);
                if(mobs.Count > 0)
                {
                    CastQ(mobs[0], Config.GetListIndex("Harass.UseQ"));
                }
            }
            Obj_AI_Base target = STS.GetTarget(1500);
            if(target.IsValidTarget(1500))
            {
                CastQ(target, Config.GetListIndex("Harass.UseQ"));
                CastW(target);
                CastE(target, CastMode.Harass);
            }
        }

        void Clear()
        {
            if (!myHero.IsManaLow(Config.GetSlice("LineClear.mper")))
            {
                List<Obj_AI_Base> mob = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy);
                if (mob.Count > 0)
                {
                    if (Q.IsReady()) CastQ(mob[0], Config.GetListIndex("LineClear.UseQ"));
                    if (E.IsReady() && Config.GetBool("LineClear.UseE"))
                    {
                        MinionManager.FarmLocation fl = E.GetLineFarmLocation(mob);
                        if (fl.MinionsHit >= 2)
                        {
                            E.Cast(fl.Position);
                        }
                    }
                }
            }

            if (!myHero.IsManaLow(Config.GetSlice("JungleClear.mper")))
            {
                List<Obj_AI_Base> mob = MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy);
                if (mob.Count > 0)
                {
                    if (Q.IsReady()) CastQ(mob[0], Config.GetListIndex("JungleClear.UseQ"));
                    if (E.IsReady() && Config.GetBool("JungleClear"))
                    {
                        MinionManager.FarmLocation fl = E.GetLineFarmLocation(mob);
                        if (fl.MinionsHit >= 2)
                        {
                            E.Cast(fl.Position);
                        }
                    }
                }
            }
        }

        void LastHit()
        {
            if (myHero.IsManaLow(Config.GetSlice("LastHit.mper"))) return;
            foreach(Obj_AI_Base minion in MinionManager.GetMinions(900, MinionTypes.All, MinionTeam.Enemy))
            {
                if(minion.IsValidTarget(myHero.AttackRange) && Config.GetBool("LastHit.UseQ") && Q.IsReady())
                {
                    float time = Game.Ping / 2000 + myHero.Position.Distance(minion.Position) / ProjectileSpeed - 100 / 1000;
                    float axeDmg = DLib.CalcSpellDamage(minion, "Q");
                    if(HealthPrediction.GetHealthPrediction(minion, (int)time) + axeDmg > -40)
                    {
                        if(AxesCatcher.GetCountAxes() == 0)
                        {
                            CastQ(minion);
                        }
                    }
                }
                if(Config.GetBool("LastHit.UseE"))
                {
                    if(HealthPrediction.GetHealthPrediction(minion, (int)E.Delay) - DLib.CalcSpellDamage(minion, "E") > 0)
                    {
                        CastE(minion, CastMode.Farm);
                    }
                }
            }
        }

        void CastQ(Obj_AI_Base target, int mode = 2)
        {
            if(Q.IsReady() && target.IsValidTarget(Q.Range) && AxesCatcher.GetCountAxes() < 2 && Orbwalking.CanAttack())
            {
                if(mode == 2)
                {
                    Q.Cast();
                }
                else if (mode == 1)
                {
                    if(AxesCatcher.GetCountAxes() < 1 )
                    {
                        Q.Cast();
                    }
                }
                else if(mode == 0)
                {
                    //
                }
            }
        }

        void CastW(Obj_AI_Base target, int mode = 2)
        {
            if(W.IsReady() && target.IsValidTarget(W.Range) && ! HasWBuff)
            {
                if(mode == 1)
                {
                    if(!Orbwalking.InAutoAttackRange(target))
                    {
                        W.Cast();
                    }
                }
                else if(mode == 2)
                {
                    W.Cast();
                }
            }
        }

        void CastE(Obj_AI_Base target, CastMode mode)
        {
            if(E.IsReady() && E.IsInRange(target))
            {
                switch(mode)
                {
                    case CastMode.Combo:
                        E.CastIfHigherThen(target, Config.GetList("HitChance.Combo.E").ToHitChance());
                        break;
                    case CastMode.Harass:
                        E.CastIfHigherThen(target, Config.GetList("HitChance.Harass.E").ToHitChance());
                        break;
                    default:
                        E.CastIfHigherThen(target, HitChance.Low);
                        break;
                }
            }
        }
    }

    class AxeCather
    {
        List<AxesAbailable> AxesAbailables = new List<AxesAbailable>();
        int CurrentAxes = 0;
        int Stack = 0;
        int AxeRadius = 100;
        float LimitTime = 1200f;
        Menu Menu;
        float ProjectileSpeed = ObjectManager.Player.AttackRange > 300 ? 1700 : float.MaxValue;
        Obj_AI_Base myHero = ObjectManager.Player;
        Menu Config;

        public AxeCather()
        {
            //
        }

        public void AddToMenu(Menu Menu)
        {
            this.Menu = Menu;

            Config = new Menu("Auto Cather", "Auto Catcher");

            Config.AddKeyToggle("Catch", "Catch Axes (Toggle)", "Z");
            Config.AddList("CatchMode", "Catch Condition", new StringList(new string[] { "When Orbwalking", "AutoCatch" }));
            Config.AddList("OrbwalkMode", "Catch Mode", new StringList(new string[] { "Mouse In Radius", "MyHero In Radius" }));
            Config.AddBool("UseW", "Use W to Catch");
            Config.AddBool("Turret", "Dont Catch Under Turret");
            Config.AddSlice("DelayCatch", "% of delay to catch", 100, 0, 100);

            Menu CatchRadius = new Menu("Catch Radius", "CatchRadius");
            CatchRadius.AddSlice("CatchRadius.Combo", "Combo Radius", 250, 150, 600);
            CatchRadius.AddSlice("CatchRadius.Harass", "Harass Radius", 350, 150, 600);
            CatchRadius.AddSlice("CatchRadius.Clear", "Clear Radius", 400, 150, 800);
            CatchRadius.AddSlice("CatchRadius.LastHit", "LastHit Radius", 400, 150, 800);
            Config.AddSubMenu(CatchRadius);

            Menu Draw = new Menu("Draw Catch Radius", "Draw");
            Draw.AddBool("Draw.Enable", "Enable");
            Draw.AddBool("Draw.Source", "Draw Source");

            Config.AddSubMenu(Draw);

            Menu.AddSubMenu(Config);
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
            if (sender != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("q_reticle_self.troy") && ObjectManager.Player.Position.Distance(sender.Position) <= (LimitTime * myHero.MoveSpeed))
            {
                AxesAbailables.Add(new AxesAbailable(sender, Utils.TickCount - Game.Ping / 2000));
                //LeagueSharp.Common.Utility.DelayAction.Add((int)(LimitTime + 0.2)*10,  () => RemoveAxe(sender) );
            }
            else if(sender != null && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("q_buf.troy"))
            {
                CurrentAxes += 1;
            }
            else if (sender != null && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("q_tar.troy"))
            {
                //
            }
            else if (sender != null && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("reticlecatchsuccess.troy"))
            {
                RemoveAxe(sender);
            }
        }

        void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            //throw new NotImplementedException();
            if(sender != null && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("q_reticle.troy"))
            {
                RemoveAxe(sender);
            }
            else if(sender != null && sender.Name != null && sender.Name.ToLower().Contains("draven") && sender.Name.ToLower().Contains("q_reticle_self.troy"))
            {
                if (CurrentAxes > 0) CurrentAxes -= 1;
            }
            else if(sender != null && sender.Name != null && sender.Name.ToLower().Contains("q_buf.troy") && sender.Name.ToLower().Contains("q_tar.troy"))
            {
                //
            }
            else if(sender != null && sender.Name != null && sender.Name.ToLower().Contains("reticlecatchsuccess.troy"))
            {
                //
            }
        }

        void Game_OnUpdate(EventArgs args)
        {
            throw new NotImplementedException();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            //foreach (BuffInstance buff in myHero.Buffs)
            //{
            //    Chat.Print(buff.Name);
            //}
            //Chat.Print("======================================");
            //Drawing.DrawText(100, 100, Color.Aqua, string.Format("Buff count : {0}, Object count : {1}, ?? : {2}", GetCountAxes(), AxesAbailables.Count, CurrentAxes));
            if (Config.GetBool("Draw.Source"))
            {
                Drawing.DrawCircle(GetSource(), GetRadius(), Color.Aqua);
            }
            if(Config.GetBool("Draw.Enable") && !myHero.IsDead)
            {
                if(AxesAbailables.Count>0)
                {
                    for(int i = 0; i < AxesAbailables.Count; i++)
                    {
                        try
                        {
                            Drawing.DrawCircle(AxesAbailables[i].Object.Position, 100, Color.Aqua);
                        }
                        catch(Exception ex)
                        {
                            AxesAbailables.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public int GetCountAxes()
        {
            //return AxesAbailables.Count + CurrentAxes;
            int add = myHero.GetBuffCount("dravenspinningattack") == -1 ? 0 : myHero.GetBuffCount("dravenspinningattack");
            return AxesAbailables.Count + add;//AxesAbailables.Count + myHero.GetBuffCount("dravenspinningattack");
        }

        float GetDelayCatch()
        {
            return Menu != null ? Config.GetSlice("DelayCatch") / 100 : 1;
        }

        float GetRadius()
        {
            switch(Program.MainOrbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    return Config.GetSlice("CatchRadius.Combo");
                case Orbwalking.OrbwalkingMode.Mixed:
                    return Config.GetSlice("CatchRadius.Harass");
                case Orbwalking.OrbwalkingMode.LaneClear:
                    return Config.GetSlice("CatchRadius.Clear");
                case Orbwalking.OrbwalkingMode.LastHit:
                    return Config.GetSlice("CatchRadius.LastHit");
                default:
                    return Config.GetSlice("CatchRadius.Clear");
            }
        }

        bool InTurret(GameObject sender)
        {
            int offset = 1700 / 2;
            if(sender != null && sender.IsValid && Config.GetBool("Turret"))
            {
                return sender.Position.IsUnderTurret();
            }
            return false;
        }

        float GetBonusSpeed()
        {
            return myHero.Spellbook.GetSpell(SpellSlot.W).Level > 0 ? (100 + 35 + myHero.Spellbook.GetSpell(SpellSlot.W).Level * 5) / 100 : 1;
        }

        Vector3 GetSource()
        {
            return Config.GetListIndex("OrbwalkMode") == 0 ? Game.CursorPos : myHero.Position;
        }

        bool InRadius(GameObject sender)
        {
            return sender != null && sender.IsValid && Vector3.Distance(GetSource(), sender.Position) < GetRadius();
        }

        bool InAxeRadius(GameObject sender)
        {
            float AxeRadius = 1 / 1 * this.AxeRadius;
            return sender != null && sender.IsValid && Vector3.Distance(myHero.Position, sender.Position) < AxeRadius;
        }

        BestAxeOutput GetBestAxe()
        {
            BestAxeOutput output = (BestAxeOutput)null;
            if(AxesAbailables.Count > 0)
            {
                for(int i = 0; i < AxesAbailables.Count; i++)
                {
                    GameObject axe = AxesAbailables[i].Object;
                    float Time = AxesAbailables[i].Time;
                    if(axe != null && axe.IsValid)
                    {
                        float timeLeft = LimitTime - (Utils.TickCount - Time);
                        float AxeRadius = 1 / 1 * this.AxeRadius;
                        if(timeLeft >= 0 && Vector3.Distance(myHero.Position, axe.Position) - Math.Min(AxeRadius, myHero.Position.Distance(axe.Position)) + AxeRadius <= myHero.MoveSpeed * timeLeft * GetDelayCatch())
                        {
                            if (output.BestAxe == null)
                            {
                                output.BestAxe = axe;
                                output.BestTime = Time;
                            }
                            else
                            {
                                if((myHero.Position.Distance(axe.Position) - Math.Min(AxeRadius, myHero.Position.Distance(axe.Position)) / Time) > (myHero.Position.Distance(axe.Position) - Math.Min(AxeRadius, myHero.Position.Distance(output.BestAxe.Position))) / output.BestTime)
                                {
                                    output.BestAxe = axe;
                                    output.BestTime = Time;
                                }
                            }
                        }
                        if(timeLeft <= 0)
                        {
                            RemoveAxe(axe);
                        }
                    }
                }
            }
            return output;
        }

        CheckAxe CheckAxe(GameObject axe, float time)
        {
            CheckAxe output = new CheckAxe(false, false);
            if(axe != null && axe.IsValid && Utils.TickCount + Game.Ping / 2000 - time <= LimitTime && InRadius(axe) && !InTurret(axe))
            {
                float timeLeft = LimitTime - (Utils.TickCount + Game.Ping / 2000 - time);
                float AxeRadius = 1 / 1 * this.AxeRadius;
                Vector3 AxeCatchPositionFromHero = axe.Position + (myHero.Position - axe.Position).Normalized() * Math.Min(AxeRadius, myHero.Position.Distance(axe.Position));
                Vector3 OrbwalkPosition = myHero.Position + (Utils.GetCursorPos().To3D() - axe.Position).Normalized() * AxeRadius;
                float Time = timeLeft - ((myHero.Position.Distance(AxeCatchPositionFromHero)) / myHero.MoveSpeed) - (Game.Ping / 2000 - 100 / 1000);

                if (InAxeRadius(axe))
                {
                    output.CanAttack = true;
                }
                else if (Utils.TickCount - time <= 600 && Orbwalking.CanMove(0.5f))
                {
                    output.CanAttack = true;
                }
                else
                {
                    output.CanAttack = false;
                }
                
                if(myHero.Position.Distance(AxeCatchPositionFromHero) + 100 > myHero.MoveSpeed * (timeLeft*0.001) * GetDelayCatch())
                {
                    output.CanMove = false;
                    if(!InAxeRadius(axe) && myHero.Position.Distance(AxeCatchPositionFromHero) > (myHero.MoveSpeed * timeLeft * 1.5) && myHero.Position.Distance(AxeCatchPositionFromHero) < (myHero.MoveSpeed*timeLeft*GetBonusSpeed()))
                    {
                        if(Config.GetBool("UseW") && myHero.Spellbook.GetSpell(SpellSlot.W).IsReady())
                        {
                            myHero.Spellbook.CastSpell(SpellSlot.W);
                        }
                    }
                }
                else
                {
                    output.CanMove = true;
                }
            }
            else
            {
                output.CanMove = true;
                output.CanAttack = true;
            }
            if(Utils.TickCount - time > LimitTime + 0.2)
            {
                RemoveAxe(axe);
            }
            return output;
        }

        public void CheckCatch()
        {
            if(AxesAbailables.Count > 0)
            {
                foreach(AxesAbailable obj in AxesAbailables)
                {
                    if(Utils.TickCount - obj.Time > 1200)
                    {
                        //AxesAbailables.Remove(obj);
                    }
                }
                Chat.Print(((Config.GetListIndex("CatchMode") == 0 && (Program.MainOrbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)) || (Config.GetListIndex("CatchMode") == 1)).ToString());
                if (Config != null && Config.GetKeyToggle("Catch") &&
                    ((Config.GetListIndex("CatchMode") == 0 && (Program.MainOrbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None)) || (Config.GetListIndex("CatchMode") == 1)))
                {
                    CheckAxe v = new CheckAxe(true, true);
                    
                    for(int i = 0; i < AxesAbailables.Count; i++)
                    {
                        if(AxesAbailables[i].Object != null)
                        {
                            AxesAbailable a = AxesAbailables[i];
                            CheckAxe c2 = CheckAxe(a.Object, a.Time);
                            if (!c2.CanMove) v.CanMove = false;
                            if (!c2.CanAttack) v.CanAttack = false;
                        }
                    }
                    if(AxesAbailables[0].Object != null)
                    {
                        if (v.CanAttack)
                        {
                            Program.MainOrbwalker.SetAttack(true);
                        }
                        else
                        {
                            Program.MainOrbwalker.SetAttack(false);
                        }
                        if(v.CanMove)
                        {
                            Program.MainOrbwalker.SetMovement(true);
                        }
                        else
                        {
                            Program.MainOrbwalker.SetMovement(false);
                            if(!InAxeRadius(AxesAbailables[0].Object) && Orbwalking.CanMove(0.2f))
                            {
                                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, AxesAbailables[0].Object.Position);
                                
                            }
                        }
                        
                    }
                }
                else
                {
                    Program.MainOrbwalker.SetAttack(true);
                    Program.MainOrbwalker.SetMovement(true);
                }
            }
            else
            {
                Program.MainOrbwalker.SetMovement(true);
                Program.MainOrbwalker.SetAttack(true);
            }
        }


        void RemoveAxe(GameObject sender)
        {
            if(AxesAbailables.Count > 0 && sender != null)
            {
                for(int i=0; i < AxesAbailables.Count; i++)
                {
                    AxesAbailable v = AxesAbailables[i];
                    if(v.Object != null && v.Object.Position.Distance(sender.Position) < 30)
                    {
                        AxesAbailables.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    class CheckAxe
    {
        public bool CanMove;
        public bool CanAttack;
        public CheckAxe(bool CanMove, bool CanAttack)
        {
            this.CanMove = CanMove;
            this.CanAttack = CanAttack;
        }
    }

    class BestAxeOutput
    {
        public GameObject BestAxe;
        public float BestTime;
        public BestAxeOutput(GameObject BestAxe, float BestTime)
        {
            this.BestAxe = BestAxe;
            this.BestTime = BestTime;
        }
    }

    class AxesAbailable
    {
        public GameObject Object;
        public float Time;
        public AxesAbailable(GameObject Object, float Time)
        {
            this.Object = Object;
            this.Time = Time;
        }
    }
}
