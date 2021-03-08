using System;
using System.Net;
using System.Net.NetworkInformation;

namespace ConsoleApp6
{
    class Program
    {
        private static TerminalDetails terminalDetails = new TerminalDetails();
       static TerminalFunction terminalFunction = new TerminalFunction();
        static void Main(string[] args)
        {
            static bool CheckForInternetConnection()
            {
                try
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://google.com/generate_204"))
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            terminalDetails = terminalFunction.GetFullTerminalInfo();

            //Console.WriteLine(CheckForInternetConnection());
        }
    }
}

