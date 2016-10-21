using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HeavenStrikeSivir
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, R;

        private static Menu Menu;


        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Sivir")
                return;

            Q = new Spell(SpellSlot.Q, 1250);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 90, 1350, false, SkillshotType.SkillshotLine);
            Q.MinHitChance = HitChance.Medium;


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("Qcombo", "use Q").SetValue(true));
            Combo.AddItem(new MenuItem("Rcombo", "use R 2 target around").SetValue(true));
            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));
            Harass.AddItem(new MenuItem("Qharass", "use Q").SetValue(true));
            Harass.AddItem(new MenuItem("manaharass", "if mana >").SetValue(new Slider(40, 0, 100)));
            Menu Clear = spellMenu.AddSubMenu(new Menu("Clear", "Clear"));
            Clear.AddItem(new MenuItem("Qclear", "use Q").SetValue(true));
            Clear.AddItem(new MenuItem("Wclear", "use W").SetValue(true));
            Clear.AddItem(new MenuItem("manaclear", "if mana >").SetValue(new Slider(40, 0, 100)));
            Menu auto = spellMenu.AddSubMenu(new Menu("Misc", "Misc"));
            auto.AddItem(new MenuItem("Qautoks", "Ks with Q").SetValue(true));
            auto.AddItem(new MenuItem("Eautotargeted", "E against targeted spells").SetValue(true));
            Menu Drawing = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Drawing.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            Menu.AddToMainMenu();

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            //Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnProcessSpellCast += oncast;
            ////CustomEvents.Unit.OnDash += Unit_OnDash;
            //Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            //EloBuddy.Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
        }
        private static bool Qcombo { get { return Menu.Item("Qcombo").GetValue<bool>(); } }
        private static bool Rcombo { get { return Menu.Item("Rcombo").GetValue<bool>(); } }
        private static bool Qharasss { get { return Menu.Item("Qharass").GetValue<bool>(); } }
        private static bool Qclear { get { return Menu.Item("Qclear").GetValue<bool>(); } }
        private static bool Wclear { get { return Menu.Item("Wclear").GetValue<bool>(); } }
        private static bool Qautoks { get { return Menu.Item("Qautoks").GetValue<bool>(); } }
        private static bool Eautotargeted { get { return Menu.Item("Eautotargeted").GetValue<bool>(); } }
        private static bool drawQ { get { return Menu.Item("drawQ").GetValue<bool>(); } }
        private static int manaharass { get { return Menu.Item("manaharass").GetValue<Slider>().Value; } }
        private static int manaclear { get { return Menu.Item("manaclear").GetValue<Slider>().Value; } }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (drawQ)
                Render.Circle.DrawCircle(Player.Position,Q.Range,Color.Green);
        }

        private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Eautotargeted && E.IsReady() && sender.IsValid<AIHeroClient>() && sender.IsEnemy && args.Target.IsMe 
                && (args.SData.TargettingType == SpellDataTargetType.Unit
                || args.SData.TargettingType == SpellDataTargetType.SelfAndUnit) && !args.SData.IsAutoAttack())
            {
                E.Cast();
            }
            if (!sender.IsMe) return;
            if (args.SData.Name == "SivirW")
            {
                Orbwalking.ResetAutoAttackTimer();
            }
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Q.IsReady() && !W.IsReady() && Qcombo)
            {
                Q.Cast(target as Obj_AI_Base);
            }
            if (target is AIHeroClient && W.IsReady())
            {
                W.Cast();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && W.IsReady() && Wclear && Player.Mana*100/Player.MaxMana >= manaclear)
            {
                W.Cast();
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            Auto();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Clear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Harass();
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Q.IsReady() && Qharasss && Player.Mana*100/Player.MaxMana >= manaharass)
            {
                Q.Cast(target);
            }
        }

        private static void Clear()
        {
            var farmlocation = Q.GetLineFarmLocation(MinionManager.GetMinions(Q.Range,MinionTypes.All));
            if (farmlocation.MinionsHit >=3 && Qclear && Q.IsReady() && Player.Mana*100/Player.MaxMana >= manaclear)
            {
                Q.Cast(farmlocation.Position);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Q.IsReady() && Qcombo)
            {
                Q.CastIfWillHit(target,2);
            }
            if (R.IsReady() && Rcombo && Player.CountEnemiesInRange(1000) >= 2)
            {
                R.Cast();
            }
        }

        private static void Auto()
        {
            foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsZombie))
            {
                if (Q.IsReady() && Qautoks && Q.GetDamage(target)*1.85 >= target.Health)
                {
                    Q.Cast(target);
                }
            }
        }
    }
}
