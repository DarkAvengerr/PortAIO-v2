#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate
{
    /// <summary>
    /// Kortatu's W Helper
    /// </summary>

    public enum Cards
    {
        Red,
        Yellow,
        Blue,
        None,
        First,
    }

    public enum SelectStatus
    {
        Ready,
        Selecting,
        Selected,
        Cooldown,
    }


    public static class CardSelector
    {
        public static Cards Select;
        public static int LastWSent = 0;
        public static int LastSendWSent = 0;


        static CardSelector()
        {
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        public static SelectStatus Status { get; set; }

        private static void SendWPacket()
        {
            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
        }

        public static void StartSelecting(Cards card)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && Status == SelectStatus.Ready)
            {
                Select = card;
                if (Utils.TickCount - LastWSent > 170 + Game.Ping / 2)
                {
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, ObjectManager.Player);
                    LastWSent = Utils.TickCount;
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
            var wState = ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.W);

            if ((wState == SpellState.Ready &&
                 wName == "PickACard" &&
                 (Status != SelectStatus.Selecting || Utils.TickCount - LastWSent > 500)) ||
                ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Ready;
            }
            else
            if (wState == SpellState.Cooldown &&
                wName == "PickACard")
            {
                Select = Cards.None;
                Status = SelectStatus.Cooldown;
            }
            else
            if (wState == SpellState.Surpressed &&
                !ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Selected;
            }

            if (Select == Cards.Blue && wName.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase))
            {
                SendWPacket();
            }
            else
            if (Select == Cards.Yellow && wName.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase))
            {
                SendWPacket();
            }
            else
            if (Select == Cards.Red && wName.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase))
            {
                SendWPacket();
            }
            else
            if (Select == Cards.First)
            {
                SendWPacket();
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.Equals("PickACard", StringComparison.InvariantCultureIgnoreCase))
            {
                Status = SelectStatus.Selecting;
            }

            if (args.SData.Name.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase) || args.SData.Name.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase) || args.SData.Name.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase))
            {
                Status = SelectStatus.Selected;
            }
        }
    }
}
