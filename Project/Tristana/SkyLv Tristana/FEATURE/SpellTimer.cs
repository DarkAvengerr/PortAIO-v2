using EloBuddy; 
 using LeagueSharp.Common; 
 namespace SkyLv_Tristana
{
    using LeagueSharp;
    using LeagueSharp.Common;


    internal class SpellTimer
    {
        #region #GET
        private static AIHeroClient Player
        {
            get
            {
                return SkyLv_Tristana.Player;
            }
        }

        private static Spell Q
        {
            get
            {
                return SkyLv_Tristana.Q;
            }
        }

        private static Spell E
        {
            get
            {
                return SkyLv_Tristana.E;
            }
        }
        #endregion

        static SpellTimer()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.IsAutoAttack())
                SkyLv_Tristana.lastAA = Utils.GameTimeTickCount;

            switch (args.SData.Name)
            {
                case "TristanaQ":
                    SkyLv_Tristana.lastQ = Utils.GameTimeTickCount;
                    break;
                case "TristanaW":
                    SkyLv_Tristana.lastW = Utils.GameTimeTickCount;
                    break;
                case "TristanaE":
                    SkyLv_Tristana.lastE = Utils.GameTimeTickCount;
                    break;
                case "TristanaR":
                    SkyLv_Tristana.lastR = Utils.GameTimeTickCount;
                    var target = CustomLib.GetTarget;
                    SkyLv_Tristana.REndPosition = Player.Position.Extend(target.Position, Player.Distance(target) + CustomLib.RPushDistance());
                    break;
            }
        }
    }
}
