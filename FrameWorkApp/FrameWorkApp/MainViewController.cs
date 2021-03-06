using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.CoreLocation;
using System.Collections.Generic;

namespace FrameWorkApp
{
	public partial class MainViewController : UIViewController
	{
		SDMFileManager fileManager= new SDMFileManager();
		CLLocationManager manager = new CLLocationManager ();
		//Event Type Constants
		const int UNKNOWN_EVENT_TYPE= 0;
		const int HARD_BRAKE_TYPE= 1;
		const int HARD_ACCEL_TYPE= 2;

		public MainViewController (IntPtr handle) : base (handle)
		{
			// Custom initialization
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}
		public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ()
		{
			return UIInterfaceOrientation.Portrait;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Phone Crashed during a Trip in Progress, write data recovered to trip log.
			if (fileManager.currentTripInProgress()) {

				User currentUser = fileManager.readUserFile ();

				//Read Data from Recovered File.
				Event[] events = fileManager.readDataFromTripEventFile ();
				RawGPS rawGPS = new RawGPS ();
				double totalDistance = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(new List<CLLocation>(fileManager.readDataFromTripDistanceFile())));
				int numberOfStarts = 0;
				int numberOfStops = 0;
				foreach (var e in events) {
					if (e.Type== HARD_ACCEL_TYPE) {
						numberOfStarts++;
					}
					if (e.Type == HARD_BRAKE_TYPE) {
						numberOfStops++;
					}
				}

				fileManager.addDataToTripLogFile(new Trip(fileManager.getDateOfLastPointEnteredInCurrentTrip(), (numberOfStops+numberOfStarts )));

				//Update user data
				currentUser.updateData (totalDistance, numberOfStops, numberOfStarts);
				fileManager.updateUserFile(currentUser);

				//Clear current trip files
				fileManager.clearCurrentTripEventFile();
				fileManager.clearCurrentTripDistanceFile ();

				//Display Alert
				new UIAlertView ("Trip Data Recovered!", "We detected your phone has shut down during a trip, but good news, we managed to recover your data up to that point your phone shut down.", null, "Yay!", null).Show ();
			}

			if(CLLocationManager.Status==CLAuthorizationStatus.NotDetermined){

				manager.AuthorizationChanged += (sender,e) => {
					if(CLLocationManager.Status == CLAuthorizationStatus.Denied){
						new UIAlertView("Location Services must be enabled to use application!","We noticed you have disabled location services for this application. Please enable these before continuing. Please enable these before starting a new trip.", null, "OK", null).Show();
					}
				};
				manager.StartUpdatingLocation ();
				manager.StopUpdatingLocation ();
			}
			else{
				if(CLLocationManager.Status == CLAuthorizationStatus.Denied){
					new UIAlertView("Location Services must be enabled to use application!","We noticed you have disabled location services for this application. Please enable these before continuing. Please enable these before starting a new trip.", null, "OK", null).Show();
				}
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
				base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion

		partial void showInfo (NSObject sender)
		{
		}
	}
}