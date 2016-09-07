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
 namespace HeavenStrikeNidalee
{
    class Program
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, E, Q2, W2, E2, R, R2, summoner1, summoner2;

        private static Menu Menu;

        private static Obj_AI_Base TTar;

        private static int qhumancount, ehumancount, qcougarcount, wcougarcount, ecougarcount;

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != "Nidalee")
                return;

            Q = new Spell(SpellSlot.Q,1500);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);
            Q2 = new Spell(SpellSlot.Q);
            W2 = new Spell(SpellSlot.W);
            E2 = new Spell(SpellSlot.E);
            R2 = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 40, 1300, true, SkillshotType.SkillshotLine);
            Q.MinHitChance = HitChance.Medium;


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);
            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector")); ;
            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu Combo = spellMenu.AddSubMenu(new Menu("Combo", "Combo"));
            Combo.AddItem(new MenuItem("useQcombohuman", "use Q human").SetValue(true));
            Combo.AddItem(new MenuItem("useWcombohuman", "use W human").SetValue(true));
            Combo.AddItem(new MenuItem("useRalways", "R marked target always").SetValue(true));
            Combo.AddItem(new MenuItem("useRmarkedaloneor2", "R marked target && target is alone or 2").SetValue(true));
            Combo.AddItem(new MenuItem("useRalwayskillable", "R killable always").SetValue(true));
            Combo.AddItem(new MenuItem("useRkillablealoneor2", "R killable target is alone or 2").SetValue(true));
            Combo.AddItem(new MenuItem("useRbackhuman", "R back to human").SetValue(true));
            Combo.AddItem(new MenuItem("useWalways", "W cougar always").SetValue(true));
            Combo.AddItem(new MenuItem("useWalwaysaloneor2", "W cougar always target is alone or 2").SetValue(true));
            Combo.AddItem(new MenuItem("useWkillable", "W cougar killable always").SetValue(true));
            Combo.AddItem(new MenuItem("useWkillablealoneor2", "W cougar killable alone or 2").SetValue(true));
            Menu Harass = spellMenu.AddSubMenu(new Menu("Harass", "Harass"));
            Harass.AddItem(new MenuItem("useQharass", "use Q").SetValue(true));
            Harass.AddItem(new MenuItem("manaharass", "if mana >").SetValue(new Slider(40, 0, 100)));
            Menu Clear = spellMenu.AddSubMenu(new Menu("Clear", "Clear"));
            Clear.AddItem(new MenuItem("useQclear", "use Q cougar").SetValue(true));
            Clear.AddItem(new MenuItem("useEclear", "use E cougar").SetValue(true));
            Clear.AddItem(new MenuItem("useWclear", "use W cougar").SetValue(true));
            Clear.AddItem(new MenuItem("autoswitchclear", "auto switch to cougar").SetValue(false));
            Menu auto = spellMenu.AddSubMenu(new Menu("Misc", "Misc"));
            auto.AddItem(new MenuItem("ksQhuman", "Ks with Q human").SetValue(true));
            auto.AddItem(new MenuItem("switchformks", "Switch form ks").SetValue(true));
            auto.AddItem(new MenuItem("heal", "Heal").SetValue(true));
            auto.AddItem(new MenuItem("rheal", "switch form heal").SetValue(true));
            auto.AddItem(new MenuItem("lowhp", "if hp <").SetValue(new Slider(40, 0, 100)));
            foreach (var hero in HeroManager.Allies)
            {
                auto.AddItem(new MenuItem(hero.ChampionName, "Heal " + hero.ChampionName).SetValue(true));
            }
            Menu Drawing = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            Drawing.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            Menu.AddToMainMenu();

            //Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            //Orbwalking.OnAttack += OnAttack;
            Obj_AI_Base.OnSpellCast += oncast;
            ////CustomEvents.Unit.OnDash += Unit_OnDash;
            //Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            EloBuddy.Drawing.OnDraw += Drawing_OnDraw;
            //LeagueSharp.Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
        }
        private static int manaharass { get { return Menu.Item("manaharass").GetValue<Slider>().Value; } }
        private static int hpheal { get { return Menu.Item("lowhp").GetValue<Slider>().Value; } }
        private static bool qcombohuman { get { return Menu.Item("useQcombohuman").GetValue<bool>(); } }
        private static bool wcombohuman { get { return Menu.Item("useWcombohuman").GetValue<bool>(); } }

        private static bool rcomboalways { get { return Menu.Item("useRalways").GetValue<bool>(); } }
        private static bool rcombomarkedaloneor2 { get { return Menu.Item("useRmarkedaloneor2").GetValue<bool>(); } }
        private static bool rcomboalwayskillable { get { return Menu.Item("useRalwayskillable").GetValue<bool>(); } }
        private static bool rcombokillablealoneor2 { get { return Menu.Item("useRkillablealoneor2").GetValue<bool>(); } }
        private static bool rcombobackhuman { get { return Menu.Item("useRbackhuman").GetValue<bool>(); } }
        private static bool wcomboalways { get { return Menu.Item("useWalways").GetValue<bool>(); } }
        private static bool wcomboalwaysaloneor2 { get { return Menu.Item("useWalwaysaloneor2").GetValue<bool>(); } }
        private static bool wcombokillable { get { return Menu.Item("useWkillable").GetValue<bool>(); } }
        private static bool wcombokillablealoneor2 { get { return Menu.Item("useWkillablealoneor2").GetValue<bool>(); } }
        private static bool qharass { get { return Menu.Item("useQharass").GetValue<bool>(); } }
        private static bool qclear { get { return Menu.Item("useQclear").GetValue<bool>(); } }
        private static bool eclear { get { return Menu.Item("useEclear").GetValue<bool>(); } }
        private static bool wclear { get { return Menu.Item("useWclear").GetValue<bool>(); } }
        private static bool rclear { get { return Menu.Item("autoswitchclear").GetValue<bool>(); } }
        private static bool qks { get { return Menu.Item("ksQhuman").GetValue<bool>(); } }
        private static bool rks { get { return Menu.Item("switchformks").GetValue<bool>(); } }
        private static bool heal { get { return Menu.Item("heal").GetValue<bool>(); } }
        private static bool rheal { get { return Menu.Item("rheal").GetValue<bool>(); } }
        private static bool drawQ { get { return Menu.Item("drawQ").GetValue<bool>(); } }
        private static bool CougarForm { get { return Q.Instance.Name == "Takedown"; } }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (drawQ)
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Yellow);
        }
        private static void oncast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            var spell = args.SData;
            if (spell.Name == "Swipe")
            {
                ecougarcount = Utils.GameTimeTickCount;
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Q.IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(300-Game.Ping/2,() => Q.Cast(Player.Position));
                }
            }
            if (spell.Name == "Pounce")
            {
                wcougarcount = Utils.GameTimeTickCount;
                //if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                //{
                //    LeagueSharp.Common.Utility.DelayAction.Add(100 - Game.Ping / 2, () => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit,TTar));
                //}
            }
            if (spell.Name == "Takedown")
            {
                qcougarcount = Utils.GameTimeTickCount;
                Orbwalking.ResetAutoAttackTimer();
            }
            if (spell.Name == "JavelinToss")
            {
                qhumancount = Utils.GameTimeTickCount;
            }
            if (spell.Name == "PrimalSurge")
            {
                ehumancount = Utils.GameTimeTickCount;
            }
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsMe) return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if(CougarForm && E.IsReady())
                {

                    E.Cast(target.Position);
                }
                if(CougarForm && !E.IsReady() && Q.IsReady())
                {
                    Q.Cast();
                }
                if(!CougarForm && Q.IsReady())
                {
                   LeagueSharp.Common.Utility.DelayAction.Add(500,() => castQtarget(target));
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (CougarForm && !E.IsReady() && Q.IsReady() && qclear)
                {
                    Q.Cast();
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && qharass && Player.Mana*100/Player.MaxHealth >= manaharass)
            {
                if (!CougarForm && Q.IsReady())
                {
                    LeagueSharp.Common.Utility.DelayAction.Add(500, () => castQtarget(target));
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Player.IsRecalling())
                Auto();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                Harass();
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                Clear();
        }

        private static void Auto()
        {
            //ks with Q
            if(!CougarForm && Q.IsReady() && qks)
            {
                foreach(var x in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsZombie))
                {
                    if (Qhumandamage(x) > x.Health)
                    {
                        Q.Cast(x);
                    }
                }
            }
            // switch form ks
            if (CougarForm && QhumanReady && R.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && rks)
            {
                foreach (var x in HeroManager.Enemies.Where(x => x.IsValidTarget(Q.Range) && !x.IsZombie))
                {
                    if (Qhumandamage(x) > x.Health)
                    {
                        R.Cast(x);
                    }
                }
            }
            // heal
            if (!CougarForm && E.IsReady() && heal)
            {
                foreach (var x in HeroManager.Allies.Where(x => x.IsValidTarget(E.Range,false) && !x.IsZombie))
                {
                    if (x.Health*100/x.MaxHealth <= hpheal && Menu.Item(x.ChampionName).GetValue<bool>())
                    {
                        E.Cast(x);
                    }
                }
            }
            // switch form heal
            if (CougarForm && EhumanReady && R.IsReady() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && rheal)
            {
                foreach (var x in HeroManager.Allies.Where(x => x.IsValidTarget(E.Range, false) && !x.IsZombie))
                {
                    if (x.Health * 100 / x.MaxHealth <= hpheal && Menu.Item(x.ChampionName).GetValue<bool>())
                    {
                        R.Cast(x);
                    }
                }
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && CougarForm)
            {
                if (Orbwalker.GetTarget() != null && Orbwalker.GetTarget().Health <= Qcougardamage(Orbwalker.GetTarget()) + Shendamage(Orbwalker.GetTarget()) + Wcougardamage(Orbwalker.GetTarget()) + Ecougardamage(Orbwalker.GetTarget()) && Q.IsReady())
                    Q.Cast();
            }
        }

        private static void Clear()
        {
            //laneclear
            // use w
            if (CougarForm && wclear)
            {
                var target = MinionManager.GetMinions(Player.Position, 375, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).Where(x => x.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault();
                if (target != null)
                    W.Cast(target.Position);
            }
            // use e
            if (CougarForm && eclear)
            {
                var target = MinionManager.GetMinions(Player.Position, 300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health).Where(x => x.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault();
                if (target != null)
                    E.Cast(target.Position);
            }
            // auto switch to cougar
            if (!CougarForm && R.IsReady() && rclear)
            {
                R.Cast();
            }
            //jungleclear
            // use w
            if (CougarForm && wclear)
            {
                var firsttarget = MinionManager.GetMinions(Player.Position, 700, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(x => x.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault(x => x.HasBuff("nidaleepassivehunted"));
                if (firsttarget != null)
                    W.Cast(firsttarget.Position);
                var target = MinionManager.GetMinions(Player.Position, 375, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(x => x.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault();
                if (target != null)
                    W.Cast(target.Position);
            }
            // use e
            if (CougarForm && eclear)
            {
                var target = MinionManager.GetMinions(Player.Position, 300, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).Where(x => x.CharData.BaseSkinName != "gangplankbarrel").FirstOrDefault();
                if (target != null)
                    E.Cast(target.Position);
            }
        }

        private static void Harass()
        {
            // use Q harass
            if (!CougarForm && qharass && Player.Mana*100/Player.MaxHealth >= manaharass && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                var target2 = Orbwalker.GetTarget();
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (!Orbwalking.InAutoAttackRange(target))
                    {
                        Q.Cast(target);
                    }
                    else if (target.NetworkId != target2.NetworkId)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void Combo()
        {
            // use Q combo
            if(!CougarForm && qcombohuman && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                var target2 = Orbwalker.GetTarget();
                if (target.IsValidTarget() && !target.IsZombie)
                {
                    if (!Orbwalking.InAutoAttackRange(target))
                    {
                        Q.Cast(target);
                    }
                    else if (target.NetworkId != target2.NetworkId)
                    {
                        Q.Cast(target);
                    }
                }
            }
            // use W combo
            if (!CougarForm && W.IsReady() && wcombohuman)
            {
                var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && !target.IsZombie)
                    W.Cast(target.Position);
            }

            // transform always when a target is marked 
            if (!CougarForm && R.IsReady() && rcomboalways)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => x.Health).LastOrDefault();
                if (heroes.IsValidTarget() && WcougarReady && Orbwalking.CanMove(40 + Game.Ping / 2))
                {
                    R.Cast();
                }
            }
            // transform when a target is marked and is alone or 2 only
            if (!CougarForm && R.IsReady() && rcombomarkedaloneor2)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => x.Health);
                foreach (var x in heroes)
                {
                    if (x.CountEnemiesInRange(1000) <= 2)
                    {
                        R.Cast();
                    }
                }
            }
            // transform killable always
            if (!CougarForm && R.IsReady() && rcomboalwayskillable)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => x.Health);
                // marked target is killable
                foreach (var x in heroes)
                {
                    if (x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0)
                    {
                        R.Cast();
                        return;
                    }
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius);
                // unmarked target is killable
                foreach (var x in targets)
                {
                    if (x.Health - Shendamage(x) - Wcougardamage(x) - Ecougardamage(x) - Qcougardamage(x) < 0)
                    {
                        R.Cast();
                        return;
                    }
                }
            }
            // transform killable and target is alone or 2 only
            if (!CougarForm && R.IsReady() && rcombokillablealoneor2)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => x.Health);
                // marked target is killable
                foreach (var x in heroes)
                {
                    if (x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0 && x.CountEnemiesInRange(1000) <= 2)
                    {
                        R.Cast();
                        return;
                    }
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius);
                // unmarked target is killable
                foreach (var x in targets)
                {
                    if (x.Health - Shendamage(x) - Wcougardamage(x) - Ecougardamage(x) - Qcougardamage(x) < 0 && x.CountEnemiesInRange(1000) <= 2)
                    {
                        R.Cast();
                        return;
                    }
                }
            }
            // W always (priority marked target)
            if (CougarForm && W.IsReady() && wcomboalways)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => 1 - (x.Health - Shendamage(x) - Wcougardamage(x) - Ecougardamage(x) - Qcougardamage(x)));
                foreach (var x in heroes)
                {
                    W.Cast(x.Position);
                    return;
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius)
                   .OrderByDescending(x => 1 - (x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x)));
                foreach (var x in heroes)
                {
                    W.Cast(x.Position);
                    return;
                }
            }
            // W always if target is alone or 2 only (priority marked target)
            if (CougarForm && W.IsReady() && wcomboalwaysaloneor2)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750)
                    .OrderByDescending(x => 1 - (x.Health - Shendamage(x) - Wcougardamage(x) - Ecougardamage(x) - Qcougardamage(x)));
                foreach (var x in heroes)
                {
                    if (x.CountEnemiesInRange(1000) <= 2)
                    {
                        W.Cast(x.Position);
                        return;
                    }
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius)
                   .OrderByDescending(x => 1 - (x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x)));
                foreach (var x in heroes)
                {
                    if (x.CountEnemiesInRange(1000) <= 2)
                    {
                        W.Cast(x.Position);
                        return;
                    }
                }
            }
            // W to killable target
            if (CougarForm && W.IsReady() && wcombokillable)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750
                     && x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0)
                    .OrderByDescending(x => 1 - x.Health);
                foreach (var x in heroes)
                {
                    W.Cast(x.Position);
                    return;
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius
                    && x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0)
                    .OrderByDescending(x => 1 - x.Health);
                foreach (var x in targets)
                {
                    W.Cast(x.Position);
                    return;
                }
            }
            // W to killable target alone or 2
            if (CougarForm && W.IsReady() && wcombokillablealoneor2)
            {
                var heroes = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 750
                     && x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0)
                    .OrderByDescending(x => 1 - x.Health);
                foreach (var x in heroes)
                {
                    if (x.CountEnemiesInRange(1000) <= 2)
                    {
                        W.Cast(x.Position);
                        return;
                    }
                }
                var targets = HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsZombie && !x.HasBuff("nidaleepassivehunted") && x.Distance(Player.Position) <= 375 + Player.BoundingRadius
                    && x.Health - Shendamage(x) - Wcougardamage(x) * 2 - Ecougardamage(x) - Qcougardamage(x) < 0)
                    .OrderByDescending(x => 1 - x.Health);
                foreach (var x in targets)
                {
                    if (x.CountEnemiesInRange(1000) <= 2)
                    {
                        W.Cast(x.Position);
                        return;
                    }
                }
            }
            // R switch back to human
            if (CougarForm && R.IsReady() && rcombobackhuman && !Q.IsReady() && !Player.HasBuff("Takedown"))
            {
                if (!W.IsReady() && Orbwalker.GetTarget() == null && QhumanReady)
                    R.Cast();
            }
        }
        private static bool QhumanReady
        {
            get
            {
                if (Q.Instance.Level == 0)
                    return false;
                return
                    Player.Mana >= new int[] { 50, 60, 70, 80, 90 }[Q.Instance.Level - 1]
                    && Utils.GameTimeTickCount - qhumancount >= 6 * (1 - Player.PercentCooldownMod);
            }
        }
        private static bool EhumanReady
        {
            get
            {
                if (E.Instance.Level == 0)
                    return false;
                return
                    Player.Mana >= new int[] { 60 , 75 , 90 , 105 , 120 }[E.Instance.Level - 1]
                    && Utils.GameTimeTickCount - qhumancount >= 12 * (1 - Player.PercentCooldownMod);
            }
        }
        private static bool QcougarReady
        {
            get
            {
                return Utils.GameTimeTickCount - qcougarcount >= 5 * (1 - Player.PercentCooldownMod);
            }
        }
        private static bool EcougarReady
        {
            get
            {
                return Utils.GameTimeTickCount - ecougarcount >= 5 * (1 - Player.PercentCooldownMod);
            }
        }
        private static bool WcougarReady
        {
            get
            {
                return (CougarForm && W.IsReady()) ? true : Utils.GameTimeTickCount - wcougarcount >= 5 * (1 - Player.PercentCooldownMod);
            }
        }
        private static void castQtarget(AttackableUnit target)
        {
            if (target.IsValidTarget() && ! target.IsZombie)
            {
                var castpos = Q.GetPrediction(target as Obj_AI_Base).CastPosition;
                var collisions = Q.GetPrediction(target as Obj_AI_Base).CollisionObjects;
                if (!collisions.Any() && Q.IsReady() && !CougarForm)
                    Q.Cast(castpos);
            }
        }
        private static double Qhumandamage(AttackableUnit target)
        {
                var raw =  new double[] { 50, 70, 90, 110, 130 }[Q.Instance.Level - 1 ]
                                    + 0.4 * Player.FlatMagicDamageMod;
                return QhumanReady ? (Player.Distance(target.Position) < 525 ?
                    Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, raw) :
                    (Player.Distance(target.Position) >= 1300 ?
                    Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, raw) * 3 :
                    Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, raw) * (1 + (Player.Distance(target.Position)-525)/(1300-525)*2)
                    )) : 0;
        }
        private static double Qcougardamage(AttackableUnit target)
        {
            var raw = (new double[] { 4, 20, 50, 90 }[
                                        R.Instance.Level - 1]
                                     + 0.36 * Player.FlatMagicDamageMod
                                     + 0.75 * (Player.BaseAttackDamage + Player.FlatPhysicalDamageMod))
                                    * ((target.MaxHealth - (target.Health - Shendamage(target) -Wcougardamage(target)- Ecougardamage(target))) 
                                    * 1.5 / target.MaxHealth + 1);
            return QcougarReady ? Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical,(target as Obj_AI_Base).HasBuff("nidaleepassivehunted") ? raw * 1.33 : raw) : 0;
        }
        private static double Wcougardamage(AttackableUnit target)
        {
            var raw = new double[] { 50, 100, 150, 200 }[
                                        R.Instance.Level - 1]
                                    + 0.3 * Player.FlatMagicDamageMod;
            return WcougarReady ? Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, raw) : 0;
        }
        private static double Ecougardamage(AttackableUnit target)
        {
            var raw = new double[] { 70, 130, 190, 250 }[
                                        R.Instance.Level - 1]
                                    + 0.45 * Player.FlatMagicDamageMod;
            return EcougarReady ? Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, (target as Obj_AI_Base).HasBuff("nidaleepassivehunted") ? raw * 1.33 : raw) : 0;
        }
        private static double Shendamage(AttackableUnit target)
        {
            if (ItemData.Lich_Bane.GetItem().IsReady())
                return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, 1.75 * Player.BaseAttackDamage + 0.5 * Player.FlatMagicDamageMod);
            if (GetItem(1402).IsReady() || GetItem(1410).IsReady() || GetItem(1414).IsReady())
                return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Magical, 2 * Player.BaseAttackDamage + 0.3 * Player.FlatMagicDamageMod);
            if (ItemData.Trinity_Force.GetItem().IsReady())
                return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Physical, 3 * Player.BaseAttackDamage);
            if (ItemData.Iceborn_Gauntlet.GetItem().IsReady())
                return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Physical, 2.25 * Player.BaseAttackDamage);
            if (ItemData.Sheen.GetItem().IsReady())
                return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Physical, 2 * Player.BaseAttackDamage);
            return Player.CalcDamage(target as Obj_AI_Base, Damage.DamageType.Physical, Player.BaseAttackDamage);
        }
        public static void checkbuff()
        {
            String temp = "";
            foreach (var buff in Player.Buffs)
            {
                temp += (buff.Name + "(" + buff.Count + ")" + "(" + buff.Type.ToString() + ")" + ", ");
            }
            Chat.Say(temp);
        }
        public static Items.Item GetItem(int Id, float Range = 0)
        {
            return new Items.Item(Id, Range);
        }
    }
}
