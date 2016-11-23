using EloBuddy; 
using LeagueSharp.Common; 
 namespace RethoughtLib.UI
{
    #region Using Directives

    using Core;
    using DefaultImplementations.Displayer;
    using Notifications;
    using Notifications.Designs;

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