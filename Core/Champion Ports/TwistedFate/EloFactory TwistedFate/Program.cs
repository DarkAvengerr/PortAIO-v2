#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Security.AccessControl;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace EloFactory_TwistedFate
{
    internal class Program
    {
        public const string ChampionName = "TwistedFate";

        public static Orbwalking.Orbwalker Orbwalker;

        public static List<Spell> SpellList = new List<Spell>();

        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Spell Ignite = new Spell(SpellSlot.Unknown, 600);

        public static float QMANA;
        public static float WMANA;
        public static float EMANA;
        public static float RMANA;

        public static Items.Item HealthPotion = new Items.Item(2003, 0);
        public static Items.Item ManaPotion = new Items.Item(2004, 0);
        public static Items.Item CrystallineFlask = new Items.Item(2041, 0);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010, 0);

        public static Items.Item WoogletsWitchcap = new Items.Item(3090, 0);
        public static Items.Item ZhonyasHourglass = new Items.Item(3157, 0);

        public static int CardTickCount;
        public static int RCardTickCount;
        public static int ManualCardGoldTickCount;
        public static int ManualCardRedTickCount;
        public static int ManualCardBlueTickCount;

        public static Menu Config;

        private static AIHeroClient Player;

        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;


        public static void Game_OnGameLoad()
        {
            Player = ObjectManager.Player;

            if (Player.ChampionName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 1500f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 40f, 1000f, false, SkillshotType.SkillshotLine);

            var ignite = Player.Spellbook.Spells.FirstOrDefault(spell => spell.Name == "summonerdot");
            if (ignite != null)
                Ignite.Slot = ignite.Slot;

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            abilitySequence = new int[] { 2, 1, 1, 3, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3 };

            Config = new Menu(ChampionName + " By LuNi", ChampionName + " By LuNi", true);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Manual Card Selector", "Manual Card Selector"));
            Config.SubMenu("Manual Card Selector").AddItem(new MenuItem("TwistedFate.GoldCardActive", "Pick Gold Card!").SetValue(new KeyBind("W".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Manual Card Selector").AddItem(new MenuItem("TwistedFate.RedCardActive", "Pick Red Card!").SetValue(new KeyBind("E".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Manual Card Selector").AddItem(new MenuItem("TwistedFate.BlueCardActive", "Pick Blue Card!").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Manual Card Selector").AddItem(new MenuItem("TwistedFate.MoveOnCursorWhenPickACardKey", "Move On Cursor When Pick A Card Key Press").SetValue(true));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddSubMenu(new Menu("KS Mode", "KS Mode"));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("TwistedFate.UseIgniteKS", "KS With Ignite").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("TwistedFate.UseQKS", "KS With Q").SetValue(true));
            Config.SubMenu("Combo").SubMenu("KS Mode").AddItem(new MenuItem("TwistedFate.UseWKS", "KS With W").SetValue(true));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Items Activator", "Items Activator"));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Zhonya's Hourglass", "Use Zhonya's Hourglass"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Zhonya's Hourglass").AddItem(new MenuItem("TwistedFate.useZhonyasHourglass", "Use Zhonya's Hourglass").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Zhonya's Hourglass").AddItem(new MenuItem("TwistedFate.MinimumHPtoZhonyasHourglass", "Minimum Health Percent To Use Zhonya's Hourglass").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Items Activator").AddSubMenu(new Menu("Use Wooglet's Witchcap", "Use Wooglet's Witchcap"));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Wooglet's Witchcap").AddItem(new MenuItem("TwistedFate.useWoogletsWitchcap", "Use Wooglet's Witchcap").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Items Activator").SubMenu("Use Wooglet's Witchcap").AddItem(new MenuItem("TwistedFate.MinimumHPtoWoogletsWitchcap", "Minimum Health Percent To Use Wooglet's Witchcap").SetValue(new Slider(30, 0, 100)));
            Config.SubMenu("Combo").AddItem(new MenuItem("TwistedFate.UseQCombo", "Use Q In Combo").SetValue(true));
            Config.SubMenu("Combo").AddSubMenu(new Menu("Use W In Combo", "Use W In Combo"));
            Config.SubMenu("Combo").SubMenu("Use W In Combo").AddItem(new MenuItem("TwistedFate.UseWCombo", "Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").SubMenu("Use W In Combo").AddItem(new MenuItem("TwistedFate.UseWComboOption", "Card Choice For Combo").SetValue(new StringList(new[] { "Intelligent Card Usage", "Gold Card", "Red Card", "Blue Card" })));
            Config.SubMenu("Combo").SubMenu("Use W In Combo").AddItem(new MenuItem("TwistedFate.WMiniManaCombo", "Minimum Mana To Use W In Combo").SetValue(new Slider(15, 0, 100)));
            Config.SubMenu("Combo").SubMenu("Use W In Combo").AddItem(new MenuItem("TwistedFate.UseWBlueCombo", "Use W Blue Card When Percent Mana Under Mana To Use W In Combo").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("TwistedFate.AutoQOnStunTarget", "Auto Use Q On Stunned Target").SetValue(true));

            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.UseQHarass", "Use Q In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.QMiniManaHarass", "Minimum Mana To Use Q In Harass").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.QWhenEnemyCastHarass", "Use Q On Enemy AA/Spell In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.UseWHarass", "Use W In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.UseWHarassOption", "Card Choice For Harass").SetValue(new StringList(new[] { "Intelligent Card Usage", "Gold Card", "Red Card", "Blue Card" })));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.WMiniManaHarass", "Minimum Mana To Use W In Harass").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.UseWBlueHarass", "Use W Blue Card When Percent Mana Under Mana To Use W In Harass").SetValue(true));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.HarassActive", "Harass!").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("Harass").AddItem(new MenuItem("TwistedFate.HarassActiveT", "Harass (toggle)!").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("LastHit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("TwistedFate.UseWLastHitBlueCard", "Use W Blue Card").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("TwistedFate.OptionW", "Use W Blue Card Only When Picking A Card Already Activated").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("TwistedFate.OptionW2", "Use W Blue Card Only When No Enemy In Dangerous Range").SetValue(false));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.UseQLaneClear", "Use Q in LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.QMiniManaLaneClear", "Minimum Mana To Use Q In LaneClear").SetValue(new Slider(45, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.QLaneClearCount", "Minimum Minion To Use Q In LaneClear").SetValue(new Slider(3, 1, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.UseWLaneClear", "Use W In LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.UseWLaneClearOption", "Card Choice For LaneClear").SetValue(new StringList(new[] { "Red Card", "Blue Card" })));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.WMiniManaLaneClear", "Minimum Mana To Use W In LaneClear").SetValue(new Slider(35, 0, 100)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.WMiniEnemyLaneClear", "Minimum Minion In Range To Use Red Card In LaneClear").SetValue(new Slider(3, 2, 6)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.UseWBlueLaneClear", "Use W Blue Card When Percent Mana Under Mana To Use W In LaneClear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("TwistedFate.LaneClearActive", "LaneClear Key!").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.UseQJungleClear", "Use Q In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.QMiniManaJungleClear", "Minimum Mana To Use Q In JungleClear").SetValue(new Slider(0, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.UseWJungleClear", "Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.UseWJungleClearOption", "Card Choice For JungleClear").SetValue(new StringList(new[] { "Intelligent Card Usage", "Gold Card", "Red Card", "Blue Card" })));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.WMiniManaJungleClear", "Minimum Mana To Use W In JungleClear").SetValue(new Slider(40, 0, 100)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.UseWBlueJungleClear", "Use W Blue Card When Percent Mana Under Mana To Use W In JungleClear").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("TwistedFate.SafeJungleClear", "Dont Use Spell In Jungle Clear If Enemy in Dangerous Range").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddSubMenu(new Menu("Skin Changer", "Skin Changer"));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("TwistedFate.SkinChanger", "Use Skin Changer").SetValue(false));
            Config.SubMenu("Misc").SubMenu("Skin Changer").AddItem(new MenuItem("TwistedFate.SkinChangerName", "Skin choice").SetValue(new StringList(new[] { "Classic", "PAX", "Jack of Hearts", "The Magnificent", "Tango", "High Noon", "Musketeer", "Underworld", "Red Card" })));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.WInterrupt", "Interrupt Spells With W Gold Card").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoQEGC", "Auto Q On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoWEGC", "Auto W On Gapclosers").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoWEGCOption", "Auto W On Gapclosers Card Choice").SetValue(new StringList(new[] { "Intelligent Card Usage", "Gold Card", "Red Card" })));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoGoldCardOnR", "Auto Gold Card When Using R").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoPotion", "Use Auto Potion").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("TwistedFate.AutoLevelSpell", "Auto Level Spell").SetValue(true));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.Indigo)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawOrbwalkTarget", "Draw Orbwalk target").SetValue(true));

            Config.AddToMainMenu();

            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnSpellCast += Obj_AI_Base_OnProcessSpellCast;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;

        }

        #region ToogleOrder Game_OnUpdate
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("TwistedFate.AutoLevelSpell").GetValue<bool>()) LevelUpSpells();

            if (Player.IsDead) return;

            if (Player.GetBuffCount("Recall") == 1) return;

            ManaManager();
            PotionManager();

            KillSteal();

            #region Manual Card Picking

            if (Config.Item("TwistedFate.GoldCardActive").GetValue<KeyBind>().Active)
            {
                if (Config.Item("TwistedFate.MoveOnCursorWhenPickACardKey").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }

                if (Environment.TickCount - ManualCardGoldTickCount < 250) return;
                ManualCardGoldTickCount = Environment.TickCount;

                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                }
                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                {
                    W.Cast();
                }
            }
            if (Config.Item("TwistedFate.RedCardActive").GetValue<KeyBind>().Active)
            {
                if (Config.Item("TwistedFate.MoveOnCursorWhenPickACardKey").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                if (Environment.TickCount - ManualCardRedTickCount < 250) return;
                ManualCardRedTickCount = Environment.TickCount;

                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                }
                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                {
                    W.Cast();
                }
            }
            if (Config.Item("TwistedFate.BlueCardActive").GetValue<KeyBind>().Active)
            {
                if (Config.Item("TwistedFate.MoveOnCursorWhenPickACardKey").GetValue<bool>())
                {
                    EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                if (Environment.TickCount - ManualCardBlueTickCount < 250) return;
                ManualCardBlueTickCount = Environment.TickCount;

                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                }
                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                {
                    W.Cast();
                }
            }


            #endregion

            if (Config.Item("TwistedFate.AutoGoldCardOnR").GetValue<bool>())
            {
                if (Environment.TickCount - RCardTickCount < 250) return;

                RCardTickCount = Environment.TickCount;
                if ((Player.HasBuff("Destiny Marker") || Player.HasBuff("Gate")) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                }
                else if ((Player.HasBuff("Destiny Marker") || Player.HasBuff("Gate")) && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                {
                    W.Cast();
                }
            }

            if (Config.Item("TwistedFate.AutoQOnStunTarget").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                if (target.IsValidTarget() && (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare)) && Player.Distance(target) < Q.Range)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High, true);
                }
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Config.Item("TwistedFate.LaneClearActive").GetValue<KeyBind>().Active)
            {
                Orbwalking.Attack = true;
                JungleClear();
                LaneClear();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                LastHit();
            }

            if (Config.Item("TwistedFate.HarassActive").GetValue<KeyBind>().Active || Config.Item("TwistedFate.HarassActiveT").GetValue<KeyBind>().Active)
            {
                Harass();
            }

        }
        #endregion

        #region Interupt OnProcessSpellCast
        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs args)
        {

            if ((unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Config.Item("TwistedFate.useZhonyasHourglass").GetValue<bool>() && ZhonyasHourglass.IsReady() && Player.HealthPercent <= Config.Item("TwistedFate.MinimumHPtoZhonyasHourglass").GetValue<Slider>().Value)
            {
                if (Player.CountEnemiesInRange(1300) > 1)
                {
                    if (Player.CountAlliesInRange(1300) >= 1 + 1)
                    {
                        ZhonyasHourglass.Cast();
                        return;
                    }
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                        {
                            ZhonyasHourglass.Cast();
                            return;
                        }
                    }
                }
                if (Player.CountEnemiesInRange(1300) == 1)
                {
                    if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                    {
                        ZhonyasHourglass.Cast();
                        return;
                    }
                }

            }

            if ((unit.IsValid<AIHeroClient>() || unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Config.Item("TwistedFate.useWoogletsWitchcap").GetValue<bool>() && WoogletsWitchcap.IsReady() && Player.HealthPercent <= Config.Item("TwistedFate.MinimumHPtoWoogletsWitchcap").GetValue<Slider>().Value)
            {
                if (Player.CountEnemiesInRange(1300) > 1)
                {
                    if (Player.CountAlliesInRange(1300) >= 1 + 1)
                    {
                        WoogletsWitchcap.Cast();
                        return;
                    }
                    if (Player.CountAlliesInRange(1300) == 0 + 1)
                    {
                        if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                        {
                            WoogletsWitchcap.Cast();
                            return;
                        }
                    }
                }
                if (Player.CountEnemiesInRange(1300) == 1)
                {
                    if (unit.GetSpellDamage(Player, args.SData.Name) >= Player.Health || (unit.GetAutoAttackDamage(Player) >= Player.Health && args.SData.IsAutoAttack()))
                    {
                        WoogletsWitchcap.Cast();
                        return;
                    }
                }

            }

            double ShouldUseOn = ShouldUse(args.SData.Name);
            if (unit.Team != ObjectManager.Player.Team && ShouldUseOn >= 0f && unit.IsValidTarget(Q.Range))
            {

                if (Config.Item("TwistedFate.WInterrupt").GetValue<bool>() && W.IsReady() && Player.Mana >= WMANA && Player.Distance(unit) <= W.Range)
                {
                    W.CastIfHitchanceEquals(unit, HitChance.High, true);
                }

            }

            if (Config.Item("TwistedFate.HarassActive").GetValue<KeyBind>().Active || Config.Item("TwistedFate.HarassActiveT").GetValue<KeyBind>().Active)
            {
                if (Config.Item("TwistedFate.QWhenEnemyCastHarass").GetValue<bool>() && (unit.IsValid<AIHeroClient>() && !unit.IsValid<Obj_AI_Turret>()) && unit.IsEnemy && args.Target.IsMe && Q.IsReady() && Player.Distance(unit) <= Q.Range)
                {
                    Q.CastIfHitchanceEquals(unit, HitChance.High, true);
                }
                Harass();
            }


        }
        #endregion

        #region AntiGapCloser
        static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {

            if (Config.Item("TwistedFate.AutoQEGC").GetValue<bool>() && Q.IsReady() && (Player.Mana >= QMANA + WMANA) && Player.Distance(gapcloser.Sender) < Q.Range)
            {
                Q.CastIfHitchanceEquals(gapcloser.Sender, HitChance.High, true);
            }

            if (Config.Item("TwistedFate.AutoWEGC").GetValue<bool>() && W.IsReady() && (Player.Mana >= WMANA) && Player.Distance(gapcloser.Sender) < 660)
            {
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                }
                switch (Config.Item("TwistedFate.AutoWEGCOption").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                            {
                                if (gapcloser.Sender.CountAlliesInRange(200) >= 1)
                                {
                                    W.Cast();
                                }

                            }

                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                            {
                                if ((getComboDamage(gapcloser.Sender) > gapcloser.Sender.Health) || (Player.HealthPercent < 50 && Player.HealthPercent < gapcloser.Sender.HealthPercent))
                                {
                                    W.Cast();
                                }
                                else if (gapcloser.Sender.CountAlliesInRange(200) == 0)
                                    W.Cast();

                            }
                            break;
                        }

                    case 1:
                        {
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                            {
                                W.Cast();
                            }
                            break;
                        }

                    case 2:
                        {
                            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                            {
                                W.Cast();
                            }
                            break;
                        }

                }
            }

        }
        #endregion

        #region On Dash
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var useQ = Config.Item("TwistedFate.UseQCombo").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy) return;

            if (sender.NetworkId == target.NetworkId)
            {

                if (useQ && Q.IsReady() && Player.Mana >= QMANA && args.EndPos.Distance(Player) < Q.Range)
                {

                    var delay = (int)(args.EndTick - Game.Time - Q.Delay - 0.1f);
                    if (delay > 0)
                    {
                        LeagueSharp.Common.Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                    }
                    else
                    {
                        Q.Cast(args.EndPos);
                    }
                }
            }
        }
        #endregion

        #region Combo
        public static void Combo()
        {

            var useQ = Config.Item("TwistedFate.UseQCombo").GetValue<bool>();
            var useW = Config.Item("TwistedFate.UseWCombo").GetValue<bool>();

            if (Player.HasBuff("Pick A Card")) Orbwalking.Attack = false;
            else Orbwalking.Attack = true;


            var target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget())
            {

                #region Sort Q combo mode
                if (useQ && Q.IsReady() && Player.Mana >= QMANA && ((!W.IsReady() && !Player.HasBuff("Pick A Card") && !Player.HasBuff("Pick A Card Red") && !Player.HasBuff("Pick A Card Gold") && !Player.HasBuff("Pick A Card Blue")) || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup)))
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High, true);
                }
                #endregion

                #region Sort W combo mode
                if (W.IsReady() && Player.Mana >= WMANA && ((Player.Distance(target) < 660 && target.IsMoving) || (Player.Distance(target) < 800) && !target.IsMoving && Player.MoveSpeed > target.MoveSpeed) || (Player.Distance(target) < 800 && target.IsFacing(Player)))
                {

                    if (!useW) return;
                    if (Environment.TickCount - CardTickCount < 250) return;

                    CardTickCount = Environment.TickCount;
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                    {
                        W.Cast();
                        return;
                    }
                    if (Config.Item("TwistedFate.UseWBlueCombo").GetValue<bool>() && Player.ManaPercent <= Config.Item("TwistedFate.WMiniManaCombo").GetValue<Slider>().Value)
                    {
                        if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                        {
                            W.Cast();
                        }
                    }
                    else

                        switch (Config.Item("TwistedFate.UseWComboOption").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                    {
                                        if (Player.CountEnemiesInRange(1200) >= 2 && target.CountAlliesInRange(200) >= 1)
                                        {
                                            W.Cast();
                                        }
                                    }

                                    else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                            case 1:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                            case 2:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                            case 3:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                        }
                }
                #endregion

            }

        }
        #endregion

        #region Harass
        public static void Harass()
        {
            var targetH = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            var useQ = Config.Item("TwistedFate.UseQHarass").GetValue<bool>();
            var useW = Config.Item("TwistedFate.UseWHarass").GetValue<bool>();
            var QMinMana = Config.Item("TwistedFate.QMiniManaHarass").GetValue<Slider>().Value;
            var WMinMana = Config.Item("TwistedFate.WMiniManaHarass").GetValue<Slider>().Value;

            if (Player.HasBuff("Pick A Card")) Orbwalking.Attack = false;
            else Orbwalking.Attack = true;

            if (useQ && Q.IsReady() && Player.Distance(targetH) < Q.Range && Player.ManaPercent >= QMinMana)
            {
                Q.CastIfHitchanceEquals(targetH, HitChance.High, true);
            }


            if (useW && W.IsReady() && Player.ManaPercent > WMinMana && Player.Mana >= WMANA && ((Player.Distance(targetH) < 660 && targetH.IsMoving) || (Player.Distance(targetH) < 800) && !targetH.IsMoving && Player.MoveSpeed > targetH.MoveSpeed) || (Player.Distance(targetH) < 800 && targetH.IsFacing(Player)))
            {
                if (Environment.TickCount - CardTickCount < 250) return;

                CardTickCount = Environment.TickCount;
                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                {
                    W.Cast();
                    return;
                }
                if (Config.Item("TwistedFate.UseWBlueHarass").GetValue<bool>() && Player.ManaPercent <= WMinMana)
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                    {
                        W.Cast();
                    }
                }
                else

                    switch (Config.Item("TwistedFate.UseWHarassOption").GetValue<StringList>().SelectedIndex)
                    {
                        case 0:
                            {

                                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                {
                                    if (Player.CountEnemiesInRange(1200) >= 2 && targetH.CountAlliesInRange(200) >= 1)
                                    {
                                        W.Cast();
                                    }
                                }

                                else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                {
                                    W.Cast();
                                }
                                break;
                            }

                        case 1:
                            {

                                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                {
                                    W.Cast();
                                }
                                break;
                            }

                        case 2:
                            {

                                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                {
                                    W.Cast();
                                }
                                break;
                            }

                        case 3:
                            {

                                if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                                {
                                    W.Cast();
                                }
                                break;
                            }

                    }
            }

        }
        #endregion

        #region LastHit
        public static void LastHit()
        {

            var useW = Config.Item("TwistedFate.UseWLastHitBlueCard").GetValue<bool>();

            if (useW && W.IsReady() && Player.Mana >= WMANA)
            {
                var AllMinions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

                if (AllMinions.Any() && ((!Config.Item("TwistedFate.OptionW").GetValue<bool>()) || (Player.CountEnemiesInRange(1400) == 0 && Config.Item("TwistedFate.OptionW2").GetValue<bool>())))
                {

                    if (Environment.TickCount - CardTickCount < 250) return;

                    CardTickCount = Environment.TickCount;
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                    {
                        W.Cast();
                        return;
                    }
                    else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                    {
                        W.Cast();
                    }

                }

                else if (AllMinions.Any() && Config.Item("TwistedFate.OptionW").GetValue<bool>() && (Player.CountEnemiesInRange(1400) != 0 || !Config.Item("TwistedFate.OptionW2").GetValue<bool>()))
                {
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                    {
                        W.Cast();
                    }

                }
            }

        }
        #endregion

        #region LaneClear
        public static void LaneClear()
        {

            var useQ = Config.Item("TwistedFate.UseQLaneClear").GetValue<bool>();
            var useW = Config.Item("TwistedFate.UseWLaneClear").GetValue<bool>();

            var QMinMana = Config.Item("TwistedFate.QMiniManaLaneClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("TwistedFate.WMiniManaLaneClear").GetValue<Slider>().Value;



            if (useQ && Q.IsReady() && Player.Mana >= QMANA && Player.ManaPercent >= QMinMana)
            {
                var allMinionsQ = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy);

                if (allMinionsQ.Any())
                {
                    var farmAll = Q.GetLineFarmLocation(allMinionsQ, 40);
                    if (farmAll.MinionsHit >= Config.Item("TwistedFate.QLaneClearCount").GetValue<Slider>().Value)
                    {
                        Q.Cast(farmAll.Position, true);
                    }
                }
            }

            if (useW && W.IsReady() && Player.Mana >= WMANA)
            {
                var AllMinions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

                if (AllMinions.Any())
                {

                    if (Environment.TickCount - CardTickCount < 250) return;

                    CardTickCount = Environment.TickCount;
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                    {
                        W.Cast();
                        return;
                    }
                    if (Config.Item("TwistedFate.UseWBlueLaneClear").GetValue<bool>() && Player.ManaPercent <= WMinMana)
                    {
                        if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                        {
                            W.Cast();
                        }
                    }
                    else


                        switch (Config.Item("TwistedFate.UseWLaneClearOption").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                    {
                                        var hits = 1;
                                        Obj_AI_Base target = null;
                                        var minions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                                        foreach (var minion in minions)
                                        {
                                            var count = minions.Count(m => m.NetworkId != minion.NetworkId && m.Distance(minion) < 200);
                                            if (count > hits)
                                            {
                                                hits = count;
                                                target = minion;
                                            }
                                        }

                                        if (target != null && hits >= Config.Item("TwistedFate.WMiniEnemyLaneClear").GetValue<Slider>().Value)
                                        {
                                            W.Cast(target);
                                        }
                                    }

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                                    {
                                        var hits = 1;
                                        Obj_AI_Base target = null;
                                        var minions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                                        foreach (var minion in minions)
                                        {
                                            var count = minions.Count(m => m.NetworkId != minion.NetworkId && m.Distance(minion) < 200);
                                            if (count > hits)
                                            {
                                                hits = count;
                                                target = minion;
                                            }
                                        }

                                        if (hits < Config.Item("TwistedFate.WMiniEnemyLaneClear").GetValue<Slider>().Value)
                                        {
                                            W.Cast();
                                        }
                                    }
                                    break;
                                }

                            case 1:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                        }
                }
            }

        }
        #endregion

        #region JungleClear
        public static void JungleClear()
        {

            var useQ = Config.Item("TwistedFate.UseQJungleClear").GetValue<bool>();
            var useW = Config.Item("TwistedFate.UseWJungleClear").GetValue<bool>();

            var QMinMana = Config.Item("TwistedFate.QMiniManaJungleClear").GetValue<Slider>().Value;
            var WMinMana = Config.Item("TwistedFate.WMiniManaJungleClear").GetValue<Slider>().Value;

            var MinionN = MinionManager.GetMinions(800, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!MinionN.IsValidTarget() || MinionN == null)
            {
                LaneClear();
                return;
            }

            if (Config.Item("TwistedFate.SafeJungleClear").GetValue<bool>() && Player.CountEnemiesInRange(1500) > 0) return;

            if (useQ && Q.IsReady() && Player.Mana >= QMANA && Player.ManaPercent >= QMinMana)
            {
                var allMonsterQ = MinionManager.GetMinions(Player.Position, 660, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                var farmAll = Q.GetLineFarmLocation(allMonsterQ, Q.Width);
                if (farmAll.MinionsHit >= 1)
                {
                    Q.Cast(farmAll.Position, true);
                }
            }

            if (useW && W.IsReady() && Player.Mana >= WMANA)
            {
                var AllMinions = MinionManager.GetMinions(Player.Position, 660, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

                if (AllMinions.Any())
                {

                    if (Environment.TickCount - CardTickCount < 250) return;

                    CardTickCount = Environment.TickCount;
                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard")
                    {
                        W.Cast();
                        return;
                    }
                    if (Config.Item("TwistedFate.UseWBlueJungleClear").GetValue<bool>() && Player.ManaPercent <= WMinMana)
                    {
                        if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                        {
                            W.Cast();
                        }
                    }
                    else


                        switch (Config.Item("TwistedFate.UseWJungleClearOption").GetValue<StringList>().SelectedIndex)
                        {
                            case 0:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                    {
                                        var hits = 1;
                                        Obj_AI_Base target = null;
                                        var minions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.Health);
                                        foreach (var minion in minions)
                                        {
                                            var count = minions.Count(m => m.NetworkId != minion.NetworkId && m.Distance(minion) < 200);
                                            if (count > hits)
                                            {
                                                hits = count;
                                                target = minion;
                                            }
                                        }

                                        if (target != null && hits >= 2)
                                        {
                                            W.Cast(target);
                                        }
                                    }

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                    {
                                        var hits = 1;
                                        Obj_AI_Base target = null;
                                        var minions = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);
                                        foreach (var minion in minions)
                                        {
                                            var count = minions.Count(m => m.NetworkId != minion.NetworkId && m.Distance(minion) < 200);
                                            if (count > hits)
                                            {
                                                hits = count;
                                                target = minion;
                                            }
                                        }

                                        if (hits < 2)
                                        {
                                            W.Cast();
                                        }
                                    }
                                    break;
                                }

                            case 1:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "goldcardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                            case 2:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "redcardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                            case 3:
                                {

                                    if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "bluecardlock")
                                    {
                                        W.Cast();
                                    }
                                    break;
                                }

                        }
                }
            }

        }
        #endregion

        #region KillSteal
        public static void KillSteal()
        {
            var useIgniteKS = Config.Item("TwistedFate.UseIgniteKS").GetValue<bool>();
            var useQKS = Config.Item("TwistedFate.UseQKS").GetValue<bool>();
            var useWKS = Config.Item("TwistedFate.UseWKS").GetValue<bool>();

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(target => !target.IsMe && target.Team != ObjectManager.Player.Team))
            {

                if (useQKS && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) && Player.Distance(target) < Q.Range && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                    return;
                }

                if (useWKS && (W.IsReady() || Player.HasBuff("Pick A Card")) && Player.Mana >= WMANA && target.Health < W.GetDamage(target) && Player.Distance(target) < 660 && !target.IsDead && target.IsValidTarget())
                {
                    W.Cast();
                    return;
                }

                if (useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) > target.Health && target.IsValidTarget(Ignite.Range))
                {
                    Ignite.Cast(target, true);
                }

                if (useQKS && useWKS && Q.IsReady() && (W.IsReady() || Player.HasBuff("Pick A Card")) && Player.Mana >= QMANA + WMANA && target.Health < W.GetDamage(target) + Q.GetDamage(target) && Player.Distance(target) < 660 && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                    return;
                }

                if (useQKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Q.IsReady() && Player.Mana >= QMANA && target.Health < Q.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < 600 && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                    return;
                }

                if (useWKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && (W.IsReady() || Player.HasBuff("Pick A Card")) && Player.Mana >= WMANA && target.Health < W.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < 600 && !target.IsDead && target.IsValidTarget())
                {
                    W.Cast();
                    return;
                }

                if (useQKS && useWKS && useIgniteKS && Ignite.Slot != SpellSlot.Unknown && Q.IsReady() && (W.IsReady() || Player.HasBuff("Pick A Card")) && Player.Mana >= QMANA + WMANA && target.Health < Q.GetDamage(target) + W.GetDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) && Player.Distance(target) < 600 && !target.IsDead && target.IsValidTarget())
                {
                    Q.CastIfHitchanceEquals(target, HitChance.VeryHigh, true);
                    return;
                }

            }
        }
        #endregion

        #region PlayerDamage
        public static float getComboDamage(AIHeroClient target)
        {
            float damage = 0f;
            if (Config.Item("TwistedFate.UseQCombo").GetValue<bool>())
            {
                if (Player.Mana >= QMANA)
                {
                    damage += Q.GetDamage(target) * 1.5f;
                }
            }
            if (Config.Item("TwistedFate.UseWCombo").GetValue<bool>())
            {
                if (Player.Mana >= QMANA + WMANA)
                {
                    damage += W.GetDamage(target);
                }
            }
            if (Ignite.Slot != SpellSlot.Unknown)
            {
                damage += (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }
        #endregion

        #region Interupt Spell List
        public static double ShouldUse(string SpellName)
        {
            if (SpellName == "KatarinaR")
                return 0;
            if (SpellName == "AlZaharNetherGrasp")
                return 0;
            if (SpellName == "GalioIdolOfDurand")
                return 0;
            if (SpellName == "LuxMaliceCannon")
                return 0;
            if (SpellName == "MissFortuneBulletTime")
                return 0;
            if (SpellName == "CaitlynPiltoverPeacemaker")
                return 0;
            if (SpellName == "EzrealTrueshotBarrage")
                return 0;
            if (SpellName == "InfiniteDuress")
                return 0;
            if (SpellName == "VelkozR")
                return 0;
            if (SpellName == "XerathLocusOfPower2")
                return 0;
            if (SpellName == "Drain")
                return 0;
            if (SpellName == "Crowstorm")
                return 0;
            if (SpellName == "ReapTheWhirlwind")
                return 0;
            if (SpellName == "FallenOne")
                return 0;
            if (SpellName == "JudicatorIntervention")
                return 0;
            if (SpellName == "KennenShurikenStorm")
                return 0;
            if (SpellName == "LucianR")
                return 0;
            if (SpellName == "SoulShackles")
                return 0;
            if (SpellName == "NamiQ")
                return 0;
            if (SpellName == "AbsoluteZero")
                return 0;
            if (SpellName == "Pantheon_GrandSkyfall_Jump")
                return 0;
            if (SpellName == "RivenMartyr")
                return 0;
            if (SpellName == "RivenTriCleave_03")
                return 0;
            if (SpellName == "RunePrison")
                return 0;
            if (SpellName == "SkarnerImpale")
                return 0;
            if (SpellName == "UndyingRage")
                return 0;
            if (SpellName == "VarusQ")
                return 0;
            if (SpellName == "MonkeyKingSpinToWin")
                return 0;
            if (SpellName == "YasuoRKnockUpComboW")
                return 0;
            if (SpellName == "ZacE")
                return 0;
            if (SpellName == "ZacR")
                return 0;
            if (SpellName == "UrgotSwap2")
                return 0;
            return -1;
        }
        # endregion

        #region ManaManager
        public static void ManaManager()
        {

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;
            RMANA = R.Instance.SData.Mana;

            if (ObjectManager.Player.Health < ObjectManager.Player.MaxHealth * 0.2)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
            }
        }
        #endregion

        #region PotionManager
        public static void PotionManager()
        {
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 1 && Player.Health >= Player.MaxHealth * 0.35) return;
            if (Player.Level == 1 && Player.CountEnemiesInRange(1000) == 2 && Player.Health >= Player.MaxHealth * 0.50) return;

            if (Config.Item("TwistedFate.AutoPotion").GetValue<bool>() && !Player.InFountain() && !Player.IsRecalling() && !Player.IsDead)
            {
                #region BiscuitofRejuvenation
                if (BiscuitofRejuvenation.IsReady() && !Player.HasBuff("ItemMiniRegenPotion") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxHealth > Player.Health + 170 && Player.MaxMana > Player.Mana + 10 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Health < Player.MaxHealth * 0.75)
                    {
                        BiscuitofRejuvenation.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 170 && Player.MaxMana > Player.Mana + 10 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Health < Player.MaxHealth * 0.6)
                    {
                        BiscuitofRejuvenation.Cast();
                    }

                }
                #endregion

                #region HealthPotion
                else if (HealthPotion.IsReady() && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxHealth > Player.Health + 150 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Health < Player.MaxHealth * 0.75)
                    {
                        HealthPotion.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 150 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Health < Player.MaxHealth * 0.6)
                    {
                        HealthPotion.Cast();
                    }

                }
                #endregion

                #region CrystallineFlask
                else if (CrystallineFlask.IsReady() && !Player.HasBuff("ItemCrystalFlask") && !Player.HasBuff("RegenerationPotion") && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemMiniRegenPotion"))
                {

                    if (Player.MaxHealth > Player.Health + 120 && Player.MaxMana > Player.Mana + 60 && Player.CountEnemiesInRange(1000) > 0 &&
                        (Player.Health < Player.MaxHealth * 0.85 || Player.Mana < Player.MaxMana * 0.65))
                    {
                        CrystallineFlask.Cast();
                    }

                    else if (Player.MaxHealth > Player.Health + 120 && Player.MaxMana > Player.Mana + 60 && Player.CountEnemiesInRange(1000) == 0 &&
                        (Player.Health < Player.MaxHealth * 0.7 || Player.Mana < Player.MaxMana * 0.5))
                    {
                        CrystallineFlask.Cast();
                    }

                }
                #endregion

                #region ManaPotion
                else if (ManaPotion.IsReady() && !Player.HasBuff("FlaskOfCrystalWater") && !Player.HasBuff("ItemCrystalFlask"))
                {

                    if (Player.MaxMana > Player.Mana + 100 && Player.CountEnemiesInRange(1000) > 0 &&
                        Player.Mana < Player.MaxMana * 0.7)
                    {
                        ManaPotion.Cast();
                    }

                    else if (Player.MaxMana > Player.Mana + 100 && Player.CountEnemiesInRange(1000) == 0 &&
                        Player.Mana < Player.MaxMana * 0.4)
                    {
                        ManaPotion.Cast();
                    }

                }
                #endregion
            }
        }
        #endregion

        #region DrawingRange
        public static void Drawing_OnDraw(EventArgs args)
        {

            foreach (var spell in SpellList)
            {
                var menuItem = Config.Item(spell.Slot + "Range").GetValue<Circle>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);
            }

            if (Config.Item("DrawOrbwalkTarget").GetValue<bool>())
            {
                var orbT = Orbwalker.GetTarget();
                if (orbT.IsValidTarget())
                    Render.Circle.DrawCircle(orbT.Position, 100, System.Drawing.Color.Pink);
            }

        }
        #endregion

        #region Up Spell
        private static void LevelUpSpells()
        {
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
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
        #endregion

    }

}
