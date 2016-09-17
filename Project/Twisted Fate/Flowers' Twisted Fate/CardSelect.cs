using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FlowersTwistedFate
{
    //  This  is Esk0r CardSelect ~  GitHub:Github.com/Esk0r/LeagueSharp/
    public enum Cards
    {
        Red,
        Yellow,
        Blue,
        None,
    }

    public enum SelectStatus
    {
        Ready,
        Selecting,
        Selected,
        Cooldown,
    }

    public static class CardSelect
    {
        public static Cards Select;
        public static int LastWSent = 0;
        public static int LastSendWSent = 0;
        public static AIHeroClient Player = ObjectManager.Player;
        public static string LastCard = "";
        public static SelectStatus Status
        {
            get;
            set;
        }
        static CardSelect()
        {
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void SendWPacket()
        {
            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
        }

        public static void StartSelecting(Cards card)
        {
            if (!ObjectManager.Player.HasBuff("pickacard_tracker") && Status == SelectStatus.Ready)
            {
                Select = card;
                if (Utils.TickCount - LastWSent > 170 + Game.Ping / 2)
                {
                    Program.W.Cast();
                    LastWSent = Utils.TickCount;
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var wName = ObjectManager.Get<Obj_GeneralParticleEmitter>().Where(x => x.IsValid && x.Name.Contains("TwistedFate_Base_W")).Select(x => x.Name).FirstOrDefault();
            var wState = Player.Spellbook.CanUseSpell(SpellSlot.W);

            if (wName != null && ObjectManager.Player.HasBuff("pickacard_tracker"))
            {
                if (Select == Cards.Blue && wName.Equals("TwistedFate_Base_W_BlueCard.troy", StringComparison.InvariantCultureIgnoreCase))
                {
                    SendWPacket();
                }
                else if (Select == Cards.Yellow && wName.Equals("TwistedFate_Base_W_GoldCard.troy", StringComparison.InvariantCultureIgnoreCase))
                {
                    SendWPacket();
                }
                else if (Select == Cards.Red && wName.Equals("TwistedFate_Base_W_RedCard.troy", StringComparison.InvariantCultureIgnoreCase))
                {
                    SendWPacket();
                }
            }
            else if (wName == null)
            {
                if (wState == SpellState.Ready)
                    Status = SelectStatus.Ready;
                else if ((wState == SpellState.Cooldown || wState == SpellState.Disabled || wState == SpellState.NoMana || wState == SpellState.NotLearned || wState == SpellState.Surpressed || wState == SpellState.Unknown) && !IsSelect)
                    Status = SelectStatus.Cooldown;
                else if (IsSelect)
                    Status = SelectStatus.Selected;
            }
        }

        private static bool IsSelect => ObjectManager.Player.HasBuff("GoldCardPreAttack") || ObjectManager.Player.HasBuff("BlueCardPreAttack") || ObjectManager.Player.HasBuff("RedCardPreAttack");
    }
}
