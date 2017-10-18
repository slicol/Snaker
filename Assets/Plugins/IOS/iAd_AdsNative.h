// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <iAd/iAd.h>

@interface iAd_AdsNative : UIView <ADBannerViewDelegate>
{
@private
    ADBannerView* iAd;
    
@public
    BOOL testing, visible;
    NSMutableArray* events;
}

- (void)CreateAd:(int)gravity;
- (void)SetGravity:(CGSize)viewSize gravity:(int)gravity;
- (void)SetVisible:(BOOL)visibleValue;
@end
