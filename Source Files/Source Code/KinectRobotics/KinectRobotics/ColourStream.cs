using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Colour
{
    class ColourImageStream
    {
        private KinectSensor sensor;

        private WriteableBitmap colorBitmap;

        private byte[] colorPixels;

        public WriteableBitmap Init()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {
                
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, System.Windows.Media.PixelFormats.Bgr32, null);

                this.sensor.ColorFrameReady += this.SensorColorFrameReady;

                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
            if (null == this.sensor)
            {
                MessageBox.Show("No Ready Kinect Found!");
            }

            return colorBitmap;
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }
    }
}
