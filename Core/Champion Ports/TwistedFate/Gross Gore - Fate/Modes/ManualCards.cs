using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    using System.Windows.Input;

    using LeagueSharp;

    internal static class ManualCards
    {
        #region Methods

        internal static void Execute()
        {
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

            if (ObjectManager.Player.Mana >= wMana && CardSelector.Status == SelectStatus.Ready)
            {
                if (Config.IsKeyPressed("csGold")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.StartSelecting(Cards.Yellow);
                }

                if (Config.IsKeyPressed("csBlue")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.StartSelecting(Cards.Blue);
                }

                if (Config.IsKeyPressed("csRed")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.StartSelecting(Cards.Red);
                }
            }

            if(CardSelector.Status == SelectStatus.Selecting)
            {
                if (Config.IsKeyPressed("csGold")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.GoToKey(Cards.Yellow);
                }

                if (Config.IsKeyPressed("csBlue")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.GoToKey(Cards.Blue);
                }

                if (Config.IsKeyPressed("csRed")
                    && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.LeftCtrl)
                        && !Keyboard.IsKeyDown(Key.LeftAlt)))
                {
                    CardSelector.GoToKey(Cards.Red);
                }
            }
        }

        #endregion
    }
}