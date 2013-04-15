using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Windows;

namespace SerialComm
{
    class SerialPortControl
    {
        SerialPort serialport = new SerialPort();
    

        public void writeToSerial(string msg)
        {
            serialport.Write(msg);
        }


    }
}
