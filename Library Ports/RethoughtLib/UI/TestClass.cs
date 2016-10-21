using EloBuddy; 
 using LeagueSharp.Common; 
 namespace RethoughtLib.UI
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;

    using global::RethoughtLib.UI.Core;
    using global::RethoughtLib.UI.Core.Designs;
    using global::RethoughtLib.UI.DefaultImplementations.Displayer;
    using global::RethoughtLib.UI.Notifications;
    using global::RethoughtLib.UI.Notifications.Designs;

    #endregion

    internal class TestClass
    {
        #region Methods

        private void Test()
        {
            // displayer that accepts every type of ELEMENT
            var displayer = new DefaultDisplayer<Element>();

            var notification = new DefaultNotification("I'm a title", "I'm the content")
                                   {
                                       Design = new CompactNotificationDesign()
                                   };

            displayer.Add(notification);
            displayer.Display();
        }

        #endregion
    }
}