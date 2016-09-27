using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SPrediction;

using Color = System.Drawing.Color;


using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sBrand
{
    class Program
    {
        private static readonly AIHeroClient player = ObjectManager.Player;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker Orbwalker;
        private static SpellSlot Ignite;
        private static Menu menu;
        public static void Main()
        {
            Game_OnGameLoad();
        }

        static void Game_OnGameLoad()
        {
            if (player.ChampionName != "Brand")
                return;
            Chat.Print("sBrand loaded!");
            Q = new Spell(SpellSlot.Q, 1050f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 625f);
            R = new Spell(SpellSlot.R, 750f);
            Q.SetSkillshot(0.25f, 60, 1600, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.85f, 240, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Ignite = player.GetSpellSlot("summonerdot");

            menu = new Menu("sBrand", "sBrand", true);

            SPrediction.Prediction.Initialize(menu);
            //Orbwalker
            menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalker"));
            //Target selector
            var tsMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(tsMenu);
            menu.AddSubMenu(tsMenu);
            //Combo Menu
            menu.AddSubMenu(new Menu("Combo", "Combo"));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.UseQ", "Use Q").SetValue(true));
           // menu.SubMenu("Combo").AddItem(new MenuItem("Combo.QOrdinal", "Use Q after W, E").SetValue(false));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.UseW", "Use W").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.UseE", "Use E").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.UseR", "Use R").SetValue(true));
            menu.SubMenu("Combo").AddItem(new MenuItem("Combo.UseIgnite", "Use Ignite").SetValue(false));
            //Harass Menu
            menu.AddSubMenu(new Menu("Harass", "Harass"));
            menu.SubMenu("Harass").AddItem(new MenuItem("Harass.UseQ", "Use Q").SetValue(true));
            //menu.SubMenu("Harass").AddItem(new MenuItem("Harass.QOrdinal", "Use Q after W, E").SetValue(false));
            menu.SubMenu("Harass").AddItem(new MenuItem("Harass.UseW", "Use W").SetValue(true));
            menu.SubMenu("Harass").AddItem(new MenuItem("Harass.UseE", "Use E").SetValue(true));
            //Hit chance
            menu.AddSubMenu(new Menu("Hit Chance settings", "HitChance"));
            menu.SubMenu("HitChance").AddItem(new MenuItem("HitChance.Q", "Q Hit chance").SetValue(new Slider(3, 1, 4)));
            menu.SubMenu("HitChance").AddItem(new MenuItem("HitChance.W", "E Hit chance").SetValue(new Slider(3, 1, 4)));
            //Combat Modes.Thanks YLiCiOUS !
            menu.AddSubMenu(new Menu("Combat Modes", "CombatMode"));
            menu.SubMenu("CombatMode").AddItem(new MenuItem("CombatMode.Mode", "Mode").SetValue(new StringList(new[] { "Q+E+W+R", "R+E+W+Q" })));
            menu.SubMenu("CombatMode").AddItem(new MenuItem("CombatMode.Key", "Key").SetValue(new KeyBind('T', KeyBindType.Press)));
            //KS Menu
            menu.AddSubMenu(new Menu("Kill Steal", "KillSteal"));
            menu.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal.Q", "Use Q").SetValue(true));
            menu.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal.W", "Use W").SetValue(true));
            menu.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal.E", "Use E").SetValue(true));
            menu.SubMenu("KillSteal").AddItem(new MenuItem("KillSteal.Ignite", "Use Ignite").SetValue(false));
            //Farm menu
            menu.AddSubMenu(new Menu("Farming", "Farm"));
            menu.SubMenu("Farm").AddItem(new MenuItem("Farm.Q", "Use Q").SetValue(true));
            menu.SubMenu("Farm").AddItem(new MenuItem("Farm.W", "Use W").SetValue(true));
            menu.SubMenu("Farm").AddItem(new MenuItem("Farm.E", "Use E").SetValue(true));
            //LC
            menu.AddSubMenu(new Menu("Lane Clean", "LaneClean"));
            menu.SubMenu("LaneClean").AddItem(new MenuItem("LaneClean.Q", "Use Q").SetValue(true));
            menu.SubMenu("LaneClean").AddItem(new MenuItem("LaneClean.W", "Use W").SetValue(true));
            menu.SubMenu("LaneClean").AddItem(new MenuItem("LaneClean.E", "Use E").SetValue(true));
            //Mana Manager
            menu.AddSubMenu(new Menu("Mana Manager", "ManaManager"));
            menu.SubMenu("ManaManager").AddItem(new MenuItem("ManaManager.Value", "Use harass, laneclean, farm, combo if mana >= ").SetValue(new Slider(20, 0, 100)));
            menu.SubMenu("ManaManager").AddItem(new MenuItem("ManaManager.Combo", "Apply with combo").SetValue(false));
            menu.SubMenu("ManaManager").AddItem(new MenuItem("ManaManager.Harass", "Apply with harass").SetValue(true));
            menu.SubMenu("ManaManager").AddItem(new MenuItem("ManaManager.Farm", "Apply with farm").SetValue(true));
            menu.SubMenu("ManaManager").AddItem(new MenuItem("ManaManager.LaneClean", "Apply with laneclean").SetValue(true));
            //Auto lever
            //menu.AddSubMenu(new Menu("Auto Level up", "AutoLeveler"));
            //menu.SubMenu("AutoLeveler").AddItem(new MenuItem("AutoLeveler.Enable", "Enable").SetValue(true));
            //Gap closer
            menu.AddSubMenu(new Menu("Gap Closer", "sBrand.GapCloser"));
            menu.SubMenu("GapCloser").AddItem(new MenuItem("GapCloser.Enable", "Auto stun on gap closer (E+Q)").SetValue(true));
            //Interrupts
            menu.AddSubMenu(new Menu("Interrupts", "sBrand.Interrupts"));
            menu.SubMenu("Interrupts").AddItem(new MenuItem("Interrupts.Enable", "Interrupts spell with E+Q").SetValue(true));
            //Drawing
            menu.AddSubMenu(new Menu("Drawing", "sBrand.Drawing"));
            menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing.Q", "Draw Q Range").SetValue(true));
            menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing.W", "Draw W Range").SetValue(true));
            menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing.E", "Draw E Range").SetValue(true));
            menu.SubMenu("Drawing").AddItem(new MenuItem("Drawing.R", "Draw R Range").SetValue(true));
            //Draw Damage
            menu.AddSubMenu(new Menu("Draw Damage", "DrawDamage"));
            menu.SubMenu("DrawDamage").AddItem(new MenuItem("DrawDamage.Enable", "Enable").SetValue(true));
            menu.SubMenu("DrawDamage").AddItem(new MenuItem("DrawDamage.DrawColor", "Fill color").SetValue(new Circle(true, Color.FromArgb(204, 255, 0, 1))));

            menu.AddToMainMenu();

            DrawDamage.DamageToUnit = GetComboDamage;
            DrawDamage.Enabled = menu.Item("DrawDamage.Enable").GetValue<bool>();
            DrawDamage.Fill = menu.Item("DrawDamage.DrawColor").GetValue<Circle>().Active;
            DrawDamage.FillColor = menu.Item("DrawDamage.DrawColor").GetValue<Circle>().Color;

            menu.Item("DrawDamage.Enable").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
            };

            menu.Item("DrawDamage.DrawColor").ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            Game.OnUpdate += Game_OnUpdate;
        }
        static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo: Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed: Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear: LaneClean();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit: Farm();
                    break;
            }

            if (menu.Item("CombatMode.Key").GetValue<KeyBind>().Active)
                Combat();

            KillSteal();
        }

        static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var enable = menu.Item("Interrupts.Enable").GetValue<bool>();
            if (enable)
            {
                if (E.IsReady() && E.IsInRange(sender))
                    E.CastOnUnit(sender);
                if (Q.IsReady() && Q.IsInRange(sender) && sender.HasBuff("brandablaze"))
                    Q.CastIfHitchanceEquals(sender, HitChance.High);
            }
        }

        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly)
                return;

            var enable = menu.Item("GapCloser.Enable").GetValue<bool>();
            if (enable)
            {
                if (E.IsReady() && E.IsInRange(gapcloser.Sender))
                    E.CastOnUnit(gapcloser.Sender);
                if (Q.IsReady() && Q.IsInRange(gapcloser.Sender) && gapcloser.Sender.HasBuff("brandablaze"))
                    Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High);

            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = menu.Item("Drawing.Q").GetValue<bool>();
            var drawW = menu.Item("Drawing.W").GetValue<bool>();
            var drawE = menu.Item("Drawing.E").GetValue<bool>();
            var drawR = menu.Item("Drawing.R").GetValue<bool>();

            if (drawQ)
                Drawing.DrawCircle(player.Position, Q.Range, System.Drawing.Color.Blue);

            if (drawW)
                Drawing.DrawCircle(player.Position, W.Range, System.Drawing.Color.Green);

            if (drawE)
                Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.BlueViolet);

            if (drawR)
                Drawing.DrawCircle(player.Position, R.Range, System.Drawing.Color.Red);
        }


        static void Combo()
        {
            var manaCombo = menu.Item("ManaManager.Combo").GetValue<bool>();
            var manapercent = menu.Item("ManaManager.Value").GetValue<Slider>().Value;
            if (manaCombo && ((int)player.ManaPercent) < manapercent)
                return;

            var useQ = menu.Item("Combo.UseQ").GetValue<bool>();
            var useW = menu.Item("Combo.UseW").GetValue<bool>();
            var useE = menu.Item("Combo.UseE").GetValue<bool>();
            var useR = menu.Item("Combo.UseR").GetValue<bool>();
            //var QOrdinal = menu.Item("Combo.QOrdinal").GetValue<bool>();
            var Qhitchance = menu.Item("HitChance.Q").GetValue<Slider>().Value;
            var Whitchance = menu.Item("HitChance.W").GetValue<Slider>().Value;
            var useIgnite = menu.Item("Combo.UseIgnite").GetValue<bool>();
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (player.Distance(target.Position) < E.Range)
                {
                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                        CastE(target);

                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        if (target.HasBuff("brandablaze"))
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                        else
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                    }

                    if (W.IsReady() && useW && target.IsValidTarget(W.Range))
                    {
                        if (Whitchance == 1)
                            W.SPredictionCast(target, HitChance.Low);
                        if (Whitchance == 2)
                            W.SPredictionCast(target, HitChance.Medium);
                        if (Whitchance == 3)
                            W.SPredictionCast(target, HitChance.High);
                        if (Whitchance == 4)
                            W.SPredictionCast(target, HitChance.VeryHigh);
                    }

                    

                    if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(1600, () => CastR(target));
                    }

                    if (useIgnite && Ignite.IsReady() && !R.IsReady())
                        player.Spellbook.CastSpell(Ignite, target);
                }

                else
                {
                    if (W.IsReady() && useW && target.IsValidTarget(W.Range))
                    {
                        if (Whitchance == 1)
                            W.SPredictionCast(target, HitChance.Low);
                        if (Whitchance == 2)
                            W.SPredictionCast(target, HitChance.Medium);
                        if (Whitchance == 3)
                            W.SPredictionCast(target, HitChance.High);
                        if (Whitchance == 4)
                            W.SPredictionCast(target, HitChance.VeryHigh);
                    }

                    if ( useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        if (target.HasBuff("brandablaze"))
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                        else
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                    }

                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        if (!Q.IsReady())
                            CastE(target);
                        else
                            CastE(target);
                    }

                    

                    if (useR && R.IsReady() && target.IsValidTarget(R.Range))
                    {
                        if (!E.IsReady())
                            LeagueSharp.Common.Utility.DelayAction.Add(1600, () => CastR(target));
                        else
                            LeagueSharp.Common.Utility.DelayAction.Add(1600, () => CastR(target));
                    }

                    if (useIgnite && Ignite.IsReady() && !R.IsReady())
                        player.Spellbook.CastSpell(Ignite, target);
                }
            }


        }
        static void Harass()
        {
            var manaHarass = menu.Item("ManaManager.Harass").GetValue<bool>();
            var manapercent = menu.Item("ManaManager.Value").GetValue<Slider>().Value;
            if (manaHarass && ((int)player.ManaPercent) < manapercent)
                return;

            var useQ = menu.Item("Harass.UseQ").GetValue<bool>();
            var useW = menu.Item("Harass.UseW").GetValue<bool>();
            var useE = menu.Item("Harass.UseE").GetValue<bool>();
            //var QOrdinal = menu.Item("Harass.QOrdinal").GetValue<bool>();
            var Qhitchance = menu.Item("HitChance.Q").GetValue<Slider>().Value;
            var Whitchance = menu.Item("HitChance.W").GetValue<Slider>().Value;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (player.Distance(target.Position) < E.Range)
                {
                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                        CastE(target);

                    if ( useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        if (target.HasBuff("brandablaze"))
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                        else
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                    }

                    if (W.IsReady() && useW && target.IsValidTarget(W.Range))
                    {
                        if (Whitchance == 1)
                            W.SPredictionCast(target, HitChance.Low);
                        if (Whitchance == 2)
                            W.SPredictionCast(target, HitChance.Medium);
                        if (Whitchance == 3)
                            W.SPredictionCast(target, HitChance.High);
                        if (Whitchance == 4)
                            W.SPredictionCast(target, HitChance.VeryHigh);
                    }

                    
                }

                else
                {
                    if (W.IsReady() && useW && target.IsValidTarget(W.Range))
                    {
                        if (Whitchance == 1)
                            W.SPredictionCast(target, HitChance.Low);
                        if (Whitchance == 2)
                            W.SPredictionCast(target, HitChance.Medium);
                        if (Whitchance == 3)
                            W.SPredictionCast(target, HitChance.High);
                        if (Whitchance == 4)
                            W.SPredictionCast(target, HitChance.VeryHigh);
                    }

                    if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                    {
                        if (target.HasBuff("brandablaze"))
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                        else
                        {
                            if (Qhitchance == 1)
                                Q.SPredictionCast(target, HitChance.Low);
                            if (Qhitchance == 2)
                                Q.SPredictionCast(target, HitChance.Medium);
                            if (Qhitchance == 3)
                                Q.SPredictionCast(target, HitChance.High);
                            if (Qhitchance == 4)
                                Q.SPredictionCast(target, HitChance.VeryHigh);
                        }
                    }

                    if (useE && E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        if (!Q.IsReady())
                            CastE(target);
                        else
                            CastE(target);
                    }

                    
                }
            }
        }
        static void Farm()
        {
            var manaFarm = menu.Item("ManaManager.Farm").GetValue<bool>();
            var manapercent = menu.Item("ManaManager.Value").GetValue<Slider>().Value;
            if (manaFarm && ((int)player.ManaPercent) < manapercent)
                return;

            var useQ = menu.Item("Farm.Q").GetValue<bool>();
            var useW = menu.Item("Farm.W").GetValue<bool>();
            var useE = menu.Item("Farm.E").GetValue<bool>();
            var minions = MinionManager.GetMinions(E.Range);

            foreach (var minion in minions)
            {
                var Qdmg = Q.GetDamage(minion) * 0.9;
                var Wdmg = W.GetDamage(minion) * 0.9;
                var Edmg = E.GetDamage(minion) * 0.9;


                if (E.IsReady() && minion.Health < Edmg && useE)
                {
                    E.CastOnUnit(minion);
                }


                if (W.IsReady() && minion.Health < Wdmg && useW)
                {
                    W.Cast(minion);
                }


                if (Q.IsReady() && minion.Health < Qdmg && useQ)
                {
                    Q.Cast(minion);
                }
            }
        }
        static void LaneClean()
        {
            var manaLaneClean = menu.Item("ManaManager.LaneClean").GetValue<bool>();
            var manapercent = menu.Item("ManaManager.Value").GetValue<Slider>().Value;
            if (manaLaneClean && ((int)player.ManaPercent) < manapercent)
                return;

            foreach (var minion in ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsEnemy && m.IsValidTarget(E.Range) && !m.IsDead && !m.IsInvulnerable))
            {
                var useQ = menu.Item("LaneClean.Q").GetValue<bool>();
                var useW = menu.Item("LaneClean.W").GetValue<bool>();
                var useE = menu.Item("LaneClean.E").GetValue<bool>();

                var Qdmg = Q.GetDamage(minion) * 0.9;
                var Wdmg = W.GetDamage(minion) * 0.9;
                var Edmg = E.GetDamage(minion) * 0.9;

                if (Q.IsReady() && minion.Health < Qdmg && useQ)
                {
                    Q.Cast(minion);
                }

                if (W.IsReady() && minion.Health < Wdmg && useW)
                {
                    W.Cast(minion);
                }

                if (E.IsReady() && minion.Health < Edmg && useE)
                {
                    E.CastOnUnit(minion);
                }
            }
        }
        static void KillSteal()
        {
            var useQ = menu.Item("KillSteal.Q").GetValue<bool>();
            var useW = menu.Item("KillSteal.W").GetValue<bool>();
            var useE = menu.Item("KillSteal.E").GetValue<bool>();
            var useIg = menu.Item("KillSteal.Ignite").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => target.IsValidTarget(E.Range) && !target.IsInvulnerable && target.IsEnemy))
            {
                var Qdmg = Q.GetDamage(target) * 0.9;
                var Wdmg = W.GetDamage(target) * 0.9;
                var Edmg = E.GetDamage(target) * 0.9;
                float Ignitedmg;
                if (Ignite != SpellSlot.Unknown)
                    Ignitedmg = (float)player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
                else
                    Ignitedmg = 0f;

                if (useIg && Ignite.IsReady() && target.Health < Ignitedmg)
                {
                    player.Spellbook.CastSpell(Ignite, target);
                }

                if (useQ && !useW && !useE && Q.IsReady() && target.Health < Qdmg)
                {
                    Q.Cast(target);
                }

                if (!useQ && !useE && useW && W.IsReady() && target.Health < Wdmg)
                {
                    W.Cast(target);
                }

                if (!useQ && !useW && useE && E.IsReady() && target.Health < Edmg)
                {
                    CastE(target);
                }
            }
        }
        static float GetComboDamage(AIHeroClient target)
        {
            var useQ = menu.Item("Combo.UseQ").GetValue<bool>();
            var useW = menu.Item("Combo.UseW").GetValue<bool>();
            var useE = menu.Item("Combo.UseE").GetValue<bool>();
            var useR = menu.Item("Combo.UseR").GetValue<bool>();
            double combodmg = 0;

            if (Q.IsInRange(target) && useQ && Q.IsReady())
                combodmg += Q.GetDamage(target);

            if (W.IsInRange(target) && useW && W.IsReady())
                combodmg += W.GetDamage(target);

            if (E.IsInRange(target) && useE && E.IsReady())
                combodmg += E.GetDamage(target);

            if (R.IsInRange(target) && useR && R.IsReady())
                combodmg += R.GetDamage(target);

            return (float)combodmg;
        }
        static void Combat()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var mode = menu.Item("CombatMode.Mode").GetValue<StringList>().SelectedIndex;
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            var Qhitchance = menu.Item("HitChance.Q").GetValue<Slider>().Value;
            var Whitchance = menu.Item("HitChance.W").GetValue<Slider>().Value;

            if (target != null)
            {
                switch (mode)
                {
                    case 0:
                        {
                            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                            {
                                if (target.HasBuff("brandablaze"))
                                {
                                    if (Qhitchance == 1)
                                        Q.SPredictionCast(target, HitChance.Low);
                                    if (Qhitchance == 2)
                                        Q.SPredictionCast(target, HitChance.Medium);
                                    if (Qhitchance == 3)
                                        Q.SPredictionCast(target, HitChance.High);
                                    if (Qhitchance == 4)
                                        Q.SPredictionCast(target, HitChance.VeryHigh);
                                }
                                else
                                {
                                    if (Qhitchance == 1)
                                        Q.SPredictionCast(target, HitChance.Low);
                                    if (Qhitchance == 2)
                                        Q.SPredictionCast(target, HitChance.Medium);
                                    if (Qhitchance == 3)
                                        Q.SPredictionCast(target, HitChance.High);
                                    if (Qhitchance == 4)
                                        Q.SPredictionCast(target, HitChance.VeryHigh);
                                }
                            }

                            if (E.IsReady() && target.HasBuff("brandablaze") && target.IsValidTarget(E.Range) && !Q.IsReady())
                                E.CastOnUnit(target);

                            if (W.IsReady() && target.IsValidTarget(W.Range) && target.HasBuff("brandablaze") && !E.IsReady())
                            {
                                if (Whitchance == 1)
                                    W.SPredictionCast(target, HitChance.Low);
                                if (Whitchance == 2)
                                    W.SPredictionCast(target, HitChance.Medium);
                                if (Whitchance == 3)
                                    W.SPredictionCast(target, HitChance.High);
                                if (Whitchance == 4)
                                    W.SPredictionCast(target, HitChance.VeryHigh);
                            }

                            if (R.IsReady() && target.IsValidTarget(R.Range) && target.HasBuff("brandablaze") && !W.IsReady())
                                R.Cast(target);
                        }
                        break;

                    case 1:
                        {
                            if (R.IsReady() && target.IsValidTarget(R.Range))
                                R.Cast(target);

                            if (E.IsReady() && target.HasBuff("brandablaze") && target.IsValidTarget(E.Range) && !R.IsReady())
                                E.CastOnUnit(target);

                            if (W.IsReady() && target.IsValidTarget(W.Range) && target.HasBuff("brandablaze") && !E.IsReady())
                            {
                                if (Whitchance == 1)
                                    W.SPredictionCast(target, HitChance.Low);
                                if (Whitchance == 2)
                                    W.SPredictionCast(target, HitChance.Medium);
                                if (Whitchance == 3)
                                    W.SPredictionCast(target, HitChance.High);
                                if (Whitchance == 4)
                                    W.SPredictionCast(target, HitChance.VeryHigh);
                            }

                            if (Q.IsReady() && target.IsValidTarget(Q.Range) && target.HasBuff("brandablaze") && !W.IsReady())
                            {
                                if (target.HasBuff("brandablaze"))
                                {
                                    if (Qhitchance == 1)
                                        Q.SPredictionCast(target, HitChance.Low);
                                    if (Qhitchance == 2)
                                        Q.SPredictionCast(target, HitChance.Medium);
                                    if (Qhitchance == 3)
                                        Q.SPredictionCast(target, HitChance.High);
                                    if (Qhitchance == 4)
                                        Q.SPredictionCast(target, HitChance.VeryHigh);
                                }
                                else
                                {
                                    if (Qhitchance == 1)
                                        Q.SPredictionCast(target, HitChance.Low);
                                    if (Qhitchance == 2)
                                        Q.SPredictionCast(target, HitChance.Medium);
                                    if (Qhitchance == 3)
                                        Q.SPredictionCast(target, HitChance.High);
                                    if (Qhitchance == 4)
                                        Q.SPredictionCast(target, HitChance.VeryHigh);
                                }
                            }
                        }
                        break;
                }
            }
        }

        static void CastE(AIHeroClient target)
        {
            if (E.IsInRange(target))
                E.CastOnUnit(target);
            else
            {
                foreach (var forcus in HeroManager.Enemies)
                {
                    if (forcus.HasBuff("brandablaze") && forcus.IsValidTarget(E.Range) && forcus.Distance(target.Position) <= 500)
                        E.CastOnUnit(forcus);
                }

                foreach (var minion in MinionManager.GetMinions(E.Range))
                {
                    if (minion.HasBuff("brandablaze") && minion.IsValidTarget(E.Range) && minion.Distance(target.Position) <= 500)
                        E.CastOnUnit(minion);
                }
            }
        }
        static void CastR(AIHeroClient target)
        {
            if (R.IsInRange(target))
                R.CastOnUnit(target);
            else
            {
                foreach (var forcus in HeroManager.Enemies)
                {
                    if (forcus.HasBuff("brandablaze") && forcus.IsValidTarget(R.Range) && forcus.Distance(target.Position) <= 500)
                        R.CastOnUnit(forcus);
                }

                foreach (var minion in MinionManager.GetMinions(R.Range))
                {
                    if (minion.HasBuff("brandablaze") && minion.IsValidTarget(R.Range) && minion.Distance(target.Position) <= 500)
                        R.CastOnUnit(minion);
                }
            }
        }

    }
}
