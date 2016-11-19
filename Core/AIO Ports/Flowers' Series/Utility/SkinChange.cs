using EloBuddy; 
using LeagueSharp.Common; 
 namespace Flowers_ADC_Series.Utility
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web.Script.Serialization;

    internal class SkinChange : Program
    {
        private static int SkinID;

        private new static readonly Menu Menu = Utilitymenu;

        private class SkinInfo
        {
            internal string Name;
        }

        static SkinChange()
        {
        }

        public static void Init()
        {
        }

        private static void EnbaleSkin(object obj, OnValueChangeEventArgs Args)
        {
            if (!Args.GetNewValue<bool>())
            {
                Me.SetSkin(Me.ChampionName, SkinID);
            }
        }

        private static void OnUpdate(EventArgs Args)
        {
            if (Me.IsDead)
            {
                return;
            }

            if (Menu.Item("EnableSkin", true).GetValue<bool>())
            {
                Me.SetSkin(Me.ChampionName,
                    Menu.Item("SelectSkin", true).GetValue<StringList>().SelectedIndex);
            }
        }
    }
}
