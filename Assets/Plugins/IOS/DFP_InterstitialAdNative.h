// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <GoogleMobileAds/GADInterstitial.h>
#import <GoogleMobileAds/DFPInterstitial.h>

@interface DFP_InterstitialAdNative : UIViewController<GADInterstitialDelegate>
{
@private
    char* unitID;
    DFPInterstitial *ad;
    
@public
    NSMutableArray* events;
    bool testing;
}

- (void)CreateAd:(char*)unitIDasci;
- (void)Cache;
- (void)Show;
@end
