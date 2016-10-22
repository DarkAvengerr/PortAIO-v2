using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI.Notifications.Designs
{
    #region Using Directives

    using System.Collections.Generic;

    using LeagueSharp;

    using global::RethoughtLib.Classes.Observer;
    using global::RethoughtLib.Transitions;
    using global::RethoughtLib.Transitions.Abstract_Base;
    using global::RethoughtLib.Transitions.Implementations;
    using global::RethoughtLib.Utility;

    using SharpDX.Direct3D9;

    #endregion

    internal class DefaultNotificationDesign : NotificationDesign
    {
        #region Static Fields

        /// <summary>
        ///     The body font.
        /// </summary>
        private static readonly Font BodyFont = new Font(
            Drawing.Direct3DDevice,
            13,
            0,
            FontWeight.Normal,
            5,
            false,
            FontCharacterSet.Default,
            FontPrecision.Character,
            FontQuality.Antialiased,
            FontPitchAndFamily.Mono | FontPitchAndFamily.Decorative,
            "Tahoma");

        /// <summary>
        ///     The header font.
        /// </summary>
        private static readonly Font HeaderFont = new Font(
            Drawing.Direct3DDevice,
            16,
            0,
            FontWeight.Bold,
            5,
            false,
            FontCharacterSet.Default,
            FontPrecision.Character,
            FontQuality.Antialiased,
            FontPitchAndFamily.Mono | FontPitchAndFamily.Decorative,
            "Tahoma");

        /// <summary>
        ///     The sprite.
        /// </summary>
        private static readonly Sprite Sprite = new Sprite(Drawing.Direct3DDevice);

        #endregion

        #region Fields

        /// <summary>
        ///     The padding bot
        /// </summary>
        public int PaddingBot = 30;

        /// <summary>
        ///     The padding left
        /// </summary>
        public int PaddingLeft = 15;

        /// <summary>
        ///     The padding right
        /// </summary>
        public int PaddingRight = 15;

        /// <summary>
        ///     The padding top
        /// </summary>
        public int PaddingTop = 20;

        private readonly List<Observer> observers = new List<Observer>();

        /// <summary>
        ///     The Body
        /// </summary>
        private string body;

        /// <summary>
        ///     The Header
        /// </summary>
        private string header;

        /// <summary>
        ///     The height
        /// </summary>
        private int height = 150;

        /// <summary>
        ///     The original Body
        /// </summary>
        private string originalBody;

        /// <summary>
        ///     The original Header
        /// </summary>
        private string originalHeader;

        /// <summary>
        ///     The width
        /// </summary>
        private int width = 300;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNotificationDesign"/> class.
        /// </summary>
        public DefaultNotificationDesign()
        { }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Body.
        /// </summary>
        /// <value>
        ///     The Body.
        /// </value>
        public string Body
        {
            get
            {
                return this.body;
            }
            set
            {
                this.originalBody = value;

                value =
                    String.FormatText(
                        value,
                        BodyFont,
                        this.MaximumBodyLineWidth,
                        this.MaximumBodyLineHeight,
                        Sprite,
                        true).ToString();

                this.body = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Header.
        /// </summary>
        /// <value>
        ///     The Header.
        /// </value>
        public override string Header
        {
            get
            {
                return this.header;
            }
            set
            {
                this.originalHeader = value;

                value = String.FormatText(value, HeaderFont, this.MaximumHeaderLineWidth, 30, Sprite, false).ToString();

                this.header = string.IsNullOrEmpty(value) ? " " : value;
            }
        }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public override int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                if (value > 0 && value != this.height)
                {
                    this.height = value;
                    this.UpdateTexts();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the transition.
        /// </summary>
        /// <value>
        ///     The transition.
        /// </value>
        public override TransitionBase TransitionBase { get; set; } = new ElasticEaseInOut(500);

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public override int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                if (value > 0 && value != this.width)
                {
                    this.width = value;
                    this.UpdateTexts();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the maximum height of the body line.
        /// </summary>
        /// <value>
        ///     The maximum height of the body line.
        /// </value>
        private int MaximumBodyLineHeight => this.Height - (this.PaddingBot + this.PaddingTop);

        /// <summary>
        ///     The maximum body line length.
        /// </summary>
        private int MaximumBodyLineWidth => this.Width - (this.PaddingLeft + this.PaddingRight);

        /// <summary>
        ///     The maximum header line length.
        /// </summary>
        private int MaximumHeaderLineWidth => this.Width - (this.PaddingLeft + this.PaddingRight);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        public override void Update<T>(T notification)
        {
            this.UpdateTexts();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Updates the texts.
        /// </summary>
        private void UpdateTexts()
        {
            this.Header = this.originalHeader;
            this.Body = this.originalBody;
        }

        #endregion
    }
}