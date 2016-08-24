using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ServerData;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; //a lot of references
using System.Net;
namespace DataThing
{
    [Serializable] // this enables the serialization via binary formatter
    public class Packet
    {
        //public List<string> pData;
        public List<int> tData;
        public string pID;
        public PacketType pType;

        public Packet(PacketType type, string pID) //primary constructor, this is basically the initialization pre-serialization
        {
            //pData = new List<string>(); // this list contains the data
            tData = new List<int>(); //this contains the telemetry ints
            this.pID = pID; //this is the packet ID
            this.pType = type; //and this is the package type (enum)
            
        }

        public Packet(byte[] packetbytes) //rearms the package from byte array
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetbytes);

            Packet p = (Packet)bf.Deserialize(ms); //this makes the new package with the data from the firts package
            ms.Close(); //always close the memory streams 
            //Initialize the object with the data from the original packet
            //this.pData = p.pData;
            this.tData = p.tData;
            this.pID = p.pID;
            this.pType = p.pType;
        }

        //method for converting the package to a byte[] by serialization
        public byte[] ToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this); //serializes all the object
            byte[] bytes = ms.ToArray();
            ms.Close(); //close the memory stream
            return bytes;

        }

        //enum for the type of package
        public enum PacketType
        {
            Telemetry, //used for telemetry
            Text, //this is for sending alerts or something like that
            Audio //audio for alerts... maybe?
        }

        public enum AudioType
        {
            Speech,

        }

        public static string getIP() //method that returns the string
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress i in ips)
            {
                if (i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return i.ToString();
                }


            }
            return "127.0.0.1";
        }

        }


}
