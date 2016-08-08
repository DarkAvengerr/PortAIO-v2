using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; namespace HoolaRiven
{
    public class Program
    {
        public static Menu Menu;
        private static Orbwalking.Orbwalker Orbwalker;
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static readonly HpBarIndicator Indicator = new HpBarIndicator();
        private const string IsFirstR = "RivenFengShuiEngine";
        private const string IsSecondR = "RivenIzunaBlade";
        private static readonly SpellSlot Flash = Player.LSGetSpellSlot("summonerFlash");
        private static Spell Q, Q1, W, E, R;
        private static int QStack = 1;
        public static Render.Text Timer, Timer2;
        private static bool forceQ;
        private static bool forceW;
        private static bool forceR;
        private static bool forceR2;
        private static bool forceItem;
        private static float LastQ;
        private static float LastR;
        private static AttackableUnit QTarget;
        private static bool Dind => Menu.Item("Dind").GetValue<bool>();
        private static bool DrawCB => Menu.Item("DrawCB").GetValue<bool>();
        private static bool KillstealW => Menu.Item("killstealw").GetValue<bool>();
        private static bool KillstealR => Menu.Item("killstealr").GetValue<bool>();
        private static bool DrawAlwaysR => Menu.Item("DrawAlwaysR").GetValue<bool>();
        private static bool DrawUseHoola => Menu.Item("DrawUseHoola").GetValue <bool>();
        private static bool DrawFH => Menu.Item("DrawFH").GetValue<bool>();
        private static bool DrawTimer1 => Menu.Item("DrawTimer1").GetValue<bool>();
        private static bool DrawTimer2 => Menu.Item("DrawTimer2").GetValue<bool>();
        private static bool DrawHS => Menu.Item("DrawHS").GetValue<bool>();
        private static bool DrawBT => Menu.Item("DrawBT").GetValue<bool>();
        private static bool UseHoola => Menu.Item("UseHoola").GetValue<KeyBind>().Active;
        private static bool AlwaysR => Menu.Item("AlwaysR").GetValue<KeyBind>().Active;
        private static bool AutoShield => Menu.Item("AutoShield").GetValue<bool>();
        private static bool Shield => Menu.Item("Shield").GetValue<bool>();
        private static bool KeepQ => Menu.Item("KeepQ").GetValue<bool>();
        private static int QD => Menu.Item("QD").GetValue<Slider>().Value;
        private static int QLD => Menu.Item("QLD").GetValue<Slider>().Value;
        private static int AutoW => Menu.Item("AutoW").GetValue<Slider>().Value;
        private static bool ComboW => Menu.Item("ComboW").GetValue<bool>();
        private static bool RMaxDam => Menu.Item("RMaxDam").GetValue<bool>();
        private static bool RKillable => Menu.Item("RKillable").GetValue<bool>();
        private static int LaneW => Menu.Item("LaneW").GetValue<Slider>().Value;
        private static bool LaneE => Menu.Item("LaneE").GetValue<bool>();
        private static bool WInterrupt => Menu.Item("WInterrupt").GetValue<bool>();
        private static bool Qstrange => Menu.Item("Qstrange").GetValue<bool>();
        private static bool FirstHydra => Menu.Item("FirstHydra").GetValue<bool>();
        private static bool LaneQ => Menu.Item("LaneQ").GetValue<bool>();
        private static bool Youmu => Menu.Item("youmu").GetValue<bool>();

      public static void OnGameLoad()
        {

            if (Player.ChampionName != "Riven") return;
            Chat.Print("Hoola Riven - Loaded Successfully, Good Luck! :):)");
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 300);
            R = new Spell(SpellSlot.R, 900);
            R.SetSkillshot(0.25f, 45, 1600, false, SkillshotType.SkillshotCone);

            OnMenuLoad();


            Timer = new Render.Text("Q Expiry =>  " + ((double)(LastQ - Utils.GameTimeTickCount + 3800) / 1000).ToString("0.0"), (int)Drawing.WorldToScreen(Player.Position).X - 140, (int)Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.MidnightBlue, "calibri");
            Timer2 = new Render.Text("R Expiry =>  " + (((double)LastR - Utils.GameTimeTickCount + 15000) / 1000).ToString("0.0"), (int)Drawing.WorldToScreen(Player.Position).X - 60, (int)Drawing.WorldToScreen(Player.Position).Y + 10, 30, Color.IndianRed, "calibri");

            Game.OnUpdate += OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCastLC;
            Obj_AI_Base.OnPlayAnimation += OnPlay;
            Obj_AI_Base.OnProcessSpellCast += OnCasting;
            Interrupter2.OnInterruptableTarget += Interrupt;
        }

        private static bool HasTitan() => (Items.HasItem(3748) && Items.CanUseItem(3748));

        private static void CastTitan()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
                Orbwalking.LastAATick = 0;
            }
        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (Dind)
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new ColorBGRA(255, 204, 0, 170));
                }

            }
        }

      private static void OnSpellCastLC(Obj_AI_Base Sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Sender.IsMe || !Orbwalking.IsAutoAttack((args.SData.Name))) return;
            QTarget = (Obj_AI_Base) args.Target;
            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var Minions = MinionManager.GetMinions(70 + 120 + Player.BoundingRadius);
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (Q.LSIsReady() && LaneQ)
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ()=>ForceCastQ(Minions[0]));
                    }
                    if ((!Q.LSIsReady() || (Q.LSIsReady() && !LaneQ)) && W.LSIsReady() && LaneW != 0 &&
                        Minions.Count >= LaneW)
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
                    }
                    if ((!Q.LSIsReady() || (Q.LSIsReady() && !LaneQ)) && (!W.LSIsReady() || (W.LSIsReady() && LaneW == 0) || Minions.Count < LaneW) &&
                        E.LSIsReady() && LaneE)
                    {
                        E.Cast(Minions[0].Position);
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ForceItem);
                    }
                }
            }
        }
        private static int Item => Items.CanUseItem(3077) && Items.HasItem(3077) ? 3077 : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;
      private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellName = args.SData.Name;
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(spellName)) return;
            QTarget = (Obj_AI_Base)args.Target;

            if (args.Target is Obj_AI_Minion)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    var Mobs = MinionManager.GetMinions(120 + 70 + Player.BoundingRadius, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                    if (Mobs.Count != 0)
                    {
                        if (HasTitan())
                        {
                            CastTitan();
                            return;
                        }
                        if (Q.LSIsReady())
                        {
                            ForceItem();
                            LeagueSharp.Common.Utility.DelayAction.Add(1, ()=> ForceCastQ(Mobs[0]));
                        }
                        else if (W.LSIsReady())
                        {
                            ForceItem();
                            LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
                        }
                        else if (E.LSIsReady())
                        {
                            E.Cast(Mobs[0].Position);
                        }
                    }
                }
            }
            if (args.Target is Obj_AI_Turret || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener || args.Target is Obj_Building) if (args.Target.IsValid && args.Target != null && Q.LSIsReady() && LaneQ && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) ForceCastQ((Obj_AI_Base)args.Target);
            if (args.Target is AIHeroClient)
            {
            var target = (AIHeroClient)args.Target;
                if (KillstealR && R.LSIsReady() && R.Instance.Name == IsSecondR) if (target.Health < (Rdame(target, target.Health) + Player.LSGetAutoAttackDamage(target)) && target.Health > Player.LSGetAutoAttackDamage(target)) R.Cast(target.Position);
                if (KillstealW && W.LSIsReady()) if (target.Health < (W.GetDamage(target) + Player.LSGetAutoAttackDamage(target)) && target.Health > Player.LSGetAutoAttackDamage(target)) W.Cast();
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (Q.LSIsReady())
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ()=>ForceCastQ(target));
                    }
                    else if (W.LSIsReady() && InWRange(target))
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
                    }
                    else if (E.LSIsReady() && !Orbwalking.InAutoAttackRange(target)) E.Cast(target.Position);
                }
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.FastHarass)
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (W.LSIsReady() && InWRange(target))
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
                        LeagueSharp.Common.Utility.DelayAction.Add(2, () => ForceCastQ(target));
                    }
                    else if (Q.LSIsReady())
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1,()=>ForceCastQ(target));
                    }
                    else if (E.LSIsReady() && !Orbwalking.InAutoAttackRange(target) && !InWRange(target))
                    {
                        E.Cast(target.Position);
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (QStack == 2 && Q.LSIsReady())
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, () => ForceCastQ(target));
                    }
                }

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst)
                {
                    if (HasTitan())
                    {
                        CastTitan();
                        return;
                    }
                    if (R.LSIsReady() && R.Instance.Name == IsSecondR)
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ForceR2);
                    }
                    else if (Q.LSIsReady())
                    {
                        ForceItem();
                        LeagueSharp.Common.Utility.DelayAction.Add(1, ()=>ForceCastQ(target));
                    }
                }
            }
        }
      private static void OnMenuLoad()
        {
            Menu = new Menu("Hoola Riven", "hoolariven", true);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            var orbwalker = new Menu("Orbwalk", "rorb");
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddSubMenu(orbwalker);
            var Combo = new Menu("Combo", "Combo");

            Combo.AddItem(new MenuItem("AlwaysR", "Always Use R (Toggle)").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            Combo.AddItem(new MenuItem("UseHoola", "Use Hoola Combo Logic (Toggle)").SetValue(new KeyBind('L', KeyBindType.Toggle)));
            Combo.AddItem(new MenuItem("ComboW", "Always use W").SetValue(true));
            Combo.AddItem(new MenuItem("RKillable", "Use R When Target Can Killable").SetValue(true));


            Menu.AddSubMenu(Combo);
            var Lane = new Menu("Lane", "Lane");
            Lane.AddItem(new MenuItem("LaneQ", "Use Q While Laneclear").SetValue(true));
            Lane.AddItem(new MenuItem("LaneW", "Use W X Minion (0 = Don't)").SetValue(new Slider(5, 0, 5)));
            Lane.AddItem(new MenuItem("LaneE", "Use E While Laneclear").SetValue(true));



            Menu.AddSubMenu(Lane);
            var Misc = new Menu("Misc", "Misc");

            Misc.AddItem(new MenuItem("youmu", "Use Youmus When E").SetValue(false));
            Misc.AddItem(new MenuItem("FirstHydra", "Flash Burst Hydra Cast before W").SetValue(false));
            Misc.AddItem(new MenuItem("Qstrange", "Strange Q For Speed").SetValue(false));
            Misc.AddItem(new MenuItem("Winterrupt", "W interrupt").SetValue(true));
            Misc.AddItem(new MenuItem("AutoW", "Auto W When x Enemy").SetValue(new Slider(5, 0, 5)));
            Misc.AddItem(new MenuItem("RMaxDam", "Use Second R Max Damage").SetValue(true));
            Misc.AddItem(new MenuItem("killstealw", "Killsteal W").SetValue(true));
            Misc.AddItem(new MenuItem("killstealr", "Killsteal Second R").SetValue(true));
            Misc.AddItem(new MenuItem("AutoShield", "Auto Cast E").SetValue(true));
            Misc.AddItem(new MenuItem("Shield", "Auto Cast E While LastHit").SetValue(true));
            Misc.AddItem(new MenuItem("KeepQ", "Keep Q Alive").SetValue(true));
            Misc.AddItem(new MenuItem("QD", "First,Second Q Delay").SetValue(new Slider(29, 23, 43)));
            Misc.AddItem(new MenuItem("QLD", "Third Q Delay").SetValue(new Slider(39, 36, 53)));


            Menu.AddSubMenu(Misc);

            var Draw = new Menu("Draw", "Draw");

            Draw.AddItem(new MenuItem("DrawAlwaysR", "Draw Always R Status").SetValue(true));
            Draw.AddItem(new MenuItem("DrawTimer1", "Draw Q Expiry Time").SetValue(true));
            Draw.AddItem(new MenuItem("DrawTimer2", "Draw R Expiry Time").SetValue(true));
            Draw.AddItem(new MenuItem("DrawUseHoola", "Draw Hoola Logic Status").SetValue(true));
            Draw.AddItem(new MenuItem("Dind", "Draw Damage Indicator").SetValue(true));
            Draw.AddItem(new MenuItem("DrawCB", "Draw Combo Engage Range").SetValue(false));
            Draw.AddItem(new MenuItem("DrawBT", "Draw Burst Engage Range").SetValue(false));
            Draw.AddItem(new MenuItem("DrawFH", "Draw FastHarass Engage Range").SetValue(false));
            Draw.AddItem(new MenuItem("DrawHS", "Draw Harass Engage Range").SetValue(false));

            Menu.AddSubMenu(Draw);

            var Credit = new Menu("Credit", "Credit");

            Credit.AddItem(new MenuItem("hoola", "Made by Hoola :)"));
            Credit.AddItem(new MenuItem("notfixe", "If High ping will be many buggy"));
            Credit.AddItem(new MenuItem("notfixed", "Not Fixed Anything Yet"));
            Credit.AddItem(new MenuItem("feedback", "So Feedback To Hoola!"));

            Menu.AddSubMenu(Credit);

            Menu.AddToMainMenu();
        }

      private static void Interrupt(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsEnemy && W.LSIsReady() && sender.LSIsValidTarget() && !sender.IsZombie && WInterrupt)
            {
                if (sender.LSIsValidTarget(125 + Player.BoundingRadius + sender.BoundingRadius)) W.Cast();
            }
        }

        private static int GetWRange => Player.LSHasBuff("RivenFengShuiEngine") ? 330 : 265;

      private static void AutoUseW()
        {
            if (AutoW > 0)
            {
                if (Player.LSCountEnemiesInRange(GetWRange) >= AutoW)
                {
                    ForceW();
                }
            }
        }

      private static void OnTick(EventArgs args)
        {
            Timer.X = (int)Drawing.WorldToScreen(Player.Position).X - 60;
            Timer.Y = (int)Drawing.WorldToScreen(Player.Position).Y + 43;
            Timer2.X = (int)Drawing.WorldToScreen(Player.Position).X - 60;
            Timer2.Y = (int)Drawing.WorldToScreen(Player.Position).Y + 65;
            ForceSkill();
            UseRMaxDam();
            AutoUseW();
            Killsteal();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) Jungleclear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.FastHarass) FastHarass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst) Burst();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee) Flee();
            if (Utils.GameTimeTickCount - LastQ >= 3650 && QStack != 1 && !Player.LSIsRecalling() && KeepQ && Q.LSIsReady()) Q.Cast(Game.CursorPos);
        }

      private static void Killsteal()
        {
            if (KillstealW && W.LSIsReady())
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < W.GetDamage(target) && InWRange(target))
                        W.Cast();
                }
            }
            if (KillstealR && R.LSIsReady() && R.Instance.Name == IsSecondR)
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health < Rdame(target, target.Health) && (!target.LSHasBuff("kindrednodeathbuff") && !target.LSHasBuff("Undying Rage") && !target.LSHasBuff("JudicatorIntervention")))
                        R.Cast(target.Position);
                }
            }
        }
      private static void UseRMaxDam()
        {
            if (RMaxDam && R.LSIsReady() && R.Instance.Name == IsSecondR)
            {
                var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget(R.Range) && !x.IsZombie);
                foreach (var target in targets)
                {
                    if (target.Health / target.MaxHealth <= 0.25 && (!target.LSHasBuff("kindrednodeathbuff") || !target.LSHasBuff("Undying Rage") || !target.LSHasBuff("JudicatorIntervention")))
                        R.Cast(target.Position);
                }
            }
        }

      private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);


            if (QStack != 1 && DrawTimer1)
            {
                Timer.text = ("Q Expiry =>  " + ((double)(LastQ - Utils.GameTimeTickCount + 3800) / 1000).ToString("0.0")+"S");
                Timer.OnEndScene();
            }

            if (Player.LSHasBuff("RivenFengShuiEngine") && DrawTimer2)
            {
                Timer2.text = ("R Expiry =>  " + (((double)LastR - Utils.GameTimeTickCount + 15000) / 1000).ToString("0.0") +"S");
                Timer2.OnEndScene();
            }

            if (DrawCB) Render.Circle.DrawCircle(Player.Position, 250 + Player.AttackRange + 70, E.LSIsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawBT && Flash != SpellSlot.Unknown) Render.Circle.DrawCircle(Player.Position, 800, R.LSIsReady() && Flash.LSIsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawFH) Render.Circle.DrawCircle(Player.Position, 450 + Player.AttackRange + 70, E.LSIsReady() && Q.LSIsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawHS) Render.Circle.DrawCircle(Player.Position, 400, Q.LSIsReady() && W.LSIsReady() ? System.Drawing.Color.FromArgb(120, 0, 170, 255) : System.Drawing.Color.IndianRed);
            if (DrawAlwaysR) 
            {
                Drawing.DrawText(heropos.X -40, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Always R  (     )");
                Drawing.DrawText(heropos.X + 40, heropos.Y + 20, AlwaysR ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, AlwaysR ? "On" : "Off");
            }
            if (DrawUseHoola)
            {
                Drawing.DrawText(heropos.X -40, heropos.Y + 33, System.Drawing.Color.DodgerBlue, "Hoola Logic  (     )");
                Drawing.DrawText(heropos.X + 60, heropos.Y + 33, UseHoola ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, UseHoola ? "On" : "Off");
            }
        }

      private static void Jungleclear()
        {

            var Mobs = MinionManager.GetMinions(250 + Player.AttackRange + 70, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

            if (W.LSIsReady() && E.LSIsReady() && !Orbwalking.InAutoAttackRange(Mobs[0]))
            {
                E.Cast(Mobs[0].Position);
                LeagueSharp.Common.Utility.DelayAction.Add(1, ForceItem);
                LeagueSharp.Common.Utility.DelayAction.Add(200, ForceW);
            }
        }

      private static void Combo()
        {
            var targetR = TargetSelector.GetTarget(250 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);
            if (R.LSIsReady() && R.Instance.Name == IsFirstR && Orbwalker.InAutoAttackRange(targetR) && AlwaysR && targetR != null) ForceR();
            if (R.LSIsReady() && R.Instance.Name == IsFirstR && W.LSIsReady() && InWRange(targetR) && ComboW && AlwaysR && targetR != null)
            {
                ForceR();
                LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
            }
            if (W.LSIsReady() && InWRange(targetR) && ComboW && targetR != null) W.Cast();
            if (UseHoola && R.LSIsReady() && R.Instance.Name == IsFirstR && W.LSIsReady() && targetR != null && E.LSIsReady() && targetR.LSIsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
            {
                if (!InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(200, ForceW);
                    LeagueSharp.Common.Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                }
            }
            else if (!UseHoola && R.LSIsReady() && R.Instance.Name == IsFirstR && W.LSIsReady() && targetR != null && E.LSIsReady() && targetR.LSIsValidTarget() && !targetR.IsZombie && (IsKillableR(targetR) || AlwaysR))
            {
                if (!InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(200, ForceW);
                }
            }
            else if (UseHoola && W.LSIsReady() && E.LSIsReady())
            {
                if (targetR.LSIsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    LeagueSharp.Common.Utility.DelayAction.Add(10, ForceItem);
                    LeagueSharp.Common.Utility.DelayAction.Add(200, ForceW);
                    LeagueSharp.Common.Utility.DelayAction.Add(305, () => ForceCastQ(targetR));
                }
            }
            else if (!UseHoola && W.LSIsReady() && targetR != null && E.LSIsReady())
            {
                if (targetR.LSIsValidTarget() && targetR != null && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                    LeagueSharp.Common.Utility.DelayAction.Add(10, ForceItem);
                    LeagueSharp.Common.Utility.DelayAction.Add(240, ForceW);
                }
            }
            else if (E.LSIsReady())
            {
                if (targetR.LSIsValidTarget() && !targetR.IsZombie && !InWRange(targetR))
                {
                    E.Cast(targetR.Position);
                }
            }
        }

      private static void Burst()
        {
            var target = TargetSelector.GetSelectedTarget();
            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                if (R.LSIsReady() && R.Instance.Name == IsFirstR && W.LSIsReady() && E.LSIsReady() && Player.LSDistance(target.Position) <= 250 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(100, ForceW);
                }
                else if (R.LSIsReady() && R.Instance.Name == IsFirstR && E.LSIsReady() && W.LSIsReady() && Q.LSIsReady() &&
                         Player.LSDistance(target.Position) <= 400 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(150,()=>ForceCastQ(target));
                    LeagueSharp.Common.Utility.DelayAction.Add(160, ForceW);
                }
                else if (Flash.LSIsReady()
                    && R.LSIsReady() && R.Instance.Name == IsFirstR && (Player.LSDistance(target.Position) <= 800) && (!FirstHydra || (FirstHydra && !HasItem())))
                {
                    E.Cast(target.Position);
                    CastYoumoo();
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(180, FlashW);
                }
                else if (Flash.LSIsReady()
                    && R.LSIsReady() && E.LSIsReady() && W.LSIsReady() && R.Instance.Name == IsFirstR && (Player.LSDistance(target.Position) <= 800) && FirstHydra && HasItem())
                {
                    E.Cast(target.Position);
                    ForceR();
                    LeagueSharp.Common.Utility.DelayAction.Add(100, ForceItem);
                    LeagueSharp.Common.Utility.DelayAction.Add(210, FlashW);
                }
            }
        }

      private static void FastHarass()
        {
            if (Q.LSIsReady() && E.LSIsReady())
            {
                var target = TargetSelector.GetTarget(450 + Player.AttackRange + 70, TargetSelector.DamageType.Physical);
                if (target.LSIsValidTarget() && !target.IsZombie)
                {
                    if (!Orbwalking.InAutoAttackRange(target) && !InWRange(target)) E.Cast(target.Position);
                    LeagueSharp.Common.Utility.DelayAction.Add(10, ForceItem);
                    LeagueSharp.Common.Utility.DelayAction.Add(170, ()=>ForceCastQ(target));
                }
            }
        }

      private static void Harass()
        {
            var target = TargetSelector.GetTarget(400, TargetSelector.DamageType.Physical);
            if (Q.LSIsReady() && W.LSIsReady() && E.LSIsReady() && QStack == 1)
            {
                if (target.LSIsValidTarget() && !target.IsZombie)
                {
                    ForceCastQ(target);
                    LeagueSharp.Common.Utility.DelayAction.Add(1, ForceW);
                }
            }
            if (Q.LSIsReady() && E.LSIsReady() && QStack == 3 && !Orbwalking.CanAttack() && Orbwalking.CanMove(5))
            {
                var epos = Player.ServerPosition +
                          (Player.ServerPosition - target.ServerPosition).LSNormalized() * 300;
                E.Cast(epos);
                LeagueSharp.Common.Utility.DelayAction.Add(190, () => Q.Cast(epos));
            }
        }

      private static void Flee()
        {
            var enemy =
                HeroManager.Enemies.Where(
                    hero =>
                        hero.LSIsValidTarget(Player.LSHasBuff("RivenFengShuiEngine")
                            ? 70 + 195 + Player.BoundingRadius
                            : 70 + 120 + Player.BoundingRadius) && W.LSIsReady());
            var x = Player.Position.LSExtend(Game.CursorPos, 300);
            if (W.LSIsReady() && enemy.Any()) foreach (var target in enemy) if (InWRange(target)) W.Cast();
            if (Q.LSIsReady() && !Player.LSIsDashing()) Q.Cast(Game.CursorPos);
            if (E.LSIsReady() && !Player.LSIsDashing()) E.Cast(x);
        }

      private static void OnPlay(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsMe) return;

            switch (args.Animation)
            {
                case "Spell1a":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None) Chat.Say("/d");
                    QStack = 2;
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Flee) LeagueSharp.Common.Utility.DelayAction.Add((QD * 10) + 1, Reset);
                    break;
                case "Spell1b":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None) Chat.Say("/d");
                    QStack = 3;
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Flee) LeagueSharp.Common.Utility.DelayAction.Add((QD * 10) + 1, Reset);
                    break;
                case "Spell1c":
                    LastQ = Utils.GameTimeTickCount;
                    if (Qstrange && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None) Chat.Say("/d");
                    QStack = 1;
                    if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.None && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.LastHit && Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Flee) LeagueSharp.Common.Utility.DelayAction.Add((QLD * 10) + 3, Reset);
                    break;
                case "Spell3":
                    if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Burst ||
                        Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                        Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.FastHarass ||
                        Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Flee) && Youmu) CastYoumoo();
                    break;
                case "Spell4a":
                    LastR = Utils.GameTimeTickCount;
                    break;
                case "Spell4b":
                    var target = TargetSelector.GetSelectedTarget();
                    if (Q.LSIsReady() && target.LSIsValidTarget()) ForceCastQ(target);
                    break;
            }
        }

      private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name.Contains("ItemTiamatCleave")) forceItem = false;
            if (args.SData.Name.Contains("RivenTriCleave")) forceQ = false;
            if (args.SData.Name.Contains("RivenMartyr")) forceW = false;
            if (args.SData.Name == IsFirstR) forceR = false;
            if (args.SData.Name == IsSecondR) forceR2 = false;
        }

      private static void Reset()
        {
            Chat.Say("/d");
            Orbwalking.LastAATick = 0;
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.LSExtend(Game.CursorPos, Player.LSDistance(Game.CursorPos) + 10));
        }

      private static bool InWRange(GameObject target)=> (Player.LSHasBuff("RivenFengShuiEngine") && target != null) ?
                    330 >= Player.LSDistance(target.Position) : 265 >= Player.LSDistance(target.Position);
        

        private static void ForceSkill()
        {
            if (forceQ && QTarget != null && QTarget.LSIsValidTarget(E.Range + Player.BoundingRadius + 70) && Q.LSIsReady()) Q.Cast(QTarget.Position);
            if (forceW) W.Cast();
            if (forceR && R.Instance.Name == IsFirstR) R.Cast();
            if (forceItem && Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) Items.UseItem(Item);
            if (forceR2 && R.Instance.Name == IsSecondR)
            {
                var target = TargetSelector.GetSelectedTarget();
                if (target != null) R.Cast(target.Position);
            }
        }

        private static void ForceItem()
        {
            if (Items.CanUseItem(Item) && Items.HasItem(Item) && Item != 0) forceItem = true;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceItem = false);
        }
        private static void ForceR()
        {
            forceR = (R.LSIsReady() && R.Instance.Name == IsFirstR);
            LeagueSharp.Common.Utility.DelayAction.Add(500, ()=> forceR = false);
        }
        private static void ForceR2()
        {
            forceR2 = R.LSIsReady() && R.Instance.Name == IsSecondR;
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceR2 = false);
        }
        private static void ForceW()
        {
            forceW = W.LSIsReady();
            LeagueSharp.Common.Utility.DelayAction.Add(500, () => forceW = false);
        }

      private static void ForceCastQ(AttackableUnit target)
        {
            forceQ = true;
            QTarget = target;
        }


      private static void FlashW()
        {
            var target = TargetSelector.GetSelectedTarget();
            if (target != null && target.LSIsValidTarget() && !target.IsZombie)
            {
                W.Cast();
                LeagueSharp.Common.Utility.DelayAction.Add(10, () => Player.Spellbook.CastSpell(Flash, target.Position));
            }
        }

        private static bool HasItem() => ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady();

        private static void CastYoumoo(){if(ItemData.Youmuus_Ghostblade.GetItem().IsReady())ItemData.Youmuus_Ghostblade.GetItem().Cast();}
      private static void OnCasting(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && sender.Type == Player.Type && (AutoShield || (Shield && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)))
            {
                var epos = Player.ServerPosition +
                          (Player.ServerPosition - sender.ServerPosition).LSNormalized() * 300;

                if (Player.LSDistance(sender.ServerPosition) <= args.SData.CastRange)
                {
                    switch (args.SData.TargettingType)
                    {
                        case SpellDataTargetType.Unit:

                            if (args.Target.NetworkId == Player.NetworkId)
                            {
                                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit && !args.SData.Name.Contains("NasusW"))
                                {
                                    if (E.LSIsReady()) E.Cast(epos);
                                }
                            }

                            break;
                        case SpellDataTargetType.SelfAoe:

                            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                            {
                                if (E.LSIsReady()) E.Cast(epos);
                            }

                            break;
                    }
                    if (args.SData.Name.Contains("IreliaEquilibriumStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady() && InWRange(sender)) W.Cast();
                            else if (E.LSIsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("TalonCutthroat"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RenektonPreExecute"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("GarenRPreCast"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast(epos);
                        }
                    }
                    if (args.SData.Name.Contains("GarenQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("XenZhaoThrust3"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarQ"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("TwitchEParticle"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("FizzPiercingStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("HungeringStrike"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaRTrigger"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady() && InWRange(sender)) W.Cast();
                            else if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("YasuoDash"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("KatarinaE"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (W.LSIsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                            else if (W.LSIsReady()) W.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                    if (args.SData.Name.Contains("MonkeyKingQAttack"))
                    {
                        if (args.Target.NetworkId == Player.NetworkId)
                        {
                            if (E.LSIsReady()) E.Cast();
                        }
                    }
                }
            }
        }
        
      private static double basicdmg(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.LSGetAutoAttackDamage(target) * 0.7;
                if (W.LSIsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.LSIsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.LSGetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.LSGetAutoAttackDamage(target) * (1 + passivenhan);
                return dmg;
            }
            return 0;
        }


      private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                float passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5f; }
                else if (Player.Level >= 15) { passivenhan = 0.45f; }
                else if (Player.Level >= 12) { passivenhan = 0.4f; }
                else if (Player.Level >= 9) { passivenhan = 0.35f; }
                else if (Player.Level >= 6) { passivenhan = 0.3f; }
                else if (Player.Level >= 3) { passivenhan = 0.25f; }
                else { passivenhan = 0.2f; }
                if (HasItem()) damage = damage + (float)Player.LSGetAutoAttackDamage(enemy) * 0.7f;
                if (W.LSIsReady()) damage = damage + W.GetDamage(enemy);
                if (Q.LSIsReady())
                {
                    var qnhan = 4 - QStack;
                    damage = damage + Q.GetDamage(enemy) * qnhan + (float)Player.LSGetAutoAttackDamage(enemy) * qnhan * (1 + passivenhan);
                }
                damage = damage + (float)Player.LSGetAutoAttackDamage(enemy) * (1 + passivenhan);
                if (R.LSIsReady())
                {
                    return damage * 1.2f + R.GetDamage(enemy);
                }

                return damage;
            }
            return 0;
        }

        public static bool IsKillableR(AIHeroClient target)
        {
            if (RKillable && target.LSIsValidTarget() && (totaldame(target) >= target.Health
                 && basicdmg(target) <= target.Health) || Player.LSCountEnemiesInRange(900) >= 2 && (!target.LSHasBuff("kindrednodeathbuff") && !target.LSHasBuff("Undying Rage") && !target.LSHasBuff("JudicatorIntervention")))
            {
                return true;
            }
            return false;
        }

      private static double totaldame(Obj_AI_Base target)
        {
            if (target != null)
            {
                double dmg = 0;
                double passivenhan = 0;
                if (Player.Level >= 18) { passivenhan = 0.5; }
                else if (Player.Level >= 15) { passivenhan = 0.45; }
                else if (Player.Level >= 12) { passivenhan = 0.4; }
                else if (Player.Level >= 9) { passivenhan = 0.35; }
                else if (Player.Level >= 6) { passivenhan = 0.3; }
                else if (Player.Level >= 3) { passivenhan = 0.25; }
                else { passivenhan = 0.2; }
                if (HasItem()) dmg = dmg + Player.LSGetAutoAttackDamage(target) * 0.7;
                if (W.LSIsReady()) dmg = dmg + W.GetDamage(target);
                if (Q.LSIsReady())
                {
                    var qnhan = 4 - QStack;
                    dmg = dmg + Q.GetDamage(target) * qnhan + Player.LSGetAutoAttackDamage(target) * qnhan * (1 + passivenhan);
                }
                dmg = dmg + Player.LSGetAutoAttackDamage(target) * (1 + passivenhan);
                if (R.LSIsReady())
                {
                    var rdmg = Rdame(target, target.Health - dmg * 1.2);
                    return dmg * 1.2 + rdmg;
                }
                return dmg;
            }
            return 0;
        }

      private static double Rdame(Obj_AI_Base target, double health)
        {
            if (target != null)
            {
                var missinghealth = (target.MaxHealth - health) / target.MaxHealth > 0.75 ? 0.75 : (target.MaxHealth - health) / target.MaxHealth;
                var pluspercent = missinghealth * (8 / 3);
                var rawdmg = new double[] { 80, 120, 160 }[R.Level - 1] + 0.6 * Player.FlatPhysicalDamageMod;
                return Player.CalcDamage(target, Damage.DamageType.Physical, rawdmg * (1 + pluspercent));
            }
            return 0;
        }
    }
}