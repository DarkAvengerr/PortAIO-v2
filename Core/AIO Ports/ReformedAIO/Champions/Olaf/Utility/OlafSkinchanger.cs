using EloBuddy; 
 using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Olaf.Utility
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class OlafSkinchanger : ChildBase
    {
        public override string Name { get; set; } = "Skinchanger";

        public void OnUpdate(EventArgs args)
        {
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, Menu.Item("Skin").GetValue<StringList>().SelectedIndex);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
            var a = ObjectManager.Player.CharData.BaseSkinName;

            Menu.AddItem(new MenuItem("Skin", "Skin")).SetValue(new StringList(new[]
                                                                                   {
            "Classic Olaf",  "Forsaken Olaf", "Glacial Olaf", "Brolaf", "Pentakill Olaf", "Marauder Olaf", "Butcher Olaf"
                                                                                   }));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
            Game.OnUpdate -= OnUpdate;
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, ObjectManager.Player.SkinId);
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
            Game.OnUpdate += OnUpdate;
        }
    }
}
