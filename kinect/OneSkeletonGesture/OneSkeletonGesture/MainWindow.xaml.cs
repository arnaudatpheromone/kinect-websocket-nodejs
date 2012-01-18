using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinect.Toolkit;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using System.Net.Sockets;

namespace OneSkeletonGesture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    #region Class : Jointure - Message - Indice 

    class Jointure
    {
        public string name;
        public string x;
        public string y;
        public string z;

        public Jointure(string name, Joint point)
        {
            var scaledJoint = point.ScaleTo(640, 480, .6f, .6f);
            this.name = name;
            this.x = scaledJoint.Position.X.ToString();
            this.y = scaledJoint.Position.Y.ToString();
            this.z = scaledJoint.Position.Z.ToString();
        }

    }

    class Message
    {

        private string str;
        private Jointure[] arr;
        private int id;

        public Message(string arg, int val, Jointure[] array)
        {
            str = arg;
            id = val;
            arr = array;
        }

        public void Initialize(string arg, int val)
        {
            str = arg;
            id = val;
        }

        public int GetID()
        {
            return id;
        }

        public string GetMessage()
        {
            return str;
        }


        public Jointure[] GetBody()
        {
            return arr;
        }

    }

    class Indice
    {

        public string _id;
        public uint _value = 0;
        public const uint max = 10;
        public bool destroy = false;

        public Indice(string id)
        {
            _id = id;
        }

        public string GetID()
        {
            return _id;
        }

        public uint GetIndice()
        {
            return _value;
        }

        public void SetIndice()
        {
            _value++;
        }

    }

    #endregion


    public partial class MainWindow : Window
    {

        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();

        
        Runtime nui = new Runtime();

        private uint loop = 0;
        private Canvas oCanvas;
        private TextBox txt;
        private List<Indice> list = new List<Indice>();
        private Jointure[] body;

        private const int SENSIBILITY = 45;

        private Point OL = new Point(0,0);
        private Point OR = new Point(0,0);
        private Point NL = new Point(0,0);
        private Point NR = new Point(0,0);


        public MainWindow()
        {
            
             #region Canvas & Screen
 
            oCanvas = new Canvas();
            txt = new TextBox();
            txt.Name = "txt_name";
            txt.Width = 525;
            txt.Height = 350;
            txt.Text = "kinect_sdk : ";
            txt.IsReadOnly = true;
            txt.TextWrapping = TextWrapping.Wrap;
            txt.AcceptsReturn = true;
            txt.BorderBrush = Brushes.Transparent;

            InitializeComponent();

            Canvas.SetLeft(txt, 0);
            Canvas.SetTop(txt, 0);

            oCanvas.Name = "CNV_00";
            oCanvas.Width = 50;
            oCanvas.Height = 50;
            oCanvas.SnapsToDevicePixels = true;
            oCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            oCanvas.VerticalAlignment = VerticalAlignment.Top;
            oCanvas.Children.Add(txt);

            this.Content = oCanvas;
           
            #endregion

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            clientSocket.Connect("127.0.0.1", 8124);
            
            //Initialize to do skeletal tracking
            nui.Initialize(RuntimeOptions.UseSkeletalTracking);

            #region TransformSmooth
            //Must set to true and set after call to Initialize
            nui.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };

            nui.SkeletonEngine.SmoothParameters = parameters;

            #endregion

            //add event to receive skeleton data
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);

        }

      

        private void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            SkeletonFrame skeletonFrame = e.SkeletonFrame;

            uint skel = 0;
            Message[] skeletons = new Message[2];

            int iSkeleton = 0;

            // skeleton.Children.Clear();

            foreach (SkeletonData data in skeletonFrame.Skeletons)
            {

                if (SkeletonTrackingState.Tracked == data.TrackingState)
                {

                    Jointure HipCenter = new Jointure("hipcenter", data.Joints[JointID.HipCenter]);
                    Jointure Spine = new Jointure("spine", data.Joints[JointID.Spine]);
                    Jointure ShoulderCenter = new Jointure("shouldercenter", data.Joints[JointID.ShoulderCenter]);
                    Jointure Head = new Jointure("head", data.Joints[JointID.Head]);
                    Jointure ShoulderLeft = new Jointure("shoulderleft", data.Joints[JointID.ShoulderLeft]);
                    Jointure ElbowLeft = new Jointure("elbowleft", data.Joints[JointID.ElbowLeft]);
                    Jointure WristLeft = new Jointure("wristleft", data.Joints[JointID.WristLeft]);
                    Jointure HandLeft = new Jointure("handleft", data.Joints[JointID.HandLeft]);
                    Jointure ShoulderRight = new Jointure("shoulderight", data.Joints[JointID.ShoulderRight]);
                    Jointure ElbowRight = new Jointure("elbowright", data.Joints[JointID.ElbowRight]);
                    Jointure WristRight = new Jointure("wristright", data.Joints[JointID.WristRight]);
                    Jointure HandRight = new Jointure("handright", data.Joints[JointID.HandRight]);
                    Jointure HipLeft = new Jointure("hipleft", data.Joints[JointID.HipLeft]);
                    Jointure KneeLeft = new Jointure("kneeleft", data.Joints[JointID.KneeLeft]);
                    Jointure AnkleLeft = new Jointure("ankleleft", data.Joints[JointID.AnkleLeft]);
                    Jointure FootLeft = new Jointure("footleft", data.Joints[JointID.FootLeft]);
                    Jointure HipRight = new Jointure("hipright", data.Joints[JointID.HipRight]);
                    Jointure KneeRight = new Jointure("kneeright", data.Joints[JointID.KneeRight]);
                    Jointure AnkleRight = new Jointure("ankleright", data.Joints[JointID.AnkleRight]);
                    Jointure FootRight = new Jointure("footright", data.Joints[JointID.FootRight]);

                    Jointure[] array = new Jointure[20];
                    array[0] = HipCenter;
                    array[1] = Spine;
                    array[2] = Head;
                    array[3] = ShoulderLeft;
                    array[4] = ElbowLeft;
                    array[5] = WristLeft;
                    array[6] = HandLeft;
                    array[7] = ShoulderCenter;
                    array[8] = ShoulderRight;
                    array[9] = ElbowRight;
                    array[10] = WristRight;
                    array[11] = HandRight;
                    array[12] = HipLeft;
                    array[13] = KneeLeft;
                    array[14] = AnkleLeft;
                    array[15] = FootLeft;
                    array[16] = HipRight;
                    array[17] = KneeRight;
                    array[18] = AnkleRight;
                    array[19] = FootRight;

                    string message = "";
                    // Message info = new Message(message, 0);

                    for (int i = 0; i < array.Length; i++)
                    {
                        message += "joint/";
                        message += array[i].name.ToString();
                        message += "/";
                        message += array[i].x.ToString();
                        message += "/";
                        message += array[i].y.ToString();
                        message += "/";
                        message += array[i].z.ToString();
                    }

                    Message info = new Message(message, data.TrackingID, array);
                    // info.Initialize(message, data.TrackingID);

                    skeletons[skel] = info;
                    skel++;

                } // if SkeletonTrackingState.Tracked == data.TrackingState 

                iSkeleton++;

            } // for each skeleton


            uint loop = 0;
            Int32[] temp = new Int32[2];

            foreach (Message i in skeletons)
            {
                if (i == null)
                {
                    temp[loop] = 0;
                }
                else
                {
                    temp[loop] = i.GetID();
                }

                loop++;
            }


            uint index;

            #region if : dispatchSkeleton
            if (temp[0] == 0)
            {
                index = 1;
                dispatchSkeleton(skeletons[index]);
                return;
            }

            if (temp[1] == 0)
            {
                index = 0;
                dispatchSkeleton(skeletons[index]);
                return;
            }

            if (temp[0] < temp[1])
            {
                index = 0;
                dispatchSkeleton(skeletons[index]);
                return;
            }

            if (temp[1] < temp[0])
            {
                index = 1;
                dispatchSkeleton(skeletons[index]);
                return;
            }
            #endregion

        }

        private void dispatchSkeleton(Message skel)
        {

            string gesture = "null";

            OL = NL;
            OR = NR;
            Jointure[] body = skel.GetBody();
            NL = new Point(Double.Parse(body[6].x), Double.Parse(body[6].y));  
            NR = new Point(Double.Parse(body[11].x), Double.Parse(body[11].y));
         
            // + ---------------------------------------------------------------
            // . GESTURE RECOGNITION .
            // + ---------------------------------------------------------------
     
            Point p1 = new Point(NL.X, NL.Y);
            Point p2 = new Point(OL.X, NL.Y);
            
            int dl = getDistance(p1, p2);
            if (dl > SENSIBILITY && NL.X > OL.X) { gesture = "left"; /*dispatchGesture("left");*/ }  

            Point p3 = new Point(NR.X, NR.Y);
            Point p4 = new Point(OR.X, NR.Y);

            int dr = getDistance(p3, p4);
            if (dr > SENSIBILITY && NR.X < OR.X) { gesture = "right"; /*dispatchGesture("right");*/ }

            // if (!b) return;

            // + ---------------------------------------------------------------
            // . SKELETON .
            // + ---------------------------------------------------------------

            string message = "";
            message += loop.ToString();
            message += "/prop/";
            message += skel.GetID().ToString();
            message += "/prop/";
            message += gesture;
            message += "/prop/";
            message += skel.GetMessage();

            loop += 1;

            // Console.Write(message); 
            
            NetworkStream serverStream = clientSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
           
        }

        private void dispatchGesture(string arg)
        {
            // Console.Write("gesture/"+arg);
            txt.Text = arg;
        }

        private int getDistance(Point p1, Point p2)
        {
            int dist; 
            double dx; 
            double dy;

			dx = p2.X-p1.X;
			dy = p2.Y-p1.Y;
            
            dist = (int)Math.Sqrt(dx * dx + dy * dy);
    		return dist;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }


    }
}