using System;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SPrediction;
using SharpDX;
using Utility = LeagueSharp.Common.Utility;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HoolaRengar
{
    public class Program
    {
        private static Menu Menu;
        private static Orbwalking.Orbwalker Orbwalker;
        private static AIHeroClient Player = ObjectManager.Player;
        
        private static Spell Q, W, E, R;
        public static AttackableUnit DashTarget;
        public static int dashcount, dashtime;
        public static bool dashwait;
        private static int AutoHP { get { return Menu.Item("AutoHP").GetValue<Slider>().Value; } }
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Rengar") return;
            Chat.Print("Hoola Rengar - Loaded Successfully, Good Luck! :)");
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 300);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R);
            E.SetSkillshot(0.20f, 70, 1500, true, SkillshotType.SkillshotLine);



            OnMenuLoad();

            Spellbook.OnCastSpell += OnCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Game.OnUpdate += Ontick;
        }

        private static void Ontick(EventArgs args)
        {
            AutoHeal();
            Killsteal();
            if (dashwait)
            {
                if (!Player.IsDashing() && Orbwalking.InAutoAttackRange(DashTarget))
                {
                    if (DashTarget is AIHeroClient)
                    {
                        UseCastItem(300);
                        Q.Cast();
                        dashwait = false;  
                    }
                }
                if (Utils.GameTimeTickCount - dashcount >= dashtime + 50)
                {
                    dashwait = false;
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) JungleClear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) LaneClear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();
        }

        private static void AutoHeal()
        {
            if (Player.Mana == 5 && Player.HealthPercent <= AutoHP && W.IsReady())
            {
                W.Cast();
            }
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (Player.Mana == 4 && Player.HealthPercent <= AutoHP && W.IsReady() && Player.Distance(target.ServerPosition) <= W.Range)
            {
                W.Cast();
            }
        }

        private static void Killsteal()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target.Health <= Q.GetDamage2(target) && Orbwalker.InAutoAttackRange(target) && Q.IsReady()) Q.Cast();
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (E.IsReady() && Player.Distance(target) <= 500 && Player.IsDashing() && Player.Mana == 5) E.SPredictionCast(target, HitChance.High);
            if (E.IsReady() && Player.Distance(target) <= E.Range - 50 && Player.Mana != 5 &&
                !Player.HasBuff2("RengarR") && !Player.HasBuff2("rengarpassivebuff")) E.SPredictionCast(target, HitChance.Medium);
            if (W.IsReady() && Player.Distance(target) <= 200 && Player.Mana != 5 && !Player.HasBuff2("RengarR")) W.Cast();
            if (W.IsReady() && Player.Distance(target) <= W.Range && !Player.IsDashing() && Player.Mana != 5 && !Player.HasBuff2("RengarR") && !Player.HasBuff2("rengarpassivebuff")) W.Cast();
            if (!Q.IsReady() && E.IsReady() && Orbwalker.InAutoAttackRange(target) && !Player.HasBuff2("RengarR") &&
                Player.Mana == 5) E.SPredictionCast(target, HitChance.High);
        }
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (E.IsReady() && Player.Distance(target) <= 500 && Player.IsDashing() && Player.Mana == 5) E.SPredictionCast(target, HitChance.High);
            if (E.IsReady() && Player.Distance(target) <= E.Range - 50 && Player.Mana != 5 &&
                !Player.HasBuff2("RengarR") && !Player.HasBuff2("rengarpassivebuff"))
                E.SPredictionCast(target, HitChance.Medium);
            if (W.IsReady() && Player.Distance(target) <= 200 && Player.Mana != 5 && !Player.HasBuff2("RengarR")) W.Cast();
            if (W.IsReady() && Player.Distance(target) <= W.Range && !Player.IsDashing() && Player.Mana != 5 && !Player.HasBuff2("RengarR") && !Player.HasBuff2("rengarpassivebuff")) W.Cast();
            if (!Q.IsReady() && E.IsReady() && Orbwalker.InAutoAttackRange(target) && !Player.HasBuff2("RengarR") &&
                Player.Mana == 5)
                E.SPredictionCast(target, HitChance.High);
        }

        private static void LaneClear()
        {
            var Minions = MinionManager.GetMinions(E.Range);
            if (Minions[0].IsValidTarget() && Minions.Count > 0 && !Minions[0].IsDead)
            {
                if (Player.Distance(Minions[0].ServerPosition) <= W.Range && Player.Mana != 5) W.Cast();
                if (Player.Distance(Minions[0].ServerPosition) <= E.Range && Player.Mana != 5 && !Player.Spellbook.IsAutoAttacking) E.Cast(Minions[0].ServerPosition);
            }
        }
        private static void JungleClear()
        {
            var Mobs = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (Mobs[0].IsValidTarget() && Mobs.Count > 0 && !Mobs[0].IsDead)
            {
                if (Player.Distance(Mobs[0].ServerPosition) <= W.Range && Player.Mana != 5) W.Cast();
                if (Player.Distance(Mobs[0].ServerPosition) <= E.Range && Player.Mana != 5 && !Player.Spellbook.IsAutoAttacking) E.Cast(Mobs[0].ServerPosition);
            }
        }
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsMe)
                return;
            Orbwalking.LastAATick = Utils.GameTimeTickCount - (int)Player.AttackCastDelay * 1000;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) CastYoumoo();
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.Target.IsValid || !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            var target = args.Target;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (target is AIHeroClient)
                {
                    UseCastItem(300);
                    CastBOTRK((AIHeroClient)target);
                    if (Q.IsReady()) Q.Cast();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (target is AIHeroClient)
                {
                    UseCastItem(300);
                    if (Q.IsReady()) Q.Cast();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (target is Obj_AI_Minion)
                {
                    UseCastItem(300);
                    if (Q.IsReady() && Player.Mana != 5) Q.Cast();
                }
            }
        }

        static void UseCastItem(int t)
        {
            for (int i = 0; i < t; i = i + 1)
            {
                if (HasItem())
                    LeagueSharp.Common.Utility.DelayAction.Add(i, () => CastItem());
            }
        }
        private static void OnCast(Spellbook sender, SpellbookCastSpellEventArgs args){if (args.Slot == SpellSlot.Q) Orbwalking.LastAATick = 0;}

        static void CastItem()
        {

            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady())
                ItemData.Tiamat_Melee_Only.GetItem().Cast();
            if (ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
                ItemData.Ravenous_Hydra_Melee_Only.GetItem().Cast();
        }
        static void CastYoumoo()
        {
            if (ItemData.Youmuus_Ghostblade.GetItem().IsReady())
                ItemData.Youmuus_Ghostblade.GetItem().Cast();
        }
        static void CastBOTRK(AIHeroClient target)
        {
            if (ItemData.Blade_of_the_Ruined_King.GetItem().IsReady())
                ItemData.Blade_of_the_Ruined_King.GetItem().Cast(target);
            if (ItemData.Bilgewater_Cutlass.GetItem().IsReady())
                ItemData.Bilgewater_Cutlass.GetItem().Cast(target);
        }
        static bool HasItem()
        {
            if (ItemData.Tiamat_Melee_Only.GetItem().IsReady() || ItemData.Ravenous_Hydra_Melee_Only.GetItem().IsReady())
            {
                return true;
            }
            return false;
        }
        
        
        private static void OnMenuLoad()
        {
            Menu = new Menu("Hoola Rengar", "hoolaRengar", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);


            var Combo = new Menu("Combo", "Combo");
            Combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(false));
            Menu.AddSubMenu(Combo);

            var Auto = new Menu("Auto", "Auto");
            Auto.AddItem(new MenuItem("AutoHP", "Auto Heal x%").SetValue(new Slider(50)));
            Menu.AddSubMenu(Auto);

            Menu.AddToMainMenu();

            SPrediction.Prediction.Initialize(Menu);
        }
    }
}