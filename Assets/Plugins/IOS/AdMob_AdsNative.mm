// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "AdMob_AdsNative.h"
#import "UnityTypes.h"

@implementation AdMob_AdsNative
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
    if (bannerView != nil)
    {
        bannerView.delegate = nil;
        [bannerView removeFromSuperview];
        #if !UNITY_5_0_0
        [bannerView release];
        #endif
        bannerView = nil;
    }
    
    #if !UNITY_5_0_0
    [events dealloc];
    [super dealloc];
    #endif
}

- (void)adViewDidReceiveAd:(GADBannerView *)view
{
    [events addObject:@"Refreshed"];
    bannerView.hidden = !visible;
}

- (void)adView:(GADBannerView *)view didFailToReceiveAdWithError:(GADRequestError *)error
{
    // failed to get ad
    [events addObject:[NSString stringWithFormat:@"Error:%@", [error localizedDescription]]];
    bannerView.hidden = !testing || !visible;
    NSLog(@"%@",[error localizedDescription]);
}

- (void)adViewWillPresentScreen:(GADBannerView *)adView
{
    // do nothing...
}

- (void)adViewWillDismissScreen:(GADBannerView *)adView
{
    // do nothing...
}

- (void)adViewDidDismissScreen:(GADBannerView *)adView
{
    // do nothing...
}

- (void)adViewWillLeaveApplication:(GADBannerView *)adView
{
    [events addObject:@"Clicked"];
}

- (void)CreateAd:(int)gravity adSizeIndex:(int)adSizeIndex unitID:(char*)unitID
{
    [super viewDidLoad];
    
    // create ad
    GADAdSize adSize = kGADAdSizeBanner;
    switch (adSizeIndex)
    {
        case 0: adSize = kGADAdSizeBanner; break;
        case 1: adSize = kGADAdSizeFullBanner; break;
        case 2: adSize = kGADAdSizeLeaderboard; break;
        case 3: adSize = kGADAdSizeMediumRectangle; break;
        case 4: adSize = kGADAdSizeSmartBannerLandscape; break;
        case 5: adSize = kGADAdSizeSmartBannerPortrait; break;
    }
    bannerView = [[GADBannerView alloc] initWithAdSize:adSize];
    
    // get view size
    UIViewController* view = UnityGetGLViewController();
    CGSize viewSize = view.view.frame.size;
    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
    if (orientation == UIDeviceOrientationLandscapeLeft || orientation == UIDeviceOrientationLandscapeRight)
    {
        float width = viewSize.width;
        viewSize.width = viewSize.height;
        viewSize.height = width;
    }
    
    // set ad position
    [self SetGravity:viewSize gravity:gravity];

    // finsih
    bannerView.adUnitID = [[NSString alloc] initWithUTF8String:unitID];
    bannerView.rootViewController = view;
    bannerView.delegate = self;
    [view.view addSubview:bannerView];
    if (testing)
    {
        GADRequest *request = [GADRequest request];
        //request.testing = YES;
        //request.testDevices = [NSArray arrayWithObjects:GAD_SIMULATOR_ID, nil];
        [bannerView loadRequest:request];
    }
    else
    {
        [bannerView loadRequest:[GADRequest request]];
    }
}

- (void)SetGravity:(CGSize)viewSize gravity:(int)gravity
{
    CGSize size = [bannerView sizeThatFits:viewSize];
    auto frame = bannerView.frame;
    switch (gravity)
    {
        case 0: frame.origin = CGPointMake(0, viewSize.height-size.height); break;
        case 1: frame.origin = CGPointMake(viewSize.width-size.width, viewSize.height-size.height); break;
        case 2: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), viewSize.height-size.height); break;
        case 3: frame.origin = CGPointMake(0, 0); break;
        case 4: frame.origin = CGPointMake(viewSize.width-size.width, 0); break;
        case 5: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), 0); break;
        case 6: frame.origin = CGPointMake((viewSize.width*.5f)-(size.width*.5f), (viewSize.height*.5f)-(size.height*.5f)); break;
    }
    
    bannerView.frame = frame;
}

- (void)SetVisible:(BOOL)visibleValue
{
    self->visible = visibleValue;
    bannerView.hidden = !visibleValue;
}

- (void)Refresh
{
    if (testing)
    {
        GADRequest *request = [GADRequest request];
        //request.testing = YES;
        //request.testDevices = [NSArray arrayWithObjects:GAD_SIMULATOR_ID, nil];
        [bannerView loadRequest:request];
    }
    else
    {
        [bannerView loadRequest:[GADRequest request]];
    }
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
    AdMob_AdsNative* AdMob_InitAd(bool testing)
    {
        AdMob_AdsNative* ad = [[AdMob_AdsNative alloc] init];
        ad->testing = testing;
        
        #if UNITY_5_0_0
        if (NativeAds == nil) NativeAds = [[NSMutableArray alloc] init];
        [NativeAds addObject:ad];
        #endif
        return ad;
    }
    
    void AdMob_DisposeAd(AdMob_AdsNative* ad)
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
    
    void AdMob_CreateAd(AdMob_AdsNative* ad, int gravity, int adSizeIndex, char* unitID)
    {
        [ad CreateAd:gravity adSizeIndex:adSizeIndex unitID:unitID];
    }
    
    void AdMob_SetAdGravity(AdMob_AdsNative* ad, int gravity)
    {
        UIViewController* view = UnityGetGLViewController();
        [ad SetGravity:view.view.frame.size gravity:gravity];
    }
    
    void AdMob_SetAdVisible(AdMob_AdsNative* ad, BOOL visible)
    {
        [ad SetVisible:visible];
    }
    
    void AdMob_Refresh(AdMob_AdsNative* ad)
    {
        [ad Refresh];
    }
    
    bool AdMob_AdHasEvents(AdMob_AdsNative* ad)
    {
        return ad->events.count != 0;
    }
    
    const char* AdMob_GetNextAdEvent(AdMob_AdsNative* ad)
    {
        const char* ptr = [[ad->events objectAtIndex:0] cStringUsingEncoding:NSUTF8StringEncoding];
        [ad->events removeObjectAtIndex:0];
        return ptr;
    }
}