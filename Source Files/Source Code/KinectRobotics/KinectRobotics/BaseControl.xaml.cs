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
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;
namespace KinectRobotics
{
    using SwipeDetector;

    using System.Windows.Threading;

    using SkeletonTrackingData;

 
    public partial class BaseControl : UserControl
    {

        SwipeDetection swipe = new SwipeDetection();

        public static int push = 0;


        public BaseControl()
        {
           

            InitializeComponent();
            
            DispatcherTimer time = new DispatcherTimer();
            time.Interval = TimeSpan.FromMilliseconds(50.0);
            time.Tick += time_Tick;
            time.Start();
           
        }


        void time_Tick(object sender, EventArgs e)
        {
            if (SkeletonTracking.mode == 1)
            {
                SkeletonTracking.mode = 0;

                MainWindow.modeType = "Main";
                try
                {
                    var pa = (Grid)this.Parent;
                    pa.Children.Remove(this);
                }
                catch (Exception e12)
                {
                    Console.Write("Error sya");
                }
            }
        }
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {
            push = 1;
        }

        


    }
}
