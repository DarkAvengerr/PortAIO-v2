using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using myWorld.Library.MenuWarpper;
using myWorld.Library.STS;
using myWorld.Library.Draw;
using myWorld.Library.DamageManager;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Jinx
{
    class Jinx
    {
        static Menu Menu;
        static Spell Q, W, E, R;
        static SimpleTS STS = new SimpleTS();
        static int QRange = 600;
        static DrawManager DM = new DrawManager();
        static DamageLib DLib = new DamageLib(ObjectManager.Player);

        bool isFishBones
        {
            get
            {
                return !ObjectManager.Player.HasBuff("JinxQ");
            }
        }

        int FishStacks
        {
            get
            {
                return ObjectManager.Player.GetBuffCount("jinxqramp");
            }
        }

        public Jinx()
        {
            Menu = Program.MainMenu;

            #region Spell

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 750);
            W.SetSkillshot(0.6f, 70f, 3300f, true, SkillshotType.SkillshotLine);
            E = new Spell(SpellSlot.E, 920);
            E.SetSkillshot(1.2f, 100f, 1750f, false, SkillshotType.SkillshotCircle);
            R = new Spell(SpellSlot.R, 3000);
            R.SetSkillshot(0.7f, 140f, 1500f, false, SkillshotType.SkillshotLine);

            #endregion

            DLib.RegistDamage("W", DamageType.Physical, 10f, 50f, DamageType.Physical, ScalingType.AD, 1.4f, delegate(Obj_AI_Base target) { return W.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });
            DLib.RegistDamage("E", DamageType.Physical, 80f, 55f, DamageType.Physical, ScalingType.AP, 1f, delegate(Obj_AI_Base target) { return E.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });
            DLib.RegistDamage("R", DamageType.Physical, 250f, 100f, DamageType.Physical, ScalingType.AD, 0.1f, delegate(Obj_AI_Base target) { return R.IsReady(); }, delegate(Obj_AI_Base target) { return (float)W.GetDamage(target) + (float)E.GetDamage(target) + (float)(ObjectManager.Player.GetAutoAttackDamage(target) * 2); });
            

            DM.AddCircle(ObjectManager.Player, W.Range, Color.Red, "W Draw", delegate() { return W.IsReady();});
            DM.AddCircle(ObjectManager.Player, E.Range, Color.Red, "E Draw", delegate() { return E.IsReady();});

            List<string> hitChances = new List<string>();
            foreach (HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                hitChances.Add(value.ToString());
            }

            Menu Config = new Menu("Jinx", "Jinx");

            Menu HitChanceMenu = new Menu("HitChance", "HitChance");

            Menu ComboHitChaceMenu = new Menu("Combo", "Combo");
            ComboHitChaceMenu.AddList("HitChance.Combo.W", "W HitChance", new StringList(hitChances.ToArray(), 5));
            ComboHitChaceMenu.AddList("HitChance.Combo.E", "E HitChance", new StringList(hitChances.ToArray(), 4));
            HitChanceMenu.AddSubMenu(ComboHitChaceMenu);

            Menu HarassHitChaceMenu = new Menu("Harass", "Harass");
            HarassHitChaceMenu.AddList("HitChance.Harass.W", "W HitChance", new StringList(hitChances.ToArray(), 4));
            HarassHitChaceMenu.AddList("HitChance.Harass.E", "E HitChance", new StringList(hitChances.ToArray(), 3));
            HitChanceMenu.AddSubMenu(HarassHitChaceMenu);

            Config.AddSubMenu(HitChanceMenu);

            DLib.AddToMenu(Config, new List<string>() { "W", "E", "R" });

            STS.AddToMenu(Config);

            DM.AddToMenu(Config);
            
            Menu Combo = new Menu("Combo", "Combo");
            Combo.AddBool("Combo.UseQ", "Use Q");
            Combo.AddBool("Combo.UseW", "Use W");
            Combo.AddBool("Combo.UseE", "Use E");
            Combo.AddBool("Combo.UseR", "Use R");
            Combo.AddSlice("Combo.mper","Dont use if my mana => (%)", 10);
            Config.AddSubMenu(Combo);

            Menu Harass = new Menu("Harass", "Harass");
            Harass.AddBool("Harass.UseQ", "Use Q");
            Harass.AddBool("Harass.UseW", "Use W");
            Harass.AddBool("Harass.UseE", "Use E");
            Harass.AddSlice("Harass.mper");
            Config.AddSubMenu(Harass);

            Menu KS = new Menu("Killsteal", "Killsteal");
            KS.AddBool("KS.UseW", "Use W");
            KS.AddBool("KS.UseE", "Use E");
            Config.AddSubMenu(KS);

            Menu Misc = new Menu("Misc", "Misc");
            Misc.AddSlice("Misc.RRange", "Max R Range", 1700, 0, 3000);
            Misc.AddSlice("Misc.WRange", "Max W Range", 300, 0, 720);
            Misc.AddSlice("Misc.MinRRange", "Min R Range", 600, 0, 1800);
            Misc.AddSlice("Misc.REnemies", "Min enemies for auto r", 4, 1, 5);
            Misc.AddBool("Misc.ROverkill", "Check R Overkill");
            Misc.AddBool("Misc.EStun", "Auto E Stun");
            Misc.AddBool("Misc.EGapcloser", "Auto E Gapcloser");
            Misc.AddBool("Misc.EAutoCast", "Auto E Slow/Immobile/Dash");
            Misc.AddBool("Misc.SwapThree", "Swap Q at three fishbone stacks");
            Misc.AddBool("Misc.SwapDistance", "Swap Q for Distance");
            Misc.AddBool("Misc.SwapAOE", "Swap Q for AoE");
            Config.AddSubMenu(Misc);

            Menu.AddSubMenu(Config);
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }

        void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            //throw new NotImplementedException();
            if (Menu.GetBool("Misc.EGapcloser") && E.IsInRange(gapcloser.End))
            {
                E.Cast(gapcloser.End);
            }
        }

        void Drawing_OnDraw(EventArgs args)
        {
            //throw new NotImplementedException();
            //Drawing.DrawText(100, 100, System.Drawing.Color.Red, isFishBones.ToString());
            //Drawing.DrawText(100, 120, System.Drawing.Color.Red, FishStacks.ToString());
        }

        void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();
            QRange = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 25 + 50 + 600;


            if(Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }
            if(Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass();

            }
            if(Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && !isFishBones)
            {
                Q.Cast();
            }
            if(Menu.GetBool("Misc.EAutoCast"))
            {
                foreach(Obj_AI_Base target in HeroManager.Enemies.Where(x => E.IsInRange(x)))
                {
                    E.CastIfHitchanceEquals(target, HitChance.Dashing);
                    E.CastIfHitchanceEquals(target, HitChance.Immobile);
                    if(target.HasBuffOfType(BuffType.Slow))
                    {
                        E.Cast(target);
                    }
                }
            }
        }

        void Combo()
        {
            Obj_AI_Base target = STS.GetTarget(1500);
            if(target != null)
            {
                if(W.IsInRange(target) && Menu.GetBool("Combo.UseW"))
                {
                    CastW(target, CastMode.Combo);
                }
                if(E.IsReady() && Menu.GetBool("Combo.UseE"))
                {
                    CastE(target);
                }
                if (E.IsReady() && Menu.GetBool("Misc.EAutoCast"))
                {
                    AutoCastE(target, CastMode.Combo);
                }
                if(Q.IsReady() && Menu.GetBool("Combo.UseQ"))
                {
                    Swap(target);
                }
                if(R.IsReady() && Menu.GetBool("Combo.UseR"))
                {
                    CastR(target);
                }
            }
        }

        void Harass()
        {
            Obj_AI_Base target = STS.GetTarget(1500);
            if(target != null && !ObjectManager.Player.IsManaLow(Menu.GetSlice("Harass.mper")))
            {
                if (W.IsInRange(target) && Menu.GetBool("Harass.UseW"))
                {
                    CastW(target, CastMode.Combo);
                }
                if (E.IsReady() && Menu.GetBool("Harass.UseE"))
                {
                    CastE(target);
                }
                if (Q.IsReady() && Menu.GetBool("Harass.UseQ"))
                {
                    Swap(target);
                }
            }
        }

        void Swap(Obj_AI_Base target)
        {
            if(target != null && !target.IsDead && target.IsValidTarget() && Q.IsReady())
            {
                PredictionOutput value = Q.GetPrediction(target, true);
                if(isFishBones)
                {
                    if (Menu.GetBool("Misc.SwapThree") && FishStacks == 3 && value.CastPosition.Distance(ObjectManager.Player.Position) < QRange)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Menu.GetBool("Misc.SwapDistance") && ObjectManager.Player.Position.Distance(target.Position) > 600 + target.BoundingRadius / 2 && ObjectManager.Player.Position.Distance(value.CastPosition) > 600 + target.BoundingRadius / 2 && ObjectManager.Player.Position.Distance(value.CastPosition) < QRange + target.BoundingRadius / 2)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Menu.GetBool("Misc.SwapAOE") && FishStacks > 2 && target.CountEnemiesInRange(150) > 1)
                    {
                        Q.Cast();
                        return;
                    }
                }
                else
                {
                    if (Menu.GetBool("Misc.SwapAOE") && target.CountEnemiesInRange(150) > 1)
                    {
                        return;
                    }
                    if (Menu.GetBool("Misc.SwapThree") && FishStacks < 3 && ObjectManager.Player.Position.Distance(value.CastPosition) < 575 + target.BoundingRadius / 2 && ObjectManager.Player.Position.Distance(value.CastPosition) < 600 + target.BoundingRadius / 2)
                    {
                        Q.Cast();
                        return;
                    }
                    if (Menu.GetBool("Misc.SwapDistance") && ObjectManager.Player.Position.Distance(value.CastPosition) < 575 + target.BoundingRadius / 2 && ObjectManager.Player.Position.Distance(value.CastPosition) < 600 + target.BoundingRadius / 2)
                    {
                        Q.Cast();
                        return;
                    }
                    if(ObjectManager.Player.IsManaLow(Menu.GetSlice("Combo.mper")) && ObjectManager.Player.Position.Distance(value.CastPosition) < 600 + target.BoundingRadius )
                    {
                        Q.Cast();
                        return;
                    }
                    if(Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && ObjectManager.Player.Position.Distance(value.CastPosition) > 600 + target.BoundingRadius +50)
                    {
                        Q.Cast();
                        return;
                    }
                }
            }
        }

        void CastE(Obj_AI_Base target)
        {
            if(E.IsReady() && target.GetDistance() < 1100)
            {
                GetWallCollision(target);
            }
        }

        void AutoCastE(Obj_AI_Base target, CastMode mode)
        {
            if (target.IsDead || mode != CastMode.Killsteal && !Orbwalking.CanMove(0.5f))
            {
                return;
            }

            switch (mode)
            {
                case CastMode.Combo:
                    E.CastIfHigherThen(target, Menu.GetList("HitChance.Combo.E").ToHitChance());
                    break;
                case CastMode.Harass:
                    E.CastIfHigherThen(target, Menu.GetList("HitChance.Harass.E").ToHitChance());
                    break;
                case CastMode.Farm:
                    PredictionOutput value = E.GetPrediction(target);
                    if (value.Hitchance != HitChance.Collision && value.Hitchance != HitChance.OutOfRange && value.CastPosition != Vector3.Zero)
                    {
                        E.Cast(value.CastPosition);
                    }
                    break;
            }
        }


        void CastW(Obj_AI_Base target, CastMode mode)
        {
            if (target.IsDead || mode != CastMode.Killsteal && !Orbwalking.CanMove(0.5f))
            {
                return;
            }

            switch (mode)
            {
                case CastMode.Combo:
                    W.CastIfHigherThen(target, Menu.GetList("HitChance.Combo.W").ToHitChance());
                    break;
                case CastMode.Harass:
                    W.CastIfHigherThen(target, Menu.GetList("HitChance.Harass.W").ToHitChance());
                    break;
                case CastMode.Farm:
                    PredictionOutput value = W.GetPrediction(target);
                    if (value.Hitchance != HitChance.Collision && value.Hitchance != HitChance.OutOfRange && value.CastPosition != Vector3.Zero)
                    {
                        W.Cast(value.CastPosition);
                    }
                    break;
            }
        }

        void GetWallCollision(Obj_AI_Base target)
        {
            PredictionOutput v1 = Prediction.GetPrediction(target, 1f);
            PredictionOutput v2 = Prediction.GetPrediction(target, 0.25f);
            Vector3[] TargetWaypoints = target.Path;

            Vector3 Destination1 = TargetWaypoints[TargetWaypoints.Length - 1];
            Vector3 Destination2 = TargetWaypoints[0];
            Vector3 Destination13D = new Vector3(Destination1.X, ObjectManager.Player.Position.Y, Destination1.Y);
            if(v1.CastPosition != Vector3.Zero && v1.Hitchance.IsHigerThen(HitChance.Low) && v2.Hitchance.IsHigerThen(HitChance.Low) &&  Destination1.Distance(Destination2) > 100)
            {
                if(target.Position.Distance(v1.CastPosition)>5)
                {
                    Vector3 UnitVector = (v1.CastPosition - target.Position).Normalized();
                    GenerateWallVectorOutput v3 = GenerateWallVector(Destination13D);
                    Vector3 DisplacedVector = target.Position + (Destination13D - target.Position).Normalized() * (target.MoveSpeed * E.Delay+110);
                    var angle = Math.Acos(UnitVector.Length() / Math.Sqrt(UnitVector.Length() * v3.DiffVector.Length()));
                    if(angle*57.2957795<105 && angle*57.2957795 > 75 && DisplacedVector.Distance(ObjectManager.Player.Position) < E.Range && E.IsReady())
                    {
                        E.Cast(DisplacedVector);
                    }
                }
            }
        }

        GenerateWallVectorOutput GenerateWallVector(Vector3 Pos)
        {
            int WallDisplacement = 120;
            Vector3 HeroToWallVector = (Pos - ObjectManager.Player.Position).Normalized();
            Vector3 RotatedVec1 = HeroToWallVector.Perpendicular();
            Vector3 RotatedVec2 = HeroToWallVector.Perpendicular2();
            Vector3 EndPoint1 = Pos + RotatedVec1 * WallDisplacement;
            Vector3 EndPoint2 = Pos + RotatedVec2 * WallDisplacement;
            Vector3 DiffVector = (EndPoint2 - EndPoint1).Normalized();
            return new GenerateWallVectorOutput(EndPoint1, EndPoint2, DiffVector);

        }

        void CastR(Obj_AI_Base target)
        {
            if (target.IsDead) return;
            if(target.GetDistance() < Menu.GetSlice("Misc.RRange") && R.IsReady())
            {
                if(target.CountEnemiesInRange(250) > Menu.GetSlice("Misc.REnemies"))
                {
                    R.Cast(target, false, true);
                    return;
                }
            }
            if(target.GetDistance() < Menu.GetSlice("Misc.MinRRange") && Menu.GetBool("Misc.ROverkill") && target.GetDistance() < Menu.GetSlice("Misc.RRange"))
            {
                float RD = DLib.CalcSpellDamage(target, "R");
                float AD = (float)ObjectManager.Player.GetAutoAttackDamage(target);
                if (target.Health < AD * 3.5) return;
                else if(target.Health < RD)
                {
                    R.Cast(target);
                }
            }

            if(target.GetDistance() <Menu.GetSlice("Misc.RRange"))
            {
                float RD = DLib.CalcSpellDamage(target, "R");
                
                if(target.Health < RD)
                {
                    R.Cast(target);
                }
            }
        }
    }

    class GenerateWallVectorOutput
    {
        public Vector3 EndPoint1;
        public Vector3 EndPoint2;
        public Vector3 DiffVector;
        public GenerateWallVectorOutput(Vector3 EndPoint1, Vector3 EndPoint2, Vector3 DiffVector)
        {
            this.EndPoint1 = EndPoint1;
            this.EndPoint2 = EndPoint2;
            this.DiffVector = DiffVector;
        }
    }
}
