using EloBuddy; 
using LeagueSharp.Common; 
 namespace ElUtilitySuite.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;

    using ElUtilitySuite.Logging;
    using PortAIO.Properties;
    using ElUtilitySuite.Vendor.SFX;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class WardTracker : IPlugin
    {
        #region Constants

        private const float CheckInterval = 300f;

        #endregion

        #region Fields

        private readonly List<HeroWard> _heroNoWards = new List<HeroWard>();

        private readonly List<WardObject> _wardObjects = new List<WardObject>();

        private readonly List<WardStruct> _wardStructs = new List<WardStruct>
        {
            new WardStruct( 60 * 1, 1100, "YellowTrinket", "TrinketTotemLvl1", WardType.Green),
            new WardStruct( int.MaxValue, 1100, "BlueTrinket", "TrinketOrbLvl3", WardType.Green),
            new WardStruct( 60 * 2, 1100, "YellowTrinketUpgrade", "TrinketTotemLvl2", WardType.Green),
            new WardStruct( 60 * 3, 1100, "SightWard", "ItemGhostWard", WardType.Green),
            new WardStruct( 75 * 2, 1100, "SightWard", "SightWard", WardType.Green),
            new WardStruct( 60 * 3, 1100, "MissileWard", "MissileWard", WardType.Green),
            new WardStruct( 60 * 4, 212, "CaitlynTrap", "CaitlynYordleTrap", WardType.Trap),
            new WardStruct( 60 * 10, 212, "TeemoMushroom", "BantamTrap", WardType.Trap),
            new WardStruct( 60 * 1, 212, "ShacoBox", "JackInTheBox", WardType.Trap),
            new WardStruct( 60 * 2, 212, "Nidalee_Spear", "Bushwhack", WardType.Trap),
            new WardStruct( 60 * 10, 212, "Noxious_Trap", "BantamTrap", WardType.Trap),
            new WardStruct( 120 * 1, 130, "jhintrap", "JhinE", WardType.Trap),
            new WardStruct(int.MaxValue, 900, "JammerDevice", "JammerDevice", WardType.Pink)
        };

        private Texture _greenWardTexture;

        private float _lastCheck = Environment.TickCount;

        private Line _line;

        private Texture _pinkWardTexture;

        private Sprite _sprite;

        private Font _text;

        #endregion

        #region Enums

        private enum WardType
        {
            Green,

            Pink,

            Trap
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu
        /// </summary>
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

            var wardtrackerMenu = menu.AddSubMenu(new Menu("Wardtracker", "wardtracker"));
            {
                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.TimeFormat", "Time Format").SetValue(
                        new StringList(new[] { "mm:ss", "ss" })));
                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.FontSize", "Font Size").SetValue(new Slider(13, 3, 30)));
                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.CircleRadius", "Circle Radius").SetValue(new Slider(150, 25, 300)));
                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.CircleThickness", "Circle Thickness").SetValue(new Slider(2, 1, 10)));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.GreenCircle", "Green Circle").SetValue(true));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.GreenColor", "Green Color").SetValue(Color.Lime));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.PinkColor", "Pink Color").SetValue(Color.Magenta));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.TrapColor", "Trap Color").SetValue(Color.Red));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.VisionRange", "Vision Range").SetValue(true));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.Minimap", "Minimap").SetValue(true));

                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.FilterWards", "Filter Wards").SetValue(new Slider(250, 0, 600)));
                wardtrackerMenu.AddItem(
                    new MenuItem("wardtracker.Hotkey", "Hotkey").SetValue(new KeyBind(16, KeyBindType.Press)));
                wardtrackerMenu.AddItem(new MenuItem("wardtracker.PermaShow", "Perma Show").SetValue(false));

                wardtrackerMenu.AddItem(new MenuItem("wardtracker.Enabled", "Enabled").SetValue(true));
            }

            this.Menu = wardtrackerMenu;
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this._greenWardTexture = Resources.WT_Green.ToTexture();
            this._pinkWardTexture = Resources.WT_Pink.ToTexture();

            this._sprite = MDrawing.GetSprite();
            this._text = MDrawing.GetFont(this.Menu.Item("wardtracker.FontSize").GetValue<Slider>().Value);
            this._line = MDrawing.GetLine(this.Menu.Item("wardtracker.CircleThickness").GetValue<Slider>().Value);

            Game.OnUpdate += this.OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += this.OnObjAiBaseProcessSpellCast;
            GameObject.OnCreate += this.OnGameObjectCreate;
            GameObject.OnDelete += this.OnGameObjectDelete;
            Drawing.OnEndScene += this.OnDrawingEndScene;
            Game.OnWndProc += this.OnGameWndProc;
            AttackableUnit.OnCreate += this.OnAttackableUnitEnterVisiblityClient;

            Drawing.OnPreReset += args =>
                {
                    this._line.OnLostDevice();
                    this._sprite.OnLostDevice();
                    this._text.OnLostDevice();
                };

            Drawing.OnPostReset += args =>
                {
                    this._line.OnResetDevice();
                    this._sprite.OnResetDevice();
                    this._text.OnResetDevice();
                };
        }

        #endregion

        #region Methods

        private void CheckDuplicateWards(WardObject wObj)
        {
            try
            {
                var range = this.Menu.Item("wardtracker.FilterWards").GetValue<Slider>().Value;
                if (wObj.Data.Duration != int.MaxValue)
                {
                    foreach (var obj in this._wardObjects.Where(w => w.Data.Duration != int.MaxValue).ToList())
                    {
                        if (wObj.Position.Distance(obj.Position) < range)
                        {
                            this._wardObjects.Remove(obj);
                            return;
                        }
                        if (obj.IsFromMissile && !obj.Corrected)
                        {
                            var newPoint = obj.StartPosition.Extend(obj.EndPosition, -(range * 1.5f));
                            if (wObj.Position.Distance(newPoint) < range)
                            {
                                this._wardObjects.Remove(obj);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var obj in
                        this._wardObjects.Where(
                            w =>
                            w.Data.Duration != int.MaxValue && w.IsFromMissile
                            && w.Position.Distance(wObj.Position) < 100).ToList())
                    {
                        this._wardObjects.Remove(obj);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private WardStruct GetWardStructForInvisible(Vector3 start, Vector3 end)
        {
            return
                HeroManager.Enemies.Where(hero => this._heroNoWards.All(h => h.Hero.NetworkId != hero.NetworkId))
                    .Any(hero => hero.Distance(start.Extend(end, start.Distance(end) / 2f)) <= 1500f)
                && HeroManager.Enemies.Any(e => e.Level > 3)
                    ? this._wardStructs[3]
                    : this._wardStructs[0];
        }

        private void OnAttackableUnitEnterVisiblityClient(GameObject sender, EventArgs args)
        {
            try
            {
                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                if (!sender.IsValid || sender.IsDead || !sender.IsEnemy)
                {
                    return;
                }

                var hero = sender as AIHeroClient;
                if (hero != null)
                {
                    if (ItemData.Sightstone.GetItem().IsOwned(hero) || ItemData.Ruby_Sightstone.GetItem().IsOwned(hero)
                        || ItemData.Vision_Ward.GetItem().IsOwned(hero))
                    {
                        this._heroNoWards.RemoveAll(h => h.Hero.NetworkId == hero.NetworkId);
                    }
                    else
                    {
                        this._heroNoWards.Add(new HeroWard(hero));
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private void OnDrawingEndScene(EventArgs args)
        {
            try
            {
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }

                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                var totalSeconds = this.Menu.Item("wardtracker.TimeFormat").GetValue<StringList>().SelectedIndex == 1;
                var circleRadius = this.Menu.Item("wardtracker.CircleRadius").GetValue<Slider>().Value;
                var circleThickness = this.Menu.Item("wardtracker.CircleThickness").GetValue<Slider>().Value;
                var visionRange = this.Menu.Item("wardtracker.VisionRange").GetValue<bool>();
                var minimap = this.Menu.Item("wardtracker.Minimap").GetValue<bool>();
                var greenCircle = this.Menu.Item("wardtracker.GreenCircle").GetValue<bool>();
                var hotkey = this.Menu.Item("wardtracker.Hotkey").GetValue<KeyBind>().Active;
                var permaShow = this.Menu.Item("wardtracker.PermaShow").GetValue<bool>();

                this._sprite.Begin(SpriteFlags.AlphaBlend);

                foreach (var ward in this._wardObjects)
                {
                    var color =
                        this.Menu.Item(
                            "wardtracker."
                            + (ward.Data.Type == WardType.Green
                                   ? "Green"
                                   : (ward.Data.Type == WardType.Pink ? "Pink" : "Trap")) + "Color").GetValue<Color>();

                    if (ward.Position.IsOnScreen())
                    {
                        if (greenCircle || ward.Data.Type != WardType.Green)
                        {
                            // checking later
                            if (ward.Object == null || !ward.Object.IsValid
                                || (ward.Object != null && ward.Object.IsValid && !ward.Object.IsVisible))
                            {
                                Render.Circle.DrawCircle(ward.Position, circleRadius, color, circleThickness);
                            }
                        }

                        if (ward.Data.Type == WardType.Green && !ward.Data.Duration.Equals(int.MaxValue))
                        {
                            this._text.DrawTextCentered(
                                string.Format(
                                    "{0} {1} {0}",
                                    ward.IsFromMissile ? (ward.Corrected ? "?" : "??") : string.Empty,
                                    (ward.EndTime - Game.Time).FormatTime(totalSeconds)),
                                Drawing.WorldToScreen(ward.Position),
                                new SharpDX.Color(color.R, color.G, color.B, color.A));
                        }
                    }
                    if (minimap && ward.Data.Type != WardType.Trap)
                    {
                        this._sprite.DrawCentered(
                            ward.Data.Type == WardType.Green ? this._greenWardTexture : this._pinkWardTexture,
                            ward.MinimapPosition.To2D());
                    }
                    if (hotkey || permaShow)
                    {
                        if (visionRange)
                        {
                            Render.Circle.DrawCircle(
                                ward.Position,
                                ward.Data.Range,
                                Color.FromArgb(30, color),
                                circleThickness);
                        }
                        if (ward.IsFromMissile)
                        {
                            this._line.Begin();
                            this._line.Draw(
                                new[]
                                    {
                                        Drawing.WorldToScreen(ward.StartPosition), Drawing.WorldToScreen(ward.EndPosition)
                                    },
                                SharpDX.Color.White);
                            this._line.End();
                        }
                    }
                }
                this._sprite.End();
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private void OnGameObjectCreate(GameObject sender, EventArgs args)
        {
            try
            {
                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                var missile = sender as MissileClient;
                if (missile != null && missile.IsValid)
                {
                    if (missile.SpellCaster != null && !missile.SpellCaster.IsAlly && missile.SData != null)
                    {
                        if (missile.SData.Name.Equals("itemplacementmissile", StringComparison.OrdinalIgnoreCase)
                            && !missile.SpellCaster.IsVisible)
                        {
                            var sPos = missile.StartPosition;
                            var ePos = missile.EndPosition;

                            LeagueSharp.Common.Utility.DelayAction.Add(
                                1000,
                                delegate
                                    {
                                        if (
                                            !this._wardObjects.Any(
                                                w =>
                                                w.Position.To2D().Distance(sPos.To2D(), ePos.To2D(), false) < 300
                                                && ((int)Game.Time - w.StartT < 2)))
                                        {
                                            var wObj = new WardObject(
                                            this.GetWardStructForInvisible(sPos, ePos),
                                            new Vector3(ePos.X, ePos.Y, NavMesh.GetHeightForPosition(ePos.X, ePos.Y)),
                                            (int)Game.Time, null, true,
                                            new Vector3(sPos.X, sPos.Y, NavMesh.GetHeightForPosition(sPos.X, sPos.Y)),
                                            missile.SpellCaster);

                                            this.CheckDuplicateWards(wObj);
                                            this._wardObjects.Add(wObj);
                                        }
                                    });
                        }
                    }
                }
                else
                {
                    var wardObject = sender as Obj_AI_Base;
                    if(wardObject != null && wardObject.IsValid && !wardObject.IsAlly)
                    {
                        foreach (var ward in this._wardStructs)
                        {
                            if (wardObject.CharData.BaseSkinName.Equals(
                                ward.ObjectBaseSkinName, StringComparison.OrdinalIgnoreCase))
                            {
                                this._wardObjects.RemoveAll(
                                    w =>
                                        w.Position.Distance(wardObject.Position) < 300 &&
                                        ((int)Game.Time - w.StartT < 0.5));

                                var wObj = new WardObject(
                                    ward,
                                    new Vector3(wardObject.Position.X, wardObject.Position.Y, wardObject.Position.Z),
                                    (int)(Game.Time - (int)(wardObject.MaxMana - wardObject.Mana)), wardObject);

                                this.CheckDuplicateWards(wObj);
                                this._wardObjects.Add(wObj);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private void OnGameObjectDelete(GameObject sender, EventArgs args)
        {
            try
            {
                var ward = sender as Obj_AI_Base;
                if (ward != null && sender.Name.ToLower().Contains("ward"))
                {
                    this._wardObjects.RemoveAll(w => w.Object != null && w.Object.NetworkId == sender.NetworkId);
                    this._wardObjects.RemoveAll(
                        w =>
                        (Math.Abs(w.Position.X - ward.Position.X) <= (w.IsFromMissile ? 25 : 10))
                        && (Math.Abs(w.Position.Y - ward.Position.Y) <= (w.IsFromMissile ? 25 : 10)));
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private void OnGameUpdate(EventArgs args)
        {
            try
            {
                if (this._lastCheck + CheckInterval > Environment.TickCount)
                {
                    return;
                }

                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                this._lastCheck = Environment.TickCount;

                this._wardObjects.RemoveAll(
                    w =>
                    (w.Data.Duration != int.MaxValue && w.EndTime <= Game.Time)
                    || (w.Object != null && !w.Object.IsValid));
                foreach (var hw in this._heroNoWards.ToArray())
                {
                    if (hw.Hero.IsVisible)
                    {
                        hw.LastVisible = Game.Time;
                    }
                    else
                    {
                        if (Game.Time - hw.LastVisible >= 15)
                        {
                            this._heroNoWards.Remove(hw);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        private void OnGameWndProc(WndEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                if (args.Msg == (ulong)WindowsMessages.WM_LBUTTONDBLCLK
                    && this.Menu.Item("wardtracker.Hotkey").GetValue<KeyBind>().Active)
                {
                    var ward = this._wardObjects.OrderBy(w => Game.CursorPos.Distance(w.Position)).FirstOrDefault();
                    if (ward != null && Game.CursorPos.Distance(ward.Position) <= 300)
                    {
                        this._wardObjects.Remove(ward);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            try
            {
                if (!this.Menu.Item("wardtracker.Enabled").IsActive())
                {
                    return;
                }

                if (sender.IsAlly)
                {
                    return;
                }

                foreach (var ward in this._wardStructs)
                {
                    if (args.SData.Name.Equals(ward.SpellName, StringComparison.OrdinalIgnoreCase))
                    {
                        var wObj = new WardObject(
                            ward,
                            ObjectManager.Player.GetPath(args.End).LastOrDefault(),
                            (int)Game.Time);
                        this.CheckDuplicateWards(wObj);
                        this._wardObjects.Add(wObj);
                    }
                }
            }
            catch (Exception e)
            {
                Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
            }
        }

        #endregion

        private struct WardStruct
        {
            #region Fields

            public readonly int Duration;

            public readonly string ObjectBaseSkinName;

            public readonly int Range;

            public readonly string SpellName;

            public readonly WardType Type;

            #endregion

            #region Constructors and Destructors

            public WardStruct(int duration, int range, string objectBaseSkinName, string spellName, WardType type)
            {
                this.Duration = duration;
                this.Range = range;
                this.ObjectBaseSkinName = objectBaseSkinName;
                this.SpellName = spellName;
                this.Type = type;
            }

            #endregion
        }

        private class HeroWard
        {
            #region Constructors and Destructors

            public HeroWard(AIHeroClient hero)
            {
                this.Hero = hero;
                this.LastVisible = Game.Time;
            }

            #endregion

            #region Public Properties

            public AIHeroClient Hero { get; private set; }

            public float LastVisible { get; set; }

            #endregion
        }

        private class WardObject
        {
            #region Fields

            public readonly bool Corrected;

            public readonly Vector3 EndPosition;

            public readonly Vector3 MinimapPosition;

            public readonly Obj_AI_Base Object;

            public readonly Vector3 StartPosition;

            public readonly int StartT;

            private Vector3 _position;

            #endregion

            #region Constructors and Destructors

            public WardObject(
                WardStruct data,
                Vector3 position,
                int startT,
                Obj_AI_Base wardObject = null,
                bool isFromMissile = false,
                Vector3 startPosition = default(Vector3), Obj_AI_Base spellCaster = null)
            {
                try
                {
                    var pos = position;
                    if (isFromMissile)
                    {
                        var newPos = this.GuessPosition(startPosition, position);
                        if (!position.X.Equals(newPos.X) || !position.Y.Equals(newPos.Y))
                        {
                            pos = newPos;
                            this.Corrected = true;
                        }
                        if (!this.Corrected)
                        {
                            pos = startPosition;
                        }
                    }

                    this.IsFromMissile = isFromMissile;
                    this.Data = data;
                    this.Position = this.RealPosition(pos);
                    this.EndPosition = this.Position.Equals(position) || this.Corrected
                                           ? position
                                           : this.RealPosition(position);
                    this.MinimapPosition = Drawing.WorldToMinimap(this.Position).To3D();
                    this.StartT = startT;
                    this.StartPosition = startPosition.Equals(default(Vector3)) || this.Corrected
                                             ? startPosition
                                             : this.RealPosition(startPosition);
                    this.Object = wardObject;
                    if (data.ObjectBaseSkinName.ToLower().Contains("yellowtrinket"))
                    {
                        var caster = spellCaster as AIHeroClient;
                        this.OverrideDuration = data.Duration +
                                           (int)Math.Ceiling(caster != null && caster.IsValid
                                               ? caster.Level
                                               : Math.Min(18,
                                                   GameObjects.EnemyHeroes.Select(e => e.Level).Average() + 1) * 3.5f);
                    }
                }
                catch (Exception e)
                {
                    Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
                }
            }

            #endregion

            #region Public Properties

            public WardStruct Data { get; private set; }

            public int EndTime
            {
                get { return StartT + (OverrideDuration > 0 ? OverrideDuration : Data.Duration); }
            }

            private int OverrideDuration { get; set; }

            public bool IsFromMissile { get; private set; }

            public Vector3 Position
            {
                get
                {
                    if (this.Object != null && this.Object.IsValid && this.Object.IsVisible)
                    {
                        this._position = this.Object.Position;
                    }
                    return this._position;
                }
                private set
                {
                    this._position = value;
                }
            }

            #endregion

            #region Methods

            private Vector3 GuessPosition(Vector3 start, Vector3 end)
            {
                try
                {
                    var grass = new List<Vector3>();
                    var distance = start.Distance(end);
                    for (var i = 0; i < distance; i++)
                    {
                        var pos = start.Extend(end, i);
                        if (NavMesh.IsWallOfGrass(pos, 1))
                        {
                            grass.Add(pos);
                        }
                    }
                    return grass.Count > 0 ? grass[(int)(grass.Count / 2d + 0.5d * Math.Sign(grass.Count / 2d))] : end;
                }
                catch (Exception e)
                {
                    Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
                }
                return end;
            }

            private Vector3 RealPosition(Vector3 end)
            {
                try
                {
                    if (end.IsWall())
                    {
                        for (var i = 0; i < 500; i = i + 5)
                        {
                            var c = new Geometry.Polygon.Circle(end, i, 15).Points;
                            foreach (var item in c.OrderBy(p => p.Distance(end)).Where(item => !item.IsWall()))
                            {
                                return new Vector3(item.X, item.Y, NavMesh.GetHeightForPosition(item.X, item.Y));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.AddEntry(LoggingEntryType.Error, "@WardTracker.cs: An error occurred: {0}", e);
                }
                return end;
            }

            #endregion
        }
    }
}
 