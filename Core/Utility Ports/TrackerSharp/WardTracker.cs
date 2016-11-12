#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using PortAIO.Properties;
using Color = System.Drawing.Color;

#endregion

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Tracker
{
    internal enum WardType
    {
        Green,
        Pink,
        Blue,
        Trap
    }

    internal class WardData
    {
        public int Duration;
        public string ObjectBaseSkinName;
        public int Range;
        public string SpellName;
        public WardType Type;


        public Bitmap Bitmap
        {
            get
            {
                switch (Type)
                {
                    case WardType.Green:
                        return Resources.Minimap_Ward_Green_Enemy;
                    case WardType.Blue:
                        return Resources.Minimap_Ward_Green_Enemy;
                    case WardType.Pink:
                        return Resources.Minimap_Ward_Pink_Enemy;
                    default:
                        return Resources.Minimap_Ward_Green_Enemy;
                }
            }
        }

        public Color Color
        {
            get
            {
                switch (Type)
                {
                    case WardType.Green:
                        return Color.Lime;
                    case WardType.Pink:
                        return Color.Magenta;
                    case WardType.Blue:
                        return Color.Blue;
                    default:
                        return Color.Red;
                }
            }
        }
    }

    internal class DetectedWard
    {
        private const float _scale = 0.7f;
        private Render.Circle _defaultCircle;
        private Render.Circle _defaultCircleFilled;
        private Render.Sprite _minimapSprite;
        private Render.Line _missileLine;
        private Render.Circle _rangeCircle;
        private Render.Circle _rangeCircleFilled;
        private Render.Text _timerText;

        public DetectedWard(WardData data,
            Vector3 position,
            int startT,
            Obj_AI_Base wardObject = null,
            bool isFromMissile = false)
        {
            WardData = data;
            Position = position;
            StartT = startT;
            WardObject = wardObject;
            IsFromMissile = isFromMissile;
            CreateRenderObjects();
        }

        public WardData WardData { get; set; }
        public int StartT { get; set; }

        public int Duration
        {
            get { return WardData.Duration; }
        }

        public int EndT
        {
            get { return StartT + Duration; }
        }

        public Vector3 StartPosition { get; set; }
        public Vector3 Position { get; set; }

        public Color Color
        {
            get { return WardData.Color; }
        }

        public int Range
        {
            get { return WardData.Range; }
        }

        public bool IsFromMissile { get; set; }
        public Obj_AI_Base WardObject { get; set; }

        private Vector2 MinimapPosition
        {
            get
            {
                return Drawing.WorldToMinimap(Position) +
                       new Vector2(-WardData.Bitmap.Width / 2f * _scale, -WardData.Bitmap.Height / 2f * _scale);
            }
        }

        public void CreateRenderObjects()
        {
            //Create the minimap sprite.

            if (Range == 1100)
            {
                _minimapSprite = new Render.Sprite(WardData.Bitmap, MinimapPosition);
                _minimapSprite.Scale = new Vector2(_scale, _scale);
                _minimapSprite.Add(0);
            }

            //Create the circle:
            _defaultCircle = new Render.Circle(Position, WardData.Type == WardType.Trap ? WardData.Range : 200, Color, 5, true);
            _defaultCircle.VisibleCondition +=
                sender =>
                    WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                    !WardTracker.Config.Item("Details").GetValue<KeyBind>().Active &&
                    Render.OnScreen(Drawing.WorldToScreen(Position));
            _defaultCircle.Add(0);
            _defaultCircleFilled = new Render.Circle(Position, WardData.Type == WardType.Trap ? WardData.Range : 200, Color.FromArgb(25, Color), -142857, true);
            _defaultCircleFilled.VisibleCondition +=
                sender =>
                    WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                    !WardTracker.Config.Item("Details").GetValue<KeyBind>().Active &&
                    Render.OnScreen(Drawing.WorldToScreen(Position));
            _defaultCircleFilled.Add(-1);

            //Create the circle that shows the range
            _rangeCircle = new Render.Circle(Position, Range, Color, 10, false);
            _rangeCircle.VisibleCondition +=
                sender =>
                    WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                    WardTracker.Config.Item("Details").GetValue<KeyBind>().Active;
            _rangeCircle.Add(0);

            _rangeCircleFilled = new Render.Circle(Position, Range, Color.FromArgb(25, Color), -142857, true);
            _rangeCircleFilled.VisibleCondition +=
                sender =>
                    WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                    WardTracker.Config.Item("Details").GetValue<KeyBind>().Active;
            _rangeCircleFilled.Add(-1);


            //Missile line;
            if (IsFromMissile)
            {
                _missileLine = new Render.Line(new Vector2(), new Vector2(), 2, new ColorBGRA(255, 255, 255, 255));
                _missileLine.EndPositionUpdate = () => Drawing.WorldToScreen(Position);
                _missileLine.StartPositionUpdate = () => Drawing.WorldToScreen(StartPosition);
                _missileLine.VisibleCondition +=
                    sender =>
                        WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                        WardTracker.Config.Item("Details").GetValue<KeyBind>().Active;
                _missileLine.Add(0);
            }


            //Create the timer text:
            if (Duration != int.MaxValue)
            {
                _timerText = new Render.Text(10, 10, "t", 18, new ColorBGRA(255, 255, 255, 255));
                _timerText.OutLined = true;
                _timerText.PositionUpdate = () => Drawing.WorldToScreen(Position);
                _timerText.Centered = true;
                _timerText.VisibleCondition +=
                    sender =>
                        WardTracker.Config.Item("Enabled").GetValue<bool>() &&
                        Render.OnScreen(Drawing.WorldToScreen(Position));

                _timerText.TextUpdate =
                    () =>
                        (IsFromMissile ? "?? " : "") + Utils.FormatTime((EndT - Utils.TickCount) / 1000f) +
                        (IsFromMissile ? " ??" : "");
                _timerText.Add(2);
            }
        }

        public bool Remove()
        {
            if (_minimapSprite != null)
            {
                _minimapSprite.Remove();
            }

            _defaultCircle.Remove();
            _rangeCircle.Remove();
            _rangeCircleFilled.Remove();
            _defaultCircleFilled.Remove();

            if (_timerText != null)
            {
                _timerText.Remove();
            }

            if (_missileLine != null)
            {
                _missileLine.Remove();
            }

            
            return true;
        }
    }

    /// <summary>
    ///     Ward tracker tracks enemy wards and traps.
    /// </summary>
    public static class WardTracker
    {
        private static readonly List<WardData> PossibleWards = new List<WardData>();
        private static readonly List<DetectedWard> DetectedWards = new List<DetectedWard>();
        private static readonly List<string> WardObjectNames = new List<string>();
        private const bool TrackAllies = false;

        public static Menu Config;

        public static List<Vector3> CrabWardPositions = new List<Vector3>
        {
            new Vector3(4400, 9600, -67),
            new Vector3(10500, 5170, -63)
        };

        static WardTracker()
        {
            //Add the posible wards and their detection type:

            #region PossibleWards

            //Trinkets:
            PossibleWards.Add(
                new WardData
                {
                    Duration = 60 * 1000,
                    ObjectBaseSkinName = "SightWard",
                    Range = 1100,
                    SpellName = "TrinketTotemLvl1",
                    Type = WardType.Green
                });

            PossibleWards.Add(
                new WardData
                {
                    Duration = int.MaxValue,
                    ObjectBaseSkinName = "SightWard",
                    Range = 605,
                    SpellName = "TrinketOrbLvl3",
                    Type = WardType.Blue
                });

            //Ward items and normal wards:
            PossibleWards.Add(
                new WardData
                {
                    Duration = 60 * 3 * 1000,
                    ObjectBaseSkinName = "SightWard",
                    Range = 1100,
                    SpellName = "SightWard",
                    Type = WardType.Green
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 150 * 1000,
                    ObjectBaseSkinName = "SightWard",
                    Range = 1100,
                    SpellName = "ItemGhostWard",
                    Type = WardType.Green
                });

            //Pinks:
            PossibleWards.Add(
                new WardData
                {
                    Duration = int.MaxValue,
                    ObjectBaseSkinName = "VisionWard",
                    Range = 1100,
                    SpellName = "VisionWard",
                    Type = WardType.Pink
                });

            //Traps
            PossibleWards.Add(
                new WardData
                {
                    Duration = 90 * 1000,
                    ObjectBaseSkinName = "CaitlynTrap",
                    Range = 100,
                    SpellName = "CaitlynYordleTrap",
                    Type = WardType.Trap
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 60 * 10 * 1000,
                    ObjectBaseSkinName = "TeemoMushroom",
                    Range = 150,
                    SpellName = "BantamTrap",
                    Type = WardType.Trap
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 60 * 1 * 1000,
                    ObjectBaseSkinName = "ShacoBox",
                    Range = 150,
                    SpellName = "JackInTheBox",
                    Type = WardType.Trap
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 60 * 2 * 1000,
                    ObjectBaseSkinName = "Nidalee_Spear",
                    Range = 100,
                    SpellName = "Bushwhack",
                    Type = WardType.Trap
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 120 * 1000,
                    ObjectBaseSkinName = "jhintrap",
                    Range = 130,
                    SpellName = "JhinE",
                    Type = WardType.Trap
                });
            PossibleWards.Add(
                new WardData
                {
                    Duration = 79 * 1000,
                    ObjectBaseSkinName = "sru_crabward",
                    Range = 1100,
                    SpellName = "",
                    Type = WardType.Green
                });
            
            #endregion

            foreach (var ward in PossibleWards)
            {
                WardObjectNames.Add(ward.ObjectBaseSkinName.ToLowerInvariant());
            }

            //Used for removing the wards that expire:
            Game.OnUpdate += GameOnOnGameUpdate;

            //Used to detect the wards when the unit that places the ward is visible:
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            //Used to detect the wards when the unit is not visible but the ward is.
            GameObject.OnCreate += Obj_AI_Base_OnCreate;

            //Used to detect the ward missile when neither the unit or the ward are visible:
            GameObject.OnCreate += ObjSpellMissileOnOnCreate;
        }

        public static void AttachToMenu(Menu menu)
        {
            Config = menu.AddSubMenu(new Menu("Ward Tracker", "Ward Tracker"));
            Config.AddItem(new MenuItem("Details", "Show more info").SetValue(new KeyBind(16, KeyBindType.Press)));
            Config.Item("Details").SetTooltip("Draw visible range of wards.");
            Config.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
        }

        private static void ObjSpellMissileOnOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid || !missile.SpellCaster.IsValid<AIHeroClient>() ||
                missile.SpellCaster.IsAlly && !TrackAllies || missile.SpellCaster.IsVisible ||
                missile.SData.Name != "itemplacementmissile")
            {
                return;
            }

            var sPos = missile.StartPosition;
            var ePos = missile.EndPosition;
            LeagueSharp.Common.Utility.DelayAction.Add(
                1000, delegate
                {
                    if (
                        DetectedWards.Any(
                            w =>
                                w.Position.To2D().Distance(sPos.To2D(), ePos.To2D(), false, false) < 300 &&
                                Math.Abs(w.StartT - Utils.TickCount) < 2000))
                    {
                        return;
                    }

                    var detectedWard = new DetectedWard(
                        PossibleWards[3], new Vector3(ePos.X, ePos.Y, NavMesh.GetHeightForPosition(ePos.X, ePos.Y)),
                        Utils.TickCount, null, true)
                    {
                        StartPosition = new Vector3(sPos.X, sPos.Y, NavMesh.GetHeightForPosition(sPos.X, sPos.Y))
                    };

                    DetectedWards.Add(detectedWard);
                });
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            var wardObject = sender as Obj_AI_Base;

            if (wardObject == null || !wardObject.IsValid || wardObject.IsAlly && !TrackAllies ||
                !WardObjectNames.Contains(wardObject.CharData.BaseSkinName.ToLowerInvariant()))

            {
                return;
            }
           
            var wardData =
                PossibleWards.FirstOrDefault(o => string.Equals(wardObject.CharData.BaseSkinName, o.ObjectBaseSkinName, StringComparison.InvariantCultureIgnoreCase));

            if (wardData == null)
            {
                return;
            }

            var startT = Utils.TickCount - (int) ((wardObject.MaxMana - wardObject.Mana) * 1000);
            var detectedWard =
                DetectedWards.FirstOrDefault(
                    w =>
                        w.Position.Distance(wardObject.Position) < 200 && w.WardObject == null &&
                        (Math.Abs(w.StartT - startT) < 5000 || wardData.Type != WardType.Green) && w.WardData.Type == wardData.Type);

            var position = wardObject.Position;

            if (detectedWard != null)
            {
                wardData.Duration = detectedWard.Duration;
                detectedWard.Remove();
                DetectedWards.Remove(detectedWard);
            }

            if (wardData.ObjectBaseSkinName == "sru_crabward")
            {
                position = CrabWardPositions.MinOrDefault(p => p.Distance(wardObject.Position));
            }
            
            DetectedWards.Add(new DetectedWard(wardData, position, startT, wardObject));
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;

            if (hero == null || sender.IsAlly && !TrackAllies)
            {
                return;
            }

            var wardData =
                PossibleWards.FirstOrDefault(
                    w => string.Equals(args.SData.Name, w.SpellName, StringComparison.InvariantCultureIgnoreCase));

            if (wardData == null)
            {
                return;
            }

            if (wardData.SpellName.Contains("TrinketTotem"))
            {
                wardData.Duration = 1000 * (60 + (int) Math.Round(3.5 * (hero.Level - 1)));
            }

            var endPosition = ObjectManager.Player.GetPath(args.End).ToList().Last();
            DetectedWards.Add(new DetectedWard(wardData, endPosition, Utils.TickCount));
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            //Delete the wards that expire or wards that get destroyed:
            DetectedWards.RemoveAll(
                w =>
                    (w.EndT <= Utils.TickCount && w.Duration != int.MaxValue && w.Remove()) ||
                    (w.WardObject != null && (!w.WardObject.IsValid || w.WardObject.IsDead) && w.Remove()));
        }
    }
}
