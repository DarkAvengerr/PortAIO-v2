using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using FontStyle = System.Drawing.FontStyle;
using FontColor = SharpDX.Color;
using Color = System.Drawing.Color;
using Flowers_Commom;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Brand
{
    class Program
    {
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        public static SpellSlot Dot;
        public static AIHeroClient Me;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Load;
        }

        private static void Load(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Brand")
                return;

            Me = ObjectManager.Player;

            {
                Q = new Spell(SpellSlot.Q, 1050) { MinHitChance = HitChance.VeryHigh };
                W = new Spell(SpellSlot.W, 900) { MinHitChance = HitChance.VeryHigh };
                E = new Spell(SpellSlot.E, 625);
                R = new Spell(SpellSlot.R, 750);
                Q.SetSkillshot(0.25f, 50f, 1600f, true, SkillshotType.SkillshotLine);
                W.SetSkillshot(1.15f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
                R.SetTargetted(0.25f, 2000f);
                Dot = Me.GetSpellSlot("SummonerDot");
            }

            {
                Menu = new Menu("Flowers - Brand", "NightMoon", true).SetFontStyle(FontStyle.Regular, FontColor.CadetBlue);

                Menu.AddSubMenu(new Menu("[FL] Orbwalking", "nightmoon.orbwalking.Menu"));
                Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.orbwalking.Menu"));

                Menu.AddSubMenu(new Menu("[FL] Combo Menu", "nightmoon.combo.Menu"));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.q", "Use Q", true).SetValue(true));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.w", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.e", "Use E", true).SetValue(true));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.r", "Use R", true).SetValue(true));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.rcount", "Min Counts In R Ranges", true).SetValue(new Slider(3, 0, 6)));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.dot", "Use Ignite", true).SetValue(true));
                Menu.SubMenu("nightmoon.combo.Menu").AddItem(new MenuItem("nightmoon.combo.qonlypas", "Use Q In Combo (Only Enemy Have Passive)", true).SetValue(new KeyBind('T', KeyBindType.Toggle, false))).Permashow();

                Menu.AddSubMenu(new Menu("[FL] Harass Menu", "nightmoon.harass.Menu"));
                Menu.SubMenu("nightmoon.harass.Menu").AddItem(new MenuItem("nightmoon.harass.q", "Use Q", true).SetValue(true));
                Menu.SubMenu("nightmoon.harass.Menu").AddItem(new MenuItem("nightmoon.harass.w", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.harass.Menu").AddItem(new MenuItem("nightmoon.harass.e", "Use E", true).SetValue(true));
                Menu.SubMenu("nightmoon.harass.Menu").AddItem(new MenuItem("nightmoon.harass.qonlypas", "Use Q In Harass (Only Enemy Have Passive)", true).SetValue(new KeyBind('U', KeyBindType.Toggle, true))).Permashow();

                Menu.AddSubMenu(new Menu("[FL] Lane Menu", "nightmoon.lc.Menu"));
                Menu.SubMenu("nightmoon.lc.Menu").AddItem(new MenuItem("nightmoon.lc.w", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.lc.Menu").AddItem(new MenuItem("nightmoon.lc.e", "Use E", true).SetValue(true));
                Menu.SubMenu("nightmoon.lc.Menu").AddItem(new MenuItem("nightmoon.lc.count", "Min Minions Counts >=", true).SetValue(new Slider(3, 0, 8)));

                Menu.AddSubMenu(new Menu("[FL] Jungle Menu", "nightmoon.jc.Menu"));
                Menu.SubMenu("nightmoon.jc.Menu").AddItem(new MenuItem("nightmoon.jc.q", "Use Q", true).SetValue(true));
                Menu.SubMenu("nightmoon.jc.Menu").AddItem(new MenuItem("nightmoon.jc.w", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.jc.Menu").AddItem(new MenuItem("nightmoon.jc.e", "Use E", true).SetValue(true));

                Menu.AddSubMenu(new Menu("[FL] KillSteal Menu", "nightmoon.ks.Menu"));
                Menu.SubMenu("nightmoon.ks.Menu").AddItem(new MenuItem("nightmoon.ks.q", "Use Q", true).SetValue(true));
                Menu.SubMenu("nightmoon.ks.Menu").AddItem(new MenuItem("nightmoon.ks.w", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.ks.Menu").AddItem(new MenuItem("nightmoon.ks.e", "Use E", true).SetValue(true));
                //Menu.SubMenu("nightmoon.ks.Menu").AddItem(new MenuItem("nightmoon.ks.r", "Use R(Smart)", true).SetTooltip("First Use Q/E and then Use R").SetValue(true));

                Menu.AddSubMenu(new Menu("[FL] Mana Control", "nightmoon.Mana.Menu"));
                Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("nightmoon.harass.mana", "Harass Mini Mana", true).SetValue(new Slider(50, 1, 100)));
                Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("nightmoon.lc.mana", "LaneClear Mini Mana", true).SetValue(new Slider(50, 1, 100)));
                Menu.SubMenu("nightmoon.Mana.Menu").AddItem(new MenuItem("nightmoon.jc.mana", "JungleClear Mini Mana", true).SetValue(new Slider(50, 1, 100)));

                Menu.AddSubMenu(new Menu("[FL] Misc Menu", "nightmoon.misc.Menu"));
                Menu.SubMenu("nightmoon.misc.Menu").AddItem(new MenuItem("nightmoon.misc.eqgap", "E+Q AntiGapcloser", true).SetValue(true));
                Menu.SubMenu("nightmoon.misc.Menu").AddItem(new MenuItem("nightmoon.misc.eqint", "E+Q Interrupter", true).SetValue(true));
                Menu.SubMenu("nightmoon.misc.Menu").AddItem(new MenuItem("nightmoon.misc.packet", "Use Packet", true).SetValue(true));
                Menu.SubMenu("nightmoon.misc.Menu").AddItem(new MenuItem("PredictionMODE", "Prediction MODE", true).SetValue(new StringList(new[] { "Common prediction", "OKTW© PREDICTION" }, 1)));
                Menu.SubMenu("nightmoon.misc.Menu").AddItem(new MenuItem("HitChance", "Hit Chance", true).SetValue(new StringList(new[] { "Very High", "High", "Medium" }, 0)));

                Menu.AddSubMenu(new Menu("[FL] Draw Menu", "nightmoon.draw.Menu"));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.enable", "Enable Drawing Circle", true).SetValue(true));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.q", "Q Range", true).SetValue(new Circle(true, Color.PowderBlue)));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.w", "W Range", true).SetValue(new Circle(true, Color.NavajoWhite)));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.e", "E Range", true).SetValue(new Circle(true, Color.Violet)));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.r", "R Range", true).SetValue(new Circle(true, Color.DarkOliveGreen)));
                Menu.SubMenu("nightmoon.draw.Menu").AddItem(new MenuItem("nightmoon.draw.damage", "Draw Combo Damage", true).SetValue(true));

                Menu.AddToMainMenu();
            }

            {
                Game.OnUpdate += Loops;
                AntiGapcloser.OnEnemyGapcloser += EQAntiGapCloerEvents;
                Interrupter2.OnInterruptableTarget += EQInterruptEnemySpells;
                Drawing.OnDraw += DrawingCirCle;
            }
        }

        public static int UseRCount => Menu.Item("nightmoon.combo.rcount", true).GetValue<Slider>().Value;
        public static bool KillStealQ => Menu.Item("nightmoon.ks.q", true).GetValue<bool>();
        public static bool KillStealW => Menu.Item("nightmoon.ks.w", true).GetValue<bool>();
        public static bool KillStealE => Menu.Item("nightmoon.ks.e", true).GetValue<bool>();
        public static bool KillStealR => Menu.Item("nightmoon.ks.r", true).GetValue<bool>();
        public static bool DrawComboDamage => Menu.Item("nightmoon.draw.damage", true).GetValue<bool>();
        public static bool ComboQ => Menu.Item("nightmoon.combo.q", true).GetValue<bool>();
        public static bool HarassQ => Menu.Item("nightmoon.harass.q", true).GetValue<bool>();
        public static bool JungleQ => Menu.Item("nightmoon.jc.q", true).GetValue<bool>();
        public static bool ComboW => Menu.Item("nightmoon.combo.w", true).GetValue<bool>();
        public static bool HarassW => Menu.Item("nightmoon.harass.w", true).GetValue<bool>();
        public static bool LaneW => Menu.Item("nightmoon.lc.w", true).GetValue<bool>();
        public static int LaneWCount => Menu.Item("nightmoon.lc.count", true).GetValue<Slider>().Value;
        public static bool JungleW => Menu.Item("nightmoon.jc.w", true).GetValue<bool>();
        public static bool ComboE => Menu.Item("nightmoon.combo.e", true).GetValue<bool>();
        public static bool HarassE => Menu.Item("nightmoon.harass.e", true).GetValue<bool>();
        public static bool LaneE => Menu.Item("nightmoon.lc.e", true).GetValue<bool>();
        public static bool JungleE => Menu.Item("nightmoon.jc.e", true).GetValue<bool>();
        public static bool ComboR => Menu.Item("nightmoon.combo.r", true).GetValue<bool>();
        public static bool ComboIgnite => Menu.Item("nightmoon.combo.dot", true).GetValue<bool>();
        public static bool ComboQOnlyPassive => Menu.Item("nightmoon.combo.qonlypas", true).GetValue<KeyBind>().Active;
        public static bool HarassQOnlyPassive => Menu.Item("nightmoon.harass.qonlypas", true).GetValue<KeyBind>().Active;
        public static int HarassMana => Menu.Item("nightmoon.harass.mana", true).GetValue<Slider>().Value;
        public static int LaneClearMana => Menu.Item("nightmoon.lc.mana", true).GetValue<Slider>().Value;
        public static int JungleClearMana => Menu.Item("nightmoon.jc.mana", true).GetValue<Slider>().Value;
        public static bool EQGapcloser => Menu.Item("nightmoon.misc.eqgap", true).GetValue<bool>();
        public static bool EQInterrupt => Menu.Item("nightmoon.misc.eqint", true).GetValue<bool>();
        public static bool UsePacket => Menu.Item("nightmoon.misc.packet", true).GetValue<bool>();
        public static bool DrawEnable => Menu.Item("nightmoon.draw.enable", true).GetValue<bool>();
        public static bool DrawQRange => Menu.Item("nightmoon.draw.q", true).GetValue<Circle>().Active;
        public static bool DrawWRange => Menu.Item("nightmoon.draw.w", true).GetValue<Circle>().Active;
        public static bool DrawERange => Menu.Item("nightmoon.draw.e", true).GetValue<Circle>().Active;
        public static bool DrawRRange => Menu.Item("nightmoon.draw.r", true).GetValue<Circle>().Active;
        public static Color DrawQColor => Menu.Item("nightmoon.draw.q", true).GetValue<Circle>().Color;
        public static Color DrawWColor => Menu.Item("nightmoon.draw.w", true).GetValue<Circle>().Color;
        public static Color DrawEColor => Menu.Item("nightmoon.draw.e", true).GetValue<Circle>().Color;
        public static Color DrawRColor => Menu.Item("nightmoon.draw.r", true).GetValue<Circle>().Color;
        public static bool HavePassive(AIHeroClient e) => e.HasBuff("Brandablaze");
        public static bool InComboMode => Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo;
        public static bool InHarassMode => Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed && Me.ManaPercent >= HarassMana;
        public static bool InLaneClearMode => Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Me.ManaPercent >= LaneClearMana;
        public static bool InJungleClearMode => Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear && Me.ManaPercent >= JungleClearMana;


        private static void Loops(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                ComboLogic();
                HarassLogic();
                LaneClearLogic();
                JungleClearLogic();
                KillStealLogic();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void ComboLogic()
        {
            try
            {
                if(InComboMode)
                {
                    var e = TargetSelector.GetTarget(W.Range - 30, TargetSelector.DamageType.Magical);

                    if (e == null && !e.IsValidTarget())
                        return;

                    {
                        if(ComboIgnite)
                            if(Dot.IsReady())
                                if(GetComboDamage(e) > e.Health - 150)
                                    Me.Spellbook.CastSpell(Dot, e);
                    }

                    {
                        if(!HavePassive(e))
                        {
                            if (Me.ServerPosition.Distance(e.ServerPosition) < E.Range)
                            {
                                if (ComboE)
                                    if (E.IsReady())
                                        E.CastOnUnit(e, UsePacket);
                            }
                            else if (Me.ServerPosition.Distance(e.ServerPosition) > E.Range && Me.ServerPosition.Distance(e.ServerPosition) < W.Range)
                            {
                                if (ComboW)
                                    if (W.IsReady())
                                        OKTWSpellCast(W, e);
                            }
                        }
                    }

                    {
                        if (e.HasBuffOfType(BuffType.Charm) || e.HasBuffOfType(BuffType.Fear) || e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Slow) || e.HasBuffOfType(BuffType.Suppression))
                            if (ComboW)
                                if (W.IsReady())
                                    OKTWSpellCast(W, e);

                        if (e.IsValidTarget(E.Range))
                            if (E.IsReady())
                                if (HavePassive(e))
                                    E.CastOnUnit(e);
                    }

                    {
                        if(HavePassive(e) && ComboQOnlyPassive)
                        {
                            if (e.IsValidTarget(Q.Range))
                                if (Q.IsReady())
                                    OKTWSpellCast(Q, e);
                        }
                        else if(!ComboQOnlyPassive)
                        {
                            if (e.IsValidTarget(Q.Range))
                                if (Q.IsReady())
                                    OKTWSpellCast(Q, e);
                        }
                    }

                    {
                        if (ComboR)
                            if (R.IsReady())
                                if (e.Health < R.GetDamage(e) || (e.HealthPercent < 50 && HavePassive(e)) || Me.CountEnemiesInRange(R.Range) >= UseRCount)
                                    R.CastOnUnit(e, UsePacket);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void HarassLogic()
        {
            try
            {
                if(InHarassMode)
                {
                    var e = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

                    if (e.IsValidTarget() && e != null)
                    {
                        {
                            if (HavePassive(e))
                                if (HarassQOnlyPassive)
                                    if (Q.IsReady())
                                        OKTWSpellCast(Q, e);
                        }

                        {
                            if (!HavePassive(e))
                            {
                                if (Me.ServerPosition.Distance(e.ServerPosition) < E.Range)
                                {
                                    if (HarassE)
                                        if (E.IsReady())
                                            E.CastOnUnit(e, UsePacket);
                                }
                                else if (Me.ServerPosition.Distance(e.ServerPosition) > E.Range && Me.ServerPosition.Distance(e.ServerPosition) < W.Range)
                                {
                                    if (HarassW)
                                        if (W.IsReady())
                                            OKTWSpellCast(W, e);
                                }
                            }
                        }

                        {
                            if (e.HasBuffOfType(BuffType.Charm) || e.HasBuffOfType(BuffType.Fear) || e.HasBuffOfType(BuffType.Stun) || e.HasBuffOfType(BuffType.Slow) || e.HasBuffOfType(BuffType.Suppression))
                                if (HarassW)
                                    if (W.IsReady())
                                        OKTWSpellCast(W, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void LaneClearLogic()
        {
            try
            {
                if (InLaneClearMode)
                {
                    var minions = MinionManager.GetMinions(Me.ServerPosition, W.Range);
                    var FarmWLoaction = W.GetCircularFarmLocation(minions, W.Width);

                    if(LaneW)
                        if (FarmWLoaction.MinionsHit >= LaneWCount)
                            if (W.IsReady())
                                W.Cast(FarmWLoaction.Position, UsePacket);

                    if(LaneE)
                        foreach (var m in MinionManager.GetMinions(Me.ServerPosition, E.Range).Where(e => e.HasBuff("Brandablaze") && e.IsValidTarget(E.Range)))
                            if (E.IsReady())
                                E.CastOnUnit(m, UsePacket);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void JungleClearLogic()
        {
            try
            {
                if(InJungleClearMode)
                {
                    var mobs = MinionManager.GetMinions(Me.ServerPosition, W.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                    if (mobs.Count > 0)
                    {
                        if (JungleW)
                            if (W.IsReady())
                                W.Cast(mobs[0].ServerPosition, UsePacket);

                        if(JungleQ)
                            if (Q.IsReady())
                                Q.Cast(mobs[0].ServerPosition, UsePacket);

                        if(JungleE)
                            if (E.IsReady())
                                E.CastOnUnit(mobs[0]);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void KillStealLogic()
        {
            try
            {
                foreach(var e in HeroManager.Enemies.Where(e => e.IsValidTarget() && !e.IsZombie && CheckTargetSureCanKill(e)))
                {
                    if (KillStealQ)
                        if (Q.IsReady())
                            if (e.IsValidTarget(Q.Range))
                                if (Me.GetSpellDamage(e, SpellSlot.Q) > e.Health)
                                    OKTWSpellCast(Q, e);

                    if (KillStealW)
                        if (W.IsReady())
                            if (e.IsValidTarget(W.Range))
                                if (Me.GetSpellDamage(e, SpellSlot.W) > e.Health)
                                    OKTWSpellCast(W, e);

                    if (KillStealE)
                        if (E.IsReady())
                            if (e.IsValidTarget(E.Range))
                                if (Me.GetSpellDamage(e, SpellSlot.E) > e.Health)
                                    E.CastOnUnit (e, UsePacket);

                    //if(KillStealR)
                    //    if (R.IsReady())
                    //        foreach(var m in MinionManager.GetMinions(Me.ServerPosition, Q.Range))
                    //        {
                    //            if (m != null)
                    //            {
                    //                if(e.ServerPosition.Distance(m.ServerPosition) < 150)
                    //                {
                    //                    Chat.Print("Return True in KillStealR if(e.ServerPosition.Distance(m.ServerPosition) < 150)");

                    //                    if (e.Health < R.GetDamage(e) - 10)
                    //                    {
                    //                        Chat.Print("Return True in KillStealR  (e.Health < R.GetDamage(e) - 10)");

                    //                        if (Q.CanCast(m))
                    //                            if (Q.Cast(m.ServerPosition))
                    //                                if (m.HasBuff("Brandablaze"))
                    //                                    R.CastOnUnit(m);
                    //                    }
                    //                    else
                    //                    {
                    //                        Chat.Print("Return False in KillStealR  (e.Health < R.GetDamage(e) - 10)");

                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    Chat.Print("Return False in KillStealR if(e.ServerPosition.Distance(m.ServerPosition) < 150)");
                    //                }
                    //            }
                    //        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static bool CheckTargetSureCanKill(Obj_AI_Base e)//wait to add more
        {
            if (e.HasBuff("Undying Rage"))
                return false;

            if (e.HasBuff("Judicator's Intervention"))//Kayle Ult (im not sure this true or not)
                return false;

            if (e.HasBuff("KindredrNoDeathBuff"))//maybe
                return false;

            return true;
        }

        private static void EQAntiGapCloerEvents(ActiveGapcloser gapcloser)
        {
            try
            {
                var e = gapcloser.Sender;

                if(EQGapcloser)
                    if(e.IsValidTarget(E.Range))
                        if (HavePassive(e))
                        {
                            if (Q.IsReady())
                                OKTWSpellCast(Q, e);
                        }
                        else if (!HavePassive(e))
                        {
                            if (E.IsReady())
                                if (E.CastOnUnit(e, UsePacket))
                                    if (Q.IsReady())
                                        OKTWSpellCast(Q, e);
                        }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void EQInterruptEnemySpells(AIHeroClient sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            try
            {
                if(EQInterrupt)
                    if(sender.IsValidTarget(E.Range))
                        if(args.DangerLevel >= Interrupter2.DangerLevel.Medium)
                        {
                            if(HavePassive(sender))
                            {
                                if (Q.IsReady())
                                    OKTWSpellCast(Q, sender);
                            }
                            else if(!HavePassive(sender))
                            {
                                if (E.IsReady())
                                    if (E.CastOnUnit(sender, UsePacket))
                                        if (Q.IsReady())
                                            OKTWSpellCast(Q, sender);
                            }
                        }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void DrawingCirCle(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                if (!DrawEnable)
                    return;

                if (DrawQRange)
                    if (Q.IsReady())
                        Render.Circle.DrawCircle(Me.Position, Q.Range, DrawQColor);

                if (DrawWRange)
                    if (W.IsReady())
                        Render.Circle.DrawCircle(Me.Position, W.Range, DrawWColor);

                if (DrawERange)
                    if (E.IsReady())
                        Render.Circle.DrawCircle(Me.Position, E.Range, DrawEColor);

                if (DrawRRange)
                    if (R.IsReady())
                        Render.Circle.DrawCircle(Me.Position, R.Range, DrawRColor);

                if (DrawComboDamage)
                    foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        HpBarDraw.Unit = e;
                        HpBarDraw.DrawDmg(GetComboDamage(e), new SharpDX.ColorBGRA(255, 204, 0, 170));
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static float GetComboDamage(AIHeroClient enemy)
        {
            double Damage = 0;

            if (Q.IsReady())
                Damage += Me.GetSpellDamage(enemy, SpellSlot.Q);

            if (W.IsReady())
                Damage += Me.GetSpellDamage(enemy, SpellSlot.W);

            if (E.IsReady())
                Damage += Me.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                Damage += Me.GetSpellDamage(enemy, SpellSlot.R);

            if (Dot.IsReady())
                Damage += Me.GetSummonerSpellDamage(enemy, LeagueSharp.Common.Damage.SummonerSpell.Ignite);

            return (float)Damage;
        }

        private static void OKTWSpellCast(Spell spells, Obj_AI_Base e)
        {
            if (Menu.Item("PredictionMODE", true).GetValue<StringList>().SelectedIndex == 1)
            {
                OktwPrediction.SkillshotType CoreType2 = OktwPrediction.SkillshotType.SkillshotLine;
                bool aoe2 = false;

                if (spells.Type == SkillshotType.SkillshotCircle)
                {
                    CoreType2 = OktwPrediction.SkillshotType.SkillshotCircle;
                    aoe2 = true;
                }

                if (spells.Width > 80 && !spells.Collision)
                    aoe2 = true;

                var predInput2 = new OktwPrediction.PredictionInput
                {
                    Aoe = aoe2,
                    Collision = spells.Collision,
                    Speed = spells.Speed,
                    Delay = spells.Delay,
                    Range = spells.Range,
                    From = Me.ServerPosition,
                    Radius = spells.Width,
                    Unit = e,
                    Type = CoreType2
                };
                var poutput2 = OktwPrediction.Prediction.GetPrediction(predInput2);

                if (spells.Speed != float.MaxValue && OktwPrediction.CollisionYasuo(Me.ServerPosition, poutput2.CastPosition))
                    return;

                if (Menu.Item("HitChance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (poutput2.Hitchance >= OktwPrediction.HitChance.VeryHigh)
                        spells.Cast(poutput2.CastPosition);
                    else if (predInput2.Aoe && poutput2.AoeTargetsHitCount > 1 && poutput2.Hitchance >= OktwPrediction.HitChance.High)
                    {
                        spells.Cast(poutput2.CastPosition);
                    }

                }
                else if (Menu.Item("HitChance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    if (poutput2.Hitchance >= OktwPrediction.HitChance.High)
                        spells.Cast(poutput2.CastPosition);

                }
                else if (Menu.Item("HitChance", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    if (poutput2.Hitchance >= OktwPrediction.HitChance.Medium)
                        spells.Cast(poutput2.CastPosition);
                }
            }
            else if (Menu.Item("PredictionMODE", true).GetValue<StringList>().SelectedIndex == 0)
            {
                if (Menu.Item("HitChance", true).GetValue<StringList>().SelectedIndex == 0)
                {
                    spells.CastIfHitchanceEquals(e, HitChance.VeryHigh);
                    return;
                }
                else if (Menu.Item("HitChance", true).GetValue<StringList>().SelectedIndex == 1)
                {
                    spells.CastIfHitchanceEquals(e, HitChance.High);
                    return;
                }
                else if (Menu.Item("HitChance ", true).GetValue<StringList>().SelectedIndex == 2)
                {
                    spells.CastIfHitchanceEquals(e, HitChance.Medium);
                    return;
                }
            }
        }
    }
}
