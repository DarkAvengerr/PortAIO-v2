using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using myWorld.Library.MenuWarpper;
using myWorld.Library.STS;
using myWorld.Library.DamageManager;
using myWorld.Library.Draw;

using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace myWorld.Champion.Ezreal
{
    class Ezreal
    {
        static Spell Q, W, E, R;
        static Menu Menu;
        static SimpleTS STS;
        static DamageLib DLib = new DamageLib(ObjectManager.Player);
        static DrawManager DM = new DrawManager();

        public Ezreal()
        {
            Menu = Program.MainMenu;

            STS = new SimpleTS();

            List<string> hitChances = new List<string>();
            foreach(HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                hitChances.Add(value.ToString());
            }

            Q = new Spell(SpellSlot.Q, 1200);
            Q.SetSkillshot(0.25f, 60f, 2000f, true, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W, 1050);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SkillshotType.SkillshotLine);

            E = new Spell(SpellSlot.E, 475);

            R = new Spell(SpellSlot.R, float.MaxValue);
            R.SetSkillshot(1f, 160f, 2000f, false, SkillshotType.SkillshotLine);

            DLib.RegistDamage("Q", DamageType.Physical, 35f, 20f, new List<DamageType>() { DamageType.Physical, DamageType.Physical }, new List<ScalingType>() { ScalingType.AD, ScalingType.AP }, new List<float>() { 1.1f, 0.4f }, delegate(Obj_AI_Base target) { return Q.IsReady(); }, delegate(Obj_AI_Base target) { return 0f; });

            DM.AddCircle(ObjectManager.Player, Q.Range, Color.Red, "Q Draw", delegate() { return Q.IsReady(); });
            DM.AddCircle(ObjectManager.Player, W.Range, Color.Red, "W Draw", delegate() { return W.IsReady(); });

            Menu Config = new Menu("Ezreal", "Ezreal");

            STS.AddToMenu(Config);

            Menu HitChanceMenu = new Menu("HitChance", "HitChance");

            Menu ComboHitChaceMenu = new Menu("Combo", "Combo");
            ComboHitChaceMenu.AddList("HitChance.Combo.Q", "Q HitChance", new StringList(hitChances.ToArray(), 5));
            ComboHitChaceMenu.AddList("HitChance.Combo.W", "W HitChance", new StringList(hitChances.ToArray(), 4));
            HitChanceMenu.AddSubMenu(ComboHitChaceMenu);

            Menu HarassHitChaceMenu = new Menu("Harass", "Harass");
            HarassHitChaceMenu.AddList("HitChance.Harass.Q", "Q HitChance", new StringList(hitChances.ToArray(), 4));
            HarassHitChaceMenu.AddList("HitChance.Harass.W", "W HitChance", new StringList(hitChances.ToArray(), 3));
            HitChanceMenu.AddSubMenu(HarassHitChaceMenu);
            
            Config.AddSubMenu(HitChanceMenu);



            DLib.AddToMenu(Config, new List<string>() { "Q" });
            DLib.SetText("After Q");

            DM.AddToMenu(Config);

            Menu Combo = new Menu("Combo", "Combo");
            Combo.AddBool("Combo.UseQ", "Use Q");
            Combo.AddBool("Combo.UseW", "Use W");
            Config.AddSubMenu(Combo);

            Menu Harass = new Menu("Harass", "Harass");
            Harass.AddBool("Harass.UseQ", "Use Q");
            Harass.AddBool("Harass.UseW", "Use W");
            Config.AddSubMenu(Harass);

            Menu Clear = new Menu("Clear", "Clear");

            Menu LineClear = new Menu("LineClear", "LineClear");
            LineClear.AddBool("Clear.LineClear.UseQ", "Use Q");
            LineClear.AddSlice("Clear.LineClear.UseQ2", "Use Q if my mana >= (%)", 80, 0, 100);
            Clear.AddSubMenu(LineClear);

            Menu JungleClear = new Menu("JungleClear", "JungleClear");
            JungleClear.AddBool("Clear.JungleClear.UseQ", "Use Q");
            JungleClear.AddSlice("Clear.JungleClear.UseQ2", "Use Q if my mana >= (%)", 80, 0, 100);
            Clear.AddSubMenu(JungleClear);
            Config.AddSubMenu(Clear);

            Menu LastHit = new Menu("LastHit", "LastHit");
            LastHit.AddBool("LastHit.UseQ", "Use Q");
            LastHit.AddSlice("LastHit.UseQ2", "Use Q if my mana >= (%)", 80, 0, 100);
            //LastHit.AddBool("LastHit.UseClear", "Use Lasthit in clear");
            Config.AddSubMenu(LastHit);

            Menu.AddSubMenu(Config);

            Game.OnUpdate += Game_OnUpdate;
        }

        void Game_OnUpdate(EventArgs args)
        {
            //throw new NotImplementedException();
            if (Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                OnCombo();
            }
            else if (Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                OnHarass();
            }
            else if (Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                if(!Orbwalking.CanAttack())
                {
                    OnLastHit();
                }
            }
            else if (Program.MainOrbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear();
                LineClear();
            }
        }

        static void OnCombo()
        {
            AIHeroClient target = STS.GetTarget(Q.Range);
            if(target != null && ! target.IsDead)
            {
                if(Menu.GetBool("Combo.UseQ"))
                {
                    CastQ(target, CastMode.Combo);
                }
                if(Menu.GetBool("Combo.UseW"))
                {
                    CastW(target, CastMode.Combo);
                }
            }
        }

        static void OnHarass()
        {
            AIHeroClient target = STS.GetTarget(Q.Range);
            if (target != null && !target.IsDead)
            {
                if(Menu.GetBool("Harass.UseQ"))
                {
                    CastQ(target, CastMode.Harass);
                }
                if(Menu.GetBool("Harass.USeW"))
                {
                    CastW(target, CastMode.Harass);
                }
            }
        }

        static void LineClear()
        {
            if (ObjectManager.Player.IsManaLow(Menu.GetSlice("Clear.LineClear.UseQ2")))
            {
                return;
            }
            Obj_AI_Base minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).OrderBy(x => x.Distance(ObjectManager.Player.Position)).FirstOrDefault(x => x.Distance(ObjectManager.Player.Position) <= Q.Range);
            if(minion != null && !minion.IsDead)
            {
                if (Menu.GetBool("Clear.LineClear.UseQ"))
                {
                    CastQ(minion, CastMode.Farm);
                }
            }
        }

        static void JungleClear()
        {
            if (ObjectManager.Player.IsManaLow(Menu.GetSlice("Clear.JungleClear.UseQ2")))
            {
                return;
            }
            Obj_AI_Base minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).OrderBy(x => x.Distance(ObjectManager.Player.Position)).FirstOrDefault(x => x.Distance(ObjectManager.Player.Position) <= Q.Range);
            if (minion != null && !minion.IsDead)
            {
                if (Menu.GetBool("Clear.JungleClear.UseQ"))
                {
                    CastQ(minion, CastMode.Farm);
                }
            }
        }

        static void OnLastHit()
        {
            if (ObjectManager.Player.IsManaLow(Menu.GetSlice("LastHit.UseQ2")))
            {
                return;
            }
            Obj_AI_Base minion = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).OrderBy(x => x.Distance(ObjectManager.Player.Position)).FirstOrDefault(x => ObjectManager.Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health);
            if(minion != null && !minion.IsDead)
            {
                if(Menu.GetBool("LastHit.UseQ"))
                {
                    CastQ(minion, CastMode.Farm);
                }
            }
        }

        static HitChance GetHitChance(string input)
        {
            foreach(HitChance value in Enum.GetValues(typeof(HitChance)))
            {
                if(value.ToString() == input)
                {
                    return value;
                }
            }
            return HitChance.VeryHigh;
        }

        static void CastQ(Obj_AI_Base target, CastMode mode)
        {
            if (target.IsDead || mode != CastMode.Killsteal && !Orbwalking.CanMove(0.5f))
            {
                return;
            }
            
            switch(mode)
            {
                case CastMode.Combo:
                    Q.CastIfHigherThen(target, GetHitChance(Menu.GetList("HitChance.Combo.Q")));
                    break;
                case CastMode.Harass:
                    Q.CastIfHigherThen(target, GetHitChance(Menu.GetList("HitChance.Harass.Q")));
                    break;
                case CastMode.Farm:
                    PredictionOutput value = Q.GetPrediction(target);
                    if(value.Hitchance != HitChance.Collision && value.Hitchance != HitChance.OutOfRange && value.CastPosition != Vector3.Zero )
                    {
                        Q.Cast(value.CastPosition);
                    }
                    break;
            }
        }

        static void CastW(Obj_AI_Base target, CastMode mode)
        {
            if(target.IsDead || mode != CastMode.Killsteal && !Orbwalking.CanMove(0.5f) )
            {
                return;
            }

            switch (mode)
            {
                case CastMode.Combo:
                    W.CastIfHigherThen(target, GetHitChance(Menu.GetList("HitChance.Combo.W")));
                    break;
                case CastMode.Harass:
                    W.CastIfHigherThen(target, GetHitChance(Menu.GetList("HitChance.Harass.W")));
                    break;
                case CastMode.Farm:
                    PredictionOutput value = W.GetPrediction(target);
                    if(value.Hitchance != HitChance.Collision && value.Hitchance != HitChance.OutOfRange && value.CastPosition != Vector3.Zero )
                    {
                        W.Cast(value.CastPosition);
                    }
                    break;
            }
        }
    }
}
