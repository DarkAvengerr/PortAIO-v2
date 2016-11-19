using EloBuddy; 
using LeagueSharp.Common; 
 namespace ReformedAIO.Champions.Gragas.Utility
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using RethoughtLib.FeatureSystem.Abstract_Classes;

    internal class GragasSkinchanger : ChildBase
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
            "Classic Gragas",  "Scuba Gragas", "Hillbilly Gragas", "Santa Gragas", "Gragas, Esq.", "Vandal Gragas", "Oktoberfest Gragas", "Superfan Gragas", "Fnatic Gragas", "Gragas Caskbreaker"
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
