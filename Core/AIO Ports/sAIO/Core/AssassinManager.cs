using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace sAIO.Core
{
    internal class AssassinManager : Helper//Thank you jQuery
    {
        public AssassinManager()
        {
            Load();
        }

        private static void Load()
        {
            menu.AddSubMenu(new Menu("Assassin Manager", "MenuAssassin"));
            menu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinActive", "Active").SetValue(true));

            foreach (
                var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                menu.SubMenu("MenuAssassin")
                    .AddItem(
                        new MenuItem("Assassin" + enemy.ChampionName, enemy.ChampionName).SetValue(
                            TargetSelector.GetPriority(enemy) > 3));
            }


            menu.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinSelectOption", "Set: ").SetValue(
                        new StringList(new[] { "Single Select", "Multi Select" })));
            menu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSetClick", "Add/Remove with click").SetValue(true));
            menu.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinReset", "Reset List").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            menu.SubMenu("MenuAssassin").AddSubMenu(new Menu("Draw:", "Draw"));

            menu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawSearch", "Search Range").SetValue(new Circle(true, Color.GreenYellow)));
            menu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawActive", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
            menu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawNearest", "Nearest Enemy").SetValue(new Circle(true, Color.DarkSeaGreen)));
            menu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawStatus", "Show Status").SetValue(true));


            menu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSearchRange", "Search Range"))
                .SetValue(new Slider(1000, 2000));

            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void ClearAssassinList()
        {
            foreach (
                var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                menu.Item("Assassin" + enemy.ChampionName).SetValue(false);
            }
        }

        private static void OnGameUpdate(EventArgs args) {}

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (menu.Item("AssassinReset").GetValue<KeyBind>().Active && args.Msg == 257)
            {
                ClearAssassinList();
                Notifications.AddNotification("Assassin List is resetted.", 5);
            }

            if (args.Msg != (uint) WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            if (menu.Item("AssassinSetClick").GetValue<bool>())
            {
                foreach (var objAiHero in from hero in ObjectManager.Get<AIHeroClient>()
                    where hero.IsValidTarget()
                    select hero
                    into h
                    orderby h.Distance(Game.CursorPos) descending
                    select h
                    into enemy
                    where enemy.Distance(Game.CursorPos) < 150f
                    select enemy)
                {
                    if (objAiHero != null && objAiHero.IsHPBarRendered && !objAiHero.IsDead)
                    {
                        var xSelect = menu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;

                        switch (xSelect)
                        {
                            case 0:
                                ClearAssassinList();
                                menu.Item("Assassin" + objAiHero.ChampionName).SetValue(true);
                                Notifications.AddNotification(
                                    "Added " + objAiHero.ChampionName + " to Assassin List", 5);
                                break;
                            case 1:
                                var menuStatus = menu.Item("Assassin" + objAiHero.ChampionName).GetValue<bool>();
                                menu.Item("Assassin" + objAiHero.ChampionName).SetValue(!menuStatus);

                                //Notifications.AddNotification("Removed " + objAiHero.ChampionName + " to Assassin List", 5);
                                Chat.Print(
                                    string.Format(
                                        "<font color='{0}'>{1}</font> <font color='#09F000'>{2} ({3})</font>",
                                        !menuStatus ? "#FFFFFF" : "#FF8877",
                                        !menuStatus ? "Added to Assassin List:" : "Removed from Assassin List:",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!menu.Item("AssassinActive").GetValue<bool>())
            {
                return;
            }

            if (menu.Item("DrawStatus").GetValue<bool>())
            {
                var enemies = ObjectManager.Get<AIHeroClient>().Where(xEnemy => xEnemy.IsEnemy);
                var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
                Drawing.DrawText(Drawing.Width * 0.89f, Drawing.Height * 0.58f, Color.GreenYellow, "Assassin Status");
                Drawing.DrawText(Drawing.Width * 0.89f, Drawing.Height * 0.58f, Color.GhostWhite, "_____________");
                for (int i = 0; i < objAiHeroes.Count(); i++)
                {
                    var xCaption = objAiHeroes[i].ChampionName;
                    var xWidth = Drawing.Width * 0.90f;
                    if (menu.Item("Assassin" + objAiHeroes[i].ChampionName).GetValue<bool>())
                    {
                        xCaption = "+ " + xCaption;
                        xWidth = Drawing.Width * 0.8910f;
                    }
                    Drawing.DrawText(xWidth, Drawing.Height * 0.58f + (float) (i + 1) * 15, Color.Gainsboro, xCaption);
                }
            }

            var drawSearch = menu.Item("DrawSearch").GetValue<Circle>();
            var drawActive = menu.Item("DrawActive").GetValue<Circle>();
            var drawNearest = menu.Item("DrawNearest").GetValue<Circle>();

            var drawSearchRange = menu.Item("AssassinSearchRange").GetValue<Slider>().Value;
            if (drawSearch.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawSearchRange, drawSearch.Color);
            }

            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>()
                    .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                    .Where(
                        enemy =>
                            enemy.IsHPBarRendered && menu.Item("Assassin" + enemy.ChampionName) != null &&
                            !enemy.IsDead)
                    .Where(enemy => menu.Item("Assassin" + enemy.ChampionName).GetValue<bool>()))
            {
                if (ObjectManager.Player.Distance(enemy) < drawSearchRange)
                {
                    if (drawActive.Active)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 85f, drawActive.Color);
                    }
                }
                else if (ObjectManager.Player.Distance(enemy) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy) < drawSearchRange + 400)
                {
                    if (drawNearest.Active)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 85f, drawNearest.Color);
                    }
                }
            }
        }
    }
}

