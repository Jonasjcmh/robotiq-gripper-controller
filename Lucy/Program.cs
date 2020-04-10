using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;


namespace CRC
{
    class Program 
    {




        public static SerialPort port = new SerialPort("COM32", 115200, Parity.None, 8, StopBits.One);

       
        byte positions=0;

        static void Main(string[] args)
        {


            port.Open();

            byte position = 100;
            byte speed = 100;
            byte force = 100;

            int line;
            byte lines;
            int[] forcese = new int[6];
            byte[] temporal = new byte[2];
            byte[] values = new byte[9];
            string message;

            port.Write(new byte[] { 0x09, 0x10, 0x03, 0xE8, 0x00, 0x03, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x73, 0x30 }, 0, 15);  // activate
            System.Threading.Thread.Sleep(1000);
            port.Write(new byte[] { 0x09, 0x10, 0x03, 0xE8, 0x00, 0x03, 0x06, 0x09, 0x00, 0x00, 0xff, 0xff, 0xff, 0x42, 0x29 }, 0, 15);  //close
            System.Threading.Thread.Sleep(1000);
            port.Write(new byte[] { 0x09, 0x10, 0x03, 0xE8, 0x00, 0x03, 0x06, 0x09, 0x00, 0x00, 0x00, 0xff, 0xff, 0x72, 0x19 }, 0, 15);  // open
            System.Threading.Thread.Sleep(1000);


            controlgripper(position, speed, force);

            byte positions = 10;
            int i = 10;

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                if (Console.ReadKey().Key == ConsoleKey.UpArrow)
                {
                    if ((int)positions < 250)
                        positions = (byte)((
                            int)positions + i);
                    else
                        positions=250;
                }
                else if (Console.ReadKey().Key == ConsoleKey.DownArrow)
                {
                    if ((int)positions > 10)
                        positions = (byte)((int)positions - i);
                    else
                        positions = 5;
                }
                byte speeds = 50;
                byte forces = 100;

                controlgripper(positions, speeds, forces);
                System.Threading.Thread.Sleep(10);

                port.DiscardInBuffer();
                port.Write(new byte[] { 0x09, 0x03, 0x07, 0xD0, 0x00, 0x03, 0x04, 0x0E }, 0, 8);  // open
                System.Threading.Thread.Sleep(20);
                Console.WriteLine("Reading");
                message = port.ReadExisting();
                if (message.Length >= 19)
                    {
                    char[] chars = message.ToCharArray();
                    Console.WriteLine((int)chars[8]);
                    Console.WriteLine((int)chars[9]);
                    Console.WriteLine((int)chars[10]);
                    Console.WriteLine((int)chars[11]);
                    Console.WriteLine((int)chars[12]);
                    Console.WriteLine((int)chars[13]);
                    Console.WriteLine((int)chars[14]);
                    Console.WriteLine((int)chars[15]);
                    Console.WriteLine((int)chars[16]);
                }
                
                System.Threading.Thread.Sleep(10);
                /*/
                line = port.ReadByte();
                Console.WriteLine(line);
                if (line == 09)
                {
                    line = port.ReadByte();
                   
                    if (line == 03)
                    {
                        
                        for (int j = 0; j <= 8; j++)
                        {
                            line = port.ReadByte();
                            lines = (byte)line;
                            
                            values[j] = lines;


                        }
                        Console.Write("Position");
                        Console.WriteLine(values[5]);
                        Console.Write("Current");
                        Console.WriteLine(values[6]);
                    }

                }
                /*/

            }

        }

        public static void controlgripper(byte position, byte speed, byte force)
        {
            byte[] beggining = { 0x09, 0x10, 0x03, 0xE8, 0x00, 0x03, 0x06, 0x09, 0x00, 0x00, position, speed, force };
            int i = 0;


            CrcStdParams.StandartParameters.TryGetValue(CrcAlgorithms.Crc16Modbus, out Parameters crc_p);
            Crc crc = new Crc(crc_p);
            crc.Initialize();
            var crc_bytes = crc.ComputeHash(beggining);
            Console.WriteLine(crc_bytes);

            byte low = crc_bytes[0];
            byte high = crc_bytes[1];
            byte[] crcend = { high, low };

            byte[] b = { 0x09, 0x10, 0x03, 0xE8, 0x00, 0x03, 0x06, 0x09, 0x00, 0x00, position, speed, force, low, high };

            port.Write(b, 0, 15);  // position desided

            Console.WriteLine(low);
            Console.WriteLine(high);
            string c = ByteArrayToString(b);
            Console.WriteLine("The String 2 is: " + c);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }


        public byte[] CalculateCrc16Modbus(byte[] bytes)
        {
            CrcStdParams.StandartParameters.TryGetValue(CrcAlgorithms.Crc16Modbus, out Parameters crc_p);
            Crc crc = new Crc(crc_p);
            crc.Initialize();
            byte[] crc_bytes = crc.ComputeHash(bytes);
            return crc_bytes;
        }


    }
}
