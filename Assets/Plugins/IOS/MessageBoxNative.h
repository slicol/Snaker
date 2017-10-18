// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface MessageBoxNative : NSObject <UIAlertViewDelegate>
{
@public
    bool okClicked, cancelClicked;
}

- (void)Show:(NSString*)title message:(NSString*)message okButtonText:(NSString*)okButtonText cancelButtonText:(NSString*)cancelButtonText type:(int)type;
@end
