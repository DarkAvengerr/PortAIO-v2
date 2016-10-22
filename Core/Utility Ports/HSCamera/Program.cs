using System;
using System.Collections.Generic;
using System.Linq;
using hsCamera.Handlers;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace hsCamera
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        public static void Main()
        {
            HsCameraOnLoad();
        }

        private static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Menu _config;
        public static List<AIHeroClient> HeroList => HeroManager.Enemies.ToList();

        private static void AddChampionMenu()
        {
            var enemycammenu = _config.AddSubMenu(new Menu("Enemies Camera", "enemiescamera"));
            for (int i = 0; i < HeroList.Count; i++)
            {
                enemycammenu.AddItem(new MenuItem("enemies" + HeroList[i].ChampionName, "Show Enemy -> (" + HeroList[i].ChampionName + ")")
                    .SetValue(new KeyBind(Convert.ToUInt32(96 + i), KeyBindType.Press)));

                _config.Item("enemies"+ HeroList[i].ChampionName).ValueChanged += (sender, e) =>
                 {
                     if (e.GetNewValue<KeyBind>().Active == e.GetOldValue<KeyBind>().Active) return;
                     if (e.GetNewValue<KeyBind>().Active == false) CameraMovement.SemiDynamic(Player.Position);
                 };
            }
        }

        /// <summary>
        /// Amazing OnLoad :jew:
        /// </summary>
        /// <param name="args"></param>
        private static void HsCameraOnLoad()
        {
            _config = new Menu("hsCamera [Official]", "hsCamera", true);
            {
                AddChampionMenu();
                var cameraSpeeds = _config.AddSubMenu(new Menu("Camera Speeds [Settings]", "cameraspeeds"));
                cameraSpeeds.AddItem(new MenuItem("followcurspeed", "Follow Cursor (Camera Speed)"))
                            .SetValue(new Slider(23, 1, 50));
                cameraSpeeds.AddItem(new MenuItem("followtfspeed", "Follow TeamFights (Camera Speed)"))
                            .SetValue(new Slider(17, 1, 50));
                _config.AddItem(new MenuItem("follow.dynamic", "Follow-Champion Camera?").SetValue(new KeyBind(17, KeyBindType.Press)));
                _config.AddItem(new MenuItem("dynamicmode", "Camera Mode?").SetValue(new StringList(new[] { "Normal", "Follow Cursor", "Follow Teamfights" }, 2)));
                _config.AddItem(new MenuItem("followoffset", "Follow Cursor (Range/Offset)").SetValue(new Slider(400, 0, 700)));
                _config.AddItem(new MenuItem("CLH", "Last Hit").SetValue(new KeyBind('X', KeyBindType.Press)));
                _config.AddItem(new MenuItem("CLC", "Lane Clear").SetValue(new KeyBind('V', KeyBindType.Press)));
                _config.AddItem(new MenuItem("CCombo", "Spacebar (Main)").SetValue(new KeyBind(32, KeyBindType.Press)))
                .ValueChanged += (sender, e) =>
                 {
                     if (e.GetNewValue<KeyBind>().Active == e.GetOldValue<KeyBind>().Active) return;
                     if (e.GetNewValue<KeyBind>().Active == false) CameraMovement.SemiDynamic(Player.Position);
                 };
                _config.AddItem(new MenuItem("credits", "                      .:Official Version of hsCamera:.")).SetFontStyle(System.Drawing.FontStyle.Bold, SharpDX.Color.DeepPink);
                _config.AddToMainMenu();
            }
            Chat.Print("<font color='#800040'>[6.17] hsCamera</font> <font color='#ff6600'>Loaded!</font>");
            Game.OnUpdate += HsCameraOnUpdate;
        }

        

        private static void HsCameraOnUpdate(EventArgs args)
        {
            AllModes.AllModes.CameraMode();

            for (int i = 0; i < HeroList.Count; i++)
            {
                if (_config.Item("enemies"+ HeroList[i].ChampionName).GetValue<KeyBind>().Active)
                {
                    if (HeroList[i].IsValid && HeroList[i] != null)
                    {
                        var position = HeroList[i].Position;
                        Camera.ScreenPosition = position.To2D();
                    }
                }
            }
            
        }
        
    }
}
