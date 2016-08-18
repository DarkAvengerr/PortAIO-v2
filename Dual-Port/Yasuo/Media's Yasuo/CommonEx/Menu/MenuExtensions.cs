using EloBuddy; 
 using LeagueSharp.Common; 
 namespace YasuoMedia.CommonEx.Menu
{
    using LeagueSharp.Common;

    public static class MenuExtensions
    {
        #region Public Methods and Operators

        public static void AddToolTip(Menu menu, string helpText)
        {
            menu.AddItem(new MenuItem(menu.Name + " Helper", "Helper").SetTooltip(helpText));
        }

        public static void Hide(this MenuItem item)
        {
            if (item != null)
            {
                if (item.ShowItem)
                {
                    item.Show(false);
                }
            }
        }

        #endregion  

        #region Methods

        internal static void RefreshTagBased(Menu menu, int tag)
        {
            if (menu == null)
            {
                return;
            }

            foreach (var item in menu.Items)
            {
                if (item.Tag != 0)
                {
                    item.Hide();
                }

                if (item.Tag == tag)
                {
                    item.Show();
                }
            }
        }

        #endregion
    }
}