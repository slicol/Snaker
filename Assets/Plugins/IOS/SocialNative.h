// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>

@interface SocialNative : NSObject
{
@public
    CGRect PopoverRect;
}

- (void)ShareImage:(Byte*)data text:(char*)text dataLength:(int)dataLength isPNG:(bool)isPNG;
@end