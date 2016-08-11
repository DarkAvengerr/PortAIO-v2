
#pragma warning disable 618

using EloBuddy; namespace ElUtilitySuite.Trackers
{
    //Recall tracker from BaseUlt

    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = System.Drawing.Color;
    using Font = SharpDX.Direct3D9.Font;
    using EloBuddy.SDK.Enumerations;

    #endregion

    internal class RecallTracker : IPlugin
    {
        #region Constants

        private const int BarHeight = 10;

        #endregion

        #region Fields

        public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        private readonly int SeperatorHeight = 5;

        private LeagueSharp.Common.Utility.Map.MapType Map;

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private int BarWidth => (int)(Drawing.Width - 2 * this.BarX);

        /// <summary>
        /// </summary>
        private int BarX => (int)(Drawing.Width * 0.425f);

        /// <summary>
        /// </summary>
        private int BarY
            => (int)(Drawing.Height - 150f - this.Menu.Item("RecallTracker.OffsetBottom").GetValue<Slider>().Value);

        private List<AIHeroClient> Enemies { get; set; }

        private List<AIHeroClient> Heroes { get; set; }

        /// <summary>
        /// </summary>
        private float Scale => (float)this.BarWidth / 8000;

        private Font Text { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            var predicate = new Func<Menu, bool>(x => x.Name == "Trackers");
            var menu = !rootMenu.Children.Any(predicate)
                           ? rootMenu.AddSubMenu(new Menu("Trackers", "Trackers"))
                           : rootMenu.Children.First(predicate);

            var notificationsMenu = menu.AddSubMenu(new Menu("Recall tracker", "Recall tracker"));
            {
                notificationsMenu.AddItem(new MenuItem("showRecalls", "Show Recalls").SetValue(true));
                notificationsMenu.AddItem(new MenuItem("notifRecFinished", "Recall finished").SetValue(true));
                notificationsMenu.AddItem(new MenuItem("notifRecAborted", "Recall aborted").SetValue(true));
                notificationsMenu.AddItem(
                    new MenuItem("RecallTracker.OffsetBottom", "Offset bottom").SetValue(new Slider(52, 0, 1500)));
                notificationsMenu.AddItem(
                    new MenuItem("RecallTracker.FontSize", "Font size").SetValue(new Slider(13, 13, 30)));
            }

            this.Menu = menu;
        }

        public void Load()
        {
            this.Heroes = ObjectManager.Get<AIHeroClient>().ToList();
            this.Enemies = HeroManager.AllHeroes.ToList();

            this.EnemyInfo = this.Enemies.Select(x => new EnemyInfo(x)).ToList();

            foreach(var enemy in EnemyInfo)
            {
                Console.WriteLine(enemy.Player.ChampionName);
            }

            this.Map = LeagueSharp.Common.Utility.Map.GetMap().Type;

            this.Text = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                    {
                        FaceName = "Calibri", Height = this.Menu.Item("RecallTracker.FontSize").GetValue<Slider>().Value,
                        Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default
                    });

            EloBuddy.SDK.Events.Teleport.OnTeleport += this.Obj_AI_Base_OnTeleport;
            Drawing.OnDraw += this.Drawing_OnDraw;
            Drawing.OnPreReset += args => { this.Text.OnLostDevice(); };
            Drawing.OnPostReset += args => { this.Text.OnResetDevice(); };
            AppDomain.CurrentDomain.DomainUnload += this.CurrentDomainDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += this.CurrentDomainDomainUnload;
        }

        #endregion

        #region Methods

        private void CurrentDomainDomainUnload(object sender, EventArgs e)
        {
            this.Text.Dispose();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!this.Menu.Item("showRecalls").GetValue<bool>() || Drawing.Direct3DDevice == null
                || Drawing.Direct3DDevice.IsDisposed)
            {
                return;
            }

            var indicated = false;

            var fadeout = 1f;
            var count = 0;

            foreach (var enemyInfo in
                this.EnemyInfo.Where(
                    x =>
                    x.Player.IsValid<AIHeroClient>() && x.RecallInfo.ShouldDraw() && !x.Player.IsDead
                    && x.RecallInfo.GetRecallCountdown() > 0).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (!indicated && (int)enemyInfo.RecallInfo.EstimatedShootT != 0)
                {
                    indicated = true;
                    this.DrawRect(
                        this.BarX + this.Scale * enemyInfo.RecallInfo.EstimatedShootT,
                        this.BarY + this.SeperatorHeight + BarHeight - 3,
                        0,
                        this.SeperatorHeight * 2,
                        2,
                        Color.White);
                }

                this.DrawRect(
                    this.BarX,
                    this.BarY,
                    (int)(this.Scale * enemyInfo.RecallInfo.GetRecallCountdown()),
                    BarHeight,
                    1,
                    Color.FromArgb(255, Color.DeepSkyBlue));

                this.DrawRect(
                    this.BarX + this.Scale * enemyInfo.RecallInfo.GetRecallCountdown() - 1,
                    this.BarY + this.SeperatorHeight + BarHeight - 3,
                    0,
                    this.SeperatorHeight + 1,
                    1,
                    Color.White);

                this.Text.DrawText(
                    null,
                    $"{enemyInfo.Player.ChampionName} ({(int)enemyInfo.Player.HealthPercent})%",
                    (int)this.BarX
                    + (int)
                      (this.Scale * enemyInfo.RecallInfo.GetRecallCountdown()
                       - (float)(enemyInfo.Player.ChampionName.Length * this.Text.Description.Width) / 2),
                    (int)this.BarY + this.SeperatorHeight + this.Text.Description.Height / 2,
                    new ColorBGRA(255, 255, 255, 255));

                count++;
            }

            if (count > 0)
            {
                if (count != 1)
                {
                    fadeout = 1f;
                }

                this.DrawRect(
                    this.BarX,
                    this.BarY,
                    this.BarWidth,
                    BarHeight,
                    1,
                    Color.FromArgb((int)(40f * fadeout), Color.White));

                this.DrawRect(
                    this.BarX - 1,
                    this.BarY + 1,
                    0,
                    BarHeight,
                    1,
                    Color.FromArgb((int)(255f * fadeout), Color.White));
                this.DrawRect(
                    this.BarX - 1,
                    this.BarY - 1,
                    this.BarWidth + 2,
                    1,
                    1,
                    Color.FromArgb((int)(255f * fadeout), Color.White));
                this.DrawRect(
                    this.BarX - 1,
                    this.BarY + BarHeight,
                    this.BarWidth + 2,
                    1,
                    1,
                    Color.FromArgb((int)(255f * fadeout), Color.White));
                this.DrawRect(
                    this.BarX + 1 + this.BarWidth,
                    this.BarY + 1,
                    0,
                    BarHeight,
                    1,
                    Color.FromArgb((int)(255f * fadeout), Color.White));
            }
        }

        private void DrawRect(float x, float y, int width, int height, float thickness, Color color)
        {
            for (var i = 0; i < height; i++)
            {
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
            }
        }

        private void Obj_AI_Base_OnTeleport(GameObject sender, EloBuddy.SDK.Events.Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var recallinfo = new RecallInf(unit.NetworkId, args.Status, args.Type, args.Duration, args.Start);
            var enemyInfo = this.EnemyInfo.Find(x => x.Player.NetworkId == unit.NetworkId).RecallInfo.UpdateRecall(recallinfo);

            if (args.Type == TeleportType.Recall)
            {
                switch (args.Status)
                {
                    case TeleportStatus.Abort:
                        if (this.Menu.Item("notifRecAborted").GetValue<bool>())
                        {
                            this.ShowNotification(
                                enemyInfo.Player.ChampionName + ": Recall ABORTED",
                                Color.Orange,
                                4000);
                        }

                        break;
                    case TeleportStatus.Finish:
                        if (this.Menu.Item("notifRecFinished").GetValue<bool>())
                        {
                            this.ShowNotification(
                                enemyInfo.Player.ChampionName + ": Recall FINISHED",
                                Color.White,
                                4000);
                        }
                        break;
                    case TeleportStatus.Start:
                        if (this.Menu.Item("notifRecFinished").GetValue<bool>())
                        {
                            this.ShowNotification(
                                enemyInfo.Player.ChampionName + ": Recall STARTED",
                                Color.White,
                                4000);
                        }

                        break;
                }
            }
        }

        private void ShowNotification(string message, Color color, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose).SetTextColor(color));
        }

        #endregion
    }

    internal class EnemyInfo
    {
        #region Fields

        public AIHeroClient Player;

        public RecallInfo RecallInfo;

        #endregion

        #region Constructors and Destructors

        public EnemyInfo(AIHeroClient player)
        {
            this.Player = player;
            this.RecallInfo = new RecallInfo(this);
        }

        #endregion
    }

    internal class RecallInfo
    {
        #region Fields

        public float EstimatedShootT;

        public const int FadeoutTime = 3000;

        private readonly EnemyInfo enemyInfo;

        private RecallInf abortedRecall;

        private int abortedT;

        private RecallInf recall;

        #endregion

        #region Constructors and Destructors

        public RecallInfo(EnemyInfo enemyInfo)
        {
            this.enemyInfo = enemyInfo;
            this.recall = new RecallInf(
                this.enemyInfo.Player.NetworkId,
                TeleportStatus.Unknown,
                TeleportType.Unknown,
                0);
        }

        #endregion

        #region Public Methods and Operators

        public int GetDrawTime()
        {
            var drawtime = 0;

            if (this.WasAborted())
            {
                drawtime = FadeoutTime - (Utils.TickCount - this.abortedT);
            }
            else
            {
                drawtime = this.GetRecallCountdown();
            }

            return drawtime < 0 ? 0 : drawtime;
        }

        public int GetRecallCountdown()
        {
            var time = Utils.TickCount;
            var countdown = 0;

            if (time - this.abortedT < FadeoutTime)
            {
                countdown = this.abortedRecall.Duration - (this.abortedT - this.abortedRecall.Start);
            }
            else if (this.abortedT > 0)
            {
                countdown = 0;
            }
            else
            {
                countdown = this.recall.Start + this.recall.Duration - time;
            }

            return countdown < 0 ? 0 : countdown;
        }

        public bool IsPorting()
        {
            return this.recall.Type == TeleportType.Recall
                   && this.recall.Status == TeleportStatus.Start;
        }

        public bool ShouldDraw()
        {
            return this.IsPorting() || (this.WasAborted() && this.GetDrawTime() > 0);
        }

        public override string ToString()
        {
            var drawtext = this.enemyInfo.Player.ChampionName + ": " + this.recall.Status;

            var countdown = this.GetRecallCountdown() / 1000f;

            if (countdown > 0)
            {
                drawtext += " (" + countdown.ToString("0.00", CultureInfo.InvariantCulture) + "s)";
            }

            return drawtext;
        }

        public EnemyInfo UpdateRecall(RecallInf newRecall)
        {
            this.EstimatedShootT = 0;

            if (newRecall.Type == TeleportType.Recall && newRecall.Status == TeleportStatus.Abort)
            {
                this.abortedRecall = this.recall;
                this.abortedT = Utils.TickCount;
            }
            else
            {
                this.abortedT = 0;
            }

            this.recall = newRecall;
            return this.enemyInfo;
        }

        public bool WasAborted()
        {
            return this.recall.Type == TeleportType.Recall
                   && this.recall.Status == TeleportStatus.Abort;
        }

        #endregion
    }

    public class RecallInf
    {
        public int NetworkID;
        public int Duration;
        public int Start;
        public TeleportType Type;
        public TeleportStatus Status;

        public RecallInf(int netid, TeleportStatus stat, TeleportType tpe, int dura, int star = 0)
        {
            NetworkID = netid;
            Status = stat;
            Type = tpe;
            Duration = dura;
            Start = star;
        }
    }
}