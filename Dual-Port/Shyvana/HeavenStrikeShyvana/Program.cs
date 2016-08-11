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
 namespace HeavenStrikeShyvana
{
    public static class Program
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }


        public static Orbwalking.Orbwalker Orbwalker;


        public static Spell Q, W, E, R;

        public static SpellSlot Smite;

        public static Menu Menu;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Shyvana")
                return;
            // spells
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E,950);
            E.SetSkillshot(0.25f, 60, 1700, false, SkillshotType.SkillshotLine);
            R = new Spell(SpellSlot.R,1000);
            R.SetSkillshot(0.25f, 150, 1500, false, SkillshotType.SkillshotLine);
            // smite
            foreach (var spell in
                Player.Spellbook.Spells.Where(
                    sSpell =>
                    sSpell.Name.ToLower().Contains("smite")
                    && (sSpell.Slot == SpellSlot.Summoner1 || sSpell.Slot == SpellSlot.Summoner2)))
            {
                Smite = spell.Slot;
            }

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

            Menu SmiteMenu = Menu.AddSubMenu(new Menu("Smite", "Smite"));

            Menu Draw = Menu.AddSubMenu(new Menu("Draw", "Draw")); ;

            Harass.AddItem(new MenuItem("harass w", "W").SetValue(true));
            Harass.AddItem(new MenuItem("harass e", "E").SetValue(true));

            Combo.AddItem(new MenuItem("combo e", "E").SetValue(true));
            Combo.AddItem(new MenuItem("combo w", "W").SetValue(true));
            Combo.AddItem(new MenuItem("combo r", "R").SetValue(true));
            Combo.AddItem(new MenuItem("combo youmuu", "Youmuu").SetValue(true));
            Combo.AddItem(new MenuItem("combo botrk", "Bilgewater/Botrk").SetValue(true));

            LaneClear.AddItem(new MenuItem("laneclear q", "Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear w", "W").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear e", "E").SetValue(true));
            LaneClear.AddItem(new MenuItem("laneclear tiamat", "Tiamat/Ravenous Hydra").SetValue(true));

            JungClear.AddItem(new MenuItem("jungleclear q", "Q").SetValue(true));
            JungClear.AddItem(new MenuItem("jungleclear w", "W").SetValue(true));
            JungClear.AddItem(new MenuItem("jungleclear e", "E").SetValue(true));
            JungClear.AddItem(new MenuItem("jungleclear tiamat", "Tiamat/Ravenous Hydra").SetValue(true));

            Ks.AddItem(new MenuItem("ks e", "E").SetValue(true));
            Ks.AddItem(new MenuItem("ks r", "R").SetValue(true));
            Ks.AddItem(new MenuItem("ks tiamat", "Tiamat-Ravenous Hydra").SetValue(true));
            Ks.AddItem(new MenuItem("ks botrk", "Botrk").SetValue(true));
            Ks.AddItem(new MenuItem("ks cutlass", "Bilgewater Cutlass").SetValue(true));

            SmiteMenu.AddItem(new MenuItem("smite ks", "Use Smite ks").SetValue(true));
            SmiteMenu.AddItem(new MenuItem("smite auto", "Auto Smite Dragon/Baron").SetValue(true));
            SmiteMenu.AddItem(new MenuItem("smite combo", "Use Smite Combo").SetValue(true));

            Draw.AddItem(new MenuItem("draw e","E").SetValue(true));
            Draw.AddItem(new MenuItem("draw r","R").SetValue(true));

            Menu.AddToMainMenu();


            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            //GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.OnAttack += OnAttack;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;


            Chat.Print("Welcome to HeavenStrikeShyvana");
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
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo.UpdateCombo();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harass.UpdateHarass();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                laneclear.UpdateLaneClear();
                jungleclear.UpdateJungleClear();
            }
            ks.UpdateKs();
            smite.UpdateSmite();
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target is Obj_Building || args.Target is Obj_HQ
                || args.Target is Obj_Barracks || args.Target is Obj_BarracksDampener)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo.BeforeAttackCombo(args);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harass.BeforeAttackHarass(args);
            }
        }
        private static void OnAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe || target is Obj_Building || target is Obj_HQ
                || target is Obj_Barracks || target is Obj_BarracksDampener)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo.OnAttackCombo(unit, target);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                laneclear.OnAttackLaneClear(unit, target);
                jungleclear.OnAttackJungleClear(unit, target);
            }
        }
        public static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target is Obj_Building || target is Obj_HQ
                || target is Obj_Barracks || target is Obj_BarracksDampener)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                combo.AfterAttackCombo(unit,target);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                harass.AfterAttackHarass(unit,target);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                laneclear.AfterAttackLaneClear(unit, target);
                jungleclear.AfterAttackJungleClear(unit, target);
            }

        }

        //menu variables
        public static bool WHarass => Menu.Item("harass w").GetValue<bool>();
        public static bool EHarass => Menu.Item("harass e").GetValue<bool>();
        public static bool WCombo => Menu.Item("combo w").GetValue<bool>();
        public static bool ECombo => Menu.Item("combo e").GetValue<bool>();
        public static bool RCombo => Menu.Item("combo r").GetValue<bool>();
        public static bool QLaneClear => Menu.Item("laneclear q").GetValue<bool>();
        public static bool WLaneClear => Menu.Item("laneclear w").GetValue<bool>();
        public static bool ELaneClear => Menu.Item("laneclear e").GetValue<bool>();
        public static bool QJungleClear => Menu.Item("jungleclear q").GetValue<bool>();
        public static bool WJungleClear => Menu.Item("jungleclear w").GetValue<bool>();
        public static bool EJungleClear => Menu.Item("jungleclear e").GetValue<bool>();
        public static bool EKs => Menu.Item("ks e").GetValue<bool>();
        public static bool RKs => Menu.Item("ks r").GetValue<bool>();
        public static bool TiamatKs => Menu.Item("ks tiamat").GetValue<bool>();
        public static bool BotrkKs => Menu.Item("ks botrk").GetValue<bool>();
        public static bool CutlassKs => Menu.Item("ks cutlass").GetValue<bool>();
        public static bool KsSmite => Menu.Item("smite ks").GetValue<bool>();
        public static bool AutoSmite => Menu.Item("smite auto").GetValue<bool>();
        public static bool ComboSmite => Menu.Item("smite combo").GetValue<bool>();
        public static bool DrawE => Menu.Item("draw e").GetValue<bool>();
        public static bool DrawR => Menu.Item("draw r").GetValue<bool>();
        public static bool YoumuuCombo => Menu.Item("combo youmuu").GetValue<bool>();
        public static bool BotrkCombo => Menu.Item("combo botrk").GetValue<bool>();
        public static bool TiamatLaneClear => Menu.Item("laneclear tiamat").GetValue<bool>();
        public static bool TiamatJungleClear => Menu.Item("jungleclear tiamat").GetValue<bool>();
    }
}
