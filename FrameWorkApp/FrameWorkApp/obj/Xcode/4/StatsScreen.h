// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <UIKit/UIKit.h>
#import <MapKit/MapKit.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>


@interface StatsScreen : UIViewController {
	UILabel *_totalDistanceLabel;
	UILabel *_totalNumberOfEventsLabel;
	UILabel *_totalPointsLabel;
	UILabel *_totalHardStartsLabel;
	UILabel *_totalHardStopsLabel;
}

@property (nonatomic, retain) IBOutlet UILabel *totalDistanceLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalNumberOfEventsLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalPointsLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalHardStartsLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalHardStopsLabel;

@end
