using System;
using System.Collections.Generic;
using SharpDX;
using _Project_Geass.Functions;

using EloBuddy; 
 using LeagueSharp.Common; 
 namespace _Project_Geass.Humanizer.TickTock
{

    internal class Manager
    {
        #region Public Fields

        /// <summary>
        ///     The ticks
        /// </summary>
        public readonly Dictionary<string, Tick> Ticks=new Dictionary<string, Tick>();

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Manager" /> class.
        /// </summary>
        public Manager() {_rng=new Random();}

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        ///     Adds the tick.
        /// </summary>
        /// <param name="keyName">
        ///     Name of the key.
        /// </param>
        /// <param name="min">
        ///     The minimum.
        /// </param>
        /// <param name="max">
        ///     The maximum.
        /// </param>
        public void AddTick(string keyName, float min, float max)
        {
            if (keyName.Length<=0)
                Console.WriteLine("Add Key can not be null");

            if (IsPresent(keyName))
            {
                Console.WriteLine($"Key {keyName} already created");
                return;
            }

            Ticks.Add(keyName, new Tick(min, max));
        }

        /// <summary>
        ///     Checks the tick.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        /// </returns>
        public bool CheckTick(string key)
        {
            if (key.Length<=0)
                Console.WriteLine("Check Key an not be null");
            if (Ticks.ContainsKey(key))
                return Ticks[key].IsReady(AssemblyTime.CurrentTime());

            Console.WriteLine($"Key {key} not found");
            return false;
        }

        /// <summary>
        ///     Determines whether the specified key is present.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     <c> true </c> if the specified key is present; otherwise, <c> false </c>.
        /// </returns>
        public bool IsPresent(string key) {return Ticks.ContainsKey(key);}

        /// <summary>
        ///     Sets the randomizer.
        /// </summary>
        /// <param name="min">
        ///     The minimum.
        /// </param>
        /// <param name="max">
        ///     The maximum.
        /// </param>
        public void SetRandomizer(float min, float max)
        {
            _randomMin=min;
            _randomMax=max;
        }

        /// <summary>
        ///     Uses the tick.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        public void UseTick(string key)
        {
            if (key.Length<=0)
                return;

            if (Ticks.ContainsKey(key))
                Ticks[key].UseTick(AssemblyTime.CurrentTime(), _rng.NextFloat(Ticks[key].GetMinDelay(), Ticks[key].GetMaxDelay())+_rng.NextFloat(_randomMin, _randomMax));
            else
                Console.WriteLine($"Key {key} not found");
        }

        #endregion Public Methods

        #region Private Fields

        /// <summary>
        ///     The RNG
        /// </summary>
        private readonly Random _rng;

        /// <summary>
        ///     The random maximum
        /// </summary>
        private float _randomMax;

        /// <summary>
        ///     The random minimum
        /// </summary>
        private float _randomMin;

        #endregion Private Fields
    }

}