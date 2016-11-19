using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Utility
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Script.Serialization;

    internal class SkinChange : Program
    {
        private static int SkinID;

        private static readonly WebClient Webclient = new WebClient();
        private static readonly JavaScriptSerializer JSerializer = new JavaScriptSerializer();
        private static readonly List<SkinInfo> SkinList = new List<SkinInfo>();

        private new static readonly Menu Menu = Utilitymenu;

        private class SkinInfo
        {
            internal string Name;
        }

        static SkinChange()
        {
            var versionJson = Webclient.DownloadString("http://ddragon.leagueoflegends.com/realms/na.json");
            var gameVersion =
                (string)
                ((Dictionary<string, object>)JSerializer.Deserialize<Dictionary<string, object>>(versionJson)["n"])
                    ["champion"];
            var champJson =
                Webclient.DownloadString(
                    $"http://ddragon.leagueoflegends.com/cdn/{gameVersion}/data/en_US/champion/{ObjectManager.Player.ChampionName}.json");
            var skins =
                (ArrayList)
                ((Dictionary<string, object>)
                ((Dictionary<string, object>)
                    JSerializer.Deserialize<Dictionary<string, object>>(champJson)["data"])[
                    ObjectManager.Player.ChampionName])["skins"];

            foreach (Dictionary<string, object> skin in skins)
            {
                SkinList.Add(new SkinInfo
                {
                    Name = skin["name"].ToString().Contains("default")
                        ? ObjectManager.Player.ChampionName
                        : skin["name"].ToString()
                });
            }
        }

        public static void Init()
        {
            SkinID = Me.SkinId;

            var SkinMenu = Menu.AddSubMenu(new Menu("Skin Change", "Skin Change"));
            {
                SkinMenu.AddItem(new MenuItem("EnableSkin", "Enabled", true).SetValue(false)).ValueChanged += EnbaleSkin;
                SkinMenu.AddItem(
                    new MenuItem("SelectSkin", "Select Skin: ", true).SetValue(
                        new StringList(SkinList.Select(x => x.Name).ToArray())));
            }

            Game.OnUpdate += OnUpdate;
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                Me.SetSkin(Me.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                Me.SetSkin(Me.ChampionName,
                    Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }
        }
    }
}
