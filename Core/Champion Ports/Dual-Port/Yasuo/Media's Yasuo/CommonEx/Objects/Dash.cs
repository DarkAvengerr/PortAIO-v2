using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Objects
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using global::YasuoMedia.Yasuo.LogicProvider;

    using LeagueSharp;
    using LeagueSharp.Common;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK;

    using SharpDX;

    using Color = System.Drawing.Color;
    using Geometry = LeagueSharp.Common.Geometry;

    #endregion

    public class Dash
    {
        #region Fields

        /// <summary>
        ///     The direction
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        ///     The distance
        /// </summary>
        public float Distance;

        /// <summary>
        ///     The end position
        /// </summary>
        public Vector3 EndPosition;

        /// <summary>
        ///     The knocked up heroes
        /// </summary>
        public List<AIHeroClient> HeroesHitCircular = new List<AIHeroClient>();

        /// <summary>
        ///     Indicates if dash through skillshot
        /// </summary>
        public bool InSkillshot;

        /// <summary>
        ///     The knocked up minions
        /// </summary>
        public List<Obj_AI_Base> MinionsHitCircular = new List<Obj_AI_Base>();

        /// <summary>
        /// The provider wall dash
        /// </summary>
        public WallDashLogicProvider ProviderWallDash = new WallDashLogicProvider();

        /// <summary>
        ///     The start position
        /// </summary>
        public Vector3 StartPosition;

        /// <summary>
        ///     The Unit
        /// </summary>
        public Obj_AI_Base Unit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dash" /> class.
        /// </summary>
        /// <param name="from">start.</param>
        /// <param name="unit">The unit.</param>
        public Dash(Vector3 from, Obj_AI_Base unit)
            : this(from, unit.ServerPosition)
        {
            this.Unit = unit;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dash" /> class.
        /// </summary>
        /// <param name="from">start.</param>
        /// <param name="direction">The direction.</param>
        public Dash(Vector3 from, Vector3 direction)
        {
            this.StartPosition = from;
            this.EndPosition = Geometry.Extend(this.StartPosition, direction, GlobalVariables.Spells[SpellSlot.E].Range);

            this.Direction = this.EndPosition;

            this.Setup();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dash" /> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public Dash(Obj_AI_Base unit)
            : this(GlobalVariables.Player.ServerPosition, unit.ServerPosition)
        {
            this.Unit = unit;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Dash" /> class.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public Dash(Vector3 direction)
            : this(GlobalVariables.Player.ServerPosition, direction)
        {
            this.Direction = direction;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the danger value.
        /// </summary>
        /// <value>
        ///     The danger value.
        /// </value>
        public int DangerValue { get; private set; }

        /// <summary>
        ///     Gets the dash lenght.
        /// </summary>
        /// <value>
        ///     The dash lenght.
        /// </value>
        public float DashLenght { get; private set; }

        /// <summary>
        ///     Gets the dash time.
        /// </summary>
        /// <value>
        ///     The dash time.
        /// </value>
        public float DashTime { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this dash is wall dash.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is wall dash; otherwise, <c>false</c>.
        /// </value>
        public bool IsWallDash { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether wall dash saves time.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [wall dash saves time]; otherwise, <c>false</c>.
        /// </value>
        public bool WallDashSavesTime { get; protected internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws this instance.
        /// </summary>
        public void Draw()
        {
            var color = Color.White;

            if (this.EndPosition.CountEnemyHeroesInRange(375) > 0)
            {
                color = Color.Red;
            }
            Drawing.DrawLine(
                Drawing.WorldToScreen(this.StartPosition),
                Drawing.WorldToScreen(this.EndPosition),
                4f,
                color);

            Render.Circle.DrawCircle(this.EndPosition, 350, color);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Checks for knockups.
        /// </summary>
        public void CheckKnockups()
        {
            this.MinionsHitCircular = new List<Obj_AI_Base>();

            foreach (var enemy in HeroManager.Enemies)
            {
                if (Geometry.Distance(enemy, this.EndPosition) <= 350 && enemy.IsValid)
                {
                    this.HeroesHitCircular.Add(enemy);
                }
            }

            foreach (var minion in SebbyLib.Cache.GetMinions(this.EndPosition, 350, MinionTeam.NotAlly))
            {
                if (minion.IsValid)
                {
                    this.MinionsHitCircular.Add(minion);
                }
            }
        }

        /// <summary>
        ///     Checks for skillshots.
        /// </summary>
        private void CheckSkillshots()
        {
            var skillshotDict = new Dictionary<Skillshot, Geometry.Polygon>();

            foreach (var skillshot in Tracker.DetectedSkillshots)
            {
                var polygon = new Geometry.Polygon();

                switch (skillshot.SData.SpellType)
                {
                    case SpellType.SkillshotLine:
                        polygon = new Geometry.Polygon.Rectangle(
                            skillshot.StartPosition,
                            skillshot.EndPosition,
                            skillshot.SData.Radius);
                        break;
                    case SpellType.SkillshotCircle:
                        polygon = new Geometry.Polygon.Circle(skillshot.EndPosition, skillshot.SData.Radius);
                        break;
                    case SpellType.SkillshotArc:
                        polygon = new Geometry.Polygon.Sector(
                            skillshot.StartPosition,
                            skillshot.Direction,
                            skillshot.SData.Angle,
                            skillshot.SData.Radius);
                        break;
                }

                skillshotDict.Add(skillshot, polygon);
            }

            foreach (var skillshot in skillshotDict)
            {
                var clipperpath = skillshot.Value.ToClipperPath();
                var connectionPolygon = new Geometry.Polygon.Line(this.StartPosition, this.EndPosition);
                var connectionclipperpath = connectionPolygon.ToClipperPath();

                if (clipperpath.Intersect(connectionclipperpath).Any())
                {
                    this.InSkillshot = true;
                }
            }
        }

        /// <summary>
        ///     Checks for wall dash.
        /// </summary>
        /// <param name="minWallWidth">Minimum width of the wall.</param>
        private void CheckWallDash(float minWallWidth = 50)
        {
            if (this.ProviderWallDash.IsWallDash(this.Direction, this.DashLenght, minWallWidth))
            {
                this.IsWallDash = true;
            }
        }

        // TODO: Add PathBase in Skillshot (Based on Skillshot Danger value) , Add Enemies Around (Based on Priority), Add Allies Around, Add Minions Around (?)
        /// <summary>
        ///     Sets the danger value.
        /// </summary>
        private void SetDangerValue()
        {
            this.DangerValue = 0;
        }

        /// <summary>
        ///     Sets the length of the dash.
        /// </summary>
        private void SetDashLength()
        {
            if (Utility.IsWall(this.EndPosition) && !this.IsWallDash)
            {
                var newEndPosition = this.ProviderWallDash.GetFirstWallPoint(this.StartPosition, this.EndPosition);

                // BUG: Navmesh seems broken and just returns Vector.Zero sometimes
                // Fixed with Broscience...
                if (Geometry.Distance(this.EndPosition, newEndPosition) <= GlobalVariables.Spells[SpellSlot.E].Range
                    && newEndPosition != Vector3.Zero)
                {
                    this.EndPosition = newEndPosition;
                }
            }
            this.DashLenght = Geometry.Distance(this.StartPosition, this.EndPosition);
        }

        /// <summary>
        ///     Sets the dash time.
        /// </summary>
        private void SetDashTime()
        {
            this.DashTime = this.DashLenght / GlobalVariables.Spells[SpellSlot.E].Speed;
        }

        /// <summary>
        ///     Setups this instance.
        /// </summary>
        private void Setup()
        {
            this.SetDashLength();
            this.SetDangerValue();

            this.CheckWallDash();

            this.Distance = Geometry.Distance(this.StartPosition, this.EndPosition);

            this.CheckKnockups();
            this.CheckSkillshots();
        }

        #endregion
    }
}