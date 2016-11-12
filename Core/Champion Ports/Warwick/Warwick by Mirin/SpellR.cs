using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using EloBuddy; namespace Warwick
{
    internal class SpellR
    {
        public static Font Text, TextBold, TextWarning;

        public SpellR()
        {
            Text = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 15,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            TextBold = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 15,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            TextWarning = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Malgun Gothic",
                    Height = 75,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearTypeNatural
                });

            Load();
        }

        private static string MenuTab
        {
            get { return "    "; }
        }

        private static float SelectorRange
        {
            get { return Program.rMenu.Item("R.SearchRange").GetValue<Slider>().Value; }
        }

        private static AIHeroClient TsEnemy
        {
            get
            {
                var vMax = HeroManager.Enemies.Where(
                    e =>
                        !e.IsDead && e.IsVisible && e.IsValidTarget(SelectorRange) && !e.IsZombie)
                    .Max(
                        h => Program.rMenu.Item("Selected" + h.ChampionName).GetValue<StringList>().SelectedIndex);

                if (!double.IsNaN(vMax))
                {
                    var enemy = HeroManager.Enemies.Where(
                        e =>
                            !e.IsDead && e.IsVisible && e.IsValidTarget(SelectorRange) && !e.IsZombie &&
                            Program.rMenu.Item("Selected" + e.ChampionName).GetValue<StringList>().SelectedIndex == vMax);

                    return enemy.MinOrDefault(hero => hero.Health);
                }

                return null;
            }
        }

        private static void Load()
        {
            Program.rMenu.AddItem(
                new MenuItem("R.Use", "Use:").SetValue(
                    new StringList(new[] {"Off", "Everytime", "If enemy is killable", "Smart R (Recommend!)"}, 3)));
            Program.rMenu.AddItem(new MenuItem("R.SearchRange", MenuTab + "Enemy Searching Range"))
                .SetValue(new Slider(1000, 1500));

            Program.rMenu.AddItem(new MenuItem("R.Title", "Enemies:"));
            {
                foreach (var enemy in HeroManager.Enemies)
                {
                    Program.rMenu.AddItem(
                        new MenuItem("Selected" + enemy.ChampionName, MenuTab + enemy.ChampionName).SetValue(
                            new StringList(
                                new[] {"Don't Use", "Low Priority", "Medium Priority", ">> High Priority <<"},
                                GetPriority(enemy.ChampionName))));
                }
            }

            Program.rMenu.AddItem(new MenuItem("R.Other.Title", "Other Settings:"));
            {
                Program.rMenu.AddItem(
                    new MenuItem("R.AutoPriority Focus", MenuTab + "Auto Arrange priorities").SetShared()
                        .SetValue(false))
                    .ValueChanged += AutoPriorityItemValueChanged;
            }
            Program.rMenu.AddItem(
                new MenuItem("R.Click", MenuTab + "Mouse Left-Click: Chance Enemy's Hitchance").SetValue(false));

            Program.rMenu.AddItem(new MenuItem("Draw.Title", "Drawings"));
            {
                Program.rMenu.AddItem(
                    new MenuItem("Draw.Search", MenuTab + "Show Search Range").SetValue(new Circle(true,
                        System.Drawing.Color.GreenYellow)));
                Program.rMenu.AddItem(new MenuItem("Draw.Status", MenuTab + "Show Targeting Status").SetValue(true));
                Program.rMenu.AddItem(
                    new MenuItem("Draw.Status.Show", MenuTab + "Show This:").SetValue(
                        new StringList(new[] {"All", "Show Only High Priority Target Status"})));
            }
            Game_OnGameLoad();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnGameLoad()
        {
            LoadEnemyPriorityData();
        }

        private static void OnUpdate(EventArgs args)
        {
        }

        private static void LoadEnemyPriorityData()
        {
            foreach (var enemy in HeroManager.Enemies)
            {
                Program.rMenu.Item("Selected" + enemy.ChampionName)
                    .SetValue(
                        new StringList(new[] {"Don't Use", "Low Priority", "Medium Priority", ">> High Priority <<"},
                            GetPriority(enemy.ChampionName)));
            }
        }

        private static void AutoPriorityItemValueChanged(object sender, OnValueChangeEventArgs e)
        {
            if (!e.GetNewValue<bool>())
                return;

            LoadEnemyPriorityData();
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint) WindowsMessages.WM_LBUTTONDOWN)
                return;

            if (Program.rMenu.Item("R.Click").GetValue<bool>())
            {
                var selectedTarget =
                    HeroManager.Enemies.FindAll(
                        hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000)
                        .OrderBy(h => h.Distance(Game.CursorPos, true))
                        .FirstOrDefault();
                {
                    if (selectedTarget != null && selectedTarget.IsVisible)
                    {
                        var vSelected =
                            Program.rMenu.Item("Selected" + selectedTarget.ChampionName)
                                .GetValue<StringList>()
                                .SelectedIndex;

                        var i = vSelected == 3 ? 0 : vSelected + 1;

                        Program.rMenu.Item("Selected" + selectedTarget.ChampionName)
                            .SetValue(
                                new StringList(
                                    new[] {"Don't Use", "Low Priority", "Medium Priority", ">> High Priority <<"}, i));
                    }
                }
            }
        }

        private static int GetPriority(string championName)
        {
            string[] lowPriority =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Tahm", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] mediumPriority =
            {
                "Aatrox", "Akali", "Darius", "Diana", "Ekko", "Elise", "Evelynn", "Fiddlesticks", "Fiora", "Fizz",
                "Galio", "Gangplank", "Gragas", "Heimerdinger", "Irelia", "Jax", "Jayce", "Kassadin", "Kayle", "Kha'Zix",
                "Lee Sin", "Lissandra", "Maokai", "Mordekaiser", "Morgana", "Nocturne", "Nidalee", "Pantheon", "Poppy",
                "RekSai", "Rengar", "Riven", "Rumble", "Ryze", "Shaco", "Swain", "Trundle", "Tryndamere", "Udyr",
                "Urgot", "Vladimir", "Vi", "XinZhao", "Yasuo", "Zilean"
            };

            string[] highPriority =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs"
            };

            if (lowPriority.Contains(championName))
            {
                return 1;
            }
            if (mediumPriority.Contains(championName))
            {
                return 2;
            }
            if (highPriority.Contains(championName))
            {
                return 3;
            }
            return 2;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Program.Config.Item("Draw.Disable").GetValue<bool>())
                return;

            if (Program.rMenu.Item("R.Use").GetValue<StringList>().SelectedIndex == 0)
                return;


            var drawSearch = Program.rMenu.Item("Draw.Search").GetValue<Circle>();
            if (drawSearch.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position,
                    SelectorRange, drawSearch.Color, 1);
            }

            if (Program.rMenu.Item("Draw.Status").GetValue<bool>())
            {
                foreach (var a in HeroManager.Enemies.Where(e => e.IsVisible && !e.IsDead && !e.IsZombie))
                {
                    var vSelected =
                        (Program.rMenu.Item("Selected" + a.ChampionName).GetValue<StringList>().SelectedIndex);

                    if (Program.rMenu.Item("Draw.Status.Show").GetValue<StringList>().SelectedIndex == 1 &&
                        vSelected != 3)
                        continue;

                    if (vSelected != 0)
                        DrawText(vSelected == 3 ? TextBold : Text,
                            Program.rMenu.Item("Selected" + a.ChampionName).GetValue<StringList>().SelectedValue,
                            a.HPBarPosition.X + a.BoundingRadius/2f - 10, a.HPBarPosition.Y - 20, vSelected == 3
                                ? SharpDX.Color.Red
                                : (vSelected == 2 ? SharpDX.Color.Yellow : SharpDX.Color.Gray));
                }
            }
        }

        public AIHeroClient GetTarget(float vRange = 0,
            TargetSelector.DamageType vDamageType = TargetSelector.DamageType.Physical)
        {
            if (Math.Abs(vRange) < 0.00001)
                return null;
            return TsEnemy;
        }

        public static void DrawText(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }
    }
}