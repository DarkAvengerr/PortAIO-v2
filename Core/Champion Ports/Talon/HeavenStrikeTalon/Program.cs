using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeTalon
{
    using static Gettarget;
    using static Extension;
    // TalonRStealth , TalonRHaste;
    public static class Program
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }


        public static Orbwalking.Orbwalker Orbwalker;


        public static Spell Q, W, E, R;

        public static Menu Menu;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        private static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Talon")
                return;
            // spells
            Q = new Spell(SpellSlot.Q, 550);
            W = new Spell(SpellSlot.W, 750);
            W.SetSkillshot(0.25f, 60, 1850, false, SkillshotType.SkillshotCone);
            W.MinHitChance = HitChance.Low;
            E = new Spell(SpellSlot.E, 700);
            R = new Spell(SpellSlot.R, 500);


            Menu = new Menu("HeavenStrike" + Player.ChampionName, Player.ChampionName, true);
            Menu.SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.Red);

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu Harass = Menu.AddSubMenu(new Menu("Harass", "Harass"));

            Menu Combo = Menu.AddSubMenu(new Menu("Combo", "Combo"));

            Menu JungClear = Menu.AddSubMenu(new Menu("Jungle Clear", "Jungle Clear"));

            Menu LaneClear = Menu.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));

            Menu Ks = Menu.AddSubMenu(new Menu("Ks", "Ks"));

            Menu Draw = Menu.AddSubMenu(new Menu("Draw", "Draw"));

            Menu SelectingMode = Menu.AddSubMenu(new Menu("Only Attack Selected Target","Selecting Mode"));

            Harass.AddItem(new MenuItem("harass w", "W").SetValue(true));

            Combo.AddItem(new MenuItem("combo q1", "Q dash").SetValue(true));
            Combo.AddItem(new MenuItem("combo w", "W").SetValue(true));
            Combo.AddItem(new MenuItem("combo r", "R").SetValue(true));

            LaneClear.AddItem(new MenuItem("laneclear q", "Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear w", "W").SetValue(false));
            LaneClear.AddItem(new MenuItem("laneclear tiamat", "Tiamat/Ravenous Hydra").SetValue(true));

            JungClear.AddItem(new MenuItem("jungleclear q", "Q").SetValue(true));
            JungClear.AddItem(new MenuItem("jungleclear w", "W").SetValue(false));
            JungClear.AddItem(new MenuItem("jungleclear tiamat", "Tiamat/Ravenous Hydra").SetValue(true));

            Ks.AddItem(new MenuItem("ks w", "W").SetValue(true));
            Ks.AddItem(new MenuItem("ks tiamat", "Tiamat-Ravenous Hydra").SetValue(true));
            Ks.AddItem(new MenuItem("ks botrk", "Botrk").SetValue(true));
            Ks.AddItem(new MenuItem("ks cutlass", "Bilgewater Cutlass").SetValue(true));

            Draw.AddItem(new MenuItem("draw w", "W").SetValue(false));
            Draw.AddItem(new MenuItem("draw r", "R").SetValue(true));
            Draw.AddItem(new MenuItem("draw e", "E").SetValue(true));

            SelectingMode.AddItem(new MenuItem("STenable", "Enable").SetValue(false));
            SelectingMode.AddItem(new MenuItem("STrange", "Range").SetValue(new Slider(900, 600, 2000)));

            Menu.AddToMainMenu();

            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;


            Chat.Print("Welcome to HeavenStrikeTalon");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            Drawing.OnDraw_Draw();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.SData.Name == "ItemTitanicHydraCleave")
            {
                Orbwalking.ResetAutoAttackTimer();
            }
            if (args.Slot == SpellSlot.Q)
            {
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Utils.GameTimeTickCount - WCount >= 500)
            {
                WCasted = false;
            }
            Ks.UpdateKs();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (Selected() && OutOfAA(TargetSelector.GetSelectedTarget()) && Orbwalking.CanMove(80))
                {
                    Orbwalker.SetAttack(false);
                }
                else
                {
                    Orbwalker.SetAttack(true);
                }
                Combo.UpdateCombo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass.UpdateHarass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear.UpdateJungleClear();
                LaneClear.UpdateLaneClear();
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_Building || args.Target is Obj_HQ
                || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.BeforeAttackCombo(args);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass.BeforeAttackHarass(args);
            }
        }
        public static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target is Obj_Building || target is Obj_HQ
                || target is Obj_Barracks || target is Obj_BarracksDampener)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo.AfterAttackCombo(unit,target);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Harass.AfterAttackHarass(unit,target);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                JungleClear.AfterAttackJungleClear(unit,target);
                LaneClear.AfterAttackLaneClear(unit, target);
            }
        }
        //global variables
        public static Vector2 LastUltPos = new Vector2();
        public static int WCount;
        public static bool WCasted = false;
        //menu variables
        public static bool WHarass => Menu.Item("harass w").GetValue<bool>();
        public static bool Q1Combo => Menu.Item("combo q1").GetValue<bool>();
        public static bool WCombo => Menu.Item("combo w").GetValue<bool>();
        public static bool RCombo => Menu.Item("combo r").GetValue<bool>();
        public static bool QLaneClear => Menu.Item("laneclear q").GetValue<bool>();
        public static bool WLaneClear => Menu.Item("laneclear w").GetValue<bool>();
        public static bool TiamatLaneClear => Menu.Item("laneclear tiamat").GetValue<bool>();
        public static bool QJungleClear => Menu.Item("jungleclear q").GetValue<bool>();
        public static bool WJungleClear => Menu.Item("jungleclear w").GetValue<bool>();
        public static bool TiamatJungleClear => Menu.Item("jungleclear tiamat").GetValue<bool>();
        public static bool WKs => Menu.Item("ks w").GetValue<bool>();
        public static bool TiamatKs => Menu.Item("ks tiamat").GetValue<bool>();
        public static bool BotrkKs => Menu.Item("ks botrk").GetValue<bool>();
        public static bool CutlassKs => Menu.Item("ks cutlass").GetValue<bool>();
        public static bool DrawE => Menu.Item("draw e").GetValue<bool>();
        public static bool DrawR => Menu.Item("draw r").GetValue<bool>();
        public static bool DrawW => Menu.Item("draw w").GetValue<bool>();
        //targeting mod variables
        public static bool GetTarget => Menu.Item("STenable").GetValue<bool>();
        public static int GetRange => Menu.Item("STrange").GetValue<Slider>().Value;
    }
}