using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HoolaWukong
{
    public class Program
    {
        public static Menu Menu;
        private static void CastYoumoo() { if (ItemData.Youmuus_Ghostblade.GetItem().IsReady()) ItemData.Youmuus_Ghostblade.GetItem().Cast(); }
        private static int Item => Items.CanUseItem(3077) && Items.HasItem(3077) ? 3077 : Items.CanUseItem(3074) && Items.HasItem(3074) ? 3074 : 0;
        private static void Drawing_OnEndScene(EventArgs args)
        {
        }
        private static bool HasTitan() => (Items.HasItem(3748) && Items.CanUseItem(3748));

        private static void CastTitan()
        {
            if (Items.HasItem(3748) && Items.CanUseItem(3748))
            {
                Items.UseItem(3748);
            }
        }
        private static Orbwalking.Orbwalker Orbwalker;
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static Spell Q, E, R;
        private static bool LQ => Menu.Item("LQ").GetValue<bool>();
        private static bool LE => Menu.Item("LE").GetValue<bool>();
        private static bool LI => Menu.Item("LI").GetValue<bool>();
        private static bool LT => Menu.Item("LT").GetValue<bool>();
        private static bool JQ => Menu.Item("JQ").GetValue<bool>();
        private static bool JE => Menu.Item("JE").GetValue<bool>();
        private static bool JI => Menu.Item("JI").GetValue<bool>();
        private static bool HQ => Menu.Item("HQ").GetValue<bool>();
        private static bool HE => Menu.Item("HE").GetValue<bool>();
        private static bool HI => Menu.Item("HI").GetValue<bool>();
        private static bool Dind => Menu.Item("Dind").GetValue<bool>();
        private static bool DrawE => Menu.Item("DrawE").GetValue<bool>();
        private static bool DrawAlwaysR => Menu.Item("DrawAlwaysR").GetValue<bool>();
        private static int UseRMin => Menu.Item("UseRMin").GetValue<Slider>().Value;
        private static bool AlwaysR => Menu.Item("AlwaysR").GetValue<KeyBind>().Active;
        public static void Main() => OnGameLoad();

        private static void OnGameLoad()
        {

            if (Player.ChampionName != "MonkeyKing") return;
            Chat.Print("Hoola Wukong - Loaded Successfully, Good Luck! :):)");
            Q = new Spell(SpellSlot.Q);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R);
            E.SetTargetted(0.5f,2000f);

            OnMenuLoad();

            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnSpellCast += DoCast;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
            Spellbook.OnCastSpell += OnSpell;
        }

        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && !E.IsReady() && !Q.IsReady() && R.IsReady() &&
                Player.CountEnemiesInRange(165 + Player.BoundingRadius) >= 1 && AlwaysR &&
                !Player.HasBuff("MonkeyKingSpinToWin") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && args.SData.Name.Contains("MonkeyKingQAttack"))
                R.Cast();
            if (!sender.IsMe) return;

            if (args.SData.Name == "ItemTitanicHydraCleave") Orbwalking.LastAATick = 0;
        }

        private static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe) return;
            
            if (args.Slot == SpellSlot.Q)
            {
                Orbwalking.LastAATick = 0;
            }
            if (args.Slot == SpellSlot.E)
            {
                CastYoumoo();
            }
        }

        private static void OnMenuLoad()
        {
            Menu = new Menu("Hoola Wukong", "hoolawukong", true);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);
            var orbwalker = new Menu("Orbwalk", "rorb");
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            Menu.AddSubMenu(orbwalker);
            var Combo = new Menu("Combo", "Combo");

            Combo.AddItem(new MenuItem("UseRMin", "Use R X Enemies (0 = Don't)").SetValue(new Slider(0, 0, 5)));
            Combo.AddItem(new MenuItem("AlwaysR", "Force Use R (Toggle)").SetValue(new KeyBind('G', KeyBindType.Toggle)));

            Menu.AddSubMenu(Combo);

            var Harass = new Menu("Harass", "Harass");

            Harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("HE", "Use E").SetValue(true));
            Harass.AddItem(new MenuItem("HI", "Use Hydra").SetValue(true));

            Menu.AddSubMenu(Harass);

            var Jungle = new Menu("JungleClear", "JungleClear");

            Jungle.AddItem(new MenuItem("JQ", "Use Q").SetValue(true));
            Jungle.AddItem(new MenuItem("JE", "Use E").SetValue(true));
            Jungle.AddItem(new MenuItem("JI", "Use Hydra").SetValue(true));

            Menu.AddSubMenu(Jungle);

            var Lane = new Menu("LaneClear", "LaneClear");

            Lane.AddItem(new MenuItem("LQ", "Use Q").SetValue(false));
            Lane.AddItem(new MenuItem("LE", "Use E").SetValue(false));
            Lane.AddItem(new MenuItem("LI", "Use Hydra").SetValue(false));
            Lane.AddItem(new MenuItem("LT", "Use Titan / Q Tower").SetValue(true));

            Menu.AddSubMenu(Lane);

            var Draw = new Menu("Draw", "Draw");

            Draw.AddItem(new MenuItem("Dind", "Draw Damage Indicator").SetValue(true));
            Draw.AddItem(new MenuItem("DrawAlwaysR", "Draw Always R Status").SetValue(true));
            Draw.AddItem(new MenuItem("DrawE", "Draw E Range").SetValue(false));

            Menu.AddSubMenu(Draw);

            Menu.AddToMainMenu();
        }

        private static void OnDraw(EventArgs args)
        {
            var heropos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            if (DrawE) Drawing.DrawCircle(Player.Position, E.Range, E.IsReady() ? Color.LimeGreen : Color.Indigo);
            if (DrawAlwaysR)
            {
                Drawing.DrawText(heropos.X - 40, heropos.Y + 20, System.Drawing.Color.DodgerBlue, "Always R  (     )");
                Drawing.DrawText(heropos.X + 40, heropos.Y + 20, AlwaysR ? System.Drawing.Color.LimeGreen : System.Drawing.Color.Red, AlwaysR ? "On" : "Off");
            }
        }

        private static void DoCastJC(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !args.SData.IsAutoAttack()) return;
            
            if (args.Target is Obj_AI_Minion)
            {
                var target = MinionManager.GetMinions(300 + Player.BoundingRadius, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
                if (!target[0].IsValidTarget(300 + Player.BoundingRadius) || target == null || target.Count <= 0) return;

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (HasTitan() && JI) CastTitan();
                    else if (!HasTitan() || (HasTitan() && !JI))
                    {
                        if (E.IsReady() && JE) E.Cast(target[0]);
                        if (Q.IsReady() && JQ) Q.Cast();
                        if (Items.CanUseItem(Item) && Items.HasItem(Item) && JI) Items.UseItem(Item);
                    }
                }
            }
        }

        private static void DoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            DoCastJC(sender,args);
            if (!sender.IsMe || !args.SData.IsAutoAttack()) return;

            if (args.Target is Obj_AI_Base)
            {
                var target = (Obj_AI_Base) args.Target;
                if (!target.IsValidTarget(300 + Player.BoundingRadius) || target == null) return;

                if (target is AIHeroClient && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    if (HasTitan()) CastTitan();
                    else if (!HasTitan())
                    {
                        if (E.IsReady()) E.Cast(target);
                        if (Q.IsReady()) Q.Cast();
                        if (Items.CanUseItem(Item) && Items.HasItem(Item)) Items.UseItem(Item);
                    }
                }
                if (target is AIHeroClient && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    if (HasTitan()) CastTitan();
                    else if (!HasTitan())
                    {
                        if (E.IsReady() && HE) E.Cast(target);
                        if (Q.IsReady() && HQ) Q.Cast();
                        if (Items.CanUseItem(Item) && Items.HasItem(Item) && HI) Items.UseItem(Item);
                    }
                }
            }
            if (args.Target is Obj_AI_Minion)
            {
                var target = (Obj_AI_Minion)args.Target;
                if (!target.IsValidTarget(300 + Player.BoundingRadius) || target == null) return;

                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (HasTitan() && LI) CastTitan();
                    else if (!HasTitan() || (HasTitan() && !LI))
                    {
                        if (E.IsReady() && LE) E.Cast(target);
                        if (Q.IsReady() && LQ) Q.Cast();
                        if (Items.CanUseItem(Item) && Items.HasItem(Item) && LI) Items.UseItem(Item);
                    }
                }
            }
            if (args.Target is Obj_AI_Turret || args.Target is Obj_BarracksDampener || args.Target is Obj_Barracks ||
                args.Target is Obj_Building && (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && LT))
            {
                if (HasTitan()) CastTitan();
                else if (!HasTitan()) if (Q.IsReady())Q.Cast();
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (Player.CountEnemiesInRange(165 + Player.BoundingRadius) >= UseRMin && UseRMin != 0 && !Player.HasBuff("MonkeyKingSpinToWin") &&
                R.IsReady()) R.Cast();
            if (!E.IsReady() && !Q.IsReady() && !Player.HasBuff("MonkeyKingDoubleAttack") && R.IsReady() && Player.CountEnemiesInRange(165 + Player.BoundingRadius) >= 1 && AlwaysR &&
                !Player.HasBuff("MonkeyKingSpinToWin") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) R.Cast(); 

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) Clear();
        }

        private static void Clear()
        {
            LaneClear();
            JungleClear();
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (E.IsReady() && target.IsValidTarget(E.Range) && !Orbwalker.InAutoAttackRange(target)) E.Cast(target);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (E.IsReady() && target.IsValidTarget(E.Range) && !Orbwalker.InAutoAttackRange(target)) E.Cast(target);
        }

        private static void JungleClear()
        {
            var target = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (E.IsReady() && target[0].IsValidTarget(E.Range) && !Orbwalker.InAutoAttackRange(target[0])) E.Cast(target[0]);
        }
        private static void LaneClear()
        {
            var target = MinionManager.GetMinions(E.Range);
            if (E.IsReady() && target[0].IsValidTarget(E.Range) && !Orbwalker.InAutoAttackRange(target[0])) E.Cast(target[0]);
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (Q.IsReady()) damage += Q.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy, true);
                if (E.IsReady()) damage += E.GetDamage(enemy);
                if (Items.HasItem(Item) && Items.CanUseItem(Item))
                    damage += (float)Player.GetAutoAttackDamage(enemy)*0.7f;

                return damage;
            }
            return 0;
        }
    }
}