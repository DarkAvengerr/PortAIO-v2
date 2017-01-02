using System;

using LeagueSharp;
using LeagueSharp.Common;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace Mundo_Sharpy
{
    /// <summary>
    /// 메인메뉴를 간편하게 조작할 수 있는 클래스입니다.
    /// </summary>
    class MenuProvider
    {
        /// <summary>
        /// 메인메뉴의 LeagueSharp.Common.Menu형식 객체입니다. 이 객체에 직접 접근하면 LeagueSharp.Common.Menu클래스의 메소드를 이용해 메인메뉴를 수정 할 수 있습니다.
        /// </summary>
        internal static Menu MenuInstance;
        internal static Menu ChampionMenuInstance { get { return MenuInstance.SubMenu("Champion"); } }
        internal static Orbwalking.Orbwalker Orbwalker;

        /// <summary>
        /// 메인메뉴를 만들고 root메뉴에 추가합니다.
        /// </summary>
        internal static void initialize()
        {
            if (PluginLoader.CanLoadPlugin(ObjectManager.Player.ChampionName))
            {
                MenuInstance = new Menu("Mundo Sharpy", "Mundo_SharpyCommon", true);

                addSubMenu("Champion", ObjectManager.Player.ChampionName);

                Champion.addOrbwalker();
                Champion.addTargetSelector();
            }
            else
            {
                MenuInstance = new Menu("Mundo Sharpy: " + "(X)", "Mundo_SharpyCommon", true);

                addItem("txt1", "Sorry. " + ObjectManager.Player.ChampionName + " is not supported.", null);
            }

            MenuInstance.AddToMainMenu();

            Console.WriteLine("Mundo Sharpy: MenuProvider Initialized.");
        }

        /// <summary>
        /// 메인메뉴에 서브메뉴를 추가합니다.
        /// </summary>
        /// <param name="DisplayName">추가할 서브메뉴의 표기이름을 기입하십시오.</param>
        internal static void addSubMenu(string DisplayName)
        {
            MenuInstance.AddSubMenu(new Menu(DisplayName, DisplayName));
        }

        /// <summary>
        /// 메인메뉴에 서브메뉴를 추가합니다.
        /// </summary>
        /// <param name="Name">추가할 서브메뉴의 이름을 기입합니다.</param>
        /// <param name="DisplayName">추가할 서브메뉴의 표기이름을 기입하십시오.</param>
        internal static void addSubMenu(string Name, string DisplayName)
        {
            MenuInstance.AddSubMenu(new Menu(DisplayName, Name));
        }

        /// <summary>
        /// 메인메뉴에 항목을 추가합니다.
        /// </summary>
        /// <param name="DisplayName">추가할 항목의 표기이름을 기입하십시오.</param>
        /// <param name="Value">항목의 값을 기입하십시오.</param>
        /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 항목의 이름에 챔피언이름을 포함한 항목이 생성됩니다.</param>
        internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = false)
        {
            if (Value == null)
            {
                MenuInstance.AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq));
                return;
            }

            MenuInstance.AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq)).SetValue(Value);
        }

        /// <summary>
        /// 메인메뉴에 항목을 추가합니다.
        /// </summary>
        /// <param name="Name">추가할 항목의 이름을 기입하십시오.</param>
        /// <param name="DisplayName">추가할 항목의 표기이름을 기입하십시오.</param>
        /// <param name="Value">항목의 값을 기입하십시오.</param>
        /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 항목의 이름에 챔피언이름을 포함한 항목이 생성됩니다.</param>
        internal static void addItem(string Name, string DisplayName, object Value = null, bool ChampUniq = false)
        {
            if (Value == null)
            {
                MenuInstance.AddItem(new MenuItem(Name, DisplayName, ChampUniq));
                return;
            }

            MenuInstance.AddItem(new MenuItem(Name, DisplayName, ChampUniq)).SetValue(Value);
        }

        /// <summary>
        /// 이 클래스를 이용해서 이 Activator 메뉴에 접근할 수 있습니다.
        /// </summary>
        internal class Activator
        {
            /// <summary>
            /// SupportedChampions 메뉴에 항목을 추가합니다.
            /// </summary>
            /// <param name="DisplayName">추가할 항목의 표기이름을 기입하십시오.</param>
            /// <param name="Value">항목의 값을 기입하십시오.</param>
            /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 항목의 이름에 챔피언이름을 포함한 항목이 생성됩니다.</param>
            internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
            {
                if (Value == null)
                {
                    MenuInstance.SubMenu("Activator").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq));
                    return;
                }

                MenuInstance.SubMenu("Activator").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq)).SetValue(Value);
            }
        }

        /// <summary>
        /// 이 클래스를 이용해서 이 SupportedChampions 메뉴에 접근할 수 있습니다.
        /// </summary>
        internal class SupportedChampions
        {
            /// <summary>
            /// SupportedChampions 메뉴에 항목을 추가합니다.
            /// </summary>
            /// <param name="DisplayName">추가할 항목의 표기이름을 기입하십시오.</param>
            /// <param name="Value">항목의 값을 기입하십시오.</param>
            /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 항목의 이름에 챔피언이름을 포함한 항목이 생성됩니다.</param>
            internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
            {
                if (Value == null)
                {
                    MenuInstance.SubMenu("Supported champions").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq));
                    return;
                }

                MenuInstance.SubMenu("Supported champions").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq)).SetValue(Value);
            }
        }

        /// <summary>
        /// 이 클래스를 이용해서 챔피언 메뉴에 접근할 수 있습니다.
        /// </summary>
        internal class Champion
        {
            /// <summary>
            /// 챔피언 메뉴에 항목을 추가합니다.
            /// </summary>
            /// <param name="DisplayName">항목의 표기이름을 기입하십시오. 이 함수는 항목의 Name도 DisplayName매개변수의 값으로 설정합니다.</param>
            /// <param name="Value">항목의 초기값을 기입하십시오.</param>
            /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
            internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
            {
                if (Value == null)
                {
                    MenuInstance.SubMenu("Champion").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq));
                    return;
                }

                MenuInstance.SubMenu("Champion").AddItem(new MenuItem(DisplayName, DisplayName, ChampUniq)).SetValue(Value);
            }

            /// <summary>
            /// 챔피언 메뉴에 오브워커를 추가합니다.
            /// </summary>
            internal static void addOrbwalker()
            {
                Orbwalker = new Orbwalking.Orbwalker(MenuInstance.SubMenu("Champion").AddSubMenu(new Menu("Orbwalker", "Orbwalker")));
            }

            /// <summary>
            /// 챔피언 메뉴에 타겟셀렉터를 추가합니다.
            /// </summary>
            internal static void addTargetSelector()
            {
                TargetSelector.AddToMenu(MenuInstance.SubMenu("Champion").AddSubMenu(new Menu("Target Selector", "Target Selector")));
            }

            /// <summary>
            /// 이 클래스를 이용해서 Combo 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Combo
            {
                /// <summary>
                /// Combo 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Combo").AddItem(new MenuItem("Combo." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Combo").AddItem(new MenuItem("Combo." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Combo 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                /// <returns>항목의 bool값을 반환합니다.</returns>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Combo." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Combo 메뉴 안에 존재하는 항목의 LeageuSharp.Common.KeyBind 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.KeyBind 구조체를 반환합니다.</returns>
                internal static KeyBind getKeyBindValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Combo." + DisplayName, ChampUniq).GetValue<KeyBind>();
                }

                /// <summary>
                /// Combo 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Combo." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                internal static StringList getStringListValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Combo." + DisplayName, ChampUniq).GetValue<StringList>();
                }

                /// <summary>
                /// Combo 메뉴에 'Use Q' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Combo 메뉴에 'Use W' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Combo 메뉴에 'Use E' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Combo 메뉴에 'Use R' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Combo 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 0)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Harass 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Harass
            {
                /// <summary>
                /// Harass 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Harass").AddItem(new MenuItem("Harass." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Harass").AddItem(new MenuItem("Harass." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Harass 메뉴에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                /// <returns>항목의 bool값을 반환합니다.</returns>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Harass." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Harass 메뉴에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Harass." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Harass 메뉴에 'Auto Harass' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addAutoHarass(bool Enabled = true)
                {
                    addItem("Auto Harass", Enabled);
                }

                /// <summary>
                /// Harass 메뉴에 'Use Q' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Harass 메뉴에 'Use W' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Harass 메뉴에 'Use E' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Harass 메뉴에 'Use R' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Harass 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 60)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// Harass 메뉴 안에 존재하는 항목의 LeageuSharp.Common.KeyBind 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.KeyBind 구조체를 반환합니다.</returns>
                internal static KeyBind getKeyBindValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Harass." + DisplayName, ChampUniq).GetValue<KeyBind>();
                }

                /// <summary>
                /// 'Auto Harass' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool AutoHarass
                {
                    get
                    {
                        return getBoolValue("Auto Harass");
                    }
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Lasthit 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Lasthit
            {
                /// <summary>
                /// Lasthit 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Lasthit").AddItem(new MenuItem("Lasthit." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Lasthit").AddItem(new MenuItem("Lasthit." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Lasthit 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Lasthit." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Lasthit 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Lasthit." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Lasthit 메뉴에 'Use Q' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Lasthit 메뉴에 'Use W' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Lasthit 메뉴에 'Use E' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Lasthit 메뉴에 'Use R' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Lasthit 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 60)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Laneclear 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Laneclear
            {
                /// <summary>
                /// Laneclear 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Laneclear").AddItem(new MenuItem("Laneclear." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Laneclear").AddItem(new MenuItem("Laneclear." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Laneclear 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Laneclear." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Laneclear 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Laneclear." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Laneclear 메뉴에 'Use Q' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Laneclear 메뉴에 'Use W' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Laneclear 메뉴에 'Use E' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Laneclear 메뉴에 'Use R' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Laneclear 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 0)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Jungleclear 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Jungleclear
            {
                /// <summary>
                /// Jungleclear 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Jungleclear").AddItem(new MenuItem("Jungleclear." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Jungleclear").AddItem(new MenuItem("Jungleclear." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Jungleclear 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Jungleclear." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Jungleclear 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Jungleclear." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Jungleclear 메뉴에 'Use Q' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Jungleclear 메뉴에 'Use W' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Jungleclear 메뉴에 'Use E' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Jungleclear 메뉴에 'Use R' 항목을 추가합니다..
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Jungleclear 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 0)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Flee 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Flee
            {
                /// <summary>
                /// Flee 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Flee").AddItem(new MenuItem("Flee." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Flee").AddItem(new MenuItem("Flee." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Flee 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Flee." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Flee 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Flee." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Flee 메뉴에 'Use Q' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseQ(bool Enabled = true)
                {
                    addItem("Use Q", Enabled);
                }

                /// <summary>
                /// Flee 메뉴에 'Use W' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseW(bool Enabled = true)
                {
                    addItem("Use W", Enabled);
                }

                /// <summary>
                /// Flee 메뉴에 'Use E' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseE(bool Enabled = true)
                {
                    addItem("Use E", Enabled);
                }

                /// <summary>
                /// Flee 메뉴에 'Use R' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseR(bool Enabled = true)
                {
                    addItem("Use R", Enabled);
                }

                /// <summary>
                /// Flee 메뉴에 'If Mana >' 항목을 추가합니다.
                /// </summary>
                /// <param name="DefaultValue">항목의 초기값을 기입하십시오.</param>
                internal static void addIfMana(int DefaultValue = 0)
                {
                    addItem("If Mana >", new Slider(DefaultValue));
                }

                /// <summary>
                /// 'Use Q' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseQ
                {
                    get
                    {
                        return getBoolValue("Use Q");
                    }
                }

                /// <summary>
                /// 'Use W' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseW
                {
                    get
                    {
                        return getBoolValue("Use W");
                    }
                }

                /// <summary>
                /// 'Use E' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseE
                {
                    get
                    {
                        return getBoolValue("Use E");
                    }
                }

                /// <summary>
                /// 'Use R' 항목의 값을 반환합니다.
                /// </summary>
                internal static bool UseR
                {
                    get
                    {
                        return getBoolValue("Use R");
                    }
                }

                /// <summary>
                /// 'If Mana >' 항목의 값을 반환합니다.
                /// </summary>
                internal static int IfMana
                {
                    get
                    {
                        return getSliderValue("If Mana >").Value;
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Misc 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Misc
            {
                /// <summary>
                /// Misc 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Misc").AddItem(new MenuItem("Misc." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Misc").AddItem(new MenuItem("Misc." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Misc 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Misc." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Misc 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Misc." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Misc 메뉴 안에 존재하는 항목의 LeageuSharp.Common.StringList 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.StringList 구조체를 반환합니다.</returns>
                internal static StringList getStringListValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Misc." + DisplayName, ChampUniq).GetValue<StringList>();
                }

                /// <summary>
                /// Misc 메뉴 안에 존재하는 항목의 LeageuSharp.Common.KeyBind 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.KeyBind 구조체를 반환합니다.</returns>
                internal static KeyBind getKeyBindValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Misc." + DisplayName, ChampUniq).GetValue<KeyBind>();
                }

                /// <summary>
                /// Misc 메뉴에 'Use Anti-Gapcloser' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseAntiGapcloser(bool Enabled = true)
                {
                    addItem("Use Anti-Gapcloser", Enabled);
                }

                /// <summary>
                /// Misc 메뉴에 'Use Interrupter' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseInterrupter(bool Enabled = true)
                {
                    addItem("Use Interrupter", Enabled);
                }

                /// <summary>
                /// Misc 메뉴에 'Use Killsteal' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addUseKillsteal(bool Enabled = true)
                {
                    addItem("Use Killsteal", Enabled);
                }

                /// <summary>
                /// 'Use Anti-Gacploser' 항목의 값을 가져옵니다.
                /// </summary>
                internal static bool UseAntiGapcloser
                {
                    get
                    {
                        return getBoolValue("Use Anti-Gapcloser");
                    }
                }

                /// <summary>
                /// 'Use Interrupter' 항목의 값을 가져옵니다.
                /// </summary>
                internal static bool UseInterrupter
                {
                    get
                    {
                        return getBoolValue("Use Interrupter");
                    }
                }

                /// <summary>
                /// 'Use Killsteal' 항목의 값을 가져옵니다.
                /// </summary>
                internal static bool UseKillsteal
                {
                    get
                    {
                        return getBoolValue("Use Killsteal");
                    }
                }

                internal static void addQHitchanceSelector(HitChance defaultHitchance = HitChance.High)
                {
                    int defaultindex;

                    switch (defaultHitchance)
                    {
                        case HitChance.Low:
                            defaultindex = 0;
                            break;
                        case HitChance.Medium:
                            defaultindex = 1;
                            break;
                        case HitChance.High:
                            defaultindex = 2;
                            break;
                        case HitChance.VeryHigh:
                            defaultindex = 3;
                            break;
                        default:
                            defaultindex = 2;
                            break;
                    }

                    addItem("Q Hitchance", new StringList(new string[] { "Low", "Medium", "High", "Very High" }, defaultindex));
                }

                internal static void addWHitchanceSelector(HitChance defaultHitchance = HitChance.High)
                {
                    int defaultindex;

                    switch (defaultHitchance)
                    {
                        case HitChance.Low:
                            defaultindex = 0;
                            break;
                        case HitChance.Medium:
                            defaultindex = 1;
                            break;
                        case HitChance.High:
                            defaultindex = 2;
                            break;
                        case HitChance.VeryHigh:
                            defaultindex = 3;
                            break;
                        default:
                            defaultindex = 2;
                            break;
                    }

                    addItem("W Hitchance", new StringList(new string[] { "Low", "Medium", "High", "Very High" }, defaultindex));
                }

                internal static void addEHitchanceSelector(HitChance defaultHitchance = HitChance.High)
                {
                    int defaultindex;

                    switch (defaultHitchance)
                    {
                        case HitChance.Low:
                            defaultindex = 0;
                            break;
                        case HitChance.Medium:
                            defaultindex = 1;
                            break;
                        case HitChance.High:
                            defaultindex = 2;
                            break;
                        case HitChance.VeryHigh:
                            defaultindex = 3;
                            break;
                        default:
                            defaultindex = 2;
                            break;
                    }

                    addItem("E Hitchance", new StringList(new string[] { "Low", "Medium", "High", "Very High" }, defaultindex));
                }

                internal static void addRHitchanceSelector(HitChance defaultHitchance = HitChance.High)
                {
                    int defaultindex;

                    switch (defaultHitchance)
                    {
                        case HitChance.Low:
                            defaultindex = 0;
                            break;
                        case HitChance.Medium:
                            defaultindex = 1;
                            break;
                        case HitChance.High:
                            defaultindex = 2;
                            break;
                        case HitChance.VeryHigh:
                            defaultindex = 3;
                            break;
                        default:
                            defaultindex = 2;
                            break;
                    }

                    addItem("R Hitchance", new StringList(new string[] { "Low", "Medium", "High", "Very High" }, defaultindex));
                }

                internal static void addHitchanceSelector(HitChance defaultHitchance = HitChance.High)
                {
                    int defaultindex;

                    switch (defaultHitchance)
                    {
                        case HitChance.Low:
                            defaultindex = 0;
                            break;
                        case HitChance.Medium:
                            defaultindex = 1;
                            break;
                        case HitChance.High:
                            defaultindex = 2;
                            break;
                        case HitChance.VeryHigh:
                            defaultindex = 3;
                            break;
                        default:
                            defaultindex = 2;
                            break;
                    }

                    addItem("Hitchance", new StringList(new string[] { "Low", "Medium", "High", "Very High" }, defaultindex));
                }

                internal static HitChance QSelectedHitchance
                {
                    get
                    {
                        switch (getStringListValue("Q Hitchance").SelectedValue)
                        {
                            case "Low":
                                return HitChance.Low;
                            case "Medium":
                                return HitChance.Medium;
                            case "High":
                                return HitChance.High;
                            case "Very High":
                                return HitChance.VeryHigh;
                            default:
                                return HitChance.High;
                        }
                    }
                }

                internal static HitChance WSelectedHitchance
                {
                    get
                    {
                        switch (getStringListValue("W Hitchance").SelectedValue)
                        {
                            case "Low":
                                return HitChance.Low;
                            case "Medium":
                                return HitChance.Medium;
                            case "High":
                                return HitChance.High;
                            case "Very High":
                                return HitChance.VeryHigh;
                            default:
                                return HitChance.High;
                        }
                    }
                }

                internal static HitChance ESelectedHitchance
                {
                    get
                    {
                        switch (getStringListValue("E Hitchance").SelectedValue)
                        {
                            case "Low":
                                return HitChance.Low;
                            case "Medium":
                                return HitChance.Medium;
                            case "High":
                                return HitChance.High;
                            case "Very High":
                                return HitChance.VeryHigh;
                            default:
                                return HitChance.High;
                        }
                    }
                }

                internal static HitChance RSelectedHitchance
                {
                    get
                    {
                        switch (getStringListValue("R Hitchance").SelectedValue)
                        {
                            case "Low":
                                return HitChance.Low;
                            case "Medium":
                                return HitChance.Medium;
                            case "High":
                                return HitChance.High;
                            case "Very High":
                                return HitChance.VeryHigh;
                            default:
                                return HitChance.High;
                        }
                    }
                }

                internal static HitChance SelectedHitchance
                {
                    get
                    {
                        switch (getStringListValue("Hitchance").SelectedValue)
                        {
                            case "Low":
                                return HitChance.Low;
                            case "Medium":
                                return HitChance.Medium;
                            case "High":
                                return HitChance.High;
                            case "Very High":
                                return HitChance.VeryHigh;
                            default:
                                return HitChance.High;
                        }
                    }
                }
            }

            /// <summary>
            /// 이 클래스를 이용해서 Drawings 메뉴에 접근할 수 있습니다.
            /// </summary>
            internal class Drawings
            {
                /// <summary>
                /// Drawings 메뉴에 항목을 추가합니다.
                /// </summary>
                /// <param name="DisplayName">항목의 표기이름을 기입하십시오.</param>
                /// <param name="Value">항목의 초기값을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 항목의 이름에 플레이어의 챔피언이름을 포함한 항목이 생성됩니다.</param>
                internal static void addItem(string DisplayName, object Value = null, bool ChampUniq = true)
                {
                    if (Value == null)
                    {
                        MenuInstance.SubMenu("Champion").SubMenu("Drawings").AddItem(new MenuItem("Drawings." + DisplayName, DisplayName, ChampUniq));
                        return;
                    }

                    MenuInstance.SubMenu("Champion").SubMenu("Drawings").AddItem(new MenuItem("Drawings." + DisplayName, DisplayName, ChampUniq)).SetValue(Value);
                }

                /// <summary>
                /// Drawings 메뉴 안에 존재하는 항목의 Boolean값을 가져옵니다.
                /// </summary>
                /// <param name="DisplayName">값을 가져올 항목의 표기이름을 기입하십시오.</param>
                /// <param name="ChampUniq">이 값을 true로 지정하면 플레이어의 챔피언 이름을 포함한 이름을 가진 항목의 값을 가져옵니다.</param>
                internal static bool getBoolValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Drawings." + DisplayName, ChampUniq).GetValue<bool>();
                }

                /// <summary>
                /// Drawings 메뉴 안에 존재하는 항목의 LeageuSharp.Common.Slider 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeageuSharp.Common.Slider 구조체를 반환합니다.</returns>
                internal static Slider getSliderValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Drawings." + DisplayName, ChampUniq).GetValue<Slider>();
                }

                /// <summary>
                /// Drawings 메뉴 안에 존재하는 항목의 LeagueSharp.Common.Circle 구조체를 가져옵니다.
                /// </summary>
                /// <param name="DisplayName"></param>
                /// <param name="ChampUniq"></param>
                /// <returns>LeagueSharp.Common.Circle 구조체를 반환합니다.</returns>
                internal static Circle getCircleValue(string DisplayName, bool ChampUniq = true)
                {
                    return MenuInstance.Item("Drawings." + DisplayName, ChampUniq).GetValue<Circle>();
                }

                /// <summary>
                /// Drawings 메뉴에 'Q Range' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addDrawQrange(System.Drawing.Color Color, bool Enabled = true)
                {
                    addItem("Draw Q Range", new Circle(Enabled, Color));
                }

                /// <summary>
                /// Drawings 메뉴에 'W Range' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addDrawWrange(System.Drawing.Color Color, bool Enabled = true)
                {
                    addItem("Draw W Range", new Circle(Enabled, Color));
                }

                /// <summary>
                /// Drawings 메뉴에 'E Range' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addDrawErange(System.Drawing.Color Color, bool Enabled = true)
                {
                    addItem("Draw E Range", new Circle(Enabled, Color));
                }

                /// <summary>
                /// Drawings 메뉴에 'R Range' 항목을 추가합니다.
                /// </summary>
                /// <param name="Enabled">항목의 초기값을 기입하십시오.</param>
                internal static void addDrawRrange(System.Drawing.Color Color, bool Enabled = true)
                {
                    addItem("Draw R Range", new Circle(Enabled, Color));
                }

                /// <summary>
                /// 'Q Range' 항목의 Circle 구조체를 반환합니다.
                /// </summary>
                internal static Circle DrawQrange
                {
                    get
                    {
                        return getCircleValue("Draw Q Range");
                    }
                }

                /// <summary>
                /// 'W Range' 항목의 Circle 구조체를 반환합니다.
                /// </summary>
                internal static Circle DrawWrange
                {
                    get
                    {
                        return getCircleValue("Draw W Range");
                    }
                }

                /// <summary>
                /// 'E Range' 항목의 Circle 구조체를 반환합니다.
                /// </summary>
                internal static Circle DrawErange
                {
                    get
                    {
                        return getCircleValue("Draw E Range");
                    }
                }

                /// <summary>
                /// 'R Range' 항목의 Circle 구조체를 반환합니다.
                /// </summary>
                internal static Circle DrawRrange
                {
                    get
                    {
                        return getCircleValue("Draw R Range");
                    }
                }

                /// <summary>
                /// Drawings 메뉴에 DamageIndicator(데미지 표시기)를 추가합니다.
                /// </summary>
                /// <param name="damage"></param>

                /// <summary>
                /// Drawings 메뉴에 DamageIndicatorForJungle(정글몹에 데미지 표시)를 추가합니다.
                /// </summary>
                /// <param name="damage"></param>
            }
        }
    }
}
