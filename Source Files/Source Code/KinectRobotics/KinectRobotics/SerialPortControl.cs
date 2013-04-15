using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SerialComm
{
    using SkeletonTrackingData;

    using SwipeDetector;

    using KinectRobotics;

    using Microsoft.Kinect.Toolkit.Controls;

    class SerialPortControl
    {
        SerialPort serialport = new SerialPort();

        public void Init()
        {
            try
            {

           serialport.PortName = "COM5";

           serialport.BaudRate = 9600;

           serialport.Open();

           // MessageBox.Show("Serial port ready");
                
            }

            catch (IOException e)
            {
                //MessageBox.Show("COM port not found");
            }

            DispatcherTimer time = new DispatcherTimer();
            time.Interval = TimeSpan.FromMilliseconds(50.0);
            time.Tick += time_Tick;
            time.Start();
        }

        public void time_Tick(object sender, EventArgs e)
        {
           
            int e_angle = SkeletonTracking.e_angle;

            int w_angle = SkeletonTracking.w_angle;
            
            int push = BaseControl.push;

            string gripStatus = KinectScrollViewer.gripStatus;

            string swept = SwipeDetection.swept;

            Console.WriteLine(MainWindow.modeType);

            if (MainWindow.modeType == "Arm")
            {
                
                if (e_angle > 0 && e_angle <= 70) writeToSerial("A" + (70 - e_angle) + "E");

                if (w_angle > 0 && w_angle <= 70) writeToSerial("B" + w_angle + "E");

                    if (gripStatus == "G") { writeToSerial("G"); KinectScrollViewer.gripStatus = "Z"; }

                    else if (gripStatus == "U") { writeToSerial("U"); KinectScrollViewer.gripStatus = "Z"; }
            }

            else if (MainWindow.modeType == "Base")
            {
                if (push == 1) { writeToSerial("PS"); BaseControl.push = 0; }

                if (swept == "R") { writeToSerial("RS"); SwipeDetection.swept = "Z"; }

                else if (swept == "L") { writeToSerial("LS"); SwipeDetection.swept = "Z"; }
            }

        }
        public void writeToSerial(string msg)
        {
           // serialport.Write(msg);
            //MessageBox.Show(msg);
        }


    }
}
