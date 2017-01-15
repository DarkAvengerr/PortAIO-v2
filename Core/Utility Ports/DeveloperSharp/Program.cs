using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;

using EloBuddy;
using LeagueSharp.Common;
namespace DeveloperSharp
{
    static class Program
    {
        private static Menu Config;
        private static Menu Types;
        private static Menu Types1;
        private static Menu Types2;
        private static Menu Types3;
        private static Menu Types4;
        private static int _lastUpdateTick = 0;
        private static int _lastMovementTick = 0;
        public static void Main()
        {
            InitMenu();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                //Chat.Print("Detected Spell Name: " + args.SData.Name + " Missile Name: " + args.SData.MissileBoneName + " Issued By: " + sender.BaseSkinName);
            }
        }

        private static void InitMenu()
        {
            Config = new Menu("Developer#", "developersharp", true);
            Config.AddItem(new MenuItem("range", "Max object dist from cursor").SetValue(new Slider(400, 100, 2000)));
            Config.AddItem(new MenuItem("antiafk", "Anti-AFK").SetValue(false));
            Types = Config.AddSubMenu(new Menu("Show Info for: ", "typesinfo"));
            Types1 = Types.AddSubMenu(new Menu("--> (1)", "firstList"));
            Types2 = Types.AddSubMenu(new Menu("--- (2)", "secondList"));
            Types3 = Types.AddSubMenu(new Menu("--- (3)", "thirdList"));
            Types4 = Types.AddSubMenu(new Menu("--- (4)", "fourthList"));

            var gameObjectTypes = Enum.GetNames(typeof(EloBuddy.GameObjectType));
            List<IEnumerable<string>> typesListsDivided = new List<IEnumerable<string>>();
            for (var i = 0; i < (double)gameObjectTypes.Length / 11; i++)
            {
                typesListsDivided.Add(gameObjectTypes.Skip(i * 11).Take(11));
            }
            var typeListNumber = 0;
            foreach (var types in typesListsDivided)
            {
                typeListNumber++;
                foreach (var type in types)
                {
                    if (typeListNumber == 1)
                    {
                        Types1.AddItem(new MenuItem(type, type).SetValue(true));
                    }
                    if (typeListNumber == 2)
                    {
                        Types2.AddItem(new MenuItem(type, type).SetValue(false));
                    }
                    if (typeListNumber == 3)
                    {
                        Types3.AddItem(new MenuItem(type, type).SetValue(false));
                    }
                    if (typeListNumber == 4)
                    {
                        Types4.AddItem(new MenuItem(type, type).SetValue(false));
                    }
                }
            }
            Config.AddItem(
                new MenuItem("masteries", "Show Masteries").SetValue(new KeyBind((uint)Keys.L, KeyBindType.Press)));
            Config.AddItem(new MenuItem("color", "Text Color: ").SetValue(Color.DarkTurquoise));
            Config.AddToMainMenu();
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Config.Item("antiafk").GetValue<bool>() && Environment.TickCount - _lastMovementTick > 140000)
            {
                EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo,
                    ObjectManager.Player.Position.Randomize(-1000, 1000));
                _lastMovementTick = Environment.TickCount;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("masteries").GetValue<KeyBind>().Active)
            {
                var masteries = ObjectManager.Player.Masteries;
                for (var i = 0; i < masteries.Length; i++)
                {
                    var mastery = masteries[i];
                    var text = "You have " + mastery.Points + " points in mastery #" + mastery.Id + " from page " + mastery.Page;
                    Drawing.DrawText(Drawing.Width / 2 - text.Length, Drawing.Height / 2 - masteries.Length + i * 10, Config.Item("color").GetValue<Color>(), text);
                }
                return;
            }
            foreach (var obj in ObjectManager.Get<GameObject>().Where(o => o.Position.Distance(Game.CursorPos) < Config.Item("range").GetValue<Slider>().Value && !(o is Obj_Turret) && o.Name != "missile" && !o.Name.Contains("MoveTo") && Types.Item(Enum.GetName(typeof(GameObjectType), o.Type)).GetValue<bool>()))
            {
                if (!obj.IsValid<GameObject>()) return;
                var X = Drawing.WorldToScreen(obj.Position).X;
                var Y = Drawing.WorldToScreen(obj.Position).Y;
                Drawing.DrawText(X, Y, Config.Item("color").GetValue<Color>(), (obj is AIHeroClient) ? ((AIHeroClient)obj).BaseSkinName : (obj is Obj_AI_Minion) ? (obj as Obj_AI_Minion).BaseSkinName : (obj is Obj_AI_Turret) ? (obj as Obj_AI_Turret).BaseSkinName : obj.Name);
                Drawing.DrawText(X, Y + 12, Config.Item("color").GetValue<Color>(), obj.Type.ToString());
                Drawing.DrawText(X, Y + 22, Config.Item("color").GetValue<Color>(), "NetworkID: " + obj.NetworkId);
                Drawing.DrawText(X, Y + 32, Config.Item("color").GetValue<Color>(), obj.Position.ToString());
                if (obj is Obj_AI_Base)
                {
                    var aiobj = obj as Obj_AI_Base;
                    Drawing.DrawText(X, Y + 42, Config.Item("color").GetValue<Color>(), "Health: " + aiobj.Health + "/" + aiobj.MaxHealth + "(" + aiobj.HealthPercent + "%)");
                }
                if (obj is AIHeroClient)
                {
                    var hero = obj as AIHeroClient;
                    Drawing.DrawText(X, Y + 62, Config.Item("color").GetValue<Color>(), "Spells:");
                    Drawing.DrawText(X, Y + 72, Config.Item("color").GetValue<Color>(), "-------");
                    Drawing.DrawText(X, Y + 82, Config.Item("color").GetValue<Color>(), "(Q): " + hero.Spellbook.Spells[0].Name);
                    Drawing.DrawText(X, Y + 92, Config.Item("color").GetValue<Color>(), "(W): " + hero.Spellbook.Spells[1].Name);
                    Drawing.DrawText(X, Y + 102, Config.Item("color").GetValue<Color>(), "(E): " + hero.Spellbook.Spells[2].Name);
                    Drawing.DrawText(X, Y + 112, Config.Item("color").GetValue<Color>(), "(R): " + hero.Spellbook.Spells[3].Name);
                    Drawing.DrawText(X, Y + 122, Config.Item("color").GetValue<Color>(), "(D): " + hero.Spellbook.Spells[4].Name);
                    Drawing.DrawText(X, Y + 132, Config.Item("color").GetValue<Color>(), "(F): " + hero.Spellbook.Spells[5].Name);
                    var buffs = hero.Buffs;
                    if (buffs.Any())
                    {
                        Drawing.DrawText(X, Y + 152, Config.Item("color").GetValue<Color>(), "Buffs:");
                        Drawing.DrawText(X, Y + 162, Config.Item("color").GetValue<Color>(), "------");
                    }
                    for (var i = 0; i < buffs.Count() * 10; i += 10)
                    {
                        Drawing.DrawText(X, (Y + 172 + i), Config.Item("color").GetValue<Color>(), buffs[i / 10].Count + "x " + buffs[i / 10].Name);
                    }

                }
                if (obj is MissileClient && obj.Name != "missile")
                {
                    var missile = obj as MissileClient;
                    Drawing.DrawText(X, Y + 40, Config.Item("color").GetValue<Color>(), "Missile Speed: " + missile.SData.MissileSpeed);
                    Drawing.DrawText(X, Y + 50, Config.Item("color").GetValue<Color>(), "Cast Range: " + missile.SData.CastRange);
                }
            }
        }
    }
}
