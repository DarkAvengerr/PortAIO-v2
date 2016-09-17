using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace KurisuSivir
{
    internal class KurisuSivir
    {
        internal static Menu Config;
        internal static Spell Q, W, E;
        internal static int CastedTick;
        internal static bool ECasted;
        internal static int ETimestamp;
        internal static string LastCastedSpellName;
        internal static Orbwalking.Orbwalker Orbwalker;
        internal static List<HitType> SivirHits = new List<HitType>(); 
        internal static AIHeroClient Player = ObjectManager.Player;

        // This is like 80% proseries with my activator shielding ported.
        // Is also evade integrated. Smoothest and best spell shield NA Keepo.

        public KurisuSivir()
        {
            if (ObjectManager.Player.ChampionName != "Sivir")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1250);
            Q.SetSkillshot(0.25f, 90f, 1350f, false, SkillshotType.SkillshotLine);

            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);

            Config = new Menu("Kurisu's Sivir", "kurisusivir", true);

            //var tMenu = new Menu("Selector", "sfx");
            //TargetSelector.AddToMenu(tMenu);
            //Config.AddSubMenu(tMenu);

            var orbMenu = new Menu(":: Orbwalker", "Orbwalk");
            Orbwalker = new Orbwalking.Orbwalker(orbMenu);
            Config.AddSubMenu(orbMenu);

            // Spell usage
            var cMenu = new Menu(":: Sivir Settings", "cmenu");
            cMenu.AddItem(new MenuItem("combomana", "Min mana %")).SetValue(new Slider(5));
            cMenu.AddItem(new MenuItem("usecomboq", "Use Q").SetValue(true));
            cMenu.AddItem(new MenuItem("usecombow", "Use W").SetValue(true));
            cMenu.AddItem(new MenuItem("usecombo", "Combo (active)")).SetValue(new KeyBind(32, KeyBindType.Press));
            Config.AddSubMenu(cMenu);

            var hMenu = new Menu(":: Harass", "hmenu");
            var wList = new Menu(":: Harass Whitelist", "hwl");
            foreach (var enemy in HeroManager.Enemies)
            {
                wList.AddItem(new MenuItem("hwl" + enemy.ChampionName, enemy.ChampionName))
                    .SetValue(false);
            }

            hMenu.AddSubMenu(wList);

            hMenu.AddItem(new MenuItem("harassmana", "Min mana %")).SetValue(new Slider(55));
            hMenu.AddItem(new MenuItem("useharassq", "Use Q").SetValue(true));
            hMenu.AddItem(new MenuItem("useharassw", "Use W").SetValue(true));
            hMenu.AddItem(new MenuItem("useharass", "Harass (active)")).SetValue(new KeyBind(67, KeyBindType.Press));
            Config.AddSubMenu(hMenu);

            var fMenu = new Menu(":: Farming", "fmenu");
            fMenu.AddItem(new MenuItem("clearmana", "Min mana %")).SetValue(new Slider(75));
            fMenu.AddItem(new MenuItem("useclearw", "Use W").SetValue(true));
            fMenu.AddItem(new MenuItem("useclear", "Wave/Jungle (active)")).SetValue(new KeyBind(86, KeyBindType.Press));
            Config.AddSubMenu(fMenu);

            var aMenu = new Menu(":: Auto Events", "amenu");


            var sMenu = new Menu(":: Auto Spell Shield", "smenu");

            var adv = new Menu(":: Advance", "adv");
            adv.AddItem(new MenuItem("blockmove", "Spell Shield Block Movement")).SetValue(true);
            adv.AddItem(new MenuItem("blockduration", "Spell Shield Block Duration"))
                .SetValue(new Slider(300, 50, 500));

            sMenu.AddSubMenu(adv);

            var sData = new Menu(":: Spell Shield Database", "sData");


            foreach (var unit in HeroManager.Enemies)
            {
                var menu = new Menu(unit.ChampionName, unit.NetworkId + "menu");

                // new menu per spell
                foreach (var entry in KurisuLib.Spells)
                {
                    if (entry.ChampionName == unit.ChampionName.ToLower())
                    {
                        var newmenu = new Menu(entry.SDataName, entry.SDataName);

                        newmenu.AddItem(new MenuItem(entry.SDataName + "predict", "Enabled").DontSave())
                            .SetValue(entry.CastRange != 0f);
                        newmenu.AddItem(new MenuItem(entry.SDataName + "ultimate", "Ultimate").DontSave())
                            .SetValue(entry.HitType.Contains(HitType.Ultimate));
                        newmenu.AddItem(new MenuItem(entry.SDataName + "danger", "Dangerous").DontSave())
                            .SetValue(entry.HitType.Contains(HitType.Danger));
                        newmenu.AddItem(new MenuItem(entry.SDataName + "crowdcontrol", "Crowd Control").DontSave())
                            .SetValue(entry.HitType.Contains(HitType.CrowdControl));
                        menu.AddSubMenu(newmenu);
                    }
                }

                sData.AddSubMenu(menu);
            }

            sMenu.AddSubMenu(sData);

            sMenu.AddItem(new MenuItem("eint", "Evede Integration"))
                .SetValue(false)
                .SetTooltip("Will disable evading if shield is ready!");
            sMenu.AddItem(new MenuItem("aint", "Disable Shielding"))
                .SetValue(false);
            sMenu.AddItem(new MenuItem("spell", "Use on Any Spell")).SetValue(false);
            sMenu.AddItem(new MenuItem("ultimate", "Use on Ultimates")).SetValue(true);
            sMenu.AddItem(new MenuItem("dangerous", "Use on Dangerous")).SetValue(true);
            sMenu.AddItem(new MenuItem("crowdcontrol", "Use on Crowd Control")).SetValue(true);

            aMenu.AddSubMenu(sMenu);

            aMenu.AddItem(new MenuItem("useqimm", "Auto Q on Immobile")).SetValue(true);
            aMenu.AddItem(new MenuItem("useqdash", "Auto Q on Dashing")).SetValue(true);

            Config.AddItem(new MenuItem("drawings", ":: Drawings")).SetValue(true);
            Config.AddSubMenu(aMenu);

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Evade_OnUpdate;
            Game.OnEnd += Game_OnEnd;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            EloBuddy.Player.OnIssueOrder += AIHeroClient_OnIssueOrder;

            LeagueSharp.Common.Utility.DelayAction.Add(1100, () =>
            {
                if (Menu.GetMenu("ezEvade", "ezEvade").IsRootMenu)
                {
                    Config.Item("eint").SetValue(true);
                    Chat.Print("<font color=\"#66FF33\"><b>KurisuSivir</b></font>: ezEvade Integration Enabled!");

                    if (Config.Item("eint").GetValue<bool>())
                    {
                        // EzEvade
                        Menu.GetMenu("ezEvade", "ezEvade")
                            .Item("SivirE" + "EvadeSpellMode")
                            .SetValue(new StringList(new[] {"Undodgeable", "Activation Time", "Always"}, 0));
                    }
                }

                if (Menu.GetMenu("Evade", "Evade").IsRootMenu)
                {
                    Config.Item("eint").SetValue(true);
                    Chat.Print("<font color=\"#66FF33\"><b>KurisuSivir</b></font>: Evade# Integration Enabled!");
                }

                if (Menu.GetMenu("Activator", "activator").IsRootMenu)
                    Chat.Print("<font color=\"#66FF33\"><b>KurisuSivir</b></font>: For the best experiance use the built in auto shield only!");

                if (Menu.GetMenu("Evade", "Evade") == null && Menu.GetMenu("ezEvade", "ezEvade").IsRootMenu)
                {
                    Config.Item("eint").SetValue(false);
                    Chat.Print("<font color=\"#66FF33\"><b>KurisuSivir</b></font>: Evade not found, Evade is reccomended!");
                }

            });

            Chat.Print("<font color=\"#66FF33\"><b>KurisuSivir</b></font>: Loaded!");
        }
        
        static void AIHeroClient_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && args.Order == GameObjectOrder.MoveTo)
            {
                if (Config.Item("blockmove").GetValue<bool>())
                {
                    args.Process = !ECasted;
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Item("drawings").GetValue<bool>())
                return;

            if (!Player.IsDead && E.Level > 0  && E.IsReady())
            {
                var pos = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(pos[0] - 35, pos[1] + 65, System.Drawing.Color.LawnGreen, "E READY");
            }

            if (!Player.IsDead && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Red, 3);
            }
        }

        static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (!unit.IsValid || !unit.IsMe)
            {
                return;
            }

            if (Config.Item("usecombo").GetValue<KeyBind>().Active && Player.ManaPercent >
                Config.Item("combomana").GetValue<Slider>().Value)
            {
                if (Config.Item("usecombow").GetValue<bool>() &&
                    target.IsValid<AIHeroClient>())
                {
                    W.Cast();
                }
            }

            if (Config.Item("useharass").GetValue<KeyBind>().Active && Player.ManaPercent >
                Config.Item("harassmana").GetValue<Slider>().Value)
            {
                if (Config.Item("useharassw").GetValue<bool>() &&
                    target.IsValid<AIHeroClient>() && IsWhiteListed((AIHeroClient)target))
                {
                    W.Cast();
                }
            }

            if (Config.Item("useclear").GetValue<KeyBind>().Active && Player.ManaPercent >
                Config.Item("clearmana").GetValue<Slider>().Value)
            {
                if (Config.Item("useclearw").GetValue<bool>() &&
                    target.IsValid<Obj_AI_Minion>())
                {
                    W.Cast();
                }
            }
        }

        static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Config.Item("aint").GetValue<bool>())
                return;

            if (sender.IsMe && args.SData.Name == "SivirE")
            {
                if (!ECasted)
                {
                    ECasted = true;
                    ETimestamp = Utils.GameTimeTickCount;
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient)
            {
                var attacker = sender as AIHeroClient;
                if (attacker == null || attacker.IsAlly || !attacker.IsValid<AIHeroClient>())
                    return;

                CastedTick = Utils.GameTimeTickCount;
                foreach (var data in KurisuLib.Spells.Where(x => x.SDataName == args.SData.Name.ToLower()))
                {
                    if (Player.Distance(attacker.ServerPosition) <= data.CastRange + 750)
                    {
                        LastCastedSpellName = data.SDataName;
                    }

                    #region self/selfaoe

                    if (args.SData.TargettingType == SpellDataTargetType.Self ||
                        args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                    {
                        var fromObj =
                            ObjectManager.Get<GameObject>()
                                .FirstOrDefault(
                                    x => data.FromObject != null && !x.IsAlly && data.FromObject.Any(y => x.Name.Contains(y)));

                        var correctpos = fromObj != null ? fromObj.Position : attacker.ServerPosition;

                        if (Player.Distance(correctpos) > data.CastRange)
                            return;

                        if (data.SDataName == "kalistaexpungewrapper" && !Player.HasBuff("kalistaexpungemarker"))
                            return;

                        if (!Config.Item(data.SDataName + "predict").GetValue<bool>())
                            return;

                        // delay the spell a bit before missile endtime
                        LeagueSharp.Common.Utility.DelayAction.Add((int)(data.Delay - (data.Delay * 0.7)), () =>
                        {
                            SivirHits.Add(HitType.Spell);

                            if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                SivirHits.Add(HitType.Danger);
                            if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                SivirHits.Add(HitType.CrowdControl);
                            if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                SivirHits.Add(HitType.Ultimate);

                            // lazy safe reset
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                            {
                                SivirHits.Remove(HitType.Spell);
                                LastCastedSpellName = string.Empty;
                                if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                    SivirHits.Remove(HitType.Danger);
                                if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                    SivirHits.Remove(HitType.CrowdControl);
                                if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                    SivirHits.Remove(HitType.Ultimate);
                            });
                        });
                    }

                    #endregion

                    #region skillshot

                    if (args.SData.TargettingType == SpellDataTargetType.Cone ||
                        args.SData.TargettingType.ToString().Contains("Location") ||
                        args.SData.TargettingType == (SpellDataTargetType) 10 && data.SDataName == "azirq")
                    {
                        var fromObj =
                            ObjectManager.Get<GameObject>()
                                .FirstOrDefault(
                                    x =>
                                        !x.IsAlly && data.FromObject != null &&
                                        data.FromObject.Any(y => x.Name.Contains(y)));

                        var islineskillshot = args.SData.TargettingType == SpellDataTargetType.Cone || args.SData.LineWidth != 0;
                        var startpos = fromObj != null ? fromObj.Position : attacker.ServerPosition;

                        var correctwidth = islineskillshot && args.SData.TargettingType != SpellDataTargetType.Cone
                            ? args.SData.LineWidth : (args.SData.CastRadius == 0 ? args.SData.CastRadiusSecondary : args.SData.CastRadius);

                        if (data.SDataName == "azirq")
                        {
                            correctwidth = 275f;
                            islineskillshot = true;
                        }

                        if (Player.Distance(startpos) > data.CastRange)
                            return;

                        var distance = (int)(1000 * (startpos.Distance(Player.ServerPosition) / data.MissileSpeed));
                        var endtime = data.Delay - 100 + Game.Ping / 2 + distance - (Utils.GameTimeTickCount - CastedTick);

                        var direction = (args.End.To2D() - startpos.To2D()).Normalized();
                        var endpos = startpos.To2D() + direction * startpos.To2D().Distance(args.End.To2D());

                        if (startpos.To2D().Distance(endpos) > data.CastRange)
                            endpos = startpos.To2D() + direction * data.CastRange;

                        var proj = Player.ServerPosition.To2D().ProjectOn(startpos.To2D(), endpos);
                        var projdist = Player.ServerPosition.To2D().Distance(proj.SegmentPoint);

                        if (!islineskillshot && Player.Distance(endpos) <= correctwidth ||
                            islineskillshot && correctwidth + Player.BoundingRadius > projdist)
                        {
                            if (!Config.Item(data.SDataName + "predict").GetValue<bool>())
                                return;

                            LeagueSharp.Common.Utility.DelayAction.Add((int)(endtime - (endtime * 0.3)), () =>
                            {
                                SivirHits.Add(HitType.Spell);
                                if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                    SivirHits.Add(HitType.Danger);
                                if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                    SivirHits.Add(HitType.CrowdControl);
                                if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                    SivirHits.Add(HitType.Ultimate);

                                LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                                {
                                    SivirHits.Remove(HitType.Spell);
                                    LastCastedSpellName = string.Empty;
                                    if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                        SivirHits.Remove(HitType.Danger);
                                    if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                        SivirHits.Remove(HitType.CrowdControl);
                                    if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                        SivirHits.Remove(HitType.Ultimate);
                                });
                            });
                        }
                    }

                    #endregion

                    #region unit type

                    if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                        args.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                    {
                        if (args.Target == null)
                            return;

                        // check if is targeteting the hero on our table
                        if (Player.NetworkId != args.Target.NetworkId)
                            return;

                        // target spell dectection
                        if (Player.Distance(attacker.ServerPosition) > data.CastRange)
                            return;

                        var distance = (int)(1000 * (attacker.Distance(Player.ServerPosition) / data.MissileSpeed));
                        var endtime = data.Delay - 100 + Game.Ping / 2 + distance - (Utils.GameTimeTickCount - CastedTick);

                        if (!Config.Item(data.SDataName + "predict").GetValue<bool>())
                            return;

                        LeagueSharp.Common.Utility.DelayAction.Add((int)(endtime - (endtime * 0.7)), () =>
                        {
                            SivirHits.Add(HitType.Spell);

                            if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                SivirHits.Add(HitType.Danger);
                            if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                SivirHits.Add(HitType.CrowdControl);
                            if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                SivirHits.Add(HitType.Ultimate);

                            // lazy reset
                            LeagueSharp.Common.Utility.DelayAction.Add(250, () =>
                            {
                                SivirHits.Remove(HitType.Spell);
                                LastCastedSpellName = string.Empty;
                                if (Config.Item(data.SDataName + "danger").GetValue<bool>())
                                    SivirHits.Remove(HitType.Danger);
                                if (Config.Item(data.SDataName + "crowdcontrol").GetValue<bool>())
                                    SivirHits.Remove(HitType.CrowdControl);
                                if (Config.Item(data.SDataName + "ultimate").GetValue<bool>())
                                    SivirHits.Remove(HitType.Ultimate);
                            });
                        });
                    }

                    #endregion
                }
            }
            
        }

        internal static bool IsWhiteListed(AIHeroClient unit)
        {
            return Config.SubMenu("hmenu").Item("hwl" + unit.ChampionName).GetValue<bool>();
        }

        internal static bool DodgeSpell()
        {
            if (string.IsNullOrEmpty(LastCastedSpellName))
                return false;

            return (Config.Item(LastCastedSpellName + "danger").GetValue<bool>() ||
                    Config.Item(LastCastedSpellName + "crowdcontrol").GetValue<bool>() ||
                    Config.Item(LastCastedSpellName + "ultimate").GetValue<bool>()) &&
                    Config.Item(LastCastedSpellName + "predict").GetValue<bool>();
        }

        static void Evade_OnUpdate(EventArgs arga)
        {
            if (Config.Item("aint").GetValue<bool>())
                return;

            if (ECasted && Utils.GameTimeTickCount - ETimestamp > 250)
            {
                if (!Player.HasBuff("SivirE"))
                {
                    ECasted = false;
                }

                if (Utils.GameTimeTickCount - ETimestamp >= Config.Item("blockduration").GetValue<Slider>().Value)
                {
                    ECasted = false;
                }
            }

            #region Evade Stuff

            if (Config.Item("eint").GetValue<bool>())
            {
                // EzEvade
                if (Menu.GetMenu("ezEvade", "ezEvade") != null)
                {
                    var item = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeSkillShots");
                    var evadestatus = new KeyBind('K', KeyBindType.Toggle);

                    if (E.IsReady() && E.Level > 0 && DodgeSpell())
                        evadestatus.Active = false;

                    if (!E.IsReady() && !Player.HasBuff("SivirE") && E.Level > 0 || !DodgeSpell())
                        evadestatus.Active = true;

                    if (E.Level > 0)
                        item.SetValue(evadestatus);
                }

                // Evade#

                if (Menu.GetMenu("Evade", "Evade") != null)
                {
                    var item = Menu.GetMenu("Evade", "Evade").Item("Enabled");
                    var evadestatus = new KeyBind('K', KeyBindType.Toggle);

                    if (E.IsReady() && E.Level > 0 && DodgeSpell())
                        evadestatus.Active = false;

                    if (!E.IsReady() && !Player.HasBuff("SivirE") && E.Level > 0 || !DodgeSpell())
                        evadestatus.Active = true;

                    if (E.Level > 0)
                        item.SetValue(evadestatus);
                }
            }

            #endregion
        }

        static void Game_OnUpdate(EventArgs args)
        {
            if (Config.Item("usecombo").GetValue<KeyBind>().Active && Player.ManaPercent >
                Config.Item("combomana").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && Config.Item("usecomboq").GetValue<bool>())
                {
                    if (Q.IsReady())
                    {
                        var qouput = Q.GetPrediction(target);
                        if (qouput.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qouput.CastPosition);
                        }
                    }
                }
            }

            if (Config.Item("useharass").GetValue<KeyBind>().Active && Player.ManaPercent >
                Config.Item("harassmana").GetValue<Slider>().Value)
            {
                var target = TargetSelector.GetTarget(Q.Range - 275, TargetSelector.DamageType.Physical);
                if (target.IsValidTarget() && IsWhiteListed(target))
                {
                    if (Config.Item("useharassq").GetValue<bool>() && Q.IsReady())
                    {
                        var qouput = Q.GetPrediction(target);
                        if (qouput.Hitchance >= HitChance.High)
                        {
                            Q.Cast(qouput.CastPosition);
                        }
                    }
                }
            }

            if (Q.IsReady() && Q.Level > 0)
            {
                foreach (var target in HeroManager.Enemies.Where(h => h.IsValidTarget(Q.Range)))
                {
                    if (Config.Item("useqimm").GetValue<bool>())
                        Q.CastIfHitchanceEquals(target, HitChance.Immobile);

                    if (Config.Item("useqdash").GetValue<bool>() && target.Distance(Player.ServerPosition) <= 375f)
                        Q.CastIfHitchanceEquals(target, HitChance.Dashing);
                }
            }

            #region E Kappa

            if (E.IsReady() && !Config.Item("aint").GetValue<bool>())
            {
                if (Config.Item("spell").GetValue<bool>())  
                {
                    if (SivirHits.Contains(HitType.Spell))
                        E.Cast();
                }

                if (Config.Item("crowdcontrol").GetValue<bool>())
                {
                    if (SivirHits.Contains(HitType.CrowdControl))
                        E.Cast();
                }

                if (Config.Item("dangerous").GetValue<bool>())
                {
                    if (SivirHits.Contains(HitType.Danger))
                        E.Cast();
                }

                if (Config.Item("ultimate").GetValue<bool>())
                {
                    if (SivirHits.Contains(HitType.Ultimate))
                    {
                        E.Cast();
                    }
                }
            }

            #endregion
        }

        static void Game_OnEnd(GameEndEventArgs args)
        {
            Game.OnUpdate -= Evade_OnUpdate;

            if (Menu.GetMenu("ezEvade", "ezEvade") != null)
            {
                var item = Menu.GetMenu("ezEvade", "ezEvade").Item("DodgeSkillShots");
                var evadestatus = new KeyBind('K', KeyBindType.Toggle);

                evadestatus.Active = true;
                item.SetValue(evadestatus);
            }

            if (Menu.GetMenu("Evade", "Evade") != null)
            {
                var item = Menu.GetMenu("Evade", "Evade").Item("Enabled");
                var evadestatus = new KeyBind('K', KeyBindType.Toggle);

                evadestatus.Active = true;
                item.SetValue(evadestatus);
            }
        }
    }
}
