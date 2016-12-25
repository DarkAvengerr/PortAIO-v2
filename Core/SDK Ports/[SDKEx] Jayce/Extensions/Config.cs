// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The Config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using EloBuddy; 
using LeagueSharp.SDK; 
namespace Jayce.Extensions
{
    #region

    using LeagueSharp.SDK.UI;

    #endregion

    /// <summary>
    ///     The config.
    /// </summary>
    internal class Config
    {
        #region Static Fields

        /// <summary>
        /// The AGCM.
        /// </summary>
        public static MenuBool AGCM;

        /// <summary>
        /// The cannon EKS.
        /// </summary>
        public static MenuBool CannonEKS;

        /// <summary>
        /// The cannon QE range.
        /// </summary>
        public static MenuBool CannonQERange;

        /// <summary>
        /// The cannon QKS.
        /// </summary>
        public static MenuBool CannonQKS;

        /// <summary>
        /// The cannon Q range.
        /// </summary>
        public static MenuBool CannonQRange;

        /// <summary>
        ///     The combo cannon E.
        /// </summary>
        public static MenuBool ComboCannonE;

        /// <summary>
        ///     The combo cannon Q.
        /// </summary>
        public static MenuBool ComboCannonQ;

        /// <summary>
        ///     The combo cannon W.
        /// </summary>
        public static MenuBool ComboCannonW;

        /// <summary>
        ///     The combo hammer E.
        /// </summary>
        public static MenuBool ComboHammerE;

        /// <summary>
        ///     The combo hammer Q.
        /// </summary>
        public static MenuBool ComboHammerQ;

        /// <summary>
        ///     The combo hammer W.
        /// </summary>
        public static MenuBool ComboHammerW;

        /// <summary>
        ///     The combo R.
        /// </summary>
        public static MenuBool ComboR;

        /// <summary>
        ///     The draw dmg.
        /// </summary>
        public static MenuBool DrawDmg;

        /// <summary>
        /// The hammer EKS.
        /// </summary>
        public static MenuBool HammerEKS;

        /// <summary>
        /// The hammer QKS.
        /// </summary>
        public static MenuBool HammerQKS;

        /// <summary>
        /// The hammer Q range.
        /// </summary>
        public static MenuBool HammerQRange;

        /// <summary>
        ///     The harass cannon E.
        /// </summary>
        public static MenuBool HarassCannonE;

        /// <summary>
        ///     The harass cannon Q.
        /// </summary>
        public static MenuBool HarassCannonQ;

        /// <summary>
        ///     The harass mana.
        /// </summary>
        public static MenuSliderButton HarassMana;

        /// <summary>
        /// The inter m.
        /// </summary>
        public static MenuBool InterM;

        /// <summary>
        ///     The jungle cannon E.
        /// </summary>
        public static MenuBool JungleCannonE;

        /// <summary>
        ///     The jungle cannon Q.
        /// </summary>
        public static MenuBool JungleCannonQ;

        /// <summary>
        ///     The jungle cannon W.
        /// </summary>
        public static MenuBool JungleCannonW;

        /// <summary>
        ///     The jungle hammer E.
        /// </summary>
        public static MenuBool JungleHammerE;

        /// <summary>
        ///     The jungle hammer Q.
        /// </summary>
        public static MenuBool JungleHammerQ;

        /// <summary>
        ///     The jungle hammer W.
        /// </summary>
        public static MenuBool JungleHammerW;

        /// <summary>
        ///     The jungle mana.
        /// </summary>
        public static MenuSliderButton JungleMana;

        /// <summary>
        ///     The jungle R.
        /// </summary>
        public static MenuBool JungleR;

        /// <summary>
        ///     The lane cannon E.
        /// </summary>
        public static MenuBool LaneCannonE;

        /// <summary>
        ///     The lane cannon Q.
        /// </summary>
        public static MenuBool LaneCannonQ;

        /// <summary>
        ///     The lane cannon Q hit.
        /// </summary>
        public static MenuSlider LaneCannonQHit;

        /// <summary>
        ///     The lane hammer Q.
        /// </summary>
        public static MenuBool LaneHammerQ;

        /// <summary>
        ///     The lane hammer Q hit.
        /// </summary>
        public static MenuSlider LaneHammerQHit;

        /// <summary>
        ///     The lane hammer W.
        /// </summary>
        public static MenuBool LaneHammerW;

        /// <summary>
        ///     The lane hammer W hit.
        /// </summary>
        public static MenuSlider LaneHammerWHit;

        /// <summary>
        ///     The lane mana.
        /// </summary>
        public static MenuSliderButton LaneMana;

        /// <summary>
        ///     The menu.
        /// </summary>
        public static Menu Menu;

        /// <summary>
        /// The skin changer m.
        /// </summary>
        public static MenuList<string> SkinChangerM;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The Initialize
        /// </summary>
        public static void Initialize()
        {
            Menu = new Menu("Jayce", "Jayce", true);

            var ComboMenu = Menu.Add(new Menu("ComboMenu", "Combo"));
            {
                var ComboCannon = ComboMenu.Add(new Menu("ComboCannon", "Cannon"));
                {
                    ComboCannonQ = ComboCannon.Add(new MenuBool("Q", "Use Q", true));
                    ComboCannonW = ComboCannon.Add(new MenuBool("W", "Use W", true));
                    ComboCannonE = ComboCannon.Add(new MenuBool("E", "Use E", true));
                }

                var ComboHammer = ComboMenu.Add(new Menu("ComboHammer", "Hammer"));
                {
                    ComboHammerQ = ComboHammer.Add(new MenuBool("Q", "Use Q", true));
                    ComboHammerW = ComboHammer.Add(new MenuBool("W", "Use W", true));
                    ComboHammerE = ComboHammer.Add(new MenuBool("E", "Use E", true));
                }

                ComboR = ComboMenu.Add(new MenuBool("R", "Use R", true));
            }

            var HarassMenu = Menu.Add(new Menu("HarassMenu", "Harass"));
            {
                var HarassCannon = HarassMenu.Add(new Menu("ComboCannon", "Cannon"));
                {
                    HarassCannonQ = HarassCannon.Add(new MenuBool("Q", "Use Q", true));
                    HarassCannonE = HarassCannon.Add(new MenuBool("E", "Use E", true));
                }

                HarassMana = HarassMenu.Add(new MenuSliderButton("Mana", "Mana(%)", 45, 0, 100, true));
            }

            var ClearMenu = Menu.Add(new Menu("ClearMenu", "Clear"));
            {
                var LaneMenu = ClearMenu.Add(new Menu("Lane", "Lane"));
                {
                    var LaneCannon = LaneMenu.Add(new Menu("LaneCannon", "Cannon"));
                    {
                        LaneCannonQ = LaneCannon.Add(new MenuBool("Q", "Use Q", true));
                        LaneCannonQHit = LaneCannon.Add(new MenuSlider("HitQ1", "Min. Minions to Hit", 3, 1, 6));
                        LaneCannonE = LaneCannon.Add(new MenuBool("E", "Use E", true));
                    }

                    var LaneHammer = LaneMenu.Add(new Menu("LaneHammer", "Hammer"));
                    {
                        LaneHammerW = LaneHammer.Add(new MenuBool("W", "Use W", true));
                        LaneHammerWHit = LaneHammer.Add(new MenuSlider("HitW3", "Min. Minions to Hit", 3, 1, 6));
                    }

                    LaneMana = LaneMenu.Add(new MenuSliderButton("Mana", "Mana(%)", 45, 0, 100, true));
                }

                var JungleMenu = ClearMenu.Add(new Menu("Jungle", "Jungle"));
                {
                    var JungleCannon = JungleMenu.Add(new Menu("JungleCannon", "Cannon"));
                    {
                        JungleCannonQ = JungleCannon.Add(new MenuBool("Q", "Use Q", true));
                        JungleCannonW = JungleCannon.Add(new MenuBool("W", "Use W", true));
                    }

                    var JungleHammer = JungleMenu.Add(new Menu("JungleHammer", "Hammer"));
                    {
                        JungleHammerQ = JungleHammer.Add(new MenuBool("Q", "Use Q", true));
                        JungleHammerW = JungleHammer.Add(new MenuBool("W", "Use W", true));
                        JungleHammerE = JungleHammer.Add(new MenuBool("E", "Use E", true));
                    }

                    JungleR = JungleMenu.Add(new MenuBool("R", "Use R", true));

                    JungleMana = JungleMenu.Add(new MenuSliderButton("Mana", "Mana(%)", 45, 0, 100, true));
                }
            }

            var DrawMenu = Menu.Add(new Menu("DrawMenu", "Drawings"));
            {
                DrawDmg = DrawMenu.Add(new MenuBool("DMG", "Draw Damage Indicator", true));
                CannonQRange = DrawMenu.Add(new MenuBool("Q", "Draw Cannon Q Range", true));
                CannonQERange = DrawMenu.Add(new MenuBool("QE", "Draw Cannon QE Range", true));
                HammerQRange = DrawMenu.Add(new MenuBool("HQ", "Draw Hammer Q Range", true));
            }

            var KSMenu = Menu.Add(new Menu("KSMenu", "Kill Steal"));
            {
                var KSCannon = KSMenu.Add(new Menu("Cannon", "Cannon"));
                {
                    CannonQKS = KSCannon.Add(new MenuBool("Q", "Use Q", true));
                    CannonEKS = KSCannon.Add(new MenuBool("E", "Use E", true));
                }

                var KSHammer = KSMenu.Add(new Menu("Hammer", "Hammer"));
                {
                    HammerQKS = KSHammer.Add(new MenuBool("Q", "Use Q", true));
                    HammerEKS = KSHammer.Add(new MenuBool("E", "Use E", true));
                }
            }

            var Inter = Menu.Add(new Menu("Inter", "Interrupter"));
            {
                InterM = Inter.Add(new MenuBool("E", "Use Hammer E", true));
            }

            var AGC = Menu.Add(new Menu("AGC", "Anti Gap Closer"));
            {
                AGCM = AGC.Add(new MenuBool("E", "Use Hammer E", true));
            }

            var SM = Menu.Add(new Menu("SM", "Skin Changer"));
            {
                SkinChangerM =
                    SM.Add(
                        new MenuList<string>(
                            "SM.M",
                            "Skins",
                            new[] { "Classic", "Full Metal", "Debonair", "Forsaken", "BrightHammer" }));
            }

            Menu.Attach();
        }

        #endregion
    }
}