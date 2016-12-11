using System;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events
{
    internal class CreateManager : Logic
    {
        internal static void Init(GameObject sender, EventArgs Args)
        {
            if (!sender.Name.Contains("Katarina"))
            {
                return;
            }
            //Katarina_Base_Q_Darrger_Land_Dirt.troy
            //Katarina_Base_W_indicator_Ally.troy
            //Katarina_Base_E_Beam.troy
            //Katarina_Base_Dagger_PickUp_Cas.troy

            if (sender.Name.Contains("Katarina_Base_Q_Darrger_Land_Dirt"))
            {
                Daggers.Add(new Spells.DaggerManager
                {
                    Dagger = sender,
                    Position = sender.Position
                });
            }

            if (sender.Name.Contains("Katarina_Base_W_indicator_Ally"))
            {
                Daggers.Add(new Spells.DaggerManager
                {
                    Dagger = sender,
                    Position = sender.Position
                });
            }

            if (sender.Name.Contains("Katarina_Base_E_Beam"))
            {
                Daggers.Add(new Spells.DaggerManager
                {
                    Dagger = sender,
                    Position = sender.Position
                });
            }

            if (sender.Name.Contains("Katarina_Base_Dagger_PickUp_Cas"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }
        }
    }
}