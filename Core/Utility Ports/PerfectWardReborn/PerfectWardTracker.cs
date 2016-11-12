using LeagueSharp;
using LeagueSharp.Common;
using Color = SharpDX.Color;
using SharpDX;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using EloBuddy;
using LeagueSharp.Common;
namespace PerfectWardReborn

{
    internal class PerfectWardTracker
    {
        public static Helper Helper;

        static readonly List<KeyValuePair<int, String>> _wards = new List<KeyValuePair<int, String>> //insertion order
        {
            new KeyValuePair<int, String>(3340, "Warding Totem Trinket"),
            new KeyValuePair<int, String>(2301, "Eye of the Watchers"),
            new KeyValuePair<int, String>(2302, "Eye of the Oasis"),
            new KeyValuePair<int, String>(2303, "Eye of the Equinox"),
            //new KeyValuePair<int, String>(3205, "Quill Coat"),
            new KeyValuePair<int, String>(3207, "Spirit Of The Ancient Golem"),
            new KeyValuePair<int, String>(3154, "Wriggle's Lantern"),
            new KeyValuePair<int, String>(2049, "Sight Stone"),
            new KeyValuePair<int, String>(2045, "Ruby Sightstone"),
            //new KeyValuePair<int, String>(3160, "Feral Flare"),
            new KeyValuePair<int, String>(2050, "Explorer's Ward"),
            new KeyValuePair<int, String>(2044, "Stealth Ward"),
            new KeyValuePair<int, String>(2055, "Control Ward"),

        };
        int _lastTimeWarded;

        private const int VK_LBUTTON = 1;
        private const int WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101, WM_CHAR = 0x0102, WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105, WM_MOUSEDOWN = 0x201;
        public static Menu Config;
        public static float lastuseward = 0;
        public class Wardspoting
        {
            public static WardSpot _PutSafeWard;
        }

        public PerfectWardTracker()
        {
            Game.OnLoad += OnGameStart;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += OnDraw;


            //Create the menu
            Config = new Menu("PerfectWardReborn", "PerfectWardReborn", true).SetFontStyle(FontStyle.Bold, Color.DarkSeaGreen);

            Config.AddSubMenu(new Menu("WardKey", "WardKey"));
            Config.SubMenu("WardKey").AddItem(new MenuItem("placekey", "NormalWard Key").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press))).SetFontStyle(FontStyle.Bold, Color.Green);
            Config.SubMenu("WardKey").AddItem(new MenuItem("placekeyconWard", "ControlWard Key").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Press))).SetFontStyle(FontStyle.Bold, Color.DarkOrange);
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawplaces", "Draw ward places").SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 255))));
            Config.SubMenu("Drawing").AddItem(new MenuItem("drawDistance", "Don't draw if the distance >")).SetValue(new Slider(2000, 10000, 1));
            Config.AddSubMenu(new Menu("AutoBushRevealer", "AutoBushRevealer"));
            Config.SubMenu("AutoBushRevealer").AddItem(new MenuItem("AutoBushKey", "Key").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
            Config.SubMenu("AutoBushRevealer").AddItem(new MenuItem("AutoBushEnabled", "Enabled").SetValue(true));
            Config.AddToMainMenu();
            foreach (var ward in _wards)
                Config.SubMenu("Auto Bush Ward Type").AddItem(new MenuItem("AutoBush" + ward.Key, ward.Value).SetValue(true));
            Game.OnUpdate += Game_OnGameUpdate;

        }

        InventorySlot GetWardSlot()
        {
            return _wards.Select(x => x.Key).Where(id => Config.Item("AutoBush" + id).GetValue<bool>() && Items.CanUseItem(id)).Select(wardId => ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId)).FirstOrDefault();
        }

        static public InventorySlot GetAnyWardSlot()
        {
            return _wards.Select(x => x.Key).Where(Items.CanUseItem).Select(wardId => ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)wardId)).FirstOrDefault();
        }

        Obj_AI_Base GetNearObject(String name, Vector3 pos, int maxDistance)
        {
            return ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(x => x.Name == name && x.Distance(pos) <= maxDistance);
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            int time = Environment.TickCount;
            if (Config.Item("AutoBushEnabled").GetValue<bool>() && Config.Item("AutoBushKey").GetValue<KeyBind>().Active)
                foreach (AIHeroClient enemy in Helper.EnemyInfo.Where(x =>
                            x.Player.IsValid &&
                            !x.Player.IsVisible &&
                            !x.Player.IsDead &&
                            x.Player.Distance(ObjectManager.Player.ServerPosition) < 1000 &&
                            time - x.LastSeen < 2500).Select(x => x.Player))
                {
                    var bestWardPos = GetWardPos(enemy.ServerPosition);
                    //           var bestWardPos = GetWardPos(enemy.ServerPosition, 165, 2);


                    if (bestWardPos != enemy.ServerPosition && bestWardPos != Vector3.Zero && bestWardPos.Distance(ObjectManager.Player.ServerPosition) <= 600)
                    {
                        int timedif = Environment.TickCount - _lastTimeWarded;

                        if (timedif > 1250 && !(timedif < 2500 && GetNearObject("SightWard", bestWardPos, 200) != null)) //no near wards
                        {
                            var wardSlot = GetWardSlot();

                            if (wardSlot != null && wardSlot.Id != ItemId.Unknown)
                            {
                                ObjectManager.Player.Spellbook.CastSpell(wardSlot.SpellSlot, bestWardPos);
                                _lastTimeWarded = Environment.TickCount;
                            }
                        }
                    }
                }

            InventorySlot wardSpellSlot = null;
            if (Config.Item("placekey").GetValue<KeyBind>().Active)
            {
                wardSpellSlot = Ward.GetWardSlot();
            }
            else if (Config.Item("placekeyconWard").GetValue<KeyBind>().Active)
            {
                wardSpellSlot = Ward.GetConWardSlot();
            }
            {
                if (wardSpellSlot == null || lastuseward + 1000 > Environment.TickCount)
                {
                    return;
                }
                Vector3? nearestWard = Ward.FindNearestWardSpot(Drawing.ScreenToWorld(Game.CursorPos.X, Game.CursorPos.Y));

                if (nearestWard != null)
                {
                    if (wardSpellSlot != null)
                    {
                        Console.WriteLine("putting ward");
                        ObjectManager.Player.Spellbook.CastSpell(wardSpellSlot.SpellSlot, (Vector3)nearestWard);
                        lastuseward = Environment.TickCount;
                    }
                }

                WardSpot nearestSafeWard = Ward.FindNearestSafeWardSpot(Drawing.ScreenToWorld(Game.CursorPos.X, Game.CursorPos.Y));

                if (nearestSafeWard != null)
                {
                    if (wardSpellSlot != null)
                    {
                        EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, nearestSafeWard.MovePosition);
                        Wardspoting._PutSafeWard = nearestSafeWard;
                    }
                }
            }


            if (Wardspoting._PutSafeWard != null && lastuseward + 1000 < Environment.TickCount)
            {
                wardSpellSlot = Items.GetWardSlot();
                if (Math.Sqrt(Math.Pow(Wardspoting._PutSafeWard.ClickPosition.X - ObjectManager.Player.Position.X, 2) + Math.Pow(Wardspoting._PutSafeWard.ClickPosition.Y - ObjectManager.Player.Position.Y, 2)) <= 640.0)
                {
                    if (Config.Item("placekey").GetValue<KeyBind>().Active)
                    {
                        wardSpellSlot = Ward.GetWardSlot();
                    }
                    else if (Config.Item("placekeyconWard").GetValue<KeyBind>().Active)
                    {
                        wardSpellSlot = Ward.GetConWardSlot();
                    }
                    if (wardSpellSlot != null)
                    {
                        Console.WriteLine("putting ward2");
                        ObjectManager.Player.Spellbook.CastSpell(wardSpellSlot.SpellSlot, Wardspoting._PutSafeWard.ClickPosition);
                        lastuseward = Environment.TickCount;

                    }
                    Wardspoting._PutSafeWard = null;
                }
            }
        }

        Vector3 GetWardPos(Vector3 lastPos, int radius = 165, int precision = 3) //maybe reverse autobushward code from the bots?
        {
            //old: Vector3 wardPos = enemy.Position + Vector3.Normalize(enemy.Position - ObjectManager.Player.Position) * 150;

            var count = precision;

            while (count > 0)
            {
                var vertices = radius;

                var wardLocations = new WardLocation[vertices];
                var angle = 2 * Math.PI / vertices;

                for (var i = 0; i < vertices; i++)
                {
                    var th = angle * i;
                    var pos = new Vector3((float)(lastPos.X + radius * Math.Cos(th)), (float)(lastPos.Y + radius * Math.Sin(th)), 0);
                    wardLocations[i] = new WardLocation(pos, NavMesh.IsWallOfGrass(pos, 5));
                }

                var grassLocations = new List<GrassLocation>();

                for (var i = 0; i < wardLocations.Length; i++)
                {
                    if (!wardLocations[i].Grass) continue;
                    if (i != 0 && wardLocations[i - 1].Grass)
                        grassLocations.Last().Count++;
                    else
                        grassLocations.Add(new GrassLocation(i, 1));
                }

                var grassLocation = grassLocations.OrderByDescending(x => x.Count).FirstOrDefault();

                if (grassLocation != null) //else: no pos found. increase/decrease radius?
                {
                    var midelement = (int)Math.Ceiling(grassLocation.Count / 2f);
                    lastPos = wardLocations[grassLocation.Index + midelement - 1].Pos;
                    radius = (int)Math.Floor(radius / 2f);
                }

                count--;
            }

            return lastPos;
        }

        class WardLocation
        {
            public readonly Vector3 Pos;
            public readonly bool Grass;

            public WardLocation(Vector3 pos, bool grass)
            {
                Pos = pos;
                Grass = grass;
            }
        }

        class GrassLocation
        {
            public readonly int Index;
            public int Count;

            public GrassLocation(int index, int count)
            {
                Index = index;
                Count = count;
            }
        }

        private void OnGameStart(EventArgs args)
        {
            Helper = new Helper();

            Chat.Print(
                string.Format(
                    "{0} v{1} loaded.",
                    Assembly.GetExecutingAssembly().GetName().Name,
                    Assembly.GetExecutingAssembly().GetName().Version
                    )
                );
        }

        private void OnDraw(EventArgs args)
        {
            if (Config.Item("drawplaces").GetValue<Circle>().Active)
            {
                Ward.DrawWardSpots();
                Ward.DrawSafeWardSpots();
            }
        }
    }

}
