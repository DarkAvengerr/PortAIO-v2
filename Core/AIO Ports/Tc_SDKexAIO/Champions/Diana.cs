using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Tc_SDKexAIO.Champions
{

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using LeagueSharp.Data.Enumerations;

    using System;
    using System.Linq;

    using Color = System.Drawing.Color;

    using Common;
    using static Common.Manager;

    using Config;


    internal static class Diana
    {

        private static Spell Q, W, E, R;
        private static Menu Menu => PlaySharp.ChampionMenu;
        private static AIHeroClient Player => PlaySharp.Player;

        private static HpBarDraw HpBarDraw = new HpBarDraw();

        private static SpellSlot Ignite = Player.GetSpellSlot("SummonerDot");

        private static float IgniteRange = 600f;

        internal static void Init()
        {
            Q = new Spell(SpellSlot.Q, 860).SetSkillshot(0.25f, 150f, 1400f, false, SkillshotType.SkillshotCircle);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, 850);

            var QMenu = Menu.Add(new Menu("Q", "Q.Set"));
            {
                QMenu.GetSeparator("Combo Mobe");
                QMenu.Add(new MenuBool("ComboQ", "Combo Q", true));
                QMenu.GetSeparator("Harass Mobe");
                QMenu.Add(new MenuBool("HarassQ", "Harass Q", true));
                QMenu.Add(new MenuSlider("HarassQMana", "Harass Q Min Mana >= ", 40, 0, 100));
                QMenu.GetSeparator("LaneClear Mobe");
                QMenu.Add(new MenuBool("LaneClearQ", "LaneClear Q", true));
                QMenu.Add(new MenuBool("JungleQ", "Jungle Q", true));
                QMenu.Add(new MenuSlider("JungleQMana", "Jungle Q Min Mana >= ", 40));
                QMenu.Add(new MenuSlider("LaneClearQMana", "LaneClear Q Min Mana >= ", 40, 0, 100));
                QMenu.Add(new MenuSlider("LaneClearMinMinions", "LaneClear Q Min Minions", 3, 1, 5));
                var QList = QMenu.Add(new Menu("QList", "Q Harass List"));
                {
                    if (GameObjects.EnemyHeroes.Any())
                    {
                        GameObjects.EnemyHeroes.ForEach(i => QList.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, !AutoEnableList.Contains(i.ChampionName))));
                    }
                }
            }

            var WMenu = Menu.Add(new Menu("W", "W.Set"));
            {
                WMenu.GetSeparator("Combo Mobe");
                WMenu.Add(new MenuBool("ComboW", "Combo W", true));
                WMenu.GetSeparator("Harass Mobe");
                WMenu.Add(new MenuBool("HarassW", "Harass W", true));
                WMenu.GetSeparator("LaneClear Mobe");
                WMenu.Add(new MenuBool("LaneClearW", "LaneClear W", true));
                WMenu.Add(new MenuBool("JungleW", "Jungle W", true));
                WMenu.Add(new MenuSlider("JungleWMana", "Jungle W Min Mana >= ", 40));
                WMenu.Add(new MenuSlider("WMinMana", "W Spell Min Mana", 40, 0, 100));
                WMenu.Add(new MenuSeparator("MISC", "    "));
                WMenu.Add(new MenuBool("AntiMelee", "Anti Melee Auto W", true));
            }

            var EMenu = Menu.Add(new Menu("E", "E.Set"));
            {
                EMenu.GetSeparator("Combo Mobe");
                EMenu.Add(new MenuBool("ComboE", "Combo E", true));
                EMenu.GetSeparator("Harass Mobe");
                EMenu.Add(new MenuBool("HarassE", "Harass E", true));
                EMenu.Add(new MenuSlider("HarassEMana", "Harass E Min Mana", 40, 0, 100));
                EMenu.GetSeparator("Misc Mobe");
                EMenu.Add(new MenuBool("InterruptE", "Use E Interrupt Enemy Spell", true));
                EMenu.Add(new MenuBool("GapcloserE", "Use E Gapcloser", true));

            }

            var RMenu = Menu.Add(new Menu("R", "R.Set"));
            {
                RMenu.GetSeparator("Combo Mobe");
                RMenu.Add(new MenuBool("ComboR", "Combo R", true));
                RMenu.Add(new MenuBool("ComboR2", "Combo R2", true));
                RMenu.Add(new MenuSlider("RLimitation", "Use R2 Nearby Enemy Counts Kill >= ", 2, 1, 5));
                RMenu.GetSeparator("R or R2 Mobe");
                RMenu.Add(new MenuList<string>("ComboRMobe", "Combo R Mobe", new[] { "QR", "RQ", "RQR" }));
                RMenu.GetSeparator("Misc Mobe");
                RMenu.Add(new MenuSlider("PreventMeMinUseR", "Prevent Me Hp Min Use R >= ", 20));
                RMenu.Add(new MenuBool("RAD", "Battle priority Use R AD Hero", true));
            }

            var FleeMenu = Menu.Add(new Menu("Flee", "Flee.Set"));
            {
                FleeMenu.GetSeparator("Flee Mobe");
                FleeMenu.Add(new MenuKeyBind("FleeKey", "Use Q R Flee Key", System.Windows.Forms.Keys.Z, KeyBindType.Press));
                FleeMenu.Add(new MenuKeyBind("FleeKey2", "Use R Flee Key", System.Windows.Forms.Keys.A, KeyBindType.Press));
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Draw"));
            {
                DrawMenu.Add(new MenuBool("Q", "Q Range"));
                DrawMenu.Add(new MenuBool("E", "E Range"));
                DrawMenu.Add(new MenuBool("R", "R Range"));
                DrawMenu.Add(new MenuBool("DrawDamage", "Draw Combo Damage", true));
            }

            Menu.Add(new MenuBool("ComboIgnite", "Combo Ignite", true));

            PlaySharp.Write(GameObjects.Player.ChampionName + "OK! :)");

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Variables.Orbwalker.OnAction += OnAction;
            Events.OnGapCloser += OnGapCloser;
            Events.OnInterruptableTarget += OnInterruptableTarget;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnAction(object sender, OrbwalkingActionArgs args)
        {

            if (args.Type == OrbwalkingType.AfterAttack)
            {
                var Target = GetTarget(Q.Range, Q.DamageType);

                if (Variables.Orbwalker.ActiveMode == OrbwalkingMode.Combo)
                {
                    if (Target != null && Menu["R"]["RAD"].GetValue<MenuBool>())
                    {
                        if (Marksman.Contains(Target.CharData.BaseSkinName) && Q.IsReady() && R.IsReady() && R.IsInRange(Target))
                        {
                            R.Cast(Target);
                            Q.Cast(Target);
                        }
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Menu["W"]["AntiMelee"].GetValue<MenuBool>() && W.IsReady())
            {
                if (sender != null && sender.IsEnemy && args.Target != null && args.Target.IsMe)
                {
                    if (sender.Type == Player.Type && sender.IsMelee)
                    {
                        W.Cast(Player);
                    }
                }
            }
        }

        private static void OnInterruptableTarget(object sender, Events.InterruptableTargetEventArgs args)
        {
            if (Menu["E"]["InterruptE"] && args.DangerLevel >= DangerLevel.High)
            {
                if (E.IsReady() && args.Sender.IsValidTarget(E.Range))
                {
                    E.Cast();
                }
            }
        }

        private static void OnGapCloser(object sender, Events.GapCloserEventArgs args)
        {
            if (args.IsDirectedToPlayer && E.IsReady() && Menu["E"]["GapcloserE"])
            {
                if (args.Sender.IsValidTarget(E.Range))
                {
                    E.Cast(args.Sender);
                }
            }
        }

        private static void OnEndScene(EventArgs args)
        {
            if (!Menu["Draw"]["DrawDamage"].GetValue<MenuBool>())
                return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(1500) && !e.IsDead && e.IsVisible))
            {
                HpBarDraw.Unit = enemy;
                HpBarDraw.DrawDmg(GetDamage(enemy), SharpDX.Color.LightCyan);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Menu["Draw"]["Q"].GetValue<MenuBool>() && Q.IsReady())
                Render.Circle.DrawCircle(Player.Position, Q.Range - 10, Color.DarkGray);

            if (Menu["Draw"]["E"].GetValue<MenuBool>() && E.IsReady())
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.BlueViolet);

            if (Menu["Draw"]["R"].GetValue<MenuBool>() && R.IsReady())
                Render.Circle.DrawCircle(Player.Position, R.Range - 20, Color.Red);
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Variables.Orbwalker.ActiveMode)
            {
                case OrbwalkingMode.Combo:
                    ComboLogic();
                    break;
                case OrbwalkingMode.Hybrid:
                    HarassLogic();
                    break;
                case OrbwalkingMode.LaneClear:
                    LaneClearLogic();
                    JungleLogic();
                    break;
                case OrbwalkingMode.LastHit:
                    break;
                case OrbwalkingMode.None:
                    FleeLogic();
                    break;
            }
        }

        private static float GetDamage(AIHeroClient target)
        {
            var Damage = 0d;

            Damage -= target.HPRegenRate;

            if (Q.IsReady())
            {
                Damage += Player.GetSpellDamage(target, SpellSlot.Q) * 2;
            }

            if (W.IsReady())
            {
                Damage += W.GetDamage(target);
            }

            if (R.IsReady())
            {
                Damage += R.GetDamage(target);
            }

            return (float)Damage;
        }

        private static double GetIgniteDamage(AIHeroClient target)
        {
            return 50 + 20 * GameObjects.Player.Level - (target.HPRegenRate / 5 * 3);
        }

        private static void ComboLogic()
        {
            var Target = GetTarget(Q.Range, Q.DamageType);
            var MinHpToDive = Menu["R"]["PreventMeMinUseR"].GetValue<MenuSlider>().Value;

            if (Target == null || !Target.IsValid)
            {
                return;
            }

            if (Menu["ComboIgnite"].GetValue<MenuBool>() && Ignite.IsReady() && Ignite != SpellSlot.Unknown)
            {
                var targetIgnite = GetTarget(IgniteRange, DamageType.True);

                if (CheckTarget(targetIgnite))
                {
                    if (targetIgnite.IsValidTarget(IgniteRange) && targetIgnite.HealthPercent < 20)
                    {
                        Player.Spellbook.CastSpell(Ignite, targetIgnite);
                        return;
                    }

                    if (GetIgniteDamage(targetIgnite) > targetIgnite.Health && targetIgnite.IsValidTarget(IgniteRange))
                    {
                        Player.Spellbook.CastSpell(Ignite, targetIgnite);
                        return;
                    }
                }
            }

            if (Menu["W"]["ComboW"].GetValue<MenuBool>() && W.IsReady() && W.IsInRange(Target))
            {
                W.Cast();
            }

            if (Menu["E"]["ComboE"].GetValue<MenuBool>() && E.IsReady() && E.IsInRange(Target))
            {
                var prediction = E.GetPrediction(Target);

                if (prediction.Hitchance >= HitChance.VeryHigh)
                {
                    E.Cast();
                }
            }

            if (Menu["Q"]["ComboQ"].GetValue<MenuBool>() && Q.IsReady() && Q.IsInRange(Target))
            {
                var prediction = Q.GetPrediction(Target);

                if (prediction.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(Target);
                }
            }

            if (Menu["R"]["ComboRMobe"].GetValue<MenuList>().Index != 3)
            {
                var UseR = Menu["R"]["ComboR"].GetValue<MenuBool>() && (!Target.IsUnderEnemyTurret() || (MinHpToDive <= Player.HealthPercent));
                var UseR2 = Menu["R"]["ComboR2"].GetValue<MenuBool>() && (!Target.IsUnderEnemyTurret() || (MinHpToDive <= Player.HealthPercent));
                var UseQ = Menu["Q"]["ComboQ"].GetValue<MenuBool>();

                if (Menu["R"]["ComboRMobe"].GetValue<MenuList>().Index == 0)
                {
                    if (UseQ && Q.IsReady() && Q.IsInRange(Target))
                    {
                        var pred = Q.GetPrediction(Target);

                        if (pred.Hitchance >= HitChance.VeryHigh)
                        {
                            Q.Cast(Target);
                        }
                    }

                    if (UseR && R.IsReady() && R.IsInRange(Target) && Target.HasBuff("dianamoonlight"))
                    {
                        R.Cast(Target);
                    }
                }
                if (Menu["R"]["ComboRMobe"].GetValue<MenuList>().Index == 1)
                {
                    if (R.IsReady() && Q.IsReady() && R.IsInRange(Target))
                    {
                        R.Cast(Target);
                        Q.Cast(Target);
                    }
                }

                if (Menu["R"]["ComboRMobe"].GetValue<MenuList>().Index == 2)
                {
                    if (R.IsReady() && Q.IsReady() && R.IsInRange(Target))
                    {
                        R.Cast(Target);
                        DelayAction.Add(250, () => Q.Cast(Target));
                    }
                    else
                    {
                        if (R.IsReady() && R.IsInRange(Target))
                            DelayAction.Add(250, () => R.Cast(Target));
                    }
                }
            }

            if (Menu["R"]["ComboR2"].GetValue<MenuBool>())
            {
                var enemy = GetEnemies(R.Range * 2).Count;
                var RLim = Menu["R"]["RLimitation"].GetValue<MenuSlider>().Value;

                if (enemy <= RLim && Menu["R"]["ComboR"].GetValue<MenuBool>() && !Q.IsReady() && R.IsReady())
                {
                    if (Target.Health < GetDamage(Target))
                    {
                        R.Cast(Target);
                    }
                }

                if (enemy <= RLim && R.IsReady())
                {
                    if (Target.Health < GetDamage(Target))
                    {
                        R.Cast(Target);
                    }
                }
            }
        }
    
        private static void HarassLogic()
        {
            var Target = GetTarget(Q.Range, Q.DamageType);
            var QMana = Menu["Q"]["HarassQMana"].GetValue<MenuSlider>().Value;
            var EMana = Menu["E"]["HarassEMana"].GetValue<MenuSlider>().Value;

            if (Target == null || !Target.IsValid)
            {
                return;
            }

            if (Player.ManaPercent < QMana + EMana)
            {
                return;
            }

            if (Menu["Q"]["HarassQ"].GetValue<MenuBool>() && Q.IsReady() && Q.IsInRange(Target))
            {
                var QPred = Q.GetPrediction(Target);

                if (QPred.Hitchance >= HitChance.VeryHigh)
                {
                    if (Menu["Q"]["QList"][Target.ChampionName.ToLower()].GetValue<MenuBool>() && !Player.IsUnderEnemyTurret())
                    {
                        Q.Cast(Target);
                    }                    
                }
            }

            if (Menu["W"]["HarassW"].GetValue<MenuBool>() && W.IsReady() && W.IsInRange(Target))
            {
                W.Cast();
            }

            if (Menu["E"]["HarassE"].GetValue<MenuBool>() && E.IsReady() && Player.Distance(Target) <= E.Range)
            {
                E.Cast();
            }
        }

        private static void LaneClearLogic()
        {
            var minion = GetMinions(Player.Position, Q.Range).FirstOrDefault();
            var minions = GetMinions(Player.Position, Q.Range);
            var QMinion = minions.FindAll(qx => minion.IsValidTarget(Q.Range));
            var QMinions = QMinion.Find(qx => qx.IsValidTarget());

            if (minion == null || minion.Name.ToLower().Contains("ward"))
            {
                return;
            }

            if (Menu["Q"]["LaneClearQ"].GetValue<MenuBool>() && Q.IsReady())
            {
                if (Player.ManaPercent >= Menu["Q"]["LaneClearQMana"].GetValue<MenuSlider>().Value)
                {
                    if (Q.GetCircularFarmLocation(minions).MinionsHit >= Menu["Q"]["LaneClearMinMinions"].GetValue<MenuSlider>().Value)
                    {
                        Q.Cast(QMinions);
                    }
                }
            }

            if (Menu["W"]["LaneClearW"].GetValue<MenuBool>() && W.IsReady())
            {
                if (Player.ManaPercent >= Menu["W"]["WMinMana"].GetValue<MenuSlider>().Value)
                {
                    if (W.GetCircularFarmLocation(minions).MinionsHit >= 3)
                    {
                        W.Cast();
                    }
                }
            }
        }

        private static void JungleLogic()
        {

            var JungleQ = GetMobs(Player.Position, Q.Range);
            var Qminions = JungleQ.FindAll(minion => minion.IsValidTarget(Q.Range));
            var Qminion = Qminions.FirstOrDefault();

            if (Qminion == null)
            {
                return;
            }

            if (Menu["Q"]["JungleQ"].GetValue<MenuBool>() && Q.IsReady())
            {
                if (Player.ManaPercent >= Menu["Q"]["JungleQMana"].GetValue<MenuSlider>().Value)
                {
                    if (Qminion.IsValidTarget())
                    {
                        Q.Cast(Qminion);
                    }
                }
            }

            if (Menu["W"]["JungleW"].GetValue<MenuBool>() && W.IsReady())
            {
                if (Player.ManaPercent >= Menu["W"]["JungleWMana"].GetValue<MenuSlider>().Value)
                {
                    W.Cast(Player);
                }
            }
        }

        private static void FleeLogic()
        {

            if (Player.IsDead)
                return;

            if (Menu["Flee"]["FleeKey2"].GetValue<MenuKeyBind>().Active)
            {
                var target = GetTarget(R.Range, R.DamageType);

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                var minions = GetMinions(Player.Position, R.Range).Find(x => x.IsValidTarget());

                if (minions != null && R.IsReady())
                {
                    R.CastOnUnit(minions);
                }
                return;
            }

            if (Menu["Flee"]["FleeKey"].GetValue<MenuKeyBind>().Active)
            {
                var target = GetTarget(Q.Range, Q.DamageType);

                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                var minions = GetMinions(Player.Position, Q.Range).Find(x => x.IsValidTarget());

                if (minions != null && Q.IsReady() && R.IsReady())
                {
                    Q.CastOnUnit(minions);
                    DelayAction.Add(500, () => R.CastOnUnit(minions));
                }
                return;
            }
        }
            
        private static string[] Marksman =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Jinx", "Kalista",
            "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Urgot", "Varus",
            "Vayne"
        };
    }
}