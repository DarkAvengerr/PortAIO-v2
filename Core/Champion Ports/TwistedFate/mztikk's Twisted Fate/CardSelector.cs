using EloBuddy; 
 using LeagueSharp.Common; 
 namespace mztikksTwistedFate
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    public enum Cards
    {
        Red, 

        Yellow, 

        Blue, 

        None
    }

    public enum SelectStatus
    {
        Ready, 

        Selecting, 

        Selected, 

        Cooldown
    }

    internal static class CardSelector
    {
        #region Constructors and Destructors

        static CardSelector()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnUpdate += OnGameUpdate;

            // GameObject.OnCreate += OnGameObjectCreate;
        }

        #endregion

        #region Properties

        internal static bool CardChanged { get; private set; }

        internal static int LastCardChange { get; private set; }

        internal static int LastSendWSent { get; } = 0;

        internal static int LastWSent { get; set; }

        internal static Cards Select { get; set; }

        internal static bool Starting { get; set; }

        internal static SelectStatus Status { get; set; }

        private static int Delay { get; set; }

        private static string LastCard { get; set; } = string.Empty;

        #endregion

        #region Methods

        internal static void StartSelecting(Cards card)
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name == "PickACard" && Status == SelectStatus.Ready)
            {
                Select = card;
                if (Environment.TickCount - LastWSent > 170 + (Game.Ping / 2))
                {
                    Starting = true;
                    LastCard = string.Empty;
                    CardChanged = false;
                    ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
                    LastWSent = Environment.TickCount;
                    Delay = Config.IsChecked("humanizePicks")
                                ? Mainframe.Rng.Next(
                                    Config.GetSliderValue("humanizeLower"), 
                                    Config.GetSliderValue("humanizeUpper"))
                                : 0;
                }
            }
        }

        private static void OnGameObjectCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsAlly && sender.Name.Contains("TwistedFate_Base_W"))
            {
                if (sender.Name.Contains("RedCard") || sender.Name.Contains("BlueCard")
                    || sender.Name.Contains("GoldCard"))
                {
                }
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            /*var cards =
                ObjectManager.Get<Obj_GeneralParticleEmitter>()
                    .Where(
                        x =>
                        x.Name.Contains("TwistedFate_Base_W") && x.Position.Distance(ObjectManager.Player.Position) < 25)
                    .ToList(); // RedCard GoldCard BlueCard
            var wName = cards.Select(x => x.Name).FirstOrDefault();
            var oldWName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;*/

             var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
            var wState = ObjectManager.Player.Spellbook.CanUseSpell(SpellSlot.W);
            if (wName != null && (wName.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase) // BlueCardLock
                                  || wName.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase)

                                  // GoldCardLock
                                  || wName.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase)))
            {
                // RedCardLock
                if (LastCard == string.Empty)
                {
                    LastCard = wName;
                }

                if (wName != LastCard)
                {
                    LastCard = wName;
                    CardChanged = true;
                    LastCardChange = Environment.TickCount;
                }
            }

            /*if (cards.Any(x => x.Name.Contains("RedCard") || x.Name.Contains("BlueCard") || x.Name.Contains("GoldCard")))
            {
                if (LastCard == string.Empty)
                {
                    LastCard = wName;
                }

                if (wName != LastCard)
                {
                    LastCard = wName;
                    CardChanged = true;
                    LastCardChange = Environment.TickCount;
                }
            }*/
            if ((wState == SpellState.Ready && wName == "PickACard"
                 && (Status != SelectStatus.Selecting || Environment.TickCount - LastWSent > 600))
                || ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Ready;
            }
            else if (wState == SpellState.Cooldown && wName == "PickACard")
            {
                Select = Cards.None;
                Status = SelectStatus.Cooldown;
            }
            else if (wState == SpellState.Surpressed && !ObjectManager.Player.IsDead)
            {
                Status = SelectStatus.Selected;
            }

            if (wName == null)
            {
                return;
            }

            if (Select == Cards.Blue && wName.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase) && Environment.TickCount - Delay > LastWSent
                && Environment.TickCount - Delay > LastCardChange)
            {
                SendWPacket();
            }
            else if (Select == Cards.Yellow && wName.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase) && Environment.TickCount - Delay > LastWSent
                     && Environment.TickCount - Delay > LastCardChange)
            {
                SendWPacket();
            }
            else if (Select == Cards.Red && wName.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase) && Environment.TickCount - Delay > LastWSent
                     && Environment.TickCount - Delay > LastCardChange)
            {
                SendWPacket();
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.SData.Name.Equals("PickACard", StringComparison.InvariantCultureIgnoreCase))
            {
                Status = SelectStatus.Selecting;
            }

            if (args.SData.Name.Equals("GoldCardLock", StringComparison.InvariantCultureIgnoreCase)
                || args.SData.Name.Equals("BlueCardLock", StringComparison.InvariantCultureIgnoreCase)
                || args.SData.Name.Equals("RedCardLock", StringComparison.InvariantCultureIgnoreCase))
            {
                Status = SelectStatus.Selected;
            }
        }

        private static void SendWPacket()
        {
            if (Config.IsChecked("ignoreFirst") && !CardChanged)
            {
                return;
            }

            ObjectManager.Player.Spellbook.CastSpell(SpellSlot.W, false);
        }

        #endregion
    }
}