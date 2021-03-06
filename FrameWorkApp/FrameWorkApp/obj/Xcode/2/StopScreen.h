// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <UIKit/UIKit.h>
#import <MapKit/MapKit.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>


@interface StopScreen : UIViewController {
	UILabel *_avgAcc;
	UILabel *_eventCounter;
	UILabel *_currentSpeedLabel;
	UILabel *_maxAvgAcc;
	UILabel *_SpeedAfterEventLabel;
	UILabel *_SpeedAtEventLabel;
	UILabel *_speedDiffLabel;
}

@property (nonatomic, retain) IBOutlet UILabel *avgAcc;

@property (nonatomic, retain) IBOutlet UILabel *eventCounter;

@property (nonatomic, retain) IBOutlet UILabel *currentSpeedLabel;

@property (nonatomic, retain) IBOutlet UILabel *maxAvgAcc;

@property (nonatomic, retain) IBOutlet UILabel *SpeedAfterEventLabel;

@property (nonatomic, retain) IBOutlet UILabel *SpeedAtEventLabel;

@property (nonatomic, retain) IBOutlet UILabel *speedDiffLabel;

- (IBAction)resetMaxValues:(id)sender;

- (IBAction)stopButton:(id)sender;

@end
