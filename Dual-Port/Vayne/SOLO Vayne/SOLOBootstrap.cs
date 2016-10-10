using LeagueSharp;
using SoloVayne.External;
using SoloVayne.Skills.General;
using SoloVayne.Utility;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SoloVayne
{
    class SOLOBootstrap
    {
        /// <summary>
        /// The menu generator module
        /// </summary>
        public MenuGenerator MenuGenerator;

        /// <summary>
        /// The assembly module
        /// </summary>
        public SOLOVayne SOLOVayne;

        /// <summary>
        /// The antigapcloser module
        /// </summary>
        public SOLOAntigapcloser Antigapcloser;

        /// <summary>
        /// The translator module
        /// </summary>
        public LanguageAdaptor Translator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SOLOBootstrap"/> class.
        /// </summary>
        public SOLOBootstrap()
        {
            if (ObjectManager.Player.ChampionName != "Vayne")
            {
                return;
            }

            if (Variables.Instance != null)
            {
                return;
            }

            SOLOVayne = new SOLOVayne();
            MenuGenerator = new MenuGenerator();
            Antigapcloser = new SOLOAntigapcloser();
            MenuGenerator.GenerateMenu();
            Translator = new LanguageAdaptor();

            PrintLoaded();
        }

        /// <summary>
        /// Prints the "SOLO Vayne Loaded" string in chat
        /// </summary>
        public void PrintLoaded()
        {
            Chat.Print("<b>[<font color='#009aff'>SOLO</font>] <font color='#009aff'>S</font>mart <font color='#009aff'>O</font>ptimized <font color='#009aff'>L</font>ightweight<font color='#009aff'> O</font>riginal <font color='#009aff'>Vayne</font></b> loaded!");
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public SOLOBootstrap GetInstance()
        {
            return Variables.Instance ?? (Variables.Instance = new SOLOBootstrap());
        }
    }
}
