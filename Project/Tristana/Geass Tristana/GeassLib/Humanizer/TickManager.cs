using SharpDX;
using System;
using System.Collections.Generic;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace GeassLib.Humanizer
{
    public class TickManager
    {

        private float _randomMax;
        private float _randomMin;
        private readonly Random _rnd;

        public readonly Dictionary<string, Tick> Ticks = new Dictionary<string, Tick>();

        public TickManager()
        {
            _rnd = new Random();
        }

        public void AddTick(string keyName, float min, float max)
        {
            if (keyName.Length <= 0)
                Console.WriteLine("Add Key can not be null");

            if (IsPresent(keyName))
            {
                Console.WriteLine($"Key {keyName} already created");
                return;
            }

            Ticks.Add(keyName, new Tick(min, max));
        }

        public bool CheckTick(string key)
        {
            if (key.Length <= 0)
                Console.WriteLine("Check Key an not be null");
            if (Ticks.ContainsKey(key))
                return Ticks[key].IsReady(Globals.Variables.AssemblyTime());

            Console.WriteLine($"Key {key} not found");
            return false;
        }

        public bool IsPresent(string key)
        {
            return Ticks.ContainsKey(key);
        }

        public void SetRandomizer(float min, float max)
        {
            _randomMin = min;
            _randomMax = max;
        }

        public void UseTick(string key)
        {
            if (key.Length <= 0) return;

            if (Ticks.ContainsKey(key))
                Ticks[key].UseTick(Globals.Variables.AssemblyTime(), _rnd.NextFloat(Ticks[key].GetMinDelay(), Ticks[key].GetMaxDelay()) + _rnd.NextFloat(_randomMin, _randomMax));
            else
                Console.WriteLine($"Key {key} not found");
        }
    }
}