using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreMotion;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	public partial class StopScreen: UIViewController
	{

		//To Be looked At############################################################################
		public static SDMFileManager fileManager = new SDMFileManager ();
		public static List<CLLocation> listOfTripLocationCoordinates = new List<CLLocation> ();
		RawGPS rawGPS = new RawGPS ();
		public static double distanceTraveledForCurrentTrip = 0;
		public static int numberHardStarts = 0;
		public static int numberHardStops = 0;
		//public static int numberHardAccel = 0;
		//###########################################################################################
		//const double STARTSPEEDTHRESHOLD = 10;
		//const double SPEED_THRESHOLD_TURNING = -18;
		//const double SPEED_THRESHOLD_ACCEL = 100;
		//Declaration Statements

		private const double SPEED_THRESHOLD_BRAKING = 4;
		private const double SPEED_THRESHOLD_STARTS = 4;
		//Type Codes for Events
		private const int UNKNOWN_EVENT_TYPE = 0;
		private const int HARD_BRAKE_TYPE = 1;
		private const int HARD_ACCEL_TYPE = 2;
		private double[] initialSpeedArray;
		private double[] finalSpeedArray;
		private int initialSpeedArrayCounter;
		private int finalSpeedArrayCounter;
		private bool isCountingInitialSpeed;
		private bool isCountingFinalSpeed;

		private double currentMaxAvgAccel;
		private double avgaccel;
		private double threshold;
		private double maxThresh;
		//number of events
		public static int eventcount = 0;
		//true if event is in progress
		private bool eventInProgress;
		private double currentSpeed;
		//container for current location
		private CLLocationCoordinate2D currentCoord;
		private CMMotionManager _motionManager;
		public StopScreen (IntPtr handle) : base (handle)
		{
			//Initialize Declarations
			initialSpeedArray = new double[8];
			finalSpeedArray = new double[8];
			initialSpeedArrayCounter = 0;
			finalSpeedArrayCounter = 0;
			isCountingInitialSpeed = false;
			isCountingFinalSpeed = false;
			//threshold for erratic behavior in G's
			threshold = .24;
			maxThresh = 1.2;
			eventcount = 0;
			//true if event is in progress
			eventInProgress = false;
			//container for current location
			//currentCoord = new CLLocationCoordinate2D ();
			currentSpeed = 0;
			avgaccel = 0;
			currentMaxAvgAccel = 0;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Check and make sure GPS is enabled
			if (CLLocationManager.Status == CLAuthorizationStatus.Authorized) {
				rawGPS.createCoordinatesWhenHeadingChangesToAddToList ();

				//Start updating accelerometer data at device motion update interval specified pulling rate
				_motionManager = new CMMotionManager ();
				_motionManager.DeviceMotionUpdateInterval = .5;
				_motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (data,error) =>
				                                     
				{
					this.handleDeviceMotionUpdate(data, error);

				});
			}

			else {
				new UIAlertView ("Location Services must be enabled to use application!", "We noticed you have disabled location services for this application. Please enable location services.", null, "Ok", null).Show ();

			}

			// Perform any additional setup after loading the view, typically from a nib.
		}
		public void handleDeviceMotionUpdate (CMDeviceMotion data, NSError error){
			avgaccel = Math.Sqrt ((data.UserAcceleration.X * data.UserAcceleration.X) +
			                      (data.UserAcceleration.Y * data.UserAcceleration.Y) +
			                      (data.UserAcceleration.Z * data.UserAcceleration.Z));

			//Calculate current speed in Km/Hr
			currentSpeed = rawGPS.convertToKilometersPerHour (rawGPS.getSpeedInMetersPerSecondUnits ());
			Console.WriteLine (currentSpeed.ToString ());
			//this.currentSpeedLabel.Text = currentSpeed.ToString ();

			//Test if accelerometer event occurs
			if ((avgaccel > threshold) && (avgaccel < maxThresh)) {
				Console.WriteLine ("Event is in progress.");

				if (!eventInProgress) {
					isCountingInitialSpeed = true;
					eventInProgress = true;
				}
			}

			//Test if accelerometer event has ended
			else if ((avgaccel < threshold) && eventInProgress) {
				Console.WriteLine ("Event has ended.");
				eventcount++;
				eventInProgress = false;
				isCountingFinalSpeed = true;
				//this.eventCounter.Text = eventcount.ToString ();
			}

			//If event has started, start counting/filling initialSpeedArray
			if (isCountingInitialSpeed == true && initialSpeedArrayCounter < initialSpeedArray.Length - 1) {

				initialSpeedArray [initialSpeedArrayCounter] = currentSpeed;
				Console.WriteLine ("Writing 1: " + currentSpeed);
				initialSpeedArrayCounter++;
				Console.WriteLine ("count1: " + initialSpeedArrayCounter);
			}

			//If event has stopped, start counting/filling finalSpeedArray
			if (isCountingFinalSpeed == true && finalSpeedArrayCounter < finalSpeedArray.Length - 1) {

				finalSpeedArray [finalSpeedArrayCounter] = currentSpeed;
				Console.WriteLine ("Writing 2: " + currentSpeed);
				finalSpeedArrayCounter++;
				Console.WriteLine ("count2: " + finalSpeedArrayCounter);
			}

			//If both speed arrays are full, test event criteria
			if ((initialSpeedArrayCounter == initialSpeedArray.Length - 1) && (finalSpeedArrayCounter == finalSpeedArray.Length - 1)) {
				Console.WriteLine ("initial" + initialSpeedArray [initialSpeedArray.Length - 1]);
				Console.WriteLine ("final  " + finalSpeedArray [finalSpeedArray.Length - 1]);
				this.determineHardStoOrHardStart (initialSpeedArray, finalSpeedArray);

				this.clearArray (initialSpeedArray);
				this.clearArray (finalSpeedArray);
				//Reset for new event
				isCountingFinalSpeed = false;
				isCountingInitialSpeed = false;
				initialSpeedArrayCounter = 0;
				finalSpeedArrayCounter = 0;
			}

			//this.avgAcc.Text = avgaccel.ToString ("0.0000");

			if (avgaccel > currentMaxAvgAccel) {
				currentMaxAvgAccel = avgaccel;
			}

			//this.maxAvgAcc.Text = currentMaxAvgAccel.ToString ("0.0000");
		}

		partial void stopButton (NSObject sender)
		{
			rawGPS.listOfRawGPSTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			listOfTripLocationCoordinates = rawGPS.listOfRawGPSTripLocationCoordinates;
			distanceTraveledForCurrentTrip = rawGPS.convertMetersToKilometers (rawGPS.CalculateDistanceTraveled (rawGPS.listOfRawGPSTripLocationCoordinates));
			rawGPS.stopGPSReadings ();
		}
		public void clearArray(double[] array){
			for (int i = 0; i< array.Length; i++) {
				array [i] = 0;
			}
		}
		//Get minimum value of an array of doubles method
		public double getMin (double[] array)
		{
			double temp = array [0];
			for (int i = 1; i<array.Length; i++) {
				if (array [i] < temp) {
					temp = array [i];
				}
			}
			return temp;
		}
		//Get maximum value of an array of doubles method
		public double getMax (double[] array)
		{
			double temp = array [0];
			for (int i = 1; i<array.Length; i++) {
				if (array [i] > temp) {
					temp = array [i];
				}
			}
			return temp;
		}
		//Get average value of an array of doubles method
		public double getAvg (double[] array)
		{
			double avg = 0;
			for (int i = 0; i < array.Length; i++) {
				avg += array [i];
			}
			avg = avg / (array.Length);
			return avg;
		}
		//Test for different event types (Hard Start/Stop)
		public void determineHardStoOrHardStart (double[] initialSpeedArray, double[] finalSpeedArray)
		{
			Console.WriteLine ("av1: " + this.getAvg (initialSpeedArray));
			Console.WriteLine ("av2: " + this.getAvg (finalSpeedArray));

			Console.WriteLine ("Initial min: " + this.getMin (initialSpeedArray));
			Console.WriteLine ("Final Max: " + this.getMax (finalSpeedArray));

			//Hard Stop Logic... Using Average of Arrays
			if (initialSpeedArray [initialSpeedArray.Length - 1] < initialSpeedArray [0]) {

				if ((this.getAvg (initialSpeedArray) - this.getAvg (finalSpeedArray)) > SPEED_THRESHOLD_BRAKING) {
					numberHardStops++;
					Console.WriteLine ("Hard stop recorded!");
					currentCoord.Latitude = rawGPS.getCurrentUserLatitude ();
					currentCoord.Longitude = rawGPS.getCurrentUserLongitude ();
					Event newevent = new Event (currentCoord, HARD_BRAKE_TYPE);
					//coordList.Add (currentCoord);
					fileManager.addEventToTripEventFile (newevent);
				}
			}

			//Hard Start Logic... Using Min and Max of Arrays
			else if (((this.getMax (finalSpeedArray) - this.getMin (initialSpeedArray)) > SPEED_THRESHOLD_STARTS)) {

				numberHardStarts++;
				Console.WriteLine ("Hard start recorded!");
				currentCoord.Latitude = rawGPS.getCurrentUserLatitude ();
				currentCoord.Longitude = rawGPS.getCurrentUserLongitude ();
				Event newevent = new Event (currentCoord, HARD_ACCEL_TYPE);
				//coordList.Add (currentCoord);
				fileManager.addEventToTripEventFile (newevent);
			} else {
				Console.WriteLine ("No event recorded!");
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			//add users location when trip starts
			if (CLLocationManager.Status == CLAuthorizationStatus.Authorized) {
				rawGPS.listOfRawGPSTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			}

			base.ViewWillAppear (animated);

			//Disable Phone Idling

			UIApplication.SharedApplication.IdleTimerDisabled = true;
			this.NavigationController.SetNavigationBarHidden (true, animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			_motionManager.StopDeviceMotionUpdates ();
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			ReleaseDesignerOutlets ();
		}
	}
}