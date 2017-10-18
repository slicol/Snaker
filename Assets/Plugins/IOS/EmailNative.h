// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <MessageUI/MFMailComposeViewController.h>

@interface EmailNative : NSObject <MFMailComposeViewControllerDelegate>
- (void)Send:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML;
@end
