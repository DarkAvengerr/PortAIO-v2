using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace JustBlitz
{
    internal class Program
    {
        public const string ChampName = "Blitzcrank";
        public const string Menuname = "JustBlitzcrank";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        private static AIHeroClient currentTarget
        {
            get
            {
                //if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget is AIHeroClient && TargetSelector.SelectedTarget.Team != player.Team)
                    //return (AIHeroClient)TargetSelector.SelectedTarget;
                if (TargetSelector.GetSelectedTarget() != null)
                    return TargetSelector.GetSelectedTarget();
                return TargetSelector.GetTarget(Q.Range + 175, TargetSelector.DamageType.Magical);
            }
        }
        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != ChampName)
                return;

            Notifications.AddNotification("JustBlitzcrank Loaded - [V.1.0.5.0]", 8000);

            //Ability Information - Range - Variables.
            Q = new Spell(SpellSlot.Q, 925);
            Q.SetSkillshot(0.55f, 80f, 1800f, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 200);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 600);
           

            abilitySequence = new int[] { 1, 3, 2, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Blacklist
            

            //Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            foreach (var obj in ObjectManager.Get<AIHeroClient>().Where(obj => obj.Team != player.Team))
            {
                Config.AddItem(new MenuItem("grapop" + obj.ChampionName, obj.ChampionName))
                    .SetValue(new StringList(new[] { "Don't Grab", "Grab"}, 1));
            }
            Config.SubMenu("Combo").AddItem(new MenuItem("qhit", "Q Hitchance 1-Low, 4-Very High")).SetValue(new Slider(4, 1, 4));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("Rene", "Min Enemies for R").SetValue(new Slider(2, 1, 5)));

            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("hQ", "Use Q").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("hW", "Use W").SetValue(false));
            Config.SubMenu("Harass").AddItem(new MenuItem("hE", "Use E").SetValue(true));
            Config.SubMenu("Harass")
                .AddItem(new MenuItem("harassmana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Item
            Config.AddSubMenu(new Menu("Item", "Item"));
            Config.SubMenu("Item").AddItem(new MenuItem("useGhostblade", "Use Youmuu's Ghostblade").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBOTRK", "Use Blade of the Ruined King").SetValue(true));
            Config.SubMenu("Item").AddItem(new MenuItem("eL", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("oL", "  Own HP Percentage").SetValue(new Slider(65, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseBilge", "Use Bilgewater Cutlass").SetValue(true));
            Config.SubMenu("Item")
                .AddItem(new MenuItem("HLe", "  Enemy HP Percentage").SetValue(new Slider(80, 0, 100)));
            Config.SubMenu("Item").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));


            //Laneclear
            Config.AddSubMenu(new Menu("Clear", "Clear"));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneW", "Use W").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneE", "Use E").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("laneR", "Use R").SetValue(true));
            Config.SubMenu("Clear").AddItem(new MenuItem("rmin", "Min Minion for R").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Clear")
                .AddItem(new MenuItem("lanemana", "Mana Percentage").SetValue(new Slider(30, 0, 100)));

            //Draw
            Config.AddSubMenu(new Menu("Draw", "Draw"));
            Config.SubMenu("Draw").AddItem(new MenuItem("Draw_Disabled", "Disable All Spell Drawings").SetValue(false));
            Config.SubMenu("Draw").AddItem(new MenuItem("Qdraw", "Draw Q Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Edraw", "Draw E Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("Rdraw", "Draw R Range").SetValue(true));
            Config.SubMenu("Draw").AddItem(new MenuItem("combodamage", "Damage on HPBar")).SetValue(true);
            Config.SubMenu("Draw").AddItem(new MenuItem("qpred", "Draw Prediction")).SetValue(true);

            //Misc
            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("ksQ", "Killsteal with Q").SetValue(true));
            Config.SubMenu("Misc")
                .AddItem(new MenuItem("qrange", "Mininum Distance to Q"))
                .SetValue(new Slider(920, 1, (int)Q.Range));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQ", "Autocast Q On Immobile Targets", true).SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("AutoQ2", "Autocast Q On Dashing Targets", true).SetValue(false));
            Config.SubMenu("Misc")
                .AddItem(new MenuItem("qmana", "Auto Q Mana Percentage").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Misc").AddItem(new MenuItem("antigap", "AntiGapCloser with W").SetValue(false));
            Config.SubMenu("Misc").AddItem(new MenuItem("interruptq", "Interrupt with Q").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("interrupte", "Interrupt with E").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("interruptr", "Interrupt with R").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("autolevel", "Auto Level Spells").SetValue(false));

            Config.AddToMainMenu();
            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            Orbwalking.BeforeAttack += OrbwalkingBeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        private static void OrbwalkingBeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (!args.Unit.IsMe || !(args.Target is AIHeroClient))
            {
                return;
            }

            if ((Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo && Config.Item("UseE").GetValue<bool>())
                || (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Config.Item("hE").GetValue<bool>()))
            {
                E.Cast();
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (Q.IsReady() && Config.Item("grapop" + sender.ChampionName).GetValue<StringList>().SelectedIndex == 1 && sender.IsValidTarget(Q.Range) && Config.Item("interruptq").GetValue<bool>())
            {
                Q.CastIfHitchanceEquals(sender, HitChance.Dashing, true);
                Q.CastIfHitchanceEquals(sender, HitChance.Immobile, true);
                var qpred = Q.GetPrediction(sender);
                if (qpred.Hitchance >= HitChance.High &&
                    qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 1)
                {
                    Q.Cast(qpred.CastPosition);
                }
            }
            
            if (R.IsReady() && sender.IsValidTarget(R.Range) && Config.Item("interruptr").GetValue<bool>())
                R.Cast();

            if (E.IsReady() && sender.IsValidTarget(E.Range) && Config.Item("interrupte").GetValue<bool>())
            {
                Orbwalker.ForceTarget(sender);
                E.Cast();
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (W.IsReady() && gapcloser.Sender.IsValidTarget(400) && Config.Item("antigap").GetValue<bool>())
                W.Cast();
        }

        private static void combo()
        {
            var qrange = Config.Item("qrange").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget())
                return;
            
            if (W.IsReady() && target.IsValidTarget(Q.Range) && Config.Item("UseW").GetValue<bool>())
                W.Cast();

            if (Q.IsReady() && Config.Item("grapop" + target.ChampionName).GetValue<StringList>().SelectedIndex == 1 && Config.Item("UseQ").GetValue<bool>() && target.IsValidTarget(qrange))
            {
                Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                var qpred = Q.GetPrediction(target);
                if (qpred.Hitchance >= (HitChance)Config.Item("qhit").GetValue<Slider>().Value + 1 &&
                    qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 1)
                {
                    Q.Cast(qpred.CastPosition);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && Config.Item("UseE").GetValue<bool>())
                E.Cast();
                
            var enemys = target.CountEnemiesInRange(R.Range);
            if (R.IsReady() && Config.Item("UseR").GetValue<bool>() && target.IsValidTarget(R.Range))
                if (Config.Item("Rene").GetValue<Slider>().Value <= enemys)
                    R.Cast();

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                items();
        }

        private static void AutoQ()
        {
            if (player.IsDead || player.IsRecalling())
            {
                return;
            }

            foreach (
                AIHeroClient target in
                    HeroManager.Enemies.Where(
                        x => x.IsValidTarget(Q.Range) && !x.HasBuffOfType(BuffType.Invulnerability)))
            {
                if (target != null)
                {
                    var qmana = Config.Item("qmana").GetValue<Slider>().Value;
                    if (Config.Item("AutoQ").GetValue<bool>() && Config.Item("grapop" + target.ChampionName).GetValue<StringList>().SelectedIndex == 1 && Q.CanCast(target) && Q.GetPrediction(target).Hitchance >= HitChance.Immobile && player.ManaPercent >= qmana)
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                        Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                        var qpred = Q.GetPrediction(target);
                        if (qpred.Hitchance >= (HitChance)Config.Item("qhit").GetValue<Slider>().Value + 1 &&
                            qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 1)
                        {
                            Q.Cast(qpred.CastPosition);
                        }
                    }
                }
                {
                    var qmana = Config.Item("qmana").GetValue<Slider>().Value;
                    if (Config.Item("AutoQ2").GetValue<bool>() && Config.Item("grapop" + target.ChampionName).GetValue<StringList>().SelectedIndex == 1 && Q.CanCast(target) && Q.GetPrediction(target).Hitchance >= HitChance.Dashing && player.ManaPercent >= qmana)
                    {
                        Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                        Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                        var qpred = Q.GetPrediction(target);
                        if (qpred.Hitchance >= (HitChance)Config.Item("qhit").GetValue<Slider>().Value + 1 &&
                            qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                        {
                            Q.Cast(qpred.CastPosition);
                        }
                    }
                }
            }
        }

        private static float GetComboDamage(AIHeroClient Target)
        {
            if (Target != null)
            {
                float ComboDamage = new float();

                ComboDamage += Q.IsReady() ? Q.GetDamage(Target) : 0;
                ComboDamage += E.IsReady() ? E.GetDamage(Target) : 0;
                ComboDamage += player.TotalAttackDamage;
                return ComboDamage;
            }
            return 0;
        }

        private static float[] GetLength()
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Target != null)
            {
                float[] Length =
                {
                    GetComboDamage(Target) > Target.Health
                        ? 0
                        : (Target.Health - GetComboDamage(Target))/Target.MaxHealth,
                    Target.Health/Target.MaxHealth
                };
                return Length;
            }
            return new float[] { 0, 0 };
        }

        private static float IgniteDamage(AIHeroClient target)
        {
            if (Ignite == SpellSlot.Unknown || player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Killsteal()
        {
            var qrange = Config.Item("qrange").GetValue<Slider>().Value;
            if (Config.Item("ksQ").GetValue<bool>() && Q.IsReady())
            {
                var target =
                    ObjectManager.Get<AIHeroClient>()
                        .FirstOrDefault(
                            enemy =>
                                enemy.IsValidTarget(qrange) && enemy.Health < player.GetSpellDamage(enemy, SpellSlot.Q));
                if (target.IsValidTarget(qrange))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                    Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                    var qpred = Q.GetPrediction(target);
                    if (qpred.Hitchance >= (HitChance)Config.Item("qhit").GetValue<Slider>().Value + 1 &&
                        qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 2)
                    {
                        Q.Cast(qpred.CastPosition);
                    }
                }
            }
        }

        private static void items()
        {
            Ignite = player.GetSpellSlot("summonerdot");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("eL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(player) && botrk.IsInRange(target)
                && target.HealthPercent <= Config.Item("oL").GetValue<Slider>().Value
                && Config.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(player) && cutlass.IsInRange(target) &&
                target.HealthPercent <= Config.Item("HLe").GetValue<Slider>().Value
                && Config.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(player) && target.IsValidTarget(Q.Range)
                && Config.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

            if (player.Distance(target.Position) <= 600 && IgniteDamage(target) >= target.Health &&
                Config.Item("UseIgnite").GetValue<bool>())
                player.Spellbook.CastSpell(Ignite, target);
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (player.IsDead || MenuGUI.IsChatOpen || player.IsRecalling())
            {
                return;
            }

            if (Config.Item("autolevel").GetValue<bool>()) LevelUpSpells();

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    break;
            }

            Killsteal();
            AutoQ();
        }

        //Thanks to LuNi
        private static void LevelUpSpells()
        {
            int qL = player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = new int[] { 0, 0, 0, 0 };
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }

        private static void harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var harassmana = Config.Item("harassmana").GetValue<Slider>().Value;
            if (target == null || !target.IsValidTarget())
                return;

            if (W.IsReady() && target.IsValidTarget(Q.Range) && player.ManaPercent >= harassmana &&
               Config.Item("hW").GetValue<bool>())
                W.Cast();

            if (target.Distance(player.ServerPosition) > Config.Item("qrange").GetValue<Slider>().Value && Config.Item("grapop" + target.ChampionName).GetValue<StringList>().SelectedIndex == 1 && Config.Item("hQ").GetValue<bool>() && player.ManaPercent >= harassmana)
            {
                Q.CastIfHitchanceEquals(target, HitChance.Dashing, true);
                Q.CastIfHitchanceEquals(target, HitChance.Immobile, true);
                var qpred = Q.GetPrediction(target);
                if (qpred.Hitchance >= (HitChance)Config.Item("qhit").GetValue<Slider>().Value + 1 &&
                    qpred.CollisionObjects.Count(h => h.IsEnemy && !h.IsDead && h is Obj_AI_Minion) < 3)
                {
                    Q.Cast(qpred.CastPosition);
                }
            }

            if (E.IsReady() && target.IsValidTarget(E.Range) && player.ManaPercent >= harassmana &&
               Config.Item("hE").GetValue<bool>())
                E.Cast();
            }

        private static void Clear()
        {
            var minionObj = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly,
                MinionOrderTypes.MaxHealth);
            var lanemana = Config.Item("lanemana").GetValue<Slider>().Value;

            if (!minionObj.Any())
            {
                return;
            }

            if (player.ManaPercent >= lanemana)
            {
               if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneW").GetValue<bool>())
                {
                    W.Cast();
                }
            }

            if (player.ManaPercent >= lanemana)
            {
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneE").GetValue<bool>())
                {
                    E.Cast();
                }
            }

            if (minionObj.Count > Config.Item("rmin").GetValue<Slider>().Value && player.ManaPercent >= lanemana)
                if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Config.Item("laneR").GetValue<bool>())
                {
                    {
                        R.Cast();
                    }
                }

        }

        private static void OnDraw(EventArgs args)
        {
            var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Config.Item("Draw_Disabled").GetValue<bool>())
                return;

            if (Config.Item("Qdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, Q.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Edraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, W.Range, System.Drawing.Color.White, 3);
            if (Config.Item("Rdraw").GetValue<bool>())
                Render.Circle.DrawCircle(player.Position, R.Range, System.Drawing.Color.White, 3);
            if (Config.Item("combodamage").GetValue<bool>() && Q.IsInRange(Target))
            {
                float[] Positions = GetLength();
                Drawing.DrawLine
                    (
                        new Vector2(Target.HPBarPosition.X + 10 + Positions[0] * 104, Target.HPBarPosition.Y + 20),
                        new Vector2(Target.HPBarPosition.X + 10 + Positions[1] * 104, Target.HPBarPosition.Y + 20),
                        9,
                        Color.Orange
                    );
            }
            if (Config.SubMenu("Draw").Item("qpred").GetValue<bool>() && !player.IsDead)
            {
                if (currentTarget != null && player.Distance(currentTarget) < Q.Range + 200)
                {
                    var playerPos = Drawing.WorldToScreen(player.Position);
                    var targetPos = Drawing.WorldToScreen(currentTarget.Position);
                    Drawing.DrawLine(playerPos, targetPos, 4,
                        Q.GetPrediction(currentTarget, overrideRange: Q.Range).Hitchance < HitChance.High
                            ? Color.LightSlateGray
                            : Color.SpringGreen);
                }
            }

        }

    }
}
