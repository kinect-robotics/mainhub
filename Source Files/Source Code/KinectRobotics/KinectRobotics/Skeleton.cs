using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows.Media;
using System.IO.Ports;
using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using Microsoft.Samples.Kinect.SwipeGestureRecognizer;

namespace SkeletonTrackingData
{
    using KinectRobotics;

    class SkeletonTracking
    {

        public static int mode = 0;
        
        public static int e_angle = 0;

        public static int w_angle = 0;

        public const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        public const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        public const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        public const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        public const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        public readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        public readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        public readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        public readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        public readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        public DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        public DrawingImage imageSource;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.

        public void InitializeSkeleton()
        {

            TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 1.3f;
                smoothingParam.Correction = 0.7f;
                smoothingParam.Prediction = 0.7f;
                smoothingParam.JitterRadius = 0.12f;
                smoothingParam.MaxDeviationRadius = 0.08f;
            };

            //Console.Clear();

        }

        void StartKinectSTWithSmoothing()
        {

            //this.sensor = KinectSensor.KinectSensors.Equals(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated; // Use Seated Mode

            TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
            {
                smoothingParam.Smoothing = 1.3f;
                smoothingParam.Correction = 0.7f;
                smoothingParam.Prediction = 0.7f;
                smoothingParam.JitterRadius = 0.12f;
                smoothingParam.MaxDeviationRadius = 0.08f;
            };

            this.sensor.SkeletonStream.Enable(smoothingParam); // Enable skeletal tracking

            this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SensorSkeletonFrameReady); // Get Ready for Skeleton Ready Events


        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SkeletonLoading()
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            //Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit
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
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }

        public void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
                
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));

               
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            /*this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);*/

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }

            ToGetTheAngleBetweenJoints(skeleton);
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        public Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        public void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));

        }

        public void ToGetTheAngleBetweenJoints(Skeleton skeleton)
        {

            // Defining all the joints between which angle must be calculated

            Joint r_wrist = skeleton.Joints[JointType.WristRight];

            Joint r_shoulder = skeleton.Joints[JointType.ShoulderRight];

            Joint r_elbow = skeleton.Joints[JointType.ElbowRight];

            Joint r_hip = skeleton.Joints[JointType.HipRight];

            Joint r_hand = skeleton.Joints[JointType.HandRight];


            Joint l_wrist = skeleton.Joints[JointType.WristLeft];

            Joint l_shoulder = skeleton.Joints[JointType.ShoulderLeft];

            Joint l_elbow = skeleton.Joints[JointType.ElbowLeft];

            Joint l_hand = skeleton.Joints[JointType.HandLeft];

            Joint head = skeleton.Joints[JointType.Head];

            // Getting the (X,Y,Z) positions of the joints

            double rs_x = r_shoulder.Position.X;
            double rs_y = r_shoulder.Position.Y;
            double rs_z = r_shoulder.Position.Z;

            double re_x = r_elbow.Position.X;
            double re_y = r_elbow.Position.Y;
            double re_z = r_elbow.Position.Z;

            double rw_x = r_wrist.Position.X;
            double rw_y = r_wrist.Position.Y;
            double rw_z = r_wrist.Position.Z;

            double rh_x = r_hip.Position.X;
            double rh_y = r_hip.Position.Y;
            double rh_z = r_hip.Position.Z;

            double rha_x = r_hand.Position.X;
            double rha_y = r_hand.Position.Y;
            double rha_z = r_hand.Position.Z;

            double ls_x = l_shoulder.Position.X;
            double ls_y = l_shoulder.Position.Y;
            double ls_z = l_shoulder.Position.Z;

            double le_x = l_elbow.Position.X;
            double le_y = l_elbow.Position.Y;
            double le_z = l_elbow.Position.Z;

            double lw_x = l_wrist.Position.X;
            double lw_y = l_wrist.Position.Y;
            double lw_z = l_wrist.Position.Z;

            // Drawing vectors

            if (l_hand.Position.Y > head.Position.Y)
            {
                mode = 1;
            }

            Vector3D re_v = new Vector3D(re_x, re_y, re_z);
            Vector3D rs_v = new Vector3D(rs_x, rs_y, rs_z);
            Vector3D rw_v = new Vector3D(rw_x, rw_y, rw_z);

            Vector3D rs_v2 = new Vector3D(rs_x, rs_y, rs_z);
            Vector3D rh_v = new Vector3D(rh_x, rh_y, rh_z);
            Vector3D re_v2 = new Vector3D(re_x, re_y, re_z);

            //Vector3D rs_v2 = new Vector3D(rw_x, rw_y, rw_z);
            //Vector3D rh_v = new Vector3D(re_x, re_y, re_z);
            Vector3D rha_v2 = new Vector3D(rha_x, rha_y, rha_z);

            Vector3D le_v = new Vector3D(le_x, le_y, le_z);
            Vector3D ls_v = new Vector3D(ls_x, ls_y, ls_z);
            Vector3D lw_v = new Vector3D(lw_x, lw_y, lw_z);

            Vector3D rew_b = rw_v - re_v;
            Vector3D res_b = rs_v - re_v;

            Vector3D rsh_b = rh_v - rs_v2;
            Vector3D rse_b = re_v2 - rs_v2;

            Vector3D les_b = ls_v - le_v;
            Vector3D lew_b = lw_v - le_v;

            Vector3D rhw_b = rha_v2 - rs_v2;
            Vector3D rew1_b = rh_v - rs_v2;

            Vector3D lse_b = le_v - ls_v;
            Vector3D ss_b = rs_v - ls_v;

            lse_b.Normalize();
            ss_b.Normalize();
            // Normalise all the vectors

            rew_b.Normalize();
            res_b.Normalize();

            rsh_b.Normalize();
            rse_b.Normalize();

            les_b.Normalize();
            lew_b.Normalize();

            rhw_b.Normalize();
            rew1_b.Normalize();

            double AngleInRadiansForElbowMovement = AngleBetweenTwoVectors(rsh_b, rse_b);
            double AngleInDegForElbowMovement = AngleInRadiansForElbowMovement * (180 / Math.PI);

            e_angle = (int)AngleInDegForElbowMovement;


            double AngleInRadiansForWristMovement = AngleBetweenTwoVectors(rhw_b, rew1_b);
            double AngleInDegForWristMovement = AngleInRadiansForWristMovement * (180 / Math.PI);


            w_angle = (int)AngleInDegForWristMovement;
          
        }

        public float AngleBetweenTwoVectors(Vector3D vectorA, Vector3D vectorB)
        {

            double dotProduct = 0.0f;

            dotProduct = Vector3D.DotProduct(vectorA, vectorB);

            return (float)Math.Acos(dotProduct);

        }
    }
}
