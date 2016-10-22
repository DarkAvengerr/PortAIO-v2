using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

using LeagueSharp;
using LeagueSharp.SDK;
using LeagueSharp.SDK.Enumerations;
using LeagueSharp.SDK.UI;
using LeagueSharp.SDK.Utils;
using NLog;

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace xcKalista
{
    internal static class SkinManager
    {
        private static readonly WebClient Webclient = new WebClient();
        private static readonly JavaScriptSerializer JSerializer = new JavaScriptSerializer();

        private static readonly List<SkinInfo> SkinList = new List<SkinInfo>();

        private static int _skiniwant = -1;
        private static readonly int _defaultSkinID;

        private static MenuBool _enabledMenuBool;

        static SkinManager()
        {
            // Trees :3
            try
            {
                var versionJson = Webclient.DownloadString("http://ddragon.leagueoflegends.com/realms/na.json");
                var gameVersion = (string)((Dictionary<string, object>)JSerializer.Deserialize<Dictionary<string, object>>(versionJson)["n"])["champion"];
                var champJson = Webclient.DownloadString($"http://ddragon.leagueoflegends.com/cdn/{gameVersion}/data/en_US/champion/{GameObjects.Player.ChampionName}.json");
                var skins = (ArrayList)((Dictionary<string, object>)((Dictionary<string, object>)JSerializer.Deserialize<Dictionary<string, object>>(champJson)["data"])[GameObjects.Player.ChampionName])["skins"];

                foreach (Dictionary<string, object> skin in skins)
                {
                    SkinList.Add(new SkinInfo
                    {
                        Model = GameObjects.Player.ChampionName, ID = (int)skin["num"], Name = skin["name"].ToString().Contains("default") ? GameObjects.Player.ChampionName : skin["name"].ToString()
                    });
                }

                var firstOrDefault = SkinList.FirstOrDefault(x => x.Name == GameObjects.Player.BaseSkinName);
                if (firstOrDefault != null)
                {
                    _defaultSkinID = firstOrDefault.ID;
                }

                Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
            }
            catch
            {
                LogManager.GetCurrentClassLogger().Error("GetSkin Failed");
            }
        }

        private static bool Enabled => _enabledMenuBool.Value;

        internal static void Initialize(Menu menu)
        {
            var submenu = menu.Add(new Menu("Skins", "Skins"));

            _enabledMenuBool = submenu.Add(new MenuBool("Enabled", "Enabled", false));
            var skinListItem = submenu.Add(new MenuList<string>("Skin", "Skin", SkinList.Select(x => x.Name)));

            _enabledMenuBool.ValueChanged += (sender, args) =>
            {
                if (_enabledMenuBool.Value)
                {
                }
                else
                {
                }
            };

            skinListItem.ValueChanged += (sender, args) =>
            {
            };

        }

        private static void SetSkin(int skinid)
        {
            if (Enabled)
            {
            }
            _skiniwant = skinid;
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (Enabled && _skiniwant > -1 && sender.IsMe && args.Animation.ToLowerInvariant() == "respawn")
            {
            }
        }

        internal class SkinInfo
        {
            internal int ID;
            internal string Model;
            internal string Name;
        }
    }
}
