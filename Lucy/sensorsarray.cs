using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace Lucy
{

    
    class sensorsarray
    {

        public static SerialPort port = new SerialPort("COM31", 115200, Parity.None, 8, StopBits.One);
        byte positions = 0;



    }
}
