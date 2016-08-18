/**
 * 
 * Love Ya Lads!
 * 
 * 
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Data;
using SharpDX;
using SebbyLib;
using Color = System.Drawing.Color;
using EloBuddy;

namespace SurvivorRyze
{
    class Program
    {

        #region Declaration
        private static Spell Q, W, E, R;
        private static SpellSlot IgniteSlot;
        private static Items.Item HealthPot;
        private static Items.Item BiscuitOfRej;
        private static Items.Item TearOfGod;
        private static Items.Item Seraph;
        private static Items.Item Manamune;
        private static Items.Item Archangel;
        private static Items.Item Flask;
        private static Items.Item Flask1;
        private static Items.Item Flask2;
        private static Items.Item HexProtobelt;
        private static Items.Item HexGunBlade;
        private static Items.Item HexGLP;
        private static SebbyLib.Orbwalking.Orbwalker Orbwalker;
        private static Menu Menu;
        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        private const string ChampionName = "Ryze";
        private static int lvl1, lvl2, lvl3, lvl4;
        private static float RangeR;
        #endregion

        public static void Game_OnGameLoad()
        {
            if (Player.ChampionName != ChampionName)
                return;

            #region Spells
            Q = new Spell(SpellSlot.Q, 1000f);
            Q.SetSkillshot(0.7f, 55f, float.MaxValue, true, SkillshotType.SkillshotLine);
            W = new Spell(SpellSlot.W, 610f);
            W.SetTargetted(0.103f, 550f);
            E = new Spell(SpellSlot.E, 610f);
            E.SetTargetted(.5f, 550f);
            R = new Spell(SpellSlot.R);
            R.SetSkillshot(2.5f, 450f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            #endregion

            #region Items/SummonerSpells
            IgniteSlot = Player.GetSpellSlot("summonerdot");
            HealthPot = new Items.Item(2003, 0);
            BiscuitOfRej = new Items.Item(2010, 0);
            TearOfGod = new Items.Item(3070, 0);
            Manamune = new Items.Item(3004, 0);
            Seraph = new Items.Item(3040, 0);
            Archangel = new Items.Item(3003, 0);
            Flask = new Items.Item(2031, 0);
            Flask1 = new Items.Item(2032, 0);
            Flask2 = new Items.Item(2033, 0);
            HexGunBlade = new Items.Item(3146, 600f);
            HexProtobelt = new Items.Item(3152, 300f);
            HexGLP = new Items.Item(3030, 300f);
            #endregion

            #region Menu
            Menu = new Menu("SurvivorRyze", "SurvivorRyze", true).SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.AliceBlue);

            Menu OrbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new SebbyLib.Orbwalking.Orbwalker(OrbwalkerMenu);

            Menu TargetSelectorMenu = Menu.AddSubMenu(new Menu("Target Selector", "TargetSelector"));
            TargetSelector.AddToMenu(TargetSelectorMenu);

            Menu ComboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            ComboMenu.AddItem(new MenuItem("ComboMode", "Combo Mode:").SetValue(new StringList(new[] {"Burst", "Survivor Mode (Shield)"})).SetTooltip("Survivor Mode - Will try to stack Shield 99% of the time."));
            ComboMenu.AddItem(new MenuItem("CUseQ", "Cast Q").SetValue(true));
            ComboMenu.AddItem(new MenuItem("CUseW", "Cast W").SetValue(true));
            ComboMenu.AddItem(new MenuItem("CUseE", "Cast E").SetValue(true));
            ComboMenu.AddItem(new MenuItem("CBlockAA", "Block AA in Combo Mode").SetValue(true));
            ComboMenu.AddItem(new MenuItem("Combo2TimesMana", "Champion needs to have mana for atleast 2 times (Q/W/E)?").SetValue(true).SetTooltip("If it's set to 'false' it'll need atleast mana for Q/W/E [1x] Post in thread if needs a change"));
            ComboMenu.AddItem(new MenuItem("CUseR", "Ultimate (R) in Ultimate Menu"));
            ComboMenu.AddItem(new MenuItem("CUseIgnite", "Use Ignite (Smart)").SetValue(true));

            Menu HarassMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            HarassMenu.AddItem(new MenuItem("HarassQ", "Use Q").SetValue(true));
            HarassMenu.AddItem(new MenuItem("HarassW", "Use W").SetValue(false));
            HarassMenu.AddItem(new MenuItem("HarassE", "Use E").SetValue(false));
            HarassMenu.AddItem(new MenuItem("HarassManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1, 100)));

            Menu LaneClearMenu = Menu.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            LaneClearMenu.AddItem(new MenuItem("UseQLC", "Use Q to LaneClear").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("UseELC", "Use E to LaneClear").SetValue(true));
            LaneClearMenu.AddItem(new MenuItem("LaneClearManaManager", "Mana Manager (%)").SetValue(new Slider(30, 1, 100)));

            Menu ItemsMenu = Menu.AddSubMenu(new Menu("Items Menu", "ItemsMenu"));
            ItemsMenu.AddItem(new MenuItem("UseHPPotion", "Use HP Potion/Biscuit/Flask at % Health").SetValue(new Slider(15, 0, 100)));
            ItemsMenu.AddItem(new MenuItem("UseItemFlask", "Use Flasks If You don't have Potions?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("UsePotionOnlyIfEnemiesAreInRange", "Use Potions/Flasks only if Enemies are nearby?").SetValue(true).SetTooltip("It'll use the potions/flask if enemies are within the e.g (~R Range)"));
            ItemsMenu.AddItem(new MenuItem("UseSeraph", "Use [Seraph's Embrace]?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("UseSeraphIfEnemiesAreNearby", "Use [Seraph's Embrace] only if Enemies are nearby?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("UseSeraphAtHP", "Activate [Seraph's Embrace] at HP %?").SetValue(new Slider(15, 0, 100)));
            ItemsMenu.AddItem(new MenuItem("UseHexGunBlade", "Use [Hextech Gunblade]?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("UseHexProtobelt", "Use [Hextech Protobelt-01]?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("UseHexGLP", "Use [Hextech GLP-800]?").SetValue(true));
            ItemsMenu.AddItem(new MenuItem("HexGunBladeAtHP", "Use [Hextech Gunblade] at HP %?").SetValue(new Slider(25, 0, 100)));
            ItemsMenu.AddItem(new MenuItem("HexProtobeltAtHP", "Use [Hextech Protobelt-01] at HP %?").SetValue(new Slider(25, 0, 100)));
            ItemsMenu.AddItem(new MenuItem("HexGLPAtHP", "Use [Hextech GLP-800] at HP %?").SetValue(new Slider(25, 0, 100)));
            ItemsMenu.AddItem(new MenuItem("StackTear", "Stack Tear/Manamune/Archangel in Fountain?").SetValue(true).SetTooltip("Stack it in Fountain?"));
            ItemsMenu.AddItem(new MenuItem("StackTearNF", "Stack Tear/Manamune/Archangel if You've Blue Buff?").SetValue(true));

            Menu SkinMenu = Menu.AddSubMenu(new Menu("Skins Menu", "SkinMenu"));
            SkinMenu.AddItem(new MenuItem("SkinID", "Skin ID")).SetValue(new Slider(3, 0, 9));
            var UseSkin = SkinMenu.AddItem(new MenuItem("UseSkin", "Enabled")).SetValue(true);
            UseSkin.ValueChanged += (sender, eventArgs) =>
            {
                if (!eventArgs.GetNewValue<bool>())
                {
                    //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
                }
            };

            Menu HitChanceMenu = Menu.AddSubMenu(new Menu("HitChance Menu", "HitChance"));
            HitChanceMenu.AddItem(new MenuItem("HitChance", "Hit Chance").SetValue(new StringList(new[] { "Medium", "High", "Very High" }, 0)));

            Menu UltimateMenu = Menu.AddSubMenu(new Menu("Ultimate Menu", "UltMenu"));
            //UltimateMenu.AddItem(new MenuItem("DontREnemyCount", "Don't R If Enemy In 'X' Range").SetValue(new Slider(1000, 0, 2000)));
            //UltimateMenu.AddItem(new MenuItem("DontRIfAlly", "Don't R if Ally is Near Target 'X' Range").SetValue(new Slider(700, 0, 2000)));
            //UltimateMenu.AddItem(new MenuItem("DontRUnderTurret", "Don't use R if enemy is Under Turret").SetValue(true));
            UltimateMenu.AddItem(new MenuItem("UseR", "Use R Automatically (Beta)").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Press)).SetTooltip("It'll Use the Ultimate if there's Ally turret nearby to teleport you to it"));
            //UltimateMenu.AddItem(new MenuItem("EnemiesAroundTarget", "Dont R If 'X' Enemies are around the Target").SetValue(new Slider(3, 0, 5)));

            Menu MiscMenu = Menu.AddSubMenu(new Menu("Misc Menu", "MiscMenu"));
            MiscMenu.AddItem(new MenuItem("KSQ", "Use Q to KS").SetValue(true));
            MiscMenu.AddItem(new MenuItem("KSW", "Use W to KS").SetValue(true));
            MiscMenu.AddItem(new MenuItem("KSE", "Use E to KS").SetValue(true));
            MiscMenu.AddItem(new MenuItem("InterruptWithW", "Use W to Interrupt Channeling Spells").SetValue(true));
            MiscMenu.AddItem(new MenuItem("WGapCloser", "Use W on Enemy GapCloser (Irelia's Q)").SetValue(true));
            MiscMenu.AddItem(new MenuItem("ChaseWithR", "Use R to Chase (Being Added)"));
            MiscMenu.AddItem(new MenuItem("EscapeWithR", "Use R to Escape (Ultimate Menu)"));

            Menu AutoLevelerMenu = Menu.AddSubMenu(new Menu("AutoLeveler Menu", "AutoLevelerMenu"));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp", "AutoLevel Up Spells?").SetValue(true));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp1", "First: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 3)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp2", "Second: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 0)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp3", "Third: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 2)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLevelUp4", "Fourth: ").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
            AutoLevelerMenu.AddItem(new MenuItem("AutoLvlStartFrom", "AutoLeveler Start from Level: ").SetValue(new Slider(2, 6, 1)));

            Menu DrawingMenu = Menu.AddSubMenu(new Menu("Drawing", "Drawing"));
            DrawingMenu.AddItem(new MenuItem("DrawQ", "Draw Q Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawWE", "Draw W/E Range").SetValue(true));
            DrawingMenu.AddItem(new MenuItem("DrawR", "Draw R Range").SetValue(false));

            Menu.AddToMainMenu();
            #endregion

            #region DrawHPDamage
            var dmgAfterShave = new MenuItem("SurvivorRyze.DrawComboDamage", "Draw Combo Damage").SetValue(true);
            var drawFill =
                new MenuItem("SurvivorRyze.DrawColour", "Fill Color", true).SetValue(
                    new Circle(true, Color.SeaGreen));
            DrawingMenu.AddItem(drawFill);
            DrawingMenu.AddItem(dmgAfterShave);
            DrawDamage.DamageToUnit = CalculateDamage;
            DrawDamage.Enabled = dmgAfterShave.GetValue<bool>();
            DrawDamage.Fill = drawFill.GetValue<Circle>().Active;
            DrawDamage.FillColor = drawFill.GetValue<Circle>().Color;
            dmgAfterShave.ValueChanged +=
                delegate(object sender, OnValueChangeEventArgs eventArgs)
                {
                    DrawDamage.Enabled = eventArgs.GetNewValue<bool>();
                };

            drawFill.ValueChanged += delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DrawDamage.Fill = eventArgs.GetNewValue<Circle>().Active;
                DrawDamage.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };
            #endregion

            #region Subscriptions
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Interrupter2.OnInterruptableTarget += Interrupter2_OnInterruptableTarget;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            #endregion
            Chat.Print("<font color='#800040'>[SurvivorSeries] Ryze</font> <font color='#ff6600'>Loaded.</font>");
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            switch(R.Level)
            {
                case 1:
                    RangeR = 1500f;
                    break;
                case 2:
                    RangeR = 3000f;
                    break;
            }
            if (Menu.Item("DrawQ").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Aqua);
            if (Menu.Item("DrawWE").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.AliceBlue);
            if (Menu.Item("DrawR").GetValue<bool>())
                Render.Circle.DrawCircle(Player.Position, RangeR, Color.Orchid);
        }

        private static void AABlock()
        {
            Orbwalker.SetAttack(!Menu.Item("CBlockAA").GetValue<bool>());
            //SebbyLib.OktwCommon.blockAttack = Menu.Item("CBlockAA").GetValue<bool>();
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !Menu.Item("AutoLevelUp").GetValue<bool>() || ObjectManager.Player.Level < Menu.Item("AutoLvlStartFrom").GetValue<Slider>().Value)
                return;
            if (lvl2 == lvl3 || lvl2 == lvl4 || lvl3 == lvl4)
                return;
            int delay = 700;
            LeagueSharp.Common.Utility.DelayAction.Add(delay, () => LevelUp(lvl1));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 50, () => LevelUp(lvl2));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 100, () => LevelUp(lvl3));
            LeagueSharp.Common.Utility.DelayAction.Add(delay + 150, () => LevelUp(lvl4));
        }

        private static void LevelUp(int indx)
        {
            if (ObjectManager.Player.Level < 4)
            {
                if (indx == 0 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2 && ObjectManager.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (indx == 0)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (indx == 1)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (indx == 2)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (indx == 3)
                    ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static void PotionsCheck()
        {
            if (Player.HasBuffOfType(BuffType.Heal) || Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlaskJungle") || Player.HasBuff("ItemCrystalFlask") || Player.HasBuff("ItemDarkCrystalFlask"))
                return;

            if (Player.CountEnemiesInRange(1250) > 0)
            {
                if (!Player.InFountain() || !Player.IsRecalling())
                {
                    if (Player.HealthPercent < Menu.Item("UseHPPotion").GetValue<Slider>().Value && (!Player.HasBuff("HealthPotion") || !Player.HasBuff("Health Potion") || !Player.HasBuff("ItemDarkCrystalFlask") || !Player.HasBuff("ItemCrystalFlask") || !Player.HasBuff("ItemCrystalFlaskJungle") || !Player.HasBuff("ItemMiniRegenPotion")))
                    {
                        HealthPot.Cast();
                    }
                    if (Player.HealthPercent < Menu.Item("UseHPPotion").GetValue<Slider>().Value && (!Player.HasBuff("ItemCrystalFlaskJungle") || !Player.HasBuff("ItemDarkCrystalFlask") || !Player.HasBuff("ItemCrystalFlask") || !Player.HasBuff("HealthPotion") || !Player.HasBuff("Health Potion") || !Player.HasBuff("ItemMiniRegenPotion")))
                    {
                        BiscuitOfRej.Cast();
                    }
                    if (Menu.Item("UseItemFlask").GetValue<bool>() && !Player.HasBuff("ItemCrystalFlaskJungle") || !Player.HasBuff("ItemCrystalFlask") || !Player.HasBuff("ItemDarkCrystalFlask") || !Player.HasBuff("HealthPotion") || !Player.HasBuff("Health Potion") || !Player.HasBuff("ItemMiniRegenPotion"))
                    {
                        Flask.Cast();
                        Flask1.Cast();
                        Flask2.Cast();
                    }
                }
            }
        }

        private static void ItemsChecks()
        {
            // Check when you can use items (potions, ex) && Cast them (Probelt Usage please)
            if (Player.HealthPercent < Menu.Item("UseSeraphAtHP").GetValue<Slider>().Value && Menu.Item("UseSeraph").GetValue<bool>() && !Player.InFountain() || !Player.IsRecalling())
            {
                if (Player.HealthPercent < Menu.Item("UseSeraphAtHP").GetValue<Slider>().Value && Menu.Item("UseSeraph").GetValue<bool>() && Player.CountEnemiesInRange(1000) > 0 && Menu.Item("UseSeraphIfEnemiesAreNearby").GetValue<bool>())
                {
                        Seraph.Cast();
                }
            }
            var target = TargetSelector.GetTarget(600f, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if (target == null || !target.IsValidTarget(600f) || target.IsInvulnerable)
                return;

            if (Menu.Item("UseHexGunBlade").GetValue<bool>() && target.IsValidTarget(600) && target.HealthPercent < Menu.Item("HexGunBladeAtHP").GetValue<Slider>().Value)
            {
                Items.UseItem(3146, target);
            }
            if (Menu.Item("UseHexProtobelt").GetValue<bool>() && target.IsValidTarget(300) && target.HealthPercent < Menu.Item("HexProtobeltAtHP").GetValue<Slider>().Value)
            {
                Items.UseItem(3152, target.Position);
            }
            if (Menu.Item("UseHexGLP").GetValue<bool>() && target.IsValidTarget(300) && target.HealthPercent < Menu.Item("HexGLPAtHP").GetValue<Slider>().Value)
            {
                Items.UseItem(3030, target.Position);
            }
        }

        private static void StackItems()
        {
            if (Player.InFountain() || Player.HasBuff("CrestoftheAncientGolem") && (Orbwalker.ActiveMode == SebbyLib.Orbwalking.OrbwalkingMode.None) && Menu.Item("StackTearNF").GetValue<bool>()) // Add if Player has Blue Buff
            {
                if (Items.HasItem(3004, Player) || Items.HasItem(3003, Player) || Items.HasItem(3070, Player) || Items.HasItem(3072, Player) || Items.HasItem(3073, Player) || Items.HasItem(3008, Player))
                {
                    Q.Cast(Player.ServerPosition);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || Player.IsRecalling())
                return;

            if (Menu.Item("StackTear").GetValue<bool>())
            {
                StackItems();
            }
            ItemsChecks();
            KSCheck();
            PotionsCheck();
            if (Menu.Item("UseSkin").GetValue<bool>())
            {
                ////Player.SetSkin(Player.CharData.BaseSkinName, Menu.Item("SkinID").GetValue<Slider>().Value);
            }
            switch (Orbwalker.ActiveMode)
            {
                case SebbyLib.Orbwalking.OrbwalkingMode.Combo:
                        AABlock();
                        Combo();
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.Mixed:
                        Harass();
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.LaneClear:
                        LaneClear();
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.LastHit:
                        LastHit();
                    break;
                case SebbyLib.Orbwalking.OrbwalkingMode.None:
                        Orbwalker.SetMovement(true);
                        Orbwalker.SetAttack(true);
                    break;
            }
            if (Menu.Item("UseR").GetValue<KeyBind>().Active)
            {
                Orbwalker.SetMovement(false);
                Orbwalker.SetAttack(false);
                REscape();
            }
            //AutoLeveler
            if (Menu.Item("AutoLevelUp").GetValue<bool>())
            {
                lvl1 = Menu.Item("AutoLevelUp1").GetValue<StringList>().SelectedIndex;
                lvl2 = Menu.Item("AutoLevelUp2").GetValue<StringList>().SelectedIndex;
                lvl3 = Menu.Item("AutoLevelUp3").GetValue<StringList>().SelectedIndex;
                lvl4 = Menu.Item("AutoLevelUp4").GetValue<StringList>().SelectedIndex;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (!Menu.Item("WGapCloser").GetValue<bool>() || Player.Mana < W.Instance.SData.Mana + Q.Instance.SData.Mana)
                return;

            var t = gapcloser.Sender;

            if (gapcloser.End.Distance(Player.ServerPosition) < W.Range)
            {
                W.Cast(t);
            }
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient t, Interrupter2.InterruptableTargetEventArgs args)
        {
            var WCast = Menu.Item("InterruptWithW").GetValue<bool>();
            if (!WCast || !t.IsValidTarget(W.Range) || !W.IsReady()) return;
            W.Cast(t);
        }

        private static void KSCheck()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if (target == null || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            var ksQ = Menu.Item("KSQ").GetValue<bool>();
            var ksW = Menu.Item("KSW").GetValue<bool>();
            var ksE = Menu.Item("KSE").GetValue<bool>();

            #region SebbyPrediction
            //SebbyPrediction
            SebbyLib.Prediction.SkillshotType PredSkillShotType = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            bool Aoe10 = false;

            var predictioninput = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = Aoe10,
                Collision = Q.Collision,
                Speed = Q.Speed,
                Delay = Q.Delay,
                Range = Q.Range,
                From = Player.ServerPosition,
                Radius = Q.Width,
                Unit = target,
                Type = PredSkillShotType
            };
            //SebbyPrediction END
            #endregion
            // Input = 'var predictioninput'
            var predpos = SebbyLib.Prediction.Prediction.GetPrediction(predictioninput);

            // KS
            if (ksQ && SebbyLib.OktwCommon.GetKsDamage(target, Q) > target.Health && target.IsValidTarget(Q.Range))
            {
                if (target.CanMove && predpos.Hitchance >= SebbyLib.Prediction.HitChance.High)
                {
                    Q.Cast(predpos.CastPosition);
                }
                else if (!target.CanMove)
                {
                    Q.Cast(target.Position);
                }
            }
            if (ksW && SebbyLib.OktwCommon.GetKsDamage(target, W) > target.Health && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }
            if (ksE && SebbyLib.OktwCommon.GetKsDamage(target, E) > target.Health && target.IsValidTarget(E.Range))
            {
                E.CastOnUnit(target);
            }
        }

        public static bool RyzeCharge0()
        {
            return Player.HasBuff("ryzeqiconnocharge");
        }

        public static bool RyzeCharge1()
        {
            return Player.HasBuff("ryzeqiconhalfcharge");
        }

        public static bool RyzeCharge2()
        {
            return Player.HasBuff("ryzeqiconfullcharge");
        }

        private static void SebbySpell(Spell QR, Obj_AI_Base target)
        {
            SebbyLib.Prediction.SkillshotType CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotLine;
            bool aoe2 = false;

            if (QR.Type == SkillshotType.SkillshotCircle)
            {
                CoreType2 = SebbyLib.Prediction.SkillshotType.SkillshotCircle;
                aoe2 = true;
            }

            if (QR.Width > 80 && !QR.Collision)
                aoe2 = true;

            var predInput2 = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = aoe2,
                Collision = QR.Collision,
                Speed = QR.Speed,
                Delay = QR.Delay,
                Range = QR.Range,
                From = Player.ServerPosition,
                Radius = QR.Width,
                Unit = target,
                Type = CoreType2
            };
            var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(predInput2);

            if (QR.Speed != float.MaxValue && OktwCommon.CollisionYasuo(Player.ServerPosition, poutput2.CastPosition))
                return;

            if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 0)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.Medium)
                    QR.Cast(poutput2.CastPosition);
            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 1)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.High)
                    QR.Cast(poutput2.CastPosition);

            }
            else if (Menu.Item("HitChance").GetValue<StringList>().SelectedIndex == 2)
            {
                if (poutput2.Hitchance >= SebbyLib.Prediction.HitChance.VeryHigh)
                    QR.Cast(poutput2.CastPosition);
            }
        }

        private static void Combo()
        {
            // Combo
            var CUseQ = Menu.Item("CUseQ").GetValue<bool>();
            var CUseW = Menu.Item("CUseW").GetValue<bool>();
            var CUseE = Menu.Item("CUseE").GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if (target == null || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            switch (Menu.Item("ComboMode").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    #region Burst Mode
                    // Execute the Lad
                    if (Menu.Item("CUseIgnite").GetValue<bool>() && target.Health < SebbyLib.OktwCommon.GetIncomingDamage(target) + Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                    {
                        Player.Spellbook.CastSpell(IgniteSlot, target);
                    }
                    if (Menu.Item("Combo2TimesMana").GetValue<bool>())
                    {
                        if (Player.Mana >= 2 * (Q.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana))
                        {
                            if (CUseQ && CUseW && CUseE && target.IsValidTarget(Q.Range))
                            {
                                if (target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                                else if (!target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                                if (target.IsValidTarget(W.Range) && W.IsReady())
                                {
                                    W.CastOnUnit(target);
                                }
                                if (target.IsValidTarget(E.Range) && E.IsReady())
                                {
                                    E.CastOnUnit(target);
                                }
                            }
                            if (CUseW && target.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (CUseQ && target.IsValidTarget(Q.Range))
                            {
                                if (target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                                else if (!target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                            }
                            if (CUseE && target.IsValidTarget(E.Range) && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }
                        }
                    }
                    else
                    {
                        if (Player.Mana >= Q.Instance.SData.Mana + W.Instance.SData.Mana + E.Instance.SData.Mana)
                        {
                            if (CUseW && target.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (CUseQ && target.IsValidTarget(Q.Range))
                            {
                                if (target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                                else if (!target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                            }
                            if (CUseE && target.IsValidTarget(E.Range) && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }
                        }
                        else
                        {
                            if (CUseW && target.IsValidTarget(W.Range) && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (CUseQ && target.IsValidTarget(Q.Range))
                            {
                                if (target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                                else if (!target.CanMove)
                                {
                                    SebbySpell(Q, target);
                                }
                            }
                            if (CUseE && target.IsValidTarget(E.Range) && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }
                        }
                    }
                    #endregion
                    break;

                case 1:
                    #region SurvivorMode
                    if (Q.Level >= 1 && W.Level >= 1 && E.Level >= 1)
                    {
                        if (!target.IsValidTarget(W.Range - 15f) && Q.IsReady())
                        {
                            if (target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                            else if (!target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                        }
                        // Try having Full Charge if either W or E spells are ready... :pokemon:
                        if (RyzeCharge1() && Q.IsReady() && (W.IsReady() || E.IsReady()))
                        {
                            if (E.IsReady())
                            {
                                E.Cast(target);
                            }
                            if (W.IsReady())
                            {
                                W.Cast(target);
                            }
                        }
                        // Rest in Piece XDDD
                        if (RyzeCharge1() && !E.IsReady() && !W.IsReady())
                        {
                            if (target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                            else if (!target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                        }

                        if (RyzeCharge0() && !E.IsReady() && !W.IsReady())
                        {
                            if (target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                            else if (!target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                        }

                        if (!RyzeCharge2())
                        {
                            E.Cast(target);
                            W.Cast(target);
                        }
                        else
                        {
                            if (target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                            else if (!target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                        }
                    }
                    else
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady())
                        {
                            if (target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                            else if (!target.CanMove)
                            {
                                SebbySpell(Q, target);
                            }
                        }

                        if (target.IsValidTarget(W.Range) && W.IsReady())
                        {
                            W.Cast(target);
                        }

                        if (target.IsValidTarget(E.Range) && E.IsReady())
                        {
                            E.Cast(target);
                        }
                    }
                    #endregion
                    break;
            }
        }

        private static void Harass()
        {
            // Harass
            var HarassUseQ = Menu.Item("HarassQ").GetValue<bool>();
            var HarassUseW = Menu.Item("HarassW").GetValue<bool>();
            var HarassUseE = Menu.Item("HarassE").GetValue<bool>();
            // Checks
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            // If Target's not in Q Range or there's no target or target's invulnerable don't fuck with him
            if (target == null || !target.IsValidTarget(Q.Range) || target.IsInvulnerable)
                return;

            // Execute the Lad
            if (Player.ManaPercentage() > Menu.Item("HarassManaManager").GetValue<Slider>().Value)
            {
                if (HarassUseW && target.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(target);
                }
                if (HarassUseQ && target.IsValidTarget(Q.Range))
                {
                    if (target.CanMove)
                    {
                        SebbySpell(Q, target);
                    }
                    else if (!target.CanMove)
                    {
                        SebbySpell(Q, target);
                    }
                }
                if (HarassUseE && target.IsValidTarget(W.Range))
                {
                    E.CastOnUnit(target);
                }
            }
        }

        private static void LastHit()
        {
            // To be Done
            if (Player.ManaPercentage() > Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
            {
                var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy);
                if (Q.IsReady())
                {
                    if (allMinionsQ.Count > 0)
                    {
                        foreach (var minion in allMinionsQ)
                        {
                            if (!minion.IsValidTarget() || minion == null)
                                return;
                            if (minion.Health < Q.GetDamage(minion))
                                Q.Cast(minion.Position);
                            else if (minion.Health < Q.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && minion.IsValidTarget(SebbyLib.Orbwalking.GetRealAutoAttackRange(minion)))
                            {
                                Q.Cast(minion.Position);
                                Orbwalker.ForceTarget(minion);
                            }
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            // LaneClear | Notes: Rework on early levels not using that much abilities since Spell Damage is lower, higher Lvl is fine
            if (Menu.Item("UseQLC").GetValue<bool>() || Menu.Item("UseELC").GetValue<bool>())
            {
                if (Player.ManaPercentage() > Menu.Item("LaneClearManaManager").GetValue<Slider>().Value)
                {
                    var ryzeebuffed = MinionManager.GetMinions(Player.Position, Q.Range).Find(x => x.HasBuff("RyzeE") && x.IsValidTarget(Q.Range));
                    var ryzenotebuffed = MinionManager.GetMinions(Player.Position, Q.Range).Find(x => !x.HasBuff("RyzeE") && x.IsValidTarget(Q.Range));
                    var allMinionsQ = Cache.GetMinions(Player.ServerPosition, Q.Range, MinionTeam.Enemy);
                    var allMinions = Cache.GetMinions(Player.ServerPosition, E.Range, MinionTeam.Enemy);
                    if (Q.IsReady() && !E.IsReady())
                    {
                        if (allMinionsQ.Count > 0)
                        {
                            foreach (var minion in allMinionsQ)
                            {
                                if (!minion.IsValidTarget() || minion == null)
                                    return;
                                if (minion.Health < Q.GetDamage(minion))
                                    Q.Cast(minion);
                                else if (minion.Health < Q.GetDamage(minion) + Player.GetAutoAttackDamage(minion) && minion.IsValidTarget(SebbyLib.Orbwalking.GetRealAutoAttackRange(minion)))
                                {
                                    Q.Cast(minion);
                                    Orbwalker.ForceTarget(minion);
                                }
                            }
                        }
                    }
                    if (!Q.IsReady() && E.IsReady())
                    {
                        if (ryzeebuffed != null)
                        {
                            if (ryzeebuffed.Health < E.GetDamage(ryzeebuffed) + Q.GetDamage(ryzeebuffed) && ryzeebuffed.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(ryzeebuffed);
                                if (Q.IsReady())
                                    Q.Cast(ryzeebuffed);

                                Orbwalker.ForceTarget(ryzeebuffed);
                            }
                        }
                        else if (ryzeebuffed == null)
                        {
                            foreach (var minion in allMinions)
                            {
                                if (minion.IsValidTarget(E.Range) && minion.Health < E.GetDamage(minion) + Q.GetDamage(minion))
                                {
                                    E.CastOnUnit(minion);
                                    if (Q.IsReady())
                                        Q.Cast(ryzeebuffed);
                                }
                            }
                        }
                    }
                    if (Q.IsReady() && E.IsReady())
                    {
                        if (ryzeebuffed != null)
                        {
                            if (ryzeebuffed.Health < Q.GetDamage(ryzeebuffed) + E.GetDamage(ryzeebuffed) + Q.GetDamage(ryzeebuffed) && ryzeebuffed.IsValidTarget(E.Range))
                            {
                                Q.Cast(ryzeebuffed);
                                if (ryzeebuffed.IsValidTarget(E.Range))
                                {
                                    E.CastOnUnit(ryzeebuffed);
                                }
                                if (!E.IsReady() && Q.IsReady())
                                    Q.Cast(ryzeebuffed);
                            }
                        }
                        else if (ryzeebuffed == null)
                        {
                            Q.Cast(ryzeebuffed);
                            if (ryzenotebuffed.IsValidTarget(E.Range))
                            {
                                Orbwalker.ForceTarget(ryzenotebuffed);
                                E.CastOnUnit(ryzenotebuffed);
                            }
                            if (!E.IsReady() && Q.IsReady())
                                Q.Cast(ryzenotebuffed);
                        }
                    }
                }
            }
        } // LaneClear End

        private static void REscape()
        {
            switch (R.Level)
            {
                case 1:
                    RangeR = 1500f;
                    break;
                case 2:
                    RangeR = 3000f;
                    break;
            }
            var NearByTurrets = ObjectManager.Get<Obj_AI_Turret>().Find(turret => turret.Distance(Player) < RangeR && turret.IsAlly);
            if (NearByTurrets != null)
            {
                R.Cast(NearByTurrets.Position);
            }
        }

        //RUsage

        private static float CalculateDamage(Obj_AI_Base enemy)
        {
            float damage = 0;
            if (Q.IsReady() || Player.Mana <= Q.Instance.SData.Mana + Q.Instance.SData.Mana)
                damage += Q.GetDamage(enemy) + Q.GetDamage(enemy);
            else if (Q.IsReady() || Player.Mana <= Q.Instance.SData.Mana)
                damage += Q.GetDamage(enemy);

            if (W.IsReady() || Player.Mana <= W.Instance.SData.Mana + W.Instance.SData.Mana)
                damage += W.GetDamage(enemy) + W.GetDamage(enemy);
            else if (W.IsReady() || Player.Mana <= W.Instance.SData.Mana)
                damage += W.GetDamage(enemy);

            if (E.IsReady() || Player.Mana <= E.Instance.SData.Mana + E.Instance.SData.Mana)
                damage += E.GetDamage(enemy) + E.GetDamage(enemy);
            else if (E.IsReady() || Player.Mana <= E.Instance.SData.Mana)
                damage += E.GetDamage(enemy);

            if (Menu.Item("CUseIgnite").GetValue<bool>())
            {
                damage += (float)Player.GetSummonerSpellDamage(enemy, Damage.SummonerSpell.Ignite);
            }

            return damage;
        }
    }
}