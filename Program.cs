using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Speech.Synthesis; // this is for the computer to speak

namespace PHSt
{
    class Program
    {
        public static PerformanceCounter PC_uptime = new PerformanceCounter("System", "System Up Time"); //uptime from the Performance Counter of windows
        public static SpeechSynthesizer synth = new SpeechSynthesizer();
        public static Computer pc = new Computer() //Creating the computer object for readings
        {
            CPUEnabled = true, //initialize the CPU
            GPUEnabled = true //initialize the GPU
        };
        static void Main(string[] args)
        {
            Console.WriteLine("Loading...");
            PC_uptime.NextValue();
            synth.Volume = 100;
            Speak("version 0.5", 2);
            
            pc.Open(); //open the pc (?)
           // Replace or fix this loop to integrate it to a menu
            while (true) //infinite while loop for temp readings
            {
                //readCPUTemperature();
                Console.Clear();
                int cputemp = (int)CPU_temp(); //convert float to int
                int cpuload = (int)CPU_load();
                Console.WriteLine(UpTime()); //prints the uptime of the system
                Console.WriteLine("-----------CPU-----------");
                Console.WriteLine("Temp: " + cputemp + "C°"); //prints the cpu temperature
                Console.WriteLine("Load: " + cpuload + "%"); //prints the cpu load
                checkTemp(cputemp);
                Thread.Sleep(1000); //wait 1 second 
            } //end of infinite loop D:
           
            endOfTest();
            

        }

        static void endOfTest()
        {
            Console.WriteLine("Testing procedure ended");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

            static void readCPUTemperature()//too many checks maybe?    //im not using this anymore
        {
            foreach (var hardwareItem in pc.Hardware)  //store the hardware in hardwareItem
            {

                if (hardwareItem.HardwareType == HardwareType.CPU) //Select the CPU of the hardware list
                {
                    hardwareItem.Update(); //call the update (i don't know what this is actually
                    foreach (var sensorItem in hardwareItem.Sensors) //store every sensor of the CPU on sensorItem
                    {
                        if (sensorItem.SensorType == SensorType.Temperature) //If the sensorItem is a temperature sensor , prints the value OR the default
                        {
                            Console.Clear();
                            Console.WriteLine("CPU Temperature : " + sensorItem.Value.GetValueOrDefault() + "C°");
                        }
                    }
                }
            }
        }
        //Code a CPU class?
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

        static string UpTime()
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

        static void checkTemp(int temp)
        {
            if (temp >= 60)
            {
                Speak("WARNING, CPU TEMPERATURE OVER 60 DEGREES!",  2);
            }
        }

        public static void Speak(string message, int rate) //speak the message at the desired speed (rate)
        {
            synth.Rate = rate;
            synth.Speak(message);
        }
    }
}
