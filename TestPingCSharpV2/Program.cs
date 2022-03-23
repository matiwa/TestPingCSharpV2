using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;

namespace TestPingCSharpV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Test Ping";
            Console.WriteLine("Enter the IP Address or WWW");
            string adresWWW = Console.ReadLine();

            if (adresWWW.Length > 0)
            {
                try
                {
                    AutoResetEvent waiter = new AutoResetEvent(false);

                    Ping pingSender = new Ping();

                    pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

                    string data = "Message";
                    byte[] buforBajtow = Encoding.ASCII.GetBytes(data);

                    int timeout = 3600;

                    PingOptions opcje = new PingOptions(64, true);

                    Console.WriteLine("Time to live: " + opcje.Ttl.ToString());
                    Console.WriteLine("Do not fragment packages: " + opcje.DontFragment.ToString());

                    pingSender.SendAsync(adresWWW, timeout, buforBajtow, opcje, waiter);

                    waiter.WaitOne();
                    Console.WriteLine("The command ended.");
                }catch(Exception)
                {
                    Console.WriteLine("Wrong IP Address or WWW");
                }
            }
            else if (adresWWW.Length == 0) Console.WriteLine("There is no IP Address or WWW");
            Console.ReadKey();
        }

        public static void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("Cancelled Ping.");
                ((AutoResetEvent)e.UserState).Set();
            }

            if (e.Error != null)
            {
                Console.WriteLine("Ping error:");
                Console.WriteLine(e.Error.ToString());
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply odpowiedz = e.Reply;
            WyswietlOdpowiedzPing(odpowiedz);
            ((AutoResetEvent)e.UserState).Set();
        }

        public static void WyswietlOdpowiedzPing(PingReply odpowiedz)
        {
            if (odpowiedz == null) return;

            Console.WriteLine("Ping command status: "+odpowiedz.Status.ToString());
            if (odpowiedz.Status == IPStatus.Success)
            {
                Console.WriteLine("IP Address: " + odpowiedz.Address.ToString());
                Console.WriteLine("Time to live: " + odpowiedz.Options.Ttl.ToString());
                Console.WriteLine("It is not fragmented: " + odpowiedz.Options.DontFragment.ToString());
                Console.WriteLine("Buffer size: " + odpowiedz.Buffer.Length.ToString());
            }
        }
    }
}
