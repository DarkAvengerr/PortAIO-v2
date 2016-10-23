using EloBuddy;
namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using PortAIO.Properties;
    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Font = SharpDX.Direct3D9.Font;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Enumerations;

    internal class LastPositionTracker : IPlugin
    {
        #region Fields

        /// <summary>
        ///     The hero texture images
        /// </summary>
        private readonly Dictionary<int, Texture> heroTextures = new Dictionary<int, Texture>();

        /// <summary>
        ///     The hero last positions
        /// </summary>
        private readonly List<LastPositionStruct> lastPositions = new List<LastPositionStruct>();

        /// <summary>
        ///    The Line drawings
        /// </summary>
        private Line line;

        /// <summary>
        ///     The enemy spawningpoint
        /// </summary>
        private Vector3 spawnPoint;

        /// <summary>
        ///     Spire drawings
        /// </summary>
        private Sprite sprite;

        /// <summary>
        ///     Teleport texture images
        /// </summary>
        private Texture teleportTexture;

        /// <summary>
        ///     Drawing font face
        /// </summary>
        private Font text;


        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))
                           : rootMenu.Children.First(predicate);

            var ssMenu = menu.AddSubMenu(new Menu("Last position tracker", "lastpostracker"));
            {
                ssMenu.AddItem(new MenuItem("LastPosition.CircleThickness", "Circle Thickness").SetValue(new Slider(1, 1, 10)));
                ssMenu.AddItem(
                    new MenuItem("LastPosition.TimeFormat", "Time Format").SetValue(new StringList(new[] { "mm:ss", "ss" })));
                ssMenu.AddItem(new MenuItem("LastPosition.FontSize", "Font Size").SetValue(new Slider(13, 3, 30)));
                ssMenu.AddItem(new MenuItem("LastPosition.SSTimerOffset", "SS Timer Offset").SetValue(new Slider(5, 0, 20)));
                ssMenu.AddItem(
                   new MenuItem("LastPosition.CircleColor", "Circle Color").SetValue(new Circle(true, System.Drawing.Color.White)));
                ssMenu.AddItem(new MenuItem("LastPosition.SSTimer", "SS Timer").SetValue(false));
                ssMenu.AddItem(new MenuItem("LastPosition.SSCircle", "SS Circle").SetValue(false));
                ssMenu.AddItem(new MenuItem("LastPosition.Minimap", "Minimap").SetValue(true));
                ssMenu.AddItem(new MenuItem("LastPosition.Map", "Map").SetValue(true));

                ssMenu.AddItem(new MenuItem("LastPosition.Enabled", "Enabled").SetValue(true));
            }

            this.Menu = menu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            if (!HeroManager.Enemies.Any())
            {
                return;
            }

            EloBuddy.SDK.Core.DelayAction(() =>
            {
                var multiplicator = EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(1000, 1000, 0)).Distance(EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(2000, 1000, 0))) / 1000f;
                if (multiplicator <= float.Epsilon)
                {
                    Load();
                }
                else
                {
                    MinimapMultiplicator = multiplicator;
                    Vector2 leftUpper;
                    Vector2 rightLower;
                    if (Game.MapId == GameMapId.CrystalScar)
                    {
                        leftUpper = EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(0, 13800, 0));
                        rightLower = EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(13800, 0, 0));
                    }
                    else
                    {
                        leftUpper = EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(0, 14800, 0));
                        rightLower = EloBuddy.SDK.Extensions.WorldToMinimap(new Vector3(14800, 0, 0));
                    }

                    MinimapRectangle = new SharpDX.Rectangle((int)leftUpper.X, (int)leftUpper.Y, (int)(rightLower.X - leftUpper.X), (int)(rightLower.Y - leftUpper.Y));

                    this.teleportTexture = Resources.LP_Teleport.ToTexture();

                    var spawn = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
                    this.spawnPoint = spawn != null ? spawn.Position : Vector3.Zero;

                    foreach (var enemy in HeroManager.Enemies)
                    {
                        this.heroTextures[enemy.NetworkId] =
                            (ImageLoader.Load("LP", enemy.ChampionName) ?? Resources.LP_Default).ToTexture();

                        var eStruct = new LastPositionStruct(enemy) { LastPosition = this.spawnPoint };

                        this.lastPositions.Add(eStruct);
                    }


                    Drawing.OnEndScene += this.OnDrawingEndScene;

                    Teleport.OnTeleport += Teleport_OnTeleport;

                    this.sprite = MDrawing.GetSprite();
                    this.text = MDrawing.GetFont(this.Menu.Item("LastPosition.FontSize").GetValue<Slider>().Value);
                    this.line = MDrawing.GetLine(1);
                }
            }, 1000);
        }

        private void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;
            try
            {
                if (!this.Menu.Item("LastPosition.Enabled").IsActive())
                {
                    return;
                }

                var packet = new RecallInfTracker(sender.NetworkId, args.Status, args.Type, args.Duration, args.Start);
                var lastPosition = this.lastPositions.FirstOrDefault(e => e.Hero.NetworkId == unit.NetworkId);
                if (lastPosition != null)
                {
                    switch (packet.Status)
                    {
                        case TeleportStatus.Start:
                            lastPosition.IsTeleporting = true;
                            break;
                        case TeleportStatus.Abort:
                            lastPosition.IsTeleporting = false;
                            break;
                        case TeleportStatus.Finish:
                            lastPosition.Teleported = true;
                            lastPosition.IsTeleporting = false;
                            lastPosition.LastSeen = Game.Time;
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        public static SharpDX.Rectangle MinimapRectangle { get; private set; }

        private void DrawCircleMinimap(Vector3 screenPosition, float radius, System.Drawing.Color color, int thickness = 5, int quality = -1)
        {
            float PI2 = (float)Math.PI * 2;

            if (quality == -1)
            {
                quality = (int)(radius / 3 + 15);
            }

            var rad = new Vector2(0, radius);
            var segments = new List<MinimapCircleSegment>();
            var full = true;

            for (var i = 0; i <= quality; i++)
            {
                var pos = (screenPosition.To2D() + rad).RotateAroundPoint(screenPosition.To2D(), PI2 * i / quality);
                var contains = MinimapRectangle.Contains(pos);
                if (!contains)
                {
                    full = false;
                }
                segments.Add(new MinimapCircleSegment(pos, contains));
            }

            foreach (var ar in FindArcs(segments, full))
            {
                EloBuddy.SDK.Rendering.Line.DrawLine(color, thickness, ar);
            }
        }

        private static IEnumerable<Vector2[]> FindArcs(IReadOnlyList<MinimapCircleSegment> points, bool full)
        {
            var ret = new List<Vector2[]>();
            if (full)
            {
                ret.Add(points.Select(segment => segment.Position).ToArray());
                return ret;
            }
            var pos = 0;
            for (var c = 0; c < 3; c++)
            {
                int start = -1, stop = -1;
                for (var i = pos; i < points.Count; i++)
                {
                    if (points[i].IsValid)
                    {
                        if (start == -1)
                        {
                            start = i;
                        }
                    }
                    else
                    {
                        if (stop == -1 && start != -1)
                        {
                            stop = i;
                            pos = i;
                            if (start != 0)
                            {
                                break;
                            }
                            for (var j = points.Count - 1; j > 0; j--)
                            {
                                if (points[j].IsValid)
                                {
                                    start = j;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        if (i == points.Count - 1)
                        {
                            pos = points.Count;
                        }
                    }
                }
                var arc = new List<Vector2>();

                if (start == -1 || stop == -1)
                {
                    continue;
                }
                var pointer = start;

                while (true)
                {
                    if (pointer == stop)
                    {
                        break;
                    }
                    arc.Add(points[pointer].Position);
                    pointer++;
                    if (pointer == points.Count)
                    {
                        pointer = 0;
                    }
                }

                ret.Add(arc.ToArray());
            }

            return ret;
        }

        public static float MinimapMultiplicator { get; private set; }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {
                if (!this.Menu.Item("LastPosition.Enabled").IsActive())
                {
                    return;
                }

                var map = this.Menu.Item("LastPosition.Map").IsActive();
                var minimap = this.Menu.Item("LastPosition.Minimap").IsActive();
                var ssCircle = this.Menu.Item("LastPosition.SSCircle").IsActive();
                var circleThickness = this.Menu.Item("LastPosition.CircleThickness").GetValue<Slider>().Value;
                var circleColor = this.Menu.Item("LastPosition.CircleColor").GetValue<Circle>();
                var totalSeconds = this.Menu.Item("LastPosition.TimeFormat").GetValue<StringList>().SelectedIndex == 1;
                var timerOffset = this.Menu.Item("LastPosition.SSTimerOffset").GetValue<Slider>().Value;
                var timer = this.Menu.Item("LastPosition.SSTimer").IsActive();


                this.sprite.Begin(SpriteFlags.None);

                foreach (var lp in this.lastPositions)
                {
                    if (!lp.Hero.IsDead && !lp.LastPosition.Equals(Vector3.Zero)
                        && lp.LastPosition.Distance(lp.Hero.Position) > 500)
                    {
                        lp.Teleported = false;
                        lp.LastSeen = Game.Time;
                    }
                    lp.LastPosition = lp.Hero.ServerPosition;
                    if (lp.Hero.IsHPBarRendered)
                    {
                        lp.Teleported = false;
                        if (!lp.Hero.IsDead)
                        {
                            lp.LastSeen = Game.Time;
                        }
                    }
                    if (!lp.Hero.IsHPBarRendered && !lp.Hero.IsDead)
                    {
                        var pos = lp.Teleported ? this.spawnPoint : lp.LastPosition;
                        var mpPos = EloBuddy.SDK.Extensions.WorldToMinimap(pos);
                        var mPos = EloBuddy.SDK.Extensions.WorldToScreen(pos);

                        if (ssCircle && !lp.LastSeen.Equals(0f) && Game.Time - lp.LastSeen > 3f)
                        {
                            var radius = Math.Abs((Game.Time - lp.LastSeen - 1) * lp.Hero.MoveSpeed * 0.9f);
                            if (radius <= 8000)
                            {
                                if (map && pos.IsOnScreen(50))
                                {
                                    Render.Circle.DrawCircle(
                                        pos,
                                        radius,
                                        circleColor.Color,
                                        circleThickness,
                                        true);
                                }
                                if (minimap)
                                {
                                    this.DrawCircleMinimap(Drawing.WorldToMinimap(pos).To3D(), radius * MinimapMultiplicator, circleColor.Color, circleThickness);
                                }
                            }
                        }

                        if (map && pos.IsOnScreen(50))
                        {
                            this.sprite.DrawCentered(this.heroTextures[lp.Hero.NetworkId], Drawing.WorldToScreen(pos));
                        }
                        if (minimap)
                        {
                            this.sprite.DrawCentered(this.heroTextures[lp.Hero.NetworkId], Drawing.WorldToMinimap(pos));
                        }

                        if (lp.IsTeleporting)
                        {
                            if (map && pos.IsOnScreen(50))
                            {
                                this.sprite.DrawCentered(this.teleportTexture, Drawing.WorldToScreen(pos));
                            }
                            if (minimap)
                            {
                                this.sprite.DrawCentered(this.teleportTexture, Drawing.WorldToMinimap(pos));
                            }
                        }

                        if (timer && !lp.LastSeen.Equals(0f) && Game.Time - lp.LastSeen > 3f)
                        {
                            var time = (Game.Time - lp.LastSeen).FormatTime(totalSeconds);
                            if (map && pos.IsOnScreen(50))
                            {
                                this.text.DrawTextCentered(
                                    time,
                                    new Vector2(Drawing.WorldToScreen(pos).X, Drawing.WorldToScreen(pos).Y + 15 + timerOffset),
                                    Color.White);
                            }
                            if (minimap)
                            {
                                this.text.DrawTextCentered(
                                    time,
                                    new Vector2(Drawing.WorldToMinimap(pos).X, Drawing.WorldToMinimap(pos).Y + 15 + timerOffset),
                                    Color.White);
                            }
                        }
                    }
                }
                this.sprite.End();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"An error occurred: '{0}'", e);
            }
        }

        #endregion

        internal class LastPositionStruct
        {
            #region Constructors and Destructors

            public LastPositionStruct(AIHeroClient hero)
            {
                this.Hero = hero;
                this.LastPosition = Vector3.Zero;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     The hero
            /// </summary>
            public AIHeroClient Hero { get; private set; }

            /// <summary>
            ///     Hero busy teleporting
            /// </summary>
            public bool IsTeleporting { get; set; }

            /// <summary>
            ///     The last hero position
            /// </summary>
            public Vector3 LastPosition { get; set; }

            /// <summary>
            ///     The last seen position
            /// </summary>
            public float LastSeen { get; set; }

            /// <summary>
            ///     Hero teleported
            /// </summary>
            public bool Teleported { get; set; }

            #endregion
        }
    }
    internal class MinimapCircleSegment
    {
        internal readonly Vector2 Position;
        internal readonly bool IsValid;

        internal MinimapCircleSegment(Vector2 position, bool valid)
        {
            Position = position;
            IsValid = valid;
        }
    }

    public class RecallInfTracker
    {
        public int NetworkID;
        public int Duration;
        public int Start;
        public TeleportType Type;
        public TeleportStatus Status;

        public RecallInfTracker(int netid, TeleportStatus stat, TeleportType tpe, int dura, int star = 0)
        {
            NetworkID = netid;
            Status = stat;
            Type = tpe;
            Duration = dura;
            Start = star;
        }
    }
}