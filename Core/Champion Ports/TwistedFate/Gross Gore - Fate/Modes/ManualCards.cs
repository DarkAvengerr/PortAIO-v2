#region Use
using System.Windows.Input;
using LeagueSharp; 
#endregion

using EloBuddy; 
using LeagueSharp.Common; 
 namespace GrossGoreTwistedFate.Modes
{
    internal static class ManualCards
    {
        #region Methods

        internal static void Execute()
        {
            var wMana = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).SData.Mana;

            switch(CardSelector.Status)
            {
                case SelectStatus.Ready:
                {
                    if(ObjectManager.Player.Mana >= wMana)
                    {
                        if(Config.GoldKey)
                        {
                            CardSelector.StartSelecting(Cards.Yellow);

                        }else if(Config.BlueKey)
                        {
                            CardSelector.StartSelecting(Cards.Blue);

                        }else if(Config.RedKey)
                        {
                            CardSelector.StartSelecting(Cards.Red);
                        }
                    }

                    return;
                }
                case SelectStatus.Selecting:
                {
                    if (Config.GoldKey)
                    {
                        CardSelector.JumpToCard(Cards.Yellow);

                    }
                    else if (Config.BlueKey)
                    {
                        CardSelector.JumpToCard(Cards.Blue);

                    }
                    else if (Config.RedKey)
                    {
                        CardSelector.JumpToCard(Cards.Red);
                    }

                    return;
                }
            }
        }

        #endregion
    }
}