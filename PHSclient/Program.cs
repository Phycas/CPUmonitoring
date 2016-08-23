using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using DataThing;

namespace PHSclient
{
    class Program
    {
        // public static Thread telRead;
        //
        /// <summary>
        /// ///////////////PHS Telemetry Monitoring Client////////////////////////////////////
        /// </summary>
        /// <param name="args"></param>
        public static IPEndPoint serverIP;
        static void Main(string[] args)
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 4242);
            serverIP = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 4242);
            askServerIP();
            Console.WriteLine("Connecting");
            try { clientSocket.Connect(serverIP); Console.WriteLine("Connected");  }
            catch (SocketException ex){
                Console.WriteLine("ERROR: " + ex.Message);
                Console.ReadLine();
            }
            //byte[] test = new byte[client.SendBufferSize];//succeded at last
            byte[] test = new byte[0]; //another way to do it
            Array.Resize(ref test, clientSocket.SendBufferSize); // resising the array to fit the packets from the client socket
            for (;;)
            {
                try
                {
                    clientSocket.Receive(test);
                }
                catch (SocketException ex) { Console.WriteLine("error writing the bytes from server /n" + ex.Message); Console.ReadLine(); }
                try
                {
                    Packet p = new Packet(test);
                    Console.Clear();
                    Console.WriteLine("-------CPU------");
                    Console.WriteLine("Temp: " + p.tData[0] + "C°");
                    Console.WriteLine("Load: " + p.tData[1] + "%");
                    Console.WriteLine("Receiving data from: " + clientSocket.RemoteEndPoint); //finish this
                }
                catch (SocketException ex) { Console.WriteLine("Problem with the Packet: " + ex.Message); }
            }
            //Console.ReadLine();


        }

        public static void askServerIP()
        {
            Console.WriteLine("Please enter the server ip");
            string rawIp = Console.ReadLine();
            serverIP = new IPEndPoint(IPAddress.Parse(rawIp), 4242);

        }

        static void telThread()
        {

        }
    }
}
