#pragma warning disable 618
using EloBuddy; namespace ElUtilitySuite.Trackers
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

    internal class LastPositionTracker// : IPlugin
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
            Obj_AI_Base.OnTeleport += this.OnObjAiBaseTeleport;

            Drawing.OnPreReset += args =>
                {
                    this.text.OnLostDevice();
                    this.line.OnLostDevice();
                    this.sprite.OnLostDevice();
                };

            Drawing.OnPostReset += args =>
                {
                    this.text.OnResetDevice();
                    this.line.OnResetDevice();
                    this.sprite.OnResetDevice();
                };

            this.sprite = MDrawing.GetSprite();
            this.text = MDrawing.GetFont(this.Menu.Item("LastPosition.FontSize").GetValue<Slider>().Value);
            this.line = MDrawing.GetLine(1);
        }

        #endregion

        #region Methods

        private void DrawCircleMinimap(Vector3 center, float radius, Color color, int thickness = 5, int quality = 30)
        {
            var sharpColor = new ColorBGRA(color.R, color.G, color.B, 255);
            var pointList = new List<Vector3>();
            for (var i = 0; i < quality; i++)
            {
                var angle = i * Math.PI * 2 / quality;
                pointList.Add(
                    new Vector3(
                        center.X + radius * (float)Math.Cos(angle),
                        center.Y + radius * (float)Math.Sin(angle),
                        center.Z));
            }
            this.line.Width = thickness;
            this.line.Begin();
            for (var i = 0; i < pointList.Count; i++)
            {
                var a = pointList[i];
                var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

                var aonScreen = Drawing.WorldToMinimap(a);
                var bonScreen = Drawing.WorldToMinimap(b);

                this.line.Draw(new[] { aonScreen, bonScreen }, sharpColor);
            }
            this.line.End();
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed ||
                    !this.Menu.Item("LastPosition.Enabled").IsActive())
                {
                    return;
                }

                var map = this.Menu.Item("LastPosition.Map").IsActive();
                var minimap = this.Menu.Item("LastPosition.Minimap").IsActive();
                var ssCircle = this.Menu.Item("LastPosition.SSCircle").IsActive();
                var circleThickness = this.Menu.Item("LastPosition.CircleThickness").GetValue<Slider>().Value;
                //var circleColor = this.Menu.Item("LastPosition.CircleColor").IsActive();
                var totalSeconds = this.Menu.Item("LastPosition.TimeFormat").GetValue<StringList>().SelectedIndex == 1;
                var timerOffset = this.Menu.Item("LastPosition.SSTimerOffset").GetValue<Slider>().Value;
                var timer = this.Menu.Item("LastPosition.SSTimer").IsActive();


                this.sprite.Begin(SpriteFlags.AlphaBlend);
                foreach (var lp in this.lastPositions)
                {
                    if (!lp.Hero.IsDead && !lp.LastPosition.Equals(Vector3.Zero)
                        && lp.LastPosition.Distance(lp.Hero.Position) > 500)
                    {
                        lp.Teleported = false;
                        lp.LastSeen = Game.Time;
                    }
                    lp.LastPosition = lp.Hero.Position;
                    if (lp.Hero.IsVisible)
                    {
                        lp.Teleported = false;
                        if (!lp.Hero.IsDead)
                        {
                            lp.LastSeen = Game.Time;
                        }
                    }
                    if (!lp.Hero.IsVisible && !lp.Hero.IsDead)
                    {
                        var pos = lp.Teleported ? this.spawnPoint : lp.LastPosition;
                        var mpPos = Drawing.WorldToMinimap(pos);
                        var mPos = Drawing.WorldToScreen(pos);

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
                                        System.Drawing.Color.White,
                                        circleThickness,
                                        true);
                                }
                                if (minimap)
                                {
                                    this.DrawCircleMinimap(pos, radius, Color.White, circleThickness);
                                }
                            }
                        }

                        if (map && pos.IsOnScreen(50))
                        {
                            this.sprite.DrawCentered(this.heroTextures[lp.Hero.NetworkId], mPos);
                        }
                        if (minimap)
                        {
                            this.sprite.DrawCentered(this.heroTextures[lp.Hero.NetworkId], mpPos);
                        }

                        if (lp.IsTeleporting)
                        {
                            if (map && pos.IsOnScreen(50))
                            {
                                this.sprite.DrawCentered(this.teleportTexture, mPos);
                            }
                            if (minimap)
                            {
                                this.sprite.DrawCentered(this.teleportTexture, mpPos);
                            }
                        }

                        if (timer && !lp.LastSeen.Equals(0f) && Game.Time - lp.LastSeen > 3f)
                        {
                            var time = (Game.Time - lp.LastSeen).FormatTime(totalSeconds);
                            if (map && pos.IsOnScreen(50))
                            {
                                this.text.DrawTextCentered(
                                    time,
                                    new Vector2(mPos.X, mPos.Y + 15 + timerOffset),
                                    Color.White);
                            }
                            if (minimap)
                            {
                                this.text.DrawTextCentered(
                                    time,
                                    new Vector2(mpPos.X, mpPos.Y + 15 + timerOffset),
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

        private void OnObjAiBaseTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("LastPosition.Enabled").IsActive())
                {
                    return;
                }

                var packet = Packet.S2C.Teleport.Decoded(sender, args);
                var lastPosition = this.lastPositions.FirstOrDefault(e => e.Hero.NetworkId == packet.UnitNetworkId);
                if (lastPosition != null)
                {
                    switch (packet.Status)
                    {
                        case Packet.S2C.Teleport.Status.Start:
                            lastPosition.IsTeleporting = true;
                            break;
                        case Packet.S2C.Teleport.Status.Abort:
                            lastPosition.IsTeleporting = false;
                            break;
                        case Packet.S2C.Teleport.Status.Finish:
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
}