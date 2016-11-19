using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Ashe.Utility
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class AsheSkinchanger : ChildBase
    {
        public override string Name { get; set; } = "Skinchanger";

        public void OnUpdate(EventArgs args)
        {
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.CharData.BaseSkinName, Menu.Item("Skin").GetValue<StringList>().SelectedIndex);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);

            Menu.AddItem(new MenuItem("Skin", "Skin")).SetValue(new StringList(new[]
                                                                                   {
            "Classic Ashe", "Freljord Ashe", "Sherwood Forest Ashe", "Queen Ashe", "Woad Ashe", "Amethyst Ashe", "Heartseeker Ashe", "Marauder Ashe", "PROJECT: Ashe"
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
