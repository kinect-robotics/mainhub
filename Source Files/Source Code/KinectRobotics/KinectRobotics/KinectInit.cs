using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;

namespace KinectRobotics
{
    class KinectInit
    {
        private KinectSensor sensor;

        public void initialize()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }

                else
                {
                    MessageBox.Show("Kinect not initialized!");
                }
            }
        }
    }
}
