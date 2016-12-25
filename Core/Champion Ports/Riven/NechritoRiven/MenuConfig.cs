using EloBuddy; 
using LeagueSharp.Common; 
namespace NechritoRiven
{
    #region

    using System.Drawing;

    using LeagueSharp.Common;

    using Color = SharpDX.Color;

    #endregion

    internal class MenuConfig : Core.Core
    {
        #region Constants

        private const string MenuName = "Nechrito Riven";

        #endregion

        #region Static Fields

        private static Menu config;

        #endregion

        #region Public Properties

        public static bool AlwaysF => config.Item("AlwaysF").GetValue<KeyBind>().Active;

        public static bool UseR1 => config.Item("UseR1").GetValue<KeyBind>().Active;

        public static bool LaneQFast => config.Item("laneQFast").GetValue<bool>();

        public static bool AnimDance => config.Item("animDance").GetValue<bool>();

        public static bool AnimLaugh => config.Item("animLaugh").GetValue<bool>();

        public static bool AnimTalk => config.Item("animTalk").GetValue<bool>();

        public static bool AnimTaunt => config.Item("animTaunt").GetValue<bool>();

        public static bool CancelPing => config.Item("CancelPing").GetValue<bool>();

        public static bool Dind => config.Item("Dind").GetValue<bool>();

        public static bool DrawAlwaysR => config.Item("DrawAlwaysR").GetValue<bool>();

        public static bool DrawBt => config.Item("DrawBT").GetValue<bool>();

        public static bool DrawCb => config.Item("DrawCB").GetValue<bool>();

        public static bool DrawFh => config.Item("DrawFH").GetValue<bool>();

        public static bool DrawHs => config.Item("DrawHS").GetValue<bool>();

        public static StringList EmoteList => config.Item("EmoteList").GetValue<StringList>();

        public static bool FleeSpot => config.Item("FleeSpot").GetValue<bool>();

        public static bool FleeYomuu => config.Item("FleeYoumuu").GetValue<bool>();

        public static bool ForceFlash => config.Item("DrawForceFlash").GetValue<bool>();

        public static bool GapcloserMenu => config.Item("GapcloserMenu").GetValue<bool>();

        public static bool Ignite => config.Item("ignite").GetValue<bool>();

        public static bool KsW => config.Item("ksW").GetValue<bool>();

        public static bool KsR2 => config.Item("ksR2").GetValue<bool>();

        public static bool InterruptMenu => config.Item("InterruptMenu").GetValue<bool>();

        public static bool IreliaLogic => config.Item("IreliaLogic").GetValue<bool>();

        public static bool JnglE => config.Item("JungleE").GetValue<bool>();

        public static bool JnglQ => config.Item("JungleQ").GetValue<bool>();

        public static bool JnglW => config.Item("JungleW").GetValue<bool>();

        public static bool KeepQ => config.Item("KeepQ").GetValue<bool>();

        public static bool LaneE => config.Item("LaneE").GetValue<bool>();

        public static bool LaneEnemy => config.Item("LaneEnemy").GetValue<bool>();

        public static bool SafeR1 => config.Item("SafeR1").GetValue<bool>();

        public static bool LaneQ => config.Item("LaneQ").GetValue<bool>();

        public static bool LaneW => config.Item("LaneW").GetValue<bool>();

        public static bool Doublecast => config.Item("Doublecast").GetValue<bool>();

        public static bool Flash => config.Item("FlashOften").GetValue<bool>();

        public static bool OverKillCheck => config.Item("OverKillCheck").GetValue<bool>();

        public static int Q2D => config.Item("Q2D").GetValue<Slider>().Value;

        public static int Qd => config.Item("QD").GetValue<Slider>().Value;

        public static int Qld => config.Item("Q3D").GetValue<Slider>().Value;

        public static bool QMove => config.Item("QMove").GetValue<KeyBind>().Active;

        public static bool QReset => config.Item("qReset").GetValue<bool>();

        public static bool R2Draw => config.Item("R2Draw").GetValue<bool>();

        public static StringList SkinList => config.Item("SkinList").GetValue<StringList>();

        public static bool UseSkin => config.Item("UseSkin").GetValue<bool>();

        public static bool WallFlee => config.Item("WallFlee").GetValue<bool>();

        public static bool Q3Wall => config.Item("Q3Wall").GetValue<bool>();

        public static bool UltHarass => config.Item("UltHarass").GetValue<bool>();

        public static int WallWidth => config.Item("WallWidth").GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public static void LoadMenu()
        {
            config = new Menu(MenuName, MenuName, true).SetFontStyle(FontStyle.Bold, Color.Cyan);

            var orbwalker = new Menu("Orbwalker", "rorb");
            Orbwalker = new Orbwalking.Orbwalker(orbwalker);
            config.AddSubMenu(orbwalker);

            var animation = new Menu("Animations", "Animation");
            animation.AddItem(new MenuItem("QD", "Q1 Ping").SetValue(new Slider(210, 210, 340)));
            animation.AddItem(new MenuItem("Q2D", "Q2 Ping").SetValue(new Slider(210, 210, 340)));
            animation.AddItem(new MenuItem("Q3D", "Q3 Ping").SetValue(new Slider(340, 340, 380)));
            animation.AddItem(new MenuItem("CancelPing", "Include Ping").SetValue(true));
            animation.AddItem(new MenuItem("EmoteList", "Emotes").SetValue(new StringList(new[] { "Laugh", "Taunt", "Joke", "Dance", "None" }, 3)));
            config.AddSubMenu(animation);

            var combo = new Menu("Combo", "Combo");
            combo.AddItem(new MenuItem("Q3Wall", "Walljump").SetValue(true));
            combo.AddItem(new MenuItem("FlashOften", "Flash Burst Frequently").SetValue(false).SetTooltip("Will flash if killable, always."));
            combo.AddItem(new MenuItem("OverKillCheck", "R2 Max Damage").SetValue(true));
            combo.AddItem(new MenuItem("Doublecast", "Doublecast").SetValue(true)).SetTooltip("Fast Combo, less dmg");
            combo.AddItem(new MenuItem("UltHarass", "Use Ult In Harass (Killable only)").SetValue(false));
            combo.AddItem(new MenuItem("UseR1", "Use R").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            combo.AddItem(new MenuItem("AlwaysF", "Use Flash").SetValue(new KeyBind('L', KeyBindType.Toggle)));
            config.AddSubMenu(combo);

            var lane = new Menu("Lane", "Lane");
            lane.AddItem(new MenuItem("LaneEnemy", "Stop If Nearby Enemy").SetValue(true));
            lane.AddItem(new MenuItem("laneQFast", "Fast Clear").SetValue(true));
            lane.AddItem(new MenuItem("LaneQ", "Use Q").SetValue(true));
            lane.AddItem(new MenuItem("LaneW", "Use W").SetValue(true));
            lane.AddItem(new MenuItem("LaneE", "Use E").SetValue(true));
            config.AddSubMenu(lane);

            var jngl = new Menu("Jungle", "Jungle");
            jngl.AddItem(new MenuItem("JungleQ", "Use Q").SetValue(true));
            jngl.AddItem(new MenuItem("JungleW", "Use W").SetValue(true));
            jngl.AddItem(new MenuItem("JungleE", "Use E").SetValue(true));
            config.AddSubMenu(jngl);

            var killsteal = new Menu("Killsteal", "Killsteal");
            killsteal.AddItem(new MenuItem("ignite", "Use Ignite").SetValue(true));
            killsteal.AddItem(new MenuItem("ksW", "Use W").SetValue(true));
            killsteal.AddItem(new MenuItem("ksR2", "Use R2").SetValue(true));
            config.AddSubMenu(killsteal);

            var misc = new Menu("Misc", "Misc");
            misc.AddItem(new MenuItem("GapcloserMenu", "Anti-Gapcloser").SetValue(true));
            misc.AddItem(new MenuItem("InterruptMenu", "Interrupter").SetValue(true));
            misc.AddItem(new MenuItem("KeepQ", "Keep Q Alive").SetValue(true));
            misc.AddItem(new MenuItem("QMove", "Q Move").SetValue(new KeyBind('K', KeyBindType.Press))).SetTooltip("Will Q Move to mouse");
            config.AddSubMenu(misc);

            var draw = new Menu("Draw", "Draw");
            draw.AddItem(new MenuItem("DrawForceFlash", "Flash Status").SetValue(true));
            draw.AddItem(new MenuItem("DrawAlwaysR", "R Status").SetValue(true));
            draw.AddItem(new MenuItem("R2Draw", "R2 Dmg").SetValue(false));
            draw.AddItem(new MenuItem("Dind", "Damage Indicator").SetValue(true));
            draw.AddItem(new MenuItem("FleeSpot", "Draw Flee Spots").SetValue(true));
            draw.AddItem(new MenuItem("DrawCB", "Combo Engage").SetValue(true));
            draw.AddItem(new MenuItem("DrawBT", "BurstMode Engage").SetValue(false));
            draw.AddItem(new MenuItem("DrawFH", "FastHarassMode Engage").SetValue(false));
            draw.AddItem(new MenuItem("DrawHS", "Harass Engage").SetValue(false));
            config.AddSubMenu(draw);

            var flee = new Menu("Flee", "Flee");
            flee.AddItem(new MenuItem("WallFlee", "WallJump in Flee").SetValue(true).SetTooltip("Jumps over walls in flee mode"));
            flee.AddItem(new MenuItem("FleeYoumuu", "Youmuu's Ghostblade").SetValue(true).SetTooltip("Will flee with ghostblade"));
            config.AddSubMenu(flee);

            var skin = new Menu("SkinChanger", "SkinChanger");
            skin.AddItem(new MenuItem("UseSkin", "Use SkinChanger").SetValue(false)).SetTooltip("Toggles Skinchanger");
            skin.AddItem(new MenuItem("SkinList", "Skin").SetValue(new StringList(new[]
                            {
                "Default",
                "Redeemed",
                "Crimson Elite",
                "Battle Bunny",
                "Championship",
                "Dragonblade",
                "Arcade",
                "Championship 2016",
                "Chroma 1",
                "Chroma 2",
                "Chroma 3",
                "Chroma 4",
                "Chroma 5",
                "Chroma 6",
                "Chroma 7",
                "Chroma 8"
                            })));

            config.AddSubMenu(skin);

            config.AddItem(new MenuItem("version", "Version: 6.24.3").SetFontStyle(FontStyle.Bold, Color.Cyan));

            config.AddItem(new MenuItem("paypal", "Paypal: nechrito@live.se").SetFontStyle(FontStyle.Regular, Color.Cyan));

            config.AddToMainMenu();
        }

        #endregion
    }
}