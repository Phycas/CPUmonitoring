using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Speech.Synthesis;// this is for the computer to speak
using System.Net;
using System.Net.Sockets;
using System.IO;
using DataThing; //Packet class here


namespace PHSt
{
    class Server
    {
        //to do: 
        //-Optimize methods
        //-(exception)error handling
        //-handle disconnection
        public static Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static IPEndPoint ip = new IPEndPoint(0, 4242);
        static void Main(string[] args)
        {
            //console things
            Console.WindowHeight = 6;
            Console.WindowWidth = 28;

            Console.WriteLine("Loading...");
            voice.Speak("version 0.8", 2);
            //voice.Speak("I Love you! ", 1);

            //Optimize this
            //mounting the server
            Console.WriteLine("Starting server"); //make the try catch later
         
            sSocket.Bind(ip);
            Thread Listen = new Thread(listenThread);
            Listen.Start();

            

            Console.WriteLine("Done!");

            Console.WriteLine("Starting Telemetry");
            telemetry.pTelemetry();//starts the telemetry reading thread
            /*
            byte[] test = telemetry.tData();

            Packet p = new Packet(test);
            Console.WriteLine(p.tData[0]);
            Console.WriteLine(p.tData[1]);
            Console.ReadLine();
            */
        }



        static void listenThread()
        {
            for (;;)
            {
            sSocket.Listen(0);
            clientData test = new clientData(sSocket.Accept());
            }
        }

        public static void dataSend(object cSocket)
        {
            Socket cs = (Socket)cSocket;
            for (;;)
            {
                cs.Send(telemetry.tData());
                Thread.Sleep(1000);
            }
        }


    }

    class telemetry
    {
        public static Thread readings;
        public static PerformanceCounter PC_uptime = new PerformanceCounter("System", "System Up Time"); //uptime from the Performance Counter of windows
        public static Computer pc = new Computer() //Creating the computer object for readings
        {
            CPUEnabled = true, //initialize the CPU
            GPUEnabled = true //initialize the GPU
        };
        public static void pTelemetry()//  print telemetry
        {
            PC_uptime.NextValue(); //the method will activate all this things and then start the thread that actually prints the info
            pc.Open();
            readings = new Thread(readThread);
            readings.Start();

        }
        static void readThread()
        {
            while (true) //infinite while loop for temp readings and print to console
            {
                //readCPUTemperature();
                Console.Clear();
                int cputemp = (int)CPU_temp(); //convert float to int
                int cpuload = (int)CPU_load();
                Console.WriteLine(UpTime()); //prints the uptime of the system
                Console.WriteLine("-----------CPU-----------");
                Console.WriteLine("Temp: " + cputemp + "C°"); //prints the cpu temperature
                Console.WriteLine("Load: " + cpuload + "%"); //prints the cpu load
                Console.WriteLine("-----------MISC----------");
                Console.WriteLine("Client Connected [X]"); //finish this
                checkTemp(cputemp);
                Thread.Sleep(1000); //wait 1 second 
            }

            //end of infinite loop D:
        }
        public static byte[] tData() //Converts readings to byte array
        {
            PC_uptime.NextValue(); //the method will activate all this things and then start the thread that actually prints the info
            pc.Open();
            
                int cputemp = (int)CPU_temp(); //convert float to int
                int cpuload = (int)CPU_load();

                Packet Telemetry = new Packet(Packet.PacketType.Telemetry, "server"); //put the object into a package
                Telemetry.tData.Add(cputemp);
                Telemetry.tData.Add(cpuload);
                byte[] Data = Telemetry.ToBytes();
               
                return Data;
         }
      

      
        public static void checkTemp(int temp)
        {
            if (temp >= 60)
            {
                voice.Speak("WARNING, CPU TEMPERATURE OVER 60 DEGREES!", 2);
            }
        }

        static float CPU_temp() //float with the temperature of the CPU
        {
            float temp = 0;
            try
            {
                #region //foreach thingy things
                foreach (var hardwareItem in pc.Hardware)  //store the hardware in hardwareItem
                {

                    if (hardwareItem.HardwareType == HardwareType.CPU) //Select the CPU of the hardware list
                    {
                        hardwareItem.Update(); //call the update (i don't know what this is actually
                        foreach (var sensorItem in hardwareItem.Sensors) //store every sensor of the CPU on sensorItem
                        {
                            if (sensorItem.SensorType == SensorType.Temperature) //If the sensorItem is a temperature sensor , prints the value OR the default
                            {
                                temp = sensorItem.Value.GetValueOrDefault();
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                temp = 0;
                Console.WriteLine("ERROR READING CPU TEMPERATURE");
                Console.WriteLine("MAKE SURE THE APP HAS ADMIN RIGHTS");
                Console.WriteLine(ex.Message);
            }

            return temp;

        }

        static float CPU_load() //float with the load of the CPU
        {
            float load = 0;
            try
            {
                #region //foreach thingy things
                foreach (var hardwareItem in pc.Hardware)  //store the hardware in hardwareItem
                {

                    if (hardwareItem.HardwareType == HardwareType.CPU) //Select the CPU of the hardware list
                    {
                        hardwareItem.Update(); //call the update (i don't know what this is actually
                        foreach (var sensorItem in hardwareItem.Sensors) //store every sensor of the CPU on sensorItem
                        {
                            if (sensorItem.SensorType == SensorType.Load) //If the sensorItem is a temperature sensor , prints the value OR the default
                            {
                                load = sensorItem.Value.GetValueOrDefault();
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                load = 0;
                Console.WriteLine("ERROR READING CPU LOAD");
                Console.WriteLine("SOMETHING WENT AWFULLY WRONG");
                Console.WriteLine(ex.Message);
            }

            return load;

        }



        public static string UpTime()
        {
            TimeSpan uptimeSpan = TimeSpan.FromSeconds(PC_uptime.NextValue()); // convert uptime to seconds
            string timeawake = string.Format("Up time: {0} days {1}:{2}:{3}", //store uptime formated into a string
                (int)uptimeSpan.TotalDays, //this stores the total of days
                (int)uptimeSpan.Hours,      //this stores the hours since last day
                (int)uptimeSpan.Minutes,    //this stores the minutes passed since last hour
                (int)uptimeSpan.Seconds        // this stores the seconds since last minute
                );

            return timeawake;
        }
    }

    class clientData
    {
        public Socket cs;
        public Thread ct;

        public clientData(Socket cs)
        {
            this.cs = cs;
            ct = new Thread(Server.dataSend);
            ct.Start(cs);
        }

    }

    public class voice
    {
        public static SpeechSynthesizer synth = new SpeechSynthesizer();
        public static void Speak(string message, int rate) //speak the message at the desired speed (rate)
        {
            synth.Volume = 100;
            synth.Rate = rate;
            synth.Speak(message);
        }

    }

   
}