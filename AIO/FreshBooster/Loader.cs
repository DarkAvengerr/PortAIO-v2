using System;
using LeagueSharp;
using LeagueSharp.Common;
using static FreshBooster.FreshCommon;
using System.Reflection.Emit;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace FreshBooster
{
    class Loader
    {
        public static void Load()
        {
            _MainMenu = new Menu("FreshBooster (" + ObjectManager.Player.ChampionName + ")", "FreshBooster", true).SetFontStyle(System.Drawing.FontStyle.Regular, SharpDX.Color.Aqua);
            _MainMenu.AddToMainMenu();
            Menu orbwalkerMenu = new Menu("OrbWalker", "OrbWalker");
            _OrbWalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            _MainMenu.AddSubMenu(orbwalkerMenu);
            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            _MainMenu.AddSubMenu(targetSelectorMenu);
            Type t = Type.GetType("FreshBooster.Champion." + ObjectManager.Player.ChampionName);
            if (t != null)
            {
                //Object obj = Activator.CreateInstance(t);
                var target = t.GetConstructor(Type.EmptyTypes);
                var dynamic = new DynamicMethod(string.Empty, t, new Type[0], target.DeclaringType);
                var il = dynamic.GetILGenerator();
                il.DeclareLocal(target.DeclaringType);
                il.Emit(OpCodes.Newobj, target);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);
                var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));
                method();
            }
            else
            {
                Chat.Print("Can't Load FreshBooster. Please send Message to KorFresh, Error Code 99");
            }
        }
    }
}
