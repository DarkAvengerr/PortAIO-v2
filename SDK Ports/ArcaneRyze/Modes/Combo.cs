#region

using LeagueSharp.SDK;
using static Arcane_Ryze.Core;

#endregion

using EloBuddy; 
 using LeagueSharp.SDK; 
 namespace Arcane_Ryze.Modes
{
    internal class Combo
    {
        private static AIHeroClient Player = ObjectManager.Player;
        public static void ComboLogic()
        {
            if (Target == null || !Target.IsValidTarget() || Target.IsInvulnerable) return;

            if(Spells.W.IsReady() && Target.Distance(Player.Position) <= Spells.W.Range && !Target.IsFacing(Player))
            {
                Spells.W.Cast(Target);
            }
            if (Player.Level < 6)
            {
                if (PassiveStack == 0 || PassiveStack == 1)
                {
                    if(Spells.Q.IsReady())
                        Spells.Q.Cast(Target);
                }
                if (PassiveStack < 4)
                {
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                    else if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                    else if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                }
                if (PassiveStack == 4)
                {
                    if (Spells.R.IsReady() && Spells.Q.IsReady())
                    {
                        Spells.R.Cast();
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                    else if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                    else if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                }
            }

            if (Player.Level >= 6)
            {

                if (PassiveStack < 4)
                {
                    if (Spells.R.IsReady() && Spells.Q.IsReady())
                    {
                        Spells.R.Cast();
                    }
                    if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                    if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                    else if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                }

                if (PassiveStack >= 4)
                {
                    if (Spells.R.IsReady() && Spells.Q.IsReady())
                    {
                        Spells.R.Cast();
                    }
                    if (Spells.W.IsReady())
                    {
                        Spells.W.Cast(Target);
                    }
                    else if (Spells.Q.IsReady())
                    {
                        Spells.Q.Cast(Target);
                    }
                    else if (Spells.E.IsReady())
                    {
                        Spells.E.Cast(Target);
                    }
                }
            }
        }
  }
}       
