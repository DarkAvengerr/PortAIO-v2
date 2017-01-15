/*
    TODO: 

    1. it doesnt rend the big super minions would be helpful for laneclear later

    2. sometimes it doesnt rend jungle mobs for me checked everything it only rend drake and baron o.O but only sometimes

    3. it uses rend into kindred ult cost my last game... :-/

    4. color change for e damage didnt work

    5. rend minions u cant get with aa

*/
using EloBuddy; 
 using LeagueSharp.Common; 
 namespace iKalistaReborn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DZLib.MenuExtensions;
    using DZLib.Modules;

    using iKalistaReborn.Modules;
    using iKalistaReborn.Utils;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    using SPrediction;

    using Color = System.Drawing.Color;
    using Prediction = SPrediction.Prediction;

    internal class Kalista
    {
        #region Static Fields

        /// <summary>
        ///     The Modules
        /// </summary>
        public static readonly List<IModule> Modules = new List<IModule>
                                                           {
                                                               new AutoRendModule(), new JungleStealModule(), new AutoELeavingModule(), 
                                                               new WallJumpModule()
                                                           };

        private readonly List<Vector3[]> possibleJumpSpots = new List<Vector3[]>();

        public static float LastAutoAttack;

        public static Menu Menu;

        public static Orbwalking.Orbwalker Orbwalker;

        #endregion

        #region Constructors and Destructors

        public Kalista()
        {
            CreateMenu();
            LoadModules();
            Prediction.Initialize(Menu);
            PopulateList();
            //Custom//DamageIndicator.Initialize(Helper.GetRendDamage);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Spellbook.OnCastSpell += (sender, args) =>
                {
                    if (sender.Owner.IsMe && args.Slot == SpellSlot.Q && ObjectManager.Player.IsDashing())
                    {
                        args.Process = false;
                    }
                };
            Orbwalker.RegisterCustomMode("com.ikalista.flee", "Flee", "V".ToCharArray()[0]);
            Orbwalking.OnNonKillableMinion += minion =>
                {
                    var killableMinion = minion as Obj_AI_Base;
                    if (killableMinion == null || !SpellManager.Spell[SpellSlot.E].IsReady()
                        || ObjectManager.Player.HasBuff("summonerexhaust") || !killableMinion.HasRendBuff())
                    {
                        return;
                    }

                    if (Menu.Item("com.ikalista.laneclear.useEUnkillable").GetValue<bool>()
                        && killableMinion.IsMobKillable())
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                };
            Orbwalking.BeforeAttack += args =>
                {
                    if (!Menu.Item("com.ikalista.misc.forceW").GetValue<bool>()) return;

                    var target =
                        HeroManager.Enemies.FirstOrDefault(
                            x => ObjectManager.Player.Distance(x) <= 600 && x.HasBuff("kalistacoopstrikemarkally"));
                    if (target != null)
                    {
                        Orbwalker.ForceTarget(target);
                    }
                };
        }

        private void PopulateList()
        {
            // blue side wolves - left wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(2848, 6942, 53), new Vector3(3058, 6960, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3064, 6962, 52), new Vector3(2809, 6936, 53) });

            // blue side wolves - left wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(2774, 6558, 57), new Vector3(3072, 6607, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3074, 6608, 51), new Vector3(2755, 6523, 57) });

            // blue side wolves - left wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(3024, 6108, 57), new Vector3(3195, 6307, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3200, 6243, 52), new Vector3(3022, 6111, 57) });

            // red side wolves - right wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 8856, 50), new Vector3(11513, 8762, 65) });
            possibleJumpSpots.Add(new[] { new Vector3(11572, 8706, 64), new Vector3(11817, 8903, 50) });

            // red side wolves - right wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 8206, 55), new Vector3(12095, 8281, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(12072, 8256, 52), new Vector3(11755, 8206, 55) });

            // red side wolves - right wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 7906, 52), new Vector3(12110, 7980, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(12072, 7906, 53), new Vector3(11767, 7900, 52) });

            // bottom bush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11410, 5526, 23), new Vector3(11647, 5452, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(11646, 5452, 54), new Vector3(11354, 5511, 8) });

            // bottom bush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(11722, 5058, 52), new Vector3(11345, 4813, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(11428, 4984, -71), new Vector3(11725, 5120, 52) });

            // bot bush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(11772, 4608, -71), new Vector3(11960, 4802, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(11922, 4758, 51), new Vector3(11697, 4614, -71) });

            // top bush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(3074, 10056, 54), new Vector3(3437, 10186, -66) });
            possibleJumpSpots.Add(new[] { new Vector3(3324, 10206, -65), new Vector3(2964, 10012, 54) });

            // top bush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(3474, 9856, -65), new Vector3(3104, 9701, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(3226, 9752, 52), new Vector3(3519, 9833, -65) });

            // top bush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(3488, 9414, 13), new Vector3(3224, 9440, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3226, 9438, 51), new Vector3(3478, 9422, 16) });

            // mid wall - top side (top)
            possibleJumpSpots.Add(new[] { new Vector3(6524, 8856, -71), new Vector3(6685, 9116, 49) });
            possibleJumpSpots.Add(new[] { new Vector3(6664, 9002, 43), new Vector3(6484, 8804, -71) });

            // mid wall - top side (middle)
            possibleJumpSpots.Add(new[] { new Vector3(6874, 8606, -69), new Vector3(7095, 8727, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7074, 8706, 52), new Vector3(6857, 8517, -71) });

            // mid wall - top side (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(7174, 8256, -33), new Vector3(7456, 8539, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(7422, 8406, 53), new Vector3(7100, 8159, -24) });

            // mid wall - bot side (top)
            possibleJumpSpots.Add(new[] { new Vector3(7658, 6512, 5), new Vector3(7378, 6298, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7470, 6260, 52), new Vector3(7714, 6544, -1) });

            // mid wall - bot side (middle)
            possibleJumpSpots.Add(new[] { new Vector3(8034, 6198, -71), new Vector3(7813, 5938, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(7898, 6004, 51), new Vector3(8139, 6210, -71) });

            // mid wall - bot side (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(8222, 5808, 32), new Vector3(8412, 6081, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(8344, 6022, -71), new Vector3(8194, 5742, 42) });

            // baron wall
            possibleJumpSpots.Add(new[] { new Vector3(5774, 10656, 55), new Vector3(5355, 10657, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(5474, 10656, -71), new Vector3(5812, 10832, 55) });

            // baron entrance wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(4474, 10406, -71), new Vector3(4292, 10199, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(4292, 10270, -71), new Vector3(4480, 10437, -71) });

            // baron entrance wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(5074, 10006, -71), new Vector3(4993, 9706, -70) });
            possibleJumpSpots.Add(new[] { new Vector3(5000, 9754, -71), new Vector3(5083, 9998, -71) });

            // dragon wall
            possibleJumpSpots.Add(new[] { new Vector3(9322, 4358, -71), new Vector3(8971, 4284, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(9072, 4208, 53), new Vector3(9378, 4431, -71) });

            // dragon entrance wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(9812, 4918, -71), new Vector3(9803, 5249, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(9822, 5158, -71), new Vector3(9751, 4884, -71) });

            // dragon entrance wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(10422, 4458, -71), new Vector3(10643, 4641, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(10622, 4558, -71), new Vector3(10375, 4441, -71) });

            // top golllems wall
            possibleJumpSpots.Add(new[] { new Vector3(6524, 12006, 56), new Vector3(6553, 11666, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(6574, 11706, 53), new Vector3(6543, 12054, 56) });

            // bot gollems wall
            possibleJumpSpots.Add(new[] { new Vector3(8250, 2894, 51), new Vector3(8213, 3326, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(8222, 3158, 51), new Vector3(8282, 2741, 51) });

            // blue side bot tribush wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(9482, 2786, 49), new Vector3(9535, 3203, 55) });
            possibleJumpSpots.Add(new[] { new Vector3(9530, 3126, 59), new Vector3(9505, 2756, 49) });

            // blue side bot tribush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(9772, 2758, 49), new Vector3(9862, 3111, 58) });
            possibleJumpSpots.Add(new[] { new Vector3(9872, 3066, 58), new Vector3(9815, 2673, 49) });

            // blue side bot tribush wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(10206, 2888, 49), new Vector3(10046, 2675, 49) });
            possibleJumpSpots.Add(new[] { new Vector3(10022, 2658, 49), new Vector3(10259, 2925, 49) });

            // red side toplane tribush wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(5274, 11806, 57), new Vector3(5363, 12185, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(5324, 12106, 56), new Vector3(5269, 11725, 57) });

            // red side toplane tribush wall (middle)
            possibleJumpSpots.Add(new[] { new Vector3(5000, 11874, 57), new Vector3(5110, 12210, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(5072, 12146, 56), new Vector3(4993, 11836, 57) });

            // red side toplane tribush wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(4624, 12006, 57), new Vector3(4825, 12307, 56) });
            possibleJumpSpots.Add(new[] { new Vector3(4776, 12224, 56), new Vector3(4605, 11970, 57) });

            // blue side razorbeak wall
            possibleJumpSpots.Add(new[] { new Vector3(7372, 5858, 52), new Vector3(7115, 5524, 55) });
            possibleJumpSpots.Add(new[] { new Vector3(7174, 5608, 58), new Vector3(7424, 5905, 52) });

            // blue side blue buff wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(3774, 7706, 52), new Vector3(3856, 7412, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(3828, 7428, 51), new Vector3(3802, 7743, 52) });

            // blue side blue buff wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(3424, 7408, 52), new Vector3(3422, 7759, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(3434, 7722, 52), new Vector3(3437, 7398, 52) });

            // blue side blue buff - right wall
            possibleJumpSpots.Add(new[] { new Vector3(4144, 8030, 50), new Vector3(4382, 8149, 48) });
            possibleJumpSpots.Add(new[] { new Vector3(4374, 8156, 48), new Vector3(4124, 8022, 50) });

            // blue side rock between blue buff/baron (left)
            possibleJumpSpots.Add(new[] { new Vector3(4664, 8652, -10), new Vector3(4624, 9010, -68) });
            possibleJumpSpots.Add(new[] { new Vector3(4662, 8896, -69), new Vector3(4672, 8519, 26) });

            // blue side rock between blue buff/baron (right)
            possibleJumpSpots.Add(new[] { new Vector3(3774, 9206, -14), new Vector3(4074, 9322, -67) });
            possibleJumpSpots.Add(new[] { new Vector3(4024, 9306, -68), new Vector3(3737, 9233, -8) });

            // red side blue buff wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(11022, 7208, 51), new Vector3(10904, 7521, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11022, 7506, 52), new Vector3(11040, 7179, 51) });

            // red side blue buff wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(11440, 7208, 52), new Vector3(11449, 7517, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11470, 7486, 52), new Vector3(11458, 7155, 52) });

            // red side rock between blue buff/dragon (left)
            possibleJumpSpots.Add(new[] { new Vector3(10172, 6208, 16), new Vector3(10189, 5922, -71) });
            possibleJumpSpots.Add(new[] { new Vector3(10172, 5958, -71), new Vector3(10185, 6286, 29) });

            // red side rock between blue buff/dragon (right)
            possibleJumpSpots.Add(new[] { new Vector3(10722, 5658, -66), new Vector3(11049, 5660, -22) });
            possibleJumpSpots.Add(new[] { new Vector3(11022, 5658, -30), new Vector3(10665, 5662, -68) });

            // blue side top tribush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(2574, 9656, 54), new Vector3(2800, 9596, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(2774, 9656, 53), new Vector3(2537, 9674, 54) });

            // blue side top tribush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(2874, 9306, 51), new Vector3(2500, 9262, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(2598, 9272, 52), new Vector3(2884, 9291, 51) });

            // blue side wolves - right wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(4624, 5858, 51), new Vector3(4772, 5636, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(4774, 5658, 50), new Vector3(4644, 5876, 51) });

            // blue side wolves - right wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(4924, 6158, 52), new Vector3(4869, 6452, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(4874, 6408, 51), new Vector3(4938, 6062, 51) });

            // blue razorbeak - left wall
            possibleJumpSpots.Add(new[] { new Vector3(6174, 5308, 49), new Vector3(5998, 5536, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(6024, 5508, 52), new Vector3(6199, 5286, 49) });

            // red side bottom tribush wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(12260, 5220, 52), new Vector3(12027, 5265, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(12122, 5208, 54), new Vector3(12327, 5243, 52) });

            // red side bottom tribush wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(11972, 5558, 54), new Vector3(12343, 5498, 53) });
            possibleJumpSpots.Add(new[] { new Vector3(12272, 5558, 53), new Vector3(11969, 5480, 55) });

            // red side razorbeak - rightdown wall
            possibleJumpSpots.Add(new[] { new Vector3(8672, 9606, 50), new Vector3(8831, 9384, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(8830, 9382, 52), new Vector3(8646, 9635, 50) });

            // red side wolves - left wall (top)
            possibleJumpSpots.Add(new[] { new Vector3(10222, 9056, 50), new Vector3(10061, 9282, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(10072, 9306, 52), new Vector3(10193, 9052, 50) });

            // red side wolves - left wall (bottom)
            possibleJumpSpots.Add(new[] { new Vector3(9972, 8506, 68), new Vector3(9856, 8831, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(9872, 8756, 50), new Vector3(9967, 8429, 65) });

            // red size razorbeak - right wall
            possibleJumpSpots.Add(new[] { new Vector3(8072, 9806, 51), new Vector3(8369, 9807, 50) });
            possibleJumpSpots.Add(new[] { new Vector3(8372, 9806, 50), new Vector3(8066, 9796, 51) });

            // blue side base wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(4524, 3258, 96), new Vector3(4780, 3460, 51) });
            possibleJumpSpots.Add(new[] { new Vector3(4774, 3408, 51), new Vector3(4463, 3260, 96) });

            // blue side base wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(3074, 4558, 96), new Vector3(3182, 4917, 54) });
            possibleJumpSpots.Add(new[] { new Vector3(3174, 4858, 54), new Vector3(3085, 4539, 96) });

            // red side base wall (right)
            possibleJumpSpots.Add(new[] { new Vector3(11712, 10390, 91), new Vector3(11621, 10092, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(11622, 10106, 52), new Vector3(11735, 10430, 91) });

            // red base wall (left)
            possibleJumpSpots.Add(new[] { new Vector3(10308, 11682, 91), new Vector3(9999, 11554, 52) });
            possibleJumpSpots.Add(new[] { new Vector3(10022, 11556, 52), new Vector3(10321, 11664, 91) });
        }

        #endregion

        #region Methods

        /// <summary>
        ///     This is where jeff creates his first Menu in a long time
        /// </summary>
        private void CreateMenu()
        {
            Menu = new Menu("iKalista: Reborn", "com.ikalista", true);

            var targetSelector = new Menu("iKalista: Reborn - Target Selector", "com.ikalista.ts");
            TargetSelector.AddToMenu(targetSelector);
            Menu.AddSubMenu(targetSelector);

            var orbwalkerMenu = new Menu("iKalista: Reborn - Orbwalker", "com.ikalista.orbwalker");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);

            var comboMenu = new Menu("iKalista: Reborn - Combo", "com.ikalista.combo");
            {
                comboMenu.AddBool("com.ikalista.combo.useQ", "Use Q", true);
                comboMenu.AddText("--", "------------------");
                comboMenu.AddBool("com.ikalista.combo.useE", "Use E", true);
                comboMenu.AddBool("com.ikalista.combo.eLeaving", "Use E Leaving", true);
                comboMenu.AddSlider("com.ikalista.combo.ePercent", "Min Percent for E Leaving", 50, 10, 100);
                comboMenu.AddBool("com.ikalista.combo.saveMana", "Save Mana for E", true);
                comboMenu.AddBool("com.ikalista.combo.autoE", "Auto E Minion > Champion", true);
                comboMenu.AddBool("com.ikalista.combo.orbwalkMinions", "Orbwalk Minions in combo", true);
                comboMenu.AddText("---", "------------------");
                comboMenu.AddBool("com.ikalista.combo.saveAlly", "Save Ally With R", true);
                comboMenu.AddBool("com.ikalista.combo.balista", "Use Balista", true);
                comboMenu.AddSlider("com.ikalista.combo.allyPercent", "Min Health % for Ally", 20, 10, 100);
                Menu.AddSubMenu(comboMenu);
            }

            var mixedMenu = new Menu("iKalista: Reborn - Mixed", "com.ikalista.mixed");
            {
                mixedMenu.AddBool("com.ikalista.mixed.useQ", "Use Q", true);
                mixedMenu.AddBool("com.ikalista.mixed.useE", "Use E", true);
                mixedMenu.AddSlider("com.ikalista.mixed.stacks", "Rend at X stacks", 10, 1, 20);
                Menu.AddSubMenu(mixedMenu);
            }

            var laneclearMenu = new Menu("iKalista: Reborn - Laneclear", "com.ikalista.laneclear");
            {
                laneclearMenu.AddBool("com.ikalista.laneclear.useQ", "Use Q", true);
                laneclearMenu.AddSlider("com.ikalista.laneclear.qMinions", "Min Minions for Q", 3, 1, 10);
                laneclearMenu.AddBool("com.ikalista.laneclear.useE", "Use E", true);
                laneclearMenu.AddSlider("com.ikalista.laneclear.eMinions", "Min Minions for E", 5, 1, 10);
                laneclearMenu.AddBool("com.ikalista.laneclear.useEUnkillable", "E Unkillable Minions", true);
                laneclearMenu.AddBool("com.ikalista.laneclear.eSiege", "Auto E Siege Minions", true);
                Menu.AddSubMenu(laneclearMenu);
            }

            var jungleStealMenu = new Menu("iKalista: Reborn - Jungle Steal", "com.ikalista.jungleSteal");
            {
                jungleStealMenu.AddBool("com.ikalista.jungleSteal.enabled", "Use Rend To Steal Jungle Minions", true);
                jungleStealMenu.AddBool("com.ikalista.jungleSteal.small", "Kill Small Minions", true);
                jungleStealMenu.AddBool("com.ikalista.jungleSteal.large", "Kill Large Minions", true);
                jungleStealMenu.AddBool("com.ikalista.jungleSteal.legendary", "Kill Legendary Minions", true);

                Menu.AddSubMenu(jungleStealMenu);
            }

            var miscMenu = new Menu("iKalista: Reborn - Misc", "com.ikalista.misc");
            {
                miscMenu.AddBool("com.ikalista.misc.forceW", "Focus Enemy With W");
                miscMenu.AddStringList("com.ikalista.misc.damage", "Damage Type", new []{ "Common", "Custom" });
                Menu.AddSubMenu(miscMenu);
            }

            var drawingMenu = new Menu("iKalista: Reborn - Drawing", "com.ikalista.drawing");
            {
                drawingMenu.AddBool("com.ikalista.drawing.spellRanges", "Draw Spell Ranges");
                drawingMenu.AddBool("com.ikalista.drawing.junpSpots", "Draw Jump Spots", true);
                drawingMenu.AddBool("com.ikalista.drawing.shine", "Shine E Range tho", true);
                drawingMenu.AddItem(
                    new MenuItem("com.ikalista.drawing.eDamage", "Draw E Damage on Enemies").SetValue(
                        new Circle(true, Color.DarkOliveGreen)));
                drawingMenu.AddItem(
                    new MenuItem("com.ikalista.drawing.damagePercent", "Draw Percent Damage").SetValue(
                        new Circle(true, Color.DarkOliveGreen)));
                Menu.AddSubMenu(drawingMenu);
            }

            Menu.AddToMainMenu();
        }

        private void LoadModules()
        {
            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                try
                {
                    module.OnLoad();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error loading module: " + module.GetName() + " Exception: " + e);
                }
            }
        }

        private void OnCombo()
        {
            if (Menu.Item("com.ikalista.misc.exploit").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(
                    ObjectManager.Player.AttackRange, 
                    TargetSelector.DamageType.Physical);
                if (target.IsValidTarget(ObjectManager.Player.AttackRange))
                {
                    if (Environment.TickCount - LastAutoAttack <= 250) EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    if (Environment.TickCount - LastAutoAttack >= 50)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                        LastAutoAttack = Environment.TickCount;
                    }
                }
            }

            if (Menu.Item("com.ikalista.combo.orbwalkMinions").GetValue<bool>())
            {
                var targets =
                    HeroManager.Enemies.Where(
                        x =>
                        ObjectManager.Player.Distance(x) <= SpellManager.Spell[SpellSlot.E].Range * 2
                        && x.IsValidTarget(SpellManager.Spell[SpellSlot.E].Range * 2));

                if (targets.Count(x => ObjectManager.Player.Distance(x) < Orbwalking.GetRealAutoAttackRange(x)) == 0)
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                ObjectManager.Player.Distance(x) <= Orbwalking.GetRealAutoAttackRange(x) && x.IsEnemy)
                            .OrderBy(x => x.Health)
                            .FirstOrDefault();
                    if (minion != null)
                    {
                        Orbwalking.Orbwalk(minion, Game.CursorPos);
                    }
                }
            }

            if (!SpellManager.Spell[SpellSlot.Q].IsReady() || !Menu.Item("com.ikalista.combo.useQ").GetValue<bool>()) return;

            if (Menu.Item("com.ikalista.combo.saveMana").GetValue<bool>() && ObjectManager.Player.Mana < SpellManager.Spell[SpellSlot.E].ManaCost * 2)
            {
                return;
            }

            var spearTarget = TargetSelector.GetTarget(
                SpellManager.Spell[SpellSlot.Q].Range, 
                TargetSelector.DamageType.Physical);
            var prediction = SpellManager.Spell[SpellSlot.Q].GetSPrediction(spearTarget);
            if (prediction.HitChance >= HitChance.Medium
                && spearTarget.IsValidTarget(SpellManager.Spell[SpellSlot.Q].Range))
            {
                SpellManager.Spell[SpellSlot.Q].Cast(prediction.CastPosition);
            }
        }

        /// <summary>
        ///     My names definatly jeffery.
        /// </summary>
        /// <param name="args">even more gay</param>
        private void OnDraw(EventArgs args)
        {
            //Custom//DamageIndicator.Enabled = Menu.Item("com.ikalista.drawing.eDamage").GetValue<Circle>().Active;

            if (Menu.Item("com.ikalista.drawing.spellRanges").GetValue<bool>())
            {
                foreach (var spell in SpellManager.Spell.Values)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, spell.Range, Color.DarkOliveGreen);
                }
            }

            if (Menu.Item("com.ikalista.drawing.junpSpots").GetValue<bool>()
                && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.CustomMode)
            {
                foreach (var spot in possibleJumpSpots)
                {
                    var start = spot[0];
                    var end = spot[1];

                    if (ObjectManager.Player.Distance(start) <= 5000f
                        || ObjectManager.Player.Distance(end) <= 5000f && SpellManager.Spell[SpellSlot.Q].IsReady())
                    {
                        Drawing.DrawCircle(start, 100, Color.Chartreuse);
                        Drawing.DrawCircle(end, 100, Color.MediumSeaGreen);
                    }
                }
            }

            if (Menu.Item("com.ikalista.drawing.shine").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    ObjectManager.Player.Position, 
                    SpellManager.Spell[SpellSlot.E].Range, 
                    Color.DarkOliveGreen);
            }

            if (Menu.Item("com.ikalista.drawing.damagePercent").GetValue<Circle>().Active)
            {
                foreach (var source in HeroManager.Enemies.Where(x => ObjectManager.Player.Distance(x) <= 2000f && !x.IsDead))
                {
                    var currentPercentage = Math.Round(
                        Helper.GetRendDamage(source) * 100 / source.GetHealthWithShield(), 
                        2);

                    Drawing.DrawText(
                        Drawing.WorldToScreen(source.Position)[0], 
                        Drawing.WorldToScreen(source.Position)[1], 
                        currentPercentage >= 100
                            ? Menu.Item("com.ikalista.drawing.damagePercent").GetValue<Circle>().Color
                            : Color.White, 
                        currentPercentage >= 100 ? "Killable With E" : "Current Damage: " + currentPercentage + "%");
                }
            }
        }

        private void OnFlee()
        {
            var bestTarget =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(x => x.IsEnemy && ObjectManager.Player.Distance(x) <= Orbwalking.GetRealAutoAttackRange(x))
                    .OrderBy(x => ObjectManager.Player.Distance(x))
                    .FirstOrDefault();

            Orbwalking.Orbwalk(bestTarget, Game.CursorPos);

            // TODO wall flee
        }

        private void OnLaneclear()
        {
            if (Menu.Item("com.ikalista.laneclear.useQ").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.Q].Range).ToList();
                if (minions.Count < 0) return;

                foreach (var minion in minions.Where(x => x.Health <= SpellManager.Spell[SpellSlot.Q].GetDamage(x)))
                {
                    var killableMinions =
                        Helper.GetCollisionMinions(
                            ObjectManager.Player, 
                            ObjectManager.Player.ServerPosition.Extend(
                                minion.ServerPosition, 
                                SpellManager.Spell[SpellSlot.Q].Range))
                            .Count(
                                collisionMinion =>
                                collisionMinion.Health
                                <= ObjectManager.Player.GetSpellDamage(collisionMinion, SpellSlot.Q));

                    if (killableMinions >= Menu.Item("com.ikalista.laneclear.qMinions").GetValue<Slider>().Value)
                    {
                        SpellManager.Spell[SpellSlot.Q].Cast(minion.ServerPosition);
                    }
                }
            }

            if (Menu.Item("com.ikalista.laneclear.useE").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(SpellManager.Spell[SpellSlot.E].Range).ToList();
                if (minions.Count < 0) return;
                var siegeMinion =
                    minions.FirstOrDefault(x => x.CharData.BaseSkinName == "MinionSiege" && x.IsRendKillable());

                if (Menu.Item("com.ikalista.laneclear.eSiege").GetValue<bool>() && siegeMinion != null)
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }

                var count = minions.Count(x => SpellManager.Spell[SpellSlot.E].CanCast(x) && x.IsMobKillable());

                if (count >= Menu.Item("com.ikalista.laneclear.eMinions").GetValue<Slider>().Value
                    && !ObjectManager.Player.HasBuff("summonerexhaust"))
                {
                    SpellManager.Spell[SpellSlot.E].Cast();
                }
            }
        }

        private void OnMixed()
        {
            if (SpellManager.Spell[SpellSlot.Q].IsReady() && Menu.Item("com.ikalista.mixed.useQ").GetValue<bool>())
            {
                var target = TargetSelector.GetTarget(
                    SpellManager.Spell[SpellSlot.Q].Range, 
                    TargetSelector.DamageType.Physical);
                var prediction = SpellManager.Spell[SpellSlot.Q].GetSPrediction(target);
                if (prediction.HitChance >= HitChance.High
                    && target.IsValidTarget(SpellManager.Spell[SpellSlot.Q].Range))
                {
                    SpellManager.Spell[SpellSlot.Q].Cast(prediction.CastPosition);
                }
            }

            if (SpellManager.Spell[SpellSlot.E].IsReady() && Menu.Item("com.ikalista.mixed.useE").GetValue<bool>())
            {
                foreach (var source in
                    HeroManager.Enemies.Where(
                        x => x.IsValid && x.HasRendBuff() && SpellManager.Spell[SpellSlot.E].IsInRange(x)))
                {
                    if (source.IsRendKillable()
                        || source.GetRendBuffCount() >= Menu.Item("com.ikalista.mixed.stacks").GetValue<Slider>().Value)
                    {
                        SpellManager.Spell[SpellSlot.E].Cast();
                    }
                }
            }
        }

        /// <summary>
        ///     The on process spell function
        /// </summary>
        /// <param name="sender">
        ///     The Spell Sender
        /// </param>
        /// <param name="args">
        ///     The Arguments
        /// </param>
        private void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "KalistaExpungeWrapper")
            {
                Orbwalking.ResetAutoAttackTimer();
            }

            if (sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy && args.Target != null
                && Menu.Item("com.ikalista.combo.saveAlly").GetValue<bool>())
            {
                var soulboundhero =
                    HeroManager.Allies.FirstOrDefault(
                        hero => hero.HasBuff("kalistacoopstrikeally") && args.Target.NetworkId == hero.NetworkId);

                if (soulboundhero != null
                    && soulboundhero.HealthPercent
                    < Menu.Item("com.ikalista.combo.allyPercent").GetValue<Slider>().Value)
                {
                    SpellManager.Spell[SpellSlot.R].Cast();
                }
            }
        }

        /// <summary>
        ///     My Names Jeff
        /// </summary>
        /// <param name="args">gay</param>
        private void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    OnCombo();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    OnMixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    OnLaneclear();
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    OnFlee();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // BALISTA
            if (Menu.Item("com.ikalista.combo.balista").GetValue<bool>() && SpellManager.Spell[SpellSlot.R].IsReady())
            {
                var soulboundhero = HeroManager.Allies.FirstOrDefault(x => x.HasBuff("kalistacoopstrikeally"));
                if (soulboundhero?.ChampionName == "Blitzcrank")
                {
                    foreach (var unit in
                        HeroManager.Enemies.Where(
                            h =>
                            h.IsHPBarRendered && h.Distance(ObjectManager.Player.ServerPosition) > 700
                            && h.Distance(ObjectManager.Player.ServerPosition) < 1400))
                    {
                        if (unit.HasBuff("rocketgrab2"))
                        {
                            SpellManager.Spell[SpellSlot.R].Cast();
                        }
                    }
                }
            }

            foreach (var module in Modules.Where(x => x.ShouldGetExecuted()))
            {
                module.OnExecute();
            }
        }

        #endregion
    }
}