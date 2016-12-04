using EloBuddy; 
using LeagueSharp.Common; 
namespace ReformedAIO.Champions.Ezreal.Utility
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class EzrealSkinchanger : ChildBase
    {
        public override string Name { get; set; } = "Skinchanger";

        public void OnUpdate(EventArgs args)
        {
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.BaseSkinName, Menu.Item("Skin").GetValue<StringList>().SelectedIndex);
        }

        protected override void OnLoad(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnLoad(sender, eventArgs);
            var a = ObjectManager.Player.BaseSkinName;

            Menu.AddItem(new MenuItem("Skin", "Skin")).SetValue(new StringList(new[]
                                                                                   {
            "Classic Ezreal",  "Nottingham Ezreal", "Frosted Ezreal", "Striker Ezreal", "Pulsefire Ezreal", "Explorer Ezreal", "Debonair Ezreal", "TPA Ezreal",
                "Ace of Spades Ezreal", "Arcade Ezreal"
                                                                                   }));
        }

        protected override void OnDisable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnDisable(sender, eventArgs);
            Game.OnUpdate -= OnUpdate;
            //ObjectManager.//Player.SetSkin(ObjectManager.Player.BaseSkinName, ObjectManager.Player.SkinId);
        }

        protected override void OnEnable(object sender, FeatureBaseEventArgs eventArgs)
        {
            base.OnEnable(sender, eventArgs);
            Game.OnUpdate += OnUpdate;
        }
    }
}
