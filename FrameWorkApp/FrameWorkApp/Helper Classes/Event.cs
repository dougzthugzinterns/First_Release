using System;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;

namespace FrameWorkApp
{
	public class Event
	{
		UIColor HARD_BRAKE_COLOR= UIColor.Magenta;
		UIColor HARD_ACCEL_COLOR= UIColor.Orange;
		UIColor UNKNOWN_EVENT_COLOR= UIColor.Purple;
		const int UNKNOWN_EVENT_TYPE= 0;
		const int HARD_BRAKE_TYPE= 1;
		const int HARD_ACCEL_TYPE= 2;
		const String HARD_BRAKE_TEXT = "Hard Stop";
		const String HARD_ACCEL_TEXT = "Hard Start";
		const String UNKNOWN_EVENT_TEXT = "Incident Occured";

		private CLLocationCoordinate2D location;
		private UIColor color;
		private int type = 0;
		private String text = "";

		public Event (CLLocationCoordinate2D location, int color)
		{
			this.Type = color;
			this.location = location;
		
		}


		public CLLocationCoordinate2D Location{
			get { return location; }
			set { this.location = value; }
		}

		public UIColor Color{
			get{ return color;}
			set { this.color = value;}
		}

		public String Text{
			get{ return text;}
			set{ this.text=value;}
		}
		public int Type{
			get{ return type;}
			set {
				this.type = value;
				switch (this.type) {
				case UNKNOWN_EVENT_TYPE:
					this.color = UNKNOWN_EVENT_COLOR;
					this.text = UNKNOWN_EVENT_TEXT;
						break;
				case HARD_BRAKE_TYPE:
					this.color = HARD_BRAKE_COLOR;
					this.text = HARD_BRAKE_TEXT;
						break;
				case HARD_ACCEL_TYPE:
					this.color = HARD_ACCEL_COLOR;
					this.text = HARD_ACCEL_TEXT;
						break;
					default:
						this.color = UNKNOWN_EVENT_COLOR;
						this.text = UNKNOWN_EVENT_TEXT;
						break;
				}
			}
		}



	}
}

