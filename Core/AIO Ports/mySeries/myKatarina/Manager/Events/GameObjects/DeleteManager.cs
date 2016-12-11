using System;
using LeagueSharp;

using EloBuddy; 
using LeagueSharp.Common; 
namespace myKatarina.Manager.Events
{
    internal class DeleteManager : Logic
    {
        internal static void Init(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("Katarina"))
            {
                return;
            }

            if (sender.Name.Contains("Katarina_Base_Q_Darrger_Land_Dirt"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }

            if (sender.Name.Contains("Katarina_Base_W_indicator_Ally"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }

            if (sender.Name.Contains("Katarina_Base_E_Beam"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }

            if (sender.Name.Contains("Katarina_Base_Dagger_PickUp_Cas"))
            {
                Daggers.RemoveAll(x => x.Dagger.NetworkId == sender.NetworkId);
            }
        }
    }
}