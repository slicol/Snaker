// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "AdMob_InterstitialAdNative.h"
#import "UnityTypes.h"

@implementation AdMob_InterstitialAdNative
- (id)init
{
    self = [super init];
    // init other data here...
    events = [[NSMutableArray alloc] init];
    return self;
}

- (void)viewDidLoad
{
    // do nothing...
}

- (void)dealloc
{
    if (ad != nil)
    {
        ad.delegate = nil;
        #if !UNITY_5_0_0
        [ad release];
        #endif
        ad = nil;
    }
    
    if (unitID != nil)
    {
        delete[] unitID;
        unitID = nil;
    }
    
    #if !UNITY_5_0_0
    [events dealloc];
    [super dealloc];
    #endif
}

- (void)interstitialDidReceiveAd:(GADInterstitial*)interstitial
{
    [events addObject:@"Cached"];
}

- (void)interstitial:(GADInterstitial*)interstitial didFailToReceiveAdWithError:(GADRequestError*)error
{
    [events addObject:[NSString stringWithFormat:@"Error:%@", [error localizedDescription]]];
    NSLog(@"%@",[error localizedDescription]);
}

- (void)interstitialWillPresentScreen:(GADInterstitial*)interstitial
{
    [events addObject:@"Shown"];
}

- (void)interstitialWillDismissScreen:(GADInterstitial*)interstitial
{
    [events addObject:@"Canceled"];
}

- (void)interstitialDidDismissScreen:(GADInterstitial*)interstitial
{
    // do nothing...
}

- (void)interstitialWillLeaveApplication:(GADInterstitial*)interstitial
{
    [events addObject:@"Clicked"];
}

- (void)CreateAd:(char*)unitIDasci
{
    unitID = new char[strlen(unitIDasci) + 1];
    strcpy(unitID, unitIDasci);
}

- (void)Cache
{
    // delete used ad
    if (ad != nil && ad.hasBeenUsed)
    {
        ad.delegate = nil;
        #if !UNITY_5_0_0
        [ad release];
        #endif
        ad = nil;
    }
    
    // create new ad
    ad = [[GADInterstitial alloc] init];
    ad.adUnitID = [[NSString alloc] initWithUTF8String:unitID];
    ad.delegate = self;
    
    if (testing)
    {
        GADRequest *request = [GADRequest request];
        //request.testing = YES;
        //request.testDevices = [NSArray arrayWithObjects:GAD_SIMULATOR_ID, nil];
        [ad loadRequest:request];
    }
    else
    {
        [ad loadRequest:[GADRequest request]];
    }
}

- (void)Show
{
    UIViewController* view = UnityGetGLViewController();
    [ad presentFromRootViewController:view];

}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
#if UNITY_5_0_0
static NSMutableArray* NativeAds = nil;
#endif

extern "C"
{
    AdMob_InterstitialAdNative* AdMob_Interstitial_InitAd(bool testing)
    {
        AdMob_InterstitialAdNative* ad = [[AdMob_InterstitialAdNative alloc] init];
        ad->testing = testing;
        
        #if UNITY_5_0_0
        if (NativeAds == nil) NativeAds = [[NSMutableArray alloc] init];
        [NativeAds addObject:ad];
        #endif
        return ad;
    }
    
    void AdMob_Interstitial_DisposeAd(AdMob_InterstitialAdNative* ad)
    {
        if (ad != nil)
        {
            #if !UNITY_5_0_0
            [ad release];
            #else
            [NativeAds removeObject:ad];
            #endif
            ad = nil;
        }
    }
    
    void AdMob_Interstitial_CreateAd(AdMob_InterstitialAdNative* ad, char* unitID)
    {
        [ad CreateAd:unitID];
    }
    
    void AdMob_Interstitial_Cache(AdMob_InterstitialAdNative* ad)
    {
        [ad Cache];
    }
    
    void AdMob_Interstitial_Show(AdMob_InterstitialAdNative* ad)
    {
        [ad Show];
    }
    
    bool AdMob_Interstitial_AdHasEvents(AdMob_InterstitialAdNative* ad)
    {
        return ad->events.count != 0;
    }
    
    const char* AdMob_Interstitial_GetNextAdEvent(AdMob_InterstitialAdNative* ad)
    {
        const char* ptr = [[ad->events objectAtIndex:0] cStringUsingEncoding:NSUTF8StringEncoding];
        [ad->events removeObjectAtIndex:0];
        return ptr;
    }
}