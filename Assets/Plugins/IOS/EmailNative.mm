// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "EmailNative.h"
#import "UnityTypes.h"

@implementation EmailNative
- (id)init
{
    self = [super init];
    // init other data here...
    return self;
}

- (void)dealloc
{
    // dispose...
    #if !UNITY_5_0_0
    [super dealloc];
    #endif
}

- (void)showViewController:(UIViewController*)viewController
{
	//UnityPause(true);
	
	// cancel the previous delayed call to dismiss the view controller if it exists
	[NSObject cancelPreviousPerformRequestsWithTarget:self];
    
    // show the view
	UIViewController *vc = UnityGetGLViewController();
    [vc presentViewController:viewController animated:YES completion:nil];
}

- (void)dismissViewController
{
    //UnityPause(false);
    
    // close the view
	UIViewController *vc = UnityGetGLViewController();
    if(vc) [vc dismissViewControllerAnimated:YES completion:nil];
}

- (void)mailComposeController:(MFMailComposeViewController*)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError*)error
{
	[self dismissViewController];
}

- (void)Send:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML attachment:(NSData*)data mimeType:(NSString*)mimeType filename:(NSString*)filename
{
	// early out if email isnt setup
	if(![MFMailComposeViewController canSendMail]) return;
	
	MFMailComposeViewController *mailer = [[MFMailComposeViewController alloc] init];
	mailer.mailComposeDelegate = self;
	
	[mailer setSubject:subject];
	[mailer setMessageBody:body isHTML:isHTML];
	
	// Add the to address if we have one and it has an '@'
	if(toAddress && toAddress.length && [toAddress rangeOfString:@"@"].location != NSNotFound) [mailer setToRecipients:[NSArray arrayWithObject:toAddress]];
	
	// Add the attachment if we have one
	if(data && filename && mimeType) [mailer addAttachmentData:data mimeType:mimeType fileName:filename];
	
	[self showViewController:mailer];
}

- (void)Send:(NSString*)toAddress subject:(NSString*)subject body:(NSString*)body isHTML:(BOOL)isHTML
{
	[self Send:toAddress subject:subject body:body isHTML:isHTML attachment:nil mimeType:nil filename:nil];
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static EmailNative* native = nil;

extern "C"
{
    void InitEmail()
    {
        if (native == nil) native = [[EmailNative alloc] init];
    }
    
    void DisposeEmail()
    {
        if (native != nil)
        {
            #if !UNITY_5_0_0
            [native release];
            #endif
            native = nil;
        }
    }
    
    void SendEmail(const char* to, const char* subject, const char* body)
    {
        [native Send:GetStringParam(to) subject:GetStringParam(subject) body:GetStringParam(body) isHTML:false];
    }
}