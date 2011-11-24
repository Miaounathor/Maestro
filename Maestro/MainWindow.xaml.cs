﻿using System;
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
using Microsoft.Research.Kinect.Nui;


namespace Maestro
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Game_Engine : Window
    {
        //Kinect attributes
        #region Kinect System

        //!Kinect UI
        Runtime nui;

        // We want to control how depth data gets converted into false-color data
        // for more intuitive visualization, so we keep 32-bit color frame buffer versions of
        // these, to be updated whenever we receive and process a 16-bit frame.
        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;
        byte[] depthFrame32 = new byte[320 * 240 * 4];

        //!Depth frame
        private byte[] convertedDepthFrame;

        //!Joint colors
        Dictionary<JointID, Brush> jointColors = new Dictionary<JointID, Brush>() { 
            {JointID.HipCenter, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.Spine, new SolidColorBrush(Color.FromRgb(169, 176, 155))},
            {JointID.ShoulderCenter, new SolidColorBrush(Color.FromRgb(168, 230, 29))},
            {JointID.Head, new SolidColorBrush(Color.FromRgb(200, 0,   0))},
            {JointID.ShoulderLeft, new SolidColorBrush(Color.FromRgb(79,  84,  33))},
            {JointID.ElbowLeft, new SolidColorBrush(Color.FromRgb(84,  33,  42))},
            {JointID.WristLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HandLeft, new SolidColorBrush(Color.FromRgb(215,  86, 0))},
            {JointID.ShoulderRight, new SolidColorBrush(Color.FromRgb(33,  79,  84))},
            {JointID.ElbowRight, new SolidColorBrush(Color.FromRgb(33,  33,  84))},
            {JointID.WristRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.HandRight, new SolidColorBrush(Color.FromRgb(37,   69, 243))},
            {JointID.HipLeft, new SolidColorBrush(Color.FromRgb(77,  109, 243))},
            {JointID.KneeLeft, new SolidColorBrush(Color.FromRgb(69,  33,  84))},
            {JointID.AnkleLeft, new SolidColorBrush(Color.FromRgb(229, 170, 122))},
            {JointID.FootLeft, new SolidColorBrush(Color.FromRgb(255, 126, 0))},
            {JointID.HipRight, new SolidColorBrush(Color.FromRgb(181, 165, 213))},
            {JointID.KneeRight, new SolidColorBrush(Color.FromRgb(71, 222,  76))},
            {JointID.AnkleRight, new SolidColorBrush(Color.FromRgb(245, 228, 156))},
            {JointID.FootRight, new SolidColorBrush(Color.FromRgb(77,  109, 243))}
        };
        #endregion

        #region Game Attributes

        

       

        //!Current difficulty
        public Difficulty _difficulty{get;set;}

        //!Display engine
        private DisplayEngine hudDisplay;

        //!Current profile
        //public Profile _profile { get; set; }
        
        //!Game Judge
       // public Judge _judge { get; set; }

        //!Text list of songs
        public List<String> _listOfSongs { get; set; }
        
        //!Song player
        //public SongPlayer _songPlayer { get; set; }

        //!Score
        public int Score {get;set;}

        //!Current screen
        public Screen currentScreen { get; set; }

        //!Hands positions
        public int rightHandPosition { get; set; }
        public Point rightHandPoint { get; set; }
        public int rightHandPreviousDepht { get; set; }

        public int leftHandPosition { get; set; }
        public Point leftHandPoint { get; set; }
        public int leftHandPreviousDepth { get; set; }

        //!Feet position
        public int rightFootPosition { get; set; }
        public Point rightFootPoint { get; set; }
        public int leftFootPosition { get; set; }
        public Point leftFootPoint { get; set; }

        #endregion

                #region Display Attributes
        private DisplayEngine displayHUD { get; set; }
        private ActionDisplay displaySteps { get; set; }
        #endregion

        private Parser dataParser { get; set; }

        public Game_Engine()
        {
            InitializeComponent();
            run();

        }

        //!Run the game engine
        public void run()
        { 
            hudDisplay = new DisplayEngine(GameScreen);

            _difficulty = Difficulty.Easy;
            //_profile = new Profile();
           // _judge = new Judge();

            //Set up the bachground
            ImageBrush main = new ImageBrush();
            main.ImageSource = new BitmapImage(
                    new Uri("images\\screen_main.png", UriKind.Relative));

            GameScreen.Background = main;
            
            currentScreen = Screen.Main;

            hudDisplay.GenerateGrid();
        }

        
        //!Play the song
        public void start_game()
        {
            //Run the song

            //Load the steps
        }

        //!Update display data
        public void refresh()
        {
            //Check the points and calculate the score

            //Update the action display


            //Update the HUD display
            //hudDisplay.updateScreen(leftHandPosition, rightHandPosition, leftFootPosition, rightFootPosition);
           
            if (leftHandPosition != 0 && rightHandPosition != 0 && Math.Sqrt(Math.Pow((leftHandPoint.X - rightHandPoint.X), 2) + Math.Pow((leftHandPoint.Y - rightHandPoint.Y), 2)) < 30)
            {
                hudDisplay.clap(leftHandPosition, leftFootPosition, rightFootPosition);
            }
        }

        //!Exit current song
        public void quitSong()
        {

        }

        //!Windows loading complete
       private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

           //Run the Kinect UI
            nui = new Runtime();
        
            //Initialize the skeletton
            try
            {
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }

            try
            {
                nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Failed to open stream. Please make sure to specify a supported image type and resolution.");
                return;
            }

            nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_ColorFrameReady);
        }

       void nui_ColorFrameReady(object sender, ImageFrameReadyEventArgs e)
       {
           // 32-bit per pixel, RGBA image
           PlanarImage Image = e.ImageFrame.Image;
            video.Source = BitmapSource.Create(
               Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, Image.Bits, Image.Width * Image.BytesPerPixel);
       }

        //!Display skeleton
       void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
       {
           SkeletonFrame skeletonFrame = e.SkeletonFrame;
           int iSkeleton = 0;
           Brush[] brushes = new Brush[6];
           brushes[0] = new SolidColorBrush(Color.FromRgb(255, 0, 0));
           brushes[1] = new SolidColorBrush(Color.FromRgb(0, 255, 0));
           brushes[2] = new SolidColorBrush(Color.FromRgb(64, 255, 255));
           brushes[3] = new SolidColorBrush(Color.FromRgb(255, 255, 64));
           brushes[4] = new SolidColorBrush(Color.FromRgb(255, 64, 255));
           brushes[5] = new SolidColorBrush(Color.FromRgb(128, 128, 255));

           GameScreen.Children.Clear();

           foreach (SkeletonData data in skeletonFrame.Skeletons)
           {
               if (SkeletonTrackingState.Tracked == data.TrackingState)
               {
                   // Draw bones
                   Brush brush = brushes[iSkeleton % brushes.Length];
                   GameScreen.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.Spine, JointID.ShoulderCenter, JointID.Head));
                   GameScreen.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ElbowLeft, JointID.WristLeft, JointID.HandLeft));
                   GameScreen.Children.Add(getBodySegment(data.Joints, brush, JointID.ShoulderCenter, JointID.ShoulderRight, JointID.ElbowRight, JointID.WristRight, JointID.HandRight));
                   GameScreen.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipLeft, JointID.KneeLeft, JointID.AnkleLeft, JointID.FootLeft));
                   GameScreen.Children.Add(getBodySegment(data.Joints, brush, JointID.HipCenter, JointID.HipRight, JointID.KneeRight, JointID.AnkleRight, JointID.FootRight));
                   

                   //Get hand position
                   GameScreen.Children.Add(getBodyPoint(data.Joints, Brushes.Yellow, JointID.HandLeft));
                  GameScreen.Children.Add(getBodyPoint(data.Joints, Brushes.Tomato, JointID.HandRight));
                  GameScreen.Children.Add(getBodyPoint(data.Joints, Brushes.SteelBlue, JointID.FootLeft));
                  GameScreen.Children.Add(getBodyPoint(data.Joints, Brushes.PaleGreen, JointID.FootRight));

                   
                   // Draw joints
                   foreach (Joint joint in data.Joints)
                   {
                       Point jointPos = getDisplayPosition(joint);
                       Line jointLine = new Line();
                       jointLine.X1 = jointPos.X - 3;
                       jointLine.X2 = jointLine.X1 + 6;
                       jointLine.Y1 = jointLine.Y2 = jointPos.Y;
                       jointLine.Stroke = jointColors[joint.ID];
                       jointLine.StrokeThickness = 6;
                       GameScreen.Children.Add(jointLine);
                   }


               }
               iSkeleton++;
           } // for each skeleton

           hudDisplay.GenerateGrid();
           refresh();

       }
        
        //Get the position of an element
        int getElementPosition(Point p){

            //First column
            if (p.X < hudDisplay.columnSpace)
            {
                //First case
                if (p.Y < hudDisplay.rowSpace)
                {
                    return 0;
                }
                    //Second case
                else if (p.Y < 2 * hudDisplay.rowSpace)
                {
                    return 3;
                }
                //Third case
                else
                {
                    return 6;
                }
            }
            //Second column
            else if (p.X < 2 * hudDisplay.columnSpace)
            {
                //First case
                if (p.Y < hudDisplay.rowSpace)
                {
                    return 1;
                }
                //Second case
                else if (p.Y < 2 * hudDisplay.rowSpace)
                {
                    return 4;
                }
                //Third case
                else
                {
                    return 7;
                }
            }
            //Last column
            else
            {
                //First case
                if (p.Y < hudDisplay.rowSpace)
                {
                    return 2;
                }
                //Second case
                else if (p.Y < 2 * hudDisplay.rowSpace)
                {
                    return 5;
                }
                //Third case
                else
                {
                    return 8;
                }
            }
        }

       #region Display Data

        // Converts a 16-bit grayscale depth frame which includes player indexes into a 32-bit frame
        // that displays different players in different colors
        byte[] convertDepthFrame(byte[] depthFrame16)
        {
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = depthFrame16[i16] & 0x07;
                int realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = 0;
                depthFrame32[i32 + GREEN_IDX] = 0;
                depthFrame32[i32 + BLUE_IDX] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case 0:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[i32 + RED_IDX] = intensity;
                        break;
                    case 2:
                        depthFrame32[i32 + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[i32 + RED_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(255 - intensity);
                        break;
                }
            }
            return depthFrame32;
        }

        //Gets the depth picture
        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage Image = e.ImageFrame.Image;
            convertedDepthFrame = convertDepthFrame(Image.Bits);
            

        }

       private Point getDisplayPosition(Joint joint)
       {
           float depthX, depthY;
           nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
           depthX = depthX * 320; //convert to 320, 240 space
           depthY = depthY * 240; //convert to 320, 240 space
           int colorX, colorY;
           ImageViewArea iv = new ImageViewArea();
           // only ImageResolution.Resolution640x480 is supported at this point
           nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel(ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY, (short)0, out colorX, out colorY);

           // map back to skeleton.Width & skeleton.Height
           return new Point((int)(GameScreen.Width * colorX / 640.0), (int)(GameScreen.Height * colorY / 480));
       }

       Polyline getBodySegment(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
       {
           PointCollection points = new PointCollection(ids.Length);
           for (int i = 0; i < ids.Length; ++i)
           {
               points.Add(getDisplayPosition(joints[ids[i]]));
           }

           Polyline polyline = new Polyline();
           polyline.Points = points;
           polyline.Stroke = brush;
           polyline.StrokeThickness = 5;
           return polyline;
       }

        //Get the point of the body part
       Ellipse getBodyPoint(Microsoft.Research.Kinect.Nui.JointsCollection joints, Brush brush, params JointID[] ids)
       {
           Ellipse point = new Ellipse();
           point.Stroke = brush;
           
           point.StrokeThickness = 50;

           Point p = new Point();

           p = getDisplayPosition(joints[ids[0]]);  

           point.RenderTransform = new TranslateTransform(p.X,p.Y);

           if (ids[0] == JointID.HandLeft)
           {
               leftHandPosition = getElementPosition(p);
               leftHandPoint = p;
               
           }
           else if (ids[0] == JointID.HandRight)
           {
               rightHandPosition = getElementPosition(p);
               rightHandPoint = p;
           }
           else if (ids[0] == JointID.FootLeft)
           {
               leftFootPosition = getElementPosition(p);
               leftFootPoint = p;
           }
           else
           {
               rightFootPosition = getElementPosition(p);
               rightFootPoint = p;
           }
           return point;
       }
       #endregion
    }
}