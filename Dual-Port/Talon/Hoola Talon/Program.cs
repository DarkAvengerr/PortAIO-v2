using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;
using Menu = LeagueSharp.Common.Menu;
using MenuItem = LeagueSharp.Common.MenuItem;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace HoolaTalon
{
    public class Program
    {
        private static Menu Menu;
        private static Orbwalking.Orbwalker Orbwalker;
        private static AIHeroClient Player = ObjectManager.Player;
        private static HpBarIndicator Indicator = new HpBarIndicator();
        private static Spell Q, W, E, R;
        private static String IsFirstR = "TalonShadowAssault";
        private static String IsSecondR = "talonshadowassaulttoggle";
        static bool Dind { get { return Menu.Item("Dind").GetValue<bool>(); } }
        static bool DW { get { return Menu.Item("DW").GetValue<bool>(); } }
        static bool DE { get { return Menu.Item("DE").GetValue<bool>(); } }
        static bool DR { get { return Menu.Item("DR").GetValue<bool>(); } }
        static bool KSW { get { return Menu.Item("KSW").GetValue<bool>(); } }
        static bool KSEW { get { return Menu.Item("KSEW").GetValue<bool>(); } }
        static bool KSR { get { return Menu.Item("KSR").GetValue<bool>(); } }
        static bool CQ { get { return Menu.Item("CQ").GetValue<bool>(); } }
        static bool CW { get { return Menu.Item("CW").GetValue<bool>(); } }
        static bool CE { get { return Menu.Item("CE").GetValue<bool>(); } }
        static bool CR { get { return Menu.Item("CR").GetValue<bool>(); } }
        static int CRS { get { return Menu.Item("CRS").GetValue<StringList>().SelectedIndex; } }
        static bool CAR { get { return Menu.Item("CAR").GetValue<bool>(); } }
        static bool HQ { get { return Menu.Item("HQ").GetValue<bool>(); } }
        static bool HW { get { return Menu.Item("HW").GetValue<bool>(); } }
        static bool HE { get { return Menu.Item("HE").GetValue<bool>(); } }
        static bool LQ { get { return Menu.Item("LQ").GetValue<bool>(); } }
        static bool LW { get { return Menu.Item("LW").GetValue<bool>(); } }
        static int minhit { get { return Menu.Item("minhit").GetValue<Slider>().Value; } }

        public static void OnGameLoad()
        {
            if (Player.ChampionName != "Talon") return;
            Chat.Print("Hoola Talon - Loaded Successfully, Good Luck! :)");

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 720f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 600f) { Delay = 0.1f, Speed = 902f * 2 };

            W.SetSkillshot(0.25f, 52f * (float)Math.PI / 180, 902f * 2, false, SkillshotType.SkillshotCone);
            E.SetTargetted(0.25f, float.MaxValue);

            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Spellbook.OnCastSpell += OnCast;
            Drawing.OnEndScene += OnEndScene;
            Drawing.OnDraw += OnDraw;

            OnMenuLoad();
        }

        private static void OnDraw(EventArgs args)
        {
            if(DR)Render.Circle.DrawCircle(Player.Position, E.Range + W.Range, E.LSIsReady() && W.LSIsReady() ? Color.LimeGreen : Color.IndianRed);
            if(DW)Render.Circle.DrawCircle(Player.Position, W.Range, W.LSIsReady() ? Color.LimeGreen : Color.IndianRed);
            if(DE)Render.Circle.DrawCircle(Player.Position, E.Range, E.LSIsReady() ? Color.LimeGreen : Color.IndianRed);
        }

        private static void OnCast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.E)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && args.Target is AIHeroClient) CastYoumoo();
            }
            if (args.Slot == SpellSlot.Q) Orbwalking.LastAATick = 0;
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || !Orbwalking.IsAutoAttack(args.SData.Name)) return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (args.Target is AIHeroClient)
                {
                    var target = (AIHeroClient) args.Target;
                    if (!target.IsDead)
                    {
                        if (Q.LSIsReady() && CQ) Q.Cast();
                        UseCastItem(300);
                        if ((!Q.LSIsReady() || (Q.LSIsReady() && !CQ)) && CW) W.Cast(target.ServerPosition);
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (args.Target is AIHeroClient)
                {
                    var target = (AIHeroClient)args.Target;
                    if (!target.IsDead)
                    {
                        if (Q.LSIsReady() && CQ) Q.Cast();
                        UseCastItem(300);
                        if ((!Q.LSIsReady() || (Q.LSIsReady() && !CQ)) && CW) W.Cast(target.ServerPosition);
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (args.Target is AIHeroClient)
                {
                    var target = (AIHeroClient)args.Target;
                    if (!target.IsDead)
                    {
                        if (Q.LSIsReady() && HQ) Q.Cast();
                        UseCastItem(300);
                        if ((!Q.LSIsReady() || (Q.LSIsReady() && !HQ)) && HW) W.Cast(target.ServerPosition);
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (args.Target is Obj_AI_Minion)
                {
                    var target = (Obj_AI_Minion)args.Target;
                    if (!target.IsDead)
                    {
                        if (Q.LSIsReady() && LQ) Q.Cast();
                        UseCastItem(300);
                    }
                }
            }
        }

        private static void OnTick(EventArgs args)
        {
            Killsteal();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo) Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed) Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) LaneClear();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) JungleClear();
        }

        private static void Killsteal()
        {
            var targets = HeroManager.Enemies.Where(x => x.LSIsValidTarget() && Player.LSDistance(x.ServerPosition) <= E.Range + W.Range && !x.IsZombie && !x.IsDead);
            if (W.LSIsReady() && KSW)
            {
                foreach (var target in targets)
                {
                    if (target.Health <= W.GetDamage2(target) * 2)
                    {
                        if (Player.LSDistance(target.ServerPosition) < W.Range && Player.Mana >= W.ManaCost)
                        {
                            W.Cast(target.ServerPosition);
                        }
                        else if (E.LSIsReady() && Player.LSDistance(target.ServerPosition) > W.Range && Player.Mana >= E.ManaCost + W.ManaCost && KSEW)
                        {
                            var minions = MinionManager.GetMinions(E.Range);
                            if (minions.Count != 0)
                            {
                                foreach (var minion in minions)
                                {
                                    if (minion.LSIsValidTarget(E.Range) &&
                                        minion.LSDistance(target.ServerPosition) <= W.Range)
                                    {
                                        TacticalMap.ShowPing(PingCategory.Normal, minion.Position);
                                        E.Cast(minion);
                                        LeagueSharp.Common.Utility.DelayAction.Add(10, ()=> W.Cast(target.ServerPosition));
                                    }

                                }
                            }
                            if (minions.Count == 0)
                            {
                                var heros = HeroManager.Enemies;
                                if (heros.Count != 0)
                                {
                                    foreach (var hero in heros)
                                    {
                                        if (hero.LSIsValidTarget(E.Range) &&
                                            hero.LSDistance(target.ServerPosition) <= W.Range)
                                        {
                                            TacticalMap.ShowPing(PingCategory.Normal, hero.Position);
                                            E.Cast(hero);
                                            LeagueSharp.Common.Utility.DelayAction.Add(10, () => W.Cast(target.ServerPosition));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (CR && CRS == 0 && R.LSIsReady() && Player.LSDistance(target.ServerPosition) <= R.Range &&
                (E.LSIsReady() || (!E.LSIsReady() && !CE)) && R.Instance.Name == IsFirstR) R.Cast();
            if (!Orbwalker.InAutoAttackRange(target) && E.LSIsReady() && CE && ((CR && CRS == 0 && R.LSIsReady() && R.Instance.Name == IsSecondR) || (CR && CRS == 1 && R.LSIsReady() && R.Instance.Name == IsFirstR) || !R.LSIsReady() || (R.LSIsReady() && !CR))) E.Cast(target);
            if ((!E.LSIsReady() || (E.LSIsReady() && !CE)) && CW && !Orbwalker.InAutoAttackRange(target) && Player.LSDistance(target) <= W.Range) W.Cast(target.ServerPosition);
            if (target.Health < R.GetDamage2(target) && Player.LSDistance(target.Position) <= R.Range - 50 && KSR) R.Cast();
            if (R.LSIsReady() && CR && CRS == 1 && Player.LSDistance(target.ServerPosition) <= R.Range &&
                (!E.LSIsReady() || (E.LSIsReady() && !CE)) && (!W.LSIsReady() || (W.LSIsReady() && !CW)))
            {
                R.Cast();
                R.Cast();
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (!Orbwalker.InAutoAttackRange(target) && E.LSIsReady() && HE) E.Cast(target);
            if ((!E.LSIsReady() || (E.LSIsReady() && !HE)) && HW && !Orbwalker.InAutoAttackRange(target) && Player.LSDistance(target) <= W.Range) W.Cast(target.ServerPosition);
        }

        private static void JungleClear()
        {
            var Mobs = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

            if (W.LSIsReady())
            {

                List<Vector2> minionVec2List = new List<Vector2>();

                foreach (var Mob in Mobs)
                    minionVec2List.Add(Mob.ServerPosition.LSTo2D());

                var MaxHit = MinionManager.GetBestCircularFarmLocation(minionVec2List, 200f, W.Range);

                if (MaxHit.MinionsHit >= 1)
                    W.Cast(MaxHit.Position);
            }
        }

        private static void LaneClear()
        {
            var Minions = MinionManager.GetMinions(W.Range, MinionTypes.All, MinionTeam.Enemy);

            if (Minions.Count <= 0)
                return;

            if (W.LSIsReady())
            {

                List<Vector2> minionVec2List = new List<Vector2>();

                foreach (var Minion in Minions)
                    minionVec2List.Add(Minion.ServerPosition.LSTo2D());

                var MaxHit = MinionManager.GetBestCircularFarmLocation(minionVec2List, 200f, W.Range);
                for (int i = 20;i > 0;i--)
                    if (MaxHit.MinionsHit >= minhit)
                    if (MaxHit.MinionsHit >= i && W.LSIsReady() && LW) W.Cast(MaxHit.Position);
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
            Menu = new Menu("Hoola Talon", "hoolatalon", true);

            Menu.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Menu.AddSubMenu(targetSelectorMenu);

            var Combo = new Menu("Combo", "Combo");
            Combo.AddItem(new MenuItem("CQ", "Use Q").SetValue(true));
            Combo.AddItem(new MenuItem("CW", "Use W").SetValue(true));
            Combo.AddItem(new MenuItem("CE", "Use E").SetValue(true));
            Combo.AddItem(new MenuItem("CR", "Use R").SetValue(true));
            Combo.AddItem(new MenuItem("CRS", "R Option").SetValue(new StringList(new[] {"First (R E Q W)", "Last (E Q W R)"})));
            Combo.AddItem(new MenuItem("CAR", "Auto R Can Hit > X (0 = Don't)").SetValue(new Slider(3,0,5)));
            Menu.AddSubMenu(Combo);

            var Harass = new Menu("Harass", "Harass");
            Harass.AddItem(new MenuItem("HQ", "Use Q").SetValue(true));
            Harass.AddItem(new MenuItem("HW", "Use W").SetValue(true));
            Harass.AddItem(new MenuItem("HE", "Use E").SetValue(false));
            Menu.AddSubMenu(Harass);

            var LaneClear = new Menu("LaneClear", "LaneClear");
            LaneClear.AddItem(new MenuItem("LQ", "Use Q").SetValue(true));
            LaneClear.AddItem(new MenuItem("LW", "Use W").SetValue(true));
            LaneClear.AddItem(new MenuItem("minhit", "W Minimum Hit").SetValue(new Slider(3,0,5)));
            Menu.AddSubMenu(LaneClear);

            var Draw = new Menu("Draw", "Draw");
            Draw.AddItem(new MenuItem("Dind", "Draw Damage Indicator").SetValue(true));
            Draw.AddItem(new MenuItem("DW", "Draw W Range").SetValue(false));
            Draw.AddItem(new MenuItem("DE", "Draw E Range").SetValue(false));
            Draw.AddItem(new MenuItem("DR", "Draw E W Killsteal Range").SetValue(false));
            Menu.AddSubMenu(Draw);


            var Killsteal = new Menu("Killsteal", "Killsteal");
            Killsteal.AddItem(new MenuItem("KSW", "Killsteal W").SetValue(true));
            Killsteal.AddItem(new MenuItem("KSEW", "Killsteal EW").SetValue(true));
            Killsteal.AddItem(new MenuItem("KSR", "Killsteal R (While Combo Only)").SetValue(true));
            Menu.AddSubMenu(Killsteal);

            Menu.AddToMainMenu();
        }

        private static float getComboDamage(Obj_AI_Base enemy)
        {
            if (enemy != null)
            {
                float damage = 0;
                if (Q.LSIsReady()) damage += Q.GetDamage2(enemy) + (float)Player.GetAutoAttackDamage2(enemy);
                if (W.LSIsReady()) damage += W.GetDamage2(enemy) * 2;
                if (R.LSIsReady()) damage += R.GetDamage2(enemy) * 2;
                if (HasItem()) damage += (float)Player.GetAutoAttackDamage2(enemy) * 0.7f;

                return damage;
            }
            return 0;
        }
        private static void OnEndScene(EventArgs args)
        {
            foreach (
                var enemy in
                    ObjectManager.Get<AIHeroClient>()
                        .Where(ene => ene.LSIsValidTarget() && !ene.IsZombie))
            {
                if (Dind)
                {
                    Indicator.unit = enemy;
                    Indicator.drawDmg(getComboDamage(enemy), new SharpDX.ColorBGRA(255, 204, 0, 170));
                }

            }
        }
    }
}