using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using EloBuddy;
using LeagueSharp.Common;
using EloBuddy.SDK.Events;
using static EloBuddy.SDK.Events.Teleport;
using EloBuddy.SDK.Enumerations;

namespace UniversalRecallTracker
{
    public class Program
    {
        private readonly IDictionary<AIHeroClient, RecallInfo> _recallInfo = new Dictionary<AIHeroClient, RecallInfo>();

        private static Program _instance;

        private MenuItem _y;
        private MenuItem _x;
        private MenuItem _textSize;
        private MenuItem _chatWarning;
        private MenuItem _barScale;

        public int X
        {
            get { return _x.GetValue<Slider>().Value; }
        }

        public int Y
        {
            get { return _y.GetValue<Slider>().Value; }
        }

        public int TextSize
        {
            get { return _textSize.GetValue<Slider>().Value; }
        }

        public float BarScale
        {
            get { return _barScale.GetValue<Slider>().Value / 100f; }
        }

        public bool ChatWarning
        {
            get { return _chatWarning.GetValue<bool>(); }
        }

        public static void Main()
        {
            new Program();
        }

        private Program()
        {
            _instance = this;
            Game_OnGameLoad();
        }

        public static Program Instance()
        {
            if (_instance == null)
            {
                return new Program();
            }
            return _instance;
        }

        public void Game_OnGameLoad()
        {
            Menu menu = new Menu("Universal RecallTracker", "universalrecalltracker", true);
            _x =
                new MenuItem("x", "X").SetValue(
                    new Slider(
                        (int) ((Drawing.Direct3DDevice.Viewport.Width - PortAIO.Properties.Resources.RecallBar.Width) / 2f), 0,
                        Drawing.Direct3DDevice.Viewport.Width));
            _x.ValueChanged += _x_ValueChanged;
            _y =
                new MenuItem("y", "Y").SetValue(
                    new Slider(
                        (int) (Drawing.Direct3DDevice.Viewport.Height * 3f / 4f), 0,
                        Drawing.Direct3DDevice.Viewport.Height));
            _y.ValueChanged += _x_ValueChanged;
            _textSize = new MenuItem("textSize", "Text Size (F5 Reload)").SetValue(new Slider(15, 5, 50));
            _chatWarning = new MenuItem("chatWarning", "Chat Notification").SetValue(false);
            _barScale = new MenuItem("barScale", "Bar Scale %").SetValue(new Slider(100, 0, 200));
            _barScale.ValueChanged += _barScale_ValueChanged;

            menu.AddItem(_x);
            menu.AddItem(_y);
            menu.AddItem(_textSize);
            menu.AddItem(_barScale);
            menu.AddItem(_chatWarning);
            menu.AddToMainMenu();

            int i = 0;
            foreach (AIHeroClient hero in
                ObjectManager.Get<AIHeroClient>().Where(hero => hero.Team != ObjectManager.Player.Team))
            {
                RecallInfo recallInfo = new RecallInfo(hero, i++);
                _recallInfo[hero] = recallInfo;
            }
            //_recallInfo[ObjectManager.Player] = new RecallInfo(ObjectManager.Player, i);
            Print("Loaded!");
        }

        private void _barScale_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            foreach (RecallInfo info in _recallInfo.Values)
            {
                info.Scale(BarScale);
            }
        }

        private void _x_ValueChanged(object sender, OnValueChangeEventArgs e)
        {
            foreach (RecallInfo info in _recallInfo.Values)
            {
                info.Reset();
            }
        }

        public void Print(string msg, bool timer = false)
        {
            string s = null;
            if (timer)
            {
                s = "<font color='#d8d8d8'>[" + Utils.FormatTime(Game.Time) + "]</font> ";
            }
            s +=
                "<font color='#ff3232'>Universal</font><font color='#d4d4d4'>RecallTracker:</font> <font color='#FFFFFF'>" +
                msg + "</font>";
            Chat.Print(s);
        }

        public void Notify(string msg)
        {
            if (ChatWarning)
            {
                Print(msg, true);
            }
        }
    }

    public class RecallInfo
    {
        public const int GapTextBar = 10;

        private static readonly Font TextFont = new Font(
            Drawing.Direct3DDevice,
            new FontDescription
            {
                FaceName = "Calibri",
                Height = Program.Instance().TextSize,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default,
            });

        private readonly AIHeroClient _hero;
        private int _duration;
        private float _begin;
        private bool _active;
        private readonly int _index;

        private readonly Render.Sprite _sprite;
        private readonly Render.Text _countdownText;
        private readonly Render.Text _healthText;
        private int lastChange;

        public RecallInfo(AIHeroClient hero, int index)
        {
            _hero = hero;
            _index = index;
            _sprite = new Render.Sprite(PortAIO.Properties.Resources.RecallBar, new Vector2(0, 0))
            {
                Scale = new Vector2(Program.Instance().BarScale, Program.Instance().BarScale),
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate =
                    () =>
                        new Vector2(Program.Instance().X, Program.Instance().Y - (_index * TextFont.Description.Height))
            };
            _sprite.Add(0);
            _healthText = new Render.Text(0, 0, "", TextFont.Description.Height, Color.Green)
            {
                OutLined = true,
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate = delegate
                {
                    Rectangle rect = TextFont.MeasureText("("+(int) hero.HealthPercent +"%)");
                    return new Vector2(
                        _sprite.X - rect.Width - GapTextBar,
                        _sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                },
                TextUpdate = () => "(" + (int) hero.HealthPercent + "%)"
            };
            _healthText.Add(1);
            Render.Text heroText = new Render.Text(0, 0, hero.ChampionName, TextFont.Description.Height, Color.White)
            {
                OutLined = true,
                VisibleCondition = sender => _active || Environment.TickCount - lastChange < 3000,
                PositionUpdate = delegate
                {
                    Rectangle rect = TextFont.MeasureText(hero.ChampionName + _healthText.text);
                    return new Vector2(
                        _sprite.X - rect.Width - GapTextBar - 3,
                        _sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                }
            };

            heroText.Add(1);
            _countdownText = new Render.Text(0, 0, "", TextFont.Description.Height, Color.White)
            {
                OutLined = true,
                VisibleCondition = sender => _active
            };
            _countdownText.Add(1);
            Game.OnUpdate += Game_OnGameUpdate;
            Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
        }

        private void Obj_AI_Base_OnTeleport(GameObject sender, TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var decoded = new RecallInf(unit.NetworkId, args.Status, args.Type, args.Duration, args.Start);
            if (unit.NetworkId == _hero.NetworkId && decoded.Type == TeleportType.Recall)
            {
                switch (decoded.Status)
                {
                    case TeleportStatus.Start:
                        _begin = Game.Time;
                        _duration = decoded.Duration;
                        _active = true;
                        break;
                    case TeleportStatus.Finish:
                        int colorIndex = (int) ((_hero.HealthPercent / 100) * 255);
                        string color = (255 - colorIndex).ToString("X2") + colorIndex.ToString("X2") + "00";
                        Program.Instance().Notify(_hero.ChampionName + " has recalled with <font color='#" + color + "'>" + (int) _hero.HealthPercent + "&#37; HP</font>");
                        _active = false;
                        break;
                    case TeleportStatus.Abort:
                        _active = false;
                        break;
                    case TeleportStatus.Unknown:
                        Program.Instance()
                            .Notify(
                                _hero.ChampionName + " is <font color='#ff3232'>unknown</font> (" +
                                _hero.Spellbook.GetSpell(SpellSlot.Recall).Name + ")");
                        _active = false;
                        break;
                }
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            float colorPercentage = (_hero.HealthPercent / 100);
            _healthText.Color = new ColorBGRA(1 - colorPercentage, colorPercentage, 0, 1);
            if (_active && _duration > 0)
            {
                float percentage = (Game.Time - _begin) / (_duration / 1000f);
                int width = (int) (_sprite.Width - (percentage * _sprite.Width));
                _countdownText.X = (int) (_sprite.X + (width * _sprite.Scale.X) + GapTextBar);
                _countdownText.text =
                    Math.Round(
                        (Decimal) ((_duration / 1000f) - (Game.Time - _begin)), 1, MidpointRounding.AwayFromZero) +
                    "s";
                Rectangle rect = TextFont.MeasureText(_countdownText.text);
                _countdownText.Y =
                    (int) (_sprite.Y - rect.Height / 2 + (_sprite.Height * Program.Instance().BarScale) / 2);
                _sprite.Crop(0, 0, width, _sprite.Height);
            }
            else
            {
                _sprite.Crop(0, 0, _sprite.Width, _sprite.Height);
            }
        }

        public void Reset()
        {
            lastChange = Environment.TickCount;
        }

        public void Scale(float barScale)
        {
            _sprite.Scale = new Vector2(barScale, barScale);
            Reset();
        }
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