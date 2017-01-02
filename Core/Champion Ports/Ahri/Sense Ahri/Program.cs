using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
using SPrediction;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Sense_Ahri
{
    class Program
    {
        public static AIHeroClient Player;
        public static string championName = "Ahri";
        public static Spell Q, W, E, R;
        public static Menu Option;
        public static Orbwalking.Orbwalker orbwalker;
        public static SpellSlot Ignite = ObjectManager.Player.GetSpellSlot("summonerDot");

        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != championName) return;

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 620);
            E = new Spell(SpellSlot.E, 980);
            R = new Spell(SpellSlot.R, 475);

            Q.SetSkillshot(0.25f, 50, 1700, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.7f, W.Range, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);

            MainMenu();
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Clear();
                    JCLear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }

            if (Option.Item("FleeK").GetValue<KeyBind>().Active)
                Flee();

            if (Option_item("KillQ"))
            {
                if (!Q.IsReady()) return;
                else
                {
                    var QTarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical, true);
                    if (QTarget != null && QTarget.Health <= Q.GetDamage(QTarget))
                        Q.CastIfHitchanceEquals(QTarget, HitChance.High, true);
                }
            }

            if (Option_item("KillE"))
            {
                if (!E.IsReady()) return;
                else
                {
                    var ETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, true);
                    if (ETarget != null && ETarget.Health <= Q.GetDamage(ETarget))
                        E.CastIfHitchanceEquals(ETarget, HitChance.High, true);
                }
            }
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (!Option_item("Interrupt")) return;
            else
            {
                if (E.IsReady())
                    if (ObjectManager.Player.Distance(sender) < E.Range && args.DangerLevel >= Interrupter2.DangerLevel.Medium)
                        if (E.GetPrediction(sender).Hitchance >= HitChance.Medium)
                            E.Cast(sender);
            }
        }

        static void Harass()
        {
            if (Player.ManaPercent <= Option.Item("HMana").GetValue<Slider>().Value) return;
            else
            {
                if (Option_item("HUseE"))
                    CastE();

                if (Option_item("HUseQ"))
                    CastQ();

                if (Option_item("HUseW"))
                    CastW();
            }

        }

        static void Clear()
        {
            if (Player.ManaPercent <= Option.Item("HMana").GetValue<Slider>().Value && !Option.Item("LToggle").GetValue<KeyBind>().Active) return;
            else
            {
                var Minions = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
                if (Minions == null) return;


                if (Option_item("LUseQ"))
                {
                    MinionManager.FarmLocation farmLocation = Q.GetLineFarmLocation(Minions);

                    if (farmLocation.Position.IsValid())
                    {
                        if (farmLocation.MinionsHit >= 3)
                            Q.Cast(farmLocation.Position, true);
                    }

                }

                if (Option_item("LUseE"))
                {
                    var Minion = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.NotAlly)
                        .Where(x => x.Health < E.GetDamage(x)).OrderByDescending(x => x.MaxHealth).ThenByDescending(x => x.Distance(Player)).FirstOrDefault();
                    if (Minion != null)
                        E.Cast(Minion, true);
                }

                if (Option_item("LUseW"))
                {
                    if (Minions.Count() >= 2)
                        W.Cast();
                }
            }
        }

        static void JCLear()
        {
            if (Player.ManaPercent <= Option.Item("HMana").GetValue<Slider>().Value && !Option.Item("JToggle").GetValue<KeyBind>().Active) return;
            var JungleMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (JungleMinions.Count >= 1)
            {
                if (Option_item("JUseQ"))
                {
                    MinionManager.FarmLocation Mobs = Q.GetLineFarmLocation(JungleMinions);
                    if (Mobs.Position.IsValid())
                    {
                        if (JungleMinions.Count == 4)
                            if (Mobs.MinionsHit >= 3)
                                Q.Cast(Mobs.Position, true);

                        if (JungleMinions.Count == 3)
                            if (Mobs.MinionsHit >= 2)
                                Q.Cast(Mobs.Position, true);

                        if (JungleMinions.Count <= 2)
                            Q.Cast(Mobs.Position, true);

                        if (JungleMinions.Count == 0) return;
                    }
                }

                if (Option_item("JUseW"))
                    W.Cast();

                if (Option_item("JUseE"))
                {
                    var Mob = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral)
    .Where(x => x.Health < W.GetDamage(x)).OrderByDescending(x => x.MaxHealth).ThenByDescending(x => x.Distance(Player)).FirstOrDefault();
                    if (Mob != null)
                        E.Cast(Mob);
                }

            }
            else return;
        }

        static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical, true);
            if (!R.IsReady())
            {
                if (Option_item("CUseQ"))
                    CastQ();

                if (Option_item("CUseE"))
                    CastE();

                if (Option_item("CUseW"))
                    CastW();
            }
            else
            {
                if (Option_item("CUseW"))
                    CastW();

                if (Option.Item("CUseR").GetValue<StringList>().SelectedIndex == 1)
                    if (GetComboDamage(target) >= target.Health)
                        R.Cast(Game.CursorPos, true);

                if (Option.Item("CUseR").GetValue<StringList>().SelectedIndex == 2)
                    R.Cast(Game.CursorPos, true);

                if (Option_item("CUseQ"))
                    CastQ();

                if (Option_item("CUseE"))
                    CastE();
            }

            if (Option_item("Ignite"))
            {
                if (Ignite != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(Ignite) == SpellState.Ready)
                {
                    if (target != null)
                    {
                        if (!target.HasBuff("summonerDot") && GetComboDamage(target) >= target.Health || ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) >= target.Health)
                            Player.Spellbook.CastSpell(Ignite, target);
                    }
                }
                else return;
            }
        }

        static void CastQ()
        {
            if (!Q.IsReady()) return;
            else
            {
                var Target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical, true);
                if (Target != null)
                {
                    if (Option.Item("Prediction M").GetValue<StringList>().SelectedValue == "SPrediction")
                    {
                        Q.SPredictionCast(Target, Q.MinHitChance);
                    }

                    Q.CastIfHitchanceEquals(Target, hitChanceQ(), true);
                }
            }
        }

        static void CastW()
        {
            if (!W.IsReady()) return;
            else
            {
                var Target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical, true);
                if (Target != null)
                {
                    if (Player.IsDashing())
                        W.Cast();
                    if (!Player.IsDashing() && Player.Distance(Target) < W.Range * 0.9)
                        W.Cast();
                }

            }
        }

        static void CastE()
        {
            if (!E.IsReady()) return;
            else
            {
                var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical, true);
                if (Target != null)
                {
                    if (Target.HasBuff("BansheesVeil")) return;

                    if (Option.Item("Prediction M").GetValue<StringList>().SelectedIndex == 1)
                    {
                        E.SPredictionCast(Target, E.MinHitChance);
                    }

                    if (Option.Item("Prediction M").GetValue<StringList>().SelectedIndex == 0)
                    {
                        if (Target.CanMove && Target.Distance(Player.Position) < E.Range * 0.9)
                            E.CastIfHitchanceEquals(Target, hitChanceE(), true);

                        if (!Target.CanMove)
                            E.CastIfHitchanceEquals(Target, hitChanceE(), true);
                    }
                }
            }
        }

        static void Flee()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Option_item("FUseQ"))
            {
                if (Q.IsReady())
                    Q.Cast((Player.Position), Game.CursorPos - 400);
            }

            if (Option_item("FUseE"))
                CastE();

            if (Option_item("FUseR"))
                R.Cast(Game.CursorPos);
        }

        static int GetRCount()
        {
            var buff = Player.Buffs.FirstOrDefault(x => x.Name.Equals("AhriTumble"));
            return buff == null ? 3 : buff.Count;
        }

        static bool Option_item(string itemname)
        {
            return Option.Item(itemname).GetValue<bool>();
        }

        private static HitChance hitChanceQ()
        {
            switch (Option.Item("HitQ").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Impossible;
                case 1:
                    return HitChance.Low;
                case 2:
                    return HitChance.Medium;
                case 3:
                    return HitChance.High;
                case 4:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        private static HitChance hitChanceE()
        {
            switch (Option.Item("HitE").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    return HitChance.Impossible;
                case 1:
                    return HitChance.Low;
                case 2:
                    return HitChance.Medium;
                case 3:
                    return HitChance.High;
                case 4:
                    return HitChance.VeryHigh;
                default:
                    return HitChance.High;
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;

            if (Option_item("QRange"))
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.White, 1);

            if (Option_item("WRange"))
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.White, 1);

            if (Option_item("ERange"))
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Yellow, 1);

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (Option_item("E Target") && target != null)
                Drawing.DrawCircle(target.Position, 150, Color.Green);

            if (Option_item("RRange"))
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.White, 1);
        }

        static float GetComboDamage(AIHeroClient Enemy)
        {
            float damage = 0;

            if (Q.IsReady())
                damage += (Q.GetDamage(Enemy) * 2.0f);

            if (W.IsReady())
                damage += W.GetDamage(Enemy);
            if (E.IsReady())

                damage += E.GetDamage(Enemy);
            if (R.IsReady())

                damage += (R.GetDamage(Enemy) * GetRCount());
            if (Player.Spellbook.CanUseSpell(Player.GetSpellSlot("summonerdot")) == SpellState.Ready)
                damage += (float)Player.GetSummonerSpellDamage(Enemy, Damage.SummonerSpell.Ignite);

            if (!Player.Spellbook.IsAutoAttacking)
                damage += (float)ObjectManager.Player.GetAutoAttackDamage(Enemy, true);


            return (float)damage;
        }
        public static void Drawing_OnEndScene(EventArgs args)
        {
            if (Player.IsDead) return;
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(ene => ene.IsValidTarget() && !ene.IsZombie))
            {
            }
        }

        public static void MainMenu()
        {
            Option = new Menu("Sense Ahri", "Sense_Ahri", true).SetFontStyle(System.Drawing.FontStyle.Regular, SharpDX.Color.SkyBlue);
            Option.AddToMainMenu();

            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Option.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Option.AddSubMenu(targetSelectorMenu);

            var Prediction = new Menu("Prediction Mode", "Prediction Mode");
            {
                Prediction.AddItem(new MenuItem("Prediction M", "Prediction Mode").SetValue(new StringList(new[] { "Common", "Sprediction" }, 0)));
                Prediction.AddItem(new MenuItem("HitQ", "Q HitChace").SetValue(new StringList(new[] { "Impossible", "Low", "Medium", "High", "VeryHigh" }, 3)));
                Prediction.AddItem(new MenuItem("HitE", "E HitChace").SetValue(new StringList(new[] { "Impossible", "Low", "Medium", "High", "VeryHigh" }, 3)));
            }
            Option.AddSubMenu(Prediction);

            var Harass = new Menu("Harass", "Harass");
            {
                Harass.AddItem(new MenuItem("HUseQ", "Use Q").SetValue(true));
                Harass.AddItem(new MenuItem("HUseW", "Use W").SetValue(false));
                Harass.AddItem(new MenuItem("HUseE", "Use E").SetValue(false));
                Harass.AddItem(new MenuItem("HMana", "Mana (%)").SetValue(new Slider(40)));
            }
            Option.AddSubMenu(Harass);

            var Clear = new Menu("Lane Clear", "Lane Clear");
            {
                Clear.AddItem(new MenuItem("LUseQ", "Use Q").SetValue(true));
                Clear.AddItem(new MenuItem("LUseW", "Use W").SetValue(false));
                Clear.AddItem(new MenuItem("LUseE", "Use E").SetValue(false));
                Clear.AddItem(new MenuItem("LMana", "Mana (%)").SetValue(new Slider(40)));
                Clear.AddItem(new MenuItem("LToggle", "Lane Clear Toggle").SetValue(new KeyBind('L', KeyBindType.Toggle)));
            }
            Option.AddSubMenu(Clear);

            var JClear = new Menu("Jungle Clear", "Jungle Clear");
            {
                JClear.AddItem(new MenuItem("JUseQ", "Use Q").SetValue(true));
                JClear.AddItem(new MenuItem("JUseW", "Use W").SetValue(true));
                JClear.AddItem(new MenuItem("JUseE", "Use E").SetValue(false));
                JClear.AddItem(new MenuItem("JMana", "Mana (%)").SetValue(new Slider(40)));
                JClear.AddItem(new MenuItem("JToggle", "Jungle Clear Toggle").SetValue(new KeyBind('J', KeyBindType.Toggle)));
            }
            Option.AddSubMenu(JClear);

            var Combo = new Menu("Combo", "Combo");
            {
                Combo.AddItem(new MenuItem("CUseQ", "Use Q").SetValue(true));
                Combo.AddItem(new MenuItem("CUseW", "Use W").SetValue(true));
                Combo.AddItem(new MenuItem("CUseE", "Use E").SetValue(true));
                Combo.AddItem(new MenuItem("CUseR", "Use R").SetValue(new StringList(new[] { "Never", "Kill", "Always" }, 1)));
                Combo.AddItem(new MenuItem("Ignite", "Use Ignite").SetValue(true));
            }
            Option.AddSubMenu(Combo);

            var Misc = new Menu("Misc", "Misc");
            {
                Misc.AddItem(new MenuItem("Interrupt", "Auto Use E to interrupt").SetValue(true));
                Misc.SubMenu("Flee").AddItem(new MenuItem("FUseE", "Use E").SetValue(true));
                Misc.SubMenu("Flee").AddItem(new MenuItem("FUseR", "Use R").SetValue(false));
                Misc.SubMenu("Flee").AddItem(new MenuItem("FleeK", "Flee Key").SetValue(new KeyBind('G', KeyBindType.Press)));
                Misc.SubMenu("KillSteal").AddItem(new MenuItem("KillQ", "Use Q").SetValue(true));
                Misc.SubMenu("KillSteal").AddItem(new MenuItem("KillE", "Use E").SetValue(true));
            }
            Option.AddSubMenu(Misc);

            var Drawing = new Menu("Drawing", "Drawing");
            {
                Drawing.AddItem(new MenuItem("QRange", "Q Range").SetValue(false));
                Drawing.AddItem(new MenuItem("WRange", "W Range").SetValue(false));
                Drawing.AddItem(new MenuItem("ERange", "E Range").SetValue(true));
                Drawing.AddItem(new MenuItem("E Target", "E Target").SetValue(true));
                Drawing.AddItem(new MenuItem("RRange", "R Range").SetValue(false));
                Drawing.AddItem(new MenuItem("DamageAfterCombo", "Draw Combo Damage").SetValue(true));
            }
            Option.AddSubMenu(Drawing);
        }
    }
}
