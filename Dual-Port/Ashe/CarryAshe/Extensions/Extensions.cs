using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace CarryAshe
{
    internal static class Extensions
    {
        private static string Namespace = null;
        public static String GetNamespace(this Object o)
        {
            return o.GetType().Namespace;
        }

        public static String GetTypeName(this Object o)
        {
            return o.GetType().Name;
        }

        public static String GetCurrentFunctionName()
        {
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }

        public static MenuItem GetItemEndKey(this Menu menu, string key,[CallerMemberNameAttribute] string parentKey = null){
            if (Namespace == null)
            {
                MethodBase current = System.Reflection.MethodBase.GetCurrentMethod();
                Namespace = current.DeclaringType.Namespace;
            }
            try
            {
                return menu.Item(String.Format("{0}.{1}.{2}", Namespace, parentKey, key));
            }
            catch
            {
                return null;
            }
        }

        public static MenuItem AddItem<T>(this Menu m,string key,string displayName,T value){
            var me = new MenuItem(String.Format("{0}.{1}", m.Name, key), displayName).SetValue(value);
            m.AddItem(me);
            return me;
        }


        public static string ToTitleCase(this string value)
        {
            string[] spacedWords
                = ((IEnumerable<char>)value)
                .Select(c => c == char.ToUpper(c)
                    ? " " + c.ToString()
                    : c.ToString()).ToArray();

            return (String.Join("", spacedWords)).Trim();
        }

        public static string ToCamelCase(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static bool IsCC(this AIHeroClient o)
        {
            return o.IsStunned || o.IsRooted || o.IsCharmed || o.IsPacified;
        }

        public static HitChance GetHitchance(this MenuItem m)
        {
            try
            {
                switch (m.GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        return HitChance.Low;
                    case 1:
                        return HitChance.Medium;
                    case 2:
                        return HitChance.High;
                    case 3:
                        return HitChance.VeryHigh;
                    default:
                        return HitChance.Medium;
                }
            }
            catch
            {
                return HitChance.Immobile;
            }
        }


 


    }
}
