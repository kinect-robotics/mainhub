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
using System.Windows.Markup;  

namespace KinectRobotics
{
    using Colour;
    using SkeletonTrackingData;
    using System.IO;
    using System.Windows.Threading;

    public partial class ArmControl : UserControl
    {
       

        public ArmControl()
        {
            InitializeComponent();

            ColourImageStream init1 = new ColourImageStream();

            WriteableBitmap RGBImage = init1.Init();

            ColorImageStream.Source = RGBImage;

            DispatcherTimer time = new DispatcherTimer();
            time.Interval = TimeSpan.FromMilliseconds(50.0);
            time.Tick += time_Tick;
            time.Start();
        }

        void time_Tick(object sender, EventArgs e)
        {
            ElbowAngle.Content = SkeletonTracking.e_angle;

            WristAngle.Content = SkeletonTracking.w_angle;

            if (SkeletonTracking.mode == 1)
            {
                MainWindow.modeType = "Main";
                SkeletonTracking.mode = 0;
                try
                {
                    var pa = (Grid)this.Parent;
                    pa.Children.Remove(this);
                }
                catch (Exception e12)
                {
                    //Console.Write("Error");
                }
            }
        }
    }
}
