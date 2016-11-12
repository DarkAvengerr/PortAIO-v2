// This file is part of LeagueSharp.Common.
// 
// LeagueSharp.Common is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Common is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Common.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace NidaleeTheBestialHuntress
{
    internal class AssassinManager
    {
        private static Font _text;
        private static Font _textBold;

        public AssassinManager()
        {
            Load();
        }

        private static void Load()
        {
            _textBold = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 13,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });
            _text = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 13,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });
            Program.MainMenu.AddSubMenu(new Menu("Assassin Manager", "MenuAssassin"));
            Program.MainMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("AssassinActive", "Active").SetValue(true));
            Program.MainMenu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSearchRange", " Search Range"))
                .SetValue(new Slider(1400, 2000));
            Program.MainMenu.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinSelectOption", " Set:").SetValue(
                        new StringList(new[] { "Single Select", "Multi Select" })));
            Program.MainMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("xM1", "Enemies:"));
            foreach (AIHeroClient enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                Program.MainMenu.SubMenu("MenuAssassin")
                    .AddItem(
                        new MenuItem("Assassin" + enemy.ChampionName, " " + enemy.ChampionName).SetValue(
                            TargetSelector.GetPriority(enemy) > 3));
            }
            Program.MainMenu.SubMenu("MenuAssassin").AddItem(new MenuItem("xM2", "Other Settings:"));
            Program.MainMenu.SubMenu("MenuAssassin")
                .AddItem(new MenuItem("AssassinSetClick", " Add/Remove with click").SetValue(true));
            Program.MainMenu.SubMenu("MenuAssassin")
                .AddItem(
                    new MenuItem("AssassinReset", " Reset List").SetValue(
                        new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));
            Program.MainMenu.SubMenu("MenuAssassin").AddSubMenu(new Menu("Drawings", "Draw"));
            Program.MainMenu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawSearch", "Search Range").SetValue(new Circle(true, Color.GreenYellow)));
            Program.MainMenu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawActive", "Active Enemy").SetValue(new Circle(true, Color.GreenYellow)));
            Program.MainMenu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawNearest", "Nearest Enemy").SetValue(new Circle(true, Color.DarkSeaGreen)));
            Program.MainMenu.SubMenu("MenuAssassin")
                .SubMenu("Draw")
                .AddItem(new MenuItem("DrawStatus", "Show status on the screen").SetValue(true));
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void ClearAssassinList()
        {
            foreach (
                var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                Program.MainMenu.Item("Assassin" + enemy.ChampionName).SetValue(false);
            }
        }

        private static void OnUpdate(EventArgs args) {}

        public static void DrawText(Font vFont, string vText, float vPosX, float vPosY, SharpDX.ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        public static void DrawTextBold(Font vFont, string vText, float vPosX, float vPosY, SharpDX.ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (Program.MainMenu.Item("AssassinReset").GetValue<KeyBind>().Active && args.Msg == 257)
            {
                ClearAssassinList();
                Chat.Print(
                    "<font color='#FFFFFF'>Reset Assassin List is Complete! Click on the enemy for Add/Remove.</font>");
            }
            if (args.Msg != (uint) WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }
            if (Program.MainMenu.Item("AssassinSetClick").GetValue<bool>())
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
                    if (objAiHero != null && objAiHero.IsVisible && !objAiHero.IsDead)
                    {
                        var xSelect = Program.MainMenu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;
                        switch (xSelect)
                        {
                            case 0:
                                ClearAssassinList();
                                Program.MainMenu.Item("Assassin" + objAiHero.ChampionName).SetValue(true);
                                Chat.Print(
                                    string.Format(
                                        "<font color='FFFFFF'>Added to Assassin List</font> <font color='#09F000'>{0} ({1})</font>",
                                        objAiHero.Name, objAiHero.ChampionName));
                                break;
                            case 1:
                                var menuStatus =
                                    Program.MainMenu.Item("Assassin" + objAiHero.ChampionName).GetValue<bool>();
                                Program.MainMenu.Item("Assassin" + objAiHero.ChampionName).SetValue(!menuStatus);
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
            if (!Program.MainMenu.Item("AssassinActive").GetValue<bool>())
            {
                return;
            }
            if (Program.MainMenu.Item("DrawStatus").GetValue<bool>())
            {
                var enemies = ObjectManager.Get<AIHeroClient>().Where(xEnemy => xEnemy.IsEnemy);
                var objAiHeroes = enemies as AIHeroClient[] ?? enemies.ToArray();
                DrawText(_textBold, "Target Mode:", Drawing.Width * 0.89f, Drawing.Height * 0.55f, SharpDX.Color.White);
                var xSelect = Program.MainMenu.Item("AssassinSelectOption").GetValue<StringList>().SelectedIndex;
                DrawText(
                    _text, xSelect == 0 ? "Single Target" : "Multi Targets", Drawing.Width * 0.94f,
                    Drawing.Height * 0.55f, SharpDX.Color.White);
                DrawText(
                    _textBold, "Priority Targets", Drawing.Width * 0.89f, Drawing.Height * 0.58f, SharpDX.Color.White);
                DrawText(_textBold, "_____________", Drawing.Width * 0.89f, Drawing.Height * 0.58f, SharpDX.Color.White);
                for (int i = 0; i < objAiHeroes.Count(); i++)
                {
                    var xValue = Program.MainMenu.Item("Assassin" + objAiHeroes[i].ChampionName).GetValue<bool>();
                    DrawTextBold(
                        xValue ? _textBold : _text, objAiHeroes[i].ChampionName, Drawing.Width * 0.895f,
                        Drawing.Height * 0.58f + (float) (i + 1) * 15,
                        xValue ? SharpDX.Color.GreenYellow : SharpDX.Color.DarkGray);
                }
            }
            var drawSearch = Program.MainMenu.Item("DrawSearch").GetValue<Circle>();
            var drawActive = Program.MainMenu.Item("DrawActive").GetValue<Circle>();
            var drawNearest = Program.MainMenu.Item("DrawNearest").GetValue<Circle>();
            var drawSearchRange = Program.MainMenu.Item("AssassinSearchRange").GetValue<Slider>().Value;
            if (drawSearch.Active)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, drawSearchRange, drawSearch.Color, 1);
            }
            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>()
                    .Where(enemy => enemy.Team != ObjectManager.Player.Team)
                    .Where(
                        enemy =>
                            enemy.IsVisible && Program.MainMenu.Item("Assassin" + enemy.ChampionName) != null &&
                            !enemy.IsDead)
                    .Where(enemy => Program.MainMenu.Item("Assassin" + enemy.ChampionName).GetValue<bool>()))
            {
                if (ObjectManager.Player.Distance(enemy) < drawSearchRange)
                {
                    if (drawActive.Active)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 115f, drawActive.Color, 1);
                    }
                }
                else if (ObjectManager.Player.Distance(enemy) > drawSearchRange &&
                         ObjectManager.Player.Distance(enemy) < drawSearchRange + 400)
                {
                    if (drawNearest.Active)
                    {
                        Render.Circle.DrawCircle(enemy.Position, 115f, drawNearest.Color, 1);
                    }
                }
            }
        }
    }
}