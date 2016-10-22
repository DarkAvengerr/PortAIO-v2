using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using ClipperLib;
using LeagueSharp;
using LeagueSharp.Common;
//using LeagueSharp.Sandbox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Config = LeagueSharp.Common.Config;
using Font = SharpDX.Direct3D9.Font;
using MenuItem = LeagueSharp.Common.MenuItem;
using Rectangle = SharpDX.Rectangle;
using ResourceManager = System.Resources.ResourceManager;

using EloBuddy;
using LeagueSharp.Common;
namespace SAssemblies
{
    using System.Drawing.Drawing2D;
    using System.Net.Http;
    using System.Security.Permissions;

    using LeagueSharp.SDK;

    using Blend = SharpDX.Direct3D9.Blend;
    using Font = System.Drawing.Font;
    using Geometry = LeagueSharp.Common.Geometry;
    using Items = LeagueSharp.Common.Items;
    using Matrix = SharpDX.Matrix;
    using Notification = LeagueSharp.Common.Notification;
    using Notifications = LeagueSharp.Common.Notifications;

    public class Menu
    {
        public static MenuItemSettings GlobalSettings = new MenuItemSettings();

        public class MenuItemSettings
        {
            public event EventHandler<EventArgs> Activated;
            public bool ForceDisable;
            public dynamic Item;
            public LeagueSharp.Common.Menu Menu;
            public String Name;
            public List<MenuItemSettings> SubMenus = new List<MenuItemSettings>();
            public Type Type;
            public bool Initialized = false;

            public MenuItemSettings(Type type, dynamic item)
            {
                Type = type;
                Item = item;
            }

            public MenuItemSettings(dynamic item)
            {
                Item = item;
            }

            public MenuItemSettings(Type type)
            {
                Type = type;
            }

            public MenuItemSettings(String name)
            {
                Name = name;
            }

            public MenuItemSettings()
            {
            }

            public MenuItemSettings AddMenuItemSettings(String displayName, String name)
            {
                SubMenus.Add(new MenuItemSettings(name));
                MenuItemSettings tempSettings = GetMenuSettings(name);
                if (tempSettings == null)
                {
                    throw new NullReferenceException(name + " not found");
                }
                tempSettings.Menu = Menu.AddSubMenu(new LeagueSharp.Common.Menu(displayName, name));
                return tempSettings;
            }

            public bool GetActive()
            {
                if (Menu == null)
                    return false;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.DisplayName == Language.GetString("GLOBAL_ACTIVE"))
                    {
                        if (item.GetValue<bool>())
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }

            public void SetActive(bool active)
            {
                if (Menu == null)
                    return;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.DisplayName == Language.GetString("GLOBAL_ACTIVE"))
                    {
                        item.SetValue(active);
                        return;
                    }
                }
            }

            public MenuItem CreateActiveMenuItem(String menuName)
            {
                return CreateActiveMenuItem(menuName, null);
            }

            public MenuItem CreateActiveMenuItem(String menuName, Func<dynamic> func)
            {
                if (Menu == null)
                    return null;
                MenuItem menuItem = null;
                if (!Menu.Items.Any(x => x.DisplayName.Equals(Language.GetString("GLOBAL_ACTIVE"))))
                {
                    menuItem = Menu.AddItem(new MenuItem(menuName, Language.GetString("GLOBAL_ACTIVE")).SetValue(false));
                }
                else
                {
                    //menuItem = Menu.Items.First(x => x.DisplayName.Equals(Language.GetString("GLOBAL_ACTIVE")));
                }
                if (func != null)
                {
                    Activated += delegate { if (!ForceDisable && !Initialized) { Item = func.Invoke(); Initialized = true; } };
                    menuItem.ValueChanged += delegate (object sender, OnValueChangeEventArgs args) { if (args.GetNewValue<bool>()) { OnActivate(); } };
                    menuItem.SetValue(menuItem.GetValue<bool>()); //Trigger OnValueChange
                }
                return menuItem;
            }

            public MenuItem GetMenuItem(String menuName)
            {
                if (Menu == null)
                    return null;
                foreach (MenuItem item in Menu.Items)
                {
                    if (item.Name == menuName)
                    {
                        return item;
                    }
                }
                return null;
            }

            public LeagueSharp.Common.Menu GetSubMenu(String menuName)
            {
                if (Menu == null)
                    return null;
                return Menu.SubMenu(menuName);
            }

            public MenuItemSettings GetMenuSettings(String name)
            {
                foreach (MenuItemSettings menu in SubMenus)
                {
                    if (menu.Name.Contains(name))
                        return menu;
                }
                return null;
            }

            public void OnActivate()
            {
                var target = Activated;

                if (target != null)
                {
                    target(this, new EventArgs());
                }
            }
        }

        public static LeagueSharp.Common.Menu GetMenu(String menuName)
        {
            return LeagueSharp.Common.Menu.RootMenus.FirstOrDefault(x => x.Key.Contains(menuName)).Value;
        }

        public static LeagueSharp.Common.Menu GetSubMenu(LeagueSharp.Common.Menu menu, String menuName)
        {
            return menu.Children.FirstOrDefault(x => x.Name.Equals(menuName));
        }
    }

    public class Menu2
    {
        protected Dictionary<MenuItemSettings, Func<dynamic>> MenuEntries;
        public static MenuItemSettings GlobalSettings = new MenuItemSettings();

        public static LeagueSharp.SDK.UI.Menu CreateMainMenu(string name = "SAssemblies", string displayName = "SAssemblies")
        {
            Language.SetLanguage();
            LeagueSharp.SDK.UI.Menu mainMenu;
            if (LeagueSharp.SDK.UI.MenuManager.Instance[name] == null)
            {
                mainMenu = new LeagueSharp.SDK.UI.Menu(name, name, true);
                mainMenu.Add(new LeagueSharp.SDK.UI.Menu("By Screeder", "By Screeder V" + Assembly.GetExecutingAssembly().GetName().Version));
                mainMenu.Attach();
            }
            else
            {
                mainMenu = LeagueSharp.SDK.UI.MenuManager.Instance[name];
            }
            return mainMenu;
        }

        public static void CreateGlobalMenuItems(LeagueSharp.SDK.UI.Menu menu)
        {
            if (GlobalSettings.Menu != null)
                return;

            AddComponent(ref menu, new LeagueSharp.SDK.UI.Menu("SAssembliesGlobalSettings", "Global Settings"));
            GlobalSettings.Menu = (LeagueSharp.SDK.UI.Menu)menu["SAssembliesGlobalSettings"];
            AddComponent(ref GlobalSettings.Menu, new LeagueSharp.SDK.UI.MenuBool("SAssembliesGlobalSettingsServerChatPingActive", "Server Chat/Ping"));
            AddComponent(ref GlobalSettings.Menu, new LeagueSharp.SDK.UI.MenuSlider("SAssembliesGlobalSettingsVoiceVolume", "Voice Volume", 100));
        }

        public static LeagueSharp.SDK.UI.AMenuComponent AddComponent(ref LeagueSharp.SDK.UI.Menu menu, LeagueSharp.SDK.UI.AMenuComponent component)
        {
            if (menu == null)
                return null;

            if (!menu.Components.Any(x => x.Value.Name.Equals(component.Name)))
            {
                return menu.Add(component);
            }
            else
            {
                return menu[component.Name];
            }
        }

        public static LeagueSharp.SDK.UI.Menu AddMenu(ref LeagueSharp.SDK.UI.Menu menu, LeagueSharp.SDK.UI.Menu component)
        {
            if (menu == null)
                return null;

            if (!menu.Components.Any(x => x.Value.Name.Equals(component.Name)))
            {
                return menu.Add(component);
            }
            else
            {
                return (LeagueSharp.SDK.UI.Menu)menu[component.Name];
            }
        }

        public Tuple<MenuItemSettings, Func<dynamic>> GetDirEntry(MenuItemSettings menuItem)
        {
            return new Tuple<MenuItemSettings, Func<dynamic>>(menuItem, MenuEntries[menuItem]);
        }

        public Dictionary<MenuItemSettings, Func<dynamic>> GetDirEntries()
        {
            return MenuEntries;
        }

        public void UpdateDirEntry(ref MenuItemSettings oldMenuItem, MenuItemSettings newMenuItem)
        {
            Func<dynamic> save = MenuEntries[oldMenuItem];
            MenuEntries.Remove(oldMenuItem);
            MenuEntries.Add(newMenuItem, save);
            oldMenuItem = newMenuItem;
        }

        public class MenuItemSettings
        {
            public bool ForceDisable;
            public dynamic Item;
            public LeagueSharp.SDK.UI.Menu Menu;
            public String Name;
            public Type Type;

            public MenuItemSettings(Type type, dynamic item)
            {
                Type = type;
                Item = item;
            }

            public MenuItemSettings(dynamic item)
            {
                Item = item;
            }

            public MenuItemSettings(Type type)
            {
                Type = type;
            }

            public MenuItemSettings(String name)
            {
                Name = name;
            }

            public MenuItemSettings()
            {
            }

            public bool GetActive()
            {
                if (Menu == null)
                    return false;
                foreach (var menuComponent in Menu.Components)
                {
                    if (menuComponent.Value.DisplayName == Language.GetString("GLOBAL_ACTIVE"))
                    {
                        if (menuComponent.Value.GetValue<LeagueSharp.SDK.UI.MenuBool>().Value)
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }

            public void SetActive(bool active)
            {
                if (Menu == null)
                    return;
                foreach (var menuComponent in Menu.Components)
                {
                    if (menuComponent.Value.DisplayName == Language.GetString("GLOBAL_ACTIVE"))
                    {
                        menuComponent.Value.GetValue<LeagueSharp.SDK.UI.MenuBool>().Value = active;
                    }
                }
            }

            public void CreateActiveMenuItem(String menuName)
            {
                if (Menu == null)
                    return;

                if (!Menu.Components.Any(x => x.Value.Name.Equals(menuName)))
                {
                    Menu.Add(new LeagueSharp.SDK.UI.MenuBool(menuName, Language.GetString("GLOBAL_ACTIVE")));
                }
            }

            public LeagueSharp.SDK.UI.MenuItem GetMenuItem<T>(String menuName)
            {
                if (Menu == null)
                    return null;
                foreach (var menuComponent in Menu.Components)
                {
                    if (menuComponent.Value.Name == menuName)
                    {
                        return (LeagueSharp.SDK.UI.MenuItem)menuComponent.Value;
                    }
                }
                return null;
            }

            public LeagueSharp.SDK.UI.Menu GetSubMenu(String menuName)
            {
                if (Menu == null)
                    return null;
                return (LeagueSharp.SDK.UI.Menu)Menu[menuName];
            }
        }

        //public static MenuItemSettings  = new MenuItemSettings();
    }

    public static class Log
    {
        public static String File = "C:\\SAssemblies.log";
        public static String Prefix = "Packet";

        public static void LogString(String text, String file = null, String prefix = null)
        {
            switch (text)
            {
                case "missile":
                case "DrawFX":
                case "Mfx_pcm_mis.troy":
                case "Mfx_bcm_tar.troy":
                case "Mfx_bcm_mis.troy":
                case "Mfx_pcm_tar.troy":
                    return;
            }
            LogWrite(text, file, prefix);
        }

        private static void LogGamePacket(GamePacket result, String file = null, String prefix = null)
        {
            byte[] b = new byte[result.Size()];
            long size = result.Size();
            int cur = 0;
            while (cur < size - 1)
            {
                b[cur] = result.ReadByte(cur);
                cur++;
            }
            LogPacket(b, file, prefix);
        }

        public static void LogPacket(byte[] data, String file = null, String prefix = null)
        {
            if (!(data[0].ToHexString().Equals("AE") || data[0].ToHexString().Equals("29") || data[0].ToHexString().Equals("1A") || data[0].ToHexString().Equals("34") || data[0].ToHexString().Equals("6E") || data[0].ToHexString().Equals("85") || data[0].ToHexString().Equals("C4") || data[0].ToHexString().Equals("61") || data[0].ToHexString().Equals("38") || data[0].ToHexString().Equals("FE")))
                LogWrite(BitConverter.ToString(data), file, prefix);
        }

        private static void LogWrite(String text, String file = null, String prefix = null)
        {
            if (text == null)
                return;
            if (file == null)
                file = File;
            if (prefix == null)
                prefix = Prefix;
            using (var stream = new StreamWriter(file, true))
            {
                stream.WriteLine(prefix + "@" + Game.Time + ": " + text);
            }
        }
    }

    public static class Common
    {
        public static bool IsOnScreen(Vector3 vector)
        {
            Vector2 screen = Drawing.WorldToScreen(vector);
            if (screen[0] < 0 || screen[0] > Drawing.Width || screen[1] < 0 || screen[1] > Drawing.Height)
                return false;
            return true;
        }

        public static bool IsOnScreen(Vector2 vector)
        {
            Vector2 screen = vector;
            if (screen[0] < 0 || screen[0] > Drawing.Width || screen[1] < 0 || screen[1] > Drawing.Height)
                return false;
            return true;
        }

        public static Size ScaleSize(this Size size, float scale, Vector2 mainPos = default(Vector2))
        {
            size.Height = (int)(((size.Height - mainPos.Y) * scale) + mainPos.Y);
            size.Width = (int)(((size.Width - mainPos.X) * scale) + mainPos.X);
            return size;
        }

        public static bool IsInside(Vector2 mousePos, Size windowPos, float width, float height)
        {
            return Utils.IsUnderRectangle(mousePos, windowPos.Width, windowPos.Height, width, height);
        }

        public static bool IsInside(Vector2 mousePos, System.Drawing.Point windowPos, float width, float height)
        {
            return Utils.IsUnderRectangle(mousePos, windowPos.X, windowPos.Y, width, height);
        }

        public static bool IsInside(Vector2 mousePos, Vector2 windowPos, float width, float height)
        {
            return Utils.IsUnderRectangle(mousePos, windowPos.X, windowPos.Y, width, height);
        }

        public static Notification ShowNotification(string message, Color color, int duration = 0, bool dispose = true)
        {
            Notification not = new Notification(message, duration, dispose).SetTextColor(color);
            Notifications.AddNotification(not);
            return not;
        }

        public static Color Interpolate(this Color source, Color target, float percent, int alpha = 255)
        {
            var r = (byte)(source.R + (target.R - source.R) * percent);
            var g = (byte)(source.G + (target.G - source.G) * percent);
            var b = (byte)(source.B + (target.B - source.B) * percent);

            return Color.FromArgb(alpha, r, g, b);
        }

        public static SharpDX.Color Interpolate(this SharpDX.Color source, SharpDX.Color target, float percent, int alpha = 255)
        {
            var r = (byte)(source.R + (target.R - source.R) * percent);
            var g = (byte)(source.G + (target.G - source.G) * percent);
            var b = (byte)(source.B + (target.B - source.B) * percent);

            return new SharpDX.Color(r, g, b, alpha);
        }

        public static SharpDX.Color PercentColorRedToGreen(float percent, int alpha = 255)
        {
            if (percent < 0 || percent > 1) { return SharpDX.Color.Black; }

            int r, g;
            if (percent < 0.5)
            {
                r = 255;
                g = (int)(255 * percent / 0.5);  //closer to 0.5, closer to yellow (255,255,0)
            }
            else
            {
                g = 255;
                r = 255 - (int)(255 * (percent - 0.5) / 0.5); //closer to 1.0, closer to green (0,255,0)
            }
            return new SharpDX.Color(r, g, 0, alpha);
        }

        public static void ExecuteInOnGameUpdate(Action action)
        {
            GameUpdate a = null;
            a = delegate (EventArgs args)
            {
                action.Invoke();
                Game.OnUpdate -= a;
            };
            Game.OnUpdate += a;
        }
    }

    public static class SummonerSpells
    {
        public static SpellSlot GetIgniteSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("dot") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetSmiteSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("smite") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetHealSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("heal") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetBarrierSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("barrier") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetExhaustSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("exhaust") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetCleanseSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("boost") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetClairvoyanceSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("clairvoyance") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static SpellSlot GetFlashSlot()
        {
            foreach (SpellDataInst spell in ObjectManager.Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("flash") && spell.State == SpellState.Ready)
                    return spell.Slot;
            }
            return SpellSlot.Unknown;
        }
    }

    public class Downloader
    {
        public delegate void DownloadFinished(object sender, DlEventArgs args);

        public static String Host = "https://github.com/Screeder/SAwareness/raw/master/Sprites/SAwareness/";
        public static String Path = "CHAMP/";

        private readonly List<Files> _downloadQueue = new List<Files>();
        public event DownloadFinished DownloadFileFinished;

        public void AddDownload(String hostFile, String localFile)
        {
            _downloadQueue.Add(new Files(hostFile, localFile));
        }

        public void StartDownload()
        {
            StartDownloadInternal();
        }

        private async Task StartDownloadInternal()
        {
            var webClient = new WebClient();
            var tasks = new List<DlTask>();
            foreach (Files files in _downloadQueue)
            {
                Task t = webClient.DownloadFileTaskAsync(new Uri(Host + Path + files.OnlineFile), files.OfflineFile);
                tasks.Add(new DlTask(files, t));
            }
            foreach (DlTask task in tasks)
            {
                await task.Task;
                tasks.Remove(task);
                OnFinished(new DlEventArgs(task.Files));
            }
        }

        protected virtual void OnFinished(DlEventArgs args)
        {
            if (DownloadFileFinished != null)
                DownloadFileFinished(this, args);
        }

        public static void DownloadFile(String hostfile, String localfile)
        {
            var webClient = new WebClient();
            webClient.DownloadFile(Host + Path + hostfile, localfile);
        }

        public class DlEventArgs : EventArgs
        {
            public Files DlFiles;

            public DlEventArgs(Files files)
            {
                DlFiles = files;
            }
        }

        private struct DlTask
        {
            public readonly Files Files;
            public readonly Task Task;

            public DlTask(Files files, Task task)
            {
                Files = files;
                Task = task;
            }
        }

        public struct Files
        {
            public String OfflineFile;
            public String OnlineFile;

            public Files(String onlineFile, String offlineFile)
            {
                OnlineFile = onlineFile;
                OfflineFile = offlineFile;
            }
        }
    }

    public static class SpriteHelper
    {
        public enum TextureType
        {
            Default,
            Summoner,
            Item
        }

        public enum DownloadType
        {
            Champion,
            ChampionSkinSmall,
            ChampionSkinBig,
            Spell,
            Summoner,
            Passive,
            Item,
            ProfileIcon
        }

        public enum ChampionType
        {
            Champion,
            ChampionSkin,
            SpellQ,
            SpellW,
            SpellE,
            SpellR,
            SpellPassive,
            Summoner1,
            Summoner2,
            Item,
            None
        }

        private static Downloader _downloader = new Downloader();
        public static readonly Dictionary<String, byte[]> MyResources = new Dictionary<String, byte[]>();

        static SpriteHelper()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            List<String> resources = new List<string>() { ".Resources.SPRITES.AutoBuy.AutoBuy", ".Resources.SPRITES.AutoLevler.AutoLevler",
                ".Resources.SPRITES.EloDisplayer.EloDisplayer", ".Resources.SPRITES.SmartPing.SmartPing", ".Resources.SPRITES.Ui.Ui" };
            foreach (string resource in resources)
            {
                try
                {
                    ResourceManager resourceManager = new ResourceManager(assembly.GetName().Name + resource, assembly);
                    ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                    foreach (DictionaryEntry entry in resourceSet)
                    {
                        var conv = entry.Value as Bitmap;
                        if (conv != null)
                        {
                            if (!MyResources.ContainsKey(entry.Key.ToString().ToLower()))
                            {
                                MyResources.Add(entry.Key.ToString().ToLower(), (byte[])new ImageConverter().ConvertTo((Bitmap)entry.Value, typeof(byte[])));
                            }
                        }
                        else
                        {
                            if (!MyResources.ContainsKey(entry.Key.ToString().ToLower()))
                            {
                                MyResources.Add(entry.Key.ToString().ToLower(), (byte[])entry.Value);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private static Dictionary<String, Bitmap> cachedMaps = new Dictionary<string, Bitmap>();

        public static String ConvertNames(String name)
        {
            if (name == null)
                return name;
            if (name.ToLower().Contains("snowball"))
            {
                return "SummonerPoroThrow";
            }
            switch (name)
            {
                case "s5summonersmiteplayerganker":
                case "itemsmiteaoe":
                case "s5_summonersmitequick":
                case "s5_summonersmiteduel":
                case "summonersmite":
                    return "SummonerSmite";

                case "Wukong":
                    return "MonkeyKing";

                case "viw":
                    return "ViW";

                case "zedult":
                    return "ZedUlt";

                case "vayneinquisition":
                    return "VayneInquisition";

                case "reksaie":
                    return "RekSaiE";

                case "dravenspinning":
                    return "DravenSpinning";

                default:
                    return name;
            }
        }

        public static void DownloadImageOpGg(string name, String subFolder)
        {
            WebRequest request = null;
            WebRequest requestSize = null;
            request =
            WebRequest.Create("http://sk2.op.gg/images/profile_icons/" + name);
            requestSize =
            WebRequest.Create("http://sk2.op.gg/images/profile_icons/" + name);
            requestSize.Method = "HEAD";
            if (request == null || requestSize == null)
                return;
            try
            {
                long fileSize = 0;
                using (var resp = (HttpWebResponse)requestSize.GetResponse())
                {
                    if (resp.StatusCode == HttpStatusCode.OK)
                    {
                        fileSize = resp.ContentLength;
                    }
                }
                if (fileSize == GetFileSize(name, subFolder))
                    return;
                Stream responseStream;
                using (var response = (HttpWebResponse)request.GetResponse())
                    if (response.StatusCode == HttpStatusCode.OK)
                        using (responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    responseStream.CopyTo(memoryStream);
                                    SaveImage(name, memoryStream.ToArray(), subFolder);
                                }
                            }
                        }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot download file: {0}, Exception: {1}", name, ex);
            }
        }

        //public static void DownloadImageRiot(string name, DownloadType type, String subFolder)
        //{
        //    String version = "";
        //    try
        //    {
        //        String json = new WebClient().DownloadString("http://ddragon.leagueoflegends.com/realms/euw.json");
        //        version = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["v"];
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Cannot download file: {0}, Exception: {1}", name, ex);
        //        return;
        //    }
        //    WebRequest request = null;
        //    WebRequest requestSize = null;
        //    name = ConvertNames(name);
        //    if (type == DownloadType.Champion)
        //    {
        //        request =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/champion/" + name + ".png");
        //        requestSize =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/champion/" + name + ".png");
        //        requestSize.Method = "HEAD";
        //    }
        //    else if (type == DownloadType.Spell)
        //    {
        //        //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
        //        request =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize.Method = "HEAD";
        //    }
        //    else if (type == DownloadType.Summoner)
        //    {
        //        //summonerexhaust
        //        if (name.Contains("summonerodingarrison"))
        //            name = "SummonerOdinGarrison";
        //        else
        //            name = name[0].ToString().ToUpper() + name.Substring(1, 7) + name[8].ToString().ToUpper() + name.Substring(9, name.Length - 9);
        //        request =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize.Method = "HEAD";
        //    }
        //    else if (type == DownloadType.Item)
        //    {
        //        //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
        //        request =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize =
        //        WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
        //        requestSize.Method = "HEAD";
        //    }
        //    if (request == null || requestSize == null)
        //        return;
        //    try
        //    {
        //        long fileSize = 0;
        //        using (WebResponse resp = requestSize.GetResponse())
        //        {
        //            fileSize = resp.ContentLength;
        //        }
        //        if (fileSize == GetFileSize(name, subFolder))
        //            return;
        //        Stream responseStream;
        //        using (WebResponse response = request.GetResponse())
        //        using (responseStream = response.GetResponseStream())
        //        {
        //            if (responseStream != null)
        //            {
        //                using (var memoryStream = new MemoryStream())
        //                {
        //                    responseStream.CopyTo(memoryStream);
        //                    SaveImage(name, memoryStream.ToArray(), subFolder);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Cannot download file: {0}, Exception: {1}", name, ex);
        //    }
        //}

        private static String GetChampionPicName(String url)
        {
            String json = new WebClient().DownloadString(url);
            //Dictionary<String, Object> data = (Dictionary<String, Object>)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["data"];
            //Dictionary<String, Object> dataImage = (Dictionary<String, Object>)((data.First().Value));
            //Dictionary<String, Object> imageChampion = (Dictionary<String, Object>)((dataImage["image"]));
            //return (string)imageChampion["full"];
            //Dictionary<String, Object> data = JsonConvert.DeserializeObject<Dictionary<String, Object>>(json);
            //Dictionary<String, Object> dataImage = (Dictionary<String, Object>)((data.First().Value));
            //Dictionary<String, Object> imageChampion = (Dictionary<String, Object>)((dataImage["image"]));
            //return (string)imageChampion["full"];
            JObject data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject<Object>(json);
            return data["data"].First.First["image"]["full"].ToString();
        }

        private static String GetChampionSkinPicName(String url, int skinId)
        {
            String json = new WebClient().DownloadString(url);
            JObject data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject<Object>(json);
            return data["data"].First.First["image"]["full"].ToString().Replace(".png", "_" + skinId + ".jpg");
        }

        private static String GetSpellPicName(String url, int index)
        {
            String json = new WebClient().DownloadString(url);
            //String data = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["data"];
            ////Spells
            //String[] spellsSpells = (string[])new JavaScriptSerializer().Deserialize<Dictionary<String[], Object>>(data)[new String[] { "spells" }];
            //for (int i = 0; i < spellsSpells.Length; i++)
            //{
            //    if (i == index)
            //    {
            //        String imageSpell = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(spellsSpells[i])["image"];
            //        return (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(imageSpell)["full"];
            //    }
            //}
            JObject data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject<Object>(json);
            if (index > 3)
            {
                return data.SelectToken(data["data"].First.First["passive"].Path)["image"]["full"].ToString();
            }
            else
            {
                return data.SelectToken(data["data"].First.First["spells"].Path + "[" + index + "]")["image"]["full"].ToString();
            }
            //return data["data"].First.First["spells"].Children();   //["image"]["full"].ToString();
        }

        private static String GetSummonerSpellPicName(String url, String name)
        {
            String json = new WebClient().DownloadString(url);
            //String data = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["data"];
            //String summonerSpellName = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(data.ToLower())[name];
            //String imageSummonerSpell = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(summonerSpellName)["image"];
            //return (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(imageSummonerSpell)["full"];
            JObject data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject<Object>(json);
            var realName = JObject.Parse(data["data"].ToString()).GetValue(name, StringComparison.OrdinalIgnoreCase).Value<JToken>();
            return realName["image"]["full"].ToString();
        }

        private static String GetItemPicName(String url, String itemId)
        {
            String json = new WebClient().DownloadString(url);
            JObject data = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject<Object>(json);
            var realName = JObject.Parse(data["data"].ToString()).GetValue(itemId, StringComparison.OrdinalIgnoreCase).Value<JToken>();
            return realName["image"]["full"].ToString();
        }

        public static String DownloadImageRiot(String sName, ChampionType champType, DownloadType type, String subFolder, int skinId = 0)
        {
            String name = "";
            String version = "";
            try
            {
                String json = new WebClient().DownloadString("http://ddragon.leagueoflegends.com/realms/euw.json");
                version = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["v"];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot download file: {0}, Exception: {1}", name, ex);
                return "";
            }
            name = ConvertNames(sName);
            try
            {
                switch (champType)
                {
                    case ChampionType.Champion:
                        name = GetChampionPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json");
                        break;

                    case ChampionType.ChampionSkin:
                        name = GetChampionSkinPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", skinId);
                        break;

                    case ChampionType.SpellQ:
                        name = GetSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", 0);
                        break;

                    case ChampionType.SpellW:
                        name = GetSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", 1);
                        break;

                    case ChampionType.SpellE:
                        name = GetSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", 2);
                        break;

                    case ChampionType.SpellR:
                        name = GetSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", 3);
                        break;

                    case ChampionType.SpellPassive:
                        name = GetSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/champion/" + sName + ".json", 4);
                        break;

                    case ChampionType.Summoner1:
                    case ChampionType.Summoner2:
                        name = GetSummonerSpellPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/summoner.json", name);
                        break;

                    case ChampionType.Item:
                        name = GetItemPicName("http://ddragon.leagueoflegends.com/cdn/" + version + "/data/en_US/item.json", name);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot download file: {0}, ChampType: {1}, Exception: {2}", name, sName, ex);
                return "";
            }
            WebRequest request = null;
            WebRequest requestSize = null;
            if (type == DownloadType.Champion)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/champion/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/champion/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.ChampionSkinSmall)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.ChampionSkinBig)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/img/champion/splash/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/img/champion/splash/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.Spell)
            {
                //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.Summoner)
            {
                //summonerexhaust
                if (name.Contains("summonerodingarrison"))
                    name = "SummonerOdinGarrison";
                else
                    name = name[0].ToString().ToUpper() + name.Substring(1, 7) + name[8].ToString().ToUpper() + name.Substring(9, name.Length - 9);
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.Passive)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/passive/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/passive/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.Item)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/item/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/item/" + name);
                requestSize.Method = "HEAD";
            }
            else if (type == DownloadType.ProfileIcon)
            {
                //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + name);
                requestSize =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + name);
                requestSize.Method = "HEAD";
            }
            if (request == null || requestSize == null)
                return "";
            try
            {
                long fileSize = 0;
                using (WebResponse resp = requestSize.GetResponse())
                {
                    fileSize = resp.ContentLength;
                }
                if (fileSize == GetFileSize(name, subFolder))
                    return name;
                Stream responseStream;
                using (WebResponse response = request.GetResponse())
                using (responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            responseStream.CopyTo(memoryStream);
                            SaveImage(name, memoryStream.ToArray(), subFolder);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot download file: {0}, Exception: {1}", name, ex);
            }
            return name;
        }

        private static int GetFileSize(String name, String subFolder, bool png = true)
        {
            int size = 0;
            string loc = Path.Combine(new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LeagueSharp", "Assemblies", "cache",
                "SAssemblies", subFolder, name  + (png ? ".png" : ".jpg")
            });
            try
            {
                byte[] bitmap = File.ReadAllBytes(loc);
                size = bitmap.Length;
            }
            catch (Exception)
            {

            }
            return size;
        }

        public static void SaveImage(String name, /*Bitmap*/byte[] bitmap, String subFolder)
        {
            string loc = Path.Combine(new[]
            {
                Config.AppDataDirectory, "Assemblies", "cache",
                "SAssemblies", subFolder, name
            });
            Directory.CreateDirectory(Path.Combine(Config.AppDataDirectory,
                        "Assemblies", "cache", "SAssemblies", subFolder));
            File.WriteAllBytes(loc, bitmap/*(byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]))*/);
        }

        public static void LoadTexture(String name, ref SpriteInfo spriteInfo, String subFolder)
        {
            if (name == null)
                return;
            if (spriteInfo == null)
                spriteInfo = new SpriteInfo();
            Byte[] bitmap = null;
            name = ConvertNames(name);
            string loc = Path.Combine(new[]
            {
                Config.AppDataDirectory, "Assemblies", "cache",
                "SAssemblies", subFolder, name
            });
            try
            {
                bitmap = File.ReadAllBytes(loc);
                spriteInfo.Bitmap = (Bitmap)new ImageConverter().ConvertFrom(bitmap);
                spriteInfo.Sprite = new Render.Sprite(bitmap, new Vector2(0, 0));
                spriteInfo.DownloadFinished = true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Cannot load file: {0}, Exception: {1}", name, ex);
            }
        }

        //public static void LoadTexture(String name, ref SpriteInfo spriteInfo, String optionalName, RafLoader.ImageList list)
        //{
        //    if (spriteInfo == null)
        //        spriteInfo = new SpriteInfo();
        //    Byte[] bitmap = null;
        //    bitmap = RafLoader.GetImage(name, list, optionalName);
        //    try
        //    {
        //        if (bitmap == null)
        //            throw new Exception("Picture not available!");
        //        Texture tex = Texture.FromMemory(Drawing.Direct3DDevice, bitmap);
        //        spriteInfo.Sprite = new Render.Sprite(tex, new Vector2(0, 0));
        //        spriteInfo.Bitmap = spriteInfo.Sprite.Bitmap;
        //        spriteInfo.DownloadFinished = true;
        //        tex.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Cannot load file: {0}, Exception: {1}", name, ex);
        //    }
        //}

        public static void LoadTexture(Bitmap bitmap, ref SpriteInfo spriteInfo)
        {
            if (spriteInfo == null)
                spriteInfo = new SpriteInfo();
            try
            {
                if (bitmap == null)
                    throw new Exception("Picture not available!");
                Texture tex = Texture.FromMemory(Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])));
                spriteInfo.Sprite = new Render.Sprite(tex, new Vector2(0, 0));
                spriteInfo.Bitmap = spriteInfo.Sprite.Bitmap;
                spriteInfo.DownloadFinished = true;
                tex.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot load texture, Exception: {0}", ex);
            }
        }

        public static void LoadTexture(String name, ref Texture texture, TextureType type)
        {
            if ((type == TextureType.Default || type == TextureType.Summoner) && MyResources.ContainsKey(name.ToLower()))
            {
                try
                {
                    texture = Texture.FromMemory(Drawing.Direct3DDevice, MyResources[name.ToLower()]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else if (type == TextureType.Summoner && MyResources.ContainsKey(name.ToLower().Remove(name.Length - 1)))
            {
                try
                {
                    texture = Texture.FromMemory(Drawing.Direct3DDevice,
                        MyResources[name.ToLower().Remove(name.Length - 1)]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else if (type == TextureType.Item && MyResources.ContainsKey(name.ToLower().Insert(0, "_")))
            {
                try
                {
                    texture = Texture.FromMemory(Drawing.Direct3DDevice, MyResources[name.ToLower().Insert(0, "_")]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else
            {
                Console.WriteLine("SAwarness: " + name + " is missing. Please inform Screeder!");
            }
        }

        public static void LoadTexture(String name, ref SpriteInfo texture, TextureType type)
        {
            if (texture == null)
                texture = new SpriteInfo();
            Bitmap bmp;
            if ((type == TextureType.Default || type == TextureType.Summoner) && MyResources.ContainsKey(name.ToLower()))
            {
                try
                {
                    using (var ms = new MemoryStream(MyResources[name.ToLower()]))
                    {
                        bmp = new Bitmap(ms);
                    }
                    texture.Bitmap = (Bitmap)bmp.Clone();
                    texture.Sprite = new Render.Sprite(bmp, new Vector2(0, 0));
                    texture.DownloadFinished = true;
                    //texture.Sprite.UpdateTextureBitmap(bmp);
                    //texture = new Render.Sprite(bmp, new Vector2(0, 0));
                }
                catch (Exception ex)
                {
                    if (texture == null)
                    {
                        texture = new SpriteInfo();
                        texture.Sprite = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                    }
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else if (type == TextureType.Summoner && MyResources.ContainsKey(name.ToLower().Remove(name.Length - 1)))
            {
                try
                {
                    //texture = new Render.Sprite((Bitmap)Resources.ResourceManager.GetObject(name.ToLower().Remove(name.Length - 1)), new Vector2(0, 0));
                }
                catch (Exception ex)
                {
                    if (texture == null)
                    {
                        texture = new SpriteInfo();
                        texture.Sprite = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                    }
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else if (type == TextureType.Item && MyResources.ContainsKey(name.ToLower().Insert(0, "_")))
            {
                try
                {
                    //texture = new Render.Sprite((Bitmap)Resources.ResourceManager.GetObject(name.ToLower().Insert(0, "_")), new Vector2(0, 0));
                }
                catch (Exception ex)
                {
                    if (texture == null)
                    {
                        texture = new SpriteInfo();
                        texture.Sprite = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                    }
                    Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                }
            }
            else
            {
                if (texture == null)
                {
                    texture = new SpriteInfo();
                    texture.Sprite = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                }
                Console.WriteLine("SAwarness: " + name + " is missing. Please inform Screeder!");
            }
        }

        public static void LoadTexture(Bitmap map, ref Render.Sprite texture)
        {
            if (texture == null)
                texture = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
            texture.UpdateTextureBitmap(map);
        }

        public static bool LoadTexture(String name, ref Render.Sprite texture, DownloadType type)
        {
            try
            {
                Bitmap map = null;
                if (!cachedMaps.ContainsKey(name))
                {
                    //map = DownloadImageRiot(name, type);
                    cachedMaps.Add(name, (Bitmap)map.Clone());
                }
                else
                {
                    map = new Bitmap((Bitmap)cachedMaps[name].Clone());
                }
                if (map == null)
                {
                    texture = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                    Console.WriteLine("SAwarness: " + name + " is missing. Please inform Screeder!");
                    return false;
                }
                texture = new Render.Sprite(map, new Vector2(0, 0));
                //texture.UpdateTextureBitmap(map);
                return true;
                //texture = new Render.Sprite(map, new Vector2(0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                return false;
            }
        }

        public async static Task<SpriteInfo> LoadTextureAsync(String name, SpriteInfo texture, DownloadType type)
        {
            try
            {
                if (texture == null)
                    texture = new SpriteInfo();
                Render.Sprite tex = texture.Sprite;
                LoadTextureAsyncInternal(name, () => texture, x => texture = x, type);
                //texture.Sprite = tex;
                texture.LoadingFinished = true;
                return texture;
                //texture = new Render.Sprite(map, new Vector2(0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                return new SpriteInfo();
            }
        }

        public async static Task<Bitmap> DownloadImageAsync(string name, DownloadType type)
        {
            String json = new WebClient().DownloadString("http://ddragon.leagueoflegends.com/realms/euw.json");
            String version = (string)new JavaScriptSerializer().Deserialize<Dictionary<String, Object>>(json)["v"];
            WebRequest request = null;
            if (type == DownloadType.Champion)
            {
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/champion/" + name + ".png");
            }
            else if (type == DownloadType.Spell)
            {
                //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
            }
            else if (type == DownloadType.Summoner)
            {
                //summonerexhaust
                if (name.Contains("summonerodingarrison"))
                    name = "SummonerOdinGarrison";
                else
                    name = name[0].ToString().ToUpper() + name.Substring(1, 7) + name[8].ToString().ToUpper() + name.Substring(9, name.Length - 9);
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
            }
            else if (type == DownloadType.Item)
            {
                //http://ddragon.leagueoflegends.com/cdn/4.20.1/img/spell/AhriFoxFire.png
                request =
                WebRequest.Create("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/spell/" + name + ".png");
            }
            if (request == null)
                return null;
            try
            {
                Stream responseStream;
                Task<WebResponse> reqA = request.GetResponseAsync();
                using (WebResponse response = await reqA) //Crash with AsyncRequest
                using (responseStream = response.GetResponseStream())
                {
                    return responseStream != null ? new Bitmap(responseStream) : null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("SAwarness: Couldn't load texture: " + name + "\n Ex: " + ex);
                return null;
            }
        }

        private static void LoadTextureAsyncInternal(String name, Func<SpriteInfo> getTexture, Action<SpriteInfo> setTexture, DownloadType type)
        {
            try
            {
                SpriteInfo spriteInfo = getTexture();
                Render.Sprite texture;
                Bitmap map;
                if (!cachedMaps.ContainsKey(name))
                {
                    Task<Bitmap> bitmap = DownloadImageAsync(name, type);
                    if (bitmap == null || bitmap.Result == null || bitmap.Status == TaskStatus.Faulted)
                    {
                        texture = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                        Console.WriteLine("SAwarness: " + name + " is missing. Please inform Screeder!");
                        spriteInfo.Sprite = texture;
                        setTexture(spriteInfo);
                        throw new Exception();
                    }
                    map = bitmap.Result; //Change to Async to make it Async, currently crashing through loading is not thread safe.
                    //Bitmap map = await bitmap;
                    cachedMaps.Add(name, (Bitmap)map.Clone());
                }
                else
                {
                    map = new Bitmap((Bitmap)cachedMaps[name].Clone());
                }
                if (map == null)
                {
                    texture = new Render.Sprite(MyResources["questionmark"], new Vector2(0, 0));
                    spriteInfo.Sprite = texture;
                    setTexture(spriteInfo);
                    Console.WriteLine("SAwarness: " + name + " is missing. Please inform Screeder!");
                    throw new Exception();
                }
                spriteInfo.Bitmap = (Bitmap)map.Clone();
                texture = new Render.Sprite(map, new Vector2(0, 0));
                spriteInfo.DownloadFinished = true;
                spriteInfo.Sprite = texture;

                setTexture(spriteInfo);
                //texture = new Render.Sprite(map, new Vector2(0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine("SAwarness: Could not load async " + name + ".");
            }
        }

        public class SpecialBitmap : IDisposable
        {
            private Bitmap origBitmap;
            public Bitmap Bitmap;

            public SpecialBitmap(MemoryStream backgroundBitmap, float[] scale = null)
            {
                origBitmap = (Bitmap)Image.FromStream(backgroundBitmap);
                if (scale != null)
                {
                    origBitmap = ResizeBitmap(origBitmap, scale);
                }
                Bitmap = (Bitmap)origBitmap.Clone();
            }

            public SpecialBitmap(Bitmap backgroundBitmap, float[] scale = null)
            {
                origBitmap = (Bitmap)backgroundBitmap.Clone();
                if (scale != null)
                {
                    origBitmap = ResizeBitmap(origBitmap, scale);
                }
                Bitmap = (Bitmap)origBitmap.Clone();
            }

            public Image AddBitmap(Bitmap bitmap, System.Drawing.Point location, float scale = 1.0f)
            {
                if (bitmap == null) return Bitmap;
                using (System.Drawing.Bitmap original = new System.Drawing.Bitmap(Bitmap))
                {
                    using (System.Drawing.Bitmap temp = new System.Drawing.Bitmap(Bitmap.Width, Bitmap.Height, original.PixelFormat))
                    {
                        using (System.Drawing.Graphics newImage = System.Drawing.Graphics.FromImage(temp))
                        {
                            Size size = new Size((int)(bitmap.Width * scale), (int)(bitmap.Height * scale));
                            newImage.DrawImage((Image)original, new System.Drawing.Point(0, 0));
                            newImage.CompositingMode = CompositingMode.SourceOver;
                            using (System.Drawing.Bitmap modBitmap = new System.Drawing.Bitmap(bitmap, size))
                            {
                                modBitmap.MakeTransparent();
                                newImage.DrawImage((Image)modBitmap, location);
                                newImage.Flush();
                                Bitmap.Dispose();
                                Bitmap = (Bitmap)temp.Clone();
                            }
                        }
                    }
                }
                return Bitmap;
            }

            public Image AddColoredRectangle(System.Drawing.Point location, System.Drawing.Size size, System.Drawing.Color color, int alpha = 255)
            {
                using (System.Drawing.Bitmap original = new System.Drawing.Bitmap(Bitmap))
                {
                    using (System.Drawing.Bitmap temp = new System.Drawing.Bitmap(Bitmap.Width, Bitmap.Height, original.PixelFormat))
                    {
                        using (System.Drawing.Graphics newImage = System.Drawing.Graphics.FromImage(temp))
                        {
                            using (Brush brush = new SolidBrush(Color.FromArgb(alpha, color)))
                            {
                                newImage.DrawImage((Image)original, new System.Drawing.Point(0, 0));
                                newImage.CompositingMode = CompositingMode.SourceOver;
                                newImage.FillRectangle(brush, location.X, location.Y, size.Width, size.Height);
                                newImage.Flush();
                                Bitmap.Dispose();
                                Bitmap = (Bitmap)temp.Clone();
                            }
                        }
                    }
                }
                return Bitmap;
            }

            public Image AddText(String text, System.Drawing.Point location, Brush brush, bool centered = false, int size = 12, bool shadow = true)
            {
                using (System.Drawing.Bitmap original = new System.Drawing.Bitmap(Bitmap))
                {
                    using (System.Drawing.Bitmap temp = new System.Drawing.Bitmap(Bitmap.Width, Bitmap.Height, original.PixelFormat))
                    {
                        using (System.Drawing.Graphics newImage = System.Drawing.Graphics.FromImage(temp))
                        {
                            using (Font arialFont = new Font("Arial", size))
                            {
                                newImage.DrawImage((Image)original, new System.Drawing.Point(0, 0));
                                newImage.SmoothingMode = SmoothingMode.AntiAlias;
                                newImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                newImage.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                StringFormat format = new StringFormat()
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center
                                };
                                if (centered)
                                {
                                    if (shadow)
                                    {
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X + 1, location.Y), format);
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X - 1, location.Y), format);
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X, location.Y + 1), format);
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X, location.Y - 1), format);
                                    }
                                    newImage.DrawString(text, arialFont, brush, location, format);
                                }
                                else
                                {
                                    if (shadow)
                                    {
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X + 1, location.Y));
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X - 1, location.Y));
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X, location.Y + 1));
                                        newImage.DrawString(text, arialFont, Brushes.Black, new System.Drawing.Point(location.X, location.Y - 1));
                                    }
                                    newImage.DrawString(text, arialFont, brush, location);
                                }
                                newImage.Flush();
                                Bitmap.Dispose();
                                Bitmap = (Bitmap)temp.Clone();
                            }
                        }
                    }
                }
                return Bitmap;
            }

            public static Bitmap LoadBitmap(String name, String subFolder)
            {
                if (name == null)
                    return null;
                Byte[] bitmap = null;
                name = ConvertNames(name);
                string loc = Path.Combine(new[]
                {
                    Config.AppDataDirectory, "Assemblies", "cache",
                    "SAssemblies", subFolder, name
                });
                try
                {
                    bitmap = File.ReadAllBytes(loc);
                    return (Bitmap)new ImageConverter().ConvertFrom(bitmap);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Cannot load file: {0}, Exception: {1}", name, ex);
                }
                return null;
            }

            public static Bitmap ResizeBitmap(Bitmap bitmap, float scale)
            {
                return ResizeBitmap(bitmap, new[] { scale, scale });
            }

            public static Bitmap ResizeBitmap(Bitmap bitmap, float[] scale)
            {
                Size size = new Size((int)(bitmap.Width * scale[0]), (int)(bitmap.Height * scale[1]));
                using (System.Drawing.Bitmap modBitmap = new System.Drawing.Bitmap(bitmap, size))
                {
                    bitmap.Dispose();
                    return (Bitmap)modBitmap.Clone();
                }
            }

            public bool ResetBitmap()
            {
                if (!Bitmap.Equals(origBitmap))
                {
                    Bitmap.Dispose();
                    Bitmap = (Bitmap)origBitmap.Clone();
                    return true;
                }
                return false;
            }

            public void SetOriginalBitmap(Bitmap bitmap)
            {
                origBitmap.Dispose();
                origBitmap = (Bitmap)bitmap.Clone();
            }

            public void Dispose()
            {
                if (Bitmap != null)
                    Bitmap.Dispose();

                if (origBitmap != null)
                    origBitmap.Dispose();

            }
        }

        public class SpriteInfo : IDisposable
        {
            public enum OVD
            {
                Small,
                Big
            }

            public Render.Sprite Sprite;
            public Bitmap Bitmap;
            public Render.Text Text;
            public Rectangle TextLength;
            public bool DownloadFinished = false;
            public bool LoadingFinished = false;
            public OVD Mode = OVD.Small;

            public void Dispose()
            {
                if (Sprite != null)
                {
                    Sprite.Remove();
                    Sprite.Dispose();
                }

                if (Bitmap != null)
                    Bitmap.Dispose();

            }

            //~SpriteInfo()
            //{
            //    Dispose();
            //}
        }
    }

    //[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public static class Speech
    {
        private static Dictionary<int, SpeechSynthesizer> tts = new Dictionary<int, SpeechSynthesizer>();

        static Speech()
        {
            for (int i = 0; i < 4; i++)
            {
                tts.Add(i, new SpeechSynthesizer());
            }

            ReadOnlyCollection<InstalledVoice> list = tts[0].GetInstalledVoices();
            String strVoice = "";
            foreach (var voice in list)
            {
                if (voice.VoiceInfo.Culture.EnglishName.Contains("English") && voice.Enabled)
                {
                    strVoice = voice.VoiceInfo.Name;
                }
            }

            foreach (KeyValuePair<int, SpeechSynthesizer> speech in tts)
            {
                if (!strVoice.Equals(""))
                {
                    speech.Value.SelectVoice(strVoice);
                }
            }
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs e)
        {
            foreach (var speech in tts)
            {
                if (speech.Value.State != SynthesizerState.Ready)
                {
                    speech.Value.SpeakAsyncCancelAll();
                }
                speech.Value.Dispose();
            }
        }

        public static void Speak(String text)
        {
            bool speaking = false;
            foreach (var speech in tts)
            {
                if (speech.Value.State == SynthesizerState.Ready && !speaking)
                {
                    if (speech.Value.Volume !=
                        Menu.GlobalSettings.GetMenuItem("SAssembliesGlobalSettingsVoiceVolume")
                            .GetValue<Slider>()
                            .Value)
                    {
                        speech.Value.Volume =
                            Menu.GlobalSettings.GetMenuItem("SAssembliesGlobalSettingsVoiceVolume")
                                .GetValue<Slider>()
                                .Value;
                    }
                    speaking = true;
                    speech.Value.SpeakAsync(text);
                }
                else if (speech.Value.State != SynthesizerState.Ready)
                {
                    speech.Value.Pause();
                    speech.Value.Volume = 1;
                    speech.Value.Resume();
                }
            }
        }
    }

    public class Ward
    {
        public enum WardType
        {
            Stealth,
            Vision,
            Temp,
            TempVision
        }

        public static readonly List<WardItem> WardItems = new List<WardItem>();
        public static Menu.MenuItemSettings Wards = new Menu.MenuItemSettings();

        static Ward()
        {
            WardItems.Add(new WardItem(3360, "Feral Flare", "", 1000, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2043, "Vision Ward", "VisionWard", 600, 180, WardType.Vision));
            WardItems.Add(new WardItem(2044, "Stealth Ward", "SightWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3154, "Wriggle's Lantern", "WriggleLantern", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2045, "Ruby Sightstone", "ItemGhostWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2049, "Sightstone", "ItemGhostWard", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(2050, "Explorer's Ward", "ItemMiniWard", 600, 60, WardType.Stealth));
            WardItems.Add(new WardItem(3340, "Greater Stealth Totem", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3361, "Greater Stealth Totem", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3362, "Greater Vision Totem", "", 600, 180, WardType.Vision));
            WardItems.Add(new WardItem(3366, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3367, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3368, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3369, "Bonetooth Necklace", "", 600, 120, WardType.Stealth));
            WardItems.Add(new WardItem(3371, "Bonetooth Necklace", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3375, "Head of Kha'Zix", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3205, "Quill Coat", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3207, "Spirit of the Ancient Golem", "", 600, 180, WardType.Stealth));
            WardItems.Add(new WardItem(3342, "Scrying Orb", "", 2500, 2, WardType.Temp));
            WardItems.Add(new WardItem(3363, "Farsight Orb", "", 4000, 2, WardType.Temp));
            WardItems.Add(new WardItem(3187, "Hextech Sweeper", "", 800, 5, WardType.TempVision));
            WardItems.Add(new WardItem(3159, "Grez's Spectral Lantern", "", 800, 5, WardType.Temp));
            WardItems.Add(new WardItem(3364, "Oracle's Lens", "", 600, 10, WardType.TempVision));
        }

        public static WardItem GetWardItem()
        {
            return WardItems.FirstOrDefault(x => Items.HasItem(x.Id) && Items.CanUseItem(x.Id));
        }

        public static InventorySlot GetWardSlot()
        {
            foreach (WardItem ward in WardItems)
            {
                if (Items.CanUseItem(ward.Id))
                {
                    return ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == (ItemId)ward.Id);
                }
            }
            return null;
        }

        public class WardItem
        {
            public readonly int Id;
            public int Duration;
            public String Name;
            public int Range;
            public String SpellName;
            public WardType Type;

            public WardItem(int id, string name, string spellName, int range, int duration, WardType type)
            {
                Id = id;
                Name = name;
                SpellName = spellName;
                Range = range;
                Duration = duration;
                Type = type;
            }
        }
    }

    public static class DirectXDrawer
    {
        public static void InternalRender(Vector3 target)
        {
            Drawing.Direct3DDevice.SetTransform(TransformState.World, Matrix.Translation(target));
            Drawing.Direct3DDevice.SetTransform(TransformState.View, Drawing.View);
            Drawing.Direct3DDevice.SetTransform(TransformState.Projection, Drawing.Projection);

            //Drawing.Direct3DDevice.VertexShader = null;
            //Drawing.Direct3DDevice.PixelShader = null;
            //Drawing.Direct3DDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.Lighting, 0);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.ZEnable, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.AntialiasedLineEnable, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.Clipping, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.EnableAdaptiveTessellation, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.MultisampleAntialias, true);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.ShadeMode, ShadeMode.Gouraud);
            //Drawing.Direct3DDevice.SetTexture(0, null);
            //Drawing.Direct3DDevice.SetRenderState(RenderState.CullMode, Cull.None);
        }

        //        VOID WorldToScreen(D3DXVECTOR3* vecWorld, D3DXVECTOR3* vecScreen)
        //{
        //    CView* pView = *(CView**)(0xE3A00C);
        //    PVOID ThisPtr = pView->ThisPtr;

        //    D3DVIEWPORT9 viewPort;
        //    memset(&viewPort, 0, sizeof(viewPort));

        //    typedef VOID (__thiscall* GetViewportFn)(PVOID, D3DVIEWPORT9*);
        //    CallVirtual<GetViewportFn >(ThisPtr, 56)(ThisPtr, &viewPort);

        //    CMatrixData* pMatrixData = *(CMatrixData**)0xE3A00C;

        //    D3DXMATRIX matWorld;
        //    memset(&matWorld, 0, sizeof(matWorld));

        //    D3DXMatrixIdentity(&matWorld);

        //    D3DXMATRIX matProjection = pMatrixData->m_matProjection;
        //    D3DXMATRIX matView = pMatrixData->m_matView;

        //    D3DXVec3Project(vecScreen, vecWorld, &viewPort, &matProjection, &matView, &matWorld);

        //    vecScreen->x = (vecScreen->x - pView->_unknown0x112F4) / (pView->m_ResolutionWidth - pView->_unknown0x112F4) * pView->m_Width;
        //    vecScreen->y = (vecScreen->y - pView->_unknown0x112F8) / (pView->m_ResolutionHeight - pView->_unknown0x112F8) * pView->m_Height;
        //}

        //        #define OFFSET_RENDERER            0x1D3D794 
        //#define OFFSET_D3D9DEVICE        0x1C226A4 

        //class CRenderer 
        //{ 
        //public: 
        //    char _0x0000[40]; 
        //    __int32 m_Width; //0x0028  
        //    __int32 m_Height; //0x002C  
        //    char _0x0030[108]; 
        //    D3DXMATRIX m_View; //0x009C  
        //    D3DXMATRIX m_Projection; //0x00DC 

        //    static LPDIRECT3DDEVICE9 GetDevice( ) 
        //    { 
        //        return *( IDirect3DDevice9** )( OFFSET_D3D9DEVICE ); 
        //    }; 

        //    static CRenderer* GetInstance( void ) 
        //    { 
        //        return *( CRenderer** )( OFFSET_RENDERER ); 
        //    }; 

        //    void WorldToScreen( D3DXVECTOR3* vWorld, D3DXVECTOR3* vScreen ) 
        //    { 
        //        // Create identity matrix for the world 
        //        D3DXMATRIX mWorld; 
        //        memset( &mWorld, 0, sizeof( mWorld ) ); 
        //        D3DXMatrixIdentity( &mWorld ); 

        //        // Get view port 
        //        D3DVIEWPORT9 vp; 
        //        this->GetDevice( )->GetViewport( &vp ); 

        //        // Project 
        //        D3DXVec3Project( vScreen, vWorld, &vp, &this->m_Projection, &this->m_View, &mWorld ); 

        //        vScreen->x = ( vScreen->x ) / ( this->m_Width  ) * this->m_Width; 
        //        vScreen->y = ( vScreen->y ) / ( this->m_Height ) * this->m_Height; 
        //    }; 
        //}; 

        // _effect.SetValue("ProjectionMatrix", Matrix.Translation(position.SwitchYZ()) * Drawing.View* Drawing.Projection);

        public static bool WorldToScreen(Vector3 vIn, ref Vector2 vOut)
        {
            float width = Drawing.Width;
            float height = Drawing.Height;

            Vector3 test = Drawing.Direct3DDevice.Viewport.Project(vIn, Drawing.Projection, Drawing.View,
                SharpDX.Matrix.Identity);

            Console.WriteLine(test);

            Matrix vProjMatrix = SharpDX.Matrix.Identity * Drawing.View * Drawing.Projection;

            float y =
                      vProjMatrix[0, 1] * vIn.X +
                      vProjMatrix[1, 1] * vIn.Y +
                      vProjMatrix[2, 1] * vIn.Z +
             vProjMatrix[3, 1];

            float x =
                      vProjMatrix[0, 0] * vIn.X +
                      vProjMatrix[1, 0] * vIn.Y +
                      vProjMatrix[2, 0] * vIn.Z +
             vProjMatrix[3, 0];

            float w =
                      vProjMatrix[0, 3] * vIn.X +
                      vProjMatrix[1, 3] * vIn.Y +
                      vProjMatrix[2, 3] * vIn.Z +
                      vProjMatrix[3, 3];
            if (w < 0.19)
            {
                return false;
            }

            vOut.Y = (float)((height * 0.5) - (height * 0.5) * y / w);
            vOut.X = (float)((width * 0.5) + (width * 0.5) * x / w);

            //vOut.Y = (float)((test.Y) / (height) * height);
            //vOut.X = (float)((test.X) / (width) * width);

            //vOut.Y = (float)((height * 0.5) - (height * 0.5) * test.Y / w);
            //vOut.X = (float)((width * 0.5) + (width * 0.5) * test.X / w);

            return true;
        }

        //public static bool WorldToScreen(Vector3 vIn, ref Vector2 vOut)
        //{
        //    float width = Drawing.Width;
        //    float height = Drawing.Height;
        //    Matrix view = Drawing.View;
        //    Matrix proj = Drawing.Projection;

        //    Vector4 vIn4 = vIn.ToVector4();

        //    Vector4.Transform(ref vIn4, ref view, out vIn4);
        //    Vector4.Transform(ref vIn4, ref proj, out vIn4);

        //    vOut.X = (vIn.X / vIn4.W + 1) * (width / 2);
        //    vOut.Y = ((vIn.Y * -1 + 350) / vIn4.W + 1) * (height / 2);



        //    return true;
        //}

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            var vertices = new PositionColored[2];
            vertices[0] = new PositionColored(Vector3.Zero, color.ToArgb());
            from = from.SwitchYZ();
            to = to.SwitchYZ();
            vertices[1] = new PositionColored(to - from, color.ToArgb());

            InternalRender(from);

            Drawing.Direct3DDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.Length / 2, vertices);
        }

        public static void DrawLine(Line line, Vector3 from, Vector3 to, ColorBGRA color, Size size = default(Size),
            float[] scale = null, float rotation = 0.0f)
        {
            if (line != null)
            {
                from = from.SwitchYZ();
                to = to.SwitchYZ();
                Matrix nMatrix = (scale != null ? Matrix.Scaling(scale[0], scale[1], 0) : Matrix.Scaling(1)) *
                                 Matrix.RotationZ(rotation) * Matrix.Translation(from);
                Vector3[] vec = { from, to };
                line.DrawTransform(vec, nMatrix, color);
            }
        }

        public static void DrawText(SharpDX.Direct3D9.Font font, String text, Size size, SharpDX.Color color)
        {
            DrawText(font, text, size.Width, size.Height, color);
        }


        //TODO: Too many drawtext for shadowtext, need another method fps issues
        public static void DrawText(SharpDX.Direct3D9.Font font, String text, int posX, int posY, SharpDX.Color color)
        {
            if (font == null || font.IsDisposed)
            {
                throw new SharpDXException("");
            }
            Rectangle rec = font.MeasureText(null, text, FontDrawFlags.Center);
            //font.DrawText(null, text, posX + 1 + rec.X, posY, Color.Black);
            font.DrawText(null, text, posX + 1 + rec.X, posY + 1, SharpDX.Color.Black);
            font.DrawText(null, text, posX + rec.X, posY + 1, SharpDX.Color.Black);
            //font.DrawText(null, text, posX - 1 + rec.X, posY, Color.Black);
            font.DrawText(null, text, posX - 1 + rec.X, posY - 1, SharpDX.Color.Black);
            font.DrawText(null, text, posX + rec.X, posY - 1, SharpDX.Color.Black);
            font.DrawText(null, text, posX + rec.X, posY, color);
        }

        public static void DrawSprite(Sprite sprite, Texture texture, Size size, float[] scale = null,
            float rotation = 0.0f)
        {
            DrawSprite(sprite, texture, size, SharpDX.Color.White, scale, rotation);
        }

        public static void DrawSprite(Sprite sprite, Texture texture, Size size, SharpDX.Color color,
            float[] scale = null,
            float rotation = 0.0f)
        {
            if (sprite != null && !sprite.IsDisposed && texture != null && !texture.IsDisposed)
            {
                Matrix matrix = sprite.Transform;
                Matrix nMatrix = (scale != null ? Matrix.Scaling(scale[0], scale[1], 0) : Matrix.Scaling(1)) *
                                 Matrix.RotationZ(rotation) * Matrix.Translation(size.Width, size.Height, 0);
                sprite.Transform = nMatrix;
                Matrix mT = Drawing.Direct3DDevice.GetTransform(TransformState.World);

                //InternalRender(mT.TranslationVector);
                if (Common.IsOnScreen(new Vector2(size.Width, size.Height)))
                    sprite.Draw(texture, color);
                sprite.Transform = matrix;
            }
        }

        public static void DrawTransformSprite(Sprite sprite, Texture texture, SharpDX.Color color, Size size,
            float[] scale,
            float rotation, Rectangle? spriteResize)
        {
            if (sprite != null && texture != null)
            {
                Matrix matrix = sprite.Transform;
                Matrix nMatrix = Matrix.Scaling(scale[0], scale[1], 0) * Matrix.RotationZ(rotation) *
                                 Matrix.Translation(size.Width, size.Height, 0);
                sprite.Transform = nMatrix;
                sprite.Draw(texture, color);
                sprite.Transform = matrix;
            }
        }

        public static void DrawTransformedSprite(Sprite sprite, Texture texture, Rectangle spriteResize,
            SharpDX.Color color)
        {
            if (sprite != null && texture != null)
            {
                sprite.Draw(texture, color);
            }
        }

        public static void DrawSprite(Sprite sprite, Texture texture, Size size, SharpDX.Color color,
            Rectangle? spriteResize)
        {
            if (sprite != null && texture != null)
            {
                sprite.Draw(texture, color, spriteResize, new Vector3(size.Width, size.Height, 0));
            }
        }

        public static void DrawSprite(Sprite sprite, Texture texture, Size size, SharpDX.Color color)
        {
            if (sprite != null && texture != null)
            {
                DrawSprite(sprite, texture, size, color, null);
            }
        }

        public struct PositionColored
        {
            public static readonly int Stride = Vector3.SizeInBytes + sizeof(int);

            public int Color;
            public Vector3 Position;

            public PositionColored(Vector3 pos, int col)
            {
                Position = pos;
                Color = col;
            }
        }
    }

    public static class MapPositions
    {

        public enum Region
        {
            Unknown,
            TopLeftOuterJungle,
            TopLeftInnerJungle,
            TopRightOuterJungle,
            TopRightInnerJungle,
            BottomLeftOuterJungle,
            BottomLeftInnerJungle,
            BottomRightOuterJungle,
            BottomRightInnerJungle,
            TopOuterRiver,
            TopInnerRiver,
            BottomOuterRiver,
            BottomInnerRiver,
            LeftMidLane,
            CenterMidLane,
            RightMidLane,
            LeftBotLane,
            CenterBotLane,
            RightBotLane,
            LeftTopLane,
            CenterTopLane,
            RightTopLane,

            TopLane,
            BotLane,
            MidLane,
            Lane,

            BlueInnerJungle,
            BlueOuterJungle,
            BlueLeftJungle,
            BlueRightJungle,
            RedInnerJungle,
            RedOuterJungle,
            RedLeftJungle,
            RedRightJungle,
            BlueJungle,
            RedJungle,
            Jungle,

            BottomRiver,
            TopRiver,
            InnerRiver,
            OuterRiver,
            River,

            Base,
            BlueBase,
            RedBase,
        }

        static readonly Dictionary<Region, List<Geometry.Polygon>> _regions = new Dictionary<Region, List<Geometry.Polygon>>();

        static MapPositions() //Positions by BestAkaliAfrica (xTeKillax)
        {
            _regions.Add(Region.TopLeftOuterJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                    new IntPoint(1770, 5001),
                    new IntPoint(2084, 11596),
                    new IntPoint(3421, 9782),
                    new IntPoint(3841, 9305),
                    new IntPoint(4703, 8844),
                    new IntPoint(6345, 7451),
                    new IntPoint(3518, 4587)
                }
            }.ToPolygons());

            _regions.Add(Region.TopLeftInnerJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                    new IntPoint(3274, 5106),
                    new IntPoint(2071, 5398),
                    new IntPoint(2088, 10702),
                    new IntPoint(2878, 10382),
                    new IntPoint(3289, 9293),
                    new IntPoint(5589, 7887)
                }
            }.ToPolygons());

            _regions.Add(Region.TopOuterRiver, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                    new IntPoint(6427, 7629),
                    new IntPoint(4693, 8805),
                    new IntPoint(3427, 9600),
                    new IntPoint(2410, 11629),
                    new IntPoint(3006, 12325),
                    new IntPoint(7340, 8331)
                }
            }.ToPolygons());

            _regions.Add(Region.TopInnerRiver, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(6217, 8077),
                new IntPoint(5287, 8507),
                new IntPoint(4440, 8988),
                new IntPoint(3408, 9699),
                new IntPoint(2667, 11359),
                new IntPoint(3227, 11953),
                new IntPoint(6886, 8668)
            }
            }.ToPolygons());

            _regions.Add(Region.TopRightOuterJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(7417, 8209),
                new IntPoint(5629, 9663),
                new IntPoint(5425, 11054),
                new IntPoint(4078, 11153),
                new IntPoint(3111, 12709),
                new IntPoint(6631, 12986),
                new IntPoint(9777, 12970),
                new IntPoint(10290, 11155)
            }
            }.ToPolygons());

            _regions.Add(Region.TopRightInnerJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(7129, 9365),
                new IntPoint(6319, 10046),
                new IntPoint(5794, 10160),
                new IntPoint(5435, 11144),
                new IntPoint(4507, 11371),
                new IntPoint(3916, 12150),
                new IntPoint(7202, 12168),
                new IntPoint(9002, 12524),
                new IntPoint(9122, 10553),
                new IntPoint(8205, 9990),
                new IntPoint(8021, 9111)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomLeftOuterJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(4485, 3800),
                new IntPoint(7368, 6600),
                new IntPoint(9245, 5131),
                new IntPoint(9247, 3949),
                new IntPoint(10707, 3730),
                new IntPoint(11388, 1980),
                new IntPoint(10492, 1801),
                new IntPoint(4938, 1780)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomLeftInnerJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(5132, 2358),
                new IntPoint(4963, 3448),
                new IntPoint(6850, 5663),
                new IntPoint(7499, 5798),
                new IntPoint(9151, 4810),
                new IntPoint(9254, 4056),
                new IntPoint(10663, 3012),
                new IntPoint(10421, 2489)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomOuterRiver, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(11752, 2728),
                new IntPoint(9485, 3968),
                new IntPoint(9072, 5126),
                new IntPoint(8449, 5828),
                new IntPoint(7462, 6567),
                new IntPoint(8327, 7223),
                new IntPoint(9692, 6463),
                new IntPoint(10907, 5673),
                new IntPoint(12552, 3442)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomInnerRiver, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(11236, 3200),
                new IntPoint(10513, 4361),
                new IntPoint(9961, 3480),
                new IntPoint(9110, 4326),
                new IntPoint(9455, 5250),
                new IntPoint(7947, 6202),
                new IntPoint(8742, 6731),
                new IntPoint(10137, 6099),
                new IntPoint(11429, 5293),
                new IntPoint(12349, 3902)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomRightOuterJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(13014, 4103),
                new IntPoint(12029, 4416),
                new IntPoint(11447, 5317),
                new IntPoint(8192, 7207),
                new IntPoint(11118, 10396),
                new IntPoint(13061, 9911)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomRightInnerJungle, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(12491, 4049),
                new IntPoint(11457, 5246),
                new IntPoint(11553, 5671),
                new IntPoint(10388, 6316),
                new IntPoint(8881, 7164),
                new IntPoint(11362, 9869),
                new IntPoint(12550, 9567),
                new IntPoint(12585, 6884),
                new IntPoint(12956, 6405)
            }
            }.ToPolygons());

            _regions.Add(Region.LeftMidLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(3297, 4261),
                new IntPoint(5930, 6897),
                new IntPoint(6895, 6141),
                new IntPoint(4112, 3575)
            }
            }.ToPolygons());

            _regions.Add(Region.CenterMidLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(5930, 6897),
                new IntPoint(7987, 8832),
                new IntPoint(9112, 7958),
                new IntPoint(6895, 6141)
            }
            }.ToPolygons());

            _regions.Add(Region.RightMidLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(9112, 7958),
                new IntPoint(7987, 8832),
                new IntPoint(10631, 11341),
                new IntPoint(11361, 10869)
            }
            }.ToPolygons());

            _regions.Add(Region.LeftBotLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(4502, 492),
                new IntPoint(4486, 1784),
                new IntPoint(11218, 1953),
                new IntPoint(12183, 485)
            }
            }.ToPolygons());

            _regions.Add(Region.CenterBotLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(12183, 485),
                new IntPoint(11218, 1953),
                new IntPoint(12552, 3442),
                new IntPoint(14283, 2620)
            }
            }.ToPolygons());

            _regions.Add(Region.RightBotLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(14283, 2620),
                new IntPoint(12552, 3442),
                new IntPoint(12997, 3971),
                new IntPoint(13048, 10432),
                new IntPoint(14580, 10329)
            }
            }.ToPolygons());

            _regions.Add(Region.LeftTopLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(23, 4744),
                new IntPoint(104, 12521),
                new IntPoint(1967, 11326),
                new IntPoint(1719, 4564)
            }
            }.ToPolygons());

            _regions.Add(Region.CenterTopLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(104, 12521),
                new IntPoint(3332, 14683),
                new IntPoint(3620, 12813),
                new IntPoint(1967, 11326)
            }
            }.ToPolygons());

            _regions.Add(Region.RightTopLane, new List<List<IntPoint>>
            {
                new List<IntPoint>()
                {
                new IntPoint(3620, 12813),
                new IntPoint(3332, 14683),
                new IntPoint(10295, 14390),
                new IntPoint(10261, 13162),
                new IntPoint(4284, 13087)
            }
            }.ToPolygons());

            _regions.Add(Region.BottomRiver, JoinPolygonLists(_regions[Region.BottomOuterRiver], _regions[Region.BottomInnerRiver]));
            _regions.Add(Region.TopRiver, JoinPolygonLists(_regions[Region.TopOuterRiver], _regions[Region.TopInnerRiver]));
            _regions.Add(Region.InnerRiver, JoinPolygonLists(_regions[Region.BottomInnerRiver], _regions[Region.TopInnerRiver]));
            _regions.Add(Region.OuterRiver, JoinPolygonLists(_regions[Region.BottomOuterRiver], _regions[Region.TopOuterRiver]));
            _regions.Add(Region.River, JoinPolygonLists(_regions[Region.TopRiver], _regions[Region.BottomRiver]));

            _regions.Add(Region.TopLane, JoinPolygonLists(_regions[Region.LeftTopLane], _regions[Region.CenterTopLane], _regions[Region.RightTopLane]));
            _regions.Add(Region.MidLane, JoinPolygonLists(_regions[Region.LeftMidLane], _regions[Region.CenterMidLane], _regions[Region.RightMidLane]));
            _regions.Add(Region.BotLane, JoinPolygonLists(_regions[Region.LeftBotLane], _regions[Region.CenterBotLane], _regions[Region.RightBotLane]));
            _regions.Add(Region.Lane, JoinPolygonLists(_regions[Region.TopLane], _regions[Region.MidLane], _regions[Region.BotLane]));

            _regions.Add(Region.BlueInnerJungle, JoinPolygonLists(_regions[Region.BottomLeftInnerJungle], _regions[Region.BottomRightInnerJungle]));
            _regions.Add(Region.BlueOuterJungle, JoinPolygonLists(_regions[Region.BottomLeftOuterJungle], _regions[Region.BottomRightOuterJungle]));
            _regions.Add(Region.BlueLeftJungle, JoinPolygonLists(_regions[Region.BottomLeftInnerJungle], _regions[Region.BottomLeftOuterJungle]));
            _regions.Add(Region.BlueRightJungle, JoinPolygonLists(_regions[Region.BottomRightInnerJungle], _regions[Region.BottomRightOuterJungle]));
            _regions.Add(Region.RedInnerJungle, JoinPolygonLists(_regions[Region.TopLeftInnerJungle], _regions[Region.TopRightInnerJungle]));
            _regions.Add(Region.RedOuterJungle, JoinPolygonLists(_regions[Region.TopLeftOuterJungle], _regions[Region.TopRightOuterJungle]));
            _regions.Add(Region.RedLeftJungle, JoinPolygonLists(_regions[Region.TopLeftInnerJungle], _regions[Region.TopLeftOuterJungle]));
            _regions.Add(Region.RedRightJungle, JoinPolygonLists(_regions[Region.TopRightInnerJungle], _regions[Region.TopRightOuterJungle]));
            _regions.Add(Region.BlueJungle, JoinPolygonLists(_regions[Region.BlueLeftJungle], _regions[Region.BlueRightJungle],
                                                             _regions[Region.BlueInnerJungle], _regions[Region.BlueOuterJungle]));
            _regions.Add(Region.RedJungle, JoinPolygonLists(_regions[Region.RedLeftJungle], _regions[Region.RedRightJungle],
                                                            _regions[Region.RedInnerJungle], _regions[Region.RedOuterJungle]));
            _regions.Add(Region.Jungle, JoinPolygonLists(_regions[Region.BlueJungle], _regions[Region.RedJungle]));

            _regions.Add(Region.Base, JoinPolygonLists(_regions[Region.Lane], _regions[Region.Jungle], _regions[Region.River]));
            _regions.Add(Region.BlueBase, _regions[Region.Base]);
            _regions.Add(Region.RedBase, _regions[Region.Base]);

            Drawing.OnDraw += Drawing_OnDraw;
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            //DrawDebug();
        }

        private static void DrawDebug()
        {
            Region pos = Region.Unknown;
            foreach (var regionList in _regions)
            {
                foreach (var region in regionList.Value)
                {
                    region.Draw(Color.Aqua);
                    if (region.IsInside(ObjectManager.Player.ServerPosition))
                    {
                        pos = regionList.Key;
                    }
                }
            }
            Drawing.DrawText(Drawing.Width, Drawing.Height - 200, Color.Aqua, pos.ToString());
        }

        private static List<Geometry.Polygon> JoinPolygonLists(params List<Geometry.Polygon>[] lists)
        {
            List<Geometry.Polygon> list = new List<Geometry.Polygon>();
            foreach (var polygonList in lists)
            {
                foreach (var polygon in polygonList)
                {
                    list.Add(polygon);
                }
            }
            return list;
        }

        public static Region GetRegion(Vector2 point)
        {
            foreach (var regionList in _regions)
            {
                foreach (var region in regionList.Value)
                {
                    if (region.IsInside(point))
                    {
                        return regionList.Key;
                    }
                }
            }
            return Region.Unknown;
        }

        public static bool IsInRegion(Vector2 point, Region region)
        {
            foreach (var regionList in _regions)
            {
                foreach (var regionPos in regionList.Value)
                {
                    if (regionPos.IsInside(point))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

    public static class ThreadHelper
    {

        static ThreadEventHelper[] _threadHelpers = new ThreadEventHelper[10];
        static Thread[] _threads = new Thread[10];
        private static bool _cancelThread;
        private static int _lastUsed = 0;

        static ThreadHelper()
        {
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
            for (int i = 0; i < _threads.Length; i++)
            {
                _threads[i] = new Thread(CallEvent);
                _threads[i].Start(i);
            }
        }

        private static void CurrentDomainOnDomainUnload(Object obj, EventArgs args)
        {
            _cancelThread = true;
        }

        private static void CallEvent(object id)
        {
            while (!_cancelThread)
            {
                Thread.Sleep(1);
                if (_threadHelpers[(int)id] != null)
                {
                    _threadHelpers[(int)id].OnCall();
                }
            }
        }

        public static ThreadEventHelper GetInstance()
        {
            if (_threadHelpers[_lastUsed] == null)
            {
                _threadHelpers[_lastUsed] = new ThreadEventHelper();
            }
            return _threadHelpers[_lastUsed];
        }

        public class ThreadEventHelper
        {

            //For later usage maybe
            public class ThreadHelperEventArgs : EventArgs
            {

            }

            public event EventHandler<EventArgs> Called;

            public void OnCall()
            {
                var target = Called;

                if (target != null)
                {
                    target(this, new EventArgs());
                }
            }
        }
    }

    public static class AssemblyResolver
    {
        private static Assembly evadeAssembly;
        private static Assembly jsonAssembly;
        private static Assembly inibinAssembly;

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String assembly = Assembly.GetExecutingAssembly().GetName().Name;
            string name = args.Name.Split(',')[0];
            if (name.ToLower().Contains("evade"))
            {
                if (evadeAssembly == null)
                {
                    evadeAssembly = Load(assembly + ".Resources.DLL.Evade.dll");
                }
                return evadeAssembly;
            }
            else if (name.ToLower().Contains("newtonsoft"))
            {
                if (jsonAssembly == null)
                {
                    jsonAssembly = Load(assembly + ".Resources.DLL.Newtonsoft.Json.dll");
                }
                return jsonAssembly;
            }
            else if (name.ToLower().Contains("gamefiles"))
            {
                if (inibinAssembly == null)
                {
                    inibinAssembly = Load(assembly + ".Resources.DLL.LeagueSharp.GameFiles.dll");
                }
                return inibinAssembly;
            }
            return null;
        }

        private static Assembly Load(String assemblyName)
        {
            byte[] ba = null;
            string resource = assemblyName;
            Assembly curAsm = Assembly.GetExecutingAssembly();
            using (Stream stm = curAsm.GetManifestResourceStream(resource))
            {
                ba = new byte[(int)stm.Length];
                stm.Read(ba, 0, (int)stm.Length);
                return Assembly.Load(ba);
            }
        }

        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
    }

    public static class Website
    {

        public static String GetWebSiteContent(String webSite, String cookie, String param)
        {
            string website = "";
            try
            {
                var baseAddress = new Uri(webSite.Substring(0, webSite.IndexOf("/", 8)));
                using (HttpClientHandler handler = new HttpClientHandler() { UseCookies = false })
                using (HttpClient client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, webSite.Substring(webSite.IndexOf("/", 8)));//"/test");
                                                                                                                     //message.Headers.Add("Cookie", "cookie1=value1; cookie2=value2");
                    message.Headers.Add("Cookie", cookie);
                    using (HttpResponseMessage response = client.SendAsync(message).Result)
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = content.ReadAsStringAsync().Result;

                        // ... Display the result.
                        if (result != null)
                        {
                            website = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot load {0} Data. Exception: " + ex.ToString(), website);
            }

            return website;
        }

        //[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static String GetWebSiteContent(String webSite, List<Cookie> cookies = null, String param = null)
        {
            string website = "";
            var request = (HttpWebRequest)WebRequest.Create(webSite);
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)";
            request.ServicePoint.Expect100Continue = false;
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    if (cookie != null)
                    {
                        TryAddCookie(request, cookie);
                    }
                }
            }
            if (param != null)
            {
                Byte[] bytes = Encoding.ASCII.GetBytes(param);//GetBytes(param);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                //Stream dataStream = request.GetRequestStream();
                //dataStream.Write(bytes, 0, bytes.Length);
                //dataStream.Close();
            }
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream receiveStream = response.GetResponseStream();
                        if (receiveStream != null)
                        {
                            if (response.CharacterSet == null)
                            {
                                using (StreamReader readStream = new StreamReader(receiveStream))
                                {
                                    website = @readStream.ReadToEnd();
                                }
                            }
                            else
                            {
                                using (
                                    StreamReader readStream = new StreamReader(receiveStream,
                                        Encoding.GetEncoding(response.CharacterSet)))
                                {
                                    website = @readStream.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Cannot load {0} Data. Exception: " + ex.ToString(), website);
                throw;
            }
            return website;
        }

        private static bool TryAddCookie(WebRequest webRequest, Cookie cookie)
        {
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return false;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }

            httpRequest.CookieContainer.Add(cookie);
            return true;
        }

        public static String GetMatch(String websiteContent, String pattern, int index = 0, int groupIndex = 1)
        {
            try
            {
                string replacement = Regex.Replace(websiteContent, @"\t|\n|\r", "");
                replacement = Regex.Replace(replacement, @"\\t|\\n|\\r", "");
                replacement = Regex.Replace(replacement, @"\\""", "\"");
                //File.WriteAllText(Config.AppDataDirectory + "\\omg.txt", replacement);
                Match websiteMatcher = new Regex(pattern).Matches(replacement)[index];
                //Match elementMatch = new Regex(websiteMatcher.Groups[groupIndex].ToString()).Matches(replacement)[0];
                //return elementMatch.ToString();
                return websiteMatcher.Groups[groupIndex].ToString();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Cannot get match for pattern {0}", pattern);
            }
            return "";
        }
    }

    public static class Language
    {

        private static System.Resources.ResourceManager resMgr;
        private static System.Resources.ResourceManager resMgrAlt;

        public static void UpdateLanguage(string langID)
        {
            String assembly = Assembly.GetExecutingAssembly().GetName().Name;
            try
            {
                resMgr = new System.Resources.ResourceManager(assembly + ".Resources.TRANSLATIONS.Translation-" + langID, Assembly.GetExecutingAssembly());
                resMgrAlt = new System.Resources.ResourceManager(assembly + ".Resources.TRANSLATIONS.Translation-en-US", Assembly.GetExecutingAssembly());
            }
            catch (Exception)
            {
                try
                {
                    resMgr = new System.Resources.ResourceManager(assembly + ".Resources.TRANSLATIONS.Translation-en-US", Assembly.GetExecutingAssembly());
                    resMgrAlt = resMgr;
                }
                catch (Exception)
                {
                    try
                    {
                        resMgr = new System.Resources.ResourceManager("SAssembliesLib.Resources.TRANSLATIONS.Translation-" + langID, Assembly.GetExecutingAssembly());
                        resMgrAlt = new System.Resources.ResourceManager("SAssembliesLib.Resources.TRANSLATIONS.Translation-en-US", Assembly.GetExecutingAssembly());
                    }
                    catch (Exception)
                    {
                        resMgr = new System.Resources.ResourceManager("SAssembliesLib.Resources.TRANSLATIONS.Translation-en-US", Assembly.GetExecutingAssembly());
                        resMgrAlt = resMgr;
                    }
                }
            }
        }

        public static string GetString(String pattern)
        {
            try
            {
                if (resMgr == null || resMgr.GetString(pattern) == null || resMgr.GetString(pattern).Equals(""))
                {
                    return resMgrAlt.GetString(pattern) ?? "";
                }
                return resMgr.GetString(pattern);
            }
            catch (Exception)
            {
                try
                {
                    return resMgrAlt.GetString(pattern);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        public static void SetLanguage()
        {
            /*
            switch (SandboxConfig.SelectedLanguage)
            {
                case "Arabic":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-SA");
                    break;

                case "Chinese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                    break;

                case "Dutch":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL");
                    break;

                case "English":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    break;

                case "French":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
                    break;

                case "German":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
                    break;

                case "Greek":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("el-GR");
                    break;

                case "Italian":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT");
                    break;

                case "Korean":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ko-KR");
                    break;

                case "Polish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
                    break;

                case "Portuguese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-PT");
                    break;

                case "Romanian":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ro-RO");
                    break;

                case "Russian":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                    break;

                case "Spanish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
                    break;

                case "Swedish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv-SE");
                    break;

                case "Thai":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("th-TH");
                    break;

                case "Turkish":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    break;

                case "Vietnamese":
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
                    break;

                default:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    break;
            }
            UpdateLanguage(Thread.CurrentThread.CurrentUICulture.Name);
        */
        }
    }

    public static class PacketCatcher
    {
        private static List<byte> exclude = new List<byte>() { 75, 160, 91, 166, 115, 190, 88, 114, 92, 79, 15, 44, 193, 198, 176, 95, 163, 78, 72, 242, 22, 207, 85, 156, 131, 157, 149, 90, 74, 247, 118, 233, 93, };
        private static List<byte> list = new List<byte>() { };

        public static void Init()
        {
            Game.OnProcessPacket += delegate (GamePacketEventArgs eventArgs)
            {
                if (!list.Contains(eventArgs.PacketData[0]) && !exclude.Contains(eventArgs.PacketData[0]))
                {
                    list.Add(eventArgs.PacketData[0]);
                    Console.Write("Got Packet: " + eventArgs.PacketData[0] + " (" + eventArgs.PacketData[0].ToString("X") + "); Length: " + eventArgs.PacketData.Length + "; ");
                    Array.ForEach(eventArgs.PacketData, x => Console.Write(x + " "));
                    Console.WriteLine();
                    Array.ForEach(eventArgs.PacketData, x => Console.Write(x.ToString("X") + " "));
                    Console.WriteLine();
                }
            };

            LeagueSharp.Common.Utility.DelayAction.Add(60000, () =>
                {
                    Console.WriteLine("Captured Packets: ");
                    list.ForEach(x => Console.Write(x + ", "));
                    Console.WriteLine();
                });
        }
    }
}