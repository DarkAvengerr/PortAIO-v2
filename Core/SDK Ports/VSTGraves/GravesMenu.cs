using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace VST_Auto_Carry_Standalone_Graves
{
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using System.Linq;
    using System.Reflection;

    internal class GravesMenu : Graves
    {
        internal static void Init()
        {
            Menu = new Menu(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Name, true).Attach();

            var Key = Menu.Add(new Menu("Key", "Kets Settings"));
            {
                Key.Add(new MenuKeyBind("Combo", "Combo", System.Windows.Forms.Keys.Space, KeyBindType.Press));
                Key.Add(new MenuKeyBind("Harass", "Harass", System.Windows.Forms.Keys.C, KeyBindType.Press));
                Key.Add(new MenuKeyBind("LaneClear", "LaneClear", System.Windows.Forms.Keys.V, KeyBindType.Press));
            }

            var QMenu = Menu.Add(new Menu("Q", "[Q] Settings"));
            {
                AddBool(QMenu, "Auto", "Auto");
                AddBool(QMenu, "Combo", "Combo");
                AddBool(QMenu, "Harass", "Harass");
                AddSlider(QMenu, "HarassMana", "Harass Min Mana >= %", 50);
                AddBool(QMenu, "Lane", "LaneClear");
                AddSlider(QMenu, "LaneMana", "LaneClear Min Mana >= %", 20);
                AddSlider(QMenu, "LaneHit", "LaneClear Min Hit >= ", 3, 1, 5);
                AddBool(QMenu, "Jungle", "Jungle");
                AddSlider(QMenu, "JungleMana", "Jungle Min Mana >= %", 20);
            }

            var WMenu = Menu.Add(new Menu("W", "[W] Settings"));
            {
                AddBool(WMenu, "Combo", "Combo");
                AddBool(WMenu, "Gap", "GapCloser");
            }

            var EMenu = Menu.Add(new Menu("E", "[E] Settings"));
            {
                AddBool(EMenu, "Combo", "Combo");
                AddBool(EMenu, "UnderTower", "Not Use When Player Under Towers(Enemy)");
                AddBool(EMenu, "Jungle", "Jungle");
                AddSlider(EMenu, "JungleMana", "Jungle Min Mana >= %", 20);
            }

            var RMenu = Menu.Add(new Menu("R", "[R] Settings"));
            {
                AddBool(RMenu, "Combo", "Combo");
                AddSlider(RMenu, "ComboHit", "Combo Min Hit >= ", 4, 1, 5);
                AddBool(RMenu, "Auto", "Auto");
                RMenu.Add(new MenuSeparator("RList", "Cast R List"));
                if (GameObjects.EnemyHeroes.Any())
                {
                    GameObjects.EnemyHeroes.ForEach(i => RMenu.Add(new MenuBool(i.ChampionName.ToLower(), i.ChampionName, AutoEnableList.Contains(i.ChampionName))));
                }
            }

            var DrawMenu = Menu.Add(new Menu("Draw", "Drawing"));
            {
                AddBool(DrawMenu, "Q", "Q", false);
                AddBool(DrawMenu, "W", "W", false);
                AddBool(DrawMenu, "E", "E", false);
                AddBool(DrawMenu, "R", "R", false);
            }
        }
    }
}