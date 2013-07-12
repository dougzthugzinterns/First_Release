using System;

namespace FrameWorkApp
{
	public class User
	{
		private double totalDistance = 0;
		private int totalNumberOfEvents = 0;
		private int totalPoints = 0;
		private int totalHardStops = 0;
		private int totalHardStarts = 0;

		public User (double totalDistance, int totalNumberOfPoints, int totalHardStarts, int totalHardStops )
		{
			this.totalDistance = totalDistance;
			this.totalPoints = totalNumberOfPoints;
			this.totalHardStops = totalHardStops;
			this.totalHardStarts = totalHardStarts;
		}

		public double TotalDistance {
			get { return totalDistance;}
			set { this.totalDistance = value;}
		}

		public int TotalNumberOfEvents {
			get{ return totalHardStops + totalHardStarts;}
		}
		public int TotalHardStarts {
			get{ return totalHardStarts;}
			set{ this.totalHardStarts = value;}
		}
		public int TotalHardStops {
			get{ return totalHardStops;}
			set{ this.totalHardStops = value;}
		}
		public int TotalPoints {
			get { return totalNumberOfEvents;}
			set { this.totalNumberOfEvents = value;}
		}
		public int updateData (double tripDistance, int numOfStops, int numOfStarts)
		{
			totalDistance += tripDistance;
			totalNumberOfEvents += numOfStops + numOfStarts;
			totalHardStops += numOfStops;
			totalHardStarts += numOfStarts;

			totalPoints = Math.Max ((totalPoints + (int)tripDistance + ((-3) * (numOfStops+numOfStarts))),0);

			                        return (totalPoints + (int)tripDistance + ((-3) * (numOfStops+numOfStarts)));
		}
	}
}