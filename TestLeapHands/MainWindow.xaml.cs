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
using Leap;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Threading;


//this project is for tutorial 4,5
namespace TestLeapHands
{

    public partial class MainWindow : Window, ILeapEventDelegate
    {

        private Controller controller = new Controller();

        private InteractionBox ibox;

        private LeapEventListener listener;
      
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 800.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 600.0f;
        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Thickness of fingertip point
        /// </summary>
        private const double jointThickness = 4;
        /// <summary>
        /// Thickness of palmCenter point
        /// </summary>
        private const double palmCenterThickness = 10;

        /// <summary>
        /// Brush of left fingertips
        /// </summary>
        private readonly Brush JointPointBrush = Brushes.Red;

        /// <summary> 
        /// Palm Center brush
        /// </summary>
        private readonly Brush PalmCenterBrush = Brushes.Red;

        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            //Create controller object in Main window
            this.controller = new Controller();
            //Create Listener
            this.listener = new LeapEventListener(this);
            controller.AddListener(listener);

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //clear the drawing every 10 seconds
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
            dispatcherTimer.Start();


        }
      

        /**create a  delegate, delegate is a keyword, it means method type*/
        delegate void LeapEventDelegate(string EventName);

        /** This method check the event in listener class
           *The activated event's name can be got through this method*/
        public void LeapEventNotification(string EventName)
        {
            if (this.CheckAccess())
            {
                switch (EventName)
                {
                    case "onInit":

                        break;
                    case "onConnect":
                        
                        this.connectHandler();
                        break;
                    case "onFrame":
                        this.HandProperties(this.controller.Frame());
                        this.paintCurve();
                        this.checkGestures(this.controller.Frame());
                      
                        break;
                }
            }
            else
            {
                Dispatcher.Invoke(new LeapEventDelegate(LeapEventNotification), new object[] { EventName });
            }
        }//end method LeapEventNotification

        public void connectHandler()
        {
            this.controller.SetPolicyFlags(Controller.PolicyFlag.POLICY_IMAGES);

            //enable swipe gesture
            this.controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            this.controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            this.controller.Config.SetFloat   ("Gesture.Swipe.MinLength", 100.0f);
            this.controller.Config.SetFloat("InteractionBox.Width", 1300.0f);
            this.controller.Config.SetFloat("InteractionBox.Height", 600.0f);


        }

        public Point toScreenCoor(Leap.Vector LVector)
        {

            ibox = controller.Frame().InteractionBox;
            
            Leap.Vector normalizedPoint = ibox.NormalizePoint(LVector, true);
            float appX = normalizedPoint.x * (float)(Image.Width);
            float appY = (1 - normalizedPoint.y) * (float)(Image.Height);
            float appX2 = LVector.x;
            float appY2 = LVector.y;
            return new Point(appX, appY);

           // return new Point(appX2, appY2);


        }

        public void checkGestures(Leap.Frame frame)
        {
           // Get gestures
		    GestureList gestures = frame.Gestures ();
            for (int i = 0; i < gestures.Count; i++) {
			Gesture gesture = gestures [i];

			switch (gesture.Type) {
			case Gesture.GestureType.TYPE_CIRCLE:
				CircleGesture circle = new CircleGesture (gesture);

                // Calculate clock direction using the angle between circle normal and pointable
				
				break;


			case Gesture.GestureType.TYPE_SWIPE:
				SwipeGesture swipe = new SwipeGesture (gesture);

             //display the swipe position in label

                SwipePosition.Content = swipe.Position;
           
           
               
			
				break;
			case Gesture.GestureType.TYPE_KEY_TAP:
				KeyTapGesture keytap = new KeyTapGesture (gesture);
				
				break;
			case Gesture.GestureType.TYPE_SCREEN_TAP:
				ScreenTapGesture screentap = new ScreenTapGesture (gesture);
				
				break;
			default:
			
				break;
			}
		}


            

        }

        void paintCurve()
        {

           // ForceRepaint();
            Leap.Frame startFrame = this.controller.Frame(0);
            //Declare lines for every finger of the hands


 //---------Left fingers------------------------------------------------
            Line left0 = new Line();
            Line left1 = new Line();
            Line left2 = new Line();
            Line left3 = new Line();
            Line left4 = new Line();
            //set the stroke of each fingers
            left0.Stroke = System.Windows.Media.Brushes.Red;
            left1.Stroke = System.Windows.Media.Brushes.Orange;
            left2.Stroke = System.Windows.Media.Brushes.Yellow;
            left3.Stroke = System.Windows.Media.Brushes.Green;
            left4.Stroke = System.Windows.Media.Brushes.Blue;


//----------Right finegrs-------------------------------------------------------
            Line right0 = new Line();
            Line right1 = new Line();
            Line right2 = new Line();
            Line right3 = new Line();
            Line right4 = new Line();
            //set the stroke of each fingers
            right0.Stroke = System.Windows.Media.Brushes.Red;
            right1.Stroke = System.Windows.Media.Brushes.Orange;
            right2.Stroke = System.Windows.Media.Brushes.Yellow;
            right3.Stroke = System.Windows.Media.Brushes.Green;
            right4.Stroke = System.Windows.Media.Brushes.Blue;

            Line drawingLine1 = new Line();
            Line drawingLine2 = new Line();
            drawingLine1.Stroke = SystemColors.WindowBrush;
            drawingLine2.Stroke = SystemColors.WindowBrush;

            foreach (Finger finger in startFrame.Hands.Leftmost.Fingers)
            {
              //  Debug.WriteLine(finger.Type());

                if (finger.Type().Equals(Finger.FingerType.TYPE_THUMB))
                {
                 
                    Debug.WriteLine("left thumb");
                    left0.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left0.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;

                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_INDEX))
                {
                    Debug.WriteLine("left index");
                    left1.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left1.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_MIDDLE))
                {
                    Debug.WriteLine("left middle");
                    left2.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left2.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_RING))
                {
                    left3.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left3.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_PINKY))
                {
                    left4.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left4.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
               // drawingLine1.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
               // drawingLine1.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;

            }


            foreach (Finger finger in startFrame.Hands.Rightmost.Fingers)
            {
                if (finger.Type().Equals(Finger.FingerType.TYPE_THUMB))
                {
                    right0.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right0.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;

                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_INDEX))
                {
                    right1.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right1.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_MIDDLE))
                {
                    right2.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right2.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_RING))
                {
                    right3.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right3.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_PINKY))
                {
                    right4.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right4.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
               // drawingLine2.X1 = toScreenCoor(finger.StabilizedTipPosition).X;
               // drawingLine2.Y1 = toScreenCoor(finger.StabilizedTipPosition).Y;

            }


            Leap.Frame nextFrame = this.controller.Frame(1);
            foreach (Finger finger in nextFrame.Hands.Leftmost.Fingers)
            {
                if (finger.Type().Equals(Finger.FingerType.TYPE_THUMB))
                {
                    left0.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left0.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;

                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_INDEX))
                {
                    left1.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left1.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_MIDDLE))
                {
                    left2.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left2.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_RING))
                {
                    left3.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left3.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_PINKY))
                {
                    left4.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    left4.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                //drawingLine1.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                //drawingLine1.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;

            }
            foreach (Finger finger in nextFrame.Hands.Rightmost.Fingers)
            {
                if (finger.Type().Equals(Finger.FingerType.TYPE_THUMB))
                {
                   right0.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                   right0.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;

                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_INDEX))
                {
                    right1.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right1.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_MIDDLE))
                {
                    right2.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right2.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_RING))
                {
                    right3.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right3.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                else if (finger.Type().Equals(Finger.FingerType.TYPE_PINKY))
                {
                    right4.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                    right4.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;
                }
                //drawingLine2.X2 = toScreenCoor(finger.StabilizedTipPosition).X;
                //drawingLine2.Y2 = toScreenCoor(finger.StabilizedTipPosition).Y;

            }
            //add the line to painSurface
           // paintSurface.Children.Add(drawingLine1);
           // paintSurface.Children.Add(drawingLine2);

            paintSurface.Children.Add(left0);
            paintSurface.Children.Add(left1);
            paintSurface.Children.Add(left2);
            paintSurface.Children.Add(left3);
            paintSurface.Children.Add(left4);

            paintSurface.Children.Add(right0);
            paintSurface.Children.Add(right1);
            paintSurface.Children.Add(right2);
            paintSurface.Children.Add(right3);
            paintSurface.Children.Add(right4);

        }
        //repaint the drawing things

        //move the drawing every several second

        //  System.Windows.Threading.DispatcherTimer.Tick handler 
        // 
        //  Updates the current seconds display and calls 
        //  InvalidateRequerySuggested on the CommandManager to force  
        //  the Command to raise the CanExecuteChanged event. 
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second

            paintSurface.Children.Clear();

            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
        


        public void HandProperties(Leap.Frame frame)
        {
            HandList hands = frame.Hands;
            Hand leftmostHand = hands.Leftmost;
            Hand rightmostHand = hands.Rightmost;
            LPalmPosition.Content = leftmostHand.PalmPosition;
            RPalmPosition.Content = rightmostHand.PalmPosition;
            float  distance = leftmostHand.PalmPosition.DistanceTo(rightmostHand.PalmPosition);
            
              //Initalize the drawing component
               Pen drawpen = new Pen(Brushes.Green, 6);
               Pen bridgePen = new Pen(Brushes.GreenYellow, 3);
               Pen drawPen2 = new Pen(Brushes.Blue, 6);
               Pen jointPen = new Pen(Brushes.Blue, 8);
               Pen bonePen = new Pen(Brushes.LightSlateGray,8);
               
            
       //1. Get left and right pinky
        FingerList fingers = frame.Fingers;
        
        Finger pinkyL = fingers.Leftmost;
        Finger pinkyR = fingers.Rightmost;
        //2. Calculate the angle (in radian) using API method angleTo:
          float angle = pinkyL.Direction.AngleTo(pinkyR.Direction);

        //3. Calculate the fingertip distance
   		 //calculate distance between two fingertip 
        float distanceofPinky = pinkyL.TipPosition.DistanceTo(pinkyR.TipPosition);
        //this two way have the same result
        float x1 = pinkyL.TipPosition.x;
        float y1 = pinkyL.TipPosition.y;
        float z1 = pinkyL.TipPosition.z;

        float x2 = pinkyR.TipPosition.x;
        float y2 = pinkyR.TipPosition.y;
        float z2 = pinkyR.TipPosition.z;

        double calculatedDistance  = Math.Sqrt((x1-x2)*(x1-x2)+(y1-y2)*(y1-y2)+(z1-z2)*(z1-z2));
       /* Debug.WriteLine("The distance of two fingers is" + distanceofPinky +
                        "The angle between two pinky is" +angle

                        );*/




            //drawing the ellipse that stand for the finger tip point
            using (DrawingContext dc = this.drawingGroup.Open())
           {
               // Draw a transparent background to set the render size
               dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight)); 
  

                //draw the the left finger
                 foreach (Finger finger in leftmostHand.Fingers){

                    Leap.Vector fingerTipPosition = finger.TipPosition;
                    //Debug.WriteLine("new finger");
                    //dc.DrawLine(drawpen, toScreenCoor(finger.TipPosition), toScreenCoor(leftmostHand.PalmPosition));
                    dc.DrawEllipse(PalmCenterBrush, drawpen, toScreenCoor(leftmostHand.PalmPosition), palmCenterThickness, palmCenterThickness);
                   
                     //draw the curve of finger tip 画指尖移动的痕迹
                    

                     // Left finger bones
                     Bone bone;
                     foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
                     {
                         bone = finger.Bone(boneType);
                         //draw the joint in red
                         dc.DrawEllipse(JointPointBrush, jointPen, toScreenCoor(bone.PrevJoint), jointThickness, jointThickness);
                         dc.DrawEllipse(JointPointBrush, jointPen, toScreenCoor(bone.NextJoint), jointThickness, jointThickness);
                        
                         //draw the bone in Grey
                         dc.DrawLine(bonePen,toScreenCoor(bone.PrevJoint),toScreenCoor(bone.NextJoint));
                     }//end inner foreach
                }//end outer foreach

                   //Right Fingers
                   foreach (Finger finger in rightmostHand.Fingers)
                 {

                     Leap.Vector fingerTipPosition = finger.TipPosition;
                    // Debug.WriteLine("new right finger");
                     dc.DrawEllipse(PalmCenterBrush, drawPen2, toScreenCoor(rightmostHand.PalmPosition), palmCenterThickness, palmCenterThickness);
                     //dc.DrawLine(drawPen2, toScreenCoor2(finger.TipPosition), toScreenCoor2(rightmostHand.PalmPosition));
                     // Right finger bones
                     dc.DrawLine(bonePen, toScreenCoor(finger.StabilizedTipPosition), toScreenCoor(finger.StabilizedTipPosition));
                    

                     //right bones
                     Bone bone;
                     foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
                     {
                         bone = finger.Bone(boneType);
                         //draw the joint in red

                         //draw the bone in white
                         dc.DrawEllipse(JointPointBrush, jointPen, toScreenCoor(bone.PrevJoint), jointThickness, jointThickness);
                         dc.DrawEllipse(JointPointBrush, jointPen, toScreenCoor (bone.NextJoint), jointThickness, jointThickness);
                         dc.DrawLine(bonePen, toScreenCoor(bone.PrevJoint), toScreenCoor(bone.NextJoint));
                     }//end inner foreach
                 }//end outer foreach
            }//end using
       
 }//end handproperties method


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            paintSurface.Children.Clear();
        }

       
    } //end mainWindows
    public interface ILeapEventDelegate
    {
        //definded a method that can be reused
        void LeapEventNotification(string EventName);
    }

    //listener class
    public class LeapEventListener : Listener
    {

        //create a interface 
        ILeapEventDelegate eventDelegate;
     

        //create a constructor with interface argument
        public LeapEventListener(ILeapEventDelegate delegateObject)
        {
            //create a object of interface
            this.eventDelegate = delegateObject;

        }

        public override void OnInit(Controller controller)
        {
            /**call the LeapEventNotification method in the eventDelegate interface. 
           If the event is activated, the event name can be reported to LeapEventNotification 
             */
            this.eventDelegate.LeapEventNotification("onInit");
        }
        public override void OnConnect(Controller controller)
        {

            this.eventDelegate.LeapEventNotification("onConnect");
        }

        public override void OnFrame(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onFrame");
        }
        public override void OnExit(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onExit");
        }
        public override void OnDisconnect(Controller controller)
        {
            this.eventDelegate.LeapEventNotification("onDisconnect");
        }

    }//end of listener class

}//end of namespace
