//using EloBuddy; 
 //using LeagueSharp.Common; 
 //namespace YasuoMedia.Common.Classes
//{
//    using System;

//    using LeagueSharp;
//    using LeagueSharp.Common;

//    internal class ClassName : FeatureChild<ChildName>
//    {
//        #region Constructors and Destructors

//        public ClassName(ChildName parent)
//            : base(parent)
//        {
//            this.OnLoad();
//        }

//        #endregion

//        #region Public Properties

//        public override string Name => "ClassName";

//        #endregion

//        #region Public Methods and Operators

//        public void OnDraw(EventArgs args)
//        {
//        }

//        public void OnUpdate(EventArgs args)
//        {
//        }

//        #endregion

//        #region Methods

//        protected override void OnDisable()
//        {
//            Game.OnUpdate -= this.OnUpdate;
//            Drawing.OnDraw -= this.OnDraw;
//            base.OnDisable();
//        }

//        protected override void OnEnable()
//        {
//            Game.OnUpdate += this.OnUpdate;
//            Drawing.OnDraw += this.OnDraw;
//            base.OnEnable();
//        }

//        protected override void OnInitialize()
//        {
//            base.OnInitialize();
//        }

//        protected override sealed void OnLoad()
//        {
//            this.Menu = new Menu(this.Name, this.Name);
//            this.Menu.AddItem(new MenuItem(this.Name + "Enabled", "Enabled").SetValue(true));

//            this.FeatureParent.Menu.AddSubMenu(this.Menu);
//        }

//        #endregion
//    }
//}