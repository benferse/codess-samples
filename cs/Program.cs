using System;
using System.IO;
using System.Threading;

namespace Microsoft.Garage.Codess.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DelayedWriteLine("(should play the game of life here)");
            DelayedWriteLine("But sadly we don't");
        }

        private static void DelayedWriteLine(string text)
        {
            try
            {
                Console.Clear();
            }
            catch (IOException)
            {
                // This should only really happen if the output is being redirected,
                // and if that's true then we assume this is safe to ignore
            }

            Console.WriteLine(text);
            Thread.Sleep(2000);
        }
    }
}
