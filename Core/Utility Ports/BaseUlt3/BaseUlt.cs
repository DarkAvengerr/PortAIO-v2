using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;



using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;

namespace BaseUlt3
{
    /*
     * fixed? use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
     * Fadeout even normal recall finishes
     * 
     * @beaving why doesn't team baseult work? did it require packets? we tried yesterday(before l# was fully updated) and my friend ulted, my character was about to ult and then after it saw ez ult it canceled I was playing jinx. WE lost a free kill lol because of it. Does it work? 
     * 
     * Hello beaving , iam a great fan of your scripts especially the baseult one , i can see u have did a great work in ur scripts , but while i was playing with baseult3 an idea came up to my mind , A script that can detect position of enemies in a circular shape
Example:http://imgur.com/2BGvB2C (sry for bad drawing )
The idea where the lines come from is that u can calculate how far they are from base (enemySpawnPos) , so is it possible to make a script that can just show the position of the enemy while recalling i guess it would help , especially if u can show these lines even when they're not recalling (not sure if it's possible tho ) that would help so much in ganks and other stuff , thanks for your time , and i hope you give me your opinion about this script , have a nice day 
     ---> draw growing circle as soon as enemies go into fow. if they start recalling, dont increase the circle range. if they finished -> reset. some time limit too?
     
     */

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

    internal class BaseUlt
    {
        Menu Menu;
        Menu TeamUlt;
        Menu DisabledChampions;

        Spell Ultimate;
        int LastUltCastT;

        LeagueSharp.Common.Utility.Map.MapType Map;

        List<AIHeroClient> Heroes;
        List<AIHeroClient> Enemies;
        List<AIHeroClient> Allies;

        public List<EnemyInfo> EnemyInfo = new List<EnemyInfo>();

        public Dictionary<int, int> RecallT = new Dictionary<int, int>();

        Vector3 EnemySpawnPos;

        Font Text;

        System.Drawing.Color NotificationColor = System.Drawing.Color.FromArgb(136, 207, 240);

        static float BarX = Drawing.Width * 0.425f;
        float BarY = Drawing.Height * 0.80f;
        static int BarWidth = (int)(Drawing.Width - 2 * BarX);
        int BarHeight = 6;
        int SeperatorHeight = 5;
        static float Scale = (float)BarWidth / 8000;

        public BaseUlt()
        {
            (Menu = new Menu("BaseUlt3", "BaseUlt3", true)).AddToMainMenu();
            Menu.AddItem(new MenuItem("showRecalls", "Show Recalls").SetValue(true));
            Menu.AddItem(new MenuItem("baseUlt", "Base Ult").SetValue(true));
            Menu.AddItem(new MenuItem("checkCollision", "Check Collision").SetValue(true));
            Menu.AddItem(new MenuItem("panicKey", "No Ult while SBTW").SetValue(new KeyBind(32, KeyBindType.Press))); //32 == space
            Menu.AddItem(new MenuItem("regardlessKey", "No timelimit (hold)").SetValue(new KeyBind(17, KeyBindType.Press))); //17 == ctrl

            Heroes = ObjectManager.Get<AIHeroClient>().ToList();
            Enemies = Heroes.Where(x => x.IsEnemy).ToList();
            Allies = Heroes.Where(x => x.IsAlly).ToList();

            EnemyInfo = Enemies.Select(x => new EnemyInfo(x)).ToList();

            bool compatibleChamp = IsCompatibleChamp(ObjectManager.Player.ChampionName);

            TeamUlt = Menu.AddSubMenu(new Menu("Team Baseult Friends", "TeamUlt"));
            DisabledChampions = Menu.AddSubMenu(new Menu("Disabled Champion targets", "DisabledChampions"));

            if (compatibleChamp)
            {
                foreach (AIHeroClient champ in Allies.Where(x => !x.IsMe && IsCompatibleChamp(x.ChampionName)))
                    TeamUlt.AddItem(new MenuItem(champ.ChampionName, "Ally with baseult: " + champ.ChampionName).SetValue(false).DontSave());

                foreach (AIHeroClient champ in Enemies)
                    DisabledChampions.AddItem(new MenuItem(champ.ChampionName, "Don't shoot: " + champ.ChampionName).SetValue(false).DontSave());
            }

            var NotificationsMenu = Menu.AddSubMenu(new Menu("Notifications", "Notifications"));

            NotificationsMenu.AddItem(new MenuItem("notifRecFinished", "Recall finished").SetValue(true));
            NotificationsMenu.AddItem(new MenuItem("notifRecAborted", "Recall aborted").SetValue(true));

            EnemySpawnPos = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy).Position; //ObjectManager.Get<GameObject>().FirstOrDefault(x => x.Type == GameObjectType.obj_SpawnPoint && x.IsEnemy).Position;

            Map = LeagueSharp.Common.Utility.Map.GetMap().Type;

            Ultimate = new Spell(SpellSlot.R);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 13, Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });

            Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;

            if (compatibleChamp)
                Game.OnUpdate += Game_OnUpdate;

            ShowNotification("BaseUlt3 by Beaving - Loaded", NotificationColor, 3000);
        }

        public void ShowNotification(string message, System.Drawing.Color color, int duration = -1, bool dispose = true)
        {
            Notifications.AddNotification(new Notification(message, duration, dispose).SetTextColor(color));
        }

        public bool IsCompatibleChamp(String championName)
        {
            return UltSpellData.Keys.Any(x => x == championName);
        }

        void Game_OnUpdate(EventArgs args)
        {
            int time = Utils.TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsHPBarRendered))
                enemyInfo.LastSeen = time;

            if (!Menu.Item("baseUlt").GetValue<bool>())
                return;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                !x.Player.IsDead &&
                !DisabledChampions.Item(x.Player.ChampionName).GetValue<bool>() &&
                x.RecallInfo.Recall.Status == TeleportStatus.Start && x.RecallInfo.Recall.Type == TeleportType.Recall).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (Utils.TickCount - LastUltCastT > 15000)
                    HandleUltTarget(enemyInfo);
            }
        }

        struct UltSpellDataS
        {
            public int SpellStage;
            public float DamageMultiplicator;
            public float Width;
            public float Delay;
            public float Speed;
            public bool Collision;
        }

        Dictionary<String, UltSpellDataS> UltSpellData = new Dictionary<string, UltSpellDataS>
        {
            {"Jinx",    new UltSpellDataS { SpellStage = 1, DamageMultiplicator = 1.0f, Width = 140f, Delay = 0600f/1000f, Speed = 1700f, Collision = true}},
            {"Ashe",    new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 130f, Delay = 0250f/1000f, Speed = 1600f, Collision = true}},
            {"Draven",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 0400f/1000f, Speed = 2000f, Collision = true}},
            {"Ezreal",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 1000f/1000f, Speed = 2000f, Collision = false}},
            {"Karthus", new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 000f, Delay = 3125f/1000f, Speed = 0000f, Collision = false}}
        };

        bool CanUseUlt(AIHeroClient hero) //use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
        {
            return hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                (hero.Spellbook.GetSpell(SpellSlot.R).Level > 0 && hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed && hero.Mana >= hero.Spellbook.GetSpell(SpellSlot.R).SData.Mana);
        }

        void HandleUltTarget(EnemyInfo enemyInfo)
        {
            bool ultNow = false;
            bool me = false;

            foreach (AIHeroClient champ in Allies.Where(x => //gathering the damage from allies should probably be done once only with timers
                            x.IsValid<AIHeroClient>() &&
                            !x.IsDead &&
                            ((x.IsMe && !x.IsStunned) || TeamUlt.Items.Any(item => item.GetValue<bool>() && item.Name == x.ChampionName)) &&
                            CanUseUlt(x)))
            {
                if (Menu.Item("checkCollision").GetValue<bool>() && UltSpellData[champ.ChampionName].Collision && IsCollidingWithChamps(champ, EnemySpawnPos, UltSpellData[champ.ChampionName].Width))
                {
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                //increase timeneeded if it should arrive earlier, decrease if later
                var timeneeded = GetUltTravelTime(champ, UltSpellData[champ.ChampionName].Speed, UltSpellData[champ.ChampionName].Delay, EnemySpawnPos) - 65;

                if (enemyInfo.RecallInfo.GetRecallCountdown() >= timeneeded)
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = (float)champ.GetSpellDamage(enemyInfo.Player, SpellSlot.R, UltSpellData[champ.ChampionName].SpellStage) * UltSpellData[champ.ChampionName].DamageMultiplicator;
                else if (enemyInfo.RecallInfo.GetRecallCountdown() < timeneeded - (champ.IsMe ? 0 : 125)) //some buffer for allies so their damage isnt getting reset
                {
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                if (champ.IsMe)
                {
                    me = true;

                    enemyInfo.RecallInfo.EstimatedShootT = timeneeded;

                    if (enemyInfo.RecallInfo.GetRecallCountdown() - timeneeded < 60)
                        ultNow = true;
                }
            }

            if (me)
            {
                if (!IsTargetKillable(enemyInfo))
                {
                    enemyInfo.RecallInfo.LockedTarget = false;
                    return;
                }

                enemyInfo.RecallInfo.LockedTarget = true;

                if (!ultNow || Menu.Item("panicKey").GetValue<KeyBind>().Active)
                    return;

                Ultimate.Cast(EnemySpawnPos, true);
                LastUltCastT = Utils.TickCount;
            }
            else
            {
                enemyInfo.RecallInfo.LockedTarget = false;
                enemyInfo.RecallInfo.EstimatedShootT = 0;
            }
        }

        bool IsTargetKillable(EnemyInfo enemyInfo)
        {
            float totalUltDamage = enemyInfo.RecallInfo.IncomingDamage.Values.Sum();

            float targetHealth = GetTargetHealth(enemyInfo, enemyInfo.RecallInfo.GetRecallCountdown());

            if (Utils.TickCount - enemyInfo.LastSeen > 20000 && !Menu.Item("regardlessKey").GetValue<KeyBind>().Active)
            {
                if (totalUltDamage < enemyInfo.Player.MaxHealth)
                    return false;
            }
            else if (totalUltDamage < targetHealth)
                return false;

            return true;
        }

        float GetTargetHealth(EnemyInfo enemyInfo, int additionalTime)
        {
            if (enemyInfo.Player.IsHPBarRendered)
                return enemyInfo.Player.Health;

            float predictedHealth = enemyInfo.Player.Health + enemyInfo.Player.HPRegenRate * ((Utils.TickCount - enemyInfo.LastSeen + additionalTime) / 1000f);

            return predictedHealth > enemyInfo.Player.MaxHealth ? enemyInfo.Player.MaxHealth : predictedHealth;
        }

        float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            if (source.ChampionName == "Karthus")
                return delay * 1000;

            float distance = Vector3.Distance(source.ServerPosition, targetpos);

            float missilespeed = speed;

            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                var acceldifference = distance - 1350f;

                if (acceldifference > 150f) //it only accelerates 150 units
                    acceldifference = 150f;

                var difference = distance - 1500f;

                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) + difference * 2200f) / distance;
            }

            return (distance / missilespeed + delay) * 1000;
        }

        bool IsCollidingWithChamps(AIHeroClient source, Vector3 targetpos, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                Unit = source,
            };

            input.CollisionObjects[0] = CollisionableObjects.Heroes;

            return Collision.GetCollision(new List<Vector3> { targetpos }, input).Any(); //x => x.NetworkId != targetnetid, hard to realize with teamult
        }

        void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly || unit.IsMe)
            {
                return;
            }

            var recall = new RecallInf(unit.NetworkId, args.Status, args.Type, args.Duration, args.Start);
            var enemyInfo = EnemyInfo.Find(x => x.Player.NetworkId == unit.NetworkId).RecallInfo.UpdateRecall(recall);

            if (recall.Type == TeleportType.Recall)
            {
                switch (recall.Status)
                {
                    case TeleportStatus.Abort:
                        if (Menu.Item("notifRecAborted").GetValue<bool>())
                        {
                            ShowNotification(enemyInfo.Player.ChampionName + ": Recall ABORTED", System.Drawing.Color.Orange, 4000);
                        }

                        break;
                    case TeleportStatus.Finish:
                        if (Menu.Item("notifRecFinished").GetValue<bool>())
                        {
                            ShowNotification(enemyInfo.Player.ChampionName + ": Recall FINISHED", NotificationColor, 4000);
                        }
                        break;
                }
            }
        }

        void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
        }

        void Drawing_OnDraw(EventArgs args)
        {
            if (!Menu.Item("showRecalls").GetValue<bool>() || Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;

            bool indicated = false;

            float fadeout = 1f;
            int count = 0;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (!enemyInfo.RecallInfo.LockedTarget)
                {
                    fadeout = 1f;
                    Color color = System.Drawing.Color.White;

                    if (enemyInfo.RecallInfo.WasAborted())
                    {
                        fadeout = (float)enemyInfo.RecallInfo.GetDrawTime() / (float)enemyInfo.RecallInfo.FADEOUT_TIME;
                        color = System.Drawing.Color.Yellow;
                    }

                    DrawRect(BarX, BarY, (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, System.Drawing.Color.FromArgb((int)(100f * fadeout), System.Drawing.Color.White));
                    DrawRect(BarX + Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY - SeperatorHeight, 0, SeperatorHeight + 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), color));

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY - SeperatorHeight - Text.Description.Height - 1, new ColorBGRA(color.R, color.G, color.B, (byte)((float)color.A * fadeout)));
                }
                else
                {
                    if (!indicated && enemyInfo.RecallInfo.EstimatedShootT != 0)
                    {
                        indicated = true;
                        DrawRect(BarX + Scale * enemyInfo.RecallInfo.EstimatedShootT, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight * 2, 2, System.Drawing.Color.Orange);
                    }

                    DrawRect(BarX, BarY, (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.Red));
                    DrawRect(BarX + Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight + 1, 1, System.Drawing.Color.IndianRed);

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * (float)enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY + SeperatorHeight + Text.Description.Height / 2, new ColorBGRA(255, 92, 92, 255));
                }

                count++;
            }

            /*
             * Show in a red rectangle right next to the normal bar the names of champs which can be killed (when they are not recalling yet)
             * Requires calculating the damages (make more functions!)
             * 
             * var BaseUltableEnemies = EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                !x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0 && x.RecallInfo.LockedTarget).OrderBy(x => x.RecallInfo.GetRecallCountdown());*/

            if (count > 0)
            {
                if (count != 1) //make the whole bar fadeout when its only 1
                    fadeout = 1f;

                DrawRect(BarX, BarY, BarWidth, BarHeight, 1, System.Drawing.Color.FromArgb((int)(40f * fadeout), System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 0, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarHeight, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarWidth, BarY + 1, 0, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
            }
        }

        public void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }
    }

    class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;

        public RecallInfo RecallInfo;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
            RecallInfo = new RecallInfo(this);
        }
    }

    class RecallInfo
    {
        public EnemyInfo EnemyInfo;
        public Dictionary<int, float> IncomingDamage; //from, damage
        public RecallInf Recall;
        public RecallInf AbortedRecall;
        public bool LockedTarget;
        public float EstimatedShootT;
        public int AbortedT;
        public int FADEOUT_TIME = 3000;

        public RecallInfo(EnemyInfo enemyInfo)
        {
            EnemyInfo = enemyInfo;
            Recall = new RecallInf(
                EnemyInfo.Player.NetworkId,
                TeleportStatus.Unknown,
                TeleportType.Unknown,
                0);
            IncomingDamage = new Dictionary<int, float>();
        }

        public bool ShouldDraw()
        {
            return IsPorting() || (WasAborted() && GetDrawTime() > 0);
        }

        public bool IsPorting()
        {
            return Recall.Type == TeleportType.Recall && Recall.Status == TeleportStatus.Start;
        }

        public bool WasAborted()
        {
            return Recall.Type == TeleportType.Recall && Recall.Status == TeleportStatus.Abort;
        }

        public EnemyInfo UpdateRecall(RecallInf newRecall)
        {
            IncomingDamage.Clear();
            LockedTarget = false;
            EstimatedShootT = 0;

            if (newRecall.Type == TeleportType.Recall && newRecall.Status == TeleportStatus.Abort)
            {
                AbortedRecall = Recall;
                AbortedT = Utils.TickCount;
            }
            else
                AbortedT = 0;

            Recall = newRecall;
            return EnemyInfo;
        }

        public int GetDrawTime()
        {
            int drawtime = 0;

            if (WasAborted())
                drawtime = FADEOUT_TIME - (Utils.TickCount - AbortedT);
            else
                drawtime = GetRecallCountdown();

            return drawtime < 0 ? 0 : drawtime;
        }

        public int GetRecallCountdown()
        {
            int time = EloBuddy.SDK.Core.GameTickCount;
            int countdown = 0;

            if (time - AbortedT < FADEOUT_TIME)
                countdown = AbortedRecall.Duration - (AbortedT - AbortedRecall.Start);
            else if (AbortedT > 0)
                countdown = 0; //AbortedT = 0
            else
                countdown = Recall.Start + Recall.Duration - time;

            return countdown < 0 ? 0 : countdown;
        }

        public override string ToString()
        {
            String drawtext = EnemyInfo.Player.ChampionName + ": " + Recall.Status; //change to better string and colored

            float countdown = GetRecallCountdown() / 1000f;

            if (countdown > 0)
                drawtext += " (" + countdown.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s)";

            return drawtext;
        }
    }
}
