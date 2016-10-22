using System;

namespace Mundo
{
    public static class Program
    {
        public static void Main()
        {
            try
            {
                new Mundo();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Could not load the assembly - {0}", exception);
                throw;
            }
        }
    }
}
