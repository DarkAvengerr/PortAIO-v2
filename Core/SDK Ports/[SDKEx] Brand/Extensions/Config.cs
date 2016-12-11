using EloBuddy; 
using LeagueSharp.SDK; 
namespace Brand.Extensions
{
    using LeagueSharp;
    using LeagueSharp.SDK.UI;

    internal class Config
    {
        public static Menu Menu;

        public static AIHeroClient MyHero { get { return ObjectManager.Player; } }

        public static bool IM_E_Q => Menu["EM"]["IM"]["IM.E.Q"];
        public static bool GM_E_Q => Menu["EM"]["GM"]["GM.E.Q"];
        public static bool IMM_Q => Menu["EM"]["IMM"]["IMM.Q"];
        public static bool IMM_W => Menu["EM"]["IMM"]["IMM.W"];
        public static bool CM_Q => Menu["CM"]["CM.Q"];
        public static bool CM_W => Menu["CM"]["CM.W"];
        public static bool CM_E => Menu["CM"]["CM.E"];
        public static string CM_Q_M => Menu["CM"]["CM.Q.M"].GetValue<MenuList<string>>().SelectedValue;
        public static bool CM_R_B => Menu["CM"]["CM.R"].GetValue<MenuSliderButton>().BValue;
        public static int CM_R_S => Menu["CM"]["CM.R"].GetValue<MenuSliderButton>().SValue;
        public static bool HM_Q => Menu["HM"]["HM.Q"];
        public static bool HM_W => Menu["HM"]["HM.W"];
        public static bool HM_E => Menu["HM"]["HM.E"];
        public static string HM_Q_M => Menu["HM"]["HM.Q.M"].GetValue<MenuList<string>>().SelectedValue;
        public static bool HM_M_SB_B => Menu["HM"]["HM.M.SB"].GetValue<MenuSliderButton>().BValue;
        public static int HM_M_SB_S => Menu["HM"]["HM.M.SB"].GetValue<MenuSliderButton>().SValue;
        public static bool LCM_W => Menu["LCM"]["LCM.W"];
        public static bool LCM_E => Menu["LCM"]["LCM.E"];
        public static int LCM_W_H => Menu["LCM"]["LCM.W.H"].GetValue<MenuSlider>().Value;
        public static bool LCM_M_SB_B => Menu["LCM"]["LCM.M.SB"].GetValue<MenuSliderButton>().BValue;
        public static int LCM_M_SB_S => Menu["LCM"]["LCM.M.SB"].GetValue<MenuSliderButton>().SValue;
        public static bool JCM_Q => Menu["JCM"]["JCM.Q"];
        public static bool JCM_W => Menu["JCM"]["JCM.W"];
        public static bool JCM_E => Menu["JCM"]["JCM.E"];
        public static int JCM_W_H => Menu["JCM"]["JCM.W.H"].GetValue<MenuSlider>().Value;
        public static bool JCM_M_SB_B => Menu["JCM"]["JCM.M.SB"].GetValue<MenuSliderButton>().BValue;
        public static int JCM_M_SB_S => Menu["JCM"]["JCM.M.SB"].GetValue<MenuSliderButton>().SValue;
        public static bool KM_Q => Menu["KM"]["KM.Q"];
        public static bool KM_W => Menu["KM"]["KM.W"];
        public static bool KM_E => Menu["KM"]["KM.E"];
        public static bool KM_R => Menu["KM"]["KM.R"];
        public static bool DM_Q => Menu["EM"]["DM"]["DM.Q"];
        public static bool DM_W => Menu["EM"]["DM"]["DM.W"];
        public static bool DM_E => Menu["EM"]["DM"]["DM.E"];
        public static bool DM_R => Menu["EM"]["DM"]["DM.R"];
        public static bool DM_D => Menu["EM"]["DM"]["DM.D"];
        public static int SM_M => Menu["EM"]["SM"]["SM.M"].GetValue<MenuList<string>>().Index;

        public static void Initialize()
        {
            Menu = new Menu("Brand", "Brand", true);
            {
                var CM = Menu.Add(new Menu("CM", "Combo Settings"));
                {
                    CM.Add(new MenuSeparator("CM.Q.S", "[Q] Settings"));
                    CM.Add(new MenuBool("CM.Q", "Use [Q]", true));
                    CM.Add(new MenuList<string>("CM.Q.M", "[Q] Mode", new[] {"Always", "Only if Ablazed"}));

                    CM.Add(new MenuSeparator("CM.W.S", "[W] Settings"));
                    CM.Add(new MenuBool("CM.W", "Use [W]", true));

                    CM.Add(new MenuSeparator("CM.E.S", "[E] Settings"));
                    CM.Add(new MenuBool("CM.E", "Use [E]", true));

                    CM.Add(new MenuSeparator("CM.R.S", "[R] Settings"));
                    CM.Add(new MenuSliderButton("CM.R", "Min. Enemies To Hit", 1, 1, 5, true));
                }

                var HM = Menu.Add(new Menu("HM", "Mixed Settings"));
                {
                    HM.Add(new MenuSeparator("HM.Q.S", "[Q] Settings"));
                    HM.Add(new MenuBool("HM.Q", "Use [Q]", true));
                    HM.Add(new MenuList<string>("HM.Q.M", "[Q] Mode", new[] { "Always", "Only if Ablazed" }));

                    HM.Add(new MenuSeparator("HM.W.S", "[W] Settings"));
                    HM.Add(new MenuBool("HM.W", "Use [W]", true));

                    HM.Add(new MenuSeparator("HM.E.S", "[E] Settings"));
                    HM.Add(new MenuBool("HM.E", "Use [E]", true));

                    HM.Add(new MenuSeparator("HM.M", "Mana Manager"));
                    HM.Add(new MenuSliderButton("HM.M.SB", "Mana(%)", 45, 0, 100, true));
                    //HM.Add(new MenuSlider("HM.M.S", "Mana(%)", 45, 0, 100));
                }

                var LCM = Menu.Add(new Menu("LCM", "Clear Settings"));
                {
                    LCM.Add(new MenuSeparator("LCM.W.S", "[W] Settings"));
                    LCM.Add(new MenuBool("LCM.W", "Use [W]", true));
                    LCM.Add(new MenuSlider("LCM.W.H", "Min. Minions to Hit", 3, 1, 6));

                    LCM.Add(new MenuSeparator("LCM.E.S", "[E] Settings"));
                    LCM.Add(new MenuBool("LCM.E", "Use [E]", true));

                    LCM.Add(new MenuSeparator("LCM.M", "Mana Manager"));
                    LCM.Add(new MenuSliderButton("LCM.M.SB", "Mana(%)", 45, 0, 100, true));
                }

                var JCM = Menu.Add(new Menu("JCM", "Jungle Settings"));
                {
                    JCM.Add(new MenuSeparator("JCM.Q.S", "[Q] Settings"));
                    JCM.Add(new MenuBool("JCM.Q", "Use [Q]", true));

                    JCM.Add(new MenuSeparator("JCM.W.S", "[W] Settings"));
                    JCM.Add(new MenuBool("JCM.W", "Use [W]", true));

                    JCM.Add(new MenuSeparator("JCM.E.S", "[E] Settings"));
                    JCM.Add(new MenuBool("JCM.E", "Use [E]", true));

                    JCM.Add(new MenuSeparator("JCM.M", "Mana Manager"));
                    JCM.Add(new MenuSliderButton("JCM.M.SB", "Mana(%)", 45, 0, 100, true));
                }

                var KM = Menu.Add(new Menu("KM", "KS Settings"));
                {
                    KM.Add(new MenuSeparator("KM.Q.S", "[Q] Settings"));
                    KM.Add(new MenuBool("KM.Q", "Use [Q]", true));

                    KM.Add(new MenuSeparator("KM.W.S", "[W] Settings"));
                    KM.Add(new MenuBool("KM.W", "Use [W]", true));

                    KM.Add(new MenuSeparator("KM.E.S", "[E] Settings"));
                    KM.Add(new MenuBool("KM.E", "Use [E]", true));

                    KM.Add(new MenuSeparator("KM.R.S", "[R] Settings"));
                    KM.Add(new MenuBool("KM.R", "Use [R]", true));
                }

                var EM = Menu.Add(new Menu("EM", "Extra Settings"));
                {
                    var DM = EM.Add(new Menu("DM", "Draw Settings"));
                    {
                        DM.Add(new MenuSeparator("DM.D.S", "HP Indicator"));
                        DM.Add(new MenuBool("DM.D", "Draw HP Indicator", true));

                        DM.Add(new MenuSeparator("DM.Q.S", "[Q] Settings"));
                        DM.Add(new MenuBool("DM.Q", "[Q] Draw range", true));

                        DM.Add(new MenuSeparator("DM.W.S", "[W] Settings"));
                        DM.Add(new MenuBool("DM.W", "[W] Draw range", true));

                        DM.Add(new MenuSeparator("DM.E.S", "[E] Settings"));
                        DM.Add(new MenuBool("DM.E", "[E] Draw range", true));

                        DM.Add(new MenuSeparator("DM.R.S", "[R] Settings"));
                        DM.Add(new MenuBool("DM.R", "[R] Draw range", true));
                    }

                    var IMM = EM.Add(new Menu("IMM", "Immobile Settings"));
                    {
                        IMM.Add(new MenuSeparator("IMM.Q.S", "[Q] Settings"));
                        IMM.Add(new MenuBool("IMM.Q", "Use [Q] on Immobile target", true));

                        IMM.Add(new MenuSeparator("IMM.W.S", "[W] Settings"));
                        IMM.Add(new MenuBool("IMM.W", "Use [W] on Immobile target", true));
                    }

                    var IM = EM.Add(new Menu("IM", "Interrupter"));
                    {
                        IM.Add(new MenuSeparator("IM.E.Q.S", "[E+Q] Settings"));
                        IM.Add(new MenuBool("IM.E.Q", "Use E+Q", true));
                    }

                    var GM = EM.Add(new Menu("GM", "Anti Gap Closer"));
                    {
                        GM.Add(new MenuSeparator("GM.E.Q.S", "[E+Q] Settings"));
                        GM.Add(new MenuBool("GM.E.Q", "Use E+Q", true));
                    }

                    var SM = EM.Add(new Menu("SM", "Skin Changer"));
                    {
                        SM.Add(new MenuList<string>("SM.M", "Skins", new[] { "Classic", "Apocalyptic", "Vandal", "Cryocore", "Zombie", "Spirit Fire" }));
                    }
                }

                Menu.Attach();
            }
        }
    }
}
