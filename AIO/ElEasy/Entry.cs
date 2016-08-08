namespace ElEasy
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.Common;
    using EloBuddy;

    internal class Entry
    {
        #region Delegates

        internal delegate T ObjectActivator<out T>(params object[] args);

        #endregion

        #region Public Properties

        public static Menu Menu { get; set; }

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        public static AIHeroClient Player => ObjectManager.Player;

        /// <summary>
        ///     Gets script version
        /// </summary>
        /// <value>
        ///     The script version
        /// </value>
        public static string ScriptVersion => typeof(Entry).Assembly.GetName().Version.ToString();

        #endregion

        #region Public Methods and Operators

        //[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
        {
            var paramsInfo = ctor.GetParameters();
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[paramsInfo.Length];

            for (var i = 0; i < paramsInfo.Length; i++)
            {
                var paramCastExp = Expression.Convert(
                    Expression.ArrayIndex(param, Expression.Constant(i)),
                    paramsInfo[i].ParameterType);

                argsExp[i] = paramCastExp;
            }

            return
                (ObjectActivator<T>)
                Expression.Lambda(typeof(ObjectActivator<T>), Expression.New(ctor, argsExp), param).Compile();
        }

        public static void OnLoad()
        {
            try
            {
                var plugins =
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(x => typeof(IPlugin).IsAssignableFrom(x) && !x.IsInterface)
                        .Select(x => GetActivator<IPlugin>(x.GetConstructors().First())(null));

                var menu = new Menu("ElEasy", "ElEasy", true);
                foreach (var plugin in plugins)
                {
                    if (plugin.ToString().ToLower().Contains(Player.ChampionName.ToLower())) //this is some real broscience
                    {
                        plugin.CreateMenu(menu);
                        plugin.Load();
                    }
                }

                menu.AddItem(new MenuItem("Versionnumber", $"Version: {ScriptVersion}"));
                menu.AddItem(new MenuItem("by.jQuery", "Created by jQuery"));
                menu.AddToMainMenu();

                Menu = menu;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion
    }
}