using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions
{
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal sealed class OrbwalkingParent : ParentBase
    {
        private Orbwalking.Orbwalker orbwalker;

       // private readonly Orbwalking.OrbwalkingMode OrbwalkingMode;

        public OrbwalkingParent(Orbwalking.Orbwalker orbwalker)
        {
           
            orbwalker = orbwalker;
           // orbwalkingMode = orbwalkingMode;
        }

        public override string Name { get; set; }

        protected override void OnDisable(object sender, FeatureBaseEventArgs args)
        {
            base.OnDisable(sender, args);

            this.orbwalker.SetAttack(false);
            this.orbwalker.SetMovement(false);
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs args)
        {
            base.OnEnable(sender, args);

            this.orbwalker.SetAttack(true);
            this.orbwalker.SetMovement(true);
        }

        protected override void SetMenu()
        {
            base.SetMenu();
            this.orbwalker = new Orbwalking.Orbwalker(Menu.Parent);
            Menu = Menu.Parent.SubMenu("Orbwalk");
        }
    }
}
