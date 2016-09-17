using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Flowers_Twitch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LeagueSharp;
    using LeagueSharp.Common;
    using Color = System.Drawing.Color;
    using FontStyle = System.Drawing.FontStyle;
    using SharpDX;
    using SColor = SharpDX.Color;
    using Flowers_Commom;

    class Program
    {
        public static Spell Q, W, E, R;
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        public static AIHeroClient Me;
        public static bool PlayerIsKillTarget;
        public static float LastFocusTime;
        public static HpBarDraw HpBarDraw = new HpBarDraw();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.ChampionName != "Twitch")
                    return;

                Me = ObjectManager.Player;

                LoadSpells();
                LoadMenu();
                LoadEvents();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OnGameLoad " + ex); ;
            }
        }

        private static void LoadEvents()
        {
            try
            {
                Drawing.OnDraw += OnDraw;
                Game.OnUpdate += OnUpdate;
                Game.OnNotify += OnNotify;
                Obj_AI_Base.OnProcessSpellCast += AboutKSQ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Events Loading " + ex);
            }
        }

        private static void AboutKSQ(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (sender.IsMe)
                    if (Menu.Item("IfKillSomeoneAutoQ", true).GetValue<bool>())
                        if (PlayerIsKillTarget == true)
                            if (Q.IsReady())
                                if (Me.CountEnemiesInRange(1000) >= 1)
                                    Q.Cast(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in AboutKSQ " + ex);
            }
        }

        private static void OnNotify(GameNotifyEventArgs args)
        {
            try
            {
                if (Me.IsDead)
                {
                    PlayerIsKillTarget = false;
                }
                else if (!Me.IsDead)
                {
                    if (args.EventId == GameEventId.OnChampionDie)
                        if (args.NetworkId == Me.NetworkId)
                        {
                            PlayerIsKillTarget = true;
                            LeagueSharp.Common.Utility.DelayAction.Add(8000, () => { PlayerIsKillTarget = false;});
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OnNotify " + ex);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            try
            {
                if (Me.IsDead)
                    return;

                if (!Menu.Item("DrawEnable", true).GetValue<bool>())
                    return;

                if (Menu.Item("WRange", true).GetValue<Circle>().Active && W.IsReady())
                    Render.Circle.DrawCircle(Me.Position, W.Range, Menu.Item("WRange", true).GetValue<Circle>().Color);

                if (Menu.Item("ERange", true).GetValue<Circle>().Active && E.IsReady())
                    Render.Circle.DrawCircle(Me.Position, E.Range, Menu.Item("ERange", true).GetValue<Circle>().Color);

                if (Menu.Item("RRange", true).GetValue<Circle>().Active && R.IsReady())
                    Render.Circle.DrawCircle(Me.Position, R.Range, Menu.Item("RRange", true).GetValue<Circle>().Color);

                if(Menu.Item("DrawComboDamage", true).GetValue<bool>())
                    foreach(var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget() && !e.IsDead && !e.IsZombie))
                    {
                        double damage = 0;

                        if (W.IsReady())
                            damage += Me.GetSpellDamage(e, SpellSlot.W);

                        if (E.IsReady())
                            damage += Me.GetSpellDamage(e, SpellSlot.E);

                        HpBarDraw.Unit = e;
                        HpBarDraw.DrawDmg((float)damage, new ColorBGRA(255, 204, 0, 170));
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OnDraw " + ex);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            try
            {
                switch (Orbwalker.ActiveMode)
                {
                    case Orbwalking.OrbwalkingMode.Combo:
                        ComboLogic();
                        break;
                    case Orbwalking.OrbwalkingMode.Mixed:
                        HarassLogic();
                        break;
                }

                CleanLogic();
                KillStealLogic();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OnUpdate " + ex);
            }
        }

        private static void KillStealLogic()
        {
            try
            {
                if (E.IsReady())
                    if (Menu.Item("KillStealE", true).GetValue<bool>())
                        foreach (var e in ObjectManager.Get<AIHeroClient>().Where(e => e.IsValidTarget(E.Range)))
                            if (Me.GetSpellDamage(e, SpellSlot.E) > e.Health + 5)
                                E.Cast();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in KillSteal Logic " + ex);
            }
        }

        private static void CleanLogic()
        {
            try
            {
                if (Menu.Item("JungleStealE", true).GetValue<bool>())
                    if ((Menu.Item("OnlyInCleanMode", true).GetValue<bool>() && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear) || !Menu.Item("OnlyInCleanMode", true).GetValue<bool>())
                    {
                        var mob = MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                        foreach (var m in mob)
                        {
                            if ((m.CharData.BaseSkinName.Contains("Dragon") || m.CharData.BaseSkinName.Contains("Baron")))
                                if (E.IsKillable(m))
                                    E.Cast();
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Clean Logic " + ex);
            }
        }

        private static void HarassLogic()
        {
            try
            {
                if (Menu.Item("HarassW", true).GetValue<bool>())
                {
                    if (Me.ManaPercent > Menu.Item("HarassWMana", true).GetValue<Slider>().Value)
                    {
                        var WTarget = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);

                        if (WTarget != null)
                            if (W.IsReady())
                                if (W.CanCast(WTarget))
                                    W.Cast(WTarget, true);
                    }
                }

                if (Menu.Item("HarassE", true).GetValue<bool>())
                {
                    if (Me.ManaPercent > Menu.Item("HarassEMana", true).GetValue<Slider>().Value)
                    {
                        var ETarget = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                        if (E.IsReady())
                        {
                            if (Me.Distance(ETarget) > E.Range * 0.8 && ETarget.IsValidTarget(E.Range) && GetEStackCount(ETarget) >= Menu.Item("HarassEStack", true).GetValue<Slider>().Value)
                            {
                                E.Cast();
                            }
                            else if (GetEStackCount(ETarget) >= 6)
                            {
                                E.Cast();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Harass Logic " + ex);
            }
        }


        private static void ComboLogic()
        {
            try
            {
                var e = TargetSelector.GetTarget(E.Range, E.DamageType);

                //Chat.Print("IsKill  " + IsKill.ToString());
                if (e != null)
                {
                    if (Menu.Item("IfEnemyInRangeUseQ", true).GetValue<bool>())
                    {
                        if (e.IsValidTarget(Menu.Item("AboutInQRange", true).GetValue<Slider>().Value))
                            if (Q.IsReady())
                                Q.Cast();
                    }

                    if (Menu.Item("ComboW", true).GetValue<bool>())
                    {
                        if (e.IsValidTarget(W.Range))
                            if (W.IsReady())
                                if (W.CanCast(e))
                                    W.Cast(e, true);
                    }

                    if (Menu.Item("ComboE", true).GetValue<bool>())
                    {
                        if (E.IsReady())
                        {
                            if (e.IsValidTarget(E.Range))
                            {
                                //Chat.Print("Stack E Count -------" + GetEStackCount(e).ToString() + " ------- " + e.ChampionName);

                                if ((e.Distance(Me.ServerPosition) >= E.Range * 0.7 && e.IsValidTarget(E.Range) && GetEStackCount(e) >= Menu.Item("MinComboEStack", true).GetValue<Slider>().Value) || (Menu.Item("UseEIfFullStack", true).GetValue<bool>() && GetEStackCount(e) >= 6))
                                {
                                    E.Cast();
                                }
                            }
                        }
                    }

                    if (Menu.Item("AutoComboR", true).GetValue<bool>())
                        if (R.IsReady())
                            if (Me.CountEnemiesInRange(Menu.Item("AboutInRRange", true).GetValue<Slider>().Value) >= Menu.Item("AutoRIfEnemiesInRange", true).GetValue<Slider>().Value)
                                R.Cast();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Combo Logic " + ex);
            }
        }

        private static int GetEStackCount(Obj_AI_Base e)
        {
            if (e.HasBuff("TwitchDeadlyVenom"))
            {
                return e.GetBuffCount("TwitchDeadlyVenom");
            }
            else
                return 0;
        }

        private static void LoadMenu()
        {
            try
            {
                Menu = new Menu("Flowers-Twitch", "NightMoon", true).SetFontStyle(FontStyle.Regular, SColor.Red);

                Menu.AddSubMenu(new Menu("[FL] Orbwalking", "nightmoon.Orbwalker.Menu"));
                Orbwalker = new Orbwalking.Orbwalker(Menu.SubMenu("nightmoon.Orbwalker.Menu"));

                Menu.AddSubMenu(new Menu("[FL] Combo Menu", "nightmoon.Combo.Menu"));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("nightmoon.ComboQ.Use", "--------- Q Logic", true)).SetFontStyle(FontStyle.Regular, SColor.Orange);
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("IfEnemyInRangeUseQ", "If Enemy in Range", true).SetValue(true));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("AboutInQRange", "Search Enemy Range", true).SetValue(new Slider(900, 0, 1800)));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("nightmoon.ComboW.Use", "--------- W Logic", true)).SetFontStyle(FontStyle.Regular, SColor.SeaGreen);
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboW", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("nightmoon.ComboE.Use", "--------- E Logic", true)).SetFontStyle(FontStyle.Regular, SColor.Yellow);
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("ComboE", "Use E", true).SetValue(true));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("UseEIfFullStack", "if enemy full stack", true).SetValue(true));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("MinComboEStack", "Min E Stack Count(Leave E Range Auto E)", true).SetValue(new Slider(3, 1, 6)));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("nightmoon.ComboR.Use", "--------- R Logic", true)).SetFontStyle(FontStyle.Regular, SColor.LightCoral);
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("AutoComboR", "Use R", true).SetValue(true));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("AutoRIfEnemiesInRange", "if enemies in counts >=", true).SetValue(new Slider(2, 1, 5)));
                Menu.SubMenu("nightmoon.Combo.Menu").AddItem(new MenuItem("AboutInRRange", "Search Enemy Range", true).SetValue(new Slider(800, 0, 1500)));

                Menu.AddSubMenu(new Menu("[FL] Harass Menu", "nightmoon.Harass.Menu"));
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("nightmoon.HarassW.Use", "--------- W Logic", true)).SetFontStyle(FontStyle.Regular, SColor.SeaGreen);
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassW", "Use W", true).SetValue(true));
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassWMana", "Harass W Min ManaPer >=", true).SetValue(new Slider(60)));
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("nightmoon.HarassE.Use", "--------- E Logic", true)).SetFontStyle(FontStyle.Regular, SColor.Yellow);
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassE", "Use E", true).SetValue(true));
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassEStack", "Min E Stack Count", true).SetValue(new Slider(2, 1, 6)));
                Menu.SubMenu("nightmoon.Harass.Menu").AddItem(new MenuItem("HarassEMana", "Harass E Min ManaPer >=", true).SetValue(new Slider(60)));

                Menu.AddSubMenu(new Menu("[FL] Misc Menu", "nightmoon.Misc.Menu"));
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("nightmoon.MiscQ.Use", "--------- Q Logic", true)).SetFontStyle(FontStyle.Regular, SColor.Orange);
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("IfKillSomeoneAutoQ", "If Me Kill Enemy & Have others Enemy in Count", true).SetValue(true));
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("nightmoon.MiscE.Use", "--------- E Logic", true)).SetFontStyle(FontStyle.Regular, SColor.Yellow);
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("JungleStealE", "Use E Steal Dragon/Baron", true).SetValue(true));
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("OnlyInCleanMode", "Only In LaneClear Mode Steal", true).SetValue(true));
                Menu.SubMenu("nightmoon.Misc.Menu").AddItem(new MenuItem("KillStealE", "Auto E KillSteal", true).SetValue(true));

                Menu.AddSubMenu(new Menu("[FL] Draw Menu", "nightmoon.Draw.Menu"));
                Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawEnable", "Enable All Drawings", true).SetValue(true));
                Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("WRange", "W Range", true).SetValue(new Circle(true, Color.AntiqueWhite)));
                Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("ERange", "E Range", true).SetValue(new Circle(true, Color.Coral)));
                Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("RRange", "R Range", true).SetValue(new Circle(true, Color.LawnGreen)));
                Menu.SubMenu("nightmoon.Draw.Menu").AddItem(new MenuItem("DrawComboDamage", "Draw (W+E) Damage", true).SetValue(true));

                Menu.AddToMainMenu(); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Menu Loading " + ex);
            }
        }

        private static void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 975);

            W.SetSkillshot(0.25f, 100f, 1410f, false, SkillshotType.SkillshotCircle);
        }
    }
}
