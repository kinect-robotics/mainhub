using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Properties;

namespace KinectRobotics
{

    using SerialComm;

    using SkeletonTrackingData;

    public partial class MainWindow : Window
    {

        private readonly KinectSensorChooser sensorChooser;

        SerialPortControl serial = new SerialPortControl();

        public static string modeType = "Main";

        public MainWindow()
        {
            
            InitializeComponent();   

            KinectInit init = new KinectInit();

            init.initialize();

            serial.Init();

            this.sensorChooser = new KinectSensorChooser(); 

            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;

            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;

            this.sensorChooser.Start();

            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };

            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            SkeletonTracking trackAngle = new SkeletonTracking();

            trackAngle.InitializeSkeleton();

            trackAngle.SkeletonLoading();

        }

        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (KinectTileButton)e.OriginalSource;
            //MessageBox.Show(button.Label.ToString());
            if(button.Label.ToString() == "1")
            {
               modeType = "Arm";
                var armControl = new ArmControl();
                this.kinectRegionGrid.Children.Add(armControl);
                e.Handled = true;
            }

            else if (button.Label.ToString() == "2")
            {
                modeType = "Base";
                var baseControl = new BaseControl();
                this.kinectRegionGrid.Children.Add(baseControl);
                e.Handled = true;
            }
        }

        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }
    }
}
