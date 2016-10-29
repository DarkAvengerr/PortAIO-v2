using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Utilities.Modules.Skinchanger
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class Skinchanger : ChildBase
    {
        public override string Name { get; set; } = "Skinchanger";

        public void OnUpdate(EventArgs args)
        {
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, Menu.Item("Skin").GetValue<StringList>().SelectedIndex);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs featureBaseEventArgs)
        {
            base.OnLoad(sender, featureBaseEventArgs);

            Menu.AddItem(new MenuItem("Skin", "Skin")).SetValue(new StringList(new []
                                                                                   {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
                                                                                   }));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
            Game.OnUpdate -= OnUpdate;
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
            Game.OnUpdate += OnUpdate;
        }
    }
}
