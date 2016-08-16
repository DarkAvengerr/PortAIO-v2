using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;using DetuksSharp;

using EloBuddy; namespace ARAMDetFull
{
    class ErrorLogger : TextWriter
    {
        private List<string> knowsExceptions = new List<string>(); 

        private TextWriter defaultOut = null;
        public ErrorLogger(TextWriter defaultOut)
        {
            this.defaultOut = defaultOut;
        }
        public override Encoding Encoding => Encoding.UTF8;
        
        public override void WriteLine(string value)
        {
            try
            {
                if (value.Contains("xception") && !knowsExceptions.Contains(value))
                {
                    knowsExceptions.Add(value);
                    DataGathering.sendError(value);
                    Console.WriteLine(value);
                }
                if (defaultOut != null && defaultOut != this)
                    defaultOut.WriteLine(value);
            }
            catch (Exception ex)
            {
                defaultOut.WriteLine(value);
            }
        }

        public override void Write(string value)
        {
            WriteLine(value);
        }
        
    }
}
